#include "stdafx.h"
#include "TechainProcess.h"
#include "PLCCommunicator.h"
#include <map>


CTechainProcess::CTechainProcess()
{
	Init();
}
CTechainProcess::~CTechainProcess()
{
	Finalize();
}
#define TIMER_ID 1
CTechainProcess* CTechainProcess::m_this = NULL;
void CTechainProcess::Init()
{
	m_hMainProcessWnd = ::FindWindow(NULL, TECHAIN_NAME);
	const PLC_DATA_ITEM_ ct_PLC_FIELD[FIELD_MAX] = {
		{ L"Version",					FIELD_VERSION,						PLC_TYPE_WORD,	ACTION_BATCH, 2, L"D",		200 },
		{ L"閥(開關)1",					FIELD_VALVE_SWITCH1,				PLC_TYPE_BIT,	ACTION_BATCH, 2, L"Y",		0 },
		{ L"閥(開關)2",					FIELD_VALVE_SWITCH2,				PLC_TYPE_BIT,	ACTION_BATCH, 2, L"Y",		1 },
		{ L"定位輸出(數位)",			FIELD_ANCHOR_DIGITAL,				PLC_TYPE_BIT,	ACTION_BATCH, 2, L"Y",		2 },
		{ L"電磁閥",					FIELD_ELE,							PLC_TYPE_BIT,	ACTION_BATCH, 2, L"Y",		3 },
		{ L"定位輸出(類比)",			FIELD_ANCHOR_ANALOG,				PLC_TYPE_WORD,	ACTION_BATCH, 2, L"SD",	    6300 },
		{ L"Ch1:閥(數值)1",				FIELD_VALVE_VALUE1,					PLC_TYPE_WORD,	ACTION_BATCH, 2, L"SD",	    6661 },
		{ L"Ch2:閥(數值)2",				FIELD_VALVE_VALUE2,					PLC_TYPE_WORD,	ACTION_BATCH, 2, L"SD",	    6701 },
		{ L"Ch3:測量壓力",				FIELD_PRESSURE,						PLC_TYPE_WORD,	ACTION_BATCH, 2, L"SD",	    6741 },
		{ L"Ch4:測量溫度",				FIELD_TEMPERATURE,					PLC_TYPE_WORD,	ACTION_BATCH, 2, L"SD",	    6781 },
		{ L"Ch5:驅動器壓力",			FIELD_DRIVE,						PLC_TYPE_WORD,	ACTION_BATCH, 2, L"SD",	    7021 },
		{ L"Ch6:開度回饋",				FIELD_FEEDBACK,						PLC_TYPE_WORD,	ACTION_BATCH, 2, L"SD",	    7061 },
		{ L"Ch7:VOC",					FIELD_VOC_1,						PLC_TYPE_WORD,	ACTION_BATCH, 2, L"SD",	    7102 },
		{ L"Ch8:VOC",					FIELD_VOC_2,						PLC_TYPE_WORD,	ACTION_BATCH, 2, L"SD",	    7142 },
	};
	m_pPLC_FIELD_INFO = new PLC_DATA_ITEM_*[FIELD_MAX];
	for (int i = 0; i < FIELD_MAX; i++){
		m_pPLC_FIELD_INFO[i] = new PLC_DATA_ITEM;
		memset(m_pPLC_FIELD_INFO[i], 0, sizeof(PLC_DATA_ITEM));

		memcpy(m_pPLC_FIELD_INFO[i], &ct_PLC_FIELD[i], sizeof(PLC_DATA_ITEM));
	}
	INIT_PLCDATA();
	NotifyMainProcess(WM_SYST_PP_PARAMINIT_CMD, NULL);

	m_this = this;
	m_tTimerEvent = SetTimer(NULL, TIMER_ID, 500, QueryTimer);
}
void CTechainProcess::Finalize()
{
	::KillTimer(NULL, m_tTimerEvent);
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
}
void CALLBACK CTechainProcess::QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer)
{
	if (m_this){
		m_this->ProcessTimer(nEventId);
	}
}
void CTechainProcess::ProcessTimer(UINT_PTR nEventId)
{
	if (m_tTimerEvent == nEventId){
		vector<int> vField = { FIELD_VALVE_VALUE1, FIELD_VALVE_VALUE2, FIELD_PRESSURE, FIELD_TEMPERATURE, FIELD_DRIVE, FIELD_FEEDBACK, FIELD_VOC_1, FIELD_VOC_2 };
		//query field size must be 2 byte
		std::map<int, WORD> mapOld;
		for (auto &i : vField){
			mapOld[i] = *(WORD*)GET_PLC_FIELD_BYTE_VALUE(i);
		}
#ifdef _DEBUG
		LARGE_INTEGER xStart, xEnd, xFreq;
		QueryPerformanceFrequency(&xFreq);
		QueryPerformanceCounter(&xStart);
#endif
		GET_PLC_FIELD_DATA(vField);
#ifdef _DEBUG
		QueryPerformanceCounter(&xEnd);

		double d = (xEnd.QuadPart - xStart.QuadPart) * 1000.0 / xFreq.QuadPart;
		//TRACE(L"query time :%.2f \n", d);
#endif
		for (auto &i : vField){
			WORD wValue = *(WORD*)GET_PLC_FIELD_BYTE_VALUE(i);
			if (mapOld.find(i) != mapOld.end() && mapOld[i] != wValue){
				switch (i){
				/*case FIELD_FEEDBACK_UP:
					if (wValue == 1)
						NotifyMainProcess(WM_TECHAIN_CMD, PTM_FEEDBACK << 24 | wValue);
					break;
				case FIELD_FEEDBACK_DOWN:
					if (wValue == 1)
						NotifyMainProcess(WM_TECHAIN_CMD, PTM_FEEDBACK << 24 | 0);
					break;*/
				case FIELD_VALVE_VALUE1:
					NotifyMainProcess(WM_TECHAIN_CMD, PTM_VALVE_VALUE1 << 24 | (wValue & 0xFFFFFF));
					break;
				case FIELD_VALVE_VALUE2:
					NotifyMainProcess(WM_TECHAIN_CMD, PTM_VALVE_VALUE2 << 24 | (wValue & 0xFFFFFF));
					break;
				case FIELD_PRESSURE:
					NotifyMainProcess(WM_TECHAIN_CMD, PTM_PRESSURE << 24 | (wValue & 0xFFFFFF));
					break;
				case FIELD_TEMPERATURE:
					NotifyMainProcess(WM_TECHAIN_CMD, PTM_TEMPERATURE << 24 | (wValue & 0xFFFFFF));
					break;
				case FIELD_DRIVE:
					NotifyMainProcess(WM_TECHAIN_CMD, PTM_DRIVE << 24 | (wValue & 0xFFFFFF));
					break;
				case FIELD_FEEDBACK:
					NotifyMainProcess(WM_TECHAIN_CMD, PTM_FEEDBACK << 24 | (wValue & 0xFFFFFF));
					break;
				case FIELD_VOC_1:
					NotifyMainProcess(WM_TECHAIN_CMD, PTM_VOC_1 << 24 | (wValue & 0xFFFFFF));
					break;
				case FIELD_VOC_2:
					NotifyMainProcess(WM_TECHAIN_CMD, PTM_VOC_2 << 24 | (wValue & 0xFFFFFF));
					break;
				}
			}
		}
	}
}
void CTechainProcess::NotifyMainProcess(WPARAM wp, LPARAM lp)
{
	if (m_hMainProcessWnd){
		::PostMessage(m_hMainProcessWnd, WM_GPIO_MSG, wp, lp);
	}
}
PLC_DATA_ITEM_* CTechainProcess::GetPLCAddressInfo(int nFieldId, BOOL bSkip)
{
	if (nFieldId >= 0 && nFieldId <= FIELD_MAX){
		return m_pPLC_FIELD_INFO[nFieldId];
	}
	return NULL;
}

