#include <Psapi.h>
#include <Dbghelp.h>
#include <afxmt.h>
#pragma warning(disable : 4996)

CMutex		g_mtProtect_ExpHandler;

typedef struct APSTARTTIME : SYSTEMTIME
{
	APSTARTTIME()
	{
		GetLocalTime(this);
	}
}APSTARTTIME;

APSTARTTIME g_tStartAP;

typedef BOOL (CALLBACK*	LPEnumProcessModules)(HANDLE hProcess,HMODULE* lphModule,DWORD cb,LPDWORD lpcbNeeded);
typedef BOOL (CALLBACK*	LPGetModuleInformation)(HANDLE hProcess,HMODULE hModule,LPMODULEINFO lpmodinfo,DWORD cb);
typedef BOOL (WINAPI* LPMiniDumpWriteDump)(HANDLE hProcess,DWORD ProcessId,HANDLE hFile,MINIDUMP_TYPE DumpType,PMINIDUMP_EXCEPTION_INFORMATION ExceptionParam, PMINIDUMP_USER_STREAM_INFORMATION UserStreamParam,PMINIDUMP_CALLBACK_INFORMATION CallbackParam);


void WriteZergFunctionIni(int nEnWriteZerg,int nEnShowMsg)
{
#ifdef _UNICODE					
	wchar_t	szWorkingDir[_MAX_PATH];	
	_wgetcwd(szWorkingDir, _MAX_PATH);
#else
	char	szWorkingDir[_MAX_PATH];
	_getcwd(szWorkingDir, _MAX_PATH);
#endif

	CString strCurrentPath;
	CString strIniSettingPath;

	strCurrentPath = szWorkingDir;
	strIniSettingPath = strCurrentPath + _T("\\ZergLogPLC.ini") ;


	CString strSection;
	CString strEntry;
	CString strValue; 

	
	strSection = _T("General");

	strEntry.Format(_T("EnWriteZerg"));
	strValue.Format(_T("%d"),nEnWriteZerg);
	::WritePrivateProfileString(  strSection, strEntry  , strValue, strIniSettingPath );

	strEntry.Format(_T("EnShowMsg"));
	strValue.Format(_T("%d"),nEnShowMsg);
	::WritePrivateProfileString(  strSection, strEntry  , strValue, strIniSettingPath );
}

void ReadZergFunctionIni(int& nEnWriteZerg,int& nEnShowMsg)
{
#ifdef _UNICODE					
	wchar_t	szWorkingDir[_MAX_PATH];	
	_wgetcwd(szWorkingDir, _MAX_PATH);
#else
	char	szWorkingDir[_MAX_PATH];
	_getcwd(szWorkingDir, _MAX_PATH);
#endif

	CString strCurrentPath;
	CString strIniSettingPath;

	strCurrentPath = szWorkingDir;
	strIniSettingPath = strCurrentPath + _T("\\ZergLogEMC.ini") ;

	CString strSection,strEntry;
	CString strTmp;
	CString strDefault;
	TCHAR szValue[_MAX_PATH];
	
	strSection = _T("General");
	
	
	strEntry.Format(_T("EnWriteZerg"));
	strDefault.Format(_T("%d"),0);
	::GetPrivateProfileString( strSection, strEntry, strDefault, szValue, _MAX_PATH, strIniSettingPath ); 
	strTmp = szValue;
	nEnWriteZerg = (int) _ttol(strTmp);

	strEntry.Format(_T("EnShowMsg"));
	strDefault.Format(_T("%d"),0);
	::GetPrivateProfileString( strSection, strEntry, strDefault, szValue, _MAX_PATH, strIniSettingPath ); 
	strTmp = szValue;
	nEnShowMsg = (int) _ttol(strTmp);
}

