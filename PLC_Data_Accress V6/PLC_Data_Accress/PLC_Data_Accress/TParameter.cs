using ActProgTypeLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PLC_Data_Access
{
    public enum ValueStatus
    {
        /// <summary>
        /// 開啟使用
        /// </summary>
        IsUse =0,
        /// <summary>
        /// 空值
        /// </summary>
        IsNull=1,
        /// <summary>
        /// 無值
        /// </summary>
        IsEmpty=3,
        /// <summary>
        /// 字串帶"~"
        /// </summary>
        IsArray = 4,
        /// <summary>
        /// 單一軟元件
        /// </summary>
        IsSingle =5
    }
    public class TParameter
    {
        public static CMXComponent MxConnect = new CMXComponent();
        public static CDeviceData DeviceData = new CDeviceData();
        public static CErrorInfo ErrorInfo = new CErrorInfo();

        public static void Init()
        {
            DeviceData.LoadData_Full();//先進行參數餵入
            SetPLCProperty(MxConnect.CpuName);//再進行PLC連線(不論有無連線成功)
            MxConnect.ProgOpen();//連線測試
        }
        public static void SetPLCProperty(string sType)
        {
            //屬性列表Property list
            switch (sType)
            {
                case "FX5UCPU":
                    #region FX5UCPU
                    MxConnect.ActHostAddress = TParameter.MxConnect._ActHostAddress;
                    MxConnect.ActConnectUnitNumber = 0;
                    MxConnect.ActCpuType = 528;

                    MxConnect.ActBaudRate = 0;
                    MxConnect.ActControl = 0;
                    MxConnect.ActDataBits = 0;
                    MxConnect.ActDataBits = 0;
                    MxConnect.ActDestinationIONumber = 0;
                    MxConnect.ActDestinationPortNumber = 5562;
                    MxConnect.ActDidPropertyBit = 1;
                    MxConnect.ActDsidPropertyBit = 1;
                    MxConnect.ActIntelligentPreferenceBit = 0;
                    MxConnect.ActIONumber = 1023;
                    MxConnect.ActNetworkNumber = 0;
                    MxConnect.ActMultiDropChannelNumber = 0;
                    MxConnect.ActSourceNetworkNumber = 0;
                    MxConnect.ActPacketType = 1;
                    MxConnect.ActPassword = "";
                    MxConnect.ActPortNumber = 0;
                    MxConnect.ActProtocolType = 5;
                    MxConnect.ActSourceStationNumber = 5;
                    MxConnect.ActStationNumber = 255;
                    MxConnect.ActStopBits = 0;
                    MxConnect.ActSumCheck = 0;
                    MxConnect.ActThroughNetworkType = 1;
                    MxConnect.ActTimeOut = 10000;
                    MxConnect.ActCpuTimeOut = 0;
                    MxConnect.ActUnitNumber = 0;
                    MxConnect.ActUnitType = 8193;
                    #endregion
                    break;
                case "Q13UDEHCPU":
                    #region Q13UDEHCPU
                    MxConnect.ActHostAddress = TParameter.MxConnect._ActHostAddress;
                    MxConnect.ActConnectUnitNumber = 0;
                    MxConnect.ActCpuType = 147;//Q13UDEHCPU

                    MxConnect.ActBaudRate = 0;//N
                    MxConnect.ActControl = 0;//N
                    MxConnect.ActDataBits = 0;//N
                    MxConnect.ActDataBits = 0;//N
                    MxConnect.ActDestinationIONumber = 0;
                    MxConnect.ActDestinationPortNumber = 5002;
                    MxConnect.ActDidPropertyBit = 1;
                    MxConnect.ActDsidPropertyBit = 1;
                    MxConnect.ActIntelligentPreferenceBit = 0;//N
                    MxConnect.ActIONumber = 1023;
                    MxConnect.ActNetworkNumber = 0;
                    MxConnect.ActMultiDropChannelNumber = 0;
                    MxConnect.ActSourceNetworkNumber = 0;
                    MxConnect.ActPacketType = 1;//N
                    MxConnect.ActPassword = "";
                    MxConnect.ActPortNumber = 0;//N
                    MxConnect.ActProtocolType = 5;
                    MxConnect.ActSourceStationNumber = 0;//可能有誤
                    MxConnect.ActStationNumber = 255;
                    MxConnect.ActStopBits = 0;//N
                    MxConnect.ActSumCheck = 0;//N
                    MxConnect.ActThroughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    MxConnect.ActTimeOut = 10000;
                    MxConnect.ActCpuTimeOut = 0;//N
                    MxConnect.ActUnitNumber = 0;
                    MxConnect.ActUnitType = 26;//UNIT_QJ71E71(0x1A)
                    #endregion
                    break;
                case "Q06UDVCPU":
                    #region Q06UDVCPU
                    MxConnect.ActHostAddress = TParameter.MxConnect._ActHostAddress;
                    MxConnect.ActConnectUnitNumber = 0;
                    MxConnect.ActCpuType = 211;

                    MxConnect.ActBaudRate = 0;//N
                    MxConnect.ActControl = 0;//N
                    MxConnect.ActDataBits = 0;//N
                    MxConnect.ActDataBits = 0;//N
                    MxConnect.ActDestinationIONumber = 0;
                    MxConnect.ActDestinationPortNumber = 5002;
                    MxConnect.ActDidPropertyBit = 1;
                    MxConnect.ActDsidPropertyBit = 1;
                    MxConnect.ActIntelligentPreferenceBit = 0;//N
                    MxConnect.ActIONumber = 1023;
                    MxConnect.ActNetworkNumber = 0;
                    MxConnect.ActMultiDropChannelNumber = 0;
                    MxConnect.ActSourceNetworkNumber = 0;
                    MxConnect.ActPacketType = 1;
                    MxConnect.ActPassword = "";
                    MxConnect.ActPortNumber = 0;//N
                    MxConnect.ActProtocolType = 5;
                    MxConnect.ActSourceStationNumber = 0;//可能有誤
                    MxConnect.ActStationNumber = 255;
                    MxConnect.ActStopBits = 0;//N
                    MxConnect.ActSumCheck = 0;//N
                    MxConnect.ActThroughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    MxConnect.ActTimeOut = 10000;
                    MxConnect.ActCpuTimeOut = 0;//N
                    MxConnect.ActUnitNumber = 0;
                    MxConnect.ActUnitType = 26;//UNIT_QJ71E71(0x1A)
                    #endregion
                    break;
                case "Q06HCPU":
                    #region Q06HCPU
                    MxConnect.ActHostAddress = TParameter.MxConnect._ActHostAddress;
                    MxConnect.ActConnectUnitNumber = 0;
                    MxConnect.ActCpuType = 70;//Q06PHCPU

                    MxConnect.ActBaudRate = 0;//N
                    MxConnect.ActControl = 0;//N
                    MxConnect.ActDataBits = 0;//N
                    MxConnect.ActDataBits = 0;//N
                    MxConnect.ActDestinationIONumber = 0;
                    MxConnect.ActDestinationPortNumber = 5002;
                    MxConnect.ActDidPropertyBit = 1;
                    MxConnect.ActDsidPropertyBit = 1;
                    MxConnect.ActIntelligentPreferenceBit = 0;//N
                    MxConnect.ActIONumber = 1023;
                    MxConnect.ActNetworkNumber = 0;
                    MxConnect.ActMultiDropChannelNumber = 0;
                    MxConnect.ActSourceNetworkNumber = 0;
                    MxConnect.ActPacketType = 1;
                    MxConnect.ActPassword = "";
                    MxConnect.ActPortNumber = 0;//N
                    MxConnect.ActProtocolType = 5;
                    MxConnect.ActSourceStationNumber = 0;//可能有誤
                    MxConnect.ActStationNumber = 255;
                    MxConnect.ActStopBits = 0;//N
                    MxConnect.ActSumCheck = 0;//N
                    MxConnect.ActThroughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    MxConnect.ActTimeOut = 10000;
                    MxConnect.ActCpuTimeOut = 0;//N
                    MxConnect.ActUnitNumber = 0;
                    MxConnect.ActUnitType = 26;//UNIT_QJ71E71(0x1A)
                    #endregion
                    break;
                case "Q06UDEHCPU":
                    #region Q06UDEHCPU
                    MxConnect.ActHostAddress = TParameter.MxConnect._ActHostAddress;
                    MxConnect.ActConnectUnitNumber = 0;
                    MxConnect.ActCpuType = 146;

                    MxConnect.ActBaudRate = 0;//N
                    MxConnect.ActControl = 0;//N
                    MxConnect.ActDataBits = 0;//N
                    MxConnect.ActDataBits = 0;
                    MxConnect.ActDestinationIONumber = 0;
                    MxConnect.ActDestinationPortNumber = 5002;
                    MxConnect.ActDidPropertyBit = 1;
                    MxConnect.ActDsidPropertyBit = 1;
                    MxConnect.ActIntelligentPreferenceBit = 0;//N
                    MxConnect.ActIONumber = 1023;
                    MxConnect.ActNetworkNumber = 0;
                    MxConnect.ActMultiDropChannelNumber = 0;
                    MxConnect.ActSourceNetworkNumber = 0;
                    MxConnect.ActPacketType = 1;
                    MxConnect.ActPassword = "";
                    MxConnect.ActPortNumber = 0;//N
                    MxConnect.ActProtocolType = 5;
                    MxConnect.ActSourceStationNumber = 0;//可能有誤
                    MxConnect.ActStationNumber = 255;
                    MxConnect.ActStopBits = 0;//N
                    MxConnect.ActSumCheck = 0;//N
                    MxConnect.ActThroughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    MxConnect.ActTimeOut = 10000;
                    MxConnect.ActCpuTimeOut = 0;//N
                    MxConnect.ActUnitNumber = 0;
                    MxConnect.ActUnitType = 26;//UNIT_QJ71E71(0x1A)
                    #endregion
                    break;
                case "R16CPU":
                    #region R16CPU
                    MxConnect.ActHostAddress = TParameter.MxConnect._ActHostAddress;
                    MxConnect.ActConnectUnitNumber = 0;
                    MxConnect.ActCpuType = 4099;

                    MxConnect.ActBaudRate = 0;
                    MxConnect.ActControl = 0;
                    MxConnect.ActDataBits = 0;
                    MxConnect.ActDataBits = 0;
                    MxConnect.ActDestinationIONumber = 0;
                    MxConnect.ActDestinationPortNumber = 5002;
                    MxConnect.ActDidPropertyBit = 1;
                    MxConnect.ActDsidPropertyBit = 1;
                    MxConnect.ActIntelligentPreferenceBit = 0;
                    MxConnect.ActIONumber = 1023;
                    MxConnect.ActNetworkNumber = 0;
                    MxConnect.ActMultiDropChannelNumber = 0;
                    MxConnect.ActSourceNetworkNumber = 0;//N
                    MxConnect.ActPacketType = 1;
                    MxConnect.ActPassword = "";
                    MxConnect.ActPortNumber = 1;
                    MxConnect.ActProtocolType = 5;
                    MxConnect.ActSourceStationNumber = 0;//可能有誤
                    MxConnect.ActStationNumber = 255;//N
                    MxConnect.ActStopBits = 0;
                    MxConnect.ActSumCheck = 0;
                    MxConnect.ActThroughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    MxConnect.ActTimeOut = 10000;
                    MxConnect.ActCpuTimeOut = 0;
                    MxConnect.ActUnitNumber = 0;
                    MxConnect.ActUnitType = 4098;//RCPU Ethernet port connection (IP specification) 0x1002
                    #endregion
                    break;
                case "R32CPU":
                    #region R32CPU
                    MxConnect.ActHostAddress = TParameter.MxConnect._ActHostAddress;
                    MxConnect.ActConnectUnitNumber = 0;
                    MxConnect.ActCpuType = 4100;

                    MxConnect.ActBaudRate = 0;
                    MxConnect.ActControl = 0;
                    MxConnect.ActDataBits = 0;
                    MxConnect.ActDataBits = 0;
                    MxConnect.ActDestinationIONumber = 0;
                    MxConnect.ActDestinationPortNumber = 5002;
                    MxConnect.ActDidPropertyBit = 1;
                    MxConnect.ActDsidPropertyBit = 1;
                    MxConnect.ActIntelligentPreferenceBit = 0;
                    MxConnect.ActIONumber = 1023;
                    MxConnect.ActNetworkNumber = 0;
                    MxConnect.ActMultiDropChannelNumber = 0;
                    MxConnect.ActSourceNetworkNumber = 0;//N
                    MxConnect.ActPacketType = 1;
                    MxConnect.ActPassword = "";
                    MxConnect.ActPortNumber = 1;
                    MxConnect.ActProtocolType = 5;
                    MxConnect.ActSourceStationNumber = 0;//可能有誤
                    MxConnect.ActStationNumber = 255;//N
                    MxConnect.ActStopBits = 0;
                    MxConnect.ActSumCheck = 0;
                    MxConnect.ActThroughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    MxConnect.ActTimeOut = 10000;
                    MxConnect.ActCpuTimeOut = 0;
                    MxConnect.ActUnitNumber = 0;
                    MxConnect.ActUnitType = 4098;//RCPU Ethernet port connection (IP specification) 0x1002
                    #endregion
                    break;
            }
        }
    }
    public class CDataStruct
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 軟元件名稱
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 軟元件位置範圍
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 元件組類別
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 數據
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 是否使用該值
        /// </summary>
        public string IsUse { get; set; }
        /// <summary>
        /// 軟元件讀取值
        /// </summary>
        public string DeviceValueGet { get; set; }
        /// <summary>
        /// 軟元件寫入值
        /// </summary>
        public string DeviceValueSet { get; set; }
    }
    public class CDeviceData
    {
        /// <summary>
        /// 預設路徑檔案位子
        /// </summary>
        public string DefaultPath;
        /// <summary>
        /// 路徑檔案位子
        /// </summary>
        public string DataFile_Path;
        /// <summary>
        /// 讀寫軟元件資料表路徑
        /// </summary>
        public string DeviceFileDataPath;
        /// <summary>
        /// PLC設定檔路徑
        /// </summary>
        public string DeviceFilePLCPath;
        /// <summary>
        /// 讀列表
        /// </summary>
        public List<CDataStruct> lReadData;
        /// <summary>
        /// 寫列表
        /// </summary>
        public List<CDataStruct> lWriteData;
        /// <summary>
        /// 模式轉換  0=編輯 1=運作中
        /// </summary>
        public int iModelChange;
        /// <summary>
        /// 連線 0=斷線 1=連線中
        /// </summary>
        public int iPLCConnect;
        
        //=================================================================
        public CDeviceData()
        {
            DefaultPath = Application.StartupPath + @"\Datatext";
            lReadData = new List<CDataStruct>();
            lWriteData = new List<CDataStruct>();
            iModelChange = 0;
            iPLCConnect = 1;
        }
        //讀取
        public void LoadData_Full()
        {
            LoadData_Path();//路徑資料夾
            LoadData_File();//預設路徑檔案
            LoadData_ReadWrite();//軟元件資料表(下載上傳)
            LoadData_Model();//PLC資料表
        }
        //找創資料夾
        public void LoadData_Path()
        {
            if (!Directory.Exists(DefaultPath))//判斷是不是有這路徑
            {
                Directory.CreateDirectory(DefaultPath);//沒有就創建
                MessageBox.Show("無路徑資料夾,已生成");
            }
            DataFile_Path = DefaultPath + "\\DataFile.txt";
        }
        //讀取對應預設路徑檔案
        public void LoadData_File()
        {
            if (!File.Exists(DataFile_Path))//判斷是不是有這檔案
            {
                using (StreamWriter createfile = File.CreateText(DataFile_Path))//沒有就創建
                {
                    MessageBox.Show("無指定路徑資料,已生成");
                    //建立預設路徑
                    DeviceFileDataPath = DefaultPath + "\\DefaultDataGrid.txt";
                    DeviceFilePLCPath = DefaultPath + "\\DefaultModelList.txt";
                    createfile.WriteLine(DeviceFileDataPath);//預設軟元件資料表
                    createfile.WriteLine(DeviceFilePLCPath);//預設PLC資料表
                    createfile.Close();
                }
            }
            else
            {
                //有這檔案,讀取檔案
                string sData;
                using (StreamReader reader = new StreamReader(DataFile_Path))//資料讀出來
                {
                    sData = reader.ReadToEnd();
                    reader.Close();
                }
                ArrayList DataList = new ArrayList();//斷字陣列
                Break_String(sData, "\r\n", ref DataList);
                if (DataList.Count != 0)
                {
                    DeviceFileDataPath = DataList[0].ToString();//軟元件資料表路徑
                    DeviceFilePLCPath = DataList[1].ToString();//PLC資料表路徑
                }
            }
        }
        //讀取軟元件資料表
        public void LoadData_ReadWrite()
        {
            #region 沒有就創建且改回預設路徑
            if (!File.Exists(DeviceFileDataPath))
            {
                DeviceFileDataPath = DefaultPath + "\\DefaultDataGrid.txt";
                using (StreamWriter createfile = File.CreateText(DeviceFileDataPath))
                {
                    //改寫預設路徑檔案
                    using (StreamWriter writer = new StreamWriter(TParameter.DeviceData.DataFile_Path))
                    {
                        writer.Write("");//清除
                        writer.WriteLine(TParameter.DeviceData.DeviceFileDataPath);
                        writer.WriteLine(TParameter.DeviceData.DeviceFilePLCPath);
                        writer.Close();
                    }
                    MessageBox.Show("無指定資料表,已生成");
                    createfile.Close();
                }
            }
            #endregion
            #region 判斷後讀取
            string sData;
            using (StreamReader reader = new StreamReader(DeviceFileDataPath))
            {
                sData = reader.ReadToEnd();
                reader.Close();
            }
            //格式
            //序號$名稱$地址$數據類型$值$是否使用(0/1)&&     ///(Read)
            //!!
            //序號$名稱$地址$數據類型$值$是否使用(0/1)&&     ///(Write)
            ArrayList DataList = new ArrayList();
            ArrayList ReadList = new ArrayList();
            ArrayList WriteList = new ArrayList();
            ArrayList SigleData_List = new ArrayList();
            try
            {
                if (sData != "")//若全資料部為不空
                {
                    DiviceDataArrClear();//清空目前有的資料
                    Break_String(sData, "!!\r\n", ref DataList);//Read & Write & 參數(以!!分隔)
                    Break_String(DataList[0].ToString(), "&&\r\n", ref ReadList);//Read解為單行(以&&分行)
                    Break_String(DataList[1].ToString(), "&&\r\n", ref WriteList);//Write解為單行(以&&分行)
                    #region 下載
                    if (ReadList.Count != 0)//若有資料
                    {
                        foreach (string single_Data in ReadList)
                        {
                            if (single_Data != "")
                            {
                                Break_String(single_Data, "$", ref SigleData_List);//Read單行解成單一資料(以$分隔)
                                CDataStruct cData = new CDataStruct
                                {
                                    SN = SigleData_List[0].ToString(),
                                    Label = SigleData_List[1].ToString(),
                                    Address = SigleData_List[2].ToString(),
                                    DataType = SigleData_List[3].ToString(),
                                    Data = SigleData_List[4].ToString(),
                                    IsUse = SigleData_List[5].ToString(),
                                    DeviceValueGet = "N/A",
                                    DeviceValueSet = ""
                                };
                                lReadData.Add(cData);
                            }
                        }
                    }
                    #endregion
                    #region 上傳
                    if (WriteList.Count != 0)//若有資料
                    {
                        foreach (string single_Data in WriteList)
                        {
                            if (single_Data != "")
                            {
                                Break_String(single_Data, "$", ref SigleData_List);//Write單行解成單一資料(以$分隔)
                                CDataStruct cData = new CDataStruct
                                {
                                    SN = SigleData_List[0].ToString(),
                                    Label = SigleData_List[1].ToString(),
                                    Address = SigleData_List[2].ToString(),
                                    DataType = SigleData_List[3].ToString(),
                                    Data = SigleData_List[4].ToString(),
                                    IsUse = SigleData_List[5].ToString(),
                                    DeviceValueGet = "N/A",
                                    DeviceValueSet = ""
                                };
                                lWriteData.Add(cData);
                            }
                        }
                    }
                    #endregion
                    #region 參數
                    TParameter.MxConnect.iReciveTime = Convert.ToInt32(DataList[2].ToString());//回傳時間
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "LoadReadWriteData", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }
        //讀取PLC型號連線資料
        public void LoadData_Model()
        {
            #region 沒有就創建且改回預設路徑
            if (!File.Exists(DeviceFilePLCPath))
            {
                DeviceFilePLCPath = DefaultPath + "\\DeviceModelList.txt";
                using (StreamWriter createfile = File.CreateText(DeviceFilePLCPath))//沒有就創建
                {
                    //改寫預設值
                    createfile.WriteLine(TParameter.MxConnect.CpuName);//預設值
                    createfile.WriteLine(TParameter.MxConnect._ActHostAddress);//預設值
                    //改寫預設路徑txt
                    using (StreamWriter writer = new StreamWriter(TParameter.DeviceData.DataFile_Path))
                    {
                        writer.Write("");//清除
                        writer.WriteLine(DeviceFileDataPath);
                        writer.WriteLine(DeviceFilePLCPath);
                        writer.Close();
                    }
                    MessageBox.Show("無指定資料表,已生成");
                    createfile.Close();
                }
            }
            #endregion
            # region 判斷後讀取
            string sData;
            using (StreamReader reader = new StreamReader(DeviceFilePLCPath))
            {
                sData = reader.ReadToEnd();
                reader.Close();
            }
            ArrayList DataList = new ArrayList();
            Break_String(sData, "\r\n", ref DataList);//(以\r\n分隔)
            if (DataList.Count != 0)
            {
                TParameter.MxConnect.CpuName = DataList[0].ToString();//CpuName
                TParameter.MxConnect._ActHostAddress = DataList[1].ToString();//Address
            }
            #endregion
        }
        ///---------------
        //寫入對應資料表
        public void SaveData_ReadWrite()
        {
            #region 沒有就創建且改回預設路徑
            if (!File.Exists(DeviceFileDataPath))//判斷是不是有這檔案
            {
                using (StreamWriter createfile = File.CreateText(DeviceFileDataPath))//沒有就創建
                {
                    MessageBox.Show("無指定資料表,已生成");
                    createfile.Close();
                }
            }
            #endregion
            # region 判斷後寫入
            using (StreamWriter writer = new StreamWriter(DeviceFileDataPath)) //寫入矩陣資料
            {
                writer.Write("");//清除
                string LineData;
                //序號$名稱$地址$數據類型$值$是否使用(0/1)&&
                if (lReadData.Count != 0)
                {
                    for (int i = 0; i < lReadData.Count; i++)//Read
                    {
                        //串起來
                        LineData = lReadData[i].SN + "$" +
                                   lReadData[i].Label + "$" +
                                   lReadData[i].Address + "$" +
                                   lReadData[i].DataType + "$" +
                                   lReadData[i].Data + "$" +
                                   lReadData[i].IsUse + "&&";
                        writer.WriteLine(LineData);//寫入
                    }
                }
                writer.WriteLine("!!");//分隔Read/Write
                if (lWriteData.Count != 0)
                {
                    for (int i = 0; i < lWriteData.Count; i++)//Write
                    {
                        //串起來
                        LineData = lWriteData[i].SN + "$" +
                                   lWriteData[i].Label + "$" +
                                   lWriteData[i].Address + "$" +
                                   lWriteData[i].DataType + "$" +
                                   lWriteData[i].Data + "$" +
                                   lWriteData[i].IsUse + "&&";
                        writer.WriteLine(LineData);//寫入
                    }
                }
                writer.WriteLine("!!");//分隔Write/參數
                writer.WriteLine(TParameter.MxConnect.iReciveTime);//參數
                writer.Close();
            }
            #endregion
        }
        public void SaveData_Model()
        {
            #region 沒有就創建且改回預設路徑
            if (!File.Exists(DeviceFileDataPath))//判斷是不是有這檔案
            {
                using (StreamWriter createfile = File.CreateText(DeviceFileDataPath))//沒有就創建
                {
                    MessageBox.Show("無指定資料表,已生成");
                    createfile.Close();
                }
            }
            #endregion
            #region 判斷後寫入
            using (StreamWriter writer = new StreamWriter(DeviceFilePLCPath))//寫入矩陣資料
            {
                writer.Write("");//清除
                writer.WriteLine(TParameter.MxConnect.CpuName);
                writer.WriteLine(TParameter.MxConnect._ActHostAddress);
                writer.Close();
            }
            #endregion
        }
        public void SaveData_ModChange(DataGridView dgv_Read, DataGridView dgv_Write)
        {
            if (iModelChange == 0)//修改=>執行
            {
                iModelChange = 1;
                //下載(讀)資料:全鎖定
                dgv_Read.ReadOnly = true;
                //上傳(讀寫)資料:把除了修改值(可以變動)以外的在修改模式中鎖定
                for(int i=0;i< dgv_Write.Columns.Count; i++)
                {
                    dgv_Write.Columns[i].ReadOnly = true;
                }
            }
            //執行=>修改
            else if (iModelChange == 1)
            {
                iModelChange = 0;
                dgv_Read.ReadOnly = false;
                for (int i = 0; i < dgv_Write.Columns.Count; i++)
                {
                    dgv_Write.Columns[i].ReadOnly = true;
                }
            }
            return;
        }
        ///---------------------------------------------
        public void GetCombineArray_str(string sDevice, out int iSize, out string sOutCombimeArray)
        {
            //僅以D值做目標
            ArrayList arrCombine = new ArrayList();
            string sStart;//軟元件開頭
            string sEnd;//軟元件結尾

            TParameter.DeviceData.Break_String(sDevice, "~", ref arrCombine);//以"~"斷字:D700~D705 =>[D700][D705]
            sStart = arrCombine[0].ToString();//軟元件開頭
            sEnd = arrCombine[1].ToString();//軟元件結尾

            int iStart = Convert.ToInt32(sStart.Replace("D", ""));//軟元件開頭 改數字
            int iEnd = Convert.ToInt32(sEnd.Replace("D", ""));//軟元件結尾 改數字
            iSize = Math.Abs(iEnd - iStart) + 1;//換算總軟元件數量

            sOutCombimeArray = "";
            if (iEnd > iStart)
            {                //D700        D705
                for (int i = iStart; i <= iEnd; i++)
                {
                    //放第一個不加換行
                    sOutCombimeArray += (sOutCombimeArray == "") ? "D" + i.ToString() : "\nD" + i.ToString();
                }
            }
            if (iEnd < iStart)
            {
                for (int i = iEnd; i <= iStart; i++)
                {
                    //放第一個不加換行
                    sOutCombimeArray += (sOutCombimeArray == "") ? "D" + i.ToString() : "\nD" + i.ToString();
                }
            }
        }
        public void GetCombineSize_int(string sDevice, out int iSize)
        {
            ArrayList arrCombine = new ArrayList();
            string sStart;//軟元件開頭
            string sEnd;//軟元件結尾

            TParameter.DeviceData.Break_String(sDevice, "~", ref arrCombine);//以"~"斷字:D700~D705 =>[D700][D705]
            sStart = arrCombine[0].ToString();//軟元件開頭
            sEnd = arrCombine[1].ToString();//軟元件結尾

            int iStart = Convert.ToInt32(sStart.Replace("D", ""));//軟元件開頭 改數字
            int iEnd = Convert.ToInt32(sEnd.Replace("D", ""));//軟元件結尾 改數字
            iSize = Math.Abs(iEnd - iStart) + 1;//換算總軟元件數量
        }
        //工具
        public void DiviceDataArrClear()
        {
            //Read
            lReadData.Clear();
            //Write
            lWriteData.Clear();
        }
        //判斷"0"為bool=>false
        public bool ZeroToBool(string isUse)
        {
            bool re;
            if (isUse == "0")//"0"=false
            {
                re = false;
            }
            else//"1"=true
            {
                re = true;
            }
            return re;
        }
        public string BoolToZero(object CellsValue)
        {
            string re = "0";
            if (CellsValue != null)//會遇到值未設定的情況 =null
            {
                if (CellsValue.ToString() == "False")//False ="0"
                {
                    re = "0";
                }
                if (CellsValue.ToString() == "True")//Ture ="1"
                {
                    re = "1";
                }
            }
            else
            {
                re = "0";//null = "0"
            }
            return re;
        }
        public void Break_String(string sData, string sBreakItem, ref ArrayList list)
        {
            list.Clear();
            string[] Spare = sData.Split(new string[] { sBreakItem }, StringSplitOptions.None);
            foreach (string item in Spare)//餵回去list
            {
                list.Add(item);
            }
        }
        public ValueStatus SetDataStatus(int count,string readWrite, string GetSet)
        {
            ValueStatus re = ValueStatus.IsEmpty;

            switch (readWrite)
            {
                case "Write":
                    if (TParameter.DeviceData.lWriteData[count].IsUse == "1")
                    {
                        if (GetSet == "Set")
                        {
                            if (TParameter.DeviceData.lWriteData[count].DeviceValueSet != "")
                                re = (TParameter.DeviceData.lWriteData[count].Address.Contains("~")) ? ValueStatus.IsArray : ValueStatus.IsSingle;
                        }
                        if(GetSet == "Get")
                        {
                            re = (TParameter.DeviceData.lWriteData[count].Address.Contains("~")) ? ValueStatus.IsArray : ValueStatus.IsSingle;
                        }
                    }
                        
                    break;
                case "Read":
                    if (TParameter.DeviceData.lReadData[count].IsUse == "1")
                    {
                        if (GetSet == "Get")
                        {
                            re = (TParameter.DeviceData.lReadData[count].Address.Contains("~")) ? ValueStatus.IsArray : ValueStatus.IsSingle;
                        }
                    }
                    break;
            }

            if (TParameter.DeviceData.lWriteData[count].IsUse == "1")
                if (TParameter.DeviceData.lWriteData[count].DeviceValueSet != "")
                    re = (TParameter.DeviceData.lWriteData[count].Address.Contains("~")) ? ValueStatus.IsArray : ValueStatus.IsSingle;

            return re;
        }
    }
    public class CMXComponent : ActProgTypeClass
    {

        #region Property 連線參數
        /// <summary>
        /// 回傳代碼
        /// </summary>
        public int iReturnCode;
        /// <summary>
        /// 程式回傳時間
        /// </summary>
        public int iReciveTime;
        /// <summary>
        /// CPU名稱
        /// </summary>
        public string CpuName;
        /// <summary>
        /// CPU代碼
        /// </summary>
        public int CPUType;
        /// <summary>
        /// HostIP
        /// </summary>
        public string _ActHostAddress;
        #endregion
        //-------------------------
        public CMXComponent()
        {
            iReturnCode = 0;
            iReciveTime = 2000;
            CpuName = "FX5UCPU";
            CPUType = 528;
            _ActHostAddress = "192.168.2.12";
        }
        //-------------------------

        public void ProgOpen()
        {
            //開啟Mx component功能
            iReturnCode = Open();
        }
        public void ProgClose()
        {
            //關閉Mx component功能
            iReturnCode = Close();
        }
        public void ProgGetBlockCombine(string sDevice, out string sOutCombimeValue)
        {
            ArrayList arrCombine = new ArrayList();
            string sStart;//軟元件開頭
            string sEnd;//軟元件結尾

            TParameter.DeviceData.Break_String(sDevice, "~", ref arrCombine);//以"~"斷字:D700~D705 =>[D700][D705]
            sStart = arrCombine[0].ToString();//軟元件開頭
            sEnd = arrCombine[1].ToString();//軟元件結尾

            int iStart = Convert.ToInt32(sStart.Replace("D", ""));//軟元件開頭 改數字
            int iEnd = Convert.ToInt32(sEnd.Replace("D", ""));//軟元件結尾 改數字
            int iSize = Math.Abs(iEnd - iStart) + 1;//換算總軟元件數量
            int[] arrData = new int[iSize];//標籤總數量(矩陣)

            sOutCombimeValue = "";
            try
            {
                iReturnCode = ReadDeviceBlock(sStart, iSize, out arrData[0]);//從軟元件開頭 讀出資料
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", "ProgGetBlockCombime", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (iReturnCode == 0)
            {
                for (int i = 0; i < arrData.Length; i++)
                {
                    sOutCombimeValue += arrData[i].ToString();//把它全部合起來[A][B][C][D][E]=>ABCDE
                }
            }
        }
        public void ProgSetBlockCombine(string sDevice, string sInCombimeValue)
        {
            ArrayList arrCombine = new ArrayList();
            string sStart;//軟元件開頭
            string sEnd;//軟元件結尾

            TParameter.DeviceData.Break_String(sDevice, "~", ref arrCombine);//以"~"斷字:D700~D705 =>[D700][D705]
            sStart = arrCombine[0].ToString();//軟元件開頭
            sEnd = arrCombine[1].ToString();//軟元件結尾

            int iStart = Convert.ToInt32(sStart.Replace("D", ""));//軟元件開頭 改數字
            int iEnd = Convert.ToInt32(sEnd.Replace("D", ""));//軟元件結尾 改數字
            int iSize = Math.Abs(iEnd - iStart) + 1;//換算總軟元件數量
            int[] arrData = new int[iSize];//標籤總數量(矩陣)
            //換算
            while (sInCombimeValue.Length < iSize * 2)//若值不為 軟元件數*元件容量則需補值 超過不管
            {
                sInCombimeValue += "0";//沒有就補0
            }
            //確認寫入資料有無問題  Substring
            for (int i = 0, j = 0; i < iSize; i++, j += 2)
            {
                arrData[i] = Convert.ToInt32(sInCombimeValue.Substring(j, 2));//word每個兩字元0123456789=>[01][23][45][67][89]
            }
            try
            {
                iReturnCode = WriteDeviceBlock(sStart, iSize, ref arrData[0]);//從軟元件開頭 讀出資料
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", "ProgSetBlockCombime", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ProgGetDeviceRandom(string arrsDevice, int iSize, out int[] arrDeviceData)
        {
            arrDeviceData = new int[iSize];
            try
            {
                iReturnCode = ReadDeviceRandom(arrsDevice, iSize, out arrDeviceData[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ProgGetDevice(string sDevice, out string sOutValue)
        {
            int iData = 0;
            try
            {
                iReturnCode = GetDevice(sDevice, out iData);//從軟元件開頭 讀出資料
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", "ProgGetDevice", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            sOutValue = iData.ToString();

        }
        public void ProgSetDevice(string sDevice, string sInValue)
        {
            int Data = Convert.ToInt32(sInValue);
            try
            {
                iReturnCode = SetDevice(sDevice, Data);//從軟元件開頭 讀出資料
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", "ProgGetDevice", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class CErrorInfo
    {
        public CErrorInfo()
        {

        }

        public string ErrorStrSend()
        {
            string sOutStr;
            int iErrorCode = TParameter.MxConnect.iReturnCode;

            sOutStr = "ErCode: " + String.Format("0x{0:x8} [HEX]", iErrorCode);

            return sOutStr;
        }

        public void ErrorMessageBox_Time()
        {
            MessageBox.Show("回傳時間設定錯誤\n請確認是否為空值或負值且不為文字\n改為預設值2000ms", "ModelChange", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ErrorMessageBox_DeviceBlock(Exception ex)
        {
            MessageBox.Show("系統訊息:" + ex.Message + "\n建議: 是否為之軟元件名稱錯誤", "ProgGetBlockCombine", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public void ErrorMessageBox_DeviceValue(Exception ex)
        {
            MessageBox.Show("系統訊息:" + ex.Message + "\n建議: 是否為之軟元件名稱錯誤", "ProgGetDevice", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
