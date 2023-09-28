
// LK-GCommunicatorDlg.cpp : 實作檔
//

#include "stdafx.h"
#include "LK-GCommunicator.h"
#include "LK-GCommunicatorDlg.h"
#include "afxdialogex.h"
#include "AoiFont.h"
#include "DataHandlerBase.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CLKGCommunicatorDlg 對話方塊

#define DATA_MAX		50

CLKGCommunicatorDlg::CLKGCommunicatorDlg(UINT nComId1, UINT nComId2, UINT nRate, UINT nTime, CWnd* pParent /*=NULL*/)
	: CDialogEx(CLKGCommunicatorDlg::IDD, pParent)
{
	m_pThickMachine = new CThickMachineManager(nComId1, nComId2, nRate, nTime);

	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}
CLKGCommunicatorDlg::~CLKGCommunicatorDlg()
{
	Finalize();
}
void CLKGCommunicatorDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CLKGCommunicatorDlg, CDialogEx)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_INFO, OnLvnGetdispinfoInfo)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_PARAM1, OnLvnGetdispinfoParam)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_PARAM2, OnLvnGetdispinfoParam)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_THICK1, OnLvnGetdispinfoThick)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_THICK2, OnLvnGetdispinfoThick)
#ifdef EXPORTCSV
	ON_BN_CLICKED(UI_BTN_CSV, OnExportCSV)
#endif
END_MESSAGE_MAP()


// CLKGCommunicatorDlg 訊息處理常式

BOOL CLKGCommunicatorDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 設定此對話方塊的圖示。當應用程式的主視窗不是對話方塊時，
	// 框架會自動從事此作業
	SetIcon(m_hIcon, TRUE);			// 設定大圖示
	SetIcon(m_hIcon, FALSE);		// 設定小圖示

	// TODO:  在此加入額外的初始設定
	Init();
	return TRUE;  // 傳回 TRUE，除非您對控制項設定焦點
}

// 如果將最小化按鈕加入您的對話方塊，您需要下列的程式碼，
// 以便繪製圖示。對於使用文件/檢視模式的 MFC 應用程式，
// 框架會自動完成此作業。

