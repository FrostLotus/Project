
// MXComponentCommunicatorDlg.h : ���Y��
//

#pragma once
#include "PLC\MelSecIOController.h"
#include <vector>
#define MAX_MESSAGE 30
// CMXComponentCommunicatorDlg ��ܤ��
class CMXComponentCommunicatorDlg : public CDialogEx, public IMelSecIOControllerInfo
{
// �غc
public:
	CMXComponentCommunicatorDlg(CWnd* pParent = NULL);	// �зǫغc�禡
	virtual ~CMXComponentCommunicatorDlg();
// ��ܤ�����
	enum { IDD = IDD_MXCOMPONENTCOMMUNICATOR_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV �䴩

// �{���X��@
protected:
	HICON m_hIcon;

	// ���ͪ��T�������禡
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
