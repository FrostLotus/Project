#include "stdafx.h"
#include "OPCController.h"
#include "OPCCommunicator.h"

#define OPC_PORT	4841
#define DLL_OPEN62541 L"libopen62541.dll"

COPCController* pThis;

UINT OPC_SERVER_THREAD(LPVOID lp)
{
	COPCController* pThis = (COPCController*)lp;
	if (pThis)
	{
		pThis->RunOPCServer();
	}
	return 0;
}
COPCController::COPCController()
{
	Init();
}
COPCController::~COPCController()
{
	Finalize();
}
void COPCController::Init()
{
	pThis = this;
	m_pOPCThread = NULL;
	LIB_INIT();
}
void COPCController::Finalize()
{
	m_bOPCRunning = FALSE;
	if (m_pOPCThread)
	{
		::WaitForSingleObject(m_pOPCThread->m_hThread, INFINITE);
		delete m_pOPCThread;
		m_pOPCThread = NULL;
	}
	LIB_FREE();
}
void COPCController::LIB_INIT()
{
	m_hDL_Dll_OPEN62541 = NULL;
	m_pUA_Server_new = NULL;
	m_pUA_Server_getConfig = NULL;
	m_pUA_ServerConfig_setMinimalCustomBuffer = NULL;
	m_pUA_Server_run = NULL;
	m_pUA_Server_delete = NULL;
	m_pUA_Server_createDataChangeMonitoredItem = NULL;
	m_pUA_String_fromChars = NULL;
	m_pUA_Variant_setScalar = NULL;
	m_p__UA_Server_addNode = NULL;
	m_pUA_TYPES = NULL;
	m_pUA_VariableAttributes = NULL;
}
void COPCController::LIB_FREE()
{
	m_hDL_Dll_OPEN62541 = NULL;
	m_pUA_Server_new = NULL;
	m_pUA_Server_getConfig = NULL;
	m_pUA_ServerConfig_setMinimalCustomBuffer = NULL;
	m_pUA_Server_run = NULL;
	m_pUA_Server_delete = NULL;
	m_pUA_Server_createDataChangeMonitoredItem = NULL;
	m_pUA_String_fromChars = NULL;
	m_pUA_Variant_setScalar = NULL;
	m_p__UA_Server_addNode = NULL;
	m_pUA_TYPES = NULL;
	m_pUA_VariableAttributes = NULL;
	if (m_hDL_Dll_OPEN62541)
	{
		::FreeLibrary(m_hDL_Dll_OPEN62541);
		m_hDL_Dll_OPEN62541 = NULL;
	}
}
UA_LocalizedText COPCController::UA_LOCALIZEDTEXT_ALLOC_(const char* locale, const char* text)
{
	UA_LocalizedText lt;
	if (m_pUA_String_fromChars)
	{
		lt.locale = m_pUA_String_fromChars(locale);
		lt.text = m_pUA_String_fromChars(text);
	}
	return lt;
}
UA_NodeId COPCController::UA_NODEID_STRING_ALLOC_(UA_UInt16 nsIndex, const char* chars)
{
	UA_NodeId id;
	id.namespaceIndex = nsIndex;
	id.identifierType = UA_NODEIDTYPE_STRING;
	if (m_pUA_String_fromChars)
	{
		id.identifier.string = m_pUA_String_fromChars(chars);
	}
	return id;
}
UA_NodeId COPCController::UA_NODEID_NUMERIC_(UA_UInt16 nsIndex, UA_UInt32 identifier)
{
	UA_NodeId id;
	id.namespaceIndex = nsIndex;
	id.identifierType = UA_NODEIDTYPE_NUMERIC;
	id.identifier.numeric = identifier;
	return id;
}
void COPCController::LIB_LOAD()
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
	if (m_hDL_Dll_OPEN62541)
	{
		m_pUA_TYPES = reinterpret_cast<UA_DataType*>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_TYPES"));
		m_pUA_VariableAttributes = reinterpret_cast<UA_VariableAttributes*>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_VariableAttributes_default"));
		m_pUA_Server_new = reinterpret_cast<pUA_Server_new>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Server_new"));
		m_pUA_Server_getConfig = reinterpret_cast<pUA_Server_getConfig>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Server_getConfig"));
		m_pUA_ServerConfig_setMinimalCustomBuffer = reinterpret_cast<pUA_ServerConfig_setMinimalCustomBuffer>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_ServerConfig_setMinimalCustomBuffer"));
		m_pUA_Server_run = reinterpret_cast<pUA_Server_run>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Server_run"));
		m_pUA_Server_delete = reinterpret_cast<pUA_Server_delete>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Server_delete"));
		m_pUA_Server_createDataChangeMonitoredItem = reinterpret_cast<pUA_Server_createDataChangeMonitoredItem>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Server_createDataChangeMonitoredItem"));
		m_pUA_String_fromChars = reinterpret_cast<pUA_String_fromChars>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_String_fromChars"));
		m_pUA_Variant_setScalar = reinterpret_cast<pUA_Variant_setScalar>(GetProcAddress(m_hDL_Dll_OPEN62541, "UA_Variant_setScalar"));
		m_p__UA_Server_addNode = reinterpret_cast<p__UA_Server_addNode>(GetProcAddress(m_hDL_Dll_OPEN62541, "__UA_Server_addNode"));

		ON_OPC_NOTIFY(strLog + L"Success");
	}
	else
	{
		ON_OPC_NOTIFY(strLog + L"Fail");
	}
}

