#include "stdafx.h"
#include "MelsecPlcSocket.h"

BYTE CMelsecPlcSocket::ConvertByteToBCD(BYTE cData)
{
	BYTE cRet = NULL;
	if (cData >= 0 && cData <= 9){
		cRet = 0x30 + cData;
	}
	else if (cData > 9){
		cRet = 0x41 + (cData - 10);
	}
	return cRet;
}
BYTE CMelsecPlcSocket::ConvertBCDToByte(BYTE cData)
{
	BYTE cRet = NULL;
	if (cData >= '0' && cData <= '9'){
		cRet = cData - '0';
	}
	else if (cData > 9){
		cRet = cData - 'A' + 10;
	}
	return cRet;
}
DWORD CMelsecPlcSocket::ConvertWordToBCD(WORD cData, BOOL bEndianL)
{
	BYTE cResult[4] = {0,0,0,0};
	BYTE cIn = NULL;

	for (int i = 0; i < 4; i++){
		cIn = (cData>>(i*4)) & 0xF;
		if (bEndianL){ //Little Endian
			cResult[3-i] = ConvertByteToBCD(cIn);
		}
		else{ //Big Endian
			cResult[i] = ConvertByteToBCD(cIn);
		}
	}
	return *(DWORD*)cResult;
}
BYTE CMelsecPlcSocket::ConvertBCDToByte(WORD cData, BOOL bNum)
{
	BYTE cHigh = (cData >> 8) & 0xFF;
	BYTE cLow = (cData & 0xFF);
	BYTE cHightR = ConvertBCDToByte(cHigh);
	BYTE cLowR = ConvertBCDToByte(cLow);
	if (bNum){
		return ((cHightR << 4) | cLowR);
	}
	return (cHightR | (cLowR<<4));
}
WORD CMelsecPlcSocket::ConvertBCDToWORD(DWORD cData, BOOL bNum)
{
	WORD cHigh = (cData >> 16) & 0xFFFF;
	WORD cLow = (cData & 0xFFFF);
	BYTE cHightR = ConvertBCDToByte(cHigh,bNum);
	BYTE cLowR = ConvertBCDToByte(cLow,bNum);
	if (bNum){
		WORD cRet = cHightR;
		cRet = cRet << 8;
		cRet |= cLowR;
		return cRet;
	}
	WORD cRet = cLowR;
	cRet = cRet << 8;
	cRet |= cHightR;
	return cRet;
}

CMelsecPlcSocket::CMelsecPlcSocket(ISocketCallBack *pParent, PLC_FRAME_TYPE eFrameType) : CBaseClientSocket(pParent), m_eFrameType(eFrameType)
{
	Init();
}
CMelsecPlcSocket::CMelsecPlcSocket(PLC_FRAME_TYPE eFrameType) : CBaseClientSocket(this), m_eFrameType(eFrameType)
{
	Init();
}

CMelsecPlcSocket::~CMelsecPlcSocket()
{
	Finalize();
}
void CMelsecPlcSocket::Init()
{
	m_xMode = MODE_BINARY;
	m_xDev = DEV_MELSEC_Q_4E;
	m_uSerial = 1;

	for (int i = NULL; i < EV_COUNT; i++) m_hEvent[i] = ::CreateEvent(NULL, TRUE, FALSE, NULL);
	m_hThread = ::CreateThread(NULL, NULL, Thread_ProcessCmd, this, NULL, NULL);
	m_bSendFlag = TRUE;
}
void CMelsecPlcSocket::Finalize()
{
	::SetEvent(m_hEvent[EV_EXIT]);
}
vector<int> CMelsecPlcSocket::GetFieldFromAddress(UINT uAddress, UINT uLenInBytes)
{
	vector<int> vRtn;
	UINT uStartAddress = uAddress;
	UINT uEndAddress = uAddress + (uLenInBytes / 2);
	int nMax = GetFieldSize();
	for (int i = 0; i < nMax; i++){
		const PLC_DATA_ITEM_ *pCur = GetDataItem(i);
		if (pCur->uAddress >= uStartAddress && (pCur->uAddress + (pCur->cLen / 2) - 1) < uEndAddress){
			vRtn.push_back(pCur->xFieldType);
			//return pCur->xFieldType;
		}
	}
	if (vRtn.size() == 0) vRtn.push_back(-1);

	return vRtn;
}
void CMelsecPlcSocket::FIELD_CREATE_MAPPING_MEMORY()
{
	int nFieldSize = GetFieldSize();
	m_pFieldVal = new unsigned char*[nFieldSize];
	m_pFieldTime = new __time64_t[nFieldSize];
	memset(m_pFieldTime, 0, sizeof(__time64_t) * nFieldSize);
	for (int i = 0; i < nFieldSize; i++){
		m_pFieldVal[i] = NULL;
	}

	for (int i = 0; i < nFieldSize; i++){
		const PLC_DATA_ITEM_ *pItem = GetDataItem(i);
		if (pItem && pItem->cLen){
			if (m_pFieldVal[i] == NULL){
				int nDataSize = pItem->cLen;
				if (pItem->xValType == PLC_TYPE_STRING){
					nDataSize = pItem->cLen + 1;
				}
				m_pFieldVal[i] = new unsigned char[nDataSize];
				memset(m_pFieldVal[i], 0, nDataSize);
			}
		}
	}
}
void CMelsecPlcSocket::FIELD_DESTROY_MAPPING_MEMORY()
{
	int nFieldSize = GetFieldSize();
	if (m_pFieldVal){
		for (int i = 0; i < nFieldSize; i++){
			if (m_pFieldVal[i]){
				delete[] m_pFieldVal[i];
				m_pFieldVal[i] = NULL;
			}
		}
		delete[] m_pFieldVal;
	}
	if (m_pFieldTime){
		delete[]m_pFieldTime;
		m_pFieldTime = NULL;
	}
}

