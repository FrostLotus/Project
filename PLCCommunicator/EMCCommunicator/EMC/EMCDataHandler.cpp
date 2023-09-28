#include "stdafx.h"
#include "EMCDataHandler.h"
#ifdef USE_IN_COMMUNICATOR
#include "EMCCommunicator.h"
#include <set>
#endif

const std::set<TCHAR> ctPreserved = { L'/', L'\\', L':', L'*', L'?', L'\"', L'<', L'>', L'|' };
#define PRESERVE_WORD L'X'

#ifdef EMC_SIMLULATE
CEMCDataHandler::CEMCDataHandler(CString strUSMID)
#else
CEMCDataHandler::CEMCDataHandler()
#endif
{
	CString strID;
#ifdef SUPPORT_AOI
	strID = BATCH_AOI2EMC_MEM_ID;
#else
#ifdef EMC_SIMLULATE
	strID = strUSMID;
#else
	strID = BATCH_EMC2AOI_MEM_ID;
#endif
#endif
	m_pUsm = new usm<unsigned char>(strID, TRUE);
#ifndef SUPPORT_AOI
	for (int i = NULL; i < EV_COUNT; i++) m_hEvent[i] = ::CreateEvent(NULL, TRUE, FALSE, NULL);
	m_hThread = ::CreateThread(NULL, NULL, Thread_ProcessCmd, this, NULL, NULL);
	m_bSendFlag = TRUE;
	NotifyAOI(WM_EMC_PARAMINIT_CMD, NULL);
#endif
}
CEMCDataHandler::~CEMCDataHandler()
{
	if (m_pUsm){
		delete m_pUsm;
		m_pUsm = NULL;
	}
}
#ifndef SUPPORT_AOI
void CEMCDataHandler::NotifyAOI(WPARAM wp, LPARAM lp)
{
	HWND hWnd = ::FindWindow(NULL, AOI_MASTER_NAME);
	if (hWnd){
		::PostMessage(hWnd, WM_GPIO_MSG, wp, lp);
	}
}
#else
void CEMCDataHandler::NotifyEMC(WPARAM wp, LPARAM lp)
{
	HWND hWnd = ::FindWindow(NULL, EMC_COMMUNICATOR_NAME);
	if (hWnd){
		::PostMessage(hWnd, WM_GPIO_MSG, wp, lp);
	}
}
void CEMCDataHandler::SkipPreserveWord(int nSize, TCHAR *pWord)
{
	//Á×§K«O¯d¦r¤¸¿ù»~
	for (auto &xPreserved : ctPreserved){
		for (int i = 0; i < nSize; i++){
			if (*(pWord + i) == xPreserved){
				*(pWord + i) = PRESERVE_WORD;
			}
		}
	}
}
void CEMCDataHandler::RevertPreserveWord(int nSize, TCHAR *pWord)
{
	for (int i = 0; i < nSize; i++){
		if (*(pWord + i) == PRESERVE_WORD){
			*(pWord + i) = L'*';
		}
	}
}
void CEMCDataHandler::OnEMCParam(vector<BATCH_SHARE_EMC_CCLPARAM> &vParam)
{
	CCLParam xParam;
	memset(&xParam, 0, sizeof(CCLParam));
	theApp.InsertDebugLog(L"ReceiveData From EMC", LOG_EMCDEBUG);
	if (ReadData((BYTE*)&xParam, sizeof(CCLParam))){
		for (int i = 0; i < xParam.nSize; i++){
			if (i < MAX_PARAM){
				vParam.push_back(xParam.xParam[i]);

				CString strLog;
				strLog.Format(L"cStation:%s cMissionID:%s cBatchName:%s cMaterial:%s cSerial:%s nNum:%d nBookNum:%d nSheetNum:%d nSplit:%d nStatus:%d cEmpID:%s "
					, xParam.xParam[i].cStation
					, xParam.xParam[i].cMissionID
					, xParam.xParam[i].cBatchName
					, xParam.xParam[i].cMaterial
					, xParam.xParam[i].cSerial
					, xParam.xParam[i].nNum
					, xParam.xParam[i].nBookNum
					, xParam.xParam[i].nSheetNum
					, xParam.xParam[i].nSplit
					, xParam.xParam[i].nStatus
					, xParam.xParam[i].cEmpID
					);
				theApp.InsertDebugLog(strLog, LOG_EMCDEBUG);
			}
		}

	}
	for (auto &i : vParam){
		SkipPreserveWord(_countof(i.cBatchName), i.cBatchName);
	}
	NotifyEMC(WM_AOI_RESPONSE_CMD, WM_EMC_PARAMCCL_CMD);
}
void CEMCDataHandler::OnEMCParam(BATCH_SHARE_EMC_PPPARAM *pParam)
{
	if (ReadData((BYTE*)pParam, sizeof(BATCH_SHARE_EMC_PPPARAM))){
		CString strLog, strTemp;

		strLog.Format(L"EMCParam cStation:%s cMissionID:%s cBatchName:%s cMaterial:%s cSerial:%s nStatus:%d cEmpID:%s "
			, pParam->cStation
			, pParam->cMissionID
			, pParam->cBatchName
			, pParam->cMaterial
			, pParam->cSerial
			, pParam->nStatus
			, pParam->cEmpID
			);
		theApp.InsertDebugLog(strLog, LOG_EMCDEBUG);
	}

	SkipPreserveWord(_countof(pParam->cBatchName), pParam->cBatchName);
	NotifyEMC(WM_AOI_RESPONSE_CMD, WM_EMC_PARAMPP_CMD);
}
#endif
BOOL CEMCDataHandler::OnInitEMCProcess(BATCH_SHARE_EMC_INITPARAM *pData)
{
#ifdef SUPPORT_AOI
	BOOL bRtn = WriteData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_INITPARAM));
	if (bRtn)
		NotifyEMC(WM_AOI_RESPONSE_CMD, WM_EMC_PARAMINIT_CMD);
