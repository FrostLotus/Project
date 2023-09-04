
// MFCApplication1Dlg.cpp : 實作檔
//

#include "stdafx.h"
#include "MFCApplication1.h"
#include "MFCApplication1Dlg.h"
#include "afxdialogex.h"

#pragma comment(lib,"libopen62541.lib")
#pragma comment(lib,"ws2_32.lib")

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

#define TIMER_ID		0
#define TIMER_GAP		500
#define CLIENT_LOOP		50
#define MY_SERVER_PORT	4841
#define DISCOVERY_SERVER_ENDPOINT		"opc.tcp://127.0.0.1:49320"
#define DISCOVERY_MY_SERVER_ENDPOINT	"opc.tcp://localhost:4841"
// 對 App About 使用 CAboutDlg 對話方塊

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// 對話方塊資料
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支援

// 程式碼實作
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// CMFCApplication1Dlg 對話方塊

CMFCApplication1Dlg::CMFCApplication1Dlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CMFCApplication1Dlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_pClient = NULL;
}

CMFCApplication1Dlg::~CMFCApplication1Dlg()
{
}
void CMFCApplication1Dlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CMFCApplication1Dlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDOK, &CMFCApplication1Dlg::OnBnClickedClient)
	ON_WM_TIMER()
	ON_BN_CLICKED(IDC_DISCOVER, &CMFCApplication1Dlg::OnBnClickedDiscover)
	ON_BN_CLICKED(IDC_BUTTON_SERVER, &CMFCApplication1Dlg::OnBnClickedButtonServer)
	ON_BN_CLICKED(IDC_BUTTON_STOP, &CMFCApplication1Dlg::OnBnClickedButtonStop)
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDCANCEL, &CMFCApplication1Dlg::OnBnClickedCancel)
END_MESSAGE_MAP()


// CMFCApplication1Dlg 訊息處理常式

BOOL CMFCApplication1Dlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 將 [關於...] 功能表加入系統功能表。

	// IDM_ABOUTBOX 必須在系統命令範圍之中。
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// 設定此對話方塊的圖示。當應用程式的主視窗不是對話方塊時，
	// 框架會自動從事此作業
	SetIcon(m_hIcon, TRUE);			// 設定大圖示
	SetIcon(m_hIcon, FALSE);		// 設定小圖示

	// TODO:  在此加入額外的初始設定
	GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(FALSE);

	return TRUE;  // 傳回 TRUE，除非您對控制項設定焦點
}

void CMFCApplication1Dlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// 如果將最小化按鈕加入您的對話方塊，您需要下列的程式碼，
// 以便繪製圖示。對於使用文件/檢視模式的 MFC 應用程式，
// 框架會自動完成此作業。

void CMFCApplication1Dlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 繪製的裝置內容

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// 將圖示置中於用戶端矩形
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// 描繪圖示
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

// 當使用者拖曳最小化視窗時，
// 系統呼叫這個功能取得游標顯示。
HCURSOR CMFCApplication1Dlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

