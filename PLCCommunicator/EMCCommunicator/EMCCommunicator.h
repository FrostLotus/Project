
// EMCCommunicator.h : PROJECT_NAME ���ε{�����D�n���Y��
//

#pragma once

#ifndef __AFXWIN_H__
	#error "�� PCH �]�t���ɮ׫e���]�t 'stdafx.h'"
#endif

#include "resource.h"		// �D�n�Ÿ�
#include "AppLogProcess.h"

// CEMCCommunicatorApp: 
// �аѾ\��@�����O�� EMCCommunicator.cpp
//

class CEMCCommunicatorApp : public CWinApp, public AppLogProcess
{
public:
	CEMCCommunicatorApp();

// �мg
public:
	virtual BOOL InitInstance();

// �{���X��@

	DECLARE_MESSAGE_MAP()
};

extern CEMCCommunicatorApp theApp;