BOOL CMelsecPlcSocket::OnAddressValueNotify(UINT uAddress, unsigned short *pValue, UINT uLen) //uLen ==> bytes
{
	BOOL bProcess = FALSE;
	int nFieldSize = GetFieldSize();
	UINT uStartAddress = uAddress;
	UINT uEndAddress = uAddress + (uLen / 2);
	vector<int> vField = GetFieldFromAddress(uAddress, uLen);
	for (int &i : vField){
		if (i != -1){
			const PLC_DATA_ITEM_ *pCur = GetDataItem(i);
			if (pCur->uAddress >= uStartAddress &&
				(pCur->uAddress + (pCur->cLen / 2) - 1) < uEndAddress){
				UINT uAddressOffset = pCur->uAddress - uStartAddress;
				//Process Data (Put Data in Field Mapping~
				ProcessFieldData(PLC_READ_UPDATE, (void *)(pValue + uAddressOffset), uLen, *pCur);
				m_pFieldTime[i] = CTime::GetTickCount().GetTime();
				bProcess = TRUE;
				HandleAddressValueNotify(RESPONSE_OK, i); // for child class to implement
			}
		}
		else{
			HandleAddressValueNotify(RESPONSE_ERROR, i); // for child class to implement
		}
	}
	
	return bProcess;
}
void CMelsecPlcSocket::NotifyWriteAction(PLC_ACTION_FLAG_ eFlag, UINT uAddress, UINT uLenInBytes)
{
	CString strDes;
	switch (eFlag){
	case PLC_ACTION_FLAG_::RESPONSE_OK:
		strDes.Format(_T("ADDRESS:%d,SUCCESS!"), uAddress);
		NotifyDeviceVal(PLC_WRITE_NOTIFY, 1, strDes);
		break;
	case PLC_ACTION_FLAG_::RESPONSE_ERROR:
		strDes.Format(_T("ADDRESS:%d,FAIL!"), uAddress);
		NotifyDeviceVal(PLC_WRITE_NOTIFY, 0, strDes);
		break;
	}

	HandleWriteAction(eFlag, uAddress, uLenInBytes);// for child class to implement
}
void CMelsecPlcSocket::ProcessSockeData(unsigned char *pData, int nLen)
{
	int nProcLen = 0;
	int nBaseSize = sizeof(ASCII_3E_HEADER_ITEM) + sizeof(ASCII_BODY_ITEM_);
	if (m_eFrameType == FRAME_4E)
		nBaseSize = sizeof(ASCII_4E_HEADER_ITEM) + sizeof(ASCII_BODY_ITEM_);
	if (m_xMode == MODE_BINARY){
		if (m_eFrameType == FRAME_4E)
			nBaseSize = sizeof(BINARY_4E_HEADER_ITEM) + sizeof(BINARY_BODY_ITEM_);
		else if (m_eFrameType == FRAME_3E)
			nBaseSize = sizeof(BINARY_3E_HEADER_ITEM) + sizeof(BINARY_BODY_ITEM_);
	}
	while (nLen >= nBaseSize){
		BOOL bSuccess = FALSE;
		bSuccess = ProcessQCmdItem((void*)pData, nProcLen);
		if (bSuccess){
			nLen -= nProcLen;
			pData += nProcLen;
		}
		else{
			break;
		}
		if (nLen <= 0){
			break;
		}
	}
}
void CMelsecPlcSocket::PushCmdItem(PLC_MODE xMode, PLC_OP_CODE_ xOp, char cDevType, UINT uAddress, UINT uLenInWord, BYTE *pWrite)
{
	std::lock_guard< std::mutex > lock(m_oMutex);

	PLC_CMD_QUEUE_ITEM xCmd;
	memset(&xCmd, 0, sizeof(xCmd));
	xCmd.nMode = xMode;
	xCmd.xOp = xOp;
	xCmd.uAddress = uAddress;
	xCmd.uSize = uLenInWord;
	xCmd.uCmdSerialNo = m_uSerial;
	xCmd.cDevType = cDevType;
	if (xOp == PLC_BATCH_WRITE && pWrite){
		if (xMode == MODE_ASCII){
			xCmd.pWrite = new BYTE[uLenInWord*sizeof(DWORD)];
			memcpy(xCmd.pWrite, pWrite, uLenInWord*sizeof(DWORD));
		}
		else if (xMode == MODE_BINARY){
			xCmd.pWrite = new BYTE[uLenInWord*sizeof(WORD)]; 
			memcpy(xCmd.pWrite, pWrite, uLenInWord*sizeof(WORD));
		}
	}

	xCmd.dwTime = ::GetTickCount();

	m_vCmd.push_back(xCmd);

	m_uSerial++;
}
void CMelsecPlcSocket::DumpOverTimeCmd()
{
	DWORD dwCurrent = ::GetTickCount();
	DWORD dwCheck = NULL;
	for (;;){
		if (m_vCmd.size()){
			dwCheck = m_vCmd[0].dwTime;
			if ((dwCurrent - dwCheck) > 5000){
				m_vCmd.erase(m_vCmd.begin());
				continue;
			}
		}
		break;
	}
}
void CMelsecPlcSocket::ProcessErrState(unsigned short wErr)
{
	CString strErr;
	switch (wErr){
	case 0x0055:
		strErr = _T("Request the RUN-State CPU module for data writing!");
		break;
	case 0xC051:
	case 0xC052:
	case 0xC053:
	case 0xC054:
		strErr = _T("The number of read or write points is outside the allowable range");
		break;
	default:
		strErr.Format(_T("ERR CODE:%X"), wErr);
		break;
	}
	NotifyDeviceVal(PLC_ERR_NOTIFY, 0, strErr);
}
BOOL CMelsecPlcSocket::ProcessQCmdItem(void *pCmd, int &nProcLen)
{
	nProcLen = 0;
	BYTE *pCmdData = (BYTE*)pCmd;

	BOOL bStartFlag = FALSE;
	unsigned short wSerial = 0;
	unsigned short wErr = NULL;
	UINT uLen = 0;
	if (m_xMode == MODE_BINARY){
		WORD *pStart = (WORD*)pCmd;
		if (m_eFrameType == FRAME_3E && *pStart == 0x00D0){
			BINARY_BODY_ITEM_ *pBinary = (BINARY_BODY_ITEM_*)(pCmdData + sizeof(BINARY_3E_HEADER_ITEM));
			bStartFlag = TRUE;
			wErr = pBinary->wError;
			nProcLen += sizeof(BINARY_3E_HEADER_ITEM) + sizeof(BINARY_BODY_ITEM_);
			uLen = (UINT)pBinary->cPacketLen[0] + (UINT)pBinary->cPacketLen[1] * 16;
			uLen -= sizeof(pBinary->wError);
		}
		else if (m_eFrameType == FRAME_4E && *pStart == 0x00D4){
			BINARY_BODY_ITEM_ *pBinary = (BINARY_BODY_ITEM_*)(pCmdData + sizeof(BINARY_4E_HEADER_ITEM));
			bStartFlag = TRUE;
			wSerial = *(pStart + 1);
			wErr = pBinary->wError;
			nProcLen += sizeof(BINARY_4E_HEADER_ITEM) + sizeof(BINARY_BODY_ITEM_);
			uLen = (UINT)pBinary->cPacketLen[0] + (UINT)pBinary->cPacketLen[1] * 16;
			uLen -= sizeof(pBinary->wError);
		}
	}
	else if (m_xMode == MODE_ASCII){
		DWORD *pStart = (DWORD*)pCmd;
		if (m_eFrameType == FRAME_3E && *pStart == 0x30303044){
			ASCII_BODY_ITEM_ *pAscii = (ASCII_BODY_ITEM_*)(pCmdData + sizeof(ASCII_3E_HEADER_ITEM));
			bStartFlag = TRUE;
			nProcLen += sizeof(ASCII_3E_HEADER_ITEM) + sizeof(ASCII_BODY_ITEM);
			wErr = ConvertBCDToWORD(pAscii->dErr, FALSE);
			uLen = (UINT)ConvertBCDToWORD(*(DWORD*)pAscii->cPacketLen, FALSE);
			uLen -= sizeof(pAscii->dErr);
		}
		else if (m_eFrameType == FRAME_4E && *pStart == 0x30303444){
			ASCII_BODY_ITEM_ *pAscii = (ASCII_BODY_ITEM_*)(pCmdData + sizeof(ASCII_4E_HEADER_ITEM));
			bStartFlag = TRUE;
			nProcLen += sizeof(ASCII_4E_HEADER_ITEM) + sizeof(ASCII_BODY_ITEM);
			wSerial = ConvertBCDToWORD(*(pStart+1), TRUE);
			wErr = ConvertBCDToWORD(pAscii->dErr, FALSE);
			uLen = (UINT)ConvertBCDToWORD(*(DWORD*)pAscii->cPacketLen, FALSE);
			uLen -= sizeof(pAscii->dErr);
		}
	}
	
	if (bStartFlag){
		int nCmdSize = (int)m_vCmd.size();
		int nDstIdx = 0;
		if (nCmdSize){
			if (m_eFrameType == FRAME_4E){
				for (int i = 0; i < nCmdSize; i++){
					if (m_vCmd[i].uCmdSerialNo == wSerial){
						nDstIdx = i;
						break;
					}
				}
			}

			PLC_CMD_QUEUE_ITEM xCmd = m_vCmd[nDstIdx];
			BOOL bErr = FALSE;
			BOOL bProcessData = FALSE;
			auto CheckErrorState = [&](){
				if (wErr == 0){
					bProcessData = TRUE;
				}
				else{
					bErr = TRUE;
					ProcessErrState(wErr);
				}
			};
			if (wSerial == xCmd.uCmdSerialNo && m_eFrameType == FRAME_4E){
				CheckErrorState();
			}
			else if (m_eFrameType == FRAME_3E){
				CheckErrorState();
			}
			nProcLen += uLen;
			if (bProcessData){
				if (uLen){
					unsigned char *pData = NULL;
					int nUnit = 4;
					int nDataSize = 0;
					if (m_xMode == MODE_BINARY){
						if (m_eFrameType == FRAME_3E)
							pData = (unsigned char*)(pCmdData + sizeof(BINARY_3E_HEADER_ITEM) + sizeof(BINARY_BODY_ITEM_));
						else if (m_eFrameType == FRAME_4E)
							pData = (unsigned char*)(pCmdData + sizeof(BINARY_4E_HEADER_ITEM) + sizeof(BINARY_BODY_ITEM_));
						nUnit = 2;
						nDataSize = uLen / 2;
					}
					else if (m_xMode == MODE_ASCII){
						if (m_eFrameType == FRAME_3E)
							pData = (unsigned char*)(pCmdData + sizeof(ASCII_3E_HEADER_ITEM) + sizeof(ASCII_BODY_ITEM_));
						else if (m_eFrameType == FRAME_4E)
							pData = (unsigned char*)(pCmdData + sizeof(ASCII_4E_HEADER_ITEM) + sizeof(ASCII_BODY_ITEM_));
						nDataSize = uLen / 4;
					}
					unsigned char *pStart = pData;
					unsigned short *pValue = new unsigned short[nDataSize];
					unsigned short *pValueStart = pValue;
					while (uLen){
						if (m_xMode == MODE_ASCII){
							*pValueStart = ConvertBCDToWORD(*(DWORD*)pStart, FALSE);
						}
						else{
							*pValueStart = *(unsigned short*)pStart;
						}
						pStart += nUnit;
						pValueStart++;
						uLen -= nUnit;
					}
					BOOL bProc = OnAddressValueNotify(xCmd.uAddress, pValue, nDataSize*sizeof(WORD));
					if (!bProc){
						for (int i = 0; i < nDataSize; i++){
							CString strDes;
							strDes.Format(_T("ADDRESS:%d,VALUE:%d"), (xCmd.uAddress + i), (int)(*(pValue + i)));
							NotifyDeviceVal(PLC_ADDRESS_VAL, NULL, strDes);
						}
					}
					delete[]pValue;
				}
			}
			if (xCmd.xOp == PLC_BATCH_WRITE){
				auto itCmd = m_vCmd.begin() + nDstIdx;

				int nUnit = 0;
				if (itCmd->nMode == MODE_ASCII){
					nUnit = sizeof(DWORD);
				}
				else if (itCmd->nMode == MODE_BINARY){
					nUnit = sizeof(WORD);
				}

				int nField = GetFieldFromAddress(itCmd->uAddress, itCmd->uSize * sizeof(WORD)).at(0);
				int nBytesLeft = xCmd.uSize * 2;
				PLC_DATA_ITEM_ *pCur = GetDataItem(nField);
				PLC_DATA_ITEM_ *pNext = NULL;
				BYTE *pData = itCmd->pWrite;
				while (pCur && pCur->cLen <= nBytesLeft){
					if (itCmd->nMode == MODE_ASCII){
						int nLen = pCur->cLen + pCur->cLen % 2;
						unsigned char *pStart = pData;
						unsigned short *pValue = new unsigned short[nLen / 2];
						unsigned short *pValueStart = pValue;
						memset(pValue, 0, nLen);
						while (nLen > 0){
							*pValueStart = ConvertBCDToWORD(*(DWORD*)pStart, FALSE);
							pValueStart++;
							pStart += 4;
							nLen -= 2;
						}
						UpdateWriteField(itCmd->nMode, pCur->cLen, pCur->uAddress, (BYTE*)pValue);
						delete[]pValue;
					}
					else if (itCmd->nMode == MODE_BINARY){
						UpdateWriteField(itCmd->nMode, pCur->cLen, pCur->uAddress, pData);
					}
					NotifyWriteAction(bProcessData ? RESPONSE_OK : RESPONSE_ERROR, pCur->uAddress, pCur->cLen);
					nField++;

					if (nBytesLeft - pCur->cLen > 0){
						pNext = GetDataItem(nField);
						if (pNext){
							int nAddressCount = (pNext->uAddress - pCur->uAddress);
							pData += nAddressCount * nUnit;
							nBytesLeft -= nAddressCount * sizeof(WORD);
							pCur = pNext;
						}
						else{
							break;
						}
					}
					else{
						break;
					}
				}
			}
			if(m_eFrameType == FRAME_4E){
				auto itItem = m_vCmd.begin() + nDstIdx;
				if (itItem->pWrite) delete[] itItem->pWrite;
				m_vCmd.erase(itItem);
			}
			else if (m_eFrameType == FRAME_3E){
				::SetEvent(m_hEvent[EV_CMDRCV]);
			}
			return TRUE;
		}
	}
	return FALSE;
}
void CMelsecPlcSocket::ReadAddress(char cDevType, UINT uAddress, UINT uLenInWord)
{
	if (m_xDev == DEV_MELSEC_Q_4E){
		if (m_xMode == MODE_ASCII){
			OpAddressAsciiQ(PLC_BATCH_READ, cDevType, uAddress, uLenInWord, NULL);
		}
		else if (m_xMode == MODE_BINARY){
			OpAddressBinaryQ(PLC_BATCH_READ, cDevType, uAddress, uLenInWord, NULL);
		}
	}
	else{
		TRACE("DEVICE TYPE NOT SUPPORT!");
	}
}

