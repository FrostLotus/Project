// OPCCommunicatorDlg.cpp : ��@��

#include "stdafx.h"
#include "OPCCommunicator.h"
#include "OPCCommunicatorDlg.h"
#include "afxdialogex.h"
#include "AoiFont.h"
#include "OPC\SystPPProcess.h"
#include "OPC\SystDONGGUANPPProcess.h"
#include "DataHandlerBase.h"
#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// COPCCommunicatorDlg ��ܤ��
#define INFO_MAX		50
#define CLR_BATCH	RGB(0,0,0)
#define CLR_NOTIFY	RGB(0xFF, 0, 0)
#define CLR_RESULT	RGB(0x80, 0x80, 0x80)
#define CLR_SKIP	RGB(0x80, 0, 0)

COPCCommunicatorDlg::COPCCommunicatorDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(COPCCommunicatorDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}
COPCCommunicatorDlg::~COPCCommunicatorDlg()
{
	Finalize();
}
void COPCCommunicatorDlg::Init()
{
	for (int i = LOCK_BEGIN; i < LOCK_MAX; i++)
	{
		InitializeCriticalSection(&m_xLock[i]);
	}
	InitUiRectPos();
	InitUI();
	m_pOPCProcessBase = NULL;
	auto NotifyExe = [](CString strTarget, WPARAM wp, LPARAM lp)
	{
		HWND hWnd = ::FindWindow(NULL, strTarget);
		if (hWnd)
		{
			::PostMessage(hWnd, WM_GPIO_MSG, wp, lp);
		}
	};
	NotifyExe(AOI_MASTER_NAME, WM_CUSTOMERTYPE_INIT, 1);

#ifdef OFFLINE_DEBUG
	int nCustomerType = CUSTOMER_SYST_PP, nSubCustomerType = SUB_CUSTOMER_DONGGUAN_SONG8;//�M�ΫȻs�ݩ�
	OnCmdGPIO(WM_CUSTOMERTYPE_INIT, nCustomerType << 8 | nSubCustomerType);//�]�m�Ȼs
	OnCmdGPIO(WM_AOI_RESPONSE_CMD, WM_OPC_PARAMINIT_CMD);//�]�mAOI
#endif
}
void COPCCommunicatorDlg::InitUiRectPos()
{
	for (int i = UI_ITEM_BEGIN; i < UI_ITEM_END; i++)
	{
		POINT ptBase = { 0, 0 };
		POINT ptSize = { 0, 0 };
		CString strCaption;
		COLORREF xColor = NULL;
		switch (i)
		{
			case UI_LC_ADDRESS:
				ptBase = { 10, 150 };
				ptSize = { 450, 690 };
				break;
			case UI_LC_PARAM:
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
void COPCCommunicatorDlg::InitUI()
{
	for (int i = UI_LC_BEGIN; i < UI_LC_END; i++)
	{
		m_xUi[i].pList = new CListCtrl;
		m_xUi[i].pList->Create(WS_CHILD | WS_VISIBLE | WS_BORDER | LVS_REPORT | LVS_OWNERDATA, m_xUi[i].rcUi, this, i);
		m_xUi[i].pList->SetExtendedStyle(LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);
		g_AoiFont.SetWindowFont(m_xUi[i].pList, FontDef::typeT1);
		if (i == UI_LC_ADDRESS)
		{
			m_xUi[i].pList->InsertColumn(LIST_COL_FIELD, L"Field", LVCFMT_LEFT, 90);
			m_xUi[i].pList->InsertColumn(LIST_COL_NODEID, L"NodeId", LVCFMT_LEFT, 120);
			m_xUi[i].pList->InsertColumn(LIST_COL_VALUE, L"Value", LVCFMT_LEFT, 160);
			m_xUi[i].pList->InsertColumn(LIST_COL_TIME, L"Time", LVCFMT_LEFT, 90);
		}
		else if (i == UI_LC_PARAM)
		{
			m_xUi[i].pList->InsertColumn(LIST_COL_TITLE, L"Info", LVCFMT_LEFT, 130);
			m_xUi[i].pList->InsertColumn(LIST_COL_DATA, L"Data", LVCFMT_LEFT, 100);
		}
		else if (i == UI_LC_INFO)
		{
			m_xUi[i].pList->InsertColumn(LIST_COL_TITLE, L"Time", LVCFMT_LEFT, 60);
			m_xUi[i].pList->InsertColumn(LIST_COL_DATA, L"Info", LVCFMT_LEFT, 200);
		}
	}
	m_xUi[UI_LC_INFO].pList->SetItemCount(INFO_MAX);
}
void COPCCommunicatorDlg::Finalize()
{
	((IOPCProcess*)this)->AttachIn(NULL);
	if (m_pOPCProcessBase)
	{
		m_pOPCProcessBase->AttachOut(NULL);
		delete m_pOPCProcessBase;
		m_pOPCProcessBase = NULL;
	}
	for (int i = LOCK_BEGIN; i < LOCK_MAX; i++)
	{
		DeleteCriticalSection(&m_xLock[i]);
	}

}
void COPCCommunicatorDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(COPCCommunicatorDlg, CDialogEx)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_ADDRESS, OnLvnGetdispinfoAddress)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_PARAM, OnLvnGetdispinfoParam)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_INFO, OnLvnGetdispinfoInfo)
	ON_MESSAGE(WM_GPIO_MSG, OnCmdGPIO)
