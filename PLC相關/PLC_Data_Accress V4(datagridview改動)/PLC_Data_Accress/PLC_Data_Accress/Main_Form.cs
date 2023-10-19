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
        private TParameter m_Param = new TParameter();
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
                        if (e.RowIndex < m_Param.Read_SN.Count)
                            e.Value = m_Param.Read_SN[e.RowIndex];
                        break;
                    case "Read_Label":
                        if (e.RowIndex < m_Param.Read_Label.Count)
                            e.Value = m_Param.Read_Label[e.RowIndex];
                        break;
                    case "Read_Address":
                        if (e.RowIndex < m_Param.Read_Address.Count)
                            e.Value = m_Param.Read_Address[e.RowIndex];
                        break;
                    case "Read_DataType":
                        if (e.RowIndex < m_Param.Read_DataType.Count)
                            e.Value = m_Param.Read_DataType[e.RowIndex];
                        break;
                    case "Read_Data":
                        if (e.RowIndex < m_Param.Read_Data.Count)
                            e.Value = m_Param.Read_Data[e.RowIndex].ToString();
                        break;
                    case "Read_IsUse":
                        if (e.RowIndex < m_Param.Read_IsUse.Count)
                            e.Value = m_Param.ZeroToBool(m_Param.Read_IsUse[e.RowIndex]);
                        break;
                    case "Read_DeviceValueGet":
                        if (e.RowIndex < m_Param.Read_DeviceValueGet.Count)
                            e.Value = m_Param.Read_DeviceValueGet[e.RowIndex];
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
                    if (e.RowIndex >= m_Param.Read_SN.Count)
                    {
                        m_Param.Read_SN.Add(e.Value.ToString());
                        //若SN增加則下面先新增預設值
                        m_Param.Read_Label.Add("");
                        m_Param.Read_Address.Add("");
                        m_Param.Read_DataType.Add("");
                        m_Param.Read_Data.Add("");
                        m_Param.Read_IsUse.Add("0");
                        m_Param.Read_DeviceValueGet.Add("N/A");
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
                        m_Param.Read_SN[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Read_Label":
                    if (e.RowIndex >= m_Param.Read_Label.Count)
                    {
                        m_Param.Read_Label.Add(e.Value.ToString());
                    }
                    else
                    {
                        m_Param.Read_Label[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Read_Address":
                    if (e.RowIndex >= m_Param.Read_Address.Count)
                    {
                        m_Param.Read_Address.Add(e.Value.ToString());
                    }
                    else
                    {
                        m_Param.Read_Address[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Read_DataType":
                    if (e.RowIndex >= m_Param.Read_DataType.Count)
                    {
                        m_Param.Read_DataType.Add(e.Value.ToString());
                    }
                    else
                    {
                        m_Param.Read_DataType[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Read_Data":
                    if (e.RowIndex >= m_Param.Read_Data.Count)
                    {
                        m_Param.Read_Data.Add(e.Value.ToString());
                    }
                    else
                    {
                        m_Param.Read_Data[e.RowIndex] = e.Value.ToString();
                    }
                    break;
                case "Read_IsUse":
                    if (e.RowIndex >= m_Param.Read_IsUse.Count)
                    {
                        m_Param.Read_IsUse.Add(e.Value.ToString());
                    }
                    else
                    {
                        m_Param.Read_IsUse[e.RowIndex] = m_Param.BoolToZero(e.Value);
                    }
                    break;
                case "Read_DeviceValueGet":
                    if (e.RowIndex >= m_Param.Read_DeviceValueGet.Count)
                    {
                        m_Param.Read_DeviceValueGet.Add(e.Value.ToString());
                    }
                    else
                    {
                        m_Param.Read_DeviceValueGet[e.RowIndex] = e.Value.ToString();
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
                        if (e.RowIndex < m_Param.Write_SN.Count)
                            e.Value = m_Param.Write_SN[e.RowIndex];
                        break;
                    case "Write_Label":
                        if (e.RowIndex < m_Param.Write_Label.Count)
                            e.Value = m_Param.Write_Label[e.RowIndex];
                        break;
                    case "Write_Address":
                        if (e.RowIndex < m_Param.Write_Address.Count)
                            e.Value = m_Param.Write_Address[e.RowIndex];
                        break;
                    case "Write_DataType":
                        if (e.RowIndex < m_Param.Write_DataType.Count)
                            e.Value = m_Param.Write_DataType[e.RowIndex];
                        break;
                    case "Write_Data":
                        if (e.RowIndex < m_Param.Write_Data.Count)
                            e.Value = m_Param.Write_Data[e.RowIndex].ToString();
                        break;
                    case "Write_IsUse":
                        if (e.RowIndex < m_Param.Write_IsUse.Count)
                            e.Value = m_Param.ZeroToBool(m_Param.Write_IsUse[e.RowIndex]);
                        break;
                    case "Write_DeviceValueGet":
                        if (e.RowIndex < m_Param.Write_DeviceValueGet.Count)
                            e.Value = m_Param.Write_DeviceValueGet[e.RowIndex];
                        break;
                    case "Write_DeviceValueSet":
                        if (e.RowIndex < m_Param.Write_DeviceValueSet.Count)
                            e.Value = m_Param.Write_DeviceValueSet[e.RowIndex];
                        break;
                }
            }
        }
        private void Dgv_WriteDataGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.Value != null)
            {
                //Write - 若對應儲存格中發生改變則跳入
                switch (dgv_WriteDataGrid.Columns[e.ColumnIndex].Name)
                {
                    case "Write_SN":
                        if (e.RowIndex >= m_Param.Write_SN.Count)
                        {
                            m_Param.Write_SN.Add(e.Value.ToString());
                            //若SN增加則下面先新增預設值
                            m_Param.Write_Label.Add("");
                            m_Param.Write_Address.Add("");
                            m_Param.Write_DataType.Add("");
                            m_Param.Write_Data.Add("");
                            m_Param.Write_IsUse.Add("0");
                            m_Param.Write_DeviceValueGet.Add("N/A");
                            m_Param.Write_DeviceValueSet.Add("");
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
                            m_Param.Write_SN[e.RowIndex] = e.Value.ToString();
                        }
                        break;
                    case "Write_Label":
                        if (e.RowIndex >= m_Param.Write_Label.Count)
                        {
                            m_Param.Write_Label.Add(e.Value.ToString());
                        }
                        else
                        {
                            m_Param.Write_Label[e.RowIndex] = e.Value.ToString();
                        }
                        break;
                    case "Write_Address":
                        if (e.RowIndex >= m_Param.Write_Address.Count)
                        {
                            m_Param.Write_Address.Add(e.Value.ToString());
                        }
                        else
                        {
                            m_Param.Write_Address[e.RowIndex] = e.Value.ToString();
                        }
                        break;
                    case "Write_DataType":
                        if (e.RowIndex >= m_Param.Write_DataType.Count)
                        {
                            m_Param.Write_DataType.Add(e.Value.ToString());
                        }
                        else
                        {
                            m_Param.Write_DataType[e.RowIndex] = e.Value.ToString();
                        }
                        break;
                    case "Write_Data":
                        if (e.RowIndex >= m_Param.Write_Data.Count)
                        {
                            m_Param.Write_Data.Add(e.Value.ToString());
                        }
                        else
                        {
                            m_Param.Write_Data[e.RowIndex] = e.Value.ToString();
                        }
                        break;
                    case "Write_IsUse":
                        if (e.RowIndex >= m_Param.Write_IsUse.Count)
                        {
                            m_Param.Write_IsUse.Add(e.Value.ToString());
                        }
                        else
                        {
                            m_Param.Write_IsUse[e.RowIndex] = m_Param.BoolToZero(e.Value);
                        }
                        break;
                    case "Write_DeviceValueGet":
                        if (e.RowIndex >= m_Param.Write_DeviceValueGet.Count)
                        {
                            m_Param.Write_DeviceValueGet.Add(e.Value.ToString());
                        }
                        else
                        {
                            m_Param.Write_DeviceValueGet[e.RowIndex] = e.Value.ToString();
                        }
                        break;
                    case "Write_DeviceValueSet":
                        if (e.RowIndex >= m_Param.Write_DeviceValueSet.Count)
                        {
                            m_Param.Write_DeviceValueSet.Add(e.Value.ToString());
                        }
                        else
                        {
                            m_Param.Write_DeviceValueSet[e.RowIndex] = e.Value.ToString();
                        }
                        break;
                }
            }
        }
        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_Param.ProgClose();
        }
        //===============================================
        public void DataUpdate()
        {

            string sOutPutCell;
            //上傳寫入
            for (int i = 0; i < m_Param.Write_SN.Count; i++)
            {
                if (m_Param.Write_IsUse[i].ToString() == "1")//有使用的檢索
                {
                    if (m_Param.Write_DeviceValueSet[i] != null)//上傳寫入值不為null
                    {
                        if (m_Param.Write_DeviceValueSet[i] != "")//上傳寫入值不為""
                        {
                            //若為軟元件區間
                            if (m_Param.Write_Address[i].Contains("~"))//帶區間"~"
                            {
                                try
                                {
                                    m_Param.ProgSetBlockCombine(m_Param.Write_Address[i], m_Param.Write_DeviceValueSet[i]);
                                    if (m_Param.iReturnCode == 0)
                                    {
                                        Console.WriteLine(m_Param.Write_Address[i] + " SetOK");
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
                                    m_Param.ProgSetDevice(m_Param.Write_Address[i], m_Param.Write_DeviceValueSet[i]);
                                    if (m_Param.iReturnCode == 0)
                                    {
                                        m_Param.ProgGetDevice(m_Param.Write_Address[i].ToString(), out sOutPutCell);
                                        Console.WriteLine(m_Param.Write_Address[i] + ": " + sOutPutCell);
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
            for (int i = 0; i < m_Param.Write_SN.Count; i++)
            {
                if (m_Param.Write_IsUse[i].ToString() == "1")//表示有使用
                {
                    //若為軟元件區間
                    if (m_Param.Write_Address[i].ToString().Contains("~"))
                    {
                        try
                        {
                            m_Param.ProgGetBlockCombine(m_Param.Write_Address[i].ToString(), out sOutPutCell);
                            if (m_Param.iReturnCode == 0)
                            {
                                m_Param.Write_DeviceValueGet[i] = sOutPutCell;
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
                            m_Param.ProgGetDevice(m_Param.Write_Address[i].ToString(), out sOutPutCell);
                            if (m_Param.iReturnCode == 0)
                            {
                                m_Param.Write_DeviceValueGet[i] = sOutPutCell;
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
            Timer_DeviceGet.Start();
        }
        public void DataGridValueFlash()
        {
            //餵入資料畫面DataGridView刷新
            #region 下載
            //dgv_ReadDataGrid.Rows.Clear();//清除原本的
            dgv_ReadDataGrid.RowCount = m_Param.Read_SN.Count + 1;
            #endregion

            #region 上傳
            //dgv_WriteDataGrid.Rows.Clear();//清除原本的
            dgv_WriteDataGrid.RowCount = m_Param.Write_SN.Count + 1;
            #endregion
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
            Timer_DeviceGet.Stop();
            Cycle_DeviceGet();
            Timer_DeviceGet.Start();
        }
        private void Cycle_DeviceGet()
        {
            swStopwatch.Restart();
            //下載讀取
            #region 下載讀取
            for (int i = 0; i < m_Param.Read_SN.Count; i++)//SN_count
            {
                if (m_Param.Read_IsUse[i] == "1")//表示有使用err
                {
                    //若為軟元件區間
                    if (m_Param.Read_Address[i].Contains("~"))
                    {
                        try
                        {
                            m_Param.ProgGetBlockCombine(m_Param.Read_Address[i], out string sOutPutCell);
                            m_Param.Read_DeviceValueGet[i] = (m_Param.iReturnCode == 0) ? sOutPutCell : m_Param.ErrorStrSend();
                        }
                        catch (Exception ex)
                        {
                            m_Param.ErrorMessageBox_DeviceBlock(ex);
                            ModelChange();//強制切換
                            return;
                        }
                    }
                    else//其他軟元件
                    {
                        try
                        {
                            m_Param.ProgGetDevice(m_Param.Read_Address[i], out string sOutPut);
                            m_Param.Read_DeviceValueGet[i] = (m_Param.iReturnCode == 0) ? sOutPut : m_Param.ErrorStrSend();
                        }
                        catch (Exception ex)
                        {
                            m_Param.ErrorMessageBox_DeviceValue(ex);
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
            for (int i = 0; i < m_Param.Write_SN.Count; i++)
            {
                if (m_Param.Write_IsUse[i] == "1")//表示有使用
                {
                    //若為軟元件區間
                    if (m_Param.Write_Address[i].ToString().Contains("~"))
                    {
                        try
                        {
                            m_Param.ProgGetBlockCombine(m_Param.Write_Address[i].ToString(), out string sOutPutCell);
                            m_Param.Write_DeviceValueGet[i] = (m_Param.iReturnCode == 0) ? sOutPutCell : m_Param.ErrorStrSend();
                        }
                        catch (Exception ex)
                        {
                            m_Param.ErrorMessageBox_DeviceBlock(ex);
                            ModelChange();//強制切換
                            return;
                        }
                    }
                    else//其他軟元件
                    {
                        try
                        {
                            m_Param.ProgGetDevice(m_Param.Write_Address[i], out string sOutPutCell);
                            m_Param.Write_DeviceValueGet[i] = (m_Param.iReturnCode == 0) ? sOutPutCell : m_Param.ErrorStrSend();
                        }
                        catch (Exception ex)
                        {
                            m_Param.ErrorMessageBox_DeviceValue(ex);
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
    }
}