template<typename T> void CMelsecPlcSocket::WriteAddress(char cDevType, UINT uAddress, UINT uLenInWord, T *pWrite)
{ //for WORD,DWORD,char []
	int nUnitSize = sizeof(T);
	if (m_xDev == DEV_MELSEC_Q_4E){
		if (m_xMode == MODE_ASCII){
			if (nUnitSize == 4 || nUnitSize == 2){  //FLOAT
				BYTE cData[8] = { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };
				if (nUnitSize == 4){
					BYTE *pVal = (BYTE*)pWrite;
					cData[0] = ConvertByteToBCD(((*(pVal + 1)) >> 4) & 0xF);	
					cData[1] = ConvertByteToBCD((*(pVal + 1)) & 0xF);			
					cData[2] = ConvertByteToBCD((*(pVal) >> 4) & 0xF);			
					cData[3] = ConvertByteToBCD((*(pVal)) & 0xF);				
					cData[4] = ConvertByteToBCD(((*(pVal + 3)) >> 4) & 0xF);	
					cData[5] = ConvertByteToBCD((*(pVal + 3)) & 0xF);			
					cData[6] = ConvertByteToBCD(((*(pVal + 2)) >> 4) & 0xF); 	
					cData[7] = ConvertByteToBCD((*(pVal + 2)) & 0xF);			
				}
				else{ //WORD
					BYTE *pVal = (BYTE*)pWrite;
					cData[0] = ConvertByteToBCD(((*(pVal + 1)) >> 4)&0xF);
					cData[1] = ConvertByteToBCD((*(pVal + 1)) &0xF);
					cData[2] = ConvertByteToBCD((*(pVal)>>4) & 0xF);
					cData[3] = ConvertByteToBCD((*(pVal)) & 0xF);
				}
				OpAddressAsciiQ(PLC_BATCH_WRITE, cDevType, uAddress, uLenInWord, (BYTE*)cData);
			}
			else if (nUnitSize == 1){ //char *
				BYTE *pNew = new BYTE[uLenInWord * 4];
				memset(pNew, 0, uLenInWord * 4);
				BYTE *pDst = pNew, *pSrc = (BYTE*)pWrite;
				for (int i = 0; i < uLenInWord; i++){
					*(pDst    ) = ConvertByteToBCD(((*(pSrc + 1)) >> 4) & 0xF);
					*(pDst + 1) = ConvertByteToBCD(((*(pSrc + 1))) & 0xF);
					*(pDst + 2) = ConvertByteToBCD(((*(pSrc)) >> 4) & 0xF);
					*(pDst + 3) = ConvertByteToBCD(((*(pSrc))) & 0xF);
					pDst += 4;
					pSrc += 2;
				}
				OpAddressAsciiQ(PLC_BATCH_WRITE, cDevType, uAddress, uLenInWord, (BYTE*)pNew);
				delete[]pNew;
			}
		}
		else if (m_xMode == MODE_BINARY){
			if (nUnitSize == 4 || nUnitSize == 2){ //DWORD , int ,WORD
				OpAddressBinaryQ(PLC_BATCH_WRITE, cDevType, uAddress, uLenInWord, (BYTE*)pWrite);
			}
			else if (nUnitSize == 1){ //char *
				OpAddressBinaryQ(PLC_BATCH_WRITE, cDevType, uAddress, uLenInWord, (BYTE*)pWrite);
			}
		}
	}
}
template<> void CMelsecPlcSocket::WriteAddress(char cDevType, UINT uAddress, UINT uLen, CString *pWrite)
{ //NOT YET
	
}

