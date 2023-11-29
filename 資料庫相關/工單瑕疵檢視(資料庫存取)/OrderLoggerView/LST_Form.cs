using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace OrderLoggerView
{
    public partial class LST_Form : Form
    {
        private string Host;// = "192.168.2.105";//
        private string Port;// = "54321";
        private string User;// = "postgres";//Account
        private string Password;// = "AOIuser80689917";

        private readonly string PartDisk = "K";//暫用網路磁碟代號
        private readonly string sharedFolder = "aoishare";//目標資料夾
        private readonly string DBname = "aoi_lst_db";//資料庫名稱
        private readonly string DBtable_Lst = "lst";//資料表名稱

        DataSet dataSet;  //資料表單組
        string connString;//連線命令字串
        NpgsqlDataAdapter dataAdapter;

        private int LstUid;
        private string LstOrder;
        private double DetectLength;
        private string PartNum;
        private string OrderTime;
        public LST_Form()
        {
            InitializeComponent();
            dataSet = new DataSet();
        }
        private void LST_Form_Load(object sender, EventArgs e)
        {

        }
        private void Btn_Login_Click(object sender, EventArgs e)
        {
            //先測試是否能夠登入DBsystem
            string tmp_host = Txt_Host.Text;
            string tmp_port = Txt_Port.Text;
            string tmp_user = Txt_User.Text;
            string tmp_password = Txt_Password.Text;

            string tmp_connString = $"Host={tmp_host};Port={tmp_port};Username={tmp_user};Password={tmp_password};Database={DBname}";
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(tmp_connString))
                {
                    connection.Open();
                    //先將資料參數撈取
                    string query = "SELECT *" +
                                   $"FROM {DBtable_Lst} ";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        new NpgsqlDataAdapter(command).Fill(dataSet, "test");//測試資料表寫入

                        //若能登入寫入 建立登入資料
                        if (Login_Access(tmp_connString))
                        {
                            //清單區塊顯示
                            Lab_LoginInfo.Text = $"Login success";
                            Grb_List.Visible = true;
                            Grb_Search.Visible = true;
                            //按鈕狀態 登入後無法修改
                            Btn_Login.Text = "登入成功";
                            Btn_Login.Enabled = false;

                            //登入成功參數建立
                            Host = tmp_host;
                            Port = tmp_port;
                            User = tmp_user;
                            Password = tmp_password;

                            Access_KeyAndDisk();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //不能登入情況
                Console.WriteLine($"Error: {ex.Message}");
                Lab_LoginInfo.Text = $"Error: {ex.Message}";
            }
        }
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            string search = Txt_Search.Text;//搜尋工單名稱

            if (!string.IsNullOrWhiteSpace(search))
            {
                dataSet = new DataSet();//re new
                using (NpgsqlConnection connection = new NpgsqlConnection(connString))
                {
                    connection.Open();
                    //先將資料單元參數撈取
                    string query = "SELECT *" +
                                    $"FROM {DBtable_Lst} " +
                                    "WHERE name = @FilterValue";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FilterValue", search);//2C表示有瑕疵
                        // 填充資料到 DataSet
                        new NpgsqlDataAdapter(command).Fill(dataSet, DBtable_Lst);
                        // 將 DataSet 中的資料綁定到 DataGridView
                        Dgv_Order.DataSource = dataSet.Tables[DBtable_Lst];
                    }
                }
            }
        }
        private void Btn_Clean_Click(object sender, EventArgs e)
        {
            dataSet = new DataSet();//re new
            using (NpgsqlConnection connection = new NpgsqlConnection(connString))
            {
                connection.Open();
                //先將資料單元參數撈取
                string query = "SELECT *" +
                                $"FROM {DBtable_Lst} ";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    // 填充資料到 DataSet
                    new NpgsqlDataAdapter(command).Fill(dataSet, DBtable_Lst);
                    // 將 DataSet 中的資料綁定到 DataGridView
                    Dgv_Order.DataSource = dataSet.Tables[DBtable_Lst];
                }
            }
        }
        private void Dgv_Order_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                DataGridViewColumn column = Dgv_Order.Columns[e.ColumnIndex];
                if (column.Name == "starttime") // 對 "starttime" 欄位做處理
                {
                    if (e.Value != null)
                    {
                        if (long.TryParse(e.Value.ToString(), out long unixTime))
                        {
                            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTime);
                            dateTimeOffset = dateTimeOffset.ToOffset(TimeSpan.FromHours(8));//GMT+8
                            e.Value = dateTimeOffset.DateTime.ToString(); // 將值轉換為 DateTime 字串
                            e.FormattingApplied = true; // 表示格式化已應用
                        }
                    }
                        
                }
                if (column.Name == "endtime") // 對 "endtime" 欄位做處理
                {
                    if (e.Value != null)
                    {
                        if (long.TryParse(e.Value.ToString(), out long unixTime))
                        {
                            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTime);
                            dateTimeOffset = dateTimeOffset.ToOffset(TimeSpan.FromHours(8));//GMT+8
                            e.Value = dateTimeOffset.DateTime.ToString(); // 將值轉換為 DateTime 字串
                            e.FormattingApplied = true; // 表示格式化已應用
                        }
                    }
                }
            }
        }
        private void Dgv_Order_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = Dgv_Order.Rows[e.RowIndex];
                selectedRow.Selected = true;
            }
        }
        private void Dgv_Order_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = Dgv_Order.Rows[e.RowIndex];
                selectedRow.Selected = true;

                //工單條件
                if (int.TryParse(Dgv_Order.Rows[e.RowIndex].Cells["uid"].Value.ToString(), out int tmp_int)) LstUid = tmp_int;
                LstOrder = Dgv_Order.Rows[e.RowIndex].Cells["name"].Value.ToString();
                if (double.TryParse(Dgv_Order.Rows[e.RowIndex].Cells["detectlength"].Value.ToString(), out double result)) DetectLength = result;
                PartNum = Dgv_Order.Rows[e.RowIndex].Cells["partnum"].Value.ToString();
                OrderTime = Dgv_Order.Rows[e.RowIndex].Cells["starttime"].Value.ToString();

                //this.DialogResult = DialogResult.OK;

                //選取工單 顯示主視窗
                MainForm form = new MainForm();
                form.SetLogin(Host, Port, User, Password);
                form.SetParam(LstUid, LstOrder, DetectLength, PartNum, OrderTime);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    form.Dispose();
                }
                else
                {
                    Remove_KeyAndDisk();
                    form.Close();
                    System.Environment.Exit(0);
                }
            }
        }
        private void LST_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            Remove_KeyAndDisk();
        }
        //----------------------------------------------------------
        private bool Login_Access(string conn)
        {
            bool re = false;
            try
            {
                connString = conn;
                dataAdapter = new NpgsqlDataAdapter($"SELECT * FROM {DBtable_Lst}", connString);
                dataSet = new DataSet();
                // 填充資料到 DataSet
                dataAdapter.Fill(dataSet, DBtable_Lst);
                // 將 DataSet 中的資料綁定到 DataGridView
                Dgv_Order.DataSource = dataSet.Tables[DBtable_Lst];
                re = true;
            }
            catch (Exception ex)
            {
                //不能登入情況
                Console.WriteLine($"Error: {ex.Message}");
            }
            return re;
        }
        private void Access_KeyAndDisk()
        {
            //建立cmdkey net use 網路磁碟 
            ProcessStartInfo psi = new ProcessStartInfo
            {
                Arguments = $@"/c cmdkey /add:{Host} /user:FAE /pass:AOIuser80689917" + " & " +
                            $@" net use {PartDisk}: \\{Host}\{sharedFolder} /user:FAE AOIuser80689917",
                FileName = "cmd.exe", // 指定要運行的程式
                UseShellExecute = false, // 使用 shell 執行
                RedirectStandardOutput = true, // 重定向標準輸出
                CreateNoWindow = true,// 不創建新視窗
                Verb = "runas" // 以管理員權限運行
            };
            //創建新的 ProcessStartInfo
            Process m_process = new Process();
            m_process.StartInfo = psi;
            m_process.Start();
            // 讀取命令輸出
            string output = m_process.StandardOutput.ReadToEnd();
            // 等待命令結束
            m_process.WaitForExit();
            // 輸出結果
            Console.WriteLine(output);
        }
        private void Remove_KeyAndDisk()
        {
            //關閉cmdkey net use 網路磁碟 
            ProcessStartInfo psi = new ProcessStartInfo
            {
                Arguments = $@"/c cmdkey /delete:{Host}" + " & " +
                            $@" net use {PartDisk}: /delete",
                FileName = "cmd.exe", // 指定要運行的程式
                UseShellExecute = false, // 使用 shell 執行
                RedirectStandardOutput = true, // 重定向標準輸出
                CreateNoWindow = true,// 不創建新視窗
                Verb = "runas" // 以管理員權限運行
            }; //創建新的 ProcessStartInfo
            Process m_process = new Process();
            m_process.StartInfo = psi;
            m_process.Start();
            // 讀取命令輸出
            string output = m_process.StandardOutput.ReadToEnd();
            // 等待命令結束
            m_process.WaitForExit();
            // 輸出結果
            Console.WriteLine(output);
        }
    }
}
