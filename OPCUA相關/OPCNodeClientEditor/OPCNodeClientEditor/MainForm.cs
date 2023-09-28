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
        private MonitoredItemNotificationEventHandler m_monitoredItem_Notification;
        private ApplicationConfiguration m_configuration;


        private delegate void ReflashListView();
        public emServerFlag ServerFlags = emServerFlag.Stop;

        //=================================================================================
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
        }
        private void Lsv_VariableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //時間最後行不觸發
            if (Lsv_VariableList.SelectedIndices.Count > 0 && Lsv_VariableList.SelectedIndices[0] < Lsv_VariableList.Items.Count - 1)
            {
                //將值變更為選取項目
                int selectedIndex = Lsv_VariableList.SelectedIndices[0];
                Console.WriteLine($"選取第{selectedIndex}行");
                Txt_Index.Text = CParam.VariableList[selectedIndex]._OpcDataItem.Index.ToString();
                Txt_ItemName.Text = CParam.VariableList[selectedIndex]._OpcDataItem.ItemName.ToString();
                Txt_NodeId.Text = CParam.VariableList[selectedIndex]._OpcDataItem.NodeID.ToString();
                Cbb_DataType.SelectedItem = CParam.VariableList[selectedIndex]._OpcDataItem.DataType.ToString();
                Txt_Value.Text = CParam.VariableList[selectedIndex]._BaseDataVariableState.Value.ToString();//暫存_VALUE
            }
            else
            {
                // 如果没有选中项，则 selectedIndex 为 -1
                Console.WriteLine("沒有選中項");
                //不做等第二次
            }
        }
        private async void Btn_Connect_Click(object sender, EventArgs e)
        {
            CParam.m_OpcUaClient = new OpcUaClient();
            if (ServerFlags == emServerFlag.Stop)
            {
                try
                {
                    CParam.ServerURL = Txt_ServerURL.Text;
                    await CParam.m_OpcUaClient.ConnectServer(CParam.ServerURL);

                    //顯示
                    Lab_ConnectStatus.Text = "連線成功";
                    Btn_Connect.BackColor = Color.Green;
                    
                    //將variable讀回Variablelist中
                    CParam.NodeItemPullOut(CParam.StartNodeTag);
                    CParam.AddVariableToSubscription(MonitoredItem_Callback);
                    //將variable加入監聽清單
                    //VariableAddListen();

                    //從list印至listview中
                    UpdateListViewInvoke();
                    ServerFlags = emServerFlag.Start;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Btn_Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Lab_ConnectStatus.Text = "連線失敗";
                }
            }
            else if (ServerFlags == emServerFlag.Start)
            {
                CParam.m_OpcUaClient.Disconnect();
                ServerFlags = emServerFlag.Stop;
                Btn_Connect.BackColor = Color.Coral;
            }
        }
        private void Btn_UpdateValue_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = -1;
                //單純修改值
                if (Lsv_VariableList.SelectedIndices.Count <= 0)
                {
                    MessageBox.Show("在列表中無選擇 沒有修改目標");
                }
                else
                {
                    selectedIndex = Lsv_VariableList.SelectedIndices[0];
                }
                //-------------------------------------------------------
                if (selectedIndex != -1)//有選取  確認有無刪除項目
                {
                    for (int i = 0; i < CParam.VariableList.Count; i++)
                    {
                        //找相同index
                        if (Convert.ToInt32(Txt_Index.Text) == CParam.VariableList[i]._OpcDataItem.Index)
                        {
                            //依格式餵入
                            switch (CParam.VariableList[i]._OpcDataItem.DataType)
                            {
                                case "String":
                                    CParam.m_OpcUaClient.WriteNode(CParam.VariableList[i]._OpcDataItem.VaribleTag, Txt_Value.Text);
                                    Console.WriteLine(String.Format("{0:hh:mm:ss.fff}", DateTime.Now));
                                    CParam.VariableList[i]._BaseDataVariableState.Value = Txt_Value.Text;
                                    break;
                                case "Real":
                                    CParam.m_OpcUaClient.WriteNode(CParam.VariableList[i]._OpcDataItem.VaribleTag, float.Parse(Txt_Value.Text));
                                    Console.WriteLine(String.Format("{0:hh:mm:ss.fff}", DateTime.Now));
                                    CParam.VariableList[i]._BaseDataVariableState.Value = float.Parse(Txt_Value.Text);
                                    break;
                                case "Bool":
                                    CParam.m_OpcUaClient.WriteNode(CParam.VariableList[i]._OpcDataItem.VaribleTag, (Txt_Value.Text == "0") ? false : true);
                                    Console.WriteLine(String.Format("{0:hh:mm:ss.fff}", DateTime.Now));
                                    CParam.VariableList[i]._BaseDataVariableState.Value = (Txt_Value.Text == "0") ? false : true;
                                    break;
                                case "Word":
                                    if (int.TryParse(Txt_Value.Text, out int tmp))
                                    {
                                        CParam.m_OpcUaClient.WriteNode(CParam.VariableList[i]._OpcDataItem.VaribleTag, tmp);
                                        Console.WriteLine(String.Format("{0:hh:mm:ss.fff}", DateTime.Now));
                                        CParam.VariableList[i]._BaseDataVariableState.Value = tmp;
                                    }
                                    break;
                            }
                        }
                    }
                    //更新ListView
                    UpdateListViewInvoke();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message} \n可能輸入值不匹配type", "Btn_UpdateValue", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Btn_ReflashList_Click(object sender, EventArgs e)
        {
            //將variable讀回list中
            CParam.NodeItemPullOut(CParam.StartNodeTag);
            //從list印至listview中
            UpdateListViewInvoke();
        }
        //=====================================================================
        public void VariableAddListen()
        {
            string[] MonitorNodeTags = new string[CParam.VariableList.Count];

            
        }
       
        public void UpdateListViewInvoke()
        {
            //判斷物件是否在同一個執行緒上
            if (this.InvokeRequired)
            {
                //當InvokeRequired為true時，表示在不同的執行緒上，所以進行委派的動作!!
                ReflashListView del = new ReflashListView(UpdateListViewInvoke);
                this.Invoke(del, null);//委託自己
            }
            else
            {
                UpdateVariableList();
            }
        }
        private void UpdateVariableList()
        {
            Lsv_VariableList.Items.Clear();//清除Listview中項目
            for (int i = 0; i < CParam.VariableList.Count; i++)
            {
                ListViewItem item = new ListViewItem($"{CParam.VariableList[i]._OpcDataItem.Index}");
                item.SubItems.Add($"{CParam.VariableList[i]._OpcDataItem.ItemName}");
                item.SubItems.Add($"{CParam.VariableList[i]._OpcDataItem.NodeID}");
                item.SubItems.Add($"{CParam.VariableList[i]._BaseDataVariableState.Value}");//以node內value為準
                Lsv_VariableList.Items.Add(item);//反應回控制項
            }
            ListViewItem tmpTime = new ListViewItem(String.Format("{0:hh:mm:ss.fff}", DateTime.Now));
            Lsv_VariableList.Items.Add(tmpTime);
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

                //目前不管
                //if (monitoredItem.StartNodeId.ToString() == MonitorNodeTags[0])
                //{
                //    //textBox5.Text = notification.Value.WrappedValue.Value.ToString();
                //}
                //else if (monitoredItem.StartNodeId.ToString() == MonitorNodeTags[1])
                //{
                //    //textBox9.Text = notification.Value.WrappedValue.Value.ToString();
                //}
                //else if (monitoredItem.StartNodeId.ToString() == MonitorNodeTags[2])
                //{
                //    //textBox10.Text = notification.Value.WrappedValue.Value.ToString();
                //}
            }
        }

        private void MonitoredItem_Callback(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs args)
        {
            if (ServerFlags == emServerFlag.Start)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<MonitoredItem, MonitoredItemNotificationEventArgs>(MonitoredItem_Callback), monitoredItem, args);
                    return;
                }
                MonitoredItemNotification notification = args.NotificationValue as MonitoredItemNotification;
                if (notification != null)
                {
                    foreach (var roll in CParam.VariableList)
                    {
                        if (roll._BaseDataVariableState.DisplayName.Text == monitoredItem.DisplayName)
                        {

                            //roll._BaseDataVariableState.Value = monitoredItem.LastValue;
                            //roll._OpcDataItem.Value = monitoredItem.LastValue;
                            roll._BaseDataVariableState.Value = CParam.m_OpcUaClient.ReadNode(roll._OpcDataItem.VaribleTag);
                            roll._OpcDataItem.Value = CParam.m_OpcUaClient.ReadNode(roll._OpcDataItem.VaribleTag);



                        }
                    }
                    UpdateVariableList();
                }
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CParam.RemoveSubscription();
            CParam.m_OpcUaClient.Disconnect();
        }
    }

}