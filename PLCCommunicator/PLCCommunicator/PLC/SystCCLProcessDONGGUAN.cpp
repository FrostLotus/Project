#include "stdafx.h"
#include "SystCCLProcessDONGGUAN.h"

CSystCCLProcessDONGGUAN::CSystCCLProcessDONGGUAN()
{
#ifdef OFF_LINE
	const int ctBASE_ADDRESS = 10000;
#else
#ifdef _DEBUG
		const int ctBASE_ADDRESS = 10000;
#else
		const int ctBASE_ADDRESS = 16000;
#endif
#endif
		const PLC_DATA_ITEM_ ctSYST_PLC_FIELD[FIELD_MAX] = {
			{ L"工單",				FIELD_ORDER,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 1 },
			{ L"料號",				FIELD_MATERIAL,				PLC_TYPE_STRING,	ACTION_BATCH,		18,		L"D",		ctBASE_ADDRESS + 7 },
			{ L"模號",				FIELD_MODEL,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 46 },
			{ L"分發號",			FIELD_ASSIGN,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 71 },
			{ L"分發號數量",		FIELD_ASSIGNNUM,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 76 },
			{ L"一開幾數",			FIELD_SPLITNUM,				PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 77 },
			{ L"第一張大小板長",	FIELD_SPLIT_ONE_Y,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 78 },
			{ L"第二張大小板長",	FIELD_SPLIT_TWO_Y,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第三張大小板長",	FIELD_SPLIT_THREE_Y,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第一張大小板寬",	FIELD_SPLIT_ONE_X,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 79 },
			{ L"第二張大小板寬",	FIELD_SPLIT_TWO_X,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第三張大小板寬",	FIELD_SPLIT_THREE_X,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"板材厚度",			FIELD_THICK_SIZE,			PLC_TYPE_FLOAT,		ACTION_BATCH,		4,		L"D",		ctBASE_ADDRESS + 80 },
			{ L"銅箔厚度",			FIELD_THICK_CCL,			PLC_TYPE_FLOAT,		ACTION_BATCH,		4,		L"D",		ctBASE_ADDRESS + 82 },
			{ L"銅箔特性",			FIELD_CCL_TYPE,				PLC_TYPE_STRING,	ACTION_BATCH,		6,		L"D",		ctBASE_ADDRESS + 85 },  
			{ L"灰度值",			FIELD_CCL_GRAYSCALE,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 90 },
			{ L"點值要求(AA版點值)",FIELD_LEVEL_AA_PIXEL,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 91 },
			{ L"點值要求(A版點值)",	FIELD_LEVEL_A_PIXEL,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 92 },
			{ L"點值要求(P版點值)",	FIELD_LEVEL_P_PIXEL,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 93 },
																													
			{ L"第一個大小板公差(長)",		FIELD_DIFF_ONE_Y,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 100 },
			{ L"第一個大小板公差(寬)",		FIELD_DIFF_ONE_X,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 101 },
			{ L"第一個大小板對角線公差",	FIELD_DIFF_ONE_XY,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 102 },

			{ L"第一個大小版經向公差下限",	FIELD_DIFF_ONE_Y_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第一個大小版經向公差上限",	FIELD_DIFF_ONE_Y_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第一個大小版緯向公差下限",	FIELD_DIFF_ONE_X_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第一個大小版緯向公差上限",	FIELD_DIFF_ONE_X_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第一個大小版對角線公差下限",FIELD_DIFF_ONE_XY_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第一個大小版對角線公差上限",FIELD_DIFF_ONE_XY_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第二個大小版經向公差下限",	FIELD_DIFF_TWO_Y_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第二個大小版經向公差上限",	FIELD_DIFF_TWO_Y_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第二個大小版緯向公差下限",	FIELD_DIFF_TWO_X_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第二個大小版緯向公差上限",	FIELD_DIFF_TWO_X_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第二個大小版對角線公差下限",FIELD_DIFF_TWO_XY_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第二個大小版對角線公差上限",FIELD_DIFF_TWO_XY_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第三個大小版經向公差下限",	FIELD_DIFF_THREE_Y_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第三個大小版經向公差上限",	FIELD_DIFF_THREE_Y_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第三個大小版緯向公差下限",	FIELD_DIFF_THREE_X_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第三個大小版緯向公差上限",	FIELD_DIFF_THREE_X_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第三個大小版對角線公差下限",FIELD_DIFF_THREE_XY_MIN,PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"第三個大小版對角線公差上限",FIELD_DIFF_THREE_XY_MAX,PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },

			{ L"小版AA級數量",		FIELD_AA_NUM,				PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 605 },
			{ L"指令下發",			FIELD_CCL_COMMAND,			PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 600 },

			{ L"C06小板剪切編號",	FIELD_CCL_NO_C06,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 200 },
			{ L"C10小板剪切編號",	FIELD_CCL_NO_C10,			PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 201 },
			{ L"C12小板剪切編號",	FIELD_CCL_NO_C12,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 202 },
		
			{ L"小板實際長度1",		FIELD_REAL_Y_ONE,				PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 324 },
			{ L"小板實際長度2",		FIELD_REAL_Y_TWO,				PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 326 },
			{ L"小板實際寬度1",		FIELD_REAL_X_ONE,				PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 328 },
			{ L"小板實際寬度2",		FIELD_REAL_X_TWO,				PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 330 },
			{ L"小板長度實際公差1", FIELD_REAL_DIFF_ONE_Y,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 332 },
			{ L"小板長度實際公差2", FIELD_REAL_DIFF_TWO_Y,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 334 },
			{ L"小板寬度實際公差1", FIELD_REAL_DIFF_ONE_X,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 336 },
			{ L"小板寬度實際公差2", FIELD_REAL_DIFF_TWO_X,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 338 },
			{ L"小板對角線實際公差1", FIELD_REAL_DIFF_ONE_XY,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 340 },
			{ L"小板對角線實際公差2", FIELD_REAL_DIFF_TWO_XY,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 342 },
		
			{ L"正面判斷級別",		FIELD_FRONT_LEVEL,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 254 },
			{ L"正面判斷代碼",		FIELD_FRONT_CODE,			PLC_TYPE_STRING,	ACTION_RESULT,		30,		L"D",		ctBASE_ADDRESS + 256 },
			{ L"正面缺陷九宮格位置",FIELD_FRONT_LOCATION,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 276 },
			{ L"反面判斷級別",		FIELD_BACK_LEVEL,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 277 },
			{ L"反面判斷代碼",		FIELD_BACK_CODE,			PLC_TYPE_STRING,	ACTION_RESULT,		30,		L"D",		ctBASE_ADDRESS + 279 },
			{ L"反面缺陷九宮格位置",FIELD_BACK_LOCATION,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 299 },
			{ L"尺寸判斷級別(G10)", FIELD_SIZE_G10,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 321 },
			{ L"尺寸判斷級別(G12)", FIELD_SIZE_G12,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 322 },
			{ L"尺寸判斷級別(G14)",	FIELD_SIZE_G14,				PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"尺寸檢測備好",		FIELD_SIZE_INFO_1,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 533, 0, 0  },
			{ L"尺寸檢測運行",		FIELD_SIZE_INFO_2,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 533, 1, 1  },
			{ L"CCD準備好",			FIELD_CCD_INFO_1,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 533, 2, 2  },
			{ L"CCD運行",			FIELD_CCD_INFO_2,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 533, 3, 3  },
			{ L"CCD故障",			FIELD_CCD_ERROR_1,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 546, 0, 0  },
			{ L"尺寸故障",			FIELD_SIZE_ERROR_1,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 546, 1, 1  },

			{ L"指令收到",			FIELD_COMMAND_RECEIVED,		PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 601 },
			{ L"檢驗結果",			FIELD_RESULT,				PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 602 },
			{ L"檢驗結果收到",		FIELD_RESULT_RECEIVED,		PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 603 },

			{ L"小版物料級別",		FIELD_RESULT_LEVEL,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"小版AA級數量",		FIELD_RESULT_AA,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 621 },
			{ L"小版A級數量",		FIELD_RESULT_A,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 622 },
			{ L"小版P級數量",		FIELD_RESULT_P,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 623 },
			{ L"訂單合格率",		FIELD_RESULT_QUALIFYRATE,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 624 },
			{ L"級差實際檢測值",	FIELD_RESULT_DIFF_XY,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"通知MES",			FIELD_RESULT_MES,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"通知MES工單資訊",	FIELD_BATCH_MES,			PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		0 },
			{ L"檢測設定",			FIELD_INSP_SETTING,			PLC_TYPE_STRING,	ACTION_RESULT,		5,		L"D",		0 },
			{ L"光源設定",			FIELD_LIGHT_SETTING,		PLC_TYPE_STRING,	ACTION_RESULT,		5,		L"D",		0 },
			{ L"檢測開始時間",		FIELD_START_TIME,			PLC_TYPE_STRING,	ACTION_RESULT,		9,		L"D",		0 },
			{ L"檢測結束時間",		FIELD_END_TIME,				PLC_TYPE_STRING,	ACTION_RESULT,		9,		L"D",		0 },
			{ L"正面缺陷大小1",		FIELD_FRONT_DEFECT_SIZE_1,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"正面缺陷大小2",		FIELD_FRONT_DEFECT_SIZE_2,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"正面缺陷大小3",		FIELD_FRONT_DEFECT_SIZE_3,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"正面缺陷大小4",		FIELD_FRONT_DEFECT_SIZE_4,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"正面缺陷大小5",		FIELD_FRONT_DEFECT_SIZE_5,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"反面缺陷大小1",		FIELD_BACK_DEFECT_SIZE_1,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"反面缺陷大小2",		FIELD_BACK_DEFECT_SIZE_2,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"反面缺陷大小3",		FIELD_BACK_DEFECT_SIZE_3,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"反面缺陷大小4",		FIELD_BACK_DEFECT_SIZE_4,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"反面缺陷大小5",		FIELD_BACK_DEFECT_SIZE_5,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"正面缺陷位置1",		FIELD_FRONT_DEFECT_LOCATION_1,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"正面缺陷位置2",		FIELD_FRONT_DEFECT_LOCATION_2,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"正面缺陷位置3",		FIELD_FRONT_DEFECT_LOCATION_3,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"正面缺陷位置4",		FIELD_FRONT_DEFECT_LOCATION_4,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"正面缺陷位置5",		FIELD_FRONT_DEFECT_LOCATION_5,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"反面缺陷位置1",		FIELD_BACK_DEFECT_LOCATION_1,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"反面缺陷位置2",		FIELD_BACK_DEFECT_LOCATION_2,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"反面缺陷位置3",		FIELD_BACK_DEFECT_LOCATION_3,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"反面缺陷位置4",		FIELD_BACK_DEFECT_LOCATION_4,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"反面缺陷位置5",		FIELD_BACK_DEFECT_LOCATION_5,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"CCD(尺寸)掃碼訂單號",		FIELD_CUTTER_ORDER,				PLC_TYPE_STRING,	ACTION_SKIP,		10,		L"D",		0 },
			{ L"CCD(尺寸)掃碼分發號",		FIELD_CUTTER_ASSIGN,			PLC_TYPE_STRING,	ACTION_SKIP,		10,		L"D",		0 },
			{ L"CCD(尺寸)長度",		FIELD_CUTTER_Y,				PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"CCD(尺寸)寬度",		FIELD_CUTTER_X,				PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"CCD(尺寸)版編號",	FIELD_CUTTER_INDEX,			PLC_TYPE_DWORD,		ACTION_SKIP,		4,		L"D",		0 },
			{ L"測厚掃碼訂單號",	FIELD_CUTTER_RETURN_ORDER,	PLC_TYPE_STRING,	ACTION_SKIP,		10,		L"D",		0 },
			{ L"測厚掃碼分發號",	FIELD_CUTTER_RETURN_ASSIGN,	PLC_TYPE_STRING,	ACTION_SKIP,		10,		L"D",		0 },
			{ L"測厚長度",			FIELD_CUTTER_RETURN_Y,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"測厚寬度",			FIELD_CUTTER_RETURN_X,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"測厚版編號",		FIELD_CUTTER_RETURN_INDEX,	PLC_TYPE_DWORD,		ACTION_SKIP,		4,		L"D",		0 },

		};
		m_pPLC_FIELD_INFO = new PLC_DATA_ITEM_*[FIELD_MAX];
		for (int i = 0; i < FIELD_MAX; i++){ 
			m_pPLC_FIELD_INFO[i] = new PLC_DATA_ITEM;
			memset(m_pPLC_FIELD_INFO[i], 0, sizeof(PLC_DATA_ITEM));

			memcpy(m_pPLC_FIELD_INFO[i], &ctSYST_PLC_FIELD[i], sizeof(PLC_DATA_ITEM));
		}
		INIT_PLCDATA();
}
CSystCCLProcessDONGGUAN::~CSystCCLProcessDONGGUAN()
{
	DESTROY_PLC_DATA();
	int nFieldSize = GetFieldSize();
	if (m_pPLC_FIELD_INFO){
		for (int i = 0; i < nFieldSize; i++){
			if (m_pPLC_FIELD_INFO[i]){
				delete[] m_pPLC_FIELD_INFO[i];
				m_pPLC_FIELD_INFO[i] = NULL;
			}
		}
		delete[] m_pPLC_FIELD_INFO;
	}
}
PLC_DATA_ITEM_* CSystCCLProcessDONGGUAN::GetPLCAddressInfo(int nFieldId, BOOL bSkip)
{
	if (nFieldId >= 0 && nFieldId <= FIELD_MAX){
		return m_pPLC_FIELD_INFO[nFieldId];
	}
	return NULL;
}
void CSystCCLProcessDONGGUAN::DoWriteResult(BATCH_SHARE_SYST_RESULTCCL &xData)
{
	SET_PLC_FIELD_DATA(FIELD_SIZE_G10, 2, (BYTE*)&xData.wSize_G10);
	SET_PLC_FIELD_DATA(FIELD_SIZE_G12, 2, (BYTE*)&xData.wSize_G12);

	SET_PLC_FIELD_DATA(FIELD_RESULT_AA, 2, (BYTE*)&xData.wNum_AA);
	SET_PLC_FIELD_DATA(FIELD_RESULT_A, 2, (BYTE*)&xData.wNum_A);
	SET_PLC_FIELD_DATA(FIELD_RESULT_P, 2, (BYTE*)&xData.wNum_P);
	SET_PLC_FIELD_DATA(FIELD_RESULT_QUALIFYRATE, 2, (BYTE*)&xData.wQualifyRate);
}
void CSystCCLProcessDONGGUAN::DoSetInfoField(BATCH_SHARE_SYST_INFO &xInfo)
{
	SET_PLC_FIELD_DATA_BIT(FIELD_SIZE_INFO_1, FIELD_CCD_INFO_2, 2, (BYTE*)&xInfo.xInfo1);
	SET_PLC_FIELD_DATA_BIT(FIELD_CCD_ERROR_1, FIELD_SIZE_ERROR_1, 2, (BYTE*)&xInfo.xInfo2);
}

void CSystCCLProcessDONGGUAN::SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData)
{
	//參考MX_componentV4_Program Manaual 4.3.7設定
	pParam->put_ActCpuType(CPU_Q13UDEHCPU);
	pParam->put_ActConnectUnitNumber(0x00);
	pParam->put_ActDestinationIONumber(0x00);				//固定為0
	pParam->put_ActDestinationPortNumber(5007);				//固定為5007
	pParam->put_ActDidPropertyBit(0x01);					//固定為1
	pParam->put_ActDsidPropertyBit(0x01);					//固定為1
	pParam->put_ActIntelligentPreferenceBit(0x00);			//固定為0
	pParam->put_ActIONumber(0x3FF);							//單CPU時, 固定為0x3FF
	pParam->put_ActMultiDropChannelNumber(0x00);			//固定為0
	pParam->put_ActNetworkNumber(xData.lTargetNetworkNo);	//物件站側模組網路No, 0
	pParam->put_ActStationNumber(xData.lTargetStationNo);	//物件站側模組站號, 0xFF
	pParam->put_ActThroughNetworkType(0x00);
	pParam->put_ActTimeOut(0x100);							//100ms timeout
	pParam->put_ActUnitNumber(0x00);						//固定為0
	pParam->put_ActUnitType(UNIT_QNETHER);
}