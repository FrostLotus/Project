﻿using ClassLibrary.SharedComponent.Log;
using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DataHeader
{
    #region [enum]定義視窗傳值、Flag、Type、客戶代碼
    /// <summary>[enum]視窗訊息參數</summary>
    public enum WM_APP_CMD
    {
        //AOI = Area of interest
        //DRL = Data Result Linker

        MAX_BATCH_FIELD_LEN = 100,
        //RESPONSE                     
        WM_AOI_RESPONSE_CMD = (0x8000 + 2),//AOI回應->DRL
        WM_SYST_RESPONSE_CMD = (0x8000 + 3),//DRL回應->AOI

        WM_SYST_PARAMINIT_CMD = (0x8000 + 4),//DRL通知初始化->AOI

        WM_SYST_PARAMCCL_CMD = (0x8000 + 5),
        WM_SYST_PARAMWEBCOOPER_CMD = (0x8000 + 6),
        WM_SYST_RESULTCCL_CMD = (0x8000 + 7),
        WM_SYST_C10CHANGE_CMD = (0x8000 + 8),
        WM_SYST_INFO_CHANGE = (0x8000 + 18),

        WM_EMC_RESPONSE_CMD = (0x8000 + 9),
        WM_EMC_PARAMINIT_CMD = (0x8000 + 10),
        WM_EMC_PARAMCCL_CMD = (0x8000 + 11),
        WM_EMC_RESULTCCL_CMD = (0x8000 + 12),
        WM_EMC_ENDCCL_CMD = (0x8000 + 13),
        WM_EMC_PARAMPP_CMD = (0x8000 + 14),
        WM_EMC_RESULTPP_CMD = (0x8000 + 15),
        WM_EMC_ENDPP_CMD = (0x8000 + 16),
        WM_EMC_ERROR_CMD = (0x8000 + 17),

        WM_MX_PARAMINIT_CMD = (0x8000 + 18),
        WM_MX_PINSTATUS_CMD = (0x8000 + 19),
        WM_MX_PININFO_CMD = (0x8000 + 20),
        WM_MX_PINRESULT_CMD = (0x8000 + 21),

        WM_CUSTOMERTYPE_INIT = (0x8000 + 22),
        WM_THICKINFO_CMD = (0x8000 + 23),
        WM_OPC_NEWBATCH_CMD = (0x8000 + 24),
        WM_SYST_PP_PARAMINIT_CMD = (0x8000 + 25),
        WM_WATCHDOG_CMD = (0x8000 + 26),
        WM_PLC_PP_CMD = (0x8000 + 27),
        WM_WS_POTENTIAL_CMD = (0x8000 + 28),
        WM_SYST_EXTRA_CMD = (0x8000 + 29),


        WM_PLC_INPUT_CMD = (0x8000 + 30),
        WM_PLC_OUTPUTPIN_CMD = (0x8000 + 31),
        WM_ASSIGN_ENCODER_CMD = (0x8000 + 32),
        WM_PLC_ENCODER_POS_CMD = (0x8000 + 33),
        WM_INSPSTATUS_CMD = (0x8000 + 34),
        WM_TECHAIN_CMD = (0x8000 + 35),
        WM_SQL_CMD = (0x8000 + 36),

        WM_OPC_PARAMINIT_CMD = (0x8000 + 37),
        WM_OPC_WRITE_CMD = (0x8000 + 38),
        WM_OPC_FINISHWRITE_CMD = (0x8000 + 39),
        WM_OPC_RETURN_STATUS_CMD = (0x8000 + 40),
        WM_OPC_SET_CONNECT_CMD = (0x8000 + 41),

        WM_PLC_REVERSE_BEGIN = (0x8000 + 42),
        WM_PLC_REVERSE_END = (0x8000 + 43),

        WM_GPIO_MSG = (0x8000 + 990)
    }
    /// <summary>[enum]視窗訊息參數</summary>
    enum WM_Status
    {
        WM_CREATE = 0,
        WM_DESTROY,
    };
    /// <summary>[enum]PLC訊息參數</summary>
    enum PLC_MESSAGE
    {
        PM_VERSION_ERROR,
        PM_A_AXIS,
        PM_B_AXIS,
        PM_SWITCH_WEB_SHEET,
        PM_SHEET_NEWBATCH,
        PM_CURRENT_INSPSTATUS,
    };
    /// <summary>[enum]系統回應參數</summary>
    enum SYST_RESULT_FLAG
    {
        SRF_REAL_Y_ONE = 0x0001,                   //小板實際長度1
        SRF_REAL_Y_TWO = 0x0002,                   //小板實際長度2
        SRF_REAL_X_ONE = 0x0004,                   //小板實際寬度1
        SRF_REAL_X_TWO = 0x0008,                   //小板實際寬度2
        SRF_REAL_DIFF_Y_ONE = 0x0010,              //小板長度實際公差1
        SRF_REAL_DIFF_Y_TWO = 0x0020,              //小板長度實際公差2
        SRF_REAL_DIFF_X_ONE = 0x0040,              //小板寬度實際公差1
        SRF_REAL_DIFF_X_TWO = 0x0080,              //小板寬度實際公差2
        SRF_REAL_DIFF_XY_ONE = 0x0100,             //小板對角線實際公差1
        SRF_REAL_DIFF_XY_TWO = 0x0200,             //小板對角線實際公差2

        SRF_FRONT_LEVEL = 0x0400,                  //正面判斷級別
        SRF_FRONT_CODE = 0x0800,                   //正面判斷代碼
        SRF_BACK_LEVEL = 0x1000,                   //反面判斷級別
        SRF_BACK_CODE = 0x2000,                    //反面判斷代碼
        SRF_SIZE_G10 = 0x4000,                     //尺寸判斷級別(G10)
        SRF_SIZE_G12 = 0x8000,                     //尺寸判斷級別(G12)
        SRF_SIZE_G14 = 0x010000,                   //尺寸判斷級別(G14)

        SRF_RESULT_LEVEL = 0x020000,               //小板物料級別
        SRF_NUM_AA = 0x040000,                     //小版AA級數量
        SRF_NUM_A = 0x080000,                      //小版A級數量
        SRF_NUM_P = 0x100000,                      //小版P級數量
        SRF_QUALIFY_RATE = 0x200000,               //訂單合格率
        SRF_DIFF_XY = 0x400000,                    //級差實際檢測值

        SRF_FRONT_SIZE = 0x800000,                 //九宮格中正面前五大缺陷大小1~5
        SRF_FRONT_LOCATION = 0x1000000,            //九宮格中正面前五大缺陷位置1~5
        SRF_BACK_SIZE = 0x2000000,                 //九宮格中反面前五大缺陷大小1~5
        SRF_BACK_LOCATION = 0x4000000,             //九宮格中反面前五大缺陷位置1~5
        SRF_INDEX = 0x8000000                      //小板編號
    };
    /// <summary>[enum]EMC錯誤參數</summary>
    enum EMC_ERROR
    {
        EMC_FIELD_NOTCOMPLETE,  //欄位不齊全
        EMC_TIMEOUT,            //TIMEOUT
    };
    /// <summary>[enum]紀錄格式參數</summary>
    enum LOG_STRUCT_TYPE
    {
        LOG_FLOAT_ARRAY,
        LOG_FLOAT,
        LOG_WORD,  //short
        LOG_STRING,
        LOG_CHAR,
        LOG_NONE//log不紀錄
    };
    /// <summary>[enum]系統資訊參數</summary>
    enum SYST_INFO_FIELD
    {
        SIZE_READY = 0x01,   //CCD尺寸檢測儀器準備好
        SIZE_RUNNING = 0x02, //CCD尺寸檢測儀器運行
        SIZE_ERROR = 0x04,   //CCD尺寸檢測儀器故障
        CCD_READY = 0x08,    //CCD表現檢測儀器準備好
        CCD_RUNNING = 0x10,  //CCD表現檢測儀器運行
        CCD_ERROR = 0x20,    //CCD表現檢測儀器故障
    };
    /// <summary>[enum]客戶格式參數</summary>    //eric chao 201
    enum AOI_CUSTOMERTYPE_
    {
        CUSTOMER_EMC_CCL = 0,         //台光CCL
        CUSTOMER_NANYA = 2,
        CUSTOMER_SYST_WEB_COPPER = 3, //生益軟板
        CUSTOMER_SYST_CCL = 4,        //生益CCL
        CUSTOMER_NANYA_WARPING = 5,   //南亞整經機
        CUSTOMER_SYST_PP = 6,         //生益 PP       //no use	for AOI_NEWUI_PP_20191121 branch
        CUSTOMER_SCRIBD_PP = 7,       //宏仁 PP       //no use	for AOI_NEWUI_PP_20191121 branch
        CUSTOMER_EMC_PP = 8,          //台光 PP      //no use	for AOI_NEWUI_PP_20191121 branch
        CUSTOMER_ITEQ = 9,            //聯茂
        CUSTOMER_JIANGXI_NANYA = 10,  //江西南亞CCL
        CUSTOMER_TUC_PP = 11,         //台耀 PP
        CUSTOMER_TG = 12,             //台玻
        CUSTOMER_YINGHUA = 13,        //盈華
        CUSTOMER_EVERSTRONG = 15,     //甬強         //23/10/19 修訂
        CUSTOMER_TECHAIN = 255,       //地謙        
# if _DEBUG
        CUSTOMER_TAG = 254,           //標籤機
#endif
    };
    /// <summary>[enum]客戶子場域格式參數</summary>
    enum AOI_SUBCUSTOMERTYPE_
    {
        SUB_CUSTOMER_NONE = 0,
        SUB_CUSTOMER_SYST_START = 1,
        SUB_CUSTOMER_DONGGUAN = SUB_CUSTOMER_SYST_START,        //東莞 (生益)
        SUB_CUSTOMER_JIUJIANG,                                  //九江 (生益)
        SUB_CUSTOMER_SUZHOU,                                    //蘇州 (生益)
        SUB_CUSTOMER_CHANGSHU,                                  //常熟 (生益)
        SUB_CUSTOMER_CHANGSHU2,                                 //常熟 (生益), A2/A4線 PLC Address全部加上 2000
        SUB_CUSTOMER_DONGGUAN_SONG8,                            //東莞松八 (生益)
        SUB_CUSTOMER_EMC_START = 5,                             //未來台光新增須把4~1代號依遞減方式補齊
        SUB_CUSTOMER_KUNSHAN = 5,                               //昆山 (台光)
        SUB_CUSTOMER_HUANGSHI,                                  //黃石 (台光)
        SUB_CUSTOMER_GUANYIN,                                   //觀音 (台光)
        SUB_CUSTOMER_ITEQ_START = 7,                            //未來聯茂新增須把6~1代號依遞減方式補齊
        SUB_CUSTOMER_WUXI = 7,                                  //無錫 (聯茂)
        SUB_CUSTOMER_NANYA_START = 1,
        SUB_CUSTOMER_NANYA_N4 = SUB_CUSTOMER_NANYA_START,       //江西南亞N4(N4和N5 ERP下發規格不同)
        SUB_CUSTOMER_NANYA_N5                                   //江西南亞N5
    };
    #endregion

    #region [struct]部屬系統參數
    /// <summary>[struct]OPC初始化參數</summary>
    public struct BATCH_SHARE_OPC_INITPARAM
    {
        string[] OPCIP;
        int RootIdNamespace;
        string[] ROOTID;
    }
    /// <summary>[struct]MC-Protocol初始化參數</summary>
    public struct BATCH_SHARE_SYST_INITPARAM
    {
        int CustomerType;
        int SubCustomerType;
        int Format;
        string[] PLCIP;
        int PLCPort;
        int FrameType;
    }
    /// <summary>[struct]MX初始化參數</summary>
    public struct BATCH_SHARE_MX_INITPARAM
    {
        long CPU;
        string[] PLCIP;
        uint StartAddress;
    }
    /// <summary>[struct]MX訊號狀態</summary>
    public struct BATCH_SHARE_MX_PINSTATUS
    {
        int Index0Base;
        bool HighLeve;
    }
    /// <summary>[BATCH=>EMC]初始化參數</summary>
    public struct BATCH_SHARE_EMC_INITPARAM
    {
        string[] EMCIP;
        int EMCPort;
        int ListenPort;
        int ProductType; //0:CCL, 1:PP
    }
    /// <summary>[struct]PLC_CCL初始化參數</summary>
    public struct BATCH_SHARE_SYSTCCL_INITPARAM
    {
        public string PLCIP;
        public long ConnectedStationNo;       //連接站側模組站號
        public long TargetNetworkNo;          //物件站側模組網路No
        public long TargetStationNo;          //物件站側模組站號
        public long PCNetworkNo;              //計算機側網路No
        public long PCStationNo;              //計算機側站號
    }
    /// <summary>[struct]PLC_PP初始化參數</summary>
    public struct BATCH_SHARE_SYSTPP_INITPARAM
    {
        public BATCH_SHARE_SYSTCCL_INITPARAM m_CCL_INITPARAM;
        //---------------------------------------------------------
        public int WatchDogTimeout;           //WatchDog timeout(second)
        public int Version;
        public int WSMode;                    //0:模式一/1:模式二
        public bool FX5U;
        public int NewbatchDelay;				//
    }
    /// <summary>[struct]系統工單額外項目</summary>
    public struct BATCH_SYST_EXTRA
    {
        DateTime Start;
        DateTime End;
        char[] Insp;
        char[] Light;
    }

    #endregion

    #region [struct]定義工單參數
    //----------- PLC[MX] -----------------------------
    /// <summary>[struct]基礎系統工單項目</summary>
    public struct BATCH_SHARE_SYST_BASE
    {
        public char[] Name;       //工單號
        public char[] Material;   //料號
    }
    /// <summary>[struct]CCL下發系統工單項目</summary>
    public struct BATCH_SHARE_SYST_PARAMCCL
    {
        public BATCH_SHARE_SYST_BASE m_BATCH_SHARE_SYST_BASE;

        public char[] Model;                        //模號
        public char[] Assign;                       //分發號
        public short AssignNum;                        //分發號數量
        public short SplitNum;                         //一開幾

        public float Split_One_X;                      //第一張大小板寬
        public float Split_One_Y;                      //第一張大小板長
        public float Split_Two_X;                      //第二張大小板寬
        public float Split_Two_Y;                      //第二張大小板長
        public float Split_Three_X;                    //第三張大小板寬
        public float Split_Three_Y;                    //第三張大小板長

        public float ThickSize;                        //板材厚度
        public float ThickCCL;                         //銅箔厚度
        public char[] CCLType;                         //銅箔特性
        public short GrayScale;                          //灰度值
        public short Pixel_AA;                         //點值要求AA
        public short Pixel_A;                          //點值要求A
        public short Pixel_P;                          //點值要求P

        public float Diff_One_Y;                       //第一個大小板公差(長)
        public float Diff_One_X;                       //第一個大小板公差(寬)
        public float Diff_One_XY;                      //第一個大小板公差(對角線)

        public float Diff_One_X_Min;                   //第一個大小版緯向公差下限
        public float Diff_One_X_Max;                   //第一個大小版緯向公差上限
        public float Diff_One_Y_Min;                   //第一個大小版經向公差下限
        public float Diff_One_Y_Max;                   //第一個大小版經向公差上限
        public float Diff_One_XY_Min;                  //第一個大小版對角線公差下限
        public float Diff_One_XY_Max;                  //第一個大小版對角線公差上限

        public float Diff_Two_X_Min;                   //第二個大小版緯向公差下限
        public float Diff_Two_X_Max;                   //第二個大小版緯向公差上限
        public float Diff_Two_Y_Min;                   //第二個大小版經向公差下限
        public float Diff_Two_Y_Max;                   //第二個大小版經向公差上限
        public float Diff_Two_XY_Min;                  //第二個大小版對角線公差下限
        public float Diff_Two_XY_Max;                  //第二個大小版對角線公差上限

        public float Diff_Three_X_Min;                 //第三個大小版緯向公差下限
        public float Diff_Three_X_Max;                 //第三個大小版緯向公差上限
        public float Diff_Three_Y_Min;                 //第三個大小版經向公差下限
        public float Diff_Three_Y_Max;                 //第三個大小版經向公差上限
        public float Diff_Three_XY_Min;                //第三個大小版對角線公差下限
        public float Diff_Three_XY_Max;                //第三個大小版對角線公差上限

        public short Num_AA;                           //小版AA級數量

        public short NO_C06;                           //C06小板剪切編號
        public short NO_C10;                           //C10小板剪切編號
        public short NO_C12;                           //C12小板剪切編號
    }
    /// <summary>[struct]CCL上傳系統工單項目</summary>
    public struct BATCH_SHARE_SYST_RESULTCCL
    {
        public float Real_One_Y;              //小板實際長度1
        public float Real_Two_Y;              //小板實際長度2
        public float Real_One_X;              //小板實際寬度1
        public float Real_Two_X;              //小板實際寬度2

        public short Diff_One_Y;               //小板長度實際公差1
        public short Diff_Two_Y;               //小板長度實際公差2
        public short Diff_One_X;               //小板寬度實際公差1
        public short Diff_Two_X;               //小板寬度實際公差2
        public short Diff_One_XY;              //小板對角線實際公差1
        public short Diff_Two_XY;              //小板對角線實際公差2

        public short FrontLevel;               //正面判斷級別
        public char[] FrontCode;            //[30]正面判斷代碼, 字串需要為2的倍數(回寫最小單位為WORD)
        public short BackLevel;                //反面判斷級別
        public char[] BackCode;             //[30]反面判斷代碼, 字串需要為2的倍數(回寫最小單位為WORD)
        public short Size_G10;                 //尺寸判斷級別(G10)
        public short Size_G12;                 //尺寸判斷級別(G12)
        public short Size_G14;                 //尺寸判斷級別(G14)

        public short ResultLevel;              //小版物料級別
        public short Num_AA;                   //小版AA級數量
        public short Num_A;                    //小版A級數量
        public short Num_P;                    //小版P級數量
        public short QualifyRate;              //訂單合格率
        public short Diff_XY;                  //級差實際檢測值
        public float[] FrontDefectSize;      //[5]九宮格中正面前五大缺陷大小1~5
        public float[] BackDefectSize;       //[5]九宮格中反面前五大缺陷大小1~5
        public short[] FrontDefectLocation;   //[5]九宮格中正面前五大缺陷位置1~5
        public short[] BackDefectLocation;    //[5]九宮格中反面前五大缺陷位置1~5
        public short Index;                    //小板編號, 僅東莞松八廠需回傳
    }
   
    /// <summary>[struct]甬強上傳系統工單項目</summary>
    public struct BATCH_SHARE_SYST_RESULT_EVERSTR
    {
        BATCH_SHARE_SYST_RESULTCCL m_BATCH_SHARE_SYST_RESULTCCL;

        char[] Name;
        char[] Assign;
        char[] Material;
    }
    //-----------  EMC   -----------------------------
    /// <summary>[struct]EMC基礎系統工單項目</summary>
    public struct BATCH_SHARE_EMC_BASE
    {
        string[] Station;        //設備號
        string[] MissionID;      //任務號
        string[] BatchName;      //工單號
        string[] Material;       //料號
        string[] Serial;         //批號
    }
    /// <summary>[struct]EMC_CCL下發系統工單項目</summary>
    public struct BATCH_SHARE_EMC_CCLPARAM
    {
        BATCH_SHARE_EMC_BASE m_BATCH_SHARE_EMC_BASE;

        int Num;                    //數量
        int BookNum;                //BOOK數
        int SheetNum;               //張數
        int Split;                  //一裁幾
        string[] Miss;              //少組信息：BOOK數-張數

        int Status;                 //任務狀態(CLEAR/START/CLOSED)
        string[] EmpID;             //員工編號
        int BeginBook;              //開始第幾BOOK
        int EndBook;                //結束第幾BOOK
        int BeginSheet;             //開始第幾張
        int EndSheet;               //結束第幾張
    }
    /// <summary>[struct]EMC_CCL下發項目</summary>
    public struct CCLParam
    {
        int Size;
        BATCH_SHARE_EMC_CCLPARAM[] Param;//[3]
    };
    /// <summary>[struct]EMC_CCL上傳系統工單項目</summary>
    public struct BATCH_SHARE_EMC_CCLRESULT
    {
        BATCH_SHARE_EMC_BASE m_BATCH_SHARE_EMC_BASE;
        int Index;                                     //檢測張數序號
        int BookNum;                                   //BOOK數
        string[] Sheet;                                //Sheet.Index
        string[] DefectType;                           //[3]缺點代碼
    }
    /// <summary>[struct]EMC_CCL擴充基礎系統工單項目</summary>
    public struct BATCH_SHARE_EMC_CCLEND
    {
        BATCH_SHARE_EMC_BASE m_BATCH_SHARE_EMC_BASE;
        int Index;                                     //檢測張數序號
    }
    /// <summary>[struct]EMC_PP下發系統工單項目</summary>
    public struct BATCH_SHARE_EMC_PPPARAM
    {
        BATCH_SHARE_EMC_BASE m_BATCH_SHARE_EMC_BASE;
        int Status;                                    //任務狀態(CLEAR/START/CLOSED)
        string[] EmpID;                //員工編號
    }
    /// <summary>[struct]EMC_PP上傳系統工單項目</summary>
    public struct BATCH_SHARE_EMC_PPRESULT
    {
        BATCH_SHARE_EMC_BASE m_BATCH_SHARE_EMC_BASE;
        float DefectBegin;                             //缺點開始米數
        float DefectEnd;                               //缺點結束米數
        string[] DefectType;                           //[3]缺點代碼
    }
    /// <summary>[struct]EMC_PP擴充基礎系統工單項目</summary>
    public struct BATCH_SHARE_EMC_PPEND
    {
        BATCH_SHARE_EMC_BASE m_BATCH_SHARE_EMC_BASE;
        float Length;                                  //每卷米數
    }
    /// <summary>[struct]EMC_錯誤系統工單項目</summary>
    public struct BATCH_SHARE_EMC_ERRORINFO_
    {
        EMC_ERROR ErrorType;                               //錯誤代碼
        string[] ErrorMsg;               //錯誤訊息
    }
    //-----------  INFO  -----------------------------
    /// <summary>[struct]資訊1系統工單項目</summary>
    public struct BATCH_SHARE_SYST_INFO1
    {
        public byte SizeReady;    //CCD尺寸檢測儀器準備好 1
        public byte SizeRunning;  //CCD尺寸檢測儀器運行 1
        public byte CCDReady;     //CCD表現檢測儀器準備好 1
        public byte CCDRunning;   //CCD表現檢測儀器運行 1
        public byte Reserve1;     //保留欄位 4
        public byte Reserve2;     //保留欄位
    }
    /// <summary>[struct]資訊2系統工單項目</summary>
    public struct BATCH_SHARE_SYST_INFO2
    {
        public byte CCDError1;    //CCD表現檢測儀器故障 1
        public byte SizeError1;   //CCD尺寸檢測儀器故障 1
        public byte Reserve1;     //保留欄位 6
        public byte Reserve2;         //保留欄位
    }
    /// <summary>[struct]資訊總系統工單項目</summary>
    public struct BATCH_SHARE_SYST_INFO
    {
        public BATCH_SHARE_SYST_INFO1 Info1;
        public BATCH_SHARE_SYST_INFO2 Info2;
    };
    #endregion

    //=================================================================
    //Interface
    //=================================================================
    public class DataHeadlerBase : AppLogProcess
    {
        //視窗相關
        /// <summary>尋找視窗</summary>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        /// <summary>顯示視窗</summary>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        /// <summary>獲取控制台窗口的句柄</summary>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
        //-------------------------------------------------------------------------------
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>使用互斥鎖</summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenMutex(uint dwDesiredAccess, bool bInheritHandle, string lpName);
        /// <summary>創建了一個互斥鎖</summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);
        /// <summary>打開一個檔案映射物件</summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, string lpName);
        /// <summary>創建一個檔案映射物件</summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);
        /// <summary>映射檔案</summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, UIntPtr dwNumberOfBytesToMap);
        /// <summary>解除映射的記憶體位址</summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
        //---------------------------------------------------------------------------------------

        //可以使用ShareMemory名稱清單
        public const string AOI_MASTER_NAME = "AOI Master";
        public const string EMC_COMMUNICATOR_NAME = "EMCCommunicator";
        public const string MX_COMMUNICATOR_NAME = "MXComponentCommunicator";
        public const string BATCH_COMMUNICATOR_MEM_ID = "BATCH_COMMUNICATOR_MEM";
        public const string BATCH_AOI_MEM_ID = "BATCH_AOI_MEM";
        public const string BATCH_AOI2EMC_MEM_ID = "BATCH_AOI2EMC_MEM";
        public const string BATCH_EMC2AOI_MEM_ID = "BATCH_EMC2AOI_MEM";
        public const string BATCH_AOI2MX_MEM_ID = "BATCH_AOI2MX_MEM";
        public const string BATCH_MX2AOI_MEM_ID = "BATCH_MX2AOI_MEM";
        public const string BATCH_SQL2AOI_MEM_ID = "SQL_AGENT_MEM";

        public const string BATCH_PLCIP = "192.168.2.99";
        //------------------------------------------------------------------
        private string m_strMemID;
#if !USE_IN_SLAVE
        private ShareMemory<byte> m_ShareMemory;//更換成C#版sharememory
#endif
        //------------------------------------------------------------------
        public DataHeadlerBase(string strMemID = "")
        {
            OpShareMemory((int)WM_Status.WM_CREATE);
        }
        ~DataHeadlerBase()
        {
            OpShareMemory((int)WM_Status.WM_DESTROY);
        }
        //------------------------------------------------------------------
        ///預先實體化
        //PLC
        public virtual void GetInitParam(BATCH_SHARE_SYST_INITPARAM pData) { }
        public virtual void SetInitParam(BATCH_SHARE_SYST_INITPARAM pData) { }
        //WebCopper
        public virtual void GetSYSTParam_WebCopper(BATCH_SHARE_SYST_BASE pData) { }
        public virtual void SetSYSTParam_WebCopper(BATCH_SHARE_SYST_BASE pData) { }
        //CCL
        public virtual void GetSYSTParam_CCL(BATCH_SHARE_SYST_PARAMCCL pData) { }
        public virtual void SetSYSYParam_CCL(BATCH_SHARE_SYST_PARAMCCL pData) { }

        //Info
        public virtual void SetSYSTInfo_CCL(ulong dwField, BATCH_SHARE_SYST_INFO xInfo) { }
        public virtual void GetSYSTInfo_CCL(BATCH_SHARE_SYST_INFO pInfo) { }
        
        //-------------------------------------------------------------------------------------
        public void NotifyResponse(string strTarget, IntPtr lparam)
        {
            IntPtr hWnd = FindWindow(null, strTarget);
            if (hWnd != IntPtr.Zero)
            {
                PostMessage(hWnd, (int)WM_APP_CMD.WM_GPIO_MSG, (IntPtr)WM_APP_CMD.WM_AOI_RESPONSE_CMD, lparam);
            }
        }
        public void WriteResponse(IntPtr pData, int nSize, string strTargetName, IntPtr wparam, IntPtr lparam) { }
        //----------------------------------------------------
        private void OpShareMemory(int nOpCode)
        {
            //初始化
            if (nOpCode == (int)WM_Status.WM_CREATE)
            {
                //若不存在
                if (!m_ShareMemory.Get_Status())
                {
                    if (m_strMemID.Length == 0)
                    {
                        m_ShareMemory = new ShareMemory<byte>(BATCH_COMMUNICATOR_MEM_ID);//預設
                    }
                    else
                    {
                        m_ShareMemory = new ShareMemory<byte>(m_strMemID);//客制名稱
                    }
                }
                //若存在
                else
                {
                    //目前暫時相同
                    if (m_strMemID.Length == 0)
                    {
                        m_ShareMemory = new ShareMemory<byte>(BATCH_COMMUNICATOR_MEM_ID);//預設
                    }
                    else
                    {
                        m_ShareMemory = new ShareMemory<byte>(m_strMemID);//客制名稱
                    }
                }
            }
            //處置
            else if (nOpCode == (int)WM_Status.WM_DESTROY)
            {
                //若名稱存在
                if (m_ShareMemory.Get_Status())
                {
                    //解構
                    m_ShareMemory.Dispose();
                }
            }
        }
        protected void GetSharedMemoryData(string strMemID, int size, ref byte[] pShare)
        {
            ShareMemory<byte> xShareMem = new ShareMemory<byte>(strMemID);
            pShare = xShareMem.ReadData(size);
        }
        protected void SetSharedMemoryData(byte[] writedata, int size, string strTargetName, IntPtr wparam, IntPtr lparam)
        {
            if (m_ShareMemory != null)
            {
                //寫入memory
                m_ShareMemory.WriteData(writedata, size);
                //通知視窗
                IntPtr hWnd = FindWindow(null, strTargetName);
                if (hWnd != IntPtr.Zero)
                {
                    PostMessage(hWnd, (int)WM_APP_CMD.WM_GPIO_MSG, wparam, lparam);
                }
            }
        }
        //=====================================================================================
        protected string MakeFloatLog(string strDes, float fData)
        {
            string strRtn;
            strRtn = $"{strDes}: {fData}\r\n";
            return strRtn;
        }
        protected string MakeWordLog(string strDes, uint wData)
        {
            string strRtn;
            strRtn = $"{strDes}: {wData}\r\n";
            return strRtn;
        }
        protected string MakestringLog(string strDes, string pData)
        {
            string strRtn;
            strRtn = $"{strDes}: {pData}\r\n";
            return strRtn;
        }
        protected string MakeByteLog(string strDes, byte cData)
        {
            string strRtn;
            strRtn = $"{strDes}: {cData}\r\n";
            return strRtn;
        }
        protected void NotifyAOI(IntPtr wparam, IntPtr lparam, string windowname = AOI_MASTER_NAME)
        {
            IntPtr hWnd = FindWindow(null, windowname);
            if (hWnd != IntPtr.Zero)
            {
                PostMessage(hWnd, (int)WM_APP_CMD.WM_GPIO_MSG, wparam, lparam);
            }
        }
    }
}


