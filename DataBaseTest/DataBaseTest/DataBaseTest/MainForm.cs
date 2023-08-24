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
        private void Set_Data_To_DataTable(string tableName, DataSet dataset, out int RowCount)
        {
            using (SqlConnection cnn = new SqlConnection(TParam.sqlConSB.ConnectionString))
            {
                cnn.Open();
                RowCount = 0;
                string squery = $"SELECT * FROM {tableName}";
                using (SqlCommand command = new SqlCommand(squery, cnn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        try
                        {
                            if (dataset.Tables.Contains(tableName))
                            {
                                dataset.Tables[tableName].Clear();
                            }
                            adapter.Fill(dataset, tableName);
                            RowCount = dataset.Tables[tableName].Rows.Count;
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
        private void Dgv_SetValue(DataGridView dgv, string tableName, DataSet dataset)
        {
            #region 更新
            using (SqlConnection cnn = new SqlConnection(TParam.sqlConSB.ConnectionString))
            {
                cnn.Open();
                for (int i = 0; i < dgv.Rows.Count - 1; i++)
                {
                    if (TParam.iPreRowCount > dgv.Rows[i].Index)//小於原值為新增
                    {
                        int id = Convert.ToInt32(dgv.Rows[i].Cells["id"].Value);
                        string name = dgv.Rows[i].Cells["name"].Value.ToString();
                        string tel = dgv.Rows[i].Cells["tel"].Value.ToString();
                        string address = dgv.Rows[i].Cells["address"].Value.ToString();
                        string initDate = dgv.Rows[i].Cells["initDate"].Value.ToString();

                        string squery = $"UPDATE {tableName} SET name = @name, tel = @tel, address = @address, initDate = @initDate WHERE id = @id";
                        using (SqlCommand command = new SqlCommand(squery, cnn))
                        {
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@tel", tel);
                            command.Parameters.AddWithValue("@address", address);
                            command.Parameters.AddWithValue("@initDate", initDate);
                            command.Parameters.AddWithValue("@id", id);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                cnn.Close();
                cnn.Dispose();
            }
            #endregion

            #region 新增
            using (SqlConnection cnn = new SqlConnection(TParam.sqlConSB.ConnectionString))
            {
                cnn.Open();
                // 新增新的資料
                for (int i = 0; i < dgv.Rows.Count - 1; i++)
                {
                    if (TParam.iPreRowCount < dgv.Rows[i].Index + 1)//超出原row數為新增
                    {
                        string name = dgv.Rows[i].Cells["name"].Value != null ? dgv.Rows[i].Cells["name"].Value.ToString() : "";
                        string tel = dgv.Rows[i].Cells["tel"].Value != null ? dgv.Rows[i].Cells["tel"].Value.ToString() : "";
                        string address = dgv.Rows[i].Cells["address"].Value != null ? dgv.Rows[i].Cells["address"].Value.ToString() : "";

                        string insertQuery = $"INSERT INTO {tableName} (name, tel, address) VALUES (@name, @tel, @address)";
                        using (SqlCommand command = new SqlCommand(insertQuery, cnn))
                        {
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@tel", tel);
                            command.Parameters.AddWithValue("@address", address);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                cnn.Close();//可不用 using會藉由 IDisposable 清除
                cnn.Dispose();
            }
            #endregion

            //原值讀回來
            Set_Data_To_DataTable(tableName, dataset, out TParam.iPreRowCount);
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
            int BaseNode = 1;
            //父系沒資料表不理它
            if (Tv_DataBaseList.SelectedNode.Level >= BaseNode)
            {
                //往上找父級名稱
                string sDataTable = Tv_DataBaseList.SelectedNode.Text;//資料表名稱
                string sDataBase = Tv_DataBaseList.SelectedNode.Parent.Text;//資料庫名稱
                TParam.Build_SqlConSB(sDataBase);//設定指定資料表位置
                Set_Data_To_DataTable(sDataTable, TParam.SQLDataSet, out TParam.iPreRowCount);//鍵入資料表資料
                Dgv_DataTable.DataSource = TParam.SQLDataSet.Tables[sDataTable];//輸出至DataGirdView
                Lab_DataTable.Text = sDataTable;//顯示資料表標籤名稱
                TParam.sPreDataTable = sDataTable;//紀錄選取資料表



            }
        }
        private void Btn_FrashDataTable_Click(object sender, EventArgs e)
        {
            if (TParam.sPreDataTable != "")
            {
                string sDataTable = Tv_DataBaseList.SelectedNode.Text;
                Dgv_SetValue(Dgv_DataTable, sDataTable, TParam.SQLDataSet);
            }
            else
            {
                Lab_DataTable.Text = "未選取資料表";
            }
        }

        private void Dgv_DataTable_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            //若執行delete
            if (MessageBox.Show("是否刪除選取之列並更新", $"資料表: {TParam.sPreDataTable}", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true;//不刪除
            }
            else
            {
                e.Cancel = false;//刪除

                //e.Row
                using (SqlConnection cnn = new SqlConnection(TParam.sqlConSB.ConnectionString))
                {
                    cnn.Open();
                    int id = (int)e.Row.Cells["id"].Value;
                    string squery = $"DELETE {TParam.sPreDataTable} WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(squery, cnn))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                    }
                    cnn.Close();
                    cnn.Dispose();
                }
            }
        }

        private void Dgv_DataTable_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            ///已結束刪除
            //更新資料表與DataGridView
            Set_Data_To_DataTable(TParam.sPreDataTable, TParam.SQLDataSet, out TParam.iPreRowCount);
            Console.WriteLine("delete done");

        }
    }
}
