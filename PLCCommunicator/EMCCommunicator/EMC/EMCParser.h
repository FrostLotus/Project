#pragma once
#include <vector>

using namespace std;

#define EMC_FIELD_SPLITER	L'|'
#define EMC_FIELD_LENGTH	2
#define EMC_DATA_SPLITER	L'-'
#define ACK_SUCCESS  L"ACK_SUCCESS"


enum PRODUCT_TYPE{
	CCL,
	PP
};

enum EMC_CCL_FIELD_TYPE{
	CCL_SC,	  //設備號
	CCL_NO,   //任務號
	CCL_SO,   //工單號
	CCL_PN,   //料號
	CCL_LT,   //批號
	CCL_QT,   //CCL:數量
	CCL_BK,   //BOOK數
	CCL_QS,   //張數
	CCL_UP,   //一裁幾
	CCL_ST,   //任務狀態(CLEAR/START/CLOSED)
	CCL_E1,   //少組信息：BOOK數-張數(會存在多個)
	CCL_SQ,   //檢測張數序號
	CCL_DT,   //缺點代碼
	CCL_CN,   //員工編號
	CCL_QR,   //第幾張sheet
	CCL_BB,	  //開始第幾BOOK
	CCL_EB,	  //結束第幾BOOK
	CCL_BS,	  //開始第幾張
	CCL_ES,	  //結束第幾張
	CCL_TIME, //時間
	CCL_MAX
};
enum EMC_PP_FIELD_TYPE{
	PP_SC,	 //設備號
	PP_NO,   //任務號
	PP_SO,   //工單號
	PP_PN,   //料號
	PP_LT,   //批號
	PP_QT,   //每卷米數
	PP_QB,	 //缺點開始米數
	PP_QE,	 //缺點結束米數
	PP_ST,   //任務狀態(CLEAR/START/CLOSED)
	PP_DT,   //缺點代碼
	PP_CN,   //員工編號
	PP_TIME, //時間
	PP_MAX
};

enum UI_TYPE{
	UT_PARAM = 0x01, //下發欄位
	UT_RESULT = 0x02,//檢測結果欄位
};

struct EMC_FIELD{
	int nFieldType;
	CString strField;
	CString strFieldName;
	BOOL bCheckExist;
	WORD wUiType;
};

const EMC_FIELD ctEMC_CCL_FIELD[] =
{
	{ CCL_SC, _T("SC"), _T("設備號"),		TRUE, UT_PARAM | UT_RESULT },
	{ CCL_NO, _T("NO"), _T("任務號"),		TRUE, UT_PARAM | UT_RESULT },
	{ CCL_SO, _T("SO"), _T("工單號"),		TRUE, UT_PARAM | UT_RESULT },
	{ CCL_PN, _T("PN"), _T("料號"),			TRUE, UT_PARAM | UT_RESULT },
	{ CCL_LT, _T("LT"), _T("批號"),			TRUE, UT_PARAM | UT_RESULT },
	{ CCL_QT, _T("QT"), _T("數量"),			TRUE, UT_PARAM  },
	{ CCL_BK, _T("BK"), _T("BOOK數"),		TRUE, UT_PARAM | UT_RESULT },
	{ CCL_QS, _T("QS"), _T("張數"),			TRUE, UT_PARAM  },
	{ CCL_UP, _T("UP"), _T("一裁幾"),		TRUE, UT_PARAM  },
	{ CCL_ST, _T("ST"), _T("任務狀態"),		TRUE, UT_PARAM | UT_RESULT  },
	{ CCL_E1, _T("E1"), _T("少組信息"),		FALSE,UT_PARAM },
	{ CCL_SQ, _T("SQ"), _T("檢測張數序號"), FALSE,UT_RESULT },
	{ CCL_DT, _T("DT"), _T("缺點代碼"),		FALSE,UT_RESULT },
	{ CCL_CN, _T("CN"), _T("員工編號"),		TRUE, UT_PARAM },
	{ CCL_QR, _T("QR"), _T("第幾Sheet"),	FALSE,UT_RESULT },
	{ CCL_BB, _T("BB"), _T("開始第幾BOOK"), TRUE, UT_PARAM },
	{ CCL_EB, _T("EB"), _T("結束第幾BOOK"), TRUE, UT_PARAM },
	{ CCL_BS, _T("BS"), _T("開始第幾張"),   TRUE, UT_PARAM },
	{ CCL_ES, _T("ES"), _T("結束第幾張"),   TRUE, UT_PARAM },
	{ CCL_TIME, _T(""), _T("Time"),			FALSE,UT_PARAM | UT_RESULT  },
};

