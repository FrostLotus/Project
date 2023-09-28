#pragma once
#include "SystCCLProcessBase.h"

class CSystCCLProcessJIUJIANG :public CSystCCLProcessBase{
public:
	CSystCCLProcessJIUJIANG();
	virtual ~CSystCCLProcessJIUJIANG();
protected:
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);
	virtual void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL &xData); 
	virtual void DoSetInfoField(BATCH_SHARE_SYST_INFO &xInfo);
	virtual void SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData);

	virtual void ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp);
private:
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
};