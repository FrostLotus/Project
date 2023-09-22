#pragma once
#include "OPCController.h"

class CSystPPProcess :public COPCController{
public:
	CSystPPProcess();
	virtual ~CSystPPProcess();
protected:
	enum OPC_FIELD_{
		FIELD_ORDER_NO = 0,				//订单号	
		FIELD_ORDER_MATERIAL,			//订单物料
		FIELD_ORDER_QUATITY,			//订单数量
		FIELD_FACTORY,					//工厂		
		FIELD_STATION,					//生产机台	
		FIELD_GLUE_TYPE,				//胶水类型	
		FIELD_WEB_SPEC,					//玻璃布規格	
		FIELD_WEB_FACTORY,				//玻璃布廠家
		FIELD_WEB_TYPE,					//玻璃布布種
		FIELD_INSP,						//檢測設定
		FIELD_LIGHT,					//光源設定
		FIELD_LOT_NO,					//Lot.Num

		FIELD_MAX,
	};

	virtual int GetNodeSize(){ return FIELD_MAX; };
	virtual NodeItem *GetNodeItem(int nIndex0Base);

	virtual void UpdateNode(UA_String &uaData, const UA_DataValue *pValue);

private:
	void Init();
	void Finalize();
	static DWORD __stdcall Thread_Process(void* pvoid);

	void ProcessNewBatch();
private:
	NodeItem **m_ppFIELD_INFO;

	enum
	{
		EV_EXIT,
		EV_NEWBATCH,
		EV_COUNT,
		CASE_EXIT = WAIT_OBJECT_0,
		CASE_NEWBATCH,
	};
	HANDLE     m_hThread;
	HANDLE     m_hEvent[EV_COUNT];
};