#include "stdafx.h"
#include "SQLControl_PP_WARPING.h"


SQLControl_PP_WARPING::SQLControl_PP_WARPING()
{
	m_nExecuteFail = 0;
}


SQLControl_PP_WARPING::~SQLControl_PP_WARPING()
{
}
void SQLControl_PP_WARPING::OnExecuteError()
{
	m_nExecuteFail++;

	if (RetryOnFail()){
		CString strLog;
		strLog.Format(L"Execute sql fail, retry once");
		AddLog(strLog);
	}
}