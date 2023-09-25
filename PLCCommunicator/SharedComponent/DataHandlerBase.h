#pragma once


#define WM_AOI_RESPONSE_CMD					(WM_APP+2)
#define WM_SYST_RESPONSE_CMD				(WM_APP+3) 
#define WM_SYST_PARAMINIT_CMD				(WM_APP+4) //PLC > AOI

#define WM_SYST_PARAMCCL_CMD				(WM_APP+5) //PLC > AOI
#define WM_SYST_PARAMWEBCOOPER_CMD			(WM_APP+6) //PLC > AOI
#define WM_SYST_RESULTCCL_CMD				(WM_APP+7) //AOI > PLC
#define WM_SYST_C10CHANGE_CMD				(WM_APP+8) //PLC > AOI
#define WM_SYST_INFO_CHANGE					(WM_APP+18) //AOI > PLC


#define WM_EMC_RESPONSE_CMD					(WM_APP+9) 
#define WM_EMC_PARAMINIT_CMD				(WM_APP+10) //EMC > AOI
#define WM_EMC_PARAMCCL_CMD					(WM_APP+11)//EMC > AOI
#define WM_EMC_RESULTCCL_CMD				(WM_APP+12)//AOI > EMC
#define WM_EMC_ENDCCL_CMD					(WM_APP+13)//AOI > EMC

#define WM_EMC_PARAMPP_CMD					(WM_APP+14)//EMC > AOI
#define WM_EMC_RESULTPP_CMD					(WM_APP+15)//AOI > EMC
#define WM_EMC_ENDPP_CMD					(WM_APP+16)//AOI > EMC

#define WM_EMC_ERROR_CMD					(WM_APP+17)//EMC > AOI

#define WM_MX_PARAMINIT_CMD					(WM_APP+18) //MX > AOI
#define WM_MX_PINSTATUS_CMD					(WM_APP+19) //AOI > MX
#define WM_MX_PININFO_CMD					(WM_APP+20) //MX > AOI
#define WM_MX_PINRESULT_CMD					(WM_APP+21) //MX > AOI
#define WM_CUSTOMERTYPE_INIT				(WM_APP+22)
#define WM_THICKINFO_CMD					(WM_APP+23) 
#define WM_OPC_NEWBATCH_CMD					(WM_APP+24) //OPC > AOI
#define WM_SYST_PP_PARAMINIT_CMD			(WM_APP+25) //PLC > AOI
//#define WM_WATCHDOG_CMD						(WM_APP+26) //AOI > PLC
#define WM_PLC_PP_CMD						(WM_APP+27) //PLC > AOI.   first byte of LPARAM is enum: PLC_MESSAGE
#define WM_WS_POTENTIAL_CMD					(WM_APP+28) //AOI > PLC
#define WM_SYST_EXTRA_CMD					(WM_APP+29) //AOI > PLC

#define WM_PLC_INPUT_CMD					(WM_APP+30) //PLC > AOI
#define WM_PLC_OUTPUTPIN_CMD				(WM_APP+31) //AOI > PLC
#define WM_ASSIGN_ENCODER_CMD				(WM_APP+32) //AOI > PLC
#define WM_PLC_ENCODER_POS_CMD				(WM_APP+33) //PLC > AOI
#define WM_INSPSTATUS_CMD					(WM_APP+34) //AOI > PLC
#define WM_TECHAIN_CMD						(WM_APP+35) //PLC > AOI.   first byte of LPARAM is enum: PLC_TECHAIN_MESSAGE
#define WM_SQL_CMD							(WM_APP+36) //SQL > AOI

#define WM_OPC_PARAMINIT_CMD				(WM_APP+37) //OPC > AOI
#define WM_OPC_WRITE_CMD					(WM_APP+38) //AOI > OPC
#define WM_OPC_FINISHWRITE_CMD				(WM_APP+39) //OPC > AOI
#define WM_OPC_RETURN_STATUS_CMD			(WM_APP+40) //OPC > AOI
#define WM_OPC_SET_CONNECT_CMD				(WM_APP+41) //AOI > OPC

