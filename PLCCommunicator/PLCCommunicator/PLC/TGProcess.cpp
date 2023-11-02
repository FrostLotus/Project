#include "stdafx.h"
#include "TGProcess.h"
#include "PLCCommunicator.h"

CTGProcess::CTGProcess()
{
	Init();
}
CTGProcess::~CTGProcess()
{
	Finalize();
}
PLC_DATA_ITEM_* CTGProcess::GetPLCAddressInfo(int nFieldId, BOOL bSkip)
{
	int nBase = CSystPPProcess::GetFieldSize();
	if (nFieldId < nBase)
	{
		return CSystPPProcess::GetPLCAddressInfo(nFieldId, bSkip);
	}
	else
	{
		int nNewId = nFieldId - nBase;
		if (nNewId >= 0 && nNewId <= FIELD_MAX)
		{
			return m_pPLC_FIELD_INFO[nNewId];
		}
		return NULL;
	}
}
void CTGProcess::Init()
{
	const PLC_DATA_ITEM_ ctSYST_PLC_FIELD[FIELD_MAX] = 
	{
	{ L"反轉距離",			FIELD_REVERSE_DIST,			PLC_TYPE_DWORD,	 ACTION_BATCH,	4, L"D",		111	},
	{ L"開始反轉",			FIELD_REVERSE_BEGIN,		PLC_TYPE_BIT,	 ACTION_BATCH,	2, L"M",		102	},
	{ L"反轉結束",			FIELD_REVERSE_END,			PLC_TYPE_BIT,	 ACTION_BATCH,	2, L"M",		103	},
	{ L"目前方向(1:正/2:反)",	FIELD_CURRENT_DIR,			PLC_TYPE_WORD,	 ACTION_BATCH,	2, L"D",		109	},
	{ L"上次方向",			FIELD_LAST_DIR,				PLC_TYPE_WORD,	 ACTION_BATCH,	2, L"D",		108	},
	{ L"開始反轉位置",		FIELD_BEGIN_REVERSE_POS,	PLC_TYPE_DWORD,	 ACTION_BATCH,	4, L"D",		102	},
	{ L"目前位置",			FIELD_CURRENT_POS,			PLC_TYPE_DWORD,	 ACTION_BATCH,	4, L"U2\\G",	20	},
	};
	m_pPLC_FIELD_INFO = new PLC_DATA_ITEM_ * [FIELD_MAX];
	for (int i = 0; i < FIELD_MAX; i++)
	{
		m_pPLC_FIELD_INFO[i] = new PLC_DATA_ITEM;
		memset(m_pPLC_FIELD_INFO[i], 0, sizeof(PLC_DATA_ITEM));
		memcpy(m_pPLC_FIELD_INFO[i], &ctSYST_PLC_FIELD[i], sizeof(PLC_DATA_ITEM));
	}
}
void CTGProcess::Finalize()
{
	int nFieldSize = FIELD_MAX;
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
void CTGProcess::OnQueryTimer()
{
	CSystPPProcess::OnQueryTimer();
	//-----------------------------------------
	vector<int> vField = { FIELD_REVERSE_DIST, FIELD_BEGIN_REVERSE_POS, FIELD_REVERSE_BEGIN, FIELD_REVERSE_END, FIELD_CURRENT_DIR, FIELD_LAST_DIR, FIELD_CURRENT_POS };

	int nOldReverseBegin = _ttoi(GET_PLC_FIELD_VALUE(FIELD_REVERSE_BEGIN));

	GET_PLC_FIELD_DATA(vField);

	for (auto& i : vField)
	{
		int nCur = _ttoi(GET_PLC_FIELD_VALUE(i));
		switch (i)
		{
			case FIELD_REVERSE_END:
				if (nCur != 0)
				{
					//if status change, reset status
					WORD wValue = 0;
					SET_PLC_FIELD_DATA(i, 2, (BYTE*)&wValue);
					//notify AOI with reverse distance
					int nDist = _ttoi(GET_PLC_FIELD_VALUE(FIELD_REVERSE_DIST));
					CString strLog;
					strLog.Format(L"end Reverse, current pos %s, reverse Dist %d", GET_PLC_FIELD_VALUE(FIELD_CURRENT_POS), nDist);
					theApp.InsertDebugLog(strLog, LOG_DEBUG);
					TRACE(L"%s \n", strLog);
					NotifyAOI(WM_PLC_REVERSE_END, nDist);
				}
				break;
			case FIELD_REVERSE_BEGIN:
				if (nOldReverseBegin == 0 && nCur == 1)
				{
					//notify AOI reverse begin
					CString strLog;
					int nBeginReverse = _ttoi(GET_PLC_FIELD_VALUE(FIELD_BEGIN_REVERSE_POS));
					strLog.Format(L"Begin Reverse, pos %d", nBeginReverse);
					theApp.InsertDebugLog(strLog, LOG_DEBUG);
					TRACE(L"%s \n", strLog);
					NotifyAOI(WM_PLC_REVERSE_BEGIN, NULL);
				}
				break;
		}
	}
}
long CTGProcess::SET_PLC_FIELD_DATA(int nFieldId, int nSizeInByte, BYTE* pData)
{
	return CSystPPProcess::SET_PLC_FIELD_DATA(nFieldId + CSystPPProcess::GetFieldSize(), nSizeInByte, pData);
}
CString CTGProcess::GET_PLC_FIELD_VALUE(int nFieldId)
{
	return CSystPPProcess::GET_PLC_FIELD_VALUE(nFieldId + CSystPPProcess::GetFieldSize());
}
long CTGProcess::GET_PLC_FIELD_DATA(vector<int> vField)
{
	for (auto& nField : vField) nField += CSystPPProcess::GetFieldSize();
	return CSystPPProcess::GET_PLC_FIELD_DATA(vField);
}
int CTGProcess::GetFieldSize()
{
	return CSystPPProcess::GetFieldSize() + FIELD_MAX;//9+7=16
}