void COPCController::RunOPCServer()
{
	m_bOPCRunning = TRUE;
	CString strLog;
	strLog.Format(L"OPC server start run!");
	TRACE(L"%s \n", strLog);
	theApp.InsertDebugLog(strLog, LOG_OPC);
	ON_OPC_NOTIFY(strLog);

	if (m_pUA_Server_run)
	{
		UA_StatusCode uRtn = m_pUA_Server_run(m_pServer, &m_bOPCRunning);
	}

	if (m_pUA_Server_delete)
	{
		m_pUA_Server_delete(m_pServer);
	}
	m_pServer = NULL;
	strLog.Format(L"OPC server stop!");
	TRACE(L"%s \n", strLog);
	ON_OPC_NOTIFY(strLog);
	theApp.InsertDebugLog(strLog, LOG_OPC);
}
void COPCController::ON_OPEN_OPC(LPARAM lp)
{
	LIB_LOAD();

	m_bOPCRunning = FALSE;

	if (m_pUA_Server_new)
	{
		m_pServer = m_pUA_Server_new();
	}
	if (m_pServer && m_pUA_Server_getConfig && m_pUA_ServerConfig_setMinimalCustomBuffer)
	{
		m_pUA_ServerConfig_setMinimalCustomBuffer(m_pUA_Server_getConfig(m_pServer), OPC_PORT, NULL, NULL, NULL);
		CString strParam = L"Port", strValue;
		strValue.Format(L"%d", OPC_PORT);
		ON_OPC_PARAM(strParam, strValue);
	}
	AddServerVariable();
	AddServerMonitoredItem();

	m_pOPCThread = AfxBeginThread(OPC_SERVER_THREAD, this, NULL, NULL, CREATE_SUSPENDED);
	m_pOPCThread->m_bAutoDelete = 0;
	m_pOPCThread->ResumeThread();
}
void COPCController::AddServerVariable()
{
	if (m_pUA_VariableAttributes)
	{
		UA_VariableAttributes xAttr = *m_pUA_VariableAttributes;

		char szText[MAX_PATH];
		int nNodeSize = GetNodeSize();

		for (int i = 0; i < nNodeSize; i++)
		{
			NodeItem* pItem = GetNodeItem(i);
			if (pItem)
			{
				memset(&xAttr, 0, sizeof(xAttr));
				size_t nConvert = 0;
				wcstombs_s(&nConvert, szText, MAX_PATH, pItem->strNodeId, MAX_PATH);
				xAttr.description = UA_LOCALIZEDTEXT_ALLOC_("en-US", szText);
				xAttr.displayName = UA_LOCALIZEDTEXT_ALLOC_("en-US", szText);
				xAttr.accessLevel = UA_ACCESSLEVELMASK_READ | UA_ACCESSLEVELMASK_WRITE;

				switch (pItem->nType)
				{
					case UA_TYPES_STRING:
					{
						UA_String uaString;
						int nTotalByte = pItem->nLen + 2;
						BYTE* pTemp = new BYTE[nTotalByte];
						memset(pTemp, 0, nTotalByte);
						memcpy(pTemp, pItem->pValue, pItem->nLen);
						int nRtn = wcstombs_s(&nConvert, szText, MAX_PATH, (TCHAR*)pTemp, nTotalByte);
						delete[]pTemp;
						if (m_pUA_String_fromChars)
						{
							uaString = m_pUA_String_fromChars(szText);
						}
						if (m_pUA_Variant_setScalar && m_pUA_TYPES)
						{
							xAttr.dataType = (m_pUA_TYPES + UA_TYPES_STRING)->typeId;
							m_pUA_Variant_setScalar(&xAttr.value, &uaString, m_pUA_TYPES + UA_TYPES_STRING);
						}
					}
					break;
					case UA_TYPES_UINT16:
					{
						UA_UInt16 uaWord;
						uaWord = *(short*)pItem->pValue;
						xAttr.dataType = (m_pUA_TYPES + UA_TYPES_UINT16)->typeId;
						m_pUA_Variant_setScalar(&xAttr.value, &uaWord, m_pUA_TYPES + UA_TYPES_UINT16);
					}
					break;
					case UA_TYPES_BOOLEAN:
					{
						UA_Boolean uaBool;
						uaBool = *(bool*)pItem->pValue;
						xAttr.dataType = (m_pUA_TYPES + UA_TYPES_BOOLEAN)->typeId;
						m_pUA_Variant_setScalar(&xAttr.value, &uaBool, m_pUA_TYPES + UA_TYPES_BOOLEAN);
					}
					break;
					default:
						ASSERT(FALSE);
						break;
				}

				/* Add the variable node to the information model */
				wcstombs_s(&nConvert, szText, MAX_PATH, pItem->strNodeId, MAX_PATH);
				UA_NodeId uaNodeId = UA_NODEID_STRING_ALLOC_(1, szText);
				UA_QualifiedName qn;
				qn.namespaceIndex = 1;
				if (m_pUA_String_fromChars)
				{
					qn.name = m_pUA_String_fromChars(szText);
				}

				UA_NodeId parentNodeId = UA_NODEID_NUMERIC_(0, UA_NS0ID_OBJECTSFOLDER);
				UA_NodeId parentReferenceNodeId = UA_NODEID_NUMERIC_(0, UA_NS0ID_ORGANIZES);
				UA_NodeId typeDefinition = UA_NODEID_NUMERIC_(0, UA_NS0ID_BASEDATAVARIABLETYPE);
				UA_DataType* pUA_TYPES_VARIABLEATTRIBUTES = NULL;
				if (m_pUA_TYPES) pUA_TYPES_VARIABLEATTRIBUTES = m_pUA_TYPES + UA_TYPES_VARIABLEATTRIBUTES;
				if (m_p__UA_Server_addNode)
				{
					m_p__UA_Server_addNode(m_pServer, UA_NODECLASS_VARIABLE,
						&uaNodeId,
						&parentNodeId,
						&parentReferenceNodeId,
						qn,
						&typeDefinition,
						(const UA_NodeAttributes*)&xAttr, pUA_TYPES_VARIABLEATTRIBUTES,
						NULL,
						NULL);
				}
			}
		}
	}
}

