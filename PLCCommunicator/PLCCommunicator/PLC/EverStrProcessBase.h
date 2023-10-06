#pragma once
#include "PLCProcessBase.h"
#include <mutex>

#ifdef OFF_LINE
#define USE_TEST_TIMER //timer固定發送測試資料
#endif
//rename later, not yet
class CEverStrProcessBase :public CPLCProcessBase
{
public:
	CEverStrProcessBase();
	virtual ~CEverStrProcessBase();
	virtual int GetFieldSize() { return FIELD_MAX; };//70
	enum PLC_FIELD_
	{
		//下發------------------------------------------------
		FIELD_BEGIN = 0,

		FIELD_ORDER = 1,			       //訂單號					             1      D1000~D1009		  string[20]    
		FIELD_SN,					       //批號					             2      D1010~D1019		  string[20]     =  FIELD_ASSIGN 分發號
		FIELD_QUANTITY,				       //工單產品數量				             3      D1020			  word    =2     =  FIELD_ASSIGNNUM 分發號數量

		FIELD_SPLITNUM,				       //一開幾數					             4      D1021			  word    =2          
		FIELD_SPLIT_ONE_Y,			       //第一張大小板長			             5      D1022~D1023		  real    =4          
		FIELD_SPLIT_TWO_Y,			       //第二張大小板長			             6      D1024~D1025		  real    =4          
		FIELD_SPLIT_THREE_Y,		       //第三張大小板長			             7      D1026~D1027		  real    =4          
		FIELD_SPLIT_ONE_X,			       //第一張大小板寬			             8      D1028~D1028		  real    =4          
		FIELD_SPLIT_TWO_X,			       //第二張大小板寬			             9      D1030~D1031		  real    =4          
		FIELD_SPLIT_THREE_X,		       //第三張大小板寬			            10      D1032~D1033		  real    =4          

		FIELD_MATERIAL = 14,		       //訂單物料代碼				            14      D1037~D1046		  string[20]    

		FIELD_DIFF_ONE_Y_MIN,		       //第一個大小版經向公差下限				15		D1047			  word          
		FIELD_DIFF_ONE_Y_MAX,		       //第一個大小版經向公差上限				16		D1048			  word          
		FIELD_DIFF_ONE_X_MIN,		       //第一個大小版緯向公差下限				17		D1049			  word          
		FIELD_DIFF_ONE_X_MAX,		       //第一個大小版緯向公差上限				18		D1050			  word          
		FIELD_DIFF_ONE_XY_MIN,		       //第一個大小版對角線公差下限				19		D1051			  word          
		FIELD_DIFF_ONE_XY_MAX,		       //第一個大小版對角線公差上限				20		D1052			  word          

		FIELD_DIFF_TWO_Y_MIN,		       //第二個大小版經向公差下限				21		D1053			  word          
		FIELD_DIFF_TWO_Y_MAX,		       //第二個大小版經向公差上限				22		D1054			  word          
		FIELD_DIFF_TWO_X_MIN,		       //第二個大小版緯向公差下限				23		D1055			  word          
		FIELD_DIFF_TWO_X_MAX,		       //第二個大小版緯向公差上限				24		D1056			  word          
		FIELD_DIFF_TWO_XY_MIN,		       //第二個大小版對角線公差下限				25		D1057			  word          
		FIELD_DIFF_TWO_XY_MAX,		       //第二個大小版對角線公差上限				26		D1058			  word          

		FIELD_DIFF_THREE_Y_MIN,		       //第三個大小版經向公差下限				27		D1059			  word          
		FIELD_DIFF_THREE_Y_MAX,		       //第三個大小版經向公差上限				28		D1060			  word          
		FIELD_DIFF_THREE_X_MIN,		       //第三個大小版緯向公差下限				29		D1061			  word          
		FIELD_DIFF_THREE_X_MAX,		       //第三個大小版緯向公差上限				30		D1062			  word          
		FIELD_DIFF_THREE_XY_MIN,	       //第三個大小版對角線公差下限				31		D1063			  word          
		FIELD_DIFF_THREE_XY_MAX,	       //第三個大小版對角線公差上限				32		D1064			  word          
		//上傳---------------------------------------------------													           
		FIELD_CCL_COMMAND = 34,		       //指令下發(1/0)						34		 D1066			  word          
		FIELD_CCL_NO_C10,			       //板剪切編(開板後序號)					35		 D1067			  word          

		FIELD_REAL_Y_ONE,			       //板實際長度1							36		 D1300~D1301	  real          
		FIELD_REAL_Y_TWO,			       //板實際長度2							37		 D1302~D1303	  real          
		FIELD_REAL_X_ONE,			       //板實際寬度1							38		 D1304~D1305	  real          
		FIELD_REAL_X_TWO,			       //板實際寬度2							39		 D1306~D1307	  real   

