#pragma once
#include "PLCProcessBase.h"
#include <mutex>

#ifdef OFF_LINE
#define USE_TEST_TIMER //timer固定發送測試資料
#endif
//rename later, not yet
class EverstrongProcess :public CPLCProcessBase
{
public:
	EverstrongProcess();
	virtual ~EverstrongProcess();
	virtual int GetFieldSize() { return FIELD_MAX; };//115

	enum PLC_FIELD_
	{
		//下發------------------------------------------------
		FIELD_BEGIN = 0,
		FIELD_ORDER = 1,			       //訂單號
		FIELD_SN,					       //批號
		FIELD_QUANTITY,				       //工單產品數量
		FIELD_SPLITNUM,				       //一開幾數
		FIELD_SPLIT_ONE_Y,			       //第一張大小板長
		FIELD_SPLIT_TWO_Y,			       //第二張大小板長
		FIELD_SPLIT_THREE_Y,		       //第三張大小板長
		FIELD_SPLIT_ONE_X,			       //第一張大小板寬
		FIELD_SPLIT_TWO_X,			       //第二張大小板寬
		FIELD_SPLIT_THREE_X,		       //第三張大小板寬
		FIELD_MATERIAL = 14,		       //訂單物料代碼
		FIELD_DIFF_ONE_Y_MIN,		       //第一個大小版經向公差下限
		FIELD_DIFF_ONE_Y_MAX,		       //第一個大小版經向公差上限
		FIELD_DIFF_ONE_X_MIN,		       //第一個大小版緯向公差下限
		FIELD_DIFF_ONE_X_MAX,		       //第一個大小版緯向公差上限
		FIELD_DIFF_ONE_XY_MIN,		       //第一個大小版對角線公差下限
		FIELD_DIFF_ONE_XY_MAX,		       //第一個大小版對角線公差上限

		FIELD_DIFF_TWO_Y_MIN,		       //第二個大小版經向公差下限
		FIELD_DIFF_TWO_Y_MAX,		       //第二個大小版經向公差上限
		FIELD_DIFF_TWO_X_MIN,		       //第二個大小版緯向公差下限
		FIELD_DIFF_TWO_X_MAX,		       //第二個大小版緯向公差上限
		FIELD_DIFF_TWO_XY_MIN,		       //第二個大小版對角線公差下限
		FIELD_DIFF_TWO_XY_MAX,		       //第二個大小版對角線公差上限

		FIELD_DIFF_THREE_Y_MIN,		       //第三個大小版經向公差下限
		FIELD_DIFF_THREE_Y_MAX,		       //第三個大小版經向公差上限
		FIELD_DIFF_THREE_X_MIN,		       //第三個大小版緯向公差下限
		FIELD_DIFF_THREE_X_MAX,		       //第三個大小版緯向公差上限
		FIELD_DIFF_THREE_XY_MIN,	       //第三個大小版對角線公差下限
		FIELD_DIFF_THREE_XY_MAX,	       //第三個大小版對角線公差上限
		//上傳---------------------------------------------
		FIELD_CCL_COMMAND = 34,		       //指令下發(1/0)
		FIELD_CCL_NO_C10,			       //板剪切編(開板後序號)
		FIELD_REAL_Y_ONE,			       //板實際長度1
		FIELD_REAL_Y_TWO,			       //板實際長度2
		FIELD_REAL_X_ONE,			       //板實際寬度1
		FIELD_REAL_X_TWO,			       //板實際寬度2

		FIELD_REAL_DIFF_ONE_Y,		       //板長度實際公差1
		FIELD_REAL_DIFF_TWO_Y,		       //板長度實際公差2
		FIELD_REAL_DIFF_ONE_X,		       //板寬度實際公差1
		FIELD_REAL_DIFF_TWO_X,		       //板寬度實際公差2
		FIELD_REAL_DIFF_ONE_XY,		       //板對角線實際公差1
		FIELD_REAL_DIFF_TWO_XY,		       //板對角線實際公差2

