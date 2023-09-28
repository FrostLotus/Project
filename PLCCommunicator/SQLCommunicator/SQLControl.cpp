
#include "stdafx.h"
#include "SQLControl.h"


const TCHAR* ctERROR = L"AOI EXECUTE SQL EXCEPTION";
#define SQL_RETURN_CODE_LEN 1024

void SQLControl::ErrorDebug(const wchar_t* pLog, const wchar_t* pCommand, SQLHANDLE hHandle)
{
    static SQLWCHAR state[ MAX_PATH ];
    static SQLWCHAR text[ MAX_PATH ];
    static CString  csLog;

    SQLINTEGER  native;
    SQLSMALLINT len;

    if ( pCommand ) csLog.Format( _T( "***Error  %s*** Command: %s" ), pLog, pCommand );
    else            csLog.Format( _T( "***Error  %s***"             ), pLog );

	wprintf(L"%s \n", pLog);
    InsertDebugLog( csLog, LOG_MSSQL );

    if ( hHandle && SQL_SUCCEEDED( SQLGetDiagRec( SQL_HANDLE_STMT, hHandle, 1, state, &native, text, sizeof( text ), &len ) ) )
    {
		wprintf(L"%s \n", text);
        InsertDebugLog( text, LOG_MSSQL );
		//if (native == WSAECONNABORTED || native == WSAECONNRESET){
		//	DisconnectSQL(); //free handle
		//}
    }
}

BOOL SQLControl::SQLExecute2(SQLHANDLE sqlConnectHandle, SQLHANDLE& sqlStmtHandle, const SQLWCHAR* pSQLCommand)
{
    SQLFreeHandle( SQL_HANDLE_STMT, sqlStmtHandle );

    sqlStmtHandle = NULL;
 
    if ( SQL_ERROR == SQLAllocHandle( SQL_HANDLE_STMT, sqlConnectHandle, &sqlStmtHandle ) )
    {
        ErrorDebug( _T( "SQLAllocHandle Fail" ) );

        return FALSE;
    }
	SQLRETURN rtn = SQLExecDirect(sqlStmtHandle, const_cast<SQLWCHAR*>(pSQLCommand), SQL_NTS);
    if ( SQL_ERROR ==  rtn)
    {
        ErrorDebug( _T( "SQLExecDirect Fail" ), pSQLCommand, sqlStmtHandle );

        return FALSE;
    }
	else if (SQL_SUCCESS_WITH_INFO == rtn)
	{
		static SQLWCHAR state[MAX_PATH];
		static SQLWCHAR text[MAX_PATH];
		SQLRETURN rtnMoreResults = SQL_ERROR;
		do
		{
			SQLINTEGER  native;
			SQLSMALLINT len;
			SQLSMALLINT  n = 1;

			SQLRETURN rtnInfo = SQL_ERROR;
			do
			{
				rtnInfo = SQLGetDiagRec(SQL_HANDLE_STMT, sqlStmtHandle, n++, state, &native, text, sizeof(text), &len);
				TRACE(L"%s \n", text);
				//SQLLEN nRow = 0;
				//SQLRETURN rtnRow = SQLRowCount(sqlStmtHandle, &nRow); //get all affected rows
				//TRACE(L"%d \n", nRow);
				if (CString(text).Find(ctERROR) != -1){
					//execute error
					return FALSE;
				}

			} while (rtnInfo == SQL_SUCCESS_WITH_INFO || rtnInfo == SQL_SUCCESS);

			rtnMoreResults = SQLMoreResults(sqlStmtHandle);
		} while (rtnMoreResults == SQL_SUCCESS_WITH_INFO || rtnMoreResults == SQL_SUCCESS);
	}
    return TRUE;
}

BOOL SQLControl::SQLExecute(SQLHANDLE sqlConnectHandle, const SQLWCHAR* pSQLCommand)
{
    SQLHANDLE sqlStmtHandle = NULL;
    BOOL      bRet          = SQLExecute2( sqlConnectHandle, sqlStmtHandle, pSQLCommand );
	if (!bRet){
		SQLFreeHandle(SQL_HANDLE_STMT, sqlStmtHandle);
		DisconnectSQL(); //free handle if error
	}
	else{
		SQLFreeHandle(SQL_HANDLE_STMT, sqlStmtHandle);
	}

    return bRet;
}