		FIELD_REAL_DIFF_ONE_Y,		       //板長度實際公差1						40		 D1308~D1309	  real          
		FIELD_REAL_DIFF_TWO_Y,		       //板長度實際公差2						41		 D1310~D1311	  real          
		FIELD_REAL_DIFF_ONE_X,		       //板寬度實際公差1						42		 D1312~D1313	  real          
		FIELD_REAL_DIFF_TWO_X,		       //板寬度實際公差2						43		 D1314~D1315	  real          
		FIELD_REAL_DIFF_ONE_XY,		       //板對角線實際公差1						44		 D1316~D1317	  real          
		FIELD_REAL_DIFF_TWO_XY,		       //板對角線實際公差2						45		 D1318~D1319	  real          

		FIELD_FRONT_LEVEL = 46,		       //表現正面判斷級別(1 = OK,2 = NG)       46    	 D1320			  word          
		FIELD_BACK_LEVEL = 48,		       //表現反面判斷級別(1 = OK,2 = NG)		48	     D1336			  word          
		FIELD_SIZE_G10 = 50,		       //版尺寸判斷OK = 1,NG = 2 一開一 一個版	50       D1352			  word          
		FIELD_SIZE_G12,				       //版尺寸判斷OK = 1,NG = 2 一開二 兩個版	51       D1353			  word          
		FIELD_SIZE_G14,				       //版尺寸判斷OK = 1,NG = 2 一開三 三個板	52       D1354			  word          

		FIELD_CCD_COMMAND_RECEIVED,        //CCD接收 MES  資料完成  [指令收到]		53		 D1355			  word          																											            
		FIELD_CCD_RESULT,			       //CCD發送檢測結果						54		 D1356			  word          
		FIELD_CCD_RESULT_RECEIVED,		   //CCD接收PLC接收檢測結果完成				55		 D1357			  word          

		FIELD_RESULT_OKNum = 57,		   //版OK数量							57		 D1359			  word           =FIELD_RESULT_A
		FIELD_RESULT_NGNum,			       //版NG数量							58		 D1360			  word           =FIELD_RESULT_P
		FIELD_RESULT_QUALIFYRATE = 60,	   //訂單合格率							60		 D1362~D1363	  real          

		FIELD_BATCH_MES = 62,		       //通知MES工單資訊						62		 D1366			  word          
		FIELD_INSP_SETTING,				   //檢測設定								63		 D1367~D1371	  string[10]    
		FIELD_LIGHT_SETTING,			   //光源設定								64		 D1372~D1376	  string[10]    
		FIELD_START_TIME,				   //檢測開始時間							65		 D1377~D1385	  string[18]    
		FIELD_END_TIME,					   //檢測結束時間							66		 D1387~D1395	  string[18]  

		FIELD_ORDER_1,					   //訂單號								67		 D2000~D2009	  string[20]    
		FIELD_SN_1,						   //批號								68		 D2010~D2019	  string[20]    
		FIELD_MATERIAL_1,				   //訂單物料代碼							69		 D2037~D2046	  string[20]    

		FIELD_MAX
	};
protected:

	virtual long ON_OPEN_PLC(LPARAM lp);
	virtual void ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp);

	virtual void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL& xData) = 0;
	virtual void DoSetInfoField(BATCH_SHARE_SYST_INFO& xInfo) = 0;
	virtual BOOL IS_SUPPORT_FLOAT_REALSIZE() { return TRUE; }; //東莞松八廠實際尺寸欄位型態為word, 非float

	virtual BOOL IS_SUPPORT_CUSTOM_ACTION() { return FALSE; } //是否支援客製化行為
	virtual void DoCustomAction() {}; //客製化行為
private:
	void Init();
	void Finalize();

	void ProcessAOIResponse(LPARAM lp);
	void ProcessResult();
	static void CALLBACK QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer);
	void ProcessTimer(UINT_PTR nEventId);

	void ON_CCL_NEWBATCH();
	void ON_C10_CHANGE(WORD wC10);

	void PushResult(BATCH_SHARE_SYST_RESULTCCL& xResult);
	void SetInfoField(BATCH_SHARE_SYST_INFO& xInfo);

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
		TIMER_MAX         //TIMER迴圈一次事件總數
	};
	UINT_PTR m_tTimerEvent[TIMER_MAX];
	static CEverStrProcessBase* m_this;

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

	PLC_DATA_ITEM_** m_pPLC_FIELD_INFO;

	static DWORD __stdcall Thread_Result(void* pvoid);
	void WriteResult(BATCH_SHARE_SYST_RESULTCCL& xData);
};
