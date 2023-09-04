
// OPCServerDlg.cpp : 實作檔
//

#include "stdafx.h"
#include "OPCServer.h"
#include "OPCServerDlg.h"
#include "afxdialogex.h"
#include "atlstr.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

#pragma comment(lib,"libopen62541.lib")
#pragma comment(lib,"ws2_32.lib")


#define MAX_VARIABLE		100
#define MAX_ONCE_QUEUE_SIZE	2
#define INI_FILE_NAME	_T(".\\OPCServer.ini")

#define INI_VARIABLE	_T("Variable")
#define INI_ITEM_NAME	_T("ItemName")
#define	INI_NODE_ID		_T("NodeId")
#define	INI_DATA_TYPE	_T("DataType")
#define INI_INITIAL		_T("Initial")
#define INI_LENGTH		_T("Length")

#define INI_TYPE_STRING	_T("String")
#define INI_TYPE_WORD	_T("Word")
#define INI_TYPE_BOOL	_T("Bool")

static COPCServerDlg *g_pOPCServer = NULL;
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


// COPCServerDlg 對話方塊



COPCServerDlg::COPCServerDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(COPCServerDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	InitializeCriticalSection(&m_csSection);
}

void COPCServerDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST_VAR, m_listVar);
	DDX_Control(pDX, IDC_COMBO_TYPE, m_cmbType);
}

BEGIN_MESSAGE_MAP(COPCServerDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON_SERVER, &COPCServerDlg::OnBnClickedButtonServer)
	ON_BN_CLICKED(IDC_BUTTON_STOP, &COPCServerDlg::OnBnClickedButtonStop)
	ON_BN_CLICKED(IDCANCEL, &COPCServerDlg::OnBnClickedCancel)
	ON_WM_CLOSE()
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_LIST_VAR, &COPCServerDlg::OnLvnItemchangedListVar)
	ON_BN_CLICKED(ID_BUTTON_UPDATE, &COPCServerDlg::OnBnClickedButtonUpdate)
	ON_BN_CLICKED(IDC_BUTTON_ADD, &COPCServerDlg::OnBnClickedButtonAdd)
	ON_BN_CLICKED(ID_BUTTON_SAVE_FILE, &COPCServerDlg::OnBnClickedButtonSaveFile)
	ON_BN_CLICKED(IDC_BUTTON_DELETE, &COPCServerDlg::OnBnClickedButtonDelete)
END_MESSAGE_MAP()


// COPCServerDlg 訊息處理常式

BOOL COPCServerDlg::OnInitDialog()
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

	//ShowWindow(SW_MINIMIZE);

	// TODO:  在此加入額外的初始設定
	g_pOPCServer = this;
	m_cmbType.ResetContent();
	m_cmbType.AddString(INI_TYPE_STRING);
	m_cmbType.AddString(INI_TYPE_WORD);
	m_cmbType.AddString(INI_TYPE_BOOL);
	m_cmbType.SetCurSel(0);
	m_listVar.SetExtendedStyle(m_listVar.GetExtendedStyle() | LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);
	InsertItemFromIniFile();
	return TRUE;  // 傳回 TRUE，除非您對控制項設定焦點
}

void COPCServerDlg::OnSysCommand(UINT nID, LPARAM lParam)
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

