#pragma once

#include <sqlext.h>
#include <sqltypes.h>
#include <sql.h>
#include "AppLogProcess.h"
#include <vector>

#define MSSQL_BULKINSERT //bulk insert, save sql server memory

struct ColumnInfo{
	CString strName;
	int nColumnNumber;
	int nTargetType;
	int nSizeInByte;
	BYTE *pValue;
};
class SQLControl: public AppLogProcess
{
public:
	SQLControl();
	virtual ~SQLControl();

    enum DATABASE_MODE
    {
        DB_SHEET,     // »Éºä
        DB_WEB_BOARD, // ÂH¦X¤ù
    };
    BOOL ExecuteSQLCommand( const wchar_t* pSQLCommand, BOOL bInsert = FALSE, BOOL bDisConnect = TRUE );
	BOOL ExecuteSelectSQLCommand(const wchar_t* pSQLCommand, std::vector<ColumnInfo> &vCol, std::vector<CString> &vResult);


	void SetExternalConnectString(CString strConnectString) { m_strExternalConnectString = strConnectString; };
	CString GetExternalConnectString(){ return m_strExternalConnectString; }
	void AddLog(CString strLog);
	CString GetWorPath(){ return m_workingDir; }
protected:
	virtual void GetConnectString(wchar_t* szConnectString) = 0;
	virtual void OnConnectSuccess() = 0;
	virtual BOOL DoExecuteSQLCommand(const wchar_t* pSQLCommand, BOOL bInsert) = 0;
	virtual BOOL DoExecuteSelectSQLCommand(const wchar_t* pSQLCommand, std::vector<ColumnInfo> &vCol, std::vector<CString> &vResult)=0;

	virtual void OnExecuteError(){};
	virtual BOOL RetryOnFail(){ return FALSE; }
	BOOL SQLExecute2(SQLHANDLE sqlConnectHandle, SQLHANDLE& sqlStmtHandle, const SQLWCHAR* pSQLCommand);
	BOOL SQLExecute(SQLHANDLE sqlConnectHandle, const SQLWCHAR* pSQLCommand);
	void ErrorDebug(const wchar_t* pLog, const wchar_t* pCommand = NULL, SQLHANDLE hHandle = NULL);
	SQLHANDLE GetSqlConnHandle(){ return m_hSQLConnHandle; }
private:
    BOOL ConnectToSQL();
    void DisconnectSQL();

    SQLHANDLE m_hSQLConnHandle;
    SQLHANDLE m_hSQLEnvHandle;

#ifdef _UNICODE					
	wchar_t	m_workingDir[_MAX_PATH];	// the working path
#else
	char	m_workingDir[_MAX_PATH];	// the working path
#endif

	CString m_strExternalConnectString; //for external sql
};

extern SQLControl *g_pSQLControl;
