#include "stdafx.h"
#include "OPCProcessBase.h"
#include "DataHandlerBase.h"

COPCProcessBase::COPCProcessBase()
{
	Init();
}
COPCProcessBase::~COPCProcessBase()
{
	Finalize();
}
void COPCProcessBase::Init()
{
	m_pOPCUsm = new usm<unsigned char>(BATCH_OPC2AOI_MEM_ID, TRUE);
	m_pAOIUsm = new usm<unsigned char>(BATCH_AOI2OPC_MEM_ID, TRUE);

	NotifyAOI(WM_OPC_PARAMINIT_CMD, NULL);
}
void COPCProcessBase::Finalize()
{
	if (m_pOPCUsm){
		delete m_pOPCUsm;
		m_pOPCUsm = NULL;
	}
	if (m_pAOIUsm){
		delete m_pAOIUsm;
		m_pAOIUsm = NULL;
	}
}
CString COPCProcessBase::GetNodeName(int nIndex0Base)
{
	NodeItem *pItem = GetNodeItem(nIndex0Base);
	if (pItem){
		return pItem->strName;
	}
	return L"";
}
CString COPCProcessBase::GetNodeId(int nIndex0Base)
{
	NodeItem *pItem = GetNodeItem(nIndex0Base);
	if (pItem){
		return pItem->strNodeId;
	}
	return L"";
}
CString COPCProcessBase::GetNodeValue(int nIndex0Base)
{
	NodeItem *pItem = GetNodeItem(nIndex0Base);
	if (pItem){
		CString strRtn;
		switch (pItem->nType){
		case UA_TYPES_STRING:
			strRtn = CString((TCHAR*)pItem->pValue, pItem->nLen / sizeof(TCHAR));
			break;
		case UA_TYPES_UINT16:
			strRtn.Format(L"%d", *(short*)pItem->pValue);
			break;
		case UA_TYPES_BOOLEAN:
			strRtn.Format(L"%d", *(bool*)pItem->pValue);
			break;
		case UA_TYPES_FLOAT:
			strRtn.Format(L"%.2f", *(float*)pItem->pValue);
			break;
		case UA_TYPES_INT32:
			strRtn.Format(L"%d", *(int*)pItem->pValue);
			break;
		case UA_TYPES_INT16:
			strRtn.Format(L"%d", *(WORD*)pItem->pValue);
			break;
		default:
			ASSERT(FALSE);
			break;
		}
		return strRtn;
	}
	return L"";
}
CString COPCProcessBase::GetNodeTime(int nIndex0Base)
{
	NodeItem *pItem = GetNodeItem(nIndex0Base);
	if (pItem){
		return  CTime(pItem->xTime).Format(L"%H:%M:%S");
	}
	return L"";
}
BOOL COPCProcessBase::USM_ReadData(BYTE *pData, int nSize, int nOffset)
{
	if (m_pOPCUsm){
		m_pOPCUsm->ReadData(pData, nSize, nOffset);
		return TRUE;
	}
	return FALSE;
}
BOOL COPCProcessBase::USM_ReadAOIData(BYTE *pData, int nSize, int nOffset)
{
	if (m_pAOIUsm){
		m_pAOIUsm->ReadData(pData, nSize, nOffset);
		return TRUE;
	}
	return FALSE;
}
BOOL COPCProcessBase::USM_WriteData(BYTE *pData, int nSize, int nOffset)
{
	if (m_pOPCUsm){
		m_pOPCUsm->WriteData(pData, nSize, nOffset);
		return TRUE;
	}
	return FALSE;
}
void COPCProcessBase::NotifyAOI(WPARAM wp, LPARAM lp)
{
	HWND hWnd = ::FindWindow(NULL, AOI_MASTER_NAME);
	if (hWnd){
		::PostMessage(hWnd, WM_GPIO_MSG, wp, lp);
	}
}