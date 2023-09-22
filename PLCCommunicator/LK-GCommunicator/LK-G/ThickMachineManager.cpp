#include "stdafx.h"
#include "ThickMachineManager.h"

CThickMachineManager::CThickMachineManager(UINT nComId1, UINT nComId2, UINT nRate, UINT nTime) : m_nComId1(nComId1), m_nComId2(nComId2), m_nRate(nRate), m_nTime(nTime)
{
	Init();
}
CThickMachineManager::~CThickMachineManager()
{
	Finalize();
}
void CThickMachineManager::ON_COMPORT_MSG(void *pInstance, CString &strMsg)
{
	for (int i = 0; i < MACHINE_MAX; i++){
		if (m_pThick[i] && m_pThick[i] == pInstance){
			ON_THICKMACHINE_MSG((ThickMachine)i, strMsg);
			break;
		}
	}
}
void CThickMachineManager::ON_RECEIVE_THICK(void *pInstance, OUT_TYPE eType, float *pData)
{
	for (int i = 0; i < MACHINE_MAX; i++){
		if (m_pThick[i] && m_pThick[i] == pInstance){
			ON_THICKMACHINE_INFO((ThickMachine)i, eType, pData);
#ifdef EXPORTCSV
			EnterCriticalSection(&m_xLock);
			m_vThick[i].push_back({ *pData, *(pData + 1), CTime::GetTickCount().GetTime() });
			LeaveCriticalSection(&m_xLock);
#endif
			break;
		}
	}
}
void CThickMachineManager::ON_COMPORT_PARAM(void *pInstance, CString &strParam, CString &strValue)
{
	for (int i = 0; i < MACHINE_MAX; i++){
		if (m_pThick[i] && m_pThick[i] == pInstance){
			ON_THICKMACHINE_PARAM((ThickMachine)i, strParam, strValue);
			break;
		}
	}
}
void CThickMachineManager::Init()
{
#ifdef EXPORTCSV
	InitializeCriticalSection(&m_xLock);
#endif
}
void CThickMachineManager::Finalize()
{
	for (int i = 0; i < MACHINE_MAX; i++){
		if (m_pThick[i]){
			delete m_pThick[i];
			m_pThick[i] = NULL;
		}
	}
#ifdef EXPORTCSV
	DeleteCriticalSection(&m_xLock);
#endif
}
void CThickMachineManager::OpenDevice()
{
	for (int i = 0; i < MACHINE_MAX; i++){
		UINT nComId = 0;
		switch (i){
		case MACHINE_1:
			nComId = m_nComId1;
			break;
		case MACHINE_2:
			nComId = m_nComId2;
			break;
		default:
			ASSERT(FALSE);
			break;
		}

		m_pThick[i] = new CThickCommunicator(nComId, m_nRate, m_nTime);
		((IThickCommunicator*)m_pThick[i])->Attach(this);

		if (m_pThick[i]) m_pThick[i]->OpenDevice();
	}
}
CString CThickMachineManager::GetComId(ThickMachine eMachine)
{
	CString strRtn;
	switch (eMachine){
	case MACHINE_1:
	case MACHINE_2:
		if (m_pThick[eMachine]){
			strRtn = m_pThick[eMachine]->GetComId();
		}
		break;
	default:
		strRtn = L"";
		break;
	}
	return strRtn;
}
#ifdef EXPORTCSV
void CThickMachineManager::OnExportCSV()
{
	vector<CString> vPath;
	for (int i = 0; i < MACHINE_MAX; i++){
		CString strPath;
		FILE *pFile = NULL;
		strPath.Format(L"D:\\THICK%d.csv", i + 1);
		wchar_t *pFileName = strPath.GetBuffer();
		_wfopen_s(&pFile, pFileName, L"wb");
		EnterCriticalSection(&m_xLock);
		if (pFile){

			fwprintf(pFile, L"Out1,Out2,Time");
			for (auto &xThick : m_vThick[i]){
				fwprintf(pFile, L"\r\n%.4f,%.4f,%s", xThick.fThick1, xThick.fThick2, CTime(xThick.xTime).Format(L"%H:%M:%S"));
			}
			fflush(pFile);
			fclose(pFile);

			vPath.push_back(strPath);
		}
		m_vThick[i].clear();
		LeaveCriticalSection(&m_xLock);
	}
	if(vPath.size() > 0){
		CString strMsg = L"export: \r\n";
		for(auto &i : vPath){
			strMsg += i + L"\r\n";
		}
		AfxMessageBox(strMsg);
	}

}
#endif