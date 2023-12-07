using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public byte Len; //bytes
        public string DevType;
        public uint Address;
        public uint StartBit;
        public uint EndBit;
        public bool Signed;
        public PLC_DATA_ITEM()
        {

        }
        public PLC_DATA_ITEM(string strFieldName, int nFieldType,
                             PLC_VALUE_TYPE eValueType, PLC_ACTION_TYPE eActionType,
                             byte cLen, string strDeviceType,
                             uint uAddress,
                             uint uStartBit = 0xFFFFFFFF, uint uEndBit = 0xFFFFFFFF // 使用此值來表示 C++ 中的 -1
                            )
        {
            this.FieldName = strFieldName;
            this.FieldType = nFieldType;
            this.ValType = eValueType;
            this.Action = eActionType;
            this.Len = cLen;
            this.DevType = strDeviceType;
            this.Address = uAddress;
            this.StartBit = uStartBit;
            this.EndBit = uEndBit;
            this.Signed = false;
        }

    }
    #endregion
    //PLCProcessBase
    public interface IPLCrocessBase
    {
        //usm<unsigned char>* m_pAOIUsm;
        //usm<unsigned char>* m_pPLCUsm;

        ///<summary>PLC資料</summary>
        PLCDATA[] PLCData { get; set; }
        bool FlushAnyway { get; set; }
        long LastRtn { get; set; }
        IntPtr Wnd_AOI { get; set; }
        byte[] PLCInitData { get; set; }
        //--------------------------------------------------------------
        //PLCProcessBase();

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

        long GET_PLC_FIELD_DATA(int nFieldId);
        long GET_PLC_FIELD_DATA(List<int> vField);
        long SET_PLC_FIELD_DATA(int nFieldId, int nSizeInByte, byte[] pData);
        long SET_PLC_FIELD_DATA(List<int> vField, byte[] pData);
        long SET_PLC_FIELD_DATA_BIT(int nFieldStart, int nFieldEnd, int nSizeInByte, byte[] pData);
        long SET_PLC_FIELD_DATA_BIT(int nField, int nBitPosition, bool bValue);

        void SET_FLUSH_ANYWAY(bool bFlushAnyway); //{ m_bFlushAnyway = bFlushAnyway; };
        bool GET_FLUSH_ANYWAY(); //{ return m_bFlushAnyway; };
        //-------------------
	    void INIT_PLCDATA();
        void DESTROY_PLC_DATA();

        bool USM_ReadData(byte[] pData, int nSize, int nOffset = 0);
        bool USM_WriteData(byte[] pData, int nSize, int nOffset = 0);

        //IPLCProcess
        long ON_OPEN_PLC(IntPtr lparam);

        void SET_INIT_PARAM(IntPtr lparam, byte[] pData);// { };
        //-------------------------------
	    void Init();
        //void Finalize();
        long GET_PLC_FIELD_DATA(int nFieldId, byte[] pData);
        void GET_PLC_RANDOM_DATA(List<int> vField, string strField, int nSizeInWord);
    }
    //CCL

    class PLCProcessBase : IPLCrocessBase
    {
        //私有項目
        private PLCDATA[] m_PLCData;
        private bool m_FlushAnyway;
        private long m_LastRtn;
        private IntPtr m_Wnd_AOI;
        private byte[] m_PLCInitData;
        //變換項目
        public virtual PLCDATA[] PLCData { get { return m_PLCData; } set { m_PLCData = value; } }
        public virtual bool FlushAnyway { get { return m_FlushAnyway; } set { m_FlushAnyway = value; } }
        public virtual long LastRtn { get { return m_LastRtn; } set { m_LastRtn = value; } }
        public virtual IntPtr Wnd_AOI { get { return m_Wnd_AOI; } set { m_Wnd_AOI = value; } }
        public virtual byte[] PLCInitData { get { return m_PLCInitData; } set { m_PLCInitData = value; } }
        //--------------------------------------------------------------------------------------------------
        public PLCProcessBase()
        {
            Init();
        }
        public virtual void DESTROY_PLC_DATA()
        {
            if (PLCData!=null)
            {
                int nFieldSize = GetFieldSize();
                for (int i = 0; i < nFieldSize; i++)
                {
                    if (PLCData[i].pData!=null)
                    {
                        PLCData[i].pData = null;
                    }
                }
                PLCData = null;
            }
        }
        public virtual void DO_CUSTOM_TEST()
        {
        }
        public virtual int GetFieldSize()
        {
            return 0;//父類不定義 根據後面表單[BATCH]實作override取得實際表單參數數量
        }
        public virtual PLC_DATA_ITEM GetPLCAddressInfo(int nFieldId, bool bSkip)
        {
            return null;//父類不定義 
        }
        
        public virtual bool GET_FLUSH_ANYWAY()
        {
            return m_FlushAnyway;
        }
        public virtual void SET_FLUSH_ANYWAY(bool bFlushAnyway)
        {
            m_FlushAnyway = bFlushAnyway;
        }


        public virtual PLC_ACTION_TYPE GET_PLC_FIELD_ACTION(int nFieldId)
        {
            int nFieldSize = GetFieldSize();
            PLC_ACTION_TYPE eType = PLC_ACTION_TYPE.ACTION_NOTIFY;
            if (nFieldId >= 0 && nFieldId < nFieldSize)
            {
                PLC_DATA_ITEM pCur = GetPLCAddressInfo(nFieldId, false);
                if (pCur!=null)
                {
                    eType = pCur.Action;//僅提取單一的所以為0
                }
            }
            return eType;
        }
        ///<summary>取得對應軟元件之名稱:(D100.2)</summary>
        public virtual string GET_PLC_FIELD_ADDRESS(int nFieldId)
        {
            int nFieldSize = GetFieldSize();
            string strDes = "fail in [GET_PLC_FIELD_ADDRESS]: out of range";
            if (nFieldId >= 0 && nFieldId < nFieldSize)
            {
                PLC_DATA_ITEM pCur = GetPLCAddressInfo(nFieldId, false);
                if (pCur!=null)
                {
                    if (pCur.ValType == PLC_VALUE_TYPE.PLC_TYPE_BIT && pCur.DevType == "D")
                    {
                        //special case for D. ex:200.F
                        if (pCur.StartBit != 0xFFFFFFFF && pCur.StartBit == pCur.EndBit)
                        {
                            strDes = $"{pCur.DevType}{ pCur.Address}.{ pCur.StartBit}";
                        }
                        else
                        {
                            //Assert(false);
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
        ///<summary>取得對應ID(field)之值(BYTE)</summary>
        public virtual byte[] GET_PLC_FIELD_BYTE_VALUE(int nFieldId)
        {
            int nFieldSize = GetFieldSize();
            if (nFieldId >= 0 && nFieldId < nFieldSize)//範圍內
            {
                PLC_DATA_ITEM pCur = GetPLCAddressInfo(nFieldId, false);
                if (pCur!=null) //取得
                {
                    return m_PLCData[nFieldId].pData;
                }
            }
            return null;
        }
        public virtual long GET_PLC_FIELD_DATA(int nFieldId)
        {
            return GET_PLC_FIELD_DATA(nFieldId, m_PLCData[nFieldId].pData);
        }
        public virtual long GET_PLC_FIELD_DATA(List<int> vField)
        {
            long lRtn = ERR_DLL_NOT_LOAD;
            string strField="", strTemp="";
            int nTotal = 0;
            GET_PLC_RANDOM_DATA(vField, strField, nTotal);

            if (nTotal!=null)
            {
                short[] pData = new short[nTotal];
                //memset(pData, 0, sizeof(short) * nTotal);
                Array.Resize(ref pData, sizeof(short));//裡面會是0嗎
                for (int i = 0; i < pData.Length; i++) pData[i] = 0;

                lRtn = ReadRandom(strField, nTotal, pData);

                if (lRtn == 0)
                {
                    BYTE* pCur = (BYTE*)pData;
                    for (auto & i : vField)
                    {
                        PLC_DATA_ITEM_* pItem = GetPLCAddressInfo(i, FALSE);
                        if (pItem)
                        {
                            //update time and data
                            m_pPLCData[i].xTime = CTime::GetCurrentTime().GetTime();
                            memcpy(m_pPLCData[i].pData, pCur, pItem->cLen);
                            pCur += pItem->cLen;
                        }
                    }
                    ON_BATCH_PLCDATA_CHANGE(*vField.begin(), *vField.rbegin());
                }
                delete[] pData;
            }
            return lRtn;
        }
        public virtual long GET_PLC_FIELD_DATA(int nFieldId, byte[] pData)
        {
            throw new NotImplementedException();
        }
        public virtual string GET_PLC_FIELD_NAME(int nFieldId)
        {
            throw new NotImplementedException();
        }
        public virtual string GET_PLC_FIELD_TIME(int nFieldId)
        {
            throw new NotImplementedException();
        }
        public virtual string GET_PLC_FIELD_VALUE(int nFieldId)
        {
            throw new NotImplementedException();
        }
        ///<summary>[READ_RANDOM2]取得對應ID之PLC資料</summary>
        public virtual void GET_PLC_RANDOM_DATA(List<int> vField, string strField, int nSizeInWord)
        {
            throw new NotImplementedException();
        }
        public virtual bool HAS_CUSTOM_TEST()
        {
            throw new NotImplementedException();
        }
        public virtual void Init()
        {
            throw new NotImplementedException();
        }
        public virtual void INIT_PLCDATA()
        {
            throw new NotImplementedException();
        }
        public virtual void NotifyAOI(IntPtr wparam, IntPtr lparam)
        {
            throw new NotImplementedException();
        }
        public virtual long ON_OPEN_PLC(IntPtr lparam)
        {
            throw new NotImplementedException();
        }
        
        public virtual void SET_INIT_PARAM(IntPtr lparam, byte[] pData)
        {
            throw new NotImplementedException();
        }
        public virtual long SET_PLC_FIELD_DATA(int nFieldId, int nSizeInByte, byte[] pData)
        {
            throw new NotImplementedException();
        }
        public virtual long SET_PLC_FIELD_DATA(List<int> vField, byte[] pData)
        {
            throw new NotImplementedException();
        }
        public virtual long SET_PLC_FIELD_DATA_BIT(int nFieldStart, int nFieldEnd, int nSizeInByte, byte[] pData)
        {
            throw new NotImplementedException();
        }
        public virtual long SET_PLC_FIELD_DATA_BIT(int nField, int nBitPosition, bool bValue)
        {
            throw new NotImplementedException();
        }
        public virtual bool USM_ReadData(byte[] pData, int nSize, int nOffset = 0)
        {
            throw new NotImplementedException();
        }
        public virtual bool USM_WriteData(byte[] pData, int nSize, int nOffset = 0)
        {
            throw new NotImplementedException();
        }
    }

}
