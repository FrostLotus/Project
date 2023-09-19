using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;

namespace TUCBatchEditorCSharp.CustomData
{
    class CmdObject
    {
        /// <summary>
        /// 命令類別
        /// </summary>
        public enum CmdType
        {
            /// <summary>
            /// 新增
            /// </summary>
            Insert,
            /// <summary>
            /// 更新
            /// </summary>
            Update,
            /// <summary>
            /// 刪除
            /// </summary>
            Delete
        }
        /// <summary>
        /// 判斷呼叫哪個function
        /// </summary>
        public CmdType Type { get; set; }
        public string Cmd { get; set; }
        public OdbcParameter[] Parameter { get; set; }
    }
    /// <summary>
    /// 通知UI層
    /// </summary>
    public interface IDataCallBack
    {
        void OnQueryThreadCallBack(GridDataType eType, int nCount, bool bSuccess);
    }
    public enum GridDataType
    {
        Used,
        Using,
        UnUsed,
        TypeMax
    }
    abstract class DataManagerBase: IDisposable
    {
        /// <summary>
        /// UI取得工單物件顯示
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public abstract DB.IEditable GetBatchObject(GridDataType eType, int nIndex);
        /// <summary>
        /// 取得DataGridView顯示資料型別
        /// </summary>
        /// <returns></returns>
        public abstract Type GetColumnType();
        /// <summary>
        /// 取得檢測設定清單
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetInspList();
        /// <summary>
        /// UI結束編輯事件
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="xOld"></param>
        /// <param name="xNew"></param>
        public abstract void OnFinishEdit(frmAddnEdit.FormType eType, DB.IEditable xOld, DB.IEditable xNew);
        /// <summary>
        /// UI刪除事件
        /// </summary>
        /// <param name="xDelete"></param>
        /// <param name="eType"></param>
        public abstract void OnDelete(DB.IEditable xDelete, GridDataType eType);
        public abstract void StartQueryThread();
        public void Dispose()
        {
            OnDispose();
        }
        protected abstract void OnDispose();
    }
    abstract class DataManagerBase2<TBatchClass> : DataManagerBase where TBatchClass : DB.IEditable, new()
    {
        private List<TBatchClass> listUsed = new List<TBatchClass>();
        private List<TBatchClass> listUsing = new List<TBatchClass>();
        private List<TBatchClass> listUnUsed = new List<TBatchClass>();
        private System.Threading.Thread threadQuery = null;
        private System.Threading.Thread threadCmd = null;

