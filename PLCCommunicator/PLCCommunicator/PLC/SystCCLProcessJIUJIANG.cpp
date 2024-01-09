#include "stdafx.h"
#include "SystCCLProcessJIUJIANG.h"

CSystCCLProcessJIUJIANG::CSystCCLProcessJIUJIANG()
{
#ifdef _DEBUG
	const int ctBASE_ADDRESS = 0;
	const int ctBASE_ADDRESS2 = 0;	//�s�W���
#else
	const int ctBASE_ADDRESS = 10000;
	const int ctBASE_ADDRESS2 = 10000;	//�s�W���
#endif
	const PLC_DATA_ITEM_ ctSYST_PLC_FIELD[FIELD_MAX] = {
		{ L"�u��",				FIELD_ORDER,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 0 },
		{ L"�Ƹ�",				FIELD_MATERIAL,				PLC_TYPE_STRING,	ACTION_BATCH,		18,		L"D",		ctBASE_ADDRESS + 6 },
		{ L"�Ҹ�",				FIELD_MODEL,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 1046 },
		{ L"���o��",			FIELD_ASSIGN,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 1071 },
		{ L"���o���ƶq",		FIELD_ASSIGNNUM,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 16 },
		{ L"�@�}�X��",			FIELD_SPLITNUM,				PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 17 },
		{ L"�Ĥ@�i�j�p�O��",	FIELD_SPLIT_ONE_Y,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1020 },
		{ L"�ĤG�i�j�p�O��",	FIELD_SPLIT_TWO_Y,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1024 },
		{ L"�ĤT�i�j�p�O��",	FIELD_SPLIT_THREE_Y,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1028 },
		{ L"�Ĥ@�i�j�p�O�e",	FIELD_SPLIT_ONE_X,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1022 },
		{ L"�ĤG�i�j�p�O�e",	FIELD_SPLIT_TWO_X,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1026 },
		{ L"�ĤT�i�j�p�O�e",	FIELD_SPLIT_THREE_X,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1030 },
		{ L"�O���p��",			FIELD_THICK_SIZE,			PLC_TYPE_FLOAT,		ACTION_BATCH,		4,		L"D",		ctBASE_ADDRESS + 1080 },
		{ L"�ɺ�p��",			FIELD_THICK_CCL,			PLC_TYPE_FLOAT,		ACTION_BATCH,		4,		L"D",		ctBASE_ADDRESS + 1082 },
		{ L"�ɺ�S��",			FIELD_CCL_TYPE,				PLC_TYPE_STRING,	ACTION_BATCH,		6,		L"D",		ctBASE_ADDRESS + 1085 },
		{ L"�ǫ׭�",			FIELD_CCL_GRAYSCALE,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
		{ L"�I�ȭn�D(AA���I��)",FIELD_LEVEL_AA_PIXEL,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1091 },
		{ L"�I�ȭn�D(A���I��)",	FIELD_LEVEL_A_PIXEL,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
		{ L"�I�ȭn�D(P���I��)",	FIELD_LEVEL_P_PIXEL,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },

		{ L"�Ĥ@�Ӥj�p�O���t(��)",		FIELD_DIFF_ONE_Y,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
		{ L"�Ĥ@�Ӥj�p�O���t(�e)",		FIELD_DIFF_ONE_X,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
		{ L"�Ĥ@�Ӥj�p�O�﨤�u���t",	FIELD_DIFF_ONE_XY,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },

		{ L"�Ĥ@�Ӥj�p���g�V���t�U��",	FIELD_DIFF_ONE_Y_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1630 },
		{ L"�Ĥ@�Ӥj�p���g�V���t�W��",	FIELD_DIFF_ONE_Y_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1631 },
		{ L"�Ĥ@�Ӥj�p���n�V���t�U��",	FIELD_DIFF_ONE_X_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1632 },
		{ L"�Ĥ@�Ӥj�p���n�V���t�W��",	FIELD_DIFF_ONE_X_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1633 },
		{ L"�Ĥ@�Ӥj�p���﨤�u���t�U��",FIELD_DIFF_ONE_XY_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1634 },
		{ L"�Ĥ@�Ӥj�p���﨤�u���t�W��",FIELD_DIFF_ONE_XY_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1635 },
		{ L"�ĤG�Ӥj�p���g�V���t�U��",	FIELD_DIFF_TWO_Y_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1636 },
		{ L"�ĤG�Ӥj�p���g�V���t�W��",	FIELD_DIFF_TWO_Y_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1637 },
		{ L"�ĤG�Ӥj�p���n�V���t�U��",	FIELD_DIFF_TWO_X_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1638 },
		{ L"�ĤG�Ӥj�p���n�V���t�W��",	FIELD_DIFF_TWO_X_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1639 },
		{ L"�ĤG�Ӥj�p���﨤�u���t�U��",FIELD_DIFF_TWO_XY_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1640 },
		{ L"�ĤG�Ӥj�p���﨤�u���t�W��",FIELD_DIFF_TWO_XY_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1641 },
		{ L"�ĤT�Ӥj�p���g�V���t�U��",	FIELD_DIFF_THREE_Y_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1642 },
		{ L"�ĤT�Ӥj�p���g�V���t�W��",	FIELD_DIFF_THREE_Y_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1643 },
		{ L"�ĤT�Ӥj�p���n�V���t�U��",	FIELD_DIFF_THREE_X_MIN,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1644 },
		{ L"�ĤT�Ӥj�p���n�V���t�W��",	FIELD_DIFF_THREE_X_MAX,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1645 },
		{ L"�ĤT�Ӥj�p���﨤�u���t�U��",FIELD_DIFF_THREE_XY_MIN,PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1646 },
		{ L"�ĤT�Ӥj�p���﨤�u���t�W��",FIELD_DIFF_THREE_XY_MAX,PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1647 },

		{ L"�p��AA�żƶq",		FIELD_AA_NUM,				PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1605 },
		{ L"���O�U�o",			FIELD_CCL_COMMAND,			PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 1600 },

		{ L"C06�p�O�Ť��s��",	FIELD_CCL_NO_C06,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1200 },
		{ L"C10�p�O�Ť��s��",	FIELD_CCL_NO_C10,			PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 1601 },
		{ L"C12�p�O�Ť��s��",	FIELD_CCL_NO_C12,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 1202 },

		{ L"�p�O��ڪ���1",		FIELD_REAL_Y_ONE,				PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 1324 },
		{ L"�p�O��ڪ���2",		FIELD_REAL_Y_TWO,				PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 1326 },
		{ L"�p�O��ڼe��1",		FIELD_REAL_X_ONE,				PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 1328 },
		{ L"�p�O��ڼe��2",		FIELD_REAL_X_TWO,				PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS + 1330 },
		{ L"�p�O���׹�ڤ��t1", FIELD_REAL_DIFF_ONE_Y,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1332 },
		{ L"�p�O���׹�ڤ��t2", FIELD_REAL_DIFF_TWO_Y,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1334 },
		{ L"�p�O�e�׹�ڤ��t1", FIELD_REAL_DIFF_ONE_X,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1336 },
		{ L"�p�O�e�׹�ڤ��t2", FIELD_REAL_DIFF_TWO_X,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1338 },
		{ L"�p�O�﨤�u��ڤ��t1", FIELD_REAL_DIFF_ONE_XY,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1340 },
		{ L"�p�O�﨤�u��ڤ��t2", FIELD_REAL_DIFF_TWO_XY,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1342 },

		{ L"�����P�_�ŧO",		FIELD_FRONT_LEVEL,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1254 },
		{ L"�����P�_�N�X",		FIELD_FRONT_CODE,			PLC_TYPE_STRING,	ACTION_RESULT,		30,		L"D",		ctBASE_ADDRESS + 1256 },
		{ L"�����ʳ��E�c���m",FIELD_FRONT_LOCATION,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1276 },
		{ L"�ϭ��P�_�ŧO",		FIELD_BACK_LEVEL,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1277 },
		{ L"�ϭ��P�_�N�X",		FIELD_BACK_CODE,			PLC_TYPE_STRING,	ACTION_RESULT,		30,		L"D",		ctBASE_ADDRESS + 1279 },
		{ L"�ϭ��ʳ��E�c���m",FIELD_BACK_LOCATION,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1299 },
		{ L"�ؤo�P�_�ŧO(G10)", FIELD_SIZE_G10,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1321 },
		{ L"�ؤo�P�_�ŧO(G12)", FIELD_SIZE_G12,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1322 },
		{ L"�ؤo�P�_�ŧO(G14)",	FIELD_SIZE_G14,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1323 },
		{ L"�ؤo�˴��Ʀn",		FIELD_SIZE_INFO_1,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0  },
		{ L"�ؤo�˴��B��",		FIELD_SIZE_INFO_2,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0  },
		{ L"CCD�ǳƦn",			FIELD_CCD_INFO_1,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0  },
		{ L"CCD�B��",			FIELD_CCD_INFO_2,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0  },
		{ L"CCD�G��",			FIELD_CCD_ERROR_1,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0  },
		{ L"�ؤo�G��",			FIELD_SIZE_ERROR_1,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0  },

		{ L"���O����",			FIELD_COMMAND_RECEIVED,		PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 1601 },
		{ L"���絲�G",			FIELD_RESULT,				PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 1602 },
		{ L"���絲�G����",		FIELD_RESULT_RECEIVED,		PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 1603 },

		{ L"�p�����ƯŧO",		FIELD_RESULT_LEVEL,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1620 },
		{ L"�p��AA�żƶq",		FIELD_RESULT_AA,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1621 },
		{ L"�p��A�żƶq",		FIELD_RESULT_A,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1622 },
		{ L"�p��P�żƶq",		FIELD_RESULT_P,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1623 },
		{ L"�q��X��v",		FIELD_RESULT_QUALIFYRATE,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1624 },
		{ L"�Ůt����˴���",	FIELD_RESULT_DIFF_XY,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 1625 },
		{ L"�q��MES",			FIELD_RESULT_MES,			PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 1604 },
		{ L"�q��MES�u���T",	FIELD_BATCH_MES,			PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS2 + 700 },
		{ L"�˴��]�w",			FIELD_INSP_SETTING,			PLC_TYPE_STRING,	ACTION_RESULT,		10,		L"D",		ctBASE_ADDRESS2 + 701 },
		{ L"�����]�w",			FIELD_LIGHT_SETTING,		PLC_TYPE_STRING,	ACTION_RESULT,		10,		L"D",		ctBASE_ADDRESS2 + 706 },
		{ L"�˴��}�l�ɶ�",		FIELD_START_TIME,			PLC_TYPE_STRING,	ACTION_RESULT,		18,		L"D",		ctBASE_ADDRESS2 + 711 },
		{ L"�˴������ɶ�",		FIELD_END_TIME,				PLC_TYPE_STRING,	ACTION_RESULT,		18,		L"D",		ctBASE_ADDRESS2 + 720 },
		{ L"�����ʳ��j�p1",		FIELD_FRONT_DEFECT_SIZE_1,	PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS2 + 730 },
		{ L"�����ʳ��j�p2",		FIELD_FRONT_DEFECT_SIZE_2,	PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS2 + 732 },
		{ L"�����ʳ��j�p3",		FIELD_FRONT_DEFECT_SIZE_3,	PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS2 + 734 },
		{ L"�����ʳ��j�p4",		FIELD_FRONT_DEFECT_SIZE_4,	PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS2 + 736 },
		{ L"�����ʳ��j�p5",		FIELD_FRONT_DEFECT_SIZE_5,	PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS2 + 738 },
		{ L"�ϭ��ʳ��j�p1",		FIELD_BACK_DEFECT_SIZE_1,	PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS2 + 740 },
		{ L"�ϭ��ʳ��j�p2",		FIELD_BACK_DEFECT_SIZE_2,	PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS2 + 742 },
		{ L"�ϭ��ʳ��j�p3",		FIELD_BACK_DEFECT_SIZE_3,	PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS2 + 744 },
		{ L"�ϭ��ʳ��j�p4",		FIELD_BACK_DEFECT_SIZE_4,	PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS2 + 746 },
		{ L"�ϭ��ʳ��j�p5",		FIELD_BACK_DEFECT_SIZE_5,	PLC_TYPE_FLOAT,		ACTION_RESULT,		4,		L"D",		ctBASE_ADDRESS2 + 748 },
		{ L"�����ʳ���m1",		FIELD_FRONT_DEFECT_LOCATION_1,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS2 + 750 },
		{ L"�����ʳ���m2",		FIELD_FRONT_DEFECT_LOCATION_2,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS2 + 751 },
		{ L"�����ʳ���m3",		FIELD_FRONT_DEFECT_LOCATION_3,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS2 + 752 },
		{ L"�����ʳ���m4",		FIELD_FRONT_DEFECT_LOCATION_4,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS2 + 753 },
		{ L"�����ʳ���m5",		FIELD_FRONT_DEFECT_LOCATION_5,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS2 + 754 },
		{ L"�ϭ��ʳ���m1",		FIELD_BACK_DEFECT_LOCATION_1,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS2 + 755 },
		{ L"�ϭ��ʳ���m2",		FIELD_BACK_DEFECT_LOCATION_2,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS2 + 756 },
		{ L"�ϭ��ʳ���m3",		FIELD_BACK_DEFECT_LOCATION_3,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS2 + 757 },
		{ L"�ϭ��ʳ���m4",		FIELD_BACK_DEFECT_LOCATION_4,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS2 + 758 },
		{ L"�ϭ��ʳ���m5",		FIELD_BACK_DEFECT_LOCATION_5,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS2 + 759 },
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
	m_pPLC_FIELD_INFO = new PLC_DATA_ITEM_ * [FIELD_MAX];
	for (int i = 0; i < FIELD_MAX; i++)
	{
		m_pPLC_FIELD_INFO[i] = new PLC_DATA_ITEM;
		memset(m_pPLC_FIELD_INFO[i], 0, sizeof(PLC_DATA_ITEM));

		memcpy(m_pPLC_FIELD_INFO[i], &ctSYST_PLC_FIELD[i], sizeof(PLC_DATA_ITEM));
	}
	INIT_PLC_DATA();
}
CSystCCLProcessJIUJIANG::~CSystCCLProcessJIUJIANG()
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
PLC_DATA_ITEM_* CSystCCLProcessJIUJIANG::GetPLCAddressInfo(int nFieldId, BOOL bSkip)
{
	if (nFieldId >= 0 && nFieldId <= FIELD_MAX)
	{
		return m_pPLC_FIELD_INFO[nFieldId];
	}
	return NULL;
}
void CSystCCLProcessJIUJIANG::DoWriteResult(BATCH_SHARE_SYST_RESULTCCL& xData)
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
	WORD wMES = 400;
	SET_PLC_FIELD_DATA(FIELD_RESULT_MES, 2, (BYTE*)&wMES);
	SET_PLC_FIELD_DATA(FIELD_FRONT_DEFECT_SIZE_1, 4, (BYTE*)&xData.fFrontDefectSize[0]);
	SET_PLC_FIELD_DATA(FIELD_FRONT_DEFECT_SIZE_2, 4, (BYTE*)&xData.fFrontDefectSize[1]);
	SET_PLC_FIELD_DATA(FIELD_FRONT_DEFECT_SIZE_3, 4, (BYTE*)&xData.fFrontDefectSize[2]);
	SET_PLC_FIELD_DATA(FIELD_FRONT_DEFECT_SIZE_4, 4, (BYTE*)&xData.fFrontDefectSize[3]);
	SET_PLC_FIELD_DATA(FIELD_FRONT_DEFECT_SIZE_5, 4, (BYTE*)&xData.fFrontDefectSize[4]);
	SET_PLC_FIELD_DATA(FIELD_BACK_DEFECT_SIZE_1, 4, (BYTE*)&xData.fBackDefectSize[0]);
	SET_PLC_FIELD_DATA(FIELD_BACK_DEFECT_SIZE_2, 4, (BYTE*)&xData.fBackDefectSize[1]);
	SET_PLC_FIELD_DATA(FIELD_BACK_DEFECT_SIZE_3, 4, (BYTE*)&xData.fBackDefectSize[2]);
	SET_PLC_FIELD_DATA(FIELD_BACK_DEFECT_SIZE_4, 4, (BYTE*)&xData.fBackDefectSize[3]);
	SET_PLC_FIELD_DATA(FIELD_BACK_DEFECT_SIZE_5, 4, (BYTE*)&xData.fBackDefectSize[4]);
	SET_PLC_FIELD_DATA(FIELD_FRONT_DEFECT_LOCATION_1, 2, (BYTE*)&xData.wFrontDefectLocation[0]);
	SET_PLC_FIELD_DATA(FIELD_FRONT_DEFECT_LOCATION_2, 2, (BYTE*)&xData.wFrontDefectLocation[1]);
	SET_PLC_FIELD_DATA(FIELD_FRONT_DEFECT_LOCATION_3, 2, (BYTE*)&xData.wFrontDefectLocation[2]);
	SET_PLC_FIELD_DATA(FIELD_FRONT_DEFECT_LOCATION_4, 2, (BYTE*)&xData.wFrontDefectLocation[3]);
	SET_PLC_FIELD_DATA(FIELD_FRONT_DEFECT_LOCATION_5, 2, (BYTE*)&xData.wFrontDefectLocation[4]);
	SET_PLC_FIELD_DATA(FIELD_BACK_DEFECT_LOCATION_1, 2, (BYTE*)&xData.wBackDefectLocation[0]);
	SET_PLC_FIELD_DATA(FIELD_BACK_DEFECT_LOCATION_2, 2, (BYTE*)&xData.wBackDefectLocation[1]);
	SET_PLC_FIELD_DATA(FIELD_BACK_DEFECT_LOCATION_3, 2, (BYTE*)&xData.wBackDefectLocation[2]);
	SET_PLC_FIELD_DATA(FIELD_BACK_DEFECT_LOCATION_4, 2, (BYTE*)&xData.wBackDefectLocation[3]);
	SET_PLC_FIELD_DATA(FIELD_BACK_DEFECT_LOCATION_5, 2, (BYTE*)&xData.wBackDefectLocation[4]);
}

void CSystCCLProcessJIUJIANG::DoSetInfoField(BATCH_SHARE_SYST_INFO& xInfo)
{
	//SET_PLC_FIELD_DATA_BIT(FIELD_SIZE_INFO_1, FIELD_CCD_INFO_2, 2, (BYTE*)&xInfo.xInfo1);
	//SET_PLC_FIELD_DATA_BIT(FIELD_CCD_ERROR_1, FIELD_SIZE_ERROR_1, 2, (BYTE*)&xInfo.xInfo2);
}

void CSystCCLProcessJIUJIANG::SetMXParam(IActProgType* pParam, BATCH_SHARE_SYSTCCL_INITPARAM& xData)
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
void CSystCCLProcessJIUJIANG::ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp)
{
	switch (wp)
	{
		case  WM_SYST_EXTRA_CMD:
		{
			BATCH_SYST_EXTRA xExtra;
			memset(&xExtra, 0, sizeof(xExtra));
			if (USM_ReadData((unsigned char*)&xExtra, sizeof(xExtra), 0))
			{
				SET_PLC_FIELD_DATA(FIELD_INSP_SETTING, 10, (BYTE*)&xExtra.cInsp);
				SET_PLC_FIELD_DATA(FIELD_LIGHT_SETTING, 10, (BYTE*)&xExtra.cLight);
				char cTime[18];
				memset(&cTime, 0, sizeof(cTime));
				sprintf_s(cTime, "%S", CTime(xExtra.xStart).Format(L"%y-%m-%d %H-%M-%S"));
				SET_PLC_FIELD_DATA(FIELD_START_TIME, sizeof(cTime), (BYTE*)&cTime);
				sprintf_s(cTime, "%S", CTime(xExtra.xEnd).Format(L"%y-%m-%d %H-%M-%S"));
				SET_PLC_FIELD_DATA(FIELD_END_TIME, sizeof(cTime), (BYTE*)&cTime);
				WORD wMES = 500;
				SET_PLC_FIELD_DATA(FIELD_BATCH_MES, 2, (BYTE*)&wMES);
			}
		}
		break;
	}
	CSystCCLProcessBase::ON_GPIO_NOTIFY(wp, lp);
}