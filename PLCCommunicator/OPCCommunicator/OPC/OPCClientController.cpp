#include "stdafx.h"
#include "OPCClientController.h"
#include "DataHandlerBase.h"
#include "OPCCommunicator.h"
#define DLL_OPEN62541 L"libopen62541.dll"
#define TIMER_ID_ITERATE		1
#define TIMER_ID_RECONNECT		2

//format parser
vector<CString> GetEachStringBySep(CString sStr, TCHAR tSep)
{
	vector<CString> vEachStr;
	TCHAR *token = NULL;
	TCHAR *pNextToken = sStr.GetBuffer();
	TCHAR xSep[2] = { NULL, NULL };
	xSep[0] = tSep;
	do{
		while (pNextToken && *pNextToken && (*pNextToken == tSep)){
			vEachStr.push_back(CString(_T('\0')));
			pNextToken++;
		}
		token = _tcstok_s(NULL, xSep, &pNextToken);
		if (token){
			CString xNew(token);
			if (xNew != _T("\r\n")){
				vEachStr.push_back(xNew);
			}
		}
	} while (token != NULL);
	return vEachStr;
}
//format parser

COPCClientController* pThis = NULL;
COPCClientController::COPCClientController()
{
	Init();
}
COPCClientController::~COPCClientController()
{
	Finalize();
}
void COPCClientController::Init()
{
	m_bForceClose = FALSE;
	m_tTimer = NULL;
	m_tReconnectTimer = NULL;
	pThis = this;
	m_pClient = NULL;
	LIB_INIT();
}
void COPCClientController::Finalize()
{
	if (m_pUA_Client_delete && m_pClient){
		m_pUA_Client_delete(m_pClient);
		m_pClient = NULL;
	}
	LIB_FREE();
}
void COPCClientController::LIB_INIT()
{
	m_hDL_Dll_OPEN62541 = NULL;
	m_pUA_Client_new = NULL;
	m_pUA_Client_getConfig = NULL;
	m_pUA_ClientConfig_setDefault = NULL;
	m_pUA_Client_connect = NULL;
	m_pUA_Client_disconnect = NULL;
	m_pUA_Client_delete = NULL;
	m_pUA_Client_run_iterate = NULL;
	m_pUA_Client_Subscriptions_create = NULL;
	m_pUA_Client_MonitoredItems_createDataChange = NULL;
	m_pUA_clear = NULL;
	m_pUA_new = NULL;
	m_p__UA_Client_Service = NULL;
	m_pUA_TYPES = NULL;
	m_pUA_String_fromChars = NULL;
	m_pUA_Variant_setScalar = NULL;
	m_p__UA_Client_writeAttribute = NULL;
}
void COPCClientController::LIB_FREE()
{
	m_pUA_Client_new = NULL;
	m_pUA_Client_getConfig = NULL;
	m_pUA_ClientConfig_setDefault = NULL;
	m_pUA_Client_connect = NULL;
	m_pUA_Client_disconnect = NULL;
	m_pUA_Client_delete = NULL;
	m_pUA_Client_run_iterate = NULL;
	m_pUA_Client_Subscriptions_create = NULL;
	m_pUA_Client_MonitoredItems_createDataChange = NULL;
	m_pUA_clear = NULL;
	m_pUA_new = NULL;
	m_p__UA_Client_Service = NULL;
	m_pUA_TYPES = NULL;
	m_pUA_String_fromChars = NULL;
	m_pUA_Variant_setScalar = NULL;
	m_p__UA_Client_writeAttribute = NULL;
	if (m_hDL_Dll_OPEN62541){
		::FreeLibrary(m_hDL_Dll_OPEN62541);
		m_hDL_Dll_OPEN62541 = NULL;
	}
}
void CALLBACK COPCClientController::OnTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer)
{
	if (pThis){
		pThis->ProcessTimer(nEventId);
	}
}
void COPCClientController::ProcessTimer(UINT_PTR nEventId)
{
	if (m_tTimer == nEventId)
	{
		if (m_pClient && m_pUA_Client_run_iterate)
		{
			m_pUA_Client_run_iterate(m_pClient, 50);
		}
	}
	else if (m_tReconnectTimer == nEventId){
		::KillTimer(NULL, m_tReconnectTimer);
		m_tReconnectTimer = NULL;
		Connect();
	}
}
///<summary></summary>
void COPCClientController::GetMonitorNodes(vector<std::pair<CString, UA_NodeId>> &vNode)
{
	if (m_pUA_new && m_pUA_clear && m_p__UA_Client_Service && m_pUA_TYPES && m_pUA_String_fromChars)//確認初始化有成功
	{
		//id容器設置(底層(變數)資料夾)
		UA_NodeId id;
		id.namespaceIndex = m_nRootNS;//= 1
		id.identifierType = UA_NODEIDTYPE_STRING;//3
		id.identifier.string = m_pUA_String_fromChars(m_strRootID);//L"PL25_PL2502043"變數資料夾

		UA_BrowseRequest bRequest = { 0 };
		UA_BrowseRequest_init(&bRequest);//初始化
		bRequest.requestedMaxReferencesPerNode = 0;
		bRequest.nodesToBrowse = (UA_BrowseDescription*)m_pUA_new(m_pUA_TYPES + UA_TYPES_BROWSEDESCRIPTION);//節點加入Type格式
		bRequest.nodesToBrowseSize = 1;
		bRequest.nodesToBrowse[0].nodeId = id;//節點加入初始位置
		bRequest.nodesToBrowse[0].resultMask = UA_BROWSERESULTMASK_ALL; /* return everything */

		UA_BrowseResponse bResponse;
		m_p__UA_Client_Service(m_pClient,                              //UA_Client* client
							   &bRequest,                              //const void* request
							   m_pUA_TYPES + UA_TYPES_BROWSEREQUEST,   //const UA_DataType * requestType
							   &bResponse,                             //void* response
							   m_pUA_TYPES + UA_TYPES_BROWSERESPONSE   //const UA_DataType* responseType
							   );
		m_pUA_clear(&bRequest, m_pUA_TYPES + UA_TYPES_BROWSEREQUEST);

		CString strLog;
		strLog.Format(L"bResp.resultsSize %d ", bResponse.resultsSize);
		ON_OPC_NOTIFY(strLog);//丟去給訊息處理 最後會貼在Log列表
		for (size_t i = 0; i < bResponse.resultsSize; ++i) {
			strLog.Format(L"bResp.results[i].referencesSize %d ", bResponse.results[i].referencesSize);
			ON_OPC_NOTIFY(strLog);
			for (size_t j = 0; j < bResponse.results[i].referencesSize; ++j) {
				UA_ReferenceDescription *ref = &(bResponse.results[i].references[j]);
				CString strDisplayName((char*)ref->displayName.text.data, ref->displayName.text.length);
				BOOL bFind = FALSE;
				for (auto &strMonitor : m_vMonitorNode){
					if (strMonitor == strDisplayName){
						switch (ref->nodeId.nodeId.identifierType){
						case UA_NODEIDTYPE_STRING:
						{
							UA_NodeId id; 
							id.namespaceIndex = ref->nodeId.nodeId.namespaceIndex;
							id.identifierType = UA_NODEIDTYPE_STRING;
							CStringA str((char*)ref->nodeId.nodeId.identifier.string.data, ref->nodeId.nodeId.identifier.string.length);
							id.identifier.string = m_pUA_String_fromChars(str);
							vNode.push_back(std::pair<CString, UA_NodeId>(strMonitor, id));
						}
							break;
						default:
							vNode.push_back(std::pair<CString, UA_NodeId>(strMonitor, ref->nodeId.nodeId));
							break;
						}
						bFind = TRUE;
						break;
					}
				}
				if (!bFind){
					strLog.Format(L"warning Display name:%s not found", strDisplayName);
					theApp.InsertDebugLog(strLog, LOG_OPC);
				}
			}
		}

		if (m_pUA_clear && m_pUA_TYPES){
			//clear old resp
			m_pUA_clear(&bResponse, m_pUA_TYPES + UA_TYPES_BROWSERESPONSE);
		}
	}
}
///<summary>Client連線狀況對策</summary>
void COPCClientController::DoStateCallback(UA_Client *client, UA_ClientState eState)
{
	switch (eState)
	{

		case UA_CLIENTSTATE_DISCONNECTED:          /* The client is disconnected */
			if (m_tTimer)
			{
				::KillTimer(NULL, m_tTimer);
				m_tTimer = NULL;
			}
			if (m_bForceClose)
			{
				ON_OPC_NOTIFY(L"Force Disconnect");
			}
			else
			{
				ON_OPC_NOTIFY(L"Disconnect, reconnect again");
				if (m_tReconnectTimer == NULL)
				{
					m_tReconnectTimer = SetTimer(NULL, TIMER_ID_RECONNECT, 3000, OnTimer);
				}
			}
			NotifyAOI(WM_OPC_RETURN_STATUS_CMD, 0);
			break;
		case UA_CLIENTSTATE_WAITING_FOR_ACK:      /* The Client has sent HEL and waiting */
			break;
		case UA_CLIENTSTATE_CONNECTED:            /* A TCP connection to the server is open */
			NotifyAOI(WM_OPC_RETURN_STATUS_CMD, 1);
			break;
		case UA_CLIENTSTATE_SECURECHANNEL:        /* A SecureChannel to the server is open */
			break;
		case UA_CLIENTSTATE_SESSION:              /* A session with the server is open */
			break;
		case UA_CLIENTSTATE_SESSION_DISCONNECTED: /* Disconnected vs renewed? */
			break;
		case UA_CLIENTSTATE_SESSION_RENEWED:      /* A session with the server is open (renewed) */
			break;
	}
}
///<summary>Client連線狀況Callback</summary>
void COPCClientController::stateCallback(UA_Client *client, UA_ClientState eState)
{
	if (pThis)
	{
		pThis->DoStateCallback(client, eState);
	}
}
///<summary>連線</summary>
void COPCClientController::Connect()
{
	if (m_pUA_Client_new)//新連線
	{
		if (!m_pClient && m_pUA_Client_getConfig && m_pUA_ClientConfig_setDefault)
		{
			m_pClient = m_pUA_Client_new();//繼承

			UA_ClientConfig *cc = m_pUA_Client_getConfig(m_pClient);
			m_pUA_ClientConfig_setDefault(cc);
			/* default timeout is 5 seconds. Set it to 500ms */
			cc->timeout = 500;
			cc->stateCallback = stateCallback;//增加Clientstate的回呼
		}
	}
	UA_StatusCode dwRetval = UA_STATUSCODE_BADUNEXPECTEDERROR;//預設錯誤
	if (m_pUA_Client_connect)//若連線
	{
		dwRetval = m_pUA_Client_connect(m_pClient, m_strConnectURL);//client,endpointUrl
	}
	if (dwRetval == UA_STATUSCODE_GOOD)
	{
		TRACE("Connect to server on %s\n", m_strConnectURL);
		CString strLog;
		strLog.Format(L"Connect to server on %s\n", CString(m_strConnectURL));
		ON_OPC_NOTIFY(strLog);
		if (m_tTimer == NULL)
		{
			m_tTimer = SetTimer(NULL, TIMER_ID_ITERATE, 1000, OnTimer);
		}
		DiscoverNode();
	}
	else
	{
		ON_OPC_NOTIFY(L"Connect fail, reconnect again");
		if (m_tReconnectTimer == NULL)
		{
			m_tReconnectTimer = SetTimer(NULL, TIMER_ID_RECONNECT, 3000, OnTimer);
		}
	}
}
void COPCClientController::ON_CLOSE_OPC()
{
	m_bForceClose = TRUE;
	if (m_pUA_Client_disconnect && m_pClient){
		m_pUA_Client_disconnect(m_pClient);
	}
	if (m_pUA_Client_delete && m_pClient){
		m_pUA_Client_delete(m_pClient);
		m_pClient = NULL;
	}
	theApp.InsertDebugLog(L"force disconnect", LOG_OPC);
}
void COPCClientController::ON_OPEN_OPC(LPARAM lp)
{
	m_bForceClose = FALSE;
	if (lp == WM_OPC_PARAMINIT_CMD){
		LIB_LOAD();

		BATCH_SHARE_OPC_INITPARAM_ xData;
		memset(&xData, 0, sizeof(xData));
#ifdef OFFLINE_DEBUG
		xData.nRootIdNamespace = 1;
		CString strIP=L"opc.tcp://localhost:4841", strRootID = L"PL25_PL2502043";
		memcpy(xData.cOPCIP, strIP.GetBuffer(), strIP.GetLength() * 2);
		memcpy(xData.cROOTID, strRootID.GetBuffer(), strRootID.GetLength() * 2);
#else
		if (USM_ReadAOIData((BYTE*)&xData, sizeof(xData)))
#endif
		{
			CString strLog;
			ON_OPC_PARAM(L"IP", xData.cOPCIP);
			strLog.Format(L"%d", xData.nRootIdNamespace);
			ON_OPC_PARAM(L"NS", strLog);

			CString strOrignID; //change to cstring format
			strOrignID.Format(L"\"%s\"", xData.cROOTID);

			ON_OPC_PARAM(L"ROOT", strOrignID);
			m_strRootID = CStringA(strOrignID);
			m_nRootNS = xData.nRootIdNamespace;//= 1

			m_strConnectURL = CString(xData.cOPCIP);
			Connect();
		}
	}
	else if (lp == NULL){ // reconnect by UI
		Connect();
		theApp.InsertDebugLog(L"ui reconnect", LOG_OPC);
	}
}
void COPCClientController::WriteFloatField(UA_NodeId& xNodeId, float fData)
{
	if (m_pUA_Variant_setScalar){
		UA_Variant input;
		UA_Variant_init(&input);
		m_pUA_Variant_setScalar(&input, &fData, m_pUA_TYPES + UA_TYPES_FLOAT);
		WriteOPCField(input, xNodeId);
	}
}
void COPCClientController::WriteIntField(UA_NodeId& xNodeId, WORD wData)
{
	if (m_pUA_Variant_setScalar){
		UA_Variant input;
		UA_Variant_init(&input);
		m_pUA_Variant_setScalar(&input, &wData, m_pUA_TYPES + UA_TYPES_INT16);
		WriteOPCField(input, xNodeId);
	}
}
UA_StatusCode COPCClientController::WriteBOOLField(UA_NodeId& xNodeId, BOOL bData)
{
	if (m_pUA_Variant_setScalar){
		UA_Variant input;
		UA_Variant_init(&input);
		m_pUA_Variant_setScalar(&input, &bData, m_pUA_TYPES + UA_TYPES_BOOLEAN);
		return WriteOPCField(input, xNodeId);
	}
	return UA_STATUSCODE_BADSYNTAXERROR;
}
void COPCClientController::WriteStringField(UA_NodeId& xNodeId, char *pStr)
{
	if (m_pUA_Variant_setScalar && m_pUA_String_fromChars){
		UA_Variant input;
		UA_Variant_init(&input);
		UA_String stringValue = m_pUA_String_fromChars(pStr);
		m_pUA_Variant_setScalar(&input, &stringValue, m_pUA_TYPES + UA_TYPES_STRING);
		WriteOPCField(input, xNodeId);
	}
}
UA_StatusCode COPCClientController::WriteOPCField(UA_Variant& xInput, UA_NodeId& xNodeId)
{
	if (m_p__UA_Client_writeAttribute){
		return m_p__UA_Client_writeAttribute(m_pClient, &xNodeId, UA_ATTRIBUTEID_VALUE,
			&xInput, m_pUA_TYPES + UA_TYPES_VARIANT);
	}
	return UA_STATUSCODE_BADSYNTAXERROR;
}
void COPCClientController::ON_SET_MONITOR_NDOE(vector<CString> vMonitor)
{
	m_vMonitorNode = vMonitor;
}
void COPCClientController::DoClientCallBack(UA_Client *client, UA_UInt32 subId, void *subContext,UA_UInt32 monId, void *monContext, UA_DataValue *value)
{
	UpdateNode(monId, value);
}
static void ClientCallBack(UA_Client *client, UA_UInt32 subId, void *subContext,UA_UInt32 monId, void *monContext, UA_DataValue *value)
{
	pThis->DoClientCallBack(client, subId, subContext, monId, monContext, value);
}
void COPCClientController::DiscoverNode()
{
	if (m_pClient)//若連線
	{
		vector<std::pair<CString, UA_NodeId>> vMonitorNode;//創建監控節點vector
		GetMonitorNodes(vMonitorNode);//取得監控節點
		UA_CreateSubscriptionRequest request = UA_CreateSubscriptionRequest_default();//請求
		UA_CreateSubscriptionResponse response = m_pUA_Client_Subscriptions_create(m_pClient, request, NULL, NULL, NULL);//回應
		for (auto &xNode : vMonitorNode)
		{
			if (m_pUA_Client_Subscriptions_create && m_pUA_Client_MonitoredItems_createDataChange)
			{
				if (response.responseHeader.serviceResult == UA_STATUSCODE_GOOD)
				{
					UA_MonitoredItemCreateRequest xMonRequest = UA_MonitoredItemCreateRequest_default(xNode.second);
					UA_MonitoredItemCreateResult monResponse = m_pUA_Client_MonitoredItems_createDataChange(
															   m_pClient,
															   response.subscriptionId, 
															   UA_TIMESTAMPSTORETURN_BOTH, 
															   xMonRequest, 
															   NULL, 
															   ClientCallBack, 
															   NULL
															  );
					if (monResponse.statusCode == UA_STATUSCODE_GOOD)
					{
						CString strLog;
						strLog.Format(L"monitor ok type:%d disp:%s, field %s, subid:%d", xNode.second.identifierType, xNode.first, CString((char*)xNode.second.identifier.string.data, xNode.second.identifier.string.length), response.subscriptionId);
						theApp.InsertDebugLog(strLog, LOG_OPC);
						SET_MONITOR_ID(xNode.first, xNode.second, monResponse.monitoredItemId);
					}
					else
					{
						CString strLog;
						strLog.Format(L"monitor fail type:%d disp:%s, field %s, status:%d", 
									  xNode.second.identifierType, 
									  xNode.first, 
									  CString((char*)xNode.second.identifier.string.data, 
									  xNode.second.identifier.string.length), 
									  response.subscriptionId, 
									  monResponse.statusCode
									 );
						theApp.InsertDebugLog(strLog, LOG_OPC);
					}
				}
				else{
					CString strLog;
					strLog.Format(L"Subscriptions_create fail type:%d disp:%s, field %s, status:%d", 
						          xNode.second.identifierType, xNode.first, 
						          CString((char*)xNode.second.identifier.string.data, xNode.second.identifier.string.length), 
						          response.responseHeader.serviceResult
								 );
					theApp.InsertDebugLog(strLog, LOG_OPC);
				}
			}
		}
	}
}
void COPCClientController::LIB_LOAD()
{
	CString strFile;
#ifdef _UNICODE					
	wchar_t	workingDir[_MAX_PATH];	// the working path
	_wgetcwd(workingDir, _MAX_PATH);
#else
	char	workingDir[_MAX_PATH];	// the working path
	_getcwd(workingDir, _MAX_PATH);
#endif
	strFile.Format(L"%s\\%s", workingDir, DLL_OPEN62541);

	CString strLog;
	strLog.Format(L"Load %s ", DLL_OPEN62541);
	m_hDL_Dll_OPEN62541 = ::LoadLibrary(strFile);
	if (m_hDL_Dll_OPEN62541){
		m_pUA_Client_new = reinterpret_cast<pUA_Client_new>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Client_new"));
		m_pUA_Client_getConfig = reinterpret_cast<pUA_Client_getConfig>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Client_getConfig"));
		m_pUA_ClientConfig_setDefault = reinterpret_cast<pUA_ClientConfig_setDefault>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_ClientConfig_setDefault"));
		m_pUA_Client_connect = reinterpret_cast<pUA_Client_connect>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Client_connect"));
		m_pUA_Client_disconnect = reinterpret_cast<pUA_Client_disconnect>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Client_disconnect"));
		m_pUA_Client_delete = reinterpret_cast<pUA_Client_delete>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Client_delete"));
		m_pUA_Client_run_iterate = reinterpret_cast<pUA_Client_run_iterate>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Client_run_iterate"));
		m_pUA_Client_Subscriptions_create = reinterpret_cast<pUA_Client_Subscriptions_create>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Client_Subscriptions_create"));
		m_pUA_Client_MonitoredItems_createDataChange = reinterpret_cast<pUA_Client_MonitoredItems_createDataChange>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Client_MonitoredItems_createDataChange"));
		m_pUA_clear = reinterpret_cast<pUA_clear>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_clear"));
		m_pUA_new = reinterpret_cast<pUA_new>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_new"));
		m_p__UA_Client_Service = reinterpret_cast<p__UA_Client_Service>(GetProcAddress(m_hDL_Dll_OPEN62541, "__UA_Client_Service"));
		m_pUA_TYPES = reinterpret_cast<UA_DataType*>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_TYPES"));
		m_pUA_String_fromChars = reinterpret_cast<pUA_String_fromChars>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_String_fromChars"));
		m_pUA_Variant_setScalar = reinterpret_cast<pUA_Variant_setScalar>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Variant_setScalar"));
		m_p__UA_Client_writeAttribute = reinterpret_cast<p__UA_Client_writeAttribute>(GetProcAddress(m_hDL_Dll_OPEN62541, "__UA_Client_writeAttribute"));
		ON_OPC_NOTIFY(strLog + L"Success");
	}
	else{
		ON_OPC_NOTIFY(strLog + L"Fail");
	}
}