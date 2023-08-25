using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TUCBatchEditorCSharp.Helper
{
    class LogHelper
    {
        static TextWriterTraceListener _DebugLog = new TextWriterTraceListener(System.IO.File.Open(string.Format("{0}_Debug.log", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name), System.IO.FileMode.Append));
        static TextWriterTraceListener _SystemLog = new TextWriterTraceListener(System.IO.File.Open(string.Format("{0}_System.log", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name), System.IO.FileMode.Append));
        static TextWriterTraceListener _ErrorLog = new TextWriterTraceListener(System.IO.File.Open(string.Format("{0}_Error.log", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name), System.IO.FileMode.Append));

        public LogHelper()
        {
        }

        public static void Info(string strText)
        {
            WriteLog(_SystemLog, strText);
        }
        public static void Debug(string strText)
        {
            WriteLog(_DebugLog, strText);
        }
        public static void Error(string strText)
        {
            WriteLog(_ErrorLog, strText);
        }
        private static void WriteLog(TextWriterTraceListener xListener, string strText)
        {
            Trace.Listeners.Add(xListener);
            Trace.WriteLine(string.Format("{0} {1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), strText));
            Trace.Flush();
            Trace.Listeners.Remove(xListener);
        }
    }
}
