#pragma once
#include "PLCProcessBase.h"


//rename later, not yet
class CSystFCCLProcess :public CPLCProcessBase{
public:
	CSystFCCLProcess();
	virtual ~CSystFCCLProcess();
	virtual int GetFieldSize(){ return FIELD_MAX; };
	virtual PLC_DATA_ITEM_* GetPLCAddressInfo(int nFieldId, BOOL bSkip);

	enum PLC_SYST_FCCL_FIELD_{
		FIELD_COMMAND = 0,
		FIELD_ORDER,				//訂單號
		FIELD_MATERIAL,				//物料號
		FIELD_MAX
	};
protected:
	virtual void SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData);
private:
	void Init();
	void Finalize();
	static void CALLBACK QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR nEventId, DWORD dwTimer);
	void ProcessTimer(UINT_PTR nEventId);
private:
	PLC_DATA_ITEM_ **m_pPLC_FIELD_INFO;
	enum {
		TIMER_COMMAND,			//指令下發
		TIMER_MAX
	};
	UINT_PTR m_tTimerEvent[TIMER_MAX];
	static CSystFCCLProcess* m_this;
	void ON_FCCL_NEWBATCH();
};