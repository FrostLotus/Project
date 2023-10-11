
// EMCCommunicatorDlg.cpp : 實作檔
//

#include "stdafx.h"
#include "EMCCommunicator.h"
#include "EMCCommunicatorDlg.h"
#include "afxdialogex.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif
#include "AoiFont.h"

// CEMCCommunicatorDlg 對話方塊

static UINT ShowMsgThread(LPVOID pParam)
{
	CString* pMsg = (CString*)pParam;
	AfxMessageBox(*pMsg);
	delete pMsg;
	TRACE(L"ShowMsgThread end \n");
	return 0;
};
CEMCCommunicatorDlg::CEMCCommunicatorDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CEMCCommunicatorDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}
CEMCCommunicatorDlg::~CEMCCommunicatorDlg()
{
	Finalize();
}
void CEMCCommunicatorDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CEMCCommunicatorDlg, CDialogEx)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_CLIENT, OnLvnGetdispinfoClient)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_PARAM, OnLvnGetdispinfoParam)
	ON_NOTIFY(LVN_GETDISPINFO, UI_LC_RESULT, OnLvnGetdispinfoResult)
	ON_MESSAGE(WM_GPIO_MSG, OnCmdGPIO)
#ifdef EMC_SIMLULATE
	ON_CONTROL_RANGE(BN_CLICKED, UI_BTN_BEGIN, UI_BTN_END, OnSimulate)
#endif
END_MESSAGE_MAP()


// CEMCCommunicatorDlg 訊息處理常式

BOOL CEMCCommunicatorDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 設定此對話方塊的圖示。當應用程式的主視窗不是對話方塊時，
	// 框架會自動從事此作業
	SetIcon(m_hIcon, TRUE);			// 設定大圖示
	SetIcon(m_hIcon, FALSE);		// 設定小圖示

	// TODO:  在此加入額外的初始設定
	Init();
	InitUiRectPos();
	InitUI();
#ifdef EMC_SIMLULATE
	InitSimulate();
#endif
	theApp.InsertDebugLog(L"OnInitDialog Finish", LOG_EMCSYSTEM);
	return TRUE;  // 傳回 TRUE，除非您對控制項設定焦點
}

// 如果將最小化按鈕加入您的對話方塊，您需要下列的程式碼，
// 以便繪製圖示。對於使用文件/檢視模式的 MFC 應用程式，
// 框架會自動完成此作業。

void CEMCCommunicatorDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 繪製的裝置內容

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// 將圖示置中於用戶端矩形
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// 描繪圖示
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
		DrawInfo();
	}
}

