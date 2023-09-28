#pragma once
#include "SystCCLProcessBase.h"

class CSystCCLProcessCHANGSHU :public CSystCCLProcessBase{
public:
	CSystCCLProcessCHANGSHU();
	virtual ~CSystCCLProcessCHANGSHU();
protected:
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);
	virtual void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL &xData);
	virtual void DoSetInfoField(BATCH_SHARE_SYST_INFO &xInfo);
	virtual void SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData);

	virtual UINT GetBaseAddress(){
#ifdef _DEBUG
		return 10000;
#else
		return 10000;
#endif
	}
private:
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
private:
	void InitField();
};

//�`���ͯqA2/A4�u �]���Ȥ�n�D, �NPLC Address�����[�W2000
class CSystCCLProcessCHANGSHU2 :public CSystCCLProcessCHANGSHU{
protected:
	virtual UINT GetBaseAddress(){
		return CSystCCLProcessCHANGSHU::GetBaseAddress() + 2000;
	}
};