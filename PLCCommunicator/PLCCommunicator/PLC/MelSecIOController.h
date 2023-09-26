#pragma once

#include "ActDefine.h"
#include "ActProgType_i.h"
#include "ActSupportMsg_i.h"
#include "DataHandlerBase.h"

#define BATCH_READ_WRITE	//批次讀寫address

#define ERR_DLL_NOT_LOAD	9999
#define ERR_PARAM_ERROR		9998

enum CPU_SERIES
{
	Q_SERIES,			//Q系列
	FX3U_SERIES,		//FX3U系列
	FX5U_SERIES,		//FX5U系列
	R_SERIES,			//R系列		
	MAX_SERIES,
};
class IPLCProcess{
public:
	IPLCProcess(){ m_pIn = NULL; m_pOut = NULL; }
	void AttachIn(IPLCProcess *pLink) { m_pIn = pLink; };
	void AttachOut(IPLCProcess *pLink) { m_pOut = pLink; };
	//in
	virtual void ON_GPIO_NOTIFY(WPARAM wp, LPARAM lp){
		if (m_pIn)
			m_pIn->ON_GPIO_NOTIFY(wp, lp);
	}
	virtual long ON_OPEN_PLC(LPARAM lp){
		if (m_pIn)
			return m_pIn->ON_OPEN_PLC(lp);
		else
			return FALSE;
	}
	//out
	virtual void ON_PLC_NOTIFY(CString strMsg){
		if (m_pOut)
			m_pOut->ON_PLC_NOTIFY(strMsg);
	}
	virtual void ON_SET_PLCPARAM(BATCH_SHARE_SYSTCCL_INITPARAM &xParam){
		if (m_pOut)
			m_pOut->ON_SET_PLCPARAM(xParam);
	}
	virtual void ON_PLCDATA_CHANGE(int nFieldId, void* pData, int nSizeInByte){
		if (m_pOut)
			m_pOut->ON_PLCDATA_CHANGE(nFieldId, pData, nSizeInByte);
	}
	virtual void ON_BATCH_PLCDATA_CHANGE(int nFieldFirst, int nFieldLast){
		if (m_pOut)
			m_pOut->ON_BATCH_PLCDATA_CHANGE(nFieldFirst, nFieldLast);
	}
private:
	IPLCProcess *m_pIn;
	IPLCProcess *m_pOut;
};
class CMelSecIOController : public IPLCProcess
{
public:
	CMelSecIOController();
	virtual ~CMelSecIOController();

	CString GetCPUType();
#ifdef BATCH_READ_WRITE
protected:
	long ReadAddress(CString strDevType, int nStartDeviceNumber, int nSize, WORD *pValue);
	long ReadAddress(CString strDevType, int nStartDeviceNumber, int nSize, float *pValue);
	long ReadAddress(CString strDevType, int nStartDeviceNumber, int nLength, char* pValue);
	long ReadRandom(CString &strList, int nSize, short *pData);
	long ReadOneAddress(CString strDevType, int nStartDeviceNumber, short *pValue);

	long WriteAddress(CString strDevType, int nDeviceNumber, int nSizeInByte, WORD *pWrite);
	long WriteAddress(CString strDevType, int nDeviceNumber, int nSizeInByte, float *pWrite);
	long WriteAddress(CString strDevType, int nDeviceNumber, int nSizeInByte, char* pWrite);
	long WriteRandom(CString &strList, int nSize, short *pData);
	long WriteOneAddress(CString strDevice, short nValue);
	CString GetErrorMessage(long lErrCode);
#endif
	CString GetPLCIP(){ return m_strIp; };
	virtual CPU_SERIES GetCPU(){ return CPU_SERIES::Q_SERIES; }
protected:
	long OpenDevice(BATCH_SHARE_SYSTCCL_INITPARAM &xData);
	virtual void SetMXParam(IActProgType *pParam, BATCH_SHARE_SYSTCCL_INITPARAM &xData) = 0;
private:
	void Finalize();
	void Init();
	void LIB_LOAD();
	void LIB_FREE();
	void ListAllIP();
#ifdef BATCH_READ_WRITE
	long ReadAddress(CString strDevType, int nStartDeviceNumber, int nSizeInWord, short **ppValue); // must delete after use
	long WriteAddress(CString strDevType, int nStartDeviceNumber, int nSizeInWord, short *pValue);
#endif
private:
	IActProgType *m_pIProgType;
	IActSupportMsg *m_pISupportMsg;
	CString m_strIp;
	BOOL m_bInit; //will be TRUE if connected with PLC

	struct GPIO_ITEM{
		UINT uAddress;
		BYTE cDeviceCode;
		BYTE cValue;
	};
private:
	CString GetDeviceCString(GPIO_ITEM &xItem);
	CRITICAL_SECTION m_xLock;
};
