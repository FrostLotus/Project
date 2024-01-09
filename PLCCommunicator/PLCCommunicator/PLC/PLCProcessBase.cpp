#include "stdafx.h"
#include "PLCProcessBase.h"
#include "PLCCommunicator.h"
//#define OFF_LINE;

///<summary>[Constructor]��l��</summary>
CPLCProcessBase::CPLCProcessBase()
{
	Init();
}
///<summary>[Constructor]�׳B�m</summary>
CPLCProcessBase::~CPLCProcessBase()
{
	Finalize();
}
//=============================================================================
///<summary>��l�ƶ���</summary>
void CPLCProcessBase::Init()
{
	m_pPLCInitData = NULL;
	m_pPLCData = NULL;
	m_lLastRtn = 0;
	m_hAOIWnd = ::FindWindow(NULL, AOI_MASTER_NAME);
	m_pAOIUsm = new usm<unsigned char>(BATCH_AOI_MEM_ID, TRUE);
	m_pPLCUsm = new usm<unsigned char>(BATCH_COMMUNICATOR_MEM_ID, TRUE);
}
///<summary>�׳B�m����</summary>
void CPLCProcessBase::Finalize()
{
	if (m_pPLCInitData)
	{
		delete[] m_pPLCInitData;
		m_pPLCInitData = NULL;
	}
	if (m_pAOIUsm)
	{
		delete m_pAOIUsm;
		m_pAOIUsm = NULL;
	}
	if (m_pPLCUsm)
	{
		delete m_pPLCUsm;
		m_pPLCUsm = NULL;
	}
}
///<summary>PLC�}��Ĳ�o</summary>
long CPLCProcessBase::ON_OPEN_PLC(LPARAM lp)
{
	long lRtn = ERR_DLL_NOT_LOAD;
	if (m_pPLCInitData == NULL)
	{ //first open, read data from shared memory
		int nDataSize = 0;
		CString str = L"192.168.2.99";
		switch (lp)
		{
			case(WM_SYST_PARAMINIT_CMD): //CCL�Ѽƫإ�
			{
				BATCH_SHARE_SYSTCCL_INITPARAM* pData = new BATCH_SHARE_SYSTCCL_INITPARAM;//CCL����Ѽ�
				memset(pData, 0, sizeof(BATCH_SHARE_SYSTCCL_INITPARAM));
#ifdef OFF_LINE
				//_tcscpy_s(pData->cPLCIP, str.GetLength() + 1, str);
				memcpy(pData->cPLCIP, str.GetBuffer(), str.GetLength() * 2);
				pData->lTargetNetworkNo = 0;
				pData->lTargetStationNo = 0xFF;
#endif
				m_pPLCInitData = (BYTE*)pData;//��J
				nDataSize = sizeof(BATCH_SHARE_SYSTCCL_INITPARAM);//�ؤo
			}
			break;
			case(WM_SYST_PP_PARAMINIT_CMD): //PP�Ѽƫإ�
			{
				BATCH_SHARE_SYSTPP_INITPARAM* pData = new BATCH_SHARE_SYSTPP_INITPARAM;//PP����Ѽ�
				memset(pData, 0, sizeof(BATCH_SHARE_SYSTPP_INITPARAM));
#ifdef OFF_LINE
				//_tcscpy_s(pData->cPLCIP, str.GetLength() + 1, str);
				memcpy(pData->cPLCIP, str.GetBuffer(), str.GetLength() * 2);
				pData->lTargetNetworkNo = 0;
				pData->lTargetStationNo = 0xFF;
				pData->bFX5U = TRUE;
#endif
				m_pPLCInitData = (BYTE*)pData;//��J
				nDataSize = sizeof(BATCH_SHARE_SYSTPP_INITPARAM);//�ؤo
			}
			break;
		}
		//-------------------------------------
		if (m_pPLCInitData && nDataSize)//�ѼƦ���
		{
			theApp.InsertDebugLog(L"ON_OPEN_PLC", LOG_DEBUG);//log
#ifdef OFF_LINE
			/*CString str = L"192.168.2.29";
			wcscpy_s(xData.cPLCIP, str.GetBuffer());
			xData.lTargetNetworkNo = 0;
			xData.lTargetStationNo = 0xFF;*/
#else
			if (USM_ReadData(m_pPLCInitData, nDataSize))
#endif
			{
				switch (lp)
				{
					case(WM_SYST_PARAMINIT_CMD):
					{
						SET_INIT_PARAM(lp, m_pPLCInitData);//��?
						ON_SET_PLCPARAM(*(BATCH_SHARE_SYSTCCL_INITPARAM*)m_pPLCInitData);//��?
					}
					break;
					case(WM_SYST_PP_PARAMINIT_CMD):
					{
						SET_INIT_PARAM(lp, m_pPLCInitData);//��?
						ON_SET_PLCPARAM(*(BATCH_SHARE_SYSTPP_INITPARAM*)m_pPLCInitData);//��?
					}
					break;
				}
			}	
		}
	}
	if (m_pPLCInitData)
	{
		CString strLog;
		lRtn = OpenDevice(*(BATCH_SHARE_SYSTCCL_INITPARAM*)m_pPLCInitData);
		if (lRtn == 0)
		{
			strLog.Format(L"open plc success");
		}
		else
		{
			strLog.Format(L"open plc fail: %ld %s", lRtn, GetErrorMessage(lRtn));
		}
		theApp.InsertDebugLog(strLog);
	}
	return lRtn;
}
//--------------------
///<summary>Ū���ثe����s����ID(field)����(BYTE)</summary>
BYTE* CPLCProcessBase::GET_PLC_FIELD_BYTE_VALUE(int nFieldId)
{
	int nFieldSize = GetFieldSize();
	if (nFieldId >= 0 && nFieldId < nFieldSize)//�d��
	{
		const PLC_DATA_ITEM_* pCur = GetPLCAddressInfo(nFieldId, FALSE);
		if (pCur) //���o
		{
			return m_pPLCData[nFieldId].pData;
		}
	}
	return NULL;
}
///<summary>Ū���ثe����s�n����ƭ�</summary>
CString CPLCProcessBase::GET_PLC_FIELD_VALUE(int nFieldId)
{
	int nFieldSize = GetFieldSize();//�̤j��
	CString strDes;
	if (nFieldId >= 0 && nFieldId < nFieldSize)//�d��
	{
		const PLC_DATA_ITEM_* pCur = GetPLCAddressInfo(nFieldId, FALSE);
		if (pCur)
		{
			switch (pCur->xValType)//type
			{
				case PLC_TYPE_STRING://string
					strDes.Format(_T("%s"), CString((unsigned char*)m_pPLCData[nFieldId].pData));
					break;
				case PLC_TYPE_WORD://word
					if (pCur->uStartBit != -1 && pCur->uEndBit != -1)//���O�w�]��
					{
						int nValue = (int)*(unsigned short*)m_pPLCData[nFieldId].pData;
						int nTemp = 0;
						for (UINT i = pCur->uStartBit; i <= pCur->uEndBit; i++)
						{
							nTemp |= (nValue & 1 << i);
						}
						nTemp >> pCur->uStartBit;
						strDes.Format(_T("%d"), nTemp);
					}
					else
					{
						if (pCur->bSigned)
							strDes.Format(_T("%d"), (int)*(short*)m_pPLCData[nFieldId].pData);
						else
							strDes.Format(_T("%d"), (int)*(unsigned short*)m_pPLCData[nFieldId].pData);
					}
					break;
				case PLC_TYPE_FLOAT:
					strDes.Format(_T("%.2f"), *(float*)m_pPLCData[nFieldId].pData);
					break;
				case PLC_TYPE_DWORD:
					strDes.Format(_T("%d"), *((int*)m_pPLCData[nFieldId].pData));
					break;
				case PLC_TYPE_BIT:
					strDes.Format(_T("%d"), (int)*(short*)m_pPLCData[nFieldId].pData);
					break;
			}
		}
	}
	return strDes;
}
//--------------------
///<summary>[READ_RANDOM2]���o����ID��PLC���</summary>
void CPLCProcessBase::GET_PLC_RANDOM_DATA(vector<int>& vField, CString& strField, int& nSizeInWord)
{
	CString strTemp;
	for (auto& i : vField)
	{
		PLC_DATA_ITEM_* pItem = GetPLCAddressInfo(i, FALSE);
		if (pItem)
		{
			switch (pItem->xValType)
			{
				case PLC_VALUE_TYPE_::PLC_TYPE_WORD:
				case PLC_VALUE_TYPE_::PLC_TYPE_BIT:
				case PLC_VALUE_TYPE_::PLC_TYPE_FLOAT:
				case PLC_VALUE_TYPE_::PLC_TYPE_DWORD:
				case PLC_VALUE_TYPE_::PLC_TYPE_STRING:
				{

					int nSize = pItem->cLen / 2;
					nSizeInWord += nSize;
					for (int j = 0; j < nSize; j++)
					{
						if (pItem->xValType == PLC_VALUE_TYPE_::PLC_TYPE_BIT && CString(pItem->cDevType) == L"D")
						{ //special case for D. ex:200.F
							if (pItem->uStartBit != -1 && pItem->uStartBit == pItem->uEndBit)
								strTemp.Format(L"%s%d.%X", pItem->cDevType, pItem->uAddress + j, pItem->uStartBit);
							else
								ASSERT(FALSE);
						}
						else
							strTemp.Format(L"%s%d", pItem->cDevType, pItem->uAddress + j);
						if (strField.GetLength() == 0)
							strField = strTemp;
						else
							strField += L"\n" + strTemp;
					}
				}
				break;
				default:
					ASSERT(FALSE);
					break;
			}
		}
	}
}
///<summary>[Vector]��s�n���󪬺A</summary>
long CPLCProcessBase::GET_PLC_FIELD_DATA(vector<int>& vField)
{
	long lRtn = ERR_DLL_NOT_LOAD;
	CString strField, strTemp;
	int nTotal = 0;
	GET_PLC_RANDOM_DATA(vField, strField, nTotal);

	if (nTotal)
	{
		short* pData = new short[nTotal];
		memset(pData, 0, sizeof(short) * nTotal);
		lRtn = ReadRandom(strField, nTotal, pData);

		if (lRtn == 0)
		{
			BYTE* pCur = (BYTE*)pData;
			for (auto& i : vField)
			{
				PLC_DATA_ITEM_* pItem = GetPLCAddressInfo(i, FALSE);
				if (pItem)
				{
					//update time and data
					m_pPLCData[i].xTime = CTime::GetCurrentTime().GetTime();
					memcpy(m_pPLCData[i].pData, pCur, pItem->cLen);
					pCur += pItem->cLen;
				}
			}
			ON_BATCH_PLCDATA_CHANGE(*vField.begin(), *vField.rbegin());
		}
		delete[]pData;
	}
	return lRtn;
}
///<summary>��s�s�n���󪬺A</summary>
long CPLCProcessBase::GET_PLC_FIELD_DATA(int nFieldId)
{
	return GET_PLC_FIELD_DATA(nFieldId, m_pPLCData[nFieldId].pData);
}
///<summary>[READ_BLOCK2]���o����ID��PLC���</summary>
long CPLCProcessBase::GET_PLC_FIELD_DATA(int nFieldId, void* pData)
{
	long lRtn = ERR_DLL_NOT_LOAD;
	PLC_DATA_ITEM_* pItem = GetPLCAddressInfo(nFieldId, FALSE);//��ӹ���ID���Ū�^��
	if (pItem)//���\
	{
		switch (pItem->xValType)
		{
			case PLC_VALUE_TYPE_::PLC_TYPE_FLOAT:
				lRtn = ReadAddress(pItem->cDevType, pItem->uAddress, 1, (float*)pData);
				break;
			case PLC_VALUE_TYPE_::PLC_TYPE_STRING:
				lRtn = ReadAddress(pItem->cDevType, pItem->uAddress, pItem->cLen, (char*)pData);
				break;
			case PLC_VALUE_TYPE_::PLC_TYPE_WORD:
				lRtn = ReadAddress(pItem->cDevType, pItem->uAddress, 1, (WORD*)pData);
				break;
			case PLC_VALUE_TYPE_::PLC_TYPE_DWORD:
				lRtn = ReadAddress(pItem->cDevType, pItem->uAddress, 2, (WORD*)pData);
				break;
			case PLC_VALUE_TYPE_::PLC_TYPE_BIT:
				lRtn = ReadOneAddress(pItem->cDevType, pItem->uAddress, (short*)pData);
				break;
		}
		if (lRtn == 0)
		{
			m_pPLCData[nFieldId].xTime = CTime::GetCurrentTime().GetTime();
			ON_PLCDATA_CHANGE(nFieldId, pData, pItem->cLen);
		}
	}
	return lRtn;
}
//-------------------
///<summary>[Vector]�g�J�n����ƭ�</summary>
long CPLCProcessBase::SET_PLC_FIELD_DATA(vector<int>& vField, BYTE* pData)
{
	long lRtn = ERR_DLL_NOT_LOAD;
	CString strField;
	int nSizeInWord = 0;
	GET_PLC_RANDOM_DATA(vField, strField, nSizeInWord);
	if (nSizeInWord)
	{
		lRtn = WriteRandom(strField, nSizeInWord, (short*)pData);
	}
	return lRtn;
}
///<summary>[Device]�g�J�n����ƭ�</summary>
long CPLCProcessBase::SET_PLC_FIELD_DATA(int nFieldId, int nSizeInByte, BYTE* pData)
{
	long lRtn = ERR_DLL_NOT_LOAD;
	int nFieldSize = GetFieldSize();
	CString strDes;
	if (nFieldId >= 0 && nFieldId < nFieldSize)
	{
		const PLC_DATA_ITEM_* pItem = GetPLCAddressInfo(nFieldId, FALSE);
		//��l��
		BYTE* pOldValue = new BYTE[nSizeInByte];
		memcpy(pOldValue, m_pPLCData[nFieldId].pData, nSizeInByte);//���ƻs�Ŧ�m
		memcpy(m_pPLCData[nFieldId].pData, pData, nSizeInByte);//���
#ifdef SHOW_PERFORMANCE
		LARGE_INTEGER xStart, xEnd, xFreq;
		QueryPerformanceFrequency(&xFreq);
		QueryPerformanceCounter(&xStart);
#endif
		switch (pItem->xValType)
		{
			case PLC_TYPE_STRING:
				lRtn = WriteAddress(pItem->cDevType, pItem->uAddress, nSizeInByte, (char*)m_pPLCData[nFieldId].pData);
				break;
			case PLC_TYPE_WORD:
			case PLC_TYPE_DWORD:
				lRtn = WriteAddress(pItem->cDevType, pItem->uAddress, nSizeInByte, (WORD*)m_pPLCData[nFieldId].pData);
				break;
			case PLC_TYPE_FLOAT:
				lRtn = WriteAddress(pItem->cDevType, pItem->uAddress, nSizeInByte, (float*)m_pPLCData[nFieldId].pData);
				break;
			case PLC_TYPE_BIT:
			{
				CString strDevice = GET_PLC_FIELD_ADDRESS(nFieldId);
				lRtn = WriteOneAddress(strDevice, *(short*)m_pPLCData[nFieldId].pData);
			}
			break;
		}
		if (lRtn == 0)
		{
			m_pPLCData[nFieldId].xTime = CTime::GetCurrentTime().GetTime();
			ON_PLCDATA_CHANGE(nFieldId, pData, pItem->cLen);
		}
		else
		{
#ifdef SHOW_PERFORMANCE
			QueryPerformanceCounter(&xEnd);
			double d = (xEnd.QuadPart - xStart.QuadPart) * 1000.0 / xFreq.QuadPart;
			TRACE(L"write one field : %.2f \n", d);
#endif
			memcpy(m_pPLCData[nFieldId].pData, pOldValue, nSizeInByte); //�g�J����, �٭���
			CString strLog;
			
			strLog.Format(L"Address %d write failed, error code: %ld, %s", pItem->uAddress, lRtn, GetErrorMessage(lRtn));
			TRACE(L"%s \n", strLog);
			theApp.InsertDebugLog(strLog);
		}
		if (lRtn != m_lLastRtn)
		{
			m_lLastRtn = lRtn;
			if (lRtn == 0)
				ON_PLC_NOTIFY(L"PLC Normal");
			else
				ON_PLC_NOTIFY(L"PLC Error");
		}
		if (pOldValue)
		{
			delete[]pOldValue;
			pOldValue = NULL;
		}
	}
	return lRtn;
}
///<summary>[Bit]�g�J�n����ƭ�</summary>
long CPLCProcessBase::SET_PLC_FIELD_DATA_BIT(int nFieldStart, int nFieldEnd, int nSizeInByte, BYTE* pData)
{
	long lRtn = ERR_DLL_NOT_LOAD;
	int nFieldSize = GetFieldSize();
	const PLC_DATA_ITEM_* pWriteItem = GetPLCAddressInfo(nFieldStart, FALSE);
	lRtn = WriteAddress(pWriteItem->cDevType, pWriteItem->uAddress, nSizeInByte, (WORD*)pData);

	if (lRtn == 0)
	{
		for (int i = nFieldStart; i <= nFieldEnd; i++)
		{
			if (i >= 0 && i < nFieldSize)
			{
				const PLC_DATA_ITEM_* pItem = GetPLCAddressInfo(i, FALSE);
				if (pItem->uStartBit != -1 && pItem->uEndBit != -1)
				{
					int nValue = *(int*)pData;
					int nTemp = 0;
					for (int j = pItem->uStartBit; j <= pItem->uEndBit; j++)
					{
						nTemp |= (nValue & 1 << j);
					}
					m_pPLCData[i].xTime = CTime::GetCurrentTime().GetTime();
					memcpy(m_pPLCData[i].pData, &nTemp, pItem->cLen);
					ON_PLCDATA_CHANGE(i, &nTemp, pItem->cLen);
				}
			}
		}
	}
	return lRtn;
}
///<summary>[Bit]�g�J�n����ƭ�</summary>
long CPLCProcessBase::SET_PLC_FIELD_DATA_BIT(int nField, int nBitPosition, BOOL bValue)
{
	long lRtn = ERR_DLL_NOT_LOAD;

	const PLC_DATA_ITEM_* pCur = GetPLCAddressInfo(nField, FALSE);
	if (pCur)
	{
		CString strDevice;
		strDevice.Format(L"%s%d.%X", pCur->cDevType, pCur->uAddress, nBitPosition);
		lRtn = WriteOneAddress(strDevice, bValue);
		ASSERT(pCur->cLen == 2);
		if (lRtn == 0)
		{
			WORD wValue = *(WORD*)m_pPLCData[nField].pData;
			if (bValue)
				wValue |= (1 << nBitPosition);
			else
				wValue &= ~(1 << nBitPosition);

			memcpy(m_pPLCData[nField].pData, &wValue, 2);
			ON_PLCDATA_CHANGE(nField, m_pPLCData[nField].pData, pCur->cLen);
		}
	}
	return lRtn;
}
//-------------------
///<summary>Ū���n����ɶ����Ҽƭ�</summary>
CString CPLCProcessBase::GET_PLC_FIELD_TIME(int nFieldId)
{
	int nFieldSize = GetFieldSize();
	CString strDes;
	if (m_pPLCData)
	{
		if (nFieldId >= 0 && nFieldId < nFieldSize && m_pPLCData[nFieldId].xTime)
		{
			strDes = CTime::CTime(m_pPLCData[nFieldId].xTime).Format(L"%H:%M:%S");
		}
	}
	return strDes;
}
///<summary>���o����ID�n�����}</summary>
CString CPLCProcessBase::GET_PLC_FIELD_ADDRESS(int nFieldId)
{
	int nFieldSize = GetFieldSize();
	CString strDes;
	if (nFieldId >= 0 && nFieldId < nFieldSize)
	{
		const PLC_DATA_ITEM_* pCur = GetPLCAddressInfo(nFieldId, FALSE);
		if (pCur)
		{
			if (pCur->xValType == PLC_VALUE_TYPE_::PLC_TYPE_BIT && CString(pCur->cDevType) == L"D")
			{ //special case for D. ex:200.F
				if (pCur->uStartBit != -1 && pCur->uStartBit == pCur->uEndBit)
					strDes.Format(L"%s%d.%X", pCur->cDevType, pCur->uAddress, pCur->uStartBit);
				else
					ASSERT(FALSE);
			}
			else
			{
				strDes.Format(_T("%s%d"), pCur->cDevType, pCur->uAddress);
			}
		}
	}
	return strDes;
}
///<summary>���o����ID�n����W��</summary>
CString CPLCProcessBase::GET_PLC_FIELD_NAME(int nFieldId)
{
	int nFieldSize = GetFieldSize();
	CString strDes;
	if (nFieldId >= 0 && nFieldId < nFieldSize)
	{
		const PLC_DATA_ITEM_* pCur = GetPLCAddressInfo(nFieldId, FALSE);
		strDes.Format(_T("%s"), pCur->strFieldName);
	}
	return strDes;
}
///<summary>���o����ID�n����B�z�Ҧ�</summary>
PLC_ACTION_TYPE_ CPLCProcessBase::GET_PLC_FIELD_ACTION(int nFieldId)
{
	int nFieldSize = GetFieldSize();
	PLC_ACTION_TYPE_ eType = ACTION_NOTIFY;
	if (nFieldId >= 0 && nFieldId < nFieldSize)
	{
		const PLC_DATA_ITEM_* pCur = GetPLCAddressInfo(nFieldId, FALSE);
		if (pCur)
		{
			eType = pCur->xAction;
		}
	}
	return eType;
}
//-------------------
///<summary>��l��PLCDATA����</summary>
void CPLCProcessBase::INIT_PLC_DATA()
{
	//�Nm_pPLCData��l��
	int nMax = GetFieldSize();
	m_pPLCData = new PLCDATA[nMax];
	memset(m_pPLCData, 0, sizeof(PLCDATA) * nMax);
	for (int i = 0; i < GetFieldSize(); i++)
	{
		PLC_DATA_ITEM_* pItem = GetPLCAddressInfo(i, FALSE);
		int nDataSize = pItem->cLen;
		if (pItem->xValType == PLC_TYPE_STRING)
		{
			nDataSize = pItem->cLen + 1;//resize
		}
		m_pPLCData[i].pData = new BYTE[nDataSize];//�]�m���O�e��
		memset(m_pPLCData[i].pData, 0, nDataSize);
	}
}
///<summary>�B�mPLCDATA����</summary>
void CPLCProcessBase::DESTROY_PLC_DATA()
{
	if (m_pPLCData)
	{
		int nFieldSize = GetFieldSize();
		for (int i = 0; i < nFieldSize; i++)
		{
			if (m_pPLCData[i].pData)
			{
				delete[] m_pPLCData[i].pData;
				m_pPLCData[i].pData = NULL;
			}
		}

		delete[]m_pPLCData;
		m_pPLCData = NULL;
	}
}
///<summary>�q��AOI����</summary>
void CPLCProcessBase::NotifyAOI(WPARAM wp, LPARAM lp)
{
	if (m_hAOIWnd)//�����s�b
	{
		::PostMessage(m_hAOIWnd, WM_GPIO_MSG, wp, lp);//PLC to AOI
	}
}
///<summary>ShareMemoryŪ�����</summary>
BOOL CPLCProcessBase::USM_ReadData(BYTE* pData, int nSize, int nOffset)
{
	if (m_pAOIUsm)//��l�Ʀ����\
	{
		m_pAOIUsm->ReadData(pData, nSize, nOffset);//����
		return TRUE;
	}
	return FALSE;
}
///<summary>ShareMemory�g�J���</summary>
BOOL CPLCProcessBase::USM_WriteData(BYTE* pData, int nSize, int nOffset)
{
	if (m_pPLCUsm)
	{
		m_pPLCUsm->WriteData(pData, nSize, nOffset);
		return TRUE;
	}
	return FALSE;
}