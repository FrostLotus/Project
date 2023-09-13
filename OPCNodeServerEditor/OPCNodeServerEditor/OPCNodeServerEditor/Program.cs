using Opc.Ua;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPCNodeServerEditor
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

            ApplicationInstance application = new ApplicationInstance();
            application.ApplicationType = ApplicationType.Server;
            application.ConfigSectionName = "OPCUASetting";

            //先載入相關參數 CParam
            CParam.Init();
            //SharpNodeSettingsServer settingsServer = new SharpNodeSettingsServer();


            Application.Run(new MainForm(application));
            //Application.Run(new MainForm());
        }
    }
}
