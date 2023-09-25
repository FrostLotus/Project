#include "stdafx.h"
#include "SystCCLProcessBase.h"
#ifdef USE_IN_COMMUNICATOR
#include "PLCCommunicator.h"//theApp
#endif

#define CCL_NOTIFYVALUE_COMMAND	101

CSystCCLProcessBase* CSystCCLProcessBase::m_this = NULL;
CSystCCLProcessBase::CSystCCLProcessBase()
{
	Init();
}
CSystCCLProcessBase::~CSystCCLProcessBase()
{
	Finalize();
}
long CSystCCLProcessBase::ON_OPEN_PLC(LPARAM lp)
{
	long lRtn = CPLCProcessBase::ON_OPEN_PLC(lp);

	if (lRtn == 0){
		for (int i = 0; i < TIMER_MAX; i++){
#ifdef USE_TEST_TIMER
			if (i == TIMER_TEST || TIMER_RESULT == i/*make write time reasonable*/){
				m_tTimerEvent[i] = SetTimer(NULL, i, 500, QueryTimer);
			}
			else
#endif
				BOOL bSettimer = TRUE;
			switch (i){
			case TIMER_CUSTOM_ACTION:
				bSettimer = IS_SUPPORT_CUSTOM_ACTION();
				break;
			default:
				bSettimer = TRUE;
				break;
			}
			if (bSettimer)
				m_tTimerEvent[i] = SetTimer(NULL, i, TIMER_INTERVAL, QueryTimer);
		}
		for (int i = NULL; i < EV_COUNT; i++) m_hEvent[i] = ::CreateEvent(NULL, TRUE, FALSE, NULL);
		m_hThread = ::CreateThread(NULL, NULL, Thread_Result, this, NULL, NULL);
	}

	return lRtn;
}
void CSystCCLProcessBase::ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp)
{
	switch (wp){
	case WM_AOI_RESPONSE_CMD:
		ProcessAOIResponse(lp);
		break;
	case WM_SYST_RESULTCCL_CMD:
		ProcessResult();
		break;
	case WM_SYST_INFO_CHANGE:
		BATCH_SHARE_SYST_INFO xInfo;
		memset(&xInfo, 0, sizeof(xInfo));
		theApp.InsertDebugLog(L"WM_SYST_INFO_CHANGE", LOG_DEBUG);
		if (USM_ReadData((unsigned char*)&xInfo, sizeof(xInfo))){
			SetInfoField(xInfo);
		}
		break;
	}
}
void CSystCCLProcessBase::Init()
{
	m_this = this;
	NotifyAOI(WM_SYST_PARAMINIT_CMD, NULL);
}
void CSystCCLProcessBase::Finalize()
{
	for (int i = 0; i < TIMER_MAX; i++){
		::KillTimer(NULL, m_tTimerEvent[i]);
	}
}
void CSystCCLProcessBase::ProcessAOIResponse(LPARAM lp)
{
	switch (lp){
	case WM_SYST_PARAMCCL_CMD: //AOI切換完成->復位
	{
		WORD wReceive = _ttoi(GET_PLC_FIELD_VALUE(FIELD_COMMAND_RECEIVED));
		WORD wCommand = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_COMMAND));
		if (wCommand == CCL_NOTIFYVALUE_COMMAND){
			WORD wReceive = 100;
			SET_PLC_FIELD_DATA(FIELD_COMMAND_RECEIVED, sizeof(WORD), (BYTE*)&wReceive);
			theApp.InsertDebugLog(L"reset FIELD_COMMAND_RECEIVED", LOG_DEBUG);
		}
		else{
			CString str;
#ifdef USE_IN_COMMUNICATOR
			str.Format(L"field error, FIELD_COMMAND_RECEIVED:%s, FIELD_CCL_COMMAND:%s", GET_PLC_FIELD_VALUE(FIELD_COMMAND_RECEIVED), GET_PLC_FIELD_VALUE(FIELD_CCL_COMMAND));
			theApp.InsertDebugLog(str);
#endif
		}
	}
		break;
	}
}
void CSystCCLProcessBase::ProcessResult()
{
	BATCH_SHARE_SYST_RESULTCCL xResult;
	memset(&xResult, 0, sizeof(xResult));
	DWORD dwFlag = 0;
	int nOffset = 0;
	int nWordSize = sizeof(WORD);
	int nFloatSize = sizeof(float);
	CString strLog, strTemp;



	auto SetData = [&](DWORD dwAddFlag, void *pDst, int nSize, CString strAddLog, int nLogType){
		if (dwFlag & dwAddFlag){
			if (USM_ReadData((BYTE*)pDst, nSize, nOffset)){
				nOffset += nSize;
				CString strTemp;
				switch (nLogType){
				case LOG_FLOAT: //float
					strTemp.Format(L" %s: %f\r\n", strAddLog, *(float*)pDst);
					break;
				case LOG_WORD: //word
					strTemp.Format(L" %s: %d\r\n", strAddLog, *(WORD*)pDst);
					break;
				case LOG_CSTRING: //cstring
					strTemp.Format(L" %s: %s\r\n", strAddLog, CString((char*)pDst));
					break;
				}
				strLog += strTemp;
			}
		}
	};

	theApp.InsertDebugLog(L"ProcessResult", LOG_DEBUG);
	if (USM_ReadData((unsigned char*)&dwFlag, sizeof(dwFlag))){
		nOffset += sizeof(DWORD);
		strTemp.Format(L"ProcessResult Flag:%d ", dwFlag);
		strLog += strTemp;

		SetData(SRF_REAL_Y_ONE, &xResult.fReal_One_Y, nFloatSize, L"fReal_One_Y", LOG_FLOAT);
		SetData(SRF_REAL_Y_TWO, &xResult.fReal_Two_Y, nFloatSize, L"fReal_Two_Y", LOG_FLOAT);
		SetData(SRF_REAL_X_ONE, &xResult.fReal_One_X, nFloatSize, L"fReal_One_X", LOG_FLOAT);
		SetData(SRF_REAL_X_TWO, &xResult.fReal_Two_X, nFloatSize, L"fReal_Two_X", LOG_FLOAT);
		SetData(SRF_REAL_DIFF_Y_ONE, &xResult.wDiff_One_Y, nWordSize, L"wDiff_One_Y", LOG_WORD);
		SetData(SRF_REAL_DIFF_Y_TWO, &xResult.wDiff_Two_Y, nWordSize, L"wDiff_Two_Y", LOG_WORD);
		SetData(SRF_REAL_DIFF_X_ONE, &xResult.wDiff_One_X, nWordSize, L"wDiff_One_X", LOG_WORD);
		SetData(SRF_REAL_DIFF_X_TWO, &xResult.wDiff_Two_X, nWordSize, L"wDiff_Two_X", LOG_WORD);
		SetData(SRF_REAL_DIFF_XY_ONE, &xResult.wDiff_One_XY, nWordSize, L"wDiff_One_XY", LOG_WORD);
		SetData(SRF_REAL_DIFF_XY_TWO, &xResult.wDiff_Two_XY, nWordSize, L"wDiff_Two_XY", LOG_WORD);
		SetData(SRF_FRONT_LEVEL, &xResult.wFrontLevel, nWordSize, L"wFrontLevel", LOG_WORD);
		SetData(SRF_FRONT_CODE, &xResult.cFrontCode, sizeof(xResult.cFrontCode), L"cFrontCode", LOG_CSTRING);
		SetData(SRF_BACK_LEVEL, &xResult.wBackLevel, nWordSize, L"wBackLevel", LOG_WORD);
		SetData(SRF_BACK_CODE, &xResult.cBackCode, sizeof(xResult.cBackCode), L"cBackCode", LOG_CSTRING);
		SetData(SRF_SIZE_G10, &xResult.wSize_G10, nWordSize, L"wSize_G10", LOG_WORD);
		SetData(SRF_SIZE_G12, &xResult.wSize_G12, nWordSize, L"wSize_G12", LOG_WORD);
		SetData(SRF_SIZE_G14, &xResult.wSize_G14, nWordSize, L"wSize_G14", LOG_WORD);
		SetData(SRF_RESULT_LEVEL, &xResult.wResultLevel, nWordSize, L"wResultLevel", LOG_WORD);
		SetData(SRF_NUM_AA, &xResult.wNum_AA, nWordSize, L"wNum_AA", LOG_WORD);
		SetData(SRF_NUM_A, &xResult.wNum_A, nWordSize, L"wNum_A", LOG_WORD);
		SetData(SRF_NUM_P, &xResult.wNum_P, nWordSize, L"wNum_P", LOG_WORD);
		SetData(SRF_QUALIFY_RATE, &xResult.wQualifyRate, nWordSize, L"wQualifyRate", LOG_WORD);
		SetData(SRF_DIFF_XY, &xResult.wDiff_XY, nWordSize, L"wDiff_XY", LOG_WORD);
		SetData(SRF_FRONT_SIZE, &xResult.fFrontDefectSize, sizeof(xResult.fFrontDefectSize), L"", LOG_NONE);
		SetData(SRF_FRONT_LOCATION, &xResult.wFrontDefectLocation, sizeof(xResult.wFrontDefectLocation), L"", LOG_NONE);
		SetData(SRF_BACK_SIZE, &xResult.fBackDefectSize, sizeof(xResult.fBackDefectSize), L"", LOG_NONE);
		SetData(SRF_BACK_LOCATION, &xResult.wBackDefectLocation, sizeof(xResult.wBackDefectLocation), L"", LOG_NONE);
		SetData(SRF_INDEX, &xResult.wIndex, nWordSize, L"", LOG_NONE);
		theApp.InsertDebugLog(strLog, LOG_DEBUG);
		PushResult(xResult);
	}
}
void CALLBACK CSystCCLProcessBase::QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer)
{
	if (m_this){
		m_this->ProcessTimer(nEventId);
	}
}
void CSystCCLProcessBase::ProcessTimer(UINT_PTR nEventId)
{
	for (int i = 0; i < TIMER_MAX; i++){
		if (m_tTimerEvent[i] == nEventId){
			switch (i)
			{
#ifdef USE_TEST_TIMER
			case TIMER_TEST:
			{
				BATCH_SHARE_SYST_RESULTCCL_ xResult;
				memset(&xResult, 0, sizeof(BATCH_SHARE_SYST_RESULTCCL_));
				static int nCount = 0;
					xResult.wDiff_One_Y = m_vResult.size();
					xResult.wDiff_Two_Y = 2;
					xResult.wDiff_One_X = 3;
					xResult.wDiff_Two_X = 4;
					xResult.wDiff_One_XY = 5;
					xResult.wDiff_Two_XY = 6;
					xResult.wFrontLevel = 7;
					xResult.wFrontLocation = 8;
					xResult.wBackLevel = 9;
					xResult.wBackLocation = 10;
					xResult.wSize_G10 = 11;
					xResult.wSize_G12 = 12;
					xResult.wSize_G14 = 13;

					xResult.fReal_One_Y = 1.1;
					xResult.fReal_Two_Y = 2.2;
					xResult.fReal_One_X = 3.3;
					xResult.fReal_Two_X = 4.4;

					CStringA str;
					str.Format("12345678901234567890123456789a");
					memcpy(xResult.cFrontCode, str.GetBuffer(), _countof(xResult.cFrontCode));
					str.Format("abcdefghijklmnopqrstuvwxyzabca");
					memcpy(xResult.cBackCode, str.GetBuffer(), _countof(xResult.cBackCode));

					xResult.wResultLevel = 1;
					xResult.wNum_AA = 2;
					xResult.wNum_A = 3;
					xResult.wNum_P = 4;
					xResult.wQualifyRate = 5;
					xResult.wDiff_XY = 6;

				BATCH_SHARE_SYST_INFO xInfo; memset(&xInfo, 0, sizeof(xInfo));
				xInfo.xInfo1.cSizeReady = 1;
				xInfo.xInfo1.cSizeRunning = 1;
				xInfo.xInfo1.cCCDReady = 1;
				xInfo.xInfo1.cCCDRunning = 1;
				xInfo.xInfo2.cCCDError1 = 1;
				xInfo.xInfo2.cSizeError1 = 1;

				PushResult(xResult);
				SetInfoField(xInfo);
			}
				break;
#endif
			case TIMER_COMMAND:
				{
					WORD wOldCommand = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_COMMAND));
					GET_PLC_FIELD_DATA(FIELD_CCL_COMMAND);
					WORD wCommand = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_COMMAND));
					
					if (wOldCommand == 0 && wCommand == CCL_NOTIFYVALUE_COMMAND){
#ifdef SHOW_PERFORMANCE
						LARGE_INTEGER xStart, xEnd, xFreq;
						QueryPerformanceFrequency(&xFreq);
						QueryPerformanceCounter(&xStart);
#endif
						for (int i = 0; i < GetFieldSize(); i++){
							PLC_DATA_ITEM_ *pItem = GetPLCAddressInfo(i, FALSE);
							if (pItem && pItem->xAction == ACTION_BATCH){
								GET_PLC_FIELD_DATA(i);
							}
						}
#ifdef SHOW_PERFORMANCE
						QueryPerformanceCounter(&xEnd);
						double d = (xEnd.QuadPart - xStart.QuadPart) * 1000.0 / xFreq.QuadPart;
						TRACE(L"Read time : %.2f \n", d);
#endif
						ON_CCL_NEWBATCH();
					}
					else if (wOldCommand != 0 && wCommand == CCL_NOTIFYVALUE_COMMAND){//log error data
						CString strLog;
						strLog.Format(L"FIELD_CCL_COMMAND previous data error: %d", wOldCommand);
						theApp.InsertDebugLog(strLog, LOG_DEBUG);
					}
					if (wCommand != CCL_NOTIFYVALUE_COMMAND && wCommand != 0){ 
						CString strLog;
						strLog.Format(L"FIELD_CCL_COMMAND data error: %d", wCommand);
						theApp.InsertDebugLog(strLog, LOG_DEBUG);
					}
				}
				break;
			case TIMER_COMMAND_RECEIVED:
				GET_PLC_FIELD_DATA(FIELD_COMMAND_RECEIVED);
				break;
			case TIMER_RESULT:
				GET_PLC_FIELD_DATA(FIELD_RESULT);
				if (GET_FLUSH_ANYWAY()){
					::SetEvent(m_hEvent[EV_WRITE]);
				}
				else{
					if (GET_PLC_FIELD_VALUE(FIELD_RESULT) == L"0"){ //ready to write
						::SetEvent(m_hEvent[EV_WRITE]);
					}
					else{
						CString str;
						str.Format(L"PLC not ready to receive insp result, Field value: %s \n", GET_PLC_FIELD_VALUE(FIELD_RESULT));
						theApp.InsertDebugLog(str);
					}
				}
				//not yet
				break;
			case TIMER_RESULT_RECEIVED:
				GET_PLC_FIELD_DATA(FIELD_RESULT_RECEIVED);
				if (GET_PLC_FIELD_VALUE(FIELD_RESULT_RECEIVED) == L"300"){
					TRACE(L"PLC has received result! \n");
					PLC_DATA_ITEM_ *pResultReceived = GetPLCAddressInfo(FIELD_RESULT_RECEIVED, FALSE);
					WORD wValue = 0;
					SET_PLC_FIELD_DATA(FIELD_RESULT_RECEIVED, sizeof(WORD), (BYTE*)&wValue); //復位
				}
				break;
			case TIMER_C10:
			{
				if (GET_PLC_FIELD_ACTION(FIELD_CCL_NO_C10) != ACTION_NOTIFY){
					//僅notify type要啟動timer看C10 index
					::KillTimer(NULL, m_tTimerEvent[TIMER_C10]);
					m_tTimerEvent[TIMER_C10] = NULL;
					continue;
				}
				WORD wOldC10 = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_NO_C10));
				GET_PLC_FIELD_DATA(FIELD_CCL_NO_C10);
				WORD wCurC10 = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_NO_C10));
				if (wCurC10 != 0 /*東莞每次變化前會歸零*/ && wOldC10 != wCurC10){
					ON_C10_CHANGE(wCurC10);
				}
			}
				break;
			case TIMER_CUSTOM_ACTION:
				DoCustomAction();
				break;
			default:
				break;
			}
		}
	}
}
void CSystCCLProcessBase::SetInfoField(BATCH_SHARE_SYST_INFO &xInfo)
{
	theApp.InsertDebugLog(L"Set Info Start", LOG_DEBUG);
	DoSetInfoField(xInfo);
	theApp.InsertDebugLog(L"Set Info End", LOG_DEBUG);
}
void CSystCCLProcessBase::ON_CCL_NEWBATCH()
{
	BATCH_SHARE_SYST_PARAMCCL xData;
	memset(&xData, 0, sizeof(BATCH_SHARE_SYST_PARAMCCL));
	wcscpy_s(xData.cName, GET_PLC_FIELD_VALUE(FIELD_ORDER).GetBuffer());
	wcscpy_s(xData.cMaterial, GET_PLC_FIELD_VALUE(FIELD_MATERIAL).GetBuffer());
	wcscpy_s(xData.cModel, GET_PLC_FIELD_VALUE(FIELD_MODEL).GetBuffer());
	wcscpy_s(xData.cAssign, GET_PLC_FIELD_VALUE(FIELD_ASSIGN).GetBuffer());
	xData.wAssignNum			= _ttoi(GET_PLC_FIELD_VALUE(FIELD_ASSIGNNUM));
	xData.wSplitNum				= _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLITNUM));
	xData.wSplit_One_Y			= _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLIT_ONE_Y));
	xData.wSplit_Two_Y			= _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLIT_TWO_Y));
	xData.wSplit_Three_Y		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLIT_THREE_Y));
	xData.wSplit_One_X			= _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLIT_ONE_X));
	xData.wSplit_Two_X			= _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLIT_TWO_X));
	xData.wSplit_Three_X		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLIT_THREE_X));
	xData.fThickSize			= (float)_ttof(GET_PLC_FIELD_VALUE(FIELD_THICK_SIZE));
	xData.fThickCCL				= (float)_ttof(GET_PLC_FIELD_VALUE(FIELD_THICK_CCL));
	wcscpy_s(xData.cCCLType, GET_PLC_FIELD_VALUE(FIELD_CCL_TYPE).GetBuffer());
	xData.wGrayScale			= _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_GRAYSCALE));
	xData.wPixel_AA				= _ttoi(GET_PLC_FIELD_VALUE(FIELD_LEVEL_AA_PIXEL));
	xData.wPixel_A				= _ttoi(GET_PLC_FIELD_VALUE(FIELD_LEVEL_A_PIXEL));
	xData.wPixel_P				= _ttoi(GET_PLC_FIELD_VALUE(FIELD_LEVEL_P_PIXEL));
	xData.wDiff_One_Y			= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_Y));
	xData.wDiff_One_X			= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_X));
	xData.wDiff_One_XY			= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_XY));

	xData.wDiff_One_Y_Min		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_Y_MIN));
	xData.wDiff_One_Y_Max		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_Y_MAX));
	xData.wDiff_One_X_Min		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_X_MIN));
	xData.wDiff_One_X_Max		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_X_MAX));

	xData.wDiff_One_XY_Min		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_XY_MIN));
	xData.wDiff_One_XY_Max		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_XY_MAX));
	
	xData.wDiff_Two_Y_Min		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_TWO_Y_MIN));
	xData.wDiff_Two_Y_Max		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_TWO_Y_MAX));
	xData.wDiff_Two_X_Min		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_TWO_X_MIN));
	xData.wDiff_Two_X_Max		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_TWO_X_MAX));
	xData.wDiff_Two_XY_Min		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_TWO_XY_MIN));
	xData.wDiff_Two_XY_Max		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_TWO_XY_MAX));
	
	xData.wDiff_Three_Y_Min		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_THREE_Y_MIN));
	xData.wDiff_Three_Y_Max		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_THREE_Y_MAX));
	xData.wDiff_Three_X_Min		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_THREE_X_MIN));
	xData.wDiff_Three_X_Max		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_THREE_X_MAX));
	xData.wDiff_Three_XY_Min		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_THREE_XY_MIN));
	xData.wDiff_Three_XY_Max		= _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_THREE_XY_MAX));

	xData.wNum_AA				= _ttoi(GET_PLC_FIELD_VALUE(FIELD_AA_NUM));

	xData.wNO_C06				= _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_NO_C06));
	xData.wNO_C10				= _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_NO_C10));
	xData.wNO_C12				= _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_NO_C12));

	if (USM_WriteData((BYTE*)&xData, sizeof(xData))){
		//log data, not yet
		NotifyAOI(WM_SYST_PARAMCCL_CMD, NULL);
	}
}
void CSystCCLProcessBase::ON_C10_CHANGE(WORD wC10)
{
	CString strLog;
	strLog.Format(L"C10 Index: %d", wC10);
	theApp.InsertDebugLog(strLog, LOG_PLCC10);
	NotifyAOI(WM_SYST_C10CHANGE_CMD, wC10);
}
void CSystCCLProcessBase::PushResult(BATCH_SHARE_SYST_RESULTCCL &xResult)
{
	std::lock_guard< std::mutex > lock(m_oMutex);
	m_vResult.push_back(xResult);
}
DWORD CSystCCLProcessBase::Thread_Result(void* pvoid)
{
	CSystCCLProcessBase* pThis = (CSystCCLProcessBase*)pvoid;
	BOOL              bRun = TRUE;
	while (bRun)
	{
		switch (::WaitForMultipleObjects(EV_COUNT, pThis->m_hEvent, FALSE, INFINITE))
		{
		case CASE_WRITE:
			{
				::ResetEvent(pThis->m_hEvent[EV_WRITE]);
				BATCH_SHARE_SYST_RESULTCCL *pData = NULL;
				static BATCH_SHARE_SYST_RESULTCCL xData;
				{
					std::lock_guard< std::mutex > lock(pThis->m_oMutex);
					if (pThis->m_vResult.size()){
						xData = pThis->m_vResult.at(0);
						pThis->m_vResult.erase(pThis->m_vResult.begin());
						pData = &xData;
					}
				}
				if (pData){
#ifdef SHOW_PERFORMANCE
					LARGE_INTEGER xStart, xEnd, xFreq;
					QueryPerformanceFrequency(&xFreq);
					QueryPerformanceCounter(&xStart);
#endif
					pThis->WriteResult(*pData);
#ifdef SHOW_PERFORMANCE
					QueryPerformanceCounter(&xEnd);
					double d = (xEnd.QuadPart - xStart.QuadPart) * 1000.0 / xFreq.QuadPart;
					TRACE(L"write time : %.2f \n", d);
#endif
				}
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
void CSystCCLProcessBase::WriteResult(BATCH_SHARE_SYST_RESULTCCL &xData)
{
#ifdef USE_IN_COMMUNICATOR
	theApp.InsertDebugLog(L"Write Insp start!", LOG_DEBUG);
#endif

	SET_PLC_FIELD_DATA(FIELD_FRONT_LEVEL, 2, (BYTE*)&xData.wFrontLevel);
	SET_PLC_FIELD_DATA(FIELD_FRONT_CODE, sizeof(xData.cFrontCode), (BYTE*)&xData.cFrontCode);
	SET_PLC_FIELD_DATA(FIELD_BACK_LEVEL, 2, (BYTE*)&xData.wBackLevel);
	SET_PLC_FIELD_DATA(FIELD_BACK_CODE, sizeof(xData.cBackCode), (BYTE*)&xData.cBackCode);

	if (IS_SUPPORT_FLOAT_REALSIZE()){
		SET_PLC_FIELD_DATA(FIELD_REAL_Y_ONE, 4, (BYTE*)&xData.fReal_One_Y);
		SET_PLC_FIELD_DATA(FIELD_REAL_Y_TWO, 4, (BYTE*)&xData.fReal_Two_Y);
		SET_PLC_FIELD_DATA(FIELD_REAL_X_ONE, 4, (BYTE*)&xData.fReal_One_X);
		SET_PLC_FIELD_DATA(FIELD_REAL_X_TWO, 4, (BYTE*)&xData.fReal_Two_X);
	}
	SET_PLC_FIELD_DATA(FIELD_REAL_DIFF_ONE_Y, 2, (BYTE*)&xData.wDiff_One_Y);
	SET_PLC_FIELD_DATA(FIELD_REAL_DIFF_TWO_Y, 2, (BYTE*)&xData.wDiff_Two_Y);
	SET_PLC_FIELD_DATA(FIELD_REAL_DIFF_ONE_X, 2, (BYTE*)&xData.wDiff_One_X);
	SET_PLC_FIELD_DATA(FIELD_REAL_DIFF_TWO_X, 2, (BYTE*)&xData.wDiff_Two_X);
	SET_PLC_FIELD_DATA(FIELD_REAL_DIFF_ONE_XY, 2, (BYTE*)&xData.wDiff_One_XY);
	SET_PLC_FIELD_DATA(FIELD_REAL_DIFF_TWO_XY, 2, (BYTE*)&xData.wDiff_Two_XY);

	DoWriteResult(xData);

	//write flag
	WORD wResult = 200;
	SET_PLC_FIELD_DATA(FIELD_RESULT, 2, (BYTE*)&wResult);

#ifdef USE_IN_COMMUNICATOR
	theApp.InsertDebugLog(L"Write Insp done!", LOG_DEBUG);
#endif
}
