using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DataBaseTest
{
    public partial class AccessForm : Form
    {
        public AccessForm()
        {
            InitializeComponent();
            TParam.Init();
        }

        private void Btn_Connect_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            //若登入成功
            TParam.sDataSource = $"{Cb_DataSource.SelectedItem}";
            //TParam.sInitialCatalog = Cb_InitialCatalog.SelectedItem.ToString();
            TParam.sUserID = Tb_UserID.Text;
            TParam.sPassword = Tb_Password.Text;
            TParam.bIntegrated_Security = true;
            //----------------------------------------------------
            //需修改
            //TParam.sTable_Customers = "Table_Customers";
            //TParam.sTable_Orderdetail = "Table_Orderdetial";
            //TParam.sTable_Orders = "Table_orders";
            //TParam.sTable_Product = "Table_Products";

            TParam.Build_SqlConSB();

            //測試資料庫是否連線成功
            //取得內涵所有資料庫
            try
            {
                using (SqlConnection cnn = new SqlConnection(TParam.sqlConSB.ConnectionString))
                {
                    cnn.Open();
                    MessageBox.Show("資料庫開啟成功!");
                    Fst_Layer_Set(cnn, TParam.sDataSet);
                    Snd_Layer_Set(cnn, TParam.sDataSet);


                    cnn.Close();//可不用 using會藉由 IDisposable 清除
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OPEN", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }


        private void Fst_Layer_Set(SqlConnection scnn, List<string> ds)
        {
            string squery = $"SELECT * FROM sys.databases";//選所有資料庫
            using (SqlCommand command = new SqlCommand(squery, scnn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    int name = reader.GetOrdinal("name");
                    while (reader.Read())
                    {
                        string stmp = reader.GetString(name);
                        ds.Add(stmp);
                        Console.WriteLine(stmp);
                    }
                }
            }
        }
        private void Snd_Layer_Set(SqlConnection cnn, List<string> ds)
        {
            for (int i = 0; i < ds.Count; i++)
            {
                TreeNode rootNode = new TreeNode(ds[i]);//資料庫節點建立
                List<string> innerList = new List<string>();
                //------------------------------------------
                string tmp_squery = $"USE {ds[i]};SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                using (SqlCommand command = new SqlCommand(tmp_squery, cnn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int name = reader.GetOrdinal("TABLE_NAME");
                        while (reader.Read())
                        {
                            string stemp = reader.GetString(name);
                            innerList.Add(stemp);
                            TreeNode Node = new TreeNode(stemp);//每一資料表建立並加入
                            rootNode.Nodes.Add(Node);
                            Console.WriteLine("--->" + stemp);
                        }
                        TParam.sFullDataSet.Add(innerList);//將樹狀鍵入List
                        TParam.treeView.Nodes.Add(rootNode);//將樹狀鍵入容器
                    }
                }


            }
        }
    }
}
