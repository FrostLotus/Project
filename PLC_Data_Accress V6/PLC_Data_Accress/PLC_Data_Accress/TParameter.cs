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
    
    public class TParameter
    {
        
        public static CMX_Component Mx_Connect = new CMX_Component();
        public static CDeviceData DeviceData = new CDeviceData();
        public static CError_Info Error_Info = new CError_Info();

        public static void Init()
        {
            DeviceData.LoadData_Full();//先進行參數餵入
            Mx_Connect.SetPLCProperty(Mx_Connect.CpuName);//再進行PLC連線(不論有無連線成功)
            TParameter.Mx_Connect.iReturnCode = TParameter.Mx_Connect.ProgOpen();//連線測試
        }
    }
    public class CDeviceData
    {
        public class CDataStruct
        {
            public string SN { get; set; }
            public string Label { get; set; }
            public string Address { get; set; }
            public string DataType { get; set; }
            public string Data { get; set; }
            public string IsUse { get; set; }
            public string DeviceValueGet { get; set; }
            public string DeviceValueSet { get; set; }
        }
        //路徑
        public string DefaultPath = System.Windows.Forms.Application.StartupPath + @"\Datatext";
        //檔案
        public string DataFile_Path;
        public string DeviceDataGrid_Path;
        public string DeviceModelList_Path;
        //檔案陣列
        public string[] arrDeviceDataGrid_Read;
        public string[] arrDeviceDataGrid_Write;
        public string[] arrDeviceModel_List;
        ///軟元件列表
        //讀列表
        List<CDataStruct> lReadData = new List<CDataStruct>();
        public List<string> Read_SN             = new List<string>();//序號
        public List<string> Read_Label          = new List<string>();//名稱
        public List<string> Read_Address        = new List<string>();//軟元件地址
        public List<string> Read_DataType       = new List<string>();//資料類型
        public List<string> Read_Data           = new List<string>();//資料包格式
        public List<string> Read_IsUse          = new List<string>();//是否使用
        public List<string> Read_DeviceValueGet = new List<string>();//讀取值

        public DataGridViewRow[] ReadGridRow;//下載DataGridview
        //寫列表
        List<CDataStruct> lWriteData = new List<CDataStruct>();
        public List<string> Write_SN             = new List<string>();//序號
        public List<string> Write_Label          = new List<string>();//名稱
        public List<string> Write_Address        = new List<string>();//軟元件地址
        public List<string> Write_DataType       = new List<string>();//資料類型
        public List<string> Write_Data           = new List<string>();//資料包格式
        public List<string> Write_IsUse          = new List<string>();//是否使用
        public List<string> Write_DeviceValueGet = new List<string>();//讀取值
        public List<string> Write_DeviceValueSet = new List<string>();//寫入值

        public DataGridViewRow[] WriteGridRow;//上傳DataGridview

        public int iModelChange = 0;//模式轉換  0=編輯 1=運作中
        public int iPLCConnect = 1;//連線中

        //=================================================================
        public CDeviceData()
        {

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
                    DeviceDataGrid_Path = DefaultPath + "\\DefaultDataGrid.txt";
                    DeviceModelList_Path = DefaultPath + "\\DefaultModelList.txt";
                    createfile.WriteLine(DeviceDataGrid_Path);//預設軟元件資料表
                    createfile.WriteLine(DeviceModelList_Path);//預設PLC資料表
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
                    DeviceDataGrid_Path = DataList[0].ToString();//軟元件資料表路徑
                    DeviceModelList_Path = DataList[1].ToString();//PLC資料表路徑
                }
            }
        }
        //讀取軟元件資料表
        public void LoadData_ReadWrite()
        {
            //判斷有無讀寫資料表這個檔案
            #region 沒有就創建且改回預設路徑
            if (!File.Exists(DeviceDataGrid_Path))
            {
                DeviceDataGrid_Path = DefaultPath + "\\DefaultDataGrid.txt";
                using (StreamWriter createfile = File.CreateText(DeviceDataGrid_Path))
                {
                    //改寫預設路徑檔案
                    using (StreamWriter writer = new StreamWriter(TParameter.DeviceData.DataFile_Path))
                    {
                        writer.Write("");//清除
                        writer.WriteLine(TParameter.DeviceData.DeviceDataGrid_Path);
                        writer.WriteLine(TParameter.DeviceData.DeviceModelList_Path);
                        writer.Close();
                    }
                    MessageBox.Show("無指定資料表,已生成");
                    createfile.Close();
                }
            }
            #endregion

            #region 判斷後讀取
            string sData;
            using (StreamReader reader = new StreamReader(DeviceDataGrid_Path))
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
                    DiviceDataArrClear();//清空目前有的資料
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
                                Read_SN.Add(SigleData_List[0].ToString());//序號
                                Read_Label.Add(SigleData_List[1].ToString());//名稱
                                Read_Address.Add(SigleData_List[2].ToString());//地址
                                Read_DataType.Add(SigleData_List[3].ToString());//數據類型
                                Read_Data.Add(SigleData_List[4].ToString());//值
                                Read_IsUse.Add(SigleData_List[5].ToString());//是否使用
                                Read_DeviceValueGet.Add("N/A");
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
                                Write_SN.Add(SigleData_List[0].ToString());//序號
                                Write_Label.Add(SigleData_List[1].ToString());//名稱
                                Write_Address.Add(SigleData_List[2].ToString());//地址
                                Write_DataType.Add(SigleData_List[3].ToString());//數據類型
                                Write_Data.Add(SigleData_List[4].ToString());//值
                                Write_IsUse.Add(SigleData_List[5].ToString());//是否使用
                                Write_DeviceValueGet.Add("N/A");
                                Write_DeviceValueSet.Add("");
                            }
                        }
                    }
                    #endregion

                    #region 參數
                    TParameter.Mx_Connect.iReciveTime = Convert.ToInt32(DataList[2].ToString());//回傳時間
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
            if (!File.Exists(DeviceModelList_Path))
            {
                DeviceModelList_Path = DefaultPath + "\\DeviceModelList.txt";
                using (StreamWriter createfile = File.CreateText(DeviceModelList_Path))//沒有就創建
                {
                    //改寫預設值
                    createfile.WriteLine(TParameter.Mx_Connect.CpuName);//預設值
                    createfile.WriteLine(TParameter.Mx_Connect._ActHostAddress);//預設值
                    //改寫預設路徑txt
                    using (StreamWriter writer = new StreamWriter(TParameter.DeviceData.DataFile_Path))
                    {
                        writer.Write("");//清除
                        writer.WriteLine(DeviceDataGrid_Path);
                        writer.WriteLine(DeviceModelList_Path);
                        writer.Close();
                    }
                    MessageBox.Show("無指定資料表,已生成");
                    createfile.Close();
                }
            }
            //讀取
            string sData;
            using (StreamReader reader = new StreamReader(DeviceModelList_Path))
            {
                sData = reader.ReadToEnd();
                reader.Close();
            }
            ArrayList DataList = new ArrayList();
            Break_String(sData, "\r\n", ref DataList);//(以\r\n分隔)
            if (DataList.Count != 0)
            {
                TParameter.Mx_Connect.CpuName = DataList[0].ToString();//CpuName
                TParameter.Mx_Connect._ActHostAddress = DataList[1].ToString();//Address
            }
        }
        ///---------------
        //寫入對應資料表
        public void SaveData_ReadWrite()
        {
            if (!File.Exists(DeviceDataGrid_Path))//判斷是不是有這檔案
            {
                using (StreamWriter createfile = File.CreateText(DeviceDataGrid_Path))//沒有就創建
                {
                    MessageBox.Show("無指定資料表,已生成");
                    createfile.Close();
                }
            }
            using (StreamWriter writer = new StreamWriter(DeviceDataGrid_Path)) //寫入矩陣資料
            {
                writer.Write("");//清除
                string LineData;
                //序號$名稱$地址$數據類型$值$是否使用(0/1)&&
                if (Read_SN.Count != 0)
                {
                    for (int i = 0; i < Read_SN.Count; i++)//Read
                    {
                        //串起來
                        LineData = Read_SN[i] + "$" +
                                   Read_Label[i] + "$" +
                                   Read_Address[i] + "$" +
                                   Read_DataType[i] + "$" +
                                   Read_Data[i] + "$" +
                                   Read_IsUse[i] + "&&";
                        writer.WriteLine(LineData);//寫入
                    }
                }
                writer.WriteLine("!!");//分隔Read/Write
                if (Write_SN.Count != 0)
                {
                    for (int i = 0; i < Write_SN.Count; i++)//Write
                    {
                        //串起來
                        LineData = Write_SN[i] + "$" +
                                   Write_Label[i] + "$" +
                                   Write_Address[i] + "$" +
                                   Write_DataType[i] + "$" +
                                   Write_Data[i] + "$" +
                                   Write_IsUse[i] + "&&";
                        writer.WriteLine(LineData);
                    }
                }
                writer.WriteLine("!!");//分隔Write/參數
                writer.WriteLine(TParameter.Mx_Connect.iReciveTime);//參數
                writer.Close();
            }
        }
        public void SaveData_Model()
        {
            if (!File.Exists(DeviceDataGrid_Path))//判斷是不是有這檔案
            {
                using (StreamWriter createfile = File.CreateText(DeviceDataGrid_Path))//沒有就創建
                {
                    MessageBox.Show("無指定資料表,已生成");
                    createfile.Close();
                }
            }
            using (StreamWriter writer = new StreamWriter(DeviceModelList_Path))//寫入矩陣資料
            {
                writer.Write("");//清除
                writer.WriteLine(TParameter.Mx_Connect.CpuName);
                writer.WriteLine(TParameter.Mx_Connect._ActHostAddress);
                writer.Close();
            }
        }
        public void SaveData_ModChange(DataGridView dgv_Read, DataGridView dgv_Write)
        {
            if (iModelChange == 0)//修改=>執行
            {
                iModelChange = 1;
                //下載(讀)資料:全鎖定
                dgv_Read.ReadOnly = true;
                //上傳(讀寫)資料:把除了修改值(可以變動)以外的在修改模式中鎖定
                dgv_Write.Columns[0].ReadOnly = true;
                dgv_Write.Columns[1].ReadOnly = true;
                dgv_Write.Columns[2].ReadOnly = true;
                dgv_Write.Columns[3].ReadOnly = true;
                dgv_Write.Columns[4].ReadOnly = true;
                dgv_Write.Columns[5].ReadOnly = true;
                dgv_Write.Columns[6].ReadOnly = true;
            }
            //執行=>修改
            else if (iModelChange == 1)
            {
                iModelChange = 0;
                dgv_Read.ReadOnly = false;
                dgv_Write.Columns[0].ReadOnly = false;
                dgv_Write.Columns[1].ReadOnly = false;
                dgv_Write.Columns[2].ReadOnly = false;
                dgv_Write.Columns[3].ReadOnly = false;
                dgv_Write.Columns[4].ReadOnly = false;
                dgv_Write.Columns[5].ReadOnly = false;
                dgv_Write.Columns[6].ReadOnly = false;
            }
            return;
        }
        ///---------------------------------------------
        public void GetCombineArray_str(string sDevice,out int iSize, out string sOutCombimeArray)
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
            iSize = Math.Abs(iEnd - iStart)+1;//換算總軟元件數量

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
            Read_SN.Clear();
            Read_Label.Clear();
            Read_Address.Clear();
            Read_DataType.Clear();
            Read_Data.Clear();
            Read_IsUse.Clear();
            //Write
            Write_SN.Clear();
            Write_Label.Clear();
            Write_Address.Clear();
            Write_DataType.Clear();
            Write_Data.Clear();
            Write_IsUse.Clear();
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
    }
    public class CMX_Component : ActProgTypeClass
    {
        
        //public ActSupportMsgClass SpMsg_Connect = new ActSupportMsgClass();//Message

        #region Property 連線參數

        public int iReturnCode = 0;//回傳代碼
        public int iReciveTime = 2000;//回傳時間

        public string CpuName = "FX5UCPU";//CPU名稱
        public int CPUType = 528;//CPU名稱:代碼

        public string _ActHostAddress = "192.168.2.12";//IP
        // 預設值走FX5U-32M
        public int _ActConnectUnitNumber = 0;
        public int _ActBaudRate = 0;
        public int _ActControl = 0;
        public int _ActCpuType = 528;
        public int _ActDataBits = 0;
        public int _ActDestinationIONumber = 0;
        public int _ActDestinationPortNumber = 5562;
        public int _ActDidPropertyBit = 1;
        public int _ActDsidPropertyBit = 1;
        public int _ActIntelligentPreferenceBit = 0;
        public int _ActIONumber = 1023;
        public int _ActNetworkNumber = 0;
        public int _ActMultiDropChannelNumber = 0;
        public int _ActSourceNetworkNumber = 0;

        public int _ActPacketType = 1;
        public string _ActPassword = "";
        public int _ActPortNumber = 0;
        public int _ActProtocolType = 5;
        public int _ActSourceStationNumber = 5;
        public int _ActStationNumber = 255;
        public int _ActStopBits = 0;
        public int _ActSumCheck = 0;
        public int _ActThoughNetworkType = 1;
        public int _ActTimeOut = 100;
        public int _ActUnitNumber = 0;
        public int _ActUnitType = 8193;
        public int _ActParity = 0;
        public int _ActCpuTimeOut = 0;
        #endregion
        //-------------------------
        public CMX_Component()
        {

        }
        //-------------------------
        public void SetPLCProperty(string sType)
        {
            //屬性列表Property list
            switch (sType)
            {
                case "FX5UCPU":
                    #region FX5UCPU

                    TParameter.Mx_Connect.Prog_Connect.ActHostAddress = _ActHostAddress;
                    TParameter.Mx_Connect.Prog_Connect.ActConnectUnitNumber = _ActConnectUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuType = _ActCpuType = 528;

                    TParameter.Mx_Connect.Prog_Connect.ActBaudRate = _ActBaudRate = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActControl = _ActControl = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActDataBits = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActDataBits = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationIONumber = _ActDestinationIONumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationPortNumber = _ActDestinationPortNumber = 5562;
                    TParameter.Mx_Connect.Prog_Connect.ActDidPropertyBit = _ActDidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActDsidPropertyBit = _ActDsidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActIntelligentPreferenceBit = _ActIntelligentPreferenceBit = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActIONumber = _ActIONumber = 1023;
                    TParameter.Mx_Connect.Prog_Connect.ActNetworkNumber = _ActNetworkNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActMultiDropChannelNumber = _ActMultiDropChannelNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceNetworkNumber = _ActSourceNetworkNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActPacketType = _ActPacketType = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActPassword = _ActPassword = "";
                    TParameter.Mx_Connect.Prog_Connect.ActPortNumber = _ActPortNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActProtocolType = _ActProtocolType = 5;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceStationNumber = _ActSourceStationNumber = 5;
                    TParameter.Mx_Connect.Prog_Connect.ActStationNumber = _ActStationNumber = 255;
                    TParameter.Mx_Connect.Prog_Connect.ActStopBits = _ActStopBits = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActSumCheck = _ActSumCheck = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActThroughNetworkType = _ActThoughNetworkType = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActTimeOut = _ActTimeOut = 10000;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuTimeOut = _ActCpuTimeOut = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActUnitNumber = _ActUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActUnitType = _ActUnitType = 8193;

                    #endregion
                    break;
                case "Q13UDEHCPU":
                    #region Q13UDEHCPU
                    TParameter.Mx_Connect.Prog_Connect.ActHostAddress = _ActHostAddress;

                    TParameter.Mx_Connect.Prog_Connect.ActConnectUnitNumber = _ActConnectUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuType = _ActCpuType = 147;//Q13UDEHCPU

                    TParameter.Mx_Connect.Prog_Connect.ActBaudRate = _ActBaudRate = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActControl = _ActControl = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActDataBits = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActParity = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationIONumber = _ActDestinationIONumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationPortNumber = _ActDestinationPortNumber = 5002;
                    TParameter.Mx_Connect.Prog_Connect.ActDidPropertyBit = _ActDidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActDsidPropertyBit = _ActDsidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActIntelligentPreferenceBit = _ActIntelligentPreferenceBit = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActIONumber = _ActIONumber = 1023;
                    TParameter.Mx_Connect.Prog_Connect.ActNetworkNumber = _ActNetworkNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActMultiDropChannelNumber = _ActMultiDropChannelNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceNetworkNumber = _ActSourceNetworkNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActPacketType = _ActPacketType = 1;//N
                    TParameter.Mx_Connect.Prog_Connect.ActPassword = _ActPassword = "";
                    TParameter.Mx_Connect.Prog_Connect.ActPortNumber = _ActPortNumber = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActProtocolType = _ActProtocolType = 5;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceStationNumber = _ActSourceStationNumber = 0;//可能有誤
                    TParameter.Mx_Connect.Prog_Connect.ActStationNumber = _ActStationNumber = 255;
                    TParameter.Mx_Connect.Prog_Connect.ActStopBits = _ActStopBits = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActSumCheck = _ActSumCheck = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActThroughNetworkType = _ActThoughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    TParameter.Mx_Connect.Prog_Connect.ActTimeOut = _ActTimeOut = 10000;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuTimeOut = _ActCpuTimeOut = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActUnitNumber = _ActUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActUnitType = _ActUnitType = 26;//UNIT_QJ71E71(0x1A)
                    #endregion
                    break;
                case "Q06UDVCPU":
                    #region Q06UDVCPU
                    TParameter.Mx_Connect.Prog_Connect.ActHostAddress = _ActHostAddress;
                    TParameter.Mx_Connect.Prog_Connect.ActConnectUnitNumber = _ActConnectUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuType = _ActCpuType = 211;

                    TParameter.Mx_Connect.Prog_Connect.ActBaudRate = _ActBaudRate = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActControl = _ActControl = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActDataBits = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActParity = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationIONumber = _ActDestinationIONumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationPortNumber = _ActDestinationPortNumber = 5002;
                    TParameter.Mx_Connect.Prog_Connect.ActDidPropertyBit = _ActDidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActDsidPropertyBit = _ActDsidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActIntelligentPreferenceBit = _ActIntelligentPreferenceBit = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActIONumber = _ActIONumber = 1023;
                    TParameter.Mx_Connect.Prog_Connect.ActNetworkNumber = _ActNetworkNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActMultiDropChannelNumber = _ActMultiDropChannelNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceNetworkNumber = _ActSourceNetworkNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActPacketType = _ActPacketType = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActPassword = _ActPassword = "";
                    TParameter.Mx_Connect.Prog_Connect.ActPortNumber = _ActPortNumber = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActProtocolType = _ActProtocolType = 5;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceStationNumber = _ActSourceStationNumber = 0;//可能有誤
                    TParameter.Mx_Connect.Prog_Connect.ActStationNumber = _ActStationNumber = 255;
                    TParameter.Mx_Connect.Prog_Connect.ActStopBits = _ActStopBits = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActSumCheck = _ActSumCheck = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActThroughNetworkType = _ActThoughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    TParameter.Mx_Connect.Prog_Connect.ActTimeOut = _ActTimeOut = 10000;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuTimeOut = _ActCpuTimeOut = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActUnitNumber = _ActUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActUnitType = _ActUnitType = 26;//UNIT_QJ71E71(0x1A)
                    #endregion
                    break;
                case "Q06HCPU":
                    #region Q06HCPU
                    TParameter.Mx_Connect.Prog_Connect.ActHostAddress = _ActHostAddress;
                    TParameter.Mx_Connect.Prog_Connect.ActConnectUnitNumber = _ActConnectUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuType = _ActCpuType = 70;//Q06PHCPU

                    TParameter.Mx_Connect.Prog_Connect.ActBaudRate = _ActBaudRate = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActControl = _ActControl = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActDataBits = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActParity = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationIONumber = _ActDestinationIONumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationPortNumber = _ActDestinationPortNumber = 5002;
                    TParameter.Mx_Connect.Prog_Connect.ActDidPropertyBit = _ActDidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActDsidPropertyBit = _ActDsidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActIntelligentPreferenceBit = _ActIntelligentPreferenceBit = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActIONumber = _ActIONumber = 1023;
                    TParameter.Mx_Connect.Prog_Connect.ActNetworkNumber = _ActNetworkNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActMultiDropChannelNumber = _ActMultiDropChannelNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceNetworkNumber = _ActSourceNetworkNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActPacketType = _ActPacketType = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActPassword = _ActPassword = "";
                    TParameter.Mx_Connect.Prog_Connect.ActPortNumber = _ActPortNumber = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActProtocolType = _ActProtocolType = 5;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceStationNumber = _ActSourceStationNumber = 0;//可能有誤
                    TParameter.Mx_Connect.Prog_Connect.ActStationNumber = _ActStationNumber = 255;
                    TParameter.Mx_Connect.Prog_Connect.ActStopBits = _ActStopBits = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActSumCheck = _ActSumCheck = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActThroughNetworkType = _ActThoughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    TParameter.Mx_Connect.Prog_Connect.ActTimeOut = _ActTimeOut = 10000;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuTimeOut = _ActCpuTimeOut = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActUnitNumber = _ActUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActUnitType = _ActUnitType = 26;//UNIT_QJ71E71(0x1A)
                    #endregion
                    break;
                case "Q06UDEHCPU":
                    #region Q06UDEHCPU
                    TParameter.Mx_Connect.Prog_Connect.ActHostAddress = _ActHostAddress;
                    TParameter.Mx_Connect.Prog_Connect.ActConnectUnitNumber = _ActConnectUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuType = _ActCpuType = 146;

                    TParameter.Mx_Connect.Prog_Connect.ActBaudRate = _ActBaudRate = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActControl = _ActControl = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActDataBits = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActParity = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationIONumber = _ActDestinationIONumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationPortNumber = _ActDestinationPortNumber = 5002;
                    TParameter.Mx_Connect.Prog_Connect.ActDidPropertyBit = _ActDidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActDsidPropertyBit = _ActDsidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActIntelligentPreferenceBit = _ActIntelligentPreferenceBit = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActIONumber = _ActIONumber = 1023;
                    TParameter.Mx_Connect.Prog_Connect.ActNetworkNumber = _ActNetworkNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActMultiDropChannelNumber = _ActMultiDropChannelNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceNetworkNumber = _ActSourceNetworkNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActPacketType = _ActPacketType = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActPassword = _ActPassword = "";
                    TParameter.Mx_Connect.Prog_Connect.ActPortNumber = _ActPortNumber = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActProtocolType = _ActProtocolType = 5;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceStationNumber = _ActSourceStationNumber = 0;//可能有誤
                    TParameter.Mx_Connect.Prog_Connect.ActStationNumber = _ActStationNumber = 255;
                    TParameter.Mx_Connect.Prog_Connect.ActStopBits = _ActStopBits = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActSumCheck = _ActSumCheck = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActThroughNetworkType = _ActThoughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    TParameter.Mx_Connect.Prog_Connect.ActTimeOut = _ActTimeOut = 10000;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuTimeOut = _ActCpuTimeOut = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActUnitNumber = _ActUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActUnitType = _ActUnitType = 26;//UNIT_QJ71E71(0x1A)
                    #endregion
                    break;
                case "R16CPU":
                    #region R16CPU
                    TParameter.Mx_Connect.Prog_Connect.ActHostAddress = _ActHostAddress;
                    TParameter.Mx_Connect.Prog_Connect.ActConnectUnitNumber = _ActConnectUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuType = _ActCpuType = 4099;

                    TParameter.Mx_Connect.Prog_Connect.ActBaudRate = _ActBaudRate = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActControl = _ActControl = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActDataBits = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActParity = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationIONumber = _ActDestinationIONumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationPortNumber = _ActDestinationPortNumber = 5002;
                    TParameter.Mx_Connect.Prog_Connect.ActDidPropertyBit = _ActDidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActDsidPropertyBit = _ActDsidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActIntelligentPreferenceBit = _ActIntelligentPreferenceBit = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActIONumber = _ActIONumber = 1023;
                    TParameter.Mx_Connect.Prog_Connect.ActNetworkNumber = _ActNetworkNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActMultiDropChannelNumber = _ActMultiDropChannelNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceNetworkNumber = _ActSourceNetworkNumber = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActPacketType = _ActPacketType = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActPassword = _ActPassword = "";
                    TParameter.Mx_Connect.Prog_Connect.ActPortNumber = _ActPortNumber = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActProtocolType = _ActProtocolType = 5;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceStationNumber = _ActSourceStationNumber = 0;//可能有誤
                    TParameter.Mx_Connect.Prog_Connect.ActStationNumber = _ActStationNumber = 255;//N
                    TParameter.Mx_Connect.Prog_Connect.ActStopBits = _ActStopBits = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActSumCheck = _ActSumCheck = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActThroughNetworkType = _ActThoughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    TParameter.Mx_Connect.Prog_Connect.ActTimeOut = _ActTimeOut = 10000;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuTimeOut = _ActCpuTimeOut = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActUnitNumber = _ActUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActUnitType = _ActUnitType = 4098;//RCPU Ethernet port connection (IP specification) 0x1002
                    #endregion
                    break;
                case "R32CPU":
                    #region R32CPU
                    TParameter.Mx_Connect.Prog_Connect.ActHostAddress = _ActHostAddress;
                    TParameter.Mx_Connect.Prog_Connect.ActConnectUnitNumber = _ActConnectUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuType = _ActCpuType = 4100;

                    TParameter.Mx_Connect.Prog_Connect.ActBaudRate = _ActBaudRate = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActControl = _ActControl = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActDataBits = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDataBits = _ActParity = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationIONumber = _ActDestinationIONumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActDestinationPortNumber = _ActDestinationPortNumber = 5002;
                    TParameter.Mx_Connect.Prog_Connect.ActDidPropertyBit = _ActDidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActDsidPropertyBit = _ActDsidPropertyBit = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActIntelligentPreferenceBit = _ActIntelligentPreferenceBit = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActIONumber = _ActIONumber = 1023;
                    TParameter.Mx_Connect.Prog_Connect.ActNetworkNumber = _ActNetworkNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActMultiDropChannelNumber = _ActMultiDropChannelNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceNetworkNumber = _ActSourceNetworkNumber = 0;//N
                    TParameter.Mx_Connect.Prog_Connect.ActPacketType = _ActPacketType = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActPassword = _ActPassword = "";
                    TParameter.Mx_Connect.Prog_Connect.ActPortNumber = _ActPortNumber = 1;
                    TParameter.Mx_Connect.Prog_Connect.ActProtocolType = _ActProtocolType = 5;
                    TParameter.Mx_Connect.Prog_Connect.ActSourceStationNumber = _ActSourceStationNumber = 0;//可能有誤
                    TParameter.Mx_Connect.Prog_Connect.ActStationNumber = _ActStationNumber = 255;//N
                    TParameter.Mx_Connect.Prog_Connect.ActStopBits = _ActStopBits = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActSumCheck = _ActSumCheck = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActThroughNetworkType = _ActThoughNetworkType = 1;//MELSECNET/10 is included.: 1 (0x01)
                    TParameter.Mx_Connect.Prog_Connect.ActTimeOut = _ActTimeOut = 10000;
                    TParameter.Mx_Connect.Prog_Connect.ActCpuTimeOut = _ActCpuTimeOut = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActUnitNumber = _ActUnitNumber = 0;
                    TParameter.Mx_Connect.Prog_Connect.ActUnitType = _ActUnitType = 4098;//RCPU Ethernet port connection (IP specification) 0x1002
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

            TParameter.DeviceData.Break_String(sDevice, "~", ref arrCombine);//以"~"斷字:D700~D705 =>[D700][D705]
            sStart = arrCombine[0].ToString();//軟元件開頭
            sEnd = arrCombine[1].ToString();//軟元件結尾

            int iStart = Convert.ToInt32(sStart.Replace("D", ""));//軟元件開頭 改數字
            int iEnd = Convert.ToInt32(sEnd.Replace("D", ""));//軟元件結尾 改數字
            int iSize = Math.Abs(iEnd - iStart)+1;//換算總軟元件數量
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
            int iSize = Math.Abs(iEnd - iStart)+1;//換算總軟元件數量
            int[] arrData = new int[iSize];//標籤總數量(矩陣)

            //確認寫入資料有無問題  Substring
            for (int i = 0, j = 0; i < iSize; i++, j += 2)
            {

                arrData[i] = Convert.ToInt32(sInCombimeValue.Substring(j, 2));//word每個兩字元0123456789=>[01][23][45][67][89]
            }
            try
            {
                iReturnCode = Prog_Connect.WriteDeviceBlock(sStart, iSize, ref arrData[0]);//從軟元件開頭 讀出資料
                //Console.WriteLine(String.Format("0x{0:x8} [HEX]", iReturnCode));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", "ProgSetBlockCombime", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ProgGetDeviceRandom(string arrsDevice,int iSize,out int[] arrDeviceData)
        {
            arrDeviceData = new int[iSize];
            try
            {
                iReturnCode = Prog_Connect.ReadDeviceRandom(arrsDevice, iSize, out arrDeviceData[0]);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ProgGetDevice(string sDevice, out string sOutValue)
        {
            int iData=0;
            try
            {
                iReturnCode = Prog_Connect.GetDevice(sDevice,out iData) ;//從軟元件開頭 讀出資料
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
                iReturnCode = Prog_Connect.SetDevice(sDevice, Data);//從軟元件開頭 讀出資料
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", "ProgGetDevice", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class CError_Info
    {
        public CError_Info()
        {

        }

        public string ErrorStrSend()
        {
            string sOutStr;
            int iErrorCode = TParameter.Mx_Connect.iReturnCode;

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