CString CMelsecPlcSocket::GET_PLC_FIELD_VALUE(int nFieldId)
{
	int nFieldSize = GetFieldSize();
	CString strDes;
	if (nFieldId >= 0 && nFieldId < nFieldSize){
		const PLC_DATA_ITEM_ *pCur = GetDataItem(nFieldId);
		switch (pCur->xValType){
		case PLC_TYPE_STRING:
			strDes.Format(_T("%s"), CString((unsigned char*)m_pFieldVal[nFieldId]));
			break;
		case PLC_TYPE_WORD:
			if (pCur->uStartBit != -1 && pCur->uEndBit != -1){
				int nValue = (int)*(unsigned short*)m_pFieldVal[nFieldId];
				int nTemp = 0;
				for (int i = pCur->uStartBit; i <= pCur->uEndBit; i++){
					nTemp |= (nValue & 1 << i);
				}
				nTemp = nTemp >> pCur->uStartBit;
				strDes.Format(_T("%d"), nTemp);
			}
			else
				strDes.Format(_T("%d"), (int)*(unsigned short*)m_pFieldVal[nFieldId]);
			break;
		case PLC_TYPE_FLOAT:
			strDes.Format(_T("%.2f"), *(float*)m_pFieldVal[nFieldId]);
			break;
		}
	}
	return strDes;
}
CString CMelsecPlcSocket::GET_PLC_FIELD_ADDRESS(int nFieldId)
{
	int nFieldSize = GetFieldSize();
	CString strDes;
	if (nFieldId >= 0 && nFieldId < nFieldSize){
		const PLC_DATA_ITEM_ *pCur = GetDataItem(nFieldId);
		strDes.Format(_T("%d"), pCur->uAddress);
	}
	return strDes;
}
CString CMelsecPlcSocket::GET_PLC_FIELD_NAME(int nFieldId)
{
	int nFieldSize = GetFieldSize();
	CString strDes;
	if (nFieldId >= 0 && nFieldId < nFieldSize){
		const PLC_DATA_ITEM_ *pCur = GetDataItem(nFieldId);
		strDes.Format(_T("%s"), pCur->strFieldName);
	}
	return strDes;
}
CString CMelsecPlcSocket::GET_PLC_FIELD_TIME(int nFieldId)
{
	int nFieldSize = GetFieldSize();
	CString strDes;
	if (nFieldId >= 0 && nFieldId < nFieldSize){
		strDes = CTime::CTime(m_pFieldTime[nFieldId]).Format(L"%H:%M:%S");
	}
	return strDes;
}
CString CMelsecPlcSocket::GetFieldInfoDes(int nFieldId)
{
	int nFieldSize = GetFieldSize();
	CString strDes;
	if (nFieldId >= 0 && nFieldId < nFieldSize){
		const PLC_DATA_ITEM_ *pCur = GetDataItem(nFieldId);
		switch (pCur->xValType){
		case PLC_TYPE_STRING:
		
			//CString ss(m_pFieldVal[nFieldId]);
			strDes.Format(_T("ADDRESS:%c%05d VALUE:%S"), pCur->cDevType, pCur->uAddress, m_pFieldVal[nFieldId]);
		
			break;
		case PLC_TYPE_WORD:
			strDes.Format(_T("ADDRESS:%c%05d VALUE:%d"), pCur->cDevType, pCur->uAddress, (int)*(unsigned short*)m_pFieldVal[nFieldId]);
			break;
		case PLC_TYPE_FLOAT:
			strDes.Format(_T("ADDRESS:%c%05d VALUE:%.6f"), pCur->cDevType, pCur->uAddress, *(float*)m_pFieldVal[nFieldId]);
			break;
		}
		DumpLog(strDes);
	}
	return strDes;
}

