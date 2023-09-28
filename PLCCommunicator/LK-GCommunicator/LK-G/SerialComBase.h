#pragma once

#include "SerialComm\SerialMFC.h"

enum OUT_TYPE{
	OUT_NONE,
	OUT_1,
	OUT_2,
	OUT_BOTH,
};


class IThickCommunicator{
public:
	IThickCommunicator(){ m_pLink = NULL; };
	~IThickCommunicator(){ m_pLink = NULL; };

	void Attach(IThickCommunicator* pLink) { m_pLink = pLink; };

	virtual void ON_COMPORT_PARAM(void *pInstance, CString &strParam, CString &strValue){
		if (m_pLink) m_pLink->ON_COMPORT_PARAM(pInstance, strParam, strValue);
	}
	virtual void ON_COMPORT_MSG(void *pInstance, CString &strMsg){
		if (m_pLink) m_pLink->ON_COMPORT_MSG(pInstance, strMsg);
	}
	virtual void ON_RECEIVE_THICK(void *pInstance, OUT_TYPE eType, float *pData){
		if (m_pLink) m_pLink->ON_RECEIVE_THICK(pInstance, eType, pData);
	}
private:
	IThickCommunicator *m_pLink;
};


class CSerialComBase : public CWnd/*Get Comport event*/, public IThickCommunicator{
	DECLARE_DYNAMIC(CSerialComBase)
public:
	CSerialComBase(UINT nComId);
	~CSerialComBase();

	void OpenDevice();

	CString GetComId(){ return m_strComId; }
	BOOL IsConnected(void) { return m_bConnected; };
	BOOL IsOpenDevice();
	LONG SendData(BYTE* pData, int nSendDataSize, DWORD &dwBytesSend);
	LONG ReceiveData(BYTE *pData, int nReadDataSize, DWORD &dwBytesRead);
protected:
	afx_msg LRESULT	OnSerialMsg(WPARAM wParam, LPARAM lParam);
	DECLARE_MESSAGE_MAP()

	virtual void OnSerialEvent(CSerialMFC::EEvent eEvent) = 0;
	virtual void OnComportOpen() = 0;
private:
	void Init();
	void Finalize();

	void ShowParam();
private:
	CSerialMFC *m_pComm;
	CString m_strComId;
	BOOL m_bConnected;

};