#include "stdafx.h"
#include "SerialComBase.h"

IMPLEMENT_DYNAMIC(CSerialComBase, CWnd)
BEGIN_MESSAGE_MAP(CSerialComBase, CWnd)
	ON_WM_SERIAL(&CSerialComBase::OnSerialMsg)
END_MESSAGE_MAP()

const int ctBaud = CSerial::EBaud9600;		/*Base on LK-Navigator*/
const int ctDataBits = CSerial::EData8;
const int ctParity = CSerial::EParNone;
const int ctStopBits = CSerial::EStop1;

CSerialComBase::CSerialComBase(UINT nComId)
{
	m_strComId.Format(L"COM%d", nComId);
	Create(NULL, _T("CSerialComBase"), WS_CHILD | WS_VISIBLE, RECT(), AfxGetMainWnd(), NULL);

	Init();
}
CSerialComBase::~CSerialComBase()
{
	Finalize();
}
BOOL CSerialComBase::IsOpenDevice()
{
	if (m_pComm)
		return m_pComm->IsOpen();
	else
		return FALSE;
}
LRESULT	CSerialComBase::OnSerialMsg(WPARAM wParam, LPARAM lParam)
{
	const CSerialMFC::EEvent eEvent = CSerialMFC::EEvent(LOWORD(wParam));

	OnSerialEvent(eEvent);

	return 0;
}
void CSerialComBase::Init()
{
	m_pComm = NULL;
	m_bConnected = FALSE;
}
void CSerialComBase::Finalize()
{
	if (m_pComm){
		if (m_pComm->IsOpen()) m_pComm->Close();
		delete m_pComm;
		m_pComm = NULL;
	}
}
void CSerialComBase::OpenDevice()
{
	CString strMsg;

	if (!m_pComm){
		m_pComm = new CSerialMFC();
		ShowParam();
		LONG nRtn = m_pComm->Open(m_strComId, this, 0, false);
		if (nRtn == ERROR_SUCCESS){
			if (m_pComm->Setup((CSerial::EBaudrate)ctBaud, (CSerial::EDataBits)ctDataBits, (CSerial::EParity)ctParity, (CSerial::EStopBits)ctStopBits) == ERROR_SUCCESS){
				if (m_pComm->SetupReadTimeouts(CSerial::EReadTimeoutNonblocking) == ERROR_SUCCESS){
					m_pComm->SetupHandshaking(CSerial::EHandshakeOff);
					m_bConnected = true;

					OnComportOpen();
				}
				else{
					m_pComm->Close();
				}
			}
			else{
				m_pComm->Close();
			}
		}
		else{
			TRACE(L"open fail \n");
		}
	}

	if (m_bConnected)
		strMsg.Format(L"open %s ok", m_strComId);
	else
		strMsg.Format(L"open %s fail", m_strComId);

	ON_COMPORT_MSG(this, strMsg);
}
LONG CSerialComBase::SendData(BYTE* pData, int nSendDataSize, DWORD &dwBytesSend)
{
	LONG lRtn = 0;
	if (m_pComm){
		lRtn = m_pComm->Write(pData, nSendDataSize, &dwBytesSend);
	}
	return lRtn;
}
LONG CSerialComBase::ReceiveData(BYTE *pData, int nReadDataSize, DWORD &dwBytesRead)
{
	LONG lRtn = 0;
	if (m_pComm){
		lRtn = m_pComm->Read(pData, nReadDataSize, &dwBytesRead);
	}
	return lRtn;
}
void CSerialComBase::ShowParam()
{
	CString strParam, strBaud, strDataBits, strParity, strStopBits;
	switch (ctBaud){
	case CSerial::EBaud9600:
		strBaud = L"9600";
		break;
	}
	switch (ctDataBits){
	case CSerial::EData8:
		strDataBits = L"8";
		break;
	}
	switch (ctParity){
	case CSerial::EParNone:
		strParity = L"None";
		break;
	}
	switch (ctStopBits){
	case CSerial::EStop1:
		strStopBits = L"1";
		break;
	}
	strParam = L"Baud";
	ON_COMPORT_PARAM(this, strParam, strBaud);
	strParam = L"DataBits";
	ON_COMPORT_PARAM(this, strParam, strDataBits);
	strParam = L"Parity";
	ON_COMPORT_PARAM(this, strParam, strParity);
	strParam = L"StopBits";
	ON_COMPORT_PARAM(this, strParam, strStopBits);
}