#include "stdafx.h"
#include "BaseClientSocket.h"
#ifdef SUPPORT_AOI
#include "aoi.h" //for insertDebugLog
#else //SUPPORT_AOI
#include "PLCCommunicator.h" //for insertDebugLog
#endif //SUPPORT_AOI

CBaseClientSocket::CBaseClientSocket(ISocketCallBack *pParent)
{
	Init();
	m_pCallBack = pParent;

#ifdef _UNICODE
	wchar_t	workingDir[_MAX_PATH];
	_wgetcwd(workingDir, _MAX_PATH);
#else
	char	workingDir[_MAX_PATH];
	_getcwd(workingDir, _MAX_PATH);
#endif
	m_strPath = workingDir;
}


CBaseClientSocket::~CBaseClientSocket()
{
	Finalize();
}
void CBaseClientSocket::SetConnectInfo(CString strIp, UINT uPort)
{
	if (m_strIp != strIp){
		m_strIp = strIp;
		//Change Ip Address
	}
	if (m_nPort != uPort){
		m_nPort = uPort;
		//Change Port Address
	}
}
void CBaseClientSocket::Init()
{
	m_xConnectState = AOI_SOCKET_STATE::NONE;
	m_strIp = _T("127.0.0.1");
	m_nPort = 8000;
	m_pCallBack = NULL;
	m_bDumpPacket = FALSE;
	if (!AfxSocketInit()){
		CString strLogMsg;
		strLogMsg.Format(_T("Failed to Initialize Sockets"));
		theApp.InsertDebugLog(strLogMsg, LOG_PLCSOCKET);
		AfxMessageBox(_T("Failed to Initialize Sockets"), MB_OK | MB_ICONSTOP);
	}
}
void CBaseClientSocket::Finalize()
{
	m_pCallBack = NULL;
}
void CBaseClientSocket::NotifyConnectState(AOI_SOCKET_STATE xState)
{
	m_xConnectState = xState;
	if (m_pCallBack){
		m_pCallBack->ConnStatusCallBack(xState);
	}
	CString strLog;
	if (xState == CONNECTED){
		strLog.Format(_T("%s(%d) Connect Success!"), m_strIp, m_nPort);
		TRACE(_T("%s \n"), strLog);
		DumpLog(strLog);
	}
	else if (xState == STOP){
		strLog.Format(_T("%s(%d) Disconnect Success!"), m_strIp, m_nPort);
		TRACE(_T("%s \n"), strLog);
		DumpLog(strLog);
	}
	else if (xState == CONNECTING){
		strLog.Format(_T("%s(%d) TRY CONNECT!"), m_strIp, m_nPort);
		TRACE(_T("%s \n"), strLog);
		DumpLog(strLog);
	}
}
void CBaseClientSocket::DumpReceivePacket(unsigned char *pData, int nLen)
{
	if (m_bDumpPacket){
		CString strFileName;
		CString strTime = CTime::GetCurrentTime().Format(_T("%Y%m%d_%H%M%S"));
		strFileName.Format(_T("%s\\PacketReceive_%s.dat"), m_strPath, strTime);
		CFile xRecvFile;
		xRecvFile.Open(strFileName, CFile::modeCreate | CFile::modeWrite, NULL);
		xRecvFile.Write(pData, nLen);
		xRecvFile.Close();
	}
}
void CBaseClientSocket::DumpWritePacket(BYTE *pItem, int nItemSize, BYTE * pWriteData, int nWriteSize)
{
	if (m_bDumpPacket){
		if (nItemSize + nWriteSize > 0){
			BYTE *pLogData = new BYTE[nItemSize + nWriteSize];
			BYTE *pTemp = pLogData;
			if (pItem){
				memcpy(pTemp, pItem, nItemSize);
				pTemp += nItemSize;
			}
			if (pWriteData){
				memcpy(pTemp, pWriteData, nWriteSize);
			}
			
			CString strFileName;
			CString strTime = CTime::GetCurrentTime().Format(_T("%Y%m%d_%H%M%S"));
			strFileName.Format(_T("%s\\PacketWrite_%s.dat"), m_strPath, strTime);
			CFile xRecvFile;
			xRecvFile.Open(strFileName, CFile::modeCreate | CFile::modeWrite, NULL);
			xRecvFile.Write(pLogData, nItemSize + nWriteSize);
			xRecvFile.Close();

			delete[]pLogData;
		}
	}
}
void CBaseClientSocket::DumpLog(CString strLog)
{
	theApp.InsertDebugLog(strLog, LOG_PLCSOCKET);
}

void CBaseClientSocket::DoConnect()
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
void CBaseClientSocket::Stop()
{
	Close();
	if (m_xConnectState == CONNECTING || m_xConnectState == CONNECTED)
	{
		NotifyConnectState(STOP);
	}
}
void CBaseClientSocket::NotifyDeviceVal(int nType, int nVal, CString strDes)
{
	if (m_pCallBack){
		m_pCallBack->OnDeviceNotify(nType, nVal, strDes);
	}
}
void CBaseClientSocket::NotifyPLCNewBatch(CString strOrder, CString strMaterial)
{
	if (m_pCallBack){
		m_pCallBack->OnPLCNewBatch(strOrder, strMaterial);
	}
}
void CBaseClientSocket::NotifyPLCSYSTParam(BATCH_SHARE_SYST_PARAMCCL *pData)
{
	if (m_pCallBack){
		m_pCallBack->OnPLCSYSTParam(pData);
	}
}
void CBaseClientSocket::NotifyC10Change(WORD wC10)
{
	if (m_pCallBack){
		m_pCallBack->OnC10Change(wC10);
	}
}
void CBaseClientSocket::OnConnect(int nErrorCode)
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
	CAsyncSocket::OnConnect(nErrorCode);
}

void CBaseClientSocket::OnReceive(int nErrorCode)
{
	BYTE buff[4096];
	int nRead;
	nRead = Receive(buff, 4096);

	switch (nRead)
	{
	case 0:
		Close();
		break;
	case SOCKET_ERROR:
		if (GetLastError() != WSAEWOULDBLOCK)
		{
			AfxMessageBox(_T("Error occurred"));
			Close();
		}
		break;
	default:
		DumpReceivePacket(buff, nRead);
		ProcessSockeData(buff, nRead);
		break;
	}
	CAsyncSocket::OnReceive(nErrorCode);
}

void CBaseClientSocket::OnClose(int nErrorCode)
{
	NotifyConnectState(DISCONNECT);
	CAsyncSocket::OnClose(nErrorCode);
}