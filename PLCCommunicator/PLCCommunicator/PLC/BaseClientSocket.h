#pragma once
#include "afxsock.h"
#include "DataHandlerBase.h"


enum AOI_SOCKET_STATE{
	//���s�u
	NONE,
	//�s�u��
	CONNECTING,
	//�s�u����
	CONNECTED,
	//�_�u
	DISCONNECT,
	//���s�s�u
	RECONNECT,
	//����
	STOP,
};

class ISocketCallBack
{
public:
	virtual void ConnStatusCallBack(AOI_SOCKET_STATE xState) = 0;
	virtual void OnDeviceNotify(int nType,int nVal, CString strDes) = 0;
	virtual void OnPLCNewBatch(CString strOrder, CString strMaterial) = 0;
	virtual void OnPLCSYSTParam(BATCH_SHARE_SYST_PARAMCCL *pData) = 0;
	virtual void OnC10Change(WORD wC10) = 0;
};


class CBaseClientSocket :public CAsyncSocket
{
public:
	CBaseClientSocket(ISocketCallBack *pParent);
	~CBaseClientSocket();
	void SetConnectInfo(CString strIp, UINT uPort);
	AOI_SOCKET_STATE GetState() { return m_xConnectState; };
	void DoConnect();
	void Stop();
	void EnableDumpPacket(BOOL bFlag) { m_bDumpPacket = bFlag; };
protected:
	void DumpLog(CString strLog);
	//for callback
	void NotifyDeviceVal(int nType, int nVal, CString strDes);
	void NotifyPLCNewBatch(CString strOrder, CString strMaterial);
	void NotifyPLCSYSTParam(BATCH_SHARE_SYST_PARAMCCL *pData);
	void NotifyC10Change(WORD wC10);
	void DumpReceivePacket(unsigned char *pData, int nLen);
	void DumpWritePacket(BYTE *pItem, int nItemSize, BYTE * pWriteData, int nWriteSize);

private:
	void Init();
	void Finalize();
	void NotifyConnectState(AOI_SOCKET_STATE xState);

	virtual void ProcessSockeData(unsigned char *pData, int nLen) = 0;
private:
	AOI_SOCKET_STATE m_xConnectState;
	CString m_strIp;
	UINT m_nPort;
	BOOL m_bDumpPacket;
	ISocketCallBack *m_pCallBack;
	CString m_strPath;
protected:
	virtual void OnConnect(int nErrorCode);
	virtual void OnReceive(int nErrorCode);
	virtual void OnClose(int nErrorCode);

};