void CMFCApplication1Dlg::OnTimer(UINT_PTR nIDEvent)
{
	if (nIDEvent == TIMER_ID)
	{
		/* Take another look at the.answer */
		if (m_pClient && !m_bUABusy)
		{
			m_bUABusy = TRUE;
			UA_Client_run_iterate(m_pClient, CLIENT_LOOP);
			m_bUABusy = FALSE;
		}
	}
	CDialogEx::OnTimer(nIDEvent);
}
static void ClientCallBack(UA_Client *client, UA_UInt32 subId, void *subContext,
UA_UInt32 monId, void *monContext, UA_DataValue *value)
{
	TRACE("Client CallBack: The Answer has changed to %i!\n", *(UA_Int32*)value->value.data);
}
void CMFCApplication1Dlg::ClientSubScript()
{
	UA_CreateSubscriptionRequest request = UA_CreateSubscriptionRequest_default();
	UA_CreateSubscriptionResponse response = UA_Client_Subscriptions_create(m_pClient, request,
		NULL, NULL, NULL);

	m_nSubId = response.subscriptionId;
	if (response.responseHeader.serviceResult == UA_STATUSCODE_GOOD)
		TRACE("Create subscription succeeded, id %u\n", m_nSubId);

	UA_MonitoredItemCreateRequest monRequest =
		UA_MonitoredItemCreateRequest_default(UA_NODEID_STRING(1, "the.answer"));

	UA_MonitoredItemCreateResult monResponse =
		UA_Client_MonitoredItems_createDataChange(m_pClient, response.subscriptionId,
		UA_TIMESTAMPSTORETURN_BOTH,
		monRequest, NULL, ClientCallBack, NULL);
	if (monResponse.statusCode == UA_STATUSCODE_GOOD)
		TRACE("Monitoring 'the.answer', id %u\n", monResponse.monitoredItemId);
	UA_Client_run_iterate(m_pClient, 1000);
}
void CMFCApplication1Dlg::ClientModifyValue()
{
	UA_StatusCode dwRetval = UA_STATUSCODE_GOOD;
	/* The first publish request should return the initial value of the variable */
	/* Read attribute */
	UA_Int32 value = 0;
	//printf("\nReading the value of node (1, \"the.answer\"):\n");
	UA_Variant *val = UA_Variant_new();
	dwRetval = UA_Client_readValueAttribute(m_pClient, UA_NODEID_STRING(1, "the.answer"), val);
	if (dwRetval == UA_STATUSCODE_GOOD && UA_Variant_isScalar(val) &&
		val->type == &UA_TYPES[UA_TYPES_INT32]) {
		value = *(UA_Int32*)val->data;
		TRACE("the value is: %i\n", value);
	}
	UA_Variant_delete(val);

	/* Write node attribute (using the highlevel API) */
	value++;
	UA_Variant *myVariant = UA_Variant_new();
	UA_Variant_setScalarCopy(myVariant, &value, &UA_TYPES[UA_TYPES_INT32]);
	UA_Client_writeValueAttribute(m_pClient, UA_NODEID_STRING(1, "the.answer"), myVariant);
	UA_Variant_delete(myVariant);
}
UA_Client *CMFCApplication1Dlg::ConnectServer()
{
	UA_StatusCode dwRetval = UA_STATUSCODE_GOOD;
	m_pClient = UA_Client_new();
	UA_ClientConfig_setDefault(UA_Client_getConfig(m_pClient));
	if (m_bServerRunning)
	{
		TRACE("Connect to my server on port %i\n", MY_SERVER_PORT);
		dwRetval = UA_Client_connect(m_pClient, DISCOVERY_MY_SERVER_ENDPOINT);
	}
	else
	{
		TRACE("Connect to the default server\n");
		dwRetval = UA_Client_connect(m_pClient, DISCOVERY_SERVER_ENDPOINT);
		//dwRetval = UA_Client_connect_username(m_pClient, DISCOVERY_SERVER_ENDPOINT, "user1", "password");
	}
	if (dwRetval == UA_STATUSCODE_GOOD)
		SetTimer(TIMER_ID, TIMER_GAP, NULL);
	else
	{
		UA_Client_delete(m_pClient);
		m_pClient = NULL;
	}
	return m_pClient;
}

void CMFCApplication1Dlg::OnBnClickedClient()
{
	// TODO:  在此加入控制項告知處理常式程式碼
	__try
	{
		if (m_bUABusy)
			return;
		if (!m_pClient)
		{
			if (!ConnectServer())
				return;
		}
	}
	__finally
	{
		if (!m_bUABusy && m_pClient)
		{
			m_bUABusy = TRUE;
			if (m_nSubId == -1)
				ClientSubScript();

			if (m_nSubId != -1)
				ClientModifyValue();

			m_bUABusy = FALSE;
		}
	}
}

