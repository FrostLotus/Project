
// PLCCommunicatorDlg.h : 標頭檔
//

#pragma once
#include <vector>
#ifndef USE_MC_PROTOCOL
#include "MelSecIOController.h"
#include "PLCProcessBase.h"
#else
#include "PLCDataHandler.h"
#include "MelsecPlcSocket.h"
#endif

struct PLC_PARAM
{
	CString strPLCIp;
	AOI_CUSTOMERTYPE_ eCustomerType;
	AOI_SUBCUSTOMERTYPE_ eSubCustomerType;
#ifdef USE_MC_PROTOCOL
	int nPLCPort;
	int nFormat;
	PLC_FRAME_TYPE eFrameType;
#endif
};

struct WND_OBJ
{
	RECT rcUi;
	UINT nID;
	CListCtrl* pList;
	CButton* pBtn;
	CString strCaption;
	CEdit* pEdit;
	COLORREF xColor;
	WND_OBJ()
	{
		memset(&rcUi, 0, sizeof(rcUi));
		nID = 0;
		pList = NULL;
		pBtn = NULL;
		pEdit = NULL;
	}
	~WND_OBJ()
	{
		if (pList)
		{
			pList->DestroyWindow();
			delete pList;
			pList = NULL;
		}
		if (pBtn)
		{
			pBtn->DestroyWindow();
			delete pBtn;
			pBtn = NULL;
		}
		if (pEdit)
		{
			pEdit->DestroyWindow();
			delete pEdit;
			pEdit = NULL;
		}
	}
};

// CPLCCommunicatorDlg 對話方塊
class CPLCCommunicatorDlg : public CDialogEx
#ifndef USE_MC_PROTOCOL
	, public IPLCProcess
#else
	, public ISocketCallBack
#endif
{
	// 建構
public:
	CPLCCommunicatorDlg(BOOL bNoShow, CWnd* pParent = NULL);	// 標準建構函式
	~CPLCCommunicatorDlg();
	// 對話方塊資料
	enum { IDD = IDD_PLCCOMMUNICATOR_DIALOG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 支援
protected:
#ifdef USE_MC_PROTOCOL
	//ISocketCallBack
	virtual void ConnStatusCallBack(AOI_SOCKET_STATE xState);
	virtual void OnDeviceNotify(int nType, int nVal, CString strDes);
	virtual void OnPLCNewBatch(CString strOrder, CString strMaterial);
	virtual void OnPLCSYSTParam(BATCH_SHARE_SYST_PARAMCCL* pData);
	virtual void OnC10Change(WORD wC10);
	void HandleAOIResponse(LPARAM lParam);
	enum
	{
		OP_CREATE = 0,
		OP_DESTROY,
	};
	void OpPLC(int nOpCode);
#endif
	// 程式碼實作
protected:
	HICON m_hIcon;

	// 產生的訊息對應函式
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg LRESULT OnCmdProcess(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnCmdGPIO(WPARAM wParam, LPARAM lParam);
	afx_msg void OnWindowPosChanging(WINDOWPOS FAR* lpwndpos);
	afx_msg void OnLvnGetdispinfoPLCAddress(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnLvnGetdispinfoPLCParam(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnLvnGetdispinfoInfo(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnCustomdrawList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnTimer(UINT_PTR);
	DECLARE_MESSAGE_MAP()
private:
	void Init();
	void InitUiRectPos();
	void InitUI();
	void Finalize();
	void DrawInfo();
	void AddInfoText(CString& strInfo);
	void InitPLCProcess();
private:
	std::vector<std::pair<CString, __time64_t>> m_vPLCInfo;


#ifdef SHOW_DEBUG_BTN
	afx_msg void OnQueryAll();
	afx_msg void OnTestWrite();
	afx_msg void OnFlushAll();
#endif

	PLC_PARAM m_xParam;

	enum
	{
		LIST_COL_FIELD = 0,
		LIST_COL_ADDRESS,
		LIST_COL_VALUE,
		LIST_COL_TIME,
	};
	enum
	{
		LIST_COL_TITLE,
		LIST_COL_DATA,
	};
	enum
	{
		UI_ITEM_BEGIN,
		//LABEL
		UI_LABEL_BEGIN,
		UI_LABEL_BATCH = UI_LABEL_BEGIN,
		UI_LABEL_NOTIFY,
		UI_LABEL_RESULT,
		UI_LABEL_SKIP,
		UI_LABEL_TIME,
		UI_LABEL_END,
		//LISTCTRL
		UI_LC_BEGIN,
		UI_LC_PLCADDRESS = UI_LC_BEGIN,
		UI_LC_PLCPARAM,
		UI_LC_INFO,
		UI_LC_END,
		//BTN
		UI_BTN_BEGIN,
#ifdef SHOW_DEBUG_BTN
		UI_BTN_QUERYALL = UI_BTN_BEGIN,
		UI_BTN_TESTWRITE,

#endif
		UI_BTN_END,
		UI_CHKBTN_BEGIN,
#ifdef SHOW_DEBUG_BTN
		UI_BTN_FULSHALL = UI_CHKBTN_BEGIN,
#endif
		UI_CHKBTN_END,

		UI_ITEM_END,
	};
	WND_OBJ m_xUi[UI_ITEM_END];

	enum
	{
		LOCK_BEGIN = 0,
		LOCK_INFO = LOCK_BEGIN,
		LOCK_MAX,
	};
	CRITICAL_SECTION m_xLock[LOCK_MAX];
	BOOL m_bNoShow;
	UINT_PTR  m_tTimerReconnect;
	UINT_PTR  m_tTimer;
#ifndef USE_MC_PROTOCOL
	CPLCProcessBase* m_pPLCProcessBase;
	virtual void ON_PLC_NOTIFY(CString strMsg);
	virtual void ON_SET_PLCPARAM(BATCH_SHARE_SYSTCCL_INITPARAM& xParam);
	virtual void ON_PLCDATA_CHANGE(int nFieldId, void* pData, int nSizeInByte);
	virtual void ON_BATCH_PLCDATA_CHANGE(int nFieldFirst, int nFieldLast);
#else
	CMelsecPlcSocket* m_pPLC;
	CPLCDataHandler* m_pPLCDataHandler;
#endif
};
