using ClassLibrary;
using ClassLibrary.DataHeader;
using ClassLibrary.SharedComponent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary.PLC.Process.CCL
{
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
    enum PLC_FIELD
    {
        FIELD_ORDER = 0,            //工單
        FIELD_MATERIAL,             //料號
        FIELD_MODEL,                //模號
        FIELD_ASSIGN,               //分發號
        FIELD_ASSIGNNUM,            //分發號數量
        FIELD_SPLITNUM,             //一開幾數
        FIELD_SPLIT_ONE_Y,          //第一張大小板長
        FIELD_SPLIT_TWO_Y,          //第二張大小板長
        FIELD_SPLIT_THREE_Y,        //第三張大小板長

        FIELD_SPLIT_ONE_X,          //第一張大小板寬
        FIELD_SPLIT_TWO_X,          //第二張大小板寬
        FIELD_SPLIT_THREE_X,        //第三張大小板寬
        FIELD_THICK_SIZE,           //板材厚度
        FIELD_THICK_CCL,            //銅箔厚度
        FIELD_CCL_TYPE,             //銅箔特性
        FIELD_CCL_GRAYSCALE,        //灰度值
        FIELD_LEVEL_AA_PIXEL,       //點值要求(AA版點值)
        FIELD_LEVEL_A_PIXEL,        //點值要求(A版點值)
        FIELD_LEVEL_P_PIXEL,        //點值要求(P版點值)
        FIELD_DIFF_ONE_Y,           //第一個大小板公差(長)
        FIELD_DIFF_ONE_X,           //第一個大小板公差(寬)
        FIELD_DIFF_ONE_XY,          //第一個大小板對角線公差

        FIELD_DIFF_ONE_Y_MIN,       //第一個大小版經向公差下限
        FIELD_DIFF_ONE_Y_MAX,       //第一個大小版經向公差上限
        FIELD_DIFF_ONE_X_MIN,       //第一個大小版緯向公差下限
        FIELD_DIFF_ONE_X_MAX,       //第一個大小版緯向公差上限
        FIELD_DIFF_ONE_XY_MIN,      //第一個大小版對角線公差下限
        FIELD_DIFF_ONE_XY_MAX,      //第一個大小版對角線公差上限

        FIELD_DIFF_TWO_Y_MIN,       //第二個大小版經向公差下限
        FIELD_DIFF_TWO_Y_MAX,       //第二個大小版經向公差上限
        FIELD_DIFF_TWO_X_MIN,       //第二個大小版緯向公差下限
        FIELD_DIFF_TWO_X_MAX,       //第二個大小版緯向公差上限
        FIELD_DIFF_TWO_XY_MIN,      //第二個大小版對角線公差下限
        FIELD_DIFF_TWO_XY_MAX,      //第二個大小版對角線公差上限

        FIELD_DIFF_THREE_Y_MIN,     //第三個大小版經向公差下限
        FIELD_DIFF_THREE_Y_MAX,     //第三個大小版經向公差上限
        FIELD_DIFF_THREE_X_MIN,     //第三個大小版緯向公差下限
        FIELD_DIFF_THREE_X_MAX,     //第三個大小版緯向公差上限
        FIELD_DIFF_THREE_XY_MIN,    //第三個大小版對角線公差下限
        FIELD_DIFF_THREE_XY_MAX,    //第三個大小版對角線公差上限

        FIELD_AA_NUM,               //小版AA級數量
        FIELD_CCL_COMMAND,          //指令下發

        FIELD_CCL_NO_C06,           //C06小板剪切編號(版材序號)
        FIELD_CCL_NO_C10,           //C10小板剪切編號(開版後序號)
        FIELD_CCL_NO_C12,           //C12小版剪切編號

        FIELD_REAL_Y_ONE,           //小板實際長度1
        FIELD_REAL_Y_TWO,           //小板實際長度2
        FIELD_REAL_X_ONE,           //小板實際寬度1
        FIELD_REAL_X_TWO,           //小板實際寬度2
        FIELD_REAL_DIFF_ONE_Y,      //小板長度實際公差1
        FIELD_REAL_DIFF_TWO_Y,      //小板長度實際公差2
        FIELD_REAL_DIFF_ONE_X,      //小板寬度實際公差1
        FIELD_REAL_DIFF_TWO_X,      //小板寬度實際公差2
        FIELD_REAL_DIFF_ONE_XY,     //小板對角線實際公差1
        FIELD_REAL_DIFF_TWO_XY,     //小板對角線實際公差2

        FIELD_FRONT_LEVEL,          //正面判斷級別(1=AA/2=A/3=P)
        FIELD_FRONT_CODE,           //正面判斷代碼(G12)
        FIELD_FRONT_LOCATION,       //正面缺陷九宮格位置
        FIELD_BACK_LEVEL,           //反面判斷級別(1=AA/2=A/3=P)
        FIELD_BACK_CODE,            //反面判斷代碼(G12)
        FIELD_BACK_LOCATION,        //反面缺陷九宮格位置
        FIELD_SIZE_G10,             //尺寸判斷級別(G10)
        FIELD_SIZE_G12,             //尺寸判斷級別(G12)
        FIELD_SIZE_G14,             //尺寸判斷級別(G14)
        FIELD_SIZE_INFO_1,          //尺寸檢測準備好
        FIELD_SIZE_INFO_2,          //尺寸檢測運行
        FIELD_CCD_INFO_1,           //CCD準備好
        FIELD_CCD_INFO_2,           //CCD運行
        FIELD_CCD_ERROR_1,          //CCD故障
        FIELD_SIZE_ERROR_1,         //尺寸故障

        FIELD_COMMAND_RECEIVED,     //指令收到
        FIELD_RESULT,               //檢驗結果
        FIELD_RESULT_RECEIVED,      //檢驗結果收到

        FIELD_RESULT_LEVEL,         //小版物料級別
        FIELD_RESULT_AA,            //小版AA級數量
        FIELD_RESULT_A,             //小版A級數量
        FIELD_RESULT_P,             //小版P級數量
        FIELD_RESULT_QUALIFYRATE,   //訂單合格率
        FIELD_RESULT_DIFF_XY,       //級差實際檢測值
        FIELD_RESULT_MES,           //通知MES

        FIELD_BATCH_MES,            //通知MES工單資訊
        FIELD_INSP_SETTING,         //檢測設定
        FIELD_LIGHT_SETTING,        //光源設定
        FIELD_START_TIME,           //檢測開始時間
        FIELD_END_TIME,             //檢測結束時間
        FIELD_FRONT_DEFECT_SIZE_1,  //九宮格中正面前五大缺陷大小1
        FIELD_FRONT_DEFECT_SIZE_2,  //九宮格中正面前五大缺陷大小2
        FIELD_FRONT_DEFECT_SIZE_3,  //九宮格中正面前五大缺陷大小3
        FIELD_FRONT_DEFECT_SIZE_4,  //九宮格中正面前五大缺陷大小4
        FIELD_FRONT_DEFECT_SIZE_5,  //九宮格中正面前五大缺陷大小5
        FIELD_BACK_DEFECT_SIZE_1,   //九宮格中反面前五大缺陷大小1
        FIELD_BACK_DEFECT_SIZE_2,   //九宮格中反面前五大缺陷大小2
        FIELD_BACK_DEFECT_SIZE_3,   //九宮格中反面前五大缺陷大小3
        FIELD_BACK_DEFECT_SIZE_4,   //九宮格中反面前五大缺陷大小4
        FIELD_BACK_DEFECT_SIZE_5,   //九宮格中反面前五大缺陷大小5
        FIELD_FRONT_DEFECT_LOCATION_1,  //九宮格中正面前五大缺陷位置1
        FIELD_FRONT_DEFECT_LOCATION_2,  //九宮格中正面前五大缺陷位置2
        FIELD_FRONT_DEFECT_LOCATION_3,  //九宮格中正面前五大缺陷位置3
        FIELD_FRONT_DEFECT_LOCATION_4,  //九宮格中正面前五大缺陷位置4
        FIELD_FRONT_DEFECT_LOCATION_5,  //九宮格中正面前五大缺陷位置5
        FIELD_BACK_DEFECT_LOCATION_1,   //九宮格中反面前五大缺陷位置1
        FIELD_BACK_DEFECT_LOCATION_2,   //九宮格中反面前五大缺陷位置2
        FIELD_BACK_DEFECT_LOCATION_3,   //九宮格中反面前五大缺陷位置3
        FIELD_BACK_DEFECT_LOCATION_4,   //九宮格中反面前五大缺陷位置4
        FIELD_BACK_DEFECT_LOCATION_5,   //九宮格中反面前五大缺陷位置5

        FIELD_CUTTER_ORDER,             //F4->CCD(尺寸)掃碼訂單號
        FIELD_CUTTER_ASSIGN,            //F4->CCD(尺寸)掃碼分發號
        FIELD_CUTTER_Y,                 //F4->CCD(尺寸)長度
        FIELD_CUTTER_X,                 //F4->CCD(尺寸)寬度
        FIELD_CUTTER_INDEX,             //F4->CCD(尺寸)版編號

        FIELD_CUTTER_RETURN_ORDER,      //CCD->測厚掃碼訂單號
        FIELD_CUTTER_RETURN_ASSIGN,     //CCD->測厚掃碼分發號
        FIELD_CUTTER_RETURN_Y,          //CCD->測厚長度
        FIELD_CUTTER_RETURN_X,          //CCD->測厚寬度
        FIELD_CUTTER_RETURN_INDEX,      //CCD->測厚版編號

        FIELD_MAX
    };
    public interface ICCLPLCProcess
    {
        //.h
        //new int GetFieldSize();
        void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL xData);
        void DoSetInfoField(BATCH_SHARE_SYST_INFO xInfo);
        bool IS_SUPPORT_FLOAT_REALSIZE();
        bool IS_SUPPORT_CUSTOM_ACTION();
        void DoCustomAction();
        //.cpp
        ///<summary>PLC啟動觸發</summary>
        int ON_OPEN_PLC(IntPtr lparam);
        ///<summary>視窗控制碼觸發</summary>
        void ON_GPIO_NOTIFY(IntPtr wp, IntPtr lp);
        ///<summary>初始化項目</summary>
        void Init();
        ///<summary>處理AOI回應</summary>
        void ProcessAOIResponse(IntPtr lp);
        ///<summary>處理結果</summary>
        void ProcessResult();
        ///<summary>CALLBACK Timer</summary>
        void QueryTimer(IntPtr hwnd, uint uMsg, uint nEventId, ushort dwTimer);
        ///<summary>根據ProcessID迭代功能用</summary>
        void ProcessTimer(uint nEventId);
        ///<summary>資訊區塊</summary>
        void SetInfoField(BATCH_SHARE_SYST_INFO xInfo);
        ///<summary>CCL工單刷新</summary>
        void ON_CCL_NEWBATCH();
        ///<summary>C10改變觸發</summary>
        void ON_C10_CHANGE(short wC10);
        ///<summary>回傳資料(push_back)</summary>
        void PushResult(BATCH_SHARE_SYST_RESULTCCL xResult);
        ///<summary>依據線程寫入</summary>
        void ThreadResult();
        ///<summary>寫入區域參數</summary>
        void WriteResult(BATCH_SHARE_SYST_RESULTCCL xData);
    }
    class CCLProcessBase : PLCProcessBase, ICCLPLCProcess
    {
        public const int CCL_NOTIFYVALUE_COMMAND = 101;
        CCLProcessBase m_this = null;
        Timer[] m_tTimerEvent = new Timer[(int)TIMER.TIMER_MAX];
        List<BATCH_SHARE_SYST_RESULTCCL> m_Result;

        private Mutex m_oMutex = new Mutex();
        private Thread m_hThread;
        private EventWaitHandle[] m_hEvent = new EventWaitHandle[(int)EVENTCASE.EV_COUNT];

        //-------------------------------------------------------------
        public CCLProcessBase()
        {
            Init();
        }
        ~CCLProcessBase()
        {
            StopAllTimers();
        }
        //-------------------------------------------------------------
        public override void Init()
        {
            m_this = this;
            NotifyAOI((IntPtr)WM_APP_CMD.WM_SYST_PARAMINIT_CMD, IntPtr.Zero);
        }
        public void StopAllTimers()
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
        //-------------------------------------------------------------
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
        //-------------------------------------------------------------
        public virtual bool IS_SUPPORT_CUSTOM_ACTION() { return false; }
        public virtual bool IS_SUPPORT_FLOAT_REALSIZE() { return true; }
        public virtual void DoCustomAction() { }
        //-------------------------------------------------------------
        public virtual void DoSetInfoField(BATCH_SHARE_SYST_INFO xInfo) { }
        public virtual void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL xData) { }
        public virtual void ON_C10_CHANGE(short wC10)
        {
            string strLog;
            strLog = $"C10 Index: {wC10}";
            InsertDebugLog(strLog, AOI_LOG_Result.LOG_PLCC10);
            NotifyAOI((IntPtr)WM_APP_CMD.WM_SYST_C10CHANGE_CMD, (IntPtr)wC10);
        }
        public virtual void ON_CCL_NEWBATCH()
        {
            BATCH_SHARE_SYST_PARAMCCL xData = new BATCH_SHARE_SYST_PARAMCCL();
            xData.m_BATCH_SHARE_SYST_BASE.Name = GET_PLC_FIELD_VALUE<char[]>((int)PLC_FIELD.FIELD_ORDER);

            xData.m_BATCH_SHARE_SYST_BASE.Material = GET_PLC_FIELD_VALUE<char[]>((int)PLC_FIELD.FIELD_MATERIAL);
            xData.Model =             GET_PLC_FIELD_VALUE<char[]>((int)PLC_FIELD.FIELD_MODEL);
            xData.Assign =            GET_PLC_FIELD_VALUE<char[]>((int)PLC_FIELD.FIELD_ASSIGN);
            xData.AssignNum =         GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_ASSIGNNUM);

            xData.SplitNum =          GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_SPLITNUM);
            xData.Split_One_Y =       GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_SPLIT_ONE_Y);
            xData.Split_Two_Y =       GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_SPLIT_TWO_Y);
            xData.Split_Three_Y =     GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_SPLIT_THREE_Y);
            xData.Split_One_X =       GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_SPLIT_ONE_X);
            xData.Split_Two_X =       GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_SPLIT_TWO_X);
            xData.Split_Three_X =     GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_SPLIT_THREE_X);
                                      
            xData.ThickSize =         GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_THICK_SIZE);
            xData.ThickCCL =          GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_THICK_CCL);
                                      
            xData.CCLType =           GET_PLC_FIELD_VALUE<char[]>((int)PLC_FIELD.FIELD_CCL_TYPE);
            xData.GrayScale =         GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_CCL_GRAYSCALE);
            xData.Pixel_AA =          GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_LEVEL_AA_PIXEL);
            xData.Pixel_A =           GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_LEVEL_A_PIXEL);
            xData.Pixel_P =           GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_LEVEL_P_PIXEL);

            xData.Diff_One_Y =        GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_ONE_Y);
            xData.Diff_One_X =        GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_ONE_X);
            xData.Diff_One_XY =       GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_ONE_XY);

            xData.Diff_One_Y_Min =    GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_ONE_Y_MIN);
            xData.Diff_One_Y_Max =    GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_ONE_Y_MAX);
            xData.Diff_One_X_Min =    GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_ONE_X_MIN);
            xData.Diff_One_X_Max =    GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_ONE_X_MAX);
            xData.Diff_One_XY_Min =   GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_ONE_XY_MIN);
            xData.Diff_One_XY_Max =   GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_ONE_XY_MAX);

            xData.Diff_Two_Y_Min =    GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_TWO_Y_MIN);
            xData.Diff_Two_Y_Max =    GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_TWO_Y_MAX);
            xData.Diff_Two_X_Min =    GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_TWO_X_MIN);
            xData.Diff_Two_X_Max =    GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_TWO_X_MAX);
            xData.Diff_Two_XY_Min =   GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_TWO_XY_MIN);
            xData.Diff_Two_XY_Max =   GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_TWO_XY_MAX);

            xData.Diff_Three_Y_Min =  GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_THREE_Y_MIN);
            xData.Diff_Three_Y_Max =  GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_THREE_Y_MAX);
            xData.Diff_Three_X_Min =  GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_THREE_X_MIN);
            xData.Diff_Three_X_Max =  GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_THREE_X_MAX);
            xData.Diff_Three_XY_Min = GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_THREE_XY_MIN);
            xData.Diff_Three_XY_Max = GET_PLC_FIELD_VALUE<float>((int)PLC_FIELD.FIELD_DIFF_THREE_XY_MAX);

            xData.Num_AA =            GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_AA_NUM);
                                      
            xData.NO_C06 =            GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_CCL_NO_C06);
            xData.NO_C10 =            GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_CCL_NO_C10);
            xData.NO_C12 =            GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_CCL_NO_C12);

            byte[] tmp_byte = StructToBytes(xData);
            if (PLC_WriteData(tmp_byte, Marshal.SizeOf(typeof(BATCH_SHARE_SYST_PARAMCCL))))
            {
                //log data, not yet
                NotifyAOI((IntPtr)WM_APP_CMD.WM_SYST_PARAMCCL_CMD, IntPtr.Zero);
            }
        }
        public virtual void ProcessAOIResponse(IntPtr lparam)
        {
            switch ((int)lparam)//控制碼
            {
                case (int)WM_APP_CMD.WM_SYST_PARAMCCL_CMD: //AOI切換完成->復位
                    {
                        string wReceive = GET_PLC_FIELD_VALUE<string>((int)PLC_FIELD.FIELD_COMMAND_RECEIVED);
                        short wCommand = GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_CCL_COMMAND);
                        if (wCommand == CCL_NOTIFYVALUE_COMMAND)
                        {
                            short Receive = 100;
                            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_COMMAND_RECEIVED, 2, Receive);
                            InsertDebugLog("reset FIELD_COMMAND_RECEIVED", AOI_LOG_Result.LOG_DEBUG);
                        }
                        else
                        {
                            string str;
#if USE_IN_COMMUNICATOR
                            str = $"field error, FIELD_COMMAND_RECEIVED:{GET_PLC_FIELD_VALUE<string>((int)PLC_FIELD.FIELD_COMMAND_RECEIVED)}, " +
                                  $"FIELD_CCL_COMMAND:{GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_CCL_COMMAND)}";
                            InsertDebugLog(str);
#endif
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// 處理結果
        /// </summary>
        public virtual void ProcessResult()
        {
            BATCH_SHARE_SYST_RESULTCCL xResult = new BATCH_SHARE_SYST_RESULTCCL();
            short dwFlag = 0;
            int nOffset = 0;//最後全尺寸
            int nWordSize = sizeof(short);
            int nFloatSize = sizeof(float);
            string strLog = "", strTemp = "";

            Action<int, object, int, string, int> SetData = (dwAddFlag, pData, nSize, strAddLog, nLogType) =>
            {
                if ((dwFlag & dwAddFlag) > 0)//若無則跳過
                {
                    char[] tmp = new char[30];
                    byte[] buffer = new byte[Marshal.SizeOf(tmp)];
                    //-------------------------
                    if (pData is float)
                    {
                        buffer = new byte[Marshal.SizeOf(typeof(float))];
                    }
                    else if (pData is ushort)
                    {
                        buffer = new byte[Marshal.SizeOf(typeof(ushort))];
                    }
                    else if (pData is char[])
                    {
                        char[] chtmp = new char[30];
                        buffer = new byte[Marshal.SizeOf(chtmp)];
                    }
                    else if (pData is float[])
                    {
                        float[] fltmp = new float[5];
                        buffer = new byte[Marshal.SizeOf(fltmp)];
                    }
                    else if (pData is ushort[])
                    {
                        ushort[] ustmp = new ushort[5];
                        buffer = new byte[Marshal.SizeOf(typeof(float))];
                    }

                    //----------------------------
                    if (AOI_ReadData(ref buffer, nSize, nOffset))
                    {
                        nOffset += nSize;
                        string strTmp;
                        switch (nLogType)
                        {
                            case (int)LOG_STRUCT_TYPE.LOG_FLOAT: //float

                                if (buffer.Length != sizeof(float))//確認Byte array TO flaot OK
                                {
                                    throw new ArgumentException("Byte array length does not match float size");
                                }
                                strTmp = $" {strAddLog}: {BitConverter.ToSingle(buffer, 0)}\r\n";
                                break;
                            case (int)LOG_STRUCT_TYPE.LOG_WORD: //word

                                if (buffer.Length != sizeof(short))//確認Byte array TO short OK
                                {
                                    throw new ArgumentException("Byte array length does not match short size");
                                }
                                strTmp = $" {strAddLog}: {BitConverter.ToInt16(buffer, 0)}\r\n";
                                break;
                            case (int)LOG_STRUCT_TYPE.LOG_STRING: //cstring

                                int charCount = buffer.Length / sizeof(char);
                                char[] chars = new char[charCount];

                                Buffer.BlockCopy(buffer, 0, chars, 0, buffer.Length);
                                strTmp = $" {strAddLog}: {chars}\r\n";
                                break;
                        }
                        strLog += strTemp;
                    }
                }
            };

            InsertDebugLog("ProcessResult", AOI_LOG_Result.LOG_DEBUG);
            byte[] tmp_Readdata = StructToBytes(dwFlag);
            if (AOI_ReadData(ref tmp_Readdata, Marshal.SizeOf(dwFlag)))
            {
                nOffset += Marshal.SizeOf(typeof(short));
                strTemp = $"ProcessResult Flag:{dwFlag}";
                strLog += strTemp;
                //預設string 為char[30]
                SetData((int)SYST_RESULT_FLAG.SRF_REAL_Y_ONE, xResult.Real_One_Y, nFloatSize, "fReal_One_Y", (int)LOG_STRUCT_TYPE.LOG_FLOAT);
                SetData((int)SYST_RESULT_FLAG.SRF_REAL_Y_TWO, xResult.Real_Two_Y, nFloatSize, "fReal_Two_Y", (int)LOG_STRUCT_TYPE.LOG_FLOAT);
                SetData((int)SYST_RESULT_FLAG.SRF_REAL_X_ONE, xResult.Real_One_X, nFloatSize, "fReal_One_X", (int)LOG_STRUCT_TYPE.LOG_FLOAT);
                SetData((int)SYST_RESULT_FLAG.SRF_REAL_X_TWO, xResult.Real_Two_X, nFloatSize, "fReal_Two_X", (int)LOG_STRUCT_TYPE.LOG_FLOAT);
                SetData((int)SYST_RESULT_FLAG.SRF_REAL_DIFF_Y_ONE, xResult.Diff_One_Y, nWordSize, "wDiff_One_Y", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_REAL_DIFF_Y_TWO, xResult.Diff_Two_Y, nWordSize, "wDiff_Two_Y", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_REAL_DIFF_X_ONE, xResult.Diff_One_X, nWordSize, "wDiff_One_X", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_REAL_DIFF_X_TWO, xResult.Diff_Two_X, nWordSize, "wDiff_Two_X", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_REAL_DIFF_XY_ONE, xResult.Diff_One_XY, nWordSize, "wDiff_One_XY", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_REAL_DIFF_XY_TWO, xResult.Diff_Two_XY, nWordSize, "wDiff_Two_XY", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_FRONT_LEVEL, xResult.FrontLevel, nWordSize, "wFrontLevel", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_FRONT_CODE, xResult.FrontCode, Marshal.SizeOf(xResult.FrontCode), "cFrontCode", (int)LOG_STRUCT_TYPE.LOG_STRING);
                SetData((int)SYST_RESULT_FLAG.SRF_BACK_LEVEL, xResult.BackLevel, nWordSize, "wBackLevel", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_BACK_CODE, xResult.BackCode, Marshal.SizeOf(xResult.BackCode), "cBackCode", (int)LOG_STRUCT_TYPE.LOG_STRING);
                SetData((int)SYST_RESULT_FLAG.SRF_SIZE_G10, xResult.Size_G10, nWordSize, "wSize_G10", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_SIZE_G12, xResult.Size_G12, nWordSize, "wSize_G12", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_SIZE_G14, xResult.Size_G14, nWordSize, "wSize_G14", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_RESULT_LEVEL, xResult.ResultLevel, nWordSize, "wResultLevel", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_NUM_AA, xResult.Num_AA, nWordSize, "wNum_AA", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_NUM_A, xResult.Num_A, nWordSize, "wNum_A", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_NUM_P, xResult.Num_P, nWordSize, "wNum_P", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_QUALIFY_RATE, xResult.QualifyRate, nWordSize, "wQualifyRate", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_DIFF_XY, xResult.Diff_XY, nWordSize, "wDiff_XY", (int)LOG_STRUCT_TYPE.LOG_WORD);
                SetData((int)SYST_RESULT_FLAG.SRF_FRONT_SIZE, xResult.FrontDefectSize, Marshal.SizeOf(xResult.FrontDefectSize), "", (int)LOG_STRUCT_TYPE.LOG_NONE);
                SetData((int)SYST_RESULT_FLAG.SRF_FRONT_LOCATION, xResult.FrontDefectLocation, Marshal.SizeOf(xResult.FrontDefectLocation), "", (int)LOG_STRUCT_TYPE.LOG_NONE);
                SetData((int)SYST_RESULT_FLAG.SRF_BACK_SIZE, xResult.BackDefectSize, Marshal.SizeOf(xResult.BackDefectSize), "", (int)LOG_STRUCT_TYPE.LOG_NONE);
                SetData((int)SYST_RESULT_FLAG.SRF_BACK_LOCATION, xResult.BackDefectLocation, Marshal.SizeOf(xResult.BackDefectLocation), "", (int)LOG_STRUCT_TYPE.LOG_NONE);
                SetData((int)SYST_RESULT_FLAG.SRF_INDEX, xResult.Index, nWordSize, "", (int)LOG_STRUCT_TYPE.LOG_NONE);
                InsertDebugLog(strLog, AOI_LOG_Result.LOG_DEBUG);
                PushResult(xResult);
            }
        }
        public virtual void ProcessTimer(uint nEventId)
        {
            for (int i = 0; i < (int)TIMER.TIMER_MAX; i++)
            {
                //if (m_tTimerEvent[i] == nEventId)
                {
                    switch (i)
                    {
# if USE_TEST_TIMER
                        case (int)TIMER.TIMER_TEST:
                            {
                                string Frontstr = "12345678901234567890123456789a";
                                string Backstr =  "abcdefghijklmnopqrstuvwxyzabca";
                                BATCH_SHARE_SYST_RESULTCCL xResult = new BATCH_SHARE_SYST_RESULTCCL
                                {
                                    Diff_One_Y = (short)m_Result.Count(),
                                    Diff_Two_Y = 2,
                                    Diff_One_X = 3,
                                    Diff_Two_X = 4,
                                    Diff_One_XY = 5,
                                    Diff_Two_XY = 6,
                                    FrontLevel = 7,
                                    //FrontLocation = 8;
                                    BackLevel = 9,
                                    //BackLocation = 10;
                                    Size_G10 = 11,
                                    Size_G12 = 12,
                                    Size_G14 = 13,

                                    Real_One_Y = 1.1f,
                                    Real_Two_Y = 2.2f,
                                    Real_One_X = 3.3f,
                                    Real_Two_X = 4.4f,

                                    FrontCode = Frontstr.ToCharArray(),
                                    BackCode = Backstr.ToCharArray(),

                                    ResultLevel = 1,
                                    Num_AA = 2,
                                    Num_A = 3,
                                    Num_P = 4,
                                    QualifyRate = 5,
                                    Diff_XY = 6
                                };
                                BATCH_SHARE_SYST_INFO xInfo = new BATCH_SHARE_SYST_INFO();
                                xInfo.Info1.SizeReady = 1;
                                xInfo.Info1.SizeRunning = 1;
                                xInfo.Info1.CCDReady = 1;
                                xInfo.Info1.CCDRunning = 1;
                                xInfo.Info2.CCDError1 = 1;
                                xInfo.Info2.SizeError1 = 1;

                                PushResult(xResult);
                                SetInfoField(xInfo);
                            }
                            break;
#endif
                        case (int)TIMER.TIMER_COMMAND:
                            {
                                short wOldCommand = GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_CCL_COMMAND);//原本控制碼
                                REFLASH_PLC_FIELD_DATA((int)PLC_FIELD.FIELD_CCL_COMMAND);//更新
                                short wCommand = GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_CCL_COMMAND);//更新控制碼

                                if (wOldCommand == 0 && wCommand == CCL_NOTIFYVALUE_COMMAND)//
                                {
                                    for (int j = 0; j < GetFieldSize(); j++)
                                    {
                                        PLC_DATA_ITEM pItem = GetPLCAddressInfo(j, false);
                                        if ((pItem != null) && (pItem.Action == emPLC_ACTION_TYPE.ACTION_BATCH))
                                        {
                                            REFLASH_PLC_FIELD_DATA(j);//更新
                                        }
                                    }
                                    ON_CCL_NEWBATCH();//餵回去BATCH
                                }
                                else if (wOldCommand != 0 && wCommand == CCL_NOTIFYVALUE_COMMAND)//控制碼變化不對
                                {   
                                    //log error data
                                    string strLog;
                                    strLog = $"FIELD_CCL_COMMAND previous data error: {wOldCommand}";
                                    InsertDebugLog(strLog, AOI_LOG_Result.LOG_DEBUG);
                                }
                                if (wCommand != CCL_NOTIFYVALUE_COMMAND && wCommand != 0)
                                {
                                    string strLog;
                                    strLog = $"FIELD_CCL_COMMAND data error: {wCommand}";
                                    InsertDebugLog(strLog, AOI_LOG_Result.LOG_DEBUG);
                                }
                            }
                            break;
                        case (int)TIMER.TIMER_COMMAND_RECEIVED:
                            {
                                REFLASH_PLC_FIELD_DATA((int)PLC_FIELD.FIELD_COMMAND_RECEIVED);
                            }
                            break;
                        case (int)TIMER.TIMER_RESULT:
                            {
                                REFLASH_PLC_FIELD_DATA((int)PLC_FIELD.FIELD_RESULT);
                                if (GET_FLUSH_ANYWAY())
                                {
                                    m_hEvent[(int)EVENTCASE.EV_WRITE].Set();
                                }
                                else
                                {
                                    if (GET_PLC_FIELD_VALUE<string>((int)PLC_FIELD.FIELD_RESULT) == "0")
                                    {
                                        //ready to write
                                        m_hEvent[(int)EVENTCASE.EV_WRITE].Set();
                                    }

                                    else
                                    {
                                        string str;
                                        str = $"PLC not ready to receive insp result, Field value: {GET_PLC_FIELD_VALUE<string>((int)PLC_FIELD.FIELD_RESULT)} \n";
                                        InsertDebugLog(str);
                                    }
                                }
                            }
                            break;
                        case (int)TIMER.TIMER_RESULT_RECEIVED:
                            {
                                REFLASH_PLC_FIELD_DATA((int)PLC_FIELD.FIELD_RESULT_RECEIVED);
                                if (GET_PLC_FIELD_VALUE<string>((int)PLC_FIELD.FIELD_RESULT_RECEIVED) == "300")
                                {
                                    Console.WriteLine("PLC has received result! \n");
                                    PLC_DATA_ITEM pResultReceived = GetPLCAddressInfo((int)PLC_FIELD.FIELD_RESULT_RECEIVED, false);
                                    short wValue = 0;
                                    SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_RESULT_RECEIVED, 2, wValue); //復位
                                }
                            }
                            break;
                        case (int)TIMER.TIMER_C10:
                            {
                                if (GET_PLC_FIELD_ACTION((int)PLC_FIELD.FIELD_CCL_NO_C10) != emPLC_ACTION_TYPE.ACTION_NOTIFY)
                                {
                                    //僅notify type要啟動timer看C10 index
                                    m_tTimerEvent[(int)TIMER.TIMER_C10]?.Dispose();
                                    continue;
                                }
                                short wOldC10 = GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_CCL_NO_C10);
                                REFLASH_PLC_FIELD_DATA((int)PLC_FIELD.FIELD_CCL_NO_C10);
                                short wCurC10 = GET_PLC_FIELD_VALUE<short>((int)PLC_FIELD.FIELD_CCL_NO_C10);
                                if (wCurC10 != 0 /*東莞每次變化前會歸零*/ && wOldC10 != wCurC10)
                                {
                                    ON_C10_CHANGE(wCurC10);
                                }
                            }
                            break;
                        case (int)TIMER.TIMER_CUSTOM_ACTION:
                            {
                                DoCustomAction();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public virtual void PushResult(BATCH_SHARE_SYST_RESULTCCL xResult)
        {
            lock (m_oMutex)
            {
                m_Result.Add(xResult);
            }
        }
        public virtual void QueryTimer(IntPtr hwnd, uint uMsg, uint nEventId, ushort dwTimer)
        {
            if (m_this !=null)
            {
                m_this.ProcessTimer(nEventId);
            }
        }
        public virtual void SetInfoField(BATCH_SHARE_SYST_INFO xInfo)
        {
            InsertDebugLog("Set Info Start", AOI_LOG_Result.LOG_DEBUG);
            DoSetInfoField(xInfo);
            InsertDebugLog("Set Info End", AOI_LOG_Result.LOG_DEBUG);
        }
        public virtual void ThreadResult()
        {
            CCLProcessBase pThis = new CCLProcessBase();
            bool bRun = true;
            while (bRun)
            {
                switch (WaitHandle.WaitAny(m_hEvent))
                {
                    case (int)EVENTCASE.CASE_WRITE:
                        {
                            //====================================
                            m_hEvent[(int)EVENTCASE.EV_WRITE].Reset();
                            BATCH_SHARE_SYST_RESULTCCL pData;
                            lock (m_oMutex)
                            {
                                if (m_Result.Count > 0)
                                {
                                    pData = m_Result[0];
                                    m_Result.RemoveAt(0);
                                }
                                else
                                {
                                    pData = new BATCH_SHARE_SYST_RESULTCCL(); // Placeholder or handle empty case
                                }
                            }
                            WriteResult(pData); // Call your WriteResult method with the data
                            //======================================
                        }
                        break;
                    case (int)EVENTCASE.CASE_EXIT:
                        {
                        }
                        break;
                }
            }
        }
        public virtual void WriteResult(BATCH_SHARE_SYST_RESULTCCL xData)
        {
# if USE_IN_COMMUNICATOR
            InsertDebugLog("Write Insp start!",AOI_LOG_Result.LOG_DEBUG);
#endif
            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_FRONT_LEVEL, 2, xData.FrontLevel);
            SET_PLC_FIELD_DATA<char[]>((int)PLC_FIELD.FIELD_FRONT_CODE, Marshal.SizeOf(xData.FrontCode), xData.FrontCode);
            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_BACK_LEVEL, 2, xData.BackLevel);
            SET_PLC_FIELD_DATA<char[]>((int)PLC_FIELD.FIELD_BACK_CODE, Marshal.SizeOf(xData.BackCode), xData.BackCode);

            if (IS_SUPPORT_FLOAT_REALSIZE())
            {
                SET_PLC_FIELD_DATA<float>((int)PLC_FIELD.FIELD_REAL_Y_ONE, 4, xData.Real_One_Y);
                SET_PLC_FIELD_DATA<float>((int)PLC_FIELD.FIELD_REAL_Y_TWO, 4, xData.Real_Two_Y);
                SET_PLC_FIELD_DATA<float>((int)PLC_FIELD.FIELD_REAL_X_ONE, 4, xData.Real_One_X);
                SET_PLC_FIELD_DATA<float>((int)PLC_FIELD.FIELD_REAL_X_TWO, 4, xData.Real_Two_X);
            }

            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_REAL_DIFF_ONE_Y, 2,  xData.Diff_One_Y);
            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_REAL_DIFF_TWO_Y, 2,  xData.Diff_Two_Y);
            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_REAL_DIFF_ONE_X, 2,  xData.Diff_One_X);
            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_REAL_DIFF_TWO_X, 2,  xData.Diff_Two_X);
            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_REAL_DIFF_ONE_XY, 2, xData.Diff_One_XY);
            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_REAL_DIFF_TWO_XY, 2, xData.Diff_Two_XY);

            DoWriteResult(xData);

            //write flag
            short wResult = 200;
            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_RESULT, 2, wResult);

# if USE_IN_COMMUNICATOR
            InsertDebugLog("Write Insp done!",AOI_LOG_Result.LOG_DEBUG);
#endif
        }


    }
}