        protected static object lockQuery = new object();
        private static object lockCmd = new object();
        private Queue<CmdObject> queueCmd = new Queue<CmdObject>();
        protected string strDBCon;
        private IDataCallBack callback;
        public DataManagerBase2(string strDBCon, IDataCallBack callback)
        {
            this.strDBCon = strDBCon;
            threadQuery = new System.Threading.Thread(Thread_Query);
            threadCmd = new System.Threading.Thread(Thread_Cmd);
            this.callback = callback;

        }
        #region 抽象/虛擬
        protected override void OnDispose()
        {
            threadQuery.Abort();
            threadCmd.Abort();
        }
        public override void StartQueryThread()
        {
            threadQuery.Start();
            threadCmd.Start();
        }
        /// <summary>
        /// 取得query string
        /// </summary>
        /// <param name="eType"></param>
        /// <returns></returns>
        protected abstract string GetQueryCmd(GridDataType eType);
        /// <summary>
        /// 取得insert指令物件
        /// </summary>
        /// <returns></returns>
        protected abstract CmdObject GetInsertCmd(DB.IEditable xNew);
        /// <summary>
        /// 取得update指令物件
        /// </summary>
        /// <returns></returns>
        protected abstract CmdObject GetUpdateCmd(List<TBatchClass> lsUnUsed, DB.IEditable xOld, DB.IEditable xNew);
        /// <summary>
        /// 取得delete指令物件
        /// </summary>
        /// <param name="xItem"></param>
        /// <param name="eType"></param>
        /// <returns></returns>
        protected abstract CmdObject GetDeleteCmd(DB.IEditable xItem, GridDataType eType);
        /// <summary>
        /// 查詢檢測設定
        /// </summary>
        protected virtual void DoQueryInsp() { }
        public override void OnFinishEdit(frmAddnEdit.FormType eType, DB.IEditable xOld, DB.IEditable xNew)
        {
            switch (eType)
            {
                case frmAddnEdit.FormType.Add:
                    lock (lockCmd)
                    {
                        queueCmd.Enqueue(GetInsertCmd(xNew));
                    }
                    break;
                case frmAddnEdit.FormType.Edit:
                    lock (lockQuery)
                    {
                        CmdObject xCmd = GetUpdateCmd(listUnUsed, xOld, xNew);
                        lock (lockCmd)
                        {
                            queueCmd.Enqueue(xCmd);
                        }
                    }
                    break;
            }
        }
        public override void OnDelete(DB.IEditable xDelete, GridDataType eType)
        {
            lock (lockCmd)
            {
                queueCmd.Enqueue(GetDeleteCmd(xDelete, eType));
            }
        }
        #endregion
        /// <summary>
        /// UI取得工單物件顯示
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public override DB.IEditable GetBatchObject(GridDataType eType, int nIndex)
        {
            TBatchClass Batch = new TBatchClass();
            lock(lockQuery)
            {
                List<TBatchClass> list = null;
                switch(eType)
                {
                    case GridDataType.UnUsed:
                        list = listUnUsed;
                        break;
                    case GridDataType.Used:
                        list = listUsed;
                        break;
                    case GridDataType.Using:
                        list = listUsing;
                        break;
                }
                if (list != null && list.Count > nIndex)
                {
                    Batch = list.ElementAt(nIndex);
                }
            }
            return Batch;
        }
        /// <summary>
        /// 取得DataGridView顯示資料型別
        /// </summary>
        /// <returns></returns>
        public override Type GetColumnType() { return typeof(TBatchClass); }

        private void DoQueryBatch(ref List<TBatchClass> lsDst, GridDataType eType)
        {
            var lsRtn = new List<TBatchClass>();
            bool bSuccess = ODBCHelper.QueryData<TBatchClass>(strDBCon, GetQueryCmd(eType), ref lsRtn);
            int nCount = 0;
            if (bSuccess)
            {
                lock (lockQuery)
                {
                    lsDst = lsRtn;
                    nCount = lsDst.Count;
                }
            }
            //callback
            callback.OnQueryThreadCallBack(eType, nCount, bSuccess);
        }
        private void Thread_Query()
        {
            while (true)
            {
                DoQueryBatch(ref listUnUsed, GridDataType.UnUsed);
                DoQueryBatch(ref listUsing, GridDataType.Using);
                DoQueryBatch(ref listUsed, GridDataType.Used);

                DoQueryInsp();//指定繼承資料表全取出(SELECT *)並鍵入指定list當中
#if DEBUG
                System.Threading.Thread.Sleep(1000);
#else
                System.Threading.Thread.Sleep(1000);
#endif
            }
        }
        private void Thread_Cmd()
        {
            while (true)
            {
                lock (lockCmd)
                {
                    if (queueCmd.Count > 0)
                    {
                        CmdObject xCmdObj = queueCmd.Dequeue();
                        switch (xCmdObj.Type)
                        {
                            case CmdObject.CmdType.Insert:
                                ODBCHelper.InsertData(strDBCon, xCmdObj.Cmd, xCmdObj.Parameter);
                                //
                                break;
                            case CmdObject.CmdType.Update:
                                ODBCHelper.UpdateData(strDBCon, xCmdObj.Cmd, xCmdObj.Parameter);
                                break;
                            case CmdObject.CmdType.Delete:
                                ODBCHelper.DeleteData(strDBCon, xCmdObj.Cmd, xCmdObj.Parameter);
                                break;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
        }
    }
}
