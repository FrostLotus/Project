// PLCCommunicatorDlg.cpp : 實作檔
#include "stdafx.h"
#include "PLCCommunicator.h"
#include "PLCCommunicatorDlg.h"
#include "afxdialogex.h"
#include "AoiFont.h"
#ifndef USE_MC_PROTOCOL
#include "SystCCLProcessJIUJIANG.h"
#include "SystCCLProcessDONGGUAN.h"
#include "SystCCLProcessDONGGUAN_SONG8.h"
#include "SystCCLProcessSUZHOU.h"
#include "SystCCLProcessCHANGSHU.h"
#include "SystFCCLProcess.h"
#include "ScribdPPProcess.h"
#include "SystPPProcess.h"
#include "TechainProcess.h"
#include "TGProcess.h"

#include "EverstrongProcess.h"  //2023/10/5 甬強新增
#ifdef _DEBUG
#include "TagProcess_FX5U.h"
#endif
#else
#include "TagProcess_FX5U.h"                 //應該不用
#include "PLC\SystWebCooperProcessSocket.h"
#include "PLC\SystCCLProcessSocket.h"
#endif
#include "usm.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif
// CPLCCommunicatorDlg 對話方塊
#define RECONNECT_TIMER 1
#define INFO_TIMER      2
#define INFO_MAX		50

#define CLR_BATCH	RGB(0,0,0)
#define CLR_NOTIFY	RGB(0xFF, 0, 0)
#define CLR_RESULT	RGB(0x80, 0x80, 0x80)
#define CLR_SKIP	RGB(0x80, 0, 0)
//========================================================================================
///<summary>[Constructor]初始化</summary>
CPLCCommunicatorDlg::CPLCCommunicatorDlg(BOOL bNoShow, CWnd* pParent /*=NULL*/)
	: CDialogEx(CPLCCommunicatorDlg::IDD, pParent)
{
	m_tTimerReconnect = NULL;
	m_bNoShow = bNoShow;
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}
///<summary>[Constructor]終處置</summary>
CPLCCommunicatorDlg::~CPLCCommunicatorDlg()
{
	Finalize();
}

void CPLCCommunicatorDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CPLCCommunicatorDlg, CDialogEx)//類似增加Listener的方式
#ifdef SHOW_DEBUG_BTN
	ON_BN_CLICKED(UI_BTN_QUERYALL, OnQueryAll)
	ON_BN_CLICKED(UI_BTN_TESTWRITE, OnTestWrite)
	ON_BN_CLICKED(UI_BTN_FULSHALL, OnFlushAll)
#endif
	ON_WM_TIMER()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_MESSAGE(WM_LOCAL_MSG, OnCmdProcess)
	ON_MESSAGE(WM_GPIO_MSG, OnCmdGPIO)
	ON_WM_WINDOWPOSCHANGING()
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_PLCADDRESS, OnLvnGetdispinfoPLCAddress)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_PLCPARAM, OnLvnGetdispinfoPLCParam)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_INFO, OnLvnGetdispinfoInfo)
	ON_NOTIFY(NM_CUSTOMDRAW, UI_LC_PLCADDRESS, OnCustomdrawList)

END_MESSAGE_MAP()

#ifdef SHOW_DEBUG_BTN

