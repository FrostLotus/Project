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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connetionString;
            SqlConnection cnn;
            connetionString = @"Data Source=(localdb)\MVC_TestDB;Initial Catalog=D:\MS_SQL_DB\MVC_TestDB.mdf;User ID=sa;";
            cnn = new SqlConnection(connetionString);
            
            cnn.Open();
            MessageBox.Show("Connection Open  !");
            cnn.Close();
        }
    }
}
