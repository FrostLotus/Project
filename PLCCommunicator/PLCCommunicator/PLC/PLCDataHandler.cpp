#include "stdafx.h"
#include "PLCDataHandler.h"
#ifdef USE_IN_COMMUNICATOR
#include "PLCCommunicator.h"
#else
#include "Aoi.h"
#endif

CPLCDataHandler::CPLCDataHandler() 
{
	memset(&m_xSYSTResult, 0, sizeof(m_xSYSTResult));
	m_dwSYSTDirty = 0;
	memset(&m_xInfo, 0, sizeof(m_xInfo));
#ifdef USE_IN_COMMUNICATOR
	NotifyAOI(WM_SYST_PARAMINIT_CMD, NULL);
#endif
}
CPLCDataHandler::~CPLCDataHandler()
{
}

void CPLCDataHandler::SetSYSTParam_WebCopper(BATCH_SHARE_SYST_BASE *pData)
{
	if (pData){
#ifdef USE_IN_COMMUNICATOR
		CString strLog;
		strLog.Format(L"Set NewBatch: Order: %s, Material %s", pData->cName, pData->cMaterial);
		theApp.InsertDebugLog(strLog, LOG_DEBUG);
#endif
		SetSharedMemoryData(pData, sizeof(BATCH_SHARE_SYST_BASE), AOI_MASTER_NAME, WM_SYST_PARAMWEBCOOPER_CMD, NULL);
#ifdef USE_IN_COMMUNICATOR
		theApp.InsertDebugLog(L"Set NewBatch Done", LOG_DEBUG);
#endif
	}
}
void CPLCDataHandler::GetSYSTParam_WebCopper(BATCH_SHARE_SYST_BASE *pData)
{
#ifndef USE_IN_COMMUNICATOR
	theApp.InsertDebugLog(L"Get NewBatch", LOG_PLC_DEBUG);
#endif
	GetSharedMemoryData(pData, sizeof(BATCH_SHARE_SYST_BASE), BATCH_COMMUNICATOR_MEM_ID);
#ifndef USE_IN_COMMUNICATOR
	CString strLog;
	strLog.Format(L"Get NewBatch Done: Order:%s, Material:%s",
		pData->cName,
		pData->cMaterial
		);
	theApp.InsertDebugLog(strLog, LOG_PLC_DEBUG);
#endif
}
void CPLCDataHandler::SetSYSYParam_CCL(BATCH_SHARE_SYST_PARAMCCL *pData)
{
#ifdef USE_IN_COMMUNICATOR
	CString strLog;
	strLog.Format(L"Set SYSTParam: \r\n");

	strLog += MakeCStringLog(L"cName", pData->cName);
	strLog += MakeCStringLog(L"cMaterial", pData->cMaterial);
	strLog += MakeCStringLog(L"cModel", pData->cModel);
	strLog += MakeCStringLog(L"cAssign", pData->cAssign);
	strLog += MakeWordLog(L"wAssignNum", pData->wAssignNum);
	strLog += MakeWordLog(L"wSplitNum", pData->wSplitNum);
	strLog += MakeWordLog(L"wSplit_One_Y", pData->wSplit_One_Y);
	strLog += MakeWordLog(L"wSplit_Two_Y", pData->wSplit_Two_Y);
	strLog += MakeWordLog(L"wSplit_Three_Y", pData->wSplit_Three_Y);
	strLog += MakeWordLog(L"wSplit_One_X", pData->wSplit_One_X);
	strLog += MakeWordLog(L"wSplit_Two_X", pData->wSplit_Two_X);
	strLog += MakeWordLog(L"wSplit_Three_X", pData->wSplit_Three_X);
	strLog += MakeFloatLog(L"fThickSize", pData->fThickSize);
	strLog += MakeFloatLog(L"fThickCCL", pData->fThickCCL);
	strLog += MakeCStringLog(L"cCCLType", pData->cCCLType);
	strLog += MakeWordLog(L"wGrayScale", pData->wGrayScale);
	strLog += MakeWordLog(L"wPixel_AA", pData->wPixel_AA);
	strLog += MakeWordLog(L"wPixel_A", pData->wPixel_A);
	strLog += MakeWordLog(L"wPixel_P", pData->wPixel_P);
	strLog += MakeWordLog(L"wDiff_One_Y", pData->wDiff_One_Y);
	strLog += MakeWordLog(L"wDiff_One_X", pData->wDiff_One_X);
	strLog += MakeWordLog(L"wDiff_One_XY", pData->wDiff_One_XY);
	strLog += MakeWordLog(L"wDiff_One_X_Min", pData->wDiff_One_X_Min);
	strLog += MakeWordLog(L"wDiff_One_X_Max", pData->wDiff_One_X_Max);
	strLog += MakeWordLog(L"wDiff_One_Y_Min", pData->wDiff_One_Y_Min);
	strLog += MakeWordLog(L"wDiff_One_Y_Max", pData->wDiff_One_Y_Max);
	strLog += MakeWordLog(L"wDiff_One_XY_Min", pData->wDiff_One_XY_Min);
	strLog += MakeWordLog(L"wDiff_One_XY_Max", pData->wDiff_One_XY_Max);
	strLog += MakeWordLog(L"wDiff_Two_X_Min", pData->wDiff_Two_X_Min);
	strLog += MakeWordLog(L"wDiff_Two_X_Max", pData->wDiff_Two_X_Max);
	strLog += MakeWordLog(L"wDiff_Two_Y_Min", pData->wDiff_Two_Y_Min);
	strLog += MakeWordLog(L"wDiff_Two_Y_Max", pData->wDiff_Two_Y_Max);
	strLog += MakeWordLog(L"wDiff_Two_XY_Min", pData->wDiff_Two_XY_Min);
	strLog += MakeWordLog(L"wDiff_Two_XY_Max", pData->wDiff_Two_XY_Max);
	strLog += MakeWordLog(L"wDiff_Three_X_Min", pData->wDiff_Three_X_Min);
	strLog += MakeWordLog(L"wDiff_Three_X_Max", pData->wDiff_Three_X_Max);
	strLog += MakeWordLog(L"wDiff_Three_Y_Min", pData->wDiff_Three_Y_Min);
	strLog += MakeWordLog(L"wDiff_Three_Y_Max", pData->wDiff_Three_Y_Max);
	strLog += MakeWordLog(L"wDiff_Three_XY_Min", pData->wDiff_Three_XY_Min);
	strLog += MakeWordLog(L"wDiff_Three_XY_Max", pData->wDiff_Three_XY_Max);
	strLog += MakeWordLog(L"wNum_AA", pData->wNum_AA);
	strLog += MakeWordLog(L"wNO_C06", pData->wNO_C06);
	strLog += MakeWordLog(L"wNO_C10", pData->wNO_C10);
	strLog += MakeWordLog(L"wNO_C12", pData->wNO_C12);

		theApp.InsertDebugLog(strLog, LOG_DEBUG);
#endif	
		SetSharedMemoryData(pData, sizeof(BATCH_SHARE_SYST_PARAMCCL), AOI_MASTER_NAME, WM_SYST_PARAMCCL_CMD, NULL);
#ifdef USE_IN_COMMUNICATOR
	theApp.InsertDebugLog(L"Set SYSTParam Done", LOG_DEBUG);
#endif
}
void CPLCDataHandler::GetSYSTParam_CCL(BATCH_SHARE_SYST_PARAMCCL *pData)
{
#ifndef USE_IN_COMMUNICATOR
	theApp.InsertDebugLog(L"Get SYSTParam", LOG_PLC_DEBUG);
#endif
	GetSharedMemoryData(pData, sizeof(BATCH_SHARE_SYST_PARAMCCL), BATCH_COMMUNICATOR_MEM_ID);
#ifndef USE_IN_COMMUNICATOR
	CString strLog;
	strLog.Format(L"Get SYSTParam: \r\n");

	strLog += MakeCStringLog(L"cName", pData->cName);
	strLog += MakeCStringLog(L"cMaterial", pData->cMaterial);
	strLog += MakeCStringLog(L"cModel", pData->cModel);
	strLog += MakeCStringLog(L"cAssign", pData->cAssign);
	strLog += MakeWordLog(L"wAssignNum", pData->wAssignNum);
	strLog += MakeWordLog(L"wSplitNum", pData->wSplitNum);
	strLog += MakeWordLog(L"wSplit_One_Y", pData->wSplit_One_Y);
	strLog += MakeWordLog(L"wSplit_Two_Y", pData->wSplit_Two_Y);
	strLog += MakeWordLog(L"wSplit_Three_Y", pData->wSplit_Three_Y);
	strLog += MakeWordLog(L"wSplit_One_X", pData->wSplit_One_X);
	strLog += MakeWordLog(L"wSplit_Two_X", pData->wSplit_Two_X);
	strLog += MakeWordLog(L"wSplit_Three_X", pData->wSplit_Three_X);
	strLog += MakeFloatLog(L"fThickSize", pData->fThickSize);
	strLog += MakeFloatLog(L"fThickCCL", pData->fThickCCL);
	strLog += MakeCStringLog(L"cCCLType", pData->cCCLType);
	strLog += MakeWordLog(L"wGrayScale", pData->wGrayScale);
	strLog += MakeWordLog(L"wPixel_AA", pData->wPixel_AA);
	strLog += MakeWordLog(L"wPixel_A", pData->wPixel_A);
	strLog += MakeWordLog(L"wPixel_P", pData->wPixel_P);
	strLog += MakeWordLog(L"wDiff_One_Y", pData->wDiff_One_Y);
	strLog += MakeWordLog(L"wDiff_One_X", pData->wDiff_One_X);
	strLog += MakeWordLog(L"wDiff_One_XY", pData->wDiff_One_XY);
	strLog += MakeWordLog(L"wDiff_One_X_Min", pData->wDiff_One_X_Min);
	strLog += MakeWordLog(L"wDiff_One_X_Max", pData->wDiff_One_X_Max);
	strLog += MakeWordLog(L"wDiff_One_Y_Min", pData->wDiff_One_Y_Min);
	strLog += MakeWordLog(L"wDiff_One_Y_Max", pData->wDiff_One_Y_Max);
	strLog += MakeWordLog(L"wDiff_One_XY_Min", pData->wDiff_One_XY_Min);
	strLog += MakeWordLog(L"wDiff_One_XY_Max", pData->wDiff_One_XY_Max);
	strLog += MakeWordLog(L"wDiff_Two_X_Min", pData->wDiff_Two_X_Min);
	strLog += MakeWordLog(L"wDiff_Two_X_Max", pData->wDiff_Two_X_Max);
	strLog += MakeWordLog(L"wDiff_Two_Y_Min", pData->wDiff_Two_Y_Min);
	strLog += MakeWordLog(L"wDiff_Two_Y_Max", pData->wDiff_Two_Y_Max);
	strLog += MakeWordLog(L"wDiff_Two_XY_Min", pData->wDiff_Two_XY_Min);
	strLog += MakeWordLog(L"wDiff_Two_XY_Max", pData->wDiff_Two_XY_Max);
	strLog += MakeWordLog(L"wDiff_Three_X_Min", pData->wDiff_Three_X_Min);
	strLog += MakeWordLog(L"wDiff_Three_X_Max", pData->wDiff_Three_X_Max);
	strLog += MakeWordLog(L"wDiff_Three_Y_Min", pData->wDiff_Three_Y_Min);
	strLog += MakeWordLog(L"wDiff_Three_Y_Max", pData->wDiff_Three_Y_Max);
	strLog += MakeWordLog(L"wDiff_Three_XY_Min", pData->wDiff_Three_XY_Min);
	strLog += MakeWordLog(L"wDiff_Three_XY_Max", pData->wDiff_Three_XY_Max);
	strLog += MakeWordLog(L"wNum_AA", pData->wNum_AA);
	strLog += MakeWordLog(L"wNO_C06", pData->wNO_C06);
	strLog += MakeWordLog(L"wNO_C10", pData->wNO_C10);
	strLog += MakeWordLog(L"wNO_C12", pData->wNO_C12);
	strLog += MakeWordLog(L"wINSP_CCD_ENABLE", pData->wINSP_CCD_ENABLE);
	strLog += MakeWordLog(L"wINSP_SIZE_ENABLE", pData->wINSP_SIZE_ENABLE);

	strLog += MakeByteLog(L"cErrorBlower1", pData->xInfo.cErrorBlower1);
	strLog += MakeByteLog(L"cErrorBlower2", pData->xInfo.cErrorBlower2);
	strLog += MakeByteLog(L"cErrorBelt1", pData->xInfo.cErrorBelt1);
	strLog += MakeByteLog(L"cErrorBelt2", pData->xInfo.cErrorBelt2);
	strLog += MakeByteLog(L"cErrorOP", pData->xInfo.cErrorOP);
	strLog += MakeByteLog(L"cErrorRoll", pData->xInfo.cErrorRoll);
	strLog += MakeByteLog(L"cErrorDrop", pData->xInfo.cErrorDrop);
	strLog += MakeByteLog(L"cErrorStock", pData->xInfo.cErrorStock);
	theApp.InsertDebugLog(strLog, LOG_PLC_DEBUG);
#endif
}
void CPLCDataHandler::SetInitParam(BATCH_SHARE_SYST_INITPARAM *pData)
{
	SetSharedMemoryData(pData, sizeof(BATCH_SHARE_SYST_INITPARAM), PLC_COMMUNICATOR_NAME, WM_AOI_RESPONSE_CMD, WM_SYST_PARAMINIT_CMD);
}
void CPLCDataHandler::GetInitParam(BATCH_SHARE_SYST_INITPARAM *pData)
{
	GetSharedMemoryData(pData, sizeof(BATCH_SHARE_SYST_INITPARAM), BATCH_AOI_MEM_ID);
}
void CPLCDataHandler::SetSYSTInfo_CCL(DWORD dwField, BATCH_SHARE_SYST_INFO &xInfo)
{
	CString strLog;
	BOOL bNotify = FALSE;
	if (dwField & SIZE_READY){
		if (m_xInfo.xInfo1.cSizeReady != xInfo.xInfo1.cSizeReady){
			m_xInfo.xInfo1.cSizeReady = xInfo.xInfo1.cSizeReady;
			bNotify = TRUE;
		}
	}
	if (dwField & SIZE_RUNNING){
		if (m_xInfo.xInfo1.cSizeRunning != xInfo.xInfo1.cSizeRunning){
			m_xInfo.xInfo1.cSizeRunning = xInfo.xInfo1.cSizeRunning;
			bNotify = TRUE;
		}
	}
	if (dwField & SIZE_ERROR){
		if (m_xInfo.xInfo2.cSizeError1 != xInfo.xInfo2.cSizeError1){
			m_xInfo.xInfo2.cSizeError1 = xInfo.xInfo2.cSizeError1;
			bNotify = TRUE;
		}
	}
	if (dwField & CCD_READY){
		if (m_xInfo.xInfo1.cCCDReady != xInfo.xInfo1.cCCDReady){
			m_xInfo.xInfo1.cCCDReady = xInfo.xInfo1.cCCDReady;
			bNotify = TRUE;
		}
	}
	if (dwField & CCD_RUNNING){
		if (m_xInfo.xInfo1.cCCDRunning != xInfo.xInfo1.cCCDRunning){
			m_xInfo.xInfo1.cCCDRunning = xInfo.xInfo1.cCCDRunning;
			bNotify = TRUE;
		}
	}
	if (dwField & CCD_ERROR){
		if (m_xInfo.xInfo2.cCCDError1 != xInfo.xInfo2.cCCDError1){
			m_xInfo.xInfo2.cCCDError1 = xInfo.xInfo2.cCCDError1;
			bNotify = TRUE;
		}
	}
	if (bNotify){
		unsigned char *pShare = BeginWrite();
		if (pShare){
			memcpy(pShare, &m_xInfo, sizeof(m_xInfo));

			EndWrite();

			strLog = L"Set Info: " + MakeWordLog(L"m_xInfo1", *(WORD*)&m_xInfo.xInfo1) + MakeWordLog(L"m_xInfo2", *(WORD*)&m_xInfo.xInfo2);
			HWND hWnd = ::FindWindow(NULL, PLC_COMMUNICATOR_NAME);
			if (hWnd){
				::PostMessage(hWnd, WM_GPIO_MSG, WM_SYST_INFO_CHANGE, NULL);
#ifndef USE_IN_COMMUNICATOR
				theApp.InsertDebugLog(strLog, LOG_PLC_DEBUG);
#endif
			}
		}
	}
}
void CPLCDataHandler::GetSYSTInfo_CCL(BATCH_SHARE_SYST_INFO *pInfo)
{
#ifdef USE_IN_COMMUNICATOR
	theApp.InsertDebugLog(L"Get SYST Info", LOG_DEBUG);
	CString strLog;
#endif
	usm<unsigned char> xShareMem(BATCH_AOI_MEM_ID, TRUE);
	const unsigned char *pShare = xShareMem.BeginRead();
	if (pShare){
		memcpy(pInfo, pShare, sizeof(BATCH_SHARE_SYST_INFO));
#ifdef USE_IN_COMMUNICATOR
		strLog = L"Get Info: " + MakeWordLog(L"xInfo1", *(WORD*)(&pInfo->xInfo1)) + MakeWordLog(L"xInfo2", *(WORD*)(&pInfo->xInfo1));
#endif
	}
	xShareMem.EndRead();
}