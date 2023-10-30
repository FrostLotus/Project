#pragma once
#include "open62541/server.h"
#include <vector>
#include "usm.h"
using namespace std;

#define BATCH_OPC2AOI_MEM_ID			_T("BATCH_OPC2AOI_MEM")				//OPC_Communicator->AOI
#define BATCH_AOI2OPC_MEM_ID			_T("BATCH_AOI2OPC_MEM")				//AOI->OPC_Communicator

///<summary>[新]OPC工單資料</summary>
struct OPCNewBatch
{
	TCHAR strOrder_No[12];				//订单号	
	TCHAR strOrder_Material[20];		//订单物料
	short nOrder_Quatity;				//订单数量
	TCHAR strFactory[18];				//工厂		
	TCHAR strStation[10];				//生产机台	
	TCHAR strGlueType[10];				//胶水类型	
	TCHAR strWebSpec[18];				//玻璃布規格	
	TCHAR strWebFactory[10];			//玻璃布廠家
	TCHAR strWebType[10];				//玻璃布布種
	TCHAR strInsp[10];					//檢測設定
	TCHAR strLight[10];					//光源設定
	TCHAR strLotNo[10];					//LotNO
};
///<summary>[新]OPC東莞松八工單資料</summary>
struct OPCNewBatchDongguan
{
	TCHAR AUFNR[10];					//订单号	
	TCHAR MATNR[18];					//物料
	float MENGE;						//数量(批次米数/张数)
	TCHAR MEINS[10];					//单位
	TCHAR MACHINE[10];					//机台号
	TCHAR YJD[10];						//严谨度
	TCHAR CHARG[20];					//批次號
	TCHAR ZFLAG[2];						//有标无标

	TCHAR SS_HD[3];						//SS黑点
	TCHAR S_HD[3];						//S黑点
	TCHAR M_HD[3];						//M黑点
	TCHAR L_HD[3];						//L黑点
	TCHAR LL_HD[3];						//LL黑点
	TCHAR L_ZH[3];						//L折痕
	TCHAR S_ZH[3];						//S折痕
	TCHAR JIAOB[3];						//胶斑
	TCHAR TW[3];						//条纹
	TCHAR S_JL[3];						//S胶粒
	TCHAR L_JL[3];						//L胶粒
	TCHAR QSZ[3];						//缺树脂
	TCHAR S_ZK[3];						//S针孔
	TCHAR L_ZK[3];						//L针孔
	TCHAR LG[3];						//流挂
	TCHAR Z_TW[3];						//棕色条纹
	TCHAR H_TW[3];						//黑色条纹
	TCHAR JIAOH[3];						//胶痕
	TCHAR WENC[3];						//蚊虫
	TCHAR MF[3];						//毛发
	TCHAR WZ[3];						//油污
	TCHAR S_SS[3];						//S疏纱
	TCHAR S_DS[3];						//S叠纱
	TCHAR L_SS[3];						//L疏纱
	TCHAR L_DS[3];						//L叠纱
	TCHAR QP[3];						//气泡
};
///<summary>OPC瑕疵資料</summary>
struct OPCInspData
{
	float QXMS;				//缺陷米數
	char QXDM[20];			//缺陷代碼
	float QXMJ;				//缺陷面積
	float QXCD;				//缺陷長度
	float QXKD;				//缺陷寬度
	float QX_X;				//缺陷X軸位置
	float QX_Y;				//缺陷Y軸位置
	char ZFMWZ[2];			//正反面位置
	float CS;				//車速
};
///<summary>OPC工單資料</summary>
struct OPCBatchData
{
	char ZAUFNR[10];			//訂單號
	char ZMATNR[18];			//物料
	char ZCHARG[20];			//CCD批號
	char strStartDate[10];		//CCD開始日期
	char strStartTime[10];		//CCD開始時間
	char strEndDate[10];		//CCD結束日期
	char strEndTime[10];		//CCD結束時間

