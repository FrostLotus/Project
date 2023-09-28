#pragma once
#include "SystCCLProcessBase.h"

class CSystCCLProcessDONGGUAN_SONG8 :public CSystCCLProcessBase
{
public:
	CSystCCLProcessDONGGUAN_SONG8();
	virtual ~CSystCCLProcessDONGGUAN_SONG8();
protected:
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);
	virtual void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL &xData);
	virtual void DoSetInfoField(BATCH_SHARE_SYST_INFO &xInfo);
	virtual void SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData);
	virtual BOOL IS_SUPPORT_FLOAT_REALSIZE(){ return FALSE; }; //東莞松八廠實際尺寸欄位型態為word, 非float
	virtual CPU_SERIES GetCPU(){ return CPU_SERIES::R_SERIES; }
	virtual BOOL IS_SUPPORT_CUSTOM_ACTION() { return TRUE; } //是否支援客製化行為
	virtual void DoCustomAction(); //客製化行為
private:
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
};