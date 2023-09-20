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
        private delegate void ReflashListView();
        public MainForm(ApplicationInstance application)
        {
            InitializeComponent();
            ViewBarTimer.Interval = 5000;
            ViewBarTimer.Tick += UpdateTimer_Tick;
            ///-----------------------------------------
            //設定SERVER
            CParam.UsingApplication = application;
            if (application.Server is StandardServer)
            {
                CParam._NodeServer = (NodeServer)CParam.UsingApplication.Server;
                EndpointDescriptionCollection EC = CParam._NodeServer.GetEndpoints();
                Console.WriteLine($"第一地址為: {EC[0].EndpointUrl}");//確認Endpoint有SERVER位置
                Cbb_EndpointsUrl.Items.Clear();// 將URL列先清除後新增
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
            }
            ///-----------------------------------------
            //更新VariableList
            UpdateVariableList();
            //先使server不為開啟動作
            CParam.UsingApplication.Server.Stop();
            Btn_Stop.Enabled = false;
            Btn_Stop.BackColor = Color.ForestGreen;
            Lab_Status.Text = "Server以視窗開啟中";
            //Form狀態
            Txt_Value.Enabled = false;
            Btn_Delete.Enabled = false;
            Btn_UpdateFile.Enabled = false;
            Btn_UpdateValue.Enabled = false;
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
                Txt_Length.Text = CParam.VariableList[selectedIndex]._OpcDataItem.DataLength.ToString();
                Txt_Initial.Text = CParam.VariableList[selectedIndex]._OpcDataItem.Value.ToString();//INI_VALUE
                Txt_Value.Text = CParam.VariableList[selectedIndex]._BaseDataVariableState.Value.ToString();//暫存_VALUE

                //Form狀態
                if (ServerFlags == emServerFlag.Stop)
                {
                    Btn_Delete.Enabled = true;
                    Btn_UpdateFile.Enabled = true;
                    Btn_UpdateValue.Enabled = false;

                }
                if (ServerFlags == emServerFlag.Start)
                {
                    Btn_Delete.Enabled = false;
                    Btn_UpdateFile.Enabled = false;
                    Btn_UpdateValue.Enabled = true;
                }
            }
            else
            {
                // 如果没有选中项，则 selectedIndex 为 -1
                Console.WriteLine("沒有選中項");
                //不做等第二次
            }
        }
        private void Btn_Add_Click(object sender, EventArgs e)
        {
            if (HaveAddCondition())//若是符合建立標準
            {
                try
                {
                    OpcDataItem tmpitem = new OpcDataItem
                    {
                        itemFlag = emItemFlag.Variable,
                        FolderName = "A",//先預設
                        Description = "",
                        Index = Convert.ToInt32(Txt_Index.Text),
                        ItemName = Txt_ItemName.Text,
                        NodeID = Txt_NodeId.Text,
                        DataType = Cbb_DataType.SelectedItem.ToString(),
                        DataLength = Convert.ToInt32(Txt_Length.Text)
                    };
                    switch (tmpitem.DataType)
                    {
                        case "Real":
                            tmpitem.Value = float.Parse(Txt_Initial.Text);
                            break;
                        case "String":
                            tmpitem.Value = Txt_Initial.Text;
                            break;
                        case "Bool":
                            tmpitem.Value = (Txt_Initial.Text == "0") ? "0" : "1";
                            break;
                        case "Word":
                            if (int.TryParse(Txt_Initial.Text, out int tmp))
                                tmpitem.Value = tmp;
                            break;
                    }
                    //加入初始化StringVariableList
                    CParam.StringVariableList.Add(tmpitem);
                    //寫入INI
                    CParam.SaveData();
                    //加入節點(以開關方式被動寫入)
                    CParam.UsingApplication.Server.Start(CParam.UsingApplication.ApplicationConfiguration);
                    CParam.UsingApplication.Server.Stop();
                    //更新ListView
                    UpdateVariableList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Btn_Add_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            int selectedIndex = Lsv_VariableList.SelectedIndices[0];
            if (selectedIndex != -1)//有選取  確認有無刪除項目
            {
                for (int i = 0; i < CParam.StringVariableList.Count; i++)
                {
                    if (Convert.ToInt32(Txt_Index.Text) == CParam.StringVariableList[i].Index)
                    {
                        CParam.StringVariableList.RemoveAt(i);//刪除特定位置
                    }
                }
                //重寫INI
                CParam.SaveData();
                //加入節點(以開關方式被動寫入)
                CParam.UsingApplication.Server.Start(CParam.UsingApplication.ApplicationConfiguration);
                CParam.UsingApplication.Server.Stop();
                //更新ListView
                UpdateVariableList();
            }
        }
        private void Btn_UpdateFile_Click(object sender, EventArgs e)
        {
            int selectedIndex = Lsv_VariableList.SelectedIndices[0];
            if (selectedIndex != -1)//有選取  確認需更改
            {
                for (int i = 0; i < CParam.StringVariableList.Count; i++)
                {
                    if (Convert.ToInt32(Txt_Index.Text) == CParam.StringVariableList[i].Index)
                    {
                        //僅只能修改length initial部分
                        CParam.StringVariableList[i].DataLength = Convert.ToInt32(Txt_Length.Text);
                        //依格式餵入
                        switch (CParam.StringVariableList[i].DataType)
                        {
                            case "String":
                                CParam.StringVariableList[i].Value = Txt_Initial.Text;
                                break;
                            case "Real":
                                CParam.StringVariableList[i].Value = float.Parse(Txt_Initial.Text);
                                break;
                            case "Bool":
                                CParam.StringVariableList[i].Value = (Txt_Initial.Text == "0") ? false : true;
                                break;
                            case "Word":
                                if (int.TryParse(Txt_Initial.Text, out int tmp))
                                    CParam.StringVariableList[i].Value = tmp;
                                break;


                        }
                        //CParam.StringVariableList[i].Value = Convert.ToInt32(Txt_Initial.Text);
                    }
                }
                //重寫INI
                CParam.SaveData();
                //加入節點(以開關方式被動寫入)
                CParam.UsingApplication.Server.Start(CParam.UsingApplication.ApplicationConfiguration);
                CParam.UsingApplication.Server.Stop();
                //更新ListView
                UpdateVariableList();
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
                                    CParam.VariableList[i]._BaseDataVariableState.Value = Txt_Value.Text;
                                    break;
                                case "Real":
                                    CParam.VariableList[i]._BaseDataVariableState.Value = float.Parse(Txt_Value.Text);
                                    break;
                                case "Bool":
                                    CParam.VariableList[i]._BaseDataVariableState.Value = (Txt_Value.Text=="0")?false:true;
                                    break;
                                case "Word":
                                    if (int.TryParse(Txt_Value.Text, out int tmp))
                                    {
                                        CParam.VariableList[i]._BaseDataVariableState.Value = tmp;
                                    }
                                    break;
                            }
                            CParam.VariableList[i]._BaseDataVariableState.StatusCode = StatusCodes.Good;
                            CParam.VariableList[i]._BaseDataVariableState.Timestamp = DateTime.Now;
                            
                        }
                    }
                    //更新ListView
                    UpdateVariableList();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message} \n可能輸入值不匹配type", "Btn_UpdateValue", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        private void Btn_Run_Click(object sender, EventArgs e)
        {
            if (ServerFlags == emServerFlag.Stop)
            {
                CParam.UsingApplication.Server.Start(CParam.UsingApplication.ApplicationConfiguration);//伺服器啟動
                //全監控
                foreach (OpcDataVariable<object> roll in CParam.VariableList)
                {
                    roll._BaseDataVariableState.OnSimpleWriteValue += new NodeValueSimpleEventHandler(UpdateStatus);
                }
                UpdateTimer_Start();
                //Form方面
                Btn_Run.Enabled = false;
                Btn_Run.BackColor = Color.ForestGreen;
                Btn_Stop.Enabled = true;
                Btn_Stop.BackColor = Color.DarkGray;

                Btn_Add.Enabled = false;
                Btn_Delete.Enabled = false;
                Btn_UpdateFile.Enabled = false;
                Btn_UpdateValue.Enabled = true;

                Txt_Index.Enabled = false;
                Txt_ItemName.Enabled = false;
                Txt_NodeId.Enabled = false;
                Cbb_DataType.Enabled = false;
                Txt_Length.Enabled = false;
                Txt_Initial.Enabled = false;
                Txt_Value.Enabled = true;

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
                //Form方面
                Btn_Stop.Enabled = false;
                Btn_Stop.BackColor = Color.ForestGreen;
                Btn_Run.Enabled = true;
                Btn_Run.BackColor = Color.DarkGray;

                Btn_Add.Enabled = true;
                Btn_Delete.Enabled = true;
                Btn_UpdateFile.Enabled = true;
                Btn_UpdateValue.Enabled = false;

                Txt_Index.Enabled = true;
                Txt_ItemName.Enabled = true;
                Txt_NodeId.Enabled = true;
                Cbb_DataType.Enabled = true;
                Txt_Length.Enabled = true;
                Txt_Initial.Enabled = true;
                Txt_Value.Enabled = false;

                Lab_Status.Text = $"Server停止";
                ServerFlags = emServerFlag.Stop;
            }
        }
        private void Btn_Close_Click(object sender, EventArgs e)
        {
            if (ServerFlags == emServerFlag.Stop)
            {
                Close();
            }
            if (ServerFlags == emServerFlag.Start)
            {
                CParam.UsingApplication.Server.Stop();

                UpdateTimer_Stop();
                Close();
            }
        }
        //=========================================================================
        private bool HaveAddCondition()
        {
            foreach (OpcDataVariable<object> roll in CParam.VariableList)
            {
                ///INDEX
                //------------------------------------------------------------
                if (!IsNumeric(Txt_Index.Text) || Txt_Index.Text == "")
                {
                    MessageBox.Show("Index存在數字以外字元或為空");
                    return false;
                }
                if (roll._OpcDataItem.Index == Convert.ToInt32(Txt_Index.Text))
                {
                    MessageBox.Show("Index已存在");
                    return false;
                }
                ///ITEMNAME
                //------------------------------------------------------------
                if (Txt_ItemName.Text == "")
                {
                    MessageBox.Show("ItemName字元為空");
                    return false;
                }
                if (roll._OpcDataItem.ItemName == Txt_ItemName.Text)
                {
                    MessageBox.Show("ItemName已存在");
                    return false;
                }
                ///NODEID
                //------------------------------------------------------------
                if (!IsAlphanumeric(Txt_NodeId.Text) || Txt_NodeId.Text == "")
                {
                    MessageBox.Show("NodeID存在英數以外字元或為空");
                    return false;
                }
                if (roll._OpcDataItem.NodeID == Txt_NodeId.Text)
                {
                    MessageBox.Show("NodeID已存在");
                    return false;
                }
                ///DATALENGTH
                //------------------------------------------------------------
                if (!IsNumeric(Txt_Length.Text) || Txt_Length.Text == "")
                {
                    MessageBox.Show("DataLength存在數字以外字元或為空");
                    return false;
                }
                ///INITIAL
                //------------------------------------------------------------
                //if (!IsAlphanumeric(Txt_Initial.Text))
                //{
                //    MessageBox.Show("initial存在英數以外字元");
                //    return false;
                //}
            }
            return true;
        }
        public void UpdateTimer_Stop()
        {
            ViewBarTimer.Stop();
        }
        public void UpdateTimer_Start()
        {
            ViewBarTimer.Start();
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
        private void UpdateVariableList()
        {
            //int selectedIndex = Lsv_VariableList.SelectedIndices.Count > 0 ? Lsv_VariableList.SelectedIndices[0] : -1;
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
            //if (selectedIndex >= 0 && selectedIndex < Lsv_VariableList.Items.Count)
            //{
            //    Lsv_VariableList.Items[selectedIndex].Selected = true;
            //    Lsv_VariableList.Items[selectedIndex].EnsureVisible(); // 确保选中项可见
            //}
            //Lsv_VariableList.Sort();
        }
        public void UpdateSession(Session session, SessionEventReason e)
        {
            //僅session啟動時讀的了
            lock (session.DiagnosticsLock)//鎖執行緒
            {
                Console.WriteLine($"Session觸發:\n" +
                                  $"SessionName =  {session.SessionDiagnostics.SessionName}\n" +
                                  $"Id = {session.Id}" +
                                  $"最後連線時間 = {session.SessionDiagnostics.ClientLastContactTime.ToLocalTime()}");
            }
        }
        public ServiceResult UpdateStatus(ISystemContext context, NodeState node, ref object value)
        {
            Console.WriteLine($"變數{node.NodeId} = {value.ToString()}");
            //餵回去變更值
            foreach (OpcDataVariable<object> roll in CParam.VariableList)
            {
                if (roll._BaseDataVariableState.NodeId.Identifier == node.NodeId.Identifier)
                {
                    roll._BaseDataVariableState.Value = value;
                    //roll._OpcDataItem.Value = value;
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
                this.Invoke(del, null);//委託自己
            }
            else
            {
                UpdateVariableList();
            }
        }
        private static bool IsNumeric(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
        private static bool IsAlphanumeric(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        
    }


}
