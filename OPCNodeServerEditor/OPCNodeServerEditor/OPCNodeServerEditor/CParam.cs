using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Opc.Ua;
using Opc.Ua.Server;
using Opc.Ua.Configuration;
using System.Runtime.Serialization;

namespace OPCNodeServerEditor
{
    public enum emVariable
    {
        FolderName = 1,//以(string)NodeID.Identifier作為唯一資料夾名稱追查
        Description = 2,//描述
        //變數用
        Index = 3,//變數序
        ItemName = 4,//對應中文名稱(client不顯示)
        NodeID = 5,//ID名稱(英數)
        DataType = 6,//數值類型:word(uint)
        DataLength = 7,//數值長度(string:?)
        Value = 8//數值
    }

    public enum emServerFlag
    {
        Start = 1,
        Stop = 2,
        pause = 3
    }
    public enum emItemFlag
    {
        None = 0,
        Folder = 1,
        Variable = 2,
        Method = 3
    }
    /// <summary>
    /// 靜態參數
    /// </summary>
    public class CParam
    {

        //參數
        public static NodeServer _NodeServer;
        public static ApplicationInstance UsingApplication;

        public static List<OpcDataItem> StringVariableList = new List<OpcDataItem>();
        public static List<OpcDataFolder> StringFolderList = new List<OpcDataFolder>();

        public static List<FolderState> FolderList = new List<FolderState>();//檔案夾節點列表
        public static List<OpcDataVariable<object>> VariableList = new List<OpcDataVariable<object>>();//變數狀態(object慢一點沒辦法)