void CMFCApplication1Dlg::OnBnClickedDiscover()
{
	// TODO:  在此加入控制項告知處理常式程式碼
	UA_Client *client = UA_Client_new();
	UA_ClientConfig_setDefault(UA_Client_getConfig(client));
	/* Listing endpoints */
	UA_EndpointDescription* endpointArray = NULL;
	size_t endpointArraySize = 0;
	UA_BrowseResponse bResp = { 0 };
	UA_BrowseRequest bReq = { 0 };

	__try
	{
		UA_StatusCode retval = UA_STATUSCODE_GOOD;
		if (m_bServerRunning)
			retval = UA_Client_getEndpoints(client, DISCOVERY_MY_SERVER_ENDPOINT,
												&endpointArraySize, &endpointArray);
		else
			retval = UA_Client_getEndpoints(client, DISCOVERY_SERVER_ENDPOINT,
												&endpointArraySize, &endpointArray);
		if (retval != UA_STATUSCODE_GOOD)
			return;
		TRACE("%i endpoints found\n", (int)endpointArraySize);
		for (size_t i = 0; i < endpointArraySize; i++) {
			TRACE("URL of endpoint %i is %.*s\n", (int)i,
				(int)endpointArray[i].endpointUrl.length,
				endpointArray[i].endpointUrl.data);
		}
		/* Browse some objects */
		TRACE("Browsing nodes in objects folder:\n");
		UA_BrowseRequest_init(&bReq);
		bReq.requestedMaxReferencesPerNode = 0;
		bReq.nodesToBrowse = UA_BrowseDescription_new();
		bReq.nodesToBrowseSize = 1;
		//bReq.nodesToBrowse[0].nodeId = UA_NODEID_STRING(2, "Channel1.Device1"); /* browse objects folder */
		bReq.nodesToBrowse[0].nodeId = UA_NODEID_NUMERIC(0, UA_NS0ID_OBJECTSFOLDER); /* browse objects folder */
		bReq.nodesToBrowse[0].resultMask = UA_BROWSERESULTMASK_ALL; /* return everything */
		if (!ConnectServer())
			return;
		UA_Variant varOutValue = { 0 };
		UA_Variant_init(&varOutValue);
		retval = UA_Client_readValueAttribute(m_pClient, UA_NODEID_STRING(2, "Channel1.Device1.Tag1"), &varOutValue);
		variant_t *p = (variant_t *)varOutValue.data;
		UA_Variant_deleteMembers(&varOutValue);

		bResp = UA_Client_Service_browse(m_pClient, bReq);
		TRACE("%-9s %-16s %-16s %-16s\n", "NAMESPACE", "NODEID", "BROWSE NAME", "DISPLAY NAME");
		for (size_t i = 0; i < bResp.resultsSize; ++i) {
			for (size_t j = 0; j < bResp.results[i].referencesSize; ++j) {
				UA_ReferenceDescription *ref = &(bResp.results[i].references[j]);
				if (ref->nodeId.nodeId.identifierType == UA_NODEIDTYPE_NUMERIC) {
					TRACE("%-9d %-16d %-16.*s %-16.*s\n", ref->nodeId.nodeId.namespaceIndex,
						ref->nodeId.nodeId.identifier.numeric, (int)ref->browseName.name.length,
						ref->browseName.name.data, (int)ref->displayName.text.length,
						ref->displayName.text.data);
				}
				else if (ref->nodeId.nodeId.identifierType == UA_NODEIDTYPE_STRING) {
					TRACE("%-9d %-16.*s %-16.*s %-16.*s\n", ref->nodeId.nodeId.namespaceIndex,
						(int)ref->nodeId.nodeId.identifier.string.length,
						ref->nodeId.nodeId.identifier.string.data,
						(int)ref->browseName.name.length, ref->browseName.name.data,
						(int)ref->displayName.text.length, ref->displayName.text.data);
				}
				/* TODO: distinguish further types */
			}
		}
	}
	__finally
	{
		UA_BrowseRequest_clear(&bReq);
		UA_BrowseResponse_clear(&bResp);
		if (endpointArraySize)
			UA_Array_delete(endpointArray, endpointArraySize, &UA_TYPES[UA_TYPES_ENDPOINTDESCRIPTION]);
		if (client)
			UA_Client_delete(client);
	}
}
static void ServerCallback(UA_Server *server, UA_UInt32 monitoredItemId,
void *monitoredItemContext, const UA_NodeId *nodeId,
void *nodeContext, UA_UInt32 attributeId,
const UA_DataValue *value) 
{
	TRACE("Server CallBack: The Answer has changed to %i!\n", *(UA_Int32*)value->value.data);
}

void CMFCApplication1Dlg::AddServerMonitoredItem(UA_Server *server)
{
	UA_NodeId VarNodeId = UA_NODEID_STRING_ALLOC(1, "the.answer");
	//UA_NodeId VarNodeId = UA_NODEID_NUMERIC(0, UA_NS0ID_SERVER_SERVERSTATUS_CURRENTTIME);
	UA_MonitoredItemCreateRequest monRequest =
		UA_MonitoredItemCreateRequest_default(VarNodeId);
	monRequest.requestedParameters.samplingInterval = 1000.0; /* 100 ms interval */
	UA_Server_createDataChangeMonitoredItem(server, UA_TIMESTAMPSTORETURN_SOURCE,
		monRequest, NULL, ServerCallback);
}