void COPCServerDlg::OnPaint()
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
HCURSOR COPCServerDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}
void COPCServerDlg::InsertItemFromIniFile()
{
	TCHAR tszSection[MAX_PATH] = { 0 };
	TCHAR tszItemName[MAX_PATH];
	TCHAR tszNodeId[MAX_PATH];
	TCHAR tszDataType[MAX_PATH];
	TCHAR tszInitial[MAX_PATH];
	WORD  wInitialValue;
	DWORD dwRtn;
	int	  nLength;
	UA_NODE uaItem;

	m_bReadIni = TRUE;
	CRect rect;
	m_listVar.GetClientRect(&rect);
	int nColInterval = rect.Width() / 4;

	m_listVar.InsertColumn(0, _T("Variable"), LVCFMT_LEFT, nColInterval * 3 / 4);
	m_listVar.InsertColumn(1, _T("Item Name"), LVCFMT_LEFT, nColInterval);
	m_listVar.InsertColumn(2, _T("Node Id"), LVCFMT_LEFT, nColInterval);
	m_listVar.InsertColumn(3, _T("Value"), LVCFMT_LEFT, rect.Width() - 11 * nColInterval / 4);

	m_nServerPort = GetPrivateProfileIntW(_T("Server"), _T("Port"), MY_SERVER_PORT, INI_FILE_NAME);
	
	int nPos = 0;
	for (int i = 1; i < MAX_VARIABLE; i++)
	{
		_stprintf(tszSection, _T("%s%03i"), INI_VARIABLE, i);
		dwRtn = GetPrivateProfileStringW(tszSection, INI_ITEM_NAME, NULL, tszItemName, MAX_PATH, INI_FILE_NAME);
		if (!dwRtn)
			continue;
		dwRtn = GetPrivateProfileStringW(tszSection, INI_NODE_ID, NULL, tszNodeId, MAX_PATH, INI_FILE_NAME);
		if (!dwRtn)
			continue;
		dwRtn = GetPrivateProfileStringW(tszSection, INI_DATA_TYPE, NULL, tszDataType, MAX_PATH, INI_FILE_NAME);
		if (!dwRtn)
			continue;
		if (_tcsicmp(tszDataType, INI_TYPE_STRING) == 0)
		{
			GetPrivateProfileStringW(tszSection, INI_INITIAL, NULL, tszInitial, MAX_PATH, INI_FILE_NAME);
			nLength = GetPrivateProfileIntW(tszSection, INI_LENGTH, 0, INI_FILE_NAME);
		}
		else
		{
			wInitialValue = GetPrivateProfileIntW(tszSection, INI_INITIAL, 0, INI_FILE_NAME);
			_stprintf(tszInitial, _T("%i"), wInitialValue);
			nLength = 0;
		}
		_stprintf(tszSection, _T("%03i"), i);
		nPos = m_listVar.InsertItem(nPos, tszSection);
		m_listVar.SetItemText(nPos, 1, tszItemName);
		m_listVar.SetItemText(nPos, 2, tszNodeId);
		m_listVar.SetItemText(nPos, 3, tszInitial);
		m_listVar.SetItemData(nPos, i);

		ZeroMemory(&uaItem, sizeof(uaItem));
		uaItem.nIndex = i;
		_tcscpy_s(uaItem.tszItemName, tszItemName);
		_tcscpy_s(uaItem.tszNodeId, tszNodeId);
		_tcscpy_s(uaItem.tszDataType, tszDataType);
		uaItem.nLength = 0;
		if (_tcsicmp(tszDataType, INI_TYPE_STRING) == 0)
		{
			_tcscpy_s(uaItem.tszInitial, tszInitial);
			uaItem.nLength = nLength;
		}
		else
			uaItem.wInitialValue = wInitialValue;
		m_vecNode.push_back(uaItem);
	}
	m_bReadIni = FALSE;
}
UINT MyServerProc(LPVOID pParam)
{
	COPCServerDlg *pObject = (COPCServerDlg *)pParam;
	if (pObject)
	{
		pObject->m_pServer = UA_Server_new();
		UA_ServerConfig_setMinimal(UA_Server_getConfig(pObject->m_pServer), pObject->m_nServerPort, NULL);
		pObject->AddServerVariable(pObject->m_pServer);
		pObject->AddServerMonitoredItem(pObject->m_pServer);

		pObject->m_bServerRunning = true;
		pObject->m_bServerStop = false;
		TRACE("My Server is running on port %i!\n", pObject->m_nServerPort);
		UA_StatusCode retval = UA_Server_run(pObject->m_pServer, &pObject->m_bServerRunning);
		TRACE("My Server is stopped!\n");
		pObject->m_bServerStop = true;
		UA_Server_delete(pObject->m_pServer);
		pObject->m_pServer = NULL;
	}
	return 0;
}

