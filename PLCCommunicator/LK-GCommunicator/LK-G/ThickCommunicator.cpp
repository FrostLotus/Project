#include "stdafx.h"
#include "ThickCommunicator.h"

#define CMD_END			0x0D
#define CMD_SPLITER		0x2C

#define TIMERID			1

BEGIN_MESSAGE_MAP(CThickCommunicator, CSerialComBase)
	ON_WM_TIMER()
END_MESSAGE_MAP()

OUT_TYPE GetOutType(BYTE cData)
{
	switch (cData){
	case 0x31:
		return OUT_1;
	case 0x32:
		return OUT_2;
	case 0x30:
		return OUT_BOTH;
	default:
		return OUT_NONE;
	}
}
BYTE GetOutByte(OUT_TYPE eType)
{
	switch (eType)
	{
	case OUT_1:
		return 0x31;
	case OUT_2:
		return 0x32;
	case OUT_BOTH:
		return 0x30;
	default:
		ASSERT(FALSE);
		return 0x00;
	}
}
void CThickCommunicator::OnTimer(UINT_PTR nEventId)
{
	for (int i = 0; i < TIMER_MAX; i++){
		if (nEventId == m_tTimerEvent[i]){
			switch (i){
			case TIMER_QUERY:
#ifdef _DEBUG
				GetCurrentData(OUT_BOTH);
#else
				GetCurrentData(OUT_1);		//現場設定out 1 =A+B
#endif
				break;
			case TIMER_AVERAGE:
			{
				EnterCriticalSection(&m_xLock);
				if (m_vThickInfo.size()){
					float fData[2];
					memset(fData, 0, sizeof(fData));
					for (auto &xInfo : m_vThickInfo){
						fData[0] += xInfo.fData1;
						fData[1] += xInfo.fData2;
					}
					fData[0] /= m_vThickInfo.size();
					fData[1] /= m_vThickInfo.size();

					ON_RECEIVE_THICK(this, m_vThickInfo.at(0).eType, fData);

					m_vThickInfo.clear();
				}
				LeaveCriticalSection(&m_xLock);
			}
				break;
			}
		}
	}
}
CThickCommunicator::CThickCommunicator(UINT nComId, UINT nRate, UINT nTime) : CSerialComBase(nComId)
{
	if (nRate >= 0)
		m_nRate = nRate;
	else
		m_nRate = 200;

	if (nTime >= 0)
		m_nTime = nTime;
	else
		m_nTime = 200;
#ifdef _DEBUG
	m_nRate = 5000;
	m_nTime = 30 * 1000;
#endif
	Init();
}
CThickCommunicator::~CThickCommunicator()
{
	Finalize();
}
void CThickCommunicator::Init()
{
	InitializeCriticalSection(&m_xLock);
	m_dReceiveBufferIndex = 0;
	memset(m_dReceiveBuffer, 0, sizeof(m_dReceiveBuffer));
	memset(m_tTimerEvent, 0, sizeof(m_tTimerEvent));
}
void CThickCommunicator::Finalize()
{
	if (m_hWnd){
		for (int i = 0; i < TIMER_MAX; i++){
			KillTimer(m_tTimerEvent[i]);
		}
	}

	DeleteCriticalSection(&m_xLock);
}
void CThickCommunicator::GetCurrentData(OUT_TYPE eType)
{
	BYTE cCmd[3] = { 0x4D/*M*/, GetOutByte(eType), CMD_END };

	DWORD dwSend = 0;

	SendData(cCmd, sizeof(cCmd), dwSend);
}
void CThickCommunicator::OnSerialEvent(CSerialMFC::EEvent eEvent)
{
	if (IsOpenDevice()){
		DWORD dwBytesRead = 0;

		switch (eEvent)
		{
		case CSerialMFC::EEventRecv:
			DWORD dwRead = 0;
			do
			{
				dwRead = 0;
				ReceiveData(m_dReceiveBuffer + m_dReceiveBufferIndex, sizeof(m_dReceiveBuffer) - m_dReceiveBufferIndex, dwRead);
				if ((int)dwRead>0)
				m_dReceiveBufferIndex += dwRead;
			} while ((int)dwRead > 0); //Not Yet

			if (m_dReceiveBufferIndex > 0){
				ProcessCmd();
			}

			break;
		}
	}
}
void CThickCommunicator::OnComportOpen()
{
	for(int i=0; i < TIMER_MAX; i++){
		CString strMsg;
		int nFreq = 0;
		if(m_tTimerEvent[i] == NULL){
			switch(i){
			case TIMER_QUERY:
				strMsg.Format(L"Sample rate:%d", m_nRate);
				nFreq = m_nRate;

				break;
			case TIMER_AVERAGE:
				strMsg.Format(L"average time:%d", m_nTime);
				nFreq = m_nTime;
				break;
			default:
				ASSERT(FALSE);
				break;
			}
			ON_COMPORT_MSG(this, strMsg);
			m_tTimerEvent[i] = SetTimer(TIMER_AVERAGE + i, nFreq, NULL);
		}
	}
}
void CThickCommunicator::ProcessCmd()
{
	BYTE *pStart = m_dReceiveBuffer;
	for (int i = 0; i < m_dReceiveBufferIndex; i++){
		if (m_dReceiveBuffer[i] == CMD_END){
			int iCmdSize = (int)(&m_dReceiveBuffer[i] - pStart + 1);
			ProcessOneCmd(pStart, iCmdSize);
			pStart = &m_dReceiveBuffer[i + 1];
		}
	}
	int iSize = (int)(&m_dReceiveBuffer[m_dReceiveBufferIndex] - pStart);
	if (iSize >= 0){
		if (pStart != m_dReceiveBuffer){
			memmove(m_dReceiveBuffer, pStart, iSize);
			m_dReceiveBufferIndex = iSize;
			memset(&m_dReceiveBuffer[m_dReceiveBufferIndex], 0, sizeof(m_dReceiveBuffer) - m_dReceiveBufferIndex);
		}
	}
}
void CThickCommunicator::ProcessOneCmd(BYTE *pData, int nSize)
{
	if (nSize >= 2){
		CString strData((char*)pData, nSize);
		CString strCompare;
		LK_GCmdType eCmdType = LKG_NONE;
		for (auto &i : ctCMD){
			if (i.strTitle.GetLength() == 1){
				strCompare = strData.Mid(0, 1);
			}
			else{
				strCompare = strData.Mid(0, 2);
			}
			if (i.strTitle == strCompare){
				switch (i.eCmdType)
				{
				case LKG_ERROR:
					ProcessError(pData, nSize);
					break;
				case LKG_M:
					ProcessThick(pData, nSize);
					break;
				default:
					break;
				}
				break;
			}
		}

	}
}
void CThickCommunicator::ProcessThick(BYTE *pData, int nSize)
{
	if (nSize >= 3){
		ThickInfo xData;
		memset(&xData, 0, sizeof(xData));
		xData.eType = GetOutType(*(pData + 1));;
		BYTE *pStart = pData + 3;
		for (int i = 3; i < nSize; i++){
			if (*(pData + i) == CMD_SPLITER){
				xData.fData1 = (float)atof((const char*)pStart);
				pStart = pData + i + 1;
			}
			else if (*(pData + i) == CMD_END){
				if (xData.eType == OUT_BOTH)
					xData.fData2 = (float)atof((const char*)pStart);
				else
					xData.fData1 = (float)atof((const char*)pStart);
			}
		}

		EnterCriticalSection(&m_xLock);
		m_vThickInfo.push_back(xData);
		LeaveCriticalSection(&m_xLock);

	}
}
void CThickCommunicator::ProcessError(BYTE *pData, int nSize)
{
	if (nSize >= 3){
		CString strMsg, strInfo((char*)pData + 3, nSize - 3);
		strMsg.Format(L"%s Error:%s", GetComId(), strInfo);
		ON_COMPORT_MSG(this, strMsg);
	}
}