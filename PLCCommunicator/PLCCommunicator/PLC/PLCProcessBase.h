#pragma once
#include "usm.h"
#include "MelSecIOController.h"

#define TIMER_INTERVAL 1000
#define QUERYSTATION_NAME	L"AOI_QueryStation(Server)"

enum PLC_VALUE_TYPE_{
	PLC_TYPE_STRING,
	PLC_TYPE_WORD,
	PLC_TYPE_FLOAT,
	PLC_TYPE_DWORD,
	PLC_TYPE_BIT,
};
enum PLC_ACTION_TYPE_{
	ACTION_SKIP,
	ACTION_NOTIFY,
	ACTION_BATCH,

	ACTION_RESULT,
};

typedef struct PLC_DATA_ITEM_{
public:
	PLC_DATA_ITEM_(){

	}
	PLC_DATA_ITEM_(CString strFieldName, int nFieldType, PLC_VALUE_TYPE_ eValueType, PLC_ACTION_TYPE_ eActionType, BYTE cLen, CString strDeviceType, UINT uAddress, UINT uStartBit = -1, UINT uEndBit = -1){
		memset(this->strFieldName, 0, sizeof(this->strFieldName));
		lstrcpy(this->strFieldName, strFieldName.GetBuffer());
		this->xFieldType = nFieldType;
		this->xValType = eValueType;
		this->xAction = eActionType;
		this->cLen = cLen;
		memset(this->cDevType, 0, sizeof(this->cDevType));
		lstrcpy(this->cDevType, strDeviceType.GetBuffer());
		this->uAddress = uAddress;
		this->uStartBit = uStartBit;
		this->uEndBit = uEndBit;
		this->bSigned = FALSE;
	}
	TCHAR strFieldName[100];
	int xFieldType;
	PLC_VALUE_TYPE_ xValType;
	PLC_ACTION_TYPE_ xAction;
	BYTE cLen; //bytes
	TCHAR cDevType[4];
	UINT uAddress;
	UINT uStartBit;
	UINT uEndBit;
	BOOL bSigned;
}PLC_DATA_ITEM;


class CPLCProcessBase : public CMelSecIOController{
public:
	CPLCProcessBase();
	virtual ~CPLCProcessBase();

	void NotifyAOI(WPARAM wp, LPARAM lp);

	virtual int GetFieldSize() = 0;
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip) = 0;

	virtual void DO_CUSTOM_TEST(){ } //change it in inherit class if needed
	virtual BOOL HAS_CUSTOM_TEST() { return FALSE; }

	BYTE *GET_PLC_FIELD_BYTE_VALUE(int nFieldId);
	CString GET_PLC_FIELD_VALUE(int nFieldId);
	CString GET_PLC_FIELD_TIME(int nFieldId);
	CString GET_PLC_FIELD_ADDRESS(int nFieldId);
	CString GET_PLC_FIELD_NAME(int nFieldId);
	PLC_ACTION_TYPE_ GET_PLC_FIELD_ACTION(int nFieldId);
	long GET_PLC_FIELD_DATA(int nFieldId);
	long GET_PLC_FIELD_DATA(vector<int> &vField);
	long SET_PLC_FIELD_DATA(int nFieldId, int nSizeInByte, BYTE *pData);
	long SET_PLC_FIELD_DATA(vector<int> &vField, BYTE *pData);
	long SET_PLC_FIELD_DATA_BIT(int nFieldStart, int nFieldEnd, int nSizeInByte, BYTE *pData);
	
	long SET_PLC_FIELD_DATA_BIT(int nField, int nBitPosition, BOOL bValue);

	void SET_FLUSH_ANYWAY(BOOL bFlushAnyway){ m_bFlushAnyway = bFlushAnyway; };
	BOOL GET_FLUSH_ANYWAY(){ return m_bFlushAnyway; };
protected:
	void INIT_PLCDATA();
	void DESTROY_PLC_DATA();

	BOOL USM_ReadData(BYTE *pData, int nSize, int nOffset = 0);
	BOOL USM_WriteData(BYTE *pData, int nSize, int nOffset = 0);

	//IPLCProcess
	virtual long ON_OPEN_PLC(LPARAM lp);

	virtual void SET_INIT_PARAM(LPARAM lp, BYTE *pData){};
private:
	void Init();
	void Finalize();
	long GET_PLC_FIELD_DATA(int nFieldId, void *pData);
	void GET_PLC_RANDOM_DATA(vector<int> &vField, CString &strField, int &nSizeInWord);
private:
	usm<unsigned char> *m_pAOIUsm;
	usm<unsigned char> *m_pPLCUsm;

	struct PLCDATA{
		BYTE * pData;
		__time64_t xTime;
	};
	PLCDATA *m_pPLCData;

	BOOL m_bFlushAnyway;
	long m_lLastRtn;
	HWND m_hAOIWnd;

	BYTE* m_pPLCInitData;
};