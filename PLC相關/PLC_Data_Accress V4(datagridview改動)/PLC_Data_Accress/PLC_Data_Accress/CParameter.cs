using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActProgTypeLib;
using ActSupportMsgLib;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace PLC_Data_Access
{
    public enum ValueStatus
    {
        /// <summary>開啟使用</summary>
        IsUse = 0,
        /// <summary>空值</summary>
        IsNull = 1,
        /// <summary>無值</summary>
        IsEmpty = 3,
        /// <summary>字串帶"~"</summary>
        IsArrayDevice = 4,
        /// <summary>單一軟元件</summary>
        IsSingleDevice = 5
    }
    public enum ReadWriteStatus
    {
        IsRead = 0,
        IsWrite = 1
    }
    public enum GetSetStatus
    {
        IsGet = 0,
        IsSet = 1
    }
    public enum CData
    {
        /// <summary>流水號</summary>
        SN = 0,
        /// <summary>軟元件名稱</summary>
        Label = 1,
        /// <summary>軟元件位置範圍</summary>
        Address = 2,
        /// <summary>元件組類別</summary>
        DataType = 3,
        /// <summary>數據</summary>
        Data = 4,
        /// <summary>是否使用該值</summary>
        IsUse = 5,
        /// <summary>軟元件讀取值</summary>
        DeviceValueGet = 6,
        /// <summary>軟元件寫入值</summary>
        DeviceValueSet = 7
    }

    public class CParameter : CMX_Component
    {
        /// <summary>檔案預設路徑</summary>
        private string DefaultPath = System.Windows.Forms.Application.StartupPath + @"\Datatext";
        /// <summary>檔案路徑</summary>
        public string DataFile_Path;
        /// <summary>讀寫軟元件資料表路徑</summary>
        public string DeviceFileData_Path;
        /// <summary>檔案預設路徑</summary>
        public string DeviceFilePLC_Path;

        /// <summary>軟元件資料列表-下發</summary>
        public List<CDataStruct> lReadData;
        /// <summary>軟元件資料列表-上傳</summary>
        public List<CDataStruct> lWriteData;
        /// <summary>清單列表-下發</summary>
        public DataGridViewRow[] ReadGridRow;
        /// <summary>清單列表-上傳</summary>
        public DataGridViewRow[] WriteGridRow;
        /// <summary>模式轉換 0=編輯 1=運作中</summary>
        public int iModelChange = 0;
        /// <summary>PLC連線狀態</summary>
        public int iPLCConnect = 1;
        /// <summary>變數狀態</summary>
        //public ValueStatus valueStatus = ValueStatus.IsEmpty;
        //=================================================================
        public CParameter()
        {
            DefaultPath = Application.StartupPath + @"\Datatext";
            lReadData = new List<CDataStruct>();
            lWriteData = new List<CDataStruct>();
            iModelChange = 0;
            iPLCConnect = 1;
        }
        public void init()
        {
            LoadData_Full();
            SetPLCProperty(CpuName);
            iReturnCode = ProgOpen();
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
        ///讀取對應預設路徑檔案
        public void LoadData_File()
        {
            if (!File.Exists(DataFile_Path))//判斷是不是有這檔案
            {
                using (StreamWriter createfile = File.CreateText(DataFile_Path))//沒有就創建
                {
                    MessageBox.Show("無指定路徑資料,已生成");
                    //建立預設路徑
                    DeviceFileData_Path = DefaultPath + "\\DefaultDataGrid.txt";
                    DeviceFilePLC_Path = DefaultPath + "\\DefaultModelList.txt";
                    createfile.WriteLine(DeviceFileData_Path);//預設軟元件資料表
                    createfile.WriteLine(DeviceFilePLC_Path);//預設PLC資料表
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
                    DeviceFileData_Path = DataList[0].ToString();//軟元件資料表路徑
                    DeviceFilePLC_Path = DataList[1].ToString();//PLC資料表路徑
                }
            }
        }
        //讀取軟元件資料表
        public void LoadData_ReadWrite()
        {
            //判斷有無讀寫資料表這個檔案
            #region 沒有就創建且改回預設路徑
            if (!File.Exists(DeviceFileData_Path))
            {
                DeviceFileData_Path = DefaultPath + "\\DefaultDataGrid.txt";
                using (StreamWriter createfile = File.CreateText(DeviceFileData_Path))
                {
                    //改寫預設路徑檔案
                    using (StreamWriter writer = new StreamWriter(DataFile_Path))
                    {
                        writer.Write("");//清除
                        writer.WriteLine(DeviceFileData_Path);
                        writer.WriteLine(DeviceFilePLC_Path);
                        writer.Close();
                    }
                    MessageBox.Show("無指定資料表,已生成");
                    createfile.Close();
                }
            }
            #endregion

            #region 判斷後讀取
            string sData;
            using (StreamReader reader = new StreamReader(DeviceFileData_Path))
            {
                sData = reader.ReadToEnd();
                reader.Close();
            }
            //序號$名稱$地址$數據類型$值$是否使用(0/1)&&     ///(Read)
            //!!
            //序號$名稱$地址$數據類型$值$是否使用(0/1)&&     ///(Write)
            ArrayList DataList = new ArrayList();//Read/Write
            ArrayList ReadList = new ArrayList();
            ArrayList WriteList = new ArrayList();
            ArrayList SigleData_List = new ArrayList();
            try
            {
                if (sData != "")//若全資料部為不空
                {
                    DeviceDataArrClear();//清空目前有的資料
                    Break_String(sData, "!!\r\n", ref DataList);//Read & Write & 參數(以!!分隔)
                    Break_String(DataList[0].ToString(), "&&\r\n", ref ReadList);//Read解為單行(以&&分行)
                    Break_String(DataList[1].ToString(), "&&\r\n", ref WriteList);//Write解為單行(以&&分行)
                    //           DataList[2] 目前單參數
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
                    iReciveTime = Convert.ToInt32(DataList[2].ToString());//回傳時間
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "LoadReadWriteData", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }
        //讀取PLCModel資料
        public void LoadData_Model()
        {
            //判斷有無PLC_List表這個檔案
            if (!File.Exists(DeviceFilePLC_Path))
            {
                DeviceFilePLC_Path = DefaultPath + "\\DeviceModelList.txt";
                using (StreamWriter createfile = File.CreateText(DeviceFilePLC_Path))//沒有就創建
                {
                    //改寫預設值
                    createfile.WriteLine(CpuName);//預設值
                    createfile.WriteLine(ActHostAddress);//預設值
                    //改寫預設路徑txt
                    using (StreamWriter writer = new StreamWriter(DataFile_Path))
                    {
                        writer.Write("");//清除
                        writer.WriteLine(DeviceFileData_Path);
                        writer.WriteLine(DeviceFilePLC_Path);
                        writer.Close();
                    }
                    MessageBox.Show("無指定資料表,已生成");
                    createfile.Close();
                }
            }
            //讀取
            string sData;
            using (StreamReader reader = new StreamReader(DeviceFilePLC_Path))
            {
                sData = reader.ReadToEnd();
                reader.Close();
            }
            ArrayList DataList = new ArrayList();
            Break_String(sData, "\r\n", ref DataList);//(以\r\n分隔)
            if (DataList.Count != 0)
            {
                CpuName = DataList[0].ToString();//CpuName
                ActHostAddress = DataList[1].ToString();//Address
            }
        }
        ///---------------
        //寫入對應資料表
        public void SaveData_ReadWrite()
        {
            if (!File.Exists(DeviceFileData_Path))//判斷是不是有這檔案
            {
                using (StreamWriter createfile = File.CreateText(DeviceFileData_Path))//沒有就創建
                {
                    MessageBox.Show("無指定資料表,已生成");
                    createfile.Close();
                }
            }
            using (StreamWriter writer = new StreamWriter(DeviceFileData_Path)) //寫入矩陣資料
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
                writer.WriteLine(iReciveTime);//參數
                writer.Close();
            }
        }
        public void SaveData_Model()
        {
            #region 沒有就創建且改回預設路徑
            if (!File.Exists(DeviceFileData_Path))//判斷是不是有這檔案
            {
                using (StreamWriter createfile = File.CreateText(DeviceFileData_Path))//沒有就創建
                {
                    MessageBox.Show("無指定資料表,已生成");
                    createfile.Close();
                }
            }
            #endregion
            #region 判斷後寫入
            using (StreamWriter writer = new StreamWriter(DeviceFilePLC_Path))//寫入矩陣資料
            {
                writer.Write("");//清除
                writer.WriteLine(CpuName);
                writer.WriteLine(ActHostAddress);
                writer.Close();
            }
            #endregion
        }
        public void SaveData_ModChange(DataGridView dgv_Read, DataGridView dgv_Write)
        {
            //修改=>執行
            if (iModelChange == 0)
            {
                iModelChange = 1;
                //下載(讀)資料:全鎖定
                dgv_Read.ReadOnly = true;
                //上傳(讀寫)資料:把除了修改值(可以變動)以外的在修改模式中鎖定
                for (int i = 0; i < dgv_Write.Columns.Count; i++)
                {
                    //dgv_Write.Columns[i].ReadOnly = true;
                }
                dgv_Write.Columns[6].ReadOnly = false;
            }
            //執行=>修改
            else if (iModelChange == 1)
            {
                iModelChange = 0;
                dgv_Read.ReadOnly = false;
                for (int i = 0; i < dgv_Write.Columns.Count; i++)
                {
                    dgv_Write.Columns[i].ReadOnly = false;
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

            Break_String(sDevice, "~", ref arrCombine);//以"~"斷字:D700~D705 =>[D700][D705]
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

            Break_String(sDevice, "~", ref arrCombine);//以"~"斷字:D700~D705 =>[D700][D705]
            sStart = arrCombine[0].ToString();//軟元件開頭
            sEnd = arrCombine[1].ToString();//軟元件結尾

            int iStart = Convert.ToInt32(sStart.Replace("D", ""));//軟元件開頭 改數字
            int iEnd = Convert.ToInt32(sEnd.Replace("D", ""));//軟元件結尾 改數字
            iSize = Math.Abs(iEnd - iStart) + 1;//換算總軟元件數量
        }
        public int GetCombineByte(string sDevice,short[] arrDeviceData,int iOrderCount,out byte[] total, out int iDeviceSize)
        {
            GetCombineSize_int(sDevice, out int _iDeviceSize);
            iDeviceSize = _iDeviceSize;
            total = System.BitConverter.GetBytes(arrDeviceData[iOrderCount]);
            iOrderCount++;
            //區間byte聯合
            for (int i = 1; i < iDeviceSize; i++)
            {
                var addvalue = System.BitConverter.GetBytes(arrDeviceData[iOrderCount]);
                var FullValue = new byte[total.Length + addvalue.Length];
                //union
                Buffer.BlockCopy(total, 0, FullValue, 0, total.Length);
                Buffer.BlockCopy(addvalue, 0, FullValue, total.Length, addvalue.Length);

                total = FullValue;//一直累積完全部byte
                iOrderCount++;
            }
            return iOrderCount;
        }
        //工具
        public void DeviceDataArrClear()
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

        public ValueStatus SetDataStatus(int count, ReadWriteStatus readWrite, GetSetStatus GetSet)
        {
            ValueStatus re = ValueStatus.IsEmpty;

            switch (readWrite)
            {
                case ReadWriteStatus.IsWrite://上傳
                    switch (GetSet)
                    {
                        case GetSetStatus.IsSet://更改
                            if (lWriteData[count].IsUse == "1" && lWriteData[count].DeviceValueSet != "")
                            {
                                re = (lWriteData[count].Address.Contains("~")) ? ValueStatus.IsArrayDevice : ValueStatus.IsSingleDevice;
                            }
                            break;
                        case GetSetStatus.IsGet://讀取
                            if (lWriteData[count].IsUse == "1")
                            {
                                re = (lWriteData[count].Address.Contains("~")) ? ValueStatus.IsArrayDevice : ValueStatus.IsSingleDevice;
                            }
                            break;
                    }
                    break;
                case ReadWriteStatus.IsRead://下發
                    switch (GetSet)
                    {
                        case GetSetStatus.IsSet://更改
                            //TBA - 讓下發也能修改
                            break;
                        case GetSetStatus.IsGet://讀取
                            if (lReadData[count].IsUse == "1")
                            {
                                re = (lReadData[count].Address.Contains("~")) ? ValueStatus.IsArrayDevice : ValueStatus.IsSingleDevice;
                            }
                            break;
                    }
                    break;
            }
            //if (lWriteData[count].IsUse == "1")
            //    if (lWriteData[count].DeviceValueSet != "")
            //        re = (lWriteData[count].Address.Contains("~")) ? ValueStatus.IsArray : ValueStatus.IsSingle;

            return re;
        }
    }
    public class CMX_Component : ActProgTypeClass
    {
        //public ActProgTypeClass Prog_Connect = new ActProgTypeClass();//Program
        public ActSupportMsgClass SpMsg_Connect = new ActSupportMsgClass();//Message

        #region Property 連線參數
        /// <summary>回傳代碼</summary>
        public int iReturnCode = 0;
        /// <summary>程式回傳時間</summary>
        public int iReciveTime = 2000;
        /// <summary>CPU名稱</summary>
        public string CpuName = "FX5UCPU";
        /// <summary>CPU代碼</summary>
        public int CPUType = 528;
        /// <summary>HostIP </summary>
        public string _ActHostAddress = "192.168.2.12";


        #endregion
        //-------------------------
        public CMX_Component()
        {
            iReturnCode = 0;
            iReciveTime = 2000;
            CpuName = "FX5UCPU";
            CPUType = 528;
            _ActHostAddress = "192.168.2.99";



        }
        //-------------------------
        public void SetPLCProperty(string sType)
        {
            //屬性列表Property list
            switch (sType)
            {
                case "FX5UCPU":
                    #region FX5UCPU

                    ActHostAddress = "192.168.2.99";
                    ActConnectUnitNumber = 0;
                    ActCpuType = 528;

                    ActBaudRate = 0;
                    ActControl = 0;
                    ActDataBits = 0;
                    ActDataBits = 0;
                    ActDestinationIONumber = 0;
                    ActDestinationPortNumber = 5562;
                    ActDidPropertyBit = 1;
                    ActDsidPropertyBit = ActDsidPropertyBit = 1;
                    ActIntelligentPreferenceBit = 0;
                    ActIONumber = 1023;
                    ActNetworkNumber = 0;
                    ActMultiDropChannelNumber = 0;
                    ActSourceNetworkNumber = 0;
                    ActPacketType = 1;
                    ActPassword = "";
                    ActPortNumber = 0;
                    ActProtocolType = 5;
                    ActSourceStationNumber = 5;
                    ActStationNumber = 255;
                    ActStopBits = 0;
                    ActSumCheck = 0;
                    ActThroughNetworkType = 1;
                    ActTimeOut = 10000;
                    ActCpuTimeOut = 0;
                    ActUnitNumber = 0;
                    ActUnitType = 8193;

                    #endregion
                    break;
                case "Q13UDEHCPU":
                    #region Q13UDEHCPU
                    ActHostAddress = ActHostAddress;

                    ActConnectUnitNumber = 0;
                    ActCpuType = 147;//Q13UDEHCPU

                    ActBaudRate = 0;//N
                    ActControl = 0;//N
                    ActDataBits = 0;//N
                    ActParity = 0;//N
                    ActDestinationIONumber = 0;
                    ActDestinationPortNumber = 5002;
                    ActDidPropertyBit = 1;
                    ActDsidPropertyBit = 1;
                    ActIntelligentPreferenceBit = 0;//N
                    ActIONumber = 1023;
                    ActNetworkNumber = 0;
                    ActMultiDropChannelNumber = 0;
                    ActSourceNetworkNumber = 0;
                    ActPacketType = 1;//N
                    ActPassword = "";
                    ActPortNumber = 0;//N
                    ActProtocolType = 5;
                    ActSourceStationNumber = 0;//可能有誤
                    ActStationNumber = 255;
                    ActStopBits = 0;//N
                    ActSumCheck = 0;//N
                    ActThroughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    ActTimeOut = 10000;
                    ActCpuTimeOut = 0;//N
                    ActUnitNumber = 0;
                    ActUnitType = 26;//UNIT_QJ71E71(0x1A)
                    #endregion
                    break;
                case "Q06UDVCPU":
                    #region Q06UDVCPU
                    ActHostAddress = ActHostAddress;
                    ActConnectUnitNumber = 0;
                    ActCpuType = 211;

                    ActBaudRate = 0;//N
                    ActControl = 0;//N
                    ActDataBits = 0;//N
                    ActParity = 0;//N
                    ActDestinationIONumber = 0;
                    ActDestinationPortNumber = 5002;
                    ActDidPropertyBit = 1;
                    ActDsidPropertyBit = 1;
                    ActIntelligentPreferenceBit = 0;//N
                    ActIONumber = 1023;
                    ActNetworkNumber = 0;
                    ActMultiDropChannelNumber = 0;
                    ActSourceNetworkNumber = 0;
                    ActPacketType = 1;
                    ActPassword = ActPassword = "";
                    ActPortNumber = 0;//N
                    ActProtocolType = 5;
                    ActSourceStationNumber = 0;//可能有誤
                    ActStationNumber = 255;
                    ActStopBits = 0;//N
                    ActSumCheck = 0;//N
                    ActThroughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    ActTimeOut = 10000;
                    ActCpuTimeOut = 0;//N
                    ActUnitNumber = 0;
                    ActUnitType = 26;//UNIT_QJ71E71(0x1A)
                    #endregion
                    break;
                case "Q06HCPU":
                    #region Q06HCPU
                    ActHostAddress = ActHostAddress;
                    ActConnectUnitNumber = 0;
                    ActCpuType = 70;//Q06PHCPU

                    ActBaudRate = 0;//N
                    ActControl = 0;//N
                    ActDataBits = 0;//N
                    ActParity = 0;//N
                    ActDestinationIONumber = 0;
                    ActDestinationPortNumber = 5002;
                    ActDidPropertyBit = 1;
                    ActDsidPropertyBit = 1;
                    ActIntelligentPreferenceBit = 0;//N
                    ActIONumber = 1023;
                    ActNetworkNumber = 0;
                    ActMultiDropChannelNumber = 0;
                    ActSourceNetworkNumber = 0;
                    ActPacketType = 1;
                    ActPassword = "";
                    ActPortNumber = 0;//N
                    ActProtocolType = 5;
                    ActSourceStationNumber = 0;//可能有誤
                    ActStationNumber = 255;
                    ActStopBits = 0;//N
                    ActSumCheck = 0;//N
                    ActThroughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    ActTimeOut = 10000;
                    ActCpuTimeOut = 0;//N
                    ActUnitNumber = 0;
                    ActUnitType = 26;//UNIT_QJ71E71(0x1A)
                    #endregion
                    break;
                case "Q06UDEHCPU":
                    #region Q06UDEHCPU
                    ActHostAddress = ActHostAddress;
                    ActConnectUnitNumber = 0;
                    ActCpuType = 146;

                    ActBaudRate = 0;//N
                    ActControl = 0;//N
                    ActDataBits = 0;//N
                    ActParity = 0;
                    ActDestinationIONumber = 0;
                    ActDestinationPortNumber = 5002;
                    ActDidPropertyBit = 1;
                    ActDsidPropertyBit = 1;
                    ActIntelligentPreferenceBit = 0;//N
                    ActIONumber = 1023;
                    ActNetworkNumber = 0;
                    ActMultiDropChannelNumber = 0;
                    ActSourceNetworkNumber = 0;
                    ActPacketType = 1;
                    ActPassword = "";
                    ActPortNumber = 0;//N
                    ActProtocolType = 5;
                    ActSourceStationNumber = 0;//可能有誤
                    ActStationNumber = 255;
                    ActStopBits = 0;//N
                    ActSumCheck = 0;//N
                    ActThroughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    ActTimeOut = 10000;
                    ActCpuTimeOut = 0;//N
                    ActUnitNumber = 0;
                    ActUnitType = 26;//UNIT_QJ71E71(0x1A)
                    #endregion
                    break;
                case "R16CPU":
                    #region R16CPU
                    ActHostAddress = ActHostAddress;
                    ActConnectUnitNumber = 0;
                    ActCpuType = 4099;

                    ActBaudRate = 0;
                    ActControl = 0;
                    ActDataBits = 0;
                    ActParity = 0;
                    ActDestinationIONumber = 0;
                    ActDestinationPortNumber = 5002;
                    ActDidPropertyBit = 1;
                    ActDsidPropertyBit = 1;
                    ActIntelligentPreferenceBit = 0;
                    ActIONumber = 1023;
                    ActNetworkNumber = 0;
                    ActMultiDropChannelNumber = 0;
                    ActSourceNetworkNumber = 0;//N
                    ActPacketType = 1;
                    ActPassword = "";
                    ActPortNumber = 1;
                    ActProtocolType = 5;
                    ActSourceStationNumber = 0;//可能有誤
                    ActStationNumber = 255;//N
                    ActStopBits = 0;
                    ActSumCheck = 0;
                    ActThroughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    ActTimeOut = 10000;
                    ActCpuTimeOut = 0;
                    ActUnitNumber = 0;
                    ActUnitType = 4098;//RCPU Ethernet port connection (IP specification) 0x1002
                    #endregion
                    break;
                case "R32CPU":
                    #region R32CPU
                    ActHostAddress = ActHostAddress;
                    ActConnectUnitNumber = 0;
                    ActCpuType = 4100;

                    ActBaudRate = 0;
                    ActControl = 0;
                    ActDataBits = 0;
                    ActParity = 0;
                    ActDestinationIONumber = 0;
                    ActDestinationPortNumber = 5002;
                    ActDidPropertyBit = 1;
                    ActDsidPropertyBit = 1;
                    ActIntelligentPreferenceBit = 0;
                    ActIONumber = 1023;
                    ActNetworkNumber = 0;
                    ActMultiDropChannelNumber = 0;
                    ActSourceNetworkNumber = 0;//N
                    ActPacketType = 1;
                    ActPassword = "";
                    ActPortNumber = 1;
                    ActProtocolType = 5;
                    ActSourceStationNumber = 0;//可能有誤
                    ActStationNumber = 255;//N
                    ActStopBits = 0;
                    ActSumCheck = 0;
                    ActThroughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    ActTimeOut = 10000;
                    ActCpuTimeOut = 0;
                    ActUnitNumber = 0;
                    ActUnitType = 4098;//RCPU Ethernet port connection (IP specification) 0x1002
                    #endregion
                    break;
            }
        }
        public int ProgOpen()
        {
            //開啟Mx component功能
            iReturnCode = Open();
            return iReturnCode;
        }
        public int ProgClose()
        {
            //關閉Mx component功能
            iReturnCode = Close();
            return iReturnCode;
        }
        public void ProgGetBlockCombine(string sDevice, out string sOutCombimeValue)
        {
            ArrayList arrCombine = new ArrayList();
            string sStart;//軟元件開頭
            string sEnd;//軟元件結尾

            Break_String(sDevice, "~", ref arrCombine);//以"~"斷字:D700~D705 =>[D700][D705]
            sStart = arrCombine[0].ToString();//軟元件開頭
            sEnd = arrCombine[1].ToString();//軟元件結尾

            int iStart = Convert.ToInt32(sStart.Replace("D", ""));//軟元件開頭 改數字
            int iEnd = Convert.ToInt32(sEnd.Replace("D", ""));//軟元件結尾 改數字
            int iSize = Math.Abs(iEnd - iStart) + 1;//換算總軟元件數量
            short[] arrData = new short[iSize];//標籤總數量(矩陣)

            sOutCombimeValue = null;
            //先取出內部值
            try
            {
                iReturnCode = ReadDeviceBlock2(sStart, iSize, out arrData[0]);//從軟元件開頭 讀出資料
                Console.WriteLine(String.Format("0x{0:x8} [HEX]", iReturnCode));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", "ProgGetBlockCombime", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //以下分為字串(string)讀取與雙精度浮點(Float)讀取  以iSize做判斷標準
            if (iSize == 2)//FLOAT
            {
                if (iReturnCode == 0)
                {
                    var Low = System.BitConverter.GetBytes(arrData[0]);
                    var High = System.BitConverter.GetBytes(arrData[1]);
                    byte[] Full = new byte[High.Length + Low.Length];
                    Buffer.BlockCopy(Low, 0, Full, 0, Low.Length);
                    Buffer.BlockCopy(High, 0, Full, Low.Length, High.Length);
                    var FF = BitConverter.ToSingle(Full, 0);//轉float(single)

                    sOutCombimeValue = FF.ToString();
                }
            }
            else //string[?]
            {

                if (iReturnCode == 0)
                {
                    var total = System.BitConverter.GetBytes(arrData[0]);
                    for (int i = 1; i < arrData.Length; i++)
                    {
                        var addvalue = System.BitConverter.GetBytes(arrData[i]);
                        var Full = new byte[total.Length + addvalue.Length];
                        Buffer.BlockCopy(total, 0, Full, 0, total.Length);
                        Buffer.BlockCopy(addvalue, 0, Full, total.Length, addvalue.Length);

                        total = Full;//一直累積完全部byte
                        //sOutCombimeValue += arrData[i].ToString();//把它全部合起來[A][B][C][D][E]=>ABCDE
                    }
                    sOutCombimeValue = System.Text.Encoding.ASCII.GetString(total);
                }
            }
        }
        public void ProgSetBlockCombine(string sDevice, string sInCombimeValue)
        {
            ArrayList arrCombine = new ArrayList();
            string sStart;//軟元件開頭
            string sEnd;//軟元件結尾

            Break_String(sDevice, "~", ref arrCombine);//以"~"斷字:D700~D705 =>[D700][D705]
            sStart = arrCombine[0].ToString();//軟元件開頭
            sEnd = arrCombine[1].ToString();//軟元件結尾

            int iStart = Convert.ToInt32(sStart.Replace("D", ""));//軟元件開頭 改數字
            int iEnd = Convert.ToInt32(sEnd.Replace("D", ""));//軟元件結尾 改數字
            int iSize = Math.Abs(iEnd - iStart) + 1;//換算總軟元件數量
            short[] arrData = new short[iSize];//標籤總數量(矩陣)

            //先判斷寫入格式
            if (iSize == 2)//Float
            {
                byte[] sp = BitConverter.GetBytes(Convert.ToSingle(sInCombimeValue));//byte共4位

                arrData[0] = BitConverter.ToInt16(sp, 0);
                arrData[1] = BitConverter.ToInt16(sp, 2);

                try
                {
                    iReturnCode = WriteDeviceBlock2(sStart, iSize, ref arrData[0]);//從軟元件開頭 寫入出資料
                    Console.WriteLine(String.Format("0x{0:x8} [HEX]", iReturnCode));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", "ProgSetBlockCombime", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else //string[?]
            {
                while (sInCombimeValue.Length < iSize * 2)//若值不為 軟元件數*元件容量則需補值 超過不管
                {
                    sInCombimeValue = "0" + sInCombimeValue;//沒有就補0在前面
                }
                //鍵入寫入資料

                for (int i = 0, j = 0; i < iSize; i++, j += 2)
                {
                    //arrData[i] = Convert.ToInt16(sInCombimeValue.Substring(j, 2));//word每個兩字元0123456789=>[01][23][45][67][89]
                    //每兩個一組轉換成ASCII的byte再轉為輸出用的short
                    arrData[i] = BitConverter.ToInt16(Encoding.ASCII.GetBytes(sInCombimeValue.Substring(j, 2)), 0);
                }
                try
                {
                    iReturnCode = WriteDeviceBlock2(sStart, iSize, ref arrData[0]);//從軟元件開頭 寫入出資料
                    Console.WriteLine(String.Format("0x{0:x8} [HEX]", iReturnCode));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", "ProgSetBlockCombime", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }
        public void ProgGetDeviceRandom(string arrsDevice, int iSize, out short[] arrDeviceData)
        {
            arrDeviceData = new short[iSize];
            try
            {
                iReturnCode = ReadDeviceRandom2(arrsDevice, iSize, out arrDeviceData[0]);
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
            int Data = 0;
            Data = Convert.ToInt32(sInValue);
            try
            {
                iReturnCode = SetDevice(sDevice, Data);//從軟元件開頭 讀出資料
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", "ProgGetDevice", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //---------------------------------------------------------------------------------------
        public string ErrorStrSend()
        {
            string sOutStr = "";
            int iErrorCode = iReturnCode;
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
        //---------------------------------------------------------------------------------------
        public void Break_String(string sData, string sBreakItem, ref ArrayList list)
        {
            list.Clear();
            string[] Spare = sData.Split(new string[] { sBreakItem }, StringSplitOptions.None);
            foreach (string item in Spare)//餵回去list
            {
                list.Add(item);
            }
        }
    }
    public class CDataStruct
    {
        /// <summary>流水號</summary>
        public string SN { get; set; }
        /// <summary>軟元件名稱</summary>
        public string Label { get; set; }
        /// <summary>軟元件位置範圍</summary>
        public string Address { get; set; }
        /// <summary>元件組類別</summary>
        public string DataType { get; set; }
        /// <summary>數據</summary>
        public string Data { get; set; }
        /// <summary>是否使用該值</summary>
        public string IsUse { get; set; }
        /// <summary>軟元件讀取值</summary>
        public string DeviceValueGet { get; set; }
        /// <summary>軟元件寫入值</summary>
        public string DeviceValueSet { get; set; }

        public CDataStruct()
        {
            SN = "";
            Label = "";
            Address = "";
            DataType = "";
            Data = "";
            IsUse = "0";
            DeviceValueGet = "N/A";
            DeviceValueSet = "";
        }

    }
}
