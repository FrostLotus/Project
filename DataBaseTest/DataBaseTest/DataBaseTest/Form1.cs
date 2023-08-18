using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataBaseTest
{
    public partial class Form1 : Form
    {
        public SqlConnectionStringBuilder sConnB;
        //@"Data Source=AOI-142\\SQLEXPRESS;Initial Catalog=MVC_TestDB;User ID=AOI;Password = aoi0817;"(string)
        public string sDataSource = "AOI-142\\SQLEXPRESS";
        public string sInitialCatalog = "MVC_TestDB";
        public string sUserID = "AOI";
        public string sPassword = "aoi0817";

        public string sTable_Customers = "Table_Customers";
        public string sTable_Orderdetail = "Table_Orderdetial";
        public string sTable_Orders = "Table_orders";
        public string sTable_Product = "Table_Products";

        public DataSet SQLDataSet = new DataSet();

        public class CustomersDB
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Tel { get; set; }
            public string Address { get; set; }
            public string InitDate { get; set; }
        }
        public class OrderdetailDB
        {
            public string Id { get; set; }
            public string OrderID { get; set; }
            public string ProductID { get; set; }
            public string Amount { get; set; }
            public string Price { get; set; }
            public string Total { get; set; }
            public string Memo { get; set; }
            public string InitDate { get; set; }
        }
        public class OrdersDB
        {
            public string Id { get; set; }
            public string CustomerID { get; set; }
            public string Total { get; set; }
            public string InitDate { get; set; }

        }
        public class ProductDB
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Price { get; set; }
            public string InitDate { get; set; }
        }
        //-----------------------
        public Form1()
        {
            InitializeComponent();
            sConnB = new SqlConnectionStringBuilder()
            {
                //建立之系統OK
                DataSource = sDataSource,
                InitialCatalog = sInitialCatalog,
                UserID = sUserID,
                Password = sPassword
            };
            Stopwatch swStopwatch = new Stopwatch();
            swStopwatch.Restart();
            All_dgv_ReFlash();
            swStopwatch.Stop();
            TimeSpan trim = swStopwatch.Elapsed;
            Console.WriteLine("迴圈1次時間: " + trim + "\n目前時間: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
        }

        private void All_dgv_ReFlash()
        {
            Dgv_Flash_By_DataSet(Dgv_Customers,   sTable_Customers,   SQLDataSet);
            Dgv_Flash_By_DataSet(Dgv_Orderdetail, sTable_Orderdetail, SQLDataSet);
            Dgv_Flash_By_DataSet(Dgv_Orders,      sTable_Orders,      SQLDataSet);
            Dgv_Flash_By_DataSet(Dgv_Product,     sTable_Product,     SQLDataSet);
        }
        private void Dgv_Flash_By_DataSet(DataGridView dgv,string tableName,DataSet dataset)
        {
            using (SqlConnection cnn = new SqlConnection(sConnB.ConnectionString))
            {
                cnn.Open();

                string squery = $"SELECT * FROM {tableName}";
                using (SqlCommand command = new SqlCommand(squery,cnn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataset, tableName);
                        dgv.DataSource = dataset.Tables[tableName];
                    }
                }
                cnn.Close();//可不用 using會藉由 IDisposable 清除
                cnn.Dispose();
            }
        }
        private void Btn_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(sConnB.ConnectionString))
                {
                    cnn.Open();
                    MessageBox.Show("資料庫開啟成功!");
                    cnn.Close();//可不用 using會藉由 IDisposable 清除
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OPEN", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Btn_Read_Click(object sender, EventArgs e)
        {
            using (SqlConnection cnn = new SqlConnection(sConnB.ConnectionString))
            {
                cnn.Open();

                string squery = $"SELECT address FROM {sTable_Customers}";
                using (SqlCommand command = new SqlCommand(squery, cnn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int address = reader.GetOrdinal("Address");

                        while (reader.Read())
                        {
                            Console.WriteLine(reader.GetString(address));
                        }
                    }
                }
                cnn.Close();//可不用 using會藉由 IDisposable 清除
                cnn.Dispose();
            }
        }

    }
}
