#pragma once
#include "SQLControl.h"

class SQLControl_ExternalBase :public  SQLControl{
public:
	SQLControl_ExternalBase();
	virtual ~SQLControl_ExternalBase();
protected:
	virtual void GetConnectString(wchar_t* szConnectString);
	virtual void OnConnectSuccess(){};
	virtual BOOL DoExecuteSQLCommand(const wchar_t* pSQLCommand, BOOL bInsert);
	virtual BOOL DoExecuteSelectSQLCommand(const wchar_t* pSQLCommand, std::vector<ColumnInfo> &vCol, std::vector<CString> &vResult){ return FALSE; };
};