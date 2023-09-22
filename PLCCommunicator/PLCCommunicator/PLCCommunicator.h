
// PLCCommunicator.h : PROJECT_NAME 應用程式的主要標頭檔
//

#pragma once




#define WM_LOCAL_MSG	(WM_APP+100)
#define WM_EXIT			(WM_APP+101)


#ifndef __AFXWIN_H__
	#error "對 PCH 包含此檔案前先包含 'stdafx.h'"
#endif

#include "resource.h"		// 主要符號
#include "AppLogProcess.h"

// CPLCCommunicatorApp: 
// 請參閱實作此類別的 PLCCommunicator.cpp
//

class CPLCCommunicatorApp : public CWinApp, public AppLogProcess
{
public:
	CPLCCommunicatorApp();

// 覆寫
public:
	virtual BOOL InitInstance();

// 程式碼實作

	DECLARE_MESSAGE_MAP()

private:
	BOOL CheckOneInstance();
	HANDLE m_hStartEvent;
};

extern CPLCCommunicatorApp theApp;

