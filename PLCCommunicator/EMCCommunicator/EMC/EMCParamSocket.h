#pragma once

#include "EMCSocket.h"
#define MAX_CLIENT 3


//tool: socket server; EMC: socket client
class EMCParamSocket : public EMCSocket{
public:
	EMCParamSocket(EMCParamSocket *pClient);
	~EMCParamSocket();

	void SetPort(int nPort){ m_nPort = nPort; };
	BOOL Start();
	void Stop();
	enum SERVER_STATE{
		MODE_SERVER_IDLE = 0,
		MODE_SERVER_LISTEN = 1,
	};
	SERVER_STATE GetServerState() { return (SERVER_STATE)m_nMode; };
	int GetListenPort(){ return m_nPort; };
	BOOL GetClientInfo(int nIndex, CString &strIp, UINT &nPort);
	void ClientStatusChange(EMCParamSocket *pClient);
	void SendAck(BOOL bSuccess);

private:
	void Init();
	bool OpProcSocket(int nOpCode);
protected:
	virtual void OnAccept(int nErrorCode);
	virtual void OnClose(int nErrorCode);

	virtual void ParseCommand(CString strData);
	virtual void CommandTimeout(CString strData);
private:
	int m_nPort;
	int m_nMode;
	EMCParamSocket *m_pProcSocket[MAX_CLIENT];
	EMCParamSocket *m_pServer;
	enum {
		SOCKET_INIT = 0,
		SOCKET_DESTROY,
	};
};
