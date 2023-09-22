#pragma once

#include "SerialComBase.h"
#include <vector>
#define MAX_RECEIVE_BUFFER_SIZE 64000


enum LK_GCmdType{
	LKG_NONE,
	LKG_M,	//���q�ȿ�X
	LKG_T,	//�w��on/off
	LKG_V,	//�۰��k�son
	LKG_W,	//�۰��k�Boff
	LKG_VR,	//�]�w
	LKG_KL,	//���O��
	LKG_PW,	//�{�ǧ��
	LKG_PR,	//�{���ˬd
	LKG_DO,	//�έp���G��X
	LKG_DQ,	//�M���έp
	LKG_AS,	//�}�l�ƾ��x�s
	LKG_AP,	//����ƾ��x�s
	LKG_AQ,	//��l�Ƽƾ��x�s
	LKG_AO,	//��X�ƾ�
	LKG_AN,	//�ƾ��x�s�֭p���A��X
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