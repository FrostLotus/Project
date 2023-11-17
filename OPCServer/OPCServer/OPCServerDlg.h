
// OPCServerDlg.h : 標頭檔
//
#include <open62541/client_config_default.h>
#include <open62541/client_highlevel.h>
#include <open62541/plugin/log_stdout.h>
#include <open62541/client_subscriptions.h>

#include <open62541/server.h>
#include <open62541/server_config_default.h>
#include <open62541/plugin/pki_default.h>
#include <stdlib.h>
#include <vector>
#include <queue>
#pragma once

#define MY_SERVER_PORT		4841

struct UA_NODE
{
	int		nIndex;
	TCHAR	tszItemName[MAX_PATH];
	TCHAR	tszNodeId[MAX_PATH];
	TCHAR	tszDataType[MAX_PATH];
	TCHAR	tszInitial[MAX_PATH];
	WORD	wInitialValue;
	int		nLength;
};
struct UA_MONITOR
{
	int nIndex;
	TCHAR szText[MAX_PATH];
};
// COPCServerDlg 對話方塊
class COPCServerDlg : public CDialogEx
{
// 建構
public:
	COPCServerDlg(CWnd* pParent = NULL);	// 標準建構函式
	void AddServerMonitoredItem(UA_Server *server);
	void AddServerVariable(UA_Server *server);
	BOOL CheckAllNodeValue(UA_NODE &uaTemp, BOOL bAppend);
	UA_Boolean	m_bServerRunning = false;
	UA_Boolean	m_bServerStop = true;
	CListCtrl	m_listVar;
	std::vector<UA_NODE>	m_vecNode;
	std::queue<UA_MONITOR>	m_queMonitor;
	BOOL		m_bReadIni = FALSE;
	UA_Server	*m_pServer = NULL;
	CRITICAL_SECTION m_csSection;
	int			m_nServerPort = MY_SERVER_PORT;
	// 對話方塊資料
	enum { IDD = IDD_OPCSERVER_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 支援
	void InsertItemFromIniFile();
	void WriteNode2Ini(TCHAR *ptszSection, UA_NODE &uaTemp);
	int GetNodeIndex(int nPos);
// 程式碼實作
protected:
	HICON m_hIcon;
	CComboBox m_cmbType;
	// 產生的訊息對應函式
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedButtonServer();
	afx_msg void OnBnClickedButtonStop();
	afx_msg void OnBnClickedCancel();
	afx_msg void OnClose();
	afx_msg void OnLvnItemchangedListVar(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnBnClickedButtonUpdate();
	virtual LRESULT WindowProc(UINT message, WPARAM wParam, LPARAM lParam);
	afx_msg void OnBnClickedButtonAdd();
	afx_msg void OnBnClickedButtonSaveFile();
	afx_msg void OnBnClickedButtonDelete();
};
