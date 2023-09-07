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

using Opc.Ua.Client.Controls;
using Opc.Ua.Configuration;


namespace OCPUAServer
{
    public partial class MainForm : Form
    {
        private ApplicationInstance m_application;
        private List<OpcDataItem> opcDataItems = new List<OpcDataItem>();
        private MyOpcServer opcServer;
        public MainForm()
        {
            InitializeComponent();
        }
        public MainForm(ApplicationInstance application)
        {
            InitializeComponent();

            m_application = application;

            if (application.Server is StandardServer)
            {
                //this.ServerDiagnosticsCTRL.Initialize((StandardServer)application.Server, application.ApplicationConfiguration);
            }

            if (!application.ApplicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                //application.ApplicationConfiguration.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
            }

            //TrayIcon.Text = this.Text = application.ApplicationName;
            //this.Icon = TrayIcon.Icon = ClientUtils.GetAppIcon();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeDataItems();

            Dgv_DataItem.DataSource = opcDataItems;
        }
        private void InitializeDataItems()
        {
            //讀取預設目錄INI檔
            #region 沒有就創建
            if (!File.Exists(Param.DataFilePath))
            {
                Param.DataFilePath = Param.DefaultPath + "\\OPCServerValue.ini";
                using (StreamWriter createfile = File.CreateText(Param.DataFilePath))
                {
                    MessageBox.Show("無指定資料,已生成");
                    createfile.Close();
                }
            }
            #endregion
            //(第一筆)
            //變數序\n
            //對應中文名稱\n
            //ID名稱\n
            //數值類型:word(uint)\n
            //數值長度(string:?)\n
            //預設值\n
            //[空行]
            #region 判斷後讀取

            //List<object> DataList = new List<object>();
            //List<object> SPList = new List<object>();

            var lines = File.ReadAllLines(Param.DataFilePath);

            OpcDataItem currentDataItem = new OpcDataItem(0, "Name", "ID", BuiltInType.Null, 1, "test");

            try
            {
                foreach (var line in lines)
                {
                    // 按照 Key=Value 格式解析每一行
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        // 根据键（Key）來更新屬性


                        switch (key)
                        {
                            case "Variable":
                                currentDataItem.VarNumber = int.Parse(value);
                                break;
                            case "ItemName":
                                currentDataItem.ItemName = value;
                                break;
                            case "NodeId":
                                currentDataItem.NodeID = value;
                                break;
                            case "DataType":
                                currentDataItem.DataType = Typechange(value);
                                break;
                            case "DataLength":
                                currentDataItem.DataLength = (value != "") ? int.Parse(value) : 0;
                                break;
                            case "Initial":
                                if (currentDataItem.DataType == BuiltInType.Float)//Real
                                {
                                    currentDataItem.Initial = float.Parse(value);
                                }
                                else if (currentDataItem.DataType == BuiltInType.String)//string
                                {
                                    currentDataItem.Initial = value;
                                }
                                else if (currentDataItem.DataType == BuiltInType.Boolean)//bool
                                {
                                    currentDataItem.Initial = (Math.Abs(Convert.ToInt32(value)) > 0) ? true : false;
                                }
                                else if (currentDataItem.DataType == BuiltInType.Int32)//word
                                {
                                    currentDataItem.Initial = value;
                                }
                                break;
                        }

                    }
                    // 如果是空行，表示為分隔符
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        opcDataItems.Add(currentDataItem);//遇到空白列表示currentDataItem填滿
                        currentDataItem = new OpcDataItem(0, "Name", "ID", BuiltInType.Null, 1, "test");
                        continue;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "載入資料", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }
        public BuiltInType Typechange(string value)
        {
            BuiltInType re = BuiltInType.Null;
            switch (value)
            {
                case "String":
                    re = BuiltInType.String;
                    break;
                case "Word":
                    re = BuiltInType.Double;
                    break;
                case "Bool":
                    re = BuiltInType.Boolean;
                    break;
                case "Real":
                    re = BuiltInType.SByte;
                    break;
            }
            return re;
        }
        //-------------------------------------------------------
        private void Btn_Run_Click(object sender, EventArgs e)
        {

        }
        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            //    // 停止OPC服务器
            opcServer.Stop();
        }
        private void Btn_Add_Click(object sender, EventArgs e)
        {
            OpcDataItem currentDataItem = new OpcDataItem
            {
                VarNumber = Convert.ToInt32(Txt_Variable.Text),
                ItemName = Txt_ItemName.Text,
                NodeID = Txt_NodeId.Text,
                DataType = Typechange(Cbb_DataType.SelectedItem.ToString()),
                DataLength = Convert.ToInt32(Txt_Length.Text),
                //Initial = Txt_Value.Text
            };
            if (currentDataItem.DataType == BuiltInType.Float)//Real
            {
                currentDataItem.Initial = float.Parse(Txt_Inital.Text);
            }
            else if (currentDataItem.DataType == BuiltInType.String)//string
            {
                currentDataItem.Initial = Txt_Inital.Text;
            }
            else if (currentDataItem.DataType == BuiltInType.Boolean)//bool
            {
                currentDataItem.Initial = (Math.Abs(Convert.ToInt32(Txt_Inital.Text)) > 0) ? true : false;
            }
            else if (currentDataItem.DataType == BuiltInType.Int32)//word
            {
                currentDataItem.Initial = Txt_Inital.Text;
            }

            opcDataItems.Add(currentDataItem);

            //Dgv_DataItem.DataSource = opcDataItems;
            //Dgv_DataItem.Invalidate();
        }
        private void Btn_Delete_Click(object sender, EventArgs e)
        {

        }
        private void Btn_UpdateFile_Click(object sender, EventArgs e)
        {

        }
        //-------------------------------
        private void Cbb_DataType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void Dgv_DataItem_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < opcDataItems.Count)
            {
                switch (Dgv_DataItem.Columns[e.ColumnIndex].Name)
                {
                    case "Variable":
                        e.Value = opcDataItems[e.RowIndex].VarNumber;
                        break;
                    case "ItemName":
                        e.Value = opcDataItems[e.RowIndex].ItemName;
                        break;
                    case "NodeId":
                        e.Value = opcDataItems[e.RowIndex].NodeID;
                        break;
                    case "DataType":
                        e.Value = opcDataItems[e.RowIndex].DataType;
                        break;
                    case "DataLength":
                        e.Value = opcDataItems[e.RowIndex].DataLength;
                        break;
                    case "Initial":
                        e.Value = opcDataItems[e.RowIndex].Initial;
                        break;
                }
            }
            Dgv_DataItem.InvalidateRow(e.RowIndex);
        }
    }

    public static class Param
    {
        public static string DefaultPath = Application.StartupPath;
        public static string DataFilePath = DefaultPath + "\\OPCServerValue.ini";
        public static void LoadData()
        {

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
        public BuiltInType DataType { get; set; }//數值類型:word(uint)
        public int DataLength { get; set; }//數值長度(string:?)
        public object Initial { get; set; }//預設值
        public object Value { get; set; }//數值

        public OpcDataItem()
        {

        }
        public OpcDataItem(int varNumber, string itemName, string nodeID, BuiltInType type, int dataLength, object initial)
        {
            VarNumber = varNumber;
            ItemName = itemName;
            NodeID = nodeID;
            DataType = type;
            DataLength = dataLength;
            Initial = initial;
            //Value = value;
        }

        public NodeId changetype()
        {
            NodeId re = new NodeId();
            switch (DataType)
            {
                case BuiltInType.Boolean:
                    re = DataTypeIds.Boolean;
                    break;
                case BuiltInType.String:
                    re = DataTypeIds.String;
                    break;
                case BuiltInType.Double://word
                    re = DataTypeIds.Double;
                    break;
                case BuiltInType.Int32:
                    re = DataTypeIds.Int32;
                    break;
            }
            return re;
        }

        public BaseDataVariableState CreateNode()
        {
            var node = new BaseDataVariableState(null)
            {
                NodeId = new NodeId(NodeID),
                BrowseName = new QualifiedName(ItemName),
                DisplayName = new LocalizedText(ItemName),
                DataType = changetype(),
                ValueRank = ValueRanks.Scalar,
                Value = new Variant(Value),
                //AccessLevel = AccessLevels.CurrentReadOrWrite
            };
            return node;
        }
    }

    public class MyOpcServer : StandardServer
    {

        public MyOpcServer()
        {
            // 初始化OPC数据项
            InitializeDataItems();
        }

        private void InitializeDataItems()
        {
            // 添加你的OPC数据项到opcDataItems列表中
            // 每个数据项包含编号、名称、ID、数据类型、数据长度和初始值
            // 例如：
            //opcDataItems.Add(new OpcDataItem(1, "Item1", "ns=2;s=Item1", BuiltInType.Float, 4, 0.0f));
            //opcDataItems.Add(new OpcDataItem(2, "Item2", "ns=2;s=Item2", BuiltInType.Int32, 4, 0));
            // 添加更多数据项
        }

        protected override void OnServerStarting(ApplicationConfiguration configuration)
        {
            base.OnServerStarting(configuration);

            // 在此处配置服务器设置

            // 启动OPC数据项
            //foreach (var item in opcDataItems)
            //{
            //    AddPredefinedNode(null, item.CreateNode());
            //}
        }

        protected override void Dispose(bool disposing)
        {
            // 清理资源
            base.Dispose(disposing);
        }
    }


}
