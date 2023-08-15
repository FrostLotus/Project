﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareMemoryTest_B
{
    public partial class Form1 : Form
    {
        int iTotalSize = 2048;
        int iStart = 0;
        int iSize = 0;//因為當作未知長度
        MemoryMappedFile mmFile;
        Mutex mMutex;
        string sShareMemory = "ShareMemoryInAB";
        string sMutexShareMemory = "MutShareMemoryInAB";
        public Form1()
        {
            InitializeComponent();
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
            }

        }
        private void btn_Write_Click(object sender, EventArgs e)
        {

            if (mMutex.WaitOne() == true)
            {
                //再丟入資料(文字資料)進ShareMemory 供A使用
                iStart = 1024;
                iSize = 1024;
                using (MemoryMappedViewStream mmvsStream = mmFile.CreateViewStream(iStart, iSize))
                {
                    using (BinaryWriter brWriter = new BinaryWriter(mmvsStream))
                    {
                        byte[] bMessageWrite = Encoding.UTF8.GetBytes(textBox1.Text);
                        brWriter.Write(bMessageWrite.Length);//先寫長度
                        brWriter.Write(bMessageWrite);
                    }
                    
                }
                mMutex.ReleaseMutex();//放掉控制權
            }
        }
        private void btn_Read_Click(object sender, EventArgs e)
        {
            if (mMutex.WaitOne() == true)
            {
                //先讀A丟到ShareMemory中的資料(文字資料)
                iStart = 0;
                iSize = 0;
                using (MemoryMappedViewStream mmvsStream = mmFile.CreateViewStream(iStart, iSize))//A丟的
                {
                    using (BinaryReader brReader = new BinaryReader(mmvsStream))
                    {
                        int ilength = brReader.ReadInt32();
                        string sMessageRead = Encoding.UTF8.GetString(brReader.ReadBytes(ilength), 0, ilength);
                        label1.Text = $"訊息＝{sMessageRead}";
                    }
                }
                mMutex.ReleaseMutex();//放掉控制權
            }
        }
    }
}