void CLKGCommunicatorDlg::OnPaint()
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
HCURSOR CLKGCommunicatorDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}
void CLKGCommunicatorDlg::OnLvnGetdispinfoInfo(NMHDR *pNMHDR, LRESULT *pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast< NMLVDISPINFO* >(pNMHDR);

	*pResult = NULL;
	if (pDispInfo->item.iItem == EOF || pDispInfo->item.iItem >= (int)m_vInfo.size()) return;

	EnterCriticalSection(&m_xLock[LOCK_INFO]);
	std::pair<CString, __time64_t> xPair = m_vInfo.at(pDispInfo->item.iItem);
	LeaveCriticalSection(&m_xLock[LOCK_INFO]);
	if (LVIF_TEXT & pDispInfo->item.mask){
		CString strText;
		switch (pDispInfo->item.iSubItem)
		{
		case LIST_COL_TITLE:
			strText = CTime(xPair.second).Format(L"%H:%M:%S");
			break;
		case LIST_COL_DATA:
			strText = xPair.first;
			break;
		}
		wcscpy_s(pDispInfo->item.pszText, strText.GetLength() + 1, strText);
	}
}
void CLKGCommunicatorDlg::OnLvnGetdispinfoParam(NMHDR *pNMHDR, LRESULT *pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast< NMLVDISPINFO* >(pNMHDR);

	*pResult = NULL;
	if (pDispInfo->item.iItem == EOF) return;

	int nMachine = -1;
	switch (pDispInfo->hdr.idFrom){
	case UI_LC_PARAM1:
		nMachine = MACHINE_1;
		break;
	case UI_LC_PARAM2:
		nMachine = MACHINE_2;
		break;
	default:
		return;
	}

	if (nMachine < 0 || nMachine >= MACHINE_MAX) return;

	if (pDispInfo->item.iItem >= m_vParam[nMachine].size()) return;

	EnterCriticalSection(&m_xLock[LOCK_INFO]);
	std::pair<CString, CString> xPair = m_vParam[nMachine].at(pDispInfo->item.iItem);
	LeaveCriticalSection(&m_xLock[LOCK_INFO]);
	if (LVIF_TEXT & pDispInfo->item.mask){
		CString strText;
		switch (pDispInfo->item.iSubItem)
		{
		case LIST_COL_TITLE:
			strText = xPair.first;
			break;
		case LIST_COL_DATA:
			strText = xPair.second;
			break;
		}
		wcscpy_s(pDispInfo->item.pszText, strText.GetLength() + 1, strText);
	}
}
void CLKGCommunicatorDlg::OnLvnGetdispinfoThick(NMHDR *pNMHDR, LRESULT *pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast< NMLVDISPINFO* >(pNMHDR);

	*pResult = NULL;
	if (pDispInfo->item.iItem == EOF) return;

	int nMachine = -1;
	switch (pDispInfo->hdr.idFrom){
	case UI_LC_THICK1:
		nMachine = MACHINE_1;
		break;
	case UI_LC_THICK2:
		nMachine = MACHINE_2;
		break;
	default:
		return;
	}

	if (nMachine < 0 || nMachine >= MACHINE_MAX) return;

	if (pDispInfo->item.iItem >= m_vThick[nMachine].size()) return;

	EnterCriticalSection(&m_xLock[LOCK_THICK]);
	THICK_INFO &xThick = m_vThick[nMachine].at(pDispInfo->item.iItem);
	if (LVIF_TEXT & pDispInfo->item.mask){
		CString strText;
		switch (pDispInfo->item.iSubItem)
		{
		case LIST_DATACOL_VALUE1:
			strText.Format(L"%.4f", xThick.fThick1);
			break;
		case LIST_DATACOL_VALUE2:
			strText.Format(L"%.4f", xThick.fThick2);
			break;
		case LIST_DATACOL_TIME:
			strText = CTime(xThick.xTime).Format(L"%H:%M:%S");
			break;
		}
		wcscpy_s(pDispInfo->item.pszText, strText.GetLength() + 1, strText);
	}
	LeaveCriticalSection(&m_xLock[LOCK_THICK]);
}
#ifdef EXPORTCSV
void CLKGCommunicatorDlg::OnExportCSV()
{
	if (m_pThickMachine){
		m_pThickMachine->OnExportCSV();
		EnterCriticalSection(&m_xLock[LOCK_THICK]);
		for (int i = 0; i < MACHINE_MAX; i++){
			m_vThick[i].clear();
		}
		LeaveCriticalSection(&m_xLock[LOCK_THICK]);
		if (m_xUi[UI_LC_THICK1].pList) m_xUi[UI_LC_THICK1].pList->Invalidate();
		if (m_xUi[UI_LC_THICK2].pList) m_xUi[UI_LC_THICK2].pList->Invalidate();
	}
}
#endif
void CLKGCommunicatorDlg::Init()
{
	for (int i = LOCK_BEGIN; i < LOCK_MAX; i++){
		InitializeCriticalSection(&m_xLock[i]);
	}
	InitUiRectPos();
	InitUI();

	m_pThickMachine->AttachLink(this);
	m_pThickMachine->OpenDevice();

	for (int i = UI_LC_PARAM1; i <= UI_LC_PARAM2; i++){
		if (m_xUi[i].pList){
			LVCOLUMN xColumn = { NULL };
			xColumn.mask = LVCF_TEXT;

			CString strComId;
			if (i == UI_LC_PARAM1)
				strComId = m_pThickMachine->GetComId(MACHINE_1);
			else if (i == UI_LC_PARAM2)
				strComId = m_pThickMachine->GetComId(MACHINE_2);

			xColumn.pszText = strComId.GetBuffer();
			m_xUi[i].pList->SetColumn(LIST_COL_TITLE, &xColumn);
		}
	}
	m_hAOI = ::FindWindow(NULL, L"AOI Master");
}
void CLKGCommunicatorDlg::InitUiRectPos()
{
	for (int i = UI_ITEM_BEGIN; i < UI_ITEM_END; i++){
		POINT ptBase = { 0, 0 };
		POINT ptSize = { 0, 0 };
		CString strCaption;
		COLORREF xColor = NULL;
		switch (i)
		{
		case UI_LC_THICK1:
			ptBase = { 10, 150 };
			ptSize = { 300, 690 };
			break;
		case UI_LC_THICK2:
			ptBase = { 320, 150 };
			ptSize = { 300, 690 };
			break;
		case UI_LC_PARAM1:
			ptBase = { 10, 10 };
			ptSize = { 300, 130 };
			break;
		case UI_LC_PARAM2:
			ptBase = { 320, 10 };
			ptSize = { 300, 130 };
			break;
		case UI_LC_INFO:
			ptBase = { 630, 150 };
			ptSize = { 200, 690 };
			break;
#ifdef EXPORTCSV
		case UI_BTN_CSV:
			ptBase = { 630, 10 };
			ptSize = { 60, 25 };
			strCaption = L"Export";
			break;
#endif
		}
		m_xUi[i].rcUi = { ptBase.x, ptBase.y, ptBase.x + ptSize.x, ptBase.y + ptSize.y };
		m_xUi[i].nID = i;
		m_xUi[i].strCaption = strCaption;
		m_xUi[i].xColor = xColor;
	}
}
void CLKGCommunicatorDlg::InitUI()
{
#ifdef EXPORTCSV
	for (int i = UI_BTN_BEGIN; i < UI_BTN_END; i++){
		m_xUi[i].pBtn = new CButton;
		m_xUi[i].pBtn->Create(m_xUi[i].strCaption, WS_VISIBLE | WS_CHILD, m_xUi[i].rcUi, this, i);
		g_AoiFont.SetWindowFont(m_xUi[i].pBtn, FontDef::typeT1);
	}
#endif
	for (int i = UI_LC_BEGIN; i < UI_LC_END; i++){ 
		m_xUi[i].pList = new CListCtrl;
		m_xUi[i].pList->Create(WS_CHILD | WS_VISIBLE | WS_BORDER | LVS_REPORT | LVS_OWNERDATA, m_xUi[i].rcUi, this, i);
		m_xUi[i].pList->SetExtendedStyle(LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);
		g_AoiFont.SetWindowFont(m_xUi[i].pList, FontDef::typeT1);
		if (i == UI_LC_THICK1 || i == UI_LC_THICK2){
			m_xUi[i].pList->InsertColumn(LIST_DATACOL_VALUE1, L"Value1", LVCFMT_LEFT, 90);
			m_xUi[i].pList->InsertColumn(LIST_DATACOL_VALUE2, L"Value2", LVCFMT_LEFT, 90);
			m_xUi[i].pList->InsertColumn(LIST_DATACOL_TIME, L"Time", LVCFMT_LEFT, 90);
		}
		else if (i == UI_LC_PARAM1 || i == UI_LC_PARAM2){
			m_xUi[i].pList->InsertColumn(LIST_COL_TITLE, L"Info", LVCFMT_LEFT, 90);
			m_xUi[i].pList->InsertColumn(LIST_COL_DATA, L"Data", LVCFMT_LEFT, 90);
		}
		else if (i == UI_LC_INFO){
			m_xUi[i].pList->InsertColumn(LIST_COL_TITLE, L"Time", LVCFMT_LEFT, 60);
			m_xUi[i].pList->InsertColumn(LIST_COL_DATA, L"Info", LVCFMT_LEFT, 200);
		}
	}
	m_xUi[UI_LC_THICK1].pList->SetItemCount(DATA_MAX);
	m_xUi[UI_LC_THICK2].pList->SetItemCount(DATA_MAX);
	m_xUi[UI_LC_INFO].pList->SetItemCount(DATA_MAX);
}
void CLKGCommunicatorDlg::Finalize()
{
	if (m_pThickMachine){
		delete m_pThickMachine;
		m_pThickMachine = NULL;
	}
	for (int i = LOCK_BEGIN; i < LOCK_MAX; i++){
		DeleteCriticalSection(&m_xLock[i]);
	}
}

