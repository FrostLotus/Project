using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using TUCBatchEditorCSharp.DB;
using TUCBatchEditorCSharp.DB.TTA;

namespace TUCBatchEditorCSharp.CustomData
{
    class TTADATA : DataManagerBase2<TUCBatchEditorCSharp.DB.TTA.CCDTTA>
    {
        /// <summary>
        /// 任務編排資料表
        /// </summary>
        List<DB.TUC.CCDBA> listInsp = new List<DB.TUC.CCDBA>();
        /// <summary>
        /// 實作有的Callback每一次執行緒的監聽)
        /// </summary>
        /// <param name="strDBCon">資料庫連結字串</param>
        /// <param name="callback">callback作用點</param>
        public TTADATA(string strDBCon, IDataCallBack callback) : base(strDBCon, callback)
        {

        }
        /// <summary>
        /// 取得資料庫cmd字串
        /// </summary>
        /// <param name="eType">資料狀態</param>
        /// <returns>cmd字串</returns>
        protected override string GetQueryCmd(GridDataType eType)
        {
            string strRe = "";
            switch (eType)
            {
                case GridDataType.UnUsed:
                    strRe = "SELECT * FROM CCDTTA";
                    break;
                case GridDataType.Using:
                    strRe = "SELECT * FROM CCDTTAUSED WHERE STSN = 1";
                    break;
                case GridDataType.Used:
                    strRe = "SELECT * FROM CCDTTAUSED WHERE STSN = 2";
                    break;
            }
            return strRe;
        }
        /// <summary>
        /// 查詢參數資料表
        /// </summary>
        protected override void DoQueryInsp()
        {
            var lsCCDBA = new List<DB.TUC.CCDBA>();
            if (ODBCHelper.QueryData<DB.TUC.CCDBA>(strDBCon, "SELECT * FROM CCDBA", ref lsCCDBA))
            {
                lock (lockQuery)
                {
                    listInsp = lsCCDBA;
                }
            }
        }
        /// /// <summary>
        /// 取得listInsp中PARAM的List
        /// </summary>
        /// <returns>PARAM的List</returns>
        public override List<string> GetInspList()
        {
            List<string> lsStrRe = null;
            lock (lockQuery)
            {
                lsStrRe = listInsp.Select(x => x.PARAM).ToList();//將listInsp中的"@PARAM"參數取出轉成List回傳
            }
            return lsStrRe;
        }
        /// <summary>
        /// 取得新增Cmd控制碼
        /// </summary>
        /// <param name="xNew">新增項目DB</param>
        /// <returns>Cmd控制碼</returns>
        protected override CmdObject GetInsertCmd(IEditable xNew)
        {
            CmdObject cmdRe = new CmdObject()
            {
                Type = CmdObject.CmdType.Insert,
                Cmd = "INSERT INTO CCDTTA(LOTSN,MPN,PARAM,STSN) VALUES (?,?,?,0)",
                Parameter = new OdbcParameter[]
                {
                    new OdbcParameter("@LOTSN", ((DB.TUC.CCDTA)xNew).LOTSN),//批次號
                    new OdbcParameter("@MPN", ((DB.TUC.CCDTA)xNew).MPN),//料號
                    new OdbcParameter("@PARAM", ((DB.TUC.CCDTA)xNew).PARAM),//參數編碼
                    new OdbcParameter("@STSN",((DB.TUC.CCDTA)xNew).STSN)//狀態碼
                }
            };
            return cmdRe;
        }
        /// <summary>
        /// 取得更新Cmd控制碼
        /// </summary>
        /// <param name="lsUnUsed">須更新列表</param>
        /// <param name="xOld">先前項目DB</param>
        /// <param name="xNew">更新項目DB</param>
        /// <returns>Cmd控制碼</returns>
        protected override CmdObject GetUpdateCmd(List<CCDTTA> lsUnUsed, IEditable xOld, IEditable xNew)
        {
            string strTable = "CCDTTA";
            if (lsUnUsed.Any(x => x.PDID == ((DB.TUC.CCDTA)xOld).PDID))
                strTable = "CCDTTA";
            else
                strTable = "CCDTTAUSED";

            CmdObject cmdRe = new CmdObject()
            {
                Type = CmdObject.CmdType.Update,
                Cmd = $"UPDATE {strTable} SET LOTSN=?, MPN=?,PARAM=? WHERE PDID=? ",
                Parameter = new OdbcParameter[]
                {
                    new OdbcParameter("@LOTSN", ((DB.TUC.CCDTA)xNew).LOTSN),
                    new OdbcParameter("@MPN", ((DB.TUC.CCDTA)xNew).MPN),
                    new OdbcParameter("@PARAM", ((DB.TUC.CCDTA)xNew).PARAM),
                    new OdbcParameter("@PDID", ((DB.TUC.CCDTA)xOld).PDID)
                }
            };
            return cmdRe;
        }
        /// <summary>
        /// 取得刪除Cmd控制碼
        /// </summary>
        /// <param name="xItem">刪除項目DB</param>
        /// <param name="eType">使用狀態</param>
        /// <returns>Cmd控制碼</returns>
        protected override CmdObject GetDeleteCmd(IEditable xItem, GridDataType eType)
        {
            CmdObject cmdRe = new CmdObject();
            if (eType == GridDataType.Used || eType == GridDataType.UnUsed)
            {
                string strTable = (eType == GridDataType.Used) ? "CCDTTAUSED" : "CCDTTA";
                cmdRe.Type = CmdObject.CmdType.Delete;
                cmdRe.Cmd = $"DECLARE @PDID int = ?; DELETE FROM {strTable} WHERE PDID=@PDID";
                cmdRe.Parameter = new OdbcParameter[]
                {
                    new OdbcParameter("@PDID", ((DB.TUC.CCDTA)xItem).PDID)
                };
            }
            return cmdRe;
        }
    }
}
