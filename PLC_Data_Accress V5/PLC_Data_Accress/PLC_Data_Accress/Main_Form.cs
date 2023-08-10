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
        private System.Timers.Timer Timer_DeviceGet = new System.Timers.Timer();

        delegate void UpdateLabel(Label lab, string Msg);
        delegate void UpdateDataGridView(DataGridView view, DataGridViewRow[] data);
        private object _objLock = new object();
        public Stopwatch swStopwatch = new Stopwatch();


        public Main_Form()
        {
            InitializeComponent();

            TParameter.init();//初始化 然後連線

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

            //先餵入資料給 ReadGridRow & WriteGridRow
            DataGridRowSet();


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

                    DataGridRowSet();
                    //dgv_ReadDataGrid.Rows.Clear();
                    //dgv_WriteDataGrid.Rows.Clear();

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
                //先將值餵回去給ReadGridRow & WriteGridRow => Read & Write
                TParameter.DeviceData.SaveData_GridRow();

                //再以Read & Write =>儲存
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
                if (e.RowIndex < TParameter.DeviceData.ReadGridRow.Length)
                {
                    switch (dgv_ReadDataGrid.Columns[e.ColumnIndex].Name)
                    {
                        case "Read_SN":
                            e.Value = TParameter.DeviceData.ReadGridRow[e.RowIndex].Cells[0].Value;
                            break;
                        case "Read_Label":
                            e.Value = TParameter.DeviceData.ReadGridRow[e.RowIndex].Cells[1].Value;
                            break;
                        case "Read_Address":
                            e.Value = TParameter.DeviceData.ReadGridRow[e.RowIndex].Cells[2].Value;
                            break;
                        case "Read_DataType":
                            e.Value = TParameter.DeviceData.ReadGridRow[e.RowIndex].Cells[3].Value;
                            break;
                        case "Read_Data":
                            e.Value = TParameter.DeviceData.ReadGridRow[e.RowIndex].Cells[4].Value;
                            break;
                        case "Read_IsUse":
                            e.Value = TParameter.DeviceData.ReadGridRow[e.RowIndex].Cells[5].Value;
                            break;
                        case "Read_DeviceValueGet":
                            e.Value = TParameter.DeviceData.ReadGridRow[e.RowIndex].Cells[6].Value;
                            break;
                    }
                }
            }
        }
        private void Dgv_ReadDataGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            //Read - 若對應儲存格中發生改變則跳入
            //if(e.RowIndex >= TParameter.DeviceData.ReadGridRow.Length)
            //{
            //    switch (dgv_ReadDataGrid.Columns[e.ColumnIndex].Name)
            //    {

            //    }
            //}


            switch (dgv_ReadDataGrid.Columns[e.ColumnIndex].Name)
            {
                case "Read_SN":
                    if (e.RowIndex >= TParameter.DeviceData.Read_SN.Count)
                    {
                        //先增加一行
                        //TParameter.DeviceData.ReadGridRow.
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
                        TParameter.DeviceData.Read_SN[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Read_Label":
                    if (e.RowIndex >= TParameter.DeviceData.Read_Label.Count)
                    {
                        TParameter.DeviceData.Read_Label.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Read_Label[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Read_Address":
                    if (e.RowIndex >= TParameter.DeviceData.Read_Address.Count)
                    {
                        TParameter.DeviceData.Read_Address.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Read_Address[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Read_DataType":
                    if (e.RowIndex >= TParameter.DeviceData.Read_DataType.Count)
                    {
                        TParameter.DeviceData.Read_DataType.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Read_DataType[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Read_Data":
                    if (e.RowIndex >= TParameter.DeviceData.Read_Data.Count)
                    {
                        TParameter.DeviceData.Read_Data.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Read_Data[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Read_IsUse":
                    if (e.RowIndex >= TParameter.DeviceData.Read_IsUse.Count)
                    {
                        TParameter.DeviceData.Read_IsUse.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Read_IsUse[e.RowIndex] = TParameter.DeviceData.BoolToValue(e.Value);
                    }
                    break;
                case "Read_DeviceValueGet":
                    if (e.RowIndex >= TParameter.DeviceData.Read_DeviceValueGet.Count)
                    {
                        TParameter.DeviceData.Read_DeviceValueGet.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Read_DeviceValueGet[e.RowIndex] = e.Value.ToString();
                    }
                    break;
            }
        }
        private void Dgv_WriteDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            //Write - 去撈Arraylist裡面所有的資料=>DataGridView

            //if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                
                if (e.RowIndex < TParameter.DeviceData.WriteGridRow.Length)
                {
                    switch (dgv_WriteDataGrid.Columns[e.ColumnIndex].Name)
                    {
                        case "Write_SN":
                            e.Value = TParameter.DeviceData.WriteGridRow[e.RowIndex].Cells[0].Value;
                            break;
                        case "Write_Label":
                            e.Value = TParameter.DeviceData.WriteGridRow[e.RowIndex].Cells[1].Value;
                            break;
                        case "Write_Address":
                            e.Value = TParameter.DeviceData.WriteGridRow[e.RowIndex].Cells[2].Value;
                            break;
                        case "Write_DataType":
                            e.Value = TParameter.DeviceData.WriteGridRow[e.RowIndex].Cells[3].Value;
                            break;
                        case "Write_Data":
                            e.Value = TParameter.DeviceData.WriteGridRow[e.RowIndex].Cells[4].Value;
                            break;
                        case "Write_IsUse":
                            e.Value = TParameter.DeviceData.WriteGridRow[e.RowIndex].Cells[5].Value;
                            break;
                        case "Write_DeviceValueGet":
                            e.Value = TParameter.DeviceData.WriteGridRow[e.RowIndex].Cells[6].Value;
                            break;
                        case "Write_DeviceValueSet":
                            e.Value = TParameter.DeviceData.WriteGridRow[e.RowIndex].Cells[7].Value;
                            break;
                    }
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
                        TParameter.DeviceData.Write_SN[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Write_Label":
                    if (e.RowIndex >= TParameter.DeviceData.Write_Label.Count)
                    {
                        TParameter.DeviceData.Write_Label.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_Label[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Write_Address":
                    if (e.RowIndex >= TParameter.DeviceData.Write_Address.Count)
                    {
                        TParameter.DeviceData.Write_Address.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_Address[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Write_DataType":
                    if (e.RowIndex >= TParameter.DeviceData.Write_DataType.Count)
                    {
                        TParameter.DeviceData.Write_DataType.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_DataType[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Write_Data":
                    if (e.RowIndex >= TParameter.DeviceData.Write_Data.Count)
                    {
                        TParameter.DeviceData.Write_Data.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_Data[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Write_IsUse":
                    if (e.RowIndex >= TParameter.DeviceData.Write_IsUse.Count)
                    {
                        TParameter.DeviceData.Write_IsUse.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_IsUse[e.RowIndex] = TParameter.DeviceData.BoolToValue(e.Value);
                    }
                    break;
                case "Write_DeviceValueGet":
                    if (e.RowIndex >= TParameter.DeviceData.Write_DeviceValueGet.Count)
                    {
                        TParameter.DeviceData.Write_DeviceValueGet.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_DeviceValueGet[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Write_DeviceValueSet":
                    if (e.RowIndex >= TParameter.DeviceData.Write_DeviceValueSet.Count)
                    {
                        TParameter.DeviceData.Write_DeviceValueSet.Add(e.Value.ToString());
                    }
                    else
                    {
                        TParameter.DeviceData.Write_DeviceValueSet[e.RowIndex] = e.Value.ToString();
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

            string sOutPutCell;
            //上傳寫入
            for (int i = 0; i < TParameter.DeviceData.WriteGridRow.Length; i++)
            {
                if (TParameter.DeviceData.WriteGridRow[i].Cells[5].Value.ToString() == "1")//有使用(IsUse)的檢索
                {
                    if (TParameter.DeviceData.WriteGridRow[i].Cells[7].Value != null)//上傳寫入值不為null
                    {
                        if (TParameter.DeviceData.WriteGridRow[i].Cells[7].Value.ToString() != "")//上傳寫入值不為""
                        {
                            //若為軟元件區間
                            if (TParameter.DeviceData.WriteGridRow[i].Cells[2].Value.ToString().Contains("~"))//軟元件地址帶區間"~"
                            {
                                try
                                {
                                    TParameter.Mx_Connect.ProgSetBlockCombine(TParameter.DeviceData.WriteGridRow[i].Cells[2].Value.ToString(), TParameter.DeviceData.WriteGridRow[i].Cells[7].ToString());
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message + "\n是否為修改值string[]之軟元件名稱錯誤", "DataUpload", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Timer_DeviceGet.Start();
                                    return;
                                }
                            }
                            else//其他軟元件(單獨)
                            {
                                try
                                {
                                    TParameter.Mx_Connect.ProgSetDevice(TParameter.DeviceData.WriteGridRow[i].Cells[2].Value.ToString(), TParameter.DeviceData.WriteGridRow[i].Cells[7].ToString());
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message + "\n上傳寫入資料填充有誤.\n請確認是否未輸入資料\n", "DataUpload", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    Timer_DeviceGet.Start();
                                    return;
                                }
                            }

                        }
                    }
                }

            }
            //上傳讀取
            for (int i = 0; i < TParameter.DeviceData.WriteGridRow.Length; i++)
            {
                if (TParameter.DeviceData.WriteGridRow[i].Cells[5].Value.ToString() == "1")//表示有使用
                {
                    //若為軟元件區間
                    if (TParameter.DeviceData.WriteGridRow[i].Cells[2].Value.ToString().Contains("~"))
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetBlockCombine(TParameter.DeviceData.WriteGridRow[i].Cells[2].Value.ToString(), out sOutPutCell);
                            if (TParameter.Mx_Connect.iReturnCode == 0)
                            {
                                TParameter.DeviceData.WriteGridRow[i].Cells[6].Value = sOutPutCell;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤(~)", "DataUpload讀取", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
                            Timer_DeviceGet.Start();
                            return;
                        }
                    }
                    else//其他軟元件
                    {

                        try
                        {
                            TParameter.Mx_Connect.ProgGetDevice(TParameter.DeviceData.WriteGridRow[i].Cells[2].Value.ToString(), out sOutPutCell);
                            if (TParameter.Mx_Connect.iReturnCode == 0)
                            {
                                TParameter.DeviceData.WriteGridRow[i].Cells[6].Value = sOutPutCell;
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為word之軟元件名稱錯誤(Device)", "DataUpload讀取", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
                            Timer_DeviceGet.Start();
                            return;
                        }
                    }
                }
            }
            Console.WriteLine("上傳成功");
            Timer_DeviceGet.Start();
        }
        public void DataGridValueFlash()
        {
            //餵入資料畫面DataGridView刷新
            #region 下載
            //dgv_ReadDataGrid.Rows.Clear();//清除原本的
            dgv_ReadDataGrid.RowCount = TParameter.DeviceData.ReadGridRow.Length + 1;
            #endregion

            #region 上傳
            //dgv_WriteDataGrid.Rows.Clear();//清除原本的
            dgv_WriteDataGrid.RowCount = TParameter.DeviceData.WriteGridRow.Length + 1;
            #endregion
        }
        public void DataGridRowSet()
        {
            //下載
            TParameter.DeviceData.ReadGridRow = null;
            TParameter.DeviceData.ReadGridRow = new DataGridViewRow[TParameter.DeviceData.Read_SN.Count];//以DataGridViewRow加入較快
            if (TParameter.DeviceData.Read_SN.Count != 0)
            {
                for (int i = 0; i < TParameter.DeviceData.Read_SN.Count; i++)
                {
                    TParameter.DeviceData.ReadGridRow[i] = new DataGridViewRow();
                    TParameter.DeviceData.ReadGridRow[i].CreateCells(dgv_ReadDataGrid);//以下載的datagridview座基底新增料表元素
                    TParameter.DeviceData.ReadGridRow[i].Cells[0].Value = TParameter.DeviceData.Read_SN[i].ToString();
                    TParameter.DeviceData.ReadGridRow[i].Cells[1].Value = TParameter.DeviceData.Read_Label[i].ToString();
                    TParameter.DeviceData.ReadGridRow[i].Cells[2].Value = TParameter.DeviceData.Read_Address[i].ToString();
                    TParameter.DeviceData.ReadGridRow[i].Cells[3].Value = TParameter.DeviceData.Read_DataType[i].ToString();
                    TParameter.DeviceData.ReadGridRow[i].Cells[4].Value = TParameter.DeviceData.Read_Data[i].ToString();
                    TParameter.DeviceData.ReadGridRow[i].Cells[5].Value = TParameter.DeviceData.ValueToBool(TParameter.DeviceData.Read_IsUse[i].ToString());
                    TParameter.DeviceData.ReadGridRow[i].Cells[6].Value = "NA";
                }
            }
            //上傳
            dgv_WriteDataGrid.Rows.Clear();//清除原本的
            TParameter.DeviceData.WriteGridRow = new DataGridViewRow[TParameter.DeviceData.Write_SN.Count];//以DataGridViewRow加入較快
            if (TParameter.DeviceData.Write_SN.Count != 0)
            {
                for (int i = 0; i < TParameter.DeviceData.Write_SN.Count; i++)
                {
                    TParameter.DeviceData.WriteGridRow[i] = new DataGridViewRow();
                    TParameter.DeviceData.WriteGridRow[i].CreateCells(dgv_WriteDataGrid);//以上傳的datagridview座基底新增料表元素
                    TParameter.DeviceData.WriteGridRow[i].Cells[0].Value = TParameter.DeviceData.Write_SN[i].ToString();
                    TParameter.DeviceData.WriteGridRow[i].Cells[1].Value = TParameter.DeviceData.Write_Label[i].ToString();
                    TParameter.DeviceData.WriteGridRow[i].Cells[2].Value = TParameter.DeviceData.Write_Address[i].ToString();
                    TParameter.DeviceData.WriteGridRow[i].Cells[3].Value = TParameter.DeviceData.Write_DataType[i].ToString();
                    TParameter.DeviceData.WriteGridRow[i].Cells[4].Value = TParameter.DeviceData.Write_Data[i].ToString();
                    TParameter.DeviceData.WriteGridRow[i].Cells[5].Value = TParameter.DeviceData.ValueToBool(TParameter.DeviceData.Write_IsUse[i].ToString());
                    TParameter.DeviceData.WriteGridRow[i].Cells[6].Value = "NA";
                    TParameter.DeviceData.WriteGridRow[i].Cells[7].Value = "";
                }
            }
        }

        public void CreateNewRow()
        {

        }

        public void ModelChange()
        {
            TParameter.DeviceData.SaveData_ModChange(dgv_ReadDataGrid, dgv_WriteDataGrid);

            switch (TParameter.DeviceData.iModelChange)
            {
                case 0: //修改模式
                    Timer_DeviceGet.Stop();
                    txt_ReadTime.Enabled = true;
                    Timer_DeviceGet.Elapsed -= On_DeviceGet;
                    //----
                    p_ModelChange.BackColor = Color.LightBlue;
                    btn_ModelChange.Text = "格式修改模式中";
                    Lb_Status.Text = "進入修改模式";
                    break;
                case 1: //執行模式
                    p_ModelChange.BackColor = Color.OrangeRed;
                    btn_ModelChange.Text = "運作模式中";
                    //----
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
                    //----
                    Timer_DeviceGet.Elapsed += On_DeviceGet;
                    txt_ReadTime.Enabled = false;
                    Timer_DeviceGet.Start();
                    break;
            }
        }
        private void On_DeviceGet(object sender, EventArgs e)
        {
            Timer_DeviceGet.Stop();
            Cycle_DeviceGet();
            Timer_DeviceGet.Start();
        }
        private void Cycle_DeviceGet()
        {
            swStopwatch.Restart();
            /*
            //下載讀取
            #region 下載讀取
            for (int i = 0; i < TParameter.DeviceData.ReadGridRow.Length; i++)//SN_count
            {
                if (TParameter.DeviceData.BoolToValue(TParameter.DeviceData.ReadGridRow[i].Cells[5].Value.ToString()) == "1")//表示有使用err
                {
                    //若為軟元件區間
                    if (TParameter.DeviceData.ReadGridRow[i].Cells[2].Value.ToString().Contains("~"))
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetBlockCombine(TParameter.DeviceData.ReadGridRow[i].Cells[2].Value.ToString(), out string sOutPutCell);
                            TParameter.DeviceData.ReadGridRow[i].Cells[6].Value = (TParameter.Mx_Connect.iReturnCode == 0) ? sOutPutCell : TParameter.Error_Info.ErrorStrSend();
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
                            TParameter.Mx_Connect.ProgGetDevice(TParameter.DeviceData.ReadGridRow[i].Cells[2].Value.ToString(), out string sOutPut);
                            TParameter.DeviceData.ReadGridRow[i].Cells[6].Value = (TParameter.Mx_Connect.iReturnCode == 0) ? sOutPut : TParameter.Error_Info.ErrorStrSend();
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
            for (int i = 0; i < TParameter.DeviceData.WriteGridRow.Length; i++)//SN_count
            {
                if (TParameter.DeviceData.BoolToValue(TParameter.DeviceData.WriteGridRow[i].Cells[5].Value.ToString()) == "1")//表示有使用err
                {
                    //若為軟元件區間
                    if (TParameter.DeviceData.WriteGridRow[i].Cells[2].Value.ToString().Contains("~"))
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetBlockCombine(TParameter.DeviceData.WriteGridRow[i].Cells[2].Value.ToString(), out string sOutPutCell);
                            TParameter.DeviceData.WriteGridRow[i].Cells[6].Value = (TParameter.Mx_Connect.iReturnCode == 0) ? sOutPutCell : TParameter.Error_Info.ErrorStrSend();
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
                            TParameter.Mx_Connect.ProgGetDevice(TParameter.DeviceData.WriteGridRow[i].Cells[2].Value.ToString(), out string sOutPut);
                            TParameter.DeviceData.WriteGridRow[i].Cells[6].Value = (TParameter.Mx_Connect.iReturnCode == 0) ? sOutPut : TParameter.Error_Info.ErrorStrSend();
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
            */
            //-----
            #region 下載讀取
            for (int i = 0; i < TParameter.DeviceData.Read_SN.Count; i++)//SN_count
            {
                if (TParameter.DeviceData.Read_IsUse[i].ToString() == "1")//表示有使用
                {
                    //若為軟元件區間
                    if (TParameter.DeviceData.Read_Address[i].ToString().Contains("~"))
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetBlockCombine(TParameter.DeviceData.Read_Address[i].ToString(), out string sOutPutCell);

                            if (TParameter.Mx_Connect.iReturnCode == 0)
                            {

                                //dgv_ReadDataGrid.Rows[i].Cells[6].Value = sOutPutCell;
                                TParameter.DeviceData.ReadGridRow[i].Cells[6].Value = sOutPutCell;
                            }
                            else
                            {
                                //顯示errorcode於輸出值當中
                                //dgv_ReadDataGrid.Rows[i].Cells[6].Value = "ErCode: " + String.Format("0x{0:x8} [HEX]", TParameter.Mx_Connect.iReturnCode);
                                TParameter.DeviceData.ReadGridRow[i].Cells[6].Value = "ErCode: " + String.Format("0x{0:x8} [HEX]", TParameter.Mx_Connect.iReturnCode);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤" + "\n" + TParameter.DeviceData.Read_Address[i].ToString(), "MDeviceGet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
                        }
                    }
                    else//其他軟元件
                    {
                        try
                        {
                            TParameter.Mx_Connect.iReturnCode = TParameter.Mx_Connect.Prog_Connect.GetDevice(TParameter.DeviceData.Read_Address[i].ToString(), out int iOutPut);
                            if (TParameter.Mx_Connect.iReturnCode == 0)
                            {
                                TParameter.DeviceData.ReadGridRow[i].Cells[6].Value = iOutPut.ToString();
                                //sOutPutCell = iOutPut.ToString();
                                //dgv_ReadDataGrid.Rows[i].Cells[6].Value = sOutPutCell;
                            }
                            else
                            {
                                //顯示errorcode於輸出值當中
                                //dgv_ReadDataGrid.Rows[i].Cells[6].Value = "ErCode: " + String.Format("0x{0:x8} [HEX]", TParameter.Mx_Connect.iReturnCode);
                                TParameter.DeviceData.ReadGridRow[i].Cells[6].Value = "ErCode: " + String.Format("0x{0:x8} [HEX]", TParameter.Mx_Connect.iReturnCode);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤(GetDevice)", "MDeviceGet_Read", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
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
                if (TParameter.DeviceData.Write_IsUse[i].ToString() == "1")//表示有使用
                {
                    //若為軟元件區間
                    if (TParameter.DeviceData.Write_Address[i].ToString().Contains("~"))
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgGetBlockCombine(TParameter.DeviceData.Write_Address[i].ToString(), out string sOutPutCell);

                            if (TParameter.Mx_Connect.iReturnCode == 0)
                            {
                                //dgv_WriteDataGrid.Rows[i].Cells[6].Value = sOutPutCell;
                                TParameter.DeviceData.WriteGridRow[i].Cells[6].Value = sOutPutCell;
                            }
                            else
                            {
                                TParameter.DeviceData.WriteGridRow[i].Cells[6].Value = "ErCode: " + String.Format("0x{0:x8} [HEX]", TParameter.Mx_Connect.iReturnCode);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤", "MDeviceGet_Write", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
                        }
                    }
                    else//其他軟元件
                    {
                        try
                        {
                            TParameter.Mx_Connect.iReturnCode = TParameter.Mx_Connect.Prog_Connect.GetDevice(TParameter.DeviceData.Write_Address[i].ToString(), out int iOutPut);
                            if (TParameter.Mx_Connect.iReturnCode == 0)
                            {
                                
                                //sOutPutCell = iOutPut.ToString();
                                //dgv_WriteDataGrid.Rows[i].Cells[6].Value = sOutPutCell;
                                TParameter.DeviceData.WriteGridRow[i].Cells[6].Value = iOutPut.ToString();
                            }
                            else
                            {
                                TParameter.DeviceData.WriteGridRow[i].Cells[6].Value = "ErCode: " + String.Format("0x{0:x8} [HEX]", TParameter.Mx_Connect.iReturnCode);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤(GetDevice)", "MDeviceGet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
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
    }
}
