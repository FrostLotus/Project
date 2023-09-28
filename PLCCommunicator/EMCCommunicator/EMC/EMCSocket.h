#pragma once
#include <afxsock.h>
#include <mutex>
#include "EMCParser.h"

//#define MOVE_PACKET //����buffer, �T�O�}�Y/�������O|

#define MAX_RECEIVE_BUFFER_SIZE 256*1024 //512 KB
#ifdef MOVE_PACKET
#define PACKET_START	0x7c
#define PACKET_END		0x7c
#endif
#define TIMEOUT_PERIOD	5000
//--------------- Need Merge ------------------------------------------
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
//-----------------------------------------------------------

class EMCParamSocket;
class IEMCSocketNotify{
public:
	IEMCSocketNotify(){
		m_pLink = NULL;
	}
	virtual ~IEMCSocketNotify(){
		m_pLink = NULL;
	}
	void AttachLink(IEMCSocketNotify *pLink){
		m_pLink = pLink;
	}
	virtual void OnEMCParamSocketStatusChange(int nIndex){
		if (m_pLink){
			m_pLink->OnEMCParamSocketStatusChange(nIndex);
		}
	}
	virtual void OnReceiveEMCParam(EMCParamSocket *pSrc, CString strData){
		if (m_pLink){
			m_pLink->OnReceiveEMCParam(pSrc, strData);
		}
	}
	virtual void OnEMCParamTimeout(EMCParamSocket *pSrc, CString strData){
		if (m_pLink){
			m_pLink->OnEMCParamTimeout(pSrc, strData);
		}
	}
	virtual void OnEMCResultSocketStatusChange(AOI_SOCKET_STATE eStatus){
		if (m_pLink){
			m_pLink->OnEMCResultSocketStatusChange(eStatus);
		}
	}
	virtual void OnReceiveEMCResult(BOOL bAckSuccess){
		if (m_pLink){
			m_pLink->OnReceiveEMCResult(bAckSuccess);
		}
	}
	virtual PRODUCT_TYPE GetProductType(){
		if (m_pLink){
			return m_pLink->GetProductType();
		}
		else
			return PRODUCT_TYPE::CCL; //default product type
	}
private:
	IEMCSocketNotify *m_pLink;
};

class EMCSocket :public CAsyncSocket, public IEMCSocketNotify{
public:
	EMCSocket();
	virtual ~EMCSocket();
	void CheckTimeout();
	BOOL NeedCheckBuffer() { return m_bCheckBuffer; };
protected:
	//�T�{������~���X��������buffer
	//�̫�@���u�榬���(���ASTART/DELETE),�S����U�@���u��(�X�{�������), �|���X��������buffer
	//time out=>�M��buffer
	virtual void OnReceive(int nErrorCode);
	virtual void OnSend(int nErrorCode);
	virtual void OnClose(int nErrorCode);

	virtual void ParseCommand(CString strData) = 0;
	virtual void CommandTimeout(CString strData) = 0;
private:
	static DWORD __stdcall TimeoutThread(void* pvoid);
	void Init();
	void ProcessCmd();
private:
	int m_nDataSize;
	BYTE m_cDataBuf[MAX_RECEIVE_BUFFER_SIZE];
	std::mutex m_oMutex;
	LARGE_INTEGER m_xLastReceive;
	LARGE_INTEGER m_xFreq;
	HANDLE m_hTimeoutThread;
	BOOL m_bCheckBuffer;
};