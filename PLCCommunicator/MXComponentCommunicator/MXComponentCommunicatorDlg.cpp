
// MXComponentCommunicatorDlg.cpp : 實作檔
//

#include "stdafx.h"
#include "MXComponentCommunicator.h"
#include "MXComponentCommunicatorDlg.h"
#include "afxdialogex.h"
#include "AoiFont.h"
#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMXComponentCommunicatorDlg 對話方塊

CMelSecIOController g_xMelSecIOController;

CMXComponentCommunicatorDlg::CMXComponentCommunicatorDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CMXComponentCommunicatorDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}
CMXComponentCommunicatorDlg::~CMXComponentCommunicatorDlg()
{
	Finalize();
}
void CMXComponentCommunicatorDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CMXComponentCommunicatorDlg, CDialogEx)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_MESSAGE(WM_GPIO_MSG, OnCmdGPIO)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_INFO, OnLvnGetdispInfo)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_MESSAGE, OnLvnGetdispMessage)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_PIN, OnLvnGetdispPin)
	ON_MESSAGE(WM_LOCAL_MSG, OnCmdProcess)
	ON_BN_CLICKED(UI_BTN_RESET, OnReset)
END_MESSAGE_MAP()

LRESULT CMXComponentCommunicatorDlg::OnCmdProcess(WPARAM wParam, LPARAM lParam)
{
	switch (wParam){
	case WM_EXIT://exit
		PostMessage(WM_CLOSE, NULL, NULL);
		break;
	}
	return 0;
}
void CMXComponentCommunicatorDlg::OnReset()
{
	OnAddMessage(L"Reset");
	g_xMelSecIOController.Reset();
}
// CMXComponentCommunicatorDlg 訊息處理常式

BOOL CMXComponentCommunicatorDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 設定此對話方塊的圖示。當應用程式的主視窗不是對話方塊時，
	// 框架會自動從事此作業
	SetIcon(m_hIcon, TRUE);			// 設定大圖示
	SetIcon(m_hIcon, FALSE);		// 設定小圖示

	// TODO:  在此加入額外的初始設定
	Init();
	InitUiRectPos();
	InitUI();

	g_xMelSecIOController.InitController();
	return TRUE;  // 傳回 TRUE，除非您對控制項設定焦點
}

// 如果將最小化按鈕加入您的對話方塊，您需要下列的程式碼，
// 以便繪製圖示。對於使用文件/檢視模式的 MFC 應用程式，
// 框架會自動完成此作業。

void CMXComponentCommunicatorDlg::OnPaint()
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
HCURSOR CMXComponentCommunicatorDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


