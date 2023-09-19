using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OCPUAServer
{
    /// <summary>
    /// 存儲數據訪問節點管理器的配置。
    /// </summary>
    [DataContract(Namespace = Namespaces.ReferenceApplications)]
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
