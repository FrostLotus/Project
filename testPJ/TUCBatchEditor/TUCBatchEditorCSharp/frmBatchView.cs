#define EDITFORM_DOMODAL //避免編輯視窗被隱藏
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TUCBatchEditorCSharp.CustomData;
using TUCBatchEditorCSharp.Helper;
namespace TUCBatchEditorCSharp
{
    public enum AOI_CUSTOMERTYPE_
    { 
        CUSTOMER_NONE = 0,
        CUSTOMER_NANYA = 2,			  //台塑南亞
        CUSTOMER_SYST_WEB_COPPER = 3, //生益軟板
        CUSTOMER_SYST_CCL = 4,		  //生益CCL
        CUSTOMER_NANYA_WARPING = 5,	  //南亞整經機
        CUSTOMER_SYST_PP = 6,		  //生益PP
        CUSTOMER_SCRIBD_PP = 7,		  //宏仁 PP
        CUSTOMER_EMC_PP = 8,		  //台光 PP
        CUSTOMER_ITEQ = 9,            //聯茂
        CUSTOMER_JIANGXI_NANYA = 10,  //江西南亞CCL
        CUSTOMER_TUC_PP = 11,		  //台耀 PP
        CUSTOMER_TG = 12,			  //台玻
        CUSTOMER_YINGHUA = 13,		  //盈華
    };
    public partial class frmBatchView : Form, IDataCallBack
    {
        /// <summary>
        /// 批次資料庫控制視窗控制項建立
        /// </summary>
        /// <param name="strDBCon">資料庫控制碼</param>
        /// <param name="xReg">控制項登記</param>
        /// <param name="eType">客製化用戶類型</param>
        public frmBatchView(string strDBCon, IHandleRegistry xReg, AOI_CUSTOMERTYPE_ eType)
        {
            //default use TUC mode
            switch (eType)
            {
                case AOI_CUSTOMERTYPE_.CUSTOMER_YINGHUA:
                    dataManager = new TUCBatchEditorCSharp.CustomData.YINGHUAData(strDBCon, this);
                    break;
                case AOI_CUSTOMERTYPE_.CUSTOMER_TUC_PP:
                default:
                    //這邊就透過TUCData中已實作有的Callback每一次執行緒的監聽
                    dataManager = new TUCBatchEditorCSharp.CustomData.TUCData(strDBCon, this);
                    break;
            }
            this.strDBCon = strDBCon;
            m_Aoi_ShowHide = false;
            m_xReg = xReg;
            m_DBName = ODBCHelper.GetDBProperty(ODBCHelper.DBProperty.DataBase, strDBCon);
            m_Server = ODBCHelper.GetDBProperty(ODBCHelper.DBProperty.Server, strDBCon);
            InitializeComponent();
        }
        //-----------------
        /// <summary>
        /// 尋找視窗控制項
        /// </summary>
        /// <param name="lpClassName">指定視窗類別名稱(通常為Null)</param>
        /// <param name="lpWindowName">視窗名稱</param>
        /// <returns>控制項代碼</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindowA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr FindWindowA(string lpClassName, string lpWindowName);
        /// <summary>
        /// 傳送指定視窗資料
        /// </summary>
        /// <param name="hWnd">指定視窗代碼</param>
        /// <param name="msg">訊息類別</param>
        /// <param name="wParam">訊息細項(代條件)</param>
        /// <param name="lParam">資料</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "PostMessage", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        //-----------------

        private DataManagerBase dataManager = null;
        private Rectangle _rcLight = new Rectangle(25, 660, 20, 20);
        private DataGridView m_LastSelGridView = null;
        private frmAddnEdit m_xEdidForm = null;

        private DB.IEditable m_lastCreate = null;
        private DB.IEditable m_CurSel = null;
        private IHandleRegistry m_xReg = null;

        private bool _DBStatus = false;
        private string m_DBName;//資料庫名稱
        private string m_Server;//伺服器名稱
        
        private bool m_Aoi_ShowHide { get; set; }
        private string strDBCon { get; set; }

        const int WM_CLOSE = 0x10;
        //-----------------------------------------
        