// 當使用者拖曳最小化視窗時，
// 系統呼叫這個功能取得游標顯示。
HCURSOR CEMCCommunicatorDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}
void CEMCCommunicatorDlg::OnLvnGetdispinfoClient(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast<NMLVDISPINFO*>(pNMHDR);

	*pResult = NULL;

	CString strIp, strText;
	UINT nPort;
	BOOL bShow = FALSE;
	if (m_pServer)
	{
		bShow = m_pServer->GetClientInfo(pDispInfo->item.iItem, strIp, nPort);
	}
	if (LVIF_TEXT & pDispInfo->item.mask)
	{
		if (pDispInfo->item.iItem == EOF || pDispInfo->item.iItem >= MAX_CLIENT || !bShow) return;
		switch (pDispInfo->item.iSubItem)
		{
			case LIST_COL_IP:
				wcscpy_s(pDispInfo->item.pszText, strIp.GetLength() + 1, strIp);
				break;
			case LIST_COL_PORT:
				strText.Format(L"%d", nPort);
				wcscpy_s(pDispInfo->item.pszText, strText.GetLength() + 1, strText);
				break;
		}
	}
}
int CEMCCommunicatorDlg::GetIndex(UI_TYPE eType, int nUiIndex, int nMax, EMC_FIELD* pField)
{
	int nCount = -1;
	for (int i = 0; i < nMax; i++)
	{
		if (eType & (pField + i)->wUiType)
		{
			nCount++;
		}

		if (nCount == nUiIndex)
			return i;
	}


	return -1;
}
void CEMCCommunicatorDlg::OnLvnGetdispinfoParam(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast<NMLVDISPINFO*>(pNMHDR);

	*pResult = NULL;
	CString strField, strValue, strTemp;
	if (LVIF_TEXT & pDispInfo->item.mask)
	{
		if (pDispInfo->item.iItem == EOF) return;

		switch (pDispInfo->item.iSubItem)
		{
			case LIST_COL_FIELDNAME:
				if (m_eType == PRODUCT_TYPE::CCL && pDispInfo->item.iItem < EMC_CCL_FIELD_TYPE::CCL_MAX)
				{
					int nIndex = GetIndex(UI_TYPE::UT_PARAM, pDispInfo->item.iItem, EMC_CCL_FIELD_TYPE::CCL_MAX, (EMC_FIELD*)&ctEMC_CCL_FIELD);
					if (nIndex >= 0)
					{
						strField = ctEMC_CCL_FIELD[nIndex].strFieldName;
					}
				}
				else if (m_eType == PRODUCT_TYPE::PP && pDispInfo->item.iItem < EMC_PP_FIELD_TYPE::PP_MAX)
				{
					int nIndex = GetIndex(UI_TYPE::UT_PARAM, pDispInfo->item.iItem, EMC_PP_FIELD_TYPE::PP_MAX, (EMC_FIELD*)&ctEMC_PP_FIELD);
					if (nIndex >= 0)
					{
						strField = ctEMC_PP_FIELD[nIndex].strFieldName;
					}
				}
				wcscpy_s(pDispInfo->item.pszText, strField.GetLength() + 1, strField);
				break;
			case LIST_COL_VALUE1:
			case LIST_COL_VALUE2:
			case LIST_COL_VALUE3:
				if (m_eType == PRODUCT_TYPE::CCL && pDispInfo->item.iSubItem <= m_vCCLParam.size())
				{
					int nIndex = GetIndex(UI_TYPE::UT_PARAM, pDispInfo->item.iItem, EMC_CCL_FIELD_TYPE::CCL_MAX, (EMC_FIELD*)&ctEMC_CCL_FIELD);
					if (nIndex >= 0)
						strValue = GetCCLValue((EMC_CCL_FIELD_TYPE)nIndex, m_vCCLParam.at(pDispInfo->item.iSubItem - 1));
				}
				else if (m_eType == PRODUCT_TYPE::PP && pDispInfo->item.iItem < EMC_PP_FIELD_TYPE::PP_MAX)
				{
					if (pDispInfo->item.iSubItem == LIST_COL_VALUE2 || pDispInfo->item.iSubItem == LIST_COL_VALUE3)
						return;
					int nIndex = GetIndex(UI_TYPE::UT_PARAM, pDispInfo->item.iItem, EMC_PP_FIELD_TYPE::PP_MAX, (EMC_FIELD*)&ctEMC_PP_FIELD);
					if (nIndex >= 0)
						strValue = GetPPValue((EMC_PP_FIELD_TYPE)nIndex, m_xPPParam);
				}
				wcscpy_s(pDispInfo->item.pszText, strValue.GetLength() + 1, strValue);
				break;
		}
	}
}
void CEMCCommunicatorDlg::OnLvnGetdispinfoResult(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMLVDISPINFO* pDispInfo = reinterpret_cast<NMLVDISPINFO*>(pNMHDR);

	*pResult = NULL;
	CString strField, strValue, strTemp;
	if (LVIF_TEXT & pDispInfo->item.mask)
	{
		if (pDispInfo->item.iItem == EOF) return;

		if (m_eType == PRODUCT_TYPE::CCL)
		{
			int nIndex = GetIndex(UI_TYPE::UT_RESULT, pDispInfo->item.iItem, EMC_CCL_FIELD_TYPE::CCL_MAX, (EMC_FIELD*)&ctEMC_CCL_FIELD);
			if (nIndex >= 0 && nIndex < EMC_CCL_FIELD_TYPE::CCL_MAX)
			{
				strField = ctEMC_CCL_FIELD[nIndex].strFieldName;
				strValue = GetCCLValue((EMC_CCL_FIELD_TYPE)nIndex, m_xCCLResult);
			}
		}
		else if (m_eType == PRODUCT_TYPE::PP)
		{
			int nIndex = GetIndex(UI_TYPE::UT_RESULT, pDispInfo->item.iItem, EMC_PP_FIELD_TYPE::PP_MAX, (EMC_FIELD*)&ctEMC_PP_FIELD);

			if (nIndex >= 0 && nIndex < EMC_PP_FIELD_TYPE::PP_MAX)
			{
				strValue = GetPPValue((EMC_PP_FIELD_TYPE)nIndex, m_xPPResult);
				strField = ctEMC_PP_FIELD[nIndex].strFieldName;
			}
		}
		switch (pDispInfo->item.iSubItem)
		{
			case LIST_COL_FIELDNAME:
				wcscpy_s(pDispInfo->item.pszText, strField.GetLength() + 1, strField);
				break;
			case LIST_COL_VALUE1:
				wcscpy_s(pDispInfo->item.pszText, strValue.GetLength() + 1, strValue);
				break;
		}
	}
}
LRESULT CEMCCommunicatorDlg::OnCmdGPIO(WPARAM wParam, LPARAM lParam)
{
#ifdef EMC_SIMLULATE
	((IEMCNotify*)this)->Attach(m_pEMCDataHandler);
#endif
	switch (wParam)
	{
		case WM_AOI_RESPONSE_CMD:
			HandleAOIResponse(lParam);
			break;
		case WM_EMC_RESULTCCL_CMD:
		{
			BATCH_SHARE_EMC_CCLRESULT xResult;
			memset(&xResult, 0, sizeof(BATCH_SHARE_EMC_CCLRESULT));

			if (OnEMCResult(&xResult))
			{
				CEMCParser::InitCCLData(m_xCCLResult);
				m_xCCLResult.strStation = xResult.cStation;
				m_xCCLResult.strMissionID = xResult.cMissionID;
				m_xCCLResult.strBatchName = xResult.cBatchName;
				m_xCCLResult.strMaterial = xResult.cMaterial;
				m_xCCLResult.strSerial = xResult.cSerial;
				m_xCCLResult.nIndex = xResult.nIndex;
				m_xCCLResult.nBookNum = xResult.nBookNum;
				m_xCCLResult.strSheet = xResult.cSheet;
				m_xCCLResult.strDefectType = xResult.cDefectType;
				m_xCCLResult.eStatus = EMC_MISSION_STATUS::EMS_EXCEPT;
				m_xCCLResult.xTime = CTime::GetTickCount();
				if (m_pClientMgr)
				{
					m_pClientMgr->SendToEMC(m_xCCLResult);//send to emc
					InvalidateRect(&m_xUi[UI_LC_RESULT].rcUi);
				}
			}
		}
		break;
		case WM_EMC_ENDCCL_CMD:
		{
			BATCH_SHARE_EMC_CCLEND xResult;
			memset(&xResult, 0, sizeof(BATCH_SHARE_EMC_CCLEND));
			if (OnEMCBatchEnd(&xResult))
			{

				CEMCParser::InitCCLData(m_xCCLResult);
				m_xCCLResult.strStation = xResult.cStation;
				m_xCCLResult.strMissionID = xResult.cMissionID;
				m_xCCLResult.strBatchName = xResult.cBatchName;
				m_xCCLResult.strMaterial = xResult.cMaterial;
				m_xCCLResult.strSerial = xResult.cSerial;
				m_xCCLResult.nIndex = xResult.nIndex;
				m_xCCLResult.eStatus = EMC_MISSION_STATUS::EMS_CLOSED;
				m_xCCLResult.xTime = CTime::GetTickCount();

				if (m_pClientMgr)
				{
					m_pClientMgr->SendToEMC(m_xCCLResult);//send to emc
					InvalidateRect(&m_xUi[UI_LC_RESULT].rcUi);
				}
			}
		}
		break;
		case WM_EMC_RESULTPP_CMD:
		{
			BATCH_SHARE_EMC_PPRESULT xResult;
			memset(&xResult, 0, sizeof(BATCH_SHARE_EMC_PPRESULT));

			if (OnEMCResult(&xResult))
			{

				CEMCParser::InitPPData(m_xPPResult);
				m_xPPResult.strStation = xResult.cStation;
				m_xPPResult.strMissionID = xResult.cMissionID;
				m_xPPResult.strBatchName = xResult.cBatchName;
				m_xPPResult.strMaterial = xResult.cMaterial;
				m_xPPResult.vSerial.push_back(xResult.cSerial);
				m_xPPResult.fDefectBegin = xResult.fDefectBegin;
				m_xPPResult.fDefectEnd = xResult.fDefectEnd;
				m_xPPResult.strDefectType = xResult.cDefectType;

				m_xPPResult.eStatus = EMC_MISSION_STATUS::EMS_EXCEPT;
				m_xPPResult.xTime = CTime::GetTickCount();

				if (m_pClientMgr)
				{
					m_pClientMgr->SendToEMC(m_xPPResult);//send to emc
					InvalidateRect(&m_xUi[UI_LC_RESULT].rcUi);
				}
			}
		}
		break;
		case WM_EMC_ENDPP_CMD:
		{
			BATCH_SHARE_EMC_PPEND xResult;
			memset(&xResult, 0, sizeof(BATCH_SHARE_EMC_PPEND));

			if (OnEMCBatchEnd(&xResult))
			{

				CEMCParser::InitPPData(m_xPPResult);
				m_xPPResult.strStation = xResult.cStation;
				m_xPPResult.strMissionID = xResult.cMissionID;
				m_xPPResult.strBatchName = xResult.cBatchName;
				m_xPPResult.strMaterial = xResult.cMaterial;
				m_xPPResult.vSerial.push_back(xResult.cSerial);
				m_xPPResult.eStatus = EMC_MISSION_STATUS::EMS_CLOSED;
				m_xPPResult.xTime = CTime::GetTickCount();
				m_xPPResult.fLength = xResult.fLength;

				if (m_pClientMgr)
				{
					m_pClientMgr->SendToEMC(m_xPPResult);//send to emc
					InvalidateRect(&m_xUi[UI_LC_RESULT].rcUi);
				}
			}
		}
		break;

	}
	return 0;
}
#ifdef EMC_SIMLULATE
void CEMCCommunicatorDlg::OnSimulate(UINT uid)
{
#ifdef EMC_SIMLULATE
	if (m_pSimulateDataHandler)
	{
		delete m_pSimulateDataHandler; //同process讀寫第二次會有問題, 模擬時先這樣解
		m_pSimulateDataHandler = new CEMCDataHandler(BATCH_AOI2EMC_MEM_ID);
	}
	((IEMCNotify*)this)->Attach(m_pSimulateDataHandler);
#endif
	theApp.InsertDebugLog(L"Send Simulate Data!", LOG_EMCSYSTEM);

	auto GetText = [&](TCHAR* pDst, int nSize, int nID)
	{
		CString strText;
		m_xUi[nID].pEdit->GetWindowText(strText);
		//wcscpy_s(pDst, nSize, strText);
		memcpy(pDst, strText.GetBuffer(), nSize * sizeof(TCHAR));
	};
	auto GetInt = [&](int& nData, int nID)
	{
		CString strText;
		m_xUi[nID].pEdit->GetWindowText(strText);
		nData = _ttoi(strText);
	};
	auto GetFloat = [&](float& fData, int nID)
	{
		CString strText;
		m_xUi[nID].pEdit->GetWindowText(strText);
		fData = _ttof(strText);
	};
	if (m_eType == CCL)
	{
		if (uid == UI_BTN_EXCEPT)
		{
			BATCH_SHARE_EMC_CCLRESULT xData;
			memset(&xData, 0, sizeof(xData));
			GetText(xData.cStation, sizeof(xData.cStation) / sizeof(TCHAR), UI_EDIT_STATION);
			GetText(xData.cMissionID, sizeof(xData.cMissionID) / sizeof(TCHAR), UI_EDIT_MISSION);
			GetText(xData.cBatchName, sizeof(xData.cBatchName) / sizeof(TCHAR), UI_EDIT_BATCHNAME);
			GetText(xData.cMaterial, sizeof(xData.cMaterial) / sizeof(TCHAR), UI_EDIT_MATERIAL);
			GetText(xData.cSerial, sizeof(xData.cSerial) / sizeof(TCHAR), UI_EDIT_SERIAL);

			GetInt(xData.nIndex, UI_EDIT_INDEX);
			GetInt(xData.nBookNum, UI_EDIT_BOOKNUM);
			GetText(xData.cSheet, sizeof(xData.cSheet) / sizeof(TCHAR), UI_EDIT_SHEETNUM);
			GetText(xData.cDefectType, sizeof(xData.cDefectType) / sizeof(TCHAR), UI_EDIT_DEFECTTYPE);

			//OnEMCResult(&xData);
			OnSimulateData((BYTE*)&xData, sizeof(BATCH_SHARE_EMC_CCLRESULT), WM_EMC_RESULTCCL_CMD, 0);
		}
		else
		{
			BATCH_SHARE_EMC_CCLEND xData;
			memset(&xData, 0, sizeof(xData));
			GetText(xData.cStation, sizeof(xData.cStation) / sizeof(TCHAR), UI_EDIT_STATION);
			GetText(xData.cMissionID, sizeof(xData.cMissionID) / sizeof(TCHAR), UI_EDIT_MISSION);
			GetText(xData.cBatchName, sizeof(xData.cBatchName) / sizeof(TCHAR), UI_EDIT_BATCHNAME);
			GetText(xData.cMaterial, sizeof(xData.cMaterial) / sizeof(TCHAR), UI_EDIT_MATERIAL);
			GetText(xData.cSerial, sizeof(xData.cSerial) / sizeof(TCHAR), UI_EDIT_SERIAL);
			GetInt(xData.nIndex, UI_EDIT_INDEX);
			//OnEMCBatchEnd(&xData);
			OnSimulateData((BYTE*)&xData, sizeof(BATCH_SHARE_EMC_CCLEND), WM_EMC_ENDCCL_CMD, 0);
		}
	}
	else if (m_eType == PP)
	{
		if (uid == UI_BTN_EXCEPT)
		{
			BATCH_SHARE_EMC_PPRESULT xData;
			memset(&xData, 0, sizeof(xData));
			GetText(xData.cStation, sizeof(xData.cStation) / sizeof(TCHAR), UI_EDIT_STATION);
			GetText(xData.cMissionID, sizeof(xData.cMissionID) / sizeof(TCHAR), UI_EDIT_MISSION);
			GetText(xData.cBatchName, sizeof(xData.cBatchName) / sizeof(TCHAR), UI_EDIT_BATCHNAME);
			GetText(xData.cMaterial, sizeof(xData.cMaterial) / sizeof(TCHAR), UI_EDIT_MATERIAL);
			GetText(xData.cSerial, sizeof(xData.cSerial) / sizeof(TCHAR), UI_EDIT_SERIAL);

			GetText(xData.cDefectType, sizeof(xData.cDefectType) / sizeof(TCHAR), UI_EDIT_DEFECTTYPE);
			GetFloat(xData.fDefectBegin, UI_EDIT_DEFECTBEGIN);
			GetFloat(xData.fDefectEnd, UI_EDIT_DEFECTEND);

			//OnEMCResult(&xData);
			OnSimulateData((BYTE*)&xData, sizeof(BATCH_SHARE_EMC_PPRESULT), WM_EMC_RESULTPP_CMD, 0);
		}
		else
		{
			BATCH_SHARE_EMC_PPEND xData;
			memset(&xData, 0, sizeof(xData));
			GetText(xData.cStation, sizeof(xData.cStation) / sizeof(TCHAR), UI_EDIT_STATION);
			GetText(xData.cMissionID, sizeof(xData.cMissionID) / sizeof(TCHAR), UI_EDIT_MISSION);
			GetText(xData.cBatchName, sizeof(xData.cBatchName) / sizeof(TCHAR), UI_EDIT_BATCHNAME);
			GetText(xData.cMaterial, sizeof(xData.cMaterial) / sizeof(TCHAR), UI_EDIT_MATERIAL);
			GetText(xData.cSerial, sizeof(xData.cSerial) / sizeof(TCHAR), UI_EDIT_SERIAL);
			GetFloat(xData.fLength, UI_EDIT_LENGTH);
			//OnEMCBatchEnd(&xData);
			OnSimulateData((BYTE*)&xData, sizeof(BATCH_SHARE_EMC_PPEND), WM_EMC_ENDPP_CMD, 0);
		}
	}
}
#endif
void CEMCCommunicatorDlg::OnEMCParamSocketStatusChange(int nIndex)
{
	if (m_xUi[UI_LC_CLIENT].pList)
	{
		m_xUi[UI_LC_CLIENT].pList->RedrawItems(nIndex, nIndex);
	}
}
#ifdef EMC_SIMLULATE
UINT AckSuccessThread(LPVOID pParam)
{
	::Sleep(1000);
	EMCParamSocket* pData = (EMCParamSocket*)pParam;
	pData->SendAck(TRUE);
	TRACE(L"ack thread end \n");
	return 0;
}
#endif
void CEMCCommunicatorDlg::OnReceiveEMCParam(EMCParamSocket* pSrc, CString strData)
{
	if (pSrc)
	{
		if (m_eType == PRODUCT_TYPE::CCL)
		{
			BOOL bComplete = CEMCParser::ParseCCL(strData, m_vCCLParam);
			if (bComplete)
			{
				vector<BATCH_SHARE_EMC_CCLPARAM> vParam;
				for (auto& i : m_vCCLParam)
				{
					BATCH_SHARE_EMC_CCLPARAM xParam;
					memset(&xParam, 0, sizeof(BATCH_SHARE_EMC_CCLPARAM));
					wcscpy_s(xParam.cStation, i.strStation.GetBuffer());
					wcscpy_s(xParam.cMissionID, i.strMissionID.GetBuffer());
					wcscpy_s(xParam.cBatchName, i.strBatchName.GetBuffer());
					wcscpy_s(xParam.cMaterial, i.strMaterial.GetBuffer());
					xParam.nStatus = i.eStatus;
					wcscpy_s(xParam.cEmpID, i.strEmpID.GetBuffer());
					wcscpy_s(xParam.cSerial, i.strSerial.GetBuffer());
					xParam.nNum = i.nNum;
					xParam.nBookNum = i.nBookNum;
					xParam.nSheetNum = i.nSheetNum;
					xParam.nSplit = i.nSplit;
					CString strMiss, strTemp;
					for (auto& j : i.vMiss)
					{
						strTemp.Format(L"%d-%d|", j.first, j.second);
						if (strMiss.GetLength() + strTemp.GetLength() < _countof(xParam.cMiss) - 1)
							strMiss += strTemp;
					}
					wcscpy_s(xParam.cMiss, strMiss.GetBuffer());
					xParam.nBeginBook = i.nBeginBook;
					xParam.nEndBook = i.nEndBook;
					xParam.nBeginSheet = i.nBeginSheet;
					xParam.nEndSheet = i.nEndSheet;

					vParam.push_back(xParam);
				}
				OnPushEMCParam(vParam);
#ifndef EMC_SIMLULATE
				pSrc->SendAck(TRUE);
#endif
			}
			else
			{
				BATCH_SHARE_EMC_ERRORINFO xInfo;
				memset(&xInfo, 0, sizeof(xInfo));
				CString strTemp;
				if (strData.GetLength() >= _countof(xInfo.cErrorMsg) - 1)
					strTemp = strData.Mid(0, _countof(xInfo.cErrorMsg) - 1);
				else
					strTemp = strData;
				wcscpy_s(xInfo.cErrorMsg, strTemp.GetBuffer());
				xInfo.eErrorType = EMC_FIELD_NOTCOMPLETE;
				OnEMCErrorMsg(&xInfo);
				CString strLog;
				strLog.Format(L"not complete %s", strData);
				theApp.InsertDebugLog(strLog, LOG_EMCSYSTEM);
#ifdef EMC_SIMLULATE
				CString* pMsg = new CString(strLog);
				AfxBeginThread(ShowMsgThread, (LPVOID)pMsg, THREAD_PRIORITY_NORMAL);
#endif
			}
		}
		else if (m_eType == PRODUCT_TYPE::PP)
		{
			BOOL bComplete = CEMCParser::ParsePP(strData, m_xPPParam);
			if (m_pEMCDataHandler)
			{
				if (bComplete)
				{
					BATCH_SHARE_EMC_PPPARAM xParam;
					memset(&xParam, 0, sizeof(BATCH_SHARE_EMC_PPPARAM));
					wcscpy_s(xParam.cStation, m_xPPParam.strStation.GetBuffer());
					wcscpy_s(xParam.cMissionID, m_xPPParam.strMissionID.GetBuffer());
					wcscpy_s(xParam.cBatchName, m_xPPParam.strBatchName.GetBuffer());
					wcscpy_s(xParam.cMaterial, m_xPPParam.strMaterial.GetBuffer());
					xParam.nStatus = m_xPPParam.eStatus == EMS_START ? 0 : 1;
					wcscpy_s(xParam.cEmpID, m_xPPParam.strEmpID.GetBuffer());
					CString strSerial, strTemp;
					for (auto& i : m_xPPParam.vSerial)
					{
						strTemp.Format(L"%s", i);
						if (strSerial.GetLength() == 0)
							strSerial = strTemp;
						else if (strSerial.GetLength() + strTemp.GetLength() < _countof(xParam.cSerial))
							strSerial += L"|" + strTemp;
					}
					wcscpy_s(xParam.cSerial, strSerial.GetBuffer());
					OnPushEMCParam(&xParam);
#ifndef EMC_SIMLULATE
					pSrc->SendAck(TRUE);
#endif
				}
				else
				{
					BATCH_SHARE_EMC_ERRORINFO xInfo;
					memset(&xInfo, 0, sizeof(xInfo));
					CString strTemp;
					if (strData.GetLength() >= _countof(xInfo.cErrorMsg) - 1)
						strTemp = strData.Mid(0, _countof(xInfo.cErrorMsg) - 1);
					else
						strTemp = strData;
					wcscpy_s(xInfo.cErrorMsg, strTemp.GetBuffer());
					xInfo.eErrorType = EMC_FIELD_NOTCOMPLETE;
					OnEMCErrorMsg(&xInfo);
					CString strLog;
					strLog.Format(L"not complete %s", strData);
					theApp.InsertDebugLog(strLog, LOG_EMCSYSTEM);
#ifdef EMC_SIMLULATE
					CString* pMsg = new CString(strLog);
					AfxBeginThread(ShowMsgThread, (LPVOID)pMsg, THREAD_PRIORITY_NORMAL);
#endif
				}
			}
		}
		//update UI
		if (m_xUi[UI_LC_PARAM].pList)
		{
			m_xUi[UI_LC_PARAM].pList->Invalidate();
		}

#ifdef EMC_SIMLULATE //wait 1 second and send response
		m_pAckSuccessThread = AfxBeginThread(AckSuccessThread, pSrc);
#else
		//AfxMessageBox(L"not implement ACK_SUCCESS yet!"); //not implement warning, delete after implement
#endif
	}
}
void CEMCCommunicatorDlg::OnEMCParamTimeout(EMCParamSocket* pSrc, CString strData)
{
	CString strLog;
	strLog.Format(L"Time out %s", strData);
	theApp.InsertDebugLog(strLog, LOG_EMCSYSTEM);
#ifdef EMC_SIMLULATE
	AfxMessageBox(strLog);
#endif
	BATCH_SHARE_EMC_ERRORINFO xInfo;
	memset(&xInfo, 0, sizeof(xInfo));
	CString strTemp;
	if (strData.GetLength() >= _countof(xInfo.cErrorMsg) - 1)
		strTemp = strData.Mid(0, _countof(xInfo.cErrorMsg) - 1);
	else
		strTemp = strData;
	wcscpy_s(xInfo.cErrorMsg, strTemp.GetBuffer());
	xInfo.eErrorType = EMC_TIMEOUT;
	OnEMCErrorMsg(&xInfo);
}
void CEMCCommunicatorDlg::OnEMCResultSocketStatusChange(AOI_SOCKET_STATE eStatus)
{
	if (m_hWnd)
		InvalidateRect(&m_xUi[UI_LABEL_INFO2].rcUi);
}
void CEMCCommunicatorDlg::OnReceiveEMCResult(BOOL bAckSuccess)
{

}
void CEMCCommunicatorDlg::Init()
{
#ifdef EMC_SIMLULATE
	m_pSimulateDataHandler = new CEMCDataHandler(BATCH_AOI2EMC_MEM_ID);

	m_pEMCDataHandler = new CEMCDataHandler(BATCH_EMC2AOI_MEM_ID);
#else
	m_pEMCDataHandler = new CEMCDataHandler;
	((IEMCNotify*)this)->Attach(m_pEMCDataHandler);
#endif
	m_pServer = NULL;
	m_pClientMgr = NULL;
	m_eType = PRODUCT_TYPE::CCL;
	CenterWindow();
}

