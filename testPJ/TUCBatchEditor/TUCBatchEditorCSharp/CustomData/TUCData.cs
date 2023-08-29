﻿using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using TUCBatchEditorCSharp.CustomData;

namespace TUCBatchEditorCSharp.CustomData
{
    /// <summary>
    /// 台耀Data
    /// </summary>
    class TUCData : DataManagerBase2<TUCBatchEditorCSharp.DB.TUC.CCDTA>
    {
        //參數基本檔
        List<DB.TUC.CCDBA> listInsp = new List<DB.TUC.CCDBA>();
        public TUCData(string strDBCon, IDataCallBack callback): base(strDBCon, callback)
        {

        }
        #region 實作抽象/虛擬
        /// <summary>
        /// 取得query string
        /// </summary>
        /// <param name="eType"></param>
        /// <returns></returns>
        protected override string GetQueryCmd(GridDataType eType)
        {
            string strRtn = "";
            switch (eType)
            {
                case GridDataType.UnUsed:
                    strRtn = "SELECT * FROM CCDTA1";
                    break;
                case GridDataType.Using:
                    strRtn = "SELECT * FROM CCDTA2 WHERE STSN=1";
                    break;
                case GridDataType.Used:
                    strRtn = "SELECT * FROM CCDTA2 WHERE STSN=2 ORDER BY STIME DESC";
                    break;
            }
            return strRtn;
        }
        /// <summary>
        /// 查詢CCDBAQuery(ALL)
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
        /// <summary>
        /// 取得檢測設定
        /// </summary>
        /// <returns></returns>
        public override List<string> GetInspList()
        {
            List<string> lsRtn = null;
            lock (lockQuery)
            {
                lsRtn = listInsp.Select(x => x.PARAM).ToList();
            }
            return lsRtn;
        }
        /// <summary>
        /// 取得insert指令物件
        /// </summary>
        /// <returns></returns>
        protected override CmdObject GetInsertCmd(DB.IEditable xNew)
        {
            CmdObject xCmd = new CmdObject();
            xCmd.Type = CmdObject.CmdType.Insert;
            xCmd.Cmd = "INSERT INTO CCDTA1(LOTSN,MPN,PARAM,STSN) VALUES (?,?,?,0)";
            xCmd.Parameter = new OdbcParameter[] { 
                new OdbcParameter("@LOTSN", ((DB.TUC.CCDTA)xNew).LOTSN),
                new OdbcParameter("@MPN", ((DB.TUC.CCDTA)xNew).MPN),
                new OdbcParameter("@PARAM", ((DB.TUC.CCDTA)xNew).PARAM),
            };
            return xCmd;
        }
        /// <summary>
        /// 取得update指令物件
        /// </summary>
        /// <returns></returns>
        protected override CmdObject GetUpdateCmd(List<TUCBatchEditorCSharp.DB.TUC.CCDTA> lsUnUsed, DB.IEditable xOld, DB.IEditable xNew)
        {
            CmdObject xCmd = new CmdObject();
            xCmd.Type = CmdObject.CmdType.Update;
            xCmd.Parameter = new OdbcParameter[] { 
                new OdbcParameter("@LOTSN", ((DB.TUC.CCDTA)xNew).LOTSN),
                new OdbcParameter("@MPN", ((DB.TUC.CCDTA)xNew).MPN),
                new OdbcParameter("@PARAM", ((DB.TUC.CCDTA)xNew).PARAM),
                new OdbcParameter("@PDID", ((DB.TUC.CCDTA)xOld).PDID)
            };

            string strTable = "CCDTA1";
            if (lsUnUsed.Any(x => x.PDID == ((DB.TUC.CCDTA)xOld).PDID))
                strTable = "CCDTA1";
            else
                strTable = "CCDTA2";

            xCmd.Cmd = string.Format("UPDATE {0} SET LOTSN=?, MPN=?,PARAM=? WHERE PDID=? ", strTable);
            return xCmd;
        }
        /// <summary>
        /// 取得update指令物件
        /// </summary>
        /// <returns></returns>
        protected override CmdObject GetDeleteCmd(DB.IEditable xItem, GridDataType eType)
        {
            CmdObject xCmd = new CmdObject();
            if (eType == GridDataType.Used || eType == GridDataType.UnUsed)
            {
                xCmd.Parameter = new OdbcParameter[] { 
                    new OdbcParameter("@PDID", ((DB.TUC.CCDTA)xItem).PDID)
                };
                xCmd.Type = CmdObject.CmdType.Delete;
                xCmd.Cmd = string.Format("DECLARE @PDID int = ?; DELETE FROM {0} WHERE PDID=@PDID", eType == GridDataType.Used ? "CCDTA2" : "CCDTA1");
            }
            return xCmd;
        }
        #endregion
    }
}