        private void frmBatchView_Load(object sender, EventArgs e)
        {
            CreateEditForm();
            InitGrid();

            dataManager.StartQueryThread();
        }
        private void CreateEditForm()
        {
            m_xEdidForm = new frmAddnEdit();
            m_xEdidForm.OnFinishEdit += this.OnFinishEdit;
            m_xEdidForm.OnCancelEdit += this.OnCancelEdit;
            m_xEdidForm.TopMost = true;//最上最大化
            if (m_xReg != null) m_xReg.AddHandle(m_xEdidForm);
            Console.WriteLine(string.Format("add {0}", m_xEdidForm.Handle));
        }
        private void InitGrid()
        {
            //init gridview
            var xList = dataManager.GetColumnType().GetProperties()
                .Select(x => x.GetCustomAttributes(typeof(DB.FieldInfoAttribute), false).Select(y => new KeyValuePair<string, DB.FieldInfoAttribute>(x.Name, (DB.FieldInfoAttribute)y))
                    .FirstOrDefault())
                .Where(x => x.Value != null && x.Value.Show)
                .ToList();
            List<DataGridView> lsGrid = new List<DataGridView>() { dgvUnUsed, dgvUsing, dgvUsed };
            lsGrid.ForEach(x => { x.Rows.Clear(); x.Columns.Clear(); });
            foreach (var xCol in xList)
            {
                lsGrid.ForEach(x => x.Columns.Add(xCol.Key, xCol.Value.Name));
            }
        }

