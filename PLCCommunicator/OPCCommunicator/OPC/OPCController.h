#pragma once
#include "OPCProcessBase.h"
enum NODE_LIST{
	NODE_ORDER_NO,	//工單(訂單號)

	NODE_MAX,
};

#define OPC_SAMPLING_INTERVAL	100.0 //100 ms

typedef UA_Server* (WINAPI fUA_Server_new)(void);
typedef fUA_Server_new* pUA_Server_new;
typedef UA_ServerConfig* (WINAPI fUA_Server_getConfig)(UA_Server *server);
typedef fUA_Server_getConfig* pUA_Server_getConfig;
typedef UA_StatusCode(WINAPI fUA_ServerConfig_setMinimalCustomBuffer)(UA_ServerConfig *config, UA_UInt16 portNumber, const UA_ByteString *certificate, UA_UInt32 sendBufferSize, UA_UInt32 recvBufferSize);
typedef fUA_ServerConfig_setMinimalCustomBuffer* pUA_ServerConfig_setMinimalCustomBuffer;
typedef UA_StatusCode (WINAPI fUA_Server_run)(UA_Server *server, const volatile UA_Boolean *running);
typedef fUA_Server_run* pUA_Server_run;
typedef void (WINAPI fUA_Server_delete)(UA_Server *server);
typedef fUA_Server_delete* pUA_Server_delete;
typedef UA_MonitoredItemCreateResult(WINAPI fUA_Server_createDataChangeMonitoredItem)(UA_Server *server, UA_TimestampsToReturn timestampsToReturn, const UA_MonitoredItemCreateRequest item, void *monitoredItemContext, UA_Server_DataChangeNotificationCallback callback);
typedef fUA_Server_createDataChangeMonitoredItem* pUA_Server_createDataChangeMonitoredItem;
typedef UA_String(WINAPI fUA_String_fromChars)(const char *src);
typedef fUA_String_fromChars* pUA_String_fromChars;
typedef void (WINAPI fUA_Variant_setScalar)(UA_Variant *v, void * UA_RESTRICT p, const UA_DataType *type);
typedef fUA_Variant_setScalar* pUA_Variant_setScalar;
typedef UA_StatusCode(WINAPI f__UA_Server_addNode)(UA_Server *server, const UA_NodeClass nodeClass, const UA_NodeId *requestedNewNodeId, const UA_NodeId *parentNodeId, const UA_NodeId *referenceTypeId, const UA_QualifiedName browseName, const UA_NodeId *typeDefinition, const UA_NodeAttributes *attr, const UA_DataType *attributeType, void *nodeContext, UA_NodeId *outNewNodeId);
typedef f__UA_Server_addNode* p__UA_Server_addNode;





class COPCController : public COPCProcessBase{
public:
	COPCController();
	virtual ~COPCController();

	void RunOPCServer();
	void DoServerCallback(UA_Server *server, UA_UInt32 monitoredItemId,
		void *monitoredItemContext, const UA_NodeId *nodeId,
		void *nodeContext, UA_UInt32 attributeId,
		const UA_DataValue *value);

	//IOPCProcess
	virtual void ON_OPEN_OPC(LPARAM lp);
protected:
	virtual void UpdateNode(UA_String &uaData, const UA_DataValue *pValue) = 0;
private:
	void Init();
	void Finalize();

	void AddServerVariable();
	void AddServerMonitoredItem();


	void LIB_INIT();
	void LIB_LOAD();
	void LIB_FREE();
	UA_LocalizedText UA_LOCALIZEDTEXT_ALLOC_(const char *locale, const char *text);
	UA_NodeId UA_NODEID_STRING_ALLOC_(UA_UInt16 nsIndex, const char *chars);
	UA_NodeId UA_NODEID_NUMERIC_(UA_UInt16 nsIndex, UA_UInt32 identifier);
private:
	UA_Server *m_pServer;
	bool m_bOPCRunning;
	CWinThread *m_pOPCThread;

private:
	HMODULE m_hDL_Dll_OPEN62541;
	pUA_Server_new m_pUA_Server_new;
	pUA_Server_getConfig m_pUA_Server_getConfig;
	pUA_ServerConfig_setMinimalCustomBuffer m_pUA_ServerConfig_setMinimalCustomBuffer;
	pUA_Server_run m_pUA_Server_run;
	pUA_Server_delete m_pUA_Server_delete;
	pUA_Server_createDataChangeMonitoredItem m_pUA_Server_createDataChangeMonitoredItem;
	pUA_String_fromChars m_pUA_String_fromChars;
	pUA_Variant_setScalar m_pUA_Variant_setScalar;
	p__UA_Server_addNode m_p__UA_Server_addNode;
	UA_DataType* m_pUA_TYPES;
	UA_VariableAttributes *m_pUA_VariableAttributes;
};
