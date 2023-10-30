#include "stdafx.h"
#include "EverStrProcess.h"
//================================================================
///<summary>[Constructor]初始化</summary>
CEverStrProcess::CEverStrProcess()
{
#ifdef OFF_LINE
	const int ctBASE_ADDRESS = 1000;
#else
#ifdef _DEBUG
	const int ctBASE_ADDRESS = 1000;
#else
	const int ctBASE_ADDRESS = 1000;
#endif
#endif
	const PLC_DATA_ITEM_ ctSYST_PLC_FIELD[FIELD_MAX] =
	{
		{ L"訂單號",						FIELD_ORDER,					PLC_TYPE_STRING,	ACTION_BATCH,		20,		L"D",		ctBASE_ADDRESS + 0 },
		{ L"批號",						FIELD_SN,						PLC_TYPE_STRING,	ACTION_BATCH,		20,		L"D",		ctBASE_ADDRESS + 10},//FIELD_ASSIGN
		{ L"工單產品數量",				FIELD_QUANTITY,					PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 20},//FIELD_ASSIGNNUM

		{ L"一開幾數",					FIELD_SPLITNUM,					PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 21},
		{ L"第一張大小板長",				FIELD_SPLIT_ONE_Y,				PLC_TYPE_FLOAT,		ACTION_BATCH,		4,		L"D",		ctBASE_ADDRESS + 22},
		{ L"第二張大小板長",				FIELD_SPLIT_TWO_Y,				PLC_TYPE_FLOAT,		ACTION_BATCH,		4,		L"D",		ctBASE_ADDRESS + 24},
		{ L"第三張大小板長",				FIELD_SPLIT_THREE_Y,			PLC_TYPE_FLOAT,		ACTION_BATCH,		4,		L"D",		ctBASE_ADDRESS + 26},
		{ L"第一張大小板寬",				FIELD_SPLIT_ONE_X,				PLC_TYPE_FLOAT,		ACTION_BATCH,		4,		L"D",		ctBASE_ADDRESS + 28},
		{ L"第二張大小板寬",				FIELD_SPLIT_TWO_X,				PLC_TYPE_FLOAT,		ACTION_BATCH,		4,		L"D",		ctBASE_ADDRESS + 30},
		{ L"第三張大小板寬",				FIELD_SPLIT_THREE_X,			PLC_TYPE_FLOAT,		ACTION_BATCH,		4,		L"D",		ctBASE_ADDRESS + 32},

		{ L"料號",						FIELD_MATERIAL,					PLC_TYPE_STRING,	ACTION_BATCH,		20,		L"D",		ctBASE_ADDRESS + 37},

		{ L"第一個大小版經向公差下限",		FIELD_DIFF_ONE_Y_MIN,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 47},
		{ L"第一個大小版經向公差上限",		FIELD_DIFF_ONE_Y_MAX,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 48},
		{ L"第一個大小版緯向公差下限",		FIELD_DIFF_ONE_X_MIN,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 49},
		{ L"第一個大小版緯向公差上限",		FIELD_DIFF_ONE_X_MAX,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 50},
		{ L"第一個大小版對角線公差下限",		FIELD_DIFF_ONE_XY_MIN,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 51},
		{ L"第一個大小版對角線公差上限",		FIELD_DIFF_ONE_XY_MAX,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 52},

		{ L"第二個大小版經向公差下限",		FIELD_DIFF_TWO_Y_MIN,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 53},
		{ L"第二個大小版經向公差上限",		FIELD_DIFF_TWO_Y_MAX,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 54},
		{ L"第二個大小版緯向公差下限",		FIELD_DIFF_TWO_X_MIN,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 55},
		{ L"第二個大小版緯向公差上限",		FIELD_DIFF_TWO_X_MAX,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 56},
		{ L"第二個大小版對角線公差下限",		FIELD_DIFF_TWO_XY_MIN,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 57},
		{ L"第二個大小版對角線公差上限",		FIELD_DIFF_TWO_XY_MAX,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 58},

		{ L"第三個大小版經向公差下限",		FIELD_DIFF_THREE_Y_MIN,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 59},
		{ L"第三個大小版經向公差上限",		FIELD_DIFF_THREE_Y_MAX,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 60},
		{ L"第三個大小版緯向公差下限",		FIELD_DIFF_THREE_X_MIN,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 61},
		{ L"第三個大小版緯向公差上限",		FIELD_DIFF_THREE_X_MAX,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 62},
		{ L"第三個大小版對角線公差下限",		FIELD_DIFF_THREE_XY_MIN,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 63},
		{ L"第三個大小版對角線公差上限",		FIELD_DIFF_THREE_XY_MAX,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 64},
		//上傳-----------------------------------------------------------------------------------------------------------------------------------------------
		{ L"指令下發",					FIELD_CCL_COMMAND,				PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 66},
		{ L"板剪切編",				    FIELD_CCL_NO_C10,				PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 67},

		{ L"小板實際長度1",				FIELD_REAL_Y_ONE,			    PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 300},
		{ L"小板實際長度2",				FIELD_REAL_Y_TWO,			    PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 302},
		{ L"小板實際寬度1",				FIELD_REAL_X_ONE,			    PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 304},
		{ L"小板實際寬度2",				FIELD_REAL_X_TWO,			    PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 306},

		{ L"小板長度實際公差1",			FIELD_REAL_DIFF_ONE_Y,		    PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 308},
		{ L"小板長度實際公差2",			FIELD_REAL_DIFF_TWO_Y,		    PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 310},
		{ L"小板寬度實際公差1",			FIELD_REAL_DIFF_ONE_X,		    PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 312},
		{ L"小板寬度實際公差2",			FIELD_REAL_DIFF_TWO_X,		    PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 314},
		{ L"小板對角線實際公差1",			FIELD_REAL_DIFF_ONE_XY,		    PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 316},
		{ L"小板對角線實際公差2",			FIELD_REAL_DIFF_TWO_XY,		    PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 318},

		{ L"正面判斷級別",				FIELD_FRONT_LEVEL,			    PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 320},
		{ L"反面判斷級別",				FIELD_BACK_LEVEL,			    PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 336},
		{ L"尺寸判斷級別(G10)",			FIELD_SIZE_G10,				    PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 352},
		{ L"尺寸判斷級別(G12)",			FIELD_SIZE_G12,				    PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 353},
		{ L"尺寸判斷級別(G14)",			FIELD_SIZE_G14,				    PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 354},

		{ L"CCD接收 MES 資料完成",		FIELD_CCD_COMMAND_RECEIVED,		PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 355},
		{ L"CCD發送檢測結果",				FIELD_CCD_RESULT,				PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 356},
		{ L"CCD接收PLC接收檢測結果完成",	FIELD_CCD_RESULT_RECEIVED,		PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 357},

		{ L"版OK數量",					FIELD_RESULT_OKNum,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 359},
		{ L"版NG數量",					FIELD_RESULT_NGNum,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 360},
		{ L"訂單合格率",					FIELD_RESULT_QUALIFYRATE,		PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 362},

		{ L"通知MES工單資訊",				FIELD_BATCH_MES,				PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 366},
		{ L"檢測設定",					FIELD_INSP_SETTING,				PLC_TYPE_STRING,	ACTION_RESULT,		10,		L"D",		ctBASE_ADDRESS + 367},
		{ L"光源設定",					FIELD_LIGHT_SETTING,			PLC_TYPE_STRING,	ACTION_RESULT,		10,		L"D",		ctBASE_ADDRESS + 372},
		{ L"檢測開始時間",				FIELD_START_TIME,				PLC_TYPE_STRING,	ACTION_RESULT,		18,		L"D",		ctBASE_ADDRESS + 377},
		{ L"檢測結束時間",				FIELD_END_TIME,					PLC_TYPE_STRING,	ACTION_RESULT,		18,		L"D",		ctBASE_ADDRESS + 387},

		{ L"訂單號",						FIELD_ORDER_1,					PLC_TYPE_STRING,	ACTION_RESULT,		20,		L"D",		ctBASE_ADDRESS + 1000},
		{ L"批號",						FIELD_SN_1,						PLC_TYPE_STRING,	ACTION_RESULT,		20,		L"D",		ctBASE_ADDRESS + 1010},
		{ L"料號",						FIELD_MATERIAL_1,				PLC_TYPE_STRING,	ACTION_RESULT,		20,		L"D",		ctBASE_ADDRESS + 1037},
	};
	m_pPLC_FIELD_INFO = new PLC_DATA_ITEM_ * [FIELD_MAX];
	for (int i = 0; i < FIELD_MAX; i++)
	{
		m_pPLC_FIELD_INFO[i] = new PLC_DATA_ITEM;
		memset(m_pPLC_FIELD_INFO[i], 0, sizeof(PLC_DATA_ITEM));//初始化
		memcpy(m_pPLC_FIELD_INFO[i], &ctSYST_PLC_FIELD[i], sizeof(PLC_DATA_ITEM));//複製值
	}
	INIT_PLCDATA();
}
///<summary>[Constructor]終處置</summary>
CEverStrProcess::~CEverStrProcess()
{
	DESTROY_PLC_DATA();
	int nFieldSize = GetFieldSize();
	if (m_pPLC_FIELD_INFO)
	{
		for (int i = 0; i < nFieldSize; i++)
		{
			if (m_pPLC_FIELD_INFO[i])
			{
				delete[] m_pPLC_FIELD_INFO[i];
				m_pPLC_FIELD_INFO[i] = NULL;
			}
		}
		delete[] m_pPLC_FIELD_INFO;
	}
}
//================================================================
///<summary>搜尋對應位址資訊</summary>
PLC_DATA_ITEM_* CEverStrProcess::GetPLCAddressInfo(int nFieldId, BOOL bSkip)
{
	if (nFieldId >= 0 && nFieldId <= FIELD_MAX)
	{
		return m_pPLC_FIELD_INFO[nFieldId];//取ID目前狀態
	}
	return NULL;
}

