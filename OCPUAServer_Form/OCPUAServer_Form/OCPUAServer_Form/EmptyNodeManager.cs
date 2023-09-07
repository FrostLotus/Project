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
        private List<BaseDataVariableState<int>> list = null;//變數狀態
        private System.Timers.Timer timer1 = null;

        #region Constructors
        /// <summary>
        /// 初始化節點管理器
        /// </summary>
        public EmptyNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        :
            base(server, configuration, Namespaces.ReferenceApplications)
        {
            SystemContext.NodeIdFactory = this;

            // 取得節點管理器配置
            m_configuration = configuration.ParseExtension<ReferenceServerConfiguration>();

            // 若空:預設配置
            if (m_configuration == null)
            {
                m_configuration = new ReferenceServerConfiguration();
            }

            timer1 = new System.Timers.Timer(500);
            timer1.Elapsed += Timer1_Elapsed;
            timer1.Start();
        }
        #endregion

        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (list != null)
            {
                lock (Lock)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].Value = list[i].Value + 1;
                        // 下面这行代码非常的关键，涉及到更改之后会不会通知到客户端
                        list[i].ClearChangeMasks(SystemContext, false);
                    }
                }
            }
        }
    }
}
