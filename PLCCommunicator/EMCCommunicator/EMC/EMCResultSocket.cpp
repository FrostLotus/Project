#include "stdafx.h"
#include "EMCResultSocket.h"
#include "EMCCommunicator.h"

EMCResultSocket::EMCResultSocket(CString &strIp, UINT nPort, IEMCClientCallBack *pCallBack) : m_strIp(strIp), m_nPort(nPort), m_pCallBack(pCallBack)
{
	Init();
}
EMCResultSocket::~EMCResultSocket()
{

}
void EMCResultSocket::Stop()
{
	Close();
	if (m_xConnectState == CONNECTING || m_xConnectState == CONNECTED)
	{
		NotifyConnectState(STOP);
	}
}
void EMCResultSocket::DoConnect()
{
	if (m_xConnectState == CONNECTING || m_xConnectState == CONNECTED)
		return;

	if (m_xConnectState != NONE || Create()){
		if (!Connect(m_strIp, m_nPort)) // handle result in OnConnect
		{
		}
		m_xConnectState = CONNECTING;
	}
	NotifyConnectState(m_xConnectState);
}

void EMCResultSocket::OnConnect(int nErrorCode)
{
	switch (nErrorCode){
	case 0:
		NotifyConnectState(CONNECTED);
		break;
	case WSAECONNABORTED:
		NotifyConnectState(DISCONNECT);
		break;
	default:
		NotifyConnectState(RECONNECT);
		break;
	}
	EMCSocket::OnConnect(nErrorCode);
}
void EMCResultSocket::OnClose(int nErrorCode)
{
	NotifyConnectState(DISCONNECT);
	EMCSocket::OnClose(nErrorCode);
}
void EMCResultSocket::ParseCommand(CString strData)
{
	CString strLog;
	strLog.Format(L"Receive From EMC: %s", strData);
	theApp.InsertDebugLog(strLog, LOG_EMCSYSTEM);

	if (strData == ACK_SUCCESS){
		TRACE(L"OK \n");
	}
	else{
		TRACE(L"fail! %s \n", strData);
	}
	OnReceiveEMCResult(strData == ACK_SUCCESS);
}
void EMCResultSocket::Init()
{
	m_xConnectState = AOI_SOCKET_STATE::NONE;

	for (int i = NULL; i < EV_COUNT; i++) m_hEvent[i] = ::CreateEvent(NULL, TRUE, FALSE, NULL);
	m_hThread = ::CreateThread(NULL, NULL, Thread_Reconnect, this, NULL, NULL);

}
void EMCResultSocket::NotifyConnectState(AOI_SOCKET_STATE xState)
{
	m_xConnectState = xState;

	CString strLog;
	if (xState == CONNECTED){
		strLog.Format(_T("%s(%d) Connect Success!"), m_strIp, m_nPort);
		TRACE(_T("%s \n"), strLog);
	}
	else if (xState == STOP){
		strLog.Format(_T("%s(%d) Disconnect Success!"), m_strIp, m_nPort);
		TRACE(_T("%s \n"), strLog);
	}
	else if (xState == CONNECTING){
		strLog.Format(_T("%s(%d) TRY CONNECT!"), m_strIp, m_nPort);
		TRACE(_T("%s \n"), strLog);
	}
	else if (xState == DISCONNECT){
		strLog.Format(_T("%s(%d) Disconnect!"), m_strIp, m_nPort);
		TRACE(_T("%s \n"), strLog);
	}
	else if (xState == RECONNECT){
		strLog.Format(_T("%s(%d) RECONNECT!"), m_strIp, m_nPort);
		TRACE(_T("%s \n"), strLog);
	}
	if (m_pCallBack){
		m_pCallBack->ClientCallBack(this, NULL, IEMCClientCallBack::ConnectState);
	}
}
void EMCResultSocket::ReConnect()
{
	::SetEvent(m_hEvent[EV_RECONNECT]);
}
CString EMCResultSocket::GetStatusString(int nState)
{
	CString strRtn;
	switch (nState)
	{
	case DISCONNECT:
	{
		strRtn = L"DISCONNECT";
	}
	break;
	case RECONNECT:
		strRtn = L"RECONNECT";
		break;
	case CONNECTING:
		strRtn = L"CONNECTING";
		break;
	case CONNECTED:
		strRtn = L"CONNECTED";
		break;
	case STOP:
		strRtn = L"STOP";
		break;
	default:
		ASSERT(FALSE);
		break;
	}
	return strRtn;
}
void EMCResultSocket::Thread_Reconnect()
{
	while (TRUE)
	{
		switch (::WaitForMultipleObjects(EV_COUNT, m_hEvent, FALSE, INFINITE))
		{
		case WAIT_EXIT:
			return;
		case WAIT_RECONNECT:
		{
			::ResetEvent(m_hEvent[EV_RECONNECT]);
			::Sleep(1000);
			if (!IsConnected()){
				DoConnect();
			}
		}
		break;
		}
	}
}
EMCResultSocketMgr::EMCResultSocketMgr(CString &strIp, UINT nPort)
{
	m_strIp = strIp;
	m_nPort = nPort;
	m_pClient = NULL;
}
EMCResultSocketMgr::~EMCResultSocketMgr()
{
	Disconnect();
}
void EMCResultSocketMgr::Disconnect()
{
	DoDisconnect();
}

