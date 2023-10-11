// PLCCommunicator.cpp : �w�q���ε{�������O�欰�C
#include "stdafx.h"
#include "PLCCommunicator.h"
#include "PLCCommunicatorDlg.h"
#include "AppProcess.h"
#include "ZergLog.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif
//=================================================================
/// CPLCCommunicatorApp
BEGIN_MESSAGE_MAP(CPLCCommunicatorApp, CWinApp)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()

LONG WINAPI OnCallback_ZergExcpHandler(struct _EXCEPTION_POINTERS* lpstExceptionInfo) //seanchen 20130607
{
#ifdef _UNICODE					
	wchar_t	szWorkingDir[_MAX_PATH];	// the working path
	_wgetcwd(szWorkingDir, _MAX_PATH);
#else
	char	szWorkingDir[_MAX_PATH];	// the working path
	_getcwd(szWorkingDir, _MAX_PATH);
#endif

	CString strCurrentPath = szWorkingDir;
	CStringA strCurrentPathA;
	strCurrentPathA = strCurrentPath;

	CString strDebug;
	strDebug.Format(_T("\n ....... xxxxxxxx OnCallback_ZergExcpHandler xxxxxxxx ....... \n"));
	OutputDebugString(strDebug);

	int nEnWriteZerg, nEnShowMsg;
	ReadZergFunctionIni(nEnWriteZerg, nEnShowMsg);
	bool bShow;
	if (nEnShowMsg)
	{
		bShow = true;
	}
	else
	{
		bShow = false;
	}
	::WriteZergLog(lpstExceptionInfo, strCurrentPathA + "\\", bShow, true);
	::TerminateProcess(GetCurrentProcess(), 1);

	return EXCEPTION_EXECUTE_HANDLER; //This usually results in process termination.
}
// CPLCCommunicatorApp �غc
CPLCCommunicatorApp::CPLCCommunicatorApp()
{
	// �䴩���s�Ұʺ޲z��
	m_dwRestartManagerSupportFlags = AFX_RESTART_MANAGER_SUPPORT_RESTART;

	// TODO:  �b���[�J�غc�{���X�A
	// �N�Ҧ����n����l�]�w�[�J InitInstance ��
}
// �Ȧ����@�� CPLCCommunicatorApp ����
CPLCCommunicatorApp theApp;
// CPLCCommunicatorApp ��l�]�w
BOOL CPLCCommunicatorApp::InitInstance()
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

	// �إߴ߼h�޲z���A�H����ܤ���]�t
	// ����߼h���˵��δ߼h�M���˵�����C
	CShellManager* pShellManager = new CShellManager;

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

	CString strCmd = GetCommandLine();
	BOOL bNoShow = FALSE;
	HWND hWnd = ::FindWindow(NULL, PLC_COMMUNICATOR_NAME);
	if (hWnd)
	{
		if (strCmd.Find(_T("/EXIT")) >= 0)
		{
			::PostMessage(hWnd, WM_LOCAL_MSG, WM_EXIT, 0);
			return FALSE;
		}
	}
#ifndef SHOW_WINDOW
	if (strCmd.Find(_T("/NOSHOW")) >= 0)
	{
		bNoShow = TRUE;
	}
#endif
	CString strExe;
	strExe.Format(L"%s.exe", PLC_COMMUNICATOR_NAME);
	if (AppProcess::FIND_PROCESS(strExe))
	{
		AppProcess::TERMINATE_PROCESS(strExe, GetCurrentProcessId()); //20200323
	}
	while (AppProcess::FIND_PROCESS(strExe))
	{
		InsertDebugLog(L"still exist");
		TRACE(L"still exist");
		Sleep(100); // wait for kill
	}
	::StartZergExcpFilter(OnCallback_ZergExcpHandler);

	CPLCCommunicatorDlg dlg(bNoShow);
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
	InsertDebugLog(_T("End"));

	// �]���w�g������ܤ���A�Ǧ^ FALSE�A�ҥH�ڭ̷|�������ε{���A
	// �ӫD���ܶ}�l���ε{�����T���C
	return FALSE;
}
