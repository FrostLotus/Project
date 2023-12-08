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
    public enum ErrorCode
    {
        ERR_DLL_NOT_LOAD = 9999,
        ERR_PARAM_ERROR = 9998
    }
    /// <summary>CPU系列</summary>
    public enum CPU_SERIES
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
    public class IPLCProcess
    {
        public IPLCProcess m_pIn;// { get; set; }
        public IPLCProcess m_pOut;// { get; set; }
        public IPLCProcess() { m_pIn = null; m_pOut = null; }
        void AttachIn(IPLCProcess pLink) { m_pIn = pLink; }
        void AttachOut(IPLCProcess pLink) { m_pOut = pLink; }
        //in
        public void ON_GPIO_NOTIFY(IntPtr wparam, IntPtr lparam)
        {
            if (m_pIn != null)
                m_pIn.ON_GPIO_NOTIFY(wparam, lparam);
        }
        public long ON_OPEN_PLC(IntPtr lparam)
        {
            if (m_pIn != null)
                return m_pIn.ON_OPEN_PLC(lparam);
            else
                return 0xFFFFFFFF;
        }
        //out
        public void ON_PLC_NOTIFY(string strMsg)
        {
            if (m_pOut != null)
                m_pOut.ON_PLC_NOTIFY(strMsg);
        }
        public void ON_SET_PLCPARAM(BATCH_SHARE_SYSTCCL_INITPARAM xParam)
        {
            if (m_pOut != null)
                m_pOut.ON_SET_PLCPARAM(xParam);
        }
        public void ON_PLCDATA_CHANGE(int nFieldId, byte[] pData, int nSizeInByte)
        {
            if (m_pOut != null)
                m_pOut.ON_PLCDATA_CHANGE(nFieldId, pData, nSizeInByte);
        }
        public void ON_BATCH_PLCDATA_CHANGE(int nFieldFirst, int nFieldLast)
        {
            if (m_pOut != null)
                m_pOut.ON_BATCH_PLCDATA_CHANGE(nFieldFirst, nFieldLast);
        }
    };
    public interface IMELSECController
    {
        IActProgType IProgType { get; set; }
        IActSupportMsg ISupportMsg { get; set; }
        string strIp { get; set; }
        bool bInit { get; set; } //will be TRUE if connected with PLC

        string GetDevicestring(ref GPIO_ITEM xItem);

        object Lock { get; }

        string GetCPUType();

#if BATCH_READ_WRITE
        int ReadAddress(string strDevType, int nStartDeviceNumber, int nSize, ref ushort[] pValue);
        int ReadAddress(string strDevType, int nStartDeviceNumber, int nSize, ref float[] pValue);
        int ReadAddress(string strDevType, int nStartDeviceNumber, int nLength, ref char[] pValue);

        int ReadRandom(string strList, int nSize, ref short[] pData);
        int ReadOneAddress(string strDevType, int nStartDeviceNumber, ref short[] pValue);

        int WriteAddress(string strDevType, int nDeviceNumber, int nSizeInByte, ushort[] pWrite);
        int WriteAddress(string strDevType, int nDeviceNumber, int nSizeInByte, float[] pWrite);
        int WriteAddress(string strDevType, int nDeviceNumber, int nSizeInByte, char[] pWrite);
        int WriteRandom(ref string strList, int nSize, short[] pData);
        int WriteOneAddress(string strDevice, short nValue);
        string GetErrorMessage(int lErrCode);
#endif
        string GetPLCIP();// { return m_strIp; }
        CPU_SERIES GetCPU();// { return CPU_SERIES::Q_SERIES; }

        int OpenDevice(BATCH_SHARE_SYSTCCL_INITPARAM xData);
        void SetMXParam(IActProgType pParam, BATCH_SHARE_SYSTCCL_INITPARAM xData);// = 0;

        //void Finalize();
        void Init();
        void LIB_LOAD();
        void LIB_FREE();
        void ListAllIP();
#if BATCH_READ_WRITE
        int ReadAddress(string strDevType, int nStartDeviceNumber, int nSizeInWord, ref short[] ppValue); // must delete after use
        int WriteAddress(string strDevType, int nStartDeviceNumber, int nSizeInWord, short[] pValue);
#endif
    }
    public class MELSECController : IPLCProcess, IMELSECController
    {

        private IActProgType m_pIProgType;
        private IActSupportMsg m_pISupportMsg;
        private string m_strIp;
        private bool m_bInit;
        private object m_Lock;
        public IActProgType IProgType { get { return m_pIProgType; } set { m_pIProgType = value; } }
        public IActSupportMsg ISupportMsg { get { return m_pISupportMsg; } set { m_pISupportMsg = value; } }
        public string strIp { get { return m_strIp; } set { m_strIp = value; } }
        public bool bInit { get { return m_bInit; } set { m_bInit = value; } }
        public object Lock { get { return m_Lock; } }
        //---------------------------------------------------------------
        public CPU_SERIES GetCPU()
        {
            return CPU_SERIES.Q_SERIES;
        }
        public string GetCPUType()
        {
            switch (GetCPU())
            {
                case CPU_SERIES.FX3U_SERIES:
                    return "FX3U Series";
                case CPU_SERIES.FX5U_SERIES:
                    return "FX5U Series";
                case CPU_SERIES.R_SERIES:
                    return "R Series";
                case CPU_SERIES.Q_SERIES:
                    return "Q Series";
                default:
                    return "Q Series(OR Wrong)";
            }
        }
        public string GetDevicestring(ref GPIO_ITEM xItem)
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
                case (int)ErrorCode.ERR_DLL_NOT_LOAD:
                    strRtn = "DLL not Load";
                    break;
                case (int)ErrorCode.ERR_PARAM_ERROR:
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
        public void Init()
        {
            lock (m_Lock)
            {
                m_bInit = false;
                m_pIProgType = null;
                m_pISupportMsg = null;
            }
        }
        public void LIB_FREE()
        {
            if (m_pIProgType != null)
            {
                //m_pIProgType.Release();
                m_pIProgType = null;
                Trace.WriteLine("Free DLL \n");
            }
            //Marshal.CoUninitialize();
        }
        public void LIB_LOAD()
        {
            //CoInitialize(NULL);
            if (m_pIProgType == null)
            {
                //HRESULT hr = CoCreateInstance(CLSID_ActProgType,
                //NULL,
                //CLSCTX_INPROC_SERVER,
                //IID_IActProgType,
                //(LPVOID*)&m_pIProgType);
                //if (!SUCCEEDED(hr))
                //{
                //    ON_PLC_NOTIFY("Load ActProgType.dll Fail");
                //}
                //else
                //{
                //    ON_PLC_NOTIFY("Load ActProgType.dll ok");
                //}
            }
            if (m_pISupportMsg == null)
            {
                //HRESULT hr = CoCreateInstance(CLSID_ActSupportMsg,
                //    NULL,
                //    CLSCTX_INPROC_SERVER,
                //    IID_IActSupportMsg,
                //    (LPVOID*)&m_pISupportMsg);
                //if (!SUCCEEDED(hr))
                //{
                //    ON_PLC_NOTIFY(L"Load ActSupportMsg.dll Fail");
                //}
                //else
                //{
                //    ON_PLC_NOTIFY(L"Load ActSupportMsg.dll ok");
                //}
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
            //    CString strIp(pNextAd->IpAddressList.IpAddress.String);
            //    ON_PLC_NOTIFY(L"MachineIP: " + strIp);
            //    theApp.InsertDebugLog(L"MachineIP: " + strIp, LOG_SYSTEM);
            //    pNextAd = pNextAd->Next;
            //}
            //delete(BYTE *)pAdptInfo;
        }
        public int OpenDevice(BATCH_SHARE_SYSTCCL_INITPARAM xData)
        {
            LIB_LOAD();

            bool bLog = m_strIp.Length == 0; //only log information on first connect, igonre logging on reconnect event
            m_strIp = xData.PLCIP;
# if _DEBUG
            m_strIp = L"192.168.2.99";
#endif
            //log Param
            string strMsg;

            Action<string, long> LogData = (strInfo, lData) =>
            {
                if (bLog)
                {
                    strMsg = $"{strInfo}: {lData}";
                    InsertDebugLog(strMsg, AOI_LOG_TYPE.LOG_SYSTEM);
                    ON_PLC_NOTIFY(strMsg);
                }
            };
            LogData("ConnectedStationNo", xData.ConnectedStationNo);
            LogData("T_NetworkNo", xData.TargetNetworkNo);
            LogData("T_StationNo", xData.TargetStationNo);
            LogData("PCNetworkNo", xData.PCNetworkNo);
            LogData("PCStationNo", xData.PCStationNo);

            int lRtn = (int)ErrorCode.ERR_DLL_NOT_LOAD;
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
                else
                {
                    strMsg = $"open PLC fail: {lRtn}";
                    ON_PLC_NOTIFY(strMsg);
                }
            }
            return lRtn;
        }
        /// <summary>
        /// 讀取軟元件[ushort]
        /// </summary>
        /// <param name="strDevType">軟元件格式[D]</param>
        /// <param name="nStartDeviceNumber">軟元件起始位置[100]</param>
        /// <param name="nSize">軟元件資料長度</param>
        /// <param name="pValue"></param>
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
        public int ReadAddress(string strDevType, int nStartDeviceNumber, int nSize, ref float[] pValue)
        {
            int nReadSize = nSize*2;
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
            int lRtn = (int)ErrorCode.ERR_DLL_NOT_LOAD;
//# if SUPPORT_AOI
            if (nSizeInWord>0)
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
            int lRtn = (int)ErrorCode.ERR_DLL_NOT_LOAD;
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
            int lRtn = (int)ErrorCode.ERR_DLL_NOT_LOAD;
            if (m_pIProgType!=null)
            {
                lock (m_Lock)
                {
                    lRtn = m_pIProgType.ReadDeviceRandom2(strList, nSize, out pData[0]);
                }
            }
            return lRtn;
        }
        public void SetMXParam(IActProgType pParam, BATCH_SHARE_SYSTCCL_INITPARAM xData)
        {
            throw new NotImplementedException();
        }
        public int WriteAddress(string strDevType, int nDeviceNumber, int nSizeInByte, ushort[] pWrite)
        {
            throw new NotImplementedException();
        }
        public int WriteAddress(string strDevType, int nDeviceNumber, int nSizeInByte, float[] pWrite)
        {
            throw new NotImplementedException();
        }
        public int WriteAddress(string strDevType, int nDeviceNumber, int nSizeInByte, char[] pWrite)
        {
            throw new NotImplementedException();
        }
        public int WriteAddress(string strDevType, int nStartDeviceNumber, int nSizeInWord, short[] pValue)
        {
            throw new NotImplementedException();
        }
        public int WriteOneAddress(string strDevice, short nValue)
        {
            throw new NotImplementedException();
        }
        public int WriteRandom(ref string strList, int nSize, short[] pData)
        {
            throw new NotImplementedException();
        }
    }
}