void CEverStrProcess::DoWriteResult(BATCH_SHARE_SYST_RESULT_EVERSTR& xData)
{
	auto WriteSizeField = [&](int nField, float fSize)
	{
		WORD nSize = (int)fSize;
		SET_PLC_FIELD_DATA(nField, 4, (BYTE*)&nSize);
	};

	WriteSizeField(FIELD_REAL_Y_ONE, xData.fReal_One_Y);
	WriteSizeField(FIELD_REAL_Y_TWO, xData.fReal_Two_Y);
	WriteSizeField(FIELD_REAL_X_ONE, xData.fReal_One_X);
	WriteSizeField(FIELD_REAL_X_TWO, xData.fReal_Two_X); 

	SET_PLC_FIELD_DATA(FIELD_CCL_NO_C10, 2, (BYTE*)&xData.wIndex);
	
	SET_PLC_FIELD_DATA(FIELD_FRONT_LEVEL, 2, (BYTE*)&xData.wFrontLevel);
	SET_PLC_FIELD_DATA(FIELD_BACK_LEVEL,  2, (BYTE*)&xData.wBackLevel);

	SET_PLC_FIELD_DATA(FIELD_SIZE_G10, 2, (BYTE*)&xData.wSize_G10);
	SET_PLC_FIELD_DATA(FIELD_SIZE_G12, 2, (BYTE*)&xData.wSize_G12);
	SET_PLC_FIELD_DATA(FIELD_SIZE_G14, 2, (BYTE*)&xData.wSize_G14);

	SET_PLC_FIELD_DATA(FIELD_RESULT_OKNum, 2, (BYTE*)&xData.wNum_A);//OK數量
	SET_PLC_FIELD_DATA(FIELD_RESULT_NGNum, 2, (BYTE*)&xData.wNum_P);//NG數量
	SET_PLC_FIELD_DATA(FIELD_RESULT_QUALIFYRATE, 2, (BYTE*)&xData.wQualifyRate);//訂單合格率

	//WORD wMES = 400;
	//SET_PLC_FIELD_DATA(FIELD_BATCH_MES, 2, (BYTE*)&wMES);

	SET_PLC_FIELD_DATA(FIELD_ORDER_1, (int)sizeof(xData.cName), (BYTE*)&xData.cName);
	SET_PLC_FIELD_DATA(FIELD_SN_1, (int)sizeof(xData.cAssign), (BYTE*)&xData.cAssign);
	SET_PLC_FIELD_DATA(FIELD_MATERIAL_1, (int)sizeof(xData.cMaterial), (BYTE*)&xData.cMaterial);
}

