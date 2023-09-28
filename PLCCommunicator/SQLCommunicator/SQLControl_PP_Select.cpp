#include "stdafx.h"
#include "SQLControl_PP_Select.h"

SQLControl_PP_Select::SQLControl_PP_Select()
{

}
SQLControl_PP_Select::~SQLControl_PP_Select()
{

}
BOOL SQLControl_PP_Select::DoExecuteSelectSQLCommand(const wchar_t* pSQLCommand, std::vector<ColumnInfo> &vCol, std::vector<CString> &vResult)
{
	int nColSize = vCol.size();
	if (nColSize){
		SQLHANDLE sqlStmtHandle = NULL;
		if (SQLExecute2(GetSqlConnHandle(), sqlStmtHandle, pSQLCommand)){
			SQLLEN *pLen = new SQLLEN[vCol.size()];
			memset(pLen, 0, sizeof(SQLLEN)*nColSize);
			SQLRETURN retcode;
			int nCount = 0;
			for (auto &xCol : vCol){
				retcode = SQLBindCol(sqlStmtHandle, xCol.nColumnNumber + 1, xCol.nTargetType, xCol.pValue, xCol.nSizeInByte, (pLen + xCol.nColumnNumber));
			}
			retcode = SQLFetch(sqlStmtHandle);
			do{
				if (retcode == SQL_ERROR){
					if (nCount != 0){//select will fail on first result
						ErrorDebug(_T("DoExecuteSelectSQLCommand Fail"), pSQLCommand, sqlStmtHandle);
					}
				}
				else if (retcode == SQL_SUCCESS){
					CString strRow, strTemp;

					for (auto &xCol : vCol){
						if ((pLen + xCol.nColumnNumber)){
							switch (xCol.nTargetType){
							case SQL_CHAR:
							{
								int nLen = *((int*)(pLen + xCol.nColumnNumber));
								strTemp = CString((char*)(xCol.pValue), nLen);
							}
							break;
							case SQL_C_WCHAR:
							{
								int nLen = *((int*)(pLen + xCol.nColumnNumber));
								strTemp = CString((TCHAR*)(xCol.pValue), nLen);
							}
								break;
							case SQL_INTEGER:
								strTemp.Format(L"%d", *(int*)xCol.pValue);
								break;
							default:
								ASSERT(FALSE);
								break;
							}
						}
						else{
							strTemp = L"";
						}
						if (strRow.GetLength() == 0) strRow = strTemp;
						else strRow += L"," + strTemp;
					}
					vResult.push_back(strRow);
				}
				nCount++;
				retcode = SQLFetch(sqlStmtHandle);
			} while (retcode != SQL_NO_DATA);
		}
	}
	return TRUE;
}