LRESULT CMXComponentCommunicatorDlg::OnCmdGPIO(WPARAM wParam, LPARAM lParam)
{
	switch (wParam){
	case WM_AOI_RESPONSE_CMD:
		HandleAOIResponse(lParam);
		break;
	case WM_MX_PINSTATUS_CMD:
		{
			g_xMelSecIOController.GetOutputPin();
		}
		break;
	}
	return 0;
}
void CMXComponentCommunicatorDlg::OnLvnGetdispInfo(NMHDR *pNMHDR, LRESULT *pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast< NMLVDISPINFO* >(pNMHDR);

	*pResult = NULL;
	CString strData;
	if (LVIF_TEXT & pDispInfo->item.mask){
		if (pDispInfo->item.iItem == EOF) return;

		if (pDispInfo->item.iItem == 0)
			strData = g_xMelSecIOController.GetPLCIP();
		else if (pDispInfo->item.iItem == 1)
			strData = g_xMelSecIOController.GetCPUType();


		switch (pDispInfo->item.iSubItem)
		{
		case LIST_COL_TITLE:
			wcscpy_s(pDispInfo->item.pszText, strData.GetLength() + 1, strData);
			break;
		}
	}
}
void CMXComponentCommunicatorDlg::OnLvnGetdispMessage(NMHDR *pNMHDR, LRESULT *pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast< NMLVDISPINFO* >(pNMHDR);

	*pResult = NULL;
	CString strData;
	if (LVIF_TEXT & pDispInfo->item.mask){
		if (pDispInfo->item.iItem == EOF) return;
		EnterCriticalSection(&m_xLock[LOCK_MESSAGE]);
		
		int nSize = m_vMessage.size();

		if (pDispInfo->item.iItem >= 0 && pDispInfo->item.iItem < nSize){
			std::pair<CString, CString> &xItem = m_vMessage.at(pDispInfo->item.iItem);
			switch (pDispInfo->item.iSubItem)
			{
			case LIST_COL_TITLE:
				wcscpy_s(pDispInfo->item.pszText, xItem.first.GetLength() + 1, xItem.first);
				break;
			case LIST_COL_DATA:
				wcscpy_s(pDispInfo->item.pszText, xItem.second.GetLength() + 1, xItem.second);
				break;
			}
		}
		LeaveCriticalSection(&m_xLock[LOCK_MESSAGE]);
	}
}
void CMXComponentCommunicatorDlg::OnLvnGetdispPin(NMHDR *pNMHDR, LRESULT *pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast< NMLVDISPINFO* >(pNMHDR);

	*pResult = NULL;
	if (LVIF_TEXT & pDispInfo->item.mask){
		if (pDispInfo->item.iItem == EOF) return;

		CString strPin, strTemp;
		int nStatus = 0;
		g_xMelSecIOController.GetPinStatus(pDispInfo->item.iItem, strTemp, nStatus);
		strPin.Format(L"Pin%d(%s)", pDispInfo->item.iItem + 1, strTemp);
		switch (pDispInfo->item.iSubItem)
		{
		case LIST_COL_TITLE:
			wcscpy_s(pDispInfo->item.pszText, strPin.GetLength() + 1, strPin);
			break;
		case LIST_COL_DATA:
			wsprintf(pDispInfo->item.pszText, _T("%d"), nStatus);
			break;
		}
	}
}
void CMXComponentCommunicatorDlg::Init()
{
	for (int i = LOCK_BEGIN; i < LOCK_MAX; i++){
		InitializeCriticalSection(&m_xLock[i]);
	}
	g_xMelSecIOController.AttachLink(this);
}
void CMXComponentCommunicatorDlg::InitUiRectPos()
{
	for (int i = UI_ITEM_BEGIN; i < UI_ITEM_END; i++){
		POINT ptBase = { 0, 0 };
		POINT ptSize = { 0, 0 };
		CString strCaption;
		switch (i)
		{
		case UI_LABEL_INFO1:
			ptBase = { 10, 10 };
			ptSize = { 150, 40 };
			break;
		case UI_LABEL_INFO2:
			ptBase = { 300, 10 };
			ptSize = { 180, 40 };
			break;
		case UI_LC_INFO:
			ptBase = { 10, 50 };
			ptSize = { 200, 90 };
			break;
		case UI_LC_MESSAGE:
			ptBase = { 280, 50 };
			ptSize = { 350, 480 };
			break;
		case UI_LC_PIN:
			ptBase = { 10, 180 };
			ptSize = { 200, 350 };
			break;
		case UI_BTN_RESET:
			ptBase = { 10, 15 };
			ptSize = { 50, 25 };
			break;
		}
		m_xUi[i].rcUi = { ptBase.x, ptBase.y, ptBase.x + ptSize.x, ptBase.y + ptSize.y };
		m_xUi[i].nID = i;
		m_xUi[i].strCaption = strCaption;
	}
}
void CMXComponentCommunicatorDlg::InitUI()
{
	for (int i = UI_BTN_BEGIN; i < UI_BTN_END; i++){
		m_xUi[i].pBtn = new CButton;
		m_xUi[i].pBtn->Create(L"Reset", WS_VISIBLE | WS_CHILD , m_xUi[i].rcUi, this, i);
		g_AoiFont.SetWindowFont(m_xUi[i].pBtn, FontDef::typeT1);
	}
	for (int i = UI_LC_BEGIN; i < UI_LC_END; i++){
		m_xUi[i].pList = new CListCtrl;
		m_xUi[i].pList->Create(WS_VISIBLE | WS_CHILD | WS_BORDER | LVS_REPORT | LVS_OWNERDATA, m_xUi[i].rcUi, this, m_xUi[i].nID);
		m_xUi[i].pList->SetExtendedStyle(LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);
	}
	m_xUi[UI_LC_INFO].pList->InsertColumn(LIST_COL_TITLE, L"Info", LVCFMT_LEFT, 180);
	m_xUi[UI_LC_INFO].pList->SetItemCount(2); // only show ip and type

	m_xUi[UI_LC_MESSAGE].pList->InsertColumn(LIST_COL_TITLE, L"Time", LVCFMT_LEFT, 150);
	m_xUi[UI_LC_MESSAGE].pList->InsertColumn(LIST_COL_DATA, L"Message", LVCFMT_LEFT, 200);

	m_xUi[UI_LC_PIN].pList->InsertColumn(LIST_COL_TITLE, L"Pin", LVCFMT_LEFT, 70);
	m_xUi[UI_LC_PIN].pList->InsertColumn(LIST_COL_DATA, L"Status", LVCFMT_LEFT, 70);

	m_xUi[UI_LC_MESSAGE].pList->SetItemCount(MAX_MESSAGE);

}
void CMXComponentCommunicatorDlg::Finalize()
{
	for (int i = LOCK_BEGIN; i < LOCK_MAX; i++){
		DeleteCriticalSection(&m_xLock[i]);
	}
}
void CMXComponentCommunicatorDlg::HandleAOIResponse(LPARAM lParam)
{
	switch (lParam){
	case WM_MX_PARAMINIT_CMD:
		{
			g_xMelSecIOController.GetInitParam();
#ifndef _DEBUG
			ShowWindow(SW_MINIMIZE);
#endif
		}
		break;
	}
}
void CMXComponentCommunicatorDlg::OnChangeInformation()
{
	if (m_xUi[UI_LC_INFO].pList){
		m_xUi[UI_LC_INFO].pList->Invalidate();
	}
}
void CMXComponentCommunicatorDlg::OnAddMessage(CString strMsg)
{
	EnterCriticalSection(&m_xLock[LOCK_MESSAGE]);
	m_vMessage.push_back(std::pair<CString, CString>(CTime::GetCurrentTime().Format(L"%Y/%m/%d %H:%M:%S"), strMsg));
	if (m_vMessage.size() >= MAX_MESSAGE){
		m_vMessage.erase(m_vMessage.begin());
	}
	if (m_xUi[UI_LC_MESSAGE].pList){
		m_xUi[UI_LC_MESSAGE].pList->Invalidate(); //should not Invalidate whole window, modify later
	}
	LeaveCriticalSection(&m_xLock[LOCK_MESSAGE]);

}
void CMXComponentCommunicatorDlg::OnPinStatusChange(int nPin)
{
	if (nPin == -1){
		if (m_xUi[UI_LC_PIN].pList){
			m_xUi[UI_LC_PIN].pList->SetItemCount(g_xMelSecIOController.GetGPIOPinNumber());
		}
	}
	else{
		m_xUi[UI_LC_PIN].pList->RedrawItems(nPin, nPin);
	}

}