void CPLCCommunicatorDlg::OnQueryAll()
{
#ifdef USE_IN_COMMUNICATOR
	theApp.InsertDebugLog(L"UI QUERYALL");//log
#endif
#ifndef USE_MC_PROTOCOL
	if (m_pPLCProcessBase)
	{
		int nMax = m_pPLCProcessBase->GetFieldSize();
		for (int i = 0; i < nMax; i++)
		{
			PLC_DATA_ITEM_* pItem = m_pPLCProcessBase->GetPLCAddressInfo(i, FALSE);
			if (pItem && pItem->xAction != ACTION_SKIP) 
			{
				m_pPLCProcessBase->GET_PLC_FIELD_DATA(i);
			}
		}
	}
#else
	if (m_pPLC){
		m_pPLC->QUERY_ALL_BATCH_INFO();
	}
#endif
}
void CPLCCommunicatorDlg::OnTestWrite()
{
	theApp.InsertDebugLog(L"UI Test Write");
#ifndef USE_MC_PROTOCOL
	if (m_pPLCProcessBase){
		if (m_pPLCProcessBase->HAS_CUSTOM_TEST()){ //for custom test
			m_pPLCProcessBase->DO_CUSTOM_TEST();
		}
		else{
			int nMax = m_pPLCProcessBase->GetFieldSize();
#ifdef SHOW_PERFORMANCE
			LARGE_INTEGER xStart, xEnd, xFreq;
			QueryPerformanceFrequency(&xFreq);
			QueryPerformanceCounter(&xStart);
#endif
			for (int i = 0; i < nMax; i++){
				PLC_DATA_ITEM_* pItem = m_pPLCProcessBase->GetPLCAddressInfo(i, FALSE);
				if (pItem && pItem->xAction == ACTION_RESULT){
					switch (pItem->xValType){
					case PLC_TYPE_STRING:
						if (i <= 0xFF)
						{
							char c[2]; memset(&c, 0, 2);
							c[0] = 0x30 + i;
							m_pPLCProcessBase->SET_PLC_FIELD_DATA(i, 2, (BYTE*)&c);
						}
						break;
					case PLC_TYPE_WORD:
					{
						m_pPLCProcessBase->SET_PLC_FIELD_DATA(i, sizeof(WORD), (BYTE*)&i);
					}
					break;
					case PLC_TYPE_FLOAT:
					{
						float f = i * 1.0f;
						m_pPLCProcessBase->SET_PLC_FIELD_DATA(i, sizeof(float), (BYTE*)&f);
					}
					break;
					}
				}
			}
#ifdef SHOW_PERFORMANCE
			QueryPerformanceCounter(&xEnd);
			double d = (xEnd.QuadPart - xStart.QuadPart) * 1000.0 / xFreq.QuadPart;
			TRACE(L"Test write time : %.2f \n", d);
#endif
		}
	}
#endif
}
void CPLCCommunicatorDlg::OnFlushAll()
{
	CButton *pFlushAll = m_xUi[UI_BTN_FULSHALL].pBtn;
#ifndef USE_MC_PROTOCOL
	if (m_pPLCProcessBase && pFlushAll){
		m_pPLCProcessBase->SET_FLUSH_ANYWAY(pFlushAll->GetCheck());
	}
	if (pFlushAll){
		if (pFlushAll->GetCheck())
			theApp.InsertDebugLog(L"UI check flush");
		else
			theApp.InsertDebugLog(L"UI unCheck flush");
	}
#endif
}
#endif
// CPLCCommunicatorDlg 訊息處理常式

BOOL CPLCCommunicatorDlg::OnInitDialog()
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

void CPLCCommunicatorDlg::OnPaint()
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
		DrawInfo();
	}
}

// 當使用者拖曳最小化視窗時，
// 系統呼叫這個功能取得游標顯示。
HCURSOR CPLCCommunicatorDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

