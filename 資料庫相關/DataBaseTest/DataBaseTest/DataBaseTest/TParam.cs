﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;

namespace DataBaseTest
{
    class TParam
    {
        /// <summary>
        /// SqlConnection連接字串內容
        /// </summary>
        public static SqlConnectionStringBuilder sqlConSB;
        /// <summary>
        /// 資料庫原位址
        /// </summary>
        public static string sDataSource = "";//"AOI-142\\SQLEXPRESS";
        /// <summary>
        /// 指定資料庫名稱
        /// </summary>
        public static string sInitialCatalog = "";// = "MVC_TestDB";
        /// <summary>
        /// 使用者ID
        /// </summary>
        public static string sUserID = "";//"AOI";
        /// <summary>
        /// 使用者ID對應密碼
        /// </summary>
        public static string sPassword = "";//"aoi0817";
        /// <summary>
        /// 
        /// </summary>
        public static bool bIntegrated_Security = true;
        //-------------------------------------------
        /// <summary>
        /// 選取資料表容器
        /// </summary>
        public static DataSet SQLDataSet;
        /// <summary>
        /// [TEST]將資料庫與資料表包為樹狀結構
        /// </summary>
        public static System.Windows.Forms.TreeView treeView;
        /// <summary>
        /// [TEST]將資料庫與資料表包為巢狀結構
        /// </summary>
        public static List<List<string>> sFullDataSet;//之後若改為不同資料表需要靠List方式來鍵入所有資料表名稱
        /// <summary>
        /// 資料庫名稱列表
        /// </summary>
        public static List<string> sDataSet;//之後若改為不同資料表需要靠List方式來鍵入所有資料表名稱
        /// <summary>
        /// 對應資料庫之資料表名稱列表
        /// </summary>
        public static List<string> sTable;//之後若改為不同資料表需要靠List方式來鍵入所有資料表名稱
        /// <summary>
        /// 修改前列數
        /// </summary>
        public static int iPreRowCount;
        /// <summary>
        /// 
        /// </summary>
        public static string sPreDataTable;

        //目前先如下
        public static string sTable_Customers = "Table_Customers";
        public static string sTable_Orderdetail = "Table_Orderdetial";
        public static string sTable_Orders = "Table_Orders";
        public static string sTable_Product = "Table_Products";
        //=================================================================
        public static void Init()
        {
            SQLDataSet = new DataSet();
            treeView = new TreeView();
            sFullDataSet = new List<List<string>>();
            sDataSet = new List<string>();
            sTable = new List<string>();

            iPreRowCount = 0;
            sPreDataTable = "";
        }
        /// <summary>
        /// SqlConnectionStringBuilder設定
        /// </summary>
        /// SqlConnectionStringBuilder
        /// <param name="ConSB"></param>
        public static void Build_SqlConSB()
        {
            //基礎型
            sqlConSB = new SqlConnectionStringBuilder
            {
                DataSource = sDataSource,

                UserID = sUserID,
                Password = sPassword,
                IntegratedSecurity = true
            };
        }
        /// <summary>
        /// SqlConnectionStringBuilder設定(帶InitialCatalog)
        /// </summary>
        /// <param name="sInitialCatalog">InitialCatalog</param>
        public static void Build_SqlConSB(string sInitialCatalog)
        {
            //帶InitialCatalog
            sqlConSB = new SqlConnectionStringBuilder
            {
                DataSource = sDataSource,
                InitialCatalog = sInitialCatalog,
                UserID = sUserID,
                Password = sPassword,
                IntegratedSecurity = true
            };
        }
        public static void CopyTreeNodes(TreeNodeCollection sourceNodes, TreeNodeCollection targetNodes)
        {
            foreach (TreeNode sourceNode in sourceNodes)
            {
                TreeNode newNode = (TreeNode)sourceNode.Clone();
                targetNodes.Add(newNode);

                // 遞迴複製子節點
                //CopyTreeNodes(sourceNode.Nodes, newNode.Nodes);
            }
        }

    }

}
