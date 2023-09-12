using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Opc.Ua;

namespace OPCNodeServerEditor
{
    public enum ServerFlag
    {
        Start = 1,
        Stop = 2,
        pause = 3
    }
    public enum folderFlag
    {

    }
    /// <summary>
    /// 靜態參數
    /// </summary>
    public static class CParam
    {
        //參數
        /// <summary>
        /// 建立OCPUA參數包為樹狀結構
        /// </summary>
        public static TreeView VariableTreeView;
        public static string DefaultPath = Application.StartupPath;
        public static string DataFilePath = DefaultPath + "\\OPCNodeValue.ini";


        //--------------------------------------------------------------------------
        public static void Init()
        {
            VariableTreeView = new TreeView();
        }
        public static void LoadData()
        {
            #region 沒有就創建
            if (!File.Exists(DataFilePath))
            {
                DataFilePath = DefaultPath + "\\OPCNodeValue.ini";
                using (StreamWriter createfile = File.CreateText(DataFilePath))
                {
                    MessageBox.Show("無指定資料,已生成");
                    createfile.Close();
                }
            }
            #endregion
            //以下鍵入treeview
        }

    }
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
        public OpcDataItem(int varNumber, string itemName, string nodeID, BuiltInType dataType, int dataLength, object initial)
        {
            VarNumber = varNumber;
            ItemName = itemName;
            NodeID = nodeID;
            DataType = DataType;
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
                DataType = NodeID,
                ValueRank = ValueRanks.Scalar,
                Value = new Variant(Value),
                AccessLevel = AccessLevels.CurrentReadOrWrite
                //AccessLevel = AccessLevels.CurrentReadOrWrite
            };
            return node;
        }
    }
}
