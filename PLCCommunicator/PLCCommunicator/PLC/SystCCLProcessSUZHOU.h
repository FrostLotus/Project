#pragma once
#include "SystCCLProcessBase.h"

class CSystCCLProcessSUZHOU :public CSystCCLProcessBase{
public:
	CSystCCLProcessSUZHOU();
	virtual ~CSystCCLProcessSUZHOU();
protected:
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);
	virtual void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL &xData);
	virtual void DoSetInfoField(BATCH_SHARE_SYST_INFO &xInfo);
	virtual void SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData);
private:
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
};