UINT MyServerProc(LPVOID pParam)
{
	CMFCApplication1Dlg *pObject = (CMFCApplication1Dlg *) pParam;
	if (pObject)
	{
		UA_Server *server = UA_Server_new();
		UA_ServerConfig_setMinimal(UA_Server_getConfig(server), MY_SERVER_PORT, NULL);
		pObject->AddServerVariable(server);
		pObject->AddServerMonitoredItem(server);

		pObject->m_bServerRunning = true;
		pObject->m_bServerStop = false;
		TRACE("My Server is running on port %i!\n", MY_SERVER_PORT);
		UA_StatusCode retval = UA_Server_run(server, &pObject->m_bServerRunning);
		TRACE("My Server is stopped!\n");
		pObject->m_bServerStop = true;
		UA_Server_delete(server);
	}
	return 0;
}
void CMFCApplication1Dlg::AddServerVariable(UA_Server *server)
{
	/* Define the attribute of the myInteger variable node */
	UA_VariableAttributes attr = UA_VariableAttributes_default;
	UA_Int32 myInteger = 101;
	UA_Variant_setScalar(&attr.value, &myInteger, &UA_TYPES[UA_TYPES_INT32]);
	attr.description = UA_LOCALIZEDTEXT("en-US", "the answer");
	attr.displayName = UA_LOCALIZEDTEXT("en-US", "the answer");
	attr.dataType = UA_TYPES[UA_TYPES_INT32].typeId;
	attr.accessLevel = UA_ACCESSLEVELMASK_READ | UA_ACCESSLEVELMASK_WRITE;

	/* Add the variable node to the information model */
	UA_NodeId myIntegerNodeId = UA_NODEID_STRING(1, "the.answer");
	UA_QualifiedName myIntegerName = UA_QUALIFIEDNAME(1, "the answer");
	UA_NodeId parentNodeId = UA_NODEID_NUMERIC(0, UA_NS0ID_OBJECTSFOLDER);
	UA_NodeId parentReferenceNodeId = UA_NODEID_NUMERIC(0, UA_NS0ID_ORGANIZES);
	UA_Server_addVariableNode(server, myIntegerNodeId, parentNodeId,
		parentReferenceNodeId, myIntegerName,
		UA_NODEID_NUMERIC(0, UA_NS0ID_BASEDATAVARIABLETYPE), attr, NULL, NULL);
}
void CMFCApplication1Dlg::OnBnClickedButtonServer()
{
	// TODO:  在此加入控制項告知處理常式程式碼
	m_bServerRunning = false;
	AfxBeginThread(MyServerProc, this);
	for (int i = 0; !m_bServerRunning && i < 100; i++)
		Sleep(1000);
	if (m_bServerRunning)
	{
		SetDlgItemText(IDC_STATIC_SERVER_STATUS, _T("Server is running"));
		SetDlgItemText(IDC_STATIC_TARGET_SERVER, _T("Target:Internal Server"));
		GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(TRUE);
	}
}

void CMFCApplication1Dlg::OnBnClickedButtonStop()
{
	// TODO:  在此加入控制項告知處理常式程式碼
	if (m_bServerRunning)
	{
		m_bServerRunning = false;
		for (int i = 0; !m_bServerStop && i < 500; i++)
			Sleep(200);
		if (m_bServerStop)
		{
			SetDlgItemText(IDC_STATIC_SERVER_STATUS, _T("Server is stopped"));
			SetDlgItemText(IDC_STATIC_TARGET_SERVER, _T("Target:External Server"));
			GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(FALSE);
		}
	}
}


void CMFCApplication1Dlg::OnClose()
{
	// TODO:  在此加入您的訊息處理常式程式碼和 (或) 呼叫預設值
	KillTimer(TIMER_ID);
	if (m_pClient)
	{
		UA_Client_disconnect(m_pClient);
		UA_Client_delete(m_pClient); /* Disconnects the client internally */
		/* Delete the subscription */
		if (m_nSubId > -1 && UA_Client_Subscriptions_deleteSingle(m_pClient, m_nSubId) == UA_STATUSCODE_GOOD)
			TRACE("Subscription removed\n");
	}
	OnBnClickedButtonStop();
	CDialogEx::OnClose();
}


void CMFCApplication1Dlg::OnBnClickedCancel()
{
	// TODO:  在此加入控制項告知處理常式程式碼
	OnClose();
	CDialogEx::OnCancel();
}
