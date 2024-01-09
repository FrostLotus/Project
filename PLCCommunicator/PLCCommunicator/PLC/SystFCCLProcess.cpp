#include "stdafx.h"
#include "SystFCCLProcess.h"
#define COMMAND_VALUE	2
#ifdef _DEBUG
const int ctBASE_ADDRESS = 10;
#else
const int ctBASE_ADDRESS = 9000;
#endif
CSystFCCLProcess* CSystFCCLProcess::m_this = NULL;
CSystFCCLProcess::CSystFCCLProcess()
{
	Init();
}
CSystFCCLProcess::~CSystFCCLProcess()
{
	Finalize();
}
void CSystFCCLProcess::Init()
{
	NotifyAOI(WM_SYST_PARAMINIT_CMD, NULL);
	m_this = this;
	const PLC_DATA_ITEM_ ctSYST_PLC_FIELD[FIELD_MAX] = {
	{ L"指令下發",				FIELD_COMMAND,					PLC_TYPE_WORD,		ACTION_NOTIFY, 2,	L"D", 0 },

	{ L"工單",					FIELD_ORDER,					PLC_TYPE_STRING,	ACTION_BATCH, 12,	L"D", ctBASE_ADDRESS + 0 },
	{ L"料號",					FIELD_MATERIAL,					PLC_TYPE_STRING,	ACTION_BATCH, 18,	L"D", ctBASE_ADDRESS + 10 },
	
	}; 
	m_pPLC_FIELD_INFO = new PLC_DATA_ITEM_*[FIELD_MAX];
	for (int i = 0; i < FIELD_MAX; i++){
		m_pPLC_FIELD_INFO[i] = new PLC_DATA_ITEM;
		memset(m_pPLC_FIELD_INFO[i], 0, sizeof(PLC_DATA_ITEM));

		memcpy(m_pPLC_FIELD_INFO[i], &ctSYST_PLC_FIELD[i], sizeof(PLC_DATA_ITEM));
	}
	INIT_PLC_DATA();

	for (int i = 0; i < TIMER_MAX; i++){
		m_tTimerEvent[i] = SetTimer(NULL, i, TIMER_INTERVAL, QueryTimer);
	}
}
void CSystFCCLProcess::Finalize()
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
PLC_DATA_ITEM_* CSystFCCLProcess::GetPLCAddressInfo(int nFieldId, BOOL bSkip)
{
	if (nFieldId >= 0 && nFieldId <= FIELD_MAX){
		return m_pPLC_FIELD_INFO[nFieldId];
	}
	return NULL;
}
void CALLBACK CSystFCCLProcess::QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer)
{
	if (m_this){
		m_this->ProcessTimer(nEventId);
	}
}
void CSystFCCLProcess::ProcessTimer(UINT_PTR nEventId)
{
	for (int i = 0; i < TIMER_MAX; i++){
		if (m_tTimerEvent[i] == nEventId){
			switch (i)
			{
			case TIMER_COMMAND:
			{
				WORD wOldCommand = _ttoi(GET_PLC_FIELD_VALUE(FIELD_COMMAND));
				GET_PLC_FIELD_DATA(FIELD_COMMAND);
				WORD wCommand = _ttoi(GET_PLC_FIELD_VALUE(FIELD_COMMAND));

				if (wOldCommand == 0 && wCommand == COMMAND_VALUE){
					for (int i = 0; i < GetFieldSize(); i++){
						PLC_DATA_ITEM_ *pItem = GetPLCAddressInfo(i, FALSE);
						if (pItem && pItem->xAction == ACTION_BATCH){
							GET_PLC_FIELD_DATA(i);
						}
					}
					ON_FCCL_NEWBATCH();
				}
			}
			break;
			}
		}
	}
}
void CSystFCCLProcess::ON_FCCL_NEWBATCH()
{
	BATCH_SHARE_SYST_BASE xData;
	memset(&xData, 0, sizeof(xData));
	wcscpy_s(xData.cName, GET_PLC_FIELD_VALUE(FIELD_ORDER));
	wcscpy_s(xData.cMaterial, GET_PLC_FIELD_VALUE(FIELD_MATERIAL));


	if (USM_WriteData((BYTE*)&xData, sizeof(xData))){
		//log data, not yet
		NotifyAOI(WM_SYST_PARAMWEBCOOPER_CMD, NULL);
	}
}
void CSystFCCLProcess::SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData)
{
	if (pParam){
#ifdef _DEBUG
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
#else
		//參考MX_componentV4_Program Manaual 4.3.7設定
		pParam->put_ActCpuType(CPU_Q13UDEHCPU);
		pParam->put_ActConnectUnitNumber(0x00);
		pParam->put_ActDestinationIONumber(0x00);				//固定為0
		pParam->put_ActDestinationPortNumber(5007);				//固定為5007
		pParam->put_ActDidPropertyBit(0x01);					//固定為1
		pParam->put_ActDsidPropertyBit(0x01);					//固定為1
		pParam->put_ActIntelligentPreferenceBit(0x00);			//固定為0
		pParam->put_ActIONumber(0x3FF);							//單CPU時, 固定為0x3FF
		pParam->put_ActMultiDropChannelNumber(0x00);			//固定為0
		pParam->put_ActNetworkNumber(xData.lTargetNetworkNo);	//物件站側模組網路No, 0
		pParam->put_ActStationNumber(xData.lTargetStationNo);	//物件站側模組站號, 0xFF
		pParam->put_ActThroughNetworkType(0x00);
		pParam->put_ActTimeOut(0x100);							//100ms timeout
		pParam->put_ActUnitNumber(0x00);						//固定為0
		pParam->put_ActUnitType(UNIT_QNETHER);
#endif
	}
}