BOOL GetExceptionModule(PVOID pExceptionAddress,LPSTR lpModule,LPVOID& lpLow, LPVOID& lpHigh)
{	
	DWORD i;
	//sizeof lpModule must > MAX_PATH
	HINSTANCE hInst = LoadLibraryA("psapi.dll");
	if (hInst == NULL)
		return FALSE;

	HMODULE *pModules = NULL;
	try{
		LPEnumProcessModules EnumProcessModulesFunc = (LPEnumProcessModules)GetProcAddress(hInst,"EnumProcessModules");
		if (EnumProcessModulesFunc == NULL)
			throw 0;

		DWORD dwRequired = 0;
		if (EnumProcessModulesFunc(GetCurrentProcess(),NULL,0,&dwRequired) == FALSE)
			throw 0;

		pModules = (HMODULE*)malloc(dwRequired);
		if (pModules == NULL)
			throw 0;

		if (EnumProcessModulesFunc(GetCurrentProcess(),pModules,dwRequired,&dwRequired) == FALSE)
			throw 0;

		LPGetModuleInformation GetModuleInformationFunc = (LPGetModuleInformation)GetProcAddress(hInst,"GetModuleInformation");
		if (GetModuleInformationFunc == NULL)
			throw 0;

		LPVOID lpAddr;
		MODULEINFO _ModuleInfo;
		dwRequired /= sizeof(HMODULE);
		for (i = 0; i < dwRequired; i++)
		{
			if (GetModuleInformationFunc(GetCurrentProcess(),pModules[i],&_ModuleInfo,sizeof(MODULEINFO)) == FALSE)
				throw 0;

			if (pExceptionAddress < _ModuleInfo.lpBaseOfDll)
				continue;

			lpAddr = (UCHAR*)_ModuleInfo.lpBaseOfDll + _ModuleInfo.SizeOfImage;
			if (pExceptionAddress < lpAddr)
			{
				if (GetModuleFileNameA(pModules[i],lpModule,MAX_PATH)==0)
					throw 0;

				//Found the right module
				lpLow  = _ModuleInfo.lpBaseOfDll;
				lpHigh = lpAddr;
				break;
			}
		}
		if (i == dwRequired)
			throw 0;
	}
	catch (...)
	{
		free(pModules);
		FreeLibrary(hInst);
		return FALSE;
	}
	free(pModules);
	FreeLibrary(hInst);
	return TRUE;
}

BOOL WriteMiniDump(PEXCEPTION_POINTERS pException,LPCSTR szPath,MINIDUMP_TYPE dType)
{
	HINSTANCE hInst = LoadLibraryA("DbgHelp.dll");
	if (hInst == NULL)
	{
		hInst = LoadLibraryA("DbgHelp_XP.dll");
		if (hInst == NULL)
			return FALSE;
	}
	LPMiniDumpWriteDump MiniDumpWriteDumpFunc = (LPMiniDumpWriteDump)GetProcAddress(hInst,"MiniDumpWriteDump");
	if (MiniDumpWriteDumpFunc == NULL)
	{
		FreeLibrary(hInst);
		return FALSE;
	}
	HANDLE hFile = CreateFileA(szPath,GENERIC_WRITE,0,NULL,CREATE_ALWAYS,FILE_ATTRIBUTE_NORMAL,NULL);
	if (hFile == INVALID_HANDLE_VALUE)
	{
		FreeLibrary(hInst);
		return FALSE;
	}
	MINIDUMP_EXCEPTION_INFORMATION eInfo;
	eInfo.ThreadId = GetCurrentThreadId(); 
	eInfo.ExceptionPointers = pException; 
	eInfo.ClientPointers = FALSE; 

	if (MiniDumpWriteDumpFunc(GetCurrentProcess(),GetCurrentProcessId(),hFile,dType,&eInfo,NULL,NULL))
	{
		FreeLibrary(hInst);
		CloseHandle(hFile);
		return TRUE;
	}
	else
	{
		FreeLibrary(hInst);
		CloseHandle(hFile); 
		remove(szPath);
		return FALSE;
	}
}

