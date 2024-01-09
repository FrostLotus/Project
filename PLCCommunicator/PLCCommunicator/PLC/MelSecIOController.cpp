#include "stdafx.h"
#include "MelSecIOController.h"
#include "PLCCommunicator.h"

CMelSecIOController::CMelSecIOController()
{
	Init();
}
CMelSecIOController::~CMelSecIOController()
{
	Finalize();
}
//------------------------
///<summary>初始化</summary>
void CMelSecIOController::Init()
{
	InitializeCriticalSection(&m_xLock);
	m_bInit = FALSE;
	m_pIProgType = NULL;
	m_pISupportMsg = NULL;
}
///<summary>終處置</summary>
void CMelSecIOController::Finalize()
{
	long lRtn = 0;
	if (m_pIProgType)
	{
		m_pIProgType->Close(&lRtn);
	}
	LIB_FREE();
	DeleteCriticalSection(&m_xLock);
}
///<summary>ActProgType載入</summary>
void CMelSecIOController::LIB_LOAD()
{
	CoInitialize(NULL);
	if (m_pIProgType == NULL)
	{
		HRESULT	hr = CoCreateInstance(CLSID_ActProgType,
			NULL,
			CLSCTX_INPROC_SERVER,
			IID_IActProgType,
			(LPVOID*)&m_pIProgType);

		if (!SUCCEEDED(hr))
		{
			ON_PLC_NOTIFY(L"Load ActProgType.dll Fail");
		}
		else
		{
			ON_PLC_NOTIFY(L"Load ActProgType.dll ok");
		}
	}
	if (m_pISupportMsg == NULL)
	{
		HRESULT	hr = CoCreateInstance(CLSID_ActSupportMsg,
			NULL,
			CLSCTX_INPROC_SERVER,
			IID_IActSupportMsg,
			(LPVOID*)&m_pISupportMsg);

		if (!SUCCEEDED(hr))
		{
			ON_PLC_NOTIFY(L"Load ActSupportMsg.dll Fail");
		}
		else
		{
			ON_PLC_NOTIFY(L"Load ActSupportMsg.dll ok");
		}
	}
}
///<summary>ActProgType釋放</summary>
void CMelSecIOController::LIB_FREE()
{
	if (m_pIProgType)
	{
		m_pIProgType->Release();
		m_pIProgType = NULL;
		TRACE(L"Free DLL \n");
	}
	CoUninitialize();
}
///<summary>排列所有徵測IP</summary>
void CMelSecIOController::ListAllIP()
{
	IP_ADAPTER_INFO* pAdptInfo = NULL;
	IP_ADAPTER_INFO* pNextAd = NULL;
	ULONG            ulLen = NULL;
	sockaddr_in      addr = { NULL };

	::GetAdaptersInfo(pAdptInfo, &ulLen);

	if (!ulLen)
	{
		return;
	}
	pAdptInfo = (IP_ADAPTER_INFO*)::new BYTE[ulLen];

	::GetAdaptersInfo(pAdptInfo, &ulLen);

	pNextAd = pAdptInfo;

	while (pNextAd)
	{
		CString strIp(pNextAd->IpAddressList.IpAddress.String);
		ON_PLC_NOTIFY(L"MachineIP: " + strIp);
		theApp.InsertDebugLog(L"MachineIP: " + strIp, LOG_SYSTEM);//將IP List 至 Log
		pNextAd = pNextAd->Next;
	}
	delete (BYTE*)pAdptInfo;
}
///<summary>取得CPU類型</summary>
CString CMelSecIOController::GetCPUType()
{
	switch (GetCPU())
	{
		case CPU_SERIES::FX3U_SERIES:
			return L"FX3U Series";
			break;
		case CPU_SERIES::FX5U_SERIES:
			return L"FX5U Series";
			break;
		case CPU_SERIES::R_SERIES:
			return L"R Series";
			break;
		case CPU_SERIES::Q_SERIES:
		default:
			return L"Q Series";
			break;
	}
}
///<summary>開啟PLC裝置</summary>
long CMelSecIOController::OpenDevice(BATCH_SHARE_SYSTCCL_INITPARAM& xData)
{
	LIB_LOAD();

	BOOL bLog = m_strIp.GetLength() == 0; //only log information on first connect, igonre logging on reconnect event
	m_strIp = xData.cPLCIP;
#ifdef _DEBUG
	m_strIp = L"192.168.2.99";
#endif
	//log Param-------------------------------------------------------
	CString strMsg;
	auto LogData = [&](CString strInfo, long lData)
	{
		if (bLog)
		{
			strMsg.Format(L"%s: %ld", strInfo, lData);
			theApp.InsertDebugLog(strMsg, LOG_SYSTEM);
			ON_PLC_NOTIFY(strMsg);
		}
	};
	LogData(L"ConnectedStationNo", xData.lConnectedStationNo);
	LogData(L"T_NetworkNo", xData.lTargetNetworkNo);
	LogData(L"T_StationNo", xData.lTargetStationNo);
	LogData(L"PCNetworkNo", xData.lPCNetworkNo);
	LogData(L"PCStationNo", xData.lPCStationNo);
	//連接部分-------------------------------------------------------
	long lRtn = ERR_DLL_NOT_LOAD;
	if (m_pIProgType)//若DLL存在
	{
		//設定連接---------------------------------------------------
		SetMXParam(m_pIProgType, xData);//設定開啟PLC - MXComponent

		BSTR bStr = m_strIp.AllocSysString();//將IP轉BasicString
		m_pIProgType->put_ActHostAddress(bStr);//鍵入IP(0x000...)
		::SysFreeString(bStr);

		m_pIProgType->put_ActProtocolType(PROTOCOL_TCPIP);

		if (bLog)
		{
			LONG lTimeOut = 0;
			m_pIProgType->get_ActTimeOut(&lTimeOut);
			LogData(L"TimeOutSetting", lTimeOut);
			ListAllIP();//for debug, check the machine has correct ip address 
		}
		//PLC連接----------------------------------------------------
		m_pIProgType->Open(&lRtn);//嘗試連接
		if (lRtn == 0)
		{
			m_bInit = TRUE;
			ON_PLC_NOTIFY(L"open PLC ok");
		}
		else
		{
			strMsg.Format(L"open PLC fail: %ld", lRtn);
			ON_PLC_NOTIFY(strMsg);
		}
	}
	return lRtn;
}
//------------------------
#ifdef BATCH_READ_WRITE
///<summary>軟元件隨機讀取</summary>
long CMelSecIOController::ReadRandom(CString& strList, int nSize, short* pData)
{
	long lRtn = ERR_DLL_NOT_LOAD;
	if (m_pIProgType)
	{
		EnterCriticalSection(&m_xLock);
		BSTR bStr = strList.AllocSysString();
		m_pIProgType->ReadDeviceRandom2(bStr, nSize, pData, &lRtn);
		::SysFreeString(bStr);
		LeaveCriticalSection(&m_xLock);
	}
	return lRtn;
}
///<summary>[WORD]軟元件讀取</summary>
long CMelSecIOController::ReadAddress(CString strDevType, int nStartDeviceNumber, int nSize, WORD* pValue)
{
	int nReadSize = nSize;
	if (nReadSize <= 0 || !pValue)
		return 0;

	short* pRead = NULL;
	long lRtn = ReadAddress(strDevType, nStartDeviceNumber, nReadSize, &pRead);

	BYTE* pSrc = (BYTE*)pRead, * pDst = (BYTE*)pValue;
	if (lRtn == 0 && pRead)
	{
		for (int i = 0; i < nReadSize; i++)
		{
			memcpy(pDst, pSrc, sizeof(short));
			pSrc += sizeof(short);
			pDst += sizeof(WORD);
		}
		delete[]pRead;
	}

	return lRtn;
}
///<summary>[float]軟元件讀取</summary>
long CMelSecIOController::ReadAddress(CString strDevType, int nStartDeviceNumber, int nSize, float* pValue)
{
	int nReadSize = nSize * 2;  // one float is stored in 2 word
	if (nReadSize <= 0)
		return 0;

	short* pRead = NULL;//輸出
	long lRtn = ReadAddress(strDevType, nStartDeviceNumber, nReadSize, &pRead); //ReadDeviceBlock2 => pRead

	BYTE* pSrc = (BYTE*)pRead,
		* pDst = (BYTE*)pValue;//各種建立size?
	//讀取成功 & 輸出有值
	if (lRtn == 0 && pRead)
	{
		for (int i = 0; i < nSize; i++)
		{
			memcpy(pDst, pSrc, sizeof(float));//實際複製鍵入pValue
			pSrc += sizeof(float);
			pDst += sizeof(float);
		}
		delete[]pRead;//回收
	}

	return lRtn;
}
///<summary>[char]軟元件讀取</summary>
long CMelSecIOController::ReadAddress(CString strDevType, int nStartDeviceNumber, int nSize, char* pValue)
{
	int nReadSize = nSize / 2;  // 2 char stored in 1 word
	if (nReadSize <= 0)
		return 0;

	short* pRead = NULL;
	long lRtn = ReadAddress(strDevType, nStartDeviceNumber, nReadSize, &pRead);

	BYTE* pSrc = (BYTE*)pRead, * pDst = (BYTE*)pValue;
	if (lRtn == 0 && pRead)
	{
		for (int i = 0; i < nSize; i++)
		{
			memcpy(pDst, pSrc, sizeof(BYTE));
			pSrc += sizeof(BYTE);
			pDst += sizeof(BYTE);
		}
		delete[]pRead;
	}

	return lRtn;
}
///<summary>[short]單一軟元件區塊讀取</summary>
long CMelSecIOController::ReadOneAddress(CString strDevType, int nStartDeviceNumber, short* pValue)
{
	long lRtn = ERR_DLL_NOT_LOAD;
	if (m_pIProgType)
	{
		EnterCriticalSection(&m_xLock);
		memset(pValue, 0, sizeof(short));
		CString strDevice;
		strDevice.Format(L"%s%d", strDevType, nStartDeviceNumber);
		BSTR bStr = strDevice.AllocSysString();
		m_pIProgType->GetDevice2(bStr, pValue, &lRtn);
		::SysFreeString(bStr);
		LeaveCriticalSection(&m_xLock);
	}
	return lRtn;
}
///<summary>[all]軟元件區塊讀取</summary>
long CMelSecIOController::ReadAddress(CString strDevType, int nStartDeviceNumber, int nSizeInWord, short** ppValue)
{
	long lRtn = ERR_DLL_NOT_LOAD;
#ifndef SUPPORT_AOI
	if (nSizeInWord)
	{
		if (m_pIProgType)//連線
		{
			EnterCriticalSection(&m_xLock);//Lock ON
			*ppValue = new short[nSizeInWord];
			memset(*ppValue, 0, nSizeInWord * sizeof(short));
			CString strDevice;
			strDevice.Format(L"%s%d", strDevType, nStartDeviceNumber);//D1000
			BSTR bStr = strDevice.AllocSysString();
			m_pIProgType->ReadDeviceBlock2(bStr, nSizeInWord, *ppValue, &lRtn);//MXCOMPONENT讀取 ->ppValue
			::SysFreeString(bStr);
			LeaveCriticalSection(&m_xLock);//Lock OFF
		}
	}
#endif
	return lRtn;
}
//------------------------
///<summary>軟元件隨機寫入</summary>
long CMelSecIOController::WriteRandom(CString& strList, int nSize, short* pData)
{
	long lRtn = ERR_DLL_NOT_LOAD;
	if (m_pIProgType)
	{
		EnterCriticalSection(&m_xLock);
		BSTR bStr = strList.AllocSysString();
		m_pIProgType->WriteDeviceRandom2(bStr, nSize, pData, &lRtn);
		::SysFreeString(bStr);
		LeaveCriticalSection(&m_xLock);
	}
	return lRtn;
}
///<summary>[WORD]軟元件寫入</summary>
long CMelSecIOController::WriteAddress(CString strDevType, int nDeviceNumber, int nSizeInByte, WORD* pWrite)
{
	int nWriteSize = nSizeInByte / 2;
	int nWordCount = nSizeInByte / sizeof(WORD);
	if (nWriteSize <= 0)
		return 0;

	long lRtn = WriteAddress(strDevType, nDeviceNumber, nWriteSize, (short*)pWrite);

	return lRtn;
}
///<summary>[float]軟元件寫入</summary>
long CMelSecIOController::WriteAddress(CString strDevType, int nDeviceNumber, int nSizeInByte, float* pWrite)
{
	int nWriteSize = nSizeInByte / 2;
	int nFloatCount = nSizeInByte / sizeof(float);
	if (nWriteSize <= 0)
		return 0;

	long lRtn = WriteAddress(strDevType, nDeviceNumber, nWriteSize, (short*)pWrite);

	return lRtn;
}
///<summary>[char]軟元件寫入</summary>
long CMelSecIOController::WriteAddress(CString strDevType, int nDeviceNumber, int nLength, char* pWrite)
{
	int nWriteSize = nLength / 2;
	if (nWriteSize <= 0)
		return 0;

	long lRtn = WriteAddress(strDevType, nDeviceNumber, nWriteSize, (short*)pWrite);

	return lRtn;
}
///<summary>[short]單一軟元件區塊寫入</summary>
long CMelSecIOController::WriteOneAddress(CString strDevice, short nValue)
{
	long lRtn = ERR_DLL_NOT_LOAD;
#ifndef SUPPORT_AOI
	if (m_pIProgType)
	{
		EnterCriticalSection(&m_xLock);
		BSTR bStr = strDevice.AllocSysString();
		m_pIProgType->SetDevice2(bStr, nValue, &lRtn);
		::SysFreeString(bStr);
		LeaveCriticalSection(&m_xLock);
	}
#endif
	return lRtn;
}
///<summary>[all]軟元件區塊寫入</summary>
long CMelSecIOController::WriteAddress(CString strDevType, int nStartDeviceNumber, int nSizeInWord, short* pValue)
{
	long lRtn = ERR_DLL_NOT_LOAD;
#ifndef SUPPORT_AOI
	if (nSizeInWord)
	{
		if (m_pIProgType)
		{
			EnterCriticalSection(&m_xLock);
			CString strDevice;
			strDevice.Format(L"%s%d", strDevType, nStartDeviceNumber);
			BSTR bStr = strDevice.AllocSysString();
			m_pIProgType->WriteDeviceBlock2(bStr, nSizeInWord, pValue, &lRtn);
			::SysFreeString(bStr);
			LeaveCriticalSection(&m_xLock);
		}
	}
#endif
	return lRtn;
}
//------------------------
///<summary>取得錯誤資訊</summary>
CString CMelSecIOController::GetErrorMessage(long lErrCode)
{
	CString strRtn;
	switch (lErrCode)
	{
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
			if (m_pISupportMsg)
			{
				BSTR bs = NULL;
				long lRtn = 0;
				m_pISupportMsg->GetErrorMessage(lErrCode, &bs, &lRtn);
				if (lRtn == 0)
				{
					bRtnDefault = FALSE;
					strRtn = bs;
				}
			}
			if (bRtnDefault)
			{
				strRtn.Format(L"0x%08x", lErrCode);
			}
			break;
	}
	return strRtn;
}
#endif
//------------------------
///<summary>取得軟元件名稱</summary>
CString CMelSecIOController::GetDeviceCString(GPIO_ITEM& xItem)
{
	CString strRtn;
	strRtn.Format(L"%c%X", xItem.cDeviceCode, xItem.uAddress);
	return strRtn;
}

