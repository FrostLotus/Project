
// OPCCommunicatorDlg.h : 標頭檔
//

#pragma once
#include <vector>
#include "OPC\OPCProcessBase.h"

struct WND_OBJ{
	RECT rcUi;
	UINT nID;
	CListCtrl* pList;
	CButton *pBtn;
	CString strCaption;
	CEdit *pEdit;
	COLORREF xColor;
	WND_OBJ(){
		memset(&rcUi, 0, sizeof(rcUi));
		nID = 0;
		pList = NULL;
		pBtn = NULL;
		pEdit = NULL;
	}
	~WND_OBJ(){
		if (pList){
			pList->DestroyWindow();
			delete pList;
			pList = NULL;
		}
		if (pBtn){
			pBtn->DestroyWindow();
			delete pBtn;
			pBtn = NULL;
		}
		if (pEdit){
			pEdit->DestroyWindow();
			delete pEdit;
			pEdit = NULL;
		}
	}
};

// COPCCommunicatorDlg 對話方塊
class COPCCommunicatorDlg : public CDialogEx, public IOPCProcess
{
// 建構
public:
	COPCCommunicatorDlg(CWnd* pParent = NULL);	// 標準建構函式
	virtual ~COPCCommunicatorDlg();
// 對話方塊資料
	enum { IDD = IDD_OPCCOMMUNICATOR_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 支援


// 程式碼實作
protected:
	HICON m_hIcon;

	// 產生的訊息對應函式
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnLvnGetdispinfoAddress(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnLvnGetdispinfoParam(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnLvnGetdispinfoInfo(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg LRESULT OnCmdGPIO(WPARAM wParam, LPARAM lParam);
	DECLARE_MESSAGE_MAP()
protected:
	//IOPCProcess
	virtual void ON_OPC_NOTIFY(CString strMsg);
	virtual void ON_OPC_PARAM(CString strName, CString strValue);
	virtual void ON_OPC_FIELD_CHANGE(int nFieldId);
private:
	void Init();
	void InitUiRectPos();
	void InitUI();
	void Finalize();
	void InitOPCProcess(int nCustomerType, int nSubCustomerType);
private:
	COPCProcessBase *m_pOPCProcessBase;
	std::vector<std::pair<CString, __time64_t>> m_vInfo;
	std::vector<std::pair<CString, CString>> m_vParam;
	enum{
		LOCK_BEGIN = 0,
		LOCK_INFO = LOCK_BEGIN,
		LOCK_PARAM,
		LOCK_MAX,
	};
	CRITICAL_SECTION m_xLock[LOCK_MAX];
	enum{
		LIST_COL_FIELD = 0,
		LIST_COL_NODEID,
		LIST_COL_VALUE,
		LIST_COL_TIME,
	};
	enum{
		LIST_COL_TITLE,
		LIST_COL_DATA,
	};
	enum{
		UI_ITEM_BEGIN,
		//LISTCTRL
		UI_LC_BEGIN,
		UI_LC_ADDRESS = UI_LC_BEGIN,
		UI_LC_PARAM,
		UI_LC_INFO,
		UI_LC_END,

		UI_ITEM_END,
	};
	WND_OBJ m_xUi[UI_ITEM_END];
};