        /// <summary>
        /// 實際Callback執行的循環動作
        /// </summary>
        /// <param name="eType">使用狀態</param>
        /// <param name="nCount"></param>
        /// <param name="bSuccess"></param>
        public void OnQueryThreadCallBack(GridDataType eType, int nCount, bool bSuccess)
        {
            //顯示目前CallBack的時間與目標
            UpdateLabelText(lbl_Status, string.Format("Server:{0} DB:{1}, UpdateTime:{2}", m_Server, m_DBName, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));

            if (_DBStatus != bSuccess)//
            {
                _DBStatus = bSuccess;
                Invalidate(_rcLight);//塊狀範圍失效
                UpdateLabelText(lbl_DBStatus, _DBStatus ? "資料庫連線正常" : "資料庫連線異常");
            }
            switch (eType)
            {
                case GridDataType.UnUsed:
                    UpdateGridViewCount(dgvUnUsed, nCount);
                    break;
                case GridDataType.Using:
                    UpdateGridViewCount(dgvUsing, nCount);
                    break;
                case GridDataType.Used:
                    UpdateGridViewCount(dgvUsed, nCount);
                    break;
            }
            
        }
        /// <summary>
        /// 更新dataGridView文字
        /// </summary>
        /// <param name="dgv">指定dataGridView</param>
        /// <param name="nCount">dataGridView所要更新的列數</param>
        private void UpdateGridViewCount(DataGridView dgv, int nCount)
        {
            if (dgv.InvokeRequired)
            {
                Action safeWrite = delegate { UpdateGridViewCount(dgv, nCount); };
                dgv.Invoke(safeWrite);
            }
            else
            {
                dgv.RowCount = nCount;
                //應該比對差異, 再各自更新欄位. 開發時間不足尚未修改
                dgv.Invalidate();//強制刷寫
            }
        }
        /// <summary>
        /// 更新標籤文字
        /// </summary>
        /// <param name="xLbl">指定標籤控制項</param>
        /// <param name="strText">輸出文字</param>
        private void UpdateLabelText(Label xLbl, string strText)
        {
            if(xLbl.InvokeRequired)//若需要用Invoke方式更新標籤
            {
                Action safeWrite = delegate { UpdateLabelText(xLbl, strText); };
                xLbl.Invoke(safeWrite);
            }
            else//直接更新
            {
                xLbl.Text = strText;
            }
        }
        private void frmBatchView_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillEllipse(new SolidBrush(_DBStatus ? Color.Green : Color.Red), _rcLight);
        }
        private void dgv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dataManager != null)
            {
                DB.IEditable Batch = dataManager.GetBatchObject(GetGridViewType(dgv), e.RowIndex);
                // Set the cell value to paint using the Customer object retrieved.
                foreach (var prop in Batch.GetType().GetProperties())
                {
                    if (prop.Name == dgv.Columns[e.ColumnIndex].Name)
                    {
                        e.Value = prop.GetValue(Batch, null);
                        break;
                    }
                }
            }
        }
        private GridDataType GetGridViewType(DataGridView dgv)
        {
            GridDataType eType = GridDataType.UnUsed;
            if (dgv == dgvUnUsed)
            {
                eType = GridDataType.UnUsed;
            }
            else if (dgv == dgvUsing)
            {
                eType = GridDataType.Using;
            }
            else if (dgv == dgvUsed)
            {
                eType = GridDataType.Used;
            }
            return eType;
        }
        private void btn_Click(object sender, EventArgs e)
        {
            if(sender == btnAdd || sender == btnEdit)
            {
                if (!m_xEdidForm.Visible)
                {
                    if (m_xEdidForm.IsDisposed)
                    {
                        CreateEditForm();
                    }

                    List<KeyValuePair<string, List<string>>> lsCombo = new List<KeyValuePair<string, List<string>>>();
                    lsCombo.Add(new KeyValuePair<string, List<string>>("PARAM", dataManager.GetInspList()));
                    if (sender == btnAdd)
                    {
                        if (m_lastCreate != null)
                            m_xEdidForm.SetEditParam(frmAddnEdit.FormType.Add, m_lastCreate, lsCombo);
                        else
                            m_xEdidForm.SetEditParam(frmAddnEdit.FormType.Add, Activator.CreateInstance(dataManager.GetColumnType()) as DB.IEditable, lsCombo);
                    }
                    else if (sender == btnEdit)
                    {
                        if (m_CurSel == null) return;
                        m_xEdidForm.SetEditParam(frmAddnEdit.FormType.Edit, m_CurSel, lsCombo);
                    }
#if EDITFORM_DOMODAL
                    m_xEdidForm.ShowDialog();
                    m_xEdidForm.Dispose();
#else
                    m_xEdidForm.Show();
#endif
                }
            }
            else if (sender == btnRemove)
            {
                if (m_CurSel != null && (m_LastSelGridView == dgvUsed ||m_LastSelGridView == dgvUnUsed) /*執行中不可刪除*/)
                {
                    if (MessageBox.Show(string.Format("警告!確認要移除({0})工單?", m_CurSel.GetName()), "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                    {
                        if (m_LastSelGridView == dgvUsed)
                            dataManager.OnDelete(m_CurSel, GridDataType.Used);
                        else if (m_LastSelGridView == dgvUnUsed)
                            dataManager.OnDelete(m_CurSel, GridDataType.UnUsed);

                        LogHelper.Info(string.Format("delete id {0} name {1}", m_CurSel.GetKey(), m_CurSel.GetName()));
                    }
                }
            }
        }
        private void dgv_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (e.StateChanged == DataGridViewElementStates.Selected)
            {
                if (m_LastSelGridView != dgv)
                {
                    if (m_LastSelGridView != null)
                    {
                        m_LastSelGridView.ClearSelection();
                    }
                    m_LastSelGridView = dgv;
                    btnEdit.Enabled = (dgv == dgvUnUsed || dgv == dgvUsed);
                }
                m_CurSel = dataManager.GetBatchObject(GetGridViewType(dgv), e.Row.Index);
            }
        }
        private void OnCancelEdit()
        {
            if(m_xReg != null)
            {
                m_xReg.RemoveHandle(m_xEdidForm.Handle);
            }
        }
        private void OnFinishEdit(frmAddnEdit.FormType eType, DB.IEditable xOld, DB.IEditable xNew)
        {
            dataManager.OnFinishEdit(eType, xOld, xNew);
            if (eType == frmAddnEdit.FormType.Add)
                m_lastCreate = xNew;
            if (m_xReg != null)
            {
                m_xReg.RemoveHandle(m_xEdidForm.Handle);
            }
        }
        public void Set_AOI_ShowHide(bool bShow)
        {
            m_Aoi_ShowHide = bShow;
        }
        public void CloseEditWindow()
        {
            if (!m_xEdidForm.IsDisposed)
            {
                IntPtr ptr = FindWindowA(null, "frmAddnEdit");
                if (ptr != IntPtr.Zero)
                {
                    PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    if (m_xReg != null)
                    {
                        m_xReg.RemoveHandle(m_xEdidForm.Handle);
                    }
                }
            }
        }
        public bool Get_AOI_ShowHide() 
        { 
            return m_Aoi_ShowHide;
        }
    }
}
