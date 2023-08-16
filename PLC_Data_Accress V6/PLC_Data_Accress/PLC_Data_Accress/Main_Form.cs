using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ActProgTypeLib;
using ActSupportMsgLib;
using System.IO;
using System.Collections;

namespace PLC_Data_Access
{
    public partial class Main_Form : Form
    {
        public System.Timers.Timer Timer_DeviceGet = new System.Timers.Timer();

        delegate void UpdateLabel(Label lab, string Msg);
        delegate void UpdateDataGridView(DataGridView view, DataGridViewRow[] data);
        //private object _objLock = new object();
        public Stopwatch swStopwatch = new Stopwatch();

        public Main_Form()
        {
            InitializeComponent();

            TParameter.Init();//初始化 然後連線

            txt_ReadTime.Text = TParameter.Mx_Connect.iReciveTime.ToString();
            //直接路徑連結
            if (TParameter.Mx_Connect.iReturnCode == 0)
            {
                p_MxOpenStatus.BackColor = Color.Lime;
                btn_MxOpen.Text = "PLC連結中";
            }
            else
            {
                p_MxOpenStatus.BackColor = Color.Red;
                btn_MxOpen.Text = "PLC連結失敗";
            }
            //DeviceGet事件
            Timer_DeviceGet.Stop();
            Timer_DeviceGet.Interval = TParameter.Mx_Connect.iReciveTime;

            DataGridValueFlash();//DataGridview 第一次刷新

        }

