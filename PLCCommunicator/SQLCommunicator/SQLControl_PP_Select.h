#pragma once
#include "SQLControl_ExternalBase.h"

class SQLControl_PP_Select :public  SQLControl_ExternalBase{
public:
	SQLControl_PP_Select();
	virtual ~SQLControl_PP_Select();
protected:
	virtual BOOL DoExecuteSelectSQLCommand(const wchar_t* pSQLCommand, std::vector<ColumnInfo> &vCol, std::vector<CString> &vResult);
};