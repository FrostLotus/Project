#pragma once
#include "DataHandlerBase.h"

class CPLCDataHandler: public CDataHandlerBase{
public:
	CPLCDataHandler();
	virtual ~CPLCDataHandler();

	virtual void SetInitParam(BATCH_SHARE_SYST_INITPARAM *pData);
	virtual void GetInitParam(BATCH_SHARE_SYST_INITPARAM *pData);

	virtual void SetSYSTParam_WebCopper(BATCH_SHARE_SYST_BASE *pData);
	virtual void GetSYSTParam_WebCopper(BATCH_SHARE_SYST_BASE *pData);
	virtual void SetSYSYParam_CCL(BATCH_SHARE_SYST_PARAMCCL *pData);
	virtual void GetSYSTParam_CCL(BATCH_SHARE_SYST_PARAMCCL *pData);
	virtual void GetSYSTInfo_CCL(BATCH_SHARE_SYST_INFO *pInfo);

	virtual void SetSYSTInfo_CCL(DWORD dwField, BATCH_SHARE_SYST_INFO &xInfo);
private:
	BATCH_SHARE_SYST_RESULTCCL m_xSYSTResult;
	DWORD m_dwSYSTDirty;

	BATCH_SHARE_SYST_INFO m_xInfo;
};