void CEMCCommunicatorDlg::InitUiRectPos()
{
	for (int i = UI_ITEM_BEGIN; i < UI_ITEM_END; i++)
	{
		POINT ptBase = { 0, 0 };
		POINT ptSize = { 0, 0 };
		CString strCaption;
		switch (i)
		{
			case UI_LABEL_INFO1:
				ptBase = { 10, 10 };
				ptSize = { 150, 40 };
				break;
			case UI_LABEL_INFO2:
				ptBase = { 300, 10 };
				ptSize = { 180, 40 };
				break;
			case UI_LC_CLIENT:
				ptBase = { 10, 50 };
				ptSize = { 250, 90 };
				break;
			case UI_LC_PARAM:
				ptBase = { 10, 150 };
				ptSize = { 500, 380 };
				break;
			case UI_LC_RESULT:
				ptBase = { 10, 550 };
				ptSize = { 240, 280 };
				break;
#ifdef EMC_SIMLULATE
			case UI_BTN_EXCEPT:
				ptBase = { 435, 590 };
				ptSize = { 90, 30 };
				strCaption = L"SendExcept";
				break;
			case UI_BTN_CLOSE:
				ptBase = { 435, 630 };
				ptSize = { 90, 30 };
				strCaption = L"SendClose";
				break;
			case UI_LABEL_STATION:
				ptBase = { 260, 590 };
				ptSize = { 80, 20 };
				strCaption = L"設備號";
				break;
			case UI_LABEL_MISSION:
				ptBase = { 260, 620 };
				ptSize = { 80, 30 };
				strCaption = L"任務號";
				break;
			case UI_LABEL_BATCHNAME:
				ptBase = { 260, 650 };
				ptSize = { 80, 30 };
				strCaption = L"工單號";
				break;
			case UI_LABEL_MATERIAL:
				ptBase = { 260, 680 };
				ptSize = { 80, 30 };
				strCaption = L"料號";
				break;
			case UI_LABEL_SERIAL:
				ptBase = { 260, 710 };
				ptSize = { 80, 30 };
				strCaption = L"批號";
				break;
			case UI_LABEL_DEFECTTYPE:
				ptBase = { 260, 740 };
				ptSize = { 80, 30 };
				strCaption = L"缺點代碼";
				break;
			case UI_LABEL_INDEX:
				ptBase = { 260, 770 };
				ptSize = { 80, 30 };
				strCaption = L"檢測張數序號";
				break;
			case UI_LABEL_DEFECTBEGIN:
				ptBase = { 260, 770 };
				ptSize = { 80, 30 };
				strCaption = L"缺點開始米數";
				break;
			case UI_LABEL_BOOKNUM:
				ptBase = { 260, 800 };
				ptSize = { 80, 30 };
				strCaption = L"BOOK數";
				break;
			case UI_LABEL_DEFECTEND:
				ptBase = { 260, 800 };
				ptSize = { 80, 30 };
				strCaption = L"缺點結束米數";
				break;
			case UI_LABEL_SHEETNUM:
				ptBase = { 260, 830 };
				ptSize = { 80, 30 };
				strCaption = L"第幾Sheet";
				break;
			case UI_LABEL_LENGTH:
				ptBase = { 260, 830 };
				ptSize = { 80, 30 };
				strCaption = L"每卷米數";
				break;
			case UI_EDIT_STATION:
				ptBase = { 350, 590 };
				ptSize = { 60, 25 };
				break;
			case UI_EDIT_MISSION:
				ptBase = { 350, 620 };
				ptSize = { 60, 25 };
				break;
			case UI_EDIT_BATCHNAME:
				ptBase = { 350, 650 };
				ptSize = { 60, 25 };
				break;
			case UI_EDIT_MATERIAL:
				ptBase = { 350, 680 };
				ptSize = { 60, 25 };
				break;
			case UI_EDIT_SERIAL:
				ptBase = { 350, 710 };
				ptSize = { 60, 25 };
				break;
			case UI_EDIT_DEFECTTYPE:
				ptBase = { 350, 740 };
				ptSize = { 60, 25 };
				break;
			case UI_EDIT_INDEX:
				ptBase = { 350, 770 };
				ptSize = { 60, 25 };
				break;
			case UI_EDIT_DEFECTBEGIN:
				ptBase = { 350, 770 };
				ptSize = { 60, 25 };
				break;
			case UI_EDIT_BOOKNUM:
				ptBase = { 350, 800 };
				ptSize = { 60, 25 };
				break;
			case UI_EDIT_DEFECTEND:
				ptBase = { 350, 800 };
				ptSize = { 60, 25 };
				break;
			case UI_EDIT_SHEETNUM:
				ptBase = { 350, 830 };
				ptSize = { 60, 25 };
				break;
			case UI_EDIT_LENGTH:
				ptBase = { 350, 830 };
				ptSize = { 60, 25 };
				break;
			case UI_GB_SIMULATE:
				ptBase = { 250, 550 };
				ptSize = { 280, 330 };
				strCaption = L"SIMULATE";
				break;
#endif
		}
		m_xUi[i].rcUi = { ptBase.x, ptBase.y, ptBase.x + ptSize.x, ptBase.y + ptSize.y };
		m_xUi[i].nID = i;
		m_xUi[i].strCaption = strCaption;
	}
}
void CEMCCommunicatorDlg::InitUI()
{
#ifdef EMC_SIMLULATE
	for (int i = UI_BTN_BEGIN; i < UI_BTN_END; i++)
	{
		m_xUi[i].pBtn = new CButton;
		m_xUi[i].pBtn->Create(m_xUi[i].strCaption, WS_VISIBLE | WS_CHILD, m_xUi[i].rcUi, this, m_xUi[i].nID);
		g_AoiFont.SetWindowFont(m_xUi[i].pBtn, FontDef::typeT1);
	}
	for (int i = UI_GB_BEGIN; i < UI_GB_END; i++)
	{
		m_xUi[i].pBtn = new CButton;
		m_xUi[i].pBtn->Create(m_xUi[i].strCaption, WS_VISIBLE | WS_CHILD | BS_GROUPBOX, m_xUi[i].rcUi, this, m_xUi[i].nID);
		g_AoiFont.SetWindowFont(m_xUi[i].pBtn, FontDef::typeT1);
	}
	for (int i = UI_EDIT_BEGIN; i < UI_EDIT_END; i++)
	{
		m_xUi[i].pEdit = new CEdit;
		m_xUi[i].pEdit->Create(WS_VISIBLE | WS_CHILD | WS_BORDER | ES_AUTOHSCROLL, m_xUi[i].rcUi, this, m_xUi[i].nID);
		g_AoiFont.SetWindowFont(m_xUi[i].pEdit, FontDef::typeT1);
	}
#endif
	for (int i = UI_LC_BEGIN; i < UI_LC_END; i++)
	{
		m_xUi[i].pList = new CListCtrl;
		m_xUi[i].pList->Create(WS_VISIBLE | WS_CHILD | WS_BORDER | LVS_REPORT | LVS_OWNERDATA, m_xUi[i].rcUi, this, m_xUi[i].nID);
		m_xUi[i].pList->SetExtendedStyle(LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);
	}
	m_xUi[UI_LC_CLIENT].pList->InsertColumn(LIST_COL_IP, L"Ip", LVCFMT_LEFT, 100);
	m_xUi[UI_LC_CLIENT].pList->InsertColumn(LIST_COL_PORT, L"Port", LVCFMT_LEFT, 80);

	m_xUi[UI_LC_PARAM].pList->InsertColumn(LIST_COL_FIELDNAME, L"FIELD", LVCFMT_LEFT, 100);
	m_xUi[UI_LC_PARAM].pList->InsertColumn(LIST_COL_VALUE1, L"VALUE1", LVCFMT_LEFT, 130);
	m_xUi[UI_LC_PARAM].pList->InsertColumn(LIST_COL_VALUE2, L"VALUE2", LVCFMT_LEFT, 130);
	m_xUi[UI_LC_PARAM].pList->InsertColumn(LIST_COL_VALUE3, L"VALUE3", LVCFMT_LEFT, 130);

	m_xUi[UI_LC_RESULT].pList->InsertColumn(LIST_COL_FIELDNAME, L"FIELD", LVCFMT_LEFT, 100);
	m_xUi[UI_LC_RESULT].pList->InsertColumn(LIST_COL_VALUE1, L"VALUE", LVCFMT_LEFT, 130);

	m_xUi[UI_LC_CLIENT].pList->SetItemCount(MAX_CLIENT);
}
void CEMCCommunicatorDlg::InitSocket()
{
	CString strLog;
	strLog.Format(L"Listen Port:%d, EMC IP: %s, EMC Port: %d", m_nListenPort, m_strServerIp, m_nServerPort);
	theApp.InsertDebugLog(strLog, LOG_EMCSYSTEM);
	m_pServer = new EMCParamSocket(NULL);
	m_pServer->SetPort(m_nListenPort);
	m_pServer->AttachLink(this);
	m_pServer->Start();

	m_pClientMgr = new EMCResultSocketMgr(m_strServerIp, m_nServerPort);
	m_pClientMgr->AttachLink(this);
	m_pClientMgr->Connect();

	InvalidateRect(&m_xUi[UI_LABEL_INFO1].rcUi);
	InvalidateRect(&m_xUi[UI_LABEL_INFO2].rcUi);
}
void CEMCCommunicatorDlg::Finalize()
{
#ifdef EMC_SIMLULATE
	if (m_pSimulateDataHandler)
	{
		delete m_pSimulateDataHandler;
		m_pSimulateDataHandler = NULL;
	}
#endif
	if (m_pEMCDataHandler)
	{
		delete m_pEMCDataHandler;
		m_pEMCDataHandler = NULL;
	}
	if (m_pServer)
	{
		m_pServer->Stop();
		delete m_pServer;
		m_pServer = NULL;
	}
	if (m_pClientMgr)
	{
		m_pClientMgr->Disconnect();
		delete m_pClientMgr;
		m_pClientMgr = NULL;
	}
	theApp.InsertDebugLog(L"End", LOG_EMCSYSTEM);
}
void CEMCCommunicatorDlg::DrawInfo()
{
	CDC* pDC = GetDC();
	CString strInfo1, strInfo2;
	pDC->SelectObject(g_AoiFont.GetFont(typeT1));
	if (m_pServer)
	{
		switch (m_pServer->GetServerState())
		{
			case EMCParamSocket::SERVER_STATE::MODE_SERVER_IDLE:
				strInfo1.Format(L"IDLE ");
				break;
			case EMCParamSocket::SERVER_STATE::MODE_SERVER_LISTEN:
				strInfo1.Format(L"Listening port:%d ", m_pServer->GetListenPort());
				break;
		}
	}
	switch (m_eType)
	{
		case CCL:
			strInfo1 += L" \nMode: CCL";
			break;
		case PP:
			strInfo1 += L" \nMode: PP";
			break;
	}

	strInfo2.Format(L"ServerIp: %s \nPort: %d ", m_strServerIp, m_nServerPort);
	if (m_pClientMgr)
	{
		strInfo2 += m_pClientMgr->GetConnectState();
	}

	pDC->SetBkMode(TRANSPARENT);
	pDC->DrawText(strInfo1, &m_xUi[UI_LABEL_INFO1].rcUi, DT_LEFT);
	pDC->DrawText(strInfo2, &m_xUi[UI_LABEL_INFO2].rcUi, DT_LEFT);

#ifdef EMC_SIMLULATE
	for (int i = UI_LABEL_STATION; i <= UI_LABEL_SERIAL; i++)
	{
		pDC->DrawText(m_xUi[i].strCaption, &m_xUi[i].rcUi, DT_LEFT);
	}
	if (m_eType == PRODUCT_TYPE::CCL)
	{
		for (int i = UI_LABEL_DEFECTTYPE; i <= UI_LABEL_SHEETNUM; i++)
		{
			pDC->DrawText(m_xUi[i].strCaption, &m_xUi[i].rcUi, DT_LEFT);
		}
	}
	else if (m_eType == PRODUCT_TYPE::PP)
	{
		pDC->DrawText(m_xUi[UI_LABEL_DEFECTTYPE].strCaption, &m_xUi[UI_LABEL_DEFECTTYPE].rcUi, DT_LEFT);
		for (int i = UI_LABEL_DEFECTBEGIN; i <= UI_LABEL_LENGTH; i++)
		{
			pDC->DrawText(m_xUi[i].strCaption, &m_xUi[i].rcUi, DT_LEFT);
		}
	}

#endif
	ReleaseDC(pDC);
}
void CEMCCommunicatorDlg::HandleAOIResponse(LPARAM lParam)
{
	switch (lParam)
	{
		case WM_EMC_PARAMINIT_CMD:
			BATCH_SHARE_EMC_INITPARAM xData;
			memset(&xData, 0, sizeof(BATCH_SHARE_EMC_INITPARAM));
			if (OnInitEMCProcess(&xData))
			{

				m_strServerIp = xData.cEMCIP;
				m_nServerPort = xData.nEMCPort;
				m_eType = (PRODUCT_TYPE)xData.nProductType;
				m_nListenPort = xData.nListenPort;

				if (m_eType == PRODUCT_TYPE::CCL)
				{
					m_xUi[UI_LC_PARAM].pList->SetItemCount(_countof(ctEMC_CCL_FIELD));
					m_xUi[UI_LC_RESULT].pList->SetItemCount(_countof(ctEMC_CCL_FIELD));
				}
				else if (m_eType == PRODUCT_TYPE::PP)
				{
					m_xUi[UI_LC_PARAM].pList->SetItemCount(_countof(ctEMC_PP_FIELD));
					m_xUi[UI_LC_RESULT].pList->SetItemCount(_countof(ctEMC_PP_FIELD));
				}

				InitSocket();
			}
			break;
		case WM_EMC_PARAMCCL_CMD:
			OnSendCCLDone();
			break;
		case WM_EMC_PARAMPP_CMD:
			OnSendPPDone();
			break;
	}
}
CString CEMCCommunicatorDlg::GetCCLValue(EMC_CCL_FIELD_TYPE eType, EMC_CCL_DATA& xData)
{
	CString strTemp, strValue;
	switch (eType)
	{
		case EMC_CCL_FIELD_TYPE::CCL_SC:
			strValue = xData.strStation;
			break;
		case EMC_CCL_FIELD_TYPE::CCL_NO:
			strValue = xData.strMissionID;
			break;
		case EMC_CCL_FIELD_TYPE::CCL_SO:
			strValue = xData.strBatchName;
			break;
		case EMC_CCL_FIELD_TYPE::CCL_PN:
			strValue = xData.strMaterial;
			break;
		case EMC_CCL_FIELD_TYPE::CCL_LT:
			strValue = xData.strSerial;
			break;
		case EMC_CCL_FIELD_TYPE::CCL_QT:
			strValue.Format(L"%d", xData.nNum);
			break;
		case EMC_CCL_FIELD_TYPE::CCL_BK:
			strValue.Format(L"%d", xData.nBookNum);
			break;
		case EMC_CCL_FIELD_TYPE::CCL_QS:
			strValue.Format(L"%d", xData.nSheetNum);
			break;
		case EMC_CCL_FIELD_TYPE::CCL_UP:
			strValue.Format(L"%d", xData.nSplit);
			break;
		case EMC_CCL_FIELD_TYPE::CCL_ST:
			strValue = CEMCParser::GetStatus(xData.eStatus);
			break;
		case EMC_CCL_FIELD_TYPE::CCL_E1:
			for (auto& i : xData.vMiss)
			{
				strTemp.Format(L"%d-%d|", i.first, i.second);
				strValue += strTemp;
			}
			break;
		case EMC_CCL_FIELD_TYPE::CCL_SQ:
			strValue.Format(L"%d", xData.nIndex);
			break;
		case EMC_CCL_FIELD_TYPE::CCL_DT:
			strValue.Format(L"%s", xData.strDefectType);
			break;
		case EMC_CCL_FIELD_TYPE::CCL_CN:
			strValue = xData.strEmpID;
			break;
		case EMC_CCL_FIELD_TYPE::CCL_QR:
			strValue = xData.strSheet;
			break;
		case EMC_CCL_FIELD_TYPE::CCL_TIME:
			strValue = xData.xTime.Format(L"%Y/%m/%d %H:%M:%S");
			break;
		case EMC_CCL_FIELD_TYPE::CCL_BB:
			strValue.Format(L"%d", xData.nBeginBook);
			break;
		case EMC_CCL_FIELD_TYPE::CCL_EB:
			strValue.Format(L"%d", xData.nEndBook);
			break;
		case EMC_CCL_FIELD_TYPE::CCL_BS:
			strValue.Format(L"%d", xData.nBeginSheet);
			break;
		case EMC_CCL_FIELD_TYPE::CCL_ES:
			strValue.Format(L"%d", xData.nEndSheet);
			break;
	}
	return strValue;
}
CString CEMCCommunicatorDlg::GetPPValue(EMC_PP_FIELD_TYPE eType, EMC_PP_DATA& xData)
{
	CString strTemp, strValue;
	switch (eType)
	{
		case EMC_PP_FIELD_TYPE::PP_SC:
			strValue = xData.strStation;
			break;
		case EMC_PP_FIELD_TYPE::PP_NO:
			strValue = xData.strMissionID;
			break;
		case EMC_PP_FIELD_TYPE::PP_SO:
			strValue = xData.strBatchName;
			break;
		case EMC_PP_FIELD_TYPE::PP_PN:
			strValue = xData.strMaterial;
			break;
		case EMC_PP_FIELD_TYPE::PP_LT:
			for (auto& i : xData.vSerial)
			{
				strTemp.Format(L"%s|", i);
				strValue += strTemp;
			}
			break;
		case EMC_PP_FIELD_TYPE::PP_QT:
			strValue.Format(L"%.1f", xData.fLength);
			break;
		case EMC_PP_FIELD_TYPE::PP_QB:
			strValue.Format(L"%.1f", xData.fDefectBegin);
			break;
		case EMC_PP_FIELD_TYPE::PP_QE:
			strValue.Format(L"%.1f", xData.fDefectEnd);
			break;
		case EMC_PP_FIELD_TYPE::PP_ST:
			strValue = CEMCParser::GetStatus(xData.eStatus);
			break;
		case EMC_PP_FIELD_TYPE::PP_DT:
			strValue.Format(L"%s", xData.strDefectType);
			break;
		case EMC_PP_FIELD_TYPE::PP_CN:
			strValue = xData.strEmpID;
			break;
		case EMC_PP_FIELD_TYPE::PP_TIME:
			strValue = xData.xTime.Format(L"%Y/%m/%d %H:%M:%S");
			break;
	}
	return strValue;
}
#ifdef EMC_SIMLULATE
void CEMCCommunicatorDlg::InitSimulate()
{
	wchar_t	workingDir[_MAX_PATH];
	_wgetcwd(workingDir, _MAX_PATH);
	CString strPath;
	strPath.Format(L"%s//SIMULATE.INI", workingDir);
	wchar_t szData[MAX_PATH];

	m_eType = (PRODUCT_TYPE)::GetPrivateProfileInt(_T("PARAM"), _T("TYPE"), NULL, strPath);
	m_nListenPort = ::GetPrivateProfileInt(_T("PARAM"), _T("LISTENPORT"), NULL, strPath);

	::GetPrivateProfileString(_T("PARAM"), _T("SERVERIP"), _T(""), szData, MAX_PATH, strPath); m_strServerIp = szData;

	m_nServerPort = ::GetPrivateProfileInt(_T("PARAM"), _T("SERVERPORT"), NULL, strPath);

	InitSocket();

	if (m_eType == PRODUCT_TYPE::CCL)
	{
		m_xUi[UI_EDIT_DEFECTBEGIN].pEdit->ShowWindow(SW_HIDE);
		m_xUi[UI_EDIT_DEFECTEND].pEdit->ShowWindow(SW_HIDE);
		m_xUi[UI_EDIT_LENGTH].pEdit->ShowWindow(SW_HIDE);

		m_xUi[UI_LC_PARAM].pList->SetItemCount(_countof(ctEMC_CCL_FIELD));
		m_xUi[UI_LC_RESULT].pList->SetItemCount(_countof(ctEMC_CCL_FIELD));
	}
	else if (m_eType == PRODUCT_TYPE::PP)
	{
		m_xUi[UI_EDIT_INDEX].pEdit->ShowWindow(SW_HIDE);
		m_xUi[UI_EDIT_BOOKNUM].pEdit->ShowWindow(SW_HIDE);
		m_xUi[UI_EDIT_SHEETNUM].pEdit->ShowWindow(SW_HIDE);

		m_xUi[UI_LC_PARAM].pList->SetItemCount(_countof(ctEMC_PP_FIELD));
		m_xUi[UI_LC_RESULT].pList->SetItemCount(_countof(ctEMC_PP_FIELD));
	}
}
#endif