using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiThreadTest
{
    public partial class Form1 : Form
    {
        AutoResetEvent notifyA = new AutoResetEvent(false);
        AutoResetEvent notifyB = new AutoResetEvent(false);
        AutoResetEvent notifyC = new AutoResetEvent(false);

        public Form1()
        {

            InitializeComponent();


            var t1 = new Thread(CallA);
            var t2 = new Thread(CallB);
            var t3 = new Thread(CallC);

            t1.Start();
            t2.Start();
            t3.Start();

            notifyA.Set();//將事件啟用
            ///-------------------------
            //當做完以後
            t1.Join();
            t2.Join();
            t3.Join();

            listBox1.Items.Add("All threads have completed.");
            //Console.ReadKey();

        }
        public void CallA()
        {
            for (int i = 0; i < 10; i++)
            {
                
                notifyA.WaitOne();//index沒轉為1時等待執行續釋放
                listBox1.Items.Add("A");

                //index = 2;
                notifyB.Set();//控制權丟給B
            }

        }
        public void CallB()
        {
            for (int i = 0; i < 10; i++)
            {
                notifyB.WaitOne();//index沒轉為2時等待執行續釋放
                listBox1.Items.Add("B");
                notifyC.Set();//控制權丟給C
            }

        }
        public void CallC()
        {
            for (int i = 0; i < 10; i++)
            {
               
                notifyC.WaitOne();//index沒轉為3時等待執行續釋放

                listBox1.Items.Add("C");
                listBox1.Items.Add("------------------------------");
                notifyA.Set();//控制權丟給A
            }
        }

    }
   

}
