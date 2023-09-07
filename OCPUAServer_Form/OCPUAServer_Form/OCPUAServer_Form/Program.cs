using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Client.Controls;

namespace OCPUAServer_Form
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // 載入應用程序配置
                
                // run the application interactively.
                Application.Run(new MainForm());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "program", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //ExceptionDlg.Show(application.ApplicationName, e);
            }
        }
    }
}
