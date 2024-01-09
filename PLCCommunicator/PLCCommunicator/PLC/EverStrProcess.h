#pragma once
#include "EverStrProcessBase.h"

class CEverStrProcess :public CEverStrProcessBase
{
public:
	CEverStrProcess();
	virtual ~CEverStrProcess();
protected:
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);
	virtual void DoWriteResult(BATCH_SHARE_SYST_RESULT_EVERSTR& xData);
	virtual void DoSetInfoField(BATCH_SHARE_SYST_INFO& xInfo);
	virtual void SetMXParam(IActProgType* pParam, BATCH_SHARE_SYSTCCL_INITPARAM& xData);
	virtual void ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp);

	virtual CPU_SERIES GetCPU() { return CPU_SERIES::FX5U_SERIES; }
	virtual BOOL IS_SUPPORT_CUSTOM_ACTION() { return FALSE; } //是否支援客製化行為
	virtual void DoCustomAction(); //客製化行為
private:
	PLC_DATA_ITEM_** m_pPLC_FIELD_INFO;
};