
// OPCCommunicator.h : PROJECT_NAME ���ε{�����D�n���Y��
//

#pragma once

#ifndef __AFXWIN_H__
	#error "�� PCH �]�t���ɮ׫e���]�t 'stdafx.h'"
#endif

#include "resource.h"		// �D�n�Ÿ�
#include "AppLogProcess.h"

// COPCCommunicatorApp: 
// �аѾ\��@�����O�� OPCCommunicator.cpp
//
class COPCCommunicatorApp : public CWinApp, public AppLogProcess
{
public:
	COPCCommunicatorApp();

// �мg
public:
	virtual BOOL InitInstance();

// �{���X��@

	DECLARE_MESSAGE_MAP()
};

extern COPCCommunicatorApp theApp;