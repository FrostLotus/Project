#include "stdafx.h"
#include "SQLControl_Default.h"
#include <list>
#include <vector>
#include "SQLCommunicator.h"
#include "DataHandlerBase.h"

SQLWCHAR* COMMAND_CREATE_PRODUCTION_TABLE = L"USE [Production];\
                                              CREATE TABLE[Production]([ProductionId] integer NOT NULL PRIMARY KEY,\
                                                                       [ProductName] varchar(30),\
                                                                       [ProductSerial] varchar(10),\
                                                                       [ProductCode] varchar(20),\
                                                                       [Station] varchar(10),\
                                                                       [ProductNum] integer DEFAULT 0,\
                                                                       [ProductInspNum] integer DEFAULT 0,\
                                                                       [ProductDefectNum] integer DEFAULT 0,\
                                                                       [StartTime] varchar(50),\
                                                                       [EndTime] varchar(50));";
SQLWCHAR* COMMAND_CREATE_PRODUCTION_TABLE2 = L"USE [Production_Board];\
                                               CREATE TABLE[Production]([ProductionId] integer NOT NULL PRIMARY KEY,\
                                                                        [ProductName] varchar(30),\
                                                                        [ProductSerial] varchar(10),\
                                                                        [ProductCode] varchar(20),\
                                                                        [Station] varchar(10),\
                                                                        [ProductNum] integer DEFAULT 0,\
                                                                        [ProductLength] integer DEFAULT 0,\
                                                                        [ProductInspNum] integer DEFAULT 0,\
                                                                        [ProductDefectNum] integer DEFAULT 0,\
                                                                        [StartTime] varchar(50),\
                                                                        [EndTime] varchar(50));";
SQLWCHAR* COMMAND_CREATE_DEFECT_TABLE      = L"USE [Production];\
                                               CREATE TABLE[Defect]([DefectId] integer NOT NULL PRIMARY KEY,\
	                                                                [ProductionId] integer,\
                                                                    [ProductIndex] integer,\
                                                                    [MatrixPos] integer,\
                                                                    [SideDir] integer,\
                                                                    [DefectType] varchar(10),\
                                                                    [DefectTime] datetime,\
	                                                                CONSTRAINT fk_Production FOREIGN KEY(ProductionId) REFERENCES Production(ProductionId) ON DELETE CASCADE ON UPDATE CASCADE);";
SQLWCHAR* COMMAND_CREATE_DEFECT_TABLE2     = L"USE [Production_Board];\
                                               CREATE TABLE[Defect]([DefectId] integer NOT NULL PRIMARY KEY,\
	                                                                [ProductionId] integer,\
                                                                    [ProductIndex] integer,\
                                                                    [DefectPosX] integer,\
                                                                    [DefectPosY] integer,\
                                                                    [SideDir] integer,\
                                                                    [DefectType] varchar(10),\
                                                                    [DefectTime] datetime,\
	                                                                CONSTRAINT fk_Production FOREIGN KEY(ProductionId) REFERENCES Production(ProductionId) ON DELETE CASCADE ON UPDATE CASCADE);";
SQLWCHAR* COMMAND_CREATE_DEFECT_TABLE_SYST_WEBCOOPER     = L"USE [Production_Board];\
															 CREATE TABLE[Defect]([DefectId] integer NOT NULL PRIMARY KEY,\
	                                                                [ProductionId] integer,\
                                                                    [ProductIndex] integer,\
                                                                    [DefectPosX] integer,\
                                                                    [DefectPosY] integer,\
                                                                    [SideDir] integer,\
																	[DefectArea] float,\
																	[Speed] integer,\
                                                                    [DefectType] varchar(10),\
                                                                    [DefectTime] datetime,\
	                                                                CONSTRAINT fk_Production FOREIGN KEY(ProductionId) REFERENCES Production(ProductionId) ON DELETE CASCADE ON UPDATE CASCADE);";


