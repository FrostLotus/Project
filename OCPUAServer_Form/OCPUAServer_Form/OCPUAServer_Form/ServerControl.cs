using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Opc.Ua.Server;
using Opc.Ua;

namespace OCPUAServer
{
    public partial class ServerControl : UserControl
    {
        private StandardServer m_server;
        private ApplicationConfiguration m_configuration;
        public ServerControl()
        {
            InitializeComponent();

        }
        /// <summary>
        /// 初始化元件
        /// </summary>
        /// <param name="server">伺服器</param>
        /// <param name="configuration">設定項</param>
        public void Initialize(StandardServer server, ApplicationConfiguration configuration)
        {
            m_server = server;
            m_configuration = configuration;
            UpdateTimer.Enabled = true;

            // add the urls to the drop down.
            Cbb_EndpointsUrl.Items.Clear();

            foreach (EndpointDescription endpoint in m_server.GetEndpoints())
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

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Lab_ServerState.Text = m_server.CurrentInstance.CurrentState.ToString();
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "UpdateTimer_Tick", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        /// <summary>
        /// 更新連線
        /// </summary>
        private void UpdateSessions()
        {
            Lsv_Sessions.Items.Clear();//清除Listview中項目
            IList<Session> sessions = m_server.CurrentInstance.SessionManager.GetSessions();//從SessionManager取得連結數

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
            IList<Subscription> subscriptions = m_server.CurrentInstance.SubscriptionManager.GetSubscriptions();//從SessionManager取得訂閱數
            
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
            UpdateTimer.Stop();
        }
        public void UpdateTimer_Start()
        {
            UpdateTimer.Start();
        }

    }
}
