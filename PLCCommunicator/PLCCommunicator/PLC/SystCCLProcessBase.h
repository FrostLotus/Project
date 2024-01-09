#pragma once
#include "PLCProcessBase.h"
#include <mutex>

#ifdef OFF_LINE
#define USE_TEST_TIMER //timer固定發送測試資料
#endif
//rename later, not yet
class CSystCCLProcessBase :public CPLCProcessBase
{
public:
	CSystCCLProcessBase();
	virtual ~CSystCCLProcessBase();
	virtual int GetFieldSize() { return FIELD_MAX; };//115

	enum PLC_FIELD_
	{
		FIELD_ORDER = 0,			//工單
		FIELD_MATERIAL,				//料號
		FIELD_MODEL,				//模號
		FIELD_ASSIGN,				//分發號
		FIELD_ASSIGNNUM,			//分發號數量
		FIELD_SPLITNUM,				//一開幾數
		FIELD_SPLIT_ONE_Y,			//第一張大小板長
		FIELD_SPLIT_TWO_Y,			//第二張大小板長
		FIELD_SPLIT_THREE_Y,		//第三張大小板長

		FIELD_SPLIT_ONE_X,			//第一張大小板寬
		FIELD_SPLIT_TWO_X,			//第二張大小板寬
		FIELD_SPLIT_THREE_X,		//第三張大小板寬
		FIELD_THICK_SIZE,			//板材厚度
		FIELD_THICK_CCL,			//銅箔厚度
		FIELD_CCL_TYPE,				//銅箔特性
		FIELD_CCL_GRAYSCALE,		//灰度值
		FIELD_LEVEL_AA_PIXEL,		//點值要求(AA版點值)
		FIELD_LEVEL_A_PIXEL,		//點值要求(A版點值)
		FIELD_LEVEL_P_PIXEL,		//點值要求(P版點值)
		FIELD_DIFF_ONE_Y,			//第一個大小板公差(長)
		FIELD_DIFF_ONE_X,			//第一個大小板公差(寬)
		FIELD_DIFF_ONE_XY,			//第一個大小板對角線公差

		FIELD_DIFF_ONE_Y_MIN,		//第一個大小版經向公差下限
		FIELD_DIFF_ONE_Y_MAX,		//第一個大小版經向公差上限
		FIELD_DIFF_ONE_X_MIN,		//第一個大小版緯向公差下限
		FIELD_DIFF_ONE_X_MAX,		//第一個大小版緯向公差上限
		FIELD_DIFF_ONE_XY_MIN,		//第一個大小版對角線公差下限
		FIELD_DIFF_ONE_XY_MAX,		//第一個大小版對角線公差上限

		FIELD_DIFF_TWO_Y_MIN,		//第二個大小版經向公差下限
		FIELD_DIFF_TWO_Y_MAX,		//第二個大小版經向公差上限
		FIELD_DIFF_TWO_X_MIN,		//第二個大小版緯向公差下限
		FIELD_DIFF_TWO_X_MAX,		//第二個大小版緯向公差上限
		FIELD_DIFF_TWO_XY_MIN,		//第二個大小版對角線公差下限
		FIELD_DIFF_TWO_XY_MAX,		//第二個大小版對角線公差上限

		FIELD_DIFF_THREE_Y_MIN,		//第三個大小版經向公差下限
		FIELD_DIFF_THREE_Y_MAX,		//第三個大小版經向公差上限
		FIELD_DIFF_THREE_X_MIN,		//第三個大小版緯向公差下限
		FIELD_DIFF_THREE_X_MAX,		//第三個大小版緯向公差上限
		FIELD_DIFF_THREE_XY_MIN,	//第三個大小版對角線公差下限
		FIELD_DIFF_THREE_XY_MAX,	//第三個大小版對角線公差上限

		FIELD_AA_NUM,				//小版AA級數量
		FIELD_CCL_COMMAND,			//指令下發

		FIELD_CCL_NO_C06,			//C06小板剪切編號(版材序號)
		FIELD_CCL_NO_C10,			//C10小板剪切編號(開版後序號)
		FIELD_CCL_NO_C12,			//C12小版剪切編號

		FIELD_REAL_Y_ONE,			//小板實際長度1
		FIELD_REAL_Y_TWO,			//小板實際長度2
		FIELD_REAL_X_ONE,			//小板實際寬度1
		FIELD_REAL_X_TWO,			//小板實際寬度2
		FIELD_REAL_DIFF_ONE_Y,		//小板長度實際公差1
		FIELD_REAL_DIFF_TWO_Y,		//小板長度實際公差2
		FIELD_REAL_DIFF_ONE_X,		//小板寬度實際公差1
		FIELD_REAL_DIFF_TWO_X,		//小板寬度實際公差2
		FIELD_REAL_DIFF_ONE_XY,		//小板對角線實際公差1
		FIELD_REAL_DIFF_TWO_XY,		//小板對角線實際公差2

