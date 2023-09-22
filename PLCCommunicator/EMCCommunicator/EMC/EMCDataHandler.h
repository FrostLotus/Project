#pragma once
#ifdef SUPPORT_AOI
#include "PLC\DataHandlerBase.h"
#else
#include "DataHandlerBase.h"
#endif
#include <mutex>


class IEMCNotify{
public:
	IEMCNotify(){ m_pLink = NULL; }
	virtual ~IEMCNotify(){ m_pLink = NULL; };
	void Attach(IEMCNotify *pLink){ m_pLink = pLink; };
protected:
#ifdef SUPPORT_AOI
	//EMC -> AOI
	virtual void OnEMCParam(vector<BATCH_SHARE_EMC_CCLPARAM> &vParam){
		if (m_pLink){
			m_pLink->OnEMCParam(vParam);
		}
	}
	virtual void OnEMCParam(BATCH_SHARE_EMC_PPPARAM *pParam){
		if (m_pLink){
			m_pLink->OnEMCParam(pParam);
		}
	}
#endif
	//AOI -> EMC
	virtual BOOL OnInitEMCProcess(BATCH_SHARE_EMC_INITPARAM *pData){
		if (m_pLink)
			return m_pLink->OnInitEMCProcess(pData);
		else 
			return FALSE;
	}
	//EMC -> AOI
	virtual BOOL OnEMCResult(BATCH_SHARE_EMC_CCLRESULT *pData){
		if (m_pLink)
			return m_pLink->OnEMCResult(pData);
		else
			return FALSE;
	}
	virtual BOOL OnEMCBatchEnd(BATCH_SHARE_EMC_CCLEND *pData){
		if (m_pLink)
			return m_pLink->OnEMCBatchEnd(pData);
		else
			return FALSE;
	}
	virtual BOOL OnEMCResult(BATCH_SHARE_EMC_PPRESULT *pData){
		if (m_pLink)
			return m_pLink->OnEMCResult(pData);
		else
			return FALSE;
	}
	virtual BOOL OnEMCBatchEnd(BATCH_SHARE_EMC_PPEND *pData){
		if (m_pLink)
			return m_pLink->OnEMCBatchEnd(pData);
		else
			return FALSE;
	}
	virtual BOOL OnEMCErrorMsg(BATCH_SHARE_EMC_ERRORINFO *pData){
		if (m_pLink)
			return m_pLink->OnEMCErrorMsg(pData);
		else
			return FALSE;
	}
#ifndef SUPPORT_AOI
	virtual void OnPushEMCParam(vector<BATCH_SHARE_EMC_CCLPARAM> &vData){
		if (m_pLink)
			m_pLink->OnPushEMCParam(vData);
	}
	virtual void OnPushEMCParam(BATCH_SHARE_EMC_PPPARAM *pData){
		if (m_pLink)
			m_pLink->OnPushEMCParam(pData);
	}
	virtual void OnSendCCLDone(){
		if (m_pLink)
			m_pLink->OnSendCCLDone();
	}
	virtual void OnSendPPDone(){
		if (m_pLink)
			m_pLink->OnSendPPDone();
	}
#ifdef EMC_SIMLULATE
	virtual void OnSimulateData(BYTE *pData, int nSize, WPARAM wp, LPARAM lp){
		if (m_pLink)
			m_pLink->OnSimulateData(pData, nSize, wp, lp);
	}
#endif
#endif
private:
	IEMCNotify *m_pLink;
};


class CEMCDataHandler : public IEMCNotify{
public:
#ifdef EMC_SIMLULATE
	CEMCDataHandler(CString strUSMID);
#else
	CEMCDataHandler();
#endif
	virtual ~CEMCDataHandler();
protected:
	//IEMCNotify
	virtual BOOL OnInitEMCProcess(BATCH_SHARE_EMC_INITPARAM *pData);
#ifdef SUPPORT_AOI
	virtual void OnEMCParam(vector<BATCH_SHARE_EMC_CCLPARAM> &vParam);
	virtual void OnEMCParam(BATCH_SHARE_EMC_PPPARAM *pParam);
#else
	virtual void OnPushEMCParam(vector<BATCH_SHARE_EMC_CCLPARAM> &vData);
	virtual void OnPushEMCParam(BATCH_SHARE_EMC_PPPARAM *pData);
	virtual void OnSendCCLDone();
	virtual void OnSendPPDone();
#ifdef EMC_SIMLULATE
	virtual void OnSimulateData(BYTE *pData, int nSize, WPARAM wp, LPARAM lp);
#endif
#endif
	virtual BOOL OnEMCResult(BATCH_SHARE_EMC_CCLRESULT *pData);
	virtual BOOL OnEMCBatchEnd(BATCH_SHARE_EMC_CCLEND *pData);

	virtual BOOL OnEMCResult(BATCH_SHARE_EMC_PPRESULT *pData);
	virtual BOOL OnEMCBatchEnd(BATCH_SHARE_EMC_PPEND *pData);

	virtual BOOL OnEMCErrorMsg(BATCH_SHARE_EMC_ERRORINFO *pData);
private:
	BOOL WriteData(BYTE *pData, int nSize);
	BOOL ReadData(BYTE *pData, int nSize);
#ifndef SUPPORT_AOI
	void SetEMCParam(vector<BATCH_SHARE_EMC_CCLPARAM> &vData);
	void SetEMCParam(BATCH_SHARE_EMC_PPPARAM *pData);

	static DWORD __stdcall Thread_ProcessCmd(void* pvoid);
	void NotifyAOI(WPARAM wp, LPARAM lp);
#else
	void NotifyEMC(WPARAM wp, LPARAM lp);
	void SkipPreserveWord(int nSize, TCHAR *pWord);
	void RevertPreserveWord(int nSize, TCHAR *pWord);
#endif
private:
#ifndef SUPPORT_AOI
	vector<vector<BATCH_SHARE_EMC_CCLPARAM>> m_vCCLParam;
	vector<BATCH_SHARE_EMC_PPPARAM> m_vPPParam;
	std::mutex  m_oMutex;
	enum
	{
		EV_EXIT,
		EV_SENDCCL,
		EV_SENDCCL_DONE,
		EV_SENDPP,
		EV_SENDPP_DONE,
		EV_COUNT,
		CASE_EXIT = WAIT_OBJECT_0,
		CASE_SENDCCL,
		CASE_SENDCCL_DONE,
		CASE_SENDPP,
		CASE_SENDPP_DONE,
	};
	BOOL m_bSendFlag;
	HANDLE m_hThread;
	HANDLE m_hEvent[EV_COUNT];
#endif
	usm<unsigned char> *m_pUsm;
};