void CTechainProcess::SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData)
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
void CTechainProcess::SET_INIT_PARAM(LPARAM lp, BYTE *pData)
{
	if (lp == WM_SYST_PP_PARAMINIT_CMD){
		CString strMsg;
		BATCH_SHARE_SYSTPP_INITPARAM_ *pParam = (BATCH_SHARE_SYSTPP_INITPARAM_*)pData;
		m_nVersion = pParam->nVersion;
		strMsg.Format(L"Version %d", m_nVersion);
		ON_PLC_NOTIFY(strMsg);
	}
}
long CTechainProcess::ON_OPEN_PLC(LPARAM lp)
{
	long lRtn = CPLCProcessBase::ON_OPEN_PLC(lp);
	//notify open result
	if (lRtn == 0){
		//check version
		GET_PLC_FIELD_DATA(FIELD_VERSION);
		int nVersion = _ttoi(GET_PLC_FIELD_VALUE(FIELD_VERSION));
		if (m_nVersion != nVersion){
			//alert main process, not yet
			NotifyMainProcess(WM_TECHAIN_CMD, (PM_VERSION_ERROR << 24 | nVersion));
			CString strLog;
			strLog.Format(L"Version error! PLC Version:%d AOI Version:%d", nVersion, m_nVersion);
			theApp.InsertDebugLog(strLog, LOG_SYSTEM);
		}
		//init valve and anchor
		BYTE *pData = NULL;
		int nSize = 0;
		vector<int> vField = { FIELD_ANCHOR_DIGITAL, FIELD_ELE, FIELD_VALVE_SWITCH1, FIELD_VALVE_SWITCH2 };
		for (auto &i : vField){
			nSize += GetPLCAddressInfo(i, FALSE)->cLen;
		}
		if (nSize) {
			pData = new BYTE[nSize];
			memset(pData, 0, nSize);
		}
		SET_PLC_FIELD_DATA(vField, pData);
		if (pData){
			delete[] pData;
			pData = NULL;
		}

		//get default analog value
		vField = { FIELD_VALVE_VALUE1, FIELD_VALVE_VALUE2, FIELD_PRESSURE, FIELD_TEMPERATURE, FIELD_DRIVE, FIELD_FEEDBACK, FIELD_VOC_1, FIELD_VOC_2 };
		GET_PLC_FIELD_DATA(vField);

		//電磁閥預設on
		WORD wInit = 1;
		SET_PLC_FIELD_DATA(FIELD_ELE, 2, (BYTE*)&wInit);
	}
	return lRtn;
}
void CTechainProcess::ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp)
{
	switch (wp){
	case WM_TECHAIN_CMD:
	{
		PLC_TECHAIN_MESSAGE eMessage = (PLC_TECHAIN_MESSAGE)(lp >> 24);
		int nValue = lp & 0xFFFFFF;
		switch (eMessage){
		//case PTM_START:
		//	ON_PLC_NOTIFY(L"Start Monitor PLC");
		//	if (m_tTimerEvent == NULL){
		//		m_tTimerEvent = SetTimer(NULL, TIMER_ID, 500, QueryTimer);
		//	}
		//	break;
		//case PTM_END:
		//	if (m_tTimerEvent){
		//		::KillTimer(NULL, m_tTimerEvent);
		//		m_tTimerEvent = NULL;
		//	}
		//	ON_PLC_NOTIFY(L"End Monitor PLC");
		//	break;
		case PTM_VALVE_SWITCH1:
			SET_PLC_FIELD_DATA(FIELD_VALVE_SWITCH1, 2, (BYTE*)&nValue);
			break;
		case PTM_VALVE_SWITCH2:
			SET_PLC_FIELD_DATA(FIELD_VALVE_SWITCH2, 2, (BYTE*)&nValue);
			break;
		case PTM_ANCHOR_ANALOG:
			SET_PLC_FIELD_DATA(FIELD_ANCHOR_ANALOG, 2, (BYTE*)&nValue);
			break;
		case PTM_ELE:
			SET_PLC_FIELD_DATA(FIELD_ELE, 2, (BYTE*)&nValue);
			if(SET_PLC_FIELD_DATA(FIELD_ANCHOR_DIGITAL, 2, (BYTE*)&nValue) == 0)
				NotifyMainProcess(WM_TECHAIN_CMD, PTM_ANCHOR_DIGITAL << 24 | (nValue & 0xFFFFFF));
			break;
		default:
			ASSERT(FALSE);
			break;
		}
	}
		break;
	}
}
