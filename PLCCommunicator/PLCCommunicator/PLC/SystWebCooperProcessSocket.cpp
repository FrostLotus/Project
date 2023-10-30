#include "stdafx.h"
#include "SystWebCooperProcessSocket.h"

CSystWebCooperProcessSocket* CSystWebCooperProcessSocket::m_this = NULL;
CSystWebCooperProcessSocket::CSystWebCooperProcessSocket(ISocketCallBack* pParent, PLC_FRAME_TYPE eFrameType) : CMelsecPlcSocket(pParent, eFrameType)
{
	Init();
}
CSystWebCooperProcessSocket::CSystWebCooperProcessSocket(PLC_FRAME_TYPE eFrameType) : CMelsecPlcSocket(this, eFrameType)
{
	Init();
}
CSystWebCooperProcessSocket::~CSystWebCooperProcessSocket()
{
	Finalize();
}
void CSystWebCooperProcessSocket::Init()
{
	m_this = this;
	m_cLastCmdValue = COMMAND_VALUE;
	m_nBatchDataReceived = 0;
	m_bWaitForData = FALSE;

	const PLC_DATA_ITEM_ ctSYST_PLC_FIELD[FIELD_MAX] = 
	{
		{ L"指令下發",				FIELD_COMMAND,					PLC_TYPE_WORD,		ACTION_NOTIFY, 2,	'D', 0 },

		//{ L"工單",					FIELD_ORDER,					PLC_TYPE_STRING,	ACTION_BATCH, 12,	'D', 9000 },
		//{ L"料號",					FIELD_MATERIAL,					PLC_TYPE_STRING,	ACTION_BATCH, 18,	'D', 9010 },
		{ L"工單",					FIELD_ORDER,					PLC_TYPE_STRING,	ACTION_BATCH, 12,	'D', 100 },
		{ L"料號",					FIELD_MATERIAL,					PLC_TYPE_STRING,	ACTION_BATCH, 18,	'D', 110 },
	};
	m_pPLC_FIELD = new PLC_DATA_ITEM_ * [FIELD_MAX];
	for (int i = 0; i < FIELD_MAX; i++)
	{
		m_pPLC_FIELD[i] = new PLC_DATA_ITEM;
		memcpy(m_pPLC_FIELD[i], &ctSYST_PLC_FIELD[i], sizeof(PLC_DATA_ITEM));
	}
	FIELD_CREATE_MAPPING_MEMORY();
	m_nTimer = ::SetTimer(NULL, TIMERID_QUERYCMD, TIMER_INTERVAL, QueryTimer);
}
void CALLBACK CSystWebCooperProcessSocket::QueryTimer(HWND hwnd, UINT uMsg, UINT_PTR idEvent, DWORD dwTimer)
{
	if (m_this)
	{
		m_this->QueryCmd();
	}
}
void CSystWebCooperProcessSocket::QueryCmd()
{
	GET_PLC_FIELD_DATA(FIELD_COMMAND);
}
void CSystWebCooperProcessSocket::Finalize()
{
	if (m_nTimer)
	{
		::KillTimer(NULL, m_nTimer);
	}
	int nFieldSize = GetFieldSize();
	if (m_pPLC_FIELD)
	{
		for (int i = 0; i < nFieldSize; i++)
		{
			if (m_pPLC_FIELD[i])
			{
				delete[] m_pPLC_FIELD[i];
				m_pPLC_FIELD[i] = NULL;
			}
		}
		delete[] m_pPLC_FIELD;
	}
	FIELD_DESTROY_MAPPING_MEMORY();
	::KillTimer(NULL, m_nTimer);
}
PLC_DATA_ITEM_* CSystWebCooperProcessSocket::GetDataItem(int nFieldId)
{
	if (nFieldId >= 0 && nFieldId < FIELD_MAX)
	{
		return m_pPLC_FIELD[nFieldId];
	}
	return NULL;
}
void CSystWebCooperProcessSocket::HandleAddressValueNotify(PLC_ACTION_FLAG_ eFlag, int nField)
{
	if (eFlag == PLC_ACTION_FLAG_::RESPONSE_OK)
	{
		switch (nField)
		{
			case FIELD_COMMAND:
			{
				int nCurCmdValue = _ttoi(GET_PLC_FIELD_VALUE(FIELD_COMMAND));
				BYTE cCurCmdValue = nCurCmdValue & 0x02;
				if (m_cLastCmdValue == 0 && cCurCmdValue == COMMAND_VALUE)
				{ //get batch data
					m_nBatchDataReceived = 0;
					m_bWaitForData = TRUE;
					GET_PLC_FIELD_DATA(FIELD_ORDER);
					GET_PLC_FIELD_DATA(FIELD_MATERIAL);
				}
				m_cLastCmdValue = cCurCmdValue;
			}
			break;
			case FIELD_ORDER:
			case FIELD_MATERIAL:
				// if received all batch data => call back to aoi for new batch process
				if (m_bWaitForData)
				{
					m_nBatchDataReceived |= 1 << nField;
					if ((m_nBatchDataReceived & (1 << FIELD_ORDER)) &&
						(m_nBatchDataReceived & (1 << FIELD_MATERIAL))
						)
					{
						TRACE(L"all data ready \n");
						NotifyPLCNewBatch(GET_PLC_FIELD_VALUE(FIELD_ORDER), GET_PLC_FIELD_VALUE(FIELD_MATERIAL));
						m_bWaitForData = FALSE;
					}
				}
				break;
			default:
				break;
		}
	}
}