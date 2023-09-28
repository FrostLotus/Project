#pragma once
#include <vector>
#include <mutex>
#include "BaseClientSocket.h"
//---------------------------------
// Packet Format
//	[HEADER] +[SUBHEADER] +[ACCESS ROUTE]+[REQUEST DATA LEN] + [MONITOR TIMER] + [REQUEST DATA]
//
//
//---------------------------------

#define TIMERID_QUERYCMD			1
#define TIMERID_QUERYRESULT			2
#define TIMERID_QUERYSPLIT			3
#define TIMER_INTERVAL				1000
#define TIMER_RESULT_INTERVAL		1000
using namespace std;

#pragma pack(push, 1)
typedef struct ASCII_3E_HEADER_ITEM_{
	//Subheader
	DWORD dStart;
}ASCII_3E_HEADER_ITEM;
//For 4E Frame
typedef struct ASCII_4E_HEADER_ITEM_{
	//Subheader
	DWORD dStart;
	DWORD dSerial;
	DWORD dReserved;
}ASCII_4E_HEADER_ITEM;

typedef struct ASCII_BODY_ITEM_{
	//Access route
	WORD wNetNo;
	WORD wPcNo;
	DWORD dDstIoNo;
	WORD wDstStationNo;
	//Packet Data Length //Length 24
	BYTE cPacketLen[4];
	union 
	{
		//Wait Read Timer
		BYTE cTimer[4]; //0.5 sec
		//Error Code
		DWORD dErr;
	};
}ASCII_BODY_ITEM;
typedef struct ASCII_MSG_ITEM_{
	//Request Data
	DWORD dCommand;
	DWORD dSubCommand;
	BYTE cDevCode[2];
	BYTE cDevNum[6];
	BYTE cDataLen[4];
}ASCII_MSG_ITEM;
typedef struct ASCII_4E_REQUEST_ITEM_{
	ASCII_4E_HEADER_ITEM xHr;
	ASCII_BODY_ITEM xBody;
	ASCII_MSG_ITEM_ xMsg;
}ASCII_4E_REQUEST_ITEM;
typedef struct ASCII_3E_REQUEST_ITEM_{
	ASCII_3E_HEADER_ITEM xHr;
	ASCII_BODY_ITEM xBody;
	ASCII_MSG_ITEM_ xMsg;
}ASCII_3E_REQUEST_ITEM;

typedef struct BINARY_3E_HEADER_ITEM_{
	//Subheader
	unsigned short wStart;
}BINARY_3E_HEADER_ITEM;
typedef struct BINARY_4E_HEADER_ITEM_{
	//Subheader
	unsigned short wStart;
	unsigned short wSerial;
	unsigned short wReserved;
}BINARY_4E_HEADER_ITEM;

typedef struct BINARY_BODY_ITEM_{
	//Access route
	BYTE cNetNo;
	BYTE cPcNo;
	unsigned short wDstIoNo;
	BYTE cDstStationNo;
	//Packet Data Length
	BYTE cPacketLen[2]; //Length 12
	union {
		//Wait Read Timer
		BYTE cTimer[2]; //0.5 sec
		// Error Code
		unsigned short wError;
	};
}BINARY_BODY_ITEM_;
typedef struct BINARY_MSG_ITEM_{
	//Request Data
	unsigned short wCommand;
	unsigned short wSubCommand;
	BYTE cDevNum[3];
	BYTE cDevCode;
	BYTE cDataLen[2];
}BINARY_MSG_ITEM;
typedef struct BINARY_4E_REQUEST_ITEM_{
	BINARY_4E_HEADER_ITEM xHr;
	BINARY_BODY_ITEM_ xBody;
	BINARY_MSG_ITEM_ xMsg;
}BINARY_4E_REQUEST_ITEM;
typedef struct BINARY_3E_REQUEST_ITEM_{
	BINARY_3E_HEADER_ITEM xHr;
	BINARY_BODY_ITEM_ xBody;
	BINARY_MSG_ITEM_ xMsg;
}BINARY_3E_REQUEST_ITEM;
//---------------------------
#pragma pack(pop)
enum PLC_VALUE_TYPE_{
	PLC_TYPE_STRING,
	PLC_TYPE_WORD,
	PLC_TYPE_FLOAT,
};
enum PLC_ACTION_TYPE_{
	ACTION_SKIP,
	ACTION_NOTIFY,
	ACTION_BATCH,

	ACTION_RESULT,
};

enum PLC_FRAME_TYPE{
	FRAME_3E,
	FRAME_4E,
};

