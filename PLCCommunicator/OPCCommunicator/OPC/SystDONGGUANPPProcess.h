#pragma once
#include "OPCClientController.h"
#include <map>

//#define WRITE_OPCFIELD_THREAD //在不同thread回寫OPC欄位. 目前測試OPC元件不支援此操作
class CSystDONGGUANPPProcess :public COPCClientController
{
public:

	CSystDONGGUANPPProcess();
	virtual ~CSystDONGGUANPPProcess();

protected:
	///<summary>專案節點列舉</summary>
	enum OPC_FIELD_
	{
		//下發
		FIELD_AUFNR = 0,		        //订单号	
		FIELD_MATNR,			        //物料
		FIELD_MENGE,			        //数量(批次米数/张数)
		FIELD_MEINS,			        //单位
		FIELD_MACHINE,			        //机台号	
		FIELD_JSPF,				        //胶水型号	
		FIELD_BLBFPH,			        //玻璃布发票号	
		FIELD_BLBBZ,			        //玻璃布布种	
		FIELD_YJD,				        //严谨度
		FIELD_CHARG,			        //批次號
		FIELD_ZFLAG,			        //有标无标
		FIELD_SS_HD,				    //SS黑点
		FIELD_S_HD,					    //S黑点
		FIELD_M_HD,					    //M黑点
		FIELD_L_HD,					    //L黑点
		FIELD_LL_HD,				    //LL黑点
		FIELD_L_ZH,					    //L折痕
		FIELD_S_ZH,					    //S折痕
		FIELD_JIAOB,				    //胶斑
		FIELD_TW,					    //条纹
		FIELD_S_JL,					    //S胶粒
		FIELD_L_JL,					    //L胶粒
		FIELD_QSZ,					    //缺树脂
		FIELD_S_ZK,					    //S针孔
		FIELD_L_ZK,					    //L针孔
		FIELD_LG,					    //流挂
		FIELD_Z_TW,					    //棕色条纹
		FIELD_H_TW,					    //黑色条纹
		FIELD_JIAOH,				    //胶痕
		FIELD_WENC,					    //蚊虫
		FIELD_MF,					    //毛发
		FIELD_WZ,					    //油污
		FIELD_S_SS,					    //S疏纱
		FIELD_S_DS,					    //S叠纱
		FIELD_L_SS,					    //L疏纱
		FIELD_L_DS,					    //L叠纱
		FIELD_QP,					    //气泡

		//上傳
		FIELD_ZAUFNR,					//订单号
		FIELD_ZMATNR,					//物料
		FIELD_ZCHARG,					//CCD批号(流水号)
		FIELD_STARTDATE,				//CCD開始日期
		FIELD_STARTTIME,				//CCD開始時間
		FIELD_ENDDATE,					//CCD結束日期
		FIELD_ENDTIME,					//CCD結束時間
		FIELD_ZMENGE,					//批次数量（R3卷状米数）
		FIELD_ZMEINS,					//单位（PC/M）R3片状张数
		FIELD_NJPKD,					//粘结片宽度
		FIELD_MS,						//每卷米数（CCD实际米数）
		FIELD_SS_HDSL,					//SS黑点数量
		FIELD_S_HDSL,					//S黑点数量
		FIELD_M_HDSL,					//M黑点数量
		FIELD_L_HDSL,					//L黑点数量
		FIELD_LL_HDSL,					//LL黑点数量
		FIELD_L_ZHSL,					//L折痕数量
		FIELD_S_ZHSL,					//S折痕数量
		FIELD_JIAOBSL,					//胶斑数量
		FIELD_TWSL,						//条纹数量
		FIELD_S_JLSL,					//S胶粒数量
		FIELD_L_JLSL,					//L胶粒数量
		FIELD_QSZSL,					//缺树脂数量
		FIELD_S_ZKSL,					//S针孔数量
		FIELD_L_ZKSL,					//L针孔数量
		FIELD_LGSL,						//流挂数量
		FIELD_Z_TWSL,					//棕色条纹数量
		FIELD_H_TWSL,					//黑色条纹数量
		FIELD_JIAOHSL,					//胶痕数量
		FIELD_WENCSL,					//蚊虫数量
		FIELD_MFSL,						//毛发数量
		FIELD_WZSL,						//油污数量
		FIELD_S_SSSL,					//S疏纱数量
		FIELD_S_DSSL,					//S叠纱数量
		FIELD_L_SSSL,					//L疏纱数量
		FIELD_L_DSSL,					//L叠纱数量
		FIELD_QPSL,						//气泡数量
		FIELD_QXSL,						//缺陷合计
		FIELD_SCORE,					//扣分
		FIELD_P_FLAG,					//批次数据刷新标志

