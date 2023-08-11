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


            //DataGridLabelFlash();//DataGridview 第一次刷新
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
            //Read - 去撈Arraylist裡面所有的資料=>DataGridView
            ///if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                switch (dgv_ReadDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Read_SN":
                        if (e.RowIndex < TParameter.DeviceData.Read_SN.Count)
                            e.Value = TParameter.DeviceData.Read_SN[e.RowIndex];
                        break;
                    case "Read_Label":
                        if (e.RowIndex < TParameter.DeviceData.Read_Label.Count)
                            e.Value = TParameter.DeviceData.Read_Label[e.RowIndex];
                        break;
                    case "Read_Address":
                        if (e.RowIndex < TParameter.DeviceData.Read_Address.Count)
                            e.Value = TParameter.DeviceData.Read_Address[e.RowIndex];
                        break;
                    case "Read_DataType":
                        if (e.RowIndex < TParameter.DeviceData.Read_DataType.Count)
                            e.Value = TParameter.DeviceData.Read_DataType[e.RowIndex];
                        break;
                    case "Read_Data":
                        if (e.RowIndex < TParameter.DeviceData.Read_Data.Count)
                            e.Value = TParameter.DeviceData.Read_Data[e.RowIndex].ToString();
                        break;
                    case "Read_IsUse":
                        if (e.RowIndex < TParameter.DeviceData.Read_IsUse.Count)
                            e.Value = TParameter.DeviceData.ZeroToBool(TParameter.DeviceData.Read_IsUse[e.RowIndex]);
                        break;
                    case "Read_DeviceValueGet":
                        if (e.RowIndex < TParameter.DeviceData.Read_DeviceValueGet.Count)
                            e.Value = TParameter.DeviceData.Read_DeviceValueGet[e.RowIndex];
                        break;
                }
            }
        }
        private void Dgv_ReadDataGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            //Read - 若對應儲存格中發生改變則跳入
            switch (dgv_ReadDataGrid.Columns[e.ColumnIndex].Name)
            {
                case "Read_SN":
                    if (e.RowIndex >= TParameter.DeviceData.Read_SN.Count)
                    {
                        TParameter.DeviceData.Read_SN.Add(e.Value.ToString());
                        //若SN增加則下面先新增預設值
                        TParameter.DeviceData.Read_Label.Add("");
                        TParameter.DeviceData.Read_Address.Add("");
                        TParameter.DeviceData.Read_DataType.Add("");
                        TParameter.DeviceData.Read_Data.Add("");
                        TParameter.DeviceData.Read_IsUse.Add("0");
                        TParameter.DeviceData.Read_DeviceValueGet.Add("N/A");
                        //刷新
                        dgv_ReadDataGrid.InvalidateCell(0, e.RowIndex);
                        dgv_ReadDataGrid.InvalidateCell(1, e.RowIndex);
                        dgv_ReadDataGrid.InvalidateCell(2, e.RowIndex);
                        dgv_ReadDataGrid.InvalidateCell(3, e.RowIndex);
                        dgv_ReadDataGrid.InvalidateCell(4, e.RowIndex);
                        dgv_ReadDataGrid.InvalidateCell(5, e.RowIndex);
                        dgv_ReadDataGrid.InvalidateCell(6, e.RowIndex);
                    }
                    else
                    {
                        TParameter.DeviceData.Read_SN[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
                case "Read_Label":
                    if (e.RowIndex >= TParameter.DeviceData.Read_Label.Count)
                    {
                        TParameter.DeviceData.Read_Label.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Read_Label[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
                case "Read_Address":
                    if (e.RowIndex >= TParameter.DeviceData.Read_Address.Count)
                    {
                        TParameter.DeviceData.Read_Address.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Read_Address[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
                case "Read_DataType":
                    if (e.RowIndex >= TParameter.DeviceData.Read_DataType.Count)
                    {
                        TParameter.DeviceData.Read_DataType.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Read_DataType[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
                case "Read_Data":
                    if (e.RowIndex >= TParameter.DeviceData.Read_Data.Count)
                    {
                        TParameter.DeviceData.Read_Data.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Read_Data[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
                case "Read_IsUse":
                    if (e.RowIndex >= TParameter.DeviceData.Read_IsUse.Count)
                    {

                        TParameter.DeviceData.Read_IsUse.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Read_IsUse[e.RowIndex] = TParameter.DeviceData.BoolToZero(e.Value);
                    }
                    break;
                case "Read_DeviceValueGet":
                    if (e.RowIndex >= TParameter.DeviceData.Read_DeviceValueGet.Count)
                    {
                        TParameter.DeviceData.Read_DeviceValueGet.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Read_DeviceValueGet[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
            }
        }
        private void Dgv_WriteDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            //Write - 去撈Arraylist裡面所有的資料=>DataGridView
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                switch (dgv_WriteDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Write_SN":
                        if (e.RowIndex < TParameter.DeviceData.Write_SN.Count)
                            e.Value = TParameter.DeviceData.Write_SN[e.RowIndex];
                        break;
                    case "Write_Label":
                        if (e.RowIndex < TParameter.DeviceData.Write_Label.Count)
                            e.Value = TParameter.DeviceData.Write_Label[e.RowIndex];
                        break;
                    case "Write_Address":
                        if (e.RowIndex < TParameter.DeviceData.Write_Address.Count)
                            e.Value = TParameter.DeviceData.Write_Address[e.RowIndex];
                        break;
                    case "Write_DataType":
                        if (e.RowIndex < TParameter.DeviceData.Write_DataType.Count)
                            e.Value = TParameter.DeviceData.Write_DataType[e.RowIndex];
                        break;
                    case "Write_Data":
                        if (e.RowIndex < TParameter.DeviceData.Write_Data.Count)
                            e.Value = TParameter.DeviceData.Write_Data[e.RowIndex].ToString();
                        break;
                    case "Write_IsUse":
                        if (e.RowIndex < TParameter.DeviceData.Write_IsUse.Count)
                            e.Value = TParameter.DeviceData.ZeroToBool(TParameter.DeviceData.Write_IsUse[e.RowIndex]);
                        break;
                    case "Write_DeviceValueGet":
                        if (e.RowIndex < TParameter.DeviceData.Write_DeviceValueGet.Count)
                            e.Value = TParameter.DeviceData.Write_DeviceValueGet[e.RowIndex];
                        break;
                    case "Write_DeviceValueSet":
                        if (e.RowIndex < TParameter.DeviceData.Write_DeviceValueSet.Count)
                            e.Value = TParameter.DeviceData.Write_DeviceValueSet[e.RowIndex];
                        break;
                }
            }
        }
        private void Dgv_WriteDataGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            //Write - 若對應儲存格中發生改變則跳入
            switch (dgv_WriteDataGrid.Columns[e.ColumnIndex].Name)
            {
                case "Write_SN":
                    if (e.RowIndex >= TParameter.DeviceData.Write_SN.Count)
                    {
                        TParameter.DeviceData.Write_SN.Add(e.Value.ToString());
                        //若SN增加則下面先新增預設值
                        TParameter.DeviceData.Write_Label.Add("");
                        TParameter.DeviceData.Write_Address.Add("");
                        TParameter.DeviceData.Write_DataType.Add("");
                        TParameter.DeviceData.Write_Data.Add("");
                        TParameter.DeviceData.Write_IsUse.Add("0");
                        TParameter.DeviceData.Write_DeviceValueGet.Add("N/A");
                        TParameter.DeviceData.Write_DeviceValueSet.Add("");
                        //刷新
                        dgv_WriteDataGrid.InvalidateCell(0, e.RowIndex);
                        dgv_WriteDataGrid.InvalidateCell(1, e.RowIndex);
                        dgv_WriteDataGrid.InvalidateCell(2, e.RowIndex);
                        dgv_WriteDataGrid.InvalidateCell(3, e.RowIndex);
                        dgv_WriteDataGrid.InvalidateCell(4, e.RowIndex);
                        dgv_WriteDataGrid.InvalidateCell(5, e.RowIndex);
                        dgv_WriteDataGrid.InvalidateCell(6, e.RowIndex);
                        dgv_WriteDataGrid.InvalidateCell(7, e.RowIndex);
                    }
                    else
                    {
                        TParameter.DeviceData.Write_SN[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
                case "Write_Label":
                    if (e.RowIndex >= TParameter.DeviceData.Write_Label.Count)
                    {
                        TParameter.DeviceData.Write_Label.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_Label[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
                case "Write_Address":
                    if (e.RowIndex >= TParameter.DeviceData.Write_Address.Count)
                    {
                        TParameter.DeviceData.Write_Address.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_Address[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
                case "Write_DataType":
                    if (e.RowIndex >= TParameter.DeviceData.Write_DataType.Count)
                    {
                        TParameter.DeviceData.Write_DataType.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_DataType[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
                case "Write_Data":
                    if (e.RowIndex >= TParameter.DeviceData.Write_Data.Count)
                    {
                        TParameter.DeviceData.Write_Data.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_Data[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
                case "Write_IsUse":
                    if (e.RowIndex >= TParameter.DeviceData.Write_IsUse.Count)
                    {
                        TParameter.DeviceData.Write_IsUse.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_IsUse[e.RowIndex] = TParameter.DeviceData.BoolToZero(e.Value);
                    }
                    break;
                case "Write_DeviceValueGet":
                    if (e.RowIndex >= TParameter.DeviceData.Write_DeviceValueGet.Count)
                    {
                        TParameter.DeviceData.Write_DeviceValueGet.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_DeviceValueGet[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
                case "Write_DeviceValueSet":
                    if (e.RowIndex >= TParameter.DeviceData.Write_DeviceValueSet.Count)
                    {
                        TParameter.DeviceData.Write_DeviceValueSet.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_DeviceValueSet[e.RowIndex] = (e.Value != null) ? e.Value.ToString() : "";
                    }
                    break;
            }
            //Console.WriteLine("修改");
        }

        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            TParameter.Mx_Connect.ProgClose();
        }
        //===============================================
        public void DataUpdate()
        {
            Timer_DeviceGet.Stop();//岔斷運行模式
            swStopwatch.Restart();
            string sOutPutCell;
            //上傳寫入
            for (int i = 0; i < TParameter.DeviceData.Write_SN.Count; i++)
            {
                if (TParameter.DeviceData.Write_IsUse[i].ToString() == "1")//有使用的檢索
                {
                    if (TParameter.DeviceData.Write_DeviceValueSet[i] != null)//上傳寫入值不為null
                    {
                        if (TParameter.DeviceData.Write_DeviceValueSet[i] != "")//上傳寫入值不為""
                        {
                            //若為軟元件區間
                            if (TParameter.DeviceData.Write_Address[i].Contains("~"))//帶區間"~"
                            {
                                try
                                {
                                    TParameter.Mx_Connect.ProgSetBlockCombine(TParameter.DeviceData.Write_Address[i], TParameter.DeviceData.Write_DeviceValueSet[i]);
                                    if (TParameter.Mx_Connect.iReturnCode == 0)
                                    {
                                        Console.WriteLine(TParameter.DeviceData.Write_Address[i] + " SetOK");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message + "\n是否為修改值string[]之軟元件名稱錯誤", "DataUpload", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Timer_DeviceGet.Start();
                                }
                            }
                            else//其他軟元件
                            {
                                try
                                {
                                    TParameter.Mx_Connect.ProgSetDevice(TParameter.DeviceData.Write_Address[i], TParameter.DeviceData.Write_DeviceValueSet[i]);
                                    if (TParameter.Mx_Connect.iReturnCode == 0)
                                    {
                                        TParameter.Mx_Connect.ProgGetDevice(TParameter.DeviceData.Write_Address[i].ToString(), out sOutPutCell);
                                        Console.WriteLine(TParameter.DeviceData.Write_Address[i] + ": " + sOutPutCell);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message + "\n上傳寫入資料填充有誤.\n請確認是否未輸入資料\n", "DataUpload", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    Timer_DeviceGet.Start();
                                }
                            }

                        }
                    }
                }

            }
            //上傳讀取
            for (int i = 0; i < TParameter.DeviceData.Write_SN.Count; i++)
            {
                if (TParameter.DeviceData.Write_IsUse[i].ToString() == "1")//表示有使用
                {
                    //若為軟元件區間
                    if (TParameter.DeviceData.Write_Address[i].ToString().Contains("~"))
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetBlockCombine(TParameter.DeviceData.Write_Address[i].ToString(), out sOutPutCell);
                            if (TParameter.Mx_Connect.iReturnCode == 0)
                            {
                                TParameter.DeviceData.Write_DeviceValueGet[i] = sOutPutCell;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤(~)", "DataUpload讀取", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
                            Timer_DeviceGet.Start();
                        }
                    }
                    else//其他軟元件
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetDevice(TParameter.DeviceData.Write_Address[i].ToString(), out sOutPutCell);
                            if (TParameter.Mx_Connect.iReturnCode == 0)
                            {
                                TParameter.DeviceData.Write_DeviceValueGet[i] = sOutPutCell;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為word之軟元件名稱錯誤(Device)", "DataUpload讀取", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
                            Timer_DeviceGet.Start();
                        }
                    }
                }
            }
            Console.WriteLine("上傳成功");
            TimeSpan trim = swStopwatch.Elapsed;

            Console.WriteLine("上傳1次時間: " + trim);
            Timer_DeviceGet.Start();
        }
        public void DataGridValueFlash()
        {
            //餵入資料畫面DataGridView刷新 =>更新陣列
            #region 下載
            dgv_ReadDataGrid.RowCount = TParameter.DeviceData.Read_SN.Count + 1;
            #endregion

            #region 上傳
            dgv_WriteDataGrid.RowCount = TParameter.DeviceData.Write_SN.Count + 1;
            #endregion
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
            Timer_DeviceGet.Stop();
            //Cycle_DeviceGet();
            DeviceGet();
            Timer_DeviceGet.Start();
        }
        private void Cycle_DeviceGet()
        {
            swStopwatch.Restart();
            //下載讀取
            #region 下載讀取
            for (int i = 0; i < TParameter.DeviceData.Read_SN.Count; i++)//SN_count
            {
                if (TParameter.DeviceData.Read_IsUse[i] == "1")//表示有使用err
                {
                    //若為軟元件區間
                    if (TParameter.DeviceData.Read_Address[i].Contains("~"))
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetBlockCombine(TParameter.DeviceData.Read_Address[i], out string sOutPutCell);
                            TParameter.DeviceData.Read_DeviceValueGet[i] = (TParameter.Mx_Connect.iReturnCode == 0) ? sOutPutCell : TParameter.Error_Info.ErrorStrSend();
                        }
                        catch (Exception ex)
                        {
                            TParameter.Error_Info.ErrorMessageBox_DeviceBlock(ex);
                            ModelChange();//強制切換
                            return;
                        }
                    }
                    else//其他軟元件
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetDevice(TParameter.DeviceData.Read_Address[i], out string sOutPut);
                            TParameter.DeviceData.Read_DeviceValueGet[i] = (TParameter.Mx_Connect.iReturnCode == 0) ? sOutPut : TParameter.Error_Info.ErrorStrSend();
                        }
                        catch (Exception ex)
                        {
                            TParameter.Error_Info.ErrorMessageBox_DeviceValue(ex);
                            ModelChange();//強制切換
                            return;
                        }
                    }
                    dgv_ReadDataGrid.InvalidateCell(6, i);
                }
            }
            #endregion
            //上傳讀取
            #region 上傳讀取
            for (int i = 0; i < TParameter.DeviceData.Write_SN.Count; i++)
            {
                if (TParameter.DeviceData.Write_IsUse[i] == "1")//表示有使用
                {
                    //若為軟元件區間
                    if (TParameter.DeviceData.Write_Address[i].ToString().Contains("~"))
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetBlockCombine(TParameter.DeviceData.Write_Address[i].ToString(), out string sOutPutCell);
                            TParameter.DeviceData.Write_DeviceValueGet[i] = (TParameter.Mx_Connect.iReturnCode == 0) ? sOutPutCell : TParameter.Error_Info.ErrorStrSend();
                        }
                        catch (Exception ex)
                        {
                            TParameter.Error_Info.ErrorMessageBox_DeviceBlock(ex);
                            ModelChange();//強制切換
                            return;
                        }
                    }
                    else//其他軟元件
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetDevice(TParameter.DeviceData.Write_Address[i], out string sOutPutCell);
                            TParameter.DeviceData.Write_DeviceValueGet[i] = (TParameter.Mx_Connect.iReturnCode == 0) ? sOutPutCell : TParameter.Error_Info.ErrorStrSend();
                        }
                        catch (Exception ex)
                        {
                            TParameter.Error_Info.ErrorMessageBox_DeviceValue(ex);
                            ModelChange();//強制切換
                            return;
                        }
                    }
                    dgv_WriteDataGrid.InvalidateCell(6, i);
                }
            }
            #endregion
            swStopwatch.Stop();
            TimeSpan trim = swStopwatch.Elapsed;

            Console.WriteLine("迴圈1次時間: " + trim + "\n目前時間: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
        }

        private void DeviceGet()
        {
            swStopwatch.Restart();
            //下載上傳讀取
            //目前未發生字串溢位問題
            string arrGetData = "";
            int iTotalItem = 0;//全總數
            //先進行全部字串讀取
            //讀
            for (int i = 0; i < TParameter.DeviceData.Read_SN.Count; i++)//SN_count
            {
                if (TParameter.DeviceData.Read_IsUse[i] == "1")//表示有觸發
                {
                    if (TParameter.DeviceData.Read_Address[i].ToString().Contains("~"))//若為軟元件區間
                    {
                        TParameter.DeviceData.GetCombineArray_str(TParameter.DeviceData.Read_Address[i].ToString(), out int iItemCount, out string sItemStr);
                        //增加軟元件總數
                        iTotalItem += iItemCount;
                        //串Random用字串
                        arrGetData += (arrGetData == "") ? sItemStr : "\n" + sItemStr;

                    }
                    else//其餘單一元件
                    {
                        //增加軟元件總數
                        iTotalItem += 1;
                        //串Random用字串
                        arrGetData += (arrGetData == "") ? TParameter.DeviceData.Read_Address[i].ToString() : "\n" + TParameter.DeviceData.Read_Address[i].ToString();

                    }
                }
            }
            //寫
            for (int i = 0; i < TParameter.DeviceData.Write_SN.Count; i++)//SN_count
            {
                if (TParameter.DeviceData.Write_IsUse[i] == "1")//表示有觸發
                {
                    if (TParameter.DeviceData.Write_Address[i].ToString().Contains("~"))//若為軟元件區間
                    {
                        TParameter.DeviceData.GetCombineArray_str(TParameter.DeviceData.Write_Address[i].ToString(), out int iItemCount, out string sItemStr);
                        //增加軟元件總數
                        iTotalItem += iItemCount;
                        //串Random用字串
                        arrGetData += (arrGetData == "") ? sItemStr : "\n" + sItemStr;

                    }
                    else//其餘單一元件
                    {
                        //增加軟元件總數
                        iTotalItem += 1;
                        //串Random用字串
                        arrGetData += (arrGetData == "") ? TParameter.DeviceData.Write_Address[i].ToString() : "\n" + TParameter.DeviceData.Write_Address[i].ToString();

                    }
                }
            }
            //取全部元件的值(ReadRandom)
            TParameter.Mx_Connect.ProgGetDeviceRandom(arrGetData, iTotalItem, out int[] arrDeviceData);
            //餵回去
            int iOrderCount = 0;
            //讀
            for (int i = 0; i < TParameter.DeviceData.Read_SN.Count; i++)//SN_count
            {
                if (TParameter.DeviceData.Read_IsUse[i] == "1")//表示有觸發
                {
                    if (TParameter.DeviceData.Read_Address[i].ToString().Contains("~"))//若為軟元件區間
                    {
                        //取得軟元件範圍與
                        TParameter.DeviceData.GetCombineSize_int(TParameter.DeviceData.Read_Address[i].ToString(), out int iDeviceCount);
                        //iOrderCount += iDeviceCount;
                        string sTmp = "";
                        for (int j = 0; j < iDeviceCount; j++)
                        {
                            sTmp += arrDeviceData[iOrderCount].ToString();
                            iOrderCount++;//每抓一參數進下一位
                        }
                        TParameter.DeviceData.Read_DeviceValueGet[i] = sTmp;
                    }
                    else//其餘單一元件
                    {
                        TParameter.DeviceData.Read_DeviceValueGet[i] = string.Format("{0:00}", arrDeviceData[iOrderCount]);
                        iOrderCount++;
                    }
                    dgv_ReadDataGrid.InvalidateCell(6, i);
                }
            }
            //寫
            for (int i = 0; i < TParameter.DeviceData.Write_SN.Count; i++)//SN_count
            {
                if (TParameter.DeviceData.Write_IsUse[i] == "1")//表示有觸發
                {
                    if (TParameter.DeviceData.Write_Address[i].ToString().Contains("~"))//若為軟元件區間
                    {
                        //取得軟元件範圍與
                        TParameter.DeviceData.GetCombineSize_int(TParameter.DeviceData.Write_Address[i].ToString(), out int iDeviceCount);
                        //iOrderCount += iDeviceCount;
                        string sTmp = "";
                        for (int j = 0; j < iDeviceCount; j++)
                        {
                            sTmp += arrDeviceData[iOrderCount].ToString();
                            iOrderCount++;//每抓一參數進下一位
                        }
                        TParameter.DeviceData.Write_DeviceValueGet[i] = sTmp;
                    }
                    else//其餘單一元件
                    {
                        TParameter.DeviceData.Write_DeviceValueGet[i] = string.Format("{0:00}", arrDeviceData[iOrderCount]);
                        iOrderCount++;
                    }
                    dgv_WriteDataGrid.InvalidateCell(6, i);
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