void CMelsecPlcSocket::GET_PLC_FIELD_DATA(int nFieldId)
{
	int nFieldSize = GetFieldSize();
	if (nFieldId >= 0 && nFieldId < nFieldSize){
		OpPlcField(PLC_FIELD_READ, nFieldId, NULL);
	}
}
PLC_ACTION_TYPE_ CMelsecPlcSocket::GET_PLC_FIELD_ACTION(int nFieldId)
{
	int nFieldSize = GetFieldSize();
	PLC_ACTION_TYPE_ eType = ACTION_NOTIFY;
	if (nFieldId >= 0 && nFieldId < nFieldSize){
		const PLC_DATA_ITEM_ *pCur = GetDataItem(nFieldId);
		if (pCur){
			eType = pCur->xAction;
		}
	}
	return eType;
}
void CMelsecPlcSocket::OpPlcField(PLC_FIELD_OP_ xOp, int nFieldId, void *pData)
{
	int nFieldSize = GetFieldSize();
	if (nFieldId < nFieldSize){
		const PLC_DATA_ITEM_ *pCur = GetDataItem(nFieldId);
		int nSIZE_WORD = pCur->cLen / sizeof(WORD);
		switch (xOp)
		{
		case PLC_FIELD_READ:
			ReadAddress(pCur->cDevType, pCur->uAddress, nSIZE_WORD); //Lens ==> Num of WORD
			break;
		case PLC_FIELD_WRITE: //NOT YET
			//WriteAddress(pCur->cDevType, pCur->uAddress, nSIZE_WORD, (unsigned char*)pData, nSIZE_WORD);
			WriteAddress(pCur->cDevType, pCur->uAddress, nSIZE_WORD, (unsigned char*)pData);
			break;
		default:
			break;
		}
	}
}
void CMelsecPlcSocket::ProcessFieldData(PLC_NOTIFY_ID_ eNotifyId, void *pData, UINT uDataBytes, const PLC_DATA_ITEM &xItem)
{
	if (pData && xItem.cLen){
		if (m_pFieldVal[xItem.xFieldType]){
			int nCopyBytes = xItem.cLen;
			if (nCopyBytes > uDataBytes){
				nCopyBytes = uDataBytes;
				CString strLog;
				strLog.Format(_T("Address:%d (%d,%d) FIELD DATA SIZE NOT MAPPING!"), xItem.uAddress, uDataBytes, (int)xItem.cLen);
				DumpLog(strLog);
			}
			switch (xItem.xValType){
			case PLC_TYPE_STRING:
				memcpy(m_pFieldVal[xItem.xFieldType], pData, nCopyBytes);
				break;
			case PLC_TYPE_WORD:
				memcpy(m_pFieldVal[xItem.xFieldType], pData, nCopyBytes);
				break;
			case PLC_TYPE_FLOAT:
				memcpy(m_pFieldVal[xItem.xFieldType], pData, nCopyBytes);
				break;
			}
			NotifyDeviceVal(eNotifyId, xItem.xFieldType, _T(""));
		}
		else{
			TRACE("NEED CREATE MAPPING MEMORY FIRST");
		}
	}
}

