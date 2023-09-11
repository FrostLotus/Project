using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Server;

namespace OCPUAServer
{

    public class EmptyNodeManager : CustomNodeManager2
    {
        //參數
        private ReferenceServerConfiguration m_configuration;//基本為空
        private Opc.Ua.Test.DataGenerator m_generator;
        private BaseDataVariableState<bool> SystemState = null;//紀錄
        private List<BaseDataVariableState<int>> TimeTickList = new List<BaseDataVariableState<int>>();//變數狀態
        private System.Timers.Timer NodeTimer = null;
        private string FirstLayerFolder = "Fatory";//"Machines"
        string[] SecondLayermachines = new string[] { "Union A", "Union B", "Union C" };


        //test
        public List<BaseDataVariableState<int>> list = null;
        public List<BaseDataVariableState<int>> intlist = null;
        public List<BaseDataVariableState<float>> floatlist = null;

        public List<TreeNodeItem> FilesTree = null;




        /// <summary>
        /// 初始化節點管理器
        /// </summary>
        public EmptyNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        : base(server, configuration, Namespaces.ReferenceApplications)
        {
            SystemContext.NodeIdFactory = this;

            // 取得節點管理器配置
            m_configuration = configuration.ParseExtension<ReferenceServerConfiguration>();

            // 若空:預設配置
            if (m_configuration == null)
            {
                m_configuration = new ReferenceServerConfiguration();
            }


            NodeTimer = new System.Timers.Timer(500);
            NodeTimer.Elapsed += NodeTimer_Elapsed;
            NodeTimer.Start();
        }

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            BaseInstanceState instance = node as BaseInstanceState;

            if (instance != null && instance.Parent != null)
            {
                string id = instance.Parent.NodeId.Identifier as string;

                if (id != null)
                {
                    return new NodeId(id + "_" + instance.SymbolicName, instance.Parent.NodeId.NamespaceIndex);
                }
            }
            return node.NodeId;
        }
        #endregion

        #region Private Helper Functions
        private static bool IsUnsignedAnalogType(BuiltInType builtInType)
        {
            bool re = false;
            if (builtInType == BuiltInType.Byte ||
                builtInType == BuiltInType.UInt16 ||
                builtInType == BuiltInType.UInt32 ||
                builtInType == BuiltInType.UInt64)
            {
                re = true;
            }
            return re;
        }

        private static bool IsAnalogType(BuiltInType builtInType)
        {
            bool re = false;
            switch (builtInType)
            {
                case BuiltInType.Byte:
                case BuiltInType.UInt16:
                case BuiltInType.UInt32:
                case BuiltInType.UInt64:
                case BuiltInType.SByte:
                case BuiltInType.Int16:
                case BuiltInType.Int32:
                case BuiltInType.Int64:
                case BuiltInType.Float:
                case BuiltInType.Double:
                    re = true;
                    break;
            }
            return re;
        }

        private static Opc.Ua.Range GetAnalogRange(BuiltInType builtInType)
        {
            switch (builtInType)
            {
                case BuiltInType.UInt16:
                    return new Range(System.UInt16.MaxValue, System.UInt16.MinValue);
                case BuiltInType.UInt32:
                    return new Range(System.UInt32.MaxValue, System.UInt32.MinValue);
                case BuiltInType.UInt64:
                    return new Range(System.UInt64.MaxValue, System.UInt64.MinValue);
                case BuiltInType.SByte:
                    return new Range(System.SByte.MaxValue, System.SByte.MinValue);
                case BuiltInType.Int16:
                    return new Range(System.Int16.MaxValue, System.Int16.MinValue);
                case BuiltInType.Int32:
                    return new Range(System.Int32.MaxValue, System.Int32.MinValue);
                case BuiltInType.Int64:
                    return new Range(System.Int64.MaxValue, System.Int64.MinValue);
                case BuiltInType.Float:
                    return new Range(System.Single.MaxValue, System.Single.MinValue);
                case BuiltInType.Double:
                    return new Range(System.Double.MaxValue, System.Double.MinValue);
                case BuiltInType.Byte:
                    return new Range(System.Byte.MaxValue, System.Byte.MinValue);
                default:
                    return new Range(System.SByte.MaxValue, System.SByte.MinValue);
            }
        }
        #endregion