#else
	BOOL bRtn = ReadData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_INITPARAM));
#endif
	return bRtn;
}
BOOL CEMCDataHandler::OnEMCResult(BATCH_SHARE_EMC_CCLRESULT *pData)
{
#ifndef SUPPORT_AOI
	BOOL bRtn = ReadData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_CCLRESULT));
#else
	RevertPreserveWord(_countof(pData->cBatchName), pData->cBatchName);
	BOOL bRtn = WriteData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_CCLRESULT));
	if (bRtn)
		NotifyEMC(WM_EMC_RESULTCCL_CMD, NULL);
#endif
	if (bRtn){
		//log
		CString strLog;
		strLog.Format(L"EMCResult_CCL cStation:%s cMissionID:%s cBatchName:%s cMaterial:%s cSerial:%s nIndex:%d nBookNum:%d cSheet:%s cDefectType:%s "
			, pData->cStation
			, pData->cMissionID
			, pData->cBatchName
			, pData->cMaterial
			, pData->cSerial
			, pData->nIndex
			, pData->nBookNum
			, pData->cSheet
			, pData->cDefectType
			);
		theApp.InsertDebugLog(strLog, LOG_EMCDEBUG);
	}
	return bRtn;
}
BOOL CEMCDataHandler::OnEMCBatchEnd(BATCH_SHARE_EMC_CCLEND *pData)
{
#ifndef SUPPORT_AOI
	BOOL bRtn = ReadData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_CCLEND));
#else
	RevertPreserveWord(_countof(pData->cBatchName), pData->cBatchName);
	BOOL bRtn = WriteData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_CCLEND));
	if (bRtn)
		NotifyEMC(WM_EMC_ENDCCL_CMD, NULL);