		FIELD_FRONT_LEVEL,			//正面判斷級別(1=AA/2=A/3=P)
		FIELD_FRONT_CODE,			//正面判斷代碼(G12)
		FIELD_FRONT_LOCATION,		//正面缺陷九宮格位置
		FIELD_BACK_LEVEL,			//反面判斷級別(1=AA/2=A/3=P)
		FIELD_BACK_CODE,			//反面判斷代碼(G12)
		FIELD_BACK_LOCATION,		//反面缺陷九宮格位置
		FIELD_SIZE_G10,				//尺寸判斷級別(G10)
		FIELD_SIZE_G12,				//尺寸判斷級別(G12)
		FIELD_SIZE_G14,				//尺寸判斷級別(G14)
		FIELD_SIZE_INFO_1,			//尺寸檢測準備好
		FIELD_SIZE_INFO_2,			//尺寸檢測運行
		FIELD_CCD_INFO_1,			//CCD準備好
		FIELD_CCD_INFO_2,			//CCD運行
		FIELD_CCD_ERROR_1,			//CCD故障
		FIELD_SIZE_ERROR_1,			//尺寸故障

		FIELD_COMMAND_RECEIVED,		//指令收到
		FIELD_RESULT,				//檢驗結果
		FIELD_RESULT_RECEIVED,		//檢驗結果收到

		FIELD_RESULT_LEVEL,			//小版物料級別
		FIELD_RESULT_AA,			//小版AA級數量
		FIELD_RESULT_A,				//小版A級數量
		FIELD_RESULT_P,				//小版P級數量
		FIELD_RESULT_QUALIFYRATE,	//訂單合格率
		FIELD_RESULT_DIFF_XY,		//級差實際檢測值
		FIELD_RESULT_MES,			//通知MES

		FIELD_BATCH_MES,		    //通知MES工單資訊
		FIELD_INSP_SETTING,		    //檢測設定
		FIELD_LIGHT_SETTING,	    //光源設定
		FIELD_START_TIME,		    //檢測開始時間
		FIELD_END_TIME,			    //檢測結束時間
		FIELD_FRONT_DEFECT_SIZE_1,	//九宮格中正面前五大缺陷大小1
		FIELD_FRONT_DEFECT_SIZE_2,	//九宮格中正面前五大缺陷大小2
		FIELD_FRONT_DEFECT_SIZE_3,	//九宮格中正面前五大缺陷大小3
		FIELD_FRONT_DEFECT_SIZE_4,	//九宮格中正面前五大缺陷大小4
		FIELD_FRONT_DEFECT_SIZE_5,	//九宮格中正面前五大缺陷大小5
		FIELD_BACK_DEFECT_SIZE_1,	//九宮格中反面前五大缺陷大小1
		FIELD_BACK_DEFECT_SIZE_2,	//九宮格中反面前五大缺陷大小2
		FIELD_BACK_DEFECT_SIZE_3,	//九宮格中反面前五大缺陷大小3
		FIELD_BACK_DEFECT_SIZE_4,	//九宮格中反面前五大缺陷大小4
		FIELD_BACK_DEFECT_SIZE_5,	//九宮格中反面前五大缺陷大小5
		FIELD_FRONT_DEFECT_LOCATION_1,	//九宮格中正面前五大缺陷位置1
		FIELD_FRONT_DEFECT_LOCATION_2,	//九宮格中正面前五大缺陷位置2
		FIELD_FRONT_DEFECT_LOCATION_3,	//九宮格中正面前五大缺陷位置3
		FIELD_FRONT_DEFECT_LOCATION_4,	//九宮格中正面前五大缺陷位置4
		FIELD_FRONT_DEFECT_LOCATION_5,	//九宮格中正面前五大缺陷位置5
		FIELD_BACK_DEFECT_LOCATION_1,	//九宮格中反面前五大缺陷位置1
		FIELD_BACK_DEFECT_LOCATION_2,	//九宮格中反面前五大缺陷位置2
		FIELD_BACK_DEFECT_LOCATION_3,	//九宮格中反面前五大缺陷位置3
		FIELD_BACK_DEFECT_LOCATION_4,	//九宮格中反面前五大缺陷位置4
		FIELD_BACK_DEFECT_LOCATION_5,	//九宮格中反面前五大缺陷位置5

		FIELD_CUTTER_ORDER,				//F4->CCD(尺寸)掃碼訂單號
		FIELD_CUTTER_ASSIGN,			//F4->CCD(尺寸)掃碼分發號
		FIELD_CUTTER_Y,					//F4->CCD(尺寸)長度
		FIELD_CUTTER_X,					//F4->CCD(尺寸)寬度
		FIELD_CUTTER_INDEX,				//F4->CCD(尺寸)版編號

		FIELD_CUTTER_RETURN_ORDER,		//CCD->測厚掃碼訂單號
		FIELD_CUTTER_RETURN_ASSIGN,		//CCD->測厚掃碼分發號
		FIELD_CUTTER_RETURN_Y,			//CCD->測厚長度
		FIELD_CUTTER_RETURN_X,			//CCD->測厚寬度
		FIELD_CUTTER_RETURN_INDEX,		//CCD->測厚版編號

		FIELD_MAX                       //FIELD總數
	};
	
protected:
	virtual long ON_OPEN_PLC(LPARAM lp);
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

	enum 
	{
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
	static CSystCCLProcessBase* m_this;

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
