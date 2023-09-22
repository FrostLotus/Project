#pragma once
#include "SQLControl.h"

class SQLControl_Default :public SQLControl{
public:
	SQLControl_Default(int nCustomerType, int nSubCustomerType);
	virtual ~SQLControl_Default();
protected:
	virtual void GetConnectString(wchar_t* szConnectString);
	virtual void OnConnectSuccess();
	virtual BOOL DoExecuteSQLCommand(const wchar_t* pSQLCommand, BOOL bInsert);
	virtual BOOL DoExecuteSelectSQLCommand(const wchar_t* pSQLCommand, std::vector<ColumnInfo> &vCol, std::vector<CString> &vResult){ return FALSE; };
private:
	BOOL CreateDatabase();
	void UpgradeTable();
	void SetSQLMode(const DATABASE_MODE eDBMode);
private:
	int m_nCustomerType;
	int m_nSubCustomerType;
	DATABASE_MODE m_eDatabaseMode;
	CString       m_csDBName;

};