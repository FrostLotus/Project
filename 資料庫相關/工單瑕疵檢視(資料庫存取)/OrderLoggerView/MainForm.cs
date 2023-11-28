using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderLoggerView
{
    public partial class MainForm : Form
    {
        private static string Host;
        private static string Port;
        private static string User;
        private static string Password;

        private static string DBname = "aoi_lst_db";
        private static string DBtable_Defects = "defects";
        private static string DBtable_Unit = "unit";

        private static string TableParameter = "Parameter";
        private static string TableUnit = "Unit";
        private static string TableView = "View";

        string connString;
        DataSet dataSet = new DataSet();

        //顯示圖片
        string BasePath;
        string ImgPath;
        string FullImgPath;

        //工單參數
        static int LstUid;
        static string LstOrder;
        static double DetectLength;
        static string PartNum;
        static string OrderTime;

        public MainForm()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //字串重組合
            connString = $"Host={Host};Port={Port};Username={User};Password={Password};Database={DBname}";
            BasePath = $@"K:\\Slave1\Slave1\";

            string query;
            using (NpgsqlConnection connection = new NpgsqlConnection(connString))
            {
                connection.Open();
                //先將資料單元參數撈取
                query = "SELECT *" +
                         $"FROM {DBtable_Defects} " +
                         "WHERE classtype = @FilterValue  AND lstid = @FilterValue1";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FilterValue", "2C");//2C表示有瑕疵
                    command.Parameters.AddWithValue("@FilterValue1", LstUid);//選擇工單編號
                    // 填充資料到 DataSet
                    new NpgsqlDataAdapter(command).Fill(dataSet, TableParameter);
                }
                //再將顯示參數撈取
                query = "SELECT *" +
                        $"FROM {DBtable_Defects} " +
                        "WHERE classtype = @FilterValue  AND lstid = @FilterValue1";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FilterValue", "2C");//2C表示有瑕疵
                    command.Parameters.AddWithValue("@FilterValue1", LstUid);//選擇工單編號
                    // 填充資料到 DataSet
                    new NpgsqlDataAdapter(command).Fill(dataSet, TableView);
                    // 將 DataSet 中的資料綁定到 DataGridView
                    Dgv_DefectListView.DataSource = dataSet.Tables[TableView];
                }
                //將相機單元參數鍵入table
                query = "SELECT *" +
                         $"FROM {DBtable_Unit} " +
                         "ORDER BY uid ASC";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    // 填充資料到 DataSet
                    new NpgsqlDataAdapter(command).Fill(dataSet, TableUnit);
                }
            }
            //[第一次]預設第一行先選取 顯示反白(Form顯示DataGridView檢索)
            DataGridViewRow selectedRow = Dgv_DefectListView.Rows[0];
            selectedRow.Selected = true;
            //[第一次]工單編號參數(視窗傳輸資料)
            Lab_Order.Text = LstOrder;
            Lab_FlawNum.Text = dataSet.Tables[TableParameter].Rows.Count.ToString();
            Lab_TotalLength.Text = DetectLength.ToString() + "m";
            Lab_Material.Text = PartNum;
            Lab_Date.Text = UnixTimeChangeDateStr(OrderTime);

            //[第一次]顯示圖片(背景資料檢索)
            if (dataSet.Tables[TableParameter] != null && dataSet.Tables[TableParameter].Rows.Count > 0)
            {
                if (dataSet.Tables[TableParameter].Rows[0]["imgname"].ToString() != null)
                {
                    ImgPath = dataSet.Tables[TableParameter].Rows[0]["imgname"].ToString();
                    FullImgPath = BasePath + ImgPath;
                    if (File.Exists(FullImgPath))
                    {
                        Console.WriteLine("檔案存在。");
                        Pcb_Defect.Image = new Bitmap(FullImgPath);
                    }
                    else
                    {
                        Console.WriteLine("檔案不存在。");
                    }
                    //[第一次]瑕疵參數顯示(背景資料檢索)
                    Lab_Length.Text = dataSet.Tables[TableParameter].Rows[0]["distance"].ToString() + "m";
                    Lab_Speed.Text = dataSet.Tables[TableParameter].Rows[0]["speed"].ToString() + "m/s";
                    Lab_Point.Text = dataSet.Tables[TableParameter].Rows[0]["area"].ToString();
                    Lab_FlawType.Text = FlawChangeStr(dataSet.Tables[TableParameter].Rows[0]["usertype"].ToString());
                    Lab_Area.Text = (Convert.ToDouble(dataSet.Tables[TableParameter].Rows[0]["area"]) * Convert.ToDouble(dataSet.Tables[TableUnit].Rows[0]["area_fov"])).ToString() + "mm²";
                    Lab_DetectTime.Text = dataSet.Tables[TableParameter].Rows[0]["detecttime"].ToString();
                }
            }
        }
        private void Dgv_DefectList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //選取顯示反白(Form顯示DataGridView檢索)
                DataGridViewRow selectedRow = Dgv_DefectListView.Rows[e.RowIndex];
                selectedRow.Selected = true;
                //顯示圖片(背景資料檢索)
                if (dataSet.Tables[TableParameter] != null && dataSet.Tables[TableParameter].Rows.Count > 0)
                    if (dataSet.Tables[TableParameter].Rows[e.RowIndex]["imgname"].ToString() != "")
                    {
                        ImgPath = dataSet.Tables[TableParameter].Rows[e.RowIndex]["imgname"].ToString();
                        FullImgPath = BasePath + ImgPath;
                        if (File.Exists(FullImgPath))
                        {
                            Console.WriteLine("檔案存在。");
                            Pcb_Defect.Image = new Bitmap(FullImgPath);
                        }
                        else
                        {
                            Console.WriteLine("檔案不存在。");
                        }
                        //瑕疵參數顯示(背景資料檢索)
                        Lab_Length.Text = dataSet.Tables[TableParameter].Rows[e.RowIndex]["distance"].ToString() + "m";
                        Lab_Speed.Text = dataSet.Tables[TableParameter].Rows[e.RowIndex]["speed"].ToString() + "m/s";
                        Lab_Point.Text = dataSet.Tables[TableParameter].Rows[e.RowIndex]["area"].ToString();
                        Lab_FlawType.Text = FlawChangeStr(dataSet.Tables[TableParameter].Rows[e.RowIndex]["usertype"].ToString());
                        Lab_Area.Text = (Convert.ToDouble(dataSet.Tables[TableParameter].Rows[e.RowIndex]["area"]) * Convert.ToDouble(dataSet.Tables[TableUnit].Rows[0]["area_fov"])).ToString() + "mm²";
                        Lab_DetectTime.Text = dataSet.Tables[TableParameter].Rows[e.RowIndex]["detecttime"].ToString();
                    }

            }
        }
        private void Btn_ReTracert_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        //-----------------------------------------------------------------------------
        public string UnixTimeChangeDateStr(string unixTime)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(unixTime));
            dateTimeOffset = dateTimeOffset.ToOffset(TimeSpan.FromHours(8));
            DateTime dateTime = new DateTime(dateTimeOffset.DateTime.Year, dateTimeOffset.DateTime.Month, dateTimeOffset.DateTime.Day);
            return dateTime.ToString("yyyy-MM-dd"); // 將值轉換為 DateTime 字串 並格式化輸出
        }
        public string FlawChangeStr(string Flaw)
        {
            string re = "null";
            switch (Flaw)
            {
                case "02":
                    re = "M黑點";
                    break;
                case "13":
                    re = "蚊蟲";
                    break;
                case "06":
                    re = "S折痕";
                    break;
                case "0F":
                    re = "條紋";
                    break;
                case "0C":
                    re = "L針孔";
                    break;
                case "05":
                    re = "L折痕";
                    break;
                case "03":
                    re = "L黑點";
                    break;
                case "07":
                    re = "膠斑";
                    break;
                case "08":
                    re = "S膠粒";
                    break;
                case "04":
                    re = "LL黑點";
                    break;
                case "0A":
                    re = "缺樹脂";
                    break;
                case "10":
                    re = "棕色條紋";
                    break;
            }
            return re;
        }
        public static void SetParam(int tmp_LstUid, string tmp_LstOrder, double tmp_DetectLength, string tmp_PartNum, string tmp_OrderTime)
        {
             LstUid = tmp_LstUid;
             LstOrder = tmp_LstOrder;
             DetectLength = tmp_DetectLength;
             PartNum = tmp_PartNum;
             OrderTime = tmp_OrderTime;
        }
        public static void SetLogin(string tmp_Host, string tmp_Port, string tmp_User, string tmp_Password)
        {
            Host = tmp_Host;
            Port = tmp_Port;
            User = tmp_User;
            Password = tmp_Password;
        }
        public bool TestDBConnect()
        {
            bool re = false;
            return re;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
