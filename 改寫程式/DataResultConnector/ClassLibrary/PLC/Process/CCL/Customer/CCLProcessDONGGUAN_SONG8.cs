using ActProgTypeLib;
using ClassLibrary.PLC.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.PLC.Process.CCL.Customer
{

    public interface ICCLProcessDONGGUAN_SONG8
    {
		PLC_DATA_ITEM[] m_pPLC_FIELD_INFO { get; set; }
		//----------------------------------------------------
		PLC_DATA_ITEM GetPLCAddressInfo(int nFieldId, bool bSkip);
		void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL xData);
		void DoSetInfoField(ref BATCH_SHARE_SYST_INFO xInfo);
		void SetMXParam(IActProgType[] pParam, ref BATCH_SHARE_SYSTCCL_INITPARAM xData);
		bool IS_SUPPORT_FLOAT_REALSIZE();// { return FALSE; }; //東莞松八廠實際尺寸欄位型態為word, 非float
		CPU_SERIES GetCPU();// { return CPU_SERIES::R_SERIES; }
	    bool IS_SUPPORT_CUSTOM_ACTION();// { return TRUE; } //是否支援客製化行為
		void DoCustomAction(); //客製化行為
	}

    class CCLProcessDONGGUAN_SONG8 : CCLProcessBase, ICCLProcessDONGGUAN_SONG8
    {
        public virtual PLC_DATA_ITEM[] m_pPLC_FIELD_INFO { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //---------------------------------------------------------------
        
        public override void DoCustomAction()
        {
            throw new NotImplementedException();
        }
        public override void DoSetInfoField(ref BATCH_SHARE_SYST_INFO xInfo)
        {
            throw new NotImplementedException();
        }
        public override PLC_DATA_ITEM GetPLCAddressInfo(int nFieldId, bool bSkip)
        {
            throw new NotImplementedException();
        }
        public override bool IS_SUPPORT_CUSTOM_ACTION()
        {
            throw new NotImplementedException();
        }
        public override bool IS_SUPPORT_FLOAT_REALSIZE()
        {
            throw new NotImplementedException();
        }
        //---------------------------------------------------------------
        public virtual void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL xData)
        {
            throw new NotImplementedException();
        }
        public virtual CPU_SERIES GetCPU()
        {
            throw new NotImplementedException();
        }
        public virtual void SetMXParam(IActProgType[] pParam, ref BATCH_SHARE_SYSTCCL_INITPARAM xData)
        {
            throw new NotImplementedException();
        }
        //---------------------------------------------------------------

    }
}