void CMelsecPlcSocket::QUERY_ALL_BATCH_INFO()
{
	int nFieldSize = GetFieldSize();
	for (int i = 0; i < nFieldSize; i++){
		const PLC_DATA_ITEM_ *pCur = GetDataItem(i);
		if (pCur->xAction == ACTION_BATCH){
			OpPlcField(PLC_FIELD_READ, pCur->xFieldType, NULL);
		}
	}
}

void CMelsecPlcSocket::MAKE_BINARY_REQUEST_ITEM(BYTE *pItem, PLC_OP_CODE_ xOp, char cDevType, UINT uAddress, UINT uLenInWord)
{//uLen for uLen WORD
	//Start Code '5400'
	int nHdrSize = sizeof(BINARY_3E_HEADER_ITEM);
	if(m_eFrameType == FRAME_4E){
		BINARY_4E_HEADER_ITEM *pHdr = (BINARY_4E_HEADER_ITEM*)pItem;
		pHdr->wStart = 0x0054;
		pHdr->wSerial = m_uSerial;
		pHdr->wReserved = 0x0000;
		nHdrSize = sizeof(BINARY_4E_HEADER_ITEM);
	}
	else if (m_eFrameType == FRAME_3E){
		BINARY_3E_HEADER_ITEM *pHdr = (BINARY_3E_HEADER_ITEM*)pItem;
		pHdr->wStart = 0x0050;
		nHdrSize = sizeof(BINARY_3E_HEADER_ITEM);
	}
	BINARY_BODY_ITEM_ *pBody = (BINARY_BODY_ITEM_*)(pItem + nHdrSize);
	BINARY_MSG_ITEM_ *pMsg = (BINARY_MSG_ITEM_*)(pItem + nHdrSize + sizeof(BINARY_BODY_ITEM_));

	//Access route
	pBody->cNetNo = 0;
	pBody->cPcNo = 0xFF;
	pBody->wDstIoNo = 0x03FF;
	pBody->cDstStationNo = 0;

	UINT uDataLen = 12 + uLenInWord*sizeof(WORD);
	if (xOp == PLC_OP_CODE_::PLC_BATCH_READ){
		uDataLen = 12;
		}
	//Request Length //12 byte
	pBody->cPacketLen[0] = uDataLen % 256;
	pBody->cPacketLen[1] = ((uDataLen / 256) % 256);

	//wTimer
	pBody->cTimer[0] = 0;
	pBody->cTimer[1] = 0;

	if (xOp == PLC_OP_CODE_::PLC_BATCH_READ){
		//Command 0401
		pMsg->wCommand = 0x0401;
	}
	else if (xOp == PLC_OP_CODE_::PLC_BATCH_WRITE){
		//Command 1401
		pMsg->wCommand = 0x1401;
	}

	//SubCommand 0000
	pMsg->wSubCommand = 0;
	//Dev Type
	switch (cDevType){ //Only Support Data Register/W Register
	case 'D':
		pMsg->cDevCode = 0xA8;
		break;
	case 'W':
		pMsg->cDevCode = 0xB4;
		break;
	}
	//Dev address
	pMsg->cDevNum[0] = uAddress % 256;
	pMsg->cDevNum[1] = (uAddress / 256) % 256;
	pMsg->cDevNum[2] = (uAddress / 65536) % 256;
	//Data Len
	pMsg->cDataLen[0] = uLenInWord % 256;
	pMsg->cDataLen[1] = (uLenInWord / 256) % 256;
}
void CMelsecPlcSocket::MAKE_ASCII_REQUEST_ITEM(BYTE *pItem, PLC_OP_CODE_ xOp, char cDevType, UINT uAddress, UINT uLenInWord)
{//uLen for uLen DWORD, pWrite ==> Ansi char Data
	//Start Code '5400'
	int nHdrSize = sizeof(ASCII_3E_HEADER_ITEM);
	if(m_eFrameType == FRAME_4E){
		ASCII_4E_HEADER_ITEM *pHdr = (ASCII_4E_HEADER_ITEM*)pItem;
		pHdr->dStart = 0x30303435;
		pHdr->dSerial = ConvertWordToBCD(m_uSerial, FALSE);
		pHdr->dReserved = 0x30303030;
		nHdrSize = sizeof(ASCII_4E_HEADER_ITEM);
	}
	else if (m_eFrameType == FRAME_3E){
		ASCII_3E_HEADER_ITEM *pHdr = (ASCII_3E_HEADER_ITEM*)pItem;
		pHdr->dStart = 0x30303035;
		nHdrSize = sizeof(ASCII_3E_HEADER_ITEM);
	}
	ASCII_BODY_ITEM *pBody = (ASCII_BODY_ITEM*)(pItem + nHdrSize);
	ASCII_MSG_ITEM_ *pMsg = (ASCII_MSG_ITEM_*)(pItem + nHdrSize + sizeof(ASCII_BODY_ITEM));

	//Access route
	pBody->wNetNo = 0x3030;
	pBody->wPcNo = 0x4646;
	pBody->dDstIoNo = 0x46463330;
	pBody->wDstStationNo = 0x3030;

	UINT uDataLen = 24 + uLenInWord*sizeof(DWORD);
	if (xOp == PLC_OP_CODE_::PLC_BATCH_READ){
		uDataLen = 24;
	}
	//Request Length //24 byte
	*(DWORD*)pBody->cPacketLen = ConvertWordToBCD((unsigned short)uDataLen, TRUE);
	//Timer
	pBody->cTimer[0] = 0x30;
	pBody->cTimer[1] = 0x30;
	pBody->cTimer[2] = 0x30;
	pBody->cTimer[3] = 0x30;

	if (xOp == PLC_OP_CODE_::PLC_BATCH_READ){
		//Command 0401
		pMsg->dCommand = 0x31303430;
	}
	else if (xOp == PLC_OP_CODE_::PLC_BATCH_WRITE){
		//Command 1401
		pMsg->dCommand = 0x31303431;
	}

	//SubCommand 0000
	pMsg->dSubCommand = 0x30303030;
	//Dev Type
	pMsg->cDevCode[0] = cDevType;
	pMsg->cDevCode[1] = '*';
	//Dev address
	pMsg->cDevNum[0] = 0x30 + (((uAddress / 100000) % 10) & 0xFF);
	pMsg->cDevNum[1] = 0x30 + (((uAddress / 10000) % 10) & 0xFF);
	pMsg->cDevNum[2] = 0x30 + (((uAddress / 1000) % 10) & 0xFF);
	pMsg->cDevNum[3] = 0x30 + (((uAddress / 100) % 10) & 0xFF);
	pMsg->cDevNum[4] = 0x30 + (((uAddress / 10) % 10) & 0xFF);
	pMsg->cDevNum[5] = 0x30 + ((uAddress % 10) & 0xFF);
	//Data Len
	*(DWORD*)pMsg->cDataLen = ConvertWordToBCD((unsigned short)uLenInWord, TRUE);
}

