#pragma once
#include "PLCProcessBase.h"

#define SET_STOPPOS				101
#define SET_STOPPOS_SUCCESS		102
#define SET_STOPPOS_FAIL		-1
#define REACH_STOPPOS_FORWARD	201
#define REACH_STOPPOS_BACKWARD	202
#define RECEIVE_STOPPOS			203
#define RESET_ALL				301
#define RESET_ALL_SUCCESS		302
#define MAX_PUTPUT_PIN			16
#define STOP_PIN				0	//停機訊號腳位

//========	msg, this should be same in QueryStation(PP) =========================
#define WM_QUERYSTATION_RESPONSE	(WM_APP + 201)
#define WM_PLC_ERROR				(WM_APP + 202)//寫入/讀取PLCaddress錯誤

enum PLC_NOTIFY{
	PLC_OPEN,					//open plc
	PLC_VERSION,				//取得版本號
	PLC_RESET,					//reset current pos/stop pos wp: reset flag
	PLC_PULSELENGTH,			//unit:100ms
	PLC_STOPPOS_FORWARD,		//下發stop pos(正) wp: set forward stop pos, lp:stop pos(long)
	PLC_STOPPOS_BACKWARD,		//下發stop pos(反) wp: set backward stop pos, lp:stop pos(long)
	PLC_TEST_PIN,				//測試輸出pin  ex: pin 0/2 high, others low=> 5
		
	PLC_REACH_STOPPOS_FORWARD,	//停機訊號觸發
	PLC_REACH_STOPPOS_BACKWARD,	//停機訊號觸發
	PLC_CHANGE_POS,				//位置變動
	PLC_CURRENT_POS,			//for base pos, 取得目前位置 wp: query current pos
	PLC_DELAY_TIME,

	//info
	PLC_SPEED_FORWARD,
	PLC_SPEED_BACKWARD,
	//error
	PLC_STOPPOS_FORWARD_FIELDERROR,
	PLC_STOPPOS_FORWARD_FLAGERROR,
	PLC_STOPPOS_BACKWARD_FIELDERROR,
	PLC_STOPPOS_BACKWARD_FLAGERROR,
};

//========	msg	end	=========================

class CScribdPPProcess :public CPLCProcessBase{
public:
	CScribdPPProcess();
	virtual ~CScribdPPProcess();

	virtual int GetFieldSize(){ return FIELD_MAX; };
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);
	enum PLC_SCRIBD_CCL_FIELD_{
		FIELD_VERSION = 0,					//版本號
		FIELD_PULSE_LENGTH,					//Pulse Length.單位(100ms)
		FIELD_DELAY_TIME,					//反應時間(ms)
		FIELD_CURRENT_POS,					//目前位置
		FIELD_PLC_STOPPOS_DELAY_FORWARD,	//停機位置-反應距離(正)
		FIELD_PLC_STOPPOS_DELAY_BACKWARD,	//停機位置+反應距離(反)
		FIELD_PLC_STOPPOS_FORWARD,			//PLC使用停機位置(正)
		FIELD_PLC_STOPPOS_BACKWARD,			//PLC使用停機位置(反)
		FIELD_STOPPOS_FORWARD,				//下發使用停機位置(正)
		FIELD_STOPPOS_BACKWARD,				//下發使用停機位置(反)
		FIELD_FLAG_STOPPOS_FORWARD,			//停機位置(正)下發flag
		FIELD_RESULTFLAG_STOPPOS_FORWARD,	//停機位置(正)回傳flag
		FIELD_FLAG_STOPPOS_BACKWARD,		//停機位置(反)下發flag
		FIELD_RESULTFLAG_STOPPOS_BACKWARD,	//停機位置(反)回傳flag
		FIELD_FLAG_STOPPOS,					//停機位置到達flag
		FIELD_RESULTFLAG_STOPPOS,			//停機位置到達回傳flag
		FIELD_FLAG_RESET,					//reset flag
		FIELD_RESULTFLAG_RESET,				//reset 回傳flag
		FIELD_OUTPUT_PIN,					//輸出腳位
		FIELD_Y0_TEST,						//Y0測試開關
		FIELD_MAX
	};
protected:
	virtual CPU_SERIES GetCPU(){ return CPU_SERIES::FX5U_SERIES; }
	//IPLCProcess
	virtual long ON_OPEN_PLC(LPARAM lp);
	virtual void ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp);
	virtual void SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData);
private:
	void Init();
	void Finalize();
	static void CALLBACK QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer);
	void ProcessTimer(UINT_PTR nEventId);
	void NotifyAOI(UINT uMsg, WPARAM wp, LPARAM lp);
private:
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
	enum {
		TIMER_CURRENT_POS,			//目前位置
		TIMER_SET_STOPPOS_FORWARD,	//設定stop pos(正)回傳flag
		TIMER_SET_STOPPOS_BACKWARD, //設定stop pos(反)回傳flag
		TIMER_STOPPOS,				//停機回傳flag
		TIMER_RESET,				//reset回傳flag
		TIMER_CHECK,				//下發欄位確認
		TIMER_MAX
	};
	static CScribdPPProcess* m_this;
	UINT_PTR m_tTimerEvent[TIMER_MAX];
	HWND m_hAOIWnd;
};