void COPCServerDlg::OnBnClickedButtonServer()
{
	// TODO:  在此加入控制項告知處理常式程式碼
	m_bServerRunning = false;
	AfxBeginThread(MyServerProc, this);
	for (int i = 0; !m_bServerRunning && i < 100; i++)
		Sleep(1000);
	if (m_bServerRunning)
	{
		TCHAR tszText[MAX_PATH];
		_stprintf(tszText, _T("Server is running on %i port"), m_nServerPort);
		SetDlgItemText(IDC_STATIC_SERVER_STATUS, tszText);

		POSITION pos = m_listVar.GetFirstSelectedItemPosition();
		int nPos = (int)pos;
	}
	POSITION pos = m_listVar.GetFirstSelectedItemPosition();
	if (pos > 0)
	{
		GetDlgItem(ID_BUTTON_UPDATE)->EnableWindow(m_bServerRunning);
		GetDlgItem(IDC_BUTTON_DELETE)->EnableWindow(!m_bServerRunning);
		GetDlgItem(ID_BUTTON_SAVE_FILE)->EnableWindow(!m_bServerRunning);
	}
	GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(m_bServerRunning);
	GetDlgItem(IDC_EDIT_VALUE)->EnableWindow(m_bServerRunning);
	GetDlgItem(IDC_BUTTON_SERVER)->EnableWindow(!m_bServerRunning);
	GetDlgItem(IDC_BUTTON_ADD)->EnableWindow(!m_bServerRunning);
	GetDlgItem(IDC_EDIT_VAR_NUM)->EnableWindow(!m_bServerRunning);
	GetDlgItem(IDC_EDIT_ITEM_NAME)->EnableWindow(!m_bServerRunning);
	GetDlgItem(IDC_EDIT_NODE_ID)->EnableWindow(!m_bServerRunning);
	GetDlgItem(IDC_EDIT_LENGTH)->EnableWindow(!m_bServerRunning);
	GetDlgItem(IDC_EDIT_INITIAL)->EnableWindow(!m_bServerRunning);
	m_cmbType.EnableWindow(!m_bServerRunning);
}

void COPCServerDlg::OnBnClickedButtonStop()
{
	// TODO:  在此加入控制項告知處理常式程式碼
	if (m_bServerRunning)
	{
		m_bServerRunning = false;
		for (int i = 0; !m_bServerStop && i < 500; i++)
			Sleep(200);
		if (m_bServerStop)
			SetDlgItemText(IDC_STATIC_SERVER_STATUS, _T("Server is stopped"));

		POSITION pos = m_listVar.GetFirstSelectedItemPosition();
		if (pos > 0)
		{
			GetDlgItem(ID_BUTTON_UPDATE)->EnableWindow(m_bServerRunning);
			GetDlgItem(IDC_BUTTON_DELETE)->EnableWindow(!m_bServerRunning);
			GetDlgItem(ID_BUTTON_SAVE_FILE)->EnableWindow(!m_bServerRunning);
		}
		GetDlgItem(IDC_BUTTON_SERVER)->EnableWindow(!m_bServerRunning);
		GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(m_bServerRunning);
		GetDlgItem(IDC_EDIT_VALUE)->EnableWindow(m_bServerRunning);
		GetDlgItem(IDC_BUTTON_ADD)->EnableWindow(!m_bServerRunning);
		GetDlgItem(IDC_EDIT_VAR_NUM)->EnableWindow(!m_bServerRunning);
		GetDlgItem(IDC_EDIT_ITEM_NAME)->EnableWindow(!m_bServerRunning);
		GetDlgItem(IDC_EDIT_NODE_ID)->EnableWindow(!m_bServerRunning);
		GetDlgItem(IDC_EDIT_LENGTH)->EnableWindow(!m_bServerRunning);
		GetDlgItem(IDC_EDIT_INITIAL)->EnableWindow(!m_bServerRunning);
		m_cmbType.EnableWindow(!m_bServerRunning);
	}
}

void COPCServerDlg::OnBnClickedCancel()
{
	// TODO:  在此加入控制項告知處理常式程式碼
	OnClose();
	CDialogEx::OnCancel();
}

void COPCServerDlg::OnClose()
{
	// TODO:  在此加入您的訊息處理常式程式碼和 (或) 呼叫預設值
	OnBnClickedButtonStop();
	DeleteCriticalSection(&m_csSection);
	CDialogEx::OnClose();
}

int COPCServerDlg::GetNodeIndex(int nPos)
{
	if (m_vecNode.size() && nPos > 0)
	{
		for (size_t i = 0; i < m_vecNode.size(); i++)
			if (nPos == m_vecNode[i].nIndex)
			{
				nPos = (int)i;
				break;
			}
	}
	return nPos;
}

