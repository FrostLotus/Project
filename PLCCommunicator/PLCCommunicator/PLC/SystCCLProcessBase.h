#pragma once
#include "PLCProcessBase.h"
#include <mutex>

#ifdef OFF_LINE
#define USE_TEST_TIMER //timer�T�w�o�e���ո��
#endif
//rename later, not yet
class CSystCCLProcessBase :public CPLCProcessBase
{
public:
	CSystCCLProcessBase();
	virtual ~CSystCCLProcessBase();
	virtual int GetFieldSize() { return FIELD_MAX; };//115

	enum PLC_FIELD_
	{
		FIELD_ORDER = 0,			//�u��
		FIELD_MATERIAL,				//�Ƹ�
		FIELD_MODEL,				//�Ҹ�
		FIELD_ASSIGN,				//���o��
		FIELD_ASSIGNNUM,			//���o���ƶq
		FIELD_SPLITNUM,				//�@�}�X��
		FIELD_SPLIT_ONE_Y,			//�Ĥ@�i�j�p�O��
		FIELD_SPLIT_TWO_Y,			//�ĤG�i�j�p�O��
		FIELD_SPLIT_THREE_Y,		//�ĤT�i�j�p�O��

		FIELD_SPLIT_ONE_X,			//�Ĥ@�i�j�p�O�e
		FIELD_SPLIT_TWO_X,			//�ĤG�i�j�p�O�e
		FIELD_SPLIT_THREE_X,		//�ĤT�i�j�p�O�e
		FIELD_THICK_SIZE,			//�O���p��
		FIELD_THICK_CCL,			//�ɺ�p��
		FIELD_CCL_TYPE,				//�ɺ�S��
		FIELD_CCL_GRAYSCALE,		//�ǫ׭�
		FIELD_LEVEL_AA_PIXEL,		//�I�ȭn�D(AA���I��)
		FIELD_LEVEL_A_PIXEL,		//�I�ȭn�D(A���I��)
		FIELD_LEVEL_P_PIXEL,		//�I�ȭn�D(P���I��)
		FIELD_DIFF_ONE_Y,			//�Ĥ@�Ӥj�p�O���t(��)
		FIELD_DIFF_ONE_X,			//�Ĥ@�Ӥj�p�O���t(�e)
		FIELD_DIFF_ONE_XY,			//�Ĥ@�Ӥj�p�O�﨤�u���t

		FIELD_DIFF_ONE_Y_MIN,		//�Ĥ@�Ӥj�p���g�V���t�U��
		FIELD_DIFF_ONE_Y_MAX,		//�Ĥ@�Ӥj�p���g�V���t�W��
		FIELD_DIFF_ONE_X_MIN,		//�Ĥ@�Ӥj�p���n�V���t�U��
		FIELD_DIFF_ONE_X_MAX,		//�Ĥ@�Ӥj�p���n�V���t�W��
		FIELD_DIFF_ONE_XY_MIN,		//�Ĥ@�Ӥj�p���﨤�u���t�U��
		FIELD_DIFF_ONE_XY_MAX,		//�Ĥ@�Ӥj�p���﨤�u���t�W��

		FIELD_DIFF_TWO_Y_MIN,		//�ĤG�Ӥj�p���g�V���t�U��
		FIELD_DIFF_TWO_Y_MAX,		//�ĤG�Ӥj�p���g�V���t�W��
		FIELD_DIFF_TWO_X_MIN,		//�ĤG�Ӥj�p���n�V���t�U��
		FIELD_DIFF_TWO_X_MAX,		//�ĤG�Ӥj�p���n�V���t�W��
		FIELD_DIFF_TWO_XY_MIN,		//�ĤG�Ӥj�p���﨤�u���t�U��
		FIELD_DIFF_TWO_XY_MAX,		//�ĤG�Ӥj�p���﨤�u���t�W��

		FIELD_DIFF_THREE_Y_MIN,		//�ĤT�Ӥj�p���g�V���t�U��
		FIELD_DIFF_THREE_Y_MAX,		//�ĤT�Ӥj�p���g�V���t�W��
		FIELD_DIFF_THREE_X_MIN,		//�ĤT�Ӥj�p���n�V���t�U��
		FIELD_DIFF_THREE_X_MAX,		//�ĤT�Ӥj�p���n�V���t�W��
		FIELD_DIFF_THREE_XY_MIN,	//�ĤT�Ӥj�p���﨤�u���t�U��
		FIELD_DIFF_THREE_XY_MAX,	//�ĤT�Ӥj�p���﨤�u���t�W��

		FIELD_AA_NUM,				//�p��AA�żƶq
		FIELD_CCL_COMMAND,			//���O�U�o

