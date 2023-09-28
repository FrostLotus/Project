#include "stdafx.h"
#include "SQLCmdMonitor.h"
#include "SQLControl.h"
#include "SQLCommunicator.h"

TCHAR MEM_SQLPARAM_FILE[] = TEXT("Local\\SQLParam.map");
TCHAR MEM_CMD_FILE[] = TEXT("Local\\SQLCmd.map");
TCHAR SQL_MUTEXLOCK[] = TEXT("SQLMutex");

const int ctCMD_MEM_SIZE = 1024;
CSQLCmdMonitor::CSQLCmdMonitor()
{
	Init();
}
CSQLCmdMonitor::~CSQLCmdMonitor()
{
	Finalize();
}
void CSQLCmdMonitor::Init()
{
	m_hParamMapFile = NULL;
	m_hCmdMapFile = NULL; 
	m_pParam = NULL;
	m_pCmd = NULL;

	m_hParamMapFile = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, MEM_SQLPARAM_FILE);

	if (m_hParamMapFile){
		m_pParam = (SQLCmdParam*)MapViewOfFile(m_hParamMapFile,   // handle to map object
			FILE_MAP_ALL_ACCESS, // read/write permission
			0,
			0,
			sizeof(SQLCmdParam));
		if (m_pParam){
			if (g_pSQLControl){
				g_pSQLControl->AddLog(L"MapViewOfFile Done");
			}
		}
		else{
			if (g_pSQLControl){
				g_pSQLControl->AddLog(L"MapViewOfFile Fail");
			}
		}
	}
	else{
		if (g_pSQLControl){
			g_pSQLControl->AddLog(L"OpenFileMapping Fail");
		}
	}

	m_hCmdMapFile = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, MEM_CMD_FILE);

	if (m_hCmdMapFile){
		m_pCmd = (TCHAR*)MapViewOfFile(m_hCmdMapFile,   // handle to map object
			FILE_MAP_ALL_ACCESS, // read/write permission
			0,
			0,
			ctCMD_MEM_SIZE);
		if (m_pCmd){
			if (g_pSQLControl){
				g_pSQLControl->AddLog(L"m_pCmd MapViewOfFile Done");
			}
		}
		else{
			if (g_pSQLControl){
				g_pSQLControl->AddLog(L"m_pCmd MapViewOfFile Fail");
			}
		}
	}
	else{
		if (g_pSQLControl){
			g_pSQLControl->AddLog(L"m_hCmdMapFile OpenFileMapping Fail");
		}
	}
}
void CSQLCmdMonitor::Finalize()
{
	UnmapViewOfFile(m_pParam);
	CloseHandle(m_hParamMapFile);
}
BOOL CSQLCmdMonitor::IsReceiveCmd()
{
	if (m_pParam){
		return m_pParam->cHasCmd;
	}
	return FALSE;
}
void CSQLCmdMonitor::GetReceiceCmd(CString &strCmd)
{
	//get from mem
	HANDLE hMutex;
	hMutex = CreateMutex(NULL, FALSE, SQL_MUTEXLOCK);
	WaitForSingleObject(hMutex, INFINITE);
	if (m_pParam && m_pCmd){
		strCmd = CString(m_pCmd, m_pParam->nCmdLen / sizeof(TCHAR));

		m_pParam->cHasCmd = 0;
	}
	ReleaseMutex(hMutex);
	CloseHandle(hMutex);
}
