using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

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
        public static string sInitialCatalog="";// = "MVC_TestDB";
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
        //目前先如下
        public static string sTable_Customers = "Table_Customers";
        public static string sTable_Orderdetail = "Table_Orderdetial";
        public static string sTable_Orders = "Table_Orders";
        public static string sTable_Product = "Table_Products";
        //=================================================================
        public static void Init()
        {
            treeView = new TreeView();
            sFullDataSet = new List<List<string>>();
            sDataSet = new List<string>();
            sTable = new List<string>();
            
        }
        public static void Build_SqlConSB()
        {
            sqlConSB = new SqlConnectionStringBuilder
            {
                //建立之系統OK
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
