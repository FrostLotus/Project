#include "stdafx.h"
#include "SQLControl_ExternalBase.h"

SQLControl_ExternalBase::SQLControl_ExternalBase()
{

}
SQLControl_ExternalBase::~SQLControl_ExternalBase()
{

}
void SQLControl_ExternalBase::GetConnectString(wchar_t* szConnectString)
{
	wcscpy(szConnectString, GetExternalConnectString());
}
BOOL SQLControl_ExternalBase::DoExecuteSQLCommand(const wchar_t* pSQLCommand, BOOL bInsert)
{
	return SQLExecute(GetSqlConnHandle(), pSQLCommand);
}