		FIELD_FRONT_LEVEL = 46,		       //表現正面判斷級別(1 = OK,2 = NG)
		FIELD_BACK_LEVEL = 48,		       //表現反面判斷級別(1 = OK,2 = NG)
		FIELD_SIZE_G10 = 50,		       //大小版尺寸判斷級別OK = 1,NG = 2 一開一 一個版
		FIELD_SIZE_G12,				       //大小版尺寸判斷級別OK = 1,NG = 2 一開二 兩個版
		FIELD_SIZE_G14,				       //大小版尺寸判斷級別OK = 1,NG = 2 一開三 三個板

		FIELD_CCD_MES_DATA,                //CCD接收 MES  資料完成
		FIELD_CCD_RESULT,			       //CCD發送檢測結果
		FIELD_CCD_RECEIVE,			       //CCD接收PLC接收檢測結果完成
		FIELD_RESULT_OK = 57,		       //版OK数量
		FIELD_RESULT_NG,			       //版NG数量
		FIELD_RESULT_QUALIFYRATE = 60,	   //訂單合格率
		FIELD_BATCH_MESSAGE = 62,		   //通知MES工單資訊
		FIELD_INSP_SETTING,				   //檢測設定
		FIELD_LIGHT_SETTING,			   //光源設定
		FIELD_START_TIME,				   //檢測開始時間
		FIELD_END_TIME,					   //檢測結束時間
		FIELD_ORDER_1,					   //訂單號
		FIELD_SN_1,						   //批號
		FIELD_MATERIAL_1,				   //訂單物料代碼
		FIELD_MAX
	};
	
protected:
	virtual long ON_OPEN_PLC(LPARAM lp);
	//IPLCProcess
	virtual void ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp);

	virtual void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL &xData) = 0;
	virtual void DoSetInfoField(BATCH_SHARE_SYST_INFO &xInfo) = 0;
	virtual BOOL IS_SUPPORT_FLOAT_REALSIZE(){ return TRUE; }; //東莞松八廠實際尺寸欄位型態為word, 非float

	virtual BOOL IS_SUPPORT_CUSTOM_ACTION() { return FALSE; } //是否支援客製化行為
	virtual void DoCustomAction(){}; //客製化行為
private:
	void Init();
	void Finalize();
	void ProcessAOIResponse(LPARAM lp);
	void ProcessResult();
	static void CALLBACK QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer);
	void ProcessTimer(UINT_PTR nEventId);

	void ON_CCL_NEWBATCH();
	void ON_C10_CHANGE(WORD wC10);

	
	void PushResult(BATCH_SHARE_SYST_RESULTCCL &xResult);
	void SetInfoField(BATCH_SHARE_SYST_INFO &xInfo);
private:

	enum {
		TIMER_COMMAND,			//指令下發
		TIMER_COMMAND_RECEIVED,	//指令收到
		TIMER_RESULT,			//檢驗結果
		TIMER_RESULT_RECEIVED,	//檢驗結果收到
		TIMER_C10,				//C10剪切小版編號
		TIMER_CUSTOM_ACTION,	//客製化行為
#ifdef USE_TEST_TIMER
		TIMER_TEST,
#endif
		TIMER_MAX
	};
	UINT_PTR m_tTimerEvent[TIMER_MAX];
	static EverstrongProcess* m_this;

	//write thread
	enum
	{
		EV_EXIT,
		EV_WRITE,
		EV_COUNT,
		CASE_EXIT = WAIT_OBJECT_0,
		CASE_WRITE,
	};
	vector<BATCH_SHARE_SYST_RESULTCCL> m_vResult;
	std::mutex m_oMutex;
	HANDLE     m_hThread;
	HANDLE     m_hEvent[EV_COUNT];

	static DWORD __stdcall Thread_Result(void* pvoid);
	void WriteResult(BATCH_SHARE_SYST_RESULTCCL &xData);
};
