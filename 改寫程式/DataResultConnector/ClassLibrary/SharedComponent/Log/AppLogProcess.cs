using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary.SharedComponent.Log
{
    public class AppLogProcess : AppLogBase
    {
        AoiLogThread m_pLogThread;
        public AppLogProcess()
        {
            Init();
        }
        ~AppLogProcess()
        {
            Dispose();
        }
        void Init()
        {
            m_pLogThread = null;
        }
        protected void Dispose()
        {
            OpLogThread((int)OP_THREAD.OP_THREAD_DESTROY);
        }
        public void StartLogServer()
        {
            OpLogThread((int)OP_THREAD.OP_THREAD_CREATE);
        }
        public void StopLogServer()
        {
            OpLogThread((int)OP_THREAD.OP_THREAD_DESTROY);
        }
        string GetLogFileName(int xType)
        {
            string strName = "No Log File Found";
            int nCount = LogFileCount();
            for (int i = 0; i < nCount; i++)
            {
                if ((int)ctLOG_INFO[i].xType == xType)
                {
                    strName = ctLOG_INFO[i].strFile;
                    return strName;
                }
            }
            return strName;
        }
        void OpLogThread(int nOpCode)
        {
            //Thread建立
            if (nOpCode == (int)OP_THREAD.OP_THREAD_CREATE)
            {
                //若null
                if (m_pLogThread != null)
                {
                    m_pLogThread.StartThreadedTask(m_pLogThread.LogExit);
                }
                m_pLogThread = new AoiLogThread();
            }
            //Thread解散
            else if (nOpCode == (int)OP_THREAD.OP_THREAD_DESTROY)
            {
                if (m_pLogThread != null)
                {
                    m_pLogThread.StartThreadedTask(m_pLogThread.LogExit);
                }
            }
        }
        public void InsertDebugLog(string xMsg, AOI_LOG_Result xType = AOI_LOG_Result.LOG_SYSTEM)
        {
            if (m_pLogThread != null)
            {
                m_pLogThread.StartThreadedTask(m_pLogThread.LogMessage, xMsg, xType);
            }
            else
            {
                InsertLog(xMsg, xType);
            }
        }
    }
}
