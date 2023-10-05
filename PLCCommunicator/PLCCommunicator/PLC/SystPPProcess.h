#pragma once
#include "PLCProcessBase.h"

class CSystPPProcess :public CPLCProcessBase
{
public:
	CSystPPProcess(); 
	virtual ~CSystPPProcess();
	virtual int GetFieldSize() { return FIELD_MAX; };//9

	enum PLC_FIELD_
	{
		FIELD_BEGIN = 0,
		FIELD_WATCHDOG = FIELD_BEGIN,
		FIELD_SWITCH_SHEET_WEB,
		FIELD_WS_POTENTIAL, //0:模式一/1:模式二
		FIELD_VERSION,
		FIELD_A_AXIS,
		FIELD_B_AXIS,
		FIELD_SHEET_NEWBATCH,
		FIELD_INSP_STOP,
		FIELD_INSP_START,
		FIELD_MAX
	};
	void DoPLCNewbatch(int nField);
	int GetNewbatchDelay(){ return m_xParam.nNewbatchDelay; }

	void DoWatchDogCheck();
protected:
	virtual CPU_SERIES GetCPU();
	virtual void SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData);
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);
	virtual void SET_INIT_PARAM(LPARAM lp, BYTE *pData);
	virtual void ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp);
	virtual long ON_OPEN_PLC(LPARAM lp);

	virtual void OnQueryTimer();
private:
	void Init();
	void Finalize();
	
	static void CALLBACK QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer);
	void ProcessTimer(UINT_PTR nEventId);
	void TriggerWatchDog(BOOL bOutput, BOOL bLog);
	void SetInspStatus(BOOL bSuspend, BOOL bClearAll);

private:
	static CSystPPProcess* m_this;
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
	enum 
	{
		//TIMER_WATCHDOG,			//watch dog
		TIMER_INPUT,
		TIMER_MAX
	};
	UINT_PTR m_tTimerEvent[TIMER_MAX];
	
	struct PP_PARAM
	{
		int nWatchDogTimeOut; //second. X秒沒有寫入FIELD_WATCHDOG就會輸出訊號
		int nVersion;//版本
		int nWSMode;
		BOOL bFX5U;
		int nNewbatchDelay;
	};
	PP_PARAM m_xParam;

	BOOL m_bExit;
};