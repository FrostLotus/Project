using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using TUCBatchEditorCSharp.CustomData;

namespace TUCBatchEditorCSharp.CustomData
{
    /// <summary>
    /// 盈華Data
    /// </summary>
    class YINGHUAData : DataManagerBase2<TUCBatchEditorCSharp.DB.YINGHUA.CCDTA>
    {
        public YINGHUAData(string strDBCon, IDataCallBack callback)
            : base(strDBCon, callback)
        {

        }
        #region 實作抽象
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
        /// 取得insert指令物件
        /// </summary>
        /// <returns></returns>
        protected override CmdObject GetInsertCmd(DB.IEditable xNew)
        {
            CmdObject xCmd = new CmdObject();
            xCmd.Type = CmdObject.CmdType.Insert;
            xCmd.Cmd = "INSERT INTO CCDTA1(PNAME,LOTSN,MPN,STSN) VALUES (?,?,?,0)";
            xCmd.Parameter = new OdbcParameter[] { 
                new OdbcParameter("@PNAME", ((DB.YINGHUA.CCDTA)xNew).PNAME),
                new OdbcParameter("@LOTSN", ((DB.YINGHUA.CCDTA)xNew).LOTSN),
                new OdbcParameter("@MPN", ((DB.YINGHUA.CCDTA)xNew).MPN),
            };
            return xCmd;
        }
        /// <summary>
        /// 取得檢測設定
        /// </summary>
        /// <returns></returns>
        public override List<string> GetInspList()
        {
            return new List<string>();
        }
        /// <summary>
        /// 取得update指令物件
        /// </summary>
        /// <returns></returns>
        protected override CmdObject GetUpdateCmd(List<TUCBatchEditorCSharp.DB.YINGHUA.CCDTA> lsUnUsed, DB.IEditable xOld, DB.IEditable xNew)
        {
            CmdObject xCmd = new CmdObject();
            xCmd.Type = CmdObject.CmdType.Update;
            xCmd.Parameter = new List<OdbcParameter>() { 
                new OdbcParameter("@PNAME", ((DB.YINGHUA.CCDTA)xNew).PNAME),
                new OdbcParameter("@LOTSN", ((DB.YINGHUA.CCDTA)xNew).LOTSN),
                new OdbcParameter("@MPN", ((DB.YINGHUA.CCDTA)xNew).MPN),
                new OdbcParameter("@PDID", ((DB.YINGHUA.CCDTA)xOld).PDID)
            }.ToArray();

            string strTable = "CCDTA1";
            if (lsUnUsed.Any(x => x.PDID == ((DB.YINGHUA.CCDTA)xOld).PDID))
                strTable = "CCDTA1";
            else
                strTable = "CCDTA2";

            xCmd.Cmd = string.Format("UPDATE {0} SET PNAME=?, LOTSN=?, MPN=? WHERE PDID=? ", strTable);
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
                    new OdbcParameter("@PDID", ((DB.YINGHUA.CCDTA)xItem).PDID)
                };
                xCmd.Type = CmdObject.CmdType.Delete;
                xCmd.Cmd = string.Format("DECLARE @PDID int = ?; DELETE FROM {0} WHERE PDID=@PDID", eType == GridDataType.Used ? "CCDTA2" : "CCDTA1");
            }
            return xCmd;
        }
        #endregion
    }
}
