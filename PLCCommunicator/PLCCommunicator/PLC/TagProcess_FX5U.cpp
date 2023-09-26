#include "stdafx.h"
#include "TagProcess_FX5U.h"
#include "PLCCommunicator.h"
#ifdef _DEBUG
const int ctWatchDogInterval = 3 * 1000; //write field every x seconds
#else
const int ctWatchDogInterval = 1 * 1000; //write field every second
#endif
const int ctWatchDogCountDown = 3600;//PLC firmware數到3600就會發出訊號

#define QUERY_INTERVAL 500


CTagProcess_FX5U* CTagProcess_FX5U::m_this = NULL;
CTagProcess_FX5U::CTagProcess_FX5U()
{
	Init();
}
CTagProcess_FX5U::~CTagProcess_FX5U()
{
	Finalize();
}
void CTagProcess_FX5U::Init()
{
	m_wOutputQueryFlag = 0;
	m_dwLastEncoder = 0;
	memset(&m_xParam, 0, sizeof(m_xParam));
	m_xParam.nWatchDogTimeOut = ctWatchDogCountDown;
	m_uCurEncoder = 0;
	m_this = this;
	const PLC_DATA_ITEM_ ctSYST_PLC_FIELD[FIELD_MAX] =
	{
		{ L"Version",					FIELD_VERSION,						PLC_TYPE_WORD,	ACTION_BATCH,  2, L"D", 0 },
		{ L"WatchDog",					FIELD_WATCHDOG,						PLC_TYPE_DWORD, ACTION_BATCH,  4, L"D", 1 },
		{ L"停止檢測",					FIELD_STOPINSP,						PLC_TYPE_DWORD, ACTION_BATCH,  4, L"D", 3 },
		{ L"Encoder",					FIELD_ENCODER,						PLC_TYPE_DWORD, ACTION_BATCH,  4, L"D", 6 }, //FX5U無法直接讀取Buffer memory, PLC定時copy encoder到D5
		{ L"X點位觸發Flag",				FIELD_INPUT_FLAG,					PLC_TYPE_WORD,	ACTION_BATCH,  2, L"D", 10 },
		{ L"Y06:卷狀停機Flag",			FIELD_Y06FLAG,						PLC_TYPE_BIT,	ACTION_NOTIFY, 2, L"D", 217, 15, 15 },
		{ L"Y07:片狀剔除Flag",			FIELD_Y07FLAG,						PLC_TYPE_BIT,	ACTION_NOTIFY, 2, L"D", 218, 15, 15 },
		{ L"Y11:貼標機Flag",			    FIELD_Y11FLAG,						PLC_TYPE_BIT,	ACTION_NOTIFY, 2, L"D", 220, 15, 15 },
		{ L"卷狀停機_反應時間",			FIELD_INFO_Y06_RESPONSE_TIME,		PLC_TYPE_WORD,	ACTION_BATCH,  2, L"D", 111 },
		{ L"卷狀停機_持續時間",			FIELD_INFO_Y06_DURATION,			PLC_TYPE_WORD,	ACTION_BATCH,  2, L"D", 112 },
		{ L"卷狀停機_StartEncoder",		FIELD_INFO_Y06_START,				PLC_TYPE_DWORD,	ACTION_BATCH,  4, L"D", 114 },
		{ L"卷狀停機_EndEncoder",		    FIELD_INFO_Y06_END,					PLC_TYPE_DWORD,	ACTION_BATCH,  4, L"D", 116 },
		{ L"片狀剔除_反應時間",			FIELD_INFO_Y07_RESPONSE_TIME,		PLC_TYPE_WORD,	ACTION_BATCH,  2, L"D", 121 },
		{ L"片狀剔除_持續時間",			FIELD_INFO_Y07_DURATION,			PLC_TYPE_WORD,	ACTION_BATCH,  2, L"D", 122 },
		{ L"片狀剔除_StartEncoder",		FIELD_INFO_Y07_START,				PLC_TYPE_DWORD,	ACTION_BATCH,  4, L"D", 124 },
		{ L"片狀剔除_EndEncoder",		    FIELD_INFO_Y07_END,					PLC_TYPE_DWORD,	ACTION_BATCH,  4, L"D", 126 },
		{ L"貼標機_反應時間",			    FIELD_INFO_Y11_RESPONSE_TIME,		PLC_TYPE_WORD,	ACTION_BATCH,  2, L"D", 131 },
		{ L"貼標機_持續時間",			    FIELD_INFO_Y11_DURATION,			PLC_TYPE_WORD,	ACTION_BATCH,  2, L"D", 132 },
		{ L"貼標機_StartEncoder",		FIELD_INFO_Y11_START,				PLC_TYPE_DWORD,	ACTION_BATCH,  4, L"D", 134 },
		{ L"貼標機_EndEncoder",			FIELD_INFO_Y11_END,					PLC_TYPE_DWORD,	ACTION_BATCH,  4, L"D", 136 },
		{ L"卷狀停機參數",				FIELD_PARAM_Y06,					PLC_TYPE_WORD,	ACTION_BATCH,  2, L"D", 217 },
		{ L"片狀剔除參數",				FIELD_PARAM_Y07,					PLC_TYPE_WORD,	ACTION_BATCH,  2, L"D", 218 },
		{ L"貼標機參數",				    FIELD_PARAM_Y11,					PLC_TYPE_WORD,	ACTION_BATCH,  2, L"D", 220 },
		{ L"Encoder(A軸X1)",			    FIELD_ENCODER_X01,					PLC_TYPE_DWORD,	ACTION_BATCH,  4, L"D", 16 },
		{ L"Encoder(B軸X2)",			    FIELD_ENCODER_X02,					PLC_TYPE_DWORD,	ACTION_BATCH,  4, L"D", 18 },
		{ L"Encoder(拖白布X3)",			FIELD_ENCODER_X03,					PLC_TYPE_DWORD,	ACTION_BATCH,  4, L"D", 20 },
		{ L"Encoder(裁片訊號X4)",		    FIELD_ENCODER_X04,					PLC_TYPE_DWORD,	ACTION_BATCH,  4, L"D", 22 },
		{ L"Encoder(片狀NewBatch X5)",	FIELD_ENCODER_X05,					PLC_TYPE_DWORD,	ACTION_BATCH,  4, L"D", 24 },
		{ L"Encoder(過布街頭X11)",		FIELD_ENCODER_X11,					PLC_TYPE_DWORD,	ACTION_BATCH,  4, L"D", 32 },
	};
	m_pPLC_FIELD_INFO = new PLC_DATA_ITEM_ * [FIELD_MAX];
	for (int i = 0; i < FIELD_MAX; i++) {
		m_pPLC_FIELD_INFO[i] = new PLC_DATA_ITEM;
		memset(m_pPLC_FIELD_INFO[i], 0, sizeof(PLC_DATA_ITEM));

		memcpy(m_pPLC_FIELD_INFO[i], &ctSYST_PLC_FIELD[i], sizeof(PLC_DATA_ITEM));
	}
	INIT_PLCDATA();

	for (int i = 0; i < TIMER_MAX; i++) {
		if (i == TIMER_WATCHDOG)
			m_tTimerEvent[i] = SetTimer(NULL, i, ctWatchDogInterval, QueryTimer);
		else
			m_tTimerEvent[i] = SetTimer(NULL, i, QUERY_INTERVAL, QueryTimer);
	}

	NotifyAOI(WM_SYST_PP_PARAMINIT_CMD, NULL);
}
long CTagProcess_FX5U::ON_OPEN_PLC(LPARAM lp)
{
	long lRtn = CPLCProcessBase::ON_OPEN_PLC(lp);
	//notify open result
	if (lRtn == 0) {
		TriggerWatchDog(FALSE, TRUE);//reset watch dog output at initial

		//check version
		GET_PLC_FIELD_DATA(FIELD_VERSION);
		int nVersion = _ttoi(GET_PLC_FIELD_VALUE(FIELD_VERSION));
		if (m_xParam.nVersion != nVersion) {
			//alert AOI, not yet
			NotifyAOI(WM_PLC_PP_CMD, (PM_VERSION_ERROR << 24 | nVersion));
			CString strLog;
			strLog.Format(L"Version error! PLC Version:%d AOI Version:%d", nVersion, m_xParam.nVersion);
			theApp.InsertDebugLog(strLog, LOG_SYSTEM);
		}
	}

	return lRtn;
}

