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
        private CParameter m_Param = new CParameter();
        private System.Timers.Timer Timer_DeviceGet = new System.Timers.Timer();

        delegate void UpdateLabel(Label lab, string Msg);
        delegate void UpdateDataGridView(DataGridView view, DataGridViewRow[] data);
        private object _objLock = new object();
        public Stopwatch swStopwatch = new Stopwatch();

        public Main_Form()
        {
            InitializeComponent();

            m_Param.init();//初始化 然後連線

            txt_ReadTime.Text = m_Param.iReciveTime.ToString();
            //直接路徑連結
            if (m_Param.iReturnCode == 0)
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
            Timer_DeviceGet.Interval = m_Param.iReciveTime;

            DataGridValueFlash();//DataGridview 第一次刷新
        }

        private void Mi_DataGridLoad_Click(object sender, EventArgs e)
        {
            if (m_Param.iModelChange != 1)
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
                    m_Param.DeviceFileData_Path = path;
                    //取代預設路徑檔案中的指定資料表路徑
                    using (StreamWriter writer = new StreamWriter(m_Param.DataFile_Path))
                    {
                        writer.Write("");//清除
                        writer.WriteLine(m_Param.DeviceFileData_Path);
                        writer.WriteLine(m_Param.DeviceFilePLC_Path);
                        writer.Close();
                    }

                    m_Param.LoadData_ReadWrite();
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
                m_Param.DeviceFileData_Path = path;
                //取代預設路徑檔案中的指定資料表路徑
                using (StreamWriter writer = new StreamWriter(m_Param.DataFile_Path))
                {
                    writer.Write("");//清除
                    writer.WriteLine(m_Param.DeviceFileData_Path);
                    writer.WriteLine(m_Param.DeviceFilePLC_Path);
                    writer.Close();
                }

                m_Param.SaveData_ReadWrite();

                MessageBox.Show("儲存成功", "格式儲存", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void Mi_PLCSet_Click(object sender, EventArgs e)
        {
            m_Param.ProgClose();//先關閉
            Form_PLC_Set form = new Form_PLC_Set();
            if (form.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("完成PLC設定");
                m_Param.SaveData_Model();//儲存PLC連線資料
                if (m_Param.iReturnCode == 0)
                {
                    p_MxOpenStatus.BackColor = Color.Lime;
                    btn_MxOpen.Text = "PLC連結中";
                }
                if (m_Param.iReturnCode != 0)
                {
                    p_MxOpenStatus.BackColor = Color.Red;
                    btn_MxOpen.Text = "PLC連結失敗";
                }
            }
        }
        //-----------
        private void Btn_ModelChange_Click(object sender, EventArgs e)
        {
            ModelChange();
        }
        private void Btn_DataUpdate_Click(object sender, EventArgs e)
        {
            DataUpdate();
        }
        //-----------
        private void Dgv_ReadDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            //Read - 去撈Arraylist裡面所有的資料=>DataGridView
            if (e.RowIndex < m_Param.lReadData.Count)
            {
                switch (dgv_ReadDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Read_SN":
                        e.Value = m_Param.lReadData[e.RowIndex].SN.ToString();
                        break;
                    case "Read_Label":
                        e.Value = m_Param.lReadData[e.RowIndex].Label.ToString();
                        break;
                    case "Read_Address":
                        e.Value = m_Param.lReadData[e.RowIndex].Address.ToString();
                        break;
                    case "Read_DataType":
                        e.Value = m_Param.lReadData[e.RowIndex].DataType.ToString();
                        break;
                    case "Read_Data":
                        e.Value = m_Param.lReadData[e.RowIndex].Data.ToString();
                        break;
                    case "Read_IsUse":
                        e.Value = m_Param.ZeroToBool(m_Param.lReadData[e.RowIndex].IsUse.ToString());
                        break;
                    case "Read_DeviceValueGet":
                        e.Value = m_Param.lReadData[e.RowIndex].DeviceValueGet.ToString();
                        break;
                }
            }
        }
        private void Dgv_ReadDataGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            //Read - 若對應儲存格中發生改變則跳入
            //分為超出範圍(新增) 與 範圍內(修改)
            if (e.RowIndex < m_Param.lReadData.Count)
            {
                switch (dgv_ReadDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Read_SN":
                        m_Param.lReadData[e.RowIndex].SN = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Read_Label":
                        m_Param.lReadData[e.RowIndex].Label = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Read_Address":
                        m_Param.lReadData[e.RowIndex].Address = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Read_DataType":
                        m_Param.lReadData[e.RowIndex].DataType = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Read_Data":
                        m_Param.lReadData[e.RowIndex].Data = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Read_IsUse":
                        m_Param.lReadData[e.RowIndex].IsUse = m_Param.BoolToZero(e.Value);
                        break;
                    case "Read_DeviceValueGet":
                        m_Param.lReadData[e.RowIndex].DeviceValueGet = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                }
            }
            else if (e.Value != null)
            {
                CDataStruct cData = new CDataStruct();
                //若增加則下面先新增預設值
                switch (dgv_ReadDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Read_SN":
                        cData.SN = e.Value.ToString();
                        break;
                    case "Read_Label":
                        cData.Label = e.Value.ToString();
                        break;
                    case "Read_Address":
                        cData.Address = e.Value.ToString();
                        break;
                    case "Read_DataType":
                        cData.DataType = e.Value.ToString();
                        break;
                    case "Read_Data":
                        cData.Data = e.Value.ToString();
                        break;
                    case "Read_IsUse":
                        cData.IsUse = m_Param.BoolToZero(e.Value);
                        break;
                    case "Read_DeviceValueGet":
                        cData.DeviceValueGet = ("N/A");
                        break;
                }
                m_Param.lReadData.Add(cData);
            }
            dgv_ReadDataGrid.InvalidateRow(e.RowIndex);
        }
        private void Dgv_WriteDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            //Write - 去撈Arraylist裡面所有的資料=>DataGridView
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                //Write - 去撈list<>裡面所有的資料=>DataGridView
                if (e.RowIndex < m_Param.lWriteData.Count)
                {
                    switch (dgv_WriteDataGrid.Columns[e.ColumnIndex].Name)
                    {
                        case "Write_SN":
                            e.Value = m_Param.lWriteData[e.RowIndex].SN.ToString();
                            break;
                        case "Write_Label":
                            e.Value = m_Param.lWriteData[e.RowIndex].Label.ToString();
                            break;
                        case "Write_Address":
                            e.Value = m_Param.lWriteData[e.RowIndex].Address.ToString();
                            break;
                        case "Write_DataType":
                            e.Value = m_Param.lWriteData[e.RowIndex].DataType.ToString();
                            break;
                        case "Write_Data":
                            e.Value = m_Param.lWriteData[e.RowIndex].Data.ToString();
                            break;
                        case "Write_IsUse":
                            e.Value = m_Param.ZeroToBool(m_Param.lWriteData[e.RowIndex].IsUse.ToString());
                            break;
                        case "Write_DeviceValueGet":
                            e.Value = m_Param.lWriteData[e.RowIndex].DeviceValueGet.ToString();
                            break;
                        case "Write_DeviceValueSet":
                            e.Value = m_Param.lWriteData[e.RowIndex].DeviceValueSet.ToString();
                            break;
                    }
                }
            }
        }
        private void Dgv_WriteDataGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            //Write - 若對應儲存格中發生改變則跳入
            //分為超出範圍(新增) 與 範圍內(修改)
            if (e.RowIndex < m_Param.lWriteData.Count)
            {
                switch (dgv_WriteDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Write_SN":
                        m_Param.lWriteData[e.RowIndex].SN = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Write_Label":
                        m_Param.lWriteData[e.RowIndex].Label = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Write_Address":
                        m_Param.lWriteData[e.RowIndex].Address = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Write_DataType":
                        m_Param.lWriteData[e.RowIndex].DataType = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Write_Data":
                        m_Param.lWriteData[e.RowIndex].Data = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Write_IsUse":
                        m_Param.lWriteData[e.RowIndex].IsUse = m_Param.BoolToZero(e.Value);
                        break;
                    case "Write_DeviceValueGet":
                        m_Param.lWriteData[e.RowIndex].DeviceValueGet = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                    case "Write_DeviceValueSet":
                        m_Param.lWriteData[e.RowIndex].DeviceValueSet = (e.Value != null) ? e.Value.ToString() : "";
                        break;
                }
            }
            else if (e.Value != null)//新增
            {
                CDataStruct cData = new CDataStruct();
                //若增加則下面先新增預設值
                switch (dgv_WriteDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Write_SN":
                        cData.SN = e.Value.ToString();
                        break;
                    case "Write_Label":
                        cData.Label = e.Value.ToString();
                        break;
                    case "Write_Address":
                        cData.Address = e.Value.ToString();
                        break;
                    case "Write_DataType":
                        cData.DataType = e.Value.ToString();
                        break;
                    case "Write_Data":
                        cData.Data = e.Value.ToString();
                        break;
                    case "Write_IsUse":
                        cData.IsUse = m_Param.BoolToZero(e.Value);
                        break;
                    case "Write_DeviceValueGet":
                        cData.DeviceValueGet = "N/A";
                        break;
                    case "Write_DeviceValueSet":
                        cData.DeviceValueSet = "";
                        break;
                }
                m_Param.lWriteData.Add(cData);
            }
            dgv_WriteDataGrid.InvalidateRow(e.RowIndex);
        }
        //-----------
        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_Param.ProgClose();
        }
        //======================================================================================================
        public void DataUpdate()
        {
            string sOutPutCell;

            for (int i = 0; i < m_Param.lWriteData.Count; i++)
            {
                //上傳寫入
                switch (m_Param.SetDataStatus(i, ReadWriteStatus.IsWrite, GetSetStatus.IsSet))
                {
                    case ValueStatus.IsEmpty:
                        break;
                    case ValueStatus.IsArrayDevice:
                        try
                        {
                            m_Param.ProgSetBlockCombine(m_Param.lWriteData[i].Address, m_Param.lWriteData[i].DeviceValueSet);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為修改值string[]之軟元件名稱錯誤", "DataUpload", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
                        }
                        break;
                    case ValueStatus.IsSingleDevice:
                        try
                        {
                            m_Param.ProgSetDevice(m_Param.lWriteData[i].Address, m_Param.lWriteData[i].DeviceValueSet);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n上傳寫入資料填充有誤.\n請確認是否未輸入資料\n", "DataUpload", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ModelChange();//強制切換
                        }
                        break;
                }
                //上傳讀取
                switch (m_Param.SetDataStatus(i, ReadWriteStatus.IsWrite, GetSetStatus.IsGet))
                {
                    case ValueStatus.IsEmpty:
                        break;
                    case ValueStatus.IsArrayDevice:
                        try
                        {
                            m_Param.ProgGetBlockCombine(m_Param.lWriteData[i].Address.ToString(), out sOutPutCell);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤(~)", "DataUpload讀取", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
                        }
                        break;
                    case ValueStatus.IsSingleDevice:
                        try
                        {
                            m_Param.ProgGetDevice(m_Param.lWriteData[i].Address.ToString(), out sOutPutCell);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為word之軟元件名稱錯誤(Device)", "DataUpload讀取", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
                        }
                        break;
                }
            }
            Console.WriteLine("上傳成功");
        }
        public void DataGridValueFlash()
        {
            //餵入資料畫面DataGridView刷新
            dgv_ReadDataGrid.RowCount = m_Param.lReadData.Count + 1;
            dgv_WriteDataGrid.RowCount = m_Param.lWriteData.Count + 1;
        }
        public void ModelChange()
        {
            m_Param.SaveData_ModChange(dgv_ReadDataGrid, dgv_WriteDataGrid);

            switch (m_Param.iModelChange)
            {
                case 0: //修改
                    p_ModelChange.BackColor = Color.LightBlue;
                    btn_ModelChange.Text = "格式修改模式中";
                    Lb_Status.Text = "進入修改模式";

                    txt_ReadTime.Enabled = true;
                    Timer_DeviceGet.Elapsed -= On_DeviceGet;
                    Timer_DeviceGet.Stop();
                    break;
                case 1: //執行
                    p_ModelChange.BackColor = Color.OrangeRed;
                    btn_ModelChange.Text = "運作模式中";

                    if (txt_ReadTime.Text != "" && Convert.ToInt32(txt_ReadTime.Text) > 0)
                    {
                        Timer_DeviceGet.Interval = m_Param.iReciveTime = Convert.ToInt32(txt_ReadTime.Text);
                    }
                    else
                    {
                        //超出範圍預設值
                        m_Param.ErrorMessageBox_Time();
                        m_Param.iReciveTime = 2000;//兩秒
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
            Cycle_DeviceGet();
        }
        private void Cycle_DeviceGet()
        {
            swStopwatch.Restart();
            string arrGetData = "";//全字串("\n"為分號)
            int iTotalItem = 0;//全總數
            ///全部字串讀取
            #region 下發讀取
            for (int i = 0; i < m_Param.lReadData.Count; i++)//SN_count
            {
                if (m_Param.lReadData[i].IsUse == "1")//表示有觸發
                {
                    if (m_Param.lReadData[i].Address.Contains("~"))//若為軟元件區間
                    {
                        m_Param.GetCombineArray_str(m_Param.lReadData[i].Address, out int iItemCount, out string sItemStr);
                        //增加軟元件總數
                        iTotalItem += iItemCount;
                        //串Random用字串
                        arrGetData += (arrGetData == "") ? sItemStr : "\n" + sItemStr;
                    }
                    else//其他軟元件
                    {
                        iTotalItem += 1;// 增加軟元件總數(單一device)
                        //串Random用字串
                        arrGetData += (arrGetData == "") ? m_Param.lReadData[i].Address: "\n" + m_Param.lReadData[i].Address;
                    }
                }
            }
            #endregion
            #region 上傳讀取
            for (int i = 0; i < m_Param.lWriteData.Count; i++)
            {
                if (m_Param.lWriteData[i].IsUse == "1")//表示有觸發
                {
                    if (m_Param.lWriteData[i].Address.Contains("~"))//若為軟元件區間
                    {
                        m_Param.GetCombineArray_str(m_Param.lWriteData[i].Address, out int iItemCount, out string sItemStr);
                        iTotalItem += iItemCount;//增加軟元件總數
                        //串Random用字串
                        arrGetData += (arrGetData == "") ? sItemStr : "\n" + sItemStr;

                    }
                    else//其餘單一元件
                    {
                        iTotalItem += 1;// 增加軟元件總數(單一device)
                        //串Random用字串
                        arrGetData += (arrGetData == "") ? m_Param.lWriteData[i].Address: "\n" + m_Param.lWriteData[i].Address;
                    }
                }
            }
            #endregion
            //取全部元件的值(ReadRandom)
            m_Param.ProgGetDeviceRandom(arrGetData, iTotalItem, out short[] arrDeviceData);
            int iOrderCount = 0;
            ///全部字串更新
            #region 下發更新
            for (int i = 0; i < m_Param.lReadData.Count; i++)//SN_count
            {
                if (m_Param.lReadData[i].IsUse == "1")//表示有觸發
                {
                    if (m_Param.lReadData[i].Address.Contains("~"))//若為軟元件區間
                    {
                        //string sTmp = "";
                        iOrderCount = m_Param.GetCombineByte(m_Param.lReadData[i].Address, arrDeviceData, iOrderCount, out byte[] total, out int iDeviceSize);
                        //判斷寫入格式
                        if (iDeviceSize == 2|| m_Param.lReadData[i].DataType=="real")//Float 先用|| 後改&& 
                        {
                            m_Param.lReadData[i].DeviceValueGet = BitConverter.ToSingle(total, 0).ToString();
                        }
                        else//string[?]
                        {
                            m_Param.lReadData[i].DeviceValueGet = System.Text.Encoding.ASCII.GetString(total);
                        }
                    }
                    else//單一元件
                    {
                        m_Param.lReadData[i].DeviceValueGet = arrDeviceData[iOrderCount].ToString();
                        iOrderCount++;
                    }
                    dgv_ReadDataGrid.InvalidateCell((int)CData.DeviceValueGet, i);
                }
            }
            #endregion
            #region 上傳更新
            for (int i = 0; i < m_Param.lWriteData.Count; i++)//SN_count
            {
                if (m_Param.lWriteData[i].IsUse == "1")//表示有觸發
                {
                    if (m_Param.lWriteData[i].Address.Contains("~"))//若為軟元件區間
                    {
                        iOrderCount = m_Param.GetCombineByte(m_Param.lWriteData[i].Address, arrDeviceData, iOrderCount, out byte[] total, out int iDeviceSize);
                        //判斷寫入格式
                        if (iDeviceSize == 2 || m_Param.lWriteData[i].DataType == "real")//Float 先用|| 後改&& 
                        {
                            m_Param.lWriteData[i].DeviceValueGet = BitConverter.ToSingle(total, 0).ToString();
                        }
                        else//string[?]
                        {
                            m_Param.lWriteData[i].DeviceValueGet = System.Text.Encoding.ASCII.GetString(total);
                        }
                    }
                    else//單一元件
                    {
                        m_Param.lWriteData[i].DeviceValueGet = arrDeviceData[iOrderCount].ToString();
                        iOrderCount++;
                    }
                    dgv_WriteDataGrid.InvalidateCell((int)CData.DeviceValueGet, i);//列更新
                }
            }
            #endregion
            if (iOrderCount != iTotalItem)
            {
                Console.WriteLine("填值有問題");
                MessageBox.Show("更新值有問題", "Cycle_DeviceGet", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            swStopwatch.Stop();
            TimeSpan trim = swStopwatch.Elapsed;
            Console.WriteLine("迴圈1次時間: " + trim + "\n目前時間: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
        }
    }
}
