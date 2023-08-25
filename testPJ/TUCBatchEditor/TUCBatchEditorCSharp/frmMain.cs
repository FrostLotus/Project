using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using TUCBatchEditorCSharp.Helper;

namespace TUCBatchEditorCSharp
{
    public interface IHandleRegistry
    {
        void AddHandle(Form xAdd);
        void RemoveHandle(IntPtr xRemove);
    }
    public partial class frmMain : Form, IHandleRegistry
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string sClass, string sWindow);
        [DllImport("User32.dll", EntryPoint = "IsWindowVisible")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
#if DEBUG
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);//取視窗title, debug用
#endif
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [DllImport("User32.dll", EntryPoint = "GetDesktopWindow")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("User32.dll", EntryPoint = "GetTopWindow")]
        private static extern IntPtr GetTopWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("User32.dll", EntryPoint = "GetWindowLong")]
        private static extern long GetWindowLong(IntPtr hWnd,int  nIndex);

        const uint GW_HWNDPREV = 3;
        const uint GW_HWNDNEXT = 2;
        const uint GW_HWNDLAST = 1;
        const uint GW_HWNDFIRST = 0;
        const int GWL_STYLE = -16;
        const Int64 WS_MINIMIZE = 0x20000000L;
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        public enum SystemMetric
        {
            SM_CXSCREEN = 0,  // 0x00
            SM_CYSCREEN = 1,  // 0x01
            SM_CXVSCROLL = 2,  // 0x02
            SM_CYHSCROLL = 3,  // 0x03
            SM_XVIRTUALSCREEN = 76,
            SM_YVIRTUALSCREEN = 77,
            SM_CXVIRTUALSCREEN = 78,
            SM_CYVIRTUALSCREEN = 79,
            SM_CMONITORS = 80,
            SM_REMOTECONTROL = 0x2001, // 0x2001
        }
        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(SystemMetric smIndex);
        private bool IsAOIMode { get; set; } //AOI 模式, 將AOI編輯工單view蓋住
        private List<frmBatchView> m_lsBatchView = new List<frmBatchView>(); //AOI模式下支援多螢幕顯示
        enum HandleType
        {
            Main,
            Form,
            EditForm
        }
        class HandleObject
        {
            public HandleObject(HandleType e, Form xCtrl)
            {
                eType = e;
                this.xCtrl = xCtrl;
            }
            public HandleType eType;
            public Form xCtrl;
        }
        private List<HandleObject> m_lsHandle = new List<HandleObject>(); //登記handle, 避免ui移到下方
        private bool m_Aoi_ShowHide { get; set; }

        private Timer m_TopMostTimer;
        public frmMain()
        {
            InitializeComponent();
        }
        private bool NeedMoveWindow(Form xForm)
        {
            //Get the top-most window in the desktop
            if(xForm.Visible)
            {
                IntPtr window = GetTopWindow(GetDesktopWindow());
                do
                {
                    if (IsWindowVisible(window))
                    {
                        RECT rct = new RECT();
                        GetWindowRect(window, ref rct);
                        Rectangle rcWnd = new Rectangle(rct.Left, rct.Top, (rct.Right - rct.Left), (rct.Bottom - rct.Top));
                        Rectangle rcForm = new Rectangle(xForm.Location.X, xForm.Location.Y, xForm.Size.Width, xForm.Size.Height);
                        if (rcForm.IntersectsWith(rcWnd) /*&& !m_lsHandle.Any(x=> x.xHandle == window)*/) //find overlap window
                        {
                            if (window == Handle || m_lsHandle.Any(x => x.xCtrl.IsDisposed == false && x.xCtrl.Handle == window))
                            {
                                Console.WriteLine("skip move");
                                break;
                            }
                            else
                            {
                                StringBuilder sb = new StringBuilder(1024);
                                if(GetClassName(window, sb, sb.Capacity) > 0 && sb.ToString() == "ComboLBox")
                                {
                                    Console.WriteLine("special case:skip combobox selecting case)"); 
                                    break;
                                }
                                if (GetClassName(window, sb, sb.Capacity) > 0 && sb.ToString().ToUpper() == "STATIC")
                                {
                                    Console.WriteLine("special case:skip CWebEdgeInfo dlg)");
                                    break;
                                }
#if DEBUG
                                const int nChars = 256;
                                System.Text.StringBuilder Buff = new System.Text.StringBuilder(nChars);
                                if (GetWindowText(window, Buff, nChars) > 0) //for testing log
                                {
                                    Console.WriteLine(string.Format("window {0} z order higher than {1}", Buff.ToString(), xForm.Text));
                                }
#endif
                                return true;
                            }
                        }
                    }
                    window = GetWindow(window, GW_HWNDNEXT);
                } while (window != IntPtr.Zero);
            }
            return false;
        }
        private void TopmostEditForm(Form xForm)
        {
            Rectangle rcForm = new Rectangle(xForm.Left, xForm.Top, xForm.Width, xForm.Height);
            m_lsHandle.Where(x => {
                Rectangle rcEdit = new Rectangle(x.xCtrl.Left, x.xCtrl.Top, x.xCtrl.Width, x.xCtrl.Height);
                return x.eType == HandleType.EditForm && rcForm.IntersectsWith(rcEdit);
            }).ToList().ForEach(x => x.xCtrl.TopMost = true); //move edit form to top 
        }
        private void TopMostTimer_Tick(object sender, EventArgs e)
        {
            int nIndex = 0;
            //check show hide and current status
            int nWinHandle = FindWindow(null, "AOI Master");
            //RECT rc = new RECT();
            long nWndStyle = GetWindowLong((IntPtr)nWinHandle, GWL_STYLE);
            if ((nWndStyle & WS_MINIMIZE) == 0) //show
            {
                if (m_Aoi_ShowHide)
                {
                    if(this.Visible == false)
                        this.Show();
                }

                nIndex = 0;
                foreach (var xView in m_lsBatchView)
                {
                    if (nIndex != 0)
                    {    //skip first(it contains in main form
                        if (xView.Get_AOI_ShowHide() && xView.Visible == false)
                        {
                            xView.Show();
                        }
                    }
                    nIndex++;
                }
            }
            else //hide all
            {
                if (this.Visible)
                {
                    this.Hide();
                }
                nIndex = 0;
                foreach (var xView in m_lsBatchView)
                {
                    if (nIndex != 0)
                    {    //skip first(it contains in main form
                        if (xView.Visible)
                        {
                            xView.Hide();
                        }
                    }
                    nIndex++;
                }
                return;
            }
            List<IntPtr> lsWindows = new List<IntPtr>();
#if DEBUG
            const int nChars = 256;
            System.Text.StringBuilder Buff = new System.Text.StringBuilder(nChars);
#endif
            Rectangle rcThis = new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height);

            if (NeedMoveWindow(this))//set top most again
            {
                Console.WriteLine("top most again");
                this.TopMost = true;
                TopmostEditForm(this);
            }
            nIndex = 0;
            foreach(var xView in m_lsBatchView)
            {
                if (nIndex != 0)
                {    //skip first(it contains in main form
                    if (NeedMoveWindow(xView))
                    {
                        Console.WriteLine("top most view again");
                        xView.TopMost = true;
                        TopmostEditForm(xView);
                    }
                }
                nIndex++;
            }
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            m_Aoi_ShowHide = false;
            this.TopMost = true;
            Form xForm = null;
            string strCon = Properties.Settings.Default.DefaultDB;

            #region for top most
            m_TopMostTimer = new Timer();
