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
        private string Host;
        private string Port;
        private string User;
        private string Password;

        private readonly string DBname = "aoi_lst_db";
        private readonly string DBtable_Defects = "defects";
        private readonly string DBtable_Unit = "unit";

        private readonly string TableParameter = "Parameter";
        private readonly string TableUnit = "Unit";
        private readonly string TableView = "View";

        string connString;
        DataSet dataSet;

        //顯示圖片
        string BasePath;
        string ImgPath;
        string FullImgPath;

        //工單參數
        int LstUid;
        string LstOrder;
        double DetectLength;
        string PartNum;
        string OrderTime;

        public MainForm()
        {
            InitializeComponent();
            dataSet = new DataSet();
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
                query = $"SELECT *" +
                        $"FROM {DBtable_Defects} " +
                        $"WHERE classtype = @FilterValue  AND lstid = @FilterValue1";
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
                //將相機單元參數鍵入 table
                query = "SELECT *" +
                         $"FROM {DBtable_Unit} " +
                         "ORDER BY uid ASC";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    // 填充資料到 DataSet
                    new NpgsqlDataAdapter(command).Fill(dataSet, TableUnit);
                }
            }
            //[第一次]工單參數(視窗傳輸資料)
            Lab_Order.Text = LstOrder;                                                 //工單編號
            Lab_FlawNum.Text = dataSet.Tables[TableParameter].Rows.Count.ToString();   //瑕疵點總量
            Lab_TotalLength.Text = DetectLength.ToString() + "m";                      //檢測總長度
            Lab_Material.Text = PartNum;                                               //料號
            Lab_Date.Text = UnixTimeChangeDateStr(OrderTime);                          //檢測日期

            //[第一次]預設第一行先選取 顯示反白(Form顯示DataGridView檢索)
            DataGridViewRow selectedRow = Dgv_DefectListView.Rows[0];
            selectedRow.Selected = true;
            
            if (dataSet.Tables[TableParameter] != null && dataSet.Tables[TableParameter].Rows.Count > 0)
            {
                //[第一次]顯示圖片(背景資料檢索)
                if (dataSet.Tables[TableParameter].Rows[0]["imgname"] != DBNull.Value)
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

                    }
                }
                //[第一次]瑕疵參數顯示(背景資料檢索)
                if (dataSet.Tables[TableParameter].Rows[0]["distance"] != DBNull.Value)
                {
                    Lab_Length.Text = dataSet.Tables[TableParameter].Rows[0]["distance"].ToString() + "m";//距離
                }
                if (dataSet.Tables[TableParameter].Rows[0]["speed"] != DBNull.Value)
                {
                    Lab_Speed.Text = dataSet.Tables[TableParameter].Rows[0]["speed"].ToString() + "m/s";             //速度
                }
                if (dataSet.Tables[TableParameter].Rows[0]["usertype"] != DBNull.Value)
                {
                    Lab_FlawType.Text = FlawChangeStr(dataSet.Tables[TableParameter].Rows[0]["usertype"].ToString());//瑕疵類別
                }
                if (dataSet.Tables[TableParameter].Rows[0]["area"] != DBNull.Value)
                {
                    Lab_Point.Text = dataSet.Tables[TableParameter].Rows[0]["area"].ToString();//點數
                }
                if (dataSet.Tables[TableParameter].Rows[0]["area"] != DBNull.Value &&
                    dataSet.Tables[TableUnit].Rows[0]["area_fov"] != DBNull.Value)
                {
                    Lab_Area.Text = PointToArea(dataSet.Tables[TableParameter].Rows[0]["area"].ToString(),
                                                dataSet.Tables[TableUnit].Rows[0]["area_fov"].ToString());//面積
                }
                if (dataSet.Tables[TableParameter].Rows[0]["detecttime"] != DBNull.Value)
                {
                    Lab_DetectTime.Text = dataSet.Tables[TableParameter].Rows[0]["detecttime"].ToString();//檢測時間
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

                if (dataSet.Tables[TableParameter] != null && dataSet.Tables[TableParameter].Rows.Count > 0)
                {
                    //顯示圖片(背景資料檢索)
                    if (dataSet.Tables[TableParameter].Rows[e.RowIndex]["imgname"] != DBNull.Value)
                    {
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
                        }
                    }
                    //瑕疵參數顯示(背景資料檢索)
                    if (dataSet.Tables[TableParameter].Rows[e.RowIndex]["distance"] != DBNull.Value)
                    {
                        Lab_Length.Text = dataSet.Tables[TableParameter].Rows[e.RowIndex]["distance"].ToString() + "m";//距離
                    }
                    if (dataSet.Tables[TableParameter].Rows[e.RowIndex]["speed"] != DBNull.Value)
                    {
                        Lab_Speed.Text = dataSet.Tables[TableParameter].Rows[e.RowIndex]["speed"].ToString() + "m/s";             //速度
                    }
                    if (dataSet.Tables[TableParameter].Rows[e.RowIndex]["usertype"] != DBNull.Value)
                    {
                        Lab_FlawType.Text = FlawChangeStr(dataSet.Tables[TableParameter].Rows[e.RowIndex]["usertype"].ToString());//瑕疵類別
                    }
                    if (dataSet.Tables[TableParameter].Rows[e.RowIndex]["area"] != DBNull.Value)
                    {
                        Lab_Point.Text = dataSet.Tables[TableParameter].Rows[e.RowIndex]["area"].ToString();//點數
                    }
                    if (dataSet.Tables[TableParameter].Rows[e.RowIndex]["area"] != DBNull.Value && dataSet.Tables[TableUnit].Rows[0]["area_fov"] != DBNull.Value)
                    {
                        Lab_Area.Text = PointToArea(dataSet.Tables[TableParameter].Rows[e.RowIndex]["area"].ToString(),
                                                    dataSet.Tables[TableUnit].Rows[0]["area_fov"].ToString());//面積
                    }
                    if (dataSet.Tables[TableParameter].Rows[e.RowIndex]["detecttime"] != DBNull.Value)
                    {
                        Lab_DetectTime.Text = dataSet.Tables[TableParameter].Rows[e.RowIndex]["detecttime"].ToString();//檢測時間
                    }
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
            string re = "fail to get time";
            if (long.TryParse(unixTime, out long tmp_long))
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(tmp_long);
                dateTimeOffset = dateTimeOffset.ToOffset(TimeSpan.FromHours(8));
                DateTime dateTime = new DateTime(dateTimeOffset.DateTime.Year, dateTimeOffset.DateTime.Month, dateTimeOffset.DateTime.Day);
                re = dateTime.ToString("yyyy-MM-dd"); // 將值轉換為 DateTime 字串 並格式化輸出
            }
            return re;
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
        public string PointToArea(string area, string areafov)
        {
            string re = "fail to convert area";
            if (double.TryParse(area, out double a) && double.TryParse(areafov, out double f))
            {
                double region = a * f;
                re = $"{region} mm²";//面積
            }
            return re;
        }
        public void SetParam(int tmp_LstUid, string tmp_LstOrder, double tmp_DetectLength, string tmp_PartNum, string tmp_OrderTime)
        {
            LstUid = tmp_LstUid;
            LstOrder = tmp_LstOrder;
            DetectLength = tmp_DetectLength;
            PartNum = tmp_PartNum;
            OrderTime = tmp_OrderTime;
        }
        public void SetLogin(string tmp_Host, string tmp_Port, string tmp_User, string tmp_Password)
        {
            Host = tmp_Host;
            Port = tmp_Port;
            User = tmp_User;
            Password = tmp_Password;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
