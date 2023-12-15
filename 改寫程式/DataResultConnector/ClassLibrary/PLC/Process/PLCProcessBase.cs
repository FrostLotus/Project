using ClassLibrary.PLC.Base;
using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.DataHeader;
using System.Runtime.InteropServices;
using ClassLibrary.SharedComponent.Log;

namespace ClassLibrary.PLC.Process
{
    #region [struct]PLC參數利用結構
    ///<summary>PLC資料結構</summary>
    public struct PLCDATA
    {
        public byte[] pData;
        public DateTime xTime;
    };
    ///<summary>項目數值格式</summary>
    public enum PLC_VALUE_TYPE
    {
        PLC_TYPE_STRING,
        PLC_TYPE_WORD,
        PLC_TYPE_FLOAT,
        PLC_TYPE_DWORD,
        PLC_TYPE_BIT,
    };
    ///<summary>項目處理模式</summary>
    public enum PLC_ACTION_TYPE
    {
        ACTION_SKIP,
        ACTION_NOTIFY,
        ACTION_BATCH,
        ACTION_RESULT,
    };
    public class PLC_DATA_ITEM
    {
        public string FieldName;
        public int FieldType;
        public PLC_VALUE_TYPE ValType;
        public PLC_ACTION_TYPE Action;
        public byte DataLength; //bytes
        public string DevType;
        public int Address;
        public uint StartBit;
        public uint EndBit;
        public bool Signed;
        public PLC_DATA_ITEM()
        {

        }
        public PLC_DATA_ITEM(string strFieldName, int nFieldType,
                             PLC_VALUE_TYPE eValueType, PLC_ACTION_TYPE eActionType,
                             byte cLen, string strDeviceType,
                             int uAddress,
                             uint uStartBit = 0xFFFFFFFF, uint uEndBit = 0xFFFFFFFF // 使用此值來表示 C++ 中的 -1
                            )
        {
            this.FieldName = strFieldName;
            this.FieldType = nFieldType;
            this.ValType = eValueType;
            this.Action = eActionType;
            this.DataLength = cLen;
            this.DevType = strDeviceType;
            this.Address = uAddress;
            this.StartBit = uStartBit;
            this.EndBit = uEndBit;
            this.Signed = false;
        }

    }
    #endregion

    //PLCProcessBase=====================================================================================
    public interface IPLCProcessBase : IMELSECIOController
    {
        void NotifyAOI(IntPtr wparam, IntPtr lparam);
        int GetFieldSize();
        PLC_DATA_ITEM GetPLCAddressInfo(int nFieldId, bool bSkip);
        void DO_CUSTOM_TEST();//change it in inherit class if needed
        bool HAS_CUSTOM_TEST();// { return FALSE; }

        byte[] GET_PLC_FIELD_BYTE_VALUE(int nFieldId);
        string GET_PLC_FIELD_VALUE(int nFieldId);
        string GET_PLC_FIELD_TIME(int nFieldId);
        string GET_PLC_FIELD_ADDRESS(int nFieldId);
        string GET_PLC_FIELD_NAME(int nFieldId);
        PLC_ACTION_TYPE GET_PLC_FIELD_ACTION(int nFieldId);

        int GET_PLC_FIELD_DATA(int nFieldId);
        int GET_PLC_FIELD_DATA(List<int> vField);
        int SET_PLC_FIELD_DATA(int nFieldId, int nSizeInByte, byte[] pData);
        int SET_PLC_FIELD_DATA(List<int> vField, byte[] pData);
        int SET_PLC_FIELD_DATA_BIT(int nFieldStart, int nFieldEnd, int nSizeInByte, byte[] pData);
        int SET_PLC_FIELD_DATA_BIT(int nField, int nBitPosition, bool bValue);

        void SET_FLUSH_ANYWAY(bool bFlushAnyway); //{ m_bFlushAnyway = bFlushAnyway; };
        bool GET_FLUSH_ANYWAY(); //{ return m_bFlushAnyway; };
                                 //-------------------
        void INIT_PLCDATA();
        void DESTROY_PLC_DATA();

        bool ShareMemory_ReadData(ref byte[] pData, int nSize, int nOffset = 0);
        bool ShareMemory_WriteData(byte[] pData, int nSize, int nOffset = 0);

        //IPLCProcess
        int ON_OPEN_PLC(IntPtr lparam);