        /// <summary>
        /// 建立OCPUA參數包為樹狀結構
        /// </summary>
        public static TreeView VariableTreeView;
        public static string DefaultPath = Application.StartupPath;
        public static string DataFilePath = DefaultPath + "\\OPCServerValue.ini";
        public static Timer ViewTimer = new Timer();
        //--------------------------------------------------------------------------
        public static void Init()
        {
            VariableTreeView = new TreeView();
            LoadData();
            _NodeServer = new NodeServer();

        }
        public static void LoadData()
        {
            //清空 避免重複追加
            StringVariableList.Clear();
            StringFolderList.Clear();

            #region 沒有就創建
            if (!File.Exists(DataFilePath))
            {
                DataFilePath = DefaultPath + "\\OPCServerValue.ini";
                using (StreamWriter createfile = File.CreateText(DataFilePath))
                {
                    MessageBox.Show("無指定資料,已生成");
                    createfile.Close();
                }
            }
            #endregion
            //以下鍵入treeview
            //(第一筆)
            //變數序\n
            //對應中文名稱\n
            //ID名稱\n
            //數值類型:word(uint)\n
            //數值長度(string:?)\n
            //預設值\n
            //[空行]
            var lines = File.ReadAllLines(DataFilePath);
            OpcDataFolder currentFolder = null;
            OpcDataItem currentDataItem = null;
            emItemFlag nowItem = emItemFlag.None;
            try
            {
                foreach (var line in lines)
                {
                    // 如果是空行，表示為分隔符
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        currentDataItem = null;
                        currentFolder = null;
                        nowItem = emItemFlag.None;
                        continue;
                    }
                    // 按照 Key=Value 格式解析每一行
                    // 根据键（Key）來更新屬性
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        if (key == "ItemFlag")
                        {
                            switch (value)
                            {
                                case "Folder":
                                    nowItem = emItemFlag.Folder;
                                    currentFolder = new OpcDataFolder();
                                    currentFolder.itemFlag = emItemFlag.Folder;
                                    break;
                                case "Variable":
                                    nowItem = emItemFlag.Variable;
                                    
                                    currentDataItem = new OpcDataItem();
                                    currentDataItem.itemFlag = emItemFlag.Variable;
                                    break;
                                case "Method"://TBD
                                    nowItem = emItemFlag.Method;
                                    break;
                            }
                            continue;//下面判斷
                        }
                        //若Key為Folder
                        if (nowItem == emItemFlag.Folder)
                        {
                            switch (key)
                            {
                                case "Parent":
                                    currentFolder.Parent = value;
                                    break;
                                case "FolderName":
                                    currentFolder.FolderName = value;
                                    break;
                                case "Description":
                                    currentFolder.Description = value;
                                    //最後加入暫存變數當中
                                    StringFolderList.Add(currentFolder);
                                    break;
                            }
                        }
                        if (nowItem == emItemFlag.Variable)
                        //if (currentDataItem != null)
                        {
                            switch (key)
                            {
                                case "FolderName":
                                    currentDataItem.FolderName = value;
                                    break;
                                case "Description":
                                    currentDataItem.Description = value;
                                    break;
                                case "Index":
                                    currentDataItem.Index = int.Parse(value);
                                    break;
                                case "ItemName":
                                    currentDataItem.ItemName = value;
                                    break;
                                case "NodeId":
                                    currentDataItem.NodeID = value;
                                    break;
                                case "DataType":
                                    currentDataItem.DataType = value;
                                    break;
                                case "DataLength":
                                    currentDataItem.DataLength = int.Parse(value);
                                    break;
                                case "Value"://Initial
                                    if (currentDataItem.DataType == "Real")
                                    {
                                        currentDataItem.Value = float.Parse(value);
                                    }
                                    else if (currentDataItem.DataType == "String")
                                    {
                                        currentDataItem.Value = value;
                                    }
                                    else if (currentDataItem.DataType == "Bool")
                                    {
                                        currentDataItem.Value = (value == "1") ? true : false;
                                    }
                                    else if (currentDataItem.DataType == "Word")
                                    {
                                        currentDataItem.Value = value;
                                    }
                                    //最後加入暫存變數當中
                                    StringVariableList.Add(currentDataItem);
                                    break;
                            }
                        }
                        if (nowItem == emItemFlag.Method)
                        {
                            //TBD-
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "載入資料", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public static void SaveData()
        {
            #region 沒有就創建
            if (!File.Exists(DataFilePath))
            {
                DataFilePath = DefaultPath + "\\OPCServerValue.ini";
                using (StreamWriter createfile = File.CreateText(DataFilePath))
                {
                    MessageBox.Show("無指定資料,已生成");
                    createfile.Close();
                }
            }
            #endregion
            //寫入所有資料
            using (StreamWriter writer = new StreamWriter(DataFilePath))
            {
                writer.Write("");
                string LineData;
                try
                {
                    //先資料夾
                    foreach(var roll in StringFolderList)
                    {
                        LineData = $"ItemFlag={roll.itemFlag}";
                        writer.WriteLine(LineData);//itemFlag
                        //-----------------------------------------------
                        LineData = $"Parent={roll.Parent}";
                        writer.WriteLine(LineData);//Parent
                        //-----------------------------------------------
                        LineData = $"FolderName={roll.FolderName}";
                        writer.WriteLine(LineData);//FolderName
                        //-----------------------------------------------
                        LineData = $"Description={roll.Description}";
                        writer.WriteLine(LineData);//Description
                        //-----------------------------------------------
                        writer.WriteLine("");//寫空一行
                    }
                    //再變數
                    foreach (var roll in StringVariableList)
                    {
                        LineData = $"ItemFlag={roll.itemFlag}";
                        writer.WriteLine(LineData);//itemFlag
                        //-----------------------------------------------
                        LineData = $"FolderName={roll.FolderName}";
                        writer.WriteLine(LineData);//FolderName
                        //-----------------------------------------------
                        LineData = $"Description={roll.Description}";
                        writer.WriteLine(LineData);//Description
                        //-----------------------------------------------
                        LineData = $"Index={roll.Index}";
                        writer.WriteLine(LineData);//Index
                        //-----------------------------------------------
                        LineData = $"ItemName={roll.ItemName}";
                        writer.WriteLine(LineData);//ItemName
                        //-----------------------------------------------
                        LineData = $"NodeId={roll.NodeID}";
                        writer.WriteLine(LineData);//NodeID
                        //-----------------------------------------------
                        LineData = $"DataType={roll.DataType}";
                        writer.WriteLine(LineData);//DataType
                        //-----------------------------------------------
                        LineData = $"DataLength={roll.DataLength}";
                        writer.WriteLine(LineData);//DataLength
                        //-----------------------------------------------
                        LineData = $"Value={roll.Value}";
                        writer.WriteLine(LineData);//Value
                        //-----------------------------------------------
                        writer.WriteLine("");//寫空一行
                    }
                    writer.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SaveData", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
    public class OpcDataItem
    {
        public emItemFlag itemFlag = emItemFlag.None;
        //通用
        public string FolderName { get; set; }//以(string)NodeID.Identifier作為唯一資料夾名稱追查
        public string Description { get; set; }//描述
        //變數用
        public int Index { get; set; }//變數序
        public string ItemName { get; set; }//對應中文名稱(client不顯示)
        public string NodeID { get; set; }//ID名稱
        public string DataType { get; set; }//數值類型:word(uint)
        public int DataLength { get; set; }//數值長度(string:?)
        public object Value { get; set; }//數值
    }
    public class OpcDataFolder
    {
        public emItemFlag itemFlag = emItemFlag.None;
        //資料夾用
        public string Parent { get; set; }
        //通用
        public string FolderName { get; set; }//以(string)NodeID.Identifier作為唯一資料夾名稱追查
        public string Description { get; set; }//描述
    }

    public class OpcDataVariable<T>
    {
        public BaseDataVariableState<T> _BaseDataVariableState;

        public OpcDataItem _OpcDataItem;
    }

    /// <summary>
    /// 存儲數據訪問節點管理器的配置。
    /// </summary>
    [DataContract(Namespace = "http://opcfoundation.org/Quickstarts/ReferenceApplications")]
    public class ReferenceServerConfiguration
    {
        /// <summary>
        /// 預設初始化結構
        /// </summary>
        public ReferenceServerConfiguration()
        {
            Initialize();
        }
        /// <summary>
        /// 反序列化初始化結構
        /// </summary>
        [OnDeserializing()]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }
        /// <summary>
        /// 預設值初始化
        /// </summary>
        private void Initialize()
        {
        }
    }
}
