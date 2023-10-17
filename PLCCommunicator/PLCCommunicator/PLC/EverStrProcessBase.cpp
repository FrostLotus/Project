#include "stdafx.h"
#include "EverStrProcessBase.h"
#ifdef USE_IN_COMMUNICATOR
#include "PLCCommunicator.h"//theApp
#endif
#define CCL_NOTIFYVALUE_COMMAND	101

CEverStrProcessBase* CEverStrProcessBase::m_this = NULL;
///<summary>[Constructor]初始化</summary>
CEverStrProcessBase::CEverStrProcessBase()
{
	Init();
}
///<summary>[Constructor]終處置</summary>
CEverStrProcessBase::~CEverStrProcessBase()
{
	Finalize();
}
//===============================================================================
///<summary>[觸發]PLC開啟觸發</summary>
long CEverStrProcessBase::ON_OPEN_PLC(LPARAM lp)
{

	long lRtn = CPLCProcessBase::ON_OPEN_PLC(lp);//先做CPLCProcessBase的原式
	///-------------------------------------------
	if (lRtn == 0)//原式建置成功
	{
		for (int i = 0; i < TIMER_MAX; i++)
		{
#ifdef USE_TEST_TIMER
			if (i == TIMER_TEST || TIMER_RESULT == i/*make write time reasonable*/)
			{
				m_tTimerEvent[i] = SetTimer(NULL, i, 500, QueryTimer);
			}
#endif
			BOOL bSettimer = TRUE;
			switch (i)
			{
				case TIMER_CUSTOM_ACTION://客製額外需求
					bSettimer = IS_SUPPORT_CUSTOM_ACTION();//false
					break;
				default:
					bSettimer = TRUE;//i.順序:指令下發=>指令收到=>檢驗結果=>檢驗結果=>檢驗結果收到=>C10剪切小版編號
					break;
			}
			if (bSettimer)
			{
				m_tTimerEvent[i] = SetTimer(
					NULL,//視窗
					i,//非0  EVENT_ID
					TIMER_INTERVAL,//遞歸時間
					QueryTimer);//指定觸發函式 若null=Time
			}
		}
		for (int i = NULL; i < EV_COUNT; i++)
		{
			m_hEvent[i] = ::CreateEvent(NULL, TRUE, FALSE, NULL);
		}
		//Result回傳
		m_hThread = ::CreateThread(NULL, NULL, Thread_Result, this, NULL, NULL);
	}
	return lRtn;
}
///<summary>視窗控制碼觸發</summary>
void CEverStrProcessBase::ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp)
{
	switch (wp)
	{
		case WM_AOI_RESPONSE_CMD://下發:上位機PLC -> AOI回應
			ProcessAOIResponse(lp);
			break;
		case WM_SYST_RESULTCCL_CMD://上傳: AOI->上位機PLC回應
			ProcessResult();
			break;
		case WM_SYST_INFO_CHANGE://資訊部分(目前應不顯示)
			BATCH_SHARE_SYST_INFO xInfo;
			memset(&xInfo, 0, sizeof(xInfo));
			theApp.InsertDebugLog(L"WM_SYST_INFO_CHANGE", LOG_DEBUG);
			if (USM_ReadData((unsigned char*)&xInfo, sizeof(xInfo)))
			{
				SetInfoField(xInfo);
			}
			break;
	}
}
///<summary>初始項目</summary>
void CEverStrProcessBase::Init()
{
	m_this = this;
	NotifyAOI(WM_SYST_PARAMINIT_CMD, NULL);
}
///<summary>終處置項目</summary>
void CEverStrProcessBase::Finalize()
{
	for (int i = 0; i < TIMER_MAX; i++)
	{
		::KillTimer(NULL, m_tTimerEvent[i]);
	}
}
//===============================================================================
///<summary>處理AOI回應</summary>
void CEverStrProcessBase::ProcessAOIResponse(LPARAM lp)
{
	switch (lp)//控制碼
	{
		case WM_SYST_PARAMCCL_CMD: //PLC 101
		{
			WORD wReceive = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCD_COMMAND_RECEIVED));//給上位機PLC回應狀態
			WORD wCommand = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_COMMAND));//目前回應狀態
			if (wCommand == CCL_NOTIFYVALUE_COMMAND)//若目前回應狀態為101  狀況:上位機(PLC)下發資料完成
			{
				WORD wReceive = 100;
				SET_PLC_FIELD_DATA(FIELD_CCD_COMMAND_RECEIVED, sizeof(WORD), (BYTE*)&wReceive);//發送 100 給PLC, PLC接收後自行清除 100
				theApp.InsertDebugLog(L"reset FIELD_COMMAND_RECEIVED", LOG_DEBUG);
			}
			else
			{
				CString str;
#ifdef USE_IN_COMMUNICATOR
				str.Format(L"field error, FIELD_COMMAND_RECEIVED:%s, FIELD_CCL_COMMAND:%s", GET_PLC_FIELD_VALUE(FIELD_CCD_COMMAND_RECEIVED), GET_PLC_FIELD_VALUE(FIELD_CCL_COMMAND));
				theApp.InsertDebugLog(str);
#endif
			}
		}
		break;
	}
}
///<summary>處理工單Result回應</summary>
void CEverStrProcessBase::ProcessResult()
{
	//參數初始化
	BATCH_SHARE_SYST_RESULT_EVERSTR xResult;
	memset(&xResult, 0, sizeof(xResult));
	DWORD dwFlag = 0;
	int nOffset = 0;
	int nWordSize = sizeof(WORD);
	int nFloatSize = sizeof(float);
	CString strLog, strTemp;

	//設置DATA結構
	auto SetData = [&](DWORD dwAddFlag, void* pDst, int nSize, CString strAddLog, int nLogType)
	{
		if (dwFlag & dwAddFlag)//取出對應值標籤 若無則跳過
		{
			if (USM_ReadData((BYTE*)pDst, nSize, nOffset))
			{
				nOffset += nSize;
				CString strTemp;
				switch (nLogType)
				{
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
	//Log
	theApp.InsertDebugLog(L"ProcessResult", LOG_DEBUG);

	if (USM_ReadData((unsigned char*)&dwFlag, sizeof(dwFlag)))
	{
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
		SetData(SRF_BACK_LEVEL, &xResult.wBackLevel, nWordSize, L"wBackLevel", LOG_WORD);

		SetData(SRF_SIZE_G10, &xResult.wSize_G10, nWordSize, L"wSize_G10", LOG_WORD);
		SetData(SRF_SIZE_G12, &xResult.wSize_G12, nWordSize, L"wSize_G12", LOG_WORD);
		SetData(SRF_SIZE_G14, &xResult.wSize_G14, nWordSize, L"wSize_G14", LOG_WORD);

		SetData(SRF_NUM_A, &xResult.wNum_A, nWordSize, L"wNum_A", LOG_WORD);//OK數量
		SetData(SRF_NUM_P, &xResult.wNum_P, nWordSize, L"wNum_P", LOG_WORD);//NG數量
		SetData(SRF_QUALIFY_RATE, &xResult.wQualifyRate, nWordSize, L"wQualifyRate", LOG_WORD);//Float=>Word

		theApp.InsertDebugLog(strLog, LOG_DEBUG);
		PushResult(xResult);
	}
}

void CALLBACK CEverStrProcessBase::QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer)
{
	if (m_this)
	{
		m_this->ProcessTimer(nEventId);
	}
}
//Timer 刷新數據
void CEverStrProcessBase::ProcessTimer(UINT_PTR nEventId)
{
	for (int i = 0; i < TIMER_MAX; i++)
	{
		if (m_tTimerEvent[i] == nEventId)
		{
			switch (i)
			{
#ifdef USE_TEST_TIMER
				case TIMER_TEST://[測試]上傳寫入
				{
					BATCH_SHARE_SYST_RESULT_EVERSTR xResult;
					memset(&xResult, 0, sizeof(BATCH_SHARE_SYST_RESULT_EVERSTR));
					static int nCount = 0;
					CStringA str;

					xResult.fReal_One_Y = 1.1;
					xResult.fReal_Two_Y = 2.2;
					xResult.fReal_One_X = 3.3;
					xResult.fReal_Two_X = 4.4;

					xResult.wDiff_One_Y = 1;
					xResult.wDiff_Two_Y = 2;
					xResult.wDiff_One_X = 3;
					xResult.wDiff_Two_X = 4;

					xResult.wDiff_One_XY = 5;
					xResult.wDiff_Two_XY = 6;

					xResult.wFrontLevel = 7;
					//str.Format("12345678901234567890123456789a");//8
					//memcpy(xResult.cFrontCode, str.GetBuffer(), _countof(xResult.cFrontCode));
					//xResult.wFrontLocation = 8;
					xResult.wBackLevel = 9;
					//str.Format("abcdefghijklmnopqrstuvwxyzabca");//10
					//memcpy(xResult.cBackCode, str.GetBuffer(), _countof(xResult.cBackCode));
					//xResult.wBackLocation = 10;
					xResult.wSize_G10 = 11;
					xResult.wSize_G12 = 12;
					xResult.wSize_G14 = 13;

					xResult.wResultLevel = 1;
					//xResult.wNum_AA = 2;
					xResult.wNum_A = 3;
					xResult.wNum_P = 4;
					xResult.wQualifyRate = 5;
					xResult.wDiff_XY = 6;

					//BATCH_SHARE_SYST_INFO xInfo;
					//memset(&xInfo, 0, sizeof(xInfo));
					
					//xInfo.xInfo1.cSizeReady = 1;
					//xInfo.xInfo1.cSizeRunning = 1;
					//xInfo.xInfo1.cCCDReady = 1;
					//xInfo.xInfo1.cCCDRunning = 1;
					//xInfo.xInfo2.cCCDError1 = 1;
					//xInfo.xInfo2.cSizeError1 = 1;

					wcscpy_s(xResult.cName, L"ABCDE");
					wcscpy_s(xResult.cAssign, L"BCDEF");
					wcscpy_s(xResult.cMaterial, L"CDEFG");
					

					SET_PLC_FIELD_DATA(FIELD_ORDER_1, sizeof((BYTE*)xResult.cName), (BYTE*)xResult.cName);
					SET_PLC_FIELD_DATA(FIELD_MATERIAL_1, sizeof((BYTE*)xResult.cMaterial), (BYTE*)xResult.cMaterial);
					SET_PLC_FIELD_DATA(FIELD_SN_1, sizeof((BYTE*)xResult.cAssign), (BYTE*)xResult.cAssign);
					
					PushResult(xResult);
					//SetInfoField(xInfo);
				}
				break;
#endif
				case TIMER_COMMAND://0 [指令下發] FIELD_CCL_COMMAND: 0 =>101 
				{
					WORD wOldCommand = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_COMMAND));//前一筆 指令下發
					GET_PLC_FIELD_DATA(FIELD_CCL_COMMAND);//鍵入更新 指令下發
					WORD wCommand = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_COMMAND));//新 指令下發

					if (wOldCommand == 0 && wCommand == CCL_NOTIFYVALUE_COMMAND)//舊命令==0 且 新命令 ==101
					{
#ifdef SHOW_PERFORMANCE
						LARGE_INTEGER xStart, xEnd, xFreq;
						QueryPerformanceFrequency(&xFreq);
						QueryPerformanceCounter(&xStart);
#endif
						for (int i = 0; i < GetFieldSize(); i++)
						{
							PLC_DATA_ITEM_* pItem = GetPLCAddressInfo(i, FALSE);//取前一筆的參數
							if (pItem && pItem->xAction == ACTION_BATCH)//工單下發
							{
								GET_PLC_FIELD_DATA(i);//更新  一筆一筆更新
							}
						}
#ifdef SHOW_PERFORMANCE
						QueryPerformanceCounter(&xEnd);
						double d = (xEnd.QuadPart - xStart.QuadPart) * 1000.0 / xFreq.QuadPart;
						TRACE(L"Read time : %.2f \n", d);
#endif
						ON_CCL_NEWBATCH();  //[更新]下發工單
#ifdef _DEBUG
						//[指令下發回覆]發送 100 給PLC, PLC接收後自行清除 100   23/10/17 試作
						WORD wReceive = 100;
						SET_PLC_FIELD_DATA(FIELD_CCD_COMMAND_RECEIVED, sizeof(WORD), (BYTE*)&wReceive);
#endif
					}
					else if (wOldCommand != 0 && wCommand == CCL_NOTIFYVALUE_COMMAND)//舊命令!=0 且 新命令 ==101
					{
						//log error data
						CString strLog;
						strLog.Format(L"FIELD_CCL_COMMAND previous data error: %d", wOldCommand);
						theApp.InsertDebugLog(strLog, LOG_DEBUG);
					}
					if (wCommand != CCL_NOTIFYVALUE_COMMAND && wCommand != 0)//錯命令
					{
						//log error data
						CString strLog;
						strLog.Format(L"FIELD_CCL_COMMAND data error: %d", wCommand);
						theApp.InsertDebugLog(strLog, LOG_DEBUG);
					}
				}
				break;
				case TIMER_COMMAND_RECEIVED://1 [下發指令收到且回覆]  
				{
					GET_PLC_FIELD_DATA(FIELD_CCD_COMMAND_RECEIVED);//更新 CCD接收 MES  資料完成
				}
				break;
				case TIMER_RESULT://2 檢驗結果
				{
					GET_PLC_FIELD_DATA(FIELD_CCD_RESULT);//鍵入更新 CCD發送檢測結果
					if (GET_FLUSH_ANYWAY())
					{
						::SetEvent(m_hEvent[EV_WRITE]);
					}
					else
					{
						if (GET_PLC_FIELD_VALUE(FIELD_CCD_RESULT) == L"0")//讀值為0
						{	//ready to write
							::SetEvent(m_hEvent[EV_WRITE]);
						}
						else
						{
							CString str;
							str.Format(L"PLC not ready to receive insp result, Field value: %s \n", GET_PLC_FIELD_VALUE(FIELD_CCD_RESULT));
							theApp.InsertDebugLog(str);
						}
					}
					//not yet
				}
				break;
				case TIMER_RESULT_RECEIVED://3 檢驗結果收到
				{
					GET_PLC_FIELD_DATA(FIELD_CCD_RESULT_RECEIVED);//鍵入更新 CCD接收PLC接收檢測結果完成
					if (GET_PLC_FIELD_VALUE(FIELD_CCD_RESULT_RECEIVED) == L"300")
					{
						TRACE(L"PLC has received result! \n");
						PLC_DATA_ITEM_* pResultReceived = GetPLCAddressInfo(FIELD_CCD_RESULT_RECEIVED, FALSE);
						WORD wValue = 0;//接收後清除300
						SET_PLC_FIELD_DATA(FIELD_CCD_RESULT_RECEIVED, sizeof(WORD), (BYTE*)&wValue); //寫入復位
					}
				}
				break;
				case TIMER_C10://4 C10剪切小版編號
				{
					if (GET_PLC_FIELD_ACTION(FIELD_CCL_NO_C10) != ACTION_NOTIFY)
					{
						//僅notify type要啟動timer看C10 index
						::KillTimer(NULL, m_tTimerEvent[TIMER_C10]);
						m_tTimerEvent[TIMER_C10] = NULL;
						continue;
					}
					WORD wOldC10 = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_NO_C10));//前一筆 C10
					GET_PLC_FIELD_DATA(FIELD_CCL_NO_C10);//鍵入更新
					WORD wCurC10 = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_NO_C10));//新 C10
					if (wCurC10 != 0 /*東莞每次變化前會歸零*/ && wOldC10 != wCurC10)//前後不等
					{
						ON_C10_CHANGE(wCurC10);
					}
				}
				break;
				case TIMER_CUSTOM_ACTION:
				{
					DoCustomAction();
				}
				break;
				default:
					break;
			}
		}
	}
}

void CEverStrProcessBase::SetInfoField(BATCH_SHARE_SYST_INFO& xInfo)
{
	theApp.InsertDebugLog(L"Set Info Start", LOG_DEBUG);
	DoSetInfoField(xInfo);
	theApp.InsertDebugLog(L"Set Info End", LOG_DEBUG);
}
///<summary>更新工單回應</summary>
void CEverStrProcessBase::ON_CCL_NEWBATCH()
{
	BATCH_SHARE_SYST_PARAMCCL xData;
	memset(&xData, 0, sizeof(BATCH_SHARE_SYST_PARAMCCL));
	//下發  讀值
	wcscpy_s(xData.cName, GET_PLC_FIELD_VALUE(FIELD_ORDER).GetBuffer());           //訂單號                               1
	wcscpy_s(xData.cMaterial, GET_PLC_FIELD_VALUE(FIELD_MATERIAL).GetBuffer());    //訂單物料代碼                         14
	wcscpy_s(xData.cAssign, GET_PLC_FIELD_VALUE(FIELD_SN).GetBuffer());			   //批號		                         2
	xData.wAssignNum = _ttoi(GET_PLC_FIELD_VALUE(FIELD_QUANTITY));			       //工單產品數量                          3

	xData.wSplitNum = _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLITNUM));                  //一開幾數					             4
	xData.wSplit_One_Y = _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLIT_ONE_Y));            //第一張大小板長			             5 
	xData.wSplit_Two_Y = _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLIT_TWO_Y));            //第二張大小板長			             6
	xData.wSplit_Three_Y = _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLIT_THREE_Y));        //第三張大小板長			             7
	xData.wSplit_One_X = _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLIT_ONE_X));            //第一張大小板寬			             8
	xData.wSplit_Two_X = _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLIT_TWO_X));	           //第二張大小板寬			             9
	xData.wSplit_Three_X = _ttoi(GET_PLC_FIELD_VALUE(FIELD_SPLIT_THREE_X));	       //第三張大小板寬			            10

	xData.wDiff_One_Y_Min = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_Y_MIN));	   //第一個大小版經向公差下限				15
	xData.wDiff_One_Y_Max = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_Y_MAX));	   //第一個大小版經向公差上限				16
	xData.wDiff_One_X_Min = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_X_MIN));	   //第一個大小版緯向公差下限				17
	xData.wDiff_One_X_Max = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_X_MAX));	   //第一個大小版緯向公差上限				18
	xData.wDiff_One_XY_Min = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_XY_MIN));    //第一個大小版對角線公差下限				19
	xData.wDiff_One_XY_Max = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_ONE_XY_MAX));    //第一個大小版對角線公差上限				20

	xData.wDiff_Two_Y_Min = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_TWO_Y_MIN));	   //第二個大小版經向公差下限				21
	xData.wDiff_Two_Y_Max = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_TWO_Y_MAX));	   //第二個大小版經向公差上限				22
	xData.wDiff_Two_X_Min = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_TWO_X_MIN));	   //第二個大小版緯向公差下限				23
	xData.wDiff_Two_X_Max = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_TWO_X_MAX));	   //第二個大小版緯向公差上限				24
	xData.wDiff_Two_XY_Min = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_TWO_XY_MIN));    //第二個大小版對角線公差下限				25
	xData.wDiff_Two_XY_Max = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_TWO_XY_MAX));    //第二個大小版對角線公差上限				26

	xData.wDiff_Three_Y_Min = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_THREE_Y_MIN));  //第三個大小版經向公差下限				27
	xData.wDiff_Three_Y_Max = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_THREE_Y_MAX));  //第三個大小版經向公差上限				28
	xData.wDiff_Three_X_Min = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_THREE_X_MIN));  //第三個大小版緯向公差下限				29
	xData.wDiff_Three_X_Max = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_THREE_X_MAX));  //第三個大小版緯向公差上限				30
	xData.wDiff_Three_XY_Min = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_THREE_XY_MIN));//第三個大小版對角線公差下限				31
	xData.wDiff_Three_XY_Max = _ttoi(GET_PLC_FIELD_VALUE(FIELD_DIFF_THREE_XY_MAX));//第三個大小版對角線公差上限				32

	xData.wNO_C10 = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CCL_NO_C10));  //上傳有需要這邊下發更新嗎

	if (USM_WriteData((BYTE*)&xData, sizeof(xData),0))
	{
		//log data, not yet
		NotifyAOI(WM_SYST_PARAMCCL_CMD, NULL);//更新至AOI(CCD)
	}
}
///<summary>更新C10(一開幾切)回應</summary>
void CEverStrProcessBase::ON_C10_CHANGE(WORD wC10)
{
	CString strLog;
	strLog.Format(L"C10 Index: %d", wC10);
	theApp.InsertDebugLog(strLog, LOG_PLCC10);
	NotifyAOI(WM_SYST_C10CHANGE_CMD, wC10);
}

void CEverStrProcessBase::PushResult(BATCH_SHARE_SYST_RESULT_EVERSTR& xResult)
{
	std::lock_guard< std::mutex > lock(m_oMutex);
	m_vResult.push_back(xResult);
}
DWORD CEverStrProcessBase::Thread_Result(void* pvoid)
{
	CEverStrProcessBase* pThis = (CEverStrProcessBase*)pvoid;
	BOOL bRun = TRUE;
	while (bRun)
	{
		switch (::WaitForMultipleObjects(EV_COUNT, pThis->m_hEvent, FALSE, INFINITE))
		{
			case CASE_WRITE:
			{
				::ResetEvent(pThis->m_hEvent[EV_WRITE]);
				BATCH_SHARE_SYST_RESULT_EVERSTR* pData = NULL;
				static BATCH_SHARE_SYST_RESULT_EVERSTR xData;
				{
					std::lock_guard< std::mutex > lock(pThis->m_oMutex);
					if (pThis->m_vResult.size())
					{
						xData = pThis->m_vResult.at(0);
						pThis->m_vResult.erase(pThis->m_vResult.begin());//消除最前項
						pData = &xData;//取出堆疊最前項
					}
				}
				if (pData)
				{
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
void CEverStrProcessBase::WriteResult(BATCH_SHARE_SYST_RESULT_EVERSTR& xData)
{
#ifdef USE_IN_COMMUNICATOR
	theApp.InsertDebugLog(L"Write Insp start!", LOG_DEBUG);
#endif

	SET_PLC_FIELD_DATA(FIELD_FRONT_LEVEL, 2, (BYTE*)&xData.wFrontLevel);    //表現正面判斷級別(1 = OK,2 = NG) 
	SET_PLC_FIELD_DATA(FIELD_BACK_LEVEL, 2, (BYTE*)&xData.wBackLevel);      //表現反面判斷級別(1 = OK,2 = NG)	

	if (IS_SUPPORT_FLOAT_REALSIZE())
	{
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
	SET_PLC_FIELD_DATA(FIELD_CCD_RESULT, 2, (BYTE*)&wResult);//

#ifdef USE_IN_COMMUNICATOR
	theApp.InsertDebugLog(L"Write Insp done!", LOG_DEBUG);
#endif
}
