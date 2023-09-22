#pragma once
#include "PLCProcessBase.h"

#define TECHAIN_NAME L"SENSOR_VIEWER"

enum PLC_TECHAIN_MESSAGE{
	PTM_VERSION_ERROR,
	//PTM_START,				//開始
	//PTM_END,				//結束
	PTM_VALVE_SWITCH1,		//閥(開關)1
	PTM_VALVE_SWITCH2,		//閥(開關)2
	PTM_ELE,				//電磁閥
	PTM_ANCHOR_ANALOG,		//定位輸出(類比)
	PTM_ANCHOR_DIGITAL,		//定位輸出(數位)
	PTM_VALVE_VALUE1,		//閥(數值)1
	PTM_VALVE_VALUE2,		//閥(數值)2
	PTM_PRESSURE,			//壓力
	PTM_TEMPERATURE,		//測量溫度
	PTM_DRIVE,				//驅動器壓力
	PTM_FEEDBACK,			//開度回饋
	PTM_VOC_1,				//VOC1
	PTM_VOC_2,				//VOC2
};
class CTechainProcess :public CPLCProcessBase{
public:
	CTechainProcess();
	virtual ~CTechainProcess();
	virtual int GetFieldSize() { return FIELD_MAX; };

	enum PLC_FIELD_{
		FIELD_BEGIN = 0,
		FIELD_VERSION = FIELD_BEGIN,
		FIELD_VALVE_SWITCH1,	//閥(開關)1
		FIELD_VALVE_SWITCH2,	//閥(開關)2
		FIELD_ANCHOR_DIGITAL,	//定位輸出(數位)
		FIELD_ELE,				//電磁閥
		FIELD_ANCHOR_ANALOG,	//定位輸出(類比)
		FIELD_VALVE_VALUE1,		//CH1:閥(數值)1
		FIELD_VALVE_VALUE2,		//CH2:閥(數值)2
		FIELD_PRESSURE,			//CH3:壓力
		FIELD_TEMPERATURE,		//CH4:測量溫度
		FIELD_DRIVE,			//CH5:驅動器壓力
		FIELD_FEEDBACK,			//CH6:開度回饋
		FIELD_VOC_1,			//CH7:VOC1
		FIELD_VOC_2,			//CH8:VOC2

		FIELD_MAX
	};
	virtual BOOL HAS_CUSTOM_TEST() { return TRUE; }
protected:
	virtual CPU_SERIES GetCPU(){ return CPU_SERIES::FX5U_SERIES; }	
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);
	virtual void SET_INIT_PARAM(LPARAM lp, BYTE *pData);
	virtual long ON_OPEN_PLC(LPARAM lp);
	virtual void ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp);
	//IPLCProcess
	virtual void SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData);

private:
	void Init();
	void Finalize();
	void NotifyMainProcess(WPARAM wp, LPARAM lp);
	static void CALLBACK QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer);
	void ProcessTimer(UINT_PTR nEventId);
private:
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
	HWND m_hMainProcessWnd;
	int m_nVersion;

	UINT_PTR m_tTimerEvent;
	static CTechainProcess* m_this;
};
