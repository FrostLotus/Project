using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpcUaHelper;
using Opc.Ua;
using Opc.Ua.Client;

namespace OPCNodeClientEditor
{
    public partial class MainForm : Form
    {
        public OpcUaClient m_OpcUaClient;
        public MainForm()
        {
            InitializeComponent();
        }
        private async void MainForm_Load(object sender, EventArgs e)
        {
            m_OpcUaClient = new OpcUaClient();
            try
            {
                await m_OpcUaClient.ConnectServer(CParam.ServerURI);
                Lab_ConnectStatus.Text = "連線成功";
                Btn_Connect.BackColor = Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Btn_Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Lab_ConnectStatus.Text = "連線失敗";
            }
        }
        private async void Btn_Connect_Click(object sender, EventArgs e)
        {
            m_OpcUaClient = new OpcUaClient();
            try
            {
                await m_OpcUaClient.ConnectServer(CParam.ServerURI);
                Lab_ConnectStatus.Text = "連線成功";
                Btn_Connect.BackColor = Color.Green;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Btn_Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Lab_ConnectStatus.Text = "連線失敗";
            }
        }
        private void SubCallback(string key, MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, MonitoredItem, MonitoredItemNotificationEventArgs>(SubCallback), key, monitoredItem, args);
                return;
            }

            if (key == "A")
            {
                // 如果有多个的订阅值都关联了当前的方法，可以通过key和monitoredItem来区分
                MonitoredItemNotification notification = args.NotificationValue as MonitoredItemNotification;
                if (notification != null)
                {
                    //textBox3.Text = notification.Value.WrappedValue.Value.ToString();
                }
            }
            else if (key == "B")
            {
                // 需要区分出来每个不同的节点信息
                MonitoredItemNotification notification = args.NotificationValue as MonitoredItemNotification;
                //if (monitoredItem.StartNodeId.ToString() == MonitorNodeTags[0])
                //{
                //    textBox5.Text = notification.Value.WrappedValue.Value.ToString();
                //}
                //else if (monitoredItem.StartNodeId.ToString() == MonitorNodeTags[1])
                //{
                //    textBox9.Text = notification.Value.WrappedValue.Value.ToString();
                //}
                //else if (monitoredItem.StartNodeId.ToString() == MonitorNodeTags[2])
                //{
                //    textBox10.Text = notification.Value.WrappedValue.Value.ToString();
                //}
            }
        }


    }
}
