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
	CCL_SC,	  //�]�Ƹ�
	CCL_NO,   //���ȸ�
	CCL_SO,   //�u�渹
	CCL_PN,   //�Ƹ�
	CCL_LT,   //�帹
	CCL_QT,   //CCL:�ƶq
	CCL_BK,   //BOOK��
	CCL_QS,   //�i��
	CCL_UP,   //�@���X
	CCL_ST,   //���Ȫ��A(CLEAR/START/CLOSED)
	CCL_E1,   //�ֲիH���GBOOK��-�i��(�|�s�b�h��)
	CCL_SQ,   //�˴��i�ƧǸ�
	CCL_DT,   //���I�N�X
	CCL_CN,   //���u�s��
	CCL_QR,   //�ĴX�isheet
	CCL_BB,	  //�}�l�ĴXBOOK
	CCL_EB,	  //�����ĴXBOOK
	CCL_BS,	  //�}�l�ĴX�i
	CCL_ES,	  //�����ĴX�i
	CCL_TIME, //�ɶ�
	CCL_MAX
};
enum EMC_PP_FIELD_TYPE{
	PP_SC,	 //�]�Ƹ�
	PP_NO,   //���ȸ�
	PP_SO,   //�u�渹
	PP_PN,   //�Ƹ�
	PP_LT,   //�帹
	PP_QT,   //�C���̼�
	PP_QB,	 //���I�}�l�̼�
	PP_QE,	 //���I�����̼�
	PP_ST,   //���Ȫ��A(CLEAR/START/CLOSED)
	PP_DT,   //���I�N�X
	PP_CN,   //���u�s��
	PP_TIME, //�ɶ�
	PP_MAX
};

enum UI_TYPE{
	UT_PARAM = 0x01, //�U�o���
	UT_RESULT = 0x02,//�˴����G���
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
	{ CCL_SC, _T("SC"), _T("�]�Ƹ�"),		TRUE, UT_PARAM | UT_RESULT },
	{ CCL_NO, _T("NO"), _T("���ȸ�"),		TRUE, UT_PARAM | UT_RESULT },
	{ CCL_SO, _T("SO"), _T("�u�渹"),		TRUE, UT_PARAM | UT_RESULT },
	{ CCL_PN, _T("PN"), _T("�Ƹ�"),			TRUE, UT_PARAM | UT_RESULT },
	{ CCL_LT, _T("LT"), _T("�帹"),			TRUE, UT_PARAM | UT_RESULT },
	{ CCL_QT, _T("QT"), _T("�ƶq"),			TRUE, UT_PARAM  },
	{ CCL_BK, _T("BK"), _T("BOOK��"),		TRUE, UT_PARAM | UT_RESULT },
	{ CCL_QS, _T("QS"), _T("�i��"),			TRUE, UT_PARAM  },
	{ CCL_UP, _T("UP"), _T("�@���X"),		TRUE, UT_PARAM  },
	{ CCL_ST, _T("ST"), _T("���Ȫ��A"),		TRUE, UT_PARAM | UT_RESULT  },
	{ CCL_E1, _T("E1"), _T("�ֲիH��"),		FALSE,UT_PARAM },
	{ CCL_SQ, _T("SQ"), _T("�˴��i�ƧǸ�"), FALSE,UT_RESULT },
	{ CCL_DT, _T("DT"), _T("���I�N�X"),		FALSE,UT_RESULT },
	{ CCL_CN, _T("CN"), _T("���u�s��"),		TRUE, UT_PARAM },
	{ CCL_QR, _T("QR"), _T("�ĴXSheet"),	FALSE,UT_RESULT },
	{ CCL_BB, _T("BB"), _T("�}�l�ĴXBOOK"), TRUE, UT_PARAM },
	{ CCL_EB, _T("EB"), _T("�����ĴXBOOK"), TRUE, UT_PARAM },
	{ CCL_BS, _T("BS"), _T("�}�l�ĴX�i"),   TRUE, UT_PARAM },
	{ CCL_ES, _T("ES"), _T("�����ĴX�i"),   TRUE, UT_PARAM },
	{ CCL_TIME, _T(""), _T("Time"),			FALSE,UT_PARAM | UT_RESULT  },
};

const EMC_FIELD ctEMC_PP_FIELD[] =
{
	{ PP_SC, _T("SC"), _T("�]�Ƹ�"),		TRUE, UT_PARAM | UT_RESULT },
	{ PP_NO, _T("NO"), _T("���ȸ�"),		TRUE, UT_PARAM | UT_RESULT },
	{ PP_SO, _T("SO"), _T("�u�渹"),		TRUE, UT_PARAM | UT_RESULT },
	{ PP_PN, _T("PN"), _T("�Ƹ�")  ,		TRUE, UT_PARAM | UT_RESULT},
	{ PP_LT, _T("LT"), _T("�帹")  ,		TRUE, UT_PARAM | UT_RESULT},
	{ PP_QT, _T("QT"), _T("�C���̼�"),		FALSE,UT_RESULT },
	{ PP_QB, _T("QB"), _T("���I�}�l�̼�") , FALSE,UT_RESULT},
	{ PP_QE, _T("QE"), _T("���I�����̼�") , FALSE,UT_RESULT},
	{ PP_ST, _T("ST"), _T("���Ȫ��A") ,		TRUE, UT_PARAM | UT_RESULT},
	{ PP_DT, _T("DT"), _T("���I�N�X") ,		FALSE,UT_RESULT },
	{ PP_CN, _T("CN"), _T("���u�s��") ,		TRUE, UT_PARAM},
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
	CString strStation;				//�]�Ƹ�
	CString strMissionID;			//���ȸ�
	CString strBatchName;			//�u�渹
	CString strMaterial;			//�Ƹ�
	EMC_MISSION_STATUS eStatus;		//���Ȫ��A(CLEAR/START/CLOSED)
	CString strDefectType;			//���I�N�X
	CString strEmpID;				//���u�s��

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
	CString strSerial;				//�帹
	int nNum;						//�ƶq
	int nBookNum;					//BOOK��
	int nSheetNum;					//�i��
	int nSplit;						//�@���X
	int nBeginBook;					//�}�l�ĴXBOOK
	int nEndBook;					//�����ĴXBOOK
	int nBeginSheet;				//�}�l�ĴX�i
	int nEndSheet;					//�����ĴX�i
	vector<pair<int, int>>	vMiss;	//�ֲիH���GBOOK��-�i��
	int nIndex;						//�˴��i�ƧǸ�
	CString strSheet;				//�ĴXSheet
};
struct EMC_PP_DATA : public EMC_BASE{
	EMC_PP_DATA(){
		fLength = 0;
		fDefectBegin = 0;
		fDefectEnd = 0;
	}
	
	vector<CString> vSerial;		//�帹
	float fLength;					//�C���̼�
	float fDefectBegin;				//���I�}�l�̼�
	float fDefectEnd;				//���I�����̼�
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
	static int CheckPPData(CString strData); //�^�ǭnparse������. 0=>buffer���u�����|�����, �~�򵥫ݫʥ]����time out
	static int CheckCCLData(CString strData);
private:

	static BOOL GetField(CString strField, PRODUCT_TYPE eType, int &nField);
	static CString GetField(int nField, PRODUCT_TYPE eType);
	static EMC_MISSION_STATUS GetStatus(CString strStatus);
};
