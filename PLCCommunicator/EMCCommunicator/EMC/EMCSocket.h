#pragma once
#include <afxsock.h>
#include <mutex>
#include "EMCParser.h"

//#define MOVE_PACKET //移動buffer, 確保開頭/結尾都是|

#define MAX_RECEIVE_BUFFER_SIZE 256*1024 //512 KB
#ifdef MOVE_PACKET
#define PACKET_START	0x7c
#define PACKET_END		0x7c
#endif
#define TIMEOUT_PERIOD	5000
//--------------- Need Merge ------------------------------------------
enum AOI_SOCKET_STATE{
	//未連線
	NONE,
	//連線中
	CONNECTING,
	//連線完成
	CONNECTED,
	//斷線
	DISCONNECT,
	//重新連線
	RECONNECT,
	//結束
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
	//確認欄位全到才取出對應長度buffer
	//最後一筆工單收到後(狀態START/DELETE),又收到下一筆工單(出現重複欄位), 會取出對應長度buffer
	//time out=>清空buffer
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