END_MESSAGE_MAP()

// COPCCommunicatorDlg �T���B�z�`��
BOOL COPCCommunicatorDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// �]�w����ܤ�����ϥܡC�����ε{�����D�������O��ܤ���ɡA
	// �ج[�|�۰ʱq�Ʀ��@�~
	SetIcon(m_hIcon, TRUE);			// �]�w�j�ϥ�
	SetIcon(m_hIcon, FALSE);		// �]�w�p�ϥ�

	// TODO:  �b���[�J�B�~����l�]�w
	Init();
#ifndef _DEBUG
	ShowWindow(SW_MINIMIZE);
#endif
	theApp.InsertDebugLog(L"OnInitDialog Done", LOG_OPC);
	return TRUE;  // �Ǧ^ TRUE�A���D�z�ﱱ��]�w�J�I
}

// �p�G�N�̤p�ƫ��s�[�J�z����ܤ���A�z�ݭn�U�C���{���X�A
// �H�Kø�s�ϥܡC���ϥΤ��/�˵��Ҧ��� MFC ���ε{���A
// �ج[�|�۰ʧ������@�~�C

void COPCCommunicatorDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // ø�s���˸m���e

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// �N�ϥܸm����Τ�ݯx��
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// �yø�ϥ�
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

// ��ϥΪ̩즲�̤p�Ƶ����ɡA
// �t�ΩI�s�o�ӥ\����o�����ܡC
HCURSOR COPCCommunicatorDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

