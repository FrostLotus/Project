using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;

namespace OPCNodeServerEditor
{
    public partial class MainForm : Form
    {
        public emServerFlag ServerFlags = emServerFlag.Stop;
        public static Timer ViewBarTimer = new Timer();

        public MainForm()
        {
            InitializeComponent();
        }
        public MainForm(ApplicationInstance application)
        {
            InitializeComponent();
            ViewBarTimer.Interval = 1000;
            ViewBarTimer.Tick += UpdateTimer_Tick;

            ///-----------------------------------------
            //設定SERVER
            CParam.UsingApplication = application;
            if(application.Server is StandardServer)
            {
                CParam.Server = (StandardServer)CParam.UsingApplication.Server;
                

                // 將URL列先清除後新增
                Cbb_EndpointsUrl.Items.Clear();
                //從OPC.UA的XML拉資料
                foreach (EndpointDescription endpoint in CParam.Server.GetEndpoints())
                {
                    if (Cbb_EndpointsUrl.FindStringExact(endpoint.EndpointUrl) == -1)
                    {
                        Cbb_EndpointsUrl.Items.Add(endpoint.EndpointUrl);//與現有列表不相符 即新增
                    }
                }
                if (Cbb_EndpointsUrl.Items.Count > 0)
                {
                    Cbb_EndpointsUrl.SelectedIndex = 0;//預設第一位
                }

            }
            

            ///-----------------------------------------
            //先使server不為開啟動作
            Btn_Stop.Enabled = false;
            Btn_Stop.BackColor = Color.ForestGreen;
            Lab_Status.Text = "Server以視窗開啟中";
        }

        private void Btn_Run_Click(object sender, EventArgs e)
        {
            if (ServerFlags == emServerFlag.Stop)
            {
                CParam.UsingApplication.Server.Start(CParam.UsingApplication.ApplicationConfiguration);//伺服器啟動

                UpdateTimer_Start();
                Btn_Run.Enabled = false;
                Btn_Run.BackColor = Color.ForestGreen;
                Btn_Stop.Enabled = true;
                Btn_Stop.BackColor = Color.DarkGray;

                Lab_Status.Text = $"Server啟動";
                ServerFlags = emServerFlag.Start;
            }
        }
        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            if (ServerFlags == emServerFlag.Start)
            {
                CParam.UsingApplication.Server.Stop();

                UpdateTimer_Stop();
                Btn_Stop.Enabled = false;
                Btn_Stop.BackColor = Color.ForestGreen;
                Btn_Run.Enabled = true;
                Btn_Run.BackColor = Color.DarkGray;

                Lab_Status.Text = $"Server停止";
                ServerFlags = emServerFlag.Stop;
            }
        }
        public void UpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Lab_ServerTimeNow.Text = String.Format("{0:HH:mm:ss}", DateTime.Now);
                UpdateSessions();
                Lab_sessionsCount.Text = Convert.ToString(Lsv_Sessions.Items.Count);
                UpdateSubscriptions();
                Lab_subscriptionsCount.Text = Convert.ToString(Lsv_Subscriptions.Items.Count);
                int itemTotal = 0;
                for (int i = 0; i < Lsv_Subscriptions.Items.Count; i++)
                {
                    itemTotal += Convert.ToInt32(Lsv_Subscriptions.Items[i].SubItems[2].Text);
                }
                Lab_itemsCount.Text = Convert.ToString(itemTotal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "UpdateTimer_Tick", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        private void UpdateSessions()
        {
            Lsv_Sessions.Items.Clear();//清除Listview中項目
            IList<Session> sessions = CParam.Server.CurrentInstance.SessionManager.GetSessions();//從SessionManager取得連結數

            for (int i = 0; i < sessions.Count; i++)
            {
                Session session = sessions[i];

                lock (session.DiagnosticsLock)//鎖執行緒
                {
                    ListViewItem item = new ListViewItem(session.SessionDiagnostics.SessionName);
                    //若偵測項不為空
                    if (session.Identity != null)
                    {
                        item.SubItems.Add(session.Identity.DisplayName);//新增顯示連線名稱
                    }
                    else
                    {
                        item.SubItems.Add(String.Empty);//加空
                    }

                    item.SubItems.Add(String.Format("{0}", session.Id));//ID
                    item.SubItems.Add(String.Format("{0:HH:mm:ss}", session.SessionDiagnostics.ClientLastContactTime.ToLocalTime()));//最後連線時間

                    Lsv_Sessions.Items.Add(item);//反應回控制項
                }
            }
            // adjust 
            for (int i = 0; i < Lsv_Sessions.Columns.Count; i++)
            {
                Lsv_Sessions.Columns[i].Width = -2;
            }
        }
        private void UpdateSubscriptions()
        {
            Lsv_Subscriptions.Items.Clear();//清除Listview中項目
            IList<Subscription> subscriptions = CParam.Server.CurrentInstance.SubscriptionManager.GetSubscriptions();//從SessionManager取得訂閱數

            for (int i = 0; i < subscriptions.Count; i++)
            {
                Subscription subscription = subscriptions[i];
                ListViewItem item = new ListViewItem(subscription.Id.ToString());

                item.SubItems.Add(String.Format("{0}", (int)subscription.PublishingInterval));//更新時間
                item.SubItems.Add(String.Format("{0}", subscription.MonitoredItemCount));//總數

                lock (subscription.DiagnosticsLock)
                {
                    item.SubItems.Add(String.Format("{0}", subscription.Diagnostics.NextSequenceNumber));//S/N
                }

                Lsv_Subscriptions.Items.Add(item);//反應回控制項
            }

            for (int i = 0; i < Lsv_Subscriptions.Columns.Count; i++)
            {
                Lsv_Subscriptions.Columns[i].Width = -2;
            }
        }
        public void UpdateTimer_Stop()
        {
            ViewBarTimer.Stop();
        }
        public void UpdateTimer_Start()
        {
            ViewBarTimer.Start();
        }

        
    }
}