		FIELD_CCL_NO_C06,			//C06�p�O�Ť��s��(�����Ǹ�)
		FIELD_CCL_NO_C10,			//C10�p�O�Ť��s��(�}����Ǹ�)
		FIELD_CCL_NO_C12,			//C12�p���Ť��s��

		FIELD_REAL_Y_ONE,			//�p�O��ڪ���1
		FIELD_REAL_Y_TWO,			//�p�O��ڪ���2
		FIELD_REAL_X_ONE,			//�p�O��ڼe��1
		FIELD_REAL_X_TWO,			//�p�O��ڼe��2
		FIELD_REAL_DIFF_ONE_Y,		//�p�O���׹�ڤ��t1
		FIELD_REAL_DIFF_TWO_Y,		//�p�O���׹�ڤ��t2
		FIELD_REAL_DIFF_ONE_X,		//�p�O�e�׹�ڤ��t1
		FIELD_REAL_DIFF_TWO_X,		//�p�O�e�׹�ڤ��t2
		FIELD_REAL_DIFF_ONE_XY,		//�p�O�﨤�u��ڤ��t1
		FIELD_REAL_DIFF_TWO_XY,		//�p�O�﨤�u��ڤ��t2

		FIELD_FRONT_LEVEL,			//�����P�_�ŧO(1=AA/2=A/3=P)
		FIELD_FRONT_CODE,			//�����P�_�N�X(G12)
		FIELD_FRONT_LOCATION,		//�����ʳ��E�c���m
		FIELD_BACK_LEVEL,			//�ϭ��P�_�ŧO(1=AA/2=A/3=P)
		FIELD_BACK_CODE,			//�ϭ��P�_�N�X(G12)
		FIELD_BACK_LOCATION,		//�ϭ��ʳ��E�c���m
		FIELD_SIZE_G10,				//�ؤo�P�_�ŧO(G10)
		FIELD_SIZE_G12,				//�ؤo�P�_�ŧO(G12)
		FIELD_SIZE_G14,				//�ؤo�P�_�ŧO(G14)
		FIELD_SIZE_INFO_1,			//�ؤo�˴��ǳƦn
		FIELD_SIZE_INFO_2,			//�ؤo�˴��B��
		FIELD_CCD_INFO_1,			//CCD�ǳƦn
		FIELD_CCD_INFO_2,			//CCD�B��
		FIELD_CCD_ERROR_1,			//CCD�G��
		FIELD_SIZE_ERROR_1,			//�ؤo�G��

		FIELD_COMMAND_RECEIVED,		//���O����
		FIELD_RESULT,				//���絲�G
		FIELD_RESULT_RECEIVED,		//���絲�G����

		FIELD_RESULT_LEVEL,			//�p�����ƯŧO
		FIELD_RESULT_AA,			//�p��AA�żƶq
		FIELD_RESULT_A,				//�p��A�żƶq
		FIELD_RESULT_P,				//�p��P�żƶq
		FIELD_RESULT_QUALIFYRATE,	//�q��X��v
		FIELD_RESULT_DIFF_XY,		//�Ůt����˴���
		FIELD_RESULT_MES,			//�q��MES

		FIELD_BATCH_MES,		    //�q��MES�u���T
		FIELD_INSP_SETTING,		    //�˴��]�w
		FIELD_LIGHT_SETTING,	    //�����]�w
		FIELD_START_TIME,		    //�˴��}�l�ɶ�
		FIELD_END_TIME,			    //�˴������ɶ�
		FIELD_FRONT_DEFECT_SIZE_1,	//�E�c�椤�����e���j�ʳ��j�p1
		FIELD_FRONT_DEFECT_SIZE_2,	//�E�c�椤�����e���j�ʳ��j�p2
		FIELD_FRONT_DEFECT_SIZE_3,	//�E�c�椤�����e���j�ʳ��j�p3
		FIELD_FRONT_DEFECT_SIZE_4,	//�E�c�椤�����e���j�ʳ��j�p4
		FIELD_FRONT_DEFECT_SIZE_5,	//�E�c�椤�����e���j�ʳ��j�p5
		FIELD_BACK_DEFECT_SIZE_1,	//�E�c�椤�ϭ��e���j�ʳ��j�p1
		FIELD_BACK_DEFECT_SIZE_2,	//�E�c�椤�ϭ��e���j�ʳ��j�p2
		FIELD_BACK_DEFECT_SIZE_3,	//�E�c�椤�ϭ��e���j�ʳ��j�p3
		FIELD_BACK_DEFECT_SIZE_4,	//�E�c�椤�ϭ��e���j�ʳ��j�p4
		FIELD_BACK_DEFECT_SIZE_5,	//�E�c�椤�ϭ��e���j�ʳ��j�p5
		FIELD_FRONT_DEFECT_LOCATION_1,	//�E�c�椤�����e���j�ʳ���m1
		FIELD_FRONT_DEFECT_LOCATION_2,	//�E�c�椤�����e���j�ʳ���m2
		FIELD_FRONT_DEFECT_LOCATION_3,	//�E�c�椤�����e���j�ʳ���m3
		FIELD_FRONT_DEFECT_LOCATION_4,	//�E�c�椤�����e���j�ʳ���m4
		FIELD_FRONT_DEFECT_LOCATION_5,	//�E�c�椤�����e���j�ʳ���m5
		FIELD_BACK_DEFECT_LOCATION_1,	//�E�c�椤�ϭ��e���j�ʳ���m1
		FIELD_BACK_DEFECT_LOCATION_2,	//�E�c�椤�ϭ��e���j�ʳ���m2
		FIELD_BACK_DEFECT_LOCATION_3,	//�E�c�椤�ϭ��e���j�ʳ���m3
		FIELD_BACK_DEFECT_LOCATION_4,	//�E�c�椤�ϭ��e���j�ʳ���m4
		FIELD_BACK_DEFECT_LOCATION_5,	//�E�c�椤�ϭ��e���j�ʳ���m5

