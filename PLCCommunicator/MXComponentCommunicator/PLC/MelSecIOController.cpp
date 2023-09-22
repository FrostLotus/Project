#include "stdafx.h"
#include "MelSecIOController.h"
#ifdef SUPPORT_AOI
#include "AOI.h"
#endif
#ifdef SUPPORT_AOI
CMelSecIOController::CMelSecIOController() : CDataHandlerBase(BATCH_AOI2MX_MEM_ID)
#else
CMelSecIOController::CMelSecIOController()
#endif
{
	Init();
}
CMelSecIOController::~CMelSecIOController()
{
	Finalize();
}
long CMelSecIOController::OpenDevice()
{
	long lRtn = ERR_DLL_NOT_LOAD;
#ifndef SUPPORT_AOI
	if (m_pIProgType){
		m_pIProgType->Open(&lRtn);
#ifdef _DEBUG //more convenient to test
		lRtn = 0;
#endif
		if (lRtn == 0){
			m_bInit = TRUE;
			OnAddMessage(L"open PLC ok");
			InitPinStatusFromPLC();
			
			SetPinInfo();//notify AOI
		}
		else{
			OnAddMessage(L"open PLC fail");
		}
	}
#endif

	return lRtn;
}
int CMelSecIOController::GetGPIOPinNumber()
{
#ifdef SUPPORT_AOI
	return m_nGPIO_PinNumber;
#else
	if (m_pIProgType && m_bInit)
		return MAX_GPIO_PIN;
	else
		return 0;
#endif

}
#ifdef SUPPORT_AOI
void CMelSecIOController::SetInitParam(BATCH_SHARE_MX_INITPARAM *pData)
{
	SetSharedMemoryData(pData, sizeof(BATCH_SHARE_MX_INITPARAM), MX_COMMUNICATOR_NAME, WM_AOI_RESPONSE_CMD, WM_MX_PARAMINIT_CMD);
}
void CMelSecIOController::GetPinInfo()
{
	PinInfo xInfo;
	memset(&xInfo, 0, sizeof(xInfo));
	GetSharedMemoryData(&xInfo, sizeof(xInfo), BATCH_MX2AOI_MEM_ID);
	m_nGPIO_PinNumber = xInfo.cPinNumber;
	for (int i = 0; i < m_nGPIO_PinNumber; i++){
		m_xGPIO[i].cValue = xInfo.cPinStatus[i];
	}
	TRACE(L"Gpio pin number change");

	//init send status
	m_bWaitResponse = FALSE;

}
void CMelSecIOController::SetOutputPin(int nPinNumber0Base, BOOL bHighLevel)
{
	if (m_xGPIO[nPinNumber0Base].cValue != bHighLevel){
		m_xGPIO[nPinNumber0Base].cValue = bHighLevel & 0xFF;

		BATCH_SHARE_MX_PINSTATUS xData;
		memset(&xData, 0, sizeof(xData));

		xData.nIndex0Base = nPinNumber0Base;
		xData.bHighLeve = bHighLevel;
		std::lock_guard< std::mutex > lock(m_oMutex);
		m_vOutputPin.push_back(xData);
		::SetEvent(m_hEvent[EV_WRITE]);
	}
}
void CMelSecIOController::GetOutputPinResult(long &lResult)
{
	GetSharedMemoryData(&lResult, sizeof(lResult), BATCH_MX2AOI_MEM_ID);
	TRACE(L"\n Gpio pin result 0x%08x\n ", lResult);
	m_bWaitResponse = FALSE;
	::SetEvent(m_hEvent[EV_WRITE]);//
}
#else
void CMelSecIOController::InitController()
{
#ifndef SUPPORT_AOI
	m_pUsm = new usm<unsigned char>(BATCH_MX2AOI_MEM_ID, TRUE);
#endif
	LIB_LOAD();
	NotifyAOI(WM_MX_PARAMINIT_CMD, NULL);
}
void CMelSecIOController::GetInitParam()
{
	BATCH_SHARE_MX_INITPARAM_ xData;
	memset(&xData, 0, sizeof(BATCH_SHARE_MX_INITPARAM_));
	GetSharedMemoryData(&xData, sizeof(BATCH_SHARE_MX_INITPARAM), BATCH_AOI2MX_MEM_ID);

	m_strIp = xData.cPLCIP;

	//init pin information with start address
		for (int j = 0; j < MAX_GPIO_PIN; j++){
		m_xGPIO[j].cDeviceCode = 'Y';
		m_xGPIO[j].uAddress = xData.lStartAddress + j;
		m_xGPIO[j].cValue = -1;
	}
	InitDevice(xData.lCPU);

	OnChangeInformation();
	OpenDevice();
	TRACE(L"GetInitParam done \n");
}
void CMelSecIOController::SetPinInfo()
{
	PinInfo xInfo;
	memset(&xInfo, 0, sizeof(xInfo));
	int nTotalPin = GetGPIOPinNumber();
	xInfo.cPinNumber = nTotalPin & 0xFF;
	if (nTotalPin){
		for (int i = 0; i < nTotalPin; i++){
			xInfo.cPinStatus[i] = m_xGPIO[i].cValue;
		}
	}
	SetSharedMemoryData(&xInfo, sizeof(xInfo), AOI_MASTER_NAME, WM_MX_PININFO_CMD, NULL);
}

