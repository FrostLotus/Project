using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary.SharedComponent.Log
{
	public enum OP_THREAD
	{
		OP_THREAD_CREATE = 0,
		OP_THREAD_DESTROY
	};
	public class AoiLogThread : AppLogBase
	{
		private Thread m_thread;
		public AoiLogThread()
        {
		}
		public void StartThreadedTask(Action methodToRun)
		{
			Thread taskThread = new Thread(() => methodToRun());
			taskThread.Start();
		}
		public void StartThreadedTask<T>(Action<T> methodToRun, T param)
		{
			Thread taskThread = new Thread(() => methodToRun(param));
			taskThread.Start();
		}
		public void StartThreadedTask<T1,T2>(Action<T1, T2> methodToRun, T1 param1, T2 param2)
		{
			Thread taskThread = new Thread(() => methodToRun(param1, param2));
			taskThread.Start();
		}
		//-----------------------------------------------------------------------
		public void LogMessage(string logMessage, AOI_LOG_TYPE xType)
		{
			string pMsg = logMessage;
			InsertLog(pMsg, xType);
		}
		public void LogExit()
		{
			//目前單次調用用不到
		}

	}
}
