
// PLCCommunicator.h : PROJECT_NAME ���ε{�����D�n���Y��
//

#pragma once




#define WM_LOCAL_MSG	(WM_APP+100)
#define WM_EXIT			(WM_APP+101)


#ifndef __AFXWIN_H__
	#error "�� PCH �]�t���ɮ׫e���]�t 'stdafx.h'"
#endif

#include "resource.h"		// �D�n�Ÿ�
#include "AppLogProcess.h"

// CPLCCommunicatorApp: 
// �аѾ\��@�����O�� PLCCommunicator.cpp
//

class CPLCCommunicatorApp : public CWinApp, public AppLogProcess
{
public:
	CPLCCommunicatorApp();

// �мg
public:
	virtual BOOL InitInstance();

// �{���X��@

	DECLARE_MESSAGE_MAP()

private:
	BOOL CheckOneInstance();
	HANDLE m_hStartEvent;
};

extern CPLCCommunicatorApp theApp;

