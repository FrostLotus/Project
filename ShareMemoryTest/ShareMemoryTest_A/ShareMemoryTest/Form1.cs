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

namespace ShareMemoryTest
{
    public partial class Form1 : Form
    {
        int iTotalSize = 2048;
        int iStart = 1024;
        int iSize = 1024;

        MemoryMappedFile mmFile;
        Mutex mMutex;
        string sShareMemory = "ShareMemoryInAB";
        string sMutexShareMemory = "MutShareMemoryInAB";
        //                1024                               1024                 = 2048
        //[       ...       A       ...      ][      ...       B        ...      ]
        public Form1()
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
        private void btn_Write_Click(object sender, EventArgs e)
        {
            if (mMutex.WaitOne() == true)
            {
                //再寫入資料
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
                //先讀B丟到ShareMemory中的資料(文字資料)
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
    }
}
