using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TUCBatchEditorCSharp.Helper;

namespace TUCBatchEditorCSharp
{
    class ODBCHelper
    {
        public static bool QueryData<T>(string strCon, string strCmd, ref List<T> lsRtn) where T : new()
        {
            lsRtn = new List<T>();
            try
            {
                OdbcConnection odbcCon = new OdbcConnection(strCon);

                OdbcDataAdapter odbcAdapter = new OdbcDataAdapter(strCmd, odbcCon);
                DataSet ds = new DataSet();
                odbcAdapter.Fill(ds);
                foreach (DataTable ta in ds.Tables)
                {
                    List<T> ls = ta.AsEnumerable().Select(x =>
                    {
                        T newObj = new T();
                        foreach (DataColumn col in x.Table.Columns)
                        {
                            foreach (var prop in typeof(T).GetProperties())
                            {
                                if (prop.Name == col.ColumnName && x.ItemArray[col.Ordinal].GetType() != typeof(DBNull))
                                {
                                    prop.SetValue(newObj, x.ItemArray[col.Ordinal], null);
                                    break;
                                }
                            }
                        }
                        return newObj;
                    }).ToList();
                    lsRtn.AddRange(ls);
                }
                odbcCon.Close();
                odbcAdapter.Dispose();
            }
            catch (System.Exception ex)
            {
                //log it
                LogHelper.Error(string.Format("QueryData Error. Cmd:{0}, error{1}", strCmd, ex.ToString()));
                return false;
            }
            return true;
        }
        public static void DeleteData(string strCon, string strCmd, OdbcParameter[] arParam)
        {
            ExecuteCmd("DeleteData", strCon, strCmd, arParam);
        }
        public static void InsertData(string strCon, string strCmd, OdbcParameter[] arParam)
        {
            ExecuteCmd("InsertData", strCon, strCmd, arParam);
        }
        public static void UpdateData(string strCon, string strCmd, OdbcParameter[] arParam)
        {
            ExecuteCmd("UpdateData", strCon, strCmd, arParam);
        }
        private static void ExecuteCmd(string strLogString, string strCon, string strCmd, OdbcParameter[] arParam)
        {
            string strLogCmd = strCmd;
            
            OdbcConnection odbcCon = new OdbcConnection(strCon);
            try
            {
                odbcCon.Open();
                OdbcCommand xCmd = new OdbcCommand(strCmd, odbcCon);
                foreach (var xParam in arParam)
                {
                    int nIndex = strLogCmd.IndexOf("?");
                    if (nIndex != -1)
                    {
                        strLogCmd = strLogCmd.Remove(nIndex, 1);
                        strLogCmd = strLogCmd.Insert(nIndex, xParam.Value.ToString());
                    }
                    xCmd.Parameters.Add(xParam);
                }
                xCmd.ExecuteNonQuery();
                LogHelper.Info(string.Format("{0}. Cmd:{1}", strLogString, strLogCmd));
            }
            catch (System.Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message);
#endif
                //log it
                LogHelper.Error(string.Format("{0} Error. Cmd:{1}, error{2}", strLogString, strLogCmd, ex.ToString()));
            }
            finally
            {
                if (odbcCon != null)
                {
                    try
                    {
                        odbcCon.Close();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(string.Format("{0} close error. Cmd:{1}, error{2}", strLogString, strLogCmd, ex.ToString()));
                    }
                }
            }
        }
        /// <summary>
        /// 資料庫屬性
        /// </summary>
        public enum DBProperty
        {
            /// <summary>
            /// 資料庫
            /// </summary>
            DataBase,
            /// <summary>
            /// 伺服器
            /// </summary>
            Server
        }
        public static string GetDBProperty(DBProperty eProperty, string strCon)
        {
            OdbcConnection odbcCon = new OdbcConnection(strCon);
            
            string strRtn = "";
            string strUpper = strCon.ToUpper(), strKey = "";
            switch (eProperty)
            {
                case DBProperty.Server:
                    strKey = "SERVER=";
                    break;
                case DBProperty.DataBase:
                default:
                    strKey = "DATABASE=";
                    break;
            }
            int nPos = strUpper.IndexOf(strKey);
            if (nPos != -1)
            {
                int nEnd = strUpper.IndexOf(";", nPos);
                if (nEnd == -1)
                    nEnd = strUpper.Length;
                int nStart = nPos + strKey.Length;
                strRtn = strCon.Substring(nStart, nEnd - nStart);
            }

            return strRtn;
        }
    }
}
