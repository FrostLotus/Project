
// LK-GCommunicatorDlg.h : 標頭檔
//

#pragma once
#include "LK-G\ThickMachineManager.h"
#include <vector>
using namespace std;

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

// CLKGCommunicatorDlg 對話方塊
class CLKGCommunicatorDlg : public CDialogEx, public IThickMachineManager
{
// 建構
public:
	CLKGCommunicatorDlg(UINT nComId1, UINT nComId2, UINT nRate, UINT nTime, CWnd* pParent = NULL);	// 標準建構函式
	~CLKGCommunicatorDlg();
// 對話方塊資料
	enum { IDD = IDD_LKGCOMMUNICATOR_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 支援


// 程式碼實作
protected:
	HICON m_hIcon;

	// 產生的訊息對應函式
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnLvnGetdispinfoInfo(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnLvnGetdispinfoParam(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnLvnGetdispinfoThick(NMHDR *pNMHDR, LRESULT *pResult);
#ifdef EXPORTCSV
	afx_msg void OnExportCSV(); 
#endif
	DECLARE_MESSAGE_MAP()
protected:
	virtual void ON_THICKMACHINE_MSG(ThickMachine eMachine, CString &strMsg);
	virtual void ON_THICKMACHINE_INFO(ThickMachine eMachine, OUT_TYPE eType, float *pData);
	virtual void ON_THICKMACHINE_PARAM(ThickMachine eMachine, CString &strMsg, CString &strValue);

private:
	void Init();
	void InitUiRectPos();
	void InitUI();
	void Finalize();
private:
	std::vector<std::pair<CString, __time64_t>> m_vInfo;
	std::vector<std::pair<CString, CString>> m_vParam[MACHINE_MAX];

	std::vector<THICK_INFO> m_vThick[MACHINE_MAX];
	enum{
		LOCK_BEGIN = 0,
		LOCK_INFO = LOCK_BEGIN,
		LOCK_PARAM,
		LOCK_THICK,
		LOCK_MAX,
	};
	CRITICAL_SECTION m_xLock[LOCK_MAX];
	CThickMachineManager *m_pThickMachine;
	enum{
		LIST_DATACOL_VALUE1 = 0,
		LIST_DATACOL_VALUE2,
		LIST_DATACOL_TIME,
		LIST_DATACOL_MAX
	};
	enum{
		LIST_COL_TITLE,
		LIST_COL_DATA,
		LIST_COL_MAX
	};
	enum{
		UI_ITEM_BEGIN,
		//LISTCTRL
		UI_LC_BEGIN,
		UI_LC_THICK1 = UI_LC_BEGIN,
		UI_LC_THICK2,
		UI_LC_PARAM1,
		UI_LC_PARAM2,
		UI_LC_INFO,
		UI_LC_END,
#ifdef EXPORTCSV
		UI_BTN_BEGIN,
		UI_BTN_CSV,
		UI_BTN_END,
#endif

		UI_ITEM_END,
	};
	WND_OBJ m_xUi[UI_ITEM_END];
	HWND m_hAOI;
};
