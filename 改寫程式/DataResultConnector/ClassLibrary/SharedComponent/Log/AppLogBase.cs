using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.SharedComponent.Log
{
    /// <summary>Sizes for buffers MAX</summary>
    enum WM_LOG
    {
        WM_LOG_PROCESS		=(0x8000 + 10),
        WM_LOG_TERMINATE	=(0x8000 + 11)
    }
    enum MAX_VALUE
    {
        MAX_PATH = 260,
        MAX_DRIVE = 3,
        MAX_DIR = 256,
        MAX_FNAME = 256,
        MAX_EXT = 256
    }
    public enum AOI_LOG_Result
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
        public AOI_LOG_Result xType;
        public string strFile;
        public int nLimitSize;//uint
    }
    public class AppLogBase
    {
        string m_workingDir;
        int m_nFileCount;

        public LOG_ITEM_INFO[] ctLOG_INFO = new LOG_ITEM_INFO[]
        {
            new LOG_ITEM_INFO {xType = AOI_LOG_Result.LOG_SYSTEM,    strFile = "PLCCommunicator.PLC",        nLimitSize = 1024 * 1024 },
            new LOG_ITEM_INFO {xType = AOI_LOG_Result.LOG_DEBUG,     strFile = "PLCCommunicator_DEBUG.PLC",  nLimitSize = 1024 * 1024 },
            new LOG_ITEM_INFO {xType = AOI_LOG_Result.LOG_PLCSOCKET, strFile = "PLCCommunicator_Socket.PLC", nLimitSize = 1024 * 1024 },
            new LOG_ITEM_INFO {xType = AOI_LOG_Result.LOG_PLCC10,    strFile = "PLCCommunicator_C10.PLC",    nLimitSize = 1024 * 1024 },
            new LOG_ITEM_INFO {xType = AOI_LOG_Result.LOG_EMCDEBUG,  strFile = "EMCCommunicator_DEBUG.log",  nLimitSize = 1024 * 1024 },
            new LOG_ITEM_INFO {xType = AOI_LOG_Result.LOG_EMCSYSTEM, strFile = "EMCCommunicator_System.log", nLimitSize = 1024 * 1024 },
            new LOG_ITEM_INFO {xType = AOI_LOG_Result.LOG_OPC,       strFile = "OPCCommunicator.log",        nLimitSize = 1024 * 1024 },
            new LOG_ITEM_INFO {xType = AOI_LOG_Result.LOG_THICK,     strFile = "LK-Communicator.log",        nLimitSize = 1024 * 1024 },
            new LOG_ITEM_INFO {xType = AOI_LOG_Result.LOG_MSSQL,     strFile = "MSSQL_PROCESS.log",          nLimitSize = 1024 * 1024 }
        };

        public AppLogBase()
        {
            m_workingDir = Environment.CurrentDirectory;//取得當前目錄的字串
            m_nFileCount = ctLOG_INFO.Length;
        }
        public int LogFileCount()
        {
            return m_nFileCount;
        }
        public void InsertLog(string xMsg, AOI_LOG_Result xLogType = AOI_LOG_Result.LOG_SYSTEM)
        {
            //路徑搜索
            int szBuffer = 0;
            string MySysLogFileName;
            MySysLogFileName = $"{m_workingDir}\\{ctLOG_INFO[(int)xLogType].strFile}";
            int nSize = m_nFileCount;
            LOG_ITEM_INFO xTarget = ctLOG_INFO[(int)xLogType];//初始化

            for (int i = 0; i < nSize; i++)
            {
                if (ctLOG_INFO[i].xType == xLogType)
                {
                    MySysLogFileName = $"{m_workingDir}\\{ctLOG_INFO[i].strFile}";
                    xTarget = ctLOG_INFO[i];
                    break;
                }
            }
            //若訊息沒超過最大值
            if (xMsg.Length < xTarget.nLimitSize)
            {
                //若無檔案則建立
                if (!File.Exists(MySysLogFileName))
                {
                    using (StreamWriter createfile = File.CreateText(MySysLogFileName))
                    {
                        createfile.WriteLine(MySysLogFileName);//設置檔案
                        createfile.Close();
                    }
                }
                //資料讀出來(檔案確定建立但可能為空)
                string fileLog;
                using (StreamReader fileStream = new StreamReader(MySysLogFileName))
                {
                    fileLog = fileStream.ReadToEnd();//先將Log所有資料讀出來
                }
                string newLog = $"{DateTime.Now.ToString("g")} : {xMsg}\r\n";
#if _UNICODE
                byte[] pNewLog = System.Text.Encoding.Unicode.GetBytes(newLog);
                int nLogSize = pNewLog.Length;
#else
                int szLogSize = newLog.Length;
#endif
                int szNewSize = fileLog.Length + szLogSize;
                //若整體緩存大於極限SIZE 則保持極限SIZE (231207新增:減1保持最後能填空字符空間)
                szBuffer = ((szNewSize > xTarget.nLimitSize) ? xTarget.nLimitSize -1 : szNewSize + 1);
                char[] pBuffer = new char[szBuffer];

                for (int i = 0; i < szBuffer; i++)
                {
                    pBuffer[i] = '\0'; // 将每个元素初始化为空字符
                }
#if _UNICODE
                for (int i = 0; i < pBuffer.Length && i < newLog.Length; i++)
                {
                    pBuffer[i] = newLog[i];
                }
                //lstrcpyA(pBuffer, CW2A(pNewLog));
                //newLog.ReleaseBuffer();
#else //_UNICODE
                //原來的
                //for (int i = 0; i < pBuffer.Length&& i < newLog.Length; i++)
                //{
                //    pBuffer[i] = newLog[i];
                //}

                //231207 將所有Log鍵回新的Log
                fileLog.CopyTo(0, pBuffer, 0, fileLog.Length);
                newLog.CopyTo(0, pBuffer, fileLog.Length, newLog.Length);
#endif //_UNICODE
                //uint szLog = (uint)(szBuffer - szLogSize);//原來未加時Log大小
                //char[] pLog = pBuffer + szLogSize;//實際Log
                try
                {
                    //if (szLog > 0)
                    //{
                    //    VERIFY(fileLog.Read(pLog, szLog) == szLog);
                    //    fileLog.SeekToBegin();
                    //}
                    using (StreamWriter fileStream = new StreamWriter(MySysLogFileName))
                    {
                        //fileStream.Write(fileLog + newLog);//將Log總體寫入
                        fileStream.WriteLine(newLog);//將Log新增寫入
                    }
                    //fileLog.Write(pBuffer, szBuffer);
                    //fileLog.SetLength(szBuffer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error[InsertLog]有問題: {ex.Message}");
                }
                if (pBuffer != null)
                {
                    pBuffer = null;
                }
                //try
                //{
                //    reader.Close();
                //}
                //catch(Exception ex)
                //{
                //    Console.WriteLine($"Error[InsertLog]檔案關閉有問題: {ex.Message}");
                //}
            }
        }
    }
}
