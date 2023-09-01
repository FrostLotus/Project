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
using System.IO;

namespace TestProject
{
    public partial class LittleSysForm : Form
    {
        SqlConnection connection;
        public LittleSysForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 初始化資料庫連接
            string connectionString = "your_connection_string";
            connection = new SqlConnection(connectionString);

            // 在控制項中顯示環境參數
            string envVariable = Environment.GetEnvironmentVariable("YOUR_ENV_VARIABLE");
            textBoxEnvVariable.Text = envVariable;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // 讀取資料庫資料並顯示在 DataGridView
            string query = "SELECT * FROM YourTable";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;
        }

        private void SaveToFile_Click(object sender, EventArgs e)
        {
            // 將 DataGridView 中的資料存入檔案
            using (StreamWriter writer = new StreamWriter("data.txt"))
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    writer.WriteLine(row.Cells["Column1"].Value + "," + row.Cells["Column2"].Value);
                }
            }
        }

        private void CheckData_Click(object sender, EventArgs e)
        {
            // 檢查資料是否合法，顯示錯誤提示
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("請輸入有效資料");
            }
            else
            {
                // 執行相關操作
            }
        }
    }
    
}