SQLControl::SQLControl() : m_hSQLConnHandle( NULL ),
                           m_hSQLEnvHandle( NULL )
{
#ifdef _UNICODE
	_wgetcwd(m_workingDir, _MAX_PATH);
#else
	_getcwd(m_workingDir, _MAX_PATH);
#endif
	StartLogServer();
}

SQLControl::~SQLControl()
{
    DisconnectSQL();
	StopLogServer();
}

BOOL SQLControl::ConnectToSQL()
{
    SQLWCHAR retconstring[ SQL_RETURN_CODE_LEN ] = { NULL };
    SQLWCHAR szConnectString[ MAX_PATH ] = { NULL };

	GetConnectString((wchar_t*)szConnectString);
	CString strLog;
	strLog.Format(L"connect string %s", szConnectString);
	AddLog(strLog);
    if ( SQL_SUCCESS != SQLAllocHandle( SQL_HANDLE_ENV, SQL_NULL_HANDLE, &m_hSQLEnvHandle ) ||
         SQL_SUCCESS != SQLSetEnvAttr( m_hSQLEnvHandle, SQL_ATTR_ODBC_VERSION, ( SQLPOINTER )SQL_OV_ODBC3, NULL ) ||
         SQL_SUCCESS != SQLAllocHandle( SQL_HANDLE_DBC, m_hSQLEnvHandle, &m_hSQLConnHandle ) )
    {
		AddLog(_T("Initial MSSQL Fail"));
    }
    switch ( SQLDriverConnect( m_hSQLConnHandle,
                               NULL,
                               szConnectString,
                               SQL_NTS,
                               retconstring,
                               SQL_RETURN_CODE_LEN,
                               NULL,
                               SQL_DRIVER_NOPROMPT ) )
    {
    case SQL_SUCCESS:
    case SQL_SUCCESS_WITH_INFO:
        {
			OnConnectSuccess();
        }
        break;
    case SQL_INVALID_HANDLE:
    case SQL_ERROR:
        {
            ErrorDebug( _T( "Connect To MS-SQL Fail" ) );

			AddLog(_T("Connect To MSSQL Fail"));
        }
        return FALSE;
    }
    return TRUE;
}

void SQLControl::DisconnectSQL()
{
    SQLDisconnect( m_hSQLConnHandle );

    SQLFreeHandle( SQL_HANDLE_DBC, m_hSQLConnHandle );
    SQLFreeHandle( SQL_HANDLE_ENV, m_hSQLEnvHandle  );

    m_hSQLConnHandle = NULL;
    m_hSQLEnvHandle  = NULL;
}

void SQLControl::AddLog(CString strLog)
{
	wprintf(L"%s \n", strLog);
	InsertDebugLog(strLog, LOG_MSSQL);
}
BOOL SQLControl::ExecuteSQLCommand(const wchar_t* pSQLCommand, BOOL bInsert, BOOL bDisConnect)
{
	if (!m_hSQLConnHandle){
		if (!ConnectToSQL())
			return FALSE;
	}

	BOOL bRet = DoExecuteSQLCommand(pSQLCommand, bInsert);

	if (bDisConnect)
		DisconnectSQL();

	if (!bRet)
		OnExecuteError(); //error handling

	if (RetryOnFail()){
		InsertDebugLog(L"execute failed! retry", LOG_MSSQL);
		return ExecuteSQLCommand(pSQLCommand, bInsert, bDisConnect);
	}
	else
		return bRet;
}
BOOL SQLControl::ExecuteSelectSQLCommand(const wchar_t* pSQLCommand, std::vector<ColumnInfo> &vCol, std::vector<CString> &vResult)
{
	if (!ConnectToSQL()) return FALSE;

	const BOOL bRet = DoExecuteSelectSQLCommand(pSQLCommand, vCol, vResult);

	DisconnectSQL();

	return bRet;
}