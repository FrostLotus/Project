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
                TParam.CopyTreeNodes(TParam.treeView.Nodes, Tv_DataBaseList.Nodes);
            }
            //test
            //TParam.Build_SqlConSB("MVC_TestDB");
            //All_dgv_ReFlash();

        }
        private void All_dgv_ReFlash()
        {
            Dgv_Flash_By_DataSet(Dgv_Customers,   TParam.sTable_Customers,   TParam.SQLDataSet);
            Dgv_Flash_By_DataSet(Dgv_Orderdetail, TParam.sTable_Orderdetail, TParam.SQLDataSet);
            Dgv_Flash_By_DataSet(Dgv_Orders,      TParam.sTable_Orders,      TParam.SQLDataSet);
            Dgv_Flash_By_DataSet(Dgv_Product,     TParam.sTable_Product,     TParam.SQLDataSet);
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
                        try
                        {
                            if (!dataset.Tables.Contains(tableName))
                            {
                                adapter.Fill(dataset, tableName);
                            }
                            
                            dgv.DataSource = dataset.Tables[tableName];
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{ex.StackTrace}\n{ex.Message}", "Dgv_Flash_By_DataSet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                cnn.Close();//可不用 using會藉由 IDisposable 清除
                cnn.Dispose();
            }
        }
        private void Dgv_SetValue_By_DataSet(DataGridView dgv, string tableName, DataSet dataset)
        {
            //先把原值讀
            using (SqlConnection cnn = new SqlConnection(TParam.sqlConSB.ConnectionString))
            {
                cnn.Open();
                string squery = $"SELECT * FROM {tableName}";
                using (SqlCommand command = new SqlCommand(squery, cnn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        try
                        {
                            if (!dataset.Tables.Contains(tableName))
                            {
                                adapter.Fill(dataset, tableName);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{ex.StackTrace}\n{ex.Message}", "Dgv_Flash_By_DataSet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                cnn.Close();//可不用 using會藉由 IDisposable 清除
                cnn.Dispose();
            }








            using (SqlConnection cnn = new SqlConnection(TParam.sqlConSB.ConnectionString))
            {
                cnn.Open();
                //List<object> list = new List<object>();
                //for (int i=0;i< dataset.Tables[tableName].Rows.Count;i++)
                //{
                //    list.Add(dataset.Tables[tableName].Rows[i].);
                //}
                //dataset.Tables[tableName].Rows[]
                // 更新修改過的資料
                for(int i=0;i< dgv.Rows.Count;i++)
                {
                    string name = dgv.Rows[i].Cells["name"].Value.ToString();
                }
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (dataset.Tables[tableName].Rows.Count> row.Index+1)
                    {
                        
                        int id          = Convert.ToInt32(row.Cells["id"].Value);
                        string name     = row.Cells["name"].Value.ToString();
                        string tel      = row.Cells["tel"].Value.ToString();
                        string address  = row.Cells["address"].Value.ToString();
                        string initDate = row.Cells["initDate"].Value.ToString();

                        //string column1Value = row.Cells["Column1"].Value.ToString();
                        //string column2Value = row.Cells["Column2"].Value.ToString();

                        string squery = $"UPDATE {tableName} SET name = @name, tel = @tel, address = @address, initDate = @initDate WHERE id = @id";
                        using (SqlCommand command = new SqlCommand(squery, cnn))
                        {
                            //command.Parameters.AddWithValue("@Column1", column1Value);
                            //command.Parameters.AddWithValue("@Column2", column2Value);

                            command.Parameters.AddWithValue("@name", name    );
                            command.Parameters.AddWithValue("@tel", tel     );
                            command.Parameters.AddWithValue("@address", address );

                            //initDate

                            command.Parameters.AddWithValue("@initDate", initDate);

                            command.Parameters.AddWithValue("@id", id);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                // 新增新的資料
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (dataset.Tables[tableName].Rows.Count < row.Index)
                    {
                        string name = row.Cells["name"].Value.ToString();
                        string tel = row.Cells["tel"].Value.ToString();
                        string address = row.Cells["address"].Value.ToString();
                        string date = row.Cells["Date"].Value.ToString();

                        string insertQuery = "INSERT INTO {tableName} (name, tel, address, Date) VALUES (@name, @tel, @address, @date)";

                        using (SqlCommand command = new SqlCommand(insertQuery, cnn))
                        {
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@tel", tel);
                            command.Parameters.AddWithValue("@address", address);
                            command.Parameters.AddWithValue("@date", date);

                            command.ExecuteNonQuery();
                        }
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

        private void Tv_DataBaseList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //父系沒資料表不理它
            if (Tv_DataBaseList.SelectedNode.Level == 1)
            {
                //Console.WriteLine("123");
                //往上找父級名稱
                
                string sDataTable = Tv_DataBaseList.SelectedNode.Text;
                string sDataBase  = Tv_DataBaseList.SelectedNode.Parent.Text;
                Lab_DataTable.Text = sDataTable;//
                TParam.Build_SqlConSB(sDataBase);//設定指定資料表位置
                Dgv_Flash_By_DataSet(Dgv_DataTable, sDataTable, TParam.SQLDataSet);
            }
            
        }
        private void Btn_FrashDataTable_Click(object sender, EventArgs e)
        {
            string sDataTable = Tv_DataBaseList.SelectedNode.Text;
            Dgv_SetValue_By_DataSet(Dgv_DataTable, sDataTable, TParam.SQLDataSet);
        }
    }
}