void COPCServerDlg::OnLvnItemchangedListVar(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMLISTVIEW pNMLV = reinterpret_cast<LPNMLISTVIEW>(pNMHDR);
	// TODO:  在此加入控制項告知處理常式程式碼
	*pResult = 0;
	if (!m_bReadIni && pNMLV->iItem != -1)
	{
		int nPos = (int)m_listVar.GetItemData(pNMLV->iItem);
		if (m_vecNode.size() && nPos > 0)
		{
			nPos = GetNodeIndex(nPos);
			TCHAR tszText[MAX_PATH];
			_stprintf(tszText, _T("%03i"), m_vecNode[nPos].nIndex);
			SetDlgItemText(IDC_EDIT_VAR_NUM, tszText);
			SetDlgItemText(IDC_EDIT_ITEM_NAME, m_vecNode[nPos].tszItemName);
			SetDlgItemText(IDC_EDIT_NODE_ID, m_vecNode[nPos].tszNodeId);

			SetDlgItemText(IDC_EDIT_VALUE, m_listVar.GetItemText(pNMLV->iItem, 3));
			if (_tcsicmp(m_vecNode[nPos].tszDataType, INI_TYPE_STRING) == 0)
			{
				SetDlgItemInt(IDC_EDIT_LENGTH, m_vecNode[nPos].nLength);
				SetDlgItemText(IDC_EDIT_INITIAL, m_vecNode[nPos].tszInitial);
				m_cmbType.SetCurSel(0);
			}
			else
			{
				SetDlgItemText(IDC_EDIT_LENGTH, _T(""));
				SetDlgItemInt(IDC_EDIT_INITIAL, m_vecNode[nPos].wInitialValue);
				if (_tcsicmp(m_vecNode[nPos].tszDataType, INI_TYPE_WORD) == 0)
					m_cmbType.SetCurSel(1);
				else if (_tcsicmp(m_vecNode[nPos].tszDataType, INI_TYPE_BOOL) == 0)
					m_cmbType.SetCurSel(2);
			}
			POSITION pos = m_listVar.GetFirstSelectedItemPosition();
			if (pos > 0)
			{
				GetDlgItem(ID_BUTTON_UPDATE)->EnableWindow(m_bServerRunning);
				GetDlgItem(IDC_BUTTON_DELETE)->EnableWindow(!m_bServerRunning);
				GetDlgItem(ID_BUTTON_SAVE_FILE)->EnableWindow(!m_bServerRunning);
			}
			GetDlgItem(IDC_BUTTON_ADD)->EnableWindow(!m_bServerRunning);
			GetDlgItem(IDC_EDIT_VAR_NUM)->EnableWindow(!m_bServerRunning);
			GetDlgItem(IDC_EDIT_ITEM_NAME)->EnableWindow(!m_bServerRunning);
			GetDlgItem(IDC_EDIT_NODE_ID)->EnableWindow(!m_bServerRunning);
			GetDlgItem(IDC_EDIT_LENGTH)->EnableWindow(!m_bServerRunning);
			GetDlgItem(IDC_EDIT_INITIAL)->EnableWindow(!m_bServerRunning);
			m_cmbType.EnableWindow(!m_bServerRunning);
		}
	}
}

void COPCServerDlg::OnBnClickedButtonUpdate()
{
	// TODO:  在此加入控制項告知處理常式程式碼
	POSITION pos = m_listVar.GetFirstSelectedItemPosition();
	TCHAR tszCurrentValue[MAX_PATH];
	char szText[MAX_PATH] = { 0 };
	GetDlgItemText(IDC_EDIT_VALUE, tszCurrentValue, MAX_PATH);

	if (pos > 0 && _tcslen(tszCurrentValue) && m_pServer && m_bServerRunning)
	{
		int nPos = GetNodeIndex((int)m_listVar.GetItemData((int)(pos - 1)));
		wcstombs(szText, m_vecNode[nPos].tszNodeId, MAX_PATH);
		UA_NodeId uaNode = UA_NODEID_STRING_ALLOC(1, szText);

		UA_Variant uaVar;
		UA_Variant_init(&uaVar);
		if (_tcsicmp(m_vecNode[nPos].tszDataType, INI_TYPE_STRING) == 0)
		{
			wcstombs(szText, tszCurrentValue, MAX_PATH);
			UA_String uaString;
			uaString = UA_STRING_ALLOC(szText);
			UA_Variant_setScalar(&uaVar, &uaString, &UA_TYPES[UA_TYPES_STRING]);
		}
		else if (_tcsicmp(m_vecNode[nPos].tszDataType, INI_TYPE_BOOL) == 0)
		{
			UA_Boolean uaBool;
			uaBool = _ttoi(tszCurrentValue);
			UA_Variant_setScalar(&uaVar, &uaBool, &UA_TYPES[UA_TYPES_BOOLEAN]);
		}
		else if (_tcsicmp(m_vecNode[nPos].tszDataType, INI_TYPE_WORD) == 0)
		{
			UA_UInt16 uaWord;
			uaWord = _ttoi(tszCurrentValue);
			UA_Variant_setScalar(&uaVar, &uaWord, &UA_TYPES[UA_TYPES_UINT16]);
			_itot(uaWord, tszCurrentValue, 10);
		}
		UA_Server_writeValue(m_pServer, uaNode, uaVar);
	}
}

