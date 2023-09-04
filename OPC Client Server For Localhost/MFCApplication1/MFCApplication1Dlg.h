
// MFCApplication1Dlg.h : 標頭檔
//
#include <open62541/client_config_default.h>
#include <open62541/client_highlevel.h>
#include <open62541/plugin/log_stdout.h>
#include <open62541/client_subscriptions.h>

#include <open62541/server.h>
#include <open62541/server_config_default.h>
#include <open62541/plugin/pki_default.h>
#include <stdlib.h>

#pragma once


// CMFCApplication1Dlg 對話方塊
class CMFCApplication1Dlg : public CDialogEx
{
// 建構
public:
	CMFCApplication1Dlg(CWnd* pParent = NULL);	// 標準建構函式
	~CMFCApplication1Dlg();
	UA_Client  *m_pClient = NULL;
	UA_UInt32	m_nSubId = -1;
	BOOL		m_bUABusy = FALSE;
	UA_Boolean	m_bServerRunning = false;
	UA_Boolean	m_bServerStop = true;
	// 對話方塊資料
	enum { IDD = IDD_MFCAPPLICATION1_DIALOG };

	void ClientSubScript();
	void ClientModifyValue();
	UA_Client *ConnectServer();
	void AddServerVariable(UA_Server *server);
	void AddServerMonitoredItem(UA_Server *server);

protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 支援


// 程式碼實作
protected:
	HICON m_hIcon;

	// 產生的訊息對應函式
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedClient();
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	afx_msg void OnBnClickedDiscover();
	afx_msg void OnBnClickedButtonServer();
	afx_msg void OnBnClickedButtonStop();
	afx_msg void OnClose();
	afx_msg void OnBnClickedCancel();
};