void CEverStrProcess::DoSetInfoField(BATCH_SHARE_SYST_INFO& xInfo)
{
	//SET_PLC_FIELD_DATA_BIT(FIELD_SIZE_INFO_1, FIELD_CCD_INFO_2, 2, (BYTE*)&xInfo.xInfo1);
	//SET_PLC_FIELD_DATA_BIT(FIELD_CCD_ERROR_1, FIELD_SIZE_ERROR_1, 2, (BYTE*)&xInfo.xInfo2);
}
///<summary>MX-Component設定</summary>
void CEverStrProcess::SetMXParam(IActProgType* pParam, BATCH_SHARE_SYSTCCL_INITPARAM& xData)
{
#ifdef _DEBUG
	pParam->put_ActBaudRate(0x00);
	pParam->put_ActControl(0x00);
	pParam->put_ActCpuType(CPU_FX5UCPU);
	pParam->put_ActDataBits(0x00);
	pParam->put_ActDestinationIONumber(0x00);
	pParam->put_ActDestinationPortNumber(5562);
	pParam->put_ActDidPropertyBit(0x01);
	pParam->put_ActDsidPropertyBit(0x01);
	pParam->put_ActIntelligentPreferenceBit(0x00);
	pParam->put_ActIONumber(0x3FF);
	pParam->put_ActNetworkNumber(0x00);
	pParam->put_ActPacketType(0x01);
	pParam->put_ActPortNumber(0x00);
	pParam->put_ActProtocolType(PROTOCOL_TCPIP);
	pParam->put_ActStationNumber(0xFF);
	pParam->put_ActStopBits(0x00);
	pParam->put_ActSumCheck(0x00);
	pParam->put_ActThroughNetworkType(0x01);
	pParam->put_ActTimeOut(0x100);							//100ms timeout
	pParam->put_ActUnitNumber(0x00);
	pParam->put_ActUnitType(UNIT_FXVETHER);
#else
	pParam->put_ActBaudRate(0x00);
	pParam->put_ActControl(0x00);
	pParam->put_ActCpuType(CPU_FX5UCPU);
	pParam->put_ActDataBits(0x00);
	pParam->put_ActDestinationIONumber(0x00);
	pParam->put_ActDestinationPortNumber(5562);
	pParam->put_ActDidPropertyBit(0x01);
	pParam->put_ActDsidPropertyBit(0x01);
	pParam->put_ActIntelligentPreferenceBit(0x00);
	pParam->put_ActIONumber(0x3FF);
	pParam->put_ActNetworkNumber(0x00);
	pParam->put_ActPacketType(0x01);
	pParam->put_ActPortNumber(0x00);
	pParam->put_ActProtocolType(PROTOCOL_TCPIP);
	pParam->put_ActStationNumber(0xFF);
	pParam->put_ActStopBits(0x00);
	pParam->put_ActSumCheck(0x00);
	pParam->put_ActThroughNetworkType(0x01);
	pParam->put_ActTimeOut(0x100);							//100ms timeout
	pParam->put_ActUnitNumber(0x00);
	pParam->put_ActUnitType(UNIT_FXVETHER);
#endif
}

