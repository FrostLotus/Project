using ClassLibrary.DataHeader;
using ClassLibrary.SharedComponent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary.PLC.Process.EVERSTRONG
{
	enum PLC_FIELD
	{
		//下發------------------------------------------------
		FIELD_BEGIN,
		FIELD_ORDER = FIELD_BEGIN,         //訂單號					             1      D1000~D1009		  string[20]    
		FIELD_SN,                          //批號					             2      D1010~D1019		  string[20]     =  FIELD_ASSIGN 分發號
		FIELD_QUANTITY,                    //工單產品數量				             3      D1020			  word    =2     =  FIELD_ASSIGNNUM 分發號數量

		FIELD_SPLITNUM,                    //一開幾數					             4      D1021			  word    =2          
		FIELD_SPLIT_ONE_Y,                 //第一張大小板長			             5      D1022~D1023		  real    =4          
		FIELD_SPLIT_TWO_Y,                 //第二張大小板長			             6      D1024~D1025		  real    =4          
		FIELD_SPLIT_THREE_Y,               //第三張大小板長			             7      D1026~D1027		  real    =4          
		FIELD_SPLIT_ONE_X,                 //第一張大小板寬			             8      D1028~D1028		  real    =4          
		FIELD_SPLIT_TWO_X,                 //第二張大小板寬			             9      D1030~D1031		  real    =4          
		FIELD_SPLIT_THREE_X,               //第三張大小板寬			            10      D1032~D1033		  real    =4          

		FIELD_MATERIAL,                    //訂單物料代碼				            14      D1037~D1046		  string[20]    

		FIELD_DIFF_ONE_Y_MIN,              //第一個大小版經向公差下限				15		D1047			  word          
		FIELD_DIFF_ONE_Y_MAX,              //第一個大小版經向公差上限				16		D1048			  word          
		FIELD_DIFF_ONE_X_MIN,              //第一個大小版緯向公差下限				17		D1049			  word          
		FIELD_DIFF_ONE_X_MAX,              //第一個大小版緯向公差上限				18		D1050			  word          
		FIELD_DIFF_ONE_XY_MIN,             //第一個大小版對角線公差下限				19		D1051			  word          
		FIELD_DIFF_ONE_XY_MAX,             //第一個大小版對角線公差上限				20		D1052			  word          

		FIELD_DIFF_TWO_Y_MIN,              //第二個大小版經向公差下限				21		D1053			  word          
		FIELD_DIFF_TWO_Y_MAX,              //第二個大小版經向公差上限				22		D1054			  word          
		FIELD_DIFF_TWO_X_MIN,              //第二個大小版緯向公差下限				23		D1055			  word          
		FIELD_DIFF_TWO_X_MAX,              //第二個大小版緯向公差上限				24		D1056			  word          
		FIELD_DIFF_TWO_XY_MIN,             //第二個大小版對角線公差下限				25		D1057			  word          
		FIELD_DIFF_TWO_XY_MAX,             //第二個大小版對角線公差上限				26		D1058			  word          

		FIELD_DIFF_THREE_Y_MIN,            //第三個大小版經向公差下限				27		D1059			  word          
		FIELD_DIFF_THREE_Y_MAX,            //第三個大小版經向公差上限				28		D1060			  word          
		FIELD_DIFF_THREE_X_MIN,            //第三個大小版緯向公差下限				29		D1061			  word          
		FIELD_DIFF_THREE_X_MAX,            //第三個大小版緯向公差上限				30		D1062			  word          
		FIELD_DIFF_THREE_XY_MIN,           //第三個大小版對角線公差下限				31		D1063			  word          
		FIELD_DIFF_THREE_XY_MAX,           //第三個大小版對角線公差上限				32		D1064			  word          
										   //上傳---------------------------------------------------													           
		FIELD_CCL_COMMAND,                 //指令下發(1/0)						34		 D1066			  word          
		FIELD_CCL_NO_C10,                  //板剪切編(開板後序號)					35		 D1067			  word          

		FIELD_REAL_Y_ONE,                  //板實際長度1							36		 D1300~D1301	  real          
		FIELD_REAL_Y_TWO,                  //板實際長度2							37		 D1302~D1303	  real          
		FIELD_REAL_X_ONE,                  //板實際寬度1							38		 D1304~D1305	  real          
		FIELD_REAL_X_TWO,                  //板實際寬度2							39		 D1306~D1307	  real   

		FIELD_REAL_DIFF_ONE_Y,             //板長度實際公差1						40		 D1308~D1309	  real          
		FIELD_REAL_DIFF_TWO_Y,             //板長度實際公差2						41		 D1310~D1311	  real          
		FIELD_REAL_DIFF_ONE_X,             //板寬度實際公差1						42		 D1312~D1313	  real          
		FIELD_REAL_DIFF_TWO_X,             //板寬度實際公差2						43		 D1314~D1315	  real          
		FIELD_REAL_DIFF_ONE_XY,            //板對角線實際公差1						44		 D1316~D1317	  real          
		FIELD_REAL_DIFF_TWO_XY,            //板對角線實際公差2						45		 D1318~D1319	  real          

		FIELD_FRONT_LEVEL,                 //表現正面判斷級別(1 = OK,2 = NG)       46    	 D1320			  word          
		FIELD_BACK_LEVEL,                  //表現反面判斷級別(1 = OK,2 = NG)		48	     D1336			  word          
		FIELD_SIZE_G10,                    //版尺寸判斷OK = 1,NG = 2 一開一 一個版	50       D1352			  word          
		FIELD_SIZE_G12,                    //版尺寸判斷OK = 1,NG = 2 一開二 兩個版	51       D1353			  word          
		FIELD_SIZE_G14,                    //版尺寸判斷OK = 1,NG = 2 一開三 三個板	52       D1354			  word          

		FIELD_CCD_COMMAND_RECEIVED,        //CCD接收 MES  資料完成  [指令收到]		53		 D1355			  word          																											            
		FIELD_CCD_RESULT,                  //CCD發送檢測結果						54		 D1356			  word          
		FIELD_CCD_RESULT_RECEIVED,         //CCD接收PLC接收檢測結果完成				55		 D1357			  word          

		FIELD_RESULT_OKNum,                //版OK数量							57		 D1359			  word           =FIELD_RESULT_A
		FIELD_RESULT_NGNum,                //版NG数量							58		 D1360			  word           =FIELD_RESULT_P
		FIELD_RESULT_QUALIFYRATE,          //訂單合格率							60		 D1362~D1363	  real          

		FIELD_BATCH_MES,                   //通知MES工單資訊						62		 D1366			  word          
		FIELD_INSP_SETTING,                //檢測設定								63		 D1367~D1371	  string[10]    
		FIELD_LIGHT_SETTING,               //光源設定								64		 D1372~D1376	  string[10]    
		FIELD_START_TIME,                  //檢測開始時間							65		 D1377~D1385	  string[18]    
		FIELD_END_TIME,                    //檢測結束時間							66		 D1387~D1395	  string[18]  

		FIELD_ORDER_1,                     //訂單號								67		 D2000~D2009	  string[20]    
		FIELD_SN_1,                        //批號								68		 D2010~D2019	  string[20]    
		FIELD_MATERIAL_1,                  //訂單物料代碼							69		 D2037~D2046	  string[20]    

		FIELD_MAX
	};
	enum EVENTCASE
	{
		EV_EXIT,
		EV_WRITE,
		EV_COUNT,
		CASE_EXIT = 0,
		CASE_WRITE,
	};
	enum TIMER
	{
		TIMER_COMMAND,          //指令下發[0]
		TIMER_COMMAND_RECEIVED, //指令收到[1]
		TIMER_RESULT,           //檢驗結果[2]
		TIMER_RESULT_RECEIVED,  //檢驗結果收到[3]
		TIMER_C10,              //C10剪切小版編號[4]
		TIMER_CUSTOM_ACTION,    //客製化行為[5]
#if USE_TEST_TIMER
        TIMER_TEST,             //[6]
#endif
		TIMER_MAX               //[7]
	};
	class EverStrongProcessBase : PLCProcessBase
	{
		public const int EVERSTR_NOTIFYVALUE_COMMAND = 101;
		//---------------------------------------------------
		private EverStrongProcessBase m_this = null;
		private Timer[] m_tTimerEvent = new Timer[(int)TIMER.TIMER_MAX];
		private List<BATCH_SHARE_SYST_RESULT_EVERSTR> m_Result;


		private Mutex m_oMutex = new Mutex();
		private Thread m_hThread;
		private EventWaitHandle[] m_hEvent = new EventWaitHandle[(int)EVENTCASE.EV_COUNT];

		public EverStrongProcessBase()
		{
			Init();
		}
		~EverStrongProcessBase()
		{
			StopAllTimers();
		}
		//------------------------------------------------------------------
		public override void Init()
		{
			m_this = this;
			NotifyAOI((IntPtr)WM_APP_CMD.WM_SYST_PARAMINIT_CMD, IntPtr.Zero);
		}
		public override int GetFieldSize() { return (int)PLC_FIELD.FIELD_MAX; }
		public override int ON_OPEN_PLC(IntPtr lparam)
		{
			int lRtn = base.ON_OPEN_PLC(lparam);

			if (lRtn == 0)
			{
				for (int i = 0; i < (int)TIMER.TIMER_MAX; i++)
				{
#if USE_TEST_TIMER
					if (i == (int)TIMER.TIMER_TEST || (int)TIMER.TIMER_RESULT == i/*make write time reasonable*/)//
					{
						m_tTimerEvent[i] = new Timer(null, i, 500, 500);
					}
					else { }
#endif
					bool bSettimer = true;
					switch (i)
					{
						case (int)TIMER.TIMER_CUSTOM_ACTION:
							bSettimer = IS_SUPPORT_CUSTOM_ACTION();
							break;
						default:
							bSettimer = true;//應該為false
							break;
					}
					if (bSettimer)
					{
						m_tTimerEvent[i] = new Timer(null, i, 500, 500);
					}
				}
				for (int i = 0; i < (int)EVENTCASE.EV_COUNT; i++)
				{
					//m_hEvent[i] = CreateEvent(IntPtr.Zero, true, false, null);
					m_hEvent[i] = new ManualResetEvent(false);
				}
				m_hThread = new Thread(new ThreadStart(ThreadResult));

			}
			return lRtn;
		}
		public override void ON_GPIO_NOTIFY(IntPtr wparam, IntPtr lparam)
		{
			switch ((int)wparam)
			{
				case (int)WM_APP_CMD.WM_AOI_RESPONSE_CMD://AOI回傳通知
					ProcessAOIResponse(lparam);
					break;
				case (int)WM_APP_CMD.WM_SYST_RESULTCCL_CMD://PLC回傳
					ProcessResult();
					break;
				case (int)WM_APP_CMD.WM_SYST_INFO_CHANGE://資訊資料變換
					BATCH_SHARE_SYST_INFO xInfo = new BATCH_SHARE_SYST_INFO();
					InsertDebugLog("WM_SYST_INFO_CHANGE", AOI_LOG_Result.LOG_DEBUG);
					byte[] tmp_byte = StructToBytes(xInfo);
					if (AOI_ReadData(ref tmp_byte, Marshal.SizeOf(typeof(BATCH_SHARE_SYST_INFO))))//取AOI資料
					{
						SetInfoField(xInfo);
					}
					break;
			}
		}
		//------------------------------------------------------------------
		public virtual bool IS_SUPPORT_CUSTOM_ACTION() { return false; }
		//------------------------------------------------------------------
		private void StopAllTimers()
		{
			for (int i = 0; i < (int)TIMER.TIMER_MAX; i++)
			{
				StopTimer(i);
			}
		}
		private void StopTimer(int i)
		{
			m_tTimerEvent[i].Change(Timeout.Infinite, Timeout.Infinite);
		}
	}
}