static int GetNodeIndex(const UA_NodeId *nodeId)
{
	TCHAR tszText[MAX_PATH];
	UA_String uaData;
	if (nodeId && g_pOPCServer)
	{
		uaData = nodeId->identifier.string;
		size_t nSize = mbstowcs(tszText, (char *)uaData.data, uaData.length);
		tszText[nSize] = _T('\0');
		for (size_t i = 0; i < g_pOPCServer->m_vecNode.size(); i++)
		{
			if (_tcscmp(tszText, g_pOPCServer->m_vecNode[i].tszNodeId) == 0)
				return (int)i;
		}
	}
	return -1;
}
static void ServerCallback(UA_Server *server, UA_UInt32 monitoredItemId,
	void *monitoredItemContext, const UA_NodeId *nodeId,
	void *nodeContext, UA_UInt32 attributeId,
	const UA_DataValue *value)
{
	int nTypeKind = value->value.type->typeKind;
	int nPos = GetNodeIndex(nodeId);
	UA_MONITOR mtTemp = { 0 };
	char szData[MAX_PATH] = { 0 };
	BOOL bNext = FALSE;
	sprintf(szData, "Enter ServerCallback g_pOPCServer =%x, nPos=%i\n", g_pOPCServer, nPos);
	OutputDebugStringA(szData);
	if (g_pOPCServer && nPos > -1)
	{
		if (nTypeKind == UA_DATATYPEKIND_STRING)
		{
			UA_String uaData = *((UA_String*)value->value.data);
			if (uaData.length <= g_pOPCServer->m_vecNode[nPos].nLength)
			{
				size_t nSize = mbstowcs(mtTemp.szText, (char *)uaData.data, uaData.length);
				mtTemp.szText[nSize] = _T('\0');
				bNext = TRUE;
				//if (nSize != uaData.length)
				//{
					sprintf(szData, "ServerCallback uaData.length=%i, nSize=%i", uaData.length, nSize);
					OutputDebugStringA(szData);
				//}
			}
		}
		else if (nTypeKind == UA_DATATYPEKIND_UINT16)
		{
			UA_Int16 wData = *(UA_Int16 *)value->value.data;
			_itot(wData, mtTemp.szText, 10);
			bNext = TRUE;
		}
		else if (nTypeKind == UA_DATATYPEKIND_BOOLEAN)
		{
			UA_Boolean bData = *(UA_Boolean*)value->value.data;
			_itot(bData ? 1 : 0, mtTemp.szText, 10);
			bNext = TRUE;
		}
		if (bNext)
		{
			mtTemp.nIndex = nPos;
			EnterCriticalSection(&g_pOPCServer->m_csSection);
			g_pOPCServer->m_queMonitor.push(mtTemp);
			LeaveCriticalSection(&g_pOPCServer->m_csSection);
			PostMessage(g_pOPCServer->m_hWnd, WM_UPDATEUISTATE, 0, 0);
		}
	}
}

