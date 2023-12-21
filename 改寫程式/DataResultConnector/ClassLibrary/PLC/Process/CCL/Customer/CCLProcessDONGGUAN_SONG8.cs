using ActProgTypeLib;
using ClassLibrary;
using ClassLibrary.DataHeader;
using ClassLibrary.PLC.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.PLC.Process.CCL.Customer
{
    public interface ICCLProcessDONGGUAN_SONG8
    {
        PLC_DATA_ITEM GetPLCAddressInfo(int nFieldId, bool bSkip);
        void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL xData);
        void DoSetInfoField(ref BATCH_SHARE_SYST_INFO xInfo);
        void SetMXParam(IActProgType pParam, ref BATCH_SHARE_SYSTCCL_INITPARAM xData);
        bool IS_SUPPORT_FLOAT_REALSIZE();// { return FALSE; }; //東莞松八廠實際尺寸欄位型態為word, 非float
        emCPU_SERIES GetCPU();// { return CPU_SERIES::R_SERIES; }
        bool IS_SUPPORT_CUSTOM_ACTION();// { return TRUE; } //是否支援客製化行為
        void DoCustomAction(); //客製化行為
    }

    class CCLProcessDONGGUAN_SONG8 : CCLProcessBase, ICCLProcessDONGGUAN_SONG8
    {
#if _DEBUG
        const int ctBASE_ADDRESS = 0;
#else
        const int ctBASE_ADDRESS = 9000;
#endif
        private const int EOF = -1;

        PLC_DATA_ITEM[] m_PLC_FIELD_INFO;


        //---------------------------------------------------------------


        //---------------------------------------------------------------
        public CCLProcessDONGGUAN_SONG8()
        {
            m_PLC_FIELD_INFO = new PLC_DATA_ITEM[(int)PLC_FIELD.FIELD_MAX];
            for (int i = 0; i < (int)PLC_FIELD.FIELD_MAX; i++)
            {
                //複製實體表單資料
                m_PLC_FIELD_INFO[i] = new PLC_DATA_ITEM();
                m_PLC_FIELD_INFO[i] = SYST_PLC_FIELD[i];/////需要確定值是否有鍵入資料
            }
            INIT_PLCDATA();
        }
        public static readonly PLC_DATA_ITEM[] SYST_PLC_FIELD = new PLC_DATA_ITEM[]
        {
            new PLC_DATA_ITEM { FieldName = "BATCH",                          FieldType = (int)PLC_FIELD.FIELD_ORDER,                     ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 10,     DevType = "D",     Address =  ctBASE_ADDRESS + 0 },
            new PLC_DATA_ITEM { FieldName = "料號",                           FieldType = (int)PLC_FIELD.FIELD_MATERIAL,                  ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 18,     DevType = "D",     Address = ctBASE_ADDRESS + 6 },
            new PLC_DATA_ITEM { FieldName = "模號",                           FieldType = (int)PLC_FIELD.FIELD_MODEL,                     ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 10,     DevType = "D",     Address = ctBASE_ADDRESS + 43 },
            new PLC_DATA_ITEM { FieldName = "分發號",                         FieldType = (int)PLC_FIELD.FIELD_ASSIGN,                    ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 10,     DevType = "D",     Address = ctBASE_ADDRESS + 112 },
            new PLC_DATA_ITEM { FieldName = "分發號數量",                     FieldType = (int)PLC_FIELD.FIELD_ASSIGNNUM,                 ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 118 },
            new PLC_DATA_ITEM { FieldName = "一開幾數",                       FieldType = (int)PLC_FIELD.FIELD_SPLITNUM,                  ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 28 },
            new PLC_DATA_ITEM { FieldName = "第一張大小板長",                 FieldType = (int)PLC_FIELD.FIELD_SPLIT_ONE_Y,               ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 49 },
            new PLC_DATA_ITEM { FieldName = "第二張大小板長",                 FieldType = (int)PLC_FIELD.FIELD_SPLIT_TWO_Y,               ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第三張大小板長",                 FieldType = (int)PLC_FIELD.FIELD_SPLIT_THREE_Y,             ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第一張大小板寬",                 FieldType = (int)PLC_FIELD.FIELD_SPLIT_ONE_X,               ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 50 },
            new PLC_DATA_ITEM { FieldName = "第二張大小板寬",                 FieldType = (int)PLC_FIELD.FIELD_SPLIT_TWO_X,               ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第三張大小板寬",                 FieldType = (int)PLC_FIELD.FIELD_SPLIT_THREE_X,             ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "板材厚度",                       FieldType = (int)PLC_FIELD.FIELD_THICK_SIZE,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 55 },
            new PLC_DATA_ITEM { FieldName = "銅箔厚度",                       FieldType = (int)PLC_FIELD.FIELD_THICK_CCL,                 ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 56 },
            new PLC_DATA_ITEM { FieldName = "銅箔特性",                       FieldType = (int)PLC_FIELD.FIELD_CCL_TYPE,                  ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 58 },
            new PLC_DATA_ITEM { FieldName = "灰度值",                         FieldType = (int)PLC_FIELD.FIELD_CCL_GRAYSCALE,             ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 60 },
            new PLC_DATA_ITEM { FieldName = "點值要求(AA版點值)",             FieldType = (int)PLC_FIELD.FIELD_LEVEL_AA_PIXEL,            ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 61 },
            new PLC_DATA_ITEM { FieldName = "點值要求(A版點值)",              FieldType = (int)PLC_FIELD.FIELD_LEVEL_A_PIXEL,             ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 62 },
            new PLC_DATA_ITEM { FieldName = "點值要求(P版點值)",              FieldType = (int)PLC_FIELD.FIELD_LEVEL_P_PIXEL,             ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 63 },

            new PLC_DATA_ITEM { FieldName = "第一個大小板公差(長)",           FieldType = (int)PLC_FIELD.FIELD_DIFF_ONE_Y,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 64 },
            new PLC_DATA_ITEM { FieldName = "第一個大小板公差(寬)",           FieldType = (int)PLC_FIELD.FIELD_DIFF_ONE_X,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 65 },
            new PLC_DATA_ITEM { FieldName = "第一個大小板對角線公差",         FieldType = (int)PLC_FIELD.FIELD_DIFF_ONE_XY,               ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 66 },

            new PLC_DATA_ITEM { FieldName = "第一個大小版經向公差下限",       FieldType = (int)PLC_FIELD.FIELD_DIFF_ONE_Y_MIN,            ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第一個大小版經向公差上限",       FieldType = (int)PLC_FIELD.FIELD_DIFF_ONE_Y_MAX,            ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第一個大小版緯向公差下限",       FieldType = (int)PLC_FIELD.FIELD_DIFF_ONE_X_MIN,            ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第一個大小版緯向公差上限",       FieldType = (int)PLC_FIELD.FIELD_DIFF_ONE_X_MAX,            ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第一個大小版對角線公差下限",     FieldType = (int)PLC_FIELD.FIELD_DIFF_ONE_XY_MIN,           ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第一個大小版對角線公差上限",     FieldType = (int)PLC_FIELD.FIELD_DIFF_ONE_XY_MAX,           ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第二個大小版經向公差下限",       FieldType = (int)PLC_FIELD.FIELD_DIFF_TWO_Y_MIN,            ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第二個大小版經向公差上限",       FieldType = (int)PLC_FIELD.FIELD_DIFF_TWO_Y_MAX,            ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第二個大小版緯向公差下限",       FieldType = (int)PLC_FIELD.FIELD_DIFF_TWO_X_MIN,            ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第二個大小版緯向公差上限",       FieldType = (int)PLC_FIELD.FIELD_DIFF_TWO_X_MAX,            ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第二個大小版對角線公差下限",     FieldType = (int)PLC_FIELD.FIELD_DIFF_TWO_XY_MIN,           ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第二個大小版對角線公差上限",     FieldType = (int)PLC_FIELD.FIELD_DIFF_TWO_XY_MAX,           ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第三個大小版經向公差下限",       FieldType = (int)PLC_FIELD.FIELD_DIFF_THREE_Y_MIN,          ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第三個大小版經向公差上限",       FieldType = (int)PLC_FIELD.FIELD_DIFF_THREE_Y_MAX,          ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第三個大小版緯向公差下限",       FieldType = (int)PLC_FIELD.FIELD_DIFF_THREE_X_MIN,          ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第三個大小版緯向公差上限",       FieldType = (int)PLC_FIELD.FIELD_DIFF_THREE_X_MAX,          ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "第三個大小版對角線公差下限",     FieldType = (int)PLC_FIELD.FIELD_DIFF_THREE_XY_MIN,         ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "第三個大小版對角線公差上限",     FieldType = (int)PLC_FIELD.FIELD_DIFF_THREE_XY_MAX,         ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },

            new PLC_DATA_ITEM { FieldName = "小版AA級數量",                   FieldType = (int)PLC_FIELD.FIELD_AA_NUM,                    ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 85 },
            new PLC_DATA_ITEM { FieldName = "指令下發",                       FieldType = (int)PLC_FIELD.FIELD_CCL_COMMAND,               ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_NOTIFY,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 99 },
                                                                                                                                          
            new PLC_DATA_ITEM { FieldName = "C06小板剪切編號",                FieldType = (int)PLC_FIELD.FIELD_CCL_NO_C06,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "C10小板剪切編號",                FieldType = (int)PLC_FIELD.FIELD_CCL_NO_C10,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 294 },
            new PLC_DATA_ITEM { FieldName = "C12小板剪切編號",                FieldType = (int)PLC_FIELD.FIELD_CCL_NO_C12,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
                                                                                                                                                                                         
            new PLC_DATA_ITEM { FieldName = "小板實際長度1",                  FieldType = (int)PLC_FIELD.FIELD_REAL_Y_ONE,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 229 },
            new PLC_DATA_ITEM { FieldName = "小板實際長度2",                  FieldType = (int)PLC_FIELD.FIELD_REAL_Y_TWO,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 230 },
            new PLC_DATA_ITEM { FieldName = "小板實際寬度1",                  FieldType = (int)PLC_FIELD.FIELD_REAL_X_ONE,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 231 },
            new PLC_DATA_ITEM { FieldName = "小板實際寬度2",                  FieldType = (int)PLC_FIELD.FIELD_REAL_X_TWO,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 232 },
            new PLC_DATA_ITEM { FieldName = "小板長度實際公差1",              FieldType = (int)PLC_FIELD.FIELD_REAL_DIFF_ONE_Y,           ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 233 },
            new PLC_DATA_ITEM { FieldName = "小板長度實際公差2",              FieldType = (int)PLC_FIELD.FIELD_REAL_DIFF_TWO_Y,           ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 234 },
            new PLC_DATA_ITEM { FieldName = "小板寬度實際公差1",              FieldType = (int)PLC_FIELD.FIELD_REAL_DIFF_ONE_X,           ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 235 },
            new PLC_DATA_ITEM { FieldName = "小板寬度實際公差2",              FieldType = (int)PLC_FIELD.FIELD_REAL_DIFF_TWO_X,           ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 236 },
            new PLC_DATA_ITEM { FieldName = "小板對角線實際公差1",            FieldType = (int)PLC_FIELD.FIELD_REAL_DIFF_ONE_XY,          ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 237 },
            new PLC_DATA_ITEM { FieldName = "小板對角線實際公差2",            FieldType = (int)PLC_FIELD.FIELD_REAL_DIFF_TWO_XY,          ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 238 },

            new PLC_DATA_ITEM { FieldName = "正面判斷級別",                   FieldType = (int)PLC_FIELD.FIELD_FRONT_LEVEL,               ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 239 },
            new PLC_DATA_ITEM { FieldName = "正面判斷代碼",                   FieldType = (int)PLC_FIELD.FIELD_FRONT_CODE,                ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 40,     DevType = "D",     Address = ctBASE_ADDRESS + 240 },
            new PLC_DATA_ITEM { FieldName = "正面缺陷九宮格位置",             FieldType = (int)PLC_FIELD.FIELD_FRONT_LOCATION,            ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0/*Address = ctBASE_ADDRESS + 261*/ },
            new PLC_DATA_ITEM { FieldName = "反面判斷級別",                   FieldType = (int)PLC_FIELD.FIELD_BACK_LEVEL,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 262 },
            new PLC_DATA_ITEM { FieldName = "反面判斷代碼",                   FieldType = (int)PLC_FIELD.FIELD_BACK_CODE,                 ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 40,     DevType = "D",     Address = ctBASE_ADDRESS + 263 },
            new PLC_DATA_ITEM { FieldName = "反面缺陷九宮格位置",             FieldType = (int)PLC_FIELD.FIELD_BACK_LOCATION,             ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0/*Address = ctBASE_ADDRESS + 284*/ },
            new PLC_DATA_ITEM { FieldName = "尺寸判斷級別(G10)",              FieldType = (int)PLC_FIELD.FIELD_SIZE_G10,                  ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 285 },
            new PLC_DATA_ITEM { FieldName = "尺寸判斷級別(G12)",              FieldType = (int)PLC_FIELD.FIELD_SIZE_G12,                  ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 286 },
            new PLC_DATA_ITEM { FieldName = "尺寸判斷級別(G14)",              FieldType = (int)PLC_FIELD.FIELD_SIZE_G14,                  ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 287 },
            new PLC_DATA_ITEM { FieldName = "尺寸檢測備好",                   FieldType = (int)PLC_FIELD.FIELD_SIZE_INFO_1,               ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0  },
            new PLC_DATA_ITEM { FieldName = "尺寸檢測運行",                   FieldType = (int)PLC_FIELD.FIELD_SIZE_INFO_2,               ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0  },
            new PLC_DATA_ITEM { FieldName = "CCD準備好",                      FieldType = (int)PLC_FIELD.FIELD_CCD_INFO_1,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0  },
            new PLC_DATA_ITEM { FieldName = "CCD運行",                        FieldType = (int)PLC_FIELD.FIELD_CCD_INFO_2,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0  },
            new PLC_DATA_ITEM { FieldName = "CCD故障",                        FieldType = (int)PLC_FIELD.FIELD_CCD_ERROR_1,               ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0  },
            new PLC_DATA_ITEM { FieldName = "尺寸故障",                       FieldType = (int)PLC_FIELD.FIELD_SIZE_ERROR_1,              ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0  },

            new PLC_DATA_ITEM { FieldName = "指令收到",                       FieldType = (int)PLC_FIELD.FIELD_COMMAND_RECEIVED,          ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_NOTIFY,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 295 },
            new PLC_DATA_ITEM { FieldName = "檢驗結果",                       FieldType = (int)PLC_FIELD.FIELD_RESULT,                    ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_NOTIFY,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 296 },
            new PLC_DATA_ITEM { FieldName = "檢驗結果收到",                   FieldType = (int)PLC_FIELD.FIELD_RESULT_RECEIVED,           ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_NOTIFY,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 297 },
                                                                                                                                          
            new PLC_DATA_ITEM { FieldName = "小版物料級別",                   FieldType = (int)PLC_FIELD. FIELD_RESULT_LEVEL,             ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address =0 },
            new PLC_DATA_ITEM { FieldName = "小版AA級數量",                   FieldType = (int)PLC_FIELD.FIELD_RESULT_AA,                 ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 289 },
            new PLC_DATA_ITEM { FieldName = "小版A級數量",                    FieldType = (int)PLC_FIELD.FIELD_RESULT_A,                  ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 290 },
            new PLC_DATA_ITEM { FieldName = "小版P級數量",                    FieldType = (int)PLC_FIELD.FIELD_RESULT_P,                  ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 291 },
            new PLC_DATA_ITEM { FieldName = "訂單合格率",                     FieldType = (int)PLC_FIELD.  FIELD_RESULT_QUALIFYRATE,      ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_RESULT,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 292 },
            new PLC_DATA_ITEM { FieldName = "級差實際檢測值",                 FieldType = (int)PLC_FIELD.  FIELD_RESULT_DIFF_XY,          ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "通知MES",                        FieldType = (int)PLC_FIELD.  FIELD_RESULT_MES,              ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "通知MES工單資訊",                FieldType = (int)PLC_FIELD. FIELD_BATCH_MES,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "檢測設定",                       FieldType = (int)PLC_FIELD. FIELD_INSP_SETTING,             ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 5,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "光源設定",                       FieldType = (int)PLC_FIELD. FIELD_LIGHT_SETTING,            ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 5,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "檢測開始時間",                   FieldType = (int)PLC_FIELD. FIELD_START_TIME,               ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 9,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "檢測結束時間",                   FieldType = (int)PLC_FIELD. FIELD_END_TIME,                 ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 9,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "正面缺陷大小1",                  FieldType = (int)PLC_FIELD. FIELD_FRONT_DEFECT_SIZE_1,      ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "正面缺陷大小2",                  FieldType = (int)PLC_FIELD. FIELD_FRONT_DEFECT_SIZE_2,      ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "正面缺陷大小3",                  FieldType = (int)PLC_FIELD. FIELD_FRONT_DEFECT_SIZE_3,      ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "正面缺陷大小4",                  FieldType = (int)PLC_FIELD. FIELD_FRONT_DEFECT_SIZE_4,      ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "正面缺陷大小5",                  FieldType = (int)PLC_FIELD. FIELD_FRONT_DEFECT_SIZE_5,      ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "反面缺陷大小1",                  FieldType = (int)PLC_FIELD. FIELD_BACK_DEFECT_SIZE_1,       ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "反面缺陷大小2",                  FieldType = (int)PLC_FIELD. FIELD_BACK_DEFECT_SIZE_2,       ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "反面缺陷大小3",                  FieldType = (int)PLC_FIELD. FIELD_BACK_DEFECT_SIZE_3,       ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "反面缺陷大小4",                  FieldType = (int)PLC_FIELD. FIELD_BACK_DEFECT_SIZE_4,       ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "反面缺陷大小5",                  FieldType = (int)PLC_FIELD. FIELD_BACK_DEFECT_SIZE_5,       ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "正面缺陷位置1",                  FieldType = (int)PLC_FIELD. FIELD_FRONT_DEFECT_LOCATION_1,  ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "正面缺陷位置2",                  FieldType = (int)PLC_FIELD. FIELD_FRONT_DEFECT_LOCATION_2,  ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "正面缺陷位置3",                  FieldType = (int)PLC_FIELD. FIELD_FRONT_DEFECT_LOCATION_3,  ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "正面缺陷位置4",                  FieldType = (int)PLC_FIELD. FIELD_FRONT_DEFECT_LOCATION_4,  ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "正面缺陷位置5",                  FieldType = (int)PLC_FIELD. FIELD_FRONT_DEFECT_LOCATION_5,  ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "反面缺陷位置1",                  FieldType = (int)PLC_FIELD. FIELD_BACK_DEFECT_LOCATION_1,   ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "反面缺陷位置2",                  FieldType = (int)PLC_FIELD. FIELD_BACK_DEFECT_LOCATION_2,   ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "反面缺陷位置3",                  FieldType = (int)PLC_FIELD. FIELD_BACK_DEFECT_LOCATION_3,   ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "反面缺陷位置4",                  FieldType = (int)PLC_FIELD. FIELD_BACK_DEFECT_LOCATION_4,   ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
            new PLC_DATA_ITEM { FieldName = "反面缺陷位置5",                  FieldType = (int)PLC_FIELD. FIELD_BACK_DEFECT_LOCATION_5,   ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_SKIP,     DataLength = 2,      DevType = "D",     Address = 0 },
                                                                              
            new PLC_DATA_ITEM { FieldName = "CCD(尺寸)掃碼訂單號",            FieldType = (int)PLC_FIELD. FIELD_CUTTER_ORDER,             ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE. ACTION_BATCH,   DataLength = 10,     DevType = "D",     Address = ctBASE_ADDRESS + 150 },
            new PLC_DATA_ITEM { FieldName = "CCD(尺寸)掃碼分發號",            FieldType = (int)PLC_FIELD. FIELD_CUTTER_ASSIGN,            ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE. ACTION_BATCH,   DataLength = 10,     DevType = "D",     Address = ctBASE_ADDRESS + 156 },
            new PLC_DATA_ITEM { FieldName = "CCD(尺寸)長度",                  FieldType = (int)PLC_FIELD.  FIELD_CUTTER_Y,                ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE. ACTION_BATCH,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 162 },
            new PLC_DATA_ITEM { FieldName = "CCD(尺寸)寬度",                  FieldType = (int)PLC_FIELD. FIELD_CUTTER_X,                 ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE. ACTION_BATCH,   DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 164 },
            new PLC_DATA_ITEM { FieldName = "CCD(尺寸)版編號",                FieldType = (int)PLC_FIELD.   FIELD_CUTTER_INDEX,           ValType = emPLC_DEVICE_TYPE.TYPE_DWORD,       Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 4,      DevType = "D",     Address = ctBASE_ADDRESS + 166 },
            new PLC_DATA_ITEM { FieldName = "測厚掃碼訂單號",                 FieldType = (int)PLC_FIELD.  FIELD_CUTTER_RETURN_ORDER,     ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 10,     DevType = "D",     Address = ctBASE_ADDRESS + 350 },
            new PLC_DATA_ITEM { FieldName = "測厚掃碼分發號",                 FieldType = (int)PLC_FIELD.  FIELD_CUTTER_RETURN_ASSIGN,    ValType = emPLC_DEVICE_TYPE.TYPE_STRING,      Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 10,     DevType = "D",     Address = ctBASE_ADDRESS + 356 },
            new PLC_DATA_ITEM { FieldName = "測厚長度",                       FieldType = (int)PLC_FIELD. FIELD_CUTTER_RETURN_Y,          ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 362 },
            new PLC_DATA_ITEM { FieldName = "測厚寬度",                       FieldType = (int)PLC_FIELD. FIELD_CUTTER_RETURN_X,          ValType = emPLC_DEVICE_TYPE.TYPE_WORD,        Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 2,      DevType = "D",     Address = ctBASE_ADDRESS + 364 },
            new PLC_DATA_ITEM { FieldName = "測厚版編號",                     FieldType = (int)PLC_FIELD. FIELD_CUTTER_RETURN_INDEX,      ValType = emPLC_DEVICE_TYPE.TYPE_DWORD,       Action = emPLC_ACTION_TYPE.ACTION_BATCH,    DataLength = 4,      DevType = "D",     Address = ctBASE_ADDRESS + 366 },
        };
        //---------------------------------------------------------------
        public override PLC_DATA_ITEM GetPLCAddressInfo(int nFieldId, bool bSkip)
        {
            if (nFieldId >= 0 && nFieldId <= (int)PLC_FIELD.FIELD_MAX) //範圍內
            {
                return m_PLC_FIELD_INFO[nFieldId];
            }
            return null;
        }
        public override bool IS_SUPPORT_CUSTOM_ACTION()
        {
            return true;
        }
        public override bool IS_SUPPORT_FLOAT_REALSIZE()
        {
            return false;
        }
        public override emCPU_SERIES GetCPU()
        {
            return emCPU_SERIES.R_SERIES;
        }
        public override void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL xData)
        {
            SET_PLC_FIELD_DATA<float>((int)PLC_FIELD.FIELD_REAL_Y_ONE, 2, xData.Real_One_Y);
            SET_PLC_FIELD_DATA<float>((int)PLC_FIELD.FIELD_REAL_Y_TWO, 2, xData.Real_Two_Y);
            SET_PLC_FIELD_DATA<float>((int)PLC_FIELD.FIELD_REAL_X_ONE, 2, xData.Real_One_X);
            SET_PLC_FIELD_DATA<float>((int)PLC_FIELD.FIELD_REAL_X_TWO, 2, xData.Real_Two_X);

            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_SIZE_G10, 2, xData.Size_G10);
            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_SIZE_G12, 2, xData.Size_G12);

            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_RESULT_AA, 2, xData.Num_AA);
            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_RESULT_A, 2, xData.Num_A);
            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_RESULT_P, 2, xData.Num_P);
            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_RESULT_QUALIFYRATE, 2, xData.QualifyRate);

            SET_PLC_FIELD_DATA<short>((int)PLC_FIELD.FIELD_CCL_NO_C10, 2, xData.Index);
        }

        //---------------------------------------------------------------
        public override void DoCustomAction()
        {
            int dwOldIndex = GET_PLC_FIELD_VALUE<int>((int)PLC_FIELD.FIELD_CUTTER_INDEX);//讀原值
            REFLASH_PLC_FIELD_DATA((int)PLC_FIELD.FIELD_CUTTER_INDEX);//更新
            int dwNewIndex = GET_PLC_FIELD_VALUE<int>((int)PLC_FIELD.FIELD_CUTTER_INDEX);//讀新值
            if (dwOldIndex != dwNewIndex)
            {
                List<int> vField = new List<int> { (int)PLC_FIELD.FIELD_CUTTER_ORDER, (int)PLC_FIELD.FIELD_CUTTER_ASSIGN, (int)PLC_FIELD.FIELD_CUTTER_Y, (int)PLC_FIELD.FIELD_CUTTER_X, (int)PLC_FIELD.FIELD_CUTTER_INDEX };

                REFLASH_PLC_FIELD_DATA(vField);
#if false  //SET_PLC_FIELD_DATA(RANDOM) does not update UI
		vector<int> vWriteField = { FIELD_CUTTER_RETURN_ORDER, FIELD_CUTTER_RETURN_ASSIGN, FIELD_CUTTER_RETURN_Y, FIELD_CUTTER_RETURN_X, FIELD_CUTTER_RETURN_INDEX };

		int nFieldByte = 0;
		for (auto& i : vField) {
			PLC_DATA_ITEM_* pField = GetPLCAddressInfo(i, FALSE);
			if (pField)
				nFieldByte += pField->cLen;
		}
		if (nFieldByte > 0) {
			BYTE* pData = new BYTE[nFieldByte];
			memset(pData, 0, nFieldByte);
			BYTE* pCur = pData;
			for (auto& i : vField) {
				PLC_DATA_ITEM_* pField = GetPLCAddressInfo(i, FALSE);
				if (pField) {
					BYTE* pValue = GET_PLC_FIELD_BYTE_VALUE(i);
					memcpy(pCur, pValue, pField->cLen);

					pCur += pField->cLen;
				}
			}
			SET_PLC_FIELD_DATA(vWriteField, pData);
			delete pData;
			pData = NULL;
	}
#else
                foreach (var i in vField)
                {
                    byte[] pValue = GET_PLC_FIELD_VALUE<byte[]>(i);
                    int nWriteField = EOF;
                    switch (i)
                    {
                        case (int)PLC_FIELD.FIELD_CUTTER_ORDER:
                            nWriteField = (int)PLC_FIELD.FIELD_CUTTER_RETURN_ORDER;
                            break;
                        case (int)PLC_FIELD.FIELD_CUTTER_ASSIGN:
                            nWriteField = (int)PLC_FIELD.FIELD_CUTTER_RETURN_ASSIGN;
                            break;
                        case (int)PLC_FIELD.FIELD_CUTTER_Y:
                            nWriteField = (int)PLC_FIELD.FIELD_CUTTER_RETURN_Y;
                            break;
                        case (int)PLC_FIELD.FIELD_CUTTER_X:
                            nWriteField = (int)PLC_FIELD.FIELD_CUTTER_RETURN_X;
                            break;
                        case (int)PLC_FIELD.FIELD_CUTTER_INDEX:
                            nWriteField = (int)PLC_FIELD.FIELD_CUTTER_RETURN_INDEX;
                            break;
                    }
                    if (nWriteField != EOF)//有輸出
                    {
                        PLC_DATA_ITEM pField = GetPLCAddressInfo(nWriteField, false);
                        if (pField != null)
                        {
                            SET_PLC_FIELD_DATA<byte[]>(nWriteField, pField.DataLength, pValue);
                        }
                    }
                }
#endif
            }
        }
        public void DoSetInfoField(ref BATCH_SHARE_SYST_INFO xInfo)
        {
            //SET_PLC_FIELD_DATA_BIT(FIELD_SIZE_INFO_1, FIELD_CCD_INFO_2, 2, (BYTE*)&xInfo.xInfo1);
            //SET_PLC_FIELD_DATA_BIT(FIELD_CCD_ERROR_1, FIELD_SIZE_ERROR_1, 2, (BYTE*)&xInfo.xInfo2);
        }

        //---------------------------------------------------------------
        public virtual void SetMXParam(IActProgType pParam, ref BATCH_SHARE_SYSTCCL_INITPARAM xData)
        {
#if _DEBUG
            pParam.ActBaudRate = 0x00;
            pParam.ActControl = 0x00;
            pParam.ActCpuType = (int)PLC_CPU_CODE.CPU_FX5UCPU;
            pParam.ActDataBits = 0x00;
            pParam.ActDestinationIONumber = 0x00;
            pParam.ActDestinationPortNumber = 5562;
            pParam.ActDidPropertyBit = 0x01;
            pParam.ActDsidPropertyBit = 0x01;
            pParam.ActIntelligentPreferenceBit = 0x00;
            pParam.ActIONumber = 0x3FF;
            pParam.ActNetworkNumber = 0x00;
            pParam.ActPacketType = 0x01;
            pParam.ActPortNumber = 0x00;
            pParam.ActProtocolType = (int)PLC_CPU_CODE.PROTOCOL_TCPIP;
            pParam.ActStationNumber = 0xFF;
            pParam.ActStopBits = 0x00;
            pParam.ActSumCheck = 0x00;
            pParam.ActThroughNetworkType = 0x01;
            pParam.ActTimeOut = 0x100;                          //100ms timeout
            pParam.ActUnitNumber = 0x00;
            pParam.ActUnitType = (int)PLC_CPU_CODE.UNIT_FXVETHER;
#else
			//參考MX_componentV4_Program Manaual 4.3.7設定
			pParam.ActCpuType = (int)PLC_CPU_CODE.CPU_R16CPU;
			pParam.ActConnectUnitNumber = 0x00;
			pParam.ActDestinationIONumber = 0x00;               //固定為0
			pParam.ActDestinationPortNumber = 5007;             //固定為5007
			pParam.ActDidPropertyBit = 0x01;                    //固定為1
			pParam.ActDsidPropertyBit = 0x01;                   //固定為1
			pParam.ActIntelligentPreferenceBit = 0x00;          //固定為0
			pParam.ActIONumber = 0x3FF;                         //單CPU時, 固定為0x3FF
			pParam.ActMultiDropChannelNumber = 0x00;            //固定為0
			pParam.ActNetworkNumber = (int)xData.TargetNetworkNo;   //物件站側模組網路No, 0
			pParam.ActStationNumber = (int)xData.TargetStationNo;   //物件站側模組站號, 0xFF
			pParam.ActThroughNetworkType = 0x00;
			pParam.ActTimeOut = 0x100;                          //100ms timeout
			pParam.ActUnitNumber = 0x00;                        //固定為0
			pParam.ActUnitType = (int)PLC_CPU_CODE.UNIT_RETHER;
#endif
        }
        //---------------------------------------------------------------

    }
}
