#pragma once
#include "PLCProcessBase.h"

#define SET_STOPPOS				101
#define SET_STOPPOS_SUCCESS		102
#define SET_STOPPOS_FAIL		-1
#define REACH_STOPPOS_FORWARD	201
#define REACH_STOPPOS_BACKWARD	202
#define RECEIVE_STOPPOS			203
#define RESET_ALL				301
#define RESET_ALL_SUCCESS		302
#define MAX_PUTPUT_PIN			16
#define STOP_PIN				0	//�����T���}��

//========	msg, this should be same in QueryStation(PP) =========================
#define WM_QUERYSTATION_RESPONSE	(WM_APP + 201)
#define WM_PLC_ERROR				(WM_APP + 202)//�g�J/Ū��PLCaddress���~

enum PLC_NOTIFY{
	PLC_OPEN,					//open plc
	PLC_VERSION,				//���o������
	PLC_RESET,					//reset current pos/stop pos wp: reset flag
	PLC_PULSELENGTH,			//unit:100ms
	PLC_STOPPOS_FORWARD,		//�U�ostop pos(��) wp: set forward stop pos, lp:stop pos(long)
	PLC_STOPPOS_BACKWARD,		//�U�ostop pos(��) wp: set backward stop pos, lp:stop pos(long)
	PLC_TEST_PIN,				//���տ�Xpin  ex: pin 0/2 high, others low=> 5
		
	PLC_REACH_STOPPOS_FORWARD,	//�����T��Ĳ�o
	PLC_REACH_STOPPOS_BACKWARD,	//�����T��Ĳ�o
	PLC_CHANGE_POS,				//��m�ܰ�
	PLC_CURRENT_POS,			//for base pos, ���o�ثe��m wp: query current pos
	PLC_DELAY_TIME,

	//info
	PLC_SPEED_FORWARD,
	PLC_SPEED_BACKWARD,
	//error
	PLC_STOPPOS_FORWARD_FIELDERROR,
	PLC_STOPPOS_FORWARD_FLAGERROR,
	PLC_STOPPOS_BACKWARD_FIELDERROR,
	PLC_STOPPOS_BACKWARD_FLAGERROR,
};

//========	msg	end	=========================

class CScribdPPProcess :public CPLCProcessBase{
public:
	CScribdPPProcess();
	virtual ~CScribdPPProcess();

	virtual int GetFieldSize(){ return FIELD_MAX; };
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);
	enum PLC_SCRIBD_CCL_FIELD_{
		FIELD_VERSION = 0,					//������
		FIELD_PULSE_LENGTH,					//Pulse Length.���(100ms)
		FIELD_DELAY_TIME,					//�����ɶ�(ms)
		FIELD_CURRENT_POS,					//�ثe��m
		FIELD_PLC_STOPPOS_DELAY_FORWARD,	//������m-�����Z��(��)
		FIELD_PLC_STOPPOS_DELAY_BACKWARD,	//������m+�����Z��(��)
		FIELD_PLC_STOPPOS_FORWARD,			//PLC�ϥΰ�����m(��)
		FIELD_PLC_STOPPOS_BACKWARD,			//PLC�ϥΰ�����m(��)
		FIELD_STOPPOS_FORWARD,				//�U�o�ϥΰ�����m(��)
		FIELD_STOPPOS_BACKWARD,				//�U�o�ϥΰ�����m(��)
		FIELD_FLAG_STOPPOS_FORWARD,			//������m(��)�U�oflag
		FIELD_RESULTFLAG_STOPPOS_FORWARD,	//������m(��)�^��flag
		FIELD_FLAG_STOPPOS_BACKWARD,		//������m(��)�U�oflag
		FIELD_RESULTFLAG_STOPPOS_BACKWARD,	//������m(��)�^��flag
		FIELD_FLAG_STOPPOS,					//������m��Fflag
		FIELD_RESULTFLAG_STOPPOS,			//������m��F�^��flag
		FIELD_FLAG_RESET,					//reset flag
		FIELD_RESULTFLAG_RESET,				//reset �^��flag
		FIELD_OUTPUT_PIN,					//��X�}��
		FIELD_Y0_TEST,						//Y0���ն}��
		FIELD_MAX
	};
protected:
	virtual CPU_SERIES GetCPU(){ return CPU_SERIES::FX5U_SERIES; }
	//IPLCProcess
	virtual long ON_OPEN_PLC(LPARAM lp);
	virtual void ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp);
	virtual void SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData);
private:
	void Init();
	void Finalize();
	static void CALLBACK QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer);
	void ProcessTimer(UINT_PTR nEventId);
	void NotifyAOI(UINT uMsg, WPARAM wp, LPARAM lp);
private:
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
	enum {
		TIMER_CURRENT_POS,			//�ثe��m
		TIMER_SET_STOPPOS_FORWARD,	//�]�wstop pos(��)�^��flag
		TIMER_SET_STOPPOS_BACKWARD, //�]�wstop pos(��)�^��flag
		TIMER_STOPPOS,				//�����^��flag
		TIMER_RESET,				//reset�^��flag
		TIMER_CHECK,				//�U�o���T�{
		TIMER_MAX
	};
	static CScribdPPProcess* m_this;
	UINT_PTR m_tTimerEvent[TIMER_MAX];
	HWND m_hAOIWnd;
};