void CLKGCommunicatorDlg::ON_THICKMACHINE_MSG(ThickMachine eMachine, CString &strMsg)
{
	std::pair<CString, __time64_t> xPair(strMsg, CTime::GetCurrentTime().GetTime());
	int nSize = 0;
	EnterCriticalSection(&m_xLock[LOCK_INFO]);
	if (m_vInfo.size() >= DATA_MAX)
		m_vInfo.erase(m_vInfo.begin());

	m_vInfo.push_back(xPair);

	LeaveCriticalSection(&m_xLock[LOCK_INFO]);

	theApp.InsertDebugLog(strMsg, LOG_THICK);

	if (m_xUi[UI_LC_INFO].pList){
		m_xUi[UI_LC_INFO].pList->Invalidate();
	}
}
void CLKGCommunicatorDlg::ON_THICKMACHINE_INFO(ThickMachine eMachine, OUT_TYPE eType, float *pData)
{
	THICK_INFO xInfo;
	memset(&xInfo, 0, sizeof(xInfo));
	BOOL bProcess = TRUE;

	switch (eType){
	case OUT_BOTH:
		xInfo.fThick1 = *pData;
		xInfo.fThick2 = *(pData + 1);
		break;
	case OUT_1:
	case OUT_2:
		xInfo.fThick1 = *pData;
		break;
	default:
		bProcess = FALSE;
		ASSERT(FALSE);//not yet
		break;
	}

	if (bProcess){
		xInfo.xTime = CTime::GetCurrentTime().GetTime();

		EnterCriticalSection(&m_xLock[LOCK_THICK]);
		if (m_vThick[eMachine].size() >= DATA_MAX)
			m_vThick[eMachine].erase(m_vThick[eMachine].begin());

		m_vThick[eMachine].push_back(xInfo);
		LeaveCriticalSection(&m_xLock[LOCK_THICK]);

		int nListId = -1;
		switch (eMachine){
		case MACHINE_1:
			nListId = UI_LC_THICK1;
			break;
		case MACHINE_2:
			nListId = UI_LC_THICK2;
			break;
		}
		if (nListId != -1 && m_xUi[nListId].pList){
			m_xUi[nListId].pList->Invalidate();
		}
		//send to AOI
		if (m_hAOI){
			unsigned long long nTemp = nListId;
			LPARAM lp = (nTemp << 32) | ((int)(xInfo.fThick1 * 10000) & 0xFFFFFFFF);
			::PostMessage(m_hAOI, WM_GPIO_MSG, WM_THICKINFO_CMD, lp);

		}
	}
}
void CLKGCommunicatorDlg::ON_THICKMACHINE_PARAM(ThickMachine eMachine, CString &strParam, CString &strValue)
{
	EnterCriticalSection(&m_xLock[LOCK_PARAM]);
	m_vParam[eMachine].push_back(std::pair<CString, CString>(strParam, strValue));
	int nSize = (int)m_vParam[eMachine].size();
	LeaveCriticalSection(&m_xLock[LOCK_PARAM]);
	int nListId = -1;
	switch (eMachine){
	case MACHINE_1:
		nListId = UI_LC_PARAM1;
		break;
	case MACHINE_2:
		nListId = UI_LC_PARAM2;
		break;
	}
	
	if (nListId != -1 && m_xUi[nListId].pList){
		m_xUi[nListId].pList->SetItemCount(nSize);
		m_xUi[nListId].pList->Invalidate();
	}
}