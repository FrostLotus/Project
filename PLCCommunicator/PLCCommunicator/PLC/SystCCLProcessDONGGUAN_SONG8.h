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
	virtual BOOL IS_SUPPORT_FLOAT_REALSIZE(){ return FALSE; }; //�F��Q�K�t��ڤؤo��쫬�A��word, �Dfloat
	virtual CPU_SERIES GetCPU(){ return CPU_SERIES::R_SERIES; }
	virtual BOOL IS_SUPPORT_CUSTOM_ACTION() { return TRUE; } //�O�_�䴩�Ȼs�Ʀ欰
	virtual void DoCustomAction(); //�Ȼs�Ʀ欰
private:
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
};