void CMelSecIOController::GetOutputPin()
{
	BATCH_SHARE_MX_PINSTATUS_ xData;
	memset(&xData, 0, sizeof(BATCH_SHARE_MX_PINSTATUS_));

	GetSharedMemoryData(&xData, sizeof(BATCH_SHARE_MX_PINSTATUS_), BATCH_AOI2MX_MEM_ID);

	GetOutputPin(xData.bHighLeve, xData.nIndex0Base);
}
void CMelSecIOController::GetOutputPin(BOOL bHighLevel, int nIndex0Base)
{
#ifdef _DEBUG
	LARGE_INTEGER xStart, xEnd, xFreq;
	QueryPerformanceFrequency(&xFreq);
	QueryPerformanceCounter(&xStart);
#endif
	DoChangePinStatus(nIndex0Base, bHighLevel);
	CString strLog;
	strLog.Format(L"AOI Set Pin%d %d", nIndex0Base + 1, bHighLevel);
	OnAddMessage(strLog);
#ifdef _DEBUG
	QueryPerformanceCounter(&xEnd);
	double d = (xEnd.QuadPart - xStart.QuadPart) * 1000.0 / xFreq.QuadPart;
	TRACE(L"time %.2f \n", d);
#endif
}
void CMelSecIOController::SetOutputPinResult(long lResult)
{
	SetSharedMemoryData(&lResult, sizeof(lResult), AOI_MASTER_NAME, WM_MX_PINRESULT_CMD, NULL);
}
#endif