        void SET_INIT_PARAM(IntPtr lparam, byte[] pData);// { };
                                                         //-------------------------------
        void Init();
        //void Finalize();
        int GET_PLC_FIELD_DATA(int nFieldId, ref byte[] pData);
        void GET_PLC_RANDOM_DATA(List<int> vField, ref string strField, ref int nSizeInWord);
    }
    class PLCProcessBase : MELSECIOController, IPLCProcessBase
    {
        //私有項目
        private PLCDATA[] m_PLCData;
        private bool m_FlushAnyway;
        private int m_LastRtn = 0;
        private IntPtr m_Wnd_AOI;
        private byte[] m_PLCInitData;

        private ShareMemory<byte> m_AOIShareMemory;
        private ShareMemory<byte> m_PLCShareMemory;
        //--------------------------------------------------------------------------------------------------
        public PLCProcessBase()
        {
            Init();
        }
        ~PLCProcessBase()
        {
            if (m_PLCInitData != null)
            {
                m_PLCInitData = null;
            }
            if (m_AOIShareMemory.Get_Status())
            {
                m_AOIShareMemory = null;
            }
            if (m_PLCShareMemory.Get_Status())
            {
                m_PLCShareMemory = null;
            }
        }
        //override---------
        public override void Init()
        {
            m_PLCInitData = null;
            m_PLCData = null;
            m_LastRtn = 0;

            m_Wnd_AOI = FindWindow(null, DataHeadlerBase.AOI_MASTER_NAME);
            m_AOIShareMemory = new ShareMemory<byte>(DataHeadlerBase.BATCH_AOI_MEM_ID);
            m_PLCShareMemory = new ShareMemory<byte>(DataHeadlerBase.BATCH_COMMUNICATOR_MEM_ID);
        }
        public override int ON_OPEN_PLC(IntPtr lparam)
        {
            int lRtn = (int)ErrorCode.ERR_DLL_NOT_LOAD;
            if (m_PLCInitData == null)
            {
                //first open, read data from shared memory
                int nDataSize = 0;
#if !OFF_LINE
                string tmp_IP = DataHeadlerBase.BATCH_PLCIP;
#endif
                switch ((int)lparam)
                {
                    case (int)WM_APP_CMD.WM_SYST_PARAMINIT_CMD: //CCL參數建立
                        {
                            BATCH_SHARE_SYSTCCL_INITPARAM pData = new BATCH_SHARE_SYSTCCL_INITPARAM();//CCL站體參數
#if !OFF_LINE
                            pData.PLCIP = tmp_IP;
                            pData.TargetNetworkNo = 0;
                            pData.TargetStationNo = 0xFF;
#endif
                            m_PLCInitData = StructToBytes(pData);//鍵入
                            nDataSize = Marshal.SizeOf(typeof(BATCH_SHARE_SYSTCCL_INITPARAM));//尺寸
                        }
                        break;
                    case (int)WM_APP_CMD.WM_SYST_PP_PARAMINIT_CMD: //PP參數建立
                        {
                            BATCH_SHARE_SYSTPP_INITPARAM pData = new BATCH_SHARE_SYSTPP_INITPARAM();//PP站體參數
#if !OFF_LINE            
                            pData.m_BATCH_SHARE_SYSTCCL_INITPARAM.PLCIP = tmp_IP;
                            pData.m_BATCH_SHARE_SYSTCCL_INITPARAM.TargetNetworkNo = 0;
                            pData.m_BATCH_SHARE_SYSTCCL_INITPARAM.TargetStationNo = 0xFF;
                            pData.FX5U = true;
#endif
                            m_PLCInitData = StructToBytes(pData);//鍵入

                            nDataSize = Marshal.SizeOf(typeof(BATCH_SHARE_SYSTPP_INITPARAM));//尺寸
                        }
                        break;
                }
                //-------------------------------------
                if (m_PLCInitData != null && nDataSize != 0)//參數成立
                {
                    InsertDebugLog("ON_OPEN_PLC", AOI_LOG_TYPE.LOG_DEBUG);//log
# if! OFF_LINE
                    //string str = L"192.168.2.29";
                    //wcscpy_s(xData.cPLCIP, str.GetBuffer());
                    //xData.lTargetNetworkNo = 0;
                    //xData.lTargetStationNo = 0xFF;*/
#else
                    if (USM_ReadData(m_pPLCInitData, nDataSize))
#endif
                    {
                        switch ((int)lparam)
                        {
                            case ((int)WM_APP_CMD.WM_SYST_PARAMINIT_CMD):
                                {
                                    SET_INIT_PARAM(lparam, m_PLCInitData);//空?
                                    ON_SET_PLCPARAM(BytesToCCL(m_PLCInitData));//空?
                                }
                                break;
                            case ((int)WM_APP_CMD.WM_SYST_PP_PARAMINIT_CMD):
                                {
                                    SET_INIT_PARAM(lparam, m_PLCInitData);//空?
                                    ON_SET_PLCPARAM(BytesToPP(m_PLCInitData).m_BATCH_SHARE_SYSTCCL_INITPARAM);//空?
                                }
                                break;
                        }
                    }
                }
            }
            if (m_PLCInitData != null)
            {
                string strLog;
                switch ((int)lparam)
                {
                    case ((int)WM_APP_CMD.WM_SYST_PARAMINIT_CMD):
                        {
                            lRtn = OpenDevice(BytesToCCL(m_PLCInitData));
                        }
                        break;
                    case ((int)WM_APP_CMD.WM_SYST_PP_PARAMINIT_CMD):
                        {
                            lRtn = OpenDevice(BytesToPP(m_PLCInitData).m_BATCH_SHARE_SYSTCCL_INITPARAM);
                        }
                        break;
                }
                if (lRtn == 0)
                {
                    strLog = "open plc success";
                }
                else
                {
                    strLog = $"open plc fail: {lRtn} : {GetErrorMessage(lRtn)}";
                }
                InsertDebugLog(strLog);
            }
            return lRtn;
        }
        ///<summary>視窗通知AOI</summary>
        public virtual void NotifyAOI(IntPtr wparam, IntPtr lparam)
        {
            if (m_Wnd_AOI != IntPtr.Zero)//視窗存在
            {
                PostMessage(m_Wnd_AOI, (int)WM_APP_CMD.WM_GPIO_MSG, wparam, lparam);//PLC to AOI
            }
        }
        //-----------------
        public virtual void SET_INIT_PARAM(IntPtr lparam, byte[] pData)
        {

        }
        public virtual void INIT_PLCDATA()
        {
            //將m_PLCData初始化
            int nMax = GetFieldSize();
            m_PLCData = new PLCDATA[nMax];
            for (int i = 0; i < GetFieldSize(); i++)
            {
                PLC_DATA_ITEM pItem = GetPLCAddressInfo(i, false);
                int nDataSize = pItem.DataLength;
                if (pItem.ValType == PLC_VALUE_TYPE.PLC_TYPE_STRING)
                {
                    nDataSize = pItem.DataLength + 1;//resize
                }
                m_PLCData[i].pData = new byte[nDataSize];//設置型別寬度
            }
        }
        public virtual void DESTROY_PLC_DATA()
        {
            if (m_PLCData != null)
            {
                int nFieldSize = GetFieldSize();
                for (int i = 0; i < nFieldSize; i++)
                {
                    if (m_PLCData[i].pData != null)
                    {
                        m_PLCData[i].pData = null;
                    }
                }
                m_PLCData = null;
            }
        }
        public virtual void DO_CUSTOM_TEST()
        {
            return;//父類不定義 根據後面表單[BATCH]實作override取得實際表單參數數量
        }
        public virtual bool HAS_CUSTOM_TEST()
        {
            return false;
        }
        public virtual int GetFieldSize()
        {
            return 0;//父類不定義 根據後面表單[BATCH]實作override取得實際表單參數數量
        }
        public virtual PLC_DATA_ITEM GetPLCAddressInfo(int nFieldId, bool bSkip)
        {
            return null;//父類不定義 根據後面表單[BATCH]實作override取得實際表單參數數量
        }
        //-------------------------------------------------------------------------
        ///<summary>取得對應ID(field)之值(BYTE)</summary>
        public virtual byte[] GET_PLC_FIELD_BYTE_VALUE(int nFieldId)
        {
            int nFieldSize = GetFieldSize();
            if (nFieldId >= 0 && nFieldId < nFieldSize)//範圍內
            {
                PLC_DATA_ITEM pCur = GetPLCAddressInfo(nFieldId, false);
                if (pCur != null) //取得
                {
                    return m_PLCData[nFieldId].pData;
                }
            }
            return null;
        }
        public virtual string GET_PLC_FIELD_VALUE(int nFieldId)
        {
            int nFieldSize = GetFieldSize();//最大值
            string strDes = "";
            if (nFieldId >= 0 && nFieldId < nFieldSize)//範圍內
            {
                PLC_DATA_ITEM pCur = GetPLCAddressInfo(nFieldId, false);
                if (pCur != null)
                {
                    switch (pCur.ValType)//type
                    {
                        case PLC_VALUE_TYPE.PLC_TYPE_STRING://string
                            strDes = $"{(byte[])m_PLCData[nFieldId].pData}";
                            break;
                        case PLC_VALUE_TYPE.PLC_TYPE_WORD://word
                            if (pCur.StartBit != 0xFFFFFFFF && pCur.EndBit != 0xFFFFFFFF)//不是預設值
                            {
                                //int nValue = (int)*(unsigned short*)m_PLCData[nFieldId].pData;
                                int nValue = BitConverter.ToUInt16(m_PLCData[nFieldId].pData, 0); // 假設 pData 是 ushort
                                int nTemp = 0;
                                for (uint i = pCur.StartBit; i <= pCur.EndBit; i++)
                                {
                                    nTemp |= (nValue & (ushort)1 << (int)i);
                                }
                                nTemp >>= (int)pCur.StartBit;
                                strDes = $"{nTemp}";
                            }
                            else
                            {
                                if (pCur.Signed)
                                    strDes = BitConverter.ToInt16(m_PLCData[nFieldId].pData, 0).ToString();
                                else
                                    strDes = BitConverter.ToUInt16(m_PLCData[nFieldId].pData, 0).ToString();
                            }
                            break;
                        case PLC_VALUE_TYPE.PLC_TYPE_FLOAT:
                            strDes = BitConverter.ToSingle(m_PLCData[nFieldId].pData, 0).ToString("F2");
                            break;
                        case PLC_VALUE_TYPE.PLC_TYPE_DWORD:
                            strDes = BitConverter.ToInt32(m_PLCData[nFieldId].pData, 0).ToString();
                            break;
                        case PLC_VALUE_TYPE.PLC_TYPE_BIT:
                            strDes = BitConverter.ToInt16(m_PLCData[nFieldId].pData, 0).ToString();
                            break;
                    }
                }
            }
            return strDes;
        }
        public virtual string GET_PLC_FIELD_TIME(int nFieldId)
        {
            int nFieldSize = GetFieldSize();
            string strDes = "";
            if (m_PLCData != null)
            {
                if (nFieldId >= 0 && nFieldId < nFieldSize && m_PLCData[nFieldId].xTime != null)
                {
                    m_PLCData[nFieldId].xTime = DateTime.Now;
                    strDes = m_PLCData[nFieldId].xTime.ToString();
                }
            }
            return strDes;
        }
        ///<summary>取得對應軟元件之名稱:(D100.2)</summary>
        public virtual string GET_PLC_FIELD_ADDRESS(int nFieldId)
        {
            int nFieldSize = GetFieldSize();
            string strDes = "fail in [GET_PLC_FIELD_ADDRESS]: out of range";
            if (nFieldId >= 0 && nFieldId < nFieldSize)
            {
                PLC_DATA_ITEM pCur = GetPLCAddressInfo(nFieldId, false);
                if (pCur != null)
                {
                    if (pCur.ValType == PLC_VALUE_TYPE.PLC_TYPE_BIT && pCur.DevType == "D")
                    {
                        //special case for D. ex:200.F
                        if (pCur.StartBit != 0xFFFFFFFF && pCur.StartBit == pCur.EndBit)
                        {
                            strDes = $"{pCur.DevType}{pCur.Address}.{pCur.StartBit}";
                        }
                        else
                        {
#if DEBUG
                            Debug.Assert(false);
#endif
                        }
                    }
                    else
                    {
                        strDes = $"{ pCur.DevType}{ pCur.Address}";
                    }
                }
            }
            return strDes;
        }
        public virtual string GET_PLC_FIELD_NAME(int nFieldId)
        {
            int nFieldSize = GetFieldSize();
            string strDes = "";
            if (nFieldId >= 0 && nFieldId < nFieldSize)
            {
                PLC_DATA_ITEM pCur = GetPLCAddressInfo(nFieldId, false);
                strDes = $"{pCur.FieldName}";
            }
            return strDes;
        }
        public virtual PLC_ACTION_TYPE GET_PLC_FIELD_ACTION(int nFieldId)
        {
            int nFieldSize = GetFieldSize();
            PLC_ACTION_TYPE eType = PLC_ACTION_TYPE.ACTION_NOTIFY;
            if (nFieldId >= 0 && nFieldId < nFieldSize)
            {
                PLC_DATA_ITEM pCur = GetPLCAddressInfo(nFieldId, false);
                if (pCur != null)
                {
                    eType = pCur.Action;//僅提取單一的所以為0
                }
            }
            return eType;
        }
        //-----------------------------------------------------------------------
        public virtual int GET_PLC_FIELD_DATA(int nFieldId)
        {
            return GET_PLC_FIELD_DATA(nFieldId, ref m_PLCData[nFieldId].pData);
        }
        public virtual int GET_PLC_FIELD_DATA(List<int> vField)
        {
            int lRtn = (int)ErrorCode.ERR_DLL_NOT_LOAD;
            string strField = "";
            int nTotal = 0;
            GET_PLC_RANDOM_DATA(vField, ref strField, ref nTotal);

            if (nTotal > 0)
            {
                short[] pData = new short[nTotal];
                //memset(pData, 0, sizeof(short) * nTotal);
                //Array.Resize(ref pData, sizeof(short));//裡面會是0嗎
                for (int i = 0; i < pData.Length; i++) pData[i] = 0;

                lRtn = ReadRandom(strField, nTotal, ref pData);

                if (lRtn == 0)
                {

                    byte[] pCur = new byte[pData.Length * sizeof(short)]; //= (byte[])pData;
                    //填入
                    Buffer.BlockCopy(pData, 0, pCur, 0, pCur.Length);

                    foreach (var i in vField)
                    {
                        PLC_DATA_ITEM pItem = GetPLCAddressInfo(i, false);
                        if (pItem != null)
                        {
                            //update time and data
                            m_PLCData[i].xTime = DateTime.Now;
                            Buffer.BlockCopy(pCur, 0, m_PLCData[i].pData, 0, pItem.DataLength);
                            pCur = pCur.Skip(pItem.DataLength).ToArray();// Move pointer to next pData section
                        }
                    }
                    //刷新PLC顯示控制項  先關閉
#if Draw_Component
                    ON_BATCH_PLCDATA_CHANGE(*vField.begin(), *vField.rbegin());
#endif
                }
            }
            return lRtn;
        }
        public virtual int GET_PLC_FIELD_DATA(int nFieldId, ref byte[] pData)
        {
            int lRtn = (int)ErrorCode.ERR_DLL_NOT_LOAD;
            PLC_DATA_ITEM pItem = GetPLCAddressInfo(nFieldId, false);//整個對應ID資料讀回來
            if (pItem != null)//成功
            {
                switch (pItem.ValType)
                {
                    case PLC_VALUE_TYPE.PLC_TYPE_FLOAT:
                        float[] tmp_floatData = new float[pItem.DataLength];
                        lRtn = ReadAddress(pItem.DevType, pItem.Address, 1, ref tmp_floatData);
                        pData = FloatToByte(tmp_floatData);
                        break;
                    case PLC_VALUE_TYPE.PLC_TYPE_STRING:
                        char[] tmp_charData = new char[pItem.DataLength];
                        lRtn = ReadAddress(pItem.DevType, pItem.Address, pItem.DataLength, ref tmp_charData);
                        pData = CharToByte(tmp_charData);
                        break;
                    case PLC_VALUE_TYPE.PLC_TYPE_WORD:
                        char[] tmp_WORDData = new char[pItem.DataLength];
                        lRtn = ReadAddress(pItem.DevType, pItem.Address, 1, ref tmp_WORDData);
                        pData = CharToByte(tmp_WORDData);
                        break;
                    case PLC_VALUE_TYPE.PLC_TYPE_DWORD:
                        char[] tmp_DWORDData = new char[pItem.DataLength];
                        lRtn = ReadAddress(pItem.DevType, pItem.Address, 2, ref tmp_DWORDData);
                        pData = CharToByte(tmp_DWORDData);
                        break;
                    case PLC_VALUE_TYPE.PLC_TYPE_BIT:
                        short[] tmp_shortData = new short[pItem.DataLength];
                        lRtn = ReadOneAddress(pItem.DevType, pItem.Address, ref tmp_shortData);
                        pData = shortToByte(tmp_shortData);
                        break;
                }
                if (lRtn == 0)
                {
                    m_PLCData[nFieldId].xTime = DateTime.Now;
                    ON_PLCDATA_CHANGE(nFieldId, pData, pItem.DataLength);
                }
            }
            return lRtn;
        }
        private byte[] CharToByte(char[] tmp_charData)
        {
            byte[] byteArray = new byte[tmp_charData.Length * 4];
            Buffer.BlockCopy(tmp_charData, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }
        private byte[] FloatToByte(float[] tmp_floatData)
        {
            byte[] byteArray = new byte[tmp_floatData.Length * 4];
            Buffer.BlockCopy(tmp_floatData, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }
        private byte[] UshortToByte(ushort[] ushortData)
        {
            byte[] byteArray = new byte[ushortData.Length * 2]; // 每個 ushort 轉換為兩個字節
            Buffer.BlockCopy(ushortData, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }
        private byte[] shortToByte(short[] shortData)
        {
            byte[] byteArray = new byte[shortData.Length * 2]; // 每個 ushort 轉換為兩個字節
            Buffer.BlockCopy(shortData, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }
        //----------------------------------------------------------
        ///<summary>[READ_RANDOM2][多數]取得對應ID之PLC資料</summary>
        public virtual void GET_PLC_RANDOM_DATA(List<int> vField, ref string strField,ref  int nSizeInWord)
        {
            string strTemp = "wrong value";
            foreach (var i in vField)//共vField.length
            {
                PLC_DATA_ITEM pItem = GetPLCAddressInfo(i, false);
                if (pItem != null)
                {
                    switch (pItem.ValType)
                    {
                        case PLC_VALUE_TYPE.PLC_TYPE_WORD:
                        case PLC_VALUE_TYPE.PLC_TYPE_BIT:
                        case PLC_VALUE_TYPE.PLC_TYPE_FLOAT:
                        case PLC_VALUE_TYPE.PLC_TYPE_DWORD:
                        case PLC_VALUE_TYPE.PLC_TYPE_STRING:
                            int nSize = pItem.DataLength / 2;
                            nSizeInWord += nSize;//大小疊加
                            for (int j = 0; j < nSize; j++)
                            {
                                if (pItem.ValType == PLC_VALUE_TYPE.PLC_TYPE_BIT && pItem.DevType == "D")
                                { //special case for D. ex:200.F
                                    if (pItem.StartBit != 0xFFFFFFFF && pItem.StartBit == pItem.EndBit)
                                    {
                                        //D100.5
                                        strTemp = $"{pItem.DevType}{pItem.Address + j}.{pItem.StartBit}";
                                    }
                                    else
                                    {
                                        Trace.Assert(false);
                                    }
                                }
                                else
                                {
                                    //D100
                                    strTemp = $"{pItem.DevType}{pItem.Address}";
                                }
                                //-------------------------------------
                                if (strField.Length == 0)
                                    strField = strTemp;
                                else
                                    strField += $"\n{strTemp}";//將資料隔行輸出
                            }
                            break;
                        default:
                            Trace.Assert(false);
                            break;
                    }
                }
            }
        }
        //-----------------------------------------------------------------------
        public virtual int SET_PLC_FIELD_DATA(int nFieldId, int nSizeInByte, byte[] pData)
        {
            int lRtn = (int)ErrorCode.ERR_DLL_NOT_LOAD;
            int nFieldSize = GetFieldSize();
            string strDes;
            if (nFieldId >= 0 && nFieldId < nFieldSize)
            {
                PLC_DATA_ITEM pItem = GetPLCAddressInfo(nFieldId, false);
                //初始化
                byte[] pOldValue = new byte[nSizeInByte];

                pOldValue = m_PLCData[nFieldId].pData;//先將原來資料複製一份

                switch (pItem.ValType)
                {
                    case PLC_VALUE_TYPE.PLC_TYPE_STRING:
                        if (TryPraseChar(pData, out char[] charArray))
                        lRtn = WriteAddress(pItem.DevType, pItem.Address, nSizeInByte, ref charArray);
                        break;
                    case PLC_VALUE_TYPE.PLC_TYPE_WORD:
                    case PLC_VALUE_TYPE.PLC_TYPE_DWORD:
                        if (TryPraseShort(pData, out short[] shortArray))
                        lRtn = WriteAddress(pItem.DevType, pItem.Address, nSizeInByte, ref shortArray);
                        break;
                    case PLC_VALUE_TYPE.PLC_TYPE_FLOAT:
                        if (TryPraseFloat(pData, out float[] floatArray))
                        lRtn = WriteAddress(pItem.DevType, pItem.Address, nSizeInByte, ref floatArray);
                        break;
                    case PLC_VALUE_TYPE.PLC_TYPE_BIT:
                        string strDevice = GET_PLC_FIELD_ADDRESS(nFieldId);
                        if(TryPraseSigleShort(pData, out short shortvalue))
                        lRtn = WriteOneAddress(strDevice, ref shortvalue);
                        break;
                }
                if (lRtn == 0)
                {
                    m_PLCData[nFieldId].xTime = DateTime.Now;
                    ON_PLCDATA_CHANGE(nFieldId, pData, pItem.DataLength);
                }
                else
                {
                    //寫入失敗, 還原資料   現在lRtn!=0應該不會寫入(所以跳過)
                    string strLog;
                    strLog = $"Address {pItem.Address} write failed, error code: {lRtn}, {GetErrorMessage(lRtn)}";
                    Trace.WriteLine($"{strLog} \n");
                    InsertDebugLog(strLog);
                }
                if (lRtn != m_LastRtn)
                {
                    m_LastRtn = lRtn;
                    if (lRtn == 0)
                        ON_PLC_NOTIFY("PLC Normal");
                    else
                        ON_PLC_NOTIFY("PLC Error");
                }
            }
            return lRtn;
        }
        private bool TryPraseSigleShort(byte[] byteArray, out short shortvalue)
        {
            if (byteArray.Length != 2)
            {
                throw new ArgumentException("Byte array length must be 2 for a short.");
            }
            shortvalue = BitConverter.ToInt16(byteArray, 0);
            return true;
        }
        private bool TryPraseFloat(byte[] byteArray, out float[] floatArray)
        {
            floatArray = new float[byteArray.Length / 4];
            if (byteArray.Length % 4 != 0)
            {
                throw new ArgumentException("Byte array length must be a multiple of 4.");
            }
            for (int i = 0; i < floatArray.Length; i++)
            {
                floatArray[i] = BitConverter.ToSingle(byteArray, i * 4);
            }
            return true;
        }
        private bool TryPraseChar(byte[] byteArray, out char[] charArray)
        {
            charArray = System.Text.Encoding.ASCII.GetChars(byteArray);
            return true;
        }
        private bool TryPraseShort(byte[] byteArray, out short[] shortArray)
        {
            shortArray = new short[byteArray.Length / 2];
            if (byteArray.Length % 2 != 0)
            {
                throw new ArgumentException("Byte array length must be a multiple of 2.");
            }
            Buffer.BlockCopy(byteArray, 0, shortArray, 0, byteArray.Length);
            return true;
        }
        private bool TryPraseUShort(byte[] byteArray, out ushort[] ushortArray)
        {
            ushortArray = new ushort[byteArray.Length / 2];
            if (byteArray.Length % 2 != 0)
            {
                throw new ArgumentException("Byte array length must be a multiple of 2.");
            }
            Buffer.BlockCopy(byteArray, 0, ushortArray, 0, byteArray.Length);
            return true;
        }
        private bool TryPraseUShort(byte[] byteArray, out ushort singleushort)
        {
            if (byteArray.Length % 2 != 0)
            {
                throw new ArgumentException("Byte array length must be a multiple of 2.");
            }
            singleushort = BitConverter.ToUInt16(byteArray, 0);
            return true;
        }

        public virtual int SET_PLC_FIELD_DATA(List<int> vField, byte[] pData)
        {
            int lRtn = (int)ErrorCode.ERR_DLL_NOT_LOAD;
            string strField="";
            int nSizeInWord = 0;
            //                   List   D100 D101..  長度
            GET_PLC_RANDOM_DATA(vField, ref strField, ref nSizeInWord);
            if (nSizeInWord!=0&& TryPraseShort(pData,out short[] shortarray ))
            {
                
                lRtn = WriteRandom(ref strField, nSizeInWord, ref shortarray);
            }
            return lRtn;
        }
        //-----------------------------------------------------------------------
        public virtual int SET_PLC_FIELD_DATA_BIT(int nFieldStart, int nFieldEnd, int nSizeInByte, byte[] pData)
        {
            int lRtn = (int)ErrorCode.ERR_DLL_NOT_LOAD;
            int nFieldSize = GetFieldSize();
            PLC_DATA_ITEM pWriteItem = GetPLCAddressInfo(nFieldStart, false);
            TryPraseUShort(pData, out ushort[] ushortArray);
            lRtn = WriteAddress(pWriteItem.DevType, pWriteItem.Address, nSizeInByte, ref ushortArray);

            if (lRtn == 0)
            {
                for (int i = nFieldStart; i <= nFieldEnd; i++)
                {
                    if (i >= 0 && i < nFieldSize)
                    {
                        PLC_DATA_ITEM pItem = GetPLCAddressInfo(i, false);
                        if (pItem.StartBit != 0xFFFFFFFF && pItem.EndBit != 0xFFFFFFFF)
                        {
                            int nValue = BitConverter.ToInt32(pData, 0);
                            int nTemp = 0;
                            for (int j = (int)pItem.StartBit; j <= (int)pItem.EndBit; j++)
                            {
                                nTemp |= (nValue & 1 << j);
                            }
                            m_PLCData[i].xTime = DateTime.Now;

                            PLC_DATA_ITEM newItem = new PLC_DATA_ITEM();
                            m_PLCData[i].pData = BitConverter.GetBytes(nTemp); // 將 nTemp 轉換為 byte[]，可能需要調整字節順序
                            ON_PLCDATA_CHANGE(i, m_PLCData[i].pData, pItem.DataLength);
                        }
                    }
                }
            }
            return lRtn;
        }
        public virtual int SET_PLC_FIELD_DATA_BIT(int nField, int nBitPosition, bool bValue)
        {
            int lRtn = (int)ErrorCode.ERR_DLL_NOT_LOAD;

            PLC_DATA_ITEM pCur = GetPLCAddressInfo(nField, false);
            if (pCur!=null)
            {
                string strDevice;
                strDevice = $"{pCur.DevType}{pCur.Address}.{nBitPosition}";
                short myShort = bValue ? (short)1 : (short)0;
                lRtn = WriteOneAddress(strDevice, ref myShort);
                if (pCur.DataLength == 2) 
                {
                    throw new ArgumentException();
                }
                //ASSERT(pCur.DataLength == 2);

                if (lRtn == 0)
                {
                    TryPraseUShort(m_PLCData[nField].pData, out ushort wValue);
                    if (bValue)
                        wValue |= (ushort)(1 << nBitPosition);
                    else
                        wValue &= (ushort)~(1 << nBitPosition);

                    m_PLCData[nField].pData = BitConverter.GetBytes(wValue);
                    ON_PLCDATA_CHANGE(nField, m_PLCData[nField].pData, pCur.DataLength);
                }
            }
            return lRtn;
        }
        //-----------------------------------------------------------------------
        public virtual bool GET_FLUSH_ANYWAY()
        {
            return m_FlushAnyway;
        }
        public virtual void SET_FLUSH_ANYWAY(bool bFlushAnyway)
        {
            m_FlushAnyway = bFlushAnyway;
        }
        //-----------------------------------------------------------------------
        public virtual bool ShareMemory_ReadData(ref byte[] pData, int nSize, int nOffset = 0)
        {
            if (m_AOIShareMemory!=null)//初始化有成功
            {
                pData = m_AOIShareMemory.ReadData(nSize, nOffset);
                //m_AOIShareMemory.ReadData(pData, nSize, nOffset);//指標
                return true;
            }
            return false;
        }
        public virtual bool ShareMemory_WriteData(byte[] pData, int nSize, int nOffset = 0)
        {
            if (m_PLCShareMemory!=null)
            {
                m_PLCShareMemory.WriteData(pData, nSize, nOffset);
                //m_pPLCUsm->WriteData(pData, nSize, nOffset);
                return true;
            }
            return false;
        }

        //Batch <=> byte[]----------------------------------------------------
        private byte[] StructToBytes(BATCH_SHARE_SYSTCCL_INITPARAM pData)
        {
            int size = Marshal.SizeOf(pData);
            byte[] bytes = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(pData, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);

            return bytes;
        }
        private byte[] StructToBytes(BATCH_SHARE_SYSTPP_INITPARAM pData)
        {
            int size = Marshal.SizeOf(pData);
            byte[] bytes = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(pData, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);

            return bytes;
        }
        private BATCH_SHARE_SYSTCCL_INITPARAM BytesToCCL(byte[] bytes)
        {
            BATCH_SHARE_SYSTCCL_INITPARAM data;
            int size = Marshal.SizeOf(typeof(BATCH_SHARE_SYSTCCL_INITPARAM));

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);

            data = Marshal.PtrToStructure<BATCH_SHARE_SYSTCCL_INITPARAM>(ptr);
            Marshal.FreeHGlobal(ptr);

            return data;
        }
        private BATCH_SHARE_SYSTPP_INITPARAM BytesToPP(byte[] bytes)
        {
            BATCH_SHARE_SYSTPP_INITPARAM data;
            int size = Marshal.SizeOf(typeof(BATCH_SHARE_SYSTPP_INITPARAM));

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);

            data = Marshal.PtrToStructure<BATCH_SHARE_SYSTPP_INITPARAM>(ptr);
            Marshal.FreeHGlobal(ptr);

            return data;
        }
    }

}
