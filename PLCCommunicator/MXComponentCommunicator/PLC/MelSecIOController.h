#pragma once

#include "ActDefine.h"
#ifndef SUPPORT_AOI
#include "ActProgType_i.h"
#include "ActSupportMsg_i.h"
#endif
#include "DataHandlerBase.h"
#include <mutex>
//#define BATCH_READ_WRITE	//�妸Ū�gaddress

#define ERR_DLL_NOT_LOAD	9999
#define ERR_PARAM_ERROR		9998
#define MAX_GPIO_PIN	16	

struct PinInfo{
	BYTE cPinNumber;
	BYTE cPinStatus[MAX_GPIO_PIN];
};
enum CPU_SERIES{
	Q_SERIES,		//Q�t�C
	FX_SERIES,		//FX�t�C
	MAX_SERIES,
};
#ifndef SUPPORT_AOI
class IMelSecIOControllerInfo{
public:
	IMelSecIOControllerInfo(){ m_pLink = NULL; }
	void AttachLink(IMelSecIOControllerInfo *pLink) { m_pLink = pLink; };
	virtual void OnChangeInformation(){
		if (m_pLink)
			m_pLink->OnChangeInformation();
	}
	virtual void OnAddMessage(CString strMsg){
		if (m_pLink)
			m_pLink->OnAddMessage(strMsg);
	}
	virtual void OnPinStatusChange(int nPin){ //nPin: -1: Pin number change
		if (m_pLink)
			m_pLink->OnPinStatusChange(nPin);
	}
private:
	IMelSecIOControllerInfo *m_pLink;
};
#endif
class CMelSecIOController : 
#ifndef SUPPORT_AOI
	 public IMelSecIOControllerInfo
#else
	protected CDataHandlerBase
#endif
{
public:
	CMelSecIOController();
	virtual ~CMelSecIOController();

	//���oGPIO�}��ƶq
	int GetGPIOPinNumber();
#ifdef SUPPORT_AOI
	//�]�w�Ѽ�
	void SetInitParam(BATCH_SHARE_MX_INITPARAM *pData);
	//���oGPIO�}�쪬�A
	void GetPinInfo();
	//�]�w�}�쪬�A
	void SetOutputPin(int nPinNumber0Base, BOOL bHighLevel);
	//���o�]�w�}�쵲�G
	void GetOutputPinResult(long &lResult);
#else
	void InitController();
	//���o�Ѽ�
	void GetInitParam();
	//��l��Pin�}���A
	void SetPinInfo();
	//���o�}�쪬�A
	void GetOutputPin();
	void GetOutputPin(BOOL bHighLevel, int nIndex0Base);
	//�]�w�]�w�}�쵲�G
	void SetOutputPinResult(long lResult);
#endif
	//reset
	void Reset();
#ifdef BATCH_READ_WRITE
	long ReadAddress(int nStartDeviceNumber, int nSize, int *pValue);
	long ReadAddress(int nStartDeviceNumber, int nSize, float *pValue);
	long ReadAddress(int nStartDeviceNumber, int nLength, char* pValue);

	long WriteAddress(int nDeviceNumber, int nSize, int *pWrite);
	long WriteAddress(int nDeviceNumber, int nSize, float *pWrite);
	long WriteAddress(int nDeviceNumber, int nLength, char* pWrite);
	CString GetErrorMessage(long lErrCode);
#endif
#ifndef SUPPORT_AOI
	CString GetPLCIP(){ return m_strIp; };
	CString GetCPUType();
	void GetPinStatus(int nIndex, CString &strPin, int &nStatus);
#endif
private:
#ifndef SUPPORT_AOI
	long DoChangePinStatus(int nIndex0Base, BOOL bHighLevel);
#endif
	void Finalize();
	void Init();
	void LIB_LOAD();
	void LIB_FREE();

	void InitDevice(long lCPU);
	long OpenDevice();

	void Set_CPU(long lCPU);
#ifdef BATCH_READ_WRITE
	long ReadAddress(int nStartDeviceNumber, int nSizeInWord, short **ppValue); // must delete after use
	long WriteAddress(int nStartDeviceNumber, int nSizeInWord, short *pValue);
#endif
#ifndef SUPPORT_AOI
	void NotifyAOI(WPARAM wp, LPARAM lp);
	void GetSharedMemoryData(void *pData, size_t size, CString strMemID);
	void SetSharedMemoryData(void *pData, size_t size, CString strTargetName, WPARAM wp, LPARAM lp);
#endif
private:
#ifndef SUPPORT_AOI
	usm<unsigned char> *m_pUsm;
	IActProgType *m_pIProgType;
	IActSupportMsg *m_pISupportMsg;
#endif
	CPU_SERIES m_eCPU;
	CString m_strIp;
	BOOL m_bInit; //will be TRUE if connected with PLC

	struct GPIO_ITEM{
		UINT uAddress;
		BYTE cDeviceCode;
		BYTE cValue;
	};
	GPIO_ITEM m_xGPIO[MAX_GPIO_PIN];
#ifdef SUPPORT_AOI
	int m_nGPIO_PinNumber;
	std::mutex m_oMutex;
	HANDLE     m_hThread;
	std::vector<BATCH_SHARE_MX_PINSTATUS> m_vOutputPin;
	BOOL m_bWaitResponse;
	static DWORD __stdcall Thread_Output(void* pvoid);

	enum
	{
		EV_EXIT,
		EV_WRITE,
		EV_COUNT,
		CASE_EXIT = WAIT_OBJECT_0,
		CASE_WRITE,
	};
	HANDLE     m_hEvent[EV_COUNT];
#endif
private:
	CString GetDeviceCString(GPIO_ITEM &xItem);
#ifndef SUPPORT_AOI
	void InitPinStatusFromPLC();
#endif
};

extern CMelSecIOController g_xMelSecIOController;