void CMelsecPlcSocket::UpdateWriteField(int nMode, UINT uSizeInBytes, UINT uAddress, BYTE *pWrite)
{
	vector<int> vField = GetFieldFromAddress(uAddress, uSizeInBytes);
	for (auto &i : vField){
		ProcessFieldData(PLC_WRITE_UPDATE, pWrite, uSizeInBytes, *GetDataItem(i));
		m_pFieldTime[i] = CTime::GetTickCount().GetTime();
	}
}

void CMelsecPlcSocket::OpAddressAsciiQ(PLC_OP_CODE_ xOp, char cDevType, UINT uAddress, UINT uLenInWord, BYTE *pWrite) //DATA LEN: uLen * sizeof(DWORD)
{
	if (m_eFrameType == FRAME_4E){
		ASCII_4E_REQUEST_ITEM xItem;
		memset(&xItem, 0, sizeof(xItem));
		MAKE_ASCII_REQUEST_ITEM((BYTE*)&xItem, xOp, cDevType, uAddress, uLenInWord);

		if (xOp == PLC_BATCH_WRITE){
			NotifyWriteAction(REQUEST, uAddress, uLenInWord * 2); // notify child class to know start request
			Send(&xItem, sizeof(xItem));
			Send(pWrite, uLenInWord*sizeof(DWORD));
		}
		else if (xOp == PLC_BATCH_READ){
			Send(&xItem, sizeof(xItem));
		}
		PushCmdItem(MODE_ASCII, xOp, cDevType, uAddress, uLenInWord, pWrite);
	}
	else if (m_eFrameType == FRAME_3E){
		PushCmdItem(MODE_ASCII, xOp, cDevType, uAddress, uLenInWord, pWrite);
		::SetEvent(m_hEvent[EV_CMDSEND]);
	}
}
void CMelsecPlcSocket::OpAddressBinaryQ(PLC_OP_CODE_ xOp, char cDevType, UINT uAddress, UINT uLenInWord, BYTE *pWrite) //DATA LEN: uLen * sizeof(WORD)
{
	if (m_eFrameType == FRAME_4E){
		BINARY_4E_REQUEST_ITEM xItem;
		memset(&xItem, 0, sizeof(xItem));
		MAKE_BINARY_REQUEST_ITEM((BYTE*)&xItem, xOp, cDevType, uAddress, uLenInWord);
		if (xOp == PLC_OP_CODE_::PLC_BATCH_WRITE){
			NotifyWriteAction(REQUEST, uAddress, uLenInWord * 2); // notify child class to know start request
			Send(&xItem, sizeof(xItem));
			Send(pWrite, uLenInWord*sizeof(WORD));
		}
		else if (xOp == PLC_BATCH_READ){
			Send(&xItem, sizeof(xItem));
		}
		PushCmdItem(MODE_BINARY, xOp, cDevType, uAddress, uLenInWord, pWrite);
	}
	else if (m_eFrameType == FRAME_3E){
		PushCmdItem(MODE_BINARY, xOp, cDevType, uAddress, uLenInWord, pWrite);
		::SetEvent(m_hEvent[EV_CMDSEND]);
	}
}

