
// LK-GCommunicator.h : PROJECT_NAME ���ε{�����D�n���Y��
//

#pragma once

#ifndef __AFXWIN_H__
	#error "�� PCH �]�t���ɮ׫e���]�t 'stdafx.h'"
#endif

#include "resource.h"		// �D�n�Ÿ�
#include "AppLogProcess.h"

// CLKGCommunicatorApp: 
// �аѾ\��@�����O�� LK-GCommunicator.cpp
//

class CLKGCommunicatorApp : public CWinApp, public AppLogProcess
{
public:
	CLKGCommunicatorApp();

// �мg
public:
	virtual BOOL InitInstance();

// �{���X��@

	DECLARE_MESSAGE_MAP()
};

extern CLKGCommunicatorApp theApp;