void CMelSecIOController::Reset()
{
#ifdef SUPPORT_AOI
	theApp.InsertDebugLog(L"Reset GPIO pin", LOG_PLC_GPIO);
	for (int i = 0; i < GetGPIOPinNumber(); i++){
		SetOutputPin(i, FALSE);
	}
#else
	long lValue = 0, lRtn = 0;
	int nGPIONumber = GetGPIOPinNumber();
	for (int i = 0; i < nGPIONumber; i++){
		CString strDevice = GetDeviceCString(m_xGPIO[i]);
		m_pIProgType->SetDevice(strDevice.AllocSysString(), lValue, &lRtn);
#ifdef _DEBUG //more convenient to test
		lRtn = 0;
#endif
		if (lRtn == 0){
			m_xGPIO[i].cValue = lValue & 0xFF;
			OnPinStatusChange(i); 
		} 
	}

	SetPinInfo(); //notify AOI
#endif
}
#ifdef BATCH_READ_WRITE
long CMelSecIOController::ReadAddress(int nStartDeviceNumber, int nSize, int *pValue)
{
	int nReadSize = nSize;
	if (nReadSize <= 0 || !pValue)
		return 0;

	short *pRead = NULL;
	long lRtn = ReadAddress(nStartDeviceNumber, nReadSize, &pRead);

	BYTE *pSrc = (BYTE*)pRead, *pDst = (BYTE*)pValue;
	if (lRtn == 0 && pRead) {
		for (int i = 0; i < nReadSize; i++) {
			memcpy(pDst, pSrc, sizeof(short));
			pSrc += sizeof(short);
			pDst += sizeof(int);
		}
		delete[]pRead;
	}
	
	return lRtn;
}
long CMelSecIOController::ReadAddress(int nStartDeviceNumber, int nSize, float *pValue)
{
	int nReadSize = nSize * 2;  // one float is stored in 2 word
	if (nReadSize <= 0)
		return 0;


	short *pRead = NULL;
	long lRtn = ReadAddress(nStartDeviceNumber, nReadSize, &pRead);

	BYTE *pSrc = (BYTE*)pRead, *pDst = (BYTE*)pValue;
	if (lRtn == 0 && pRead) {
		for (int i = 0; i < nSize; i++) {
			memcpy(pDst, pSrc, sizeof(float));
			pSrc += sizeof(float);
			pDst += sizeof(float);
		}
		delete[]pRead;
	}

	return lRtn;
}
long CMelSecIOController::ReadAddress(int nStartDeviceNumber, int nSize, char* pValue)
{
	int nReadSize = nSize / 2;  // 2 char stored in 1 word
	if (nReadSize <= 0)
		return 0;

	short *pRead = NULL;
	long lRtn = ReadAddress(nStartDeviceNumber, nReadSize, &pRead);

	BYTE *pSrc = (BYTE*)pRead, *pDst = (BYTE*)pValue;
	if (lRtn == 0 && pRead) {
		for (int i = 0; i < nSize; i++) {
			memcpy(pDst, pSrc, sizeof(BYTE));
			pSrc += sizeof(BYTE);
			pDst += sizeof(BYTE);
		}
		delete[]pRead;
	}

	return lRtn;
}
long CMelSecIOController::ReadAddress(int nStartDeviceNumber, int nSizeInWord, short **ppValue)
{
	long lRtn = ERR_DLL_NOT_LOAD;
#ifndef SUPPORT_AOI
	if (nSizeInWord){
		if (m_pIProgType){
			*ppValue = new short[nSizeInWord];
			memset(*ppValue, 0, nSizeInWord * sizeof(short));
			CString strDevice;
			strDevice.Format(L"D%d", nStartDeviceNumber);
			m_pIProgType->ReadDeviceBlock2(strDevice.AllocSysString(), nSizeInWord, *ppValue, &lRtn);
		}
	}
#endif
	return lRtn;
}

