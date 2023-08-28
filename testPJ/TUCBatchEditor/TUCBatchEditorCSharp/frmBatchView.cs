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
        CUSTOMER_NANYA = 2,			 //台塑南亞
        CUSTOMER_SYST_WEB_COPPER = 3, //生益軟板
        CUSTOMER_SYST_CCL = 4,		 //生益CCL
        CUSTOMER_NANYA_WARPING = 5,	 //南亞整經機
        CUSTOMER_SYST_PP = 6,		 //生益PP
        CUSTOMER_SCRIBD_PP = 7,		 //宏仁 PP
        CUSTOMER_EMC_PP = 8,		 //台光 PP
        CUSTOMER_ITEQ = 9,           //聯茂
        CUSTOMER_JIANGXI_NANYA = 10, //江西南亞CCL
        CUSTOMER_TUC_PP = 11,		 //台耀 PP
        CUSTOMER_TG = 12,			 //台玻
        CUSTOMER_YINGHUA = 13,			 //盈華
    };
    public partial class frmBatchView : Form, IDataCallBack
    {
        private DataManagerBase dataManager = null;
        private Rectangle _rcLight = new Rectangle(25, 660, 20, 20);
        bool _DBStatus = false;
        string m_DBName;//資料庫名稱
        string m_Server;//伺服器名稱
        private DB.IEditable m_lastCreate = null;
        DataGridView m_LastSelGridView = null;
        frmAddnEdit m_xEdidForm = null;
        private DB.IEditable m_CurSel = null;
        private IHandleRegistry m_xReg = null;
        private bool m_Aoi_ShowHide { get; set; }
        private string strDBCon { get; set; }
        //-----------------------------------------
        /// <summary>
        /// 批次處理視窗顯示
        /// </summary>
        /// <param name="strDBCon">資料庫對應名稱</param>
        /// <param name="xReg">控制項設置</param>
        /// <param name="eType">預設版型</param>
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
        public void OnQueryThreadCallBack(GridDataType eType, int nCount, bool bSuccess)
        {
            UpdateLabelText(lblStatus, string.Format("Server:{0} DB:{1}, UpdateTime:{2}", m_Server, m_DBName, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));

            if (_DBStatus != bSuccess)
            {
                _DBStatus = bSuccess;
                Invalidate(_rcLight);
                UpdateLabelText(lblDBStatus, _DBStatus ? "資料庫連線正常" : "資料庫連線異常");
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
                dgv.Invalidate();
            }
        }
        private void UpdateLabelText(Label xLbl,  string strText)
        {
            if(xLbl.InvokeRequired)
            {
                Action safeWrite = delegate { UpdateLabelText(xLbl, strText); };
                xLbl.Invoke(safeWrite);
            }
            else
            {
                xLbl.Text = strText;
            }
        }

        private void CreateEditForm()
        {
            m_xEdidForm = new frmAddnEdit();
            m_xEdidForm.OnFinishEdit += this.OnFinishEdit;
            m_xEdidForm.OnCancelEdit += this.OnCancelEdit;
            m_xEdidForm.TopMost = true;
            if(m_xReg != null) m_xReg.AddEditHandle(m_xEdidForm);
            Console.WriteLine(string.Format("add {0}", m_xEdidForm.Handle));
        }
        private void frmBatchView_Load(object sender, EventArgs e)
        {
            CreateEditForm();
            InitGrid();

            dataManager.StartQueryThread();
        }
        private void frmBatchView_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillEllipse(new SolidBrush(_DBStatus ? Color.Green : Color.Red), _rcLight);
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
                lsGrid.ForEach(x=> x.Columns.Add(xCol.Key, xCol.Value.Name));
            }
        }
        private void dgv_CellValueNeeded(object sender, System.Windows.Forms.DataGridViewCellValueEventArgs e)
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
        [DllImport("user32.dll", EntryPoint = "FindWindowA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "PostMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        const int WM_CLOSE = 0x10;

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
                IntPtr ptr = FindWindow(null, "frmAddnEdit");
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
        public bool Get_AOI_ShowHide() { return m_Aoi_ShowHide;}
    }
}
