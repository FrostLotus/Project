using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace ShareMemory_A
{
    public partial class Form_A : Form
    {
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);


        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        private static int WM_COPYDATA = 0x004A;

        public string FormClient = "FormB";

        int iTotalSize = 2048;
        int iStart = 1024;
        int iSize = 1024;

        MemoryMappedFile mmFile;
        Mutex mMutex;
        string sShareMemory = "ShareMemoryInAB";
        string sMutexShareMemory = "MutShareMemoryInAB";
        //                1024                               1024                 = 2048
        //[       ...       A       ...      ][      ...       B        ...      ]
        public Form_A()
        {
            InitializeComponent();
            //ShareMemory
            try
            {
                //試著Create一個ShareMemory
                mmFile = MemoryMappedFile.CreateNew(sShareMemory, iTotalSize);
            }
            catch
            {
                //若存在 讀取現存對應ShareMemory
                mmFile = MemoryMappedFile.OpenExisting(sShareMemory);
                Console.WriteLine("已有ShareMemoryInAB");
            }
            //Mutex
            try
            {
                //試著Create一個Mutex
                mMutex = new Mutex(true, sMutexShareMemory, out bool mutexCreated);
                mMutex.ReleaseMutex();
            }
            catch
            {
                //若存在 讀取現存對應Mutex
                mMutex = Mutex.OpenExisting(sMutexShareMemory);
                Console.WriteLine("MutShareMemoryInAB");
            }
            //-------------------------------------------------------------------------------
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));

                if (cds.cbData > 0 && cds.lpData != null)
                {
                    label2.Text = cds.lpData;
                    Console.WriteLine(cds.lpData);
                }

            }
            base.WndProc(ref m);
        }

        private void btn_Write_Click(object sender, EventArgs e)
        {
            if (mMutex.WaitOne() == true)
            {
                //寫入資料
                iStart = 0;
                iSize = 0;//寫入先不給大小
                using (MemoryMappedViewStream mmvsStream = mmFile.CreateViewStream(iStart, iSize))
                {
                    using (BinaryWriter bwWriter = new BinaryWriter(mmvsStream))
                    {
                        byte[] bMessageWrite = Encoding.UTF8.GetBytes(textBox1.Text);
                        bwWriter.Write(bMessageWrite.Length); //先寫Length
                        bwWriter.Write(bMessageWrite); //再寫byte[]
                    }
                }
                mMutex.ReleaseMutex();//釋放對mMutex的控制權
            }
        }
        private void btn_Read_Click(object sender, EventArgs e)
        {
            if (mMutex.WaitOne() == true)
            {
                //讀B丟到ShareMemory中的資料(文字資料)
                iStart = 1024;
                iSize = 1204;
                using (MemoryMappedViewStream mmvsStream = mmFile.CreateViewStream(iStart, iSize))
                {
                    using (BinaryReader brReader = new BinaryReader(mmvsStream))
                    {
                        int ilength = brReader.ReadInt32();
                        string sMessageRead = Encoding.UTF8.GetString(brReader.ReadBytes(ilength), 0, ilength);
                        label1.Text = $"訊息＝{sMessageRead}";
                        //Console.WriteLine($"回應＝{sMessageRead}");
                    }
                }
                mMutex.ReleaseMutex();//釋放對mMutex的控制權
            }
        }
        private void btn_Send_Click(object sender, EventArgs e)
        {
            try
            {
                int hwndReceiver = FindWindow(null, FormClient);//找TestB的IntPtr 用來代表指標或控制代碼
                if (hwndReceiver != 0)
                {
                    COPYDATASTRUCT cds = new COPYDATASTRUCT();
                    cds.dwData = (IntPtr)0;
                    cds.cbData = Encoding.Unicode.GetBytes(textBox2.Text).Length;//有中文字長度上要注意
                    cds.lpData = textBox2.Text;
                    SendMessage(hwndReceiver, WM_COPYDATA, 0, ref cds);
                }
                else
                {
                    MessageBox.Show("指定Form: " + FormClient + " 未發現");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        
    }
}
