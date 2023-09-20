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

namespace OPCNodeClientEditor
{
    public partial class MainForm : Form
    {
        public emServerFlag ServerFlags = emServerFlag.Stop;
        private OpcUaClient m_OpcUaClient = new OpcUaClient();
        private string ServerURL;
        public static int IterationIndex = 1;
        public string StartNodeTag = "ns=2;s=A";

        public List<OpcDataItem> VariableList = new List<OpcDataItem>();

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
                Txt_Index.Text = VariableList[selectedIndex].Index.ToString();
                Txt_ItemName.Text = VariableList[selectedIndex].ItemName.ToString();
                Txt_NodeId.Text = VariableList[selectedIndex].NodeID.ToString();
                Cbb_DataType.SelectedItem = VariableList[selectedIndex].DataType.ToString();
                Txt_Value.Text = VariableList[selectedIndex].Value.ToString();//暫存_VALUE
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
            m_OpcUaClient = new OpcUaClient();
            if (ServerFlags == emServerFlag.Stop)
            {
                try
                {
                    ServerURL = Txt_ServerURL.Text;
                    await m_OpcUaClient.ConnectServer(ServerURL);
                    //顯示
                    Lab_ConnectStatus.Text = "連線成功";
                    Btn_Connect.BackColor = Color.Green;
                    ServerFlags = emServerFlag.Start;
                    //將variable讀回list中
                    ItemPullOut(StartNodeTag);
                    //從list印至listview中
                    UpdateVariableList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Btn_Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Lab_ConnectStatus.Text = "連線失敗";
                }
            }
            if (ServerFlags == emServerFlag.Start)
            {
                m_OpcUaClient.Disconnect();
                ServerFlags = emServerFlag.Stop;
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
                    for (int i = 0; i < VariableList.Count; i++)
                    {
                        //找相同index
                        if (Convert.ToInt32(Txt_Index.Text) == VariableList[i].Index)
                        {
                            //依格式餵入
                            switch (VariableList[i].DataType)
                            {
                                case "String":
                                    m_OpcUaClient.WriteNode(VariableList[i].VaribleTag, Txt_Value.Text);
                                    Console.WriteLine(String.Format("{0:hh:mm:ss.fff}", DateTime.Now));
                                    VariableList[i].Value = Txt_Value.Text;
                                    break;
                                case "Real":
                                    m_OpcUaClient.WriteNode(VariableList[i].VaribleTag, float.Parse(Txt_Value.Text));
                                    Console.WriteLine(String.Format("{0:hh:mm:ss.fff}", DateTime.Now));
                                    VariableList[i].Value = float.Parse(Txt_Value.Text);
                                    break;
                                case "Bool":
                                    m_OpcUaClient.WriteNode(VariableList[i].VaribleTag, (Txt_Value.Text == "0") ? false : true);
                                    Console.WriteLine(String.Format("{0:hh:mm:ss.fff}", DateTime.Now));
                                    VariableList[i].Value = (Txt_Value.Text == "0") ? false : true;
                                    
                                    break;
                                case "Word":
                                    if (int.TryParse(Txt_Value.Text, out int tmp))
                                    {
                                        m_OpcUaClient.WriteNode(VariableList[i].VaribleTag, tmp);
                                        Console.WriteLine(String.Format("{0:hh:mm:ss.fff}", DateTime.Now));
                                        VariableList[i].Value = tmp;
                                    }
                                    break;
                            }
                        }
                    }
                    //更新ListView
                    UpdateVariableList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message} \n可能輸入值不匹配type", "Btn_UpdateValue", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //=====================================================================
        public void ItemPullOut(string startNodetag)
        {
            VariableList.Clear();
            IterationIndex = 1;
            try
            {
                ReferenceDescription[] references = m_OpcUaClient.BrowseNodeReference(startNodetag);//目前變數皆在A資料夾當中 修改待續
                if (references != null)
                {
                    foreach (var item in references)
                    {
                        switch (item.NodeClass.ToString())
                        {
                            case "Object"://目前定調為資料夾
                                FolderPullOut(item.NodeId);
                                break;
                            case "Variable"://變數
                                VariableToList(item);
                                break;
                            case "Method"://方法
                                //TBD
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ItemPullOut", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ////建立讀value之
            //List<NodeId> nodeIds = new List<NodeId>();
            //foreach (var roll in VariableList)
            //{
            //    nodeIds.Add(new NodeId(roll.VaribleTag));
            //}
            //// dataValues按顺序定义的值，每个值里面需要重新判断类型
            //List<DataValue> dataValues = m_OpcUaClient.ReadNodes(nodeIds.ToArray());
            //// 然后遍历你的数据信息
            //foreach (var dataValue in dataValues)
            //{
            //    // 获取你的实际的数据
            //    object value = dataValue.WrappedValue.Value;
            //}
        }
        public void FolderPullOut(ExpandedNodeId nodeid)
        {
            try
            {
                ReferenceDescription[] references = m_OpcUaClient.BrowseNodeReference(nodeid.ToString());
                if (references != null)
                {
                    foreach (var item in references)
                    {
                        switch (item.NodeClass.ToString())
                        {
                            case "Object"://目前定調為資料夾
                                FolderPullOut(item.NodeId);
                                break;
                            case "Variable"://變數
                                VariableToList(item);
                                break;
                            case "Method"://方法
                                //TBD
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "FolderPullOut", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void VariableToList(ReferenceDescription rdescription)
        {
            OpcDataItem tmpVariable = new OpcDataItem
            {
                VaribleTag = rdescription.NodeId.ToString(),
                Index = IterationIndex,
                ItemName = rdescription.DisplayName.Text,//先暫定
                NodeID = rdescription.BrowseName.Name
            };
            IterationIndex++;//使找出的節點照編號順序加入
            OpcNodeAttribute[] nodeAttributes = m_OpcUaClient.ReadNoteAttributes(tmpVariable.VaribleTag);
            //單一variable節點定義屬性
            foreach (var item in nodeAttributes)
            {

                if (item.Name == "Value")//取值及type
                {
                    switch (item.Type)
                    {
                        case "String":
                            tmpVariable.DataType = "String";
                            tmpVariable.Value = item.Value;
                            break;
                        case "Boolean":
                            tmpVariable.DataType = "Bool";
                            tmpVariable.Value = item.Value;
                            break;
                        case "Float":
                            tmpVariable.DataType = "Real";
                            tmpVariable.Value = item.Value;
                            break;
                        case "Int32":
                            tmpVariable.DataType = "Word";
                            if (int.TryParse(item.Value.ToString(), out int tmp))
                                tmpVariable.Value = tmp;
                            break;

                    }
                }
            }
            VariableList.Add(tmpVariable);//剩下DataType value要抓
        }
        private void UpdateVariableList()
        {
            //int selectedIndex = Lsv_VariableList.SelectedIndices.Count > 0 ? Lsv_VariableList.SelectedIndices[0] : -1;
            Lsv_VariableList.Items.Clear();//清除Listview中項目
            for (int i = 0; i < VariableList.Count; i++)
            {
                ListViewItem item = new ListViewItem($"{VariableList[i].Index}");
                item.SubItems.Add($"{VariableList[i].ItemName}");
                item.SubItems.Add($"{VariableList[i].NodeID}");
                item.SubItems.Add($"{VariableList[i].Value}");
                Lsv_VariableList.Items.Add(item);//反應回控制項
            }
            ListViewItem tmpTime = new ListViewItem(String.Format("{0:hh:mm:ss.fff}", DateTime.Now));
            Lsv_VariableList.Items.Add(tmpTime);
        }
        private void Btn_ReflashList_Click(object sender, EventArgs e)
        {
            //將variable讀回list中
            ItemPullOut(StartNodeTag);
            //從list印至listview中
            UpdateVariableList();
        }
    }
    public class OpcDataItem
    {

        //通用
        public string VaribleTag { get; set; }//以(string)NodeID作為唯一節點名稱追查(Tag) 僅此即可更改值
        //變數用
        public int Index { get; set; }//變數序
        public string ItemName { get; set; }//對應中文名稱(client不顯示)
        public string NodeID { get; set; }//ID名稱
        public string DataType { get; set; }//數值類型:word(uint)
        public object Value { get; set; }//數值
    }
    public enum emServerFlag
    {
        Start = 1,
        Stop = 2,
        pause = 3
    }
}