void COPCServerDlg::AddServerMonitoredItem(UA_Server *server)
{
	UA_NodeId VarNodeId;
	UA_MonitoredItemCreateRequest monRequest;
	char szText[MAX_PATH];
	for (size_t i = 0; i < m_vecNode.size(); i++)
	{
		ZeroMemory(&monRequest, sizeof(monRequest));
		wcstombs(szText, m_vecNode[i].tszNodeId, MAX_PATH);
		VarNodeId = UA_NODEID_STRING_ALLOC(1, szText);
		monRequest = UA_MonitoredItemCreateRequest_default(VarNodeId);
		monRequest.requestedParameters.samplingInterval = 1000.0; /* 100 ms interval */
		UA_Server_createDataChangeMonitoredItem(server, UA_TIMESTAMPSTORETURN_SOURCE,
			monRequest, NULL, ServerCallback);
	}
}
void COPCServerDlg::AddServerVariable(UA_Server *server)
{
	/* Define the attribute of the myInteger variable node */
	UA_VariableAttributes attr = UA_VariableAttributes_default;
	char szText[MAX_PATH];
	for (size_t i = 0; i < m_vecNode.size(); i++)
	{
		ZeroMemory(&attr, sizeof(attr));
		wcstombs(szText, m_vecNode[i].tszNodeId, MAX_PATH);
		attr.description = UA_LOCALIZEDTEXT_ALLOC("en-US", szText);
		attr.displayName = UA_LOCALIZEDTEXT_ALLOC("en-US", szText);
		attr.accessLevel = UA_ACCESSLEVELMASK_READ | UA_ACCESSLEVELMASK_WRITE;

		if (_tcsicmp(m_vecNode[i].tszDataType, INI_TYPE_STRING) == 0)
		{
			UA_String uaString;
			wcstombs(szText, m_vecNode[i].tszInitial, MAX_PATH);
			uaString = UA_STRING_ALLOC(szText);
			attr.dataType = UA_TYPES[UA_TYPES_STRING].typeId;
			UA_Variant_setScalar(&attr.value, &uaString, &UA_TYPES[UA_TYPES_STRING]);
		}
		else if (_tcsicmp(m_vecNode[i].tszDataType, INI_TYPE_BOOL) == 0)
		{
			UA_Boolean uaBool;
			uaBool = (UA_Boolean)m_vecNode[i].wInitialValue;
			attr.dataType = UA_TYPES[UA_TYPES_BOOLEAN].typeId;
			UA_Variant_setScalar(&attr.value, &uaBool, &UA_TYPES[UA_TYPES_BOOLEAN]);
		}
		else if (_tcsicmp(m_vecNode[i].tszDataType, INI_TYPE_WORD) == 0)
		{
			UA_UInt16 uaWord;
			uaWord = m_vecNode[i].wInitialValue;
			attr.dataType = UA_TYPES[UA_TYPES_UINT16].typeId;
			UA_Variant_setScalar(&attr.value, &uaWord, &UA_TYPES[UA_TYPES_UINT16]);
		}
		else
			continue;

		/* Add the variable node to the information model */
		wcstombs(szText, m_vecNode[i].tszNodeId, MAX_PATH);
		UA_NodeId uaNodeId = UA_NODEID_STRING_ALLOC(1, szText);
		UA_QualifiedName qalifiedName = UA_QUALIFIEDNAME_ALLOC(1, szText);
		UA_NodeId parentNodeId = UA_NODEID_NUMERIC(0, UA_NS0ID_OBJECTSFOLDER);
		UA_NodeId parentReferenceNodeId = UA_NODEID_NUMERIC(0, UA_NS0ID_ORGANIZES);

		UA_Server_addVariableNode(server, uaNodeId, parentNodeId,
			parentReferenceNodeId, qalifiedName,
			UA_NODEID_NUMERIC(0, UA_NS0ID_BASEDATAVARIABLETYPE), attr, NULL, NULL);
	}
}

LRESULT COPCServerDlg::WindowProc(UINT message, WPARAM wParam, LPARAM lParam)
{
	// TODO:  在此加入特定的程式碼和 (或) 呼叫基底類別
	if (WM_UPDATEUISTATE == message)
	{
		UA_MONITOR utTemp;
		for (int i = 0; i < MAX_ONCE_QUEUE_SIZE && m_queMonitor.size(); i++)		// avoid windows is too busy to receieve postmessage
		{
			EnterCriticalSection(&m_csSection);
			utTemp = m_queMonitor.front();
			m_queMonitor.pop();
			LeaveCriticalSection(&m_csSection);
			m_listVar.SetItemText(utTemp.nIndex, 3, utTemp.szText);
			TRACE(_T("Received WM_UPDATEUISTATE m_queMonitor.size = %i, index = %i, value=%s\n"),
				m_queMonitor.size(), utTemp.nIndex, utTemp.szText);
		}
	}
	return CDialogEx::WindowProc(message, wParam, lParam);
}