#endif
	if (bRtn){
		//log
		CString strLog;
		strLog.Format(L"EMCBatchEnd_CCL cStation:%s cMissionID:%s cBatchName:%s cMaterial:%s cSerial:%s nIndex:%d"
			, pData->cStation
			, pData->cMissionID
			, pData->cBatchName
			, pData->cMaterial
			, pData->cSerial
			, pData->nIndex
			);
		theApp.InsertDebugLog(strLog, LOG_EMCDEBUG);
	}
	return bRtn;
}
BOOL CEMCDataHandler::WriteData(BYTE *pData, int nSize)
{
	if (m_pUsm && m_pUsm->WriteData(pData, nSize)){
			return TRUE;
		}
	return FALSE;
}
BOOL CEMCDataHandler::ReadData(BYTE *pData, int nSize)
{
#ifdef SUPPORT_AOI
	usm<unsigned char> xUsm(BATCH_EMC2AOI_MEM_ID, TRUE);
#else
	usm<unsigned char> xUsm(BATCH_AOI2EMC_MEM_ID, TRUE);
#endif
	if (xUsm.ReadData(pData, nSize)){
		return TRUE;
	}
	return FALSE;
}
#ifndef SUPPORT_AOI
void CEMCDataHandler::SetEMCParam(vector<BATCH_SHARE_EMC_CCLPARAM> &vData)
{
	int nSize = (int)vData.size();
	if (nSize > 0 && nSize <= MAX_PARAM){

		CCLParam xCCLParam;
		memset(&xCCLParam, 0, sizeof(CCLParam));
		xCCLParam.nSize = nSize;
		theApp.InsertDebugLog(L"SetEMCParam ", LOG_EMCDEBUG);
		for (int i = 0; i < nSize; i++){
			BATCH_SHARE_EMC_CCLPARAM &xParam = vData.at(i);
			memcpy(&xCCLParam.xParam[i], &xParam, sizeof(BATCH_SHARE_EMC_CCLPARAM));
			
			//log
			CString strLog, strTemp;
			
			strLog.Format(L"cStation:%s cMissionID:%s cBatchName:%s cMaterial:%s cSerial:%s nNum:%d nBookNum:%d nSheetNum:%d nSplit:%d nStatus:%d cEmpID:%s cMiss:%s nBeginBook:%d nEndBook:%d nBeginSheet:%d nEndSheet:%d"
				, xParam.cStation
				, xParam.cMissionID
				, xParam.cBatchName
				, xParam.cMaterial
				, xParam.cSerial
				, xParam.nNum
				, xParam.nBookNum
				, xParam.nSheetNum
				, xParam.nSplit
				, xParam.nStatus
				, xParam.cEmpID
				, xParam.cMiss
				, xParam.nBeginBook
				, xParam.nEndBook
				, xParam.nBeginSheet
				, xParam.nEndSheet
				);
			theApp.InsertDebugLog(strLog, LOG_EMCDEBUG);
		}
		if (WriteData((BYTE*)&xCCLParam, sizeof(CCLParam))){
			theApp.InsertDebugLog(L"WriteOK", LOG_EMCDEBUG);
			NotifyAOI(WM_EMC_PARAMCCL_CMD, NULL);
		}
		else{
			theApp.InsertDebugLog(L"WriteFail", LOG_EMCDEBUG);
		}
	}
}
void CEMCDataHandler::SetEMCParam(BATCH_SHARE_EMC_PPPARAM *pData)
{
	if (WriteData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_PPPARAM))){
		NotifyAOI(WM_EMC_PARAMPP_CMD, NULL);
		//log
		CString strLog, strTemp;

		strLog.Format(L"SetEMCParam cStation:%s cMissionID:%s cBatchName:%s cMaterial:%s cSerial:%s nStatus:%d cEmpID:%s "
			, pData->cStation
			, pData->cMissionID
			, pData->cBatchName
			, pData->cMaterial
			, pData->cSerial
			, pData->nStatus
			, pData->cEmpID
			);
		theApp.InsertDebugLog(strLog, LOG_EMCDEBUG);
	}
}

void CEMCDataHandler::OnPushEMCParam(vector<BATCH_SHARE_EMC_CCLPARAM> &vData)
{
	std::lock_guard< std::mutex > lock(m_oMutex);
	m_vCCLParam.push_back(vData);
	::SetEvent(m_hEvent[EV_SENDCCL]);
}
void CEMCDataHandler::OnPushEMCParam(BATCH_SHARE_EMC_PPPARAM *pData)
{
	std::lock_guard< std::mutex > lock(m_oMutex);
	m_vPPParam.push_back(*pData);
	::SetEvent(m_hEvent[EV_SENDPP]);
}
void CEMCDataHandler::OnSendCCLDone()
{
	::SetEvent(m_hEvent[EV_SENDCCL_DONE]);
}
void CEMCDataHandler::OnSendPPDone()
{
	::SetEvent(m_hEvent[EV_SENDPP_DONE]);
}
#ifdef EMC_SIMLULATE
void CEMCDataHandler::OnSimulateData(BYTE *pData, int nSize, WPARAM wp, LPARAM lp)
{
	BOOL bRtn = WriteData(pData, nSize);
	if (bRtn){
		HWND hWnd = ::FindWindow(NULL, EMC_COMMUNICATOR_NAME);
		if (hWnd){
			::PostMessage(hWnd, WM_GPIO_MSG, wp, lp);
		}
	}
}
#endif
DWORD CEMCDataHandler::Thread_ProcessCmd(void* pvoid)
{
	CEMCDataHandler* pThis = (CEMCDataHandler*)pvoid;
	BOOL              bRun = TRUE;
	while (bRun)
	{
		switch (::WaitForMultipleObjects(EV_COUNT, pThis->m_hEvent, FALSE, INFINITE))
		{
		case CASE_SENDCCL:
		{
			::ResetEvent(pThis->m_hEvent[EV_SENDCCL]);
			{
				std::lock_guard< std::mutex > lock(pThis->m_oMutex);
				if (pThis->m_bSendFlag && pThis->m_vCCLParam.size()){
					pThis->m_bSendFlag = FALSE;
					vector<BATCH_SHARE_EMC_CCLPARAM> &vParam = pThis->m_vCCLParam.at(0);
					pThis->SetEMCParam(vParam);
					pThis->m_vCCLParam.erase(pThis->m_vCCLParam.begin());
				}
			}
		}
		break;
		case CASE_SENDCCL_DONE:
		{
			::ResetEvent(pThis->m_hEvent[EV_SENDCCL_DONE]);
			{
				std::lock_guard< std::mutex > lock(pThis->m_oMutex);
				pThis->m_bSendFlag = TRUE;
			}
			::SetEvent(pThis->m_hEvent[EV_SENDCCL]);
		}
		break;
		case CASE_SENDPP:
		{
			::ResetEvent(pThis->m_hEvent[EV_SENDPP]);
			{
				std::lock_guard< std::mutex > lock(pThis->m_oMutex);
				if (pThis->m_bSendFlag && pThis->m_vPPParam.size()){
					pThis->m_bSendFlag = FALSE;
					BATCH_SHARE_EMC_PPPARAM *pParam = &pThis->m_vPPParam.at(0);
					pThis->SetEMCParam(pParam);
					pThis->m_vPPParam.erase(pThis->m_vPPParam.begin());
				}
			}
		}
		break;
		case CASE_SENDPP_DONE:
		{
			::ResetEvent(pThis->m_hEvent[EV_SENDPP_DONE]);
			{
				std::lock_guard< std::mutex > lock(pThis->m_oMutex);
				pThis->m_bSendFlag = TRUE;
			}
			::SetEvent(pThis->m_hEvent[EV_SENDPP]);
		}
		break;
		case CASE_EXIT:
		{
			bRun = FALSE;
		}
		break;
		}
	}
	return NULL;
}
#endif