LRESULT CPLCCommunicatorDlg::OnCmdProcess(WPARAM wParam, LPARAM lParam)
{
	switch (wParam){
	case WM_EXIT://exit
		PostMessage(WM_CLOSE, NULL, NULL);
		break;
	}
	return 0;
}
#ifndef USE_MC_PROTOCOL
void CPLCCommunicatorDlg::ON_PLC_NOTIFY(CString strMsg)
{
	AddInfoText(strMsg);
}
void CPLCCommunicatorDlg::ON_SET_PLCPARAM(BATCH_SHARE_SYSTCCL_INITPARAM &xParam)
{
	m_xParam.strPLCIp = xParam.cPLCIP;
	if (m_xUi[UI_LC_PLCPARAM].pList){
		m_xUi[UI_LC_PLCPARAM].pList->SetItemCount(4);
	}
	if (m_xUi[UI_LC_PLCADDRESS].pList && m_pPLCProcessBase){
		m_xUi[UI_LC_PLCADDRESS].pList->SetItemCount(m_pPLCProcessBase->GetFieldSize());
	}
}
void CPLCCommunicatorDlg::ON_PLCDATA_CHANGE(int nFieldId, void* pData, int nSizeInByte)
{
	if (m_xUi[UI_LC_PLCADDRESS].pList){
		m_xUi[UI_LC_PLCADDRESS].pList->RedrawItems(nFieldId, nFieldId);
	}
}
void CPLCCommunicatorDlg::ON_BATCH_PLCDATA_CHANGE(int nFieldFirst, int nFieldLast)
{
	if (m_xUi[UI_LC_PLCADDRESS].pList){
		m_xUi[UI_LC_PLCADDRESS].pList->RedrawItems(nFieldFirst, nFieldLast);
	}
}
#endif
LRESULT CPLCCommunicatorDlg::OnCmdGPIO(WPARAM wParam, LPARAM lParam)
{
#ifndef USE_MC_PROTOCOL
	ON_GPIO_NOTIFY(wParam, lParam);
#endif
	switch (wParam){
	case WM_CUSTOMERTYPE_INIT:
		m_xParam.eCustomerType = (AOI_CUSTOMERTYPE_)(lParam >> 8 & 0xFF);
		m_xParam.eSubCustomerType = (AOI_SUBCUSTOMERTYPE_)(lParam & 0xFF);
#ifdef _DEBUG
		//m_xParam.eCustomerType = AOI_CUSTOMERTYPE_::CUSTOMER_SYST_CCL;
#endif
		InitPLCProcess();
		theApp.InsertDebugLog(L"init customer type done");
		break;
	case WM_AOI_RESPONSE_CMD:
#ifndef USE_MC_PROTOCOL
		if (lParam == WM_SYST_PARAMINIT_CMD || lParam == WM_SYST_PP_PARAMINIT_CMD){
			if (ON_OPEN_PLC(lParam) != 0){
				//open failed start reconnect timer
				if (m_tTimerReconnect == NULL){
					m_tTimerReconnect = SetTimer(RECONNECT_TIMER, 3000, NULL);
				}
			}
#ifndef _DEBUG
			ShowWindow(SW_MINIMIZE);
#endif
		}
#else
		HandleAOIResponse(lParam);
#endif
		break;
#ifdef USE_MC_PROTOCOL
	case WM_SYST_INFO_CHANGE:
		if (m_pPLCDataHandler && m_pPLC){
			BATCH_SHARE_SYST_INFO xInfo;
			memset(&xInfo, 0, sizeof(xInfo));
			m_pPLCDataHandler->GetSYSTInfo_CCL(&xInfo);
			m_pPLC->SetInfo(&xInfo);
		}
#endif
		break;
	}
	return 0;
}
void CPLCCommunicatorDlg::OnWindowPosChanging(WINDOWPOS FAR* lpwndpos)
{
	if (m_bNoShow)
		lpwndpos->flags &= ~SWP_SHOWWINDOW;

	CDialog::OnWindowPosChanging(lpwndpos);
}
void CPLCCommunicatorDlg::OnLvnGetdispinfoPLCAddress(NMHDR *pNMHDR, LRESULT *pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast< NMLVDISPINFO* >(pNMHDR);

	*pResult = NULL;

	int nMax = 0;
#ifndef USE_MC_PROTOCOL
	if (m_pPLCProcessBase){
		nMax = m_pPLCProcessBase->GetFieldSize();
	}
#else
	if (m_pPLC)
		nMax = m_pPLC->GetFieldSize();
#endif
	CString strText;

	if (LVIF_TEXT & pDispInfo->item.mask){
		if (pDispInfo->item.iItem == EOF || pDispInfo->item.iItem >= nMax) return;
		switch (pDispInfo->item.iSubItem)
		{
		case LIST_COL_FIELD:
		{
#ifndef USE_MC_PROTOCOL
			strText.Format(L"%s", m_pPLCProcessBase->GET_PLC_FIELD_NAME(pDispInfo->item.iItem));
#else
			strText.Format(L"%s", m_pPLC->GET_PLC_FIELD_NAME(pDispInfo->item.iItem));
#endif
		}
		break;
		case LIST_COL_ADDRESS:
		{
#ifndef USE_MC_PROTOCOL
			strText.Format(L"%s", m_pPLCProcessBase->GET_PLC_FIELD_ADDRESS(pDispInfo->item.iItem)); 
#else
			strText.Format(L"%s", m_pPLC->GET_PLC_FIELD_ADDRESS(pDispInfo->item.iItem));
#endif
		}
		break;
		case LIST_COL_VALUE:
		{
#ifndef USE_MC_PROTOCOL
			strText.Format(L"%s", m_pPLCProcessBase->GET_PLC_FIELD_VALUE(pDispInfo->item.iItem));
#else
			strText.Format(L"%s", m_pPLC->GET_PLC_FIELD_VALUE(pDispInfo->item.iItem));
#endif
		}
		break;
		case LIST_COL_TIME:
		{
#ifndef USE_MC_PROTOCOL
			strText.Format(L"%s", m_pPLCProcessBase->GET_PLC_FIELD_TIME(pDispInfo->item.iItem));
#else
			strText.Format(L"%s", m_pPLC->GET_PLC_FIELD_TIME(pDispInfo->item.iItem));
#endif
		}
		break;
		}
		wcscpy_s(pDispInfo->item.pszText, strText.GetLength() + 1, strText);
	}
}
void CPLCCommunicatorDlg::OnLvnGetdispinfoPLCParam(NMHDR *pNMHDR, LRESULT *pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast< NMLVDISPINFO* >(pNMHDR);

	*pResult = NULL;
	if (LVIF_TEXT & pDispInfo->item.mask){
		CString strText;
		switch (pDispInfo->item.iSubItem)
		{
		case LIST_COL_TITLE:
			switch (pDispInfo->item.iItem){
			case 0:
				strText.Format(L"IP");
				break;
			case 1:
#ifndef USE_MC_PROTOCOL
				strText.Format(L"CPU");
#else
				strText.Format(L"Port");
#endif
				break;
			case 2:
				strText.Format(L"CustomerType");
				break;
			case 3:
				strText.Format(L"SubCustomerType");
				break;
			case 4:
				strText.Format(L"Format");
				break;
			case 5:
				strText.Format(L"Frame");
				break;
			}
			break;
		case LIST_COL_DATA:
			switch (pDispInfo->item.iItem){
			case 0:
				strText.Format(L"%s", m_xParam.strPLCIp);
				break;
			case 1:
#ifndef USE_MC_PROTOCOL
				if (m_pPLCProcessBase)
					strText = m_pPLCProcessBase->GetCPUType();
#else
				strText.Format(L"%d", m_xParam.nPLCPort);
#endif
				break;
			case 2:
				strText.Format(L"%d", m_xParam.eCustomerType);
				break;
			case 3:
				strText.Format(L"%d", m_xParam.eSubCustomerType);
				break;
#ifdef USE_MC_PROTOCOL
			case 4:
				if (m_xParam.nFormat == 0)
					strText = L"Binary";
				else if (m_xParam.nFormat == 1)
					strText = L"ASCII";
				break;
			case 5:
				if (m_xParam.eFrameType == PLC_FRAME_TYPE::FRAME_3E)
					strText = L"3E";
				else if (m_xParam.eFrameType == PLC_FRAME_TYPE::FRAME_4E)
					strText = L"4E";
				break;
#endif
			}
			break;
		}
		wcscpy_s(pDispInfo->item.pszText, strText.GetLength() + 1, strText);
	}
}
void CPLCCommunicatorDlg::OnLvnGetdispinfoInfo(NMHDR *pNMHDR, LRESULT *pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast< NMLVDISPINFO* >(pNMHDR);

	*pResult = NULL;
	if (pDispInfo->item.iItem == EOF || pDispInfo->item.iItem >= (int)m_vPLCInfo.size()) return;

	EnterCriticalSection(&m_xLock[LOCK_INFO]);
	std::pair<CString, __time64_t> xPair = m_vPLCInfo.at(pDispInfo->item.iItem);
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
void CPLCCommunicatorDlg::OnCustomdrawList(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMLVCUSTOMDRAW* pLVCD = reinterpret_cast<NMLVCUSTOMDRAW*>(pNMHDR);

	// Take the default processing unless we 
	// set this to something else below.
	*pResult = CDRF_DODEFAULT;

	// First thing - check the draw stage. If it's the control's prepaint
	// stage, then tell Windows we want messages for every item.

	if (CDDS_PREPAINT == pLVCD->nmcd.dwDrawStage)
	{
		*pResult = CDRF_NOTIFYITEMDRAW;
	}
	else if (CDDS_ITEMPREPAINT == pLVCD->nmcd.dwDrawStage)
	{
		// This is the prepaint stage for an item. Here's where we set the
		// item's text color. Our return value will tell Windows to draw the
		// item itself, but it will use the new color we set here.
		// We'll cycle the colors through red, green, and light blue.

		COLORREF crText;
		
		PLC_ACTION_TYPE_ eType = ACTION_NOTIFY;
#ifndef USE_MC_PROTOCOL
		if (m_pPLCProcessBase) eType = m_pPLCProcessBase->GET_PLC_FIELD_ACTION((int)pLVCD->nmcd.dwItemSpec);
#else
		if (m_pPLC) eType = m_pPLC->GET_PLC_FIELD_ACTION((int)pLVCD->nmcd.dwItemSpec);
#endif
		switch (eType)
		{
		case ACTION_NOTIFY:
			crText = CLR_NOTIFY;
			break;
		case ACTION_BATCH:
			crText = CLR_BATCH;
			break;
		case ACTION_RESULT:
			crText = CLR_RESULT;
			break;
		case ACTION_SKIP:
			crText = CLR_SKIP;
			break;
		default:
			ASSERT(FALSE);
			break;
		}

		// Store the color back in the NMLVCUSTOMDRAW struct.
		pLVCD->clrText = crText;

		// Tell Windows to paint the control itself.
		*pResult = CDRF_DODEFAULT;
	}
}
void CPLCCommunicatorDlg::OnTimer(UINT_PTR nEventId)
{
	if (nEventId == m_tTimerReconnect){
		if (ON_OPEN_PLC(NULL) != 0){
			CString strLog;
			strLog.Format(L"open plc fail, reconnect in 3 sec");
			theApp.InsertDebugLog(strLog, LOG_SYSTEM);
			AddInfoText(strLog);
		}
		else{
			KillTimer(m_tTimerReconnect);
			m_tTimerReconnect = NULL;
		}
	}
	else if (nEventId == m_tTimer){
		m_xUi[UI_LABEL_TIME].strCaption = CTime::GetTickCount().Format(L"%Y/%m/%d %H:%M:%S");
		InvalidateRect(&m_xUi[UI_LABEL_TIME].rcUi);
	}
}
void CPLCCommunicatorDlg::Init()
{
	theApp.InsertDebugLog(L"Start");
	SetWindowText(PLC_COMMUNICATOR_NAME);
	for (int i = LOCK_BEGIN; i < LOCK_MAX; i++)
	{
		InitializeCriticalSection(&m_xLock[i]);
	}
#ifndef USE_MC_PROTOCOL
	m_pPLCProcessBase = NULL;
#else
	m_pPLCDataHandler = new CPLCDataHandler;
#endif
	InitUiRectPos();
	InitUI();
#ifndef USE_MC_PROTOCOL
#ifdef OFF_LINE
	m_xParam.eCustomerType = CUSTOMER_SYST_CCL;//(AOI_CUSTOMERTYPE_)(lParam >> 8 & 0xFF);
	m_xParam.eSubCustomerType = SUB_CUSTOMER_SUZHOU;//(AOI_SUBCUSTOMERTYPE_)(lParam & 0xFF);
	InitPLCProcess();
	theApp.InsertDebugLog(L"init customer type done");
	ON_OPEN_PLC();
#else
	auto NotifyExe = [](CString strTarget, WPARAM wp, LPARAM lp){
		HWND hWnd = ::FindWindow(NULL, strTarget);
		if (hWnd){
			::PostMessage(hWnd, WM_GPIO_MSG, wp, lp);
		}
	};
	NotifyExe(AOI_MASTER_NAME, WM_CUSTOMERTYPE_INIT, NULL);
	NotifyExe(QUERYSTATION_NAME, WM_CUSTOMERTYPE_INIT, NULL);
	NotifyExe(TECHAIN_NAME, WM_CUSTOMERTYPE_INIT, NULL);
#endif
#endif
	CenterWindow();
	m_tTimer = SetTimer(INFO_TIMER, 1000, NULL);
}
void CPLCCommunicatorDlg::InitUiRectPos()
{
	for (int i = UI_ITEM_BEGIN; i < UI_ITEM_END; i++){
		POINT ptBase = { 0, 0 };
		POINT ptSize = { 0, 0 };
		CString strCaption;
		COLORREF xColor = NULL;
		switch (i)
		{
		case UI_LABEL_BATCH:  
			ptBase = { 470, 740 };
			ptSize = { 145, 20 };
			strCaption = L"下發";
			xColor = CLR_BATCH;
			break;
		case UI_LABEL_NOTIFY:
			ptBase = { 470, 760 };
			ptSize = { 145, 20 };
			strCaption = L"指令";
			xColor = CLR_NOTIFY;
			break;
		case UI_LABEL_RESULT:
			ptBase = { 470, 780 };
			ptSize = { 145, 20 };
			strCaption = L"檢測結果(資料來自AOI)";
			xColor = CLR_RESULT;
			break;
		case UI_LABEL_SKIP:
			ptBase = { 470, 800 };
			ptSize = { 145, 20 };
			strCaption = L"不使用";
			xColor = CLR_SKIP;
			break;
		case UI_LABEL_TIME:
			ptBase = { 470, 820 };
			ptSize = { 145, 20 };
			strCaption = L"";
			xColor = CLR_BATCH;
			break;
#ifdef SHOW_DEBUG_BTN
		case UI_BTN_QUERYALL:
			ptBase = { 300, 10 };
			ptSize = { 145, 20 };
			strCaption = L"QueryAll";
			break;
		case UI_BTN_TESTWRITE:
			ptBase = { 445, 10 };
			ptSize = { 145, 20 };
			strCaption = L"TestWrite";
			break;
		case UI_BTN_FULSHALL:
			ptBase = { 590, 10 };
			ptSize = { 145, 20 };
			strCaption = L"Flush Anyway";
			break;
#endif
		case UI_LC_PLCADDRESS:
			ptBase = { 10, 150 };
			ptSize = { 450, 690 };
			break;
		case UI_LC_PLCPARAM:
			ptBase = { 10, 10 };
			ptSize = { 280, 130 };
			break;
		case UI_LC_INFO:
			ptBase = { 470, 150 };
			ptSize = { 280, 490 };
			break;
		}
		m_xUi[i].rcUi = { ptBase.x, ptBase.y, ptBase.x + ptSize.x, ptBase.y + ptSize.y };
		m_xUi[i].nID = i;
		m_xUi[i].strCaption = strCaption;
		m_xUi[i].xColor = xColor;
	}
}
void CPLCCommunicatorDlg::InitUI()
{
	for (int i = UI_BTN_BEGIN; i < UI_BTN_END; i++)
	{
		m_xUi[i].pBtn = new CButton;
		m_xUi[i].pBtn->Create(m_xUi[i].strCaption, WS_VISIBLE | WS_CHILD, m_xUi[i].rcUi, this, i);
		g_AoiFont.SetWindowFont(m_xUi[i].pBtn, FontDef::typeT1);
	}
	for (int i = UI_CHKBTN_BEGIN; i < UI_CHKBTN_END; i++)
	{
		m_xUi[i].pBtn = new CButton;
		m_xUi[i].pBtn->Create(m_xUi[i].strCaption, WS_VISIBLE | WS_CHILD | BS_AUTOCHECKBOX, m_xUi[i].rcUi, this, i);
		g_AoiFont.SetWindowFont(m_xUi[i].pBtn, FontDef::typeT1);
	}
	for (int i = UI_LC_BEGIN; i < UI_LC_END; i++)
	{
		m_xUi[i].pList = new CListCtrl;
		m_xUi[i].pList->Create(WS_CHILD | WS_VISIBLE | WS_BORDER | LVS_REPORT | LVS_OWNERDATA, m_xUi[i].rcUi, this, i);
		m_xUi[i].pList->SetExtendedStyle(LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);
		g_AoiFont.SetWindowFont(m_xUi[i].pList, FontDef::typeT1);
		if (i == UI_LC_PLCADDRESS)
		{
			m_xUi[i].pList->InsertColumn(LIST_COL_FIELD, L"Field", LVCFMT_LEFT, 180);
			m_xUi[i].pList->InsertColumn(LIST_COL_ADDRESS, L"Address", LVCFMT_LEFT, 90);
			m_xUi[i].pList->InsertColumn(LIST_COL_VALUE, L"Value", LVCFMT_LEFT, 90);
			m_xUi[i].pList->InsertColumn(LIST_COL_TIME, L"Time", LVCFMT_LEFT, 90);
		}
		else if (i == UI_LC_PLCPARAM)
		{
			m_xUi[i].pList->InsertColumn(LIST_COL_TITLE, L"Info", LVCFMT_LEFT, 130);
			m_xUi[i].pList->InsertColumn(LIST_COL_DATA, L"Data", LVCFMT_LEFT, 100);
		}
		else if (i == UI_LC_INFO)
		{
			m_xUi[i].pList->InsertColumn(LIST_COL_TITLE, L"Time", LVCFMT_LEFT, 80);
			m_xUi[i].pList->InsertColumn(LIST_COL_DATA, L"Info", LVCFMT_LEFT, 180);
		}
	}
	m_xUi[UI_LC_INFO].pList->SetItemCount(INFO_MAX);
}
void CPLCCommunicatorDlg::Finalize()
{
	if (m_tTimerReconnect){
		KillTimer(m_tTimerReconnect);
		m_tTimerReconnect = NULL;
	}
	if (m_tTimer){
		KillTimer(m_tTimer);
		m_tTimer = NULL;
	}
#ifndef USE_MC_PROTOCOL
	if (m_pPLCProcessBase){
		((IPLCProcess*)this)->AttachIn(NULL);
		m_pPLCProcessBase->AttachOut(NULL);

		delete m_pPLCProcessBase;
		m_pPLCProcessBase = NULL;
	}
#else
	OpPLC(OP_DESTROY);
	if (m_pPLCDataHandler){
		delete m_pPLCDataHandler;
		m_pPLCDataHandler = NULL;
	}
#endif
	for (int i = LOCK_BEGIN; i < LOCK_MAX; i++){ 
		DeleteCriticalSection(&m_xLock[i]);
	}
}
void CPLCCommunicatorDlg::DrawInfo()
{
	CDC *pDC = GetDC();
	pDC->SetBkMode(TRANSPARENT);
	pDC->SelectObject(g_AoiFont.GetFont(typeT1));

	for (int i = UI_LABEL_BEGIN; i < UI_LABEL_END; i++){
		pDC->SetTextColor(m_xUi[i].xColor);
		pDC->DrawText(m_xUi[i].strCaption, &m_xUi[i].rcUi, DT_LEFT);
	}

	ReleaseDC(pDC);
}
void CPLCCommunicatorDlg::AddInfoText(CString &strInfo)
{
	std::pair<CString, __time64_t> xPair(strInfo, CTime::GetCurrentTime().GetTime());
	int nSize = 0;
	EnterCriticalSection(&m_xLock[LOCK_INFO]);
	if (m_vPLCInfo.size() >= INFO_MAX)
		m_vPLCInfo.erase(m_vPLCInfo.begin());

	m_vPLCInfo.push_back(xPair);

	nSize = m_vPLCInfo.size();
	LeaveCriticalSection(&m_xLock[LOCK_INFO]);

	if (m_xUi[UI_LC_INFO].pList){
		m_xUi[UI_LC_INFO].pList->Invalidate();
		m_xUi[UI_LC_INFO].pList->EnsureVisible(nSize - 1, TRUE);
	}
}
void CPLCCommunicatorDlg::InitPLCProcess()
{
#ifndef USE_MC_PROTOCOL
	switch (m_xParam.eCustomerType){
#ifdef _DEBUG
	case CUSTOMER_TAG:
		m_pPLCProcessBase = new CTagProcess_FX5U;
		break;
#endif
	case CUSTOMER_SYST_WEB_COPPER:
		m_pPLCProcessBase = new CSystFCCLProcess;
		break;
	case CUSTOMER_SYST_CCL:
		switch (m_xParam.eSubCustomerType){
		case SUB_CUSTOMER_DONGGUAN:
			m_pPLCProcessBase = new CSystCCLProcessDONGGUAN;
			break;
		case SUB_CUSTOMER_DONGGUAN_SONG8:
			m_pPLCProcessBase = new CSystCCLProcessDONGGUAN_SONG8;
			break;
		case SUB_CUSTOMER_JIUJIANG:
			m_pPLCProcessBase = new CSystCCLProcessJIUJIANG;
			break;
		case SUB_CUSTOMER_SUZHOU:
			m_pPLCProcessBase = new CSystCCLProcessSUZHOU;
			break;
		case SUB_CUSTOMER_CHANGSHU:
			m_pPLCProcessBase = new CSystCCLProcessCHANGSHU;
			break;
		case SUB_CUSTOMER_CHANGSHU2:
			m_pPLCProcessBase = new CSystCCLProcessCHANGSHU2;
			break;
		}
		break;
	case CUSTOMER_SCRIBD_PP:
	case CUSTOMER_SYST_PP:
	case CUSTOMER_TUC_PP:
	case CUSTOMER_ITEQ:
	case CUSTOMER_EMC_PP:
	case CUSTOMER_YINGHUA:
		m_pPLCProcessBase = new CSystPPProcess;
		break;
	case CUSTOMER_TECHAIN:
		m_pPLCProcessBase = new CTechainProcess;
		break;
	case CUSTOMER_TG:
		m_pPLCProcessBase = new CTGProcess;
		break;
	}
	if (m_pPLCProcessBase){
		((IPLCProcess*)this)->AttachIn(m_pPLCProcessBase);
		m_pPLCProcessBase->AttachOut(this);
		CButton *pFlushAll = m_xUi[UI_BTN_FULSHALL].pBtn;
		if (pFlushAll){ //預設flush anyway為勾選
			pFlushAll->SetCheck(TRUE);
			OnFlushAll();
		}
	}
#endif
}
#ifdef USE_MC_PROTOCOL
void CPLCCommunicatorDlg::ConnStatusCallBack(AOI_SOCKET_STATE xState)
{
	CString strInfo;
	switch (xState)
	{
	case NONE:
		strInfo.Format(L"無連線");
		break;
	case CONNECTING:
		strInfo.Format(L"連線中");
		break;
	case CONNECTED:
		strInfo.Format(L"連線完成");
		break;
	case DISCONNECT:
		strInfo.Format(L"連線中斷");
		break;
	case RECONNECT:
		strInfo.Format(L"重新連線");
		break;
	case STOP:
		strInfo.Format(L"連線結束");
		break;
	default:
		break;
	}
	AddInfoText(strInfo);
}

void CPLCCommunicatorDlg::OnDeviceNotify(int nType, int nVal, CString strDes)
{
	BOOL bAddInfo = FALSE;
	CString strMsg;
	switch (nType)
	{
	case CMelsecPlcSocket::PLC_ERR_NOTIFY:
		bAddInfo = TRUE;
		strMsg.Format(L"PLC ERROR:");
		break;
	case CMelsecPlcSocket::PLC_ADDRESS_VAL:
		bAddInfo = TRUE;
		strMsg.Format(L"Unexcepted Address:");
		break;
	case CMelsecPlcSocket::PLC_WRITE_NOTIFY:
		strMsg.Format(L"PLC Write:");
		break;
	case CMelsecPlcSocket::PLC_INFO:
		bAddInfo = TRUE;
		strMsg.Format(L"PLC Info:");
		break;
	case CMelsecPlcSocket::PLC_READ_UPDATE:
	case CMelsecPlcSocket::PLC_WRITE_UPDATE:
		if (m_pPLC){
			if (nType == CMelsecPlcSocket::PLC_READ_UPDATE)
				strMsg.Format(L"PLC Read: Address %s", m_pPLC->GET_PLC_FIELD_ADDRESS(nVal));
			else if (nType == CMelsecPlcSocket::PLC_WRITE_UPDATE)
				strMsg.Format(L"PLC Write: Address %s", m_pPLC->GET_PLC_FIELD_ADDRESS(nVal));
		}
		if (m_xUi[UI_LC_PLCADDRESS].pList){
			m_xUi[UI_LC_PLCADDRESS].pList->RedrawItems(nVal, nVal);
		}
		break;
	default:
		break;
	}
	if (bAddInfo){
		//write info
		CString strInfo;
		strInfo.Format(L"%s %s", strMsg, strDes);
		AddInfoText(strInfo);

		theApp.InsertDebugLog(strInfo);
	}
}
void CPLCCommunicatorDlg::OnPLCNewBatch(CString strOrder, CString strMaterial)
{
	if (m_pPLCDataHandler){
		BATCH_SHARE_SYST_BASE xData;
		memset(&xData, 0, sizeof(xData));
		wcscpy_s(xData.cMaterial, strMaterial.GetBuffer());
		wcscpy_s(xData.cName, strOrder.GetBuffer());
		strMaterial.ReleaseBuffer();
		strOrder.ReleaseBuffer();
		m_pPLCDataHandler->SetSYSTParam_WebCopper(&xData);
	}
}
void CPLCCommunicatorDlg::OnPLCSYSTParam(BATCH_SHARE_SYST_PARAMCCL *pData)
{
	if (m_pPLCDataHandler){
		m_pPLCDataHandler->SetSYSYParam_CCL(pData);
	}
}
void CPLCCommunicatorDlg::OnC10Change(WORD wC10)
{
	CString strLog;
	strLog.Format(L"C10 Index: %d", wC10);
	theApp.InsertDebugLog(strLog, LOG_PLCC10);
	if (m_pPLCDataHandler){
		m_pPLCDataHandler->NotifyAOI(WM_SYST_C10CHANGE_CMD, wC10);
	}
}
void CPLCCommunicatorDlg::HandleAOIResponse(LPARAM lParam)
{
	switch (lParam){
	case WM_SYST_PARAMINIT_CMD:
		if (m_pPLCDataHandler){
			theApp.InsertDebugLog(L"[WM_PLCPARAM_CMD] ShareMem Begin Read");
			BATCH_SHARE_SYST_INITPARAM xData;
			memset(&xData, 0, sizeof(BATCH_SHARE_SYST_INITPARAM));
			m_pPLCDataHandler->GetInitParam(&xData);
			m_xParam.strPLCIp = xData.cPLCIP;
			m_xParam.nPLCPort = xData.nPLCPort;
			m_xParam.eCustomerType = (AOI_CUSTOMERTYPE_)xData.nCustomerType;
			m_xParam.eSubCustomerType = (AOI_SUBCUSTOMERTYPE_)xData.nSubCustomerType;
			m_xParam.nFormat = xData.nFormat;
			m_xParam.eFrameType = (PLC_FRAME_TYPE)xData.nFrameType;

			OpPLC(OP_CREATE);
			CMelsecPlcSocket::PLC_MODE eMode = CMelsecPlcSocket::PLC_MODE::MODE_BINARY;
			switch (m_xParam.nFormat)
			{
			case 0:
				eMode = CMelsecPlcSocket::PLC_MODE::MODE_BINARY;
				break;
			case 1:
				eMode = CMelsecPlcSocket::PLC_MODE::MODE_ASCII;
				break;
			}
			if (m_pPLC){
				m_pPLC->SetMode(eMode);
			}

			theApp.InsertDebugLog(L"[WM_PLCPARAM_CMD] ShareMem End Read");
			m_xUi[UI_LC_INFO].pList->Invalidate();
			m_xUi[UI_LC_PLCPARAM].pList->SetItemCount(6);
		}
		break;
	}
	if (m_pPLC){
		m_pPLC->ON_NOTIFY_AOI_RESPONSE(lParam);

	}
}

void CPLCCommunicatorDlg::OpPLC(int nOpCode)
{
	if (nOpCode == OP_CREATE){
		OpPLC(OP_DESTROY);


		switch (m_xParam.eCustomerType)
		{
		case CUSTOMER_SYST_WEB_COPPER: //生益軟板
			m_pPLC = new CSystWebCooperProcessSocket(this, m_xParam.eFrameType);
			break;

		default:
			ASSERT(FALSE);
			break;
		}
		int nMax = m_pPLC->GetFieldSize();
		if (m_pPLC){
			m_pPLC->SetConnectInfo(m_xParam.strPLCIp, m_xParam.nPLCPort);
			m_pPLC->DoConnect();
		}
		//adjust windows size by the number of field

		if (m_xUi[UI_LC_PLCADDRESS].pList){
			m_xUi[UI_LC_PLCADDRESS].pList->SetItemCount(nMax);
		}

		CenterWindow();

		if (m_xUi[UI_BTN_FULSHALL].pBtn){ //預設flush anyway為勾選
			m_xUi[UI_BTN_FULSHALL].pBtn->SetCheck(TRUE);
			if (m_pPLC) m_pPLC->OnCheckFlushAnyway(TRUE);
		}
	}
	else if (nOpCode == OP_DESTROY){
		if (m_pPLC){
			delete m_pPLC;
			m_pPLC = NULL;
		}
	}
}
#endif