#define WM_PLC_REVERSE_BEGIN				(WM_APP+42) //PLC > AOI
#define WM_PLC_REVERSE_END					(WM_APP+43) //PLC > AOI

enum PLC_MESSAGE{
	PM_VERSION_ERROR,
	PM_A_AXIS,
	PM_B_AXIS,
	PM_SWITCH_WEB_SHEET,
	PM_SHEET_NEWBATCH,
	PM_CURRENT_INSPSTATUS,

};

#ifdef USE_IN_COMMUNICATOR
#define WM_GPIO_MSG	(WM_APP+990)
#else
#include "Aoi.h"
#endif
#ifdef SUPPORT_AOI
//#define USE_MC_PROTOCOL //use 3E/4E Frame
#endif
#ifdef USE_MC_PROTOCOL
#define PLC_COMMUNICATOR_NAME _T("PLCCommunicator_MC")
#else
#define PLC_COMMUNICATOR_NAME _T("PLCCommunicator")
#endif
#define EMC_COMMUNICATOR_NAME _T("EMCCommunicator")
#define MX_COMMUNICATOR_NAME _T("MXComponentCommunicator")

#define BATCH_COMMUNICATOR_MEM_ID	_T("BATCH_COMMUNICATOR_MEM")	//Communicator->AOI
#define BATCH_AOI_MEM_ID			_T("BATCH_AOI_MEM")				//AOI->Communicator

#define BATCH_AOI2EMC_MEM_ID			_T("BATCH_AOI2EMC_MEM")				//AOI->EMC Communicator
#define BATCH_EMC2AOI_MEM_ID	_T("BATCH_EMC2AOI_MEM")	//EMC Communicator->AOI

#define BATCH_AOI2MX_MEM_ID			_T("BATCH_AOI2MX_MEM")				//AOI->MX_Communicator
#define BATCH_MX2AOI_MEM_ID			_T("BATCH_MX2AOI_MEM")				//MX_Communicator->AOI
#define BATCH_SQL2AOI_MEM_ID		_T("SQL_AGENT_MEM")					
#include <vector>
using namespace std;

#ifndef USE_IN_SLAVE
#include "usm.h"
#else
#define MAX_BATCH_FIELD_LEN 100
#endif

