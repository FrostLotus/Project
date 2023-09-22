#include "stdafx.h"
#include "SystCCLProcessCHANGSHU.h"

CSystCCLProcessCHANGSHU::CSystCCLProcessCHANGSHU()
{
	m_pPLC_FIELD_INFO = NULL;
}
CSystCCLProcessCHANGSHU::~CSystCCLProcessCHANGSHU()
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
void CSystCCLProcessCHANGSHU::InitField()
{
	UINT nBaseAddress = GetBaseAddress();
		const PLC_DATA_ITEM_ ctSYST_PLC_FIELD[FIELD_MAX] = {
			{ L"�u��",				FIELD_ORDER,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		nBaseAddress + 1500 },
			{ L"�Ƹ�",				FIELD_MATERIAL,				PLC_TYPE_STRING,	ACTION_BATCH,		18,		L"D",		nBaseAddress + 1534 },
			{ L"�Ҹ�",				FIELD_MODEL,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		nBaseAddress + 1529 },
			{ L"���o��",			FIELD_ASSIGN,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		nBaseAddress + 1505 },
			{ L"���o���ƶq",		FIELD_ASSIGNNUM,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1510 },
			{ L"�@�}�X��",			FIELD_SPLITNUM,				PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1511 },
			{ L"�Ĥ@�i�j�p�O��",	FIELD_SPLIT_ONE_Y,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1512 },
			{ L"�ĤG�i�j�p�O��",	FIELD_SPLIT_TWO_Y,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1514 },
			{ L"�ĤT�i�j�p�O��",	FIELD_SPLIT_THREE_Y,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�Ĥ@�i�j�p�O�e",	FIELD_SPLIT_ONE_X,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1513 },
			{ L"�ĤG�i�j�p�O�e",	FIELD_SPLIT_TWO_X,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1515 },
			{ L"�ĤT�i�j�p�O�e",	FIELD_SPLIT_THREE_X,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�O���p��",			FIELD_THICK_SIZE,			PLC_TYPE_FLOAT,		ACTION_BATCH,		4,		L"D",		nBaseAddress + 1516 },
			{ L"�ɺ�p��",			FIELD_THICK_CCL,			PLC_TYPE_FLOAT,		ACTION_BATCH,		4,		L"D",		nBaseAddress + 1518 },
			{ L"�ɺ�S��",			FIELD_CCL_TYPE,				PLC_TYPE_STRING,	ACTION_BATCH,		6,		L"D",		nBaseAddress + 1520 },  
			{ L"�ǫ׭�",			FIELD_CCL_GRAYSCALE,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1525 },
			{ L"�I�ȭn�D(AA���I��)",FIELD_LEVEL_AA_PIXEL,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1526 },
			{ L"�I�ȭn�D(A���I��)",	FIELD_LEVEL_A_PIXEL,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1527 },
			{ L"�I�ȭn�D(P���I��)",	FIELD_LEVEL_P_PIXEL,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1528 },
																													
			{ L"�Ĥ@�Ӥj�p�O���t(��)",		FIELD_DIFF_ONE_Y,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�Ĥ@�Ӥj�p�O���t(�e)",		FIELD_DIFF_ONE_X,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�Ĥ@�Ӥj�p�O�﨤�u���t",	FIELD_DIFF_ONE_XY,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			
			{ L"�Ĥ@�Ӥj�p���g�V���t�U��",	FIELD_DIFF_ONE_Y_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",	nBaseAddress + 1544 },
			{ L"�Ĥ@�Ӥj�p���g�V���t�W��",	FIELD_DIFF_ONE_Y_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",	nBaseAddress + 1545 },
			{ L"�Ĥ@�Ӥj�p���n�V���t�U��",	FIELD_DIFF_ONE_X_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",	nBaseAddress + 1546 },
			{ L"�Ĥ@�Ӥj�p���n�V���t�W��",	FIELD_DIFF_ONE_X_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",	nBaseAddress + 1547 },
			{ L"�Ĥ@�Ӥj�p���﨤�u���t�U��",FIELD_DIFF_ONE_XY_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",	nBaseAddress + 1548 },
			{ L"�Ĥ@�Ӥj�p���﨤�u���t�W��",FIELD_DIFF_ONE_XY_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",	nBaseAddress + 1549 },
			{ L"�ĤG�Ӥj�p���g�V���t�U��",	FIELD_DIFF_TWO_Y_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",	nBaseAddress + 1550 },
			{ L"�ĤG�Ӥj�p���g�V���t�W��",	FIELD_DIFF_TWO_Y_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",	nBaseAddress + 1551 },
			{ L"�ĤG�Ӥj�p���n�V���t�U��",	FIELD_DIFF_TWO_X_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",	nBaseAddress + 1552 },
			{ L"�ĤG�Ӥj�p���n�V���t�W��",	FIELD_DIFF_TWO_X_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",	nBaseAddress + 1553 },
			{ L"�ĤG�Ӥj�p���﨤�u���t�U��",FIELD_DIFF_TWO_XY_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",	nBaseAddress + 1554 },
			{ L"�ĤG�Ӥj�p���﨤�u���t�W��",FIELD_DIFF_TWO_XY_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",	nBaseAddress + 1555 },
			{ L"�ĤT�Ӥj�p���g�V���t�U��",	FIELD_DIFF_THREE_Y_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",	0 },
			{ L"�ĤT�Ӥj�p���g�V���t�W��",	FIELD_DIFF_THREE_Y_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",	0 },
			{ L"�ĤT�Ӥj�p���n�V���t�U��",	FIELD_DIFF_THREE_X_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",	0 },
			{ L"�ĤT�Ӥj�p���n�V���t�W��",	FIELD_DIFF_THREE_X_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",	0 },
			{ L"�ĤT�Ӥj�p���﨤�u���t�U��",FIELD_DIFF_THREE_XY_MIN,PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",	0 },
			{ L"�ĤT�Ӥj�p���﨤�u���t�W��",FIELD_DIFF_THREE_XY_MAX,PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",	0 },

			{ L"�p��AA�żƶq",		FIELD_AA_NUM,				PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1640 },
			{ L"���O�U�o",			FIELD_CCL_COMMAND,			PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		nBaseAddress + 1558 },

			{ L"C06�p�O�Ť��s��",	FIELD_CCL_NO_C06,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1557 },
			{ L"C10�p�O�Ť��s��",	FIELD_CCL_NO_C10,			PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		nBaseAddress + 1556 },
			{ L"C12�p�O�Ť��s��",	FIELD_CCL_NO_C12,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		nBaseAddress + 1559 },
		
			{ L"�p�O��ڪ���1",		FIELD_REAL_Y_ONE,				PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		nBaseAddress + 1647 },
			{ L"�p�O��ڪ���2",		FIELD_REAL_Y_TWO,				PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		nBaseAddress + 1649 },
			{ L"�p�O��ڼe��1",		FIELD_REAL_X_ONE,				PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		nBaseAddress + 1651 },
			{ L"�p�O��ڼe��2",		FIELD_REAL_X_TWO,				PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		nBaseAddress + 1653 },
			{ L"�p�O���׹�ڤ��t1", FIELD_REAL_DIFF_ONE_Y,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1574 },
			{ L"�p�O���׹�ڤ��t2", FIELD_REAL_DIFF_TWO_Y,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1575 },
			{ L"�p�O�e�׹�ڤ��t1", FIELD_REAL_DIFF_ONE_X,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1576 },
			{ L"�p�O�e�׹�ڤ��t2", FIELD_REAL_DIFF_TWO_X,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1577 },
			{ L"�p�O�﨤�u��ڤ��t1", FIELD_REAL_DIFF_ONE_XY,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1578 },
			{ L"�p�O�﨤�u��ڤ��t2", FIELD_REAL_DIFF_TWO_XY,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1579 },
		
			{ L"�����P�_�ŧO",		FIELD_FRONT_LEVEL,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1580 },
			{ L"�����P�_�N�X",		FIELD_FRONT_CODE,			PLC_TYPE_STRING,	ACTION_RESULT,		30,		L"D",		nBaseAddress + 1581 },
			{ L"�����ʳ��E�c���m",FIELD_FRONT_LOCATION,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1601 },
			{ L"�ϭ��P�_�ŧO",		FIELD_BACK_LEVEL,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1602 },
			{ L"�ϭ��P�_�N�X",		FIELD_BACK_CODE,			PLC_TYPE_STRING,	ACTION_RESULT,		30,		L"D",		nBaseAddress + 1603 },
			{ L"�ϭ��ʳ��E�c���m",FIELD_BACK_LOCATION,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1623 },
			{ L"�ؤo�P�_�ŧO(G10)", FIELD_SIZE_G10,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1624 },
			{ L"�ؤo�P�_�ŧO(G12)", FIELD_SIZE_G12,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1625 },
			{ L"�ؤo�P�_�ŧO(G14)", FIELD_SIZE_G14,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1635 },
			{ L"�ؤo�˴��Ʀn",		FIELD_SIZE_INFO_1,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1626, 0, 0  },
			{ L"�ؤo�˴��B��",		FIELD_SIZE_INFO_2,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1627, 0, 0  },
			{ L"CCD�ǳƦn",			FIELD_CCD_INFO_1,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1628, 0, 0  },
			{ L"CCD�B��",			FIELD_CCD_INFO_2,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1629, 0, 0  },
			{ L"CCD�G��",			FIELD_CCD_ERROR_1,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1630, 0, 0  },
			{ L"�ؤo�G��",			FIELD_SIZE_ERROR_1,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1631, 0, 0  },

			{ L"���O����",			FIELD_COMMAND_RECEIVED,		PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		nBaseAddress + 1632 },
			{ L"���絲�G",			FIELD_RESULT,				PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		nBaseAddress + 1633 },
			{ L"���絲�G����",		FIELD_RESULT_RECEIVED,		PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		nBaseAddress + 1634 },

			{ L"�p�����ƯŧO",		FIELD_RESULT_LEVEL,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1641 },
			{ L"�p��AA�żƶq",		FIELD_RESULT_AA,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1642 },
			{ L"�p��A�żƶq",		FIELD_RESULT_A,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1643 },
			{ L"�p��P�żƶq",		FIELD_RESULT_P,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1644 },
			{ L"�q��X��v",		FIELD_RESULT_QUALIFYRATE,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1645 },
			{ L"�Ůt����˴���",	FIELD_RESULT_DIFF_XY,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		nBaseAddress + 1646 },
			{ L"�q��MES",			FIELD_RESULT_MES,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�q��MES�u���T",	FIELD_BATCH_MES,			PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		0 },
			{ L"�˴��]�w",			FIELD_INSP_SETTING,			PLC_TYPE_STRING,	ACTION_RESULT,		5,		L"D",		0 },
			{ L"�����]�w",			FIELD_LIGHT_SETTING,		PLC_TYPE_STRING,	ACTION_RESULT,		5,		L"D",		0 },
			{ L"�˴��}�l�ɶ�",		FIELD_START_TIME,			PLC_TYPE_STRING,	ACTION_RESULT,		9,		L"D",		0 },
			{ L"�˴������ɶ�",		FIELD_END_TIME,				PLC_TYPE_STRING,	ACTION_RESULT,		9,		L"D",		0 },
			{ L"�����ʳ��j�p1",		FIELD_FRONT_DEFECT_SIZE_1,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�����ʳ��j�p2",		FIELD_FRONT_DEFECT_SIZE_2,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�����ʳ��j�p3",		FIELD_FRONT_DEFECT_SIZE_3,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�����ʳ��j�p4",		FIELD_FRONT_DEFECT_SIZE_4,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�����ʳ��j�p5",		FIELD_FRONT_DEFECT_SIZE_5,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ϭ��ʳ��j�p1",		FIELD_BACK_DEFECT_SIZE_1,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ϭ��ʳ��j�p2",		FIELD_BACK_DEFECT_SIZE_2,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ϭ��ʳ��j�p3",		FIELD_BACK_DEFECT_SIZE_3,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ϭ��ʳ��j�p4",		FIELD_BACK_DEFECT_SIZE_4,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ϭ��ʳ��j�p5",		FIELD_BACK_DEFECT_SIZE_5,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�����ʳ���m1",		FIELD_FRONT_DEFECT_LOCATION_1,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�����ʳ���m2",		FIELD_FRONT_DEFECT_LOCATION_2,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�����ʳ���m3",		FIELD_FRONT_DEFECT_LOCATION_3,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�����ʳ���m4",		FIELD_FRONT_DEFECT_LOCATION_4,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�����ʳ���m5",		FIELD_FRONT_DEFECT_LOCATION_5,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ϭ��ʳ���m1",		FIELD_BACK_DEFECT_LOCATION_1,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ϭ��ʳ���m2",		FIELD_BACK_DEFECT_LOCATION_2,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ϭ��ʳ���m3",		FIELD_BACK_DEFECT_LOCATION_3,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ϭ��ʳ���m4",		FIELD_BACK_DEFECT_LOCATION_4,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ϭ��ʳ���m5",		FIELD_BACK_DEFECT_LOCATION_5,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"CCD(�ؤo)���X�q�渹",		FIELD_CUTTER_ORDER,				PLC_TYPE_STRING,	ACTION_SKIP,		10,		L"D",		0 },
			{ L"CCD(�ؤo)���X���o��",		FIELD_CUTTER_ASSIGN,			PLC_TYPE_STRING,	ACTION_SKIP,		10,		L"D",		0 },
			{ L"CCD(�ؤo)����",		FIELD_CUTTER_Y,				PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"CCD(�ؤo)�e��",		FIELD_CUTTER_X,				PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"CCD(�ؤo)���s��",	FIELD_CUTTER_INDEX,			PLC_TYPE_DWORD,		ACTION_SKIP,		4,		L"D",		0 },
			{ L"���p���X�q�渹",	FIELD_CUTTER_RETURN_ORDER,	PLC_TYPE_STRING,	ACTION_SKIP,		10,		L"D",		0 },
			{ L"���p���X���o��",	FIELD_CUTTER_RETURN_ASSIGN,	PLC_TYPE_STRING,	ACTION_SKIP,		10,		L"D",		0 },
			{ L"���p����",			FIELD_CUTTER_RETURN_Y,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"���p�e��",			FIELD_CUTTER_RETURN_X,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"���p���s��",		FIELD_CUTTER_RETURN_INDEX,	PLC_TYPE_DWORD,		ACTION_SKIP,		4,		L"D",		0 },
		};
		m_pPLC_FIELD_INFO = new PLC_DATA_ITEM_*[FIELD_MAX];
		for (int i = 0; i < FIELD_MAX; i++){ 
			m_pPLC_FIELD_INFO[i] = new PLC_DATA_ITEM;
			memset(m_pPLC_FIELD_INFO[i], 0, sizeof(PLC_DATA_ITEM));

			memcpy(m_pPLC_FIELD_INFO[i], &ctSYST_PLC_FIELD[i], sizeof(PLC_DATA_ITEM));
		}
		INIT_PLCDATA();
}
PLC_DATA_ITEM_* CSystCCLProcessCHANGSHU::GetPLCAddressInfo(int nFieldId, BOOL bSkip)
{
	if (!m_pPLC_FIELD_INFO){
		InitField();
	}
	if (m_pPLC_FIELD_INFO){
		if (nFieldId >= 0 && nFieldId <= FIELD_MAX){
			return m_pPLC_FIELD_INFO[nFieldId];
		}
	}
	return NULL;
}

void CSystCCLProcessCHANGSHU::DoWriteResult(BATCH_SHARE_SYST_RESULTCCL &xData)
{
	SET_PLC_FIELD_DATA(FIELD_SIZE_G10, 2, (BYTE*)&xData.wSize_G10);
	SET_PLC_FIELD_DATA(FIELD_SIZE_G12, 2, (BYTE*)&xData.wSize_G12);
	SET_PLC_FIELD_DATA(FIELD_SIZE_G14, 2, (BYTE*)&xData.wSize_G14);

	SET_PLC_FIELD_DATA(FIELD_RESULT_LEVEL, 2, (BYTE*)&xData.wResultLevel);
	SET_PLC_FIELD_DATA(FIELD_RESULT_AA, 2, (BYTE*)&xData.wNum_AA);
	SET_PLC_FIELD_DATA(FIELD_RESULT_A, 2, (BYTE*)&xData.wNum_A);
	SET_PLC_FIELD_DATA(FIELD_RESULT_P, 2, (BYTE*)&xData.wNum_P);
	SET_PLC_FIELD_DATA(FIELD_RESULT_QUALIFYRATE, 2, (BYTE*)&xData.wQualifyRate);
	SET_PLC_FIELD_DATA(FIELD_RESULT_DIFF_XY, 2, (BYTE*)&xData.wDiff_XY);
}
void CSystCCLProcessCHANGSHU::DoSetInfoField(BATCH_SHARE_SYST_INFO &xInfo)
{
	WORD wTemp = 0;
	wTemp = xInfo.xInfo1.cSizeReady;
	SET_PLC_FIELD_DATA(FIELD_SIZE_INFO_1, 2, (BYTE*)&wTemp);
	wTemp = xInfo.xInfo1.cSizeRunning;
	SET_PLC_FIELD_DATA(FIELD_SIZE_INFO_2, 2, (BYTE*)&wTemp);
	wTemp = xInfo.xInfo1.cCCDReady;
	SET_PLC_FIELD_DATA(FIELD_CCD_INFO_1, 2, (BYTE*)&wTemp);
	wTemp = xInfo.xInfo1.cCCDRunning;
	SET_PLC_FIELD_DATA(FIELD_CCD_INFO_2, 2, (BYTE*)&wTemp);
	wTemp = xInfo.xInfo2.cCCDError1;
	SET_PLC_FIELD_DATA(FIELD_CCD_ERROR_1, 2, (BYTE*)&wTemp);
	wTemp = xInfo.xInfo2.cSizeError1;
	SET_PLC_FIELD_DATA(FIELD_SIZE_ERROR_1, 2, (BYTE*)&wTemp);
}

void CSystCCLProcessCHANGSHU::SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData)
{
	if (pParam){
#ifndef _DEBUG
		//�Ѧ�MX_componentV4_Program Manaual 4.3.3�]�w
		pParam->put_ActConnectUnitNumber(xData.lConnectedStationNo);	//�s�������Ҳկ���,1
		pParam->put_ActCpuType(CPU_Q06UDEHCPU);
		pParam->put_ActDestinationIONumber(0);							//�T�w��0
		pParam->put_ActDestinationPortNumber(5002);						//�T�w��5002
		pParam->put_ActDidPropertyBit(0x01);							//�T�w��1
		pParam->put_ActDsidPropertyBit(0x01);							//�T�w��1
		pParam->put_ActIONumber(0x3FF);									//��CPU��, �T�w��0x3FF
		pParam->put_ActMultiDropChannelNumber(0x00);					//�T�w��0
		pParam->put_ActNetworkNumber(xData.lTargetNetworkNo);			//���󯸰��Ҳպ���No,1	
		pParam->put_ActSourceNetworkNumber(xData.lPCNetworkNo);			//�p���������No,1					
		pParam->put_ActSourceStationNumber(xData.lPCStationNo);			//�p���������,2					
		pParam->put_ActStationNumber(xData.lTargetStationNo);			//���󯸰��Ҳկ���,1		
		pParam->put_ActThroughNetworkType(0x00);
		pParam->put_ActTimeOut(0x100);									//100ms timeout
		pParam->put_ActUnitNumber(0x00);								//�T�w��0
		pParam->put_ActUnitType(UNIT_QJ71E71);
#else
		//�Ѧ�MX_componentV4_Program Manaual 4.3.7�]�w
		pParam->put_ActCpuType(CPU_Q13UDEHCPU);
		pParam->put_ActConnectUnitNumber(0x00);
		pParam->put_ActDestinationIONumber(0x00);				//�T�w��0
		pParam->put_ActDestinationPortNumber(5007);				//�T�w��5007
		pParam->put_ActDidPropertyBit(0x01);					//�T�w��1
		pParam->put_ActDsidPropertyBit(0x01);					//�T�w��1
		pParam->put_ActIntelligentPreferenceBit(0x00);			//�T�w��0
		pParam->put_ActIONumber(0x3FF);							//��CPU��, �T�w��0x3FF
		pParam->put_ActMultiDropChannelNumber(0x00);			//�T�w��0
		pParam->put_ActNetworkNumber(xData.lTargetNetworkNo);	//���󯸰��Ҳպ���No, 0
		pParam->put_ActStationNumber(xData.lTargetStationNo);	//���󯸰��Ҳկ���, 0xFF
		pParam->put_ActThroughNetworkType(0x00);
		pParam->put_ActTimeOut(0x100);							//100ms timeout
		pParam->put_ActUnitNumber(0x00);						//�T�w��0
		pParam->put_ActUnitType(UNIT_QNETHER);
#endif
	}
}