#include "stdafx.h"
#include "SystCCLProcessDONGGUAN_SONG8.h"

CSystCCLProcessDONGGUAN_SONG8::CSystCCLProcessDONGGUAN_SONG8()
{

#ifdef _DEBUG
		const int ctBASE_ADDRESS = 0;
#else
		const int ctBASE_ADDRESS = 9000;
#endif
		const PLC_DATA_ITEM_ ctSYST_PLC_FIELD[FIELD_MAX] = {
			{ L"�u��",				FIELD_ORDER,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 0 },
			{ L"�Ƹ�",				FIELD_MATERIAL,				PLC_TYPE_STRING,	ACTION_BATCH,		18,		L"D",		ctBASE_ADDRESS + 6 },
			{ L"�Ҹ�",				FIELD_MODEL,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 43 },
			{ L"���o��",			FIELD_ASSIGN,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 112 },
			{ L"���o���ƶq",		FIELD_ASSIGNNUM,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 118 },
			{ L"�@�}�X��",			FIELD_SPLITNUM,				PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 28 },
			{ L"�Ĥ@�i�j�p�O��",	FIELD_SPLIT_ONE_Y,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 49 },
			{ L"�ĤG�i�j�p�O��",	FIELD_SPLIT_TWO_Y,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤT�i�j�p�O��",	FIELD_SPLIT_THREE_Y,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�Ĥ@�i�j�p�O�e",	FIELD_SPLIT_ONE_X,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 50 },
			{ L"�ĤG�i�j�p�O�e",	FIELD_SPLIT_TWO_X,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤT�i�j�p�O�e",	FIELD_SPLIT_THREE_X,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�O���p��",			FIELD_THICK_SIZE,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 55 },
			{ L"�ɺ�p��",			FIELD_THICK_CCL,			PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 56 },
			{ L"�ɺ�S��",			FIELD_CCL_TYPE,				PLC_TYPE_STRING,	ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 58 },  
			{ L"�ǫ׭�",			FIELD_CCL_GRAYSCALE,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 60 },
			{ L"�I�ȭn�D(AA���I��)",FIELD_LEVEL_AA_PIXEL,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 61 },
			{ L"�I�ȭn�D(A���I��)",	FIELD_LEVEL_A_PIXEL,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 62 },
			{ L"�I�ȭn�D(P���I��)",	FIELD_LEVEL_P_PIXEL,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 63 },
																													
			{ L"�Ĥ@�Ӥj�p�O���t(��)",		FIELD_DIFF_ONE_Y,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 64 },
			{ L"�Ĥ@�Ӥj�p�O���t(�e)",		FIELD_DIFF_ONE_X,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 65 },
			{ L"�Ĥ@�Ӥj�p�O�﨤�u���t",	FIELD_DIFF_ONE_XY,	PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 66 },

			{ L"�Ĥ@�Ӥj�p���g�V���t�U��",	FIELD_DIFF_ONE_Y_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�Ĥ@�Ӥj�p���g�V���t�W��",	FIELD_DIFF_ONE_Y_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�Ĥ@�Ӥj�p���n�V���t�U��",	FIELD_DIFF_ONE_X_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�Ĥ@�Ӥj�p���n�V���t�W��",	FIELD_DIFF_ONE_X_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�Ĥ@�Ӥj�p���﨤�u���t�U��",FIELD_DIFF_ONE_XY_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�Ĥ@�Ӥj�p���﨤�u���t�W��",FIELD_DIFF_ONE_XY_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤG�Ӥj�p���g�V���t�U��",	FIELD_DIFF_TWO_Y_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤG�Ӥj�p���g�V���t�W��",	FIELD_DIFF_TWO_Y_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤG�Ӥj�p���n�V���t�U��",	FIELD_DIFF_TWO_X_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤG�Ӥj�p���n�V���t�W��",	FIELD_DIFF_TWO_X_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤG�Ӥj�p���﨤�u���t�U��",FIELD_DIFF_TWO_XY_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤG�Ӥj�p���﨤�u���t�W��",FIELD_DIFF_TWO_XY_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤT�Ӥj�p���g�V���t�U��",	FIELD_DIFF_THREE_Y_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤT�Ӥj�p���g�V���t�W��",	FIELD_DIFF_THREE_Y_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤT�Ӥj�p���n�V���t�U��",	FIELD_DIFF_THREE_X_MIN,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤT�Ӥj�p���n�V���t�W��",	FIELD_DIFF_THREE_X_MAX,	PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤT�Ӥj�p���﨤�u���t�U��",FIELD_DIFF_THREE_XY_MIN,PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�ĤT�Ӥj�p���﨤�u���t�W��",FIELD_DIFF_THREE_XY_MAX,PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },

			{ L"�p��AA�żƶq",		FIELD_AA_NUM,				PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 85 },
			{ L"���O�U�o",			FIELD_CCL_COMMAND,			PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 99 },

			{ L"C06�p�O�Ť��s��",	FIELD_CCL_NO_C06,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"C10�p�O�Ť��s��",	FIELD_CCL_NO_C10,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 294 },
			{ L"C12�p�O�Ť��s��",	FIELD_CCL_NO_C12,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
		
			{ L"�p�O��ڪ���1",		FIELD_REAL_Y_ONE,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 229 },
			{ L"�p�O��ڪ���2",		FIELD_REAL_Y_TWO,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 230 },
			{ L"�p�O��ڼe��1",		FIELD_REAL_X_ONE,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 231 },
			{ L"�p�O��ڼe��2",		FIELD_REAL_X_TWO,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 232 },
			{ L"�p�O���׹�ڤ��t1", FIELD_REAL_DIFF_ONE_Y,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 233 },
			{ L"�p�O���׹�ڤ��t2", FIELD_REAL_DIFF_TWO_Y,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 234 },
			{ L"�p�O�e�׹�ڤ��t1", FIELD_REAL_DIFF_ONE_X,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 235 },
			{ L"�p�O�e�׹�ڤ��t2", FIELD_REAL_DIFF_TWO_X,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 236 },
			{ L"�p�O�﨤�u��ڤ��t1", FIELD_REAL_DIFF_ONE_XY,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 237 },
			{ L"�p�O�﨤�u��ڤ��t2", FIELD_REAL_DIFF_TWO_XY,		PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 238 },
		
			{ L"�����P�_�ŧO",		FIELD_FRONT_LEVEL,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 239 },
			{ L"�����P�_�N�X",		FIELD_FRONT_CODE,			PLC_TYPE_STRING,	ACTION_RESULT,		40,		L"D",		ctBASE_ADDRESS + 240 },
			{ L"�����ʳ��E�c���m",FIELD_FRONT_LOCATION,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0/*ctBASE_ADDRESS + 261*/ },
			{ L"�ϭ��P�_�ŧO",		FIELD_BACK_LEVEL,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 262 },
			{ L"�ϭ��P�_�N�X",		FIELD_BACK_CODE,			PLC_TYPE_STRING,	ACTION_RESULT,		40,		L"D",		ctBASE_ADDRESS + 263 },
			{ L"�ϭ��ʳ��E�c���m",FIELD_BACK_LOCATION,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0/*ctBASE_ADDRESS + 284*/ },
			{ L"�ؤo�P�_�ŧO(G10)", FIELD_SIZE_G10,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 285 },
			{ L"�ؤo�P�_�ŧO(G12)", FIELD_SIZE_G12,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 286 },
			{ L"�ؤo�P�_�ŧO(G14)",	FIELD_SIZE_G14,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 287 },
			{ L"�ؤo�˴��Ʀn",		FIELD_SIZE_INFO_1,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0  },
			{ L"�ؤo�˴��B��",		FIELD_SIZE_INFO_2,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0  },
			{ L"CCD�ǳƦn",			FIELD_CCD_INFO_1,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0  },
			{ L"CCD�B��",			FIELD_CCD_INFO_2,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0  },
			{ L"CCD�G��",			FIELD_CCD_ERROR_1,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0  },
			{ L"�ؤo�G��",			FIELD_SIZE_ERROR_1,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0  },

			{ L"���O����",			FIELD_COMMAND_RECEIVED,		PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 295 },
			{ L"���絲�G",			FIELD_RESULT,				PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 296 },
			{ L"���絲�G����",		FIELD_RESULT_RECEIVED,		PLC_TYPE_WORD,		ACTION_NOTIFY,		2,		L"D",		ctBASE_ADDRESS + 297 },

			{ L"�p�����ƯŧO",		FIELD_RESULT_LEVEL,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�p��AA�żƶq",		FIELD_RESULT_AA,			PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 289 },
			{ L"�p��A�żƶq",		FIELD_RESULT_A,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 290 },
			{ L"�p��P�żƶq",		FIELD_RESULT_P,				PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 291 },
			{ L"�q��X��v",		FIELD_RESULT_QUALIFYRATE,	PLC_TYPE_WORD,		ACTION_RESULT,		2,		L"D",		ctBASE_ADDRESS + 292 },
			{ L"�Ůt����˴���",	FIELD_RESULT_DIFF_XY,		PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�q��MES",			FIELD_RESULT_MES,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�q��MES�u���T",	FIELD_BATCH_MES,			PLC_TYPE_WORD,		ACTION_SKIP,		2,		L"D",		0 },
			{ L"�˴��]�w",			FIELD_INSP_SETTING,			PLC_TYPE_STRING,	ACTION_SKIP,		5,		L"D",		0 },
			{ L"�����]�w",			FIELD_LIGHT_SETTING,		PLC_TYPE_STRING,	ACTION_SKIP,		5,		L"D",		0 },
			{ L"�˴��}�l�ɶ�",		FIELD_START_TIME,			PLC_TYPE_STRING,	ACTION_SKIP,		9,		L"D",		0 },
			{ L"�˴������ɶ�",		FIELD_END_TIME,				PLC_TYPE_STRING,	ACTION_SKIP,		9,		L"D",		0 },
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

			{ L"CCD(�ؤo)���X�q�渹",		FIELD_CUTTER_ORDER,				PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 150 },
			{ L"CCD(�ؤo)���X���o��",		FIELD_CUTTER_ASSIGN,			PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 156 },
			{ L"CCD(�ؤo)����",		FIELD_CUTTER_Y,				PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 162 },
			{ L"CCD(�ؤo)�e��",		FIELD_CUTTER_X,				PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 164 },
			{ L"CCD(�ؤo)���s��",	FIELD_CUTTER_INDEX,			PLC_TYPE_DWORD,		ACTION_BATCH,		4,		L"D",		ctBASE_ADDRESS + 166 },
			{ L"���p���X�q�渹",	FIELD_CUTTER_RETURN_ORDER,	PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 350 },
			{ L"���p���X���o��",	FIELD_CUTTER_RETURN_ASSIGN,	PLC_TYPE_STRING,	ACTION_BATCH,		10,		L"D",		ctBASE_ADDRESS + 356 },
			{ L"���p����",			FIELD_CUTTER_RETURN_Y,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 362 },
			{ L"���p�e��",			FIELD_CUTTER_RETURN_X,		PLC_TYPE_WORD,		ACTION_BATCH,		2,		L"D",		ctBASE_ADDRESS + 364 },
			{ L"���p���s��",		FIELD_CUTTER_RETURN_INDEX,	PLC_TYPE_DWORD,		ACTION_BATCH,		4,		L"D",		ctBASE_ADDRESS + 366 },
		};
		m_pPLC_FIELD_INFO = new PLC_DATA_ITEM_*[FIELD_MAX];
		for (int i = 0; i < FIELD_MAX; i++){ 
			m_pPLC_FIELD_INFO[i] = new PLC_DATA_ITEM;
			memset(m_pPLC_FIELD_INFO[i], 0, sizeof(PLC_DATA_ITEM));

			memcpy(m_pPLC_FIELD_INFO[i], &ctSYST_PLC_FIELD[i], sizeof(PLC_DATA_ITEM));
		}
		INIT_PLCDATA();
}
CSystCCLProcessDONGGUAN_SONG8::~CSystCCLProcessDONGGUAN_SONG8()
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
PLC_DATA_ITEM_* CSystCCLProcessDONGGUAN_SONG8::GetPLCAddressInfo(int nFieldId, BOOL bSkip)
{
	if (nFieldId >= 0 && nFieldId <= FIELD_MAX){
		return m_pPLC_FIELD_INFO[nFieldId];
	}
	return NULL;
}
void CSystCCLProcessDONGGUAN_SONG8::DoWriteResult(BATCH_SHARE_SYST_RESULTCCL &xData)
{
	auto WriteSizeField = [&](int nField, float fSize){
		WORD nSize = (int)fSize;
		SET_PLC_FIELD_DATA(nField, 2, (BYTE*)&nSize);
	};
	WriteSizeField(FIELD_REAL_Y_ONE, xData.fReal_One_Y);
	WriteSizeField(FIELD_REAL_Y_TWO, xData.fReal_Two_Y);
	WriteSizeField(FIELD_REAL_X_ONE, xData.fReal_One_X);
	WriteSizeField(FIELD_REAL_X_TWO, xData.fReal_Two_X);

	SET_PLC_FIELD_DATA(FIELD_SIZE_G10, 2, (BYTE*)&xData.wSize_G10);
	SET_PLC_FIELD_DATA(FIELD_SIZE_G12, 2, (BYTE*)&xData.wSize_G12);

	SET_PLC_FIELD_DATA(FIELD_RESULT_AA, 2, (BYTE*)&xData.wNum_AA);
	SET_PLC_FIELD_DATA(FIELD_RESULT_A, 2, (BYTE*)&xData.wNum_A);
	SET_PLC_FIELD_DATA(FIELD_RESULT_P, 2, (BYTE*)&xData.wNum_P);
	SET_PLC_FIELD_DATA(FIELD_RESULT_QUALIFYRATE, 2, (BYTE*)&xData.wQualifyRate);

	SET_PLC_FIELD_DATA(FIELD_CCL_NO_C10, 2, (BYTE*)&xData.wIndex);
}
void CSystCCLProcessDONGGUAN_SONG8::DoSetInfoField(BATCH_SHARE_SYST_INFO &xInfo)
{
	//SET_PLC_FIELD_DATA_BIT(FIELD_SIZE_INFO_1, FIELD_CCD_INFO_2, 2, (BYTE*)&xInfo.xInfo1);
	//SET_PLC_FIELD_DATA_BIT(FIELD_CCD_ERROR_1, FIELD_SIZE_ERROR_1, 2, (BYTE*)&xInfo.xInfo2);
}

void CSystCCLProcessDONGGUAN_SONG8::SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData)
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
	pParam->put_ActCpuType(CPU_R16CPU);
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
	pParam->put_ActUnitType(UNIT_RETHER);
#endif
}
void CSystCCLProcessDONGGUAN_SONG8::DoCustomAction() //�Ȼs�Ʀ欰
{
	DWORD dwOldIndex = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CUTTER_INDEX));
	GET_PLC_FIELD_DATA(FIELD_CUTTER_INDEX);
	DWORD dwNewIndex = _ttoi(GET_PLC_FIELD_VALUE(FIELD_CUTTER_INDEX));
	if (dwOldIndex != dwNewIndex){
		vector<int> vField = { FIELD_CUTTER_ORDER, FIELD_CUTTER_ASSIGN, FIELD_CUTTER_Y, FIELD_CUTTER_X, FIELD_CUTTER_INDEX };

		GET_PLC_FIELD_DATA(vField);
#if 0  //SET_PLC_FIELD_DATA(RANDOM) does not update UI
		vector<int> vWriteField = { FIELD_CUTTER_RETURN_ORDER, FIELD_CUTTER_RETURN_ASSIGN, FIELD_CUTTER_RETURN_Y, FIELD_CUTTER_RETURN_X, FIELD_CUTTER_RETURN_INDEX };

		int nFieldByte = 0;
		for (auto &i : vField){
			PLC_DATA_ITEM_* pField = GetPLCAddressInfo(i, FALSE);
			if (pField)
				nFieldByte += pField->cLen;
		}
		if (nFieldByte > 0){
			BYTE* pData = new BYTE[nFieldByte];
			memset(pData, 0, nFieldByte);
			BYTE* pCur = pData;
			for (auto &i : vField){
				PLC_DATA_ITEM_* pField = GetPLCAddressInfo(i, FALSE);
				if (pField){
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
		for (auto &i : vField){
			BYTE* pValue = GET_PLC_FIELD_BYTE_VALUE(i);
			int nWriteField = EOF;
			switch (i){
			case FIELD_CUTTER_ORDER:
				nWriteField = FIELD_CUTTER_RETURN_ORDER;
				break;
			case FIELD_CUTTER_ASSIGN:
				nWriteField = FIELD_CUTTER_RETURN_ASSIGN;
				break;
			case FIELD_CUTTER_Y:
				nWriteField = FIELD_CUTTER_RETURN_Y;
				break;
			case FIELD_CUTTER_X:
				nWriteField = FIELD_CUTTER_RETURN_X;
				break;
			case FIELD_CUTTER_INDEX:
				nWriteField = FIELD_CUTTER_RETURN_INDEX;
				break;
			}
			if (nWriteField != EOF){
				PLC_DATA_ITEM_* pField = GetPLCAddressInfo(nWriteField, FALSE);
				if (pField){
					SET_PLC_FIELD_DATA(nWriteField, pField->cLen, pValue);
				}
			}
		}
#endif
	}
}