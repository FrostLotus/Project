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
    public partial class MainForm : Form
    {
        
        //@"Data Source=AOI-142\\SQLEXPRESS;Initial Catalog=MVC_TestDB;User ID=AOI;Password = aoi0817;"(string)
     
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
        public MainForm()
        {
            InitializeComponent();

            AccessForm form = new AccessForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                TParam.CopyTreeNodes(TParam.treeView.Nodes, treeView1.Nodes);
            }
            

           
            //All_dgv_ReFlash();
            
        }
        private void All_dgv_ReFlash()
        {
            Dgv_Flash_By_DataSet(Dgv_Customers,   TParam.sTable_Customers,   SQLDataSet);
            Dgv_Flash_By_DataSet(Dgv_Orderdetail, TParam.sTable_Orderdetail, SQLDataSet);
            Dgv_Flash_By_DataSet(Dgv_Orders,      TParam.sTable_Orders,      SQLDataSet);
            Dgv_Flash_By_DataSet(Dgv_Product,     TParam.sTable_Product,     SQLDataSet);
        }
        private void Dgv_Flash_By_DataSet(DataGridView dgv,string tableName,DataSet dataset)
        {
            using (SqlConnection cnn = new SqlConnection(TParam.sqlConSB.ConnectionString))
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
                using (SqlConnection cnn = new SqlConnection(TParam.sqlConSB.ConnectionString))
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
            using (SqlConnection cnn = new SqlConnection(TParam.sqlConSB.ConnectionString))
            {
                cnn.Open();

                string squery = $"SELECT address FROM {TParam.sTable_Customers}";
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