        #region INodeManager Members
        /// <summary>
        /// 初始化做完再使用address space
        /// </summary>
        /// <param name="externalReferences"></param>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                //加載預定義節點
                LoadPredefinedNodes(SystemContext, externalReferences);
                //節點參考列表
                IList<IReference> references = null;
                //嘗試取得至Objects節點檔案夾參考資料
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    //沒有的話 置入IReference結構給它
                    references = new List<IReference>();
                    externalReferences[ObjectIds.ObjectsFolder] = references;
                }
                FilesTree = new List<TreeNodeItem>();

                //建立"Objects"下的第一層目錄
                FolderState rootMy = CreateFolder(null, FirstLayerFolder,"第一層");

                rootMy.AddReference(ReferenceTypes.Organizes, false, ObjectIds.ObjectsFolder);//提供目錄參考(狀態 排列等) 中間本為true 
                references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, rootMy.NodeId));//建立目錄
                rootMy.EventNotifier = EventNotifiers.SubscribeToEvents;//觸發事件
                AddRootNotifier(rootMy);

                //FilesTree.Add(new TreeNodeItem(FirstLayerFolder, ItemType.Folder));


                string[] machines = new string[] { "Machine A", "Machine B", "Machine C" };
                list = new List<BaseDataVariableState<int>>();
                intlist = new List<BaseDataVariableState<int>>();
                floatlist = new List<BaseDataVariableState<float>>();
                //=================================================================

                for (int i = 0; i < machines.Length; i++)
                {
                    //新增參數節點
                    FolderState myFolder = CreateFolder(rootMy, machines[i]);//建立所屬參數資料夾
                    #region Add Variable
                    CreateVariable(myFolder, "Name", DataTypeIds.String, ValueRanks.Scalar, "設備的名稱(String)", "測試文字");
                    CreateVariable(myFolder, "IsActive", DataTypeIds.Boolean, ValueRanks.Scalar, "設備是否啟動(Bool)", true);
                    CreateVariable(myFolder, "ValueFloat", DataTypeIds.Float, ValueRanks.Scalar, "設備參數(Float)", 100.5f);
                    CreateVariable(myFolder, "ValueInt", DataTypeIds.Int32, ValueRanks.Scalar, "設備參數(Int)", 0);
                    CreateVariable(myFolder, "AlarmTime", DataTypeIds.DateTime, ValueRanks.Scalar, "建立時間", DateTime.Now);
                    intlist.Add(CreateVariable(myFolder, "UseInt", DataTypeIds.Int32, ValueRanks.Scalar, "測試(INT)",0));
                    floatlist.Add(CreateVariable(myFolder, "UseFloat", DataTypeIds.Float, ValueRanks.Scalar, "測試(Float)", 0.0f));

                    TimeTickList.Add(CreateVariable(myFolder, "ValueIntTick", DataTypeIds.Int32, ValueRanks.Scalar, "時間增加建立次數(Int)", 1000));
                    #endregion

                    #region Add Method
                    MethodState addMethod = CreateMethod(myFolder, "Calculate","計算描述");
                    // set input arguments 設定函式輸入
                    InPutMethod(addMethod);
                    // set output arguments 設定函式輸出
                    OutPutMethod(addMethod);
                    //將函式放到事件處理中
                    addMethod.OnCallMethod = new GenericMethodCalledEventHandler(OnAddCall);
                    #endregion

                }
                SystemState = CreateVariable(rootMy, "Enable", DataTypeIds.Boolean, ValueRanks.Scalar,"許可", false);
                CreateVariable(rootMy, "Mat", DataTypeIds.Double, ValueRanks.TwoDimensions, "單一4*4陣列(double)", new double[4, 4]);

                AddPredefinedNode(SystemContext, rootMy);//將定義好的項目推送出去

                FolderState robots = CreateFolder(null, "Robots");
                robots.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, robots.NodeId));
                robots.EventNotifier = EventNotifiers.SubscribeToEvents;
                AddRootNotifier(robots);

                AddPredefinedNode(SystemContext, robots);//將定義好的項目推送出去
            }
        }
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
                NodeId=(parent == null)//有無父節點
                ? new NodeId(name, NamespaceIndex)//以自身為節點
                : new NodeId(parent.NodeId.ToString() + "/" + name)//以父+自身為節點
            };
            if (parent != null)
            {
                parent.AddChild(folder);
            }
            return folder;
        }
        /// <summary>
        /// 產生一個變數節點<T>
        /// </summary>
        private BaseDataVariableState<T> CreateVariable<T>(NodeState parent, string name, NodeId dataType, int valueRank,string description, T defaultValue)
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
                NodeId = (parent == null)? new NodeId(name, NamespaceIndex): new NodeId(parent.NodeId.ToString() + "/" + name)
            };
            if (parent != null)
            {
                parent.AddChild(variable);
            }
            return variable;
        }
        /// <summary>
        /// 產生一個新函數
        /// </summary>
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
                NodeId = (parent == null)? new NodeId(name, NamespaceIndex) : new NodeId(parent.NodeId.ToString() + "/" + name)
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
        /// <summary>
        /// 事件處理中函式
        /// </summary>
        /// <param name="context"></param>
        /// <param name="method"></param>
        /// <param name="inputArguments"></param>
        /// <param name="outputArguments"></param>
        /// <returns></returns>
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
        
        //------------------------------------------



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
    public class TreeNodeItem
    {
        public string Name { get; }
        public ItemType Type { get; }
        public List<TreeNodeItem> Children { get; }

        public TreeNodeItem(string name, ItemType type)
        {
            Name = name;
            Type = type;
            Children = new List<TreeNodeItem>();
        }
    }
    public enum ItemType
    {

        UFolder,
        Variable,
        Function
    }
}