long CMelSecIOController::WriteAddress(int nDeviceNumber, int nSize, int *pWrite)
{
	int nWriteSize = nSize;
	if (nWriteSize <= 0)
		return 0;

	short *pValue = new short[nWriteSize];
	memset(pValue, 0, nWriteSize * sizeof(short));
	BYTE *pSrc = (BYTE*)pWrite, *pDst = (BYTE*)pValue;
	for (int i = 0; i < nWriteSize; i++){
		memcpy(pDst, pSrc, sizeof(short));
		pDst += sizeof(short);
		pSrc += sizeof(int);
	}
	long lRtn = WriteAddress(nDeviceNumber, nWriteSize, pValue);

	delete[]pValue;

	return lRtn;
}
long CMelSecIOController::WriteAddress(int nDeviceNumber, int nSize, float *pWrite)
{
	int nWriteSize = nSize * 2;
	if (nWriteSize <= 0)
		return 0;

	short *pValue = new short[nWriteSize];
	memset(pValue, 0, nWriteSize * sizeof(short));
	BYTE *pSrc = (BYTE*)pWrite, *pDst = (BYTE*)pValue;
	for (int i = 0; i < nSize; i++){
		memcpy(pDst, pSrc, sizeof(float));
		pDst += sizeof(float);
		pSrc += sizeof(float);
	}
	long lRtn = WriteAddress(nDeviceNumber, nWriteSize, pValue);

	delete[]pValue;

	return lRtn;
}
long CMelSecIOController::WriteAddress(int nDeviceNumber, int nLength, char* pWrite)
{
	int nWriteSize = nLength / 2;
	if (nWriteSize <= 0)
		return 0;

	short *pValue = new short[nWriteSize];
	memset(pValue, 0, nWriteSize * sizeof(short));
	BYTE *pSrc = (BYTE*)pWrite, *pDst = (BYTE*)pValue;
	for (int i = 0; i < nLength; i++){
		*pDst = *pSrc;
		pDst += sizeof(BYTE);
		pSrc += sizeof(BYTE);
	}
	long lRtn = WriteAddress(nDeviceNumber, nWriteSize, pValue);

	delete[]pValue;

	return lRtn;
}
long CMelSecIOController::WriteAddress(int nStartDeviceNumber, int nSizeInWord, short *pValue)
{
	long lRtn = ERR_DLL_NOT_LOAD;
#ifndef SUPPORT_AOI
	if (nSizeInWord){
		if (m_pIProgType){
			CString strDevice;
			strDevice.Format(L"D%d", nStartDeviceNumber);
			m_pIProgType->WriteDeviceBlock2(strDevice.AllocSysString(), nSizeInWord, pValue, &lRtn);
		}
	}
#endif
	return lRtn;
}
CString CMelSecIOController::GetErrorMessage(long lErrCode)
{
	CString strRtn;
	switch (lErrCode){
	case 0:
		strRtn = L"Success";
		break;
	case ERR_DLL_NOT_LOAD:
		strRtn = L"DLL not Load";
		break;
	case ERR_PARAM_ERROR:
		strRtn = L"Parameter Error";
		break;
	default:
		BOOL bRtnDefault = TRUE;
#ifndef SUPPORT_AOI
		if (m_pISupportMsg){
			BSTR bs = NULL;
			long lRtn = 0;
			m_pISupportMsg->GetErrorMessage(lErrCode, &bs, &lRtn);
			if (lRtn == 0){
				bRtnDefault = FALSE;
				strRtn = bs;
			}
		}
#endif
		if (bRtnDefault){
			strRtn.Format(L"0x%08x", lErrCode);
		}
		break;
	}
	return strRtn;
}
#endif
#ifndef SUPPORT_AOI
long CMelSecIOController::DoChangePinStatus(int nIndex0Base, BOOL bHighLevel)
{
	long lRtn = ERR_DLL_NOT_LOAD;
	if (nIndex0Base < 0 || nIndex0Base >= MAX_GPIO_PIN)
		return ERR_PARAM_ERROR;

	GPIO_ITEM &xItem = m_xGPIO[nIndex0Base];

	if (xItem.cValue != bHighLevel){
		xItem.cValue = bHighLevel;

		CString strDevice = GetDeviceCString(xItem);

		if (m_pIProgType){
			m_pIProgType->SetDevice(strDevice.AllocSysString(), bHighLevel, &lRtn);
		}
	}
	else{
		lRtn = 0;
	}
	OnPinStatusChange(nIndex0Base);
	SetOutputPinResult(lRtn); //response to AOI
	return lRtn;
}
#endif

