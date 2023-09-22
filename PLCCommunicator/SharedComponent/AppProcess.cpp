#include "stdafx.h"
#include "AppProcess.h"
#include "psapi.h"

AppProcess::AppProcess(CString strAppName)
{
	Init();
	m_strAppName = strAppName;
}


AppProcess::~AppProcess()
{
	Finalize();
}

void AppProcess::Init()
{

}
void AppProcess::Finalize()
{


}
BOOL AppProcess::IS_APP_RUNNING()
{
	BOOL bExit = FIND_PROCESS(m_strAppName);
	return bExit;
}

BOOL AppProcess::FIND_PROCESS(CString strProcName)
{
	BOOL bFind = FALSE;

	strProcName.MakeUpper();

	TCHAR xName[255];
	memset(xName, 0, sizeof(TCHAR) * 255);
	DWORD xPID = ::GetCurrentProcessId();
	GetModuleBaseName(::GetCurrentProcess(), NULL, xName, 255);
	DWORD aProcess[1024];
	memset(aProcess, 0, 1024);
	DWORD chNeed = 0;
	DWORD cProcess = 0;
	BOOL FindProcessFlag = EnumProcesses(aProcess, sizeof(aProcess), &chNeed);
	if (FindProcessFlag){
		cProcess = chNeed / sizeof(DWORD);
		for (int i = 0; i<(int)cProcess; i++){
			if (aProcess[i] == xPID)
				continue;
			void *hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, aProcess[i]);
			if (hProcess != NULL){
				TCHAR mname[255];
				memset(mname, 0, sizeof(TCHAR) * 255);
				GetModuleBaseName(hProcess, NULL, mname, 255);
				CloseHandle(hProcess);

				CString strT;
				strT = mname;
				strT.MakeUpper();

				if (strProcName == strT){
					bFind = TRUE;
					break;
				}
			}
		}
	}
	return bFind;
}
BOOL AppProcess::TERMINATE_PROCESS(CString strName, DWORD dwCurProcessId, BOOL bEnforce)
{
	BOOL bFind = FALSE;

	strName.MakeUpper();

	TCHAR xName[255];
	memset(xName, 0, sizeof(TCHAR) * 255);
	DWORD xPID = ::GetCurrentProcessId();
	GetModuleBaseName(::GetCurrentProcess(), NULL, xName, 255);
	DWORD aProcess[1024];
	memset(aProcess, 0, 1024);
	DWORD chNeed = 0;
	DWORD cProcess = 0;
	BOOL FindProcessFlag = EnumProcesses(aProcess, sizeof(aProcess), &chNeed);
	if (FindProcessFlag){
		cProcess = chNeed / sizeof(DWORD);
		for (int i = 0; i<(int)cProcess; i++){
			if (aProcess[i] == xPID)
				continue;
			void *hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, aProcess[i]);
			if (hProcess != NULL){
				DWORD dExit = NULL;
				GetExitCodeProcess(hProcess, &dExit);

				TCHAR mname[255];
				memset(mname, 0, sizeof(TCHAR) * 255);
				GetModuleBaseName(hProcess, NULL, mname, 255);
				CString strT;
				strT = mname;
				strT.MakeUpper();
				CloseHandle(hProcess);
				if (strName == strT){
					bFind = TRUE;
					CString strCmd;
					if (bEnforce){
						//strCmd.Format(_T("taskKill /f /im %s"), strName);
						strCmd.Format(_T("cmd /c wmic process where \"name = \'%s\' and ProcessId != %d \" call terminate"), strName, dwCurProcessId); //20200331
					}
					else{
						strCmd.Format(_T("taskKill /im %s"), strName);
					}
					WinExec(CW2A((LPCTSTR)strCmd), SW_SHOW);
				}
			}
		}
	}
	return bFind;
}