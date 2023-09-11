using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;

namespace OCPUAServer
{
    enum ServerFlag
    {
        Start = 1,
        Stop = 2,
        pause = 3
    }
    public partial class MainForm : Form
    {
        private ApplicationInstance UsingApplication;
        private ServerFlag ServerFlags;
        public MainForm()
        {
            InitializeComponent();
        }
        public MainForm(ApplicationInstance application)
        {
            InitializeComponent();

            UsingApplication = application;

            if (application.Server is StandardServer)
            {
                //serverDiagnosticsCtrl1.Initialize((StandardServer)application.Server, application.ApplicationConfiguration);
                serverControl1.Initialize((StandardServer)UsingApplication.Server, UsingApplication.ApplicationConfiguration);
                ServerFlags = ServerFlag.Start;
            }
            //其他狀態
            Btn_Run.Enabled = false;
            Btn_Run.BackColor = Color.ForestGreen;
            Lab_Status.Text = "Server以視窗開啟中";
        }

        #region Event Handlers
        
        #endregion

        private void Btn_Add_Click(object sender, EventArgs e)
        {

        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {

        }

        private void Btn_UpdateFile_Click(object sender, EventArgs e)
        {

        }

        private void Btn_Run_Click(object sender, EventArgs e)
        {
            if (ServerFlags == ServerFlag.Stop)
            {
                UsingApplication.Server.Start(UsingApplication.ApplicationConfiguration);

                serverControl1.UpdateTimer_Start();
                Btn_Run.Enabled = false;
                Btn_Run.BackColor = Color.ForestGreen;
                Btn_Stop.Enabled = true;
                Btn_Run.BackColor = Color.DarkGray;

                Lab_Status.Text = $"Server啟動";
                ServerFlags = ServerFlag.Start;
            }
        }

        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            if(ServerFlags== ServerFlag.Start)
            {
                UsingApplication.Server.Stop();

                serverControl1.UpdateTimer_Stop();

                Btn_Stop.Enabled = false;
                Btn_Stop.BackColor = Color.ForestGreen;
                Btn_Run.Enabled = true;
                Btn_Run.BackColor = Color.DarkGray;

                Lab_Status.Text = "Server已停止";
                ServerFlags = ServerFlag.Stop;
            }
        }
        private void Btn_Close_Click(object sender, EventArgs e)
        {
            UsingApplication.Server.Stop();
            UsingApplication = null;
            serverControl1.UpdateTimer_Stop();
            this.Close();
        }

    }
}