	float ZMENGE;				//批次数量（R3卷状米数）
	char ZMEINS[10];			//单位（PC/M）R3片状张数
	float NJPKD;				//粘结片宽度
	float MS;					//每卷米数（CCD实际米数）
	int SS_HDSL;				//SS黑点数量
	int S_HDSL;					//S黑点数量
	int M_HDSL;					//M黑点数量
	int L_HDSL;					//L黑点数量
	int LL_HDSL;				//LL黑点数量
	int L_ZHSL;					//L折痕数量
	int S_ZHSL;					//S折痕数量
	int JIAOBSL;				//胶斑数量
	int TWSL;					//条纹数量
	int S_JLSL;					//S胶粒数量
	int L_JLSL;					//L胶粒数量
	int QSZSL;					//缺树脂数量
	int S_ZKSL;					//S针孔数量
	int L_ZKSL;					//L针孔数量
	int LGSL;					//流挂数量
	int Z_TWSL;					//棕色条纹数量
	int H_TWSL;					//黑色条纹数量
	int JIAOHSL;				//胶痕数量
	int WENCSL;					//蚊虫数量
	int MFSL;					//毛发数量
	int WZSL;					//油污数量
	int S_SSSL;					//S疏纱数量
	int S_DSSL;					//S叠纱数量
	int L_SSSL;					//L疏纱数量
	int L_DSSL;					//L叠纱数量
	int QPSL;					//气泡数量
	int QXSL;					//缺陷合计
	int SCORE;					//扣分
};
///<summary>節點</summary>
struct NodeItem
{
	int nFieldId;
	TCHAR strName[100];//名稱(可中文)
	TCHAR strNodeId[100];//名稱(英數)
	void* pValue;//值
	int nLen; //inBytes
	int nType;//類型
	time_t xTime;
};

class IOPCProcess
{
public:
	IOPCProcess() { m_pOut = NULL; m_pIn = NULL; };
	virtual ~IOPCProcess() { m_pOut = NULL; m_pIn = NULL; };
	void AttachOut(IOPCProcess* pLink) { m_pOut = pLink; };
	void AttachIn(IOPCProcess* pLink) { m_pIn = pLink; };
protected:
	///<summary>OPC資料型態</summary>
	enum OPCDataType
	{
		INSP,//瑕疵資料
		BATCH//工單資料
	};
	//in
	virtual void ON_OPEN_OPC(LPARAM lp)
	{
		if (m_pIn)
			m_pIn->ON_OPEN_OPC(lp);
	}
	virtual void ON_CLOSE_OPC()
	{
		if (m_pIn)
			m_pIn->ON_CLOSE_OPC();
	}
	virtual void ON_RECEIVE_AOIDATA(OPCDataType eType)
	{
		if (m_pIn)
			m_pIn->ON_RECEIVE_AOIDATA(eType);
	}
	//out
	virtual void ON_OPC_NOTIFY(CString strMsg)
	{
		if (m_pOut)
			m_pOut->ON_OPC_NOTIFY(strMsg);
	}
	virtual void ON_OPC_PARAM(CString strName, CString strValue)
	{
		if (m_pOut)
			m_pOut->ON_OPC_PARAM(strName, strValue);
	}
	virtual void ON_OPC_FIELD_CHANGE(int nFieldId)
	{
		if (m_pOut)
			m_pOut->ON_OPC_FIELD_CHANGE(nFieldId);
	}
private:
	IOPCProcess* m_pOut;
	IOPCProcess* m_pIn;
};

class COPCProcessBase : public IOPCProcess
{
public:
	COPCProcessBase();
	virtual ~COPCProcessBase();

	CString GetNodeId(int nIndex0Base);
	CString GetNodeName(int nIndex0Base);
	CString GetNodeValue(int nIndex0Base);
	CString GetNodeTime(int nIndex0Base);
public:
	virtual int GetNodeSize() = 0;
protected:
	virtual NodeItem* GetNodeItem(int nIndex0Base) = 0;

protected:
	BOOL USM_ReadData(BYTE* pData, int nSize, int nOffset = 0);
	BOOL USM_ReadAOIData(BYTE* pData, int nSize, int nOffset = 0);
	BOOL USM_WriteData(BYTE* pData, int nSize, int nOffset = 0);
	void NotifyAOI(WPARAM wp, LPARAM lp);
private:
	void Init();
	void Finalize();
private:
	usm<unsigned char>* m_pOPCUsm;
	usm<unsigned char>* m_pAOIUsm;
};