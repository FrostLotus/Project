#include "stdafx.h"
#include "SystDONGGUANPPProcess.h"
#include "OPCCommunicator.h"
#include "DataHandlerBase.h"
#include <share.h>

#define INI_SECTION_RULE L"RULE"
#define INI_SECTION_ITEM L"ITEM"
#define INI_FIELD_NAME L"NAME"
#define INI_FIELD_SCORE L"SCORE"
#define INI_FIELD_OPC_FIELD L"OPC_FIELD"
#define INI_FIELD_DEFECT_ID L"DEFECT_ID"
const vector<CString> ctYJDFiles = { L"AA.ini", L"BB.ini", L"CC.ini", L"DD.ini" }; //保留不刪除檔案
///<summary>寫入Float</summary>
void CSystDONGGUANPPProcess::WriteFloat(int nField, float fData)
{

	UA_NodeId xNode;
	if (GetUANodeId(nField, xNode)){
		WriteFloatField(xNode, fData);
	}
}
///<summary>寫入int</summary>
void CSystDONGGUANPPProcess::WriteInt(int nField, int nData)
{
	UA_NodeId xNode;
	if (GetUANodeId(nField, xNode)){
		WriteIntField(xNode, nData);
	}
}
///<summary>寫入String</summary>
void CSystDONGGUANPPProcess::WriteString(int nField, char* pStr, int nLen)
{
	UA_NodeId xNode;
	CStringA str(pStr, nLen);
	if (GetUANodeId(nField, xNode)){
		WriteStringField(xNode, str.GetBuffer());
	}
}
///<summary>[Constructor]初始化</summary>
CSystDONGGUANPPProcess::CSystDONGGUANPPProcess()
{
	Init();
}
///<summary>[Constructor]終處置</summary>
CSystDONGGUANPPProcess::~CSystDONGGUANPPProcess()
{
	Finalize();
}