typedef struct PLC_DATA_ITEM_{
public:
	PLC_DATA_ITEM_(){

	}
	PLC_DATA_ITEM_(CString strFieldName, int nFieldType, PLC_VALUE_TYPE_ eValueType, PLC_ACTION_TYPE_ eActionType, BYTE cLen, BYTE cDeviceType, UINT uAddress, UINT uStartBit = -1, UINT uEndBit = -1){
		lstrcpy(this->strFieldName, strFieldName.GetBuffer());
		this->xFieldType = nFieldType;
		this->xValType = eValueType;
		this->xAction = eActionType;
		this->cLen = cLen;
		this->cDevType = cDeviceType;
		this->uAddress = uAddress;
		this->uStartBit = uStartBit;
		this->uEndBit = uEndBit;
	}
	TCHAR strFieldName[100];
	int xFieldType;
	PLC_VALUE_TYPE_ xValType;
	PLC_ACTION_TYPE_ xAction;
	BYTE cLen; //bytes
	BYTE cDevType;
	UINT uAddress;
	UINT uStartBit;
	UINT uEndBit;
}PLC_DATA_ITEM;

class CMelsecPlcSocket :
	public CBaseClientSocket
	, public ISocketCallBack
{
public:
	enum PLC_MODE{
		MODE_ASCII = 0,
		MODE_BINARY,
	};
	enum PLC_DEVICE{ //Only Support Q Series
		DEV_MELSEC_Q_4E = 0,
	};
	enum PLC_NOTIFY_ID_{
		PLC_ERR_NOTIFY = 0,
		PLC_READ_UPDATE,
		PLC_ADDRESS_VAL,
		PLC_WRITE_NOTIFY,
		PLC_WRITE_UPDATE,
		PLC_INFO, //to show debug info
	};
	enum PLC_FIELD_OP_ {
		PLC_FIELD_READ = 0,
		PLC_FIELD_WRITE,
	};
public:
	CMelsecPlcSocket(PLC_FRAME_TYPE eFrameType);
	CMelsecPlcSocket(ISocketCallBack *pParent, PLC_FRAME_TYPE eFrameType);
	~CMelsecPlcSocket();

	template<typename T> void Test(T *pData);
	void SetMode(PLC_MODE xMode) { m_xMode = xMode; };
	PLC_MODE GetMode(){ return m_xMode; };

	void ReadAddress(char cDevType, UINT uAddress, UINT uLenInWord);

	template<typename T> void WriteAddress(char cDevType, UINT uAddress, UINT uLenInWord, T *pWrite);
	template<> void WriteAddress(char cDevType, UINT uAddress, UINT uLen, CString *pWrite);
	CString GET_PLC_FIELD_VALUE(int nFieldId);
	CString GET_PLC_FIELD_ADDRESS(int nFieldId);
	CString GET_PLC_FIELD_NAME(int nFieldId);
	CString GET_PLC_FIELD_TIME(int nFieldId);
	CString GetFieldInfoDes(int nFieldId);
	void GET_PLC_FIELD_DATA(int nFieldId);
	PLC_ACTION_TYPE_ GET_PLC_FIELD_ACTION(int nFieldId);
	void QUERY_ALL_BATCH_INFO();
	//ISocketCallBack
	void ConnStatusCallBack(AOI_SOCKET_STATE xState) {};
	void OnDeviceNotify(int nType, int nVal, CString strDes) {};
	void OnPLCNewBatch(CString strOrder, CString strMaterial){};
	void OnPLCSYSTParam(BATCH_SHARE_SYST_PARAMCCL *pData){};
	void OnC10Change(WORD wC10){};
public:
	virtual void ON_NOTIFY_AOI_RESPONSE(LPARAM lParam){};
	virtual void PushResult(BATCH_SHARE_SYST_RESULTCCL xResult){};
	virtual void SetInfo(BATCH_SHARE_SYST_INFO *pInfo){};
#ifdef SHOW_DEBUG_BTN
	virtual void OnCheckFlushAnyway(BOOL bCheck){};
#endif
	virtual int GetFieldSize() { return 0; };
	virtual PLC_DATA_ITEM_* GetDataItem(int nFieldId) = 0;

protected:
	BYTE ConvertByteToBCD(BYTE cData);
	DWORD ConvertWordToBCD(WORD cData,BOOL bEndianL);
	BYTE ConvertBCDToByte(BYTE cData);
	BYTE ConvertBCDToByte(WORD cData,BOOL bNum);
	WORD ConvertBCDToWORD(DWORD cData,BOOL bNum);
	void SetSubCustomerType(AOI_SUBCUSTOMERTYPE_ eSubCustomerType){ m_eSubCustomerType = eSubCustomerType; };
	AOI_SUBCUSTOMERTYPE_ GetSubCustomerType() { return m_eSubCustomerType; };

protected:
	enum PLC_ACTION_FLAG_{
		REQUEST,
		RESPONSE_OK,
		RESPONSE_ERROR,
	};

	vector<int> GetFieldFromAddress(UINT uAddress, UINT uLenInBytes); //bit型態欄位的address會重複, 但
	void FIELD_CREATE_MAPPING_MEMORY();
	void FIELD_DESTROY_MAPPING_MEMORY();
	virtual void HandleAddressValueNotify(PLC_ACTION_FLAG_ eFlag, int nField){};
	virtual void HandleWriteAction(PLC_ACTION_FLAG_ eFlag, UINT uAddress, UINT uLenInBytes){};

private:
	enum PLC_OP_CODE_{
		PLC_BATCH_READ = 0,
		PLC_BATCH_WRITE = 1,
	};

	void Init();
	void Finalize();
	void NotifyWriteAction(PLC_ACTION_FLAG_ eFlag, UINT uAddress, UINT uLenInBytes);
	BOOL OnAddressValueNotify(UINT uAddress, unsigned short *pValue, UINT uLen);

	void OpAddressAsciiQ(PLC_OP_CODE_ xOp, char cDevType, UINT uAddress, UINT uLenInWord, BYTE *pWrite); //DATA LEN: uLen * sizeof(WORD)
	void OpAddressBinaryQ(PLC_OP_CODE_ xOp, char cDevType, UINT uAddress, UINT uLenInWord, BYTE *pWrite); //DATA LEN: uLen * sizeof(WORD)

	BOOL ProcessQCmdItem(void *pCmd, int &nProcLen);
	void ProcessErrState(unsigned short wErr);
	void PushCmdItem(PLC_MODE xMode, PLC_OP_CODE_ xOp, char cDevType, UINT uAddress, UINT uLenInWord, BYTE *pWrite);
	void DumpOverTimeCmd();

	void ProcessSockeData(unsigned char *pData, int nLen);

	void OpPlcField(PLC_FIELD_OP_ xOp, int nFieldId, void *pData);
	void ProcessFieldData(PLC_NOTIFY_ID_ eNotifyId, void *pData, UINT uDataBytes, const PLC_DATA_ITEM &xItem);
	void MAKE_BINARY_REQUEST_ITEM(BYTE *pItem, PLC_OP_CODE_ xOp, char cDevType, UINT uAddress, UINT uLenInWord);
	void MAKE_ASCII_REQUEST_ITEM(BYTE *pItem, PLC_OP_CODE_ xOp, char cDevType, UINT uAddress, UINT uLenInWord);
private:
	typedef struct PLC_CMD_QUEUE_ITEM_{
		int nMode;
		PLC_OP_CODE_ xOp;
		UINT uAddress;
		DWORD dwTime;
		UINT uSize; //Field Len(Size in Word), when sending write data: 2 Bytes(BINARY)/4 Bytes(ASCII)
		unsigned short uCmdSerialNo;
		char cDevType;
		BYTE *pWrite;
	}PLC_CMD_QUEUE_ITEM;
	void UpdateWriteField(int nMode, UINT uSizeInBytes, UINT uAddress, BYTE *pWrite);
	vector<PLC_CMD_QUEUE_ITEM> m_vCmd;
	PLC_MODE m_xMode;
	PLC_DEVICE m_xDev;
	unsigned short m_uSerial;

	std::mutex  m_oMutex;
	enum
	{
		EV_EXIT,
		EV_CMDSEND,
		EV_CMDRCV,
		EV_COUNT,
		CASE_EXIT = WAIT_OBJECT_0,
		CASE_CMDSEND,
		CASE_CMDRCV,
	};
	HANDLE m_hThread;
	HANDLE m_hEvent[EV_COUNT];
	BOOL m_bSendFlag; //3E frame 沒有serial no. 發送cmd之後要確認收到才能發送下一筆.批次送大量會有不回cmd的狀況
	static DWORD __stdcall Thread_ProcessCmd(void* pvoid);
	void SendCmd();

	unsigned char **m_pFieldVal;
	__time64_t *m_pFieldTime;
	AOI_SUBCUSTOMERTYPE_ m_eSubCustomerType;
	PLC_FRAME_TYPE m_eFrameType;

};

template void CMelsecPlcSocket::WriteAddress<float>(char cDevType, UINT uAddress, UINT uLen, float *pWrite);
template void CMelsecPlcSocket::WriteAddress<WORD>(char cDevType, UINT uAddress, UINT uLen, WORD *pWrite);
template void CMelsecPlcSocket::WriteAddress<char>(char cDevType, UINT uAddress, UINT uLen, char *pWrite);

