#include "stdafx.h"
#include "ScribdPPProcess.h"
#include "PLCCommunicator.h"

CScribdPPProcess* CScribdPPProcess::m_this = NULL;
CScribdPPProcess::CScribdPPProcess()
{
	Init();
}
CScribdPPProcess::~CScribdPPProcess()
{
	Finalize();
}

void CScribdPPProcess::Init()
{
	m_hAOIWnd = ::FindWindow(NULL, QUERYSTATION_NAME);
	NotifyAOI(WM_GPIO_MSG, WM_SYST_PARAMINIT_CMD, NULL);

	m_this = this;
	const PLC_DATA_ITEM_ ctSYST_PLC_FIELD[FIELD_MAX] = {
		{ L"版本號",						FIELD_VERSION,						PLC_TYPE_WORD,	ACTION_BATCH,	2, L"D",	200		},
		{ L"PulseLength.單位(100ms)",		FIELD_PULSE_LENGTH,					PLC_TYPE_WORD,	ACTION_BATCH,	2, L"D",	201		},
		{ L"反應時間(ms)",					FIELD_DELAY_TIME,					PLC_TYPE_WORD,	ACTION_BATCH,	2, L"D",	206		},

		{ L"目前位置",						FIELD_CURRENT_POS,					PLC_TYPE_DWORD, ACTION_NOTIFY,	4, L"SD",	4500	},
		{ L"停機位置-反應距離(正)",			FIELD_PLC_STOPPOS_DELAY_FORWARD,	PLC_TYPE_DWORD, ACTION_NOTIFY,	4, L"D",	207		},
		{ L"停機位置+反應距離(反)",			FIELD_PLC_STOPPOS_DELAY_BACKWARD,	PLC_TYPE_DWORD, ACTION_NOTIFY,	4, L"D",	209		},
		{ L"PLC使用停機位置(正)",			FIELD_PLC_STOPPOS_FORWARD,			PLC_TYPE_DWORD, ACTION_BATCH,	4, L"D",	202		},
		{ L"PLC使用停機位置(反)",			FIELD_PLC_STOPPOS_BACKWARD,			PLC_TYPE_DWORD, ACTION_BATCH,	4, L"D",	204		},
		{ L"下發停機位置(正)",				FIELD_STOPPOS_FORWARD,				PLC_TYPE_DWORD, ACTION_BATCH,	4, L"D",	0		},
		{ L"下發停機位置(反)",				FIELD_STOPPOS_BACKWARD,				PLC_TYPE_DWORD, ACTION_BATCH,	4, L"D",	2		},
		{ L"停機位置(正)下發flag",			FIELD_FLAG_STOPPOS_FORWARD,			PLC_TYPE_WORD,	ACTION_NOTIFY,	2, L"D",	301		},
		{ L"停機位置(正)回傳flag",			FIELD_RESULTFLAG_STOPPOS_FORWARD,	PLC_TYPE_WORD,	ACTION_NOTIFY,	2, L"D",	302		},
		{ L"停機位置(反)下發flag",			FIELD_FLAG_STOPPOS_BACKWARD,		PLC_TYPE_WORD,	ACTION_NOTIFY,	2, L"D",	303		},
		{ L"停機位置(反)回傳flag",			FIELD_RESULTFLAG_STOPPOS_BACKWARD,	PLC_TYPE_WORD,	ACTION_NOTIFY,	2, L"D",	304		},
		
		{ L"停機位置到達flag",				FIELD_FLAG_STOPPOS,					PLC_TYPE_WORD,	ACTION_NOTIFY,	2, L"D",	305		},
		{ L"停機位置到達回傳flag",			FIELD_RESULTFLAG_STOPPOS,			PLC_TYPE_WORD,	ACTION_NOTIFY,	2, L"D",	306		},
		{ L"reset flag",					FIELD_FLAG_RESET,					PLC_TYPE_WORD,	ACTION_NOTIFY,	2, L"D",	307		},
		{ L"reset回傳flag",					FIELD_RESULTFLAG_RESET,				PLC_TYPE_WORD,	ACTION_NOTIFY,	2, L"D",	308		},
		{ L"輸出腳位測試",					FIELD_OUTPUT_PIN,					PLC_TYPE_WORD,	ACTION_BATCH,	2, L"Y",	0		},
		{ L"Y0測試開關",					FIELD_Y0_TEST,						PLC_TYPE_BIT,	ACTION_BATCH,	2, L"M",	506		},
	};
	m_pPLC_FIELD_INFO = new PLC_DATA_ITEM_*[FIELD_MAX];
	for (int i = 0; i < FIELD_MAX; i++){
		m_pPLC_FIELD_INFO[i] = new PLC_DATA_ITEM;
		memset(m_pPLC_FIELD_INFO[i], 0, sizeof(PLC_DATA_ITEM));

		memcpy(m_pPLC_FIELD_INFO[i], &ctSYST_PLC_FIELD[i], sizeof(PLC_DATA_ITEM));

		//mark signed field
		switch (i){
		case FIELD_CURRENT_POS:
		case FIELD_PLC_STOPPOS_FORWARD:
		case FIELD_PLC_STOPPOS_BACKWARD:
		case FIELD_STOPPOS_FORWARD:
		case FIELD_STOPPOS_BACKWARD:
		case FIELD_RESULTFLAG_STOPPOS_FORWARD:
		case FIELD_RESULTFLAG_STOPPOS_BACKWARD:
			m_pPLC_FIELD_INFO[i]->bSigned = TRUE;
			break;
		}
	}
	INIT_PLCDATA();

	for (int i = 0; i < TIMER_MAX; i++){
		m_tTimerEvent[i] = SetTimer(NULL, i, TIMER_INTERVAL, QueryTimer);
	}
}
void CScribdPPProcess::Finalize()
{
	DESTROY_PLC_DATA();
	int nFieldSize = GetFieldSize();
	if (m_pPLC_FIELD_INFO){
		for (int i = 0; i < nFieldSize; i++){
			if (m_pPLC_FIELD_INFO[i]){
				delete[] m_pPLC_FIELD_INFO[i];
				m_pPLC_FIELD_INFO[i] = NULL;
			}
		}
		delete[] m_pPLC_FIELD_INFO;
	}
	for (int i = 0; i < TIMER_MAX; i++){
		::KillTimer(NULL, m_tTimerEvent[i]);
	}
}
PLC_DATA_ITEM_* CScribdPPProcess::GetPLCAddressInfo(int nFieldId, BOOL bSkip)
{
	if (nFieldId >= 0 && nFieldId <= FIELD_MAX){
		return m_pPLC_FIELD_INFO[nFieldId];
	}
	return NULL;
}
void CALLBACK CScribdPPProcess::QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer)
{
	if (m_this){
		m_this->ProcessTimer(nEventId);
	}
}
void CScribdPPProcess::ProcessTimer(UINT_PTR nEventId)
{
	CString strLog;

	for (int i = 0; i < TIMER_MAX; i++){
		if (m_tTimerEvent[i] == nEventId){
			switch (i)
			{
			case TIMER_CHECK:
				GET_PLC_FIELD_DATA(FIELD_PLC_STOPPOS_DELAY_FORWARD);
				GET_PLC_FIELD_DATA(FIELD_PLC_STOPPOS_DELAY_BACKWARD);
				GET_PLC_FIELD_DATA(FIELD_FLAG_STOPPOS_FORWARD);
				GET_PLC_FIELD_DATA(FIELD_FLAG_STOPPOS_BACKWARD);
				GET_PLC_FIELD_DATA(FIELD_FLAG_STOPPOS);
				GET_PLC_FIELD_DATA(FIELD_FLAG_RESET);
				GET_PLC_FIELD_DATA(FIELD_RESULTFLAG_STOPPOS);
				GET_PLC_FIELD_DATA(FIELD_OUTPUT_PIN);
				GET_PLC_FIELD_DATA(FIELD_Y0_TEST);
				break;
			case TIMER_CURRENT_POS:
			{
				int nOld = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CURRENT_POS));
				GET_PLC_FIELD_DATA(FIELD_CURRENT_POS);
				int nNew = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CURRENT_POS));

				if (nOld != nNew){
					//notify 
					NotifyAOI(WM_GPIO_MSG, PLC_CHANGE_POS, nNew);
				}
			}
			break;
			case TIMER_SET_STOPPOS_FORWARD:
			case TIMER_SET_STOPPOS_BACKWARD:
			{
				int nFieldId = 0, nCheckField = 0;
				CString strDirection;
				if (i == TIMER_SET_STOPPOS_FORWARD){
					nFieldId = FIELD_RESULTFLAG_STOPPOS_FORWARD;
					nCheckField = FIELD_PLC_STOPPOS_FORWARD;
					strDirection = L"forward";
				}
				else if (i == TIMER_SET_STOPPOS_BACKWARD){
					nFieldId = FIELD_RESULTFLAG_STOPPOS_BACKWARD;
					nCheckField = FIELD_PLC_STOPPOS_BACKWARD;
					strDirection = L"backward";
				}
				GET_PLC_FIELD_DATA(nFieldId); // get forward/backward stop pos result flag
				short nCommand = _ttoi(GET_PLC_FIELD_VALUE(nFieldId));
				if (nCommand != 0){
					if (nCommand == SET_STOPPOS_SUCCESS){
						strLog.Format(L"set %s stop pos success!", strDirection);
						GET_PLC_FIELD_DATA(nCheckField); //check destination field
					}
					else if (nCommand == SET_STOPPOS_FAIL){
						strLog.Format(L"set %s stop pos fail!", strDirection);
					}
					else{
						//error, log it
						strLog.Format(L"set %s stop pos error %d!", strDirection);
					}
					if (nFieldId == FIELD_RESULTFLAG_STOPPOS_FORWARD)
						NotifyAOI(WM_GPIO_MSG, PLC_STOPPOS_FORWARD, nCommand);
					else if (nFieldId == FIELD_RESULTFLAG_STOPPOS_BACKWARD)
						NotifyAOI(WM_GPIO_MSG, PLC_STOPPOS_BACKWARD, nCommand);

					TRACE(L"%s \n", strLog);
					theApp.InsertDebugLog(strLog);
					WORD wRtn = 0;
					SET_PLC_FIELD_DATA(nFieldId, 2, (BYTE*)&wRtn);
				}
			}
			break;
			case TIMER_STOPPOS:
			{
				GET_PLC_FIELD_DATA(FIELD_FLAG_STOPPOS);
				WORD wCommand = _ttoi(GET_PLC_FIELD_VALUE(FIELD_FLAG_STOPPOS));
				if (wCommand != 0){
					if (wCommand == REACH_STOPPOS_FORWARD){
						strLog.Format(L"reach forward stop pos!");
					}
					else if (wCommand == REACH_STOPPOS_BACKWARD){
						strLog.Format(L"reach backward stop pos!");
					}
					else{
						//error, log it
						strLog.Format(L"reach stop pos error! %d", wCommand);
					}
					if (wCommand == REACH_STOPPOS_FORWARD)
						NotifyAOI(WM_GPIO_MSG, PLC_REACH_STOPPOS_FORWARD, 0);
					else if (wCommand == REACH_STOPPOS_FORWARD)
						NotifyAOI(WM_GPIO_MSG, PLC_REACH_STOPPOS_BACKWARD, 0);

					TRACE(L"%s \n", strLog);
					theApp.InsertDebugLog(strLog);
					WORD wRtn = 0, wFlag = RECEIVE_STOPPOS;
					SET_PLC_FIELD_DATA(FIELD_FLAG_STOPPOS, 2, (BYTE*)&wRtn);
					SET_PLC_FIELD_DATA(FIELD_RESULTFLAG_STOPPOS, 2, (BYTE*)&wFlag);
				}
			}
			break;
			case TIMER_RESET:
			{
				GET_PLC_FIELD_DATA(FIELD_RESULTFLAG_RESET);
				WORD wCommand = _ttoi(GET_PLC_FIELD_VALUE(FIELD_RESULTFLAG_RESET));
				if (wCommand != 0){
					if (wCommand == RESET_ALL_SUCCESS){
						strLog.Format(L"reset success!");
						NotifyAOI(WM_GPIO_MSG, PLC_RESET, wCommand);
					}
					else{
						strLog.Format(L"reset error! %d", wCommand);
						NotifyAOI(WM_PLC_ERROR, PLC_RESET, wCommand);
					}
					TRACE(L"%s \n", strLog);
					theApp.InsertDebugLog(strLog);
					WORD wRtn = 0;
					SET_PLC_FIELD_DATA(FIELD_RESULTFLAG_RESET, 2, (BYTE*)&wRtn);
				}
			}
			break;
			}
		}
	}
}
long CScribdPPProcess::ON_OPEN_PLC(LPARAM lp)
{
	long lRtn = CPLCProcessBase::ON_OPEN_PLC(lp); 
	//notify open result
	if (lRtn == 0)
		NotifyAOI(WM_GPIO_MSG, PLC_OPEN, lRtn);
	else
		NotifyAOI(WM_PLC_ERROR, PLC_OPEN, lRtn);

	return lRtn;
}
void CScribdPPProcess::ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp)
{
	long lRtn = 0, lRtn2 = 0;
	CString strLog;
	switch (wp){
	case PLC_STOPPOS_FORWARD:
		lRtn = SET_PLC_FIELD_DATA(FIELD_STOPPOS_FORWARD, 4, (BYTE*)&lp);
		if (lRtn == 0){
			WORD wFlag = SET_STOPPOS;
			lRtn2 = SET_PLC_FIELD_DATA(FIELD_FLAG_STOPPOS_FORWARD, 2, (BYTE*)&wFlag);
		}
		if (lRtn != 0)
			NotifyAOI(WM_PLC_ERROR, PLC_STOPPOS_FORWARD_FIELDERROR, lRtn);
		else if (lRtn2 != 0)
			NotifyAOI(WM_PLC_ERROR, PLC_STOPPOS_FORWARD_FLAGERROR, lRtn2);

		strLog.Format(L"set forward stop pos: %d, %d %d", lp, lRtn == 0, lRtn2 == 0);
		theApp.InsertDebugLog(strLog);
		break;
	case PLC_STOPPOS_BACKWARD:
		lRtn = SET_PLC_FIELD_DATA(FIELD_STOPPOS_BACKWARD, 4, (BYTE*)&lp);
		if (lRtn == 0){
			WORD wFlag = SET_STOPPOS;
			lRtn2 = SET_PLC_FIELD_DATA(FIELD_FLAG_STOPPOS_BACKWARD, 2, (BYTE*)&wFlag);
		}
		if (lRtn != 0)
			NotifyAOI(WM_PLC_ERROR, PLC_STOPPOS_BACKWARD_FIELDERROR, lRtn);
		else if (lRtn2 != 0)
			NotifyAOI(WM_PLC_ERROR, PLC_STOPPOS_BACKWARD_FLAGERROR, lRtn2);
		strLog.Format(L"set backward stop pos: %d, %d %d", lp, lRtn == 0, lRtn2 == 0);
		theApp.InsertDebugLog(strLog);
		break;
	case PLC_RESET:
	{
		WORD wValue = RESET_ALL;
		lRtn = SET_PLC_FIELD_DATA(FIELD_FLAG_RESET, 2, (BYTE*)&wValue);
		strLog.Format(L"reset address %d", lRtn == 0);
		theApp.InsertDebugLog(strLog);
	}
		break;
	case PLC_CURRENT_POS:
	{
		lRtn = GET_PLC_FIELD_DATA(FIELD_CURRENT_POS);
		if (lRtn == 0){
			int nPos = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CURRENT_POS));
			NotifyAOI(WM_GPIO_MSG, PLC_CURRENT_POS, nPos); //notify on success
			strLog.Format(L"Get Current Pos %d", nPos);
			theApp.InsertDebugLog(strLog);
		}
		else
			NotifyAOI(WM_PLC_ERROR, PLC_CURRENT_POS, lRtn);
	}
		break;
	case PLC_PULSELENGTH:
		lRtn = SET_PLC_FIELD_DATA(FIELD_PULSE_LENGTH, 2, (BYTE*)&lp);
		strLog.Format(L"set pulse length %d, %d", lp, lRtn == 0);
		theApp.InsertDebugLog(strLog);
		break;
	case PLC_TEST_PIN:
		lRtn = SET_PLC_FIELD_DATA(FIELD_OUTPUT_PIN, 2, (BYTE*)&lp);
		for (int i = 0; i < MAX_PUTPUT_PIN; i++){
			if (i == STOP_PIN){ //輸出腳位由梯形圖邏輯控制,  需透過測試開關才能變化
				BOOL bHigh = (1 << i) & lp;
				SET_PLC_FIELD_DATA(FIELD_Y0_TEST, 2, (BYTE*)&bHigh);
				break;
			}
		}
		break;
	case PLC_VERSION:
		lRtn = GET_PLC_FIELD_DATA(FIELD_VERSION);
		if (lRtn == 0){
			int nVersion = _ttoi(GET_PLC_FIELD_VALUE(FIELD_VERSION));
			NotifyAOI(WM_GPIO_MSG, wp, nVersion); 
			strLog.Format(L"Get version %d", nVersion);
			theApp.InsertDebugLog(strLog);
		}
		else{
			NotifyAOI(WM_PLC_ERROR, wp, lRtn);
		}
		break;
	case PLC_DELAY_TIME:
		lRtn = SET_PLC_FIELD_DATA(FIELD_DELAY_TIME, 2, (BYTE*)&lp);
		strLog.Format(L"set delay time %d, %d", lp, lRtn == 0);
		theApp.InsertDebugLog(strLog);
		break;
	}
	if (lRtn != 0){
		//write fail: notify AOI immediately. if success, wait for PLC return flag
		NotifyAOI(WM_PLC_ERROR, wp, lRtn);
	}
}
void CScribdPPProcess::SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData)
{
	if (pParam){
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
		pParam->put_ActTimeOut(0x100);							//100ms timeout
		pParam->put_ActUnitNumber(0x00);

		pParam->put_ActUnitType(UNIT_FXVETHER);
	}
}
void CScribdPPProcess::NotifyAOI(UINT uMsg, WPARAM wp, LPARAM lp)
{
	if (m_hAOIWnd){
		::PostMessage(m_hAOIWnd, uMsg, wp, lp);
	}
}