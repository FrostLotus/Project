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
            try
            {
                // 載入應用程序配置
                application.LoadApplicationConfiguration(false).Wait();
                // 檢查應用程式證書
                bool certOk = application.CheckApplicationInstanceCertificate(false, 0).Result;
                if (!certOk)
                {
                    throw new Exception("Application instance certificate invalid!");
                }
                // start the server.
                application.Start(CParam._NodeServer).Wait();
                // 運行主視窗
                Application.Run(new MainForm(application));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "program", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