SQLControl_Default::SQLControl_Default(int nCustomerType, int nSubCustomerType)
{
#ifdef _DEBUG
	SetSQLMode(DB_WEB_BOARD);
#else
	SetSQLMode(DB_SHEET);
#endif
	m_nCustomerType = nCustomerType;
	m_nSubCustomerType = nSubCustomerType;
	switch (m_nCustomerType){
	case CUSTOMER_NANYA:
	case CUSTOMER_SYST_WEB_COPPER:
	case CUSTOMER_NANYA_WARPING:
	case CUSTOMER_SYST_PP:
	case CUSTOMER_SCRIBD_PP:
	case CUSTOMER_EMC_PP:
	case CUSTOMER_TUC_PP:
	default:
		SetSQLMode(DB_WEB_BOARD);
		break;
	case CUSTOMER_SYST_CCL:
	case CUSTOMER_JIANGXI_NANYA:
		SetSQLMode(DB_SHEET);
		break;
	}
}
SQLControl_Default::~SQLControl_Default()
{

}
void SQLControl_Default::GetConnectString(wchar_t* szConnectString)
{
#ifdef _DEBUG
	wcscpy_s(szConnectString, MAX_PATH, _T("DRIVER={SQL Server};SERVER=localhost;UID=sa;PWD=80689917;"));
	return;
#endif
	USES_CONVERSION;

	IP_ADAPTER_INFO* pAdptInfo = NULL;
	IP_ADAPTER_INFO* pNextAd = NULL;
	int              idx = NULL;
	ULONG            ulLen = NULL;
	char             szAddress[16] = { NULL };

	::GetAdaptersInfo(pAdptInfo, &ulLen);

	if (!ulLen)
	{
		AddLog(_T("Can't find Network-card"));

		return;
	}
	pAdptInfo = (IP_ADAPTER_INFO*)::new BYTE[ulLen];

	::GetAdaptersInfo(pAdptInfo, &ulLen);

	pNextAd = pAdptInfo;

	while (pNextAd)
	{
		sockaddr_in addr;

		addr.sin_addr.s_addr = inet_addr(pNextAd->IpAddressList.IpAddress.String);

		if (addr.sin_addr.S_un.S_un_b.s_b1 == 192 &&
			addr.sin_addr.S_un.S_un_b.s_b2 == 168 &&
			addr.sin_addr.S_un.S_un_b.s_b3 < 10 &&
			addr.sin_addr.S_un.S_un_b.s_b4 > 10 &&
			addr.sin_addr.S_un.S_un_b.s_b4 < 100)
		{
			strcpy(szAddress, pNextAd->IpAddressList.IpAddress.String);

			break;
		}
		pNextAd = pNextAd->Next;
	}
	delete (BYTE*)pAdptInfo;

	const size_t iIPLen = strlen(szAddress);

	if (iIPLen)
	{
		szAddress[iIPLen - 1] = '1';
	}
	wsprintf(szConnectString, _T("DRIVER={SQL Server};SERVER=%s,1433;UID=sa;PWD=80689917;"), CString(szAddress));

}
void SQLControl_Default::OnConnectSuccess()
{
	CString csCommand;
	SQLHANDLE sqlStmtHandle = NULL;

	csCommand.Format( _T( "SELECT name FROM master.dbo.sysdatabases WHERE name = '%s'" ), m_csDBName );

	SQLExecute2(GetSqlConnHandle(), sqlStmtHandle, csCommand);

	if ( SQLFetch( sqlStmtHandle ) != SQL_SUCCESS ) // if no database. try attach database
	{
		csCommand.Format( _T( "EXEC sp_attach_db @dbname = N'%s', @filename1=N'D:\\MS_SQL_DB\\%s.mdf', @filename2=N'D:\\MS_SQL_DB\\%s_log.ldf';" ), m_csDBName,
																																					m_csDBName,
																																					m_csDBName );
		if (!SQLExecute2(GetSqlConnHandle(), sqlStmtHandle, csCommand)) // if attach fail. create database & create table & create user
		{
			SHCreateDirectory( NULL, _T( "D:\\MS_SQL_DB" ) );

			if ( !CreateDatabase() )
			{
				ErrorDebug( _T( "Create SQL Database Fail" ) );
			}
		}                
	}
	SQLFreeHandle( SQL_HANDLE_STMT, sqlStmtHandle );

	csCommand.Format( _T( "USE [%s];\
							IF SUSER_ID('AOI_SHARE') IS NULL\
							BEGIN\
							EXEC sp_addlogin 'AOI_SHARE', '5436AOI4885', '%s';\
							EXEC sp_change_users_login 'Auto_Fix','AOI_SHARE',null,'5436AOI4885';\
							CREATE USER AOI_SHARE FROM LOGIN AOI_SHARE WITH DEFAULT_SCHEMA=%s;\
							EXEC sp_addrolemember db_datareader, AOI_SHARE;\
							END" ), m_csDBName, m_csDBName, m_csDBName );

	if (!SQLExecute(GetSqlConnHandle(), csCommand))
	{
		ErrorDebug( _T( "Create AOI_SHARE Account Fail" ) );
	}
	UpgradeTable();
}
BOOL SQLControl_Default::DoExecuteSQLCommand(const wchar_t* pSQLCommand, BOOL bInsert)
{
	auto GetProductionCount = [&]() -> SQLINTEGER {
		CString strQuery;
		strQuery.Format(L"SELECT COUNT(*) FROM [%s].[dbo].[Production]", m_csDBName);

		SQLHANDLE sqlStmtHandle = NULL;

		SQLExecute2(GetSqlConnHandle(), sqlStmtHandle, strQuery);

		SQLLEN nLen = 0;
		SQLINTEGER nCount = 0;
		SQLRETURN retcode = SQLBindCol(sqlStmtHandle, 1, SQL_C_LONG, &nCount, sizeof(SQLINTEGER), &nLen);
		retcode = SQLFetch(sqlStmtHandle);

		SQLINTEGER nRtn = 0;
		if (retcode == SQL_SUCCESS) {
			nRtn = nCount;
		}
		return nRtn;
	};

	BOOL bRet = FALSE;

	if (!bInsert) {
		bRet = SQLExecute(GetSqlConnHandle(), pSQLCommand);
	}
	else {
		SQLINTEGER nOld = 0, nNew = 0;
		nOld = GetProductionCount();

		bRet = SQLExecute(GetSqlConnHandle(), pSQLCommand);

		nNew = GetProductionCount();

		bRet = nOld != nNew; //bulk insert無法以狀態判斷是否成功, 改成以production筆數判斷
	}

	return bRet;
}
BOOL SQLControl_Default::CreateDatabase()
{
	std::list< CString > lstCommand;

	SHCreateDirectory(NULL, _T("D:\\MS_SQL_DB"));

	lstCommand.push_back(CString());
	lstCommand.rbegin()->Format(_T("EXEC sp_configure 'contained', 1; RECONFIGURE; CREATE DATABASE %s containment = partial;"), m_csDBName);

	switch (m_eDatabaseMode) // create table
	{
	case DB_SHEET:
	{
		lstCommand.push_back(COMMAND_CREATE_PRODUCTION_TABLE);
		lstCommand.push_back(COMMAND_CREATE_DEFECT_TABLE);
	}
	break;
	case DB_WEB_BOARD:
	{
		lstCommand.push_back(COMMAND_CREATE_PRODUCTION_TABLE2);
		if (m_nCustomerType == CUSTOMER_SYST_WEB_COPPER)
			lstCommand.push_back(COMMAND_CREATE_DEFECT_TABLE_SYST_WEBCOOPER);
		else
			lstCommand.push_back(COMMAND_CREATE_DEFECT_TABLE2);
	}
	break;
	}
	for (auto& i : lstCommand)
	{
		if (!SQLExecute(GetSqlConnHandle(), i))
		{
			return FALSE;
		}
	}
	return TRUE;
}
void SQLControl_Default::UpgradeTable()
{
	switch (m_eDatabaseMode)
	{
	case DB_SHEET:
	{
		const std::vector< CString > vecUpgradeColumn{ _T("IF COL_LENGTH('Defect', 'DefectTime') IS NULL BEGIN ALTER TABLE Defect ADD [DefectTime] datetime; END") };

		for (auto& i : vecUpgradeColumn)
		{
			SQLExecute(GetSqlConnHandle(), i);
		}
	}
	break;
#ifdef MSSQL_BULKINSERT
	case DB_WEB_BOARD:
	{
		CFile     xDbUpdateFile;
		CString   csFilePath, csCommand;
		SQLHANDLE sqlStmtHandle = NULL;

		csCommand.Format(_T("SELECT ParamValue FROM [Production_Board].[dbo].[Param] WHERE ParamName = 'Version'"));

		SQLExecute2(GetSqlConnHandle(), sqlStmtHandle, csCommand);

#define VERSION_LEN 50+1/*null terminated*/
		SQLLEN nLen = 0;
		SQLCHAR szVersion[VERSION_LEN];
		memset(szVersion, 0, sizeof(szVersion));
		SQLRETURN retcode = SQLBindCol(sqlStmtHandle, 1, SQL_C_CHAR, szVersion, VERSION_LEN, &nLen);
		retcode = SQLFetch(sqlStmtHandle);

		int nCurrentVersion = 1, nCurrentSubVersion = 0;
		if (retcode == SQL_SUCCESS) {
			CString strVersion(szVersion);
			std::vector<CString> vStr = GetEachStringBySep(strVersion, _T('_'));
			if (vStr.size() >= 1)
				nCurrentVersion = _ttoi(vStr[0]);//get from Param table
			if (vStr.size() >= 2)
				nCurrentSubVersion = _ttoi(vStr[1]);
		}
#define SQL_VERSION 1
		//version format: X_XX  (大版號(共同更新)_小板號(各customer_type各自更新)).   依據customer_type和subcustomer_type找對應update script
		BYTE* pData = NULL;
		ULONGLONG uBufferSize = NULL;

		for (int i = nCurrentVersion; i <= SQL_VERSION; i++) {
			for (int j = 0; j <= 99; j++) {
				if (retcode == SQL_SUCCESS && i == nCurrentVersion && (j == 0 || j <= nCurrentSubVersion)) {
					continue;
				}
				if (j == 0) {
					csFilePath.Format(L"%s\\UPDATE_MSSQL\\%d.sql", GetWorPath(), i);
				}
				else {
					csFilePath.Format(L"%s\\UPDATE_MSSQL\\%d_%02d_%d_%d.sql", GetWorPath(), i, j, m_nCustomerType, m_nSubCustomerType);
				}

				ULONGLONG uLen = NULL;

				if (xDbUpdateFile.Open(csFilePath, CFile::modeRead, NULL))
				{
					uLen = xDbUpdateFile.GetLength();

					if (uLen > uBufferSize)
					{
						if (pData) delete[] pData;
						uBufferSize = uLen + 2;

						pData = new BYTE[uBufferSize];
					}
					if (pData) {
						memset(pData, 0, uBufferSize);

						xDbUpdateFile.Read(pData, uLen);
						xDbUpdateFile.Close();

						pData[uLen] = NULL;
						pData[uLen + 1] = NULL;
						SQLHANDLE sqlStmtHandle = NULL;
						BOOL bResult = SQLExecute2(GetSqlConnHandle(), sqlStmtHandle, (wchar_t*)&pData[2]);

						CString strLog;
						if (bResult) {
							CString strUpdate;
							if (j == 0) {
								strUpdate.Format(L"UPDATE [Production_Board].[dbo].[Param] SET ParamValue='%d' WHERE ParamName='Version' ", i);
							}
							else {
								strUpdate.Format(L"UPDATE [Production_Board].[dbo].[Param] SET ParamValue='%d_%02d' WHERE ParamName='Version' ", i, j);
							}
							SQLExecute2(GetSqlConnHandle(), sqlStmtHandle, strUpdate);
							strLog.Format(L"execute %s success", csFilePath);
						}
						else {
							strLog.Format(L"execute %s fail", csFilePath);
						}
						wprintf(L"%s \n", strLog);
						InsertDebugLog(strLog, LOG_MSSQL);
						SQLFreeHandle(SQL_HANDLE_STMT, sqlStmtHandle);

						if (!bResult) //must not continue
							break;
					}
				}
				else
					break;
			}
		}
		if (pData)
			delete[]pData;
	}
	break;
#endif
	}
}
void SQLControl_Default::SetSQLMode(const DATABASE_MODE eDBMode)
{
	m_eDatabaseMode = eDBMode;

	switch (eDBMode)
	{
	case DB_SHEET:     m_csDBName = _T("Production"); break;
	case DB_WEB_BOARD: m_csDBName = _T("Production_Board"); break;
	}
}