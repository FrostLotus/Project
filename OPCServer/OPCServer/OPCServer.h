
// OPCServer.h : PROJECT_NAME ���ε{�����D�n���Y��
//

#pragma once

#ifndef __AFXWIN_H__
	#error "�� PCH �]�t���ɮ׫e���]�t 'stdafx.h'"
#endif

#include "resource.h"		// �D�n�Ÿ�


// COPCServerApp: 
// �аѾ\��@�����O�� OPCServer.cpp
//

class COPCServerApp : public CWinApp
{
public:
	COPCServerApp();

// �мg
public:
	virtual BOOL InitInstance();

// �{���X��@

	DECLARE_MESSAGE_MAP()
};

extern COPCServerApp theApp;