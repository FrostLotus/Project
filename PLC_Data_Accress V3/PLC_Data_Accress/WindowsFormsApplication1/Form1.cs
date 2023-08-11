using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
	public enum ABC
	{
		A=1,
		B=2
	}	
        public Form1()
        {
            InitializeComponent();
        }
        List<int> lList = new List<int>();

        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < lList.Count)
                e.Value = lList[e.RowIndex];
        }
        private void button1_Click(object sender, EventArgs e)
        {
            lList.Add(lList.Count);
            dataGridView1.RowCount = lList.Count;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lList[0] = lList.Count;
            lList.Add(lList.Count);
            dataGridView1.InvalidateCell(0, 0);//強制寫入dataGridView
        }
    }
}