void COPCController::DoServerCallback(UA_Server* server, UA_UInt32 monitoredItemId, void* monitoredItemContext, const UA_NodeId* nodeId, void* nodeContext, UA_UInt32 attributeId, const UA_DataValue* value)
{
	UA_String uaString = nodeId->identifier.string;
	UpdateNode(uaString, value);
}
void ServerCallback(UA_Server* server, UA_UInt32 monitoredItemId, void* monitoredItemContext, const UA_NodeId* nodeId, void* nodeContext, UA_UInt32 attributeId, const UA_DataValue* value)
{
	pThis->DoServerCallback(server, monitoredItemId, monitoredItemContext, nodeId, nodeContext, attributeId, value);
}
void COPCController::AddServerMonitoredItem()
{
	UA_NodeId xNodeId;
	UA_MonitoredItemCreateRequest xRequest;
	char szText[MAX_PATH];

	int nNodeSize = GetNodeSize();

	for (int i = 0; i < nNodeSize; i++)
	{
		NodeItem* pItem = GetNodeItem(i);
		if (pItem)
		{
			size_t nConvert = 0;
			wcstombs_s(&nConvert, szText, MAX_PATH, pItem->strNodeId, MAX_PATH);
			xNodeId = UA_NODEID_STRING_ALLOC_(1, szText);

			memset(&xRequest, 0, sizeof(xRequest));
			xRequest.itemToMonitor.nodeId = xNodeId;
			xRequest.itemToMonitor.attributeId = UA_ATTRIBUTEID_VALUE;
			xRequest.monitoringMode = UA_MONITORINGMODE_REPORTING;
			xRequest.requestedParameters.samplingInterval = 250;
			xRequest.requestedParameters.discardOldest = true;
			xRequest.requestedParameters.queueSize = 1;

			xRequest.requestedParameters.samplingInterval = OPC_SAMPLING_INTERVAL; /* 100 ms interval */
			if (m_pUA_Server_createDataChangeMonitoredItem)
			{
				m_pUA_Server_createDataChangeMonitoredItem(m_pServer, UA_TIMESTAMPSTORETURN_SOURCE, xRequest, NULL, ServerCallback);
			}
		}
	}
}