#include "stdafx.h"
#include "EMCSocket.h"
#include "EMCCommunicator.h"
EMCSocket::EMCSocket()
{
	Init();
}
EMCSocket::~EMCSocket()
{
	m_bCheckBuffer = FALSE;
	WaitForSingleObject(m_hTimeoutThread, INFINITE);
}
void EMCSocket::CheckTimeout()
{
	std::lock_guard< std::mutex > lock(m_oMutex);
	LARGE_INTEGER xNow;
	QueryPerformanceCounter(&xNow);
	double dDiff = (xNow.QuadPart - m_xLastReceive.QuadPart) * 1000.0 / m_xFreq.QuadPart;
	if (dDiff >= TIMEOUT_PERIOD){
		if (m_nDataSize){
			//timeout
			CommandTimeout(CString(m_cDataBuf));
			memset(m_cDataBuf, 0, m_nDataSize);
			m_nDataSize = 0;
		}
	}
}
DWORD EMCSocket::TimeoutThread(void *pParam)
{
	EMCSocket *pThis = (EMCSocket*)pParam;
	while (pThis && pThis->NeedCheckBuffer()){
		pThis->CheckTimeout();
		::Sleep(1);
	}
	TRACE(L"TimeoutThread end \n");
	return 0;
}
void EMCSocket::OnReceive(int nErrorCode)
{
	std::lock_guard< std::mutex > lock(m_oMutex);
	int nRead = 0;
	nRead = Receive(m_cDataBuf + m_nDataSize, sizeof(m_cDataBuf) - m_nDataSize);
#ifdef _DEBUG
	CString strLog;
	for (int i = 0; i < nRead; i++){
		CString strTemp;
		strTemp.Format(L"%02X ", *(m_cDataBuf + m_nDataSize + i));
		strLog += strTemp;
	}
	theApp.InsertDebugLog(strLog, LOG_EMCDEBUG);
#endif
	bool bSuccess = true;
	switch (nRead){
	case 0:
		Close();
		bSuccess = false;
		break;
	case SOCKET_ERROR:
		if (GetLastError() != WSAEWOULDBLOCK){
			AfxMessageBox(_T("Error occurred"));
			Close();
			bSuccess = false;
		}
		break;
	default:
		m_nDataSize += nRead;
		break;
	}

	if (bSuccess){
		QueryPerformanceCounter(&m_xLastReceive);
		ProcessCmd();
	}
	
	CAsyncSocket::OnReceive(nErrorCode);
}
void EMCSocket::OnSend(int nErrorCode)
{

}
void EMCSocket::OnClose(int nErrorCode)
{
	CAsyncSocket::OnClose(nErrorCode);
}
void EMCSocket::Init()
{
	m_bCheckBuffer = TRUE;
	QueryPerformanceFrequency(&m_xFreq);
	memset(&m_xLastReceive, 0, sizeof(m_xLastReceive));
	m_nDataSize = 0;
	memset(m_cDataBuf, 0, sizeof(m_cDataBuf));
	if (!AfxSocketInit()){
		AfxMessageBox(_T("Failed to Initialize Sockets"), MB_OK | MB_ICONSTOP);
	}
	m_hTimeoutThread = ::CreateThread(NULL, NULL, TimeoutThread, this, NULL, NULL);
}

void EMCSocket::ProcessCmd()
{
	BYTE *pStart = m_cDataBuf;
	int nDataLen = 0;
		
	PRODUCT_TYPE eType = GetProductType();
	if (eType == PRODUCT_TYPE::PP){
		nDataLen = CEMCParser::CheckPPData(CString(pStart));
	}
	else if (eType == PRODUCT_TYPE::CCL){
		nDataLen = CEMCParser::CheckCCLData(CString(pStart));
	}
	while (nDataLen > 0){
		if (nDataLen){
			char *pData = new char[nDataLen + 1];
			memset(pData, 0, nDataLen + 1);
			memcpy(pData, pStart, nDataLen);
			ParseCommand(CString(pData));
			delete[] pData;

			memcpy(m_cDataBuf, pStart + nDataLen, m_nDataSize - nDataLen);
			memset(pStart + m_nDataSize - nDataLen, 0, nDataLen);//clear data
			m_nDataSize -= nDataLen;

			//check if there's more
			if (eType == PRODUCT_TYPE::PP){
				nDataLen = CEMCParser::CheckPPData(CString(pStart));
			}
			else if (eType == PRODUCT_TYPE::CCL){
				nDataLen = CEMCParser::CheckCCLData(CString(pStart));
			}
		}
	}
}