void CSystDONGGUANPPProcess::IninOPCClient()
{
	vector<CString> vNodeName;
	int nFieldSize = GetNodeSize();
	if (m_ppFIELD_INFO){
		for (int i = 0; i < nFieldSize; i++){
			if (m_ppFIELD_INFO[i]){
				vNodeName.push_back(CString(m_ppFIELD_INFO[i]->strNodeId));
			}
		}
	}
	ON_SET_MONITOR_NDOE(vNodeName);
}
///<summary>初始化項目</summary>
void CSystDONGGUANPPProcess::Init()
{
#ifdef WRITE_OPCFIELD_THREAD
	InitializeCriticalSection(&m_xLock);
	InitializeCriticalSection(&m_xBatch);
#endif

	for (int i = NULL; i < EV_COUNT; i++)
	{
		m_hEvent[i] = ::CreateEvent(NULL, TRUE, FALSE, NULL);
	}
	m_hThread = ::CreateThread(NULL, NULL, Thread_Process, this, NULL, NULL);

	struct FIELD_ITEM{
		int nFieldId;
		TCHAR strName[100];
		TCHAR strNodeId[100];
		TCHAR strValue[100];
		int nLen; //inBytes
		int nType;
		time_t xTime;
	};
	const FIELD_ITEM ctSYST_FIELD[FIELD_MAX] = {
#ifdef _DEBUG
		{ FIELD_AUFNR, L"订单号", L"AUFNR", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_MATNR, L"物料", L"MATNR", L"0123456789012345", 36, UA_TYPES_STRING, 0 },
		{ FIELD_MENGE, L"数量(批次米数/张数)", L"MENGE", L"1", 4, UA_TYPES_FLOAT, 0 },
		{ FIELD_MEINS, L"单位", L"MEINS", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_MACHINE, L"机台号", L"MACHINE", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_JSPF, L"胶水型号", L"JSPF", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_BLBFPH, L"玻璃布发票号", L"BLBFPH", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_BLBBZ, L"玻璃布布种", L"BLBBZ", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_YJD, L"严谨度", L"YJD", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_CHARG, L"批次號", L"CHARG", L"01234567890123456789", 40, UA_TYPES_STRING, 0 },
		{ FIELD_ZFLAG, L"有标无标", L"ZFLAG", L"01", 4, UA_TYPES_STRING, 0 },
		{ FIELD_SS_HD, L"SS黑点", L"SS_HD", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_S_HD, L"S黑点", L"S_HD", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_M_HD, L"M黑点", L"M_HD", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_L_HD, L"L黑点", L"L_HD", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_LL_HD, L"LL黑点", L"LL_HD", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_L_ZH, L"L折痕", L"L_ZH", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_S_ZH, L"S折痕", L"S_ZH", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_JIAOB, L"胶斑", L"JIAOB", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_TW, L"条纹", L"TW", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_S_JL, L"S胶粒", L"S_JL", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_L_JL, L"L胶粒", L"L_JL", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_QSZ, L"缺树脂", L"QSZ", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_S_ZK, L"S针孔", L"S_ZK", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_L_ZK, L"L针孔", L"L_ZK", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_LG, L"流挂", L"LG", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_Z_TW, L"棕色条纹", L"Z_TW", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_H_TW, L"黑色条纹", L"H_TW", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_JIAOH, L"胶痕", L"JIAOH", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_WENC, L"蚊虫", L"WENC", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_MF, L"毛发", L"MF", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_WZ, L"油污", L"WZ", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_S_SS, L"S疏纱", L"S_SS", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_S_DS, L"S叠纱", L"S_DS", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_L_SS, L"L疏纱", L"L_SS", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_L_DS, L"L叠纱", L"L_DS", L"012", 6, UA_TYPES_STRING, 0 },
		{ FIELD_QP, L"气泡", L"QP", L"012", 6, UA_TYPES_STRING, 0 },

		{ FIELD_ZAUFNR, L"订单号", L"ZAUFNR", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_ZMATNR, L"物料", L"ZMATNR", L"0123456789012345", 36, UA_TYPES_STRING, 0 },
		{ FIELD_ZCHARG, L"CCD批号(流水号)", L"ZCHARG", L"0123456789", 40, UA_TYPES_STRING, 0 },
		{ FIELD_STARTDATE, L"CCD開始日期", L"STARTDATE", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_STARTTIME, L"CCD開始時間", L"STARTTIME", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_ENDDATE, L"CCD結束日期", L"ENDDATE", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_ENDTIME, L"CCD結束時間", L"ENDTIME", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_ZMENGE, L"批次数量", L"ZMENGE", L"1", 4, UA_TYPES_FLOAT, 0 },
		{ FIELD_ZMEINS, L"单位", L"ZMEINS", L"0123456789", 20, UA_TYPES_STRING, 0 },
		{ FIELD_NJPKD, L"粘结片宽度", L"NJPKD", L"1", 4, UA_TYPES_FLOAT, 0 },
		{ FIELD_MS, L"每卷米数", L"MS", L"1", 4, UA_TYPES_FLOAT, 0 },
		{ FIELD_SS_HDSL, L"SS黑点数量", L"SS_HDSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_S_HDSL, L"S黑点数量", L"S_HDSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_M_HDSL, L"M黑点数量", L"M_HDSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_L_HDSL, L"L黑点数量", L"L_HDSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_LL_HDSL, L"LL黑点数量", L"LL_HDSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_L_ZHSL, L"L折痕数量", L"L_ZHSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_S_ZHSL, L"S折痕数量", L"S_ZHSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_JIAOBSL, L"胶斑数量", L"JIAOBSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_TWSL, L"条纹数量", L"TWSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_S_JLSL, L"S胶粒数量", L"S_JLSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_L_JLSL, L"L胶粒数量", L"L_JLSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_QSZSL, L"缺树脂数量", L"QSZSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_S_ZKSL, L"S针孔数量", L"S_ZKSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_L_ZKSL, L"L针孔数量", L"L_ZKSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_LGSL, L"流挂数量", L"LGSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_Z_TWSL, L"棕色条纹数量", L"Z_TWSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_H_TWSL, L"黑色条纹数量", L"H_TWSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_JIAOHSL, L"胶痕数量", L"JIAOHSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_WENCSL, L"蚊虫数量", L"WENCSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_MFSL, L"毛发数量", L"MFSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_WZSL, L"油污数量", L"WZSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_S_SSSL, L"S疏纱数量", L"S_SSSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_S_DSSL, L"S叠纱数量", L"S_DSSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_L_SSSL, L"L疏纱数量", L"L_SSSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_L_DSSL, L"L叠纱数量", L"L_DSSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_QPSL, L"气泡数量", L"QPSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_QXSL, L"缺陷合计", L"QXSL", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_SCORE, L"扣分", L"SCORE", L"1", 2, UA_TYPES_INT16, 0 },
		{ FIELD_P_FLAG, L"批次数据刷新标志", L"P_FLAG", L"1", 1, UA_TYPES_BOOLEAN, 0 },


		{ FIELD_QXMS, L"缺陷米數", L"QXMS", L"1", 4, UA_TYPES_FLOAT, 0 },
		{ FIELD_QXDM, L"缺陷代碼", L"QXDM", L"", 40, UA_TYPES_STRING, 0 },
		{ FIELD_QXMJ, L"缺陷面積", L"QXMJ", L"1", 4, UA_TYPES_FLOAT, 0 },
		{ FIELD_QXCD, L"缺陷長度", L"QXCD", L"1", 4, UA_TYPES_FLOAT, 0 },
		{ FIELD_QXKD, L"缺陷寬度", L"QXKD", L"1", 4, UA_TYPES_FLOAT, 0 },
		{ FIELD_QX_X, L"缺陷X軸位置", L"QX_X", L"1", 4, UA_TYPES_FLOAT, 0 },
		{ FIELD_QX_Y, L"缺陷Y軸位置", L"QX_Y", L"1", 4, UA_TYPES_FLOAT, 0 },
		{ FIELD_ZFMWZ, L"正反面位置", L"ZFMWZ", L"1", 4, UA_TYPES_STRING, 0 },
		{ FIELD_CS, L"車速", L"CS", L"1", 4, UA_TYPES_FLOAT, 0 },
		{ FIELD_Z_QXFLAG, L"缺陷刷新标志", L"Z_QXFLAG", L"1", 1, UA_TYPES_BOOLEAN, 0 },
#else
		{ FIELD_AUFNR,		L"订单号",				L"AUFNR",	    L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_MATNR,		L"物料",				    L"MATNR",	    L"",			        36, UA_TYPES_STRING, 0 },
		{ FIELD_MENGE,		L"数量(批次米数/张数)",    L"MENGE",	    L"",			        4,	UA_TYPES_FLOAT,  0 },
		{ FIELD_MEINS,		L"单位",				    L"MEINS",	    L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_MACHINE,	L"机台号",				L"MACHINE",     L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_JSPF,		L"胶水型号",			    L"JSPF",	    L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_BLBFPH,		L"玻璃布发票号",		    L"BLBFPH",	    L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_BLBBZ,		L"玻璃布布种",			L"BLBBZ",	    L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_YJD,		L"严谨度",				L"YJD",		    L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_CHARG,		L"批次號",				L"CHARG",	    L"",			        40, UA_TYPES_STRING, 0 },
		{ FIELD_ZFLAG,		L"有标无标",			    L"ZFLAG",	    L"",			        4,	UA_TYPES_STRING, 0 },
		{ FIELD_SS_HD,		L"SS黑点",				L"SS_HD",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_S_HD,		L"S黑点",				L"S_HD",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_M_HD,		L"M黑点",				L"M_HD",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_L_HD,		L"L黑点",				L"L_HD",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_LL_HD,		L"LL黑点",				L"LL_HD",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_L_ZH,		L"L折痕",				L"L_ZH",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_S_ZH,		L"S折痕",				L"S_ZH",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_JIAOB,		L"胶斑",				    L"JIAOB",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_TW,			L"条纹",				    L"TW",		    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_S_JL,		L"S胶粒",				L"S_JL",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_L_JL,		L"L胶粒",				L"L_JL",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_QSZ,		L"缺树脂",				L"QSZ",		    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_S_ZK,		L"S针孔",				L"S_ZK",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_L_ZK,		L"L针孔",				L"L_ZK",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_LG,			L"流挂",				    L"LG",		    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_Z_TW,		L"棕色条纹",			    L"Z_TW",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_H_TW,		L"黑色条纹",			    L"H_TW",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_JIAOH,		L"胶痕",				    L"JIAOH",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_WENC,		L"蚊虫",				    L"WENC",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_MF,			L"毛发",				    L"MF",		    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_WZ,			L"油污",				    L"WZ",		    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_S_SS,		L"S疏纱",				L"S_SS",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_S_DS,		L"S叠纱",				L"S_DS",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_L_SS,		L"L疏纱",				L"L_SS",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_L_DS,		L"L叠纱",				L"L_DS",	    L"",			        6,	UA_TYPES_STRING, 0 },
		{ FIELD_QP,			L"气泡",				    L"QP",		    L"",			        6,	UA_TYPES_STRING, 0 },

		{ FIELD_ZAUFNR,		L"订单号",			    L"ZAUFNR",		L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_ZMATNR,		L"物料",			        L"ZMATNR",		L"",			        36, UA_TYPES_STRING, 0 },
		{ FIELD_ZCHARG,		L"CCD批号(流水号)",       L"ZCHARG",		L"",			        40, UA_TYPES_STRING, 0 },
		{ FIELD_STARTDATE,	L"CCD開始日期",		    L"STARTDATE",	L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_STARTTIME,	L"CCD開始時間",		    L"STARTTIME",	L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_ENDDATE,	L"CCD結束日期",		    L"ENDDATE",		L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_ENDTIME,	L"CCD結束時間",		    L"ENDTIME",		L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_ZMENGE,		L"批次数量",		        L"ZMENGE",		L"",			        4,	UA_TYPES_FLOAT,	 0 },
		{ FIELD_ZMEINS,		L"单位",			        L"ZMEINS",		L"",			        20, UA_TYPES_STRING, 0 },
		{ FIELD_NJPKD,		L"粘结片宽度",		    L"NJPKD",		L"",			        4,	UA_TYPES_FLOAT,	 0 },
		{ FIELD_MS,			L"每卷米数",		        L"MS",			L"",			        4,	UA_TYPES_FLOAT,	 0 },
		{ FIELD_SS_HDSL,	L"SS黑点数量",	       	L"SS_HDSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_S_HDSL,		L"S黑点数量",		        L"S_HDSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_M_HDSL,		L"M黑点数量",		        L"M_HDSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_L_HDSL,		L"L黑点数量",		        L"L_HDSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_LL_HDSL,	L"LL黑点数量",		    L"LL_HDSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_L_ZHSL,		L"L折痕数量",		        L"L_ZHSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_S_ZHSL,		L"S折痕数量",		        L"S_ZHSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_JIAOBSL,	L"胶斑数量",		        L"JIAOBSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_TWSL,		L"条纹数量",		        L"TWSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_S_JLSL,		L"S胶粒数量",		        L"S_JLSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_L_JLSL,		L"L胶粒数量",		        L"L_JLSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_QSZSL,		L"缺树脂数量",		    L"QSZSL",		L"1",			        2,  UA_TYPES_INT16,  0 },
		{ FIELD_S_ZKSL,		L"S针孔数量",		        L"S_ZKSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_L_ZKSL,		L"L针孔数量",		        L"L_ZKSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_LGSL,		L"流挂数量",		        L"LGSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_Z_TWSL,		L"棕色条纹数量",	        L"Z_TWSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_H_TWSL,		L"黑色条纹数量",	        L"H_TWSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_JIAOHSL,	L"胶痕数量",		        L"JIAOHSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_WENCSL,		L"蚊虫数量",		        L"WENCSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_MFSL,		L"毛发数量",		        L"MFSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_WZSL,		L"油污数量",		        L"WZSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_S_SSSL,		L"S疏纱数量",		        L"S_SSSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_S_DSSL,		L"S叠纱数量",		        L"S_DSSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_L_SSSL,		L"L疏纱数量",		        L"L_SSSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_L_DSSL,		L"L叠纱数量",		        L"L_DSSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_QPSL,		L"气泡数量",		        L"QPSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_QXSL,		L"缺陷合计",		        L"QXSL",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_SCORE,		L"扣分",			        L"SCORE",		L"",			        2,	UA_TYPES_INT16,	 0 },
		{ FIELD_P_FLAG,		L"批次数据刷新标志",       L"P_FLAG",		L"",			        1,	UA_TYPES_BOOLEAN,0 },


		{ FIELD_QXMS,		L"缺陷米數",		        L"QXMS",		L"",					4,	UA_TYPES_FLOAT,	 0 },
		{ FIELD_QXDM,		L"缺陷代碼",		        L"QXDM",		L"",					40,	UA_TYPES_STRING, 0 },
		{ FIELD_QXMJ,		L"缺陷面積",		        L"QXMJ",		L"",					4,	UA_TYPES_FLOAT,	 0 },
		{ FIELD_QXCD,		L"缺陷長度",		        L"QXCD",		L"",					4,	UA_TYPES_FLOAT,	 0 },
		{ FIELD_QXKD,		L"缺陷寬度",		        L"QXKD",		L"",					4,	UA_TYPES_FLOAT,	 0 },
		{ FIELD_QX_X,		L"缺陷X軸位置",		    L"QX_X",		L"",					4,	UA_TYPES_FLOAT,	 0 },
		{ FIELD_QX_Y,		L"缺陷Y軸位置",		    L"QX_Y",		L"",					4,	UA_TYPES_FLOAT,	 0 },
		{ FIELD_ZFMWZ,		L"正反面位置",		    L"ZFMWZ",		L"",					4,	UA_TYPES_STRING, 0 },
		{ FIELD_CS,			L"車速",			        L"CS",			L"",					4,	UA_TYPES_FLOAT,	 0 },
		{ FIELD_Z_QXFLAG,	L"缺陷刷新标志",	        L"Z_QXFLAG",	L"",					1,	UA_TYPES_BOOLEAN,0 },
#endif
	};

	m_ppFIELD_INFO = new SubscribeNodeItem*[FIELD_MAX];
	for (int i = 0; i < FIELD_MAX; i++){
		m_ppFIELD_INFO[i] = new SubscribeNodeItem;
		memset(m_ppFIELD_INFO[i], 0, sizeof(SubscribeNodeItem));

		//memcpy(m_ppFIELD_INFO[i], &ctSYST_FIELD[i], sizeof(NodeItem));
		m_ppFIELD_INFO[i]->nFieldId = ctSYST_FIELD[i].nFieldId;
		memcpy(m_ppFIELD_INFO[i]->strName, ctSYST_FIELD[i].strName, sizeof(ctSYST_FIELD[i].strName));
		memcpy(m_ppFIELD_INFO[i]->strNodeId, ctSYST_FIELD[i].strNodeId, sizeof(ctSYST_FIELD[i].strNodeId));
		m_ppFIELD_INFO[i]->nLen = ctSYST_FIELD[i].nLen;
		m_ppFIELD_INFO[i]->nType = ctSYST_FIELD[i].nType;
		m_ppFIELD_INFO[i]->xTime = ctSYST_FIELD[i].xTime;

		m_ppFIELD_INFO[i]->pValue = new BYTE[ctSYST_FIELD[i].nLen];
		memset(m_ppFIELD_INFO[i]->pValue, 0, ctSYST_FIELD[i].nLen);
#ifdef _DEBUG
		switch (ctSYST_FIELD[i].nType){
		case UA_TYPES_STRING:
			memcpy(m_ppFIELD_INFO[i]->pValue, ctSYST_FIELD[i].strValue, ctSYST_FIELD[i].nLen);
			break;
		case UA_TYPES_BOOLEAN:
			*(bool*)m_ppFIELD_INFO[i]->pValue = _ttoi(ctSYST_FIELD[i].strValue);
			break;
		case UA_TYPES_UINT16:
			*(short*)m_ppFIELD_INFO[i]->pValue = _ttoi(ctSYST_FIELD[i].strValue);
			break;
		}
#endif
	}
	//load AA.ini for defectid and opc field mapping
	LoadYJD();
	IninOPCClient();
}
///<summary>初始化項目</summary>
void CSystDONGGUANPPProcess::LoadYJD()
{
	if (ctYJDFiles.size() > 0)//找檔案且size>0(有內容)
	{
		CString strFolder = GetYJDFolder(), strPath;//檔案路徑
		strPath.Format(L"%s\\%s", strFolder, ctYJDFiles.at(0));
		if (PathFileExists(strPath)) //若存在
		{
			CString strSection, strTemp;
			WCHAR szData[MAX_PATH] = { NULL };
			for (int nIndex = 0;; nIndex++)
			{
				strSection.Format(L"%s_%d", INI_SECTION_ITEM, nIndex);
				if (::GetPrivateProfileString(strSection, INI_FIELD_OPC_FIELD, L"", szData, MAX_PATH, strPath))
				{
					CString strOPCField = szData;
					m_mapOPCField[strOPCField] = 0;
					::GetPrivateProfileString(strSection, INI_FIELD_DEFECT_ID, L"", szData, MAX_PATH, strPath);
					m_mapOPCField[strOPCField] = _ttoi(szData);
				}
				else
					break;
			}
		}
	}
}
///<summary>關閉程式處置參數</summary>
void CSystDONGGUANPPProcess::Finalize()
{
	::SetEvent(m_hEvent[EV_EXIT]);

	::WaitForSingleObject(m_hThread, INFINITE);

	int nFieldSize = GetNodeSize();//FIELD_MAX

	if (m_ppFIELD_INFO)//SubscribeNodeItem訂閱存在
	{
		for (int i = 0; i < nFieldSize; i++)
		{
			if (m_ppFIELD_INFO[i])
			{
				//刪除訂閱
				delete[] m_ppFIELD_INFO[i]->pValue;
				delete[] m_ppFIELD_INFO[i];
				m_ppFIELD_INFO[i] = NULL;
			}
		}
		delete[] m_ppFIELD_INFO;
	}
#ifdef WRITE_OPCFIELD_THREAD
	DeleteCriticalSection(&m_xLock);
	DeleteCriticalSection(&m_xBatch);
#endif
}
///<summary>取得節點項目</summary>
NodeItem *CSystDONGGUANPPProcess::GetNodeItem(int nIndex0Base)
{
	if (nIndex0Base >= 0 && nIndex0Base < FIELD_MAX)
	{
		return m_ppFIELD_INFO[nIndex0Base];//返回訂閱節點
	}
	return NULL;
}
///<summary>設定監視ID</summary>
void CSystDONGGUANPPProcess::SET_MONITOR_ID(CString strKey, UA_NodeId xNodeId, int nMonId)
{
	int nFieldSize = GetNodeSize();//FIELD_MAX
	if (m_ppFIELD_INFO)//SubscribeNodeItem訂閱存在
	{
		for (int i = 0; i < nFieldSize; i++)
		{
			if (m_ppFIELD_INFO[i])//內部對應訂閱
			{
				if (CString(m_ppFIELD_INFO[i]->strNodeId) == strKey)//Key相同將缺少的監視相關值補入訂閱項中
				{
					m_ppFIELD_INFO[i]->nMonId = nMonId;
					m_ppFIELD_INFO[i]->xNodeId = xNodeId;
					break;
				}
			}
		}
	}
}
///<summary>AOI資料接收觸發</summary>
void CSystDONGGUANPPProcess::ON_RECEIVE_AOIDATA(OPCDataType eType)
{
	switch (eType)
	{
		case OPCDataType::INSP:
		{
			OPCInspData xData;
			memset(&xData, 0, sizeof(OPCInspData));
			if (USM_ReadAOIData((BYTE*)&xData, sizeof(OPCInspData)))
			{
#ifdef			WRITE_OPCFIELD_THREAD
				EnterCriticalSection(&m_xLock);
				m_vInspData.push_back(xData);
				LeaveCriticalSection(&m_xLock);
				::SetEvent(m_hEvent[EV_INSP]);
#else
				m_vInspData.push_back(xData);
				ProcessInspData();
#endif
			}
		}
		break;
	case OPCDataType::BATCH:
		{
			OPCBatchData xData;
			memset(&xData, 0, sizeof(OPCBatchData));
			if (USM_ReadAOIData((BYTE*)&xData, sizeof(OPCBatchData)))
			{
#ifdef			WRITE_OPCFIELD_THREAD
				EnterCriticalSection(&m_xBatch);
				m_vBatchData.push_back(xData);
				LeaveCriticalSection(&m_xBatch);
				::SetEvent(m_hEvent[EV_NEWBATCH_DONE]);
#else			
				m_vBatchData.push_back(xData);
				ProcessNewBatchDone();
#endif
			}
		}
		break;
	}
	NotifyAOI(WM_OPC_FINISHWRITE_CMD, eType);//ShareMemory由視窗給AOI
}
///<summary>AOI資料接收觸發</summary>
BOOL CSystDONGGUANPPProcess::GetUANodeId(int nField, UA_NodeId& xNodeId)
{
	int nFieldSize = GetNodeSize();//FIELD_MAX
	if (m_ppFIELD_INFO)//若存在
	{
		for (int i = 0; i < nFieldSize; i++)
		{
			if (i == nField && m_ppFIELD_INFO[i])//
			{
				xNodeId = m_ppFIELD_INFO[i]->xNodeId;
				return TRUE;
			}
		}
		/*
		if(nField>=0&&nField<nFieldSize)
		{
			if (m_ppFIELD_INFO[nField])
			{
				xNodeId = m_ppFIELD_INFO[i]->xNodeId;
				return TRUE;
			}
		}
		*/

	}
	return FALSE;
}
///<summary>更新NodeValue資料</summary>
void CSystDONGGUANPPProcess::UpdateNode(int nMonId, const UA_DataValue *pValue)
{
	int nTypeKind = pValue->value.type->typeKind;//取得欲鍵入資料格式(type)
	int nFieldSize = GetNodeSize();//FIELD_MAX
	if (m_ppFIELD_INFO)//訂閱資料存在
	{
		for (int i = 0; i < nFieldSize; i++)
		{
			if (m_ppFIELD_INFO[i])//單筆監控ID存在
			{
				if (m_ppFIELD_INFO[i]->nMonId == nMonId)//找到欲修改的監控ID
				{
					SubscribeNodeItem* pItem = m_ppFIELD_INFO[i];
					switch (nTypeKind)
					{
						case UA_DATATYPEKIND_STRING://String
						{
							UA_String uaData = *((UA_String*)pValue->value.data);
							CString strData((char *)uaData.data, (int)uaData.length);
							CString strOld((TCHAR*)pItem->pValue);

							if (strOld != strData)
							{
								memset(pItem->pValue, 0, pItem->nLen);

								int nCopySize = (strData.GetLength() + 1) * sizeof(TCHAR);
								if (nCopySize > pItem->nLen) nCopySize = pItem->nLen;

								memcpy(pItem->pValue, strData.GetBuffer(), nCopySize);
								TRACE(L"ServerCallback uaData.length=%i, Data:%s \n", uaData.length, strData);

								if (pItem->nFieldId == FIELD_CHARG)
								{
									theApp.InsertDebugLog(L"Order No change! wait 3 sec to new batch", LOG_OPC); //wait for other field get ready
									::SetEvent(m_hEvent[EV_NEWBATCH]);
								}
							}
						}
							break;
						case UA_DATATYPEKIND_FLOAT://Float
						{
							UA_Float fData = *(UA_Float *)pValue->value.data;
							*(float*)pItem->pValue = fData;
						}
							break;
						case UA_DATATYPEKIND_BOOLEAN://Bool
						{
							UA_Boolean bData = *(UA_Boolean *)pValue->value.data;
#ifdef WRITE_OPCFIELD_THREAD
							if (pItem->nFieldId == FIELD_Z_QXFLAG)
							{ // mark flag in thread. prevent opc error
								::SetEvent(m_hEvent[bData ? EV_INSP_TRUE : EV_INSP_FALSE]);
							}
							else if (pItem->nFieldId == FIELD_P_FLAG)
							{// mark flag in thread. prevent opc error
								::SetEvent(m_hEvent[bData ? EV_NEWBATCH_FALG_TRUE : EV_NEWBATCH_FALG_FALSE]);
							}
							else
							{
								*(bool*)pItem->pValue = bData;
							}
#else
							*(bool*)pItem->pValue = bData;
							if (!bData)
							{
								switch (pItem->nFieldId)
								{
									case FIELD_Z_QXFLAG:
										ProcessInspData();
										break;
									case FIELD_P_FLAG:
										ProcessNewBatchDone();
										break;
								}
							}
#endif
						}
							break;
						case UA_DATATYPEKIND_INT16://Int
						{
							UA_Int16 nData = *(UA_Int16 *)pValue->value.data;
							*(int*)pItem->pValue = nData;
						}
							break;
						default:
							ASSERT(FALSE);
							break;
					}

					pItem->xTime = CTime::GetCurrentTime().GetTime();
					ON_OPC_FIELD_CHANGE(pItem->nFieldId); //notify UI change

					CString strLog;
					strLog.Format(L"Field:%s change to : %s", GetNodeId(pItem->nFieldId), GetNodeValue(pItem->nFieldId));
					theApp.InsertDebugLog(strLog, LOG_OPC);
					break;
				}
			}
		}
	}
}
#ifdef WRITE_OPCFIELD_THREAD
void CSystDONGGUANPPProcess::MarkInspFlag(bool bValue)
{
	*(bool*)m_ppFIELD_INFO[FIELD_Z_QXFLAG]->pValue = bValue;
	ON_OPC_FIELD_CHANGE(FIELD_Z_QXFLAG); //notify UI change
	if (!bValue){ //check if there's another data to write
		::SetEvent(m_hEvent[EV_INSP]);
	}
}
void CSystDONGGUANPPProcess::MarkNewbatchFlag(bool bValue)
{
	*(bool*)m_ppFIELD_INFO[FIELD_P_FLAG]->pValue = bValue;
	ON_OPC_FIELD_CHANGE(FIELD_P_FLAG); //notify UI change
	if (!bValue){ //check if there's another data to write
		::SetEvent(m_hEvent[EV_NEWBATCH_DONE]);
	}
}
#endif
///<summary>處理新工單資料</summary>
void CSystDONGGUANPPProcess::ProcessNewBatch()
{
	OPCNewBatchDongguan xData;
	memset(&xData, 0, sizeof(OPCNewBatchDongguan));
	auto CopyString = [&](TCHAR* pDst, SubscribeNodeItem* pItem)
	{
		memcpy(pDst, pItem->pValue, pItem->nLen);
	};
	//複製所有下發資料
	CopyString(xData.AUFNR,     m_ppFIELD_INFO[FIELD_AUFNR]);
	CopyString(xData.MATNR,     m_ppFIELD_INFO[FIELD_MATNR]);
	xData.MENGE = *(float*)     m_ppFIELD_INFO[FIELD_MENGE]->pValue;//数量(批次米数/张数)有float?
	CopyString(xData.MEINS,     m_ppFIELD_INFO[FIELD_MEINS]);
	CopyString(xData.MACHINE,   m_ppFIELD_INFO[FIELD_MACHINE]);
	CopyString(xData.YJD,       m_ppFIELD_INFO[FIELD_YJD]);
	CopyString(xData.CHARG,     m_ppFIELD_INFO[FIELD_CHARG]);
	CopyString(xData.ZFLAG,     m_ppFIELD_INFO[FIELD_ZFLAG]);
							    
	CopyString(xData.SS_HD,     m_ppFIELD_INFO[FIELD_SS_HD]);
	CopyString(xData.S_HD,      m_ppFIELD_INFO[FIELD_S_HD]);
	CopyString(xData.M_HD,      m_ppFIELD_INFO[FIELD_M_HD]);
	CopyString(xData.L_HD,      m_ppFIELD_INFO[FIELD_L_HD]);
	CopyString(xData.LL_HD,     m_ppFIELD_INFO[FIELD_LL_HD]);
	CopyString(xData.L_ZH,      m_ppFIELD_INFO[FIELD_L_ZH]);
	CopyString(xData.S_ZH,      m_ppFIELD_INFO[FIELD_S_ZH]);
	CopyString(xData.JIAOB,     m_ppFIELD_INFO[FIELD_JIAOB]);
	CopyString(xData.TW,        m_ppFIELD_INFO[FIELD_TW]);
	CopyString(xData.S_JL,      m_ppFIELD_INFO[FIELD_S_JL]);
	CopyString(xData.L_JL,      m_ppFIELD_INFO[FIELD_L_JL]);
	CopyString(xData.QSZ,       m_ppFIELD_INFO[FIELD_QSZ]);
	CopyString(xData.S_ZK,      m_ppFIELD_INFO[FIELD_S_ZK]);
	CopyString(xData.L_ZK,      m_ppFIELD_INFO[FIELD_L_ZK]);
	CopyString(xData.LG,        m_ppFIELD_INFO[FIELD_LG]);
	CopyString(xData.Z_TW,      m_ppFIELD_INFO[FIELD_Z_TW]);
	CopyString(xData.H_TW,      m_ppFIELD_INFO[FIELD_H_TW]);
	CopyString(xData.JIAOH,     m_ppFIELD_INFO[FIELD_JIAOH]);
	CopyString(xData.WENC,      m_ppFIELD_INFO[FIELD_WENC]);
	CopyString(xData.MF,        m_ppFIELD_INFO[FIELD_MF]);
	CopyString(xData.WZ,        m_ppFIELD_INFO[FIELD_WZ]);
	CopyString(xData.S_SS,      m_ppFIELD_INFO[FIELD_S_SS]);
	CopyString(xData.S_DS,      m_ppFIELD_INFO[FIELD_S_DS]);
	CopyString(xData.L_SS,      m_ppFIELD_INFO[FIELD_L_SS]);
	CopyString(xData.L_DS,      m_ppFIELD_INFO[FIELD_L_DS]);
	CopyString(xData.QP,        m_ppFIELD_INFO[FIELD_QP]);

	//log file, keep 500 latest files
	LogNewbatchData(xData);

	//Save R3 for master generate lst header
	SaveR3Data(xData);

	if (USM_WriteData((BYTE*)&xData, sizeof(xData)))
	{
		NotifyAOI(WM_OPC_NEWBATCH_CMD, 1/*0:九江;1:松八*/);//Shared memory=>AOI通知
	}
	else
	{
		theApp.InsertDebugLog(L"Write Shared memory fail!", LOG_OPC);//Shared memory=>AOI通知 Fail
	}
}
///<summary>取指定檔案夾</summary>
CString CSystDONGGUANPPProcess::GetYJDFolder()
{
	CString strRtn;
	static const CString ctFOLDER_YJD = L"SYST_DATA";
	TCHAR workingDir[MAX_PATH];
#ifdef _UNICODE
	_wgetcwd(workingDir, _MAX_PATH);
#else
	_getcwd(workingDir, _MAX_PATH);
#endif
	//TCHAR parentPath[MAX_PATH];
	//_tcscpy_s(parentPath, workingDir);

	//// Remove the last component (file/folder name) from the path
	//PathRemoveFileSpec(parentPath);
	strRtn.Format(_T("%s\\%s"), workingDir, ctFOLDER_YJD);
	if (!PathFileExists(strRtn))//make sure folder exist 
	{
		SHCreateDirectoryEx(AfxGetMainWnd()->GetSafeHwnd(), strRtn, NULL);
	}
	return strRtn;
}
///<summary>Log新工單資料</summary>
void CSystDONGGUANPPProcess::LogNewbatchData(OPCNewBatchDongguan& xData)
{
	CString strFolder = GetYJDFolder(), strFilePath;
#ifdef _DEBUG
	RemoveOldestFileIfOverLimit(strFolder, 1); //only keep latest 1 files
#else
	RemoveOldestFileIfOverLimit(strFolder, 500); //only keep latest 500 files
#endif
	//...\PLCCommunicator\OPCCommunicator\SYST_DATA\456AAA.ini
	strFilePath.Format(L"%s\\%s%s.ini", 
					   strFolder, //%s 路徑
					   CString(xData.AUFNR, _countof(xData.AUFNR)), //%s 订单号456
					   CString(xData.CHARG, _countof(xData.CHARG)));//%s 批次號AAA
	//------------------------------------------------------------------------------
	///寫入
	FILE *stream = _wfsopen(strFilePath, L"wb+", _SH_DENYRW);
	if (stream)
	{
		WORD wStart = 0xFEFF;
		fwrite(&wStart, 1, sizeof(wStart), stream);//add unicode file header
		fclose(stream);

		::WritePrivateProfileString(INI_SECTION_RULE, INI_FIELD_NAME, CString(xData.YJD, _countof(xData.YJD)), strFilePath);
		static const vector<std::pair<CString, CString>> ctFIELD_NAMES = 
		{
			{ L"SS_HD", L"SS黑點"},
			{ L"S_HD",  L"S黑點" },
			{ L"M_HD",  L"M黑點" },
			{ L"L_HD",  L"L黑點" },
			{ L"LL_HD", L"LL黑點"},
			{ L"L_ZH",  L"L折痕" },
			{ L"S_ZH",  L"S折痕" },
			{ L"JIAOB", L"膠斑"  },
			{ L"TW",    L"條紋"  },
			{ L"S_JL",  L"S膠粒" },
			{ L"L_JL",  L"L膠粒" },
			{ L"QSZ",   L"缺樹脂" },
			{ L"S_ZK",  L"S針孔" },
			{ L"L_ZK",  L"L針孔" },
			{ L"LG",    L"流掛"  },
			{ L"Z_TW",  L"棕色條紋" },
			{ L"H_TW",  L"黑色條紋" },
			{ L"JIAOH", L"膠痕" },
			{ L"WENC",  L"蚊蟲" },
			{ L"MF",    L"毛髮" },
			{ L"WZ",    L"油污" },
			{ L"S_SS",  L"S疏紗"},
			{ L"S_DS",  L"S疊紗"},
			{ L"L_SS",  L"L疏紗"},
			{ L"L_DS",  L"L疊紗"},
			{ L"QP",    L"氣泡" }
		};
		int nIndex = 0;
		CString strSection;
		TCHAR* pCur = xData.SS_HD;
		int nLen = _countof(xData.SS_HD);
		for (auto& xField : ctFIELD_NAMES)
		{
			strSection.Format(L"%s_%d", INI_SECTION_ITEM, nIndex++);//[ITEM_0]
			::WritePrivateProfileString(strSection, INI_FIELD_OPC_FIELD, xField.first, strFilePath);
			::WritePrivateProfileString(strSection, INI_FIELD_NAME, xField.second, strFilePath);
			::WritePrivateProfileString(strSection, INI_FIELD_SCORE, CString(pCur, nLen), strFilePath);
			if (m_mapOPCField.find(xField.first) != m_mapOPCField.end()){
				CString strID;
				strID.Format(L"%d", m_mapOPCField[xField.first]);
				::WritePrivateProfileString(strSection, INI_FIELD_DEFECT_ID, strID, strFilePath);
			}
			pCur += nLen;
		}
	}
}
///<summary>Log新工單資料</summary>
void CSystDONGGUANPPProcess::SaveR3Data(OPCNewBatchDongguan& xData)
{
#define INI_R3DATA L"_R3DATA.txt"
	CString strFolder = GetYJDFolder(), strFilePath;
	strFilePath.Format(L"%s\\%s", strFolder, INI_R3DATA);
	FILE* stream = _wfsopen(strFilePath, L"ab", _SH_DENYRW);
	if (stream) {
		fwprintf(stream, L"%s,%s,%s,%.2f,%s\r\n", CString(xData.AUFNR, _countof(xData.AUFNR))
			, CString(xData.CHARG, _countof(xData.CHARG))
			, CString(xData.MATNR, _countof(xData.MATNR))
			, xData.MENGE
			, CString(xData.MEINS, _countof(xData.MEINS))
			);

		fclose(stream);
	}
}
///<summary>Log新工單資料</summary>
void RemoveOldestFileIfOverLimit(const CString& folderPath, int limit) {
	CFileFind finder;
	CString searchPath = folderPath + "\\*.*";
	BOOL bWorking = finder.FindFile(searchPath);

	int fileCount = 0;
	CString oldestFileName;
	CTime oldestFileTime = 0;

	while (bWorking)
	{
		bWorking = finder.FindNextFile();

		if (!finder.IsDots() && !finder.IsDirectory())
		{
			CString strFileName = finder.GetFileName();
			BOOL bSkip = FALSE;
			for (auto& xReserveFile : ctYJDFiles)
			{//保留檔案不移除
				if (strFileName == xReserveFile)
				{
					bSkip = TRUE;
					break;
				}
			}
			if (!bSkip){
				if (oldestFileTime == 0)
				{
					finder.GetLastWriteTime(oldestFileTime);
					oldestFileName = finder.GetFilePath();
				}

				fileCount++;

				if (fileCount > limit || (fileCount == limit && finder.GetFilePath() != oldestFileName))
				{
					CTime fileTime;
					finder.GetLastWriteTime(fileTime);

					if (fileTime < oldestFileTime)
					{
						oldestFileTime = fileTime;
						oldestFileName = finder.GetFilePath();
					}
				}
			}
		}
	}

	finder.Close();

	if (fileCount > limit && !oldestFileName.IsEmpty()) {
		CFile::Remove(oldestFileName);
		CString strLog;
		strLog.Format(_T("Removed oldest file: %s"), oldestFileName);
		theApp.InsertDebugLog(strLog, LOG_OPC);
	}
}
void CSystDONGGUANPPProcess::ProcessInspData()
{
	OPCInspData* pData = NULL;
	static OPCInspData xData;

	bool bFlag = *(bool*)m_ppFIELD_INFO[FIELD_Z_QXFLAG]->pValue; //false: ready to write

	if (!bFlag){
#ifdef WRITE_OPCFIELD_THREAD
		EnterCriticalSection(&m_xLock);
#endif
		int nCmdSize = m_vInspData.size();
		if (nCmdSize) {
			xData = m_vInspData.at(0);
			m_vInspData.erase(m_vInspData.begin());

			pData = &xData;
		}
#ifdef WRITE_OPCFIELD_THREAD
		LeaveCriticalSection(&m_xLock);
#endif

		if (pData) {
			WriteFloat(FIELD_QXMS, xData.QXMS);
			WriteString(FIELD_QXDM, xData.QXDM, _countof(xData.QXDM));
			WriteFloat(FIELD_QXMJ, xData.QXMJ);
			WriteFloat(FIELD_QXCD, xData.QXCD);
			WriteFloat(FIELD_QXKD, xData.QXKD);
			WriteFloat(FIELD_QX_X, xData.QX_X);
			WriteFloat(FIELD_QX_Y, xData.QX_Y);
			WriteString(FIELD_ZFMWZ, xData.ZFMWZ, _countof(xData.ZFMWZ));
			WriteFloat(FIELD_CS, xData.CS);

			UA_NodeId xNode;//mark flag to true
			if (GetUANodeId(FIELD_Z_QXFLAG, xNode)){
				bool bValue = true;
				if (WriteBOOLField(xNode, bValue) == UA_STATUSCODE_GOOD){
					*(bool*)m_ppFIELD_INFO[FIELD_Z_QXFLAG]->pValue = bValue; //prevent opc transfer delay, mark flag if function return true
				}
			}
		}
	}
}

void CSystDONGGUANPPProcess::ProcessNewBatchDone()
{
	OPCBatchData* pData = NULL;
	static OPCBatchData xData;

	bool bFlag = *(bool*)m_ppFIELD_INFO[FIELD_P_FLAG]->pValue; //false: ready to write

	if (!bFlag){
#ifdef WRITE_OPCFIELD_THREAD
		EnterCriticalSection(&m_xBatch);
#endif
		int nCmdSize = m_vBatchData.size();
		if (nCmdSize) {
			xData = m_vBatchData.at(0);
			m_vBatchData.erase(m_vBatchData.begin());

			pData = &xData;
		}
#ifdef WRITE_OPCFIELD_THREAD
		LeaveCriticalSection(&m_xBatch);
#endif
		if (pData) {
#ifdef WRITE_OPCFIELD_THREAD
			EnterCriticalSection(&m_xLock); //new batch後刪除尚未完成上傳的檢測資料
#endif
			int nSize = m_vInspData.size();
			if (nSize){
				CString strLog;
				strLog.Format(L"warning!Insp data size %d, drop all data", nSize);
				theApp.InsertDebugLog(strLog, LOG_OPC);
				m_vInspData.clear();
			}
#ifdef WRITE_OPCFIELD_THREAD
			LeaveCriticalSection(&m_xLock);
#endif
			WriteString(FIELD_ZAUFNR, xData.ZAUFNR, _countof(xData.ZAUFNR));
			WriteString(FIELD_ZMATNR, xData.ZMATNR, _countof(xData.ZMATNR));
			WriteString(FIELD_ZCHARG, xData.ZCHARG, _countof(xData.ZCHARG));
			WriteString(FIELD_STARTDATE, xData.strStartDate, _countof(xData.strStartDate));
			WriteString(FIELD_STARTTIME, xData.strStartTime, _countof(xData.strStartTime));
			WriteString(FIELD_ENDDATE, xData.strEndDate, _countof(xData.strEndDate));
			WriteString(FIELD_ENDTIME, xData.strEndTime, _countof(xData.strEndTime));

			WriteFloat(FIELD_ZMENGE, xData.ZMENGE);
			WriteString(FIELD_ZMEINS, xData.ZMEINS, _countof(xData.ZMEINS));
			WriteFloat(FIELD_NJPKD, xData.NJPKD);
			WriteFloat(FIELD_MS, xData.MS);
			WriteInt(FIELD_SS_HDSL, xData.SS_HDSL);
			WriteInt(FIELD_S_HDSL, xData.S_HDSL);
			WriteInt(FIELD_M_HDSL, xData.M_HDSL);
			WriteInt(FIELD_L_HDSL, xData.L_HDSL);
			WriteInt(FIELD_LL_HDSL, xData.LL_HDSL);
			WriteInt(FIELD_L_ZHSL, xData.L_ZHSL);
			WriteInt(FIELD_S_ZHSL, xData.S_ZHSL);
			WriteInt(FIELD_JIAOBSL, xData.JIAOBSL);
			WriteInt(FIELD_TWSL, xData.TWSL);
			WriteInt(FIELD_S_JLSL, xData.S_JLSL);
			WriteInt(FIELD_L_JLSL, xData.L_JLSL);
			WriteInt(FIELD_QSZSL, xData.QSZSL);
			WriteInt(FIELD_S_ZKSL, xData.S_ZKSL);
			WriteInt(FIELD_L_ZKSL, xData.L_ZKSL);
			WriteInt(FIELD_LGSL, xData.LGSL);
			WriteInt(FIELD_Z_TWSL, xData.Z_TWSL);
			WriteInt(FIELD_H_TWSL, xData.H_TWSL);
			WriteInt(FIELD_JIAOHSL, xData.JIAOHSL);
			WriteInt(FIELD_WENCSL, xData.WENCSL);
			WriteInt(FIELD_MFSL, xData.MFSL);
			WriteInt(FIELD_WZSL, xData.WZSL);
			WriteInt(FIELD_S_SSSL, xData.S_SSSL);
			WriteInt(FIELD_S_DSSL, xData.S_DSSL);
			WriteInt(FIELD_L_SSSL, xData.L_SSSL);
			WriteInt(FIELD_L_DSSL, xData.L_DSSL);
			WriteInt(FIELD_QPSL, xData.QPSL);
			WriteInt(FIELD_QXSL, xData.QXSL);
			WriteInt(FIELD_SCORE, xData.SCORE);


			UA_NodeId xNode;//mark flag to true
			if (GetUANodeId(FIELD_P_FLAG, xNode)){
				bool bValue = true;
				if (WriteBOOLField(xNode, bValue) == UA_STATUSCODE_GOOD){
					*(bool*)m_ppFIELD_INFO[FIELD_P_FLAG]->pValue = bValue; //prevent opc transfer delay, mark flag if function return true
				}
			}
		}
	}

}
DWORD CSystDONGGUANPPProcess::Thread_Process(void* pvoid)
{
	CSystDONGGUANPPProcess* pThis = (CSystDONGGUANPPProcess*)pvoid;
	BOOL              bRun = TRUE;
	while (bRun)
	{
		switch (::WaitForMultipleObjects(EV_COUNT, pThis->m_hEvent, FALSE, INFINITE))
		{
		case CASE_EXIT:
		{
			bRun = FALSE;
		}
		break;
		case CASE_NEWBATCH:
		{
			::ResetEvent(pThis->m_hEvent[EV_NEWBATCH]);
			::Sleep(3000); //wait for 3 sec

			pThis->ProcessNewBatch();
		}
		break;
#ifdef WRITE_OPCFIELD_THREAD
		case CASE_INSP:
			::ResetEvent(pThis->m_hEvent[EV_INSP]);

			pThis->ProcessInspData();
			break;
		case CASE_INSP_TRUE:
			::ResetEvent(pThis->m_hEvent[EV_INSP_TRUE]);
			pThis->MarkInspFlag(true);
			break;

		case CASE_INSP_FALSE:
			::ResetEvent(pThis->m_hEvent[EV_INSP_FALSE]);
			pThis->MarkInspFlag(false);
			break;
		case CASE_NEWBATCH_DONE:
			::ResetEvent(pThis->m_hEvent[EV_NEWBATCH_DONE]);
			pThis->ProcessNewBatchDone();
			break;
		case CASE_NEWBATCH_FALG_TRUE:
			::ResetEvent(pThis->m_hEvent[EV_NEWBATCH_FALG_TRUE]);
			pThis->MarkNewbatchFlag(true);
			break;
		case CASE_NEWBATCH_FALG_FALSE:
			::ResetEvent(pThis->m_hEvent[EV_NEWBATCH_FALG_FALSE]);
			pThis->MarkNewbatchFlag(false);
			break;
#endif
		}
	}
	return NULL;
}