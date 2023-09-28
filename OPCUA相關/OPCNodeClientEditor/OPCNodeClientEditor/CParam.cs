﻿using Opc.Ua;
using OpcUaHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPCNodeClientEditor
{
    /// <summary>
    /// 公用變數庫
    /// </summary>
    public class CParam
    {
        public static string ServerURL;//對應server位置
        public static int IterationIndex = 1;//變數索引
        public static string StartNodeTag = "ns=2;s=A";//預設節點
        public static ushort NamespaceIndex = 2;

        public static OpcUaClient m_OpcUaClient;// = new OpcUaClient();//OpcUaHelper

        public static List<FolderState> FolderList = new List<FolderState>();//檔案夾節點列表
        public static List<OpcDataVariable<object>> VariableList = new List<OpcDataVariable<object>>();//變數列表
  
        public static void NodeItemPullOut(string startNodetag)
        {
            //StringVariableList.Clear();
            VariableList.Clear();
            IterationIndex = 1;

            //先將當前資料夾(預設節點)加入stringFolderList中
            NamespaceIndex = 2;
            FolderState rootButtom = new FolderState(null);//A
            rootButtom = CreateFolder(null, "A", null);
            FolderList.Add(rootButtom);
            //將當前資料夾全部東西讀出來
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
                                //先將當下檔案夾=>FolderList
                                NamespaceIndex = 2;
                                FolderState rootMy = CreateFolder(FindFolder("A"), item.BrowseName.Name, null);//第一層
                                CParam.FolderList.Add(rootMy);
                                //再往下找
                                NodeFolderPullOut(item.NodeId, rootMy.NodeId.Identifier.ToString());
                                break;
                            case "Variable"://變數
                                VariableToList(item, rootButtom.NodeId.Identifier.ToString());
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
        }
        private static void NodeFolderPullOut(ExpandedNodeId nodeid,string parent)
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
                                //先將當下檔案夾=>FolderList
                                NamespaceIndex = 2;
                                FolderState rootMy = CreateFolder(FindFolder(nodeid.Identifier.ToString()), item.BrowseName.Name, null);//第一層
                                CParam.FolderList.Add(rootMy);
                                //再往下找
                                NodeFolderPullOut(item.NodeId, rootMy.NodeId.Identifier.ToString());
                                break;
                            case "Variable"://變數
                                VariableToList(item,parent);
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
        private static void VariableToList(ReferenceDescription rdescription,string parent)
        {
            //FolderState rootNode = FindFolder(rdescription.NodeId.FolderName);
            OpcDataVariable<object> tmpData;

            OpcDataItem tmpOpcDataItem = new OpcDataItem
            {
                VaribleTag = rdescription.NodeId.ToString(),
                Index = IterationIndex,
                ItemName = rdescription.DisplayName.Text,//先暫定
                NodeID = rdescription.BrowseName.Name
            };

            IterationIndex++;//使找出的節點照編號順序加入
            OpcNodeAttribute[] nodeAttributes = m_OpcUaClient.ReadNoteAttributes(tmpOpcDataItem.VaribleTag);
            foreach (var roll in nodeAttributes)//單一variable節點定義屬性
            {
                if (roll.Name == "Value")//取值及type
                {
                    switch (roll.Type)
                    {
                        case "String":
                            //補屬性
                            tmpOpcDataItem.DataType = "String";
                            //tmpOpcDataItem.Value = roll.Value;
                            //將變數餵入BaseDataVariableState回填List
                            tmpData = new OpcDataVariable<object>
                            {

                                _BaseDataVariableState = CreateVariable(FindFolder(parent),
                                                                        rdescription.BrowseName.Name,
                                                                        DataTypeIds.String,
                                                                        ValueRanks.Scalar,
                                                                        null,
                                                                        roll.Value),
                                _OpcDataItem = tmpOpcDataItem
                            };
                            VariableList.Add(tmpData);
                            break;
                        case "Boolean":
                            tmpOpcDataItem.DataType = "Bool";
                            //tmpOpcDataItem.Value = roll.Value;
                            //將變數餵入BaseDataVariableState回填List
                            tmpData = new OpcDataVariable<object>
                            {
                                _BaseDataVariableState = CreateVariable(FindFolder(parent),
                                                                        rdescription.BrowseName.Name,
                                                                        DataTypeIds.Boolean,
                                                                        ValueRanks.Scalar,
                                                                        null,
                                                                        roll.Value),
                                _OpcDataItem = tmpOpcDataItem
                            };
                            VariableList.Add(tmpData);
                            break;
                        case "Float":
                            tmpOpcDataItem.DataType = "Real";
                            //tmpOpcDataItem.Value = roll.Value;
                            //將變數餵入BaseDataVariableState回填List
                            tmpData = new OpcDataVariable<object>
                            {
                                _BaseDataVariableState = CreateVariable(FindFolder(parent),
                                                                        rdescription.BrowseName.Name,
                                                                        DataTypeIds.Float,
                                                                        ValueRanks.Scalar,
                                                                        null,
                                                                        roll.Value),
                                _OpcDataItem = tmpOpcDataItem
                            };
                            VariableList.Add(tmpData);
                            break;
                        case "Int32":
                            tmpOpcDataItem.DataType = "Word";
                            //將變數餵入BaseDataVariableState回填List
                            //if (int.TryParse(roll.Value.ToString(), out int tmp))
                            //{
                            //    tmpOpcDataItem.Value = tmp;
                            //}
                            tmpData = new OpcDataVariable<object>
                            {
                                _BaseDataVariableState = CreateVariable(FindFolder(parent),
                                                                        rdescription.BrowseName.Name,
                                                                        DataTypeIds.Integer,
                                                                        ValueRanks.Scalar,
                                                                        null,
                                                                        roll.Value),
                                _OpcDataItem = tmpOpcDataItem
                            };
                            VariableList.Add(tmpData);
                            break;
                    }
                }
                
            }
            //StringVariableList.Add(tmpOpcDataItem);//剩下DataType value要抓
        }

        //------------------------------------------------------
        protected static FolderState FindFolder(string Parent)
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
        protected static FolderState CreateFolder(NodeState parent, string name, string description)
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
        private static BaseDataVariableState<T> CreateVariable<T>(NodeState parent, string name, NodeId dataType, int valueRank, string description, T defaultValue)
        {
            BaseDataVariableState<T> variable = new BaseDataVariableState<T>(parent)
            {
                SymbolicName = name,//
                ReferenceTypeId = ReferenceTypes.Organizes,//組織架構下
                TypeDefinitionId = VariableTypeIds.BaseDataVariableType,//變數

                BrowseName = new QualifiedName(name),//, NamespaceIndex),//瀏覽名稱
                DisplayName = new LocalizedText(name),//顯示名稱
                Description = description,//描述

                WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,//公開屬性供Client修改
                UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,//公開屬性供Client修改

                DataType = dataType,//TYPE
                ValueRank = valueRank,//變數形式
                AccessLevel = AccessLevels.CurrentReadOrWrite,//可否讀寫
                UserAccessLevel = AccessLevels.CurrentReadOrWrite,//Client可否讀寫
                Historizing = false,//是否紀錄舊有歷史紀錄
                Value = defaultValue,//變數值
                StatusCode = StatusCodes.Good,//狀態碼
                Timestamp = DateTime.Now,//紀錄目前時間
                NodeId = (parent == null)//節點位置
                ? new NodeId(name)
                : new NodeId(parent.NodeId.ToString() + "/" + name),
            };

            if (parent != null)
            {
                parent.AddChild(variable);
            }
            //追加
            return variable;
        }

    }
    public class OpcDataFolder
    {
        //資料夾用
        public string Parent { get; set; }
        //通用
        public string FolderName { get; set; }//以(string)NodeID.Identifier作為唯一資料夾名稱追查
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
    public class OpcDataVariable<T>
    {
        public BaseDataVariableState<T> _BaseDataVariableState;

        public OpcDataItem _OpcDataItem;
    }
    public enum emServerFlag
    {
        Start = 1,
        Stop = 2,
        pause = 3
    }
}