#define AOI_MASTER_NAME _T("AOI Master")
/// <summary>客戶</summary>
enum AOI_CUSTOMERTYPE_ { //eric chao 201
	CUSTOMER_EMC_CCL = 0,		 //台光CCL
	CUSTOMER_NANYA = 2,
	CUSTOMER_SYST_WEB_COPPER = 3, //生益軟板
	CUSTOMER_SYST_CCL = 4,		  //生益CCL
	CUSTOMER_NANYA_WARPING = 5,	  //南亞整經機
	CUSTOMER_SYST_PP = 6,		  //生益 PP       //no use	for AOI_NEWUI_PP_20191121 branch
	CUSTOMER_SCRIBD_PP = 7,		  //宏仁 PP       //no use	for AOI_NEWUI_PP_20191121 branch
	CUSTOMER_EMC_PP = 8,		  //台光 PP      //no use	for AOI_NEWUI_PP_20191121 branch
	CUSTOMER_ITEQ = 9,            //聯茂
	CUSTOMER_JIANGXI_NANYA = 10,  //江西南亞CCL
	CUSTOMER_TUC_PP = 11,		  //台耀 PP
	CUSTOMER_TG = 12,			  //台玻
	CUSTOMER_YINGHUA = 13,		  //盈華
	CUSROMER_EVERSTRONG = 14,     //甬強         //23/09/25新增
	CUSTOMER_TECHAIN = 255,		  //地謙        
#ifdef _DEBUG
	CUSTOMER_TAG = 254,           //標籤機
#endif
};
/// <summary>客戶子場域</summary>
enum AOI_SUBCUSTOMERTYPE_ 
{
	SUB_CUSTOMER_NONE = 0,
	SUB_CUSTOMER_SYST_START = 1,
	SUB_CUSTOMER_DONGGUAN = SUB_CUSTOMER_SYST_START,		//東莞 (生益)
	SUB_CUSTOMER_JIUJIANG,		//九江 (生益)
	SUB_CUSTOMER_SUZHOU,		//蘇州 (生益)
	SUB_CUSTOMER_CHANGSHU,		//常熟 (生益)
	SUB_CUSTOMER_CHANGSHU2,		//常熟 (生益), A2/A4線 PLC Address全部加上 2000
	SUB_CUSTOMER_DONGGUAN_SONG8,//東莞松八 (生益)
	SUB_CUSTOMER_EMC_START = 5,					    //未來台光新增須把4~1代號依遞減方式補齊
	SUB_CUSTOMER_KUNSHAN = 5,	//昆山 (台光)
	SUB_CUSTOMER_HUANGSHI,		//黃石 (台光)
	SUB_CUSTOMER_GUANYIN,		//觀音 (台光)
	SUB_CUSTOMER_ITEQ_START = 7,                   //未來聯茂新增須把6~1代號依遞減方式補齊
	SUB_CUSTOMER_WUXI = 7,      //無錫 (聯茂)
	SUB_CUSTOMER_NANYA_START = 1,                     
	SUB_CUSTOMER_NANYA_N4 = SUB_CUSTOMER_NANYA_START, //江西南亞N4(N4和N5 ERP下發規格不同)
	SUB_CUSTOMER_NANYA_N5							  //江西南亞N5
};
//--------		SYST		--------------------------
/// <summary>[BATCH=>OPC]初始化參數</summary>
typedef struct BATCH_SHARE_OPC_INITPARAM_
{
	TCHAR cOPCIP[MAX_BATCH_FIELD_LEN];
	int nRootIdNamespace;
	TCHAR cROOTID[MAX_BATCH_FIELD_LEN];

}BATCH_SHARE_OPC_INITPARAM;
/// <summary>[BATCH=>SYSTCCL]初始化參數</summary>
typedef struct BATCH_SHARE_SYSTCCL_INITPARAM_
{
	TCHAR cPLCIP[MAX_BATCH_FIELD_LEN];
	long lConnectedStationNo;		//連接站側模組站號
	long lTargetNetworkNo;			//物件站側模組網路No
	long lTargetStationNo;			//物件站側模組站號
	long lPCNetworkNo;				//計算機側網路No
	long lPCStationNo;				//計算機側站號
}BATCH_SHARE_SYSTCCL_INITPARAM;
/// <summary>[BATCH=>SYSTPP]初始化參數</summary>
struct BATCH_SHARE_SYSTPP_INITPARAM_ : public BATCH_SHARE_SYSTCCL_INITPARAM_ 
{
	int nWatchDogTimeout;			//WatchDog timeout(second)
	int nVersion;		
	int nWSMode;					//0:模式一/1:模式二
	BOOL bFX5U;
	int nNewbatchDelay;				//
};
/// <summary>系統Flag參數</summary>
enum SYST_RESULT_FLAG
 {
	SRF_REAL_Y_ONE = 0x0001,			//小板實際長度1
	SRF_REAL_Y_TWO = 0x0002,			//小板實際長度2
	SRF_REAL_X_ONE = 0x0004,			//小板實際寬度1
	SRF_REAL_X_TWO = 0x0008,			//小板實際寬度2
	SRF_REAL_DIFF_Y_ONE = 0x0010,		//小板長度實際公差1
	SRF_REAL_DIFF_Y_TWO = 0x0020,		//小板長度實際公差2
	SRF_REAL_DIFF_X_ONE = 0x0040,		//小板寬度實際公差1
	SRF_REAL_DIFF_X_TWO = 0x0080,		//小板寬度實際公差2
	SRF_REAL_DIFF_XY_ONE = 0x0100,		//小板對角線實際公差1
	SRF_REAL_DIFF_XY_TWO = 0x0200,		//小板對角線實際公差2
	SRF_FRONT_LEVEL = 0x0400,			//正面判斷級別
	SRF_FRONT_CODE = 0x0800,			//正面判斷代碼
	SRF_BACK_LEVEL = 0x1000,			//反面判斷級別
	SRF_BACK_CODE = 0x2000,				//反面判斷代碼
	SRF_SIZE_G10 = 0x4000,			    //尺寸判斷級別(G10)
	SRF_SIZE_G12 = 0x8000,			    //尺寸判斷級別(G12)
	SRF_SIZE_G14 = 0x010000,		    //尺寸判斷級別(G14)
	SRF_RESULT_LEVEL = 0x020000,		//小板物料級別
	SRF_NUM_AA = 0x040000,				//小版AA級數量
	SRF_NUM_A = 0x080000,				//小版A級數量
	SRF_NUM_P = 0x100000,				//小版P級數量
	SRF_QUALIFY_RATE = 0x200000,		//訂單合格率
	SRF_DIFF_XY = 0x400000,			    //級差實際檢測值
	SRF_FRONT_SIZE = 0x800000,			//九宮格中正面前五大缺陷大小1~5
	SRF_FRONT_LOCATION = 0x1000000,		//九宮格中正面前五大缺陷位置1~5
	SRF_BACK_SIZE = 0x2000000,			//九宮格中反面前五大缺陷大小1~5
	SRF_BACK_LOCATION = 0x4000000,		//九宮格中反面前五大缺陷位置1~5
	SRF_INDEX = 0x8000000,		        //小板編號
};
/// <summary>系統工單額外項目</summary>
struct BATCH_SYST_EXTRA 
{
	__time64_t xStart;
	__time64_t xEnd;
	char cInsp[MAX_BATCH_FIELD_LEN];
	char cLight[MAX_BATCH_FIELD_LEN];
};
typedef struct BATCH_SHARE_SYST_BASE_
{
	TCHAR cName[MAX_BATCH_FIELD_LEN];
	TCHAR cMaterial[MAX_BATCH_FIELD_LEN];
}BATCH_SHARE_SYST_BASE;

