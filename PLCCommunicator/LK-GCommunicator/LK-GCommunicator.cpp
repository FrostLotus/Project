
// LK-GCommunicator.cpp : �w�q���ε{�������O�欰�C
//

#include "stdafx.h"
#include "LK-GCommunicator.h"
#include "LK-GCommunicatorDlg.h"
#include "AppProcess.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

#include <vector>
using namespace std;
vector<CString> GetEachStringBySep(CString sStr, TCHAR tSep)
{
	vector<CString> vEachStr;
	TCHAR* token = NULL;
	TCHAR* pNextToken = sStr.GetBuffer();
	TCHAR xSep[2] = { NULL, NULL };
	xSep[0] = tSep;
	do {
		while (pNextToken && *pNextToken && (*pNextToken == tSep)) {
			vEachStr.push_back(CString(_T('\0')));
			pNextToken++;
		}
		token = _tcstok_s(NULL, xSep, &pNextToken);
		if (token) {
			CString xNew(token);
			if (xNew != _T("\r\n")) {
				vEachStr.push_back(xNew);
			}
		}
	} while (token != NULL);
	return vEachStr;
}
// CLKGCommunicatorApp

BEGIN_MESSAGE_MAP(CLKGCommunicatorApp, CWinApp)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()


// CLKGCommunicatorApp �غc

CLKGCommunicatorApp::CLKGCommunicatorApp()
{
	// �䴩���s�Ұʺ޲z��
	m_dwRestartManagerSupportFlags = AFX_RESTART_MANAGER_SUPPORT_RESTART;

	// TODO:  �b���[�J�غc�{���X�A
	// �N�Ҧ����n����l�]�w�[�J InitInstance ��
}


// �Ȧ����@�� CLKGCommunicatorApp ����

CLKGCommunicatorApp theApp;


// CLKGCommunicatorApp ��l�]�w

BOOL CLKGCommunicatorApp::InitInstance()
{
	// ���p���ε{����T�M����w�ϥ� ComCtl32.dll 6 (�t) �H�᪩���A
	// �ӱҰʵ�ı�Ƽ˦��A�b Windows XP �W�A�h�ݭn InitCommonControls()�C
	// �_�h����������إ߳��N���ѡC
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// �]�w�n�]�t�Ҧ��z�Q�n�Ω����ε{������
	// �q�α�����O�C
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinApp::InitInstance();
	StartLogServer();

	AfxEnableControlContainer();

	CString strExe= L"LK-GCommunicator.exe";
	if (AppProcess::FIND_PROCESS(strExe)){
		AppProcess::TERMINATE_PROCESS(strExe, GetCurrentProcessId()); //20200323
	}
	while (AppProcess::FIND_PROCESS(strExe)){
		InsertDebugLog(L"still exist");
		TRACE(L"still exist");
		Sleep(100); // wait for kill
	}
	// �إߴ߼h�޲z���A�H����ܤ���]�t
	// ����߼h���˵��δ߼h�M���˵�����C
	CShellManager *pShellManager = new CShellManager;

	// �Ұ� [Windows ���] ��ı�ƺ޲z���i�ҥ� MFC ��������D�D
	CMFCVisualManager::SetDefaultManager(RUNTIME_CLASS(CMFCVisualManagerWindows));

	// �зǪ�l�]�w
	// �p�G�z���ϥγo�ǥ\��åB�Q���
	// �̫᧹�����i�����ɤj�p�A�z�i�H
	// �q�U�C�{���X�������ݭn����l�Ʊ`���A
	// �ܧ��x�s�]�w�Ȫ��n�����X
	// TODO:  �z���ӾA�׭ק惡�r��
	// (�Ҧp�A���q�W�٩β�´�W��)
	SetRegistryKey(_T("���� AppWizard �Ҳ��ͪ����ε{��"));

	InsertDebugLog(_T("Start"), LOG_THICK);

	CString strCmd = GetCommandLine();
	
	InsertDebugLog(strCmd, LOG_THICK);
	UINT nComId1 = 2, nComId2 = 4, nRate = 200, nTime = 200;

	vector<CString> vParam = GetEachStringBySep(strCmd, L' ');
	for (auto& strParam : vParam) {
		CString strCompare, strValue;
		if (strParam.Find(L"/COM:") != -1 && strParam.GetLength() >= 6) {
			strValue = strParam.Mid(5);
			vector<CString> vTemp = GetEachStringBySep(strValue, L',');
			if (vTemp.size() == 1) {
				nComId1 = _ttoi(vTemp.at(0));
				nComId2 = 0;
			}
			else if (vTemp.size() >= 2) {
				nComId1 = _ttoi(vTemp.at(0));
				nComId2 = _ttoi(vTemp.at(1));
			}
		}
		else if (strParam.Find(L"/RATE:") != -1 && strParam.GetLength() >= 7) {
			strValue = strParam.Mid(6);
			nRate = _ttoi(strValue);
		}
		if (strParam.Find(L"/TIME:") != -1 && strParam.GetLength() >= 7) {
			strValue = strParam.Mid(6);
			nTime = _ttoi(strValue);
		}
	}

	CLKGCommunicatorDlg dlg(nComId1, nComId2, nRate, nTime);
	m_pMainWnd = &dlg;
	INT_PTR nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
		// TODO:  �b����m��ϥ� [�T�w] �Ӱ���ϥι�ܤ����
		// �B�z���{���X
	}
	else if (nResponse == IDCANCEL)
	{
		// TODO:  �b����m��ϥ� [����] �Ӱ���ϥι�ܤ����
		// �B�z���{���X
	}
	else if (nResponse == -1)
	{
		TRACE(traceAppMsg, 0, "ĵ�i: ��ܤ���إߥ��ѡA�]���A���ε{���N�~�פ�C\n");
		TRACE(traceAppMsg, 0, "ĵ�i: �p�G�z�n�b��ܤ���W�ϥ� MFC ����A�h�L�k #define _AFX_NO_MFC_CONTROLS_IN_DIALOGS�C\n");
	}

	// �R���W���ҫإߪ��߼h�޲z���C
	if (pShellManager != NULL)
	{
		delete pShellManager;
	}
	StopLogServer();
	InsertDebugLog(_T("End"), LOG_THICK);
	// �]���w�g������ܤ���A�Ǧ^ FALSE�A�ҥH�ڭ̷|�������ε{���A
	// �ӫD���ܶ}�l���ε{�����T���C
	return FALSE;
}

