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
        

        public static List<FolderState> FolderList = new List<FolderState>();//檔案夾節點列表

        public static List<OpcDataItem> StringVariableList = new List<OpcDataItem>();
        public static List<OpcDataFolder> StringFolderList =new List<OpcDataFolder>();

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
            

        }
        public static void LoadData()
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

                        if(key == "ItemFlag")
                        {
                            switch (value)
                            {
                                case "Folder":
                                    nowItem = emItemFlag.Folder;
                                    currentFolder = new OpcDataFolder();
                                    break;
                                case "Variable":
                                    nowItem = emItemFlag.Variable;
                                    currentDataItem = new OpcDataItem();
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
                                case "Value":
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
                                        currentDataItem.Value = (value == "1")? true:false;
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
    }
    public class OpcDataNodeManager: CustomNodeManager2
    {
        private ReferenceServerConfiguration Configuration;//基本為空
        private List<BaseDataVariableState<int>> TimeTickList = new List<BaseDataVariableState<int>>();//變數狀態
        private System.Timers.Timer NodeTimer = null;

        private IList<IReference> references = null;//節點參考列表
        /// <summary>
        /// 初始化節點管理器
        /// </summary>
        public OpcDataNodeManager(IServerInternal server, ApplicationConfiguration configuration) 
            : base(server, configuration, "http://opcfoundation.org/Quickstarts/ReferenceApplications")
        {
            //CustomNodeManager2內部參數
            SystemContext.NodeIdFactory = this;
            // 取得節點管理器配置(基本無物)
            Configuration = configuration.ParseExtension<ReferenceServerConfiguration>();
            if (Configuration == null)
            {
                Configuration = new ReferenceServerConfiguration();
            }
           

            NodeTimer = new System.Timers.Timer(500);
            NodeTimer.Elapsed += NodeTimer_Elapsed;
            NodeTimer.Start();

        }
        //override區域------------------------------------------------------------------------
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                //加載預定義節點
                LoadPredefinedNodes(SystemContext, externalReferences);
                //節點參考列表
                IList<IReference> references = null;
                //嘗試取得至Objects節點檔案夾參考資料(之後作為位入資料夾主體位置)
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    //沒有的話 置入IReference結構給它
                    references = new List<IReference>();
                    externalReferences[ObjectIds.ObjectsFolder] = references;
                }
                #region 資料夾建置
                FolderState rootButtom = new FolderState(null);
                FolderState rootMy = new FolderState(null);
                //以下為新增 目錄節點 DATA已入建置矩陣中
                //先建立資料夾
                foreach (OpcDataFolder roll in CParam.StringFolderList)
                {
                    if (roll.Parent == "null")
                    {
                        rootButtom = CreateFolder(null, roll.FolderName, roll.Description);
                        CParam.FolderList.Add(rootButtom);

                        references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, rootButtom));//建立目錄(中間為false)
                        AddPredefinedNode(SystemContext, rootButtom);//根目錄 處理皆可
                    }
                    else
                    {
                        //搜尋roll.Parent根資料夾
                        FolderState rootNode = FindFolder(roll.Parent);
                        rootMy = CreateFolder(rootNode, roll.FolderName, roll.Description);
                        CParam.FolderList.Add(rootMy);
                    }
                }
                #endregion
                foreach(OpcDataItem roll in CParam.StringVariableList)
                {
                    FolderState rootNode = FindFolder(roll.FolderName);
                    //單一值變數
                    switch (roll.DataType)
                    {
                        case "String":
                            CreateVariable(rootNode, roll.NodeID, DataTypeIds.String, ValueRanks.Scalar, roll.Description, roll.Value);
                            AddPredefinedNode(SystemContext, rootNode);
                            break;
                        case "Bool":
                            CreateVariable(rootNode, roll.NodeID, DataTypeIds.Boolean, ValueRanks.Scalar, roll.Description, roll.Value);
                            AddPredefinedNode(SystemContext, rootNode);
                            break;
                        case "Real":
                            CreateVariable(rootNode, roll.NodeID, DataTypeIds.Float, ValueRanks.Scalar, roll.Description, (float)roll.Value);
                            AddPredefinedNode(SystemContext, rootNode);
                            break;
                        case "Word":
                            CreateVariable(rootNode, roll.NodeID, DataTypeIds.String, ValueRanks.Scalar, roll.Description, roll.Value);
                            AddPredefinedNode(SystemContext, rootNode);
                            break;
                    }
                }
            }
        }
        protected override NodeHandle GetManagerHandle(ServerSystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        {
            lock (Lock)
            {
                // quickly exclude nodes that are not in the namespace. 
                if (!IsNodeIdInNamespace(nodeId))
                {
                    return null;
                }

                NodeState node = null;

                if (!PredefinedNodes.TryGetValue(nodeId, out node))
                {
                    return null;
                }

                NodeHandle handle = new NodeHandle();

                handle.NodeId = nodeId;
                handle.Node = node;
                handle.Validated = true;

                return handle;
            }
        }
        //---------------------------------------------------------------------------------------
        #region 檔案夾
        /// <summary>
        /// [簡化描述]新增下位節點檔案夾 
        /// </summary>
        /// <param name="parent">父節點狀態</param>
        /// <param name="name">自身節點名稱</param>
        /// <returns></returns>
        protected FolderState CreateFolder(NodeState parent, string name)
        {
            return CreateFolder(parent, name, string.Empty);
        }
        /// <summary>
        /// 新增下位節點檔案夾
        /// </summary>
        /// <param name="parent">父節點狀態</param>
        /// <param name="name">自身節點名稱</param>
        /// <param name="description">附帶描述</param>
        /// <returns></returns>
        protected FolderState CreateFolder(NodeState parent, string name, string description)
        {
            FolderState folder = new FolderState(parent)//BaseObjectState
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypes.Organizes,//節點形式 組織
                TypeDefinitionId = ObjectTypeIds.FolderType,//類型 資料夾
                Description = description, //描述
                BrowseName = new QualifiedName(name), //, NamespaceIndex );
                DisplayName = new LocalizedText(name),
                WriteMask = AttributeWriteMask.None,
                UserWriteMask = AttributeWriteMask.None,
                EventNotifier = EventNotifiers.None,
                NodeId = (parent == null)//有無父節點
                ? new NodeId(name, NamespaceIndex)//以自身為節點
                : new NodeId(parent.NodeId.ToString() + "/" + name)//以父+自身為節點
            };
            if (parent != null)
            {
                parent.AddChild(folder);
            }
            return folder;
        }
        protected void AddFolder(NodeState parent, string name, string description)
        {
            FolderState Folder = CreateFolder(parent, name, description);
            Folder.AddReference(ReferenceTypes.Organizes, false, ObjectIds.ObjectsFolder);//提供目錄參考(狀態 排列等) 中間本為true 
            references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, Folder.NodeId));//建立目錄
            Folder.EventNotifier = EventNotifiers.SubscribeToEvents;//觸發事件
            AddRootNotifier(Folder);
        }
        protected FolderState FindFolder(string Parent)
        {
            FolderState re = new FolderState(null);
            foreach (FolderState roll in CParam.FolderList)
            {
                if ((string)roll.NodeId.Identifier == Parent)
                {
                    re = roll;
                }
            }
            return re;
        }
        #endregion

        #region 新增變數
        /// <summary>
        /// 產生一個變數節點<T>
        /// </summary>
        /// <typeparam name="T">類型</typeparam>
        /// <param name="parent">父類</param>
        /// <param name="name">名稱</param>
        /// <param name="dataType">變數類型</param>
        /// <param name="valueRank">變數格式</param>
        /// <param name="description">變數描述</param>
        /// <param name="defaultValue">變數值</param>
        /// <returns></returns>
        private BaseDataVariableState<T> CreateVariable<T>(NodeState parent, string name, NodeId dataType,int valueRank, string description, T defaultValue)
        {
            BaseDataVariableState<T> variable = new BaseDataVariableState<T>(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypes.Organizes,
                TypeDefinitionId = VariableTypeIds.BaseDataVariableType,

                BrowseName = new QualifiedName(name),//, NamespaceIndex),
                DisplayName = new LocalizedText(name),
                Description = description,

                WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,
                UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,

                DataType = dataType,
                ValueRank = valueRank,
                AccessLevel = AccessLevels.CurrentReadOrWrite,
                UserAccessLevel = AccessLevels.CurrentReadOrWrite,
                Historizing = false,
                Value = defaultValue,
                StatusCode = StatusCodes.Good,
                Timestamp = DateTime.Now,
                NodeId = (parent == null) ? new NodeId(name, NamespaceIndex) : new NodeId(parent.NodeId.ToString() + "/" + name)

            };
            if (parent != null)
            {
                parent.AddChild(variable);
            }
            return variable;
        }
        
        private bool ChangeBool(string s_Bool)
        {
            bool re =false;
            if (s_Bool == "0")
            {
                re = false;
            }
            else
            {
                re = true;
            }
            return re;
        }

        #endregion

        #region 新增計算(計算原理尚不甚理解)
        private MethodState CreateMethod(NodeState parent, string name, string description)
        {
            //函數狀態基礎狀態
            MethodState method = new MethodState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                BrowseName = new QualifiedName(name, NamespaceIndex),
                DisplayName = new LocalizedText(name),
                WriteMask = AttributeWriteMask.None,
                UserWriteMask = AttributeWriteMask.None,
                Executable = true,
                UserExecutable = true,
                Description = description,
                NodeId = (parent == null) ? new NodeId(name, NamespaceIndex) : new NodeId(parent.NodeId.ToString() + "/" + name)
            };
            if (parent != null)
            {
                parent.AddChild(method);
            }
            return method;
        }
        /// <summary>
        /// 設定新函數(輸入)
        /// </summary>
        /// <param name="method">對應函數</param>
        private void InPutMethod(MethodState method)
        {
            method.InputArguments = new PropertyState<Argument[]>(method)
            {
                NodeId = new NodeId(method.BrowseName.Name + "InArgs", NamespaceIndex),
                BrowseName = BrowseNames.InputArguments
            };
            method.InputArguments.DisplayName = method.InputArguments.BrowseName.Name;
            method.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            method.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            method.InputArguments.DataType = DataTypeIds.Argument;
            method.InputArguments.ValueRank = ValueRanks.OneDimension;

            method.InputArguments.Value = new Argument[]
            {
                new Argument() { Name = "Int32", Description = "Int32 value",  DataType = DataTypeIds.Int32, ValueRank = ValueRanks.Scalar , Value = 100},
                new Argument() { Name = "Float", Description = "Float value",  DataType = DataTypeIds.Float, ValueRank = ValueRanks.Scalar , Value = 150.3f}
            };
        }
        /// <summary>
        /// 設定新函數(輸出)
        /// </summary>
        /// <param name="method">對應函數</param>
        private void OutPutMethod(MethodState method)
        {
            method.OutputArguments = new PropertyState<Argument[]>(method);
            method.OutputArguments.NodeId = new NodeId(method.BrowseName.Name + "OutArgs", NamespaceIndex);
            method.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            method.OutputArguments.DisplayName = method.OutputArguments.BrowseName.Name;
            method.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            method.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            method.OutputArguments.DataType = DataTypeIds.Argument;
            method.OutputArguments.ValueRank = ValueRanks.OneDimension;

            method.OutputArguments.Value = new Argument[]
            {
                new Argument() { Name = "Operater Result", Description = "Operater Result",  DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar }
            };
        }
        private ServiceResult OnAddCall(ISystemContext context, MethodState method, IList<object> inputArguments, IList<object> outputArguments)
        {
            // 必須拿出所有參數
            if (inputArguments.Count < 2)
            {
                return StatusCodes.BadArgumentsMissing;//輸入值錯誤
            }
            try
            {
                //inputArguments值未能輸入  要找原因
                Int32 intValue = (Int32)inputArguments[0];
                float floatValue = (float)inputArguments[1];

                //inputArguments[0] = intValue = intlist[0].Value;
                //inputArguments[1] = floatValue = floatlist[0].Value;

                int re = intValue + (int)floatValue;
                // set output parameter
                //outputArguments[0] = "我也不知道刚刚发生了什么，调用设备为：" + method.Parent.DisplayName;
                outputArguments[0] = "輸出為：" + re;
                return ServiceResult.Good;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return new ServiceResult(StatusCodes.BadInvalidArgument);
            }
        }
        #endregion

        private void NodeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (TimeTickList != null)
            {
                lock (Lock)
                {
                    for (int i = 0; i < TimeTickList.Count; i++)
                    {
                        TimeTickList[i].Value = TimeTickList[i].Value + 1;
                        // 下面这行代码非常的关键，涉及到更改之后会不会通知到客户端
                        TimeTickList[i].ClearChangeMasks(SystemContext, false);
                    }
                }
            }
        }
    }

    public class OpcDataItem
    {
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
        //資料夾用
        public string Parent { get; set; }
        //通用
        public string FolderName { get; set; }//以(string)NodeID.Identifier作為唯一資料夾名稱追查
        public string Description { get; set; }//描述
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