///<summary>OPC�T��Ĳ�o</summary>
void COPCCommunicatorDlg::ON_OPC_NOTIFY(CString strMsg)
{
	std::pair<CString, __time64_t> xPair(strMsg, CTime::GetCurrentTime().GetTime());//�T��,�ɶ�
	int nSize = 0;
	EnterCriticalSection(&m_xLock[LOCK_INFO]);//LOCK_ON
	if (m_vInfo.size() >= INFO_MAX)//�j��50�Y���� 
	{
		m_vInfo.erase(m_vInfo.begin());
	}
	m_vInfo.push_back(xPair);//��^log��vector�̧���

	//nSize = (int)m_vInfo.size();
	LeaveCriticalSection(&m_xLock[LOCK_INFO]);//LOCK_OFF

	//�^����UI
	if (m_xUi[UI_LC_INFO].pList)
	{
		m_xUi[UI_LC_INFO].pList->Invalidate();
	}
}
void COPCCommunicatorDlg::ON_OPC_PARAM(CString strName, CString strValue)
{
	EnterCriticalSection(&m_xLock[LOCK_PARAM]);
	m_vParam.push_back(std::pair<CString, CString>(strName, strValue));
	int nSize = (int)m_vParam.size();
	LeaveCriticalSection(&m_xLock[LOCK_PARAM]);
	if (m_xUi[UI_LC_PARAM].pList)
	{
		m_xUi[UI_LC_PARAM].pList->SetItemCount(nSize);
		m_xUi[UI_LC_PARAM].pList->Invalidate();
	}
}
void COPCCommunicatorDlg::ON_OPC_FIELD_CHANGE(int nFieldId)
{
	m_xUi[UI_LC_ADDRESS].pList->RedrawItems(nFieldId, nFieldId);
}
void COPCCommunicatorDlg::OnLvnGetdispinfoAddress(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast<NMLVDISPINFO*>(pNMHDR);

	*pResult = NULL;

	if (!m_pOPCProcessBase || pDispInfo->item.iItem == EOF || pDispInfo->item.iItem >= (int)m_pOPCProcessBase->GetNodeSize()) return;

	if (LVIF_TEXT & pDispInfo->item.mask)
	{
		CString strText;
		switch (pDispInfo->item.iSubItem)
		{
			case LIST_COL_FIELD:
				strText = m_pOPCProcessBase->GetNodeName(pDispInfo->item.iItem);
				break;
			case LIST_COL_NODEID:
				strText = m_pOPCProcessBase->GetNodeId(pDispInfo->item.iItem);
				break;
			case LIST_COL_VALUE:
				strText = m_pOPCProcessBase->GetNodeValue(pDispInfo->item.iItem);
				break;
			case LIST_COL_TIME:
				strText = m_pOPCProcessBase->GetNodeTime(pDispInfo->item.iItem);
				break;
		}
		wcscpy_s(pDispInfo->item.pszText, strText.GetLength() + 1, strText);
	}
}
void COPCCommunicatorDlg::OnLvnGetdispinfoParam(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast<NMLVDISPINFO*>(pNMHDR);

	*pResult = NULL;
	if (pDispInfo->item.iItem == EOF || pDispInfo->item.iItem >= (int)m_vParam.size()) return;

	EnterCriticalSection(&m_xLock[LOCK_PARAM]);
	std::pair<CString, CString> xPair = m_vParam.at(pDispInfo->item.iItem);
	LeaveCriticalSection(&m_xLock[LOCK_PARAM]);
	if (LVIF_TEXT & pDispInfo->item.mask)
	{
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
void COPCCommunicatorDlg::OnLvnGetdispinfoInfo(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast<NMLVDISPINFO*>(pNMHDR);

	*pResult = NULL;
	if (pDispInfo->item.iItem == EOF || pDispInfo->item.iItem >= (int)m_vInfo.size()) return;

	EnterCriticalSection(&m_xLock[LOCK_INFO]);
	std::pair<CString, __time64_t> xPair = m_vInfo.at(pDispInfo->item.iItem);
	LeaveCriticalSection(&m_xLock[LOCK_INFO]);
	if (LVIF_TEXT & pDispInfo->item.mask)
	{
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
LRESULT COPCCommunicatorDlg::OnCmdGPIO(WPARAM wParam, LPARAM lParam)
{
	//�����ǰe����N�X
	switch (wParam)
	{
		case WM_CUSTOMERTYPE_INIT://�Ȼs�榡�T��
			InitOPCProcess(lParam >> 8 & 0xFF, lParam & 0xFF);
			theApp.InsertDebugLog(L"init customer type done", LOG_OPC);
			break;
		case WM_AOI_RESPONSE_CMD://AOI�^�ǰT��
			if (lParam == WM_OPC_PARAMINIT_CMD)
			{
				ON_OPEN_OPC(lParam);
			}
			break;
		case WM_OPC_WRITE_CMD://
			ON_RECEIVE_AOIDATA((OPCDataType)lParam);
			break;
		case WM_OPC_SET_CONNECT_CMD:
			if (lParam)
				ON_OPEN_OPC(NULL);
			else
				ON_CLOSE_OPC();
			break;
	}
	return 0;
}
void COPCCommunicatorDlg::InitOPCProcess(int nCustomerType, int nSubCustomerType)
{
	CString strLog;
	strLog.Format(L"%d", nCustomerType);
	ON_OPC_PARAM(L"CustomerType", strLog);
	strLog.Format(L"%d", nSubCustomerType);
	ON_OPC_PARAM(L"SubCustomerType", strLog);

	//�Ȥ�
	if (nCustomerType == CUSTOMER_SYST_PP)
	{
		//��ΫȤ�Ȼs��
		switch (nSubCustomerType)
		{
			case SUB_CUSTOMER_JIUJIANG:
				m_pOPCProcessBase = new CSystPPProcess;
				break;
			case SUB_CUSTOMER_DONGGUAN_SONG8:
				m_pOPCProcessBase = new CSystDONGGUANPPProcess();
				break;
		}
	}
	if (m_pOPCProcessBase == NULL) //default
	{
		m_pOPCProcessBase = new CSystPPProcess;
	}
	((IOPCProcess*)this)->AttachIn(m_pOPCProcessBase);
	m_pOPCProcessBase->AttachOut(this);

	m_xUi[UI_LC_ADDRESS].pList->SetItemCount(m_pOPCProcessBase->GetNodeSize());
}