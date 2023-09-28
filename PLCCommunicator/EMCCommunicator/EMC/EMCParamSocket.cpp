#include "stdafx.h"
#include "EMCParamSocket.h"
#include "EMCCommunicator.h"

EMCParamSocket::EMCParamSocket(EMCParamSocket *pClient) : m_pServer(pClient)
{
	Init();
}
EMCParamSocket::~EMCParamSocket()
{
	OpProcSocket(SOCKET_DESTROY);
}
void EMCParamSocket::Init()
{
	m_nMode = MODE_SERVER_IDLE;
	OpProcSocket(SOCKET_INIT);
}
bool EMCParamSocket::OpProcSocket(int nOpCode)
{
	bool bFlag = true;
	if (nOpCode == SOCKET_INIT){
		for (int i = 0; i<MAX_CLIENT; i++){
			m_pProcSocket[i] = NULL;
		}
	}
	else if (nOpCode == SOCKET_DESTROY){
		for (int i = 0; i<MAX_CLIENT; i++){
			if (m_pProcSocket[i]){
				m_pProcSocket[i]->Close();
				delete m_pProcSocket[i];
				m_pProcSocket[i] = NULL;
			}
		}
	}
	return bFlag;
}
BOOL EMCParamSocket::Start()
{
	BOOL bFlag = false;
	if (m_nMode == MODE_SERVER_IDLE){
		if (Create(m_nPort)){
			if (Listen()){
				m_nMode = MODE_SERVER_LISTEN;
				bFlag = true;
			}
		}
		if (!bFlag){
			int nError = GetLastError();
			TRACE("EMCParamSocket::Start Error(%d) \n", nError);
		}
	}

	return bFlag;
}
void EMCParamSocket::Stop()
{
	if (m_nMode == MODE_SERVER_LISTEN){
		Close();
		m_nMode = MODE_SERVER_IDLE;
	}
}
BOOL EMCParamSocket::GetClientInfo(int nIndex, CString &strIp, UINT &nPort)
{
	if (nIndex < MAX_CLIENT){
		if (m_pProcSocket[nIndex]){
			m_pProcSocket[nIndex]->GetPeerName(strIp, nPort);
			return TRUE;
		}
	}
	return FALSE;
}
void EMCParamSocket::ClientStatusChange(EMCParamSocket *pClient)
{
	if (pClient){
		CString strIp, strLog;
		UINT nPort;
		pClient->GetPeerName(strIp, nPort);

		for (int i = 0; i < MAX_CLIENT; i++){
			if (m_pProcSocket[i] && m_pProcSocket[i] == pClient){
				OnEMCParamSocketStatusChange(i);
				delete m_pProcSocket[i];
				m_pProcSocket[i] = NULL;
				break;
			}
		}
		strLog.Format(L"DisConnect! Ip: %s Port:%d", strIp, nPort);
		theApp.InsertDebugLog(strLog, LOG_EMCSYSTEM);
	}
}
void EMCParamSocket::SendAck(BOOL bSuccess)
{
	if(bSuccess){
		CString strIp;
		UINT nPort;
		this->GetPeerName(strIp, nPort);

		CString strData(ACK_SUCCESS);
		int nSize = strData.GetLength() + 1;
		char *pData = new char[nSize];
		memset(pData, 0, nSize);
		lstrcpyA(pData, CW2A(strData));

		Send(pData, nSize);

		delete pData;
	}
}
void EMCParamSocket::OnClose(int nErrorCode)
{
	CAsyncSocket::OnClose(nErrorCode);
	if (m_pServer){
		m_pServer->ClientStatusChange(this);
	}

}
void EMCParamSocket::OnAccept(int nErrorCode)
{
	CAsyncSocket::OnAccept(nErrorCode);
	if (nErrorCode == NO_ERROR){
		bool bMaxClient = true;
		int nNewSocketIdx = -1;
		for (int i = 0; i<MAX_CLIENT; i++){
			if (m_pProcSocket[i] == NULL){
				nNewSocketIdx = i;
				bMaxClient = false;
				break;
			}
		}
		if (bMaxClient){
			for (int i = 0; i<MAX_CLIENT; i++){
				if (m_pProcSocket[i] != NULL){
					m_pProcSocket[i]->Close();
					delete m_pProcSocket[i];
					m_pProcSocket[i] = NULL;
					for (int j = i; j<MAX_CLIENT; j++){
						if (m_pProcSocket[j] == NULL && ((j + 1)<MAX_CLIENT)){
							m_pProcSocket[j] = m_pProcSocket[j + 1];
							m_pProcSocket[j + 1] = NULL;
							nNewSocketIdx = j + 1;
						}
					}
					break;
				}
			}
		}
		//Create New Socket,Accept the Connection
		if ((nNewSocketIdx >= 0) && (nNewSocketIdx<MAX_CLIENT)){
			m_pProcSocket[nNewSocketIdx] = new EMCParamSocket(this);
			m_pProcSocket[nNewSocketIdx]->AttachLink(this);
			Accept(*m_pProcSocket[nNewSocketIdx]);
			OnEMCParamSocketStatusChange(nNewSocketIdx);
			CString strLog, strIp;
			UINT nPort;
			m_pProcSocket[nNewSocketIdx]->GetPeerName(strIp, nPort);
			strLog.Format(L"Connect! Ip: %s Port:%d", strIp, nPort);
			theApp.InsertDebugLog(strLog, LOG_EMCSYSTEM);
		}
	}
}
void EMCParamSocket::ParseCommand(CString strData)
{
	CString strLog, strIp;
	UINT nPort;
	GetPeerName(strIp, nPort);
	strLog.Format(L"Ip: %s Port %d Receive data: %s", strIp, nPort, strData);
	theApp.InsertDebugLog(strLog, LOG_EMCSYSTEM);
	TRACE(L"ParseCommand %s \n", strData);
	OnReceiveEMCParam(this, strData);
}
void EMCParamSocket::CommandTimeout(CString strData)
{
	CString strLog, strIp;
	UINT nPort;
	GetPeerName(strIp, nPort);
	strLog.Format(L"Ip: %s Port %d CommandTimeout: %s", strIp, nPort, strData);
	theApp.InsertDebugLog(strLog, LOG_EMCSYSTEM);
	OnEMCParamTimeout(this, strData);
}