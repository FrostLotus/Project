#include "stdafx.h"
#include "SystPPProcess.h"
#include "OPCCommunicator.h"
#include "DataHandlerBase.h"

#define COMMAND_TIMER_INTERVAL 3000 //wait for 3 second to new batch

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
	for (int i = NULL; i < EV_COUNT; i++) m_hEvent[i] = ::CreateEvent(NULL, TRUE, FALSE, NULL);
	m_hThread = ::CreateThread(NULL, NULL, Thread_Process, this, NULL, NULL);

	struct FIELD_ITEM{
		int nFieldId;
		TCHAR strName[100];
		TCHAR strNodeId[100];
		TCHAR strValue[100];
		int nLen; //inBytes
		int nType;
		time_t xTime;
	};
	const FIELD_ITEM ctSYST_FIELD[FIELD_MAX] = {
#ifdef _DEBUG
		{ FIELD_ORDER_NO,		L"订单号",			L"Order_No",		L"0123456789ab",			24,  UA_TYPES_STRING,	0 },
		{ FIELD_ORDER_MATERIAL, L"订单物料",		    L"Order_Material",	L"01234567890123456789",	40,  UA_TYPES_STRING,	0 },
		{ FIELD_ORDER_QUATITY,	L"订单数量",		    L"Order_Quatity",	L"1",						2,   UA_TYPES_UINT16,	0 },
		{ FIELD_FACTORY,		L"客戶代碼",		    L"Factory",			L"012345678901234567",		36,  UA_TYPES_STRING,	0 },
		{ FIELD_STATION,		L"生产机台",		    L"Station",			L"123456",					12,  UA_TYPES_STRING,	0 },
		{ FIELD_GLUE_TYPE,		L"胶水类型",		    L"Glue_Type",		L"123456",					12,  UA_TYPES_STRING,	0 },
		{ FIELD_WEB_SPEC,		L"玻璃布发票号",	    L"Web_Spec",		L"012345678901234567",		36,  UA_TYPES_STRING,	0 },
		{ FIELD_WEB_FACTORY,	L"玻璃布廠家",		L"Web_Factory",		L"123456",					12,  UA_TYPES_STRING,	0 },
		{ FIELD_WEB_TYPE,		L"玻璃布布種",		L"Web_Type",		L"123456",					12,  UA_TYPES_STRING,	0 },
		{ FIELD_INSP,			L"檢測設定",		L"Insp",			L"Insp123456",				20,	 UA_TYPES_STRING,	0 },
		{ FIELD_LIGHT,			L"光源設定",		L"Light",			L"Light12345",				20,	 UA_TYPES_STRING,	0 },
		{ FIELD_LOT_NO,			L"Lot.Num",			L"Lot_No",			L"0123456789",				20,	 UA_TYPES_STRING,	0 },
#else
		{ FIELD_ORDER_NO,		L"订单号",			L"Order_No",		L"",				24,  UA_TYPES_STRING,	0 },
		{ FIELD_ORDER_MATERIAL, L"订单物料",		L"Order_Material",	L"",				40,  UA_TYPES_STRING,	0 },
		{ FIELD_ORDER_QUATITY,	L"订单数量",		L"Order_Quatity",	L"",				2,   UA_TYPES_UINT16,	0 },
		{ FIELD_FACTORY,		L"客戶代碼",		L"Factory",			L"",				36,  UA_TYPES_STRING,	0 },
		{ FIELD_STATION,		L"生产机台",		L"Station",			L"",				12,  UA_TYPES_STRING,	0 },
		{ FIELD_GLUE_TYPE,		L"胶水类型",		L"Glue_Type",		L"",				12,  UA_TYPES_STRING,	0 },
		{ FIELD_WEB_SPEC,		L"玻璃布发票号",	L"Web_Spec",		L"",				36,  UA_TYPES_STRING,	0 },
		{ FIELD_WEB_FACTORY,	L"玻璃布廠家",		L"Web_Factory",		L"",				12,  UA_TYPES_STRING,	0 },
		{ FIELD_WEB_TYPE,		L"玻璃布布種",		L"Web_Type",		L"",				12,  UA_TYPES_STRING,	0 },
		{ FIELD_INSP,			L"檢測設定",		L"Insp",			L"",				20,	 UA_TYPES_STRING,	0 },
		{ FIELD_LIGHT,			L"光源設定",		L"Light",			L"",				20,	 UA_TYPES_STRING,	0 },
		{ FIELD_LOT_NO,			L"Lot.Num",			L"Lot_No",			L"",				20,	 UA_TYPES_STRING,	0 },
#endif
	};
	TRACE(L"thread id%u \n", GetCurrentThreadId());
	m_ppFIELD_INFO = new NodeItem*[FIELD_MAX];
	for (int i = 0; i < FIELD_MAX; i++){
		m_ppFIELD_INFO[i] = new NodeItem;
		memset(m_ppFIELD_INFO[i], 0, sizeof(NodeItem));

		//memcpy(m_ppFIELD_INFO[i], &ctSYST_FIELD[i], sizeof(NodeItem));
		m_ppFIELD_INFO[i]->nFieldId = ctSYST_FIELD[i].nFieldId;
		memcpy(m_ppFIELD_INFO[i]->strName, ctSYST_FIELD[i].strName, sizeof(ctSYST_FIELD[i].strName));
		memcpy(m_ppFIELD_INFO[i]->strNodeId, ctSYST_FIELD[i].strNodeId, sizeof(ctSYST_FIELD[i].strNodeId));
		m_ppFIELD_INFO[i]->nLen = ctSYST_FIELD[i].nLen;
		m_ppFIELD_INFO[i]->nType = ctSYST_FIELD[i].nType;
		m_ppFIELD_INFO[i]->xTime = ctSYST_FIELD[i].xTime;

		m_ppFIELD_INFO[i]->pValue = new BYTE[ctSYST_FIELD[i].nLen];
		memset(m_ppFIELD_INFO[i]->pValue, 0, ctSYST_FIELD[i].nLen);
#ifdef _DEBUG
		switch (ctSYST_FIELD[i].nType){
		case UA_TYPES_STRING:
			memcpy(m_ppFIELD_INFO[i]->pValue, ctSYST_FIELD[i].strValue, ctSYST_FIELD[i].nLen);
			break;
		case UA_TYPES_BOOLEAN:
			*(bool*)m_ppFIELD_INFO[i]->pValue = _ttoi(ctSYST_FIELD[i].strValue);
			break;
		case UA_TYPES_UINT16:
			*(short*)m_ppFIELD_INFO[i]->pValue = _ttoi(ctSYST_FIELD[i].strValue);
			break;
		}
#endif
	}
}
void CSystPPProcess::Finalize()
{
	::SetEvent(m_hEvent[EV_EXIT]);

	::WaitForSingleObject(m_hThread, INFINITE);

	int nFieldSize = GetNodeSize();
	if (m_ppFIELD_INFO){
		for (int i = 0; i < nFieldSize; i++){
			if (m_ppFIELD_INFO[i]){
				delete[] m_ppFIELD_INFO[i]->pValue;
				delete[] m_ppFIELD_INFO[i];
				m_ppFIELD_INFO[i] = NULL;
			}
		}
		delete[] m_ppFIELD_INFO;
	}
}
NodeItem *CSystPPProcess::GetNodeItem(int nIndex0Base)
{
	if (nIndex0Base >= 0 && nIndex0Base < FIELD_MAX){
		return m_ppFIELD_INFO[nIndex0Base];
	}
	return NULL;
}
void CSystPPProcess::UpdateNode(UA_String &uaData, const UA_DataValue *pValue)
{
	int nTypeKind = pValue->value.type->typeKind;
	for (int i = 0; i < GetNodeSize(); i++){
		NodeItem* pItem = GetNodeItem(i);
		if (pItem && pItem->strNodeId == CString((char*)uaData.data, (int)uaData.length)){
			switch (nTypeKind){
			case UA_DATATYPEKIND_STRING:
			{
				UA_String uaData = *((UA_String*)pValue->value.data);
				CString strData((char *)uaData.data, (int)uaData.length);
				CString strOld((TCHAR*)pItem->pValue);
				
				if (strOld != strData){
					memset(pItem->pValue, 0, pItem->nLen);

					int nCopySize = (strData.GetLength() + 1) * sizeof(TCHAR);
					if (nCopySize > pItem->nLen) nCopySize = pItem->nLen;

					memcpy(pItem->pValue, strData.GetBuffer(), nCopySize);
					TRACE(L"ServerCallback uaData.length=%i, Data:%s \n", uaData.length, strData);

					if (pItem->nFieldId == FIELD_LOT_NO){
						theApp.InsertDebugLog(L"Order No change! wait 3 sec to new batch"); //wait for other field get ready
						::SetEvent(m_hEvent[EV_NEWBATCH]);
					}
				}
			}
			break;
			case UA_DATATYPEKIND_UINT16:
			{
				UA_Int16 wData = *(UA_Int16 *)pValue->value.data;
				*(short*)pItem->pValue = wData;
			}
			break;
			case UA_DATATYPEKIND_BOOLEAN:
			{
				UA_Boolean bData = *(UA_Boolean*)pValue->value.data;
				*(bool*)pItem->pValue = bData;
			}
				break;
			default:
				ASSERT(FALSE);
				break;
			}
			pItem->xTime = CTime::GetCurrentTime().GetTime();
			ON_OPC_FIELD_CHANGE(pItem->nFieldId); //notify UI change

			CString strLog;
			strLog.Format(L"Field:%s change to : %s", GetNodeId(pItem->nFieldId), GetNodeValue(pItem->nFieldId));
			theApp.InsertDebugLog(strLog, LOG_OPC);
			break;
		}
	}
}
void CSystPPProcess::ProcessNewBatch()
{
	OPCNewBatch xData;
	memset(&xData, 0, sizeof(OPCNewBatch));
	memcpy(xData.strOrder_No, m_ppFIELD_INFO[FIELD_ORDER_NO]->pValue, m_ppFIELD_INFO[FIELD_ORDER_NO]->nLen);
	memcpy(xData.strOrder_Material, m_ppFIELD_INFO[FIELD_ORDER_MATERIAL]->pValue, m_ppFIELD_INFO[FIELD_ORDER_MATERIAL]->nLen);
	xData.nOrder_Quatity = *(short*)m_ppFIELD_INFO[FIELD_ORDER_QUATITY]->pValue;
	memcpy(xData.strFactory, m_ppFIELD_INFO[FIELD_FACTORY]->pValue, m_ppFIELD_INFO[FIELD_FACTORY]->nLen);
	memcpy(xData.strStation, m_ppFIELD_INFO[FIELD_STATION]->pValue, m_ppFIELD_INFO[FIELD_STATION]->nLen);
	memcpy(xData.strGlueType, m_ppFIELD_INFO[FIELD_GLUE_TYPE]->pValue, m_ppFIELD_INFO[FIELD_GLUE_TYPE]->nLen);
	memcpy(xData.strWebSpec, m_ppFIELD_INFO[FIELD_WEB_SPEC]->pValue, m_ppFIELD_INFO[FIELD_WEB_SPEC]->nLen);
	memcpy(xData.strWebFactory, m_ppFIELD_INFO[FIELD_WEB_FACTORY]->pValue, m_ppFIELD_INFO[FIELD_WEB_FACTORY]->nLen);
	memcpy(xData.strWebType, m_ppFIELD_INFO[FIELD_WEB_TYPE]->pValue, m_ppFIELD_INFO[FIELD_WEB_TYPE]->nLen);
	memcpy(xData.strInsp, m_ppFIELD_INFO[FIELD_INSP]->pValue, m_ppFIELD_INFO[FIELD_INSP]->nLen);
	memcpy(xData.strLight, m_ppFIELD_INFO[FIELD_LIGHT]->pValue, m_ppFIELD_INFO[FIELD_LIGHT]->nLen);
	memcpy(xData.strLotNo, m_ppFIELD_INFO[FIELD_LOT_NO]->pValue, m_ppFIELD_INFO[FIELD_LOT_NO]->nLen);


	CString strLog;
	
	strLog.Format(L"strOrder_No:%s, strOrder_Material:%s, nOrder_Quatity:%d,strFactory:%s,strStation:%s,strGlueType:%s,strWebSpec:%s,strWebFactory:%s,strWebType:%s,strInsp:%s,strLight:%s, strLotNo:%s",
		CString(xData.strOrder_No, _countof(xData.strOrder_No)),
		CString(xData.strOrder_Material, _countof(xData.strOrder_Material)),
		xData.nOrder_Quatity,
		CString(xData.strFactory, _countof(xData.strFactory)),
		CString(xData.strStation, _countof(xData.strStation)),
		CString(xData.strGlueType, _countof(xData.strGlueType)),
		CString(xData.strWebSpec, _countof(xData.strWebSpec)),
		CString(xData.strWebFactory, _countof(xData.strWebFactory)),
		CString(xData.strWebType, _countof(xData.strWebType)),
		CString(xData.strInsp, _countof(xData.strInsp)),
		CString(xData.strLight, _countof(xData.strLight)),
		CString(xData.strLotNo, _countof(xData.strLotNo))
		);

	theApp.InsertDebugLog(strLog, LOG_OPC);
	if (USM_WriteData((BYTE*)&xData, sizeof(xData))){
		NotifyAOI(WM_OPC_NEWBATCH_CMD, NULL);
	}
	else{
		theApp.InsertDebugLog(L"Write Shared memory fail!", LOG_OPC);
	}
}
DWORD CSystPPProcess::Thread_Process(void* pvoid)
{
	CSystPPProcess* pThis = (CSystPPProcess*)pvoid;
	BOOL              bRun = TRUE;
	while (bRun)
	{
		switch (::WaitForMultipleObjects(EV_COUNT, pThis->m_hEvent, FALSE, INFINITE))
		{
		case CASE_NEWBATCH:
		{	
			::ResetEvent(pThis->m_hEvent[EV_NEWBATCH]);
			::Sleep(COMMAND_TIMER_INTERVAL); //wait for 3 sec
			
			pThis->ProcessNewBatch();
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