		//瑕疵
		FIELD_QXMS,						//缺陷米數
		FIELD_QXDM,						//缺陷代碼
		FIELD_QXMJ,						//缺陷面積
		FIELD_QXCD,						//缺陷長度
		FIELD_QXKD,						//缺陷寬度
		FIELD_QX_X,						//缺陷X軸位置
		FIELD_QX_Y,						//缺陷Y軸位置
		FIELD_ZFMWZ,					//正反面位置
		FIELD_CS,						//車速
		FIELD_Z_QXFLAG,					//缺陷刷新标志
		FIELD_MAX,                      //全域最大值(enum最後值)
	};
	///<summary>回傳本專案節點最大值</summary>
	virtual int GetNodeSize(){ return FIELD_MAX; };
	///<summary>取得節點項目</summary>
	virtual NodeItem *GetNodeItem(int nIndex0Base);
	///<summary>更新NodeValue資料</summary>
	virtual void UpdateNode(int nMonId, const UA_DataValue *pValue);
	///<summary>設定監視ID</summary>
	virtual void SET_MONITOR_ID(CString strKey, UA_NodeId xNodeId, int nSubId);
	///<summary>AOI資料接收觸發</summary>
	virtual void ON_RECEIVE_AOIDATA(OPCDataType eType);
private:
	///<summary>初始化項目</summary>
	void Init();
	///<summary>關閉程式處置參數</summary>
	void Finalize();
	///<summary>載入YJD</summary>
	void LoadYJD();
	///<summary>取指定檔案夾</summary>
	CString GetYJDFolder();
	///<summary>將節點加入監控</summary>
	void IninOPCClient();
	///<summary>執行緒處理</summary>
	static DWORD __stdcall Thread_Process(void* pvoid);
	///<summary>處理工單資料</summary>
	void ProcessNewBatch();
	///<summary>處理瑕疵資料</summary>
	void ProcessInspData();
	///<summary>處理工單資料後</summary>
	void ProcessNewBatchDone();
#ifdef WRITE_OPCFIELD_THREAD
	void MarkInspFlag(bool bValue);
	void MarkNewbatchFlag(bool bValue);
#endif
	///<summary>Get節點資料</summary>
	BOOL GetUANodeId(int nField, UA_NodeId& xNodeId);
	///<summary>寫入Float</summary>
	void WriteFloat(int nField, float fData);
	///<summary>寫入int</summary>
	void WriteInt(int nField, int nData);
	///<summary>寫入String</summary>
	void WriteString(int nField, char* pStr, int nLen);
	///<summary>Log工單資料</summary>
	void LogNewbatchData(OPCNewBatchDongguan& xData);
	///<summary>儲存R3</summary>
	void SaveR3Data(OPCNewBatchDongguan& xData);
private:
	enum
	{
		EV_EXIT,
		EV_NEWBATCH,// 下發
#ifdef WRITE_OPCFIELD_THREAD
		EV_INSP,
		EV_INSP_TRUE,
		EV_INSP_FALSE,
		EV_NEWBATCH_DONE,//上傳
		EV_NEWBATCH_FALG_TRUE,
		EV_NEWBATCH_FALG_FALSE,
#endif
		EV_COUNT,
		CASE_EXIT = WAIT_OBJECT_0,
		CASE_NEWBATCH,
#ifdef WRITE_OPCFIELD_THREAD
		CASE_INSP,
		CASE_INSP_TRUE,
		CASE_INSP_FALSE,
		CASE_NEWBATCH_DONE,
		CASE_NEWBATCH_FALG_TRUE,
		CASE_NEWBATCH_FALG_FALSE,
#endif
	};
	///<summary>訂閱節點結構</summary>
	struct SubscribeNodeItem : public NodeItem
	{
		int nMonId;//MonitorId 監視ID
		UA_NodeId xNodeId;//NodeId 節點ID
	};
	///<summary>訂閱節點</summary>
	SubscribeNodeItem **m_ppFIELD_INFO;

	std::map<CString, int> m_mapOPCField;

	HANDLE     m_hThread;
	HANDLE     m_hEvent[EV_COUNT];
#ifdef WRITE_OPCFIELD_THREAD
	CRITICAL_SECTION m_xLock;
	CRITICAL_SECTION m_xBatch;
#endif
	///<summary>[vector]缺陷資料</summary>
	std::vector<OPCInspData> m_vInspData;
	///<summary>[vector]工單資料</summary>
	std::vector<OPCBatchData> m_vBatchData;
};