const EMC_FIELD ctEMC_PP_FIELD[] =
{
	{ PP_SC, _T("SC"), _T("設備號"),		TRUE, UT_PARAM | UT_RESULT },
	{ PP_NO, _T("NO"), _T("任務號"),		TRUE, UT_PARAM | UT_RESULT },
	{ PP_SO, _T("SO"), _T("工單號"),		TRUE, UT_PARAM | UT_RESULT },
	{ PP_PN, _T("PN"), _T("料號")  ,		TRUE, UT_PARAM | UT_RESULT},
	{ PP_LT, _T("LT"), _T("批號")  ,		TRUE, UT_PARAM | UT_RESULT},
	{ PP_QT, _T("QT"), _T("每卷米數"),		FALSE,UT_RESULT },
	{ PP_QB, _T("QB"), _T("缺點開始米數") , FALSE,UT_RESULT},
	{ PP_QE, _T("QE"), _T("缺點結束米數") , FALSE,UT_RESULT},
	{ PP_ST, _T("ST"), _T("任務狀態") ,		TRUE, UT_PARAM | UT_RESULT},
	{ PP_DT, _T("DT"), _T("缺點代碼") ,		FALSE,UT_RESULT },
	{ PP_CN, _T("CN"), _T("員工編號") ,		TRUE, UT_PARAM},
	{ PP_TIME, _T(""), _T("Time"),			FALSE,UT_PARAM | UT_RESULT},
};
enum EMC_MISSION_STATUS{
	EMS_NONE,
	EMS_CLEAR,
	EMS_START,
	EMS_CLOSED,
	EMS_DELETE,
	EMS_EXCEPT,
};
struct EMC_BASE{
public:
	EMC_BASE(){
		eStatus = EMC_MISSION_STATUS::EMS_NONE;
		xTime = 0;
	}
	CString strStation;				//設備號
	CString strMissionID;			//任務號
	CString strBatchName;			//工單號
	CString strMaterial;			//料號
	EMC_MISSION_STATUS eStatus;		//任務狀態(CLEAR/START/CLOSED)
	CString strDefectType;			//缺點代碼
	CString strEmpID;				//員工編號

	CTime xTime;					//Time
};
struct EMC_CCL_DATA : public EMC_BASE{
	EMC_CCL_DATA(){
		nNum = 0;
		nBookNum = 0;
		nSheetNum = 0;
		nSplit = 0;
		nIndex = 0;
		vMiss.clear();
	}
	CString strSerial;				//批號
	int nNum;						//數量
	int nBookNum;					//BOOK數
	int nSheetNum;					//張數
	int nSplit;						//一裁幾
	int nBeginBook;					//開始第幾BOOK
	int nEndBook;					//結束第幾BOOK
	int nBeginSheet;				//開始第幾張
	int nEndSheet;					//結束第幾張
	vector<pair<int, int>>	vMiss;	//少組信息：BOOK數-張數
	int nIndex;						//檢測張數序號
	CString strSheet;				//第幾Sheet
};
struct EMC_PP_DATA : public EMC_BASE{
	EMC_PP_DATA(){
		fLength = 0;
		fDefectBegin = 0;
		fDefectEnd = 0;
	}
	
	vector<CString> vSerial;		//批號
	float fLength;					//每卷米數
	float fDefectBegin;				//缺點開始米數
	float fDefectEnd;				//缺點結束米數
};

class CEMCParser{
public:
	CEMCParser();
	virtual ~CEMCParser();

public:
	static void InitCCLData(EMC_CCL_DATA &xData);
	static void InitPPData(EMC_PP_DATA &xData);
	static BOOL ParseCCL(CString &strData, vector<EMC_CCL_DATA> &vParam);
	static BOOL ParsePP(CString &strData, EMC_PP_DATA &xData);
	static CString MakeString(EMC_CCL_DATA &xResult);
	static CString MakeString(EMC_PP_DATA &xParam);
	static CString GetStatus(EMC_MISSION_STATUS eStatus);
	static int CheckPPData(CString strData); //回傳要parse的長度. 0=>buffer內工單欄位尚未到齊, 繼續等待封包直到time out
	static int CheckCCLData(CString strData);
private:

	static BOOL GetField(CString strField, PRODUCT_TYPE eType, int &nField);
	static CString GetField(int nField, PRODUCT_TYPE eType);
	static EMC_MISSION_STATUS GetStatus(CString strStatus);
};