void CEverStrProcess::ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp)
{
	switch (wp)
	{
		case  WM_SYST_EXTRA_CMD:
		{
			BATCH_SYST_EXTRA xExtra;
			memset(&xExtra, 0, sizeof(xExtra));
			if (USM_ReadData((unsigned char*)&xExtra, sizeof(xExtra)))
			{
				CStringA str;
				str.Format("1234567890");
				memcpy(xExtra.cInsp, str.GetBuffer(), _countof(xExtra.cInsp));
				SET_PLC_FIELD_DATA(FIELD_INSP_SETTING, 10, (BYTE*)&xExtra.cInsp);
				memcpy(xExtra.cLight, str.GetBuffer(), _countof(xExtra.cLight));
				SET_PLC_FIELD_DATA(FIELD_LIGHT_SETTING, 10, (BYTE*)&xExtra.cLight);
				
				char cTime[18];
				memset(&cTime, 0, sizeof(cTime));
#ifndef _DEBUG
				sprintf_s(cTime, "%S", CTime(xExtra.xStart).Format(L"%y-%m-%d %H-%M-%S"));
#else
				sprintf_s(cTime, "%S", CTime::GetCurrentTime().Format(L"%y-%m-%d %H-%M-%S"));
#endif
				SET_PLC_FIELD_DATA(FIELD_START_TIME, sizeof(cTime), (BYTE*)&cTime);

#ifndef _DEBUG
				sprintf_s(cTime, "%S", CTime(xExtra.xEnd).Format(L"%y-%m-%d %H-%M-%S"));
#else
				sprintf_s(cTime, "%S", CTime::GetCurrentTime().Format(L"%y-%m-%d %H-%M-%S"));
#endif
				SET_PLC_FIELD_DATA(FIELD_END_TIME, sizeof(cTime), (BYTE*)&cTime);
				//發送 400 給PLC, PLC接收後自行清除 400
				WORD wMES = 400;
				SET_PLC_FIELD_DATA(FIELD_BATCH_MES, 2, (BYTE*)&wMES);
			}
		}
		break;
	}
	CEverStrProcessBase::ON_GPIO_NOTIFY(wp, lp);
}

///<summary>客製化行為</summary>
void CEverStrProcess::DoCustomAction()
{

}