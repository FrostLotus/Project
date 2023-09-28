#pragma once
#include "MelsecPlcSocket.h"

#define COMMAND_VALUE	2

//for 東莞生益(軟板)
class CSystWebCooperProcessSocket : 
	public CMelsecPlcSocket
{
public:
	CSystWebCooperProcessSocket(PLC_FRAME_TYPE eFrameType);
	CSystWebCooperProcessSocket(ISocketCallBack *pParent, PLC_FRAME_TYPE eFrameType);
	~CSystWebCooperProcessSocket();

	void QueryCmd(void);
	static void CALLBACK QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR idEvent, DWORD dwTimer);

	enum PLC_DONGGUAN_FIELD_{
		FIELD_COMMAND = 0,				
		FIELD_ORDER,				//訂單號
		FIELD_MATERIAL,				//物料號
		FIELD_MAX
	};

protected:
	virtual int GetFieldSize() { return FIELD_MAX; };
	virtual PLC_DATA_ITEM_* GetDataItem(int nFieldId);
	virtual void HandleAddressValueNotify(PLC_ACTION_FLAG_ eFlag, int nField);
	virtual void HandleWriteAction(PLC_ACTION_FLAG_ eFlag, int nField){};
private:
	void Init();
	void Finalize();
private:
	PLC_DATA_ITEM_ **m_pPLC_FIELD;
	UINT_PTR m_nTimer;
	static CSystWebCooperProcessSocket* m_this;
	int m_nBatchDataReceived;
	BYTE m_cLastCmdValue;
	BOOL m_bWaitForData;
};

