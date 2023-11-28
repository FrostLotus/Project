using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
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
        public MainForm()
        {
            InitializeComponent();

            AccessForm form = new AccessForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                TParam.CopyTreeNodes(TParam.treeView.Nodes, Tv_DataBaseList.Nodes);
            }
            else
            {
                form.Close();
                //this.Close();
                System.Environment.Exit(0);
                
            }
        }
        /// <summary>
        /// 鍵入資料表進 DataSet中
        /// </summary>
        /// <param name="tableName">資料表名稱</param>
        /// <param name="dataset">目標快取資料集</param>
        /// <param name="nowRowCount">資料表Row數目</param>
        private void SELECT_Data_To_DataTable(string tableName, DataSet dataset, out int nowRowCount)
        {
            nowRowCount = 0;
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
                            if (dataset.Tables.Contains(tableName))
                            {
                                dataset.Tables[tableName].Clear();
                            }
                            adapter.Fill(dataset, tableName);
                            nowRowCount = dataset.Tables[tableName].Rows.Count;
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
            //---------------------------------------
            //OdbcConnectionStringBuilder strDBCon = new OdbcConnectionStringBuilder
            //{
            //    Driver = "AOI-142\\SQLEXPRESS",
            //    ["SERVER"] = "localhost",
            //    ["UID"] = "AOI",
            //    ["PWD"] = "aoi0817",
            //    ["Database"] = "MVC_TestDB"
            //};
            //string strDBCon = "DRIVER ={ SQL Server}; SERVER = localhost; UID = AOI; PWD = aoi0817; Database = MVC_TestDB;";
            //string strCmd = $"SELECT * FROM {tableName}";
            //using (OdbcConnection odbcCon = new OdbcConnection(strDBCon.ConnectionString))
            //{
            //    using (OdbcDataAdapter odbcAdapter = new OdbcDataAdapter(strCmd, odbcCon))
            //    {
            //        try
            //        {
            //            if (dataset.Tables.Contains(tableName))
            //            {
            //                dataset.Tables[tableName].Clear();
            //            }
            //            odbcAdapter.Fill(dataset, tableName);
            //            nowRowCount = dataset.Tables[tableName].Rows.Count;
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine($"{ex.StackTrace}\n\n{ex.Message}");
            //            MessageBox.Show($"{ex.StackTrace}\n\n{ex.Message}", "Dgv_Flash_By_DataSet", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //    }
            //}
        }
        /// <summary>
        /// 更新對應資料進儲存資料庫
        /// </summary>
        /// <param name="dgv">顯示之DataGirdView</param>
        /// <param name="tableName">資料表名稱</param>
        private void UPDATE_Data_To_DataTable(DataGridView dgv,string tableName)
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
        }
        /// <summary>
        /// 新增對應資料進儲存資料庫
        /// </summary>
        /// <param name="dgv">顯示之DataGirdView</param>
        /// <param name="tableName">資料表名稱</param>
        private void INSERT_Data_To_DataTable(DataGridView dgv, string tableName)
        {
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
        }
        /// <summary>
        /// 從儲存資料庫刪除對應資料
        /// </summary>
        /// <param name="Where_Row_Value">目標Row標號</param>
        private void DELETE_Data_To_DataTable(DataGridView dgv)
        {
            using (SqlConnection cnn = new SqlConnection(TParam.sqlConSB.ConnectionString))
            {
                cnn.Open();
                foreach (DataGridViewRow row in dgv.SelectedRows)
                {
                    int id = (int)row.Cells["id"].Value;
                    string squery = $"DELETE {TParam.sPreDataTable} WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(squery, cnn))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                    }
                }
                cnn.Close();
                cnn.Dispose();
            }
        }
        /// <summary>
        ///  更新與新增對應資料進儲存資料庫 並刷新快取資料集
        /// </summary>
        /// <param name="dgv">顯示之DataGirdView</param>
        /// <param name="tableName">資料表名稱</param>
        /// <param name="dataset">目標快取資料集</param>
        private void Dgv_SetValue(DataGridView dgv, string tableName, DataSet dataset)
        {
            //更新(修改)
            UPDATE_Data_To_DataTable(dgv, tableName);
            //新增
            INSERT_Data_To_DataTable(dgv, tableName);
            //原值讀回來
            SELECT_Data_To_DataTable(tableName, dataset, out TParam.iPreRowCount);
        }
        //----------------------------------------------------------------
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
                SELECT_Data_To_DataTable(sDataTable, TParam.SQLDataSet, out TParam.iPreRowCount);//鍵入資料表資料
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
            //若執行delete 判斷
            if (MessageBox.Show("是否刪除選取之列並更新", $"資料表: {TParam.sPreDataTable}", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true;//不刪除
            }
            else
            {
                e.Cancel = false;//刪除
                DELETE_Data_To_DataTable(Dgv_DataTable);
            }
        }
        private void Dgv_DataTable_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            //已結束刪除 更新資料表與DataGridView
            SELECT_Data_To_DataTable(TParam.sPreDataTable, TParam.SQLDataSet, out TParam.iPreRowCount);

        }
    }
}
