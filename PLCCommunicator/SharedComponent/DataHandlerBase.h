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
/// <summary>�Ȥ�</summary>
enum AOI_CUSTOMERTYPE_ { //eric chao 201
	CUSTOMER_EMC_CCL = 0,		 //�x��CCL
	CUSTOMER_NANYA = 2,
	CUSTOMER_SYST_WEB_COPPER = 3, //�ͯq�n�O
	CUSTOMER_SYST_CCL = 4,		  //�ͯqCCL
	CUSTOMER_NANYA_WARPING = 5,	  //�n�Ⱦ�g��
	CUSTOMER_SYST_PP = 6,		  //�ͯq PP       //no use	for AOI_NEWUI_PP_20191121 branch
	CUSTOMER_SCRIBD_PP = 7,		  //���� PP       //no use	for AOI_NEWUI_PP_20191121 branch
	CUSTOMER_EMC_PP = 8,		  //�x�� PP      //no use	for AOI_NEWUI_PP_20191121 branch
	CUSTOMER_ITEQ = 9,            //�p�Z
	CUSTOMER_JIANGXI_NANYA = 10,  //����n��CCL
	CUSTOMER_TUC_PP = 11,		  //�xģ PP
	CUSTOMER_TG = 12,			  //�x��
	CUSTOMER_YINGHUA = 13,		  //�յ�
	CUSROMER_EVERSTRONG = 14,     //�i�j         //23/09/25�s�W
	CUSTOMER_TECHAIN = 255,		  //�a��        
#ifdef _DEBUG
	CUSTOMER_TAG = 254,           //���Ҿ�
#endif
};
/// <summary>�Ȥ�l����</summary>
enum AOI_SUBCUSTOMERTYPE_ 
{
	SUB_CUSTOMER_NONE = 0,
	SUB_CUSTOMER_SYST_START = 1,
	SUB_CUSTOMER_DONGGUAN = SUB_CUSTOMER_SYST_START,		//�F�� (�ͯq)
	SUB_CUSTOMER_JIUJIANG,		//�E�� (�ͯq)
	SUB_CUSTOMER_SUZHOU,		//Ĭ�{ (�ͯq)
	SUB_CUSTOMER_CHANGSHU,		//�`�� (�ͯq)
	SUB_CUSTOMER_CHANGSHU2,		//�`�� (�ͯq), A2/A4�u PLC Address�����[�W 2000
	SUB_CUSTOMER_DONGGUAN_SONG8,//�F��Q�K (�ͯq)
	SUB_CUSTOMER_EMC_START = 5,					    //���ӥx���s�W����4~1�N���̻���覡�ɻ�
	SUB_CUSTOMER_KUNSHAN = 5,	//���s (�x��)
	SUB_CUSTOMER_HUANGSHI,		//���� (�x��)
	SUB_CUSTOMER_GUANYIN,		//�[�� (�x��)
	SUB_CUSTOMER_ITEQ_START = 7,                   //�����p�Z�s�W����6~1�N���̻���覡�ɻ�
	SUB_CUSTOMER_WUXI = 7,      //�L�� (�p�Z)
	SUB_CUSTOMER_NANYA_START = 1,                     
	SUB_CUSTOMER_NANYA_N4 = SUB_CUSTOMER_NANYA_START, //����n��N4(N4�MN5 ERP�U�o�W�椣�P)
	SUB_CUSTOMER_NANYA_N5							  //����n��N5
};
//--------		SYST		--------------------------
/// <summary>[BATCH=>OPC]��l�ưѼ�</summary>
typedef struct BATCH_SHARE_OPC_INITPARAM_
{
	TCHAR cOPCIP[MAX_BATCH_FIELD_LEN];
	int nRootIdNamespace;
	TCHAR cROOTID[MAX_BATCH_FIELD_LEN];

}BATCH_SHARE_OPC_INITPARAM;
/// <summary>[BATCH=>SYSTCCL]��l�ưѼ�</summary>
typedef struct BATCH_SHARE_SYSTCCL_INITPARAM_
{
	TCHAR cPLCIP[MAX_BATCH_FIELD_LEN];
	long lConnectedStationNo;		//�s�������Ҳկ���
	long lTargetNetworkNo;			//���󯸰��Ҳպ���No
	long lTargetStationNo;			//���󯸰��Ҳկ���
	long lPCNetworkNo;				//�p���������No
	long lPCStationNo;				//�p���������
}BATCH_SHARE_SYSTCCL_INITPARAM;
/// <summary>[BATCH=>SYSTPP]��l�ưѼ�</summary>
struct BATCH_SHARE_SYSTPP_INITPARAM_ : public BATCH_SHARE_SYSTCCL_INITPARAM_ 
{
	int nWatchDogTimeout;			//WatchDog timeout(second)
	int nVersion;		
	int nWSMode;					//0:�Ҧ��@/1:�Ҧ��G
	BOOL bFX5U;
	int nNewbatchDelay;				//
};
/// <summary>�t��Flag�Ѽ�</summary>
enum SYST_RESULT_FLAG
 {
	SRF_REAL_Y_ONE = 0x0001,			//�p�O��ڪ���1
	SRF_REAL_Y_TWO = 0x0002,			//�p�O��ڪ���2
	SRF_REAL_X_ONE = 0x0004,			//�p�O��ڼe��1
	SRF_REAL_X_TWO = 0x0008,			//�p�O��ڼe��2
	SRF_REAL_DIFF_Y_ONE = 0x0010,		//�p�O���׹�ڤ��t1
	SRF_REAL_DIFF_Y_TWO = 0x0020,		//�p�O���׹�ڤ��t2
	SRF_REAL_DIFF_X_ONE = 0x0040,		//�p�O�e�׹�ڤ��t1
	SRF_REAL_DIFF_X_TWO = 0x0080,		//�p�O�e�׹�ڤ��t2
	SRF_REAL_DIFF_XY_ONE = 0x0100,		//�p�O�﨤�u��ڤ��t1
	SRF_REAL_DIFF_XY_TWO = 0x0200,		//�p�O�﨤�u��ڤ��t2
	SRF_FRONT_LEVEL = 0x0400,			//�����P�_�ŧO
	SRF_FRONT_CODE = 0x0800,			//�����P�_�N�X
	SRF_BACK_LEVEL = 0x1000,			//�ϭ��P�_�ŧO
	SRF_BACK_CODE = 0x2000,				//�ϭ��P�_�N�X
	SRF_SIZE_G10 = 0x4000,			    //�ؤo�P�_�ŧO(G10)
	SRF_SIZE_G12 = 0x8000,			    //�ؤo�P�_�ŧO(G12)
	SRF_SIZE_G14 = 0x010000,		    //�ؤo�P�_�ŧO(G14)
	SRF_RESULT_LEVEL = 0x020000,		//�p�O���ƯŧO
	SRF_NUM_AA = 0x040000,				//�p��AA�żƶq
	SRF_NUM_A = 0x080000,				//�p��A�żƶq
	SRF_NUM_P = 0x100000,				//�p��P�żƶq
	SRF_QUALIFY_RATE = 0x200000,		//�q��X��v
	SRF_DIFF_XY = 0x400000,			    //�Ůt����˴���
	SRF_FRONT_SIZE = 0x800000,			//�E�c�椤�����e���j�ʳ��j�p1~5
	SRF_FRONT_LOCATION = 0x1000000,		//�E�c�椤�����e���j�ʳ���m1~5
	SRF_BACK_SIZE = 0x2000000,			//�E�c�椤�ϭ��e���j�ʳ��j�p1~5
	SRF_BACK_LOCATION = 0x4000000,		//�E�c�椤�ϭ��e���j�ʳ���m1~5
	SRF_INDEX = 0x8000000,		        //�p�O�s��
};
/// <summary>�t�Τu���B�~����</summary>
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
	TCHAR cModel[MAX_BATCH_FIELD_LEN];		//�Ҹ�
	TCHAR cAssign[MAX_BATCH_FIELD_LEN];		//���o��
	WORD wAssignNum;						//���o���ƶq
	WORD wSplitNum;							//�@�}�X
	WORD wSplit_One_X;						//�Ĥ@�i�j�p�O�e
	WORD wSplit_One_Y;						//�Ĥ@�i�j�p�O��
	WORD wSplit_Two_X;						//�ĤG�i�j�p�O�e
	WORD wSplit_Two_Y;						//�ĤG�i�j�p�O��
	WORD wSplit_Three_X;					//�ĤT�i�j�p�O�e
	WORD wSplit_Three_Y;					//�ĤT�i�j�p�O��
	float fThickSize;						//�O���p��
	float fThickCCL;						//�ɺ�p��
	TCHAR cCCLType[MAX_BATCH_FIELD_LEN];	//�ɺ�S��
	WORD wGrayScale;						//�ǫ׭�
	WORD wPixel_AA;							//�I�ȭn�DAA
	WORD wPixel_A;							//�I�ȭn�DA
	WORD wPixel_P;							//�I�ȭn�DP
	WORD wDiff_One_Y;						//�Ĥ@�Ӥj�p�O���t(��)
	WORD wDiff_One_X;						//�Ĥ@�Ӥj�p�O���t(�e)
	WORD wDiff_One_XY;						//�Ĥ@�Ӥj�p�O���t(�﨤�u)
	WORD wDiff_One_X_Min;					//�Ĥ@�Ӥj�p���n�V���t�U��
	WORD wDiff_One_X_Max;					//�Ĥ@�Ӥj�p���n�V���t�W��
	WORD wDiff_One_Y_Min;					//�Ĥ@�Ӥj�p���g�V���t�U��
	WORD wDiff_One_Y_Max;					//�Ĥ@�Ӥj�p���g�V���t�W��
	WORD wDiff_One_XY_Min;					//�Ĥ@�Ӥj�p���﨤�u���t�U��
	WORD wDiff_One_XY_Max;					//�Ĥ@�Ӥj�p���﨤�u���t�W��
	WORD wDiff_Two_X_Min;					//�ĤG�Ӥj�p���n�V���t�U��
	WORD wDiff_Two_X_Max;					//�ĤG�Ӥj�p���n�V���t�W��
	WORD wDiff_Two_Y_Min;					//�ĤG�Ӥj�p���g�V���t�U��
	WORD wDiff_Two_Y_Max;					//�ĤG�Ӥj�p���g�V���t�W��
	WORD wDiff_Two_XY_Min;					//�ĤG�Ӥj�p���﨤�u���t�U��
	WORD wDiff_Two_XY_Max;					//�ĤG�Ӥj�p���﨤�u���t�W��
	WORD wDiff_Three_X_Min;					//�ĤT�Ӥj�p���n�V���t�U��
	WORD wDiff_Three_X_Max;					//�ĤT�Ӥj�p���n�V���t�W��
	WORD wDiff_Three_Y_Min;					//�ĤT�Ӥj�p���g�V���t�U��
	WORD wDiff_Three_Y_Max;					//�ĤT�Ӥj�p���g�V���t�W��
	WORD wDiff_Three_XY_Min;				//�ĤT�Ӥj�p���﨤�u���t�U��
	WORD wDiff_Three_XY_Max;				//�ĤT�Ӥj�p���﨤�u���t�W��
	WORD wNum_AA;							//�p��AA�żƶq
	WORD wNO_C06;							//C06�p�O�Ť��s��
	WORD wNO_C10;							//C10�p�O�Ť��s��
	WORD wNO_C12;							//C12�p�O�Ť��s��
}BATCH_SHARE_SYST_PARAMCCL;
typedef struct BATCH_SHARE_SYST_INFO1_
{
	BYTE cSizeReady : 1;	//CCD�ؤo�˴������ǳƦn
	BYTE cSizeRunning : 1;	//CCD�ؤo�˴������B��
	BYTE cCCDReady : 1;		//CCD��{�˴������ǳƦn
	BYTE cCCDRunning : 1;	//CCD��{�˴������B��
	BYTE cReserve1 : 4;		//�O�d���
	BYTE cReserve2;			//�O�d���
}BATCH_SHARE_SYST_INFO1;
typedef struct BATCH_SHARE_SYST_INFO2_
{
	BYTE cCCDError1 : 1;	//CCD��{�˴������G��
	BYTE cSizeError1 : 1;	//CCD�ؤo�˴������G��
	BYTE cReserve1 : 6;		//�O�d���
	BYTE cReserve2;			//�O�d���
}BATCH_SHARE_SYST_INFO2;
struct BATCH_SHARE_SYST_INFO
{
	BATCH_SHARE_SYST_INFO1 xInfo1;
	BATCH_SHARE_SYST_INFO2 xInfo2;
};
typedef struct BATCH_SHARE_SYST_RESULTCCL_
{
	float fReal_One_Y;		        //�p�O��ڪ���1
	float fReal_Two_Y;		        //�p�O��ڪ���2
	float fReal_One_X;		        //�p�O��ڼe��1
	float fReal_Two_X;		        //�p�O��ڼe��2
	WORD wDiff_One_Y;		        //�p�O���׹�ڤ��t1
	WORD wDiff_Two_Y;		        //�p�O���׹�ڤ��t2
	WORD wDiff_One_X;		        //�p�O�e�׹�ڤ��t1
	WORD wDiff_Two_X;		        //�p�O�e�׹�ڤ��t2
	WORD wDiff_One_XY;		        //�p�O�﨤�u��ڤ��t1
	WORD wDiff_Two_XY;		        //�p�O�﨤�u��ڤ��t2
	WORD wFrontLevel;		        //�����P�_�ŧO
	char cFrontCode[30];	        //�����P�_�N�X, �r��ݭn��2������(�^�g�̤p��쬰WORD)
	WORD wBackLevel;		        //�ϭ��P�_�ŧO
	char cBackCode[30];		        //�ϭ��P�_�N�X, �r��ݭn��2������(�^�g�̤p��쬰WORD)
	WORD wSize_G10;			        //�ؤo�P�_�ŧO(G10)
	WORD wSize_G12;			        //�ؤo�P�_�ŧO(G12)
	WORD wSize_G14;			        //�ؤo�P�_�ŧO(G14)
							        
	WORD wResultLevel;		        //�p�����ƯŧO
	WORD wNum_AA;			        //�p��AA�żƶq
	WORD wNum_A;			        //�p��A�żƶq
	WORD wNum_P;			        //�p��P�żƶq
	WORD wQualifyRate;		        //�q��X��v
	WORD wDiff_XY;			        //�Ůt����˴���
	float fFrontDefectSize[5];		//�E�c�椤�����e���j�ʳ��j�p1~5
	float fBackDefectSize[5];		//�E�c�椤�ϭ��e���j�ʳ��j�p1~5
	WORD wFrontDefectLocation[5];	//�E�c�椤�����e���j�ʳ���m1~5
	WORD wBackDefectLocation[5];	//�E�c�椤�ϭ��e���j�ʳ���m1~5
	WORD wIndex;			        //�p�O�s��, �ȪF��Q�K�t�ݦ^��
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
#define MAX_PARAM 3 //�̦h1�}3

typedef struct BATCH_SHARE_EMC_INITPARAM_{
	TCHAR cEMCIP[MAX_BATCH_FIELD_LEN];
	int nEMCPort;
	int nListenPort;
	int nProductType; //0:CCL, 1:PP
}BATCH_SHARE_EMC_INITPARAM;
typedef struct BATCH_SHARE_EMC_BASE_ {
	TCHAR cStation[MAX_BATCH_FIELD_LEN];		//�]�Ƹ�
	TCHAR cMissionID[MAX_BATCH_FIELD_LEN];		//���ȸ�
	TCHAR cBatchName[MAX_BATCH_FIELD_LEN];		//�u�渹
	TCHAR cMaterial[MAX_BATCH_FIELD_LEN];		//�Ƹ�
	TCHAR cSerial[MAX_BATCH_FIELD_LEN];			//�帹
}BATCH_SHARE_EMC_BASE;
typedef struct BATCH_SHARE_EMC_CCLPARAM_ : public BATCH_SHARE_EMC_BASE_ {
	int nNum;										//�ƶq
	int nBookNum;									//BOOK��
	int nSheetNum;									//�i��
	int nSplit;										//�@���X
	TCHAR cMiss[MAX_BATCH_FIELD_LEN];				//�ֲիH���GBOOK��-�i��

	int nStatus;									//���Ȫ��A(CLEAR/START/CLOSED)
	TCHAR cEmpID[MAX_BATCH_FIELD_LEN];				//���u�s��
	int nBeginBook;									//�}�l�ĴXBOOK
	int nEndBook;									//�����ĴXBOOK
	int nBeginSheet;								//�}�l�ĴX�i
	int nEndSheet;									//�����ĴX�i
}BATCH_SHARE_EMC_CCLPARAM;
struct CCLParam{
	int nSize;
	BATCH_SHARE_EMC_CCLPARAM xParam[MAX_PARAM];
};
typedef struct BATCH_SHARE_EMC_CCLRESULT_ : public BATCH_SHARE_EMC_BASE_ {
	int nIndex;										//�˴��i�ƧǸ�
	int nBookNum;									//BOOK��
	TCHAR cSheet[MAX_BATCH_FIELD_LEN];				//Sheet.Index
	TCHAR cDefectType[3];							//���I�N�X
}BATCH_SHARE_EMC_CCLRESULT;
typedef struct BATCH_SHARE_EMC_CCLEND_ : public BATCH_SHARE_EMC_BASE_ {
	int nIndex;										//�˴��i�ƧǸ�
}BATCH_SHARE_EMC_CCLEND;

typedef struct BATCH_SHARE_EMC_PPPARAM_ : public BATCH_SHARE_EMC_BASE_ {
	int nStatus;									//���Ȫ��A(CLEAR/START/CLOSED)
	TCHAR cEmpID[MAX_BATCH_FIELD_LEN];				//���u�s��
}BATCH_SHARE_EMC_PPPARAM;

typedef struct BATCH_SHARE_EMC_PPRESULT_ : public BATCH_SHARE_EMC_BASE_ {
	float fDefectBegin;								//���I�}�l�̼�
	float fDefectEnd;								//���I�����̼�
	TCHAR cDefectType[3];							//���I�N�X
}BATCH_SHARE_EMC_PPRESULT;
typedef struct BATCH_SHARE_EMC_PPEND_ : public BATCH_SHARE_EMC_BASE_ {
	float fLength;									//�C���̼�
}BATCH_SHARE_EMC_PPEND;

enum EMC_ERROR{
	EMC_FIELD_NOTCOMPLETE,	//��줣����
	EMC_TIMEOUT,			//TIMEOUT

};
enum LOG_TYPE{
	LOG_FLOAT,
	LOG_WORD,
	LOG_CSTRING,
	LOG_NONE,
};
typedef struct BATCH_SHARE_EMC_ERRORINFO_  {
	EMC_ERROR eErrorType;								//���~�N�X
	TCHAR cErrorMsg[MAX_BATCH_FIELD_LEN];				//���~�T��
}BATCH_SHARE_EMC_ERRORINFO;

enum SYST_INFO_FIELD{
	SIZE_READY		= 0x01,	 //CCD�ؤo�˴������ǳƦn
	SIZE_RUNNING	= 0x02,	 //CCD�ؤo�˴������B��
	SIZE_ERROR		= 0x04,	 //CCD�ؤo�˴������G��
	CCD_READY		= 0x08,	 //CCD��{�˴������ǳƦn
	CCD_RUNNING		= 0x10,	 //CCD��{�˴������B��
	CCD_ERROR		= 0x20,	 //CCD��{�˴������G��
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