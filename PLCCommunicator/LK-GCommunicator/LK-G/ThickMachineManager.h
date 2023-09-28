#pragma once

#include "ThickCommunicator.h"
#include <vector>
using namespace std;

enum ThickMachine{
	MACHINE_1,
	MACHINE_2,
	MACHINE_MAX,
};

struct THICK_INFO{
	float fThick1;
	float fThick2;
	__time64_t xTime;
};


class IThickMachineManager{
public:
	IThickMachineManager(){ m_pLink = NULL; };
	~IThickMachineManager(){ m_pLink = NULL; };

	void AttachLink(IThickMachineManager *pLink){ m_pLink = pLink; };
protected:
	virtual void ON_THICKMACHINE_MSG(ThickMachine eMachine, CString &strMsg){
		if (m_pLink) m_pLink->ON_THICKMACHINE_MSG(eMachine, strMsg);
	}
	virtual void ON_THICKMACHINE_INFO(ThickMachine eMachine, OUT_TYPE eType, float *pData){
		if (m_pLink) m_pLink->ON_THICKMACHINE_INFO(eMachine, eType, pData);
	}
	virtual void ON_THICKMACHINE_PARAM(ThickMachine eMachine, CString &strMsg, CString &strValue){
		if (m_pLink) m_pLink->ON_THICKMACHINE_PARAM(eMachine, strMsg, strValue);
	}

private:
	IThickMachineManager *m_pLink;

};


class CThickMachineManager : public IThickCommunicator, public IThickMachineManager{
public:
	CThickMachineManager(UINT nComId1, UINT nComId2, UINT nRate, UINT nTime);
	~CThickMachineManager();

	void OpenDevice();

	CString GetComId(ThickMachine eMachine);
#ifdef EXPORTCSV
	void OnExportCSV();
#endif
protected:
	virtual void ON_COMPORT_MSG(void *pInstance, CString &strMsg);
	virtual void ON_RECEIVE_THICK(void *pInstance, OUT_TYPE eType, float *pData);
	virtual void ON_COMPORT_PARAM(void *pInstance, CString &strParam, CString &strValue);
private:
	void Init();
	void Finalize();
private:
	CThickCommunicator *m_pThick[MACHINE_MAX];
	UINT m_nComId1;
	UINT m_nComId2;
	UINT m_nRate;
	UINT m_nTime;
#ifdef EXPORTCSV
	CRITICAL_SECTION m_xLock;
	vector<THICK_INFO> m_vThick[MACHINE_MAX];
#endif
};