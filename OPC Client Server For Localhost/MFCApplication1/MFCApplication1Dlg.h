
// MFCApplication1Dlg.h : ���Y��
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


// CMFCApplication1Dlg ��ܤ��
class CMFCApplication1Dlg : public CDialogEx
{
// �غc
public:
	CMFCApplication1Dlg(CWnd* pParent = NULL);	// �зǫغc�禡
	~CMFCApplication1Dlg();
	UA_Client  *m_pClient = NULL;
	UA_UInt32	m_nSubId = -1;
	BOOL		m_bUABusy = FALSE;
	UA_Boolean	m_bServerRunning = false;
	UA_Boolean	m_bServerStop = true;
	// ��ܤ�����
	enum { IDD = IDD_MFCAPPLICATION1_DIALOG };

	void ClientSubScript();
	void ClientModifyValue();
	UA_Client *ConnectServer();
	void AddServerVariable(UA_Server *server);
	void AddServerMonitoredItem(UA_Server *server);

protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV �䴩


// �{���X��@
protected:
	HICON m_hIcon;

	// ���ͪ��T�������禡
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
