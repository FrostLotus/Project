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
    #region PLC參數利用
    ///<summary>PLC資料結構</summary>
    public struct PLCDATA
    {
        public byte[] pData;
        public DateTime xTime;
    };
    ///<summary>項目數值格式</summary>
    public enum emPLC_DEVICE_TYPE
    {
        TYPE_STRING,
        TYPE_WORD,
        TYPE_FLOAT,
        TYPE_DWORD,
        TYPE_BIT,
    };
    ///<summary>項目處理模式</summary>
    public enum emPLC_ACTION_TYPE
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
        public emPLC_DEVICE_TYPE ValType;
        public emPLC_ACTION_TYPE Action;
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
                             emPLC_DEVICE_TYPE eValueType, emPLC_ACTION_TYPE eActionType,
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
    //=================================================================
    //Interface
    //=================================================================
    class PLCProcessBase : MELSECIOController
    {
        //私有項目
        private PLCDATA[] m_PLCData;
        private bool m_FlushAnyway;
        private int m_LastRtn = 0;
        private IntPtr m_Wnd_AOI;
        private byte[] m_PLCInitData;

        private ShareMemory<byte> m_AOIShareMemory;
        private ShareMemory<byte> m_PLCShareMemory;
        //------------------------------------------------------------------------------------
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
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
            if (m_PLCInitData == null)
            {
                //first open, read data from shared memory
                int nDataSize = 0;
#if OFF_LINE
                string tmp_IP = DataHeadlerBase.BATCH_PLCIP;//IP
#endif
                switch ((int)lparam)//PLC環境參數建立
                {
                    case (int)WM_APP_CMD.WM_SYST_PARAMINIT_CMD: //CCL參數建立
                        {
                            BATCH_SHARE_SYSTCCL_INITPARAM pData = new BATCH_SHARE_SYSTCCL_INITPARAM();//CCL站體參數
#if OFF_LINE
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
#if OFF_LINE            
                            pData.m_CCL_INITPARAM.PLCIP = tmp_IP;
                            pData.m_CCL_INITPARAM.TargetNetworkNo = 0;
                            pData.m_CCL_INITPARAM.TargetStationNo = 0xFF;
                            pData.FX5U = true;
#endif
                            m_PLCInitData = StructToBytes(pData);//鍵入
                            nDataSize = Marshal.SizeOf(typeof(BATCH_SHARE_SYSTPP_INITPARAM));//尺寸
                        }
                        break;
                }
                //-------------------------------------
                if (m_PLCInitData != null && nDataSize != 0) //PLC環境參數建立成功
                {
                    InsertDebugLog("ON_OPEN_PLC", AOI_LOG_Result.LOG_DEBUG);//log
# if OFF_LINE
                    //string str = L"192.168.2.29";
                    //wcscpy_s(xData.cPLCIP, str.GetBuffer());
                    //xData.lTargetNetworkNo = 0;
                    //xData.lTargetStationNo = 0xFF;*/
#else
                    if (AOI_ReadData(ref m_PLCInitData, nDataSize))
#endif
                    {
                        switch ((int)lparam)
                        {
                            case ((int)WM_APP_CMD.WM_SYST_PARAMINIT_CMD):
                                {
                                    SET_INIT_PARAM(lparam, m_PLCInitData);//空?
                                    //ON_SET_PLCPARAM(BytesToCCL(m_PLCInitData));//空?
                                    ON_SET_PLCPARAM(BytesToInitParam<BATCH_SHARE_SYSTCCL_INITPARAM>(m_PLCInitData));
                                }
                                break;
                            case ((int)WM_APP_CMD.WM_SYST_PP_PARAMINIT_CMD):
                                {
                                    SET_INIT_PARAM(lparam, m_PLCInitData);//空?
                                    //ON_SET_PLCPARAM(BytesToPP(m_PLCInitData).m_CCL_INITPARAM);//空?
                                    ON_SET_PLCPARAM(BytesToInitParam<BATCH_SHARE_SYSTPP_INITPARAM>(m_PLCInitData).m_CCL_INITPARAM);
                                }
                                break;
                        }
                    }
                }
            }
            if (m_PLCInitData != null)//表單參數建立成功
            {
                switch ((int)lparam)
                {
                    case ((int)WM_APP_CMD.WM_SYST_PARAMINIT_CMD):
                        {
                            //lRtn = OpenDevice(BytesToCCL(m_PLCInitData));
                            lRtn = OpenDevice(BytesToInitParam<BATCH_SHARE_SYSTCCL_INITPARAM>(m_PLCInitData));
                        }
                        break;
                    case ((int)WM_APP_CMD.WM_SYST_PP_PARAMINIT_CMD):
                        {
                            //lRtn = OpenDevice(BytesToPP(m_PLCInitData).m_BATCH_SHARE_SYSTCCL_INITPARAM);
                            lRtn = OpenDevice(BytesToInitParam<BATCH_SHARE_SYSTPP_INITPARAM>(m_PLCInitData).m_CCL_INITPARAM);
                        }
                        break;
                }
                string strLog;
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
        //------------------------------------------------------------------------
        public virtual void SET_INIT_PARAM(IntPtr lparam, byte[] pData)
        {

        }
        public virtual void INIT_PLCDATA()
        {
            //將m_PLCData初始化
            int nMax = GetFieldSize();
            m_PLCData = new PLCDATA[nMax];
            for (int i = 0; i < nMax; i++)//
            {
                PLC_DATA_ITEM pItem = GetPLCAddressInfo(i, false);
                int nDataSize = pItem.DataLength;
                if (pItem.ValType == emPLC_DEVICE_TYPE.TYPE_STRING)
                {
                    nDataSize = pItem.DataLength + 1;//resize
                }
                m_PLCData[i].pData = new byte[nDataSize];//設置型別寬度 先不用加入時間
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
        //-----------------------------------------------------------------------
        ///<summary>取得對應ID(field)之值(BYTE)</summary>
        public virtual T GET_PLC_FIELD_VALUE<T>(int nFieldId)
        {
            int nFieldSize = GetFieldSize();//以最外層override呼叫為主
            if (nFieldId >= 0 && nFieldId < nFieldSize)//範圍內
            {
                PLC_DATA_ITEM pCur = GetPLCAddressInfo(nFieldId, false);//取得此ID的相關PLC DATA
                if (pCur != null) //取得
                {
                    switch (typeof(T).Name)
                    {
                        case "Byte[]":
                            return (T)(object)m_PLCData[nFieldId].pData;
                        case "String":
                            return (T)(object)Encoding.ASCII.GetString(m_PLCData[nFieldId].pData);
                        case "Int32":
                            return (T)(object)BitConverter.ToInt32(m_PLCData[nFieldId].pData, 0);
                        case "UInt32":
                            return (T)(object)BitConverter.ToUInt32(m_PLCData[nFieldId].pData, 0);
                        case "Int16":
                            return (T)(object)BitConverter.ToInt16(m_PLCData[nFieldId].pData, 0);
                        case "UInt16":
                            return (T)(object)BitConverter.ToUInt16(m_PLCData[nFieldId].pData, 0);
                        case "Single":
                            return (T)(object)BitConverter.ToSingle(m_PLCData[nFieldId].pData, 0);
                        case "Char[]":
                            return (T)(object)Encoding.ASCII.GetChars(m_PLCData[nFieldId].pData);
                        case "Single[]":
                            //byte[] to float[]
                            int floatCount = m_PLCData[nFieldId].pData.Length / 4;
                            float[] floatArray = new float[floatCount];//每个 float 占用 4 个字节
                            for (int i = 0; i < floatCount; i++)
                            {
                                floatArray[i] = BitConverter.ToSingle(m_PLCData[nFieldId].pData, i * 4); // 每次转换 4 个字节为一个 float
                            }
                            return (T)(object)floatArray;
                        default:
                            throw new ArgumentException("Unsupported type");
                    }
                }
                throw new ArgumentException("未取得");
            }
            throw new ArgumentException("out of range");
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
                    if (pCur.ValType == emPLC_DEVICE_TYPE.TYPE_BIT && pCur.DevType == "D")
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
        public virtual emPLC_ACTION_TYPE GET_PLC_FIELD_ACTION(int nFieldId)
        {
            int nFieldSize = GetFieldSize();
            emPLC_ACTION_TYPE eType = emPLC_ACTION_TYPE.ACTION_NOTIFY;
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
        public virtual int REFLASH_PLC_FIELD_DATA(int nFieldId)
        {
            return REFLASH_PLC_FIELD_DATA(nFieldId, ref m_PLCData[nFieldId].pData);
        }
        public virtual int REFLASH_PLC_FIELD_DATA(List<int> vField)
        {
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
            string strField = "";
            int nTotal = 0;
            REFLASH_PLC_RANDOM_DATA(vField, ref strField, ref nTotal);

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
        public virtual int REFLASH_PLC_FIELD_DATA(int nFieldId, ref byte[] pData)
        {
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
            PLC_DATA_ITEM pItem = GetPLCAddressInfo(nFieldId, false);//整個對應ID資料讀回來
            if (pItem != null)//成功
            {
                switch (pItem.ValType)
                {
                    case emPLC_DEVICE_TYPE.TYPE_FLOAT:
                        float[] tmp_floatData = new float[pItem.DataLength];
                        lRtn = ReadAddress(pItem.DevType, pItem.Address, 1, ref tmp_floatData);
                        //pData = FloatToByte(tmp_floatData);
                        pData = GetByteArray(tmp_floatData);
                        break;
                    case emPLC_DEVICE_TYPE.TYPE_STRING:
                        char[] tmp_charData = new char[pItem.DataLength];
                        lRtn = ReadAddress(pItem.DevType, pItem.Address, pItem.DataLength, ref tmp_charData);
                        //pData = CharToByte(tmp_charData);
                        pData = GetByteArray(tmp_charData);
                        break;
                    case emPLC_DEVICE_TYPE.TYPE_WORD:
                        char[] tmp_WORDData = new char[pItem.DataLength];
                        lRtn = ReadAddress(pItem.DevType, pItem.Address, 1, ref tmp_WORDData);
                        //pData = CharToByte(tmp_WORDData);
                        pData = GetByteArray(tmp_WORDData);
                        break;
                    case emPLC_DEVICE_TYPE.TYPE_DWORD:
                        char[] tmp_DWORDData = new char[pItem.DataLength];
                        lRtn = ReadAddress(pItem.DevType, pItem.Address, 2, ref tmp_DWORDData);
                        //pData = CharToByte(tmp_DWORDData);
                        pData = GetByteArray(tmp_DWORDData);
                        break;
                    case emPLC_DEVICE_TYPE.TYPE_BIT:
                        short[] tmp_shortData = new short[pItem.DataLength];
                        lRtn = ReadOneAddress(pItem.DevType, pItem.Address, ref tmp_shortData);
                        //pData = shortToByte(tmp_shortData);
                        pData = GetByteArray(tmp_shortData);
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
        public virtual void REFLASH_PLC_RANDOM_DATA(List<int> vField, ref string strField, ref int nSizeInWord)
        {
            string strTemp = "wrong value";
            foreach (var i in vField)//共vField.length
            {
                PLC_DATA_ITEM pItem = GetPLCAddressInfo(i, false);
                if (pItem != null)
                {
                    switch (pItem.ValType)
                    {
                        case emPLC_DEVICE_TYPE.TYPE_WORD:
                        case emPLC_DEVICE_TYPE.TYPE_BIT:
                        case emPLC_DEVICE_TYPE.TYPE_FLOAT:
                        case emPLC_DEVICE_TYPE.TYPE_DWORD:
                        case emPLC_DEVICE_TYPE.TYPE_STRING:
                            int nSize = pItem.DataLength / 2;
                            nSizeInWord += nSize;//大小疊加
                            for (int j = 0; j < nSize; j++)
                            {
                                if (pItem.ValType == emPLC_DEVICE_TYPE.TYPE_BIT && pItem.DevType == "D")
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
        public virtual int SET_PLC_FIELD_DATA<T>(int nFieldId, int nSizeInByte, T pData)
        {
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
            int nFieldSize = GetFieldSize();

            if (nFieldId >= 0 && nFieldId < nFieldSize)
            {
                PLC_DATA_ITEM pItem = GetPLCAddressInfo(nFieldId, false);
                //初始化
                byte[] pOldValue = new byte[nSizeInByte];
                pOldValue = m_PLCData[nFieldId].pData;//先將原來資料複製一份
                //--------------------------------------------------------------
                //依類型轉換為byte[]
                byte[] tmpByteArray;
#if SHOW_PERFORMANCE
                Stopwatch stopwatch = new Stopwatch();
                long frequency = Stopwatch.Frequency;// 获取性能频率
                stopwatch.Start();// 获取起始时间
#endif
                switch (typeof(T).Name)
                {
                    case "Byte[]":
                        tmpByteArray = pData as byte[];
                        break;
                    case "String":
                        string tmpString = pData as string;
                        tmpByteArray = Encoding.UTF8.GetBytes(tmpString);
                        break;
                    case "Int32":
                        int tmpInt = (int)(object)pData;
                        tmpByteArray = BitConverter.GetBytes(tmpInt);
                        break;
                    case "UInt32":
                        uint tmpUInt = (uint)(object)pData;
                        tmpByteArray = BitConverter.GetBytes(tmpUInt);
                        break;
                    case "Int16":
                        short tmpShort = (short)(object)pData;
                        tmpByteArray = BitConverter.GetBytes(tmpShort);
                        break;
                    case "UInt16":
                        ushort tmpUShort = (ushort)(object)pData;
                        tmpByteArray = BitConverter.GetBytes(tmpUShort);
                        break;
                    case "Single":
                        float tmpFloat = (float)(object)pData;
                        tmpByteArray = BitConverter.GetBytes(tmpFloat);
                        break;
                    case "Char[]":
                        char[] tmpCharArray = (char[])(object)pData;
                        tmpByteArray = Encoding.ASCII.GetBytes(tmpCharArray);
                        break;
                    case "Single[]":
                        float[] tmpfloatArray = (float[])(object)pData;
                        tmpByteArray = new byte[tmpfloatArray.Length * sizeof(float)];//給大小
                        Buffer.BlockCopy(tmpfloatArray, 0, tmpByteArray, 0, tmpByteArray.Length);
                        break;
                    default:
                        throw new ArgumentException("Unsupported type 輸入型別未定義");
                }
                //--------------------------------------------------------------
                //使直接轉WriteAddress(short[]型或short)
                switch (pItem.ValType)
                {
                    case emPLC_DEVICE_TYPE.TYPE_STRING:
                    case emPLC_DEVICE_TYPE.TYPE_WORD:
                    case emPLC_DEVICE_TYPE.TYPE_DWORD:
                    case emPLC_DEVICE_TYPE.TYPE_FLOAT:
                        //組合byte[]轉為short[]
                        if (TryPraseShort(tmpByteArray, out short[] shortArray))
                        {
                            lRtn = WriteAddress(pItem.DevType, pItem.Address, nSizeInByte, ref shortArray);
                        }
                        break;
                    case emPLC_DEVICE_TYPE.TYPE_BIT:
                        //判斷short[]是否僅1位大小
                        if (TryPraseShort(tmpByteArray, out short[] shortValue))
                        {
                            string strDevice = GET_PLC_FIELD_ADDRESS(nFieldId);
                            if (shortValue.Length == 1)//僅一個
                            {
                                short tmpSingleShort = shortValue[0];
                                lRtn = WriteOneAddress(strDevice, ref tmpSingleShort);
                            }
                        }
                        break;
                }
#if SHOW_PERFORMANCE
                stopwatch.Stop();// 获取结束时间
                double elapsedTimeSeconds = stopwatch.ElapsedTicks / Stopwatch.Frequency;
                Console.WriteLine($"write one field : {elapsedTimeSeconds} \n");
#endif
                if (lRtn == 0)//變更軟元件資料內容
                {
                    m_PLCData[nFieldId].xTime = DateTime.Now;
                    ON_PLCDATA_CHANGE(nFieldId, tmpByteArray, pItem.DataLength);
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
        public virtual int SET_PLC_FIELD_DATA(List<int> vField, byte[] pData)
        {
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
            string strField = "";
            int nSizeInWord = 0;
            //                   List   D100 D101..  長度
            REFLASH_PLC_RANDOM_DATA(vField, ref strField, ref nSizeInWord);
            if (nSizeInWord != 0 && TryPraseShort(pData, out short[] shortarray))
            {

                lRtn = WriteRandom(ref strField, nSizeInWord, ref shortarray);
            }
            return lRtn;
        }
        //-----------------------------------------------------------------------
        public virtual int SET_PLC_FIELD_DATA_BIT(int nFieldStart, int nFieldEnd, int nSizeInByte, byte[] pData)
        {
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;
            int nFieldSize = GetFieldSize();
            PLC_DATA_ITEM pWriteItem = GetPLCAddressInfo(nFieldStart, false);
            TryPraseShort(pData, out short[] shortArray);
            lRtn = WriteAddress(pWriteItem.DevType, pWriteItem.Address, nSizeInByte, ref shortArray);

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
            int lRtn = (int)emErrorCode.ERR_DLL_NOT_LOAD;

            PLC_DATA_ITEM pCur = GetPLCAddressInfo(nField, false);
            if (pCur != null)
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
                    TryPraseShort(m_PLCData[nField].pData, out short wValue);
                    if (bValue)
                        wValue |= (short)(1 << nBitPosition);
                    else
                        wValue &= (short)~(1 << nBitPosition);

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
        public virtual bool AOI_ReadData(ref byte[] pData, int nSize, int nOffset = 0)
        {
            if (m_AOIShareMemory != null)//初始化有成功
            {
                pData = m_AOIShareMemory.ReadData(nSize, nOffset);//指標
                return true;
            }
            return false;
        }
        public virtual bool PLC_WriteData(byte[] pData, int nSize, int nOffset = 0)
        {
            if (m_PLCShareMemory != null)
            {
                m_PLCShareMemory.WriteData(pData, nSize, nOffset);
                return true;
            }
            return false;
        }
        //Batch <=> byte[]-------------------------------------------------------
        public byte[] StructToBytes<T>(T pData) where T : struct
        {
            int size = Marshal.SizeOf(pData);
            byte[] bytes = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(pData, ptr, true);
                Marshal.Copy(ptr, bytes, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return bytes;
        }
        private byte[] GetByteArray<T>(T pData)
        {
            byte[] byteArray;
            switch (typeof(T).Name)
            {
                case "Char[]":
                    {
                        char[] charArray = pData as char[];
                        //2個字節
                        byteArray = new byte[charArray.Length * sizeof(char)];
                        Buffer.BlockCopy(charArray, 0, byteArray, 0, byteArray.Length);
                        return byteArray;
                    }
                case "Sing[]":
                    {
                        float[] floatArray = pData as float[];
                        //2個字節
                        byteArray = new byte[floatArray.Length * sizeof(float)];
                        Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
                        return byteArray;
                    }
                case "Int16[]":
                    {
                        short[] shortArray = pData as short[];
                        //2個字節
                        byteArray = new byte[shortArray.Length * sizeof(short)];
                        Buffer.BlockCopy(shortArray, 0, byteArray, 0, byteArray.Length);
                        return byteArray;
                    }
                default:
                    throw new ArgumentException("Unsupported type 輸入型別未定義");
            }
        }
        private bool TryPraseShort(byte[] byteArray, out short[] shortArray)
        {
            shortArray = new short[byteArray.Length / 2];
            if (byteArray.Length % 2 != 0)
            {
                return false;
            }
            for (int i = 0; i < shortArray.Length; i++)
            {
                shortArray[i] = (short)(byteArray[i * 2] | byteArray[i * 2 + 1] << 8); // 将两个字节合并为一个 short
            }
            return true;
        }
        private bool TryPraseShort(byte[] byteArray, out short singleshort)
        {
            singleshort = new short();
            if (byteArray.Length % 2 != 0)
            {
                return false;
            }
            singleshort = BitConverter.ToInt16(byteArray, 0);
            return true;
        }
        //------------------------------------------------------------------------------------
        private T BytesToInitParam<T>(byte[] bytes)
        {
            switch (typeof(T).Name)
            {
                case "BATCH_SHARE_SYSTCCL_INITPARAM":
                    {
                        //BATCH_SHARE_SYSTCCL_INITPARAM data;
                        int size = Marshal.SizeOf(typeof(BATCH_SHARE_SYSTCCL_INITPARAM));

                        IntPtr ptr = Marshal.AllocHGlobal(size);
                        Marshal.Copy(bytes, 0, ptr, size);

                        var data = Marshal.PtrToStructure<BATCH_SHARE_SYSTCCL_INITPARAM>(ptr);
                        Marshal.FreeHGlobal(ptr);

                        return (T)(object)data;
                    }
                case "BATCH_SHARE_SYSTPP_INITPARAM":
                    {
                        BATCH_SHARE_SYSTPP_INITPARAM data;
                        int size = Marshal.SizeOf(typeof(BATCH_SHARE_SYSTPP_INITPARAM));

                        IntPtr ptr = Marshal.AllocHGlobal(size);
                        Marshal.Copy(bytes, 0, ptr, size);

                        data = Marshal.PtrToStructure<BATCH_SHARE_SYSTPP_INITPARAM>(ptr);
                        Marshal.FreeHGlobal(ptr);

                        return (T)(object)data;
                    }
                default:
                    Console.WriteLine("不支援的表單格式");
                    throw new ArgumentException("Unsupported Batch");
            }
        }
    }

}
