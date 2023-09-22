
// MXComponentCommunicatorDlg.h : 標頭檔
//

#pragma once
#include "PLC\MelSecIOController.h"
#include <vector>
#define MAX_MESSAGE 30
// CMXComponentCommunicatorDlg 對話方塊
class CMXComponentCommunicatorDlg : public CDialogEx, public IMelSecIOControllerInfo
{
// 建構
public:
	CMXComponentCommunicatorDlg(CWnd* pParent = NULL);	// 標準建構函式
	virtual ~CMXComponentCommunicatorDlg();
// 對話方塊資料
	enum { IDD = IDD_MXCOMPONENTCOMMUNICATOR_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 支援

// 程式碼實作
protected:
	HICON m_hIcon;

	// 產生的訊息對應函式
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg LRESULT OnCmdGPIO(WPARAM wParam, LPARAM lParam);

	afx_msg void OnLvnGetdispInfo(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnLvnGetdispMessage(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnLvnGetdispPin(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg LRESULT OnCmdProcess(WPARAM wParam, LPARAM lParam);
	afx_msg void OnReset();
	DECLARE_MESSAGE_MAP()
private:
	void Init();
	void InitUiRectPos();
	void InitUI();
	void Finalize();
private:
	void HandleAOIResponse(LPARAM lParam);
protected:
	//IMelSecIOControllerInfo
	virtual void OnChangeInformation();
	virtual void OnAddMessage(CString strMsg);
	virtual void OnPinStatusChange(int nPin);
private:
	struct WND_OBJ{
		RECT rcUi;
		UINT nID;
		CListCtrl* pList;
		CButton *pBtn;
		CString strCaption;
		CEdit *pEdit;

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

	enum{
		LIST_COL_TITLE,
		LIST_COL_DATA,
	};
	enum{
		UI_ITEM_BEGIN,
		//LABEL
		UI_LABEL_BEGIN,
		UI_LABEL_INFO1 = UI_LABEL_BEGIN,
		UI_LABEL_INFO2,

		UI_LABEL_END,
		//LISTCTRL
		UI_LC_BEGIN,
		UI_LC_INFO = UI_LC_BEGIN,
		UI_LC_MESSAGE,
		UI_LC_PIN,
		UI_LC_END,
		//BTN
		UI_BTN_BEGIN,
		UI_BTN_RESET = UI_BTN_BEGIN,
		UI_BTN_END,

		UI_ITEM_END,
	};
	enum{
		LOCK_BEGIN = 0,
		LOCK_MESSAGE = LOCK_BEGIN,
		LOCK_MAX,
	};
	CRITICAL_SECTION m_xLock[LOCK_MAX];
	WND_OBJ m_xUi[UI_ITEM_END];
	std::vector<std::pair<CString, CString>> m_vMessage;
};
