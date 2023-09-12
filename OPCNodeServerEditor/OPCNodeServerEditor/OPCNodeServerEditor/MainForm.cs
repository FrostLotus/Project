using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Opc.Ua.Configuration;
using Opc.Ua.Server;

namespace OPCNodeServerEditor
{
    public partial class MainForm : Form
    {
        private ApplicationInstance UsingApplication;
        private ServerFlag ServerFlags;


        public MainForm()
        {
            InitializeComponent();
        }

        private void Btn_Run_Click(object sender, EventArgs e)
        {
            if (ServerFlags == ServerFlag.Stop)
            {
                UsingApplication.Server.Start(UsingApplication.ApplicationConfiguration);//伺服器啟動

                serverControl1.UpdateTimer_Start();
                Btn_Run.Enabled = false;
                Btn_Run.BackColor = Color.ForestGreen;
                Btn_Stop.Enabled = true;
                Btn_Run.BackColor = Color.DarkGray;

                Lab_Status.Text = $"Server啟動";
                ServerFlags = ServerFlag.Start;
            }
        }
    }
}