typedef struct BATCH_SHARE_SYST_PARAMCCL_ : public BATCH_SHARE_SYST_BASE
{
	TCHAR cModel[MAX_BATCH_FIELD_LEN];		//模號
	TCHAR cAssign[MAX_BATCH_FIELD_LEN];		//分發號
	WORD wAssignNum;						//分發號數量
	WORD wSplitNum;							//一開幾
	WORD wSplit_One_X;						//第一張大小板寬
	WORD wSplit_One_Y;						//第一張大小板長
	WORD wSplit_Two_X;						//第二張大小板寬
	WORD wSplit_Two_Y;						//第二張大小板長
	WORD wSplit_Three_X;					//第三張大小板寬
	WORD wSplit_Three_Y;					//第三張大小板長
	float fThickSize;						//板材厚度
	float fThickCCL;						//銅箔厚度
	TCHAR cCCLType[MAX_BATCH_FIELD_LEN];	//銅箔特性
	WORD wGrayScale;						//灰度值
	WORD wPixel_AA;							//點值要求AA
	WORD wPixel_A;							//點值要求A
	WORD wPixel_P;							//點值要求P
	WORD wDiff_One_Y;						//第一個大小板公差(長)
	WORD wDiff_One_X;						//第一個大小板公差(寬)
	WORD wDiff_One_XY;						//第一個大小板公差(對角線)
	WORD wDiff_One_X_Min;					//第一個大小版緯向公差下限
	WORD wDiff_One_X_Max;					//第一個大小版緯向公差上限
	WORD wDiff_One_Y_Min;					//第一個大小版經向公差下限
	WORD wDiff_One_Y_Max;					//第一個大小版經向公差上限
	WORD wDiff_One_XY_Min;					//第一個大小版對角線公差下限
	WORD wDiff_One_XY_Max;					//第一個大小版對角線公差上限
	WORD wDiff_Two_X_Min;					//第二個大小版緯向公差下限
	WORD wDiff_Two_X_Max;					//第二個大小版緯向公差上限
	WORD wDiff_Two_Y_Min;					//第二個大小版經向公差下限
	WORD wDiff_Two_Y_Max;					//第二個大小版經向公差上限
	WORD wDiff_Two_XY_Min;					//第二個大小版對角線公差下限
	WORD wDiff_Two_XY_Max;					//第二個大小版對角線公差上限
	WORD wDiff_Three_X_Min;					//第三個大小版緯向公差下限
	WORD wDiff_Three_X_Max;					//第三個大小版緯向公差上限
	WORD wDiff_Three_Y_Min;					//第三個大小版經向公差下限
	WORD wDiff_Three_Y_Max;					//第三個大小版經向公差上限
	WORD wDiff_Three_XY_Min;				//第三個大小版對角線公差下限
	WORD wDiff_Three_XY_Max;				//第三個大小版對角線公差上限
	WORD wNum_AA;							//小版AA級數量
	WORD wNO_C06;							//C06小板剪切編號
	WORD wNO_C10;							//C10小板剪切編號
	WORD wNO_C12;							//C12小板剪切編號
}BATCH_SHARE_SYST_PARAMCCL;
typedef struct BATCH_SHARE_SYST_INFO1_
{
	BYTE cSizeReady : 1;	//CCD尺寸檢測儀器準備好
	BYTE cSizeRunning : 1;	//CCD尺寸檢測儀器運行
	BYTE cCCDReady : 1;		//CCD表現檢測儀器準備好
	BYTE cCCDRunning : 1;	//CCD表現檢測儀器運行
	BYTE cReserve1 : 4;		//保留欄位
	BYTE cReserve2;			//保留欄位
}BATCH_SHARE_SYST_INFO1;
typedef struct BATCH_SHARE_SYST_INFO2_
{
	BYTE cCCDError1 : 1;	//CCD表現檢測儀器故障
	BYTE cSizeError1 : 1;	//CCD尺寸檢測儀器故障
	BYTE cReserve1 : 6;		//保留欄位
	BYTE cReserve2;			//保留欄位
}BATCH_SHARE_SYST_INFO2;
struct BATCH_SHARE_SYST_INFO
{
	BATCH_SHARE_SYST_INFO1 xInfo1;
	BATCH_SHARE_SYST_INFO2 xInfo2;
};
typedef struct BATCH_SHARE_SYST_RESULTCCL_
{
	float fReal_One_Y;		        //小板實際長度1
	float fReal_Two_Y;		        //小板實際長度2
	float fReal_One_X;		        //小板實際寬度1
	float fReal_Two_X;		        //小板實際寬度2
	WORD wDiff_One_Y;		        //小板長度實際公差1
	WORD wDiff_Two_Y;		        //小板長度實際公差2
	WORD wDiff_One_X;		        //小板寬度實際公差1
	WORD wDiff_Two_X;		        //小板寬度實際公差2
	WORD wDiff_One_XY;		        //小板對角線實際公差1
	WORD wDiff_Two_XY;		        //小板對角線實際公差2
	WORD wFrontLevel;		        //正面判斷級別
	char cFrontCode[30];	        //正面判斷代碼, 字串需要為2的倍數(回寫最小單位為WORD)
	WORD wBackLevel;		        //反面判斷級別
	char cBackCode[30];		        //反面判斷代碼, 字串需要為2的倍數(回寫最小單位為WORD)
	WORD wSize_G10;			        //尺寸判斷級別(G10)
	WORD wSize_G12;			        //尺寸判斷級別(G12)
	WORD wSize_G14;			        //尺寸判斷級別(G14)
							        
	WORD wResultLevel;		        //小版物料級別
	WORD wNum_AA;			        //小版AA級數量
	WORD wNum_A;			        //小版A級數量
	WORD wNum_P;			        //小版P級數量
	WORD wQualifyRate;		        //訂單合格率
	WORD wDiff_XY;			        //級差實際檢測值
	float fFrontDefectSize[5];		//九宮格中正面前五大缺陷大小1~5
	float fBackDefectSize[5];		//九宮格中反面前五大缺陷大小1~5
	WORD wFrontDefectLocation[5];	//九宮格中正面前五大缺陷位置1~5
	WORD wBackDefectLocation[5];	//九宮格中反面前五大缺陷位置1~5
	WORD wIndex;			        //小板編號, 僅東莞松八廠需回傳
}BATCH_SHARE_SYST_RESULTCCL;

