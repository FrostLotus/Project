#pragma once
#include "SQLControl_ExternalBase.h"
class SQLControl_PP_WARPING :
	public SQLControl_ExternalBase
{
public:
	SQLControl_PP_WARPING();
	~SQLControl_PP_WARPING();
protected:
	//override
	virtual void OnExecuteError();
	virtual BOOL RetryOnFail(){ return m_nExecuteFail == 1; }  //only retry once
private:
	BOOL m_nExecuteFail;
};

