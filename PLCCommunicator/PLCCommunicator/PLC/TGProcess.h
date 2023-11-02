#pragma once
#include "SystPPProcess.h"


class CTGProcess : public CSystPPProcess{
public:
	CTGProcess();
	virtual ~CTGProcess();
	virtual int GetFieldSize();

	enum PLC_FIELD_
	{
		FIELD_BEGIN = 0,
		FIELD_REVERSE_DIST = FIELD_BEGIN,
		FIELD_REVERSE_BEGIN,
		FIELD_REVERSE_END,
		FIELD_CURRENT_DIR,
		FIELD_LAST_DIR,
		FIELD_BEGIN_REVERSE_POS,
		FIELD_CURRENT_POS,
		FIELD_MAX
	};
protected:
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);
private:
	void Init();
	void Finalize();
	virtual void OnQueryTimer();
	CString GET_PLC_FIELD_VALUE(int nFieldId);
	long GET_PLC_FIELD_DATA(vector<int> vField);
	long SET_PLC_FIELD_DATA(int nFieldId, int nSizeInByte, BYTE *pData);
private:
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
};