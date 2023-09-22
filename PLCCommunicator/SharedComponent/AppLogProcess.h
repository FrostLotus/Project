#pragma once
#include "afxwin.h"

const unsigned int _LOG_SIZE = 1024 * 1024;

enum AOI_LOG_TYPE{
	LOG_TYPE_BEGIN = 0,
	LOG_SYSTEM = 0,
	LOG_DEBUG,
	LOG_PLCSOCKET,
	LOG_PLCC10,
	LOG_EMCDEBUG,
	LOG_EMCSYSTEM,
	LOG_OPC,
	LOG_THICK,
	LOG_MSSQL,
	LOG_TYPE_MAX
};

typedef struct LOG_ITEM_INFO_
{
	AOI_LOG_TYPE xType;
	CString strFile;
	UINT nLimitSize;
}LOG_ITEM_INFO;

const LOG_ITEM_INFO ctLOG_INFO[] = {
	{ LOG_SYSTEM, _T("PLCCommunicator.PLC"), _LOG_SIZE },
	{ LOG_DEBUG, _T("PLCCommunicator_DEBUG.PLC"), _LOG_SIZE },
	{ LOG_PLCSOCKET, _T("PLCCommunicator_Socket.PLC"), _LOG_SIZE },
	{ LOG_PLCC10, _T("PLCCommunicator_C10.PLC"), _LOG_SIZE },
	{ LOG_EMCDEBUG, _T("EMCCommunicator_DEBUG.log"), _LOG_SIZE },
	{ LOG_EMCSYSTEM, _T("EMCCommunicator_System.log"), _LOG_SIZE },
	{ LOG_OPC, _T("OPCCommunicator.log"), _LOG_SIZE },
	{ LOG_THICK, _T("LK-Communicator.log"), _LOG_SIZE },
	{ LOG_MSSQL, _T("MSSQL_PROCESS.log"), _LOG_SIZE },
};


#define WM_LOG_PROCESS		(WM_USER + 10)
#define WM_LOG_TERMINATE	(WM_USER + 11)

class AppLogBase
{
public:
	AppLogBase();
	~AppLogBase();
public:
	int LogFileCount() { return m_nFileCount; };
protected:
	void InsertLog(CString &xMsg, AOI_LOG_TYPE xType = LOG_SYSTEM);
private:
	int m_nFileCount;
	TCHAR m_workingDir[_MAX_PATH];
};


class CAoiLogThread :
	public CWinThread
	, public AppLogBase
{
	DECLARE_DYNCREATE(CAoiLogThread)
public:
	CAoiLogThread(void) {};
	~CAoiLogThread(void) {};
	virtual BOOL InitInstance() { return TRUE; };
	virtual int ExitInstance() { return CWinThread::ExitInstance(); };

protected:
	DECLARE_MESSAGE_MAP()
	void OnLogMessage(WPARAM wParam, LPARAM lParam);
	void OnLogExit(WPARAM wParam, LPARAM lParam);
};


class AppLogProcess :
	public AppLogBase
{
public:
	AppLogProcess();
	~AppLogProcess();
	void StartLogServer() { OpLogThread(OP_THREAD_CREATE); };
	void StopLogServer() { OpLogThread(OP_THREAD_DESTROY); };
	void InsertDebugLog(CString xMsg, AOI_LOG_TYPE xType = LOG_SYSTEM);
	CString GetLogFileName(int xType);

private:
	void Init();
	void Finalize();
	enum {
		OP_THREAD_CREATE = 0,
		OP_THREAD_DESTROY
	};
	void OpLogThread(int nOpCode);

private:
	CWinThread *m_pLogThread;
};

