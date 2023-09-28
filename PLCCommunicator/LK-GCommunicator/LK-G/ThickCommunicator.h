#pragma once

#include "SerialComBase.h"
#include <vector>
#define MAX_RECEIVE_BUFFER_SIZE 64000


enum LK_GCmdType{
	LKG_NONE,
	LKG_M,	//測量值輸出
	LKG_T,	//定時on/off
	LKG_V,	//自動歸零on
	LKG_W,	//自動歸劉off
	LKG_VR,	//設定
	LKG_KL,	//面板鎖
	LKG_PW,	//程序更改
	LKG_PR,	//程序檢查
	LKG_DO,	//統計結果輸出
	LKG_DQ,	//清除統計
	LKG_AS,	//開始數據儲存
	LKG_AP,	//停止數據儲存
	LKG_AQ,	//初始化數據儲存
	LKG_AO,	//輸出數據
	LKG_AN,	//數據儲存累計狀態輸出
	LKG_ERROR,	//error
	LKG_MAX,
};

struct LK_GTITLE{
	LK_GCmdType eCmdType;
	CString strTitle;
};

const LK_GTITLE ctCMD[] = {
	{ LKG_M,		L"M"	},
	{ LKG_T,		L"T"	},
	{ LKG_V,		L"V"	},
	{ LKG_W,		L"W"	},
	{ LKG_VR,		L"VR"	},
	{ LKG_KL,		L"KL"	},
	{ LKG_PW,		L"PW"	},
	{ LKG_PR,		L"PR"	},
	{ LKG_DO,		L"DO"	},
	{ LKG_DQ,		L"DQ"	},
	{ LKG_AS,		L"AS"	},
	{ LKG_AP,		L"AP"	},
	{ LKG_AQ,		L"AQ"	},
	{ LKG_AO,		L"AO"	},
	{ LKG_AN,		L"AN"	},
	{ LKG_ERROR,	L"ER"	},
};	 


class CThickCommunicator : public CSerialComBase{
public:
	CThickCommunicator(UINT nComId, UINT nRate, UINT nTime);
	~CThickCommunicator();
protected:
	virtual void OnSerialEvent(CSerialMFC::EEvent eEvent);
	virtual void OnComportOpen();
	afx_msg void OnTimer(UINT_PTR);
	DECLARE_MESSAGE_MAP()
private:
	void Init();
	void Finalize();
	void ProcessCmd();
	void ProcessOneCmd(BYTE *pData, int nSize);
	void ProcessThick(BYTE *pData, int nSize);
	void ProcessError(BYTE *pData, int nSize);

	void GetCurrentData(OUT_TYPE eType);
private:
	int m_dReceiveBufferIndex;
	BYTE m_dReceiveBuffer[MAX_RECEIVE_BUFFER_SIZE];
	enum{
		TIMER_QUERY,
		TIMER_AVERAGE,
		TIMER_MAX
	};
	UINT_PTR m_tTimerEvent[TIMER_MAX];
	UINT m_nRate;
	UINT m_nTime;

	struct ThickInfo{
		OUT_TYPE eType;
		float fData1;
		float fData2;
	};
	std::vector<ThickInfo> m_vThickInfo;

	CRITICAL_SECTION m_xLock;
};