		FIELD_CUTTER_ORDER,				//F4->CCD(�ؤo)���X�q�渹
		FIELD_CUTTER_ASSIGN,			//F4->CCD(�ؤo)���X���o��
		FIELD_CUTTER_Y,					//F4->CCD(�ؤo)����
		FIELD_CUTTER_X,					//F4->CCD(�ؤo)�e��
		FIELD_CUTTER_INDEX,				//F4->CCD(�ؤo)���s��

		FIELD_CUTTER_RETURN_ORDER,		//CCD->���p���X�q�渹
		FIELD_CUTTER_RETURN_ASSIGN,		//CCD->���p���X���o��
		FIELD_CUTTER_RETURN_Y,			//CCD->���p����
		FIELD_CUTTER_RETURN_X,			//CCD->���p�e��
		FIELD_CUTTER_RETURN_INDEX,		//CCD->���p���s��

		FIELD_MAX                       //FIELD�`��
	};
	
protected:
	virtual long ON_OPEN_PLC(LPARAM lp);
	virtual void ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp);

	virtual void DoWriteResult(BATCH_SHARE_SYST_RESULTCCL &xData) = 0;
	virtual void DoSetInfoField(BATCH_SHARE_SYST_INFO &xInfo) = 0;
	virtual BOOL IS_SUPPORT_FLOAT_REALSIZE(){ return TRUE; }; //�F��Q�K�t��ڤؤo��쫬�A��word, �Dfloat

	virtual BOOL IS_SUPPORT_CUSTOM_ACTION() { return FALSE; } //�O�_�䴩�Ȼs�Ʀ欰
	virtual void DoCustomAction(){}; //�Ȼs�Ʀ欰
private:
	void Init();
	void Finalize();
	void ProcessAOIResponse(LPARAM lp);
	void ProcessResult();
	static void CALLBACK QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer);
	void ProcessTimer(UINT_PTR nEventId);

	void ON_CCL_NEWBATCH();
	void ON_C10_CHANGE(WORD wC10);

	
	void PushResult(BATCH_SHARE_SYST_RESULTCCL &xResult);
	void SetInfoField(BATCH_SHARE_SYST_INFO &xInfo);
private:

	enum 
	{
		TIMER_COMMAND,			//���O�U�o
		TIMER_COMMAND_RECEIVED,	//���O����
		TIMER_RESULT,			//���絲�G
		TIMER_RESULT_RECEIVED,	//���絲�G����
		TIMER_C10,				//C10�Ť��p���s��
		TIMER_CUSTOM_ACTION,	//�Ȼs�Ʀ欰
#ifdef USE_TEST_TIMER
		TIMER_TEST,
#endif
		TIMER_MAX
	};
	UINT_PTR m_tTimerEvent[TIMER_MAX];
	static CSystCCLProcessBase* m_this;

	//write thread
	enum
	{
		EV_EXIT,
		EV_WRITE,
		EV_COUNT,
		CASE_EXIT = WAIT_OBJECT_0,
		CASE_WRITE,
	};
	vector<BATCH_SHARE_SYST_RESULTCCL> m_vResult;
	std::mutex m_oMutex;
	HANDLE     m_hThread;
	HANDLE     m_hEvent[EV_COUNT];

	static DWORD __stdcall Thread_Result(void* pvoid);
	void WriteResult(BATCH_SHARE_SYST_RESULTCCL &xData);
};