void CMelsecPlcSocket::SendCmd()
{
	if (m_bSendFlag && m_vCmd.size()){
		PLC_CMD_QUEUE_ITEM& xCmdItem = m_vCmd.at(0);

		BINARY_3E_REQUEST_ITEM xBinary3EItem;
		memset(&xBinary3EItem, 0, sizeof(xBinary3EItem));
		BINARY_4E_REQUEST_ITEM xBinary4EItem;
		memset(&xBinary4EItem, 0, sizeof(xBinary4EItem));
		ASCII_4E_REQUEST_ITEM xAscii4EItem;
		memset(&xAscii4EItem, 0, sizeof(xAscii4EItem));
		ASCII_3E_REQUEST_ITEM xAscii3EItem;
		memset(&xAscii3EItem, 0, sizeof(xAscii3EItem));

		BYTE *pItem = NULL;
		int nSize = 0;
		if (xCmdItem.nMode == MODE_ASCII){
			if (m_eFrameType == FRAME_3E){
				pItem = (BYTE*)&xAscii3EItem;
				MAKE_ASCII_REQUEST_ITEM(pItem, xCmdItem.xOp, xCmdItem.cDevType, xCmdItem.uAddress, xCmdItem.uSize);
				nSize = sizeof(ASCII_3E_REQUEST_ITEM);
			}
			else if(m_eFrameType == FRAME_4E){
				pItem = (BYTE*)&xAscii4EItem;
				MAKE_ASCII_REQUEST_ITEM(pItem, xCmdItem.xOp, xCmdItem.cDevType, xCmdItem.uAddress, xCmdItem.uSize);
				nSize = sizeof(ASCII_4E_REQUEST_ITEM);
			}
		}
		else if (xCmdItem.nMode == MODE_BINARY){
			if (m_eFrameType == FRAME_3E){
				pItem = (BYTE*)&xBinary3EItem;
				MAKE_BINARY_REQUEST_ITEM(pItem, xCmdItem.xOp, xCmdItem.cDevType, xCmdItem.uAddress, xCmdItem.uSize);
				nSize = sizeof(BINARY_3E_REQUEST_ITEM);
			}
			else if (m_eFrameType == FRAME_4E){
				pItem = (BYTE*)&xBinary4EItem;
				MAKE_BINARY_REQUEST_ITEM(pItem, xCmdItem.xOp, xCmdItem.cDevType, xCmdItem.uAddress, xCmdItem.uSize);
				nSize = sizeof(BINARY_4E_REQUEST_ITEM);
			}
		}
		if (pItem && nSize){
			if (xCmdItem.xOp == PLC_OP_CODE_::PLC_BATCH_WRITE){
				int nLenInBytes = 0; 
				if (xCmdItem.nMode == MODE_ASCII){
					NotifyWriteAction(REQUEST, xCmdItem.uAddress, xCmdItem.uSize * 2); // notify child class to know start request
				}
				else{
					NotifyWriteAction(REQUEST, xCmdItem.uAddress, xCmdItem.uSize * 2);
				}

				Send(pItem, nSize);
				
				if (xCmdItem.pWrite){
					if (xCmdItem.nMode == MODE_ASCII){
						Send(xCmdItem.pWrite, xCmdItem.uSize *sizeof(DWORD));
						DumpWritePacket(pItem, nSize, xCmdItem.pWrite, xCmdItem.uSize *sizeof(DWORD));
					}
					else if (xCmdItem.nMode == MODE_BINARY){
						Send(xCmdItem.pWrite, xCmdItem.uSize *sizeof(WORD));
						DumpWritePacket(pItem, nSize, xCmdItem.pWrite, xCmdItem.uSize *sizeof(WORD));
					}
				}
			}
			else if (xCmdItem.xOp == PLC_BATCH_READ){
				Send(pItem, nSize);
			}
			else{
				ASSERT(FALSE);
			}
		}
		m_bSendFlag = FALSE;
	}
}

DWORD CMelsecPlcSocket::Thread_ProcessCmd(void* pvoid)
{
	CMelsecPlcSocket* pThis = (CMelsecPlcSocket*)pvoid;
	BOOL              bRun = TRUE;
	while (bRun)
	{
		switch (::WaitForMultipleObjects(EV_COUNT, pThis->m_hEvent, FALSE, INFINITE))
		{
		case CASE_CMDSEND:
		{
			::ResetEvent(pThis->m_hEvent[EV_CMDSEND]);
			{
				std::lock_guard< std::mutex > lock(pThis->m_oMutex);
				pThis->SendCmd();
			}
		}
		break;
		case CASE_CMDRCV:
		{
			::ResetEvent(pThis->m_hEvent[EV_CMDRCV]);
			{
				std::lock_guard< std::mutex > lock(pThis->m_oMutex);

				if (pThis->m_vCmd.size()){
					auto itItem = pThis->m_vCmd.begin();
					if (itItem->pWrite) delete[] itItem->pWrite;
					pThis->m_vCmd.erase(itItem);
				}
				pThis->m_bSendFlag = TRUE;
			}
			::SetEvent(pThis->m_hEvent[EV_CMDSEND]);
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
