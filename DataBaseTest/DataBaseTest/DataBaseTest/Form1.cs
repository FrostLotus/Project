using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataBaseTest
{
    public partial class Form1 : Form
    {
        //SqlConnection cnn;
        SqlConnectionStringBuilder sConnB;
        //SqlCommand command;
        //@"Data Source=AOI-142\\SQLEXPRESS;Initial Catalog=MVC_TestDB;User ID=AOI;Password = aoi0817;"(string)
        string sDataSource = "AOI-142\\SQLEXPRESS";
        string sInitialCatalog = "MVC_TestDB";
        string sUserID = "AOI";
        string sPassword = "aoi0817";

        string sTable_Customers = "Table_Customers";
        string sTable_Orderdetial = "Table_Orderdetial";
        string sTable_orders = "Table_orders";
        string sTable_Products = "Table_Products";

        enum Customers
        {
        }

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
        }
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                using(SqlConnection cnn = new SqlConnection(sConnB.ConnectionString))
                {
                    cnn.Open();
                    MessageBox.Show("資料庫開啟成功!");
                    cnn.Close();//可不用 using會藉由 IDisposable 清除
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "OPEN", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btn_Read_Click(object sender, EventArgs e)
        {
            using (SqlConnection cnn = new SqlConnection(sConnB.ConnectionString))
            {
                cnn.Open();

                string squery = $"SELECT address FROM {sTable_Customers}";
                using (SqlCommand command = new SqlCommand(squery,cnn))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int address = reader.GetOrdinal("Address");

                        while (reader.Read())
                        {
                            //string address = reader["Address"].ToString();
                            //Console.WriteLine(address);
                            Console.WriteLine(reader.GetString(address));
                        }
                    }
                }

                cnn.Close();//可不用 using會藉由 IDisposable 清除
            }
        }

    }
}