void WriteZergLog(  PEXCEPTION_POINTERS pException
					, LPCSTR szExePath
					, bool bShowMsg
					, bool bMiniDump = false
					, MINIDUMP_TYPE dType = MiniDumpScanMemory)
{
	
	g_mtProtect_ExpHandler.Lock();
	// ...
	char szTemp[MAX_PATH + 40];
	SYSTEMTIME tNow;
	GetLocalTime(&tNow);
	sprintf(szTemp,"%sZergLog\\",szExePath);
	CreateDirectoryA(szTemp,NULL);
	sprintf(szTemp + strlen(szTemp),"%04d%02d.log",tNow.wYear,tNow.wMonth);
	CStringA strOPFile = szTemp;
	CString strOPFileW ;
	strOPFileW =  strOPFile;

	CString strErrString;
	CString strErrStringTmp;

	strErrString = _T("");

	CFile cf;
	if (cf.Open(strOPFileW,CFile::modeWrite|CFile::modeCreate|CFile::modeNoTruncate))
	{
		cf.SeekToEnd();

		sprintf(szTemp,"DUMP(1.0.0.1)-%04d/%02d/%02d-%02d:%02d:%02d.%03d\r\n",
			tNow.wYear,tNow.wMonth,tNow.wDay,tNow.wHour,tNow.wMinute,tNow.wSecond,tNow.wMilliseconds);
		cf.Write(szTemp,(UINT)strlen(szTemp));
		strErrStringTmp = szTemp;
		strErrString = strErrString + strErrStringTmp;

		if (GetModuleFileNameA(NULL,szTemp + strlen("ModuleName:"),MAX_PATH))
		{
			strncpy(szTemp,"ModuleName:",strlen("ModuleName:"));
			strcat(szTemp,"\r\n");

			strErrStringTmp = szTemp;
			strErrString = strErrString + strErrStringTmp;
		}
		else
		{
			sprintf(szTemp,"ModuleName:%s%s.exe\r\n",szExePath,AfxGetApp()->m_pszExeName);
			strErrStringTmp = szTemp;
			strErrString = strErrString + strErrStringTmp;
		}

		cf.Write(szTemp,(UINT)strlen(szTemp));

		sprintf(szTemp,"StartTime-%04d/%02d/%02d-%02d:%02d:%02d\r\n",
			g_tStartAP.wYear,g_tStartAP.wMonth,g_tStartAP.wDay,g_tStartAP.wHour,g_tStartAP.wMinute,g_tStartAP.wSecond);
		cf.Write(szTemp,(UINT)strlen(szTemp));

		strErrStringTmp = szTemp;
		strErrString = strErrString + strErrStringTmp;

		LPVOID lpLow  = NULL;
		LPVOID lpHigh = NULL;
		PEXCEPTION_RECORD pExceptionRecord = pException->ExceptionRecord;
		if (GetExceptionModule(pExceptionRecord->ExceptionAddress,szTemp + strlen("ModuleLibrary:"),lpLow,lpHigh))
		{
			strncpy(szTemp,"ModuleLibrary:",strlen("ModuleLibrary:"));
			strcat(szTemp,"\r\n");
			cf.Write(szTemp,(UINT)strlen(szTemp));

			strErrStringTmp = szTemp;
			strErrString = strErrString + strErrStringTmp;

			szTemp[strlen(szTemp) - sizeof("\r\n")] = 0;
			CFileStatus fStatus;


			//.
			CString strStatus; //NEW
			strStatus = szTemp;  //NEW
			TCHAR szWW[MAX_PATH + 40];  //NEW
			_tcsncpy_s(szWW,strStatus,sizeof(szWW));  //NEW
			if (CFile::GetStatus(szWW + strlen("ModuleLibrary:"),fStatus))
			//if (CFile::GetStatus(szTemp + strlen("ModuleLibrary:"),fStatus))
			{	//Write last modified time
				sprintf(szTemp,"MapBuilt:%04d/%02d/%02d-%02d:%02d:%02d\r\n",
					fStatus.m_mtime.GetYear(),fStatus.m_mtime.GetMonth(),fStatus.m_mtime.GetDay(),
					fStatus.m_mtime.GetHour(),fStatus.m_mtime.GetMinute(),fStatus.m_mtime.GetSecond());
				cf.Write(szTemp,(UINT)strlen(szTemp));

				strErrStringTmp = szTemp;
				strErrString = strErrString + strErrStringTmp;
			}
			sprintf(szTemp,"Adress:0x%p-0x%p\r\n",lpLow,lpHigh);
			cf.Write(szTemp,(UINT)strlen(szTemp));

			strErrStringTmp = szTemp;
			strErrString = strErrString + strErrStringTmp;
		}
		else
		{
			cf.Write("ModuleLibrary:Unknown File\r\n",(UINT)strlen("ModuleLibrary:Unknown File\r\n"));
			sprintf(szTemp,"Adress:0x%p-0x%p\r\n",lpLow,lpHigh);
			cf.Write(szTemp,(UINT)strlen(szTemp));

			strErrStringTmp = szTemp;
			strErrString = strErrString + strErrStringTmp;
		}
		
		sprintf(szTemp,"ExceptionCode:0x%X\r\n",pExceptionRecord->ExceptionCode);
		cf.Write(szTemp,(UINT)strlen(szTemp));
		strErrStringTmp = szTemp;
		strErrString = strErrString + strErrStringTmp;

		sprintf(szTemp,"ExceptionFlags:0x%X\r\n",pExceptionRecord->ExceptionFlags);
		cf.Write(szTemp,(UINT)strlen(szTemp));
		strErrStringTmp = szTemp;
		strErrString = strErrString + strErrStringTmp;

		sprintf(szTemp,"ExceptionAddress:0x%p\r\n",pExceptionRecord->ExceptionAddress);
		cf.Write(szTemp,(UINT)strlen(szTemp));
		strErrStringTmp = szTemp;
		strErrString = strErrString + strErrStringTmp;

		sprintf(szTemp,"NumberParameters:0x%X\r\n",pExceptionRecord->NumberParameters);
		cf.Write(szTemp,(UINT)strlen(szTemp));
		strErrStringTmp = szTemp;
		strErrString = strErrString + strErrStringTmp;

		int nCnt = 1;
		pExceptionRecord = pExceptionRecord->ExceptionRecord;
		while (pExceptionRecord)
		{
			sprintf(szTemp,"nested (%d) exceptions-\r\n",nCnt++);
			cf.Write(szTemp,(UINT)strlen(szTemp));
			strErrStringTmp = szTemp;
			strErrString = strErrString + strErrStringTmp;

			sprintf(szTemp,"ExceptionCode:0x%X\r\n",pExceptionRecord->ExceptionCode);
			cf.Write(szTemp,(UINT)strlen(szTemp));
			strErrStringTmp = szTemp;
			strErrString = strErrString + strErrStringTmp;

			sprintf(szTemp,"ExceptionFlags:0x%X\r\n",pExceptionRecord->ExceptionFlags);
			cf.Write(szTemp,(UINT)strlen(szTemp));
			strErrStringTmp = szTemp;
			strErrString = strErrString + strErrStringTmp;

			sprintf(szTemp,"ExceptionAddress:0x%p\r\n",pExceptionRecord->ExceptionAddress);
			cf.Write(szTemp,(UINT)strlen(szTemp));
			strErrStringTmp = szTemp;
			strErrString = strErrString + strErrStringTmp;

			sprintf(szTemp,"NumberParameters:0x%X\r\n",pExceptionRecord->NumberParameters);
			cf.Write(szTemp,(UINT)strlen(szTemp));
			strErrStringTmp = szTemp;
			strErrString = strErrString + strErrStringTmp;

			pExceptionRecord = pExceptionRecord->ExceptionRecord;
		}
		cf.Write("\r\n",(UINT)strlen("\r\n"));
	}

	if ( bMiniDump )
	{
		sprintf(szTemp,"%sZergLog\\%04d%02d%02d-%02d%02d%02d.zergEMC",szExePath,
			tNow.wYear,tNow.wMonth,tNow.wDay,tNow.wHour,tNow.wMinute,tNow.wSecond);
		WriteMiniDump(pException,szTemp,dType);
	}

	// ...
	g_mtProtect_ExpHandler.Unlock();

	if(bShowMsg)
	{
		::AfxMessageBox(strErrString);
	}
}

void StartZergExcpFilter(LPTOP_LEVEL_EXCEPTION_FILTER lpTopLevelExceptionFilter)
{
	SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOALIGNMENTFAULTEXCEPT|SEM_NOGPFAULTERRORBOX);
	SetUnhandledExceptionFilter(lpTopLevelExceptionFilter);
}