CString CMelSecIOController::GetDeviceCString(GPIO_ITEM &xItem)
{
	CString strRtn;
	strRtn.Format(L"%c%X", xItem.cDeviceCode, xItem.uAddress);
	return strRtn;
}
#ifndef SUPPORT_AOI
void CMelSecIOController::InitPinStatusFromPLC()
{
	OnPinStatusChange(-1); //set item count 
	long lRtn = 0, lValue = 0;
	int nGPIONumber = GetGPIOPinNumber();
	for (int i = 0; i < nGPIONumber; i++){
		CString strDevice = GetDeviceCString(m_xGPIO[i]);
		m_pIProgType->GetDevice(strDevice.AllocSysString(), &lValue, &lRtn);
		if (lRtn == 0){
			m_xGPIO[i].cValue = lValue & 0xFF;
		}
		OnPinStatusChange(i); 
	}
}
#else
DWORD CMelSecIOController::Thread_Output(void* pvoid)
{
	CMelSecIOController* pThis = (CMelSecIOController*)pvoid;
	BOOL              bRun = TRUE;
	while (bRun)
	{
		switch (::WaitForMultipleObjects(EV_COUNT, pThis->m_hEvent, FALSE, INFINITE))
		{
		case CASE_WRITE:
		{
			::ResetEvent(pThis->m_hEvent[EV_WRITE]); //send output pin one at a time
			if (!pThis->m_bWaitResponse){
				std::lock_guard< std::mutex > lock(pThis->m_oMutex);
				if (pThis->m_vOutputPin.size()){
					pThis->m_bWaitResponse = TRUE;
					BATCH_SHARE_MX_PINSTATUS xData = pThis->m_vOutputPin.at(0);
					pThis->m_vOutputPin.erase(pThis->m_vOutputPin.begin());
					pThis->SetSharedMemoryData(&xData, sizeof(BATCH_SHARE_MX_PINSTATUS), MX_COMMUNICATOR_NAME, WM_MX_PINSTATUS_CMD, NULL);
				}
			}
		}
		break;
		case CASE_EXIT:
		{
			bRun = FALSE;
		}
		break;
		}
	}
	return NULL;
}
#endif
#ifndef SUPPORT_AOI
CString CMelSecIOController::GetCPUType()
{
	switch (m_eCPU)
	{
	case CPU_SERIES::FX_SERIES:
		return L"F Series";
		break;
	case CPU_SERIES::Q_SERIES:
	default:
		return L"Q Series";
		break;
	}
}
void CMelSecIOController::GetPinStatus(int nIndex, CString &strPin, int &nStatus)
{
	if (nIndex < GetGPIOPinNumber()){
		nStatus = m_xGPIO[nIndex].cValue;
		strPin = GetDeviceCString(m_xGPIO[nIndex]);
	}
}
#endif
void CMelSecIOController::Init()
{
	memset(&m_xGPIO, 0, sizeof(m_xGPIO));
#ifdef SUPPORT_AOI
	m_nGPIO_PinNumber = 0;
	m_bWaitResponse = FALSE;
	m_hThread = NULL;
#endif
	m_bInit = FALSE;
	m_eCPU = CPU_SERIES::Q_SERIES;
#ifndef SUPPORT_AOI
	m_pUsm = NULL;
	m_pIProgType = NULL;
	m_pISupportMsg = NULL;
#endif

#ifdef SUPPORT_AOI
	for (int i = NULL; i < EV_COUNT; i++) m_hEvent[i] = ::CreateEvent(NULL, TRUE, FALSE, NULL);
	m_hThread = ::CreateThread(NULL, NULL, Thread_Output, this, NULL, NULL);
#endif
}
void CMelSecIOController::Finalize()
{
#ifdef SUPPORT_AOI
	::SetEvent(m_hEvent[EV_EXIT]);

	::WaitForSingleObject(m_hThread, INFINITE);
#endif
	LIB_FREE();
}
void CMelSecIOController::LIB_LOAD()
{
	CoInitialize(NULL);
#ifndef SUPPORT_AOI
	if (m_pIProgType == NULL){
		HRESULT	hr = CoCreateInstance(CLSID_ActProgType,
			NULL,
			CLSCTX_INPROC_SERVER,
			IID_IActProgType,
			(LPVOID*)&m_pIProgType);

		if (!SUCCEEDED(hr)){
			CString strLog;
			strLog.Format(L"Load ActProgType.dll Fail");
			OnAddMessage(strLog);
		}
		else{
			OnAddMessage(L"Load ActProgType.dll ok");
		}
	}
	if (m_pISupportMsg == NULL){
		HRESULT	hr = CoCreateInstance(CLSID_ActSupportMsg,
			NULL,
			CLSCTX_INPROC_SERVER,
			IID_IActSupportMsg,
			(LPVOID*)&m_pISupportMsg);

		if (!SUCCEEDED(hr)){
			CString strLog;
			strLog.Format(L"Load ActSupportMsg.dll Fail");
			OnAddMessage(strLog);
		}
		else{
			OnAddMessage(L"Load ActSupportMsg.dll ok");
		}
	}
#endif
}
void CMelSecIOController::LIB_FREE()
{
#ifndef SUPPORT_AOI
	if (m_pIProgType){
		m_pIProgType->Release();
		m_pIProgType = NULL;
		TRACE(L"Free DLL \n");
	}
#endif
	CoUninitialize();
}
void CMelSecIOController::InitDevice(long lCPU)
{
	Set_CPU(lCPU);
#ifndef SUPPORT_AOI
	if (m_pIProgType){
		switch (m_eCPU){
		case CPU_SERIES::Q_SERIES:
			{
				//參考MX_componentV4_Program Manaual 4.3.7設定
				m_pIProgType->put_ActCpuType(lCPU);
				m_pIProgType->put_ActDestinationIONumber(0);		//固定為0
				m_pIProgType->put_ActDestinationPortNumber(5007);	//固定為5007
				m_pIProgType->put_ActDidPropertyBit(0x01);			//固定為1
				m_pIProgType->put_ActDsidPropertyBit(0x01);			//固定為1
				m_pIProgType->put_ActHostAddress(m_strIp.AllocSysString());
				m_pIProgType->put_ActIntelligentPreferenceBit(0x00);//固定為0
				m_pIProgType->put_ActIONumber(0x3FF);				//單CPU時, 固定為0x3FF
				m_pIProgType->put_ActMultiDropChannelNumber(0x00);	//固定為0
				m_pIProgType->put_ActNetworkNumber(0);				//固定為0
				m_pIProgType->put_ActStationNumber(0xFF);			//物件站側模組站號
				m_pIProgType->put_ActThroughNetworkType(0x00);
				m_pIProgType->put_ActTimeOut(0x100);				//100ms timeout
				m_pIProgType->put_ActUnitNumber(0x00);				//固定為0
				m_pIProgType->put_ActUnitType(UNIT_QNETHER);
			}
			break;
		case CPU_SERIES::FX_SERIES:
			m_pIProgType->put_ActCpuType(lCPU);
			m_pIProgType->put_ActHostAddress(m_strIp.AllocSysString());
			m_pIProgType->put_ActUnitType(UNIT_FXETHER);
			break;
		default:
			ASSERT(FALSE);
			return;
		}
		m_pIProgType->put_ActProtocolType(PROTOCOL_TCPIP);

	}
#endif
}
void CMelSecIOController::Set_CPU(long lCPU)
{
#ifndef SUPPORT_AOI
	if (lCPU >= CPU_FX0CPU && lCPU <= CPU_FX3GCPU)
		m_eCPU = CPU_SERIES::FX_SERIES;
	else
		m_eCPU = CPU_SERIES::Q_SERIES;
#endif
}
#ifndef SUPPORT_AOI
void CMelSecIOController::NotifyAOI(WPARAM wp, LPARAM lp)
{
	HWND hWnd = ::FindWindow(NULL, AOI_MASTER_NAME);
	if (hWnd){
		::PostMessage(hWnd, WM_GPIO_MSG, wp, lp);
	}
}
void CMelSecIOController::GetSharedMemoryData(void *pData, size_t size, CString strMemID)
{
	usm<unsigned char> xShareMem(strMemID, TRUE);
	const unsigned char *pShare = xShareMem.BeginRead();
	memcpy(pData, pShare, size);

	xShareMem.EndRead();
}
void CMelSecIOController::SetSharedMemoryData(void *pData, size_t size, CString strTargetName, WPARAM wp, LPARAM lp)
{
	if (m_pUsm){
		unsigned char *pShare = m_pUsm->BeginWrite();
		if (pShare){
			memcpy(pShare, pData, size);
			m_pUsm->EndWrite();

			HWND hWnd = ::FindWindow(NULL, strTargetName);
			if (hWnd){
				::PostMessage(hWnd, WM_GPIO_MSG, wp, lp);
			}
		}
	}
}
#endif