#if DEBUG
            m_TopMostTimer.Interval = 3000;
#else
            m_TopMostTimer.Interval = 300;
#endif
            m_TopMostTimer.Tick += TopMostTimer_Tick;
            m_TopMostTimer.Start();
            m_lsHandle.Add(new HandleObject(HandleType.Main, this));
            #endregion

            this.Size = new Size(795, 900);
            IsAOIMode = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            const int ctUINT_FIX_WIDTH = 1920;
            int nLeft = GetSystemMetrics(SystemMetric.SM_XVIRTUALSCREEN);
            int nTop = GetSystemMetrics(SystemMetric.SM_YVIRTUALSCREEN);
            int nRight = GetSystemMetrics(SystemMetric.SM_CXVIRTUALSCREEN);
            int nBottom = GetSystemMetrics(SystemMetric.SM_CYVIRTUALSCREEN);
            Rectangle rcFull = new Rectangle(nLeft, nTop, nRight - nLeft, nBottom - nTop);

            int nW = rcFull.Right - rcFull.Left;
            int nUnit = nW / ctUINT_FIX_WIDTH;
            int nHeight = 62;
#if DEBUG
            nHeight = 78;
#endif
            string[] args = Environment.GetCommandLineArgs();
            AOI_CUSTOMERTYPE_ eCustomerType = AOI_CUSTOMERTYPE_.CUSTOMER_TUC_PP;//default tuc
            if (args.Length >= 2)
            {
                int nTemp = 0;
                if(int.TryParse(args[1], out nTemp))
                {
                   eCustomerType = (AOI_CUSTOMERTYPE_)nTemp;
                }
            }
            for (int i = 0; i < nUnit; i++)
            {
                frmBatchView xFormTmp = new frmBatchView(strCon, this, eCustomerType);
                //for top most
                m_lsHandle.Add(new HandleObject(HandleType.Form, xFormTmp));

                Point xPt = new Point(0, 0);
                if (i == 0)
                {
                    xPt = new Point(135 + 1000, nHeight);
                    xForm = xFormTmp;
                    this.Location = xPt;
                }
                else
                {
                    xPt = new Point(i * ctUINT_FIX_WIDTH + 1035, nHeight);
                    xFormTmp.Size = new Size(849, 900);
                }
                xFormTmp.Show();
                xFormTmp.Location = xPt; 
                xFormTmp.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                xFormTmp.TopLevel = true;
                xFormTmp.TopMost = true;
                m_lsBatchView.Add(xFormTmp);
                xFormTmp.Hide();
            }

            
            if (xForm != null)
            {
                xForm.TopLevel = false;
                xForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                xForm.Visible = true;
                xForm.Dock = DockStyle.Fill;
                this.Controls.Add(xForm);
            }
        }
        private bool bInit = false;
        private void frmMain_Shown(object sender, EventArgs e)
        {
#if DEBUG
            bInit = true;
#endif
            if (IsAOIMode && bInit == false) //AOI模式要預設隱藏
            {
                this.Hide();
                bInit = true;
            }
        }
        public const int WM_TUC_MSG = 0x8000 + 2000;
        public const int WM_TUC_SHOW_CMD = 0x8000 + 2001;//
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_TUC_MSG)
            {
                if ((m.WParam.ToInt32() == WM_TUC_SHOW_CMD))
                {
                    int nData = m.LParam.ToInt32();
                    for (int i = 0; i < m_lsBatchView.Count; i++ )
                    {
                        bool bShow = (nData & (1 << i)) > 0;
                        if(i == 0) 
                        {
                            m_Aoi_ShowHide = bShow;
                            if (bShow)
                            {
                                this.Show();
                            }
                            else
                                this.Hide();
                        }
                        else
                        {
                            m_lsBatchView[i].Set_AOI_ShowHide(bShow);
                            if (bShow)
                            {
                                m_lsBatchView[i].Show();
                            }
                            else
                                m_lsBatchView[i].Hide();
                        }
                        if (!bShow)
                        {
                            m_lsBatchView[i].CloseEditWindow();
                        }
                    }
                }
            }
            else
                base.WndProc(ref m);
        }
        public void AddHandle(Form xAdd)
        {
            m_lsHandle.Add(new HandleObject(HandleType.EditForm, xAdd));
        }
        public void RemoveHandle(IntPtr xRemove)
        {
            foreach(HandleObject xPtr in m_lsHandle)
            {
                if (xPtr.xCtrl.IsDisposed == false && xPtr.xCtrl.Handle == xRemove)
                {
                    m_lsHandle.Remove(xPtr);
                    Console.WriteLine("remove handle");
                    break;
                }
            }
            foreach(HandleObject xPtr in m_lsHandle)
            {
                if(xPtr.xCtrl.IsDisposed)
                {
                    m_lsHandle.Remove(xPtr);
                    break;
                }
            }
        }
        
    }
}