        private void Mi_DataGridLoad_Click(object sender, EventArgs e)
        {
            if (TParameter.DeviceData.iModelChange != 1)
            {
                OpenFileDialog DataFileForm = new OpenFileDialog();
                if (DataFileForm.ShowDialog() == DialogResult.OK)
                {
                    string path;
                    path = DataFileForm.FileName.ToString();
                    if (!DataFileForm.FileName.ToString().Contains(".txt"))
                    {
                        path = DataFileForm.FileName.ToString() + ".txt";
                    }
                    TParameter.DeviceData.DeviceDataGrid_Path = path;
                    //取代預設路徑檔案中的指定資料表路徑
                    using (StreamWriter writer = new StreamWriter(TParameter.DeviceData.DataFile_Path))
                    {
                        writer.Write("");//清除
                        writer.WriteLine(TParameter.DeviceData.DeviceDataGrid_Path);
                        writer.WriteLine(TParameter.DeviceData.DeviceModelList_Path);
                        writer.Close();
                    }

                    TParameter.DeviceData.LoadData_ReadWrite();
                    dgv_ReadDataGrid.Rows.Clear();
                    dgv_WriteDataGrid.Rows.Clear();
                    //DataGridLabelFlash();
                    DataGridValueFlash();

                }
            }
            else
            {
                MessageBox.Show("格式運作中 無法載入", "DataGridLoad", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        private void Mi_DataGridSave_Click(object sender, EventArgs e)
        {
            //儲存Datagridview資料
            SaveFileDialog DataFileForm = new SaveFileDialog();

            if (DataFileForm.ShowDialog() == DialogResult.OK)
            {
                //若不相同則取代
                //防呆
                string path;
                path = DataFileForm.FileName.ToString();
                if (!DataFileForm.FileName.ToString().Contains(".txt"))
                {
                    path = DataFileForm.FileName.ToString() + ".txt";
                }
                TParameter.DeviceData.DeviceDataGrid_Path = path;
                //取代預設路徑檔案中的指定資料表路徑
                using (StreamWriter writer = new StreamWriter(TParameter.DeviceData.DataFile_Path))
                {
                    writer.Write("");//清除
                    writer.WriteLine(TParameter.DeviceData.DeviceDataGrid_Path);
                    writer.WriteLine(TParameter.DeviceData.DeviceModelList_Path);
                    writer.Close();
                }

                TParameter.DeviceData.SaveData_ReadWrite();

                MessageBox.Show("儲存成功", "格式儲存", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void Mi_PLCSet_Click(object sender, EventArgs e)
        {
            TParameter.Mx_Connect.ProgClose();//先關閉
            Form_PLC_Set form = new Form_PLC_Set();
            if (form.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("完成PLC設定");
                TParameter.DeviceData.SaveData_Model();//儲存PLC連線資料
                if (TParameter.Mx_Connect.iReturnCode == 0)
                {
                    p_MxOpenStatus.BackColor = Color.Lime;
                    btn_MxOpen.Text = "PLC連結中";
                }
                if (TParameter.Mx_Connect.iReturnCode != 0)
                {
                    p_MxOpenStatus.BackColor = Color.Red;
                    btn_MxOpen.Text = "PLC連結失敗";
                }
            }
        }

        private void Btn_ModelChange_Click(object sender, EventArgs e)
        {
            ModelChange();
        }
        private void Btn_DataUpdate_Click(object sender, EventArgs e)
        {
            DataUpdate();
        }

        private void Dgv_ReadDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            //Read - 去撈list<>裡面所有的資料=>DataGridView
            if (e.RowIndex < TParameter.DeviceData.lReadData.Count)
            {
                switch (dgv_ReadDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Read_SN":
                            e.Value = TParameter.DeviceData.lReadData[e.RowIndex].SN.ToString();
                        break;
                    case "Read_Label":
                            e.Value = TParameter.DeviceData.lReadData[e.RowIndex].Label.ToString();
                        break;
                    case "Read_Address":
                            e.Value = TParameter.DeviceData.lReadData[e.RowIndex].Address.ToString();
                        break;
                    case "Read_DataType":
                            e.Value = TParameter.DeviceData.lReadData[e.RowIndex].DataType.ToString();
                        break;
                    case "Read_Data":
                            e.Value = TParameter.DeviceData.lReadData[e.RowIndex].Data.ToString();
                        break;
                    case "Read_IsUse":
                            e.Value = TParameter.DeviceData.ZeroToBool(TParameter.DeviceData.lReadData[e.RowIndex].IsUse.ToString());
                        break;
                    case "Read_DeviceValueGet":
                            e.Value = TParameter.DeviceData.lReadData[e.RowIndex].DeviceValueGet.ToString();
                        break;
                }
            }
        }
        private void Dgv_ReadDataGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            //Read - 若對應儲存格中發生改變則跳入
            //分為超出範圍(新增) 與 範圍內(修改)
            if (e.RowIndex < TParameter.DeviceData.lReadData.Count)
            {
                switch (dgv_ReadDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Read_SN":
                        TParameter.DeviceData.lReadData[e.RowIndex].SN = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Read_Label":
                        TParameter.DeviceData.lReadData[e.RowIndex].Label = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Read_Address":
                        TParameter.DeviceData.lReadData[e.RowIndex].Address = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Read_DataType":
                        TParameter.DeviceData.lReadData[e.RowIndex].DataType = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Read_Data":
                        TParameter.DeviceData.lReadData[e.RowIndex].Data = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Read_IsUse":
                        TParameter.DeviceData.lReadData[e.RowIndex].IsUse = TParameter.DeviceData.BoolToZero(e.Value);
                        break;
                    case "Read_DeviceValueGet":
                        TParameter.DeviceData.lReadData[e.RowIndex].DeviceValueGet = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                }
            }
            else
            {
                CDataStruct cData = new CDataStruct();
                //若增加則下面先新增預設值
                switch (dgv_ReadDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Read_SN":
                        cData.SN = e.Value.ToString();
                        cData.Label = "";
                        cData.Address = "";
                        cData.DataType = "";
                        cData.Data = "";
                        cData.IsUse = "0";
                        cData.DeviceValueGet = ("N/A");
                        TParameter.DeviceData.lReadData.Add(cData);
                        break;
                    case "Read_Label":
                        cData.SN = "";
                        cData.Label = e.Value.ToString();
                        cData.Address = "";
                        cData.DataType = "";
                        cData.Data = "";
                        cData.IsUse = "0";
                        cData.DeviceValueGet = ("N/A");
                        TParameter.DeviceData.lReadData.Add(cData);
                        break;
                    case "Read_Address":
                        cData.SN = "";
                        cData.Label = "";
                        cData.Address = e.Value.ToString();
                        cData.DataType = "";
                        cData.Data = "";
                        cData.IsUse = "0";
                        cData.DeviceValueGet = ("N/A");
                        TParameter.DeviceData.lReadData.Add(cData);
                        break;
                    case "Read_DataType":
                        cData.SN = "";
                        cData.Label = "";
                        cData.Address = "";
                        cData.DataType = e.Value.ToString();
                        cData.Data = "";
                        cData.IsUse = "0";
                        cData.DeviceValueGet = ("N/A");
                        TParameter.DeviceData.lReadData.Add(cData);
                        break;
                    case "Read_Data":
                        cData.SN = "";
                        cData.Label = "";
                        cData.Address = "";
                        cData.DataType = "";
                        cData.Data = e.Value.ToString();
                        cData.IsUse = "0";
                        cData.DeviceValueGet = ("N/A");
                        TParameter.DeviceData.lReadData.Add(cData);
                        break;
                    case "Read_IsUse":
                        cData.SN = "";
                        cData.Label = "";
                        cData.Address = "";
                        cData.DataType = "";
                        cData.Data = "";
                        cData.IsUse = TParameter.DeviceData.BoolToZero(e.Value);
                        cData.DeviceValueGet = ("N/A");
                        TParameter.DeviceData.lReadData.Add(cData);
                        break;
                    case "Read_DeviceValueGet":
                        cData.SN = "";
                        cData.Label = "";
                        cData.Address = "";
                        cData.DataType = "";
                        cData.Data = "";
                        cData.IsUse = "0";
                        cData.DeviceValueGet = ("N/A");
                        TParameter.DeviceData.lReadData.Add(cData);
                        break;
                }
            }
            dgv_ReadDataGrid.InvalidateRow(e.RowIndex);
        }
        private void Dgv_WriteDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            //Write - 去撈list<>裡面所有的資料=>DataGridView
            if (e.RowIndex < TParameter.DeviceData.lWriteData.Count)
            {
                switch (dgv_WriteDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Write_SN":
                        e.Value = TParameter.DeviceData.lWriteData[e.RowIndex].SN.ToString();
                        break;
                    case "Write_Label":
                        e.Value = TParameter.DeviceData.lWriteData[e.RowIndex].Label.ToString();
                        break;
                    case "Write_Address":
                        e.Value = TParameter.DeviceData.lWriteData[e.RowIndex].Address.ToString();
                        break;
                    case "Write_DataType":
                        e.Value = TParameter.DeviceData.lWriteData[e.RowIndex].DataType.ToString();
                        break;
                    case "Write_Data":
                        e.Value = TParameter.DeviceData.lWriteData[e.RowIndex].Data.ToString();
                        break;
                    case "Write_IsUse":
                        e.Value = TParameter.DeviceData.ZeroToBool(TParameter.DeviceData.lWriteData[e.RowIndex].IsUse.ToString());
                        break;
                    case "Write_DeviceValueGet":
                        e.Value = TParameter.DeviceData.lWriteData[e.RowIndex].DeviceValueGet.ToString();
                        break;
                    case "Write_DeviceValueSet":
                        e.Value = TParameter.DeviceData.lWriteData[e.RowIndex].DeviceValueSet.ToString();
                        break;
                }
            }
        }
        private void Dgv_WriteDataGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            //Write - 若對應儲存格中發生改變則跳入
            //分為超出範圍(新增) 與 範圍內(修改)
            if (e.RowIndex < TParameter.DeviceData.lWriteData.Count)
            {
                switch (dgv_WriteDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Write_SN":
                        TParameter.DeviceData.lWriteData[e.RowIndex].SN = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Write_Label":
                        TParameter.DeviceData.lWriteData[e.RowIndex].Label = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Write_Address":
                        TParameter.DeviceData.lWriteData[e.RowIndex].Address = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Write_DataType":
                        TParameter.DeviceData.lWriteData[e.RowIndex].DataType = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Write_Data":
                        TParameter.DeviceData.lWriteData[e.RowIndex].Data = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Write_IsUse":
                        TParameter.DeviceData.lWriteData[e.RowIndex].IsUse = TParameter.DeviceData.BoolToZero(e.Value);
                        break;
                    case "Write_DeviceValueGet":
                        TParameter.DeviceData.lWriteData[e.RowIndex].DeviceValueGet = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Write_DeviceValueSet":
                        TParameter.DeviceData.lWriteData[e.RowIndex].DeviceValueSet = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                }
            }
            else
            {
                CDataStruct cData = new CDataStruct();
                //若增加則下面先新增預設值
                switch (dgv_WriteDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Write_SN":
                        cData.SN = e.Value.ToString();
                        cData.Label = "";
                        cData.Address = "";
                        cData.DataType = "";
                        cData.Data = "";
                        cData.IsUse = "0";
                        cData.DeviceValueGet = ("N/A");
                        cData.DeviceValueSet = "";
                        TParameter.DeviceData.lWriteData.Add(cData);
                        break;
                    case "Write_Label":
                        cData.SN = "";
                        cData.Label = e.Value.ToString();
                        cData.Address = "";
                        cData.DataType = "";
                        cData.Data = "";
                        cData.IsUse = "0";
                        cData.DeviceValueGet = ("N/A");
                        cData.DeviceValueSet = "";
                        TParameter.DeviceData.lWriteData.Add(cData);
                        break;
                    case "Write_Address":
                        cData.SN = "";
                        cData.Label = "";
                        cData.Address = e.Value.ToString();
                        cData.DataType = "";
                        cData.Data = "";
                        cData.IsUse = "0";
                        cData.DeviceValueGet = ("N/A");
                        cData.DeviceValueSet = "";
                        TParameter.DeviceData.lWriteData.Add(cData);
                        break;
                    case "Write_DataType":
                        cData.SN = "";
                        cData.Label = "";
                        cData.Address = "";
                        cData.DataType = e.Value.ToString();
                        cData.Data = "";
                        cData.IsUse = "0";
                        cData.DeviceValueGet = ("N/A");
                        cData.DeviceValueSet = "";
                        TParameter.DeviceData.lWriteData.Add(cData);
                        break;
                    case "Write_Data":
                        cData.SN = "";
                        cData.Label = "";
                        cData.Address = "";
                        cData.DataType = "";
                        cData.Data = e.Value.ToString();
                        cData.IsUse = "0";
                        cData.DeviceValueGet = ("N/A");
                        cData.DeviceValueSet = "";
                        TParameter.DeviceData.lWriteData.Add(cData);
                        break;
                    case "Write_IsUse":
                        cData.SN = "";
                        cData.Label = "";
                        cData.Address = "";
                        cData.DataType = "";
                        cData.Data = "";
                        cData.IsUse = TParameter.DeviceData.BoolToZero(e.Value);
                        cData.DeviceValueGet = ("N/A");
                        cData.DeviceValueSet = "";
                        TParameter.DeviceData.lWriteData.Add(cData);
                        break;
                    case "Write_DeviceValueGet":
                        cData.SN = "";
                        cData.Label = "";
                        cData.Address = "";
                        cData.DataType = "";
                        cData.Data = "";
                        cData.IsUse = "0";
                        cData.DeviceValueGet = "N/A";
                        cData.DeviceValueSet = "";
                        TParameter.DeviceData.lWriteData.Add(cData);
                        break;
                    case "Write_DeviceValueSet":
                        cData.SN = "";
                        cData.Label = "";
                        cData.Address = "";
                        cData.DataType = "";
                        cData.Data = "";
                        cData.IsUse = "0";
                        cData.DeviceValueGet = "N/A";
                        cData.DeviceValueSet = "";
                        TParameter.DeviceData.lWriteData.Add(cData);
                        break;
                }
            }
            dgv_WriteDataGrid.InvalidateRow(e.RowIndex);
        }

        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            TParameter.Mx_Connect.ProgClose();
        }
        //===============================================
        public void DataUpdate()
        {
            string sOutPutCell;
            //上傳寫入
            for (int i = 0; i < TParameter.DeviceData.lWriteData.Count; i++)
            {
                if (TParameter.DeviceData.lWriteData[i].IsUse.ToString() == "1")//有使用的檢索
                {
                    if (TParameter.DeviceData.lWriteData[i].DeviceValueSet != null)//上傳寫入值不為null
                    {
                        if (TParameter.DeviceData.lWriteData[i].DeviceValueSet != "")//上傳寫入值不為""
                        {
                            if (TParameter.DeviceData.lWriteData[i].Address.Contains("~"))//若為軟元件區間(帶區間"~")
                            {
                                try
                                {
                                    TParameter.Mx_Connect.ProgSetBlockCombine(TParameter.DeviceData.lWriteData[i].Address, TParameter.DeviceData.lWriteData[i].DeviceValueSet);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message + "\n是否為修改值string[]之軟元件名稱錯誤", "DataUpload", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else//其他軟元件
                            {
                                try
                                {
                                    TParameter.Mx_Connect.ProgSetDevice(TParameter.DeviceData.lWriteData[i].Address, TParameter.DeviceData.lWriteData[i].DeviceValueSet);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message + "\n上傳寫入資料填充有誤.\n請確認是否未輸入資料\n", "DataUpload", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    ModelChange();//強制切換
                                }
                            }
                        }
                    }
                }
            }
            //上傳讀取
            for (int i = 0; i < TParameter.DeviceData.lWriteData.Count; i++)
            {
                if (TParameter.DeviceData.lWriteData[i].IsUse.ToString() == "1")//表示有使用
                {
                    if (TParameter.DeviceData.lWriteData[i].Address.ToString().Contains("~"))//若為軟元件區間
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetBlockCombine(TParameter.DeviceData.lWriteData[i].Address.ToString(), out sOutPutCell);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤(~)", "DataUpload讀取", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
                        }
                    }
                    else//其他軟元件
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetDevice(TParameter.DeviceData.lWriteData[i].Address.ToString(), out sOutPutCell);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為word之軟元件名稱錯誤(Device)", "DataUpload讀取", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
                        }
                    }
                }
            }
            Console.WriteLine("上傳成功");
        }
        public void DataGridValueFlash()
        {
            //餵入資料畫面DataGridView刷新 =>更新陣列(下載/上傳)
            dgv_ReadDataGrid.RowCount = TParameter.DeviceData.lReadData.Count + 1;
            dgv_WriteDataGrid.RowCount = TParameter.DeviceData.lWriteData.Count + 1;
        }
        public void ModelChange()
        {
            TParameter.DeviceData.SaveData_ModChange(dgv_ReadDataGrid, dgv_WriteDataGrid);

            switch (TParameter.DeviceData.iModelChange)
            {
                case 0: //修改
                    p_ModelChange.BackColor = Color.LightBlue;
                    btn_ModelChange.Text = "格式修改模式中";

                    txt_ReadTime.Enabled = true;
                    Timer_DeviceGet.Elapsed -= On_DeviceGet;
                    Timer_DeviceGet.Stop();
                    break;
                case 1: //執行
                    p_ModelChange.BackColor = Color.OrangeRed;
                    btn_ModelChange.Text = "運作模式中";

                    if (txt_ReadTime.Text != "" && Convert.ToInt32(txt_ReadTime.Text) > 0)
                    {
                        Timer_DeviceGet.Interval = TParameter.Mx_Connect.iReciveTime = Convert.ToInt32(txt_ReadTime.Text);
                    }
                    else
                    {
                        //超出範圍預設值
                        TParameter.Error_Info.ErrorMessageBox_Time();
                        TParameter.Mx_Connect.iReciveTime = 2000;//兩秒
                        Timer_DeviceGet.Interval = 2000;
                        txt_ReadTime.Text = "2000";
                    }

                    txt_ReadTime.Enabled = false;
                    Timer_DeviceGet.Elapsed += On_DeviceGet;
                    Timer_DeviceGet.Start();
                    break;
            }
        }
        private void On_DeviceGet(object sender, EventArgs e)
        {
            try
            {
                DeviceGet();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "DeviceGet", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        private void DeviceGet()
        {
            swStopwatch.Restart();
            string arrGetData = "";//全字串("\n"為分號)
            int iTotalItem = 0;//全總數

            //先進行全部字串讀取
            //讀
            for (int i = 0; i < TParameter.DeviceData.lReadData.Count; i++)//SN_count
            {
                if (TParameter.DeviceData.lReadData[i].IsUse == "1")//表示有觸發
                {
                    if (TParameter.DeviceData.lReadData[i].Address.ToString().Contains("~"))//若為軟元件區間
                    {
                        TParameter.DeviceData.GetCombineArray_str(TParameter.DeviceData.lReadData[i].Address.ToString(), out int iItemCount, out string sItemStr);
                        //增加軟元件總數
                        iTotalItem += iItemCount;
                        //串Random用字串
                        arrGetData += (arrGetData == "") ? sItemStr : "\n" + sItemStr;
                    }
                    else//其餘單一元件
                    {
                        iTotalItem += 1;// 增加軟元件總數(單一device)
                        //串Random用字串
                        arrGetData += (arrGetData == "") ? TParameter.DeviceData.lReadData[i].Address.ToString() : "\n" + TParameter.DeviceData.lReadData[i].Address.ToString();
                    }
                }
            }
            //寫
            for (int i = 0; i < TParameter.DeviceData.lWriteData.Count; i++)//SN_count
            {
                if (TParameter.DeviceData.lWriteData[i].IsUse == "1")//表示有觸發
                {
                    if (TParameter.DeviceData.lWriteData[i].Address.ToString().Contains("~"))//若為軟元件區間
                    {
                        TParameter.DeviceData.GetCombineArray_str(TParameter.DeviceData.lWriteData[i].Address.ToString(), out int iItemCount, out string sItemStr);
                        
                        iTotalItem += iItemCount;//增加軟元件總數
                        //串Random用字串
                        arrGetData += (arrGetData == "") ? sItemStr : "\n" + sItemStr;

                    }
                    else//其餘單一元件
                    {
                        iTotalItem += 1;// 增加軟元件總數(單一device)
                        //串Random用字串
                        arrGetData += (arrGetData == "") ? TParameter.DeviceData.lWriteData[i].Address.ToString() : "\n" + TParameter.DeviceData.lWriteData[i].Address.ToString();

                    }
                }
            }
            //取全部元件的值(ReadRandom)
            TParameter.Mx_Connect.ProgGetDeviceRandom(arrGetData, iTotalItem, out int[] arrDeviceData);
            //餵回去
            int iOrderCount = 0;
            //讀
            for (int i = 0; i < TParameter.DeviceData.lReadData.Count; i++)//SN_count
            {
                if (TParameter.DeviceData.lReadData[i].IsUse == "1")//表示有觸發
                {
                    if (TParameter.DeviceData.lReadData[i].Address.ToString().Contains("~"))//若為軟元件區間
                    {
                        TParameter.DeviceData.GetCombineSize_int(TParameter.DeviceData.lReadData[i].Address.ToString(), out int iDeviceCount);
                        string sTmp = "";
                        for (int j = 0; j < iDeviceCount; j++)
                        {
                            sTmp += arrDeviceData[iOrderCount].ToString();
                            iOrderCount++;//每抓一參數進下一位
                        }
                        TParameter.DeviceData.lReadData[i].DeviceValueGet = sTmp;
                    }
                    else//其餘單一元件
                    {
                        TParameter.DeviceData.lReadData[i].DeviceValueGet = string.Format("{0:00}", arrDeviceData[iOrderCount]);
                        iOrderCount++;
                    }
                    dgv_ReadDataGrid.InvalidateCell(6, i);
                }
            }
            //寫
            for (int i = 0; i < TParameter.DeviceData.lWriteData.Count; i++)//SN_count
            {
                if (TParameter.DeviceData.lWriteData[i].IsUse == "1")//表示有觸發
                {
                    if (TParameter.DeviceData.lWriteData[i].Address.ToString().Contains("~"))//若為軟元件區間
                    {
                        TParameter.DeviceData.GetCombineSize_int(TParameter.DeviceData.lWriteData[i].Address.ToString(), out int iDeviceCount);
                        string sTmp = "";
                        for (int j = 0; j < iDeviceCount; j++)
                        {
                            sTmp += arrDeviceData[iOrderCount].ToString();
                            iOrderCount++;//每抓一參數進下一位
                        }
                        TParameter.DeviceData.lWriteData[i].DeviceValueGet = sTmp;
                    }
                    else//其餘單一元件
                    {
                        TParameter.DeviceData.lWriteData[i].DeviceValueGet = string.Format("{0:00}", arrDeviceData[iOrderCount]);
                        iOrderCount++;
                    }
                    dgv_WriteDataGrid.InvalidateRow(i);//列更新
                }
            }

            if (iOrderCount != iTotalItem)
            {
                Console.WriteLine("填值有問題");
            }

            swStopwatch.Stop();
            TimeSpan trim = swStopwatch.Elapsed;

            Console.WriteLine("迴圈1次時間: " + trim + "\n目前時間: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
        }
    }
}
