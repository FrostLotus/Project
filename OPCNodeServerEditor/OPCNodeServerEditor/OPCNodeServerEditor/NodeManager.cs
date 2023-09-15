using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCNodeServerEditor
{
    public class NodeManager : CustomNodeManager2
    {
        private ReferenceServerConfiguration Configuration;//基本為空
        private List<BaseDataVariableState<int>> TimeTickList = new List<BaseDataVariableState<int>>();//變數狀態
        private System.Timers.Timer NodeTimer = null;

        private IList<IReference> references = null;//節點參考列表
        /// <summary>
        /// 初始化節點管理器
        /// </summary>
        public NodeManager(IServerInternal server, ApplicationConfiguration configuration)
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
            //先初始化所有需要進來的DATA容器為empty(除INI.DATA)
            //CParam.LoadData();
            CParam.FolderList.Clear();
            CParam.VariableList.Clear();
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
                
                foreach (OpcDataItem roll in CParam.StringVariableList)
                {
                    FolderState rootNode = FindFolder(roll.FolderName);
                    OpcDataVariable<object> tmpData;
                    //單一值變數
                    switch (roll.DataType)
                    {
                        case "String":
                            tmpData = new OpcDataVariable<object>
                            {
                                _BaseDataVariableState = CreateVariable(rootNode, roll.NodeID, DataTypeIds.String, ValueRanks.Scalar, roll.Description, roll.Value),
                                _OpcDataItem = roll
                            };
                            CParam.VariableList.Add(tmpData);
                            AddPredefinedNode(SystemContext, rootNode);
                            //AddNotifier(SystemContext,)
                            break;
                        case "Bool":
                            tmpData = new OpcDataVariable<object>
                            {
                                _BaseDataVariableState = CreateVariable(rootNode, roll.NodeID, DataTypeIds.Boolean, ValueRanks.Scalar, roll.Description, roll.Value),
                                _OpcDataItem = roll
                            };
                            CParam.VariableList.Add(tmpData);
                            AddPredefinedNode(SystemContext, rootNode);
                            break;
                        case "Real":
                            tmpData = new OpcDataVariable<object>
                            {
                                _BaseDataVariableState = CreateVariable(rootNode, roll.NodeID, DataTypeIds.Float, ValueRanks.Scalar, roll.Description, roll.Value),
                                _OpcDataItem = roll
                            };
                            CParam.VariableList.Add(tmpData);
                            AddPredefinedNode(SystemContext, rootNode);
                            break;
                        case "Word":
                            tmpData = new OpcDataVariable<object>
                            {
                                _BaseDataVariableState = CreateVariable(rootNode, roll.NodeID, DataTypeIds.String, ValueRanks.Scalar, roll.Description, roll.Value),
                                _OpcDataItem = roll
                            };
                            CParam.VariableList.Add(tmpData);
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
        private BaseDataVariableState<T> CreateVariable<T>(NodeState parent, string name, NodeId dataType, int valueRank, string description, T defaultValue)
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
                NodeId = (parent == null) 
                ? new NodeId(name, NamespaceIndex) 
                : new NodeId(parent.NodeId.ToString() + "/" + name), 
            };

            if (parent != null)
            {
                parent.AddChild(variable);
            }
            return variable;
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
 
}