BOOL CEMCDataHandler::OnEMCResult(BATCH_SHARE_EMC_PPRESULT *pData)
{
#ifndef SUPPORT_AOI
	BOOL bRtn = ReadData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_PPRESULT));
#else
	BOOL bRtn = WriteData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_PPRESULT));
	if (bRtn)
		NotifyEMC(WM_EMC_RESULTPP_CMD, NULL);
#endif
	if (bRtn){
		//log
		CString strLog;
		strLog.Format(L"EMCResult_PP cStation:%s cMissionID:%s cBatchName:%s cMaterial:%s cSerial:%s fDefectBegin:%.2f fDefectEnd:%.2f cDefectType:%s "
			, pData->cStation
			, pData->cMissionID
			, pData->cBatchName
			, pData->cMaterial
			, pData->cSerial
			, pData->fDefectBegin
			, pData->fDefectEnd
			, pData->cDefectType
			);
		theApp.InsertDebugLog(strLog, LOG_EMCDEBUG);
	}
	return bRtn;
}
BOOL CEMCDataHandler::OnEMCBatchEnd(BATCH_SHARE_EMC_PPEND *pData)
{
#ifndef SUPPORT_AOI
	BOOL bRtn = ReadData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_PPEND));
#else
	RevertPreserveWord(_countof(pData->cBatchName), pData->cBatchName);
	BOOL bRtn = WriteData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_PPEND));
	if (bRtn)
		NotifyEMC(WM_EMC_ENDPP_CMD, NULL);
#endif
	if (bRtn){
		//log
		CString strLog;
		strLog.Format(L"GetEMCBatchEnd_PP cStation:%s cMissionID:%s cBatchName:%s cMaterial:%s cSerial:%s fLength:%.2f"
			, pData->cStation
			, pData->cMissionID
			, pData->cBatchName
			, pData->cMaterial
			, pData->cSerial
			, pData->fLength
			);
		theApp.InsertDebugLog(strLog, LOG_EMCDEBUG);
	}
	return bRtn;
}
BOOL CEMCDataHandler::OnEMCErrorMsg(BATCH_SHARE_EMC_ERRORINFO *pData)
{
#ifdef SUPPORT_AOI
	BOOL bRtn = ReadData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_ERRORINFO));
#else
	BOOL bRtn = WriteData((BYTE*)pData, sizeof(BATCH_SHARE_EMC_ERRORINFO));
	if (bRtn)
		NotifyAOI(WM_EMC_ERROR_CMD, NULL);
#endif
	if (bRtn){
		//log
		CString strLog;
		strLog.Format(L"EMCErrorMsg eErrorType:%d cErrorMsg:%s"
			, pData->eErrorType
			, pData->cErrorMsg
			);
		theApp.InsertDebugLog(strLog, LOG_EMCDEBUG);
	}
	return bRtn;
}
