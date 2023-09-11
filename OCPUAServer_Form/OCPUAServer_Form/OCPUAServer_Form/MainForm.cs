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
    public partial class MainForm : Form
    {
        private ApplicationInstance m_application;
        public MainForm()
        {
            InitializeComponent();
        }
        public MainForm(ApplicationInstance application)
        {
            InitializeComponent();

            m_application = application;

            if (application.Server is StandardServer)
            {
                this.serverDiagnosticsCtrl1.Initialize((StandardServer)application.Server, application.ApplicationConfiguration);
            }

            if (!application.ApplicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                //application.ApplicationConfiguration.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
            }

            //TrayIcon.Text = this.Text = application.ApplicationName;
            //this.Icon = TrayIcon.Icon = ClientUtils.GetAppIcon();
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

        }

        private void Btn_Stop_Click(object sender, EventArgs e)
        {

        }

        private void Btn_Close_Click(object sender, EventArgs e)
        {

        }

    }
}
