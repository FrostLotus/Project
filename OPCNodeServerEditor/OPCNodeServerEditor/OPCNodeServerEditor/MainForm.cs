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
        //public static StandardServer Server;
        //public static ApplicationInstance UsingApplication;
        public emServerFlag ServerFlags = emServerFlag.Stop;
        public static Timer ViewBarTimer = new Timer();
        private delegate void ReflashListView();

        public MainForm()
        {
            InitializeComponent();

        }
        public MainForm(ApplicationInstance application)
        {
            InitializeComponent();
            ViewBarTimer.Interval = 200;
            ViewBarTimer.Tick += UpdateTimer_Tick;

            ///-----------------------------------------
            //設定SERVER
            CParam.UsingApplication = application;

            if (application.Server is StandardServer)
            {
                //CParam._NodeServer = (StandardServer)CParam.UsingApplication.Server;
                CParam._NodeServer = (NodeServer)CParam.UsingApplication.Server;



                EndpointDescriptionCollection EC = CParam._NodeServer.GetEndpoints();
                Console.WriteLine($"第一地址為: {EC[0].EndpointUrl}");//確認Endpoint有SERVER位置
                // 將URL列先清除後新增
                Cbb_EndpointsUrl.Items.Clear();
                //從OPC.UA的XML拉資料
                foreach (EndpointDescription endpoint in CParam._NodeServer.GetEndpoints())
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
                //SessionActivated += UpdateStatus;

                CParam._NodeServer.CurrentInstance.SessionManager.SessionActivated += new SessionEventHandler(UpdateStatus);

            }
            ///-----------------------------------------
            //更新VariableList
            UpdateVariable();


            //先使server不為開啟動作

            CParam.UsingApplication.Server.Stop();
            Btn_Stop.Enabled = false;
            Btn_Stop.BackColor = Color.ForestGreen;
            Lab_Status.Text = "Server以視窗開啟中";
        }

        private void Btn_Run_Click(object sender, EventArgs e)
        {
            if (ServerFlags == emServerFlag.Stop)
            {
                CParam.UsingApplication.Server.Start(CParam.UsingApplication.ApplicationConfiguration);//伺服器啟動
                foreach (OpcDataVariable<object> roll in CParam.VariableList)
                {
                    roll._BaseDataVariableState.OnSimpleWriteValue += new NodeValueSimpleEventHandler(UpdateStatus2);
                    
                    //roll._BaseDataVariableState.OnSimpleWriteValue += (UpdateStatus2);
                }
                //CParam.VariableList[0]._BaseDataVariableState.OnSimpleWriteValue += new NodeValueSimpleEventHandler(UpdateStatus2);

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
                Lab_ServerTimeNow.Text = String.Format("{0:hh:mm:ss.fff}", DateTime.Now);
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
            IList<Session> sessions = CParam._NodeServer.CurrentInstance.SessionManager.GetSessions();//從SessionManager取得連結數

            for (int i = 0; i < sessions.Count; i++)
            {
                Session session = sessions[i];
                //UpdateStatus(session, SessionEventReason.Activated);
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
                    item.SubItems.Add(String.Format("{0:HH:mm:ss.fff}", session.SessionDiagnostics.ClientLastContactTime.ToLocalTime()));//最後連線時間

                    Lsv_Sessions.Items.Add(item);//反應回控制項

                }
            }
            // adjust 控制項外觀寬度
            for (int i = 0; i < Lsv_Sessions.Columns.Count; i++)
            {
                Lsv_Sessions.Columns[i].Width = -2;
            }
        }
        private void UpdateSubscriptions()
        {
            Lsv_Subscriptions.Items.Clear();//清除Listview中項目
            IList<Subscription> subscriptions = CParam._NodeServer.CurrentInstance.SubscriptionManager.GetSubscriptions();//從SessionManager取得訂閱數
            for (int i = 0; i < subscriptions.Count; i++)
            {
                Subscription subscription = subscriptions[i];
                ListViewItem item = new ListViewItem(subscription.Id.ToString());
                lock (subscription.DiagnosticsLock)
                {
                    item.SubItems.Add(String.Format("{0}", (int)subscription.PublishingInterval));//更新時間
                    item.SubItems.Add(String.Format("{0}", subscription.MonitoredItemCount));//總數

                    item.SubItems.Add(String.Format("{0}", subscription.Diagnostics.NextSequenceNumber));//S/N
                }
                Lsv_Subscriptions.Items.Add(item);//反應回控制項
            }

            for (int i = 0; i < Lsv_Subscriptions.Columns.Count; i++)
            {
                Lsv_Subscriptions.Columns[i].Width = -2;
            }

        }
        private void UpdateVariable()
        {
            int selectedIndex = Lsv_VariableList.SelectedIndices.Count > 0 ? Lsv_VariableList.SelectedIndices[0] : -1;
            Lsv_VariableList.Items.Clear();//清除Listview中項目
            for (int i = 0; i < CParam.VariableList.Count; i++)
            {
                ListViewItem item = new ListViewItem($"{CParam.VariableList[i]._OpcDataItem.Index}");
                item.SubItems.Add($"{CParam.VariableList[i]._OpcDataItem.ItemName}");
                item.SubItems.Add($"{CParam.VariableList[i]._OpcDataItem.NodeID}");
                item.SubItems.Add($"{CParam.VariableList[i]._BaseDataVariableState.Value}");
                CParam.StringVariableList[i].Value = CParam.VariableList[i]._BaseDataVariableState.Value;//使CreateAddressSpace重新運作不直接拿原本INI值拿暫存
                Lsv_VariableList.Items.Add(item);//反應回控制項
            }
            ListViewItem tmpTime = new ListViewItem(String.Format("{0:hh:mm:ss.fff}", DateTime.Now));
            Lsv_VariableList.Items.Add(tmpTime);
            if (selectedIndex >= 0 && selectedIndex < Lsv_VariableList.Items.Count)
            {
                Lsv_VariableList.Items[selectedIndex].Selected = true;
                Lsv_VariableList.Items[selectedIndex].EnsureVisible(); // 确保选中项可见
            }
            //Lsv_VariableList.Sort();
        }
        public static void UpdateStatus(Session session, SessionEventReason e)
        {
            lock (session.DiagnosticsLock)//鎖執行緒
            {

                //UpdateVariable();

                Console.WriteLine($"Session觸發:\n" +
                                  $"SessionName =  {session.SessionDiagnostics.SessionName}\n" +
                                  $"Id = {session.Id}" +
                                  $"最後連線時間 = {session.SessionDiagnostics.ClientLastContactTime.ToLocalTime()}");
            }
        }


        public ServiceResult UpdateStatus2(ISystemContext context, NodeState node, ref object value)
        {
            Console.WriteLine($"變數{node.NodeId} = {value.ToString()}");
            //餵回去變更值
            foreach(OpcDataVariable<object> roll in CParam.VariableList)
            {
                if(roll._BaseDataVariableState.NodeId.Identifier == node.NodeId.Identifier)
                {
                    roll._BaseDataVariableState.Value = value;
                    roll._OpcDataItem.Value = value;
                }  
            }
            UpdateListViewChange();
            return ServiceResult.Good;
        }


        public void UpdateListViewChange()
        {
            //判斷物件是否在同一個執行緒上
            if (this.InvokeRequired)
            {
                //當InvokeRequired為true時，表示在不同的執行緒上，所以進行委派的動作!!
                ReflashListView del = new ReflashListView(UpdateListViewChange);
                this.Invoke(del, null);
            }
            else
            {
                UpdateVariable();
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
