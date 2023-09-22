
// EMCCommunicatorDlg.h : 標頭檔
//

#pragma once

#include "EMC\EMCParamSocket.h"
#include "EMC\EMCResultSocket.h"
#include "EMC\EMCDataHandler.h"
#include "EMC\EMCParser.h"


// CEMCCommunicatorDlg 對話方塊
class CEMCCommunicatorDlg : public CDialogEx, public IEMCSocketNotify, public IEMCNotify
{
// 建構
public:
	CEMCCommunicatorDlg(CWnd* pParent = NULL);	// 標準建構函式
	~CEMCCommunicatorDlg();
// 對話方塊資料
	enum { IDD = IDD_EMCCOMMUNICATOR_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 支援


// 程式碼實作
protected:
	HICON m_hIcon;

	// 產生的訊息對應函式
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnLvnGetdispinfoClient(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnLvnGetdispinfoParam(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnLvnGetdispinfoResult(NMHDR *pNMHDR, LRESULT *pResult);
#ifdef EMC_SIMLULATE
	afx_msg void OnSimulate(UINT uid);
#endif
	afx_msg LRESULT OnCmdGPIO(WPARAM wParam, LPARAM lParam);
	DECLARE_MESSAGE_MAP()
protected:
	//IEMCSocketNotify
	virtual void OnEMCParamSocketStatusChange(int nIndex);
	virtual void OnReceiveEMCParam(EMCParamSocket *pSrc, CString strData);
	virtual void OnEMCResultSocketStatusChange(AOI_SOCKET_STATE eStatus);
	virtual void OnReceiveEMCResult(BOOL bAckSuccess);
	virtual PRODUCT_TYPE GetProductType(){ return m_eType; };
	virtual void OnEMCParamTimeout(EMCParamSocket *pSrc, CString strData);
private:
	void Init();
	void InitUiRectPos();
	void InitUI();
	void InitSocket();
	void Finalize();
	void DrawInfo();
	void HandleAOIResponse(LPARAM lParam);
	int GetIndex(UI_TYPE eType, int nUiIndex, int nMax, EMC_FIELD *pField);
	CString GetCCLValue(EMC_CCL_FIELD_TYPE eType, EMC_CCL_DATA &xData);
	CString GetPPValue(EMC_PP_FIELD_TYPE eType, EMC_PP_DATA &xData);
#ifdef EMC_SIMLULATE
	void InitSimulate(); //模擬的時候才讀取ini資料, 否則參數應來自AOI
#endif
	enum{
		UI_ITEM_BEGIN,
		//LABEL
		UI_LABEL_BEGIN,
		UI_LABEL_INFO1 = UI_LABEL_BEGIN,
		UI_LABEL_INFO2,
#ifdef EMC_SIMLULATE
		UI_LABEL_STATION,
		UI_LABEL_MISSION,
		UI_LABEL_BATCHNAME,
		UI_LABEL_MATERIAL,
		UI_LABEL_SERIAL,
		UI_LABEL_DEFECTTYPE,
		UI_LABEL_INDEX,
		UI_LABEL_BOOKNUM,
		UI_LABEL_SHEETNUM,
		UI_LABEL_DEFECTBEGIN,
		UI_LABEL_DEFECTEND,
		UI_LABEL_LENGTH,
#endif
		UI_LABEL_END,
		//LISTCTRL
		UI_LC_BEGIN,
		UI_LC_CLIENT = UI_LC_BEGIN,
		UI_LC_PARAM,
		UI_LC_RESULT,
		UI_LC_END,
#ifdef EMC_SIMLULATE
		UI_BTN_BEGIN,
		UI_BTN_EXCEPT = UI_BTN_BEGIN,
		UI_BTN_CLOSE,
		UI_BTN_END,
		UI_EDIT_BEGIN,
		UI_EDIT_STATION = UI_EDIT_BEGIN,
		UI_EDIT_MISSION,
		UI_EDIT_BATCHNAME,
		UI_EDIT_MATERIAL,
		UI_EDIT_SERIAL,
		UI_EDIT_DEFECTTYPE,
		UI_EDIT_INDEX,
		UI_EDIT_BOOKNUM,
		UI_EDIT_SHEETNUM,
		UI_EDIT_DEFECTBEGIN,
		UI_EDIT_DEFECTEND,
		UI_EDIT_LENGTH,
		UI_EDIT_END,
		UI_GB_BEGIN,
		UI_GB_SIMULATE = UI_GB_BEGIN,
		UI_GB_END,
#endif
		UI_ITEM_END,
	};

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
		LIST_COL_IP,
		LIST_COL_PORT,
	};
	enum{
		LIST_COL_FIELDNAME,
		LIST_COL_VALUE1,
		LIST_COL_VALUE2,
		LIST_COL_VALUE3,
	};
	WND_OBJ m_xUi[UI_ITEM_END];
	EMCParamSocket *m_pServer;
	EMCResultSocketMgr *m_pClientMgr;
	CEMCDataHandler *m_pEMCDataHandler;
#ifdef EMC_SIMLULATE
	CEMCDataHandler *m_pSimulateDataHandler;
#endif
	vector<EMC_CCL_DATA> m_vCCLParam;
	EMC_CCL_DATA m_xCCLResult;

	EMC_PP_DATA m_xPPParam;
	EMC_PP_DATA m_xPPResult;

	PRODUCT_TYPE m_eType;
	CString m_strServerIp;
	UINT m_nServerPort;
	UINT m_nListenPort;
#ifdef EMC_SIMLULATE
	CWinThread *m_pAckSuccessThread;
#endif
};
