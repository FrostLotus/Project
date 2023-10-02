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
using System.Diagnostics;

namespace OPCNodeClientEditor
{
    public partial class MainForm : Form
    {
        private delegate void ReflashListView();
        public emServerFlag ServerFlags = emServerFlag.Stop;
        public Timer ViewBarTimer = new Timer();

        public Stopwatch swStopwatch = new Stopwatch();
        //=================================================================================
        /// <summary>[constructor]初始化</summary>
        public MainForm()
        {
            InitializeComponent();
            //----------------------------------
            //ViewBarTimer.Interval = 1000;
            //ViewBarTimer.Tick += UpdateTimer;
            //ViewBarTimer.Start();

            CParam.m_OpcUaClient.ReconnectPeriod = Convert.ToInt32(textBox1.Text);//重連線時間覆寫
            

            CParam.m_OpcUaClient.OpcStatusChange += M_OpcUaClient_OpcStatusChange;
            CParam.m_OpcUaClient.ConnectComplete += M_OpcUaClient_ConnectComplete;

        }
        //------------------------------------------
        /// <summary>[ListView]目錄選取</summary>
        private void Lsv_VariableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //若為時間最後行則不觸發
            if ((Lsv_VariableList.SelectedIndices.Count > 0) && (Lsv_VariableList.SelectedIndices[0] < Lsv_VariableList.Items.Count - 1))
            {
                //將值變更為選取項目
                int selectedIndex = Lsv_VariableList.SelectedIndices[0];
                Console.WriteLine($"選取第{selectedIndex}行");
                //下列修改項目
                Txt_Index.Text =             CParam.VariableList[selectedIndex]._OpcDataItem.Index.ToString();
                Txt_ItemName.Text =          CParam.VariableList[selectedIndex]._OpcDataItem.ItemName.ToString();
                Txt_NodeId.Text =            CParam.VariableList[selectedIndex]._OpcDataItem.NodeID.ToString();
                Cbb_DataType.SelectedItem =  CParam.VariableList[selectedIndex]._OpcDataItem.DataType.ToString();
                Txt_Value.Text =             CParam.VariableList[selectedIndex]._BaseDataVariableState.Value.ToString();//暫存_VALUE
            }
            else
            {
                // 如果没有选中项，则 selectedIndex 为 -1
                Console.WriteLine("沒有選中項");
                //不做等第二次
            }
        }
        /// <summary>[Button]連線</summary>
        private async void Btn_Connect_Click(object sender, EventArgs e)
        {
            if (ServerFlags == emServerFlag.Stop)//若為未連線
            {
                try
                {
                    CParam.ServerURL = Txt_ServerURL.Text;
                    await CParam.m_OpcUaClient.ConnectServer(CParam.ServerURL);
                    //若連線成功成功則跳入M_OpcUaClient_ConnectComplete
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Btn_Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Lab_ConnectStatus.Text = "連線失敗";
                }
            }
            else if (ServerFlags == emServerFlag.Start)
            {
                //關閉連線
                CParam.RemoveSubscription();
                CParam.m_OpcUaClient.Disconnect();
                ServerFlags = emServerFlag.Stop;
                Btn_Connect.Text = "重新連線";
                Btn_Connect.BackColor = Color.Coral;
            }
        }
        /// <summary>[Button]更新數值</summary>
        private void Btn_UpdateValue_Click(object sender, EventArgs e)
        {
            try
            {
                //單純修改值
                if (Lsv_VariableList.SelectedIndices.Count <= 0)
                {
                    MessageBox.Show("在列表中無選擇 沒有修改目標", "UpdateValue",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
                else
                {
                    CParam.UpdateValue(Txt_Index.Text, Txt_Value.Text);//更新數值
                    UpdateListViewInvoke();//更新ListView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message} \n可能輸入值不匹配type", "Btn_UpdateValue", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>[Form]關閉視窗</summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(ServerFlags == emServerFlag.Start)
            {
                CParam.RemoveSubscription();
                CParam.m_OpcUaClient.Disconnect();
            }
        }
        //=================================================================================
        /// <summary>更新列表Invoke</summary>
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
        /// <summary>更新列表</summary>
        private void UpdateVariableList()
        {
            swStopwatch.Restart();
            Lsv_VariableList.Items.Clear();//清除Listview中項目
            for (int i = 0; i < CParam.VariableList.Count; i++)
            {
                ListViewItem item = new ListViewItem($"{CParam.VariableList[i]._OpcDataItem.Index}");//目錄
                item.SubItems.Add($"{CParam.VariableList[i]._OpcDataItem.ItemName}");//名稱
                item.SubItems.Add($"{CParam.VariableList[i]._OpcDataItem.NodeID}");//顯示ID
                item.SubItems.Add($"{CParam.VariableList[i]._BaseDataVariableState.Value}");//以node內value為準
                Lsv_VariableList.Items.Add(item);//反應回控制項
            }
            ListViewItem tmpTime = new ListViewItem(String.Format("{0:hh:mm:ss.ff}", DateTime.Now));//顯示最後更新時間
            Lsv_VariableList.Items.Add(tmpTime);
            //時間測試------
            swStopwatch.Stop();
            TimeSpan trim = swStopwatch.Elapsed;

            Console.WriteLine("迴圈1次時間: " + trim + "\n目前時間: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.ff tt"));
        }
        private void MonitoredItem_Callback(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            // 判斷物件是否在同一個執行緒上
            if (InvokeRequired)
            {
                BeginInvoke(new Action<MonitoredItem, MonitoredItemNotificationEventArgs>(MonitoredItem_Callback), monitoredItem, e);
                return;
            }
            //若Client端初始化建立顯示完成  才做觸發的動作
            if (ServerFlags == emServerFlag.Start)
            {
                //監控項目內部通知項目
                MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
                if (notification != null)//若無
                {
                    foreach (var roll in CParam.VariableList)
                    {
                        //將對應修改的值對應名稱 讀取新值寫入列表
                        if (roll._BaseDataVariableState.DisplayName.Text == monitoredItem.DisplayName)
                        {
                            roll._BaseDataVariableState.Value = notification.Value.WrappedValue;
                            roll._OpcDataItem.Value = notification.Value.WrappedValue;
                            //roll._BaseDataVariableState.Value = CParam.m_OpcUaClient.ReadNode(roll._OpcDataItem.VaribleTag);
                            //roll._OpcDataItem.Value = CParam.m_OpcUaClient.ReadNode(roll._OpcDataItem.VaribleTag);
                        }
                    }
                    //刷新變數列表
                    UpdateListViewInvoke();
                }
            }
        }

        private void M_OpcUaClient_OpcStatusChange(object sender, OpcUaStatusEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(  new Action( () => { M_OpcUaClient_OpcStatusChange(sender, e); } )  );
                return;
            }
            //若異常
            if (e.Error)
            {
                Lab_ConnectNow.BackColor = Color.Red;
            }
            else
            {
                //正常狀態
                Lab_ConnectNow.BackColor = SystemColors.Control;
            }

            Lab_ConnectNow.Text = e.Text;//狀態輸出
        }
        private void M_OpcUaClient_ConnectComplete(object sender, EventArgs e)
        {
            try
            {
                OpcUaClient client = (OpcUaClient)sender;
                if (client.Connected)
                {
                    //顯示
                    Lab_ConnectStatus.Text = "連線成功";
                    Btn_Connect.Text = "連線中";
                    Btn_Connect.BackColor = Color.Green;

                    //將variable讀進Variablelist中 且建立參考節點
                    CParam.NodeItemPullOut(CParam.StartNodeTag);
                    //將variable加入監聽清單
                    CParam.AddVariableToSubscription(MonitoredItem_Callback);
                    //從list印至listview中
                    UpdateListViewInvoke();
                    ServerFlags = emServerFlag.Start;
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(Text, exception);
            }
        }
        private void UpdateTimer(object sender, EventArgs e)
        {
            try
            {
                Lab_ServerTimeNow.Text = String.Format("{0:hh:mm:ss.ff}", DateTime.Now);
                if (CParam.m_OpcUaClient.Connected && ServerFlags == emServerFlag.Start)
                {
                    Lab_ConnectNow.Text = "連線中";
                }
                else if(ServerFlags == emServerFlag.Stop)
                {
                    Lab_ConnectNow.Text = "未連線";
                }
                else if (!CParam.m_OpcUaClient.Connected && ServerFlags == emServerFlag.Start)
                {
                    Lab_ConnectNow.Text = "連線中斷";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "有連線問題", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }

}