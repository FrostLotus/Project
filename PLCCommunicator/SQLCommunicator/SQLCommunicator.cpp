// SQLCommunicator.cpp : 定義主控台應用程式的進入點。
//

#include "stdafx.h"
#include "SQLCommunicator.h"
#include <map>
#include "SQLControl.h"
#include "SQLControl_Default.h"
#include "SQLControl_PP_Select.h"
#include "SQLControl_ExternalBase.h"
#include "SQLControl_PP_WARPING.h"
#include "usm.h"
#include "DataHandlerBase.h"
#include "SQLCmdMonitor.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// 僅有的一個應用程式物件

CWinApp theApp;
SQLControl *g_pSQLControl = NULL;

using namespace std;

vector<CString> GetEachStringBySep(CString sStr, TCHAR tSep)
{
	vector<CString> vEachStr;
	TCHAR *token = NULL;
	TCHAR *pNextToken = sStr.GetBuffer();
	TCHAR xSep[2] = { NULL, NULL };
	xSep[0] = tSep;
	do{
		while (pNextToken && *pNextToken && (*pNextToken == tSep)){
			vEachStr.push_back(CString(_T('\0')));
			pNextToken++;
		}
		token = _tcstok_s(NULL, xSep, &pNextToken);
		if (token){
			CString xNew(token);
			if (xNew != _T("\r\n")){
				vEachStr.push_back(xNew);
			}
		}
	} while (token != NULL);
	return vEachStr;
}
enum ParamType{
	CustomerType,
	SubCustomerType,
	ExternalConnectString,
	FilePath,
	ExecuteCmd,
	SelectCmd,
	MonitorCmd,
	ParamMax
};
std::map<ParamType, CString> ctParam= {
	{ CustomerType			, L"/C:" },
	{ SubCustomerType		, L"/S:" },
	{ ExternalConnectString	, L"/E:" },
	{ FilePath				, L"/F:" },
	{ ExecuteCmd			, L"/EC:" },
	{ SelectCmd				, L"/SC:" },
	{ MonitorCmd			, L"/M:" },
};
std::map<ParamType, CString> ParseCmdLine(CString strCmdLine)
{
	std::map<ParamType, CString> mapRtn;
	for (int i = 0; i < ParamMax; i++){
		if (ctParam.find((ParamType)i) != ctParam.end()){
			CString strKey = ctParam[(ParamType)i];
			int nPos = strCmdLine.Find(strKey);
			if (nPos != -1){
			int nStart = nPos + strKey.GetLength();
			int nEnd = strCmdLine.GetLength();
				for (int j = nStart; j < strCmdLine.GetLength(); j++){
					if (strCmdLine.Mid(j, 1) == L"/"){
						nEnd = j;
						break;
					}
				}
				mapRtn[(ParamType)i] = strCmdLine.Mid(nStart, nEnd - nStart);
			}
		}
	}
	return mapRtn;
}
BYTE* GetFileContent(CString &strPath)
{
	BYTE*     pData = NULL;
	ULONGLONG uLen = NULL;
	CFile     xDbUpdateFile;

	if (xDbUpdateFile.Open(strPath, CFile::modeRead, NULL))
	{
		if (uLen < xDbUpdateFile.GetLength())
		{
			if (pData) delete[] pData;

			pData = new BYTE[xDbUpdateFile.GetLength() + 2];
		}
		uLen = xDbUpdateFile.GetLength();

		xDbUpdateFile.Read(pData, uLen);
		xDbUpdateFile.Close();

		pData[uLen] = NULL;
		pData[uLen + 1] = NULL;
	}
	return pData;
}
vector<ColumnInfo> GetColumnInfo(CString strPath)
{
	vector<ColumnInfo> vRtn;
	WCHAR szData[MAX_PATH] = { NULL };
	int nCount = ::GetPrivateProfileInt(L"COLUMN", L"COUNT", 0, strPath);
	for (int i = 0; i < nCount; i++){
		CString strSection;
		ColumnInfo xInfo;
		xInfo.nColumnNumber = i;
		strSection.Format(L"COLUMN_%d", i);
		::GetPrivateProfileString(strSection, L"NAME", L"", szData, MAX_PATH, strPath);
		xInfo.strName = szData;
		xInfo.nTargetType = ::GetPrivateProfileInt(strSection, L"TARGET_TYPE", 0, strPath);
		xInfo.nSizeInByte = ::GetPrivateProfileInt(strSection, L"SIZEINBYTE", 0, strPath);
		if (xInfo.nSizeInByte){
			xInfo.pValue = new BYTE[xInfo.nSizeInByte];
			memset(xInfo.pValue, 0, xInfo.nSizeInByte);
		}
		vRtn.push_back(xInfo);
	}
	return vRtn;
}
void ProcessData()
{
	auto GetValueFromParam = [](CString &str)->CString{
		CString strRtn;
		vector<CString> vTemp = GetEachStringBySep(str, L':');
		if (vTemp.size() >= 2){
			strRtn = vTemp.at(1);
		}
		return strRtn;
	};
	CString strCmdLine = ::GetCommandLine();
#ifdef _DEBUG
	strCmdLine += L" /C:10 /S:2 /E:DRIVER={SQL Server};SERVER=localhost;UID=sa;PWD=80689917;Database=Production_JIANGXI_NANYA; /SC:D:\\AOI_Code\\AOI_NEWUI_20171205\\master\\AOI\\WEB_PRODUCTION\\JIANGXI_NANYA_CCDTA1.sql,D:\\AOI_Code\\AOI_NEWUI_20171205\\master\\AOI\\WEB_PRODUCTION\\JIANGXI_NANYA_CCDTA1.INI";
	//strCmdLine += L" /C:11 /S:0 /M:1";
	//strCmdLine += L" /C:7 /S:0 /F:D:\\AOI_Code\\AOI_NEWUI_PP_20191121\\master\\AOI\\WEB_PRODUCTION\\a15.sql";
	//strCmdLine += L" /C:6 /S:2 /E:DRIVER={SQL Server};SERVER=localhost;UID=sa;PWD=80689917;Database=SCADA; /F:D:\\AOI_Code\\AOI_NEWUI_PP_20191121\\master\\AOI\\WEB_PRODUCTION\\000800081088V4J1C15015.sql";
#endif
	std::map<ParamType, CString> mapParam = ParseCmdLine(strCmdLine);
	int nCustomerType = 0, nSubCustomerType = 0;
	if (mapParam.find(CustomerType) != mapParam.end() && mapParam.find(SubCustomerType) != mapParam.end()){
		nCustomerType = _ttoi(mapParam[CustomerType]);
		nSubCustomerType = _ttoi(mapParam[SubCustomerType]);
		switch (nCustomerType){
		case CUSTOMER_NANYA_WARPING:
			g_pSQLControl = new SQLControl_PP_WARPING;
			break;
		case CUSTOMER_YINGHUA:
		case CUSTOMER_TUC_PP:
			g_pSQLControl = new SQLControl_PP_Select;
			break;
		case CUSTOMER_SYST_PP:
			if (nSubCustomerType == SUB_CUSTOMER_JIUJIANG)
				g_pSQLControl = new SQLControl_ExternalBase;
			break;
		case CUSTOMER_JIANGXI_NANYA:
			if (nSubCustomerType == SUB_CUSTOMER_NANYA_N5)
				g_pSQLControl = new SQLControl_PP_Select;
			break;
		}
	}
	else{
		wprintf(L"Error!CmdLine does not contains CustomerType and SubCustomrType:%s \n", strCmdLine);
	}
	if (g_pSQLControl == NULL) g_pSQLControl = new SQLControl_Default(nCustomerType, nSubCustomerType); //if it is not specical customer type, use default 
	if (g_pSQLControl){
		g_pSQLControl->AddLog(strCmdLine);

		for (auto &i : mapParam){
			switch (i.first){
			case ExternalConnectString:
				g_pSQLControl->SetExternalConnectString(i.second);
				break;
			case ExecuteCmd:
				g_pSQLControl->ExecuteSQLCommand(i.second);
				break;
			case SelectCmd:
			{
				vector<CString> vStr = GetEachStringBySep(i.second, L',');
				if (vStr.size() >= 2){
					BYTE* pData = GetFileContent(vStr.at(0));
					vector<ColumnInfo> vCol = GetColumnInfo(vStr.at(1));
					if (pData && g_pSQLControl){
						vector<CString> vResult;
						if (g_pSQLControl->ExecuteSelectSQLCommand((wchar_t*)&pData[2], vCol, vResult) && vResult.size()>0){
							CString strAll = vStr.at(0);
							for (auto &str : vResult){
								if (strAll.GetLength() == 0)
									strAll = str;
								else
									strAll += L"\n" + str;
							}
							//copy to shared memory
							usm<unsigned char> xShareMem(BATCH_SQL2AOI_MEM_ID, TRUE);
							if (xShareMem.WriteData((const BYTE*)strAll.GetBuffer(), strAll.GetLength() * sizeof(TCHAR))){
								//post message
								HWND hWnd = ::FindWindow(NULL, L"AOI Master");
								if (hWnd){
									::PostMessage(hWnd, WM_GPIO_MSG, WM_SQL_CMD, strAll.GetLength()* sizeof(TCHAR));
								}
							}
						}

						for (auto &xCol : vCol){
							if (xCol.pValue) delete[]xCol.pValue;
						}
						delete[]pData;
					}

				}
			}
				break;
			case FilePath:
			{
				CString strValue = i.second;
				CString strLog;
				strLog.Format(L"Before execute sql:%s", strValue);
				g_pSQLControl->AddLog(strLog);

				BYTE* pData = GetFileContent(i.second);
				if (pData){
					CString csWaitToSQLPath;
					CString csWaitToSQLFilePath;
					csWaitToSQLPath.Format(_T("%s\\WaitToSQL"), g_pSQLControl->GetWorPath());

					::SHCreateDirectoryEx(NULL, csWaitToSQLPath, NULL);

					if (!g_pSQLControl->ExecuteSQLCommand((wchar_t*)&pData[2], TRUE)){
						g_pSQLControl->AddLog(L"execute fail!");
						csWaitToSQLFilePath.Format(_T("%s\\%s"), csWaitToSQLPath, CTime::GetTickCount().Format("%Y%m%d%H%M%S.cmd"));

						::CopyFile(strValue, csWaitToSQLFilePath, FALSE);
					}
					else{
#ifdef MSSQL_BULKINSERT
						CString strTxt(strValue);
						CString strLog;
						strLog.Format(L"After execute sql:%s", strTxt);
						g_pSQLControl->AddLog(strLog);
						strTxt.Replace(L".sql", L".txt");
						if (PathFileExists(strTxt)){
#ifndef _DEBUG
							CFile::Remove(strTxt);
#endif
						}
#endif
					}
					delete[] pData;

					if (PathFileExists(strValue)){
#ifndef _DEBUG
						CFile::Remove(strValue);
#endif
					}
				}
			}
			break;
			case MonitorCmd:
				if (i.second == L"1"){
					CSQLCmdMonitor xMonitor;
					while (true){
						if (xMonitor.IsReceiveCmd()){
							//execute cmd
							CString strCmd;
							xMonitor.GetReceiceCmd(strCmd);
							if (g_pSQLControl){
								g_pSQLControl->AddLog(L"Receive Cmd: " + strCmd);
								g_pSQLControl->ExecuteSQLCommand(strCmd.GetBuffer(), strCmd.Find(L"INSERT") != -1, FALSE);
							}
						}
						::Sleep(1);
					}
				}
				break;
			}
		}
	}

	if (g_pSQLControl) {
		delete g_pSQLControl;
		g_pSQLControl = NULL;
	}
}
int _tmain(int argc, TCHAR* argv[], TCHAR* envp[])
{
	int nRetCode = 0;

	HMODULE hModule = ::GetModuleHandle(NULL);

	if (hModule != NULL)
	{
		// 初始化 MFC 並於失敗時列印錯誤
		if (!AfxWinInit(hModule, NULL, ::GetCommandLine(), 0))
		{
			// TODO:  配合您的需要變更錯誤碼
			_tprintf(_T("嚴重錯誤:  MFC 初始化失敗\n"));
			nRetCode = 1;
		}
		else
		{
			// TODO:  在此撰寫應用程式行為的程式碼。
			ProcessData();
		}
	}
	else
	{
		// TODO:  配合您的需要變更錯誤碼
		_tprintf(_T("嚴重錯誤:  GetModuleHandle 失敗\n"));
		nRetCode = 1;
	}

	return nRetCode;
}
