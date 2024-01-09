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
///<summary>��l��</summary>
void CMelSecIOController::Init()
{
	InitializeCriticalSection(&m_xLock);
	m_bInit = FALSE;
	m_pIProgType = NULL;
	m_pISupportMsg = NULL;
}
///<summary>�׳B�m</summary>
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
///<summary>ActProgType���J</summary>
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
///<summary>ActProgType����</summary>
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
///<summary>�ƦC�Ҧ��x��IP</summary>
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
		theApp.InsertDebugLog(L"MachineIP: " + strIp, LOG_SYSTEM);//�NIP List �� Log
		pNextAd = pNextAd->Next;
	}
	delete (BYTE*)pAdptInfo;
}
///<summary>���oCPU����</summary>
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
///<summary>�}��PLC�˸m</summary>
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
	//�s������-------------------------------------------------------
	long lRtn = ERR_DLL_NOT_LOAD;
	if (m_pIProgType)//�YDLL�s�b
	{
		//�]�w�s��---------------------------------------------------
		SetMXParam(m_pIProgType, xData);//�]�w�}��PLC - MXComponent

		BSTR bStr = m_strIp.AllocSysString();//�NIP��BasicString
		m_pIProgType->put_ActHostAddress(bStr);//��JIP(0x000...)
		::SysFreeString(bStr);

		m_pIProgType->put_ActProtocolType(PROTOCOL_TCPIP);

		if (bLog)
		{
			LONG lTimeOut = 0;
			m_pIProgType->get_ActTimeOut(&lTimeOut);
			LogData(L"TimeOutSetting", lTimeOut);
			ListAllIP();//for debug, check the machine has correct ip address 
		}
		//PLC�s��----------------------------------------------------
		m_pIProgType->Open(&lRtn);//���ճs��
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
///<summary>�n�����H��Ū��</summary>
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
///<summary>[WORD]�n����Ū��</summary>
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
///<summary>[float]�n����Ū��</summary>
long CMelSecIOController::ReadAddress(CString strDevType, int nStartDeviceNumber, int nSize, float* pValue)
{
	int nReadSize = nSize * 2;  // one float is stored in 2 word
	if (nReadSize <= 0)
		return 0;

	short* pRead = NULL;//��X
	long lRtn = ReadAddress(strDevType, nStartDeviceNumber, nReadSize, &pRead); //ReadDeviceBlock2 => pRead

	BYTE* pSrc = (BYTE*)pRead,
		* pDst = (BYTE*)pValue;//�U�ثإ�size?
	//Ū�����\ & ��X����
	if (lRtn == 0 && pRead)
	{
		for (int i = 0; i < nSize; i++)
		{
			memcpy(pDst, pSrc, sizeof(float));//��ڽƻs��JpValue
			pSrc += sizeof(float);
			pDst += sizeof(float);
		}
		delete[]pRead;//�^��
	}

	return lRtn;
}
///<summary>[char]�n����Ū��</summary>
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
///<summary>[short]��@�n����϶�Ū��</summary>
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
///<summary>[all]�n����϶�Ū��</summary>
long CMelSecIOController::ReadAddress(CString strDevType, int nStartDeviceNumber, int nSizeInWord, short** ppValue)
{
	long lRtn = ERR_DLL_NOT_LOAD;
#ifndef SUPPORT_AOI
	if (nSizeInWord)
	{
		if (m_pIProgType)//�s�u
		{
			EnterCriticalSection(&m_xLock);//Lock ON
			*ppValue = new short[nSizeInWord];
			memset(*ppValue, 0, nSizeInWord * sizeof(short));
			CString strDevice;
			strDevice.Format(L"%s%d", strDevType, nStartDeviceNumber);//D1000
			BSTR bStr = strDevice.AllocSysString();
			m_pIProgType->ReadDeviceBlock2(bStr, nSizeInWord, *ppValue, &lRtn);//MXCOMPONENTŪ�� ->ppValue
			::SysFreeString(bStr);
			LeaveCriticalSection(&m_xLock);//Lock OFF
		}
	}
#endif
	return lRtn;
}
//------------------------
///<summary>�n�����H���g�J</summary>
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
///<summary>[WORD]�n����g�J</summary>
long CMelSecIOController::WriteAddress(CString strDevType, int nDeviceNumber, int nSizeInByte, WORD* pWrite)
{
	int nWriteSize = nSizeInByte / 2;
	int nWordCount = nSizeInByte / sizeof(WORD);
	if (nWriteSize <= 0)
		return 0;

	long lRtn = WriteAddress(strDevType, nDeviceNumber, nWriteSize, (short*)pWrite);

	return lRtn;
}
///<summary>[float]�n����g�J</summary>
long CMelSecIOController::WriteAddress(CString strDevType, int nDeviceNumber, int nSizeInByte, float* pWrite)
{
	int nWriteSize = nSizeInByte / 2;
	int nFloatCount = nSizeInByte / sizeof(float);
	if (nWriteSize <= 0)
		return 0;

	long lRtn = WriteAddress(strDevType, nDeviceNumber, nWriteSize, (short*)pWrite);

	return lRtn;
}
///<summary>[char]�n����g�J</summary>
long CMelSecIOController::WriteAddress(CString strDevType, int nDeviceNumber, int nLength, char* pWrite)
{
	int nWriteSize = nLength / 2;
	if (nWriteSize <= 0)
		return 0;

	long lRtn = WriteAddress(strDevType, nDeviceNumber, nWriteSize, (short*)pWrite);

	return lRtn;
}
///<summary>[short]��@�n����϶��g�J</summary>
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
///<summary>[all]�n����϶��g�J</summary>
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
///<summary>���o���~��T</summary>
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
///<summary>���o�n����W��</summary>
CString CMelSecIOController::GetDeviceCString(GPIO_ITEM& xItem)
{
	CString strRtn;
	strRtn.Format(L"%c%X", xItem.cDeviceCode, xItem.uAddress);
	return strRtn;
}

