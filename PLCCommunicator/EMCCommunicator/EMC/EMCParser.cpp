#include "stdafx.h"
#include "EMCParser.h"

//From AoiFormatParser
vector<CString> GetEachStringBySep(CString sStr, TCHAR tSep)
{
	vector<CString> vEachStr;
	TCHAR *token = NULL;
	TCHAR *pNextToken = sStr.GetBuffer();
	TCHAR xSep[2] = { NULL, NULL };
	xSep[0] = tSep;
	do{
		while (*pNextToken && (*pNextToken == tSep)){
			vEachStr.push_back(CString(_T('')));
			pNextToken++;
		}
		token = _tcstok_s(NULL, xSep, &pNextToken);
		if (token){
			CString xNew(token);
			if (xNew != _T("\r\n")){
				vEachStr.push_back(xNew);
			}
		}
	} while (token != NULL);
	return vEachStr;
}

CEMCParser::CEMCParser()
{

}
CEMCParser::~CEMCParser()
{

}
void CEMCParser::InitCCLData(EMC_CCL_DATA &xData)
{
	xData.strStation = L"";
	xData.strMissionID = L"";
	xData.strBatchName = L"";
	xData.strMaterial = L"";
	xData.eStatus = EMC_MISSION_STATUS::EMS_NONE;
	xData.strDefectType = L"";
	xData.strEmpID = L"";
	xData.xTime = 0;

	xData.strSerial = L"";
	xData.nNum = 0;
	xData.nBookNum = 0;
	xData.nSheetNum = 0;
	xData.nSplit = 0;
	xData.vMiss.clear();
	xData.nIndex = 0;
}
void CEMCParser::InitPPData(EMC_PP_DATA &xData)
{
	xData.strStation = L"";
	xData.strMissionID = L"";
	xData.strBatchName = L"";
	xData.strMaterial = L"";
	xData.eStatus = EMC_MISSION_STATUS::EMS_NONE;
	xData.strDefectType = L"";
	xData.strEmpID = L"";
	xData.xTime = 0;

	xData.fLength = 0;
	xData.fDefectBegin = 0;
	xData.fDefectEnd = 0;
	xData.vSerial.clear();
}
BOOL CEMCParser::ParseCCL(CString &strData, vector<EMC_CCL_DATA> &vParam)
{
	vParam.clear();
	vector<CString> vData = GetEachStringBySep(strData, EMC_FIELD_SPLITER);
	WORD wAllField = 0;
	WORD wField = 0;
	for (auto &i : ctEMC_CCL_FIELD){
		if (i.bCheckExist && i.nFieldType != EMC_CCL_FIELD_TYPE::CCL_ST/*1裁3第二筆工單沒有狀態*/) wAllField |= 1 << i.nFieldType;
	}
	EMC_CCL_DATA xData;
	for (CString &i : vData){
		if (i.GetLength() >= EMC_FIELD_LENGTH){
			int nField;
			BOOL bFieldOK = GetField(i.Mid(0, EMC_FIELD_LENGTH), PRODUCT_TYPE::CCL, nField);

			if (bFieldOK){
				CString strValue = i.Mid(EMC_FIELD_LENGTH, i.GetLength() - EMC_FIELD_LENGTH);

				EMC_CCL_FIELD_TYPE eField = (EMC_CCL_FIELD_TYPE)nField;
				//有重複欄位, 則視為下一筆工單開始
				if (wField & (1 << eField) && eField != CCL_E1/*少組訊息會重複*/){

					if (wAllField > wField){//一裁n時, 每筆下發資訊都需完整, 否則當作錯誤(不完整工單)
						break;
					}

					xData.xTime = CTime::GetTickCount();
					vParam.push_back(xData);
					//init again
					InitCCLData(xData);
					wField = 0;
				}
				if (eField != EMC_CCL_FIELD_TYPE::CCL_ST)//一裁三的第二筆沒有狀態
					wField |= (1 << eField);

				switch (eField)
				{
				case CCL_SC:
					xData.strStation = strValue;
					break;
				case CCL_NO:
					xData.strMissionID = strValue;
					break;
				case CCL_SO:
					xData.strBatchName = strValue;
					break;
				case CCL_PN:
					xData.strMaterial = strValue;
					break;
				case CCL_LT:
					xData.strSerial = strValue;
					break;
				case CCL_QT:
					xData.nNum = _ttoi(strValue);
					break;
				case CCL_BK:
					xData.nBookNum = _ttoi(strValue);
					break;
				case CCL_QS:
					xData.nSheetNum = _ttoi(strValue);
					break;
				case CCL_UP:
					xData.nSplit = _ttoi(strValue);
					break;
				case CCL_ST:
					xData.eStatus = GetStatus(strValue);
					break;
				case CCL_BB:
					xData.nBeginBook = _ttoi(strValue);
					break;
				case CCL_EB:
					xData.nEndBook = _ttoi(strValue);
					break;
				case CCL_BS:
					xData.nBeginSheet = _ttoi(strValue);
					break;
				case CCL_ES:
					xData.nEndSheet = _ttoi(strValue);
					break;
				case CCL_E1:
				{
					vector<CString> vTemp = GetEachStringBySep(strValue, EMC_DATA_SPLITER);
					if (vTemp.size() == 2 && vTemp.at(0).GetLength() > 0 && vTemp.at(1).GetLength() > 0){
						xData.vMiss.push_back({ _ttoi(vTemp.at(0)), _ttoi(vTemp.at(1)) });
					}
				}
				break;
				case CCL_CN:
					xData.strEmpID = strValue;
					break;
				default:
					ASSERT(FALSE);
					break;
				}
			}
		}
	}
	xData.xTime = CTime::GetTickCount();
	vParam.push_back(xData);//last one or the only one 

	BOOL bRtn = wAllField <= wField;

	if (!bRtn)//1裁n狀況下, 要每筆工單都是完整的
		vParam.clear();

	return bRtn;
}
BOOL CEMCParser::ParsePP(CString &strData, EMC_PP_DATA &xData)
{
	InitPPData(xData);
	WORD wAllField = 0;
	WORD wField = 0;
	for (auto &i : ctEMC_PP_FIELD){
		if (i.bCheckExist) wAllField |= 1 << i.nFieldType;
	}

	vector<CString> vData = GetEachStringBySep(strData, EMC_FIELD_SPLITER);
	for (CString &i : vData){
		if (i.GetLength() >= EMC_FIELD_LENGTH){
			int nField = 0;
			BOOL bFieldOK = GetField(i.Mid(0, EMC_FIELD_LENGTH), PRODUCT_TYPE::PP, nField);

			if (bFieldOK){
			EMC_PP_FIELD_TYPE eField = (EMC_PP_FIELD_TYPE)nField;
			CString strValue = i.Mid(EMC_FIELD_LENGTH, i.GetLength() - EMC_FIELD_LENGTH);
			
				wField |= (1 << eField);
				switch (eField)
				{
				case PP_SC:
					xData.strStation = strValue;
					break;
				case PP_NO:
					xData.strMissionID = strValue;
					break;
				case PP_SO:
					xData.strBatchName = strValue;
					break;
				case PP_PN:
					xData.strMaterial = strValue;
					break;
				case PP_LT:
					xData.vSerial.push_back(strValue);
					break;
				case PP_QT:
					xData.fLength = (float)_ttof(strValue);
					break;
				case PP_QB:
					xData.fDefectBegin = (float)_ttof(strValue);
					break;
				case PP_QE:
					xData.fDefectEnd = (float)_ttof(strValue);
					break;
				case PP_ST:
					xData.eStatus = GetStatus(strValue);
					break;
				case PP_DT:
					xData.strDefectType = strValue;
					break;
				case PP_CN:
					xData.strEmpID = strValue;
					break;
				default:
					ASSERT(FALSE);
					break;
				}
			}
		}
	}
	xData.xTime = CTime::GetTickCount();
	return wAllField == wField;
}
CString CEMCParser::MakeString(EMC_CCL_DATA &xResult)
{
	CString strRtn;
	CString strLT;
	CString strTemp;
	if (xResult.eStatus == EMS_EXCEPT || xResult.eStatus == EMS_CLOSED){
		strTemp.Format(L"|%s%s|%s%s|%s%s|%s%s|%s%s|%s%d"
			, GetField(CCL_SC, CCL), xResult.strStation
			, GetField(CCL_NO, CCL), xResult.strMissionID
			, GetField(CCL_SO, CCL), xResult.strBatchName
			, GetField(CCL_PN, CCL), xResult.strMaterial
			, GetField(CCL_LT, CCL), xResult.strSerial
			, GetField(CCL_SQ, CCL), xResult.nIndex
			);
		strRtn += strTemp;

		if (xResult.eStatus == EMS_EXCEPT){
			strTemp.Format(L"|%s%d|%s%s|%s%s|%s%s"
				, GetField(CCL_BK, CCL), xResult.nBookNum
				, GetField(CCL_QR, CCL), xResult.strSheet
				, GetField(CCL_DT, CCL), xResult.strDefectType
				, GetField(CCL_ST, CCL), GetStatus(xResult.eStatus)
				);
			strRtn += strTemp;
		}
		else if (xResult.eStatus == EMS_CLOSED){
			strTemp.Format(L"|%s%s"
				, GetField(CCL_ST, CCL), GetStatus(xResult.eStatus)
				);
			strRtn += strTemp;
		}

		strRtn += L"|";
	}
	return strRtn;
}
CString CEMCParser::MakeString(EMC_PP_DATA &xData)
{
	CString strRtn;
	CString strLT;
	CString strTemp;
	if (xData.eStatus == EMS_EXCEPT || xData.eStatus == EMS_CLOSED){
		strTemp.Format(L"|%s%s|%s%s|%s%s|%s%s"
			, GetField(PP_SC, PP), xData.strStation
			, GetField(PP_NO, PP), xData.strMissionID
			, GetField(PP_SO, PP), xData.strBatchName
			, GetField(PP_PN, PP), xData.strMaterial
			);
		strRtn += strTemp;
		for (auto &i : xData.vSerial){
			strLT.Format(L"|%s%s", GetField(PP_LT, PP), i);
			strRtn += strLT;
		}

		if (xData.eStatus == EMS_EXCEPT){
			strTemp.Format(L"|%s%.1f|%s%.1f|%s%s|%s%s"
				, GetField(PP_QB, PP), xData.fDefectBegin
				, GetField(PP_QE, PP), xData.fDefectEnd
				, GetField(PP_DT, PP), xData.strDefectType
				, GetField(PP_ST, PP), GetStatus(xData.eStatus)
				);
			strRtn += strTemp;
		}
		else if (xData.eStatus == EMS_CLOSED){
			strTemp.Format(L"|%s%.1f|%s%s"
				, GetField(PP_QT, PP), xData.fLength
				, GetField(PP_ST, PP), GetStatus(xData.eStatus)
				);
			strRtn += strTemp;
		}

		strRtn += L"|";
	}
	return strRtn;
}
BOOL CEMCParser::GetField(CString strField, PRODUCT_TYPE eType, int &nField)
{
	if (eType == CCL){
		for (const EMC_FIELD &i : ctEMC_CCL_FIELD){
			if (strField == i.strField){
				nField = i.nFieldType;
				return TRUE;
			}
		}
	}
	else if (eType == PP){
		for (const EMC_FIELD &i : ctEMC_PP_FIELD){
			if (strField == i.strField){
				nField = i.nFieldType;
				return TRUE;
			}
		}
	}
	return FALSE;
}
CString CEMCParser::GetField(int nField, PRODUCT_TYPE eType)
{
	if (eType == CCL){
		for (auto &i : ctEMC_CCL_FIELD){
			if (i.nFieldType == nField)
				return i.strField;
		}
	}
	else if (eType == PP){
		for (auto &i : ctEMC_PP_FIELD){
			if (i.nFieldType == nField)
				return i.strField;
		}
	}
	return L"";
}
EMC_MISSION_STATUS CEMCParser::GetStatus(CString strStatus)
{
	if (strStatus == L"CLEAR")
		return EMS_CLEAR;
	else if (strStatus == L"START")
		return EMS_START;
	else if (strStatus == L"CLOSED")
		return EMS_CLOSED;
	else if (strStatus == L"DELETE")
		return EMS_DELETE;

	return EMS_NONE;
}
CString CEMCParser::GetStatus(EMC_MISSION_STATUS eStatus)
{
	switch (eStatus)
	{
	case EMS_CLEAR:
		return L"CLEAR";
		break;
	case EMS_START:
		return L"START";
		break;
	case EMS_CLOSED:
		return L"CLOSED";
		break;
	case EMS_DELETE:
		return L"DELETE";
		break;
	case EMS_EXCEPT:
		return L"EXCEPT";
		break;
	default:
		break;
	}
	return L"";
}
//1. 欄位全到+狀態正確, 取該工單長度; 2. 欄位重複(包含兩筆工單), 取第一筆工單長度
int CEMCParser::CheckPPData(CString strData)
{
	int nLength = 0;

	//ACK_SUCCESS
	int nAckLength = CString(ACK_SUCCESS).GetLength();
	if (strData.Mid(0, nAckLength) == ACK_SUCCESS){
		nLength = nAckLength;
		return nLength;
	}

	BOOL bStatusReady = FALSE; //狀態需符合START OR DELETE
	//EMC Param
	int nTokenPos = 0;
	CString strToken = strData.Tokenize(CString(EMC_FIELD_SPLITER), nTokenPos);
	BOOL bForceSend = FALSE;//PP一次只會下發一組工單, 只要出現第二筆工單時, 不管欄位是否到齊都要送出(Parse時會檢查欄位完整性)
	WORD wAllField = 0;
	WORD wField = 0;
	for (auto &i : ctEMC_PP_FIELD){
		if (i.bCheckExist) wAllField |= 1 << i.nFieldType;
	}
	while (!strToken.IsEmpty()){
		if (strToken.GetLength() > EMC_FIELD_LENGTH){
			int nField = 0;

			BOOL bFieldOK = GetField(strToken.Mid(0, EMC_FIELD_LENGTH), PRODUCT_TYPE::PP, nField);

			if (bFieldOK){
				EMC_PP_FIELD_TYPE eField = (EMC_PP_FIELD_TYPE)nField;
				//有重複欄位, 則視為下一筆工單開始
				if (wField & (1 << eField) && eField != PP_LT/*批號會重複*/){
					bForceSend = TRUE;
					break;
				}
				wField |= (1 << eField);
				//判斷狀態
				if (eField == PP_ST){
					CString strValue = strToken.Mid(EMC_FIELD_LENGTH, strToken.GetLength() - EMC_FIELD_LENGTH);
					EMC_MISSION_STATUS eStatus = GetStatus(strValue);
					if (EMC_MISSION_STATUS::EMS_START == eStatus || EMC_MISSION_STATUS::EMS_DELETE == eStatus)
						bStatusReady = TRUE;
				}
			}
		}
		nLength = nTokenPos - 1;
		strToken = strData.Tokenize(CString(EMC_FIELD_SPLITER), nTokenPos);
	}
	if (strData.Right(1) == EMC_FIELD_SPLITER) nLength++;//end with EMC_FIELD_SPLITER

	if (!bForceSend){ //非強制送出, 需等狀態和欄位到齊才送出
		if (!bStatusReady || wAllField != wField)
			nLength = 0;
	}

	return nLength;
}
//1. 欄位全到+狀態正確(最後一筆工單), 會將正確長度buffer取出 2.最後一筆工單到了(狀態START), 又收到下一筆工單時, 取到最後一筆工單長度
int CEMCParser::CheckCCLData(CString strData)
{
	int nLength = 0;

	//ACK_SUCCESS
	int nAckLength = CString(ACK_SUCCESS).GetLength();
	if (strData.Mid(0, nAckLength) == ACK_SUCCESS){
		nLength = nAckLength;
		return nLength;
	}

	BOOL bStatusReady = FALSE; //狀態需符合START
	//EMC Param
	int nTokenPos = 0;
	CString strToken = strData.Tokenize(CString(EMC_FIELD_SPLITER), nTokenPos);
	BOOL bForceSend = FALSE;//最後一筆工單到了(狀態START), 又收到下一筆工單時, 不管欄位是否到齊都要送出(Parse時會檢查欄位完整性)
	WORD wAllField = 0;
	WORD wField = 0;
	for (auto &i : ctEMC_CCL_FIELD){
		if (i.bCheckExist) wAllField |= 1 << i.nFieldType;
	}
	while (!strToken.IsEmpty()){
		if (strToken.GetLength() > EMC_FIELD_LENGTH){
			int nField = 0;
			BOOL bFieldOK = GetField(strToken.Mid(0, EMC_FIELD_LENGTH), PRODUCT_TYPE::CCL, nField);

			if (bFieldOK){
				EMC_CCL_FIELD_TYPE eField = (EMC_CCL_FIELD_TYPE)nField;

				//判斷狀態
				if (eField == CCL_ST){
					CString strValue = strToken.Mid(EMC_FIELD_LENGTH, strToken.GetLength() - EMC_FIELD_LENGTH);
					EMC_MISSION_STATUS eStatus = GetStatus(strValue);
					if (EMC_MISSION_STATUS::EMS_START == eStatus)
						bStatusReady = TRUE;
				}

				//有重複欄位, 則視為下一筆工單開始
				if (wField & (1 << eField) && eField != CCL_E1/*少組訊息會重複*/){
					if (bStatusReady){//最後一筆工單到了(狀態START), 又收到下一筆工單時, 要強制送出
						bForceSend = TRUE;
						break;
					}
					wField = 0;
				}
				wField |= (1 << eField);
				
				nLength = nTokenPos - 1;
			}
		}
		strToken = strData.Tokenize(CString(EMC_FIELD_SPLITER), nTokenPos);
	}
	if (strData.Right(1) == EMC_FIELD_SPLITER) nLength++;//end with EMC_FIELD_SPLITER

	if (!bForceSend){ //非強制送出, 需等狀態和欄位到齊才送出
		if (!bStatusReady || wAllField > wField)
			nLength = 0;
	}

	return nLength;
}