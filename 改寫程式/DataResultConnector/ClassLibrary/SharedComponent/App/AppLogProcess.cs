using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.SharedComponent.App
{

    public enum AOI_LOG_TYPE
    {
        LOG_TYPE_BEGIN = 0,
        LOG_SYSTEM = 0,
        LOG_DEBUG,
        LOG_PLCSOCKET,
        LOG_PLCC10,
        LOG_EMCDEBUG,
        LOG_EMCSYSTEM,
        LOG_OPC,
        LOG_THICK,
        LOG_MSSQL,
        LOG_TYPE_MAX
    };
    public struct LOG_ITEM_INFO
    {
        public AOI_LOG_TYPE xType;
        public string strFile;
        public uint nLimitSize;
    }

    //LOG_ITEM_INFO[] ctLOG_INFO = new LOG_ITEM_INFO[]
    //{
    //    new LOG_ITEM_INFO {xType = AOI_LOG_TYPE.LOG_SYSTEM,    strFile = "PLC",        nLimitSize = 1024 * 1024 },
    //    new LOG_ITEM_INFO {xType = AOI_LOG_TYPE.LOG_DEBUG,     strFile = "DEBUG.PLC",  nLimitSize = 1024 * 1024 },
    //    new LOG_ITEM_INFO {xType = AOI_LOG_TYPE.LOG_PLCSOCKET, strFile = "Socket.PLC", nLimitSize = 1024 * 1024 },
    //    new LOG_ITEM_INFO {xType = AOI_LOG_TYPE.LOG_PLCC10,    strFile = "C10.PLC",    nLimitSize = 1024 * 1024 },
    //    new LOG_ITEM_INFO {xType = AOI_LOG_TYPE.LOG_EMCDEBUG,  strFile = "DEBUG.log",  nLimitSize = 1024 * 1024 },
    //    new LOG_ITEM_INFO {xType = AOI_LOG_TYPE.LOG_EMCSYSTEM, strFile = "System.log", nLimitSize = 1024 * 1024 },
    //    new LOG_ITEM_INFO {xType = AOI_LOG_TYPE.LOG_OPC,       strFile = "OPC.log",    nLimitSize = 1024 * 1024 },
    //    new LOG_ITEM_INFO {xType = AOI_LOG_TYPE.LOG_THICK,     strFile = "LK.log",     nLimitSize = 1024 * 1024 },
    //    new LOG_ITEM_INFO {xType = AOI_LOG_TYPE.LOG_MSSQL,     strFile = "MSSQL.log",  nLimitSize = 1024 * 1024 }
    //};

    public interface IAppLogProcess
    {

    }
    class AppLogProcess
    {
    }
}
