#define BATCH_READ_WRITE    //批次讀寫address
using ActProgTypeLib;
using ActSupportMsgLib;
using ClassLibrary.DataHeader;
using ClassLibrary.SharedComponent.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace ClassLibrary.PLC.Base
{
    
    /// <summary>錯誤代碼</summary>
    public enum emErrorCode
    {
        ERR_DLL_NOT_LOAD = 9999,
        ERR_PARAM_ERROR = 9998
    }
    /// <summary>CPU系列</summary>
    public enum emCPU_SERIES
    {
        Q_SERIES,           //Q系列
        FX3U_SERIES,        //FX3U系列
        FX5U_SERIES,        //FX5U系列
        R_SERIES,           //R系列		
        MAX_SERIES,
        DEFAULT
    };
    public struct GPIO_ITEM
    {
        public uint Address;
        public byte DeviceCode;
        public byte Value;
    };
    //=================================================================
    //Interface
    //=================================================================
    public class MELSECIOController : IPLCProcess
    {
        private ShareMemory<byte> m_ShareMemory;
        private AppLogProcess m_LogProcess;
        private IActProgType m_pIProgType;
        private IActSupportMsg m_pISupportMsg;
        private emCPU_SERIES m_eCPU;
        private string m_strIp;
        private bool m_bInit;
        private object m_Lock;
        private GPIO_ITEM[] m_xGPIO;
        //---------------------------------------------------------------
        public MELSECIOController()
        {
            Init();
        }
        ~MELSECIOController()
        {
            int lRtn = 0;

            lock(m_Lock)
            {
                if (m_pIProgType!=null)
                {
                    lRtn = m_pIProgType.Close();
                    if (lRtn==0)
                    {
                        Console.WriteLine("關閉成功");
                    }
                }
                LIB_FREE();
            }
        }
        public string GetCPUType()
        {
            switch (GetCPU())
            {
                case emCPU_SERIES.FX3U_SERIES:
                    return "FX3U Series";
                case emCPU_SERIES.FX5U_SERIES:
                    return "FX5U Series";
                case emCPU_SERIES.R_SERIES:
                    return "R Series";
                case emCPU_SERIES.Q_SERIES:
                    return "Q Series";
                default:
                    return "Q Series(OR Wrong)";
            }
        }
        //virtual(可override)-------------------------------------------------------
        public virtual void Init()
        {
            lock (m_Lock)
            {
                m_bInit = false;
                m_pIProgType = null;
                m_pISupportMsg = null;
            }
        }
        public virtual emCPU_SERIES GetCPU()
        {
            return emCPU_SERIES.Q_SERIES;
        }
        public virtual void SetMXParam(IActProgType pParam, BATCH_SHARE_SYSTCCL_INITPARAM xData)
        {
            return;
        }
        public virtual int OpenDevice(BATCH_SHARE_SYSTCCL_INITPARAM xData)
        {
            LIB_LOAD();

            bool bLog = m_strIp.Length == 0; //only log information on first connect, igonre logging on reconnect event
            m_strIp = xData.PLCIP;
# if _DEBUG
            m_strIp = "192.168.2.99";
#endif
            //log Param
            string strMsg;

            Action<string, long> LogData = (strInfo, lData) =>
            {
                if (bLog)
                {
                    strMsg = $"{strInfo}: {lData}";
                    m_LogProcess.InsertDebugLog(strMsg, AOI_LOG_Result.LOG_SYSTEM);//這邊會是
                    ON_PLC_NOTIFY(strMsg);
                }
            };
            LogData("ConnectedStationNo", xData.ConnectedStationNo);
            LogData("T_NetworkNo", xData.TargetNetworkNo);
            LogData("T_StationNo", xData.TargetStationNo);
            LogData("PCNetworkNo", xData.PCNetworkNo);
            LogData("PCStationNo", xData.PCStationNo);

            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
            if (m_pIProgType != null)
            {
                m_pIProgType.ActHostAddress = m_strIp;//鍵入IP
                m_pIProgType.ActProtocolType = (int)PLC_CPU_CODE.PROTOCOL_TCPIP;
                SetMXParam(m_pIProgType, xData);//鍵入檔案

                if (bLog)
                {
                    long lTimeOut = 0;
                    lTimeOut = m_pIProgType.ActTimeOut;
                    LogData("TimeOutSetting", lTimeOut);
                    ListAllIP();//for debug, check the machine has correct ip address 
                }
                lRtn = m_pIProgType.Open();
                if (lRtn == 0)
                {
                    m_bInit = true;
                    ON_PLC_NOTIFY("Open PLC ok");
                }
                else if (lRtn == 25199627)
                {
                    strMsg = $"connect PLC fail: {lRtn}";
                    ON_PLC_NOTIFY(strMsg);
                }
                else
                {
                    strMsg = $"open PLC fail: {lRtn}";
                    ON_PLC_NOTIFY(strMsg);
                }
            }
            return lRtn;
        }
        //---------------------------------------------------------------
        public string GetDevicestring(GPIO_ITEM xItem)
        {
            string strRtn;
            strRtn = $"{xItem.DeviceCode}{xItem.Address}";
            return strRtn;
        }
        public string GetErrorMessage(int lErrCode)
        {
            string strRtn = "";
            switch (lErrCode)
            {
                case 0:
                    strRtn = "Success";
                    break;
                case (int)emErrorCode.ERR_DLL_NOT_LOAD:
                    strRtn = "DLL not Load";
                    break;
                case (int)emErrorCode.ERR_PARAM_ERROR:
                    strRtn = "Parameter Error";
                    break;
                default:
                    bool bRtnDefault = true;
                    if (m_pISupportMsg != null)
                    {
                        string bs;
                        int lRtn = 0;
                        lRtn = m_pISupportMsg.GetErrorMessage(lErrCode, out bs);
                        if (lRtn == 0)
                        {
                            bRtnDefault = false;
                            strRtn = bs;
                        }
                    }
                    if (bRtnDefault)
                    {
                        strRtn = $"{lErrCode}";
                    }
                    break;
            }
            return strRtn;
        }
        public string GetPLCIP()
        {
            return m_strIp;
        }
        public void LIB_FREE()
        {
            if (m_LogProcess != null)
            {
                m_LogProcess.StopLogServer();
                m_LogProcess = null;
            }
            if (m_pIProgType != null)
            {
                m_pIProgType = null;
                Trace.WriteLine("Free DLL \n");
            }
        }
        public void LIB_LOAD()
        {
            //CoInitialize(NULL);
            //開啟線程Log寫入
            if (m_LogProcess == null)
            {
                m_LogProcess = new AppLogProcess();
                m_LogProcess.StartLogServer();
            }
            //開啟
            if (m_pIProgType == null)
            {
                m_pIProgType = new ActProgTypeClass();
                if (m_pIProgType != null)
                {
                    ON_PLC_NOTIFY("Load ActProgType.dll Fail");
                }
                else
                {
                    ON_PLC_NOTIFY("Load ActProgType.dll ok");
                }
            }
            if (m_pISupportMsg == null)
            {
                m_pISupportMsg = new ActSupportMsgClass();
                if (m_pISupportMsg != null)
                {
                    ON_PLC_NOTIFY("Load ActSupportMsg.dll Fail");
                }
                else
                {
                    ON_PLC_NOTIFY("Load ActSupportMsg.dll ok");
                }
            }
        }
        public void ListAllIP()
        {
            //IP_ADAPTER_INFO* pAdptInfo = null;
            //IP_ADAPTER_INFO* pNextAd = null;
            //ulong ulLen = 0;
            //sockaddr_in addr = { NULL };


            //GetAdaptersInfo(pAdptInfo, &ulLen);

            //if (!ulLen)
            //{
            //    return;
            //}
            //pAdptInfo = (IP_ADAPTER_INFO*)::new BYTE[ulLen];


            //GetAdaptersInfo(pAdptInfo, &ulLen);

            //pNextAd = pAdptInfo;

            //while (pNextAd)
            //{
            //    string strIp(pNextAd->IpAddressList.IpAddress.String);
            //    ON_PLC_NOTIFY(L"MachineIP: " + strIp);
            //    theApp.InsertDebugLog(L"MachineIP: " + strIp, LOG_SYSTEM);
            //    pNextAd = pNextAd->Next;
            //}
            //delete(BYTE *)pAdptInfo;
        }
        //-------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 讀取軟元件[ushort]
        /// </summary>
        /// <param name="strDevType">軟元件格式[D]</param>
        /// <param name="nStartDeviceNumber">軟元件起始位置[100]</param>
        /// <param name="nSize">軟元件資料長度</param>
        /// <param name="pValue">輸出資料</param>
        /// <returns></returns>
        public int ReadAddress(string strDevType, int nStartDeviceNumber, int nSize, ref ushort[] pValue)
        {
            int nReadSize = nSize;
            if (nReadSize <= 0 || pValue.Length > 0)
            {
                return 0;
            }
            short[] pRead = new short[nReadSize];//將short大小初始化
            int lRtn = ReadAddress(strDevType, nStartDeviceNumber, nReadSize, ref pRead);

            pValue = pRead.Select(s => (ushort)s).ToArray();

            return lRtn;
        }
        /// <summary>
        /// 讀取一float
        /// </summary>
        /// <param name="strDevType"></param>
        /// <param name="nStartDeviceNumber"></param>
        /// <param name="nSize"></param>
        /// <param name="pValue"></param>
        /// <returns></returns>
        public int ReadAddress(string strDevType, int nStartDeviceNumber, int nSize, ref float[] pValue)
        {
            int nReadSize = nSize * 2;
            if (nReadSize <= 0 || pValue.Length > 0)
            {
                return 0;
            }
            short[] pRead = new short[nReadSize];//將short大小初始化
            int lRtn = ReadAddress(strDevType, nStartDeviceNumber, nReadSize, ref pRead);

            pValue = pRead.Select(s => (float)s).ToArray();

            return lRtn;
        }
        public int ReadAddress(string strDevType, int nStartDeviceNumber, int nLength, ref char[] pValue)
        {
            int nReadSize = nLength / 2;
            if (nReadSize <= 0 || pValue.Length > 0)
            {
                return 0;
            }
            short[] pRead = new short[nReadSize];//將short大小初始化
            int lRtn = ReadAddress(strDevType, nStartDeviceNumber, nReadSize, ref pRead);

            pValue = pRead.Select(s =>(char)s).ToArray();

            return lRtn;
        }
        public int ReadAddress(string strDevType, int nStartDeviceNumber, int nSizeInWord, ref short[] ppValue)
        {
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
//# if SUPPORT_AOI
            if (nSizeInWord > 0)
            {
                if (m_pIProgType!=null)//連線
                {
                    //Lock ON
                    lock(m_Lock)
                    {
                        string strDevice = $"{strDevType}{nStartDeviceNumber}";//D1000
                        lRtn = m_pIProgType.ReadDeviceBlock2(strDevice, nSizeInWord, out ppValue[0]);//MXCOMPONENT讀取 ->ppValue
                    }
                }
            }
//#endif
            return lRtn;
        }
        public int ReadOneAddress(string strDevType, int nStartDeviceNumber, ref short[] pValue)
        {
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
            if (m_pIProgType!=null)
            {
                lock (m_Lock)
                {
                    string strDevice = $"{strDevType}{nStartDeviceNumber}";
                    lRtn = m_pIProgType.GetDevice2(strDevice, out pValue[0]);
                }
            }
            return lRtn;
        }
        public int ReadRandom(string strList, int nSize, ref short[] pData)
        {
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
            if (m_pIProgType!=null)
            {
                lock (m_Lock)
                {
                    lRtn = m_pIProgType.ReadDeviceRandom2(strList, nSize, out pData[0]);
                }
            }
            return lRtn;
        }
        //---------
        public int WriteAddress(string strDevType, int nDeviceNumber, int nSizeInByte, ref ushort[] pWrite)
        {
            int nWriteSize = nSizeInByte / 2;
            int nWordCount = nSizeInByte / sizeof(ushort);
            if (nWriteSize <= 0)
                return 0;

            short[] short_Write = new short[pWrite.Length];
            for (int i = 0; i < nSizeInByte; i++)
            {
                if(!short.TryParse(pWrite[i].ToString(),out short_Write[i]))
                {
                    return (int)emErrorCode.ERR_PARAM_ERROR;
                }
            }

            int lRtn = WriteAddress(strDevType, nDeviceNumber, nWriteSize, ref short_Write);

            return lRtn;
        }
        public int WriteAddress(string strDevType, int nDeviceNumber, int nSizeInByte, ref float[] pWrite)
        {
            int nWriteSize = nSizeInByte / 2;
            int nWordCount = nSizeInByte / sizeof(float);
            if (nWriteSize <= 0)
                return 0;

            short[] short_Write = new short[pWrite.Length];
            for (int i = 0; i < nSizeInByte; i++)
            {
                if (!short.TryParse(pWrite[i].ToString(), out short_Write[i]))
                {
                    return (int)emErrorCode.ERR_PARAM_ERROR;
                }
            }

            int lRtn = WriteAddress(strDevType, nDeviceNumber, nWriteSize, ref short_Write);

            return lRtn;
        }
        public int WriteAddress(string strDevType, int nDeviceNumber, int nSizeInByte, ref char[] pWrite)
        {
            int nWriteSize = nSizeInByte / 2;
            if (nWriteSize <= 0)
                return 0;

            short[] short_Write = new short[pWrite.Length];
            for (int i = 0; i < nSizeInByte; i++)
            {
                if (!short.TryParse(pWrite[i].ToString(), out short_Write[i]))
                {
                    return (int)emErrorCode.ERR_PARAM_ERROR;
                }
            }

            int lRtn = WriteAddress(strDevType, nDeviceNumber, nWriteSize, ref short_Write);

            return lRtn;
        }
        public int WriteAddress(string strDevType, int nStartDeviceNumber, int nSizeInWord, ref short[] pValue)
        {
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
//# if SUPPORT_AOI
            if (nSizeInWord > 0)
            {
                if (m_pIProgType != null)
                {
                    lock (m_Lock)
                    {
                        string strDevice = $"{strDevType}{nStartDeviceNumber}";
                        lRtn = m_pIProgType.WriteDeviceBlock2(strDevice, nSizeInWord, ref pValue[0]);
                    }
                }
            }
//#endif
            return lRtn;
        }
        public int WriteOneAddress(string strDevice, ref short nValue)
        {
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
//# if SUPPORT_AOI
            if (m_pIProgType!=null)
            {
                lock (m_Lock)
                {
                    lRtn =  m_pIProgType.SetDevice2(strDevice, nValue);
                }
            }
//#endif
            return lRtn;
        }
        public int WriteRandom(ref string strList, int nSize, ref short[] pData)
        {
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
            if (m_pIProgType!=null)
            {
                lock (m_Lock)
                {
                    lRtn =  m_pIProgType.WriteDeviceRandom2(strList, nSize,ref pData[0]);
                }
            }
            return lRtn;
        }
    }
}
