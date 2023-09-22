#pragma once
#include "EMCSocket.h"
#include "EMCParser.h"


class IEMCClientCallBack{
public:
	enum CallBackType{
		ConnectState,
		ReceiveData,
	};
	virtual void ClientCallBack(void* pInstance, void* pData, int nType) = 0;
};

//tool: socket client; EMC: socket server
class EMCResultSocket : public EMCSocket{
public:
	EMCResultSocket(CString &strIp, UINT nPort, IEMCClientCallBack *pCallBack);
	~EMCResultSocket();
	void Stop();
	void DoConnect();
	void ReConnect();
	AOI_SOCKET_STATE GetConnectState() { return m_xConnectState; }
	BOOL IsConnected(){ return m_xConnectState == CONNECTED; };
	static CString GetStatusString(int nState);
protected:
	virtual void OnConnect(int nErrorCode);
	virtual void OnClose(int nErrorCode);
	virtual void ParseCommand(CString strData);
	virtual void CommandTimeout(CString strData){};
private:
	void Init();
	void NotifyConnectState(AOI_SOCKET_STATE xState);
	void Thread_Reconnect();
	static DWORD __stdcall Thread_Reconnect(void* pvoid)
	{
		((EMCResultSocket*)pvoid)->Thread_Reconnect();
		return NULL;
	}
private:
	CString m_strIp;
	UINT m_nPort;
	AOI_SOCKET_STATE m_xConnectState;
	IEMCClientCallBack *m_pCallBack;

	enum
	{
		EV_EXIT,
		EV_RECONNECT,
		EV_COUNT,

		WAIT_EXIT = WAIT_OBJECT_0,
		WAIT_RECONNECT,
	};
	HANDLE        m_hThread;
	HANDLE        m_hEvent[EV_COUNT];
};

class EMCResultSocketMgr : public IEMCClientCallBack, public IEMCSocketNotify{
public:
	EMCResultSocketMgr(CString &strIp, UINT nPort);
	~EMCResultSocketMgr();

	void Disconnect();
	void Connect();


	void SendToEMC(EMC_CCL_DATA &xResult);
	void SendToEMC(EMC_PP_DATA &xResult);

	CString GetConnectState();
protected:
	//IEMCClientCallBack
	virtual void ClientCallBack(void* pInstance, void* pData, int nType);

	//IEMCNotify

private:
	void SendToEMC(CString &strData);
	void OnConnectStateChange(EMCResultSocket* pClient);
	void DoDisconnect();
private:
	CString m_strIp;
	UINT m_nPort;
	EMCResultSocket *m_pClient;
};