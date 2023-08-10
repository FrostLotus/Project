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
            //DeviceGet事件
            Timer_DeviceGet.Stop();
            Timer_DeviceGet.Interval = TParameter.Mx_Connect.iReciveTime;
            //Timer_DeviceGet.Elapsed += On_DeviceGet;

            DataGridLabelFlash();//DataGrid刷新
        } 
        
        private void mi_DataGridLoad_Click(object sender, EventArgs e)
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
                    DataGridLabelFlash();
                }
            }
            else
            {
                MessageBox.Show("格式運作中 無法載入", "DataGridLoad", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        private void mi_DataGridSave_Click(object sender, EventArgs e)
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
        private void mi_PLCSet_Click(object sender, EventArgs e)
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

        private void btn_ModelChange_Click(object sender, EventArgs e)
        {
            ModelChange();
        }
        private void btn_DataUpdate_Click(object sender, EventArgs e)
        {
            DataUpdate();
        }
        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            TParameter.Mx_Connect.ProgClose();
        }
        //---------------------------------------
        public void DataUpdate()
        {
            Timer_DeviceGet.Stop();//岔斷運行模式

            string sOutPutCell;
            //上傳寫入
            for (int i = 0; i < TParameter.DeviceData.Write_SN.Count; i++)
            {
                if (TParameter.DeviceData.Write_IsUse[i].ToString() == "1")//有使用的檢索
                {
                    if (dgv_WriteDataGrid.Rows[i].Cells[7].Value != null)//上傳寫入值不為null
                    {
                        if (dgv_WriteDataGrid.Rows[i].Cells[7].Value.ToString() != "")//上傳寫入值不為""
                        {
                            //若為軟元件區間
                            if (TParameter.DeviceData.Write_Address[i].ToString().Contains("~"))//帶區間"~"
                            {
                                try
                                {
                                    TParameter.Mx_Connect.ProgWriteBlockCombine(TParameter.DeviceData.Write_Address[i].ToString(), dgv_WriteDataGrid.Rows[i].Cells[7].Value.ToString());
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
                                    TParameter.Mx_Connect.Prog_Connect.SetDevice(TParameter.DeviceData.Write_Address[i].ToString(), Convert.ToInt32(dgv_WriteDataGrid.Rows[i].Cells[7].Value));
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message + "\n上傳寫入資料填充有誤.\n請確認是否未輸入資料\n","DataUpload",MessageBoxButtons.OK,MessageBoxIcon.Warning);
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
                            TParameter.Mx_Connect.ProgReadBlockCombine(TParameter.DeviceData.Write_Address[i].ToString(), out sOutPutCell);
                            dgv_WriteDataGrid.Rows[i].Cells[6].Value = sOutPutCell;
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
                        int iOutPut;
                        try
                        {
                            TParameter.Mx_Connect.Prog_Connect.GetDevice(TParameter.DeviceData.Write_Address[i].ToString(), out iOutPut);
                            sOutPutCell = iOutPut.ToString();
                            dgv_WriteDataGrid.Rows[i].Cells[6].Value = sOutPutCell;
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
            Timer_DeviceGet.Start();
        }
        public void DataGridLabelFlash()
        {
            //餵入資料畫面DataGridView刷新
            #region 下載
            dgv_ReadDataGrid.Rows.Clear();//清除原本的
            TParameter.DeviceData.ReadGridRow = new DataGridViewRow[TParameter.DeviceData.Read_SN.Count];//以DataGridViewRow加入較快
            if (TParameter.DeviceData.Read_SN.Count != 0)
            {
                for (int i = 0; i < TParameter.DeviceData.Read_SN.Count; i++)
                {
                    TParameter.DeviceData.ReadGridRow[i] = new DataGridViewRow();
                    TParameter.DeviceData.ReadGridRow[i].CreateCells(dgv_ReadDataGrid);//以下載的datagridview座基底新增料表元素
                    TParameter.DeviceData.ReadGridRow[i].Cells[0].Value = TParameter.DeviceData.Read_SN[i].ToString();
                    TParameter.DeviceData.ReadGridRow[i].Cells[1].Value = TParameter.DeviceData.Read_LabelName[i].ToString();
                    TParameter.DeviceData.ReadGridRow[i].Cells[2].Value = TParameter.DeviceData.Read_Address[i].ToString();
                    TParameter.DeviceData.ReadGridRow[i].Cells[3].Value = TParameter.DeviceData.Read_DataType[i].ToString();
                    TParameter.DeviceData.ReadGridRow[i].Cells[4].Value = TParameter.DeviceData.Read_Data[i].ToString();
                    TParameter.DeviceData.ReadGridRow[i].Cells[5].Value = TParameter.DeviceData.ZeroToBool(TParameter.DeviceData.Read_IsUse[i].ToString());
                    TParameter.DeviceData.ReadGridRow[i].Cells[6].Value = "";
                }
            }
            dgv_ReadDataGrid.Rows.AddRange(TParameter.DeviceData.ReadGridRow);//貼回下載datagridview
            #endregion

            #region 上傳
            dgv_WriteDataGrid.Rows.Clear();//清除原本的
            TParameter.DeviceData.WriteGridRow = new DataGridViewRow[TParameter.DeviceData.Write_SN.Count];//以DataGridViewRow加入較快
            if (TParameter.DeviceData.Write_SN.Count != 0)
            {
                for (int i = 0; i < TParameter.DeviceData.Write_SN.Count; i++)
                {
                    TParameter.DeviceData.WriteGridRow[i] = new DataGridViewRow();
                    TParameter.DeviceData.WriteGridRow[i].CreateCells(dgv_WriteDataGrid);//以上傳的datagridview座基底新增料表元素
                    TParameter.DeviceData.WriteGridRow[i].Cells[0].Value = TParameter.DeviceData.Write_SN[i].ToString();
                    TParameter.DeviceData.WriteGridRow[i].Cells[1].Value = TParameter.DeviceData.Write_LabelName[i].ToString();
                    TParameter.DeviceData.WriteGridRow[i].Cells[2].Value = TParameter.DeviceData.Write_Address[i].ToString();
                    TParameter.DeviceData.WriteGridRow[i].Cells[3].Value = TParameter.DeviceData.Write_DataType[i].ToString();
                    TParameter.DeviceData.WriteGridRow[i].Cells[4].Value = TParameter.DeviceData.Write_Data[i].ToString();
                    TParameter.DeviceData.WriteGridRow[i].Cells[5].Value = TParameter.DeviceData.ZeroToBool(TParameter.DeviceData.Write_IsUse[i].ToString());
                    TParameter.DeviceData.WriteGridRow[i].Cells[6].Value = "";
                    TParameter.DeviceData.WriteGridRow[i].Cells[7].Value = "";
                }
            }
            dgv_WriteDataGrid.Rows.AddRange(TParameter.DeviceData.WriteGridRow);//貼回上傳datagridview
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
                    Timer_DeviceGet.Stop();
                    Timer_DeviceGet.Elapsed -= On_DeviceGet;
                    Lb_Status.Text = "進入修改模式";
                    break;
                case 1: //執行
                    p_ModelChange.BackColor = Color.OrangeRed;
                    btn_ModelChange.Text = "運作模式中";
                    try
                    {
                        TParameter.Mx_Connect.iReciveTime = Convert.ToInt32(txt_ReadTime.Text);
                        Timer_DeviceGet.Interval = TParameter.Mx_Connect.iReciveTime = Convert.ToInt32(txt_ReadTime.Text);
                    }
                    catch
                    {
                        MessageBox.Show("回傳時間設定錯誤\n請確認是否為空值或負值且不為文字\n改為預設值2000ms", "ModelChange", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        TParameter.Mx_Connect.iReciveTime = 2000;//兩秒
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
            //MDeviceGet();
            Cycle_DeviceGet();
            this.BeginInvoke(new UpdateLabel(Update_Label), new object[] { this.Lb_Status, "目前時間為: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") });
            Timer_DeviceGet.Start();
        }
        private void MDeviceGet()
        {


            string sOutPutCell;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            //下載讀取
            for (int i = 0; i < TParameter.DeviceData.Read_SN.Count; i++)
            {
                if (TParameter.DeviceData.Read_IsUse[i].ToString() == "1")//表示有使用
                {
                    //若為軟元件區間
                    if (TParameter.DeviceData.Read_Address[i].ToString().Contains("~"))
                    {
                        try
                        {
                            TParameter.Mx_Connect.ProgReadBlockCombine(TParameter.DeviceData.Read_Address[i].ToString(), out sOutPutCell);

                            if (TParameter.Mx_Connect.iReturnCode == 0)
                            {
                                dgv_ReadDataGrid.Rows[i].Cells[6].Value = sOutPutCell;
                            }
                            else
                            {
                                //顯示errorcode於輸出值當中
                                dgv_ReadDataGrid.Rows[i].Cells[6].Value = "ErCode: " + String.Format("0x{0:x8} [HEX]", TParameter.Mx_Connect.iReturnCode);
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
                        int iOutPut;
                        try
                        {
                            TParameter.Mx_Connect.Prog_Connect.GetDevice(TParameter.DeviceData.Read_Address[i].ToString(), out iOutPut);
                            sOutPutCell = iOutPut.ToString();
                            dgv_ReadDataGrid.Rows[i].Cells[6].Value = sOutPutCell;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤(GetDevice)", "MDeviceGet_Read", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
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
                            TParameter.Mx_Connect.ProgReadBlockCombine(TParameter.DeviceData.Write_Address[i].ToString(), out sOutPutCell);

                            if (TParameter.Mx_Connect.iReturnCode == 0)
                            {
                                dgv_WriteDataGrid.Rows[i].Cells[6].Value = sOutPutCell;
                            }
                            else
                            {
                                dgv_WriteDataGrid.Rows[i].Cells[6].Value = "ErCode: " + String.Format("0x{0:x8} [HEX]", TParameter.Mx_Connect.iReturnCode);
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
                        int iOutPut;
                        try
                        {
                            TParameter.Mx_Connect.Prog_Connect.GetDevice(TParameter.DeviceData.Write_Address[i].ToString(), out iOutPut);
                            sOutPutCell = iOutPut.ToString();
                            dgv_WriteDataGrid.Rows[i].Cells[6].Value = sOutPutCell;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n是否為string[]之軟元件名稱錯誤(GetDevice)", "MDeviceGet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ModelChange();//強制切換
                        }
                    }
                }
            }
            //測試
            //int test = 0;
            //TParameter.Mx_Connect.Prog_Connect.GetDevice("D16077", out test);
            //switch (test)
            //{
            //    case 0:
            //        TParameter.DeviceData.ReadGridRow[4].Cells[6].Value = "1";
            //        TParameter.Mx_Connect.Prog_Connect.SetDevice("D16077", 1);
            //        break;
            //    case 1:
            //        TParameter.DeviceData.ReadGridRow[4].Cells[6].Value = "2";
            //        TParameter.Mx_Connect.Prog_Connect.SetDevice("D16077", 2);
            //        break;
            //    case 2:
            //        TParameter.DeviceData.ReadGridRow[4].Cells[6].Value = "0";
            //        TParameter.Mx_Connect.Prog_Connect.SetDevice("D16077", 0);
            //        break;
            //}
            sw.Stop();
            TimeSpan trim = sw.Elapsed;
            //this.BeginInvoke(new UpdateControl(UpdateControl_Label), new object[] { this.txt_Status, "目前時間為: " + DateTime.Now.ToString() });
            Console.WriteLine("迴圈1次時間: " + trim + "\n目前時間: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
        }


        private void Cycle_DeviceGet()
        {
            string sOutPutCell;
            int iOutPut;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            //下載讀取
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
                            TParameter.Mx_Connect.ProgReadBlockCombine(TParameter.DeviceData.Read_Address[i].ToString(), out sOutPutCell);

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
                            TParameter.Mx_Connect.iReturnCode = TParameter.Mx_Connect.Prog_Connect.GetDevice(TParameter.DeviceData.Read_Address[i].ToString(), out iOutPut);
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
                            TParameter.Mx_Connect.ProgReadBlockCombine(TParameter.DeviceData.Write_Address[i].ToString(), out sOutPutCell);

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
                            if (TParameter.Mx_Connect.iReturnCode == 0)
                            {
                                TParameter.Mx_Connect.iReturnCode = TParameter.Mx_Connect.Prog_Connect.GetDevice(TParameter.DeviceData.Write_Address[i].ToString(), out iOutPut);
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
                }
            }
            #endregion
            //餵回去顯示用dataGridView
            this.BeginInvoke(new UpdateDataGridView(Update_DataGridView), new object[] { this.dgv_ReadDataGrid, TParameter.DeviceData.ReadGridRow });
            this.BeginInvoke(new UpdateDataGridView(Update_DataGridView), new object[] { this.dgv_WriteDataGrid, TParameter.DeviceData.WriteGridRow});
            sw.Stop();
            TimeSpan trim = sw.Elapsed;
            Console.WriteLine("迴圈1次時間: " + trim + "\n目前時間: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
        }


        //委派更新
        void Update_DataGridView(DataGridView View, DataGridViewRow[] Data)
        {
            lock (this._objLock)
            {
                View.Rows.Clear();
                View.Rows.AddRange(Data);
            }
        }
        void Update_Label(Label lab, string Msg)
        {
            lock (this._objLock)
            {
                if (lab is Label)
                    ((Label)lab).Text = Msg;
            }
        }

        

        


    }
}
