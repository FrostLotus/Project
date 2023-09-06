using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Server;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace OCPUAServer_Form
{
    public partial class MainForm : Form
    {

        private MyOpcServer opcServer;

        public MainForm()
        {
            InitializeComponent();

        }
        public void ReadTXTFile()
        {

        }
        public void SaveTXTFile()
        {

        }


        
        //-------------------------------------------------------
        private void Btn_Run_Click(object sender, EventArgs e)
        {
            // 启动OPC服务器
            opcServer = new MyOpcServer();
            //opcServer.Start()
        }

        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            // 停止OPC服务器
            opcServer.Stop();
        }
        //----------
        private void Btn_Add_Click(object sender, EventArgs e)
        {

        }
        private void Btn_Delete_Click(object sender, EventArgs e)
        {

        }
        private void Btn_UpdateFile_Click(object sender, EventArgs e)
        {

        }
        private void Btn_UpdateMember_Click(object sender, EventArgs e)
        {

        }

    }

    public static class Param
    {
        public static string DefaultPath = Application.StartupPath + @"\Datatext";
        public static string DataFilePath = DefaultPath + "\\OPCServer_Value.ini";
        public static void LoadData()
        {
            #region 沒有就創建
            if (!File.Exists(DataFilePath))
            {
                DataFilePath = DefaultPath + "\\OPCServer_Value.ini";
                using (StreamWriter createfile = File.CreateText(DataFilePath))
                {
                    MessageBox.Show("無指定資料,已生成");
                    createfile.Close();
                }
            }
            #endregion

        }
        public static void Break_String(string sData, string sBreakItem, ref List<object> list)
        {
            list.Clear();
            string[] Spare = sData.Split(new string[] { sBreakItem }, StringSplitOptions.None);
            foreach (string item in Spare)//餵回去list
            {
                list.Add(item);
            }
        }
    }
    //==================================================

    public class OpcDataItem
    {
        public int VarNumber { get; set; }//變數序
        public string ItemName { get; set; }//對應中文名稱(client不顯示)
        public string NodeID { get; set; }//ID名稱
        public BuiltInType Type { get; set; }//數值類型:word(uint)
        public int DataLength { get; set; }//數值長度(string:?)
        public object Initial { get; set; }//預設值
        public object Value { get; set; }//數值

        public OpcDataItem(int varNumber, string itemName, string nodeID, BuiltInType type, int dataLength, object initial)
        {
            VarNumber = varNumber;
            ItemName = itemName;
            NodeID = nodeID;
            Type = type;
            DataLength = dataLength;
            Initial = initial;
            //Value = value;
        }

        public BaseDataVariableState CreateNode()
        {
            var node = new BaseDataVariableState(null)
            {
                NodeId = new NodeId(NodeID),
                BrowseName = new QualifiedName(ItemName),
                DisplayName = new LocalizedText(ItemName),
                DataType = NodeID,//???
                ValueRank = ValueRanks.Scalar,
                Value = new Variant(Value),
                AccessLevel = AccessLevels.CurrentReadOrWrite
            };
            return node;
        }
    }

    public class MyOpcServer : StandardServer
    {

        private List<OpcDataItem> opcDataItems = new List<OpcDataItem>();
        public MyOpcServer()
        {
            InitializeDataItems();
        }
        /// <summary>
        /// 資料添加
        /// </summary>
        private void InitializeDataItems()
        {
            //(第一筆)
            //變數序\n
            //對應中文名稱\n
            //ID名稱\n
            //數值類型:word(uint)\n
            //數值長度(string:?)\n
            //預設值\n
            //[空行]

            // 添加你的OPC数据项到opcDataItems列表中
            // 每个数据项包含编号、名称、ID、数据类型、数据长度和初始值
            // 例如：
            //                              Var/  Name(中)/ NodeID(實際)/      類型/        長度/ 數值/
            //opcDataItems.Add(new OpcDataItem(1, "Item1", "ns=2;s=Item1", BuiltInType.Float, 4, 0.0f));
            //opcDataItems.Add(new OpcDataItem(2, "Item2", "ns=2;s=Item2", BuiltInType.Int32, 4, 0));
            Param.LoadData();


            #region 判斷後讀取
            
            List<object> DataList = new List<object>();
            List<object> SPList = new List<object>();
                 
            var lines = File.ReadAllLines(Param.DataFilePath);
            OpcDataItem currentDataItem = null;

            try
            {
                foreach (var line in lines)
                {
                    // 如果是空行，表示為分隔符
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        currentDataItem = null;
                        continue;
                    }
                    // 按照 Key=Value 格式解析每一行
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        // 根据键（Key）來更新屬性
                        if (currentDataItem != null)
                        {
                            switch (key)
                            {
                                case "VarNumber":
                                    currentDataItem.VarNumber = int.Parse(value);
                                    break;
                                case "ItemName":
                                    currentDataItem.ItemName = value;
                                    break;
                                case "NodeID":
                                    currentDataItem.NodeID = value;
                                    break;
                                case "Type":
                                    currentDataItem.Type = (BuiltInType)Enum.Parse(typeof(BuiltInType), value);
                                    break;
                                case "DataLength":
                                    currentDataItem.DataLength = int.Parse(value);
                                    break;
                                case "Initial":
                                    if (currentDataItem.Type == BuiltInType.Float)
                                    {
                                        currentDataItem.Value = float.Parse(value);
                                    }
                                    else if (currentDataItem.Type == BuiltInType.String)
                                    {
                                        currentDataItem.Value = value;
                                    }
                                    else if (currentDataItem.Type == BuiltInType.Boolean)
                                    {
                                        currentDataItem.Value=(value="1") = ;
                                    }
                                    break;
                            }
                        }
                    }

                }
             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "載入資料", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
            // 添加更多数据项
        }
        protected override void OnServerStarting(ApplicationConfiguration configuration)
        {
            base.OnServerStarting(configuration);

            // 在此处配置服务器设置

            // 启动OPC数据项
            foreach (var item in opcDataItems)
            {
                //AddPredefinedNode(null, item.CreateNode());
            }
        }
        protected override void Dispose(bool disposing)
        {
            // 清理资源
            base.Dispose(disposing);
        }
    }
}