typedef struct BATCH_SHARE_SYST_INITPARAM_
{
	int nCustomerType;
	int nSubCustomerType;
	int nFormat;
	TCHAR cPLCIP[MAX_BATCH_FIELD_LEN];
	int nPLCPort;
	int nFrameType;
}BATCH_SHARE_SYST_INITPARAM;

//--------		MX		--------------------------
typedef struct BATCH_SHARE_MX_INITPARAM_{
	long lCPU;
	TCHAR cPLCIP[MAX_BATCH_FIELD_LEN];
	UINT lStartAddress;
}BATCH_SHARE_MX_INITPARAM;
typedef struct BATCH_SHARE_MX_PINSTATUS_{
	int nIndex0Base;
	BOOL bHighLeve;
}BATCH_SHARE_MX_PINSTATUS;
//--------		EMC		--------------------------
#define MAX_PARAM 3 //最多1開3

typedef struct BATCH_SHARE_EMC_INITPARAM_{
	TCHAR cEMCIP[MAX_BATCH_FIELD_LEN];
	int nEMCPort;
	int nListenPort;
	int nProductType; //0:CCL, 1:PP
}BATCH_SHARE_EMC_INITPARAM;
typedef struct BATCH_SHARE_EMC_BASE_ {
	TCHAR cStation[MAX_BATCH_FIELD_LEN];		//設備號
	TCHAR cMissionID[MAX_BATCH_FIELD_LEN];		//任務號
	TCHAR cBatchName[MAX_BATCH_FIELD_LEN];		//工單號
	TCHAR cMaterial[MAX_BATCH_FIELD_LEN];		//料號
	TCHAR cSerial[MAX_BATCH_FIELD_LEN];			//批號
}BATCH_SHARE_EMC_BASE;
typedef struct BATCH_SHARE_EMC_CCLPARAM_ : public BATCH_SHARE_EMC_BASE_ {
	int nNum;										//數量
	int nBookNum;									//BOOK數
	int nSheetNum;									//張數
	int nSplit;										//一裁幾
	TCHAR cMiss[MAX_BATCH_FIELD_LEN];				//少組信息：BOOK數-張數

	int nStatus;									//任務狀態(CLEAR/START/CLOSED)
	TCHAR cEmpID[MAX_BATCH_FIELD_LEN];				//員工編號
	int nBeginBook;									//開始第幾BOOK
	int nEndBook;									//結束第幾BOOK
	int nBeginSheet;								//開始第幾張
	int nEndSheet;									//結束第幾張
}BATCH_SHARE_EMC_CCLPARAM;
struct CCLParam{
	int nSize;
	BATCH_SHARE_EMC_CCLPARAM xParam[MAX_PARAM];
};
typedef struct BATCH_SHARE_EMC_CCLRESULT_ : public BATCH_SHARE_EMC_BASE_ {
	int nIndex;										//檢測張數序號
	int nBookNum;									//BOOK數
	TCHAR cSheet[MAX_BATCH_FIELD_LEN];				//Sheet.Index
	TCHAR cDefectType[3];							//缺點代碼
}BATCH_SHARE_EMC_CCLRESULT;
typedef struct BATCH_SHARE_EMC_CCLEND_ : public BATCH_SHARE_EMC_BASE_ {
	int nIndex;										//檢測張數序號
}BATCH_SHARE_EMC_CCLEND;