void EMCResultSocketMgr::SendToEMC(EMC_CCL_DATA &xResult)
{
	CString strData = CEMCParser::MakeString(xResult);
	SendToEMC(strData);
}
void EMCResultSocketMgr::SendToEMC(EMC_PP_DATA &xResult)
{
	CString strData = CEMCParser::MakeString(xResult);
	SendToEMC(strData);
}
CString EMCResultSocketMgr::GetConnectState()
{
	CString strRtn;
	if (m_pClient){
		AOI_SOCKET_STATE eStatus = m_pClient->GetConnectState();
		strRtn = EMCResultSocket::GetStatusString(eStatus);
	}
	return strRtn;
}
void EMCResultSocketMgr::ClientCallBack(void* pInstance, void* pData, int nType)
{
	switch (nType){
	case ConnectState:
		if (pInstance)
			OnConnectStateChange((EMCResultSocket*)pInstance);
		break;
	}
}
void EMCResultSocketMgr::SendToEMC(CString &strData)
{
	CString strLog;
	strLog.Format(L"SendToEMC: %s", strData);
	theApp.InsertDebugLog(strLog, LOG_EMCSYSTEM);
	int nSize = strData.GetLength() + 1;
	char *pData = new char[nSize];
	memset(pData, 0, nSize);
	lstrcpyA(pData, CW2A(strData));

	if (m_pClient && m_pClient->IsConnected()){
		m_pClient->Send(pData, nSize - 1/*台光要求不要null terminator*/);
	}

	delete pData;
}
void EMCResultSocketMgr::OnConnectStateChange(EMCResultSocket* pClient)
{
	if (!pClient)
		return;
	CString strMsg;

	AOI_SOCKET_STATE eStatus = pClient->GetConnectState();
	strMsg.Format(L"client Ip: %s Port: %d  %s \n", m_strIp, m_nPort, EMCResultSocket::GetStatusString(eStatus));
	TRACE(strMsg);
	
	OnEMCResultSocketStatusChange(eStatus);
	if (eStatus == DISCONNECT){
		Connect();
	}
	else if (eStatus == RECONNECT){
		pClient->ReConnect();
	}
	if (eStatus == DISCONNECT || eStatus == CONNECTED){
		CString strLog;
		strLog.Format(L"Server Ip: %s Port: %d  %s", m_strIp, m_nPort, EMCResultSocket::GetStatusString(eStatus));
		theApp.InsertDebugLog(strLog, LOG_EMCSYSTEM);
	}

}
void EMCResultSocketMgr::Connect()
{
	Disconnect();

	m_pClient = new EMCResultSocket(m_strIp, m_nPort, this);
	m_pClient->AttachLink(this);
	m_pClient->DoConnect();
}
void EMCResultSocketMgr::DoDisconnect()
{
	if (m_pClient){
		m_pClient->Stop();//close connect
		delete m_pClient;
		m_pClient = NULL;
	}
}