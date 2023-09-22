#pragma once

class AppProcess
{
public:
	AppProcess(CString strAppName);
	~AppProcess();

	static BOOL FIND_PROCESS(CString strProcName);
	static BOOL TERMINATE_PROCESS(CString strName, DWORD dwCurProcessId, BOOL bEnforce = TRUE);
protected:
	BOOL IS_APP_RUNNING();
private:
	void Init();
	void Finalize();

private:
	CString m_strAppName;
};