typedef struct BATCH_SHARE_EMC_PPPARAM_ : public BATCH_SHARE_EMC_BASE_ {
	int nStatus;									//任務狀態(CLEAR/START/CLOSED)
	TCHAR cEmpID[MAX_BATCH_FIELD_LEN];				//員工編號
}BATCH_SHARE_EMC_PPPARAM;

typedef struct BATCH_SHARE_EMC_PPRESULT_ : public BATCH_SHARE_EMC_BASE_ {
	float fDefectBegin;								//缺點開始米數
	float fDefectEnd;								//缺點結束米數
	TCHAR cDefectType[3];							//缺點代碼
}BATCH_SHARE_EMC_PPRESULT;
typedef struct BATCH_SHARE_EMC_PPEND_ : public BATCH_SHARE_EMC_BASE_ {
	float fLength;									//每卷米數
}BATCH_SHARE_EMC_PPEND;

enum EMC_ERROR{
	EMC_FIELD_NOTCOMPLETE,	//欄位不齊全
	EMC_TIMEOUT,			//TIMEOUT

};
enum LOG_TYPE{
	LOG_FLOAT,
	LOG_WORD,
	LOG_CSTRING,
	LOG_NONE,
};
typedef struct BATCH_SHARE_EMC_ERRORINFO_  {
	EMC_ERROR eErrorType;								//錯誤代碼
	TCHAR cErrorMsg[MAX_BATCH_FIELD_LEN];				//錯誤訊息
}BATCH_SHARE_EMC_ERRORINFO;