void CTagProcess_FX5U::Finalize()
{
	for (int i = 0; i < TIMER_MAX; i++) {
		::KillTimer(NULL, m_tTimerEvent[i]);
	}
	DESTROY_PLC_DATA();
	int nFieldSize = GetFieldSize();
	if (m_pPLC_FIELD_INFO) {
		for (int i = 0; i < nFieldSize; i++) {
			if (m_pPLC_FIELD_INFO[i]) {
				delete[] m_pPLC_FIELD_INFO[i];
				m_pPLC_FIELD_INFO[i] = NULL;
			}
		}
		delete[] m_pPLC_FIELD_INFO;
	}
}
void CALLBACK CTagProcess_FX5U::QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer)
{
	if (m_this) {
		m_this->ProcessTimer(nEventId);
	}
}
void CTagProcess_FX5U::ProcessTimer(UINT_PTR nEventId)
{
	for (int i = 0; i < TIMER_MAX; i++) {
		if (m_tTimerEvent[i] == nEventId) {
			switch (i)
			{
			case TIMER_WATCHDOG:
			{
				//if AOI exist and slave checkalive ok, then reset PLC watchdog timer
				HWND hWnd = ::FindWindow(NULL, AOI_MASTER_NAME);
				if (hWnd) {
					TriggerWatchDog(FALSE, FALSE);
				}
			}
			break;
			case TIMER_QUERY:
			{
				vector<int> vField = { FIELD_ENCODER, FIELD_INPUT_FLAG,/*FIELD_Y06FLAG, FIELD_Y07FLAG, FIELD_Y11FLAG*/ };
				//需確認6/7/11 pin是距離或時間模式才需要定時查flag狀態
				for (int j = 0; j < PIN_MAX; j++) {
					if (m_wOutputQueryFlag & 1 << j) {
						switch (j) {
						case PIN_6:
							vField.push_back(FIELD_Y06FLAG);
							break;
						case PIN_7:
							vField.push_back(FIELD_Y07FLAG);
							break;
						case PIN_11:
							vField.push_back(FIELD_Y11FLAG);
							break;
						}
					}
				}
				BYTE* pLastEncoder = GET_PLC_FIELD_BYTE_VALUE(FIELD_ENCODER);
				if (pLastEncoder)
					memcpy(&m_dwLastEncoder, pLastEncoder, sizeof(m_dwLastEncoder));

				GET_PLC_FIELD_DATA(vField);

				for (auto& j : vField) {
					BYTE* pCur = GET_PLC_FIELD_BYTE_VALUE(j);
					switch (j) {
					case FIELD_ENCODER:
						if (memcmp(pCur, &m_dwLastEncoder, sizeof(m_dwLastEncoder)) != 0)
							NotifyAOI(WM_PLC_ENCODER_POS_CMD, *(DWORD*)pCur);
						break;
					case FIELD_INPUT_FLAG:
						ProcessInputFlag(*(int*)pCur);
						break;
					case FIELD_Y06FLAG:
					case FIELD_Y07FLAG:
					case FIELD_Y11FLAG:
						if ((*(WORD*)pCur & 0xF) != 1) {
							CheckPLCField();
						}
						break;
					}
				}
			}
			break;
			}
		}
	}
}
void CTagProcess_FX5U::ProcessInputFlag(int nValue)
{
	for (int j = 0; j < NOTIFY_MAX; j++) {
		if (nValue & (1 << j)) {
			int nField = 0;
			BOOL bNotify = TRUE;
			INPUT_PIN_NOTIFY xNotify;
			memset(&xNotify, 0, sizeof(INPUT_PIN_NOTIFY));
			switch (j) {
			case NOTIFY_BIT_X1:
				nField = FIELD_ENCODER_X01;
				xNotify.nPin0Base = 1;
				break;
			case NOTIFY_BIT_X2:
				nField = FIELD_ENCODER_X02;
				xNotify.nPin0Base = 2;
				break;
			case NOTIFY_BIT_X3:
				nField = FIELD_ENCODER_X03;
				xNotify.nPin0Base = 3;
				break;
			case NOTIFY_BIT_X4:
				nField = FIELD_ENCODER_X04;
				xNotify.nPin0Base = 4;
				break;
			case NOTIFY_BIT_X5:
				nField = FIELD_ENCODER_X05;
				xNotify.nPin0Base = 5;
				break;
			case NOTIFY_BIT_X6:
				//暫停檢測不需encoder
				xNotify.nPin0Base = 6;
				break;
			case NOTIFY_BIT_X11:
				nField = FIELD_ENCODER_X11;
				xNotify.nPin0Base = 11;
				break;
			default:
				bNotify = FALSE;
				break;
			}
			if (nField != 0) {
				GET_PLC_FIELD_DATA(nField);

				//reset flag
				WORD dw = 0;
				SET_PLC_FIELD_DATA_BIT(FIELD_INPUT_FLAG, j, FALSE);
			}
			if (bNotify) {
				//notify AOI
				xNotify.dwEncoder = _ttoi(GET_PLC_FIELD_VALUE(nField));
				if (USM_WriteData((BYTE*)&xNotify, sizeof(xNotify))) {
					NotifyAOI(WM_PLC_INPUT_CMD, NULL);
				}
			}

		}
	}
}
void CTagProcess_FX5U::SET_INIT_PARAM(LPARAM lp, BYTE* pData)
{
	switch (lp) {
	case WM_SYST_PP_PARAMINIT_CMD:
	{
		CString strMsg;
		BATCH_SHARE_SYSTPP_INITPARAM_* pParam = (BATCH_SHARE_SYSTPP_INITPARAM_*)pData;
		m_xParam.nWatchDogTimeOut = pParam->nWatchDogTimeout;
		m_xParam.nVersion = pParam->nVersion;
		strMsg.Format(L"WatchDog %d", m_xParam.nWatchDogTimeOut);
		ON_PLC_NOTIFY(strMsg);
		strMsg.Format(L"Version %d", m_xParam.nVersion);
		ON_PLC_NOTIFY(strMsg);
	}
	break;
	}
}
void CTagProcess_FX5U::ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp)
{
	switch (wp) {
		//notify aoi to get pin information, then set to PLC address
	case WM_PLC_OUTPUTPIN_CMD:
		ProcessOutputPin(lp);
		break;
	case WM_ASSIGN_ENCODER_CMD:
		AssignEncoderPos();
		break;
	}
}
int CTagProcess_FX5U::ModeToBit(OUTPUT_MODE eMode)
{
	switch (eMode) {
	case MODE_NOUSE:
		return 0;
	case MODE_DIRECTIO:
		return 1;
	case MODE_TIME:
		return 2;
	case MODE_DIST:
		return 3;
	default:
		ASSERT(FALSE);
		return -1;
	}
}
void CTagProcess_FX5U::ProcessOutputPin(int nCount)  //not yet
{
	int nTotalSize = nCount * sizeof(OUTPUT_PIN_INFO);
	OUTPUT_PIN_INFO* pData = new OUTPUT_PIN_INFO[nCount];
	memset(pData, 0, nTotalSize);
	if (USM_ReadData((BYTE*)pData, nTotalSize)) {
		for (int i = 0; i < nCount; i++) {
			OUTPUT_PIN_INFO* pInfo = (pData + i);
			OUTPUT_PIN_INFO* pOld = NULL;

			for (auto& j : m_vOutPin) {
				if (j.eType == pInfo->eType) {
					pOld = &j;
					break;
				}
			}
			if (pOld == NULL) {
				m_vOutPin.push_back(*pInfo);
			}

			int nPinField = 0, nTimeField = 0, nDurationField = 0;
			BOOL bProcess = TRUE;
			OUTPUT_PIN_QUERY_FLAG eFlag;
			switch (pInfo->eType) {
			case TYPE_STOP:
				nPinField = FIELD_PARAM_Y06;
				nTimeField = FIELD_INFO_Y06_RESPONSE_TIME;
				nDurationField = FIELD_INFO_Y06_DURATION;
				eFlag = PIN_6;
				break;
			case TYPE_SPLIT:
				nPinField = FIELD_PARAM_Y07;
				nTimeField = FIELD_INFO_Y07_RESPONSE_TIME;
				nDurationField = FIELD_INFO_Y07_DURATION;
				eFlag = PIN_7;
				break;
			case TYPE_TAG:
				nPinField = FIELD_PARAM_Y11;
				nTimeField = FIELD_INFO_Y11_RESPONSE_TIME;
				nDurationField = FIELD_INFO_Y11_DURATION;
				eFlag = PIN_11;
				break;
			default:
				ASSERT(FALSE);
				bProcess = FALSE;
				break;
			}
			if (bProcess) {
				if (pOld) {
					//update mode: reset old mode then set new mode and other information
					int nOldBit = ModeToBit(pOld->eMode);
					if (nPinField != 0 && nOldBit != -1)
						SET_PLC_FIELD_DATA_BIT(nPinField, nOldBit, FALSE); //reset old mode

					memcpy(pOld, pInfo, sizeof(OUTPUT_PIN_INFO));
				}

				int nNewBit = ModeToBit(pInfo->eMode); //set new mode
				if (nPinField != 0 && nNewBit != -1)
					SET_PLC_FIELD_DATA_BIT(nPinField, nNewBit, TRUE); //set new mode

				if (nTimeField != 0) {
					SET_PLC_FIELD_DATA(nTimeField, 2, (BYTE*)&pInfo->nResponse);	//set response time
				}
				if (nDurationField != 0) {
					SET_PLC_FIELD_DATA(nDurationField, 2, (BYTE*)&pInfo->nDuration); //set duration
				}

				if (pInfo->eMode == MODE_TIME || pInfo->eMode == MODE_DIST) //mark query flag
					m_wOutputQueryFlag |= (1 << eFlag);
				else
					m_wOutputQueryFlag ^= (1 << eFlag);
			}
		}
	}
	if (pData)
		delete[] pData;
}
void CTagProcess_FX5U::AssignEncoderPos()
{
	ENCODER_POS xPos;
	memset(&xPos, 0, sizeof(xPos));
	if (USM_ReadData((BYTE*)&xPos, sizeof(xPos))) {
		std::lock_guard< std::mutex > lock(m_oMutex);
		m_vEncoderPos.push_back(xPos);
		//check flag
		CheckPLCField();

	}
}
void CTagProcess_FX5U::CheckPLCField()
{
	std::lock_guard< std::mutex > lock(m_oMutex);
	if (m_vEncoderPos.size()) {
		ENCODER_POS& xPos = m_vEncoderPos.front();

		int nFlagField = 0, nStartField = 0, nEndField = 0;
		BOOL bProcess = TRUE;
		switch (xPos.eType) {
		case TYPE_STOP:
			nFlagField = FIELD_Y06FLAG;
			nStartField = FIELD_INFO_Y06_START;
			nEndField = FIELD_INFO_Y06_END;
			break;
		case TYPE_SPLIT:
			nFlagField = FIELD_Y07FLAG;
			nStartField = FIELD_INFO_Y07_START;
			nEndField = FIELD_INFO_Y07_END;
			break;
		case TYPE_TAG:
			nFlagField = FIELD_Y11FLAG;
			nStartField = FIELD_INFO_Y11_START;
			nEndField = FIELD_INFO_Y11_END;
			break;
		default:
			ASSERT(FALSE);
			bProcess = FALSE;
			break;
		}
		OUTPUT_PIN_INFO* pInfo = NULL;
		for (auto& i : m_vOutPin) {
			if (i.eType == xPos.eType) {
				pInfo = &i;
				break;
			}
		}

		if (bProcess && pInfo) {
			int nValue = _ttoi(GET_PLC_FIELD_VALUE(nFlagField));
			if ((nValue & 0xF) != 1) {
				DWORD dwDiff = *(DWORD*)GET_PLC_FIELD_BYTE_VALUE(FIELD_ENCODER) - m_dwLastEncoder;
				double dSpeed = dwDiff * 1.0 / QUERY_INTERVAL; //encoder / ms
				int nResponseDis = pInfo->nResponse * dSpeed; //拿速度和反應時間計算反應距離

				DWORD dwStartPos = xPos.dwStartEncoder - nResponseDis;
				SET_PLC_FIELD_DATA(nStartField, 4, (BYTE*)&dwStartPos);

				DWORD dwEndPos = xPos.dwEndEncoder - nResponseDis;
				SET_PLC_FIELD_DATA(nEndField, 4, (BYTE*)&dwEndPos);

				SET_PLC_FIELD_DATA_BIT(nFlagField, 15, TRUE);//mark flag
				//log it
				CString strLog;
				strLog.Format(L"assign encdoer pos, start:%u end:%u", dwStartPos, dwEndPos);
				theApp.InsertDebugLog(strLog, LOG_SYSTEM);
				m_vEncoderPos.erase(m_vEncoderPos.begin());
			}
		}
	}
}
void CTagProcess_FX5U::TriggerWatchDog(BOOL bOutput, BOOL bLog)// TRUE:直接觸發PLC發送watchdog邏輯, FALSE:reset PLC watchdog timer
{
	if (bOutput) {
		DWORD dw = 1;
		SET_PLC_FIELD_DATA(FIELD_WATCHDOG, 4, (BYTE*)&dw);
		if (bLog) theApp.InsertDebugLog(L"Output WatchDog signal", LOG_SYSTEM);
	}
	else {
		DWORD dw = (ctWatchDogCountDown - m_xParam.nWatchDogTimeOut) << 16;
		SET_PLC_FIELD_DATA(FIELD_WATCHDOG, 4, (BYTE*)&dw);
		if (bLog) theApp.InsertDebugLog(L"reset WatchDog signal", LOG_SYSTEM);
	}
}
void CTagProcess_FX5U::SetMXParam(IActProgType* pParam, BATCH_SHARE_SYSTCCL_INITPARAM& xData)
{
	if (pParam) {
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
PLC_DATA_ITEM_* CTagProcess_FX5U::GetPLCAddressInfo(int nFieldId, BOOL bSkip)
{
	if (nFieldId >= 0 && nFieldId <= FIELD_MAX) {
		return m_pPLC_FIELD_INFO[nFieldId];
	}
	return NULL;
}
