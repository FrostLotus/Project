#pragma once
#include "PLCProcessBase.h"

#define TECHAIN_NAME L"SENSOR_VIEWER"

enum PLC_TECHAIN_MESSAGE{
	PTM_VERSION_ERROR,
	//PTM_START,				//�}�l
	//PTM_END,				//����
	PTM_VALVE_SWITCH1,		//��(�}��)1
	PTM_VALVE_SWITCH2,		//��(�}��)2
	PTM_ELE,				//�q�ϻ�
	PTM_ANCHOR_ANALOG,		//�w���X(����)
	PTM_ANCHOR_DIGITAL,		//�w���X(�Ʀ�)
	PTM_VALVE_VALUE1,		//��(�ƭ�)1
	PTM_VALVE_VALUE2,		//��(�ƭ�)2
	PTM_PRESSURE,			//���O
	PTM_TEMPERATURE,		//���q�ū�
	PTM_DRIVE,				//�X�ʾ����O
	PTM_FEEDBACK,			//�}�צ^�X
	PTM_VOC_1,				//VOC1
	PTM_VOC_2,				//VOC2
};
class CTechainProcess :public CPLCProcessBase{
public:
	CTechainProcess();
	virtual ~CTechainProcess();
	virtual int GetFieldSize() { return FIELD_MAX; };

	enum PLC_FIELD_{
		FIELD_BEGIN = 0,
		FIELD_VERSION = FIELD_BEGIN,
		FIELD_VALVE_SWITCH1,	//��(�}��)1
		FIELD_VALVE_SWITCH2,	//��(�}��)2
		FIELD_ANCHOR_DIGITAL,	//�w���X(�Ʀ�)
		FIELD_ELE,				//�q�ϻ�
		FIELD_ANCHOR_ANALOG,	//�w���X(����)
		FIELD_VALVE_VALUE1,		//CH1:��(�ƭ�)1
		FIELD_VALVE_VALUE2,		//CH2:��(�ƭ�)2
		FIELD_PRESSURE,			//CH3:���O
		FIELD_TEMPERATURE,		//CH4:���q�ū�
		FIELD_DRIVE,			//CH5:�X�ʾ����O
		FIELD_FEEDBACK,			//CH6:�}�צ^�X
		FIELD_VOC_1,			//CH7:VOC1
		FIELD_VOC_2,			//CH8:VOC2

		FIELD_MAX
	};
	virtual BOOL HAS_CUSTOM_TEST() { return TRUE; }
protected:
	virtual CPU_SERIES GetCPU(){ return CPU_SERIES::FX5U_SERIES; }	
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);
	virtual void SET_INIT_PARAM(LPARAM lp, BYTE *pData);
	virtual long ON_OPEN_PLC(LPARAM lp);
	virtual void ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp);
	//IPLCProcess
	virtual void SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData);

private:
	void Init();
	void Finalize();
	void NotifyMainProcess(WPARAM wp, LPARAM lp);
	static void CALLBACK QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer);
	void ProcessTimer(UINT_PTR nEventId);
private:
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
	HWND m_hMainProcessWnd;
	int m_nVersion;

	UINT_PTR m_tTimerEvent;
	static CTechainProcess* m_this;
};
