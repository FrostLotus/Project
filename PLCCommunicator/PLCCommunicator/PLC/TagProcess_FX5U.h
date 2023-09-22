#pragma once
#include "PLCProcessBase.h"
#include <mutex>

struct PLC_INPUT{
	int nPin0Base;
	DWORD dwPos;
};
enum OUTPUT_MODE{
	MODE_NOUSE = 0,
	MODE_DIRECTIO,
	MODE_TIME,
	MODE_DIST
};
enum OUTPUT_PIN_TYPE{
	TYPE_TAG,	//貼標機
	TYPE_STOP,	//捲狀停機
	TYPE_SPLIT,	//片狀剔除
};
struct OUTPUT_PIN_INFO{
	OUTPUT_PIN_TYPE eType;
	int nResponse;//反應時間(ms)
	int nDuration;//持續時間(ms)
	BOOL bHighPotenial; //0:Default Low/1:Default High
	OUTPUT_MODE eMode; //0:不輸出/1:直接輸出/2:時間模式/3:距離模式
};
struct ENCODER_POS{
	OUTPUT_PIN_TYPE eType;
	DWORD dwStartEncoder;
	DWORD dwEndEncoder;
};
struct INPUT_PIN_NOTIFY{
	int nPin0Base;
	DWORD dwEncoder;
};
class CTagProcess_FX5U :public CPLCProcessBase{
public:
	CTagProcess_FX5U();
	virtual ~CTagProcess_FX5U();

	virtual int GetFieldSize() { return FIELD_MAX; };

	enum PLC_FIELD_{
		FIELD_BEGIN = 0,
		FIELD_VERSION = FIELD_BEGIN,
		FIELD_WATCHDOG,	//watchdog輸出
		FIELD_STOPINSP,	//停止檢測輸出
		FIELD_ENCODER,
		FIELD_INPUT_FLAG,				//X點位觸發FLAG
		FIELD_Y06FLAG, //Y06:卷狀停機
		FIELD_Y07FLAG, //Y07:片狀剔除
		FIELD_Y11FLAG, //Y11:貼標機
		FIELD_INFO_Y06_RESPONSE_TIME,	//Y06 反應時間
		FIELD_INFO_Y06_DURATION,		//Y06 持續時間
		FIELD_INFO_Y06_START,			//Y06 Start Encoder
		FIELD_INFO_Y06_END,				//Y06 End Encoder
		FIELD_INFO_Y07_RESPONSE_TIME,	//Y07 反應時間
		FIELD_INFO_Y07_DURATION,		//Y07 持續時間
		FIELD_INFO_Y07_START,			//Y07 Start Encoder
		FIELD_INFO_Y07_END,				//Y07 End Encoder
		FIELD_INFO_Y11_RESPONSE_TIME,	//Y11 反應時間
		FIELD_INFO_Y11_DURATION,		//Y11 持續時間
		FIELD_INFO_Y11_START,			//Y11 Start Encoder
		FIELD_INFO_Y11_END,				//Y11 End Encoder
		FIELD_PARAM_Y06,				//Y06參數
		FIELD_PARAM_Y07,				//Y07參數
		FIELD_PARAM_Y11,				//Y11參數
		FIELD_ENCODER_X01,				//Encoder:A軸 (X001)
		FIELD_ENCODER_X02,				//Encoder:B軸 (X002)
		FIELD_ENCODER_X03,				//Encoder:拖白布 (X003)
		FIELD_ENCODER_X04,				//Encoder:裁片訊號 (X004)
		FIELD_ENCODER_X05,				//Encoder:片狀NewBatch (X005)
		FIELD_ENCODER_X11,				//Encoder:過布接頭 (X011)
		FIELD_MAX
	};
	enum OUTPUT_PIN_QUERY_FLAG{
		PIN_6,
		PIN_7,
		PIN_11,
		PIN_MAX
	};
protected:
	virtual long ON_OPEN_PLC(LPARAM lp);
	virtual void SET_INIT_PARAM(LPARAM lp, BYTE *pData);
	virtual CPU_SERIES GetCPU(){ return CPU_SERIES::FX5U_SERIES; }
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);
	//IPLCProcess
	virtual void SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData);
	virtual void ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp);

private:
	void Init();
	void Finalize();

	static void CALLBACK QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer);
	void ProcessTimer(UINT_PTR nEventId);
	void TriggerWatchDog(BOOL bOutput, BOOL bLog);

	void ProcessInputFlag(int nValue);

	void ProcessOutputPin(int nCount);
	int ModeToBit(OUTPUT_MODE eMode);
	void AssignEncoderPos();
	void CheckPLCField();
private:
	static CTagProcess_FX5U* m_this;
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
	enum {
		TIMER_WATCHDOG,			//watch dog
		TIMER_QUERY,
		TIMER_MAX
	};
	enum{
		NOTIFY_BIT_X1=1, //A軸
		NOTIFY_BIT_X2=2, //B軸
		NOTIFY_BIT_X3=3, //拖白布
		NOTIFY_BIT_X4=4, //裁片訊號
		NOTIFY_BIT_X5=5, //片狀NewBatch
		NOTIFY_BIT_X6=6, //暫停檢測
		NOTIFY_BIT_X11=9, //過布街頭
		NOTIFY_MAX
	};
	UINT_PTR m_tTimerEvent[TIMER_MAX];
	UINT m_uCurEncoder;

	struct TAG_PARAM{
		int nWatchDogTimeOut; //second. X秒沒有寫入FIELD_WATCHDOG就會輸出訊號
		int nVersion;//版本
	};
	TAG_PARAM m_xParam;
	DWORD m_dwLastEncoder;		//計算速度用
	vector<OUTPUT_PIN_INFO> m_vOutPin;

	vector<ENCODER_POS> m_vEncoderPos;
	std::mutex m_oMutex;

	WORD m_wOutputQueryFlag;
};
