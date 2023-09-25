#pragma once
#include <open62541/client.h>
#include <open62541/client_subscriptions.h>
#include <vector>
#include "OPCProcessBase.h"

using namespace std;

typedef UA_Client* (WINAPI fUA_Client_new)(void);
typedef fUA_Client_new* pUA_Client_new;
typedef UA_ClientConfig* (WINAPI fUA_Client_getConfig)
(
	UA_Client *Client
);
typedef fUA_Client_getConfig* pUA_Client_getConfig;
typedef UA_StatusCode(WINAPI fUA_ClientConfig_setDefault)
(
	UA_ClientConfig *config
);
typedef fUA_ClientConfig_setDefault* pUA_ClientConfig_setDefault;
typedef UA_StatusCode(WINAPI fUA_Client_connect)
(
	UA_Client *client,
	const char *endpointUrl
);
typedef fUA_Client_connect* pUA_Client_connect;
typedef UA_StatusCode(WINAPI fUA_Client_disconnect)
(
	UA_Client *client
);
typedef fUA_Client_disconnect* pUA_Client_disconnect;
typedef void (WINAPI fUA_Client_delete)
(
	UA_Client *Client
);
typedef fUA_Client_delete* pUA_Client_delete;
typedef UA_StatusCode(WINAPI fUA_Client_run_iterate)
(
	UA_Client *client,
	UA_UInt16 timeout
);
typedef fUA_Client_run_iterate* pUA_Client_run_iterate;
typedef UA_CreateSubscriptionResponse(WINAPI fUA_Client_Subscriptions_create)
(
	UA_Client *client,
    const UA_CreateSubscriptionRequest request,
    void *subscriptionContext,
    UA_Client_StatusChangeNotificationCallback statusChangeCallback,
    UA_Client_DeleteSubscriptionCallback deleteCallback
);
typedef fUA_Client_Subscriptions_create* pUA_Client_Subscriptions_create;
typedef UA_MonitoredItemCreateResult(WINAPI fUA_Client_MonitoredItems_createDataChange)
(
	UA_Client *client, 																					
	UA_UInt32 subscriptionId,																					
	UA_TimestampsToReturn timestampsToReturn,
	const UA_MonitoredItemCreateRequest item,																					
	void *context, UA_Client_DataChangeNotificationCallback callback,																			
	UA_Client_DeleteMonitoredItemCallback deleteCallback
);
typedef fUA_Client_MonitoredItems_createDataChange* pUA_Client_MonitoredItems_createDataChange;
typedef void(WINAPI fUA_clear)
(
	void *p,
	const UA_DataType *type
);
typedef fUA_clear* pUA_clear;
typedef void*(WINAPI fUA_new)
(
	const UA_DataType *type
);
typedef fUA_new* pUA_new;
typedef void(WINAPI f__UA_Client_Service)
(
	UA_Client *client, 
	const void *request,
	const UA_DataType *requestType,
	void *response,
	const UA_DataType *responseType
);
typedef f__UA_Client_Service* p__UA_Client_Service;

typedef UA_String(WINAPI fUA_String_fromChars)
(
	const char *src
);
typedef fUA_String_fromChars* pUA_String_fromChars;

typedef void(WINAPI fUA_Variant_setScalar)
(
	UA_Variant *v,
	void * UA_RESTRICT p,
	const UA_DataType *type
);
typedef fUA_Variant_setScalar* pUA_Variant_setScalar;

typedef UA_StatusCode(WINAPI f__UA_Client_writeAttribute)
(
	UA_Client *client, 
	const UA_NodeId *nodeId,
	UA_AttributeId attributeId, 
	const void *in,
	const UA_DataType *inDataType
);
typedef f__UA_Client_writeAttribute* p__UA_Client_writeAttribute;
class COPCClientController: public COPCProcessBase
{
public:
	COPCClientController();
	virtual ~COPCClientController();
	void DoClientCallBack(UA_Client *client, UA_UInt32 subId, void *subContext,
		UA_UInt32 monId, void *monContext, UA_DataValue *value);
protected:
	void WriteFloatField(UA_NodeId& xNodeId, float fData);
	void WriteIntField(UA_NodeId& xNodeId, WORD wData);
	UA_StatusCode WriteBOOLField(UA_NodeId& xNodeId, BOOL bData);
	void WriteStringField(UA_NodeId& xNodeId, char *pStr);
	void ON_SET_MONITOR_NDOE(vector<CString> vMonitor);
	virtual void ON_OPEN_OPC(LPARAM lp);
	virtual void ON_CLOSE_OPC();
	virtual void SET_MONITOR_ID(CString strKey, UA_NodeId xNodeId, int nMonId) = 0;
	virtual void UpdateNode(int nMonId, const UA_DataValue *pValue) = 0;
private:
	UA_StatusCode WriteOPCField(UA_Variant&xInput, UA_NodeId &xNodeId);
	void Init();
	void Finalize();

	void LIB_INIT();
	void LIB_LOAD();
	void LIB_FREE();
	static void CALLBACK OnTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer);
	void ProcessTimer(UINT_PTR nEventId);
	void GetMonitorNodes(vector<std::pair<CString, UA_NodeId>>& vNode);
	void DiscoverNode();
	static void stateCallback(UA_Client *client, UA_ClientState eState);
	void DoStateCallback(UA_Client *client, UA_ClientState eState);
	void Connect();
private:
	UA_Client *m_pClient;
	UINT_PTR m_tTimer;
	UINT_PTR m_tReconnectTimer;
	CString m_strNodePath;
	CStringA m_strConnectURL;
	vector<CString> m_vMonitorNode;
	BOOL m_bForceClose;
private:
	HMODULE m_hDL_Dll_OPEN62541;
	pUA_Client_new m_pUA_Client_new;
	pUA_Client_getConfig m_pUA_Client_getConfig;
	pUA_ClientConfig_setDefault m_pUA_ClientConfig_setDefault;
	pUA_Client_connect m_pUA_Client_connect;
	pUA_Client_disconnect m_pUA_Client_disconnect;
	pUA_Client_delete m_pUA_Client_delete;
	pUA_Client_run_iterate m_pUA_Client_run_iterate;
	//pUA_Array_delete m_pUA_Array_delete;
	pUA_Client_Subscriptions_create m_pUA_Client_Subscriptions_create;
	pUA_Client_MonitoredItems_createDataChange m_pUA_Client_MonitoredItems_createDataChange;
	pUA_clear m_pUA_clear;
	pUA_new m_pUA_new;
	p__UA_Client_Service m_p__UA_Client_Service;
	UA_DataType* m_pUA_TYPES;
	pUA_String_fromChars m_pUA_String_fromChars;
	pUA_Variant_setScalar m_pUA_Variant_setScalar;
	p__UA_Client_writeAttribute m_p__UA_Client_writeAttribute;
	int m_nRootNS;
	CStringA m_strRootID;
};