BOOL COPCServerDlg::CheckAllNodeValue(UA_NODE &uaTemp, BOOL bAppend)
{
	CString strTemp;
	uaTemp.nIndex = GetDlgItemInt(IDC_EDIT_VAR_NUM);
	if (uaTemp.nIndex < 1 || uaTemp.nIndex > MAX_VARIABLE)
	{
		AfxMessageBox(_T("Var Num is over the range!"), MB_OK | MB_ICONWARNING);
		return FALSE;
	}
	BOOL bSameExist = FALSE;
	for (size_t i = 0; i < m_vecNode.size(); i++)
	{
		if (uaTemp.nIndex == m_vecNode[i].nIndex)
			bSameExist = TRUE;
	}
	if (bSameExist && bAppend)
	{
		AfxMessageBox(_T("Var Num already exists!"), MB_OK | MB_ICONWARNING);
		return FALSE;
	}
	if (!bSameExist && !bAppend)
	{
		AfxMessageBox(_T("Sould append new one!"), MB_OK | MB_ICONWARNING);
		return FALSE;
	}
	GetDlgItemText(IDC_EDIT_ITEM_NAME, strTemp);
	if (strTemp.GetLength() == 0)
	{
		AfxMessageBox(_T("Item Name is empty!"), MB_OK | MB_ICONWARNING);
		return FALSE;
	}
	_tcscpy(uaTemp.tszItemName, strTemp.GetBuffer(0)); strTemp.ReleaseBuffer();
	//for (size_t i = 0; i < _tcslen(uaTemp.tszItemName); i++)
	//	if (!_istalnum(uaTemp.tszItemName[i]) && uaTemp.tszItemName[i] != _T('_') && uaTemp.tszItemName[i] != _T(' '))
	//	{
	//		AfxMessageBox(_T("Only English letter or number are permitted for Item Name!"), MB_OK | MB_ICONWARNING);
	//		return FALSE;
	//	}

	GetDlgItemText(IDC_EDIT_NODE_ID, strTemp);
	if (strTemp.GetLength() == 0)
	{
		AfxMessageBox(_T("Node Id Name is empty!"), MB_OK | MB_ICONWARNING);
		return FALSE;
	}
	_tcscpy(uaTemp.tszNodeId, strTemp.GetBuffer(0)); strTemp.ReleaseBuffer();
	bSameExist = FALSE;
	for (size_t i = 0; i < m_vecNode.size(); i++)
	{
		if (_tcsicmp(uaTemp.tszNodeId, m_vecNode[i].tszNodeId) == 0)
			bSameExist = TRUE;
	}
	if (bSameExist && bAppend)
	{
		AfxMessageBox(_T("Var Num already exists!"), MB_OK | MB_ICONWARNING);
		return FALSE;
	}
	if (!bSameExist && !bAppend)
	{
		AfxMessageBox(_T("Sould append new one!"), MB_OK | MB_ICONWARNING);
		return FALSE;
	}
	for (size_t i = 0; i < _tcslen(uaTemp.tszNodeId); i++)
		if (!_istalnum(uaTemp.tszNodeId[i]) && uaTemp.tszNodeId[i] != _T('_'))
	{
		AfxMessageBox(_T("Only English letter or number are permitted for Node Id!"), MB_OK | MB_ICONWARNING);
		return FALSE;
	}
	uaTemp.nLength = GetDlgItemInt(IDC_EDIT_LENGTH);

	m_cmbType.GetLBText(m_cmbType.GetCurSel(), strTemp);
	if (strTemp == INI_TYPE_STRING)
	{
		GetDlgItemText(IDC_EDIT_INITIAL, strTemp); _tcscpy(uaTemp.tszInitial, strTemp.GetBuffer(0)); strTemp.ReleaseBuffer();
		for (size_t i = 0; i < _tcslen(uaTemp.tszInitial); i++)
			if (!_istalnum(uaTemp.tszInitial[i]))
			{
				AfxMessageBox(_T("Only English letter or number are permitted for Node Id!"), MB_OK | MB_ICONWARNING);
				return FALSE;
			}
		_tcscpy(uaTemp.tszDataType, INI_TYPE_STRING);
	}
	else if (strTemp == INI_TYPE_WORD)
	{
		uaTemp.wInitialValue = GetDlgItemInt(IDC_EDIT_INITIAL);
		_tcscpy(uaTemp.tszDataType, INI_TYPE_WORD);
	}
	else if (strTemp == INI_TYPE_BOOL)
	{
		uaTemp.wInitialValue = GetDlgItemInt(IDC_EDIT_INITIAL);
		_tcscpy(uaTemp.tszDataType, INI_TYPE_BOOL);
	}
	return TRUE;
}

void COPCServerDlg::WriteNode2Ini(TCHAR *ptszSection, UA_NODE &uaTemp)
{
	TCHAR tszText[MAX_PATH];
	if (ptszSection && _tcslen(ptszSection))
	{
		WritePrivateProfileStringW(ptszSection, INI_ITEM_NAME, uaTemp.tszItemName, INI_FILE_NAME);
		WritePrivateProfileStringW(ptszSection, INI_NODE_ID, uaTemp.tszNodeId, INI_FILE_NAME);
		WritePrivateProfileStringW(ptszSection, INI_DATA_TYPE, uaTemp.tszDataType, INI_FILE_NAME);
		if (_tcsicmp(uaTemp.tszDataType, INI_TYPE_STRING) == 0)
		{
			_itot(uaTemp.nLength, tszText, 10);
			WritePrivateProfileStringW(ptszSection, INI_INITIAL, uaTemp.tszInitial, INI_FILE_NAME);
			WritePrivateProfileStringW(ptszSection, INI_LENGTH, tszText, INI_FILE_NAME);
		}
		else
		{
			_itot(uaTemp.wInitialValue, tszText, 10);
			WritePrivateProfileStringW(ptszSection, INI_INITIAL, tszText, INI_FILE_NAME);
			WritePrivateProfileStringW(ptszSection, INI_LENGTH, NULL, INI_FILE_NAME);
		}
	}
}

void COPCServerDlg::OnBnClickedButtonAdd()
{
	// TODO:  在此加入控制項告知處理常式程式碼
	UA_NODE uaTemp = { 0 };
	if (CheckAllNodeValue(uaTemp, TRUE))
	{
		TCHAR tszText[MAX_PATH];
		_stprintf(tszText, _T("Variable%03i"), uaTemp.nIndex);
		WriteNode2Ini(tszText, uaTemp);
		m_vecNode.clear();
		m_listVar.DeleteAllItems();
		InsertItemFromIniFile();
	}
}


void COPCServerDlg::OnBnClickedButtonSaveFile()
{
	// TODO:  在此加入控制項告知處理常式程式碼
	UA_NODE uaTemp = { 0 };
	if (CheckAllNodeValue(uaTemp, FALSE))
	{
		POSITION pos = m_listVar.GetFirstSelectedItemPosition();
		if (pos > 0)
		{
			int nPos = GetNodeIndex((int)m_listVar.GetItemData((int)(pos - 1)));
			TCHAR tszText[MAX_PATH];
			_stprintf(tszText, _T("Variable%03i"), m_vecNode[nPos].nIndex);
			WriteNode2Ini(tszText, uaTemp);
			m_vecNode.clear();
			m_listVar.DeleteAllItems();
			InsertItemFromIniFile();
		}
	}
}


void COPCServerDlg::OnBnClickedButtonDelete()
{
	// TODO:  在此加入控制項告知處理常式程式碼
	POSITION pos = m_listVar.GetFirstSelectedItemPosition();
	if (pos > 0)
	{
		int nPos = GetNodeIndex((int)m_listVar.GetItemData((int)(pos - 1)));
		TCHAR tszText[MAX_PATH];
		_stprintf(tszText, _T("Variable%03i"), m_vecNode[nPos].nIndex);
		WritePrivateProfileString(tszText, NULL, NULL, INI_FILE_NAME);
		m_vecNode.clear();
		m_listVar.DeleteAllItems();
		InsertItemFromIniFile();
	}
}
