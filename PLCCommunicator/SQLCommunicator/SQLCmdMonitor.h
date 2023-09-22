#pragma once
#include <vector>

struct SQLCmdParam{
	BYTE cHasCmd;
	int nCmdLen;
};

class CSQLCmdMonitor{
public:
	CSQLCmdMonitor();
	virtual ~CSQLCmdMonitor();

	BOOL IsReceiveCmd();
	void GetReceiceCmd(CString &strCmd);
private:
	void Init();
	void Finalize();
private:
	HANDLE m_hParamMapFile;
	HANDLE m_hCmdMapFile;
	SQLCmdParam *m_pParam;
	TCHAR* m_pCmd;
};