enum SYST_INFO_FIELD{
	SIZE_READY		= 0x01,	 //CCD尺寸檢測儀器準備好
	SIZE_RUNNING	= 0x02,	 //CCD尺寸檢測儀器運行
	SIZE_ERROR		= 0x04,	 //CCD尺寸檢測儀器故障
	CCD_READY		= 0x08,	 //CCD表現檢測儀器準備好
	CCD_RUNNING		= 0x10,	 //CCD表現檢測儀器運行
	CCD_ERROR		= 0x20,	 //CCD表現檢測儀器故障
};

class CDataHandlerBase{
public:
	CDataHandlerBase(CString strMemID = L"");
	virtual ~CDataHandlerBase();

	void NotifyResponse(CString strTarget, LPARAM lp);
	void NotifyAOI(WPARAM wp, LPARAM lp);

	//PLC
	virtual void SetInitParam(BATCH_SHARE_SYST_INITPARAM *pData){};
	virtual void GetInitParam(BATCH_SHARE_SYST_INITPARAM *pData){};

	virtual void SetSYSTParam_WebCopper(BATCH_SHARE_SYST_BASE *pData){};
	virtual void GetSYSTParam_WebCopper(BATCH_SHARE_SYST_BASE *pData){};
	virtual void SetSYSYParam_CCL(BATCH_SHARE_SYST_PARAMCCL *pData){};
	virtual void GetSYSTParam_CCL(BATCH_SHARE_SYST_PARAMCCL *pData){};
	virtual void GetSYSTInfo_CCL(BATCH_SHARE_SYST_INFO *pInfo){};

	virtual void SetSYSTInfo_CCL(DWORD dwField, BATCH_SHARE_SYST_INFO &xInfo){};

	void WriteResponse(void *pData, int nSize, CString strTargetName, WPARAM wp, LPARAM lp);
protected:
	BYTE* BeginWrite();
	void EndWrite();
	void GetSharedMemoryData(void *pData, size_t size, CString strMemID);
	void SetSharedMemoryData(void *pData, size_t size, CString strTargetName, WPARAM wp, LPARAM lp);

	CString MakeFloatLog(CString strDes, float fData);
	CString MakeWordLog(CString strDes, WORD wData);
	CString MakeCStringLog(CString strDes, char *pData);
	CString MakeCStringLog(CString strDes, TCHAR *pData);
	CString MakeByteLog(CString strDes, BYTE cData);
private:
	CString m_strMemID;

#ifndef USE_IN_SLAVE
	usm<unsigned char> *m_pUsm;
#endif
	enum {
		OP_CREATE = 0,
		OP_DESTROY,
	};
	void OpShareMemory(int nOpCode);
};