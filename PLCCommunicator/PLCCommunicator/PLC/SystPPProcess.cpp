#include "stdafx.h"
#include "SystPPProcess.h"
#include "PLCCommunicator.h"
CSystPPProcess* CSystPPProcess::m_this = NULL;
#ifdef _DEBUG
const int ctHungTime = 5000; //wait xx sec for system hung
const int ctWatchDogInterval = 3 * 1000; //write field every x seconds
#else
const int ctHungTime = 15000; //wait 15 sec for system hung
const int ctWatchDogInterval = 1 * 1000; //write field every second
#endif
const int ctWatchDogCountDown = 3600;//PLC firmware數到3600就會發出訊號



UINT THREAD_WATCHDOG(LPVOID lp)
{
	CSystPPProcess* pThis = (CSystPPProcess*)lp;

	pThis->DoWatchDogCheck();

	return 0;
}
void CSystPPProcess::DoWatchDogCheck()
{
	while (!m_bExit)
	{
		//if AOI exist and slave checkalive ok, then reset PLC watchdog timer
		HWND hWnd = ::FindWindow(NULL, AOI_MASTER_NAME);
		if (hWnd)
		{
			LRESULT nResult = ::SendMessageTimeout(hWnd, WM_NULL, 0, 0, SMTO_NORMAL, ctHungTime, NULL);
			if (nResult != 0)
			{ //send ok
				TriggerWatchDog(FALSE, FALSE);
			}
			else
			{//AOI hung
				TriggerWatchDog(TRUE, TRUE);
			}
		}
		else
		{
			SetInspStatus(NULL, TRUE);
		}
		::Sleep(ctWatchDogInterval);
	}
}
CSystPPProcess::CSystPPProcess()
{
	Init();
}
CSystPPProcess::~CSystPPProcess()
{
	Finalize();
}
void CSystPPProcess::Init()
{
	m_bExit = FALSE;
	memset(&m_xParam, 0, sizeof(m_xParam));
	m_xParam.nWatchDogTimeOut = ctWatchDogCountDown;

	m_this = this;
	const PLC_DATA_ITEM_ ctSYST_PLC_FIELD[FIELD_MAX] =
	{
		{ L"WatchDog",						FIELD_WATCHDOG,						PLC_TYPE_DWORD,	ACTION_BATCH,	4, L"D",	0			},
		{ L"卷片切換",						FIELD_SWITCH_SHEET_WEB,				PLC_TYPE_BIT,	ACTION_BATCH,	2, L"M",	1			},//0:卷/1:片
		{ L"卷片電位模式",					FIELD_WS_POTENTIAL,					PLC_TYPE_BIT,	ACTION_BATCH,	2, L"M",	3,			},
		{ L"版本",							FIELD_VERSION,						PLC_TYPE_WORD,	ACTION_BATCH,	2, L"D",	200			},
		{ L"A軸",							FIELD_A_AXIS,						PLC_TYPE_BIT,	ACTION_BATCH,	2, L"D",	201, 1, 1	},
		{ L"B軸",							FIELD_B_AXIS,						PLC_TYPE_BIT,	ACTION_BATCH,	2, L"D",	201, 2, 2	},
		{ L"片狀NewBatch",					FIELD_SHEET_NEWBATCH,				PLC_TYPE_BIT,	ACTION_BATCH,	2, L"D",	201, 5, 5	},
		{ L"非檢測中",						FIELD_INSP_STOP,					PLC_TYPE_BIT,	ACTION_BATCH,	2, L"Y",	0	},
		{ L"檢測中",						    FIELD_INSP_START,					PLC_TYPE_BIT,	ACTION_BATCH,	2, L"Y",	1	},
	};
	m_pPLC_FIELD_INFO = new PLC_DATA_ITEM_ * [FIELD_MAX];
	for (int i = 0; i < FIELD_MAX; i++)
	{
		m_pPLC_FIELD_INFO[i] = new PLC_DATA_ITEM;
		memset(m_pPLC_FIELD_INFO[i], 0, sizeof(PLC_DATA_ITEM));
		memcpy(m_pPLC_FIELD_INFO[i], &ctSYST_PLC_FIELD[i], sizeof(PLC_DATA_ITEM));
	}
	for (int i = 0; i < TIMER_MAX; i++)
	{
		//if (i == TIMER_WATCHDOG)
		//	m_tTimerEvent[i] = SetTimer(NULL, i, ctWatchDogInterval, QueryTimer);
		//else
		m_tTimerEvent[i] = SetTimer(NULL, i, 500, QueryTimer);
	}
	NotifyAOI(WM_SYST_PP_PARAMINIT_CMD, NULL);
}
void CSystPPProcess::SetInspStatus(BOOL bSuspend, BOOL bClearAll)
{
	WORD wStop = 0;
	WORD wStart = 0;
	CString strLog;
	if (bClearAll)
	{
		wStop = 0;
		wStart = 0;

		strLog.Format(L"clear all insp pin");
	}
	else
	{
		strLog.Format(L"change Insp status:%d", bSuspend);

		if (bSuspend)
		{
			wStop = 1;
			wStart = 0;
		}
		else
		{
			wStop = 0;
			wStart = 1;
		}
	}
	theApp.InsertDebugLog(strLog, LOG_SYSTEM);

	SET_PLC_FIELD_DATA(FIELD_INSP_STOP, 2, (BYTE*)&wStop);
	SET_PLC_FIELD_DATA(FIELD_INSP_START, 2, (BYTE*)&wStart);
}
void CSystPPProcess::Finalize()
{
	m_bExit = TRUE;
	for (int i = 0; i < TIMER_MAX; i++)
	{
		::KillTimer(NULL, m_tTimerEvent[i]);
	}

	DESTROY_PLC_DATA();
	int nFieldSize = GetFieldSize();
	if (m_pPLC_FIELD_INFO)
	{
		for (int i = 0; i < nFieldSize; i++)
		{
			if (m_pPLC_FIELD_INFO[i])
			{
				delete[] m_pPLC_FIELD_INFO[i];
				m_pPLC_FIELD_INFO[i] = NULL;
			}
		}
		delete[] m_pPLC_FIELD_INFO;
	}
}
void CALLBACK CSystPPProcess::QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer)
{
	if (m_this)
	{
		m_this->ProcessTimer(nEventId);
	}
}
UINT THREAD_NEWBATCH(LPVOID lp)
{
	std::pair<CSystPPProcess*, int>* pPair = (std::pair<CSystPPProcess*, int>*)lp;
	if (pPair)
	{
		::Sleep(pPair->first->GetNewbatchDelay());
		pPair->first->DoPLCNewbatch(pPair->second);
		delete pPair;
	}
	return 0;
}
void CSystPPProcess::ProcessTimer(UINT_PTR nEventId)
{
	for (int i = 0; i < TIMER_MAX; i++)
	{
		if (m_tTimerEvent[i] == nEventId)
		{
			switch (i)
			{
				//case TIMER_WATCHDOG:
				//{
				//	//if AOI exist and slave checkalive ok, then reset PLC watchdog timer
				//	HWND hWnd = ::FindWindow(NULL, AOI_MASTER_NAME);
				//	if (hWnd){
				//		TriggerWatchDog(FALSE, FALSE);
				//	}
				//	else{
				//		SetInspStatus(NULL, TRUE);
				//	}			
				//}
				//	break;
				case TIMER_INPUT:
					OnQueryTimer();
					break;
			}
		}
	}
}
void CSystPPProcess::OnQueryTimer()
{
	vector<int> vField = { FIELD_SWITCH_SHEET_WEB, FIELD_A_AXIS, FIELD_B_AXIS, FIELD_SHEET_NEWBATCH };
	int nOldSwitch = _ttoi(GET_PLC_FIELD_VALUE(FIELD_SWITCH_SHEET_WEB));

	GET_PLC_FIELD_DATA(vField);

	for (auto& i : vField)
	{
		int nCur = _ttoi(GET_PLC_FIELD_VALUE(i));
		switch (i)
		{
			case FIELD_A_AXIS:
			case FIELD_B_AXIS:
			case FIELD_SHEET_NEWBATCH:
				if (nCur != 0)
				{
					if (m_xParam.nNewbatchDelay == 0)
					{
						DoPLCNewbatch(i);
					}
					else
					{
						CString strLog;
						strLog.Format(L"newbatch delay %d ms ", m_xParam.nNewbatchDelay);
						theApp.InsertDebugLog(strLog, LOG_SYSTEM);
						std::pair<CSystPPProcess*, int>* pPair = new std::pair<CSystPPProcess*, int>(this, i);
						AfxBeginThread(THREAD_NEWBATCH, pPair);
					}

					//if status change, reset status
					WORD wValue = 0;
					SET_PLC_FIELD_DATA(i, 2, (BYTE*)&wValue);
				}
				break;
			case FIELD_SWITCH_SHEET_WEB:
				if (nOldSwitch != nCur)
				{
					//notify AOI
					NotifyAOI(WM_PLC_PP_CMD, (PM_SWITCH_WEB_SHEET << 24) | nCur);
					TRACE(L"notify aoi switch %d to %d\n", nOldSwitch, nCur);
				}
				break;
		}
	}
}
void CSystPPProcess::DoPLCNewbatch(int nField)
{
	//notify AOI
	if (nField == FIELD_A_AXIS)
	{
		NotifyAOI(WM_PLC_PP_CMD, (PM_A_AXIS << 24));
		theApp.InsertDebugLog(L"A Axis Notify!", LOG_SYSTEM);
	}
	else if (nField == FIELD_B_AXIS)
	{
		NotifyAOI(WM_PLC_PP_CMD, (PM_B_AXIS << 24));
		theApp.InsertDebugLog(L"B Axis Notify!", LOG_SYSTEM);
	}
	else if (nField == FIELD_SHEET_NEWBATCH)
	{
		NotifyAOI(WM_PLC_PP_CMD, (PM_SHEET_NEWBATCH << 24));
		theApp.InsertDebugLog(L"Sheet Newbatch!", LOG_SYSTEM);
	}
}
void CSystPPProcess::SetMXParam(IActProgType* pParam, BATCH_SHARE_SYSTCCL_INITPARAM& xData)
{
	if (m_xParam.bFX5U)
	{
		//參考sh081085engr.pdf 4.2設定
		//Ethernet communication when the connected station is FX5CPU(TCP)
		pParam->put_ActBaudRate(0x00);
		pParam->put_ActControl(0x00);
		pParam->put_ActCpuType(CPU_FX5UCPU);
		pParam->put_ActDataBits(0x00);
		pParam->put_ActDestinationIONumber(0x00);
		pParam->put_ActDestinationPortNumber(5562);
		pParam->put_ActDidPropertyBit(0x01);
		pParam->put_ActDsidPropertyBit(0x01);
		pParam->put_ActIntelligentPreferenceBit(0x00);
		pParam->put_ActIONumber(0x3FF);
		pParam->put_ActNetworkNumber(0x00);
		pParam->put_ActPacketType(0x01);
		pParam->put_ActPortNumber(0x00);
		pParam->put_ActProtocolType(PROTOCOL_TCPIP);
		pParam->put_ActStationNumber(0xFF);
		pParam->put_ActStopBits(0x00);
		pParam->put_ActSumCheck(0x00);
		pParam->put_ActThroughNetworkType(0x01);
		pParam->put_ActTimeOut(3000);							//100ms timeout
		pParam->put_ActUnitNumber(0x00);

		pParam->put_ActUnitType(UNIT_FXVETHER);
	}
	else
	{
#ifdef _DEBUG
		pParam->put_ActHostAddress(L"192.168.2.99");
#endif
		//pParam->put_ActCpuType(CPU_FX5UCPU);
		//pParam->put_ActUnitType(UNIT_FXETHER);
	}
}
PLC_DATA_ITEM_* CSystPPProcess::GetPLCAddressInfo(int nFieldId, BOOL bSkip)
{
	if (nFieldId >= 0 && nFieldId <= FIELD_MAX)
	{
		return m_pPLC_FIELD_INFO[nFieldId];
	}
	return NULL;
}
CPU_SERIES CSystPPProcess::GetCPU()
{
	if (m_xParam.bFX5U)
		return CPU_SERIES::FX5U_SERIES;
	else
		return CPU_SERIES::FX3U_SERIES;
}
void CSystPPProcess::SET_INIT_PARAM(LPARAM lp, BYTE* pData)
{
	if (lp == WM_SYST_PP_PARAMINIT_CMD)
	{
		INIT_PLCDATA();

		CString strMsg;
		BATCH_SHARE_SYSTPP_INITPARAM_* pParam = (BATCH_SHARE_SYSTPP_INITPARAM_*)pData;
		m_xParam.nWatchDogTimeOut = pParam->nWatchDogTimeout;
		m_xParam.nVersion = pParam->nVersion;
		m_xParam.nWSMode = pParam->nWSMode;
		m_xParam.bFX5U = pParam->bFX5U;
		m_xParam.nNewbatchDelay = pParam->nNewbatchDelay;

		strMsg.Format(L"WatchDog %d", m_xParam.nWatchDogTimeOut);
		theApp.InsertDebugLog(strMsg, LOG_SYSTEM);
		ON_PLC_NOTIFY(strMsg);

		strMsg.Format(L"Version %d", m_xParam.nVersion);
		theApp.InsertDebugLog(strMsg, LOG_SYSTEM);
		ON_PLC_NOTIFY(strMsg);

		strMsg.Format(L"WSMode %d", m_xParam.nWSMode);
		theApp.InsertDebugLog(strMsg, LOG_SYSTEM);
		ON_PLC_NOTIFY(strMsg);

		strMsg.Format(L"FX5U %d", m_xParam.bFX5U);
		theApp.InsertDebugLog(strMsg, LOG_SYSTEM);
		ON_PLC_NOTIFY(strMsg);

		strMsg.Format(L"NewbatchDelay %d", m_xParam.nNewbatchDelay);
		theApp.InsertDebugLog(strMsg, LOG_SYSTEM);
		ON_PLC_NOTIFY(strMsg);
	}
}
void CSystPPProcess::ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp)
{
	switch (wp)
	{
		case WM_WS_POTENTIAL_CMD:
		{
			CString strLog;
			strLog.Format(L"change WS mode to %d", lp);
			theApp.InsertDebugLog(strLog, LOG_SYSTEM);
			SET_PLC_FIELD_DATA(FIELD_WS_POTENTIAL, 2, (BYTE*)&lp);
		}
		break;
		case WM_INSPSTATUS_CMD:
			SetInspStatus(lp, FALSE);
			break;
	}
}
void CSystPPProcess::TriggerWatchDog(BOOL bOutput, BOOL bLog)// TRUE:直接觸發PLC發送watchdog邏輯, FALSE:reset PLC watchdog timer
{
	if (bOutput)
	{
		DWORD dw = 1;
		SET_PLC_FIELD_DATA(FIELD_WATCHDOG, 4, (BYTE*)&dw);
		if (bLog) theApp.InsertDebugLog(L"Output WatchDog signal", LOG_SYSTEM);
	}
	else
	{
		DWORD dw = (ctWatchDogCountDown - m_xParam.nWatchDogTimeOut) << 16;
		SET_PLC_FIELD_DATA(FIELD_WATCHDOG, 4, (BYTE*)&dw);
		if (bLog) theApp.InsertDebugLog(L"reset WatchDog signal", LOG_SYSTEM);
	}
}
long CSystPPProcess::ON_OPEN_PLC(LPARAM lp)
{
	long lRtn = CPLCProcessBase::ON_OPEN_PLC(lp);
	//notify open result
	if (lRtn == 0)
	{
		TriggerWatchDog(FALSE, TRUE);//reset watch dog output at initial
		DWORD dwValue = 0;
		vector<int> vField = { FIELD_A_AXIS, FIELD_B_AXIS, FIELD_SHEET_NEWBATCH };
		SET_PLC_FIELD_DATA(vField, (BYTE*)&dwValue); //reset cache status for A/B 

		//check version
		GET_PLC_FIELD_DATA(FIELD_VERSION);
		int nVersion = _ttoi(GET_PLC_FIELD_VALUE(FIELD_VERSION));
		if (m_xParam.nVersion != nVersion)
		{
			//alert aoi
			NotifyAOI(WM_PLC_PP_CMD, (PM_VERSION_ERROR << 24 | nVersion));
			CString strLog;
			strLog.Format(L"Version error! PLC Version:%d AOI Version:%d", nVersion, m_xParam.nVersion);
			theApp.InsertDebugLog(strLog, LOG_SYSTEM);
		}

		//query current web/sheet status
		GET_PLC_FIELD_DATA(FIELD_SWITCH_SHEET_WEB);
		int nCur = _ttoi(GET_PLC_FIELD_VALUE(FIELD_SWITCH_SHEET_WEB));
		NotifyAOI(WM_PLC_PP_CMD, (PM_SWITCH_WEB_SHEET << 24) | nCur);


		SET_PLC_FIELD_DATA(FIELD_WS_POTENTIAL, 2, (BYTE*)&m_xParam.nWSMode);

		//request current status
		NotifyAOI(WM_PLC_PP_CMD, (PM_CURRENT_INSPSTATUS << 24));

		AfxBeginThread(THREAD_WATCHDOG, this);
	}
	return lRtn;
}
