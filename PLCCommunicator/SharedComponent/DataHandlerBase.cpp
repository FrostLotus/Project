#include "stdafx.h"
#include "DataHandlerBase.h"
CDataHandlerBase::CDataHandlerBase(CString strMemID) : m_strMemID(strMemID)
{
	m_pUsm = NULL;
	OpShareMemory(OP_CREATE);
}
CDataHandlerBase::~CDataHandlerBase()
{
	OpShareMemory(OP_DESTROY);
}
void CDataHandlerBase::NotifyResponse(CString strTarget, LPARAM lp)
{
	HWND hWnd = ::FindWindow(NULL, strTarget);
	if (hWnd)
	{
		::PostMessage(hWnd, WM_GPIO_MSG, WM_AOI_RESPONSE_CMD, lp);
	}
}
void CDataHandlerBase::OpShareMemory(int nOpCode)
{
	if (nOpCode == OP_CREATE)
	{
		if (m_pUsm == NULL)
		{
#ifdef USE_IN_COMMUNICATOR
			if (m_strMemID.GetLength() == 0) 
			{
				m_pUsm = new usm<unsigned char>(BATCH_COMMUNICATOR_MEM_ID, TRUE);
			}
			else 
			{
				m_pUsm = new usm<unsigned char>(m_strMemID, TRUE);
			}	
#else
			if (m_strMemID.GetLength() == 0)
				m_pUsm = new usm<unsigned char>(BATCH_AOI_MEM_ID, TRUE);
			else
				m_pUsm = new usm<unsigned char>(m_strMemID, TRUE);
#endif
		}
	}
	else if (nOpCode == OP_DESTROY)
	{
		if (m_pUsm)
		{
			delete m_pUsm;
			m_pUsm = NULL;
		}
	}
}
BYTE* CDataHandlerBase::BeginWrite()
{
	if (m_pUsm) 
	{
		return m_pUsm->BeginWrite();
	}
	else 
	{
		return NULL;
	}
}
void CDataHandlerBase::EndWrite()
{
	if (m_pUsm) 
	{
		m_pUsm->EndWrite();
	}	
}
void CDataHandlerBase::GetSharedMemoryData(void *pData, size_t size, CString strMemID)
{
	usm<unsigned char> xShareMem(strMemID, TRUE);
	const unsigned char *pShare = xShareMem.BeginRead();
	memcpy(pData, pShare, size);

	xShareMem.EndRead();
}
void CDataHandlerBase::SetSharedMemoryData(void *pData, size_t size, CString strTargetName, WPARAM wp, LPARAM lp)
{
	if (m_pUsm)
	{
		unsigned char *pShare = m_pUsm->BeginWrite();
		if (pShare)
		{
			memcpy(pShare, pData, size);
			m_pUsm->EndWrite();

			HWND hWnd = ::FindWindow(NULL, strTargetName);
			if (hWnd)
			{
				::PostMessage(hWnd, WM_GPIO_MSG, wp, lp);
			}
		}
	}
}
//------------------------------------------------------------------------
CString CDataHandlerBase::MakeFloatLog(CString strDes, float fData)
{
	CString strRtn;
	strRtn.Format(L"%s: %f\r\n", strDes, fData);
	return strRtn;
}
CString CDataHandlerBase::MakeWordLog(CString strDes, WORD wData)
{
	CString strRtn;
	strRtn.Format(L"%s: %d\r\n", strDes, wData);
	return strRtn;
}
CString CDataHandlerBase::MakeCStringLog(CString strDes, char *pData)
{
	CString strRtn;
	strRtn.Format(L"%s: %s\r\n", strDes, CString(pData));
	return strRtn;
}
CString CDataHandlerBase::MakeCStringLog(CString strDes, TCHAR *pData)
{
	CString strRtn;
	strRtn.Format(L"%s: %s\r\n", strDes, CString(pData));
	return strRtn;
}
CString CDataHandlerBase::MakeByteLog(CString strDes, BYTE cData)
{
	CString strRtn;
	strRtn.Format(L"%s: %d\r\n", strDes, cData);
	return strRtn;
}
void CDataHandlerBase::NotifyAOI(WPARAM wp, LPARAM lp)
{
	HWND hWnd = ::FindWindow(NULL, AOI_MASTER_NAME);
	if (hWnd){
		::PostMessage(hWnd, WM_GPIO_MSG, wp, lp);
	}
}