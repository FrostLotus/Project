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
    /// <summary>
    /// 控制項註冊
    /// </summary>
    public interface IHandleRegistry
    {
        /// <summary>
        /// 新增控制項
        /// </summary>
        /// <param name="xAdd">目標Form</param>
        void AddHandle(Form xAdd);
        /// <summary>
        /// 移除控制項
        /// </summary>
        /// <param name="xRemove">控制項碼</param>
        void RemoveHandle(IntPtr xRemove);
    }
    public partial class frmMain : Form, IHandleRegistry
    {
        public frmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 搜尋視窗
        /// </summary>
        /// <param name="sClassName">控制項名稱</param>
        /// <param name="sWindowName">視窗名稱</param>
        /// <returns>回值是具有指定類別名稱和視窗名稱之視窗的控制碼,反之,則為NULL</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string sClassName, string sWindowName);
        /// <summary>
        /// 視窗的可見度狀態
        /// </summary>
        /// <param name="hWnd">視窗控制碼</param>
        /// <returns>指定的視窗、其父視窗、其父視窗等具有 WS_VISIBLE 樣式，則傳回值為非零,反之,則為零</returns>
        [DllImport("User32.dll", EntryPoint = "IsWindowVisible")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
#if DEBUG
        /// <summary>
        /// 複製指定視窗標題控制項的文字
        /// </summary>
        /// <param name="hWnd">視窗控制項控制碼</param>
        /// <param name="text">接收文字的緩衝區</param>
        /// <param name="count">緩衝區的最大字元數</param>
        /// <returns>傳回值是複製字串的長度,反之,則為NULL</returns>
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);//取視窗title, debug用
#endif
        /// <summary>
        /// 擷取指定視窗周框的維度
        /// </summary>
        /// <param name="hWnd">視窗控制項控制碼</param>
        /// <param name="lpRect">接收視窗左上角和右下角的螢幕座標</param>
        /// <returns>如果函式成功，則傳回非零的值</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        /// <summary>
        /// 擷取具有指定關聯性的視窗控制碼,至指定的視窗
        /// </summary>
        /// <param name="hWnd">視窗的控點。所擷取的視窗控制碼會根據 uCmd 參數的值，相對於這個視窗。</param>
        /// <param name="uCmd">所指定視窗與要擷取其控制碼的視窗之間的關聯性</param>
        /// <returns>如果函式成功，則傳回值是視窗控制碼,反之,則為NULL</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        /// <summary>
        /// 擷取桌面視窗的控制碼
        /// </summary>
        /// <returns>傳回值是桌面視窗的控制碼</returns>
        [DllImport("User32.dll", EntryPoint = "GetDesktopWindow")]
        private static extern IntPtr GetDesktopWindow();
        /// <summary>
        /// 檢查與指定父視窗相關聯的子視窗Z順序(視窗物件堆疊順序)
        /// </summary>
        /// <param name="hWnd">檢查其子視窗之父視窗的控制碼</param>
        /// <returns>傳回值是 Z 順序頂端子視窗的控制碼。 如果指定的視窗沒有子視窗，則傳回值為 Null。</returns>
        [DllImport("User32.dll", EntryPoint = "GetTopWindow")]
        private static extern IntPtr GetTopWindow(IntPtr hWnd);
        /// <summary>
        /// 擷取指定視窗所屬類別的名稱
        /// </summary>
        /// <param name="hWnd">視窗的控制碼，並間接地是視窗所屬的類別。</param>
        /// <param name="lpClassName">類別名稱字串</param>
        /// <param name="nMaxCount">lpClassName緩衝區的長度(包含終止的 Null)</param>
        /// <returns>如果函式成功，傳回值就是複製到緩衝區的字元數，不包括終止的 Null 字元。</returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        /// <summary>
        /// 獲取指定視窗的訊息
        /// </summary>
        /// <param name="hWnd">指定視窗控制碼</param>
        /// <param name="nIndex">視窗記憶體的位元組起始位置</param>
        /// <returns>如果函式成功，則傳回值是要求的值。</returns>
        [DllImport("User32.dll", EntryPoint = "GetWindowLong")]
        private static extern long GetWindowLong(IntPtr hWnd,int  nIndex);
        /// <summary>
        /// 擷取指定的系統計量或系統組態設定
        /// </summary>
        /// <param name="smIndex">擷取的系統計量或組態設定</param>
        /// <returns>函式成功，則傳回值為要求的系統計量或組態設定,反之,則為0</returns>
        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(SystemMetricCode smIndex);
        //=================================================
        /// <summary>
        /// RECT 結構會依其左上角和右下角的座標來定義矩形
        /// </summary>
        private struct RECT
        {
            //原始是long
            /// <summary>
            /// 指定矩形左上角的 X座標
            /// </summary>
            public int Left;
            /// <summary>
            /// 指定矩形左上角的 Y座標
            /// </summary>
            public int Top;
            /// <summary>
            /// 指定矩形右下角的 X座標
            /// </summary>
            public int Right;
            /// <summary>
            /// 指定矩形右下角的 Y座標
            /// </summary>
            public int Bottom;
        }
        /// <summary>
        /// 所指定視窗與要擷取其控制碼的視窗之間的關聯性。 此參數為下列其中一個值。
        /// </summary>
        public static class GetWindowCode
        {
            /// <summary>
            /// 識別指定視窗所擁有的已啟用快顯視窗
            /// </summary>
            public const uint GW_ENABLEDPOPUP = 6;
            /// <summary>
            /// 檢查指定視窗的子視窗
            /// </summary>
            public const uint GW_CHILD = 5;
            /// <summary>
            /// 識別指定視窗的擁有者視窗
            /// </summary>
            public const uint GW_OWNER = 4;
            /// <summary>
            /// 識別指定視窗上方的視窗
            /// </summary>
            public const uint GW_HWNDPREV = 3;
            /// <summary>
            /// 識別指定視窗下方的視窗
            /// </summary>
            public const uint GW_HWNDNEXT = 2;
            /// <summary>
            /// 識別順序中最低類型的視窗
            /// </summary>
            public const uint GW_HWNDLAST = 1;
            /// <summary>
            /// 識別順序中最高類型的視窗
            /// </summary>
            public const uint GW_HWNDFIRST = 0;
        }
        /// <summary>
        /// 控制視窗項目(含標籤)
        /// </summary>
        class HandleObject
        {
            /// <summary>
            /// 建立視窗類別(含標籤)
            /// </summary>
            /// <param name="e">視窗類型</param>
            /// <param name="xCtrl">控制視窗</param>
            public HandleObject(HandleType e, Form xCtrl)
            {
                eType = e;
                this.xCtrl = xCtrl;
            }
            public HandleType eType;
            public Form xCtrl;
        }
        /// <summary>
        /// 要擷取的系統計量或組態設定。
        /// </summary>
        public enum SystemMetricCode
        {
            /// <summary>
            /// 主要顯示監視器的螢幕寬度，以圖元為單位。
            /// </summary>
            SM_CXSCREEN = 0,
            /// <summary>
            /// 主要顯示監視器的螢幕高度，以圖元為單位。
            /// </summary>
            SM_CYSCREEN = 1,
            /// <summary>
            /// 垂直捲動條的寬度，以圖元為單位。
            /// </summary>
            SM_CXVSCROLL = 2,
            /// <summary>
            /// 水準捲軸的高度，以圖元為單位。
            /// </summary>
            SM_CYHSCROLL = 3,
            /// <summary>
            /// 虛擬畫面左側的座標。 虛擬畫面是所有顯示監視器的周框
            /// </summary>
            SM_XVIRTUALSCREEN = 76,
            /// <summary>
            /// 虛擬畫面頂端的座標。 虛擬畫面是所有顯示監視器的周框。
            /// </summary>
            SM_YVIRTUALSCREEN = 77,
            /// <summary>
            /// 虛擬螢幕的寬度，以圖元為單位。
            /// </summary>
            SM_CXVIRTUALSCREEN = 78,
            /// <summary>
            /// 虛擬螢幕的高度，以圖元為單位。 
            /// </summary>
            SM_CYVIRTUALSCREEN = 79,
            /// <summary>
            /// 桌面上的顯示監視器數目
            /// </summary>
            SM_CMONITORS = 80,
            /// <summary>
            /// 判斷目前的終端機伺服器會話是否正在遠端控制
            /// </summary>
            SM_REMOTECONTROL = 0x2001, // 0x2001
        }
        /// <summary>
        /// 零起始位移,額外視窗記憶體的位元組數目
        /// </summary>
        public enum GetWindowLongCode: int
        {
            /// <summary>
            /// 擷取 擴充視窗樣式。
            /// </summary>
            GWL_EXSTYLE = -20,
            /// <summary>
            /// 擷取應用程式實例的控制碼。
            /// </summary>
            GWL_HINSTANCE = -6,
            /// <summary>
            /// 如果有的話，擷取父視窗的控制碼。
            /// </summary>
            GWL_HWNDPARENT = -8,
            /// <summary>
            /// 擷取視窗的識別碼。
            /// </summary>
            GWL_ID = -12,
            /// <summary>
            /// 擷取 視窗樣式。(預設是這個)
            /// </summary>
            GWL_STYLE = -16,
            /// <summary>
            /// 擷取與視窗相關聯的使用者資料。 此資料供建立視窗的應用程式使用。 其值一開始為零。
            /// </summary>
            GWL_USERDATA = -21,
            /// <summary>
            /// 擷取視窗程式的位址，或表示視窗程式位址的控制碼。 您必須使用 CallWindowProc 函式來呼叫視窗程式。
            /// </summary>
            GWL_WNDPROC = -4
        }
        /// <summary>
        /// 視窗類型標籤
        /// </summary>
        public enum HandleType
        {
            /// <summary>
            /// 主視窗
            /// </summary>
            Main,
            /// <summary>
            /// 視窗
            /// </summary>
            Form,
            /// <summary>
            /// 編輯視窗
            /// </summary>
            EditForm
        }
        //---------------------------------------------------
        private Timer m_TopMostTimer;

        private const Int64 WS_MINIMIZE = 0x20000000L;//視窗一開始會最小化,?不確定有無用
        public const int WM_TUC_MSG = 0x8000 + 2000;//WM_APP+2000
        public const int WM_TUC_SHOW_CMD = 0x8000 + 2001;//WM_APP+2001
        private bool IsAOIMode { get; set; } //AOI 模式, 將AOI編輯工單view蓋住
        private bool m_Aoi_ShowHide { get; set; } //MainForm用 基本上為true
        private List<frmBatchView> m_lsBatchView = new List<frmBatchView>(); //AOI模式下支援多螢幕顯示
        private List<HandleObject> m_lsHandle = new List<HandleObject>(); //登記handle, 避免ui移到下方
                                                                          //-----------------------------------------------------------------
        private void frmMain_Load(object sender, EventArgs e)
        {
            m_Aoi_ShowHide = false;//不顯示
            this.TopMost = true;//本視窗上層顯示
            Form xForm = null;
            string strCon = Properties.Settings.Default.T4;//預設資料庫設定//defaultDB

            #region for top most
            m_TopMostTimer = new Timer();
#if DEBUG
            m_TopMostTimer.Interval = 3000;//DEBUG用 3秒turn one
#else
            m_TopMostTimer.Interval = 300;
#endif
            m_TopMostTimer.Tick += TopMostTimer_Tick;

            m_TopMostTimer.Start();

            m_lsHandle.Add(new HandleObject(HandleType.Main, this));//控制項註冊
            #endregion

            this.Size = new Size(795, 900);//主視窗尺吋
            IsAOIMode = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            const int ctUINT_FIX_WIDTH = 1920;
            int nLeft = GetSystemMetrics(SystemMetricCode.SM_XVIRTUALSCREEN);//左座標=0
            int nTop = GetSystemMetrics(SystemMetricCode.SM_YVIRTUALSCREEN);//頂座標=0
            int nRight = GetSystemMetrics(SystemMetricCode.SM_CXVIRTUALSCREEN);//右座標
            int nBottom = GetSystemMetrics(SystemMetricCode.SM_CYVIRTUALSCREEN);//底座標
            Rectangle rcFull = new Rectangle(nLeft, nTop, nRight - nLeft, nBottom - nTop);//指定位置

            int nWidth = rcFull.Right - rcFull.Left;//總寬
            int nUnit = nWidth / ctUINT_FIX_WIDTH;//  總寬/螢幕寬 = 多少螢幕?單元?
            int nHeight = 62;
#if DEBUG
            nHeight = 78;
#endif
            //第一個元素是可執行檔名稱後面的零或多個元素包含其餘的命令列引數
            string[] args = Environment.GetCommandLineArgs();
            //AOI_CUSTOMERTYPE_ eCustomerType = AOI_CUSTOMERTYPE_.CUSTOMER_TUC_PP;//台耀 mode
            AOI_CUSTOMERTYPE_ eCustomerType = AOI_CUSTOMERTYPE_.CUSTOMER_TTA_TEST;//TTA mode (TEST)
            if (args.Length >= 2)//有回傳
            {
                if (int.TryParse(args[1], out int nTemp))//可以換成int
                {
                    eCustomerType = (AOI_CUSTOMERTYPE_)nTemp;
                }
            }
            for (int i = 0; i < nUnit; i++)
            {
                //新增處理視窗
                frmBatchView xFormTmp = new frmBatchView(strCon, this, eCustomerType);//新增處裡視窗
                //for top most
                m_lsHandle.Add(new HandleObject(HandleType.Form, xFormTmp));//處裡視窗加入控制項

                Point xPt = new Point(0, 0);
                if (i == 0)//主單元螢幕
                {
                    xPt = new Point(135 + 1000, nHeight);//(135 + 1000, 78)
                    xForm = xFormTmp;
                    this.Location = xPt;//顯示位置
                }
                else
                {
                    //次要螢幕
                    xPt = new Point(i * ctUINT_FIX_WIDTH + 1035, nHeight);//(單元數*1920+1035,78)
                    xFormTmp.Size = new Size(849, 900);
                }
                xFormTmp.Show();
                xFormTmp.Location = xPt;
                xFormTmp.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                xFormTmp.TopLevel = true;
                xFormTmp.TopMost = true;
                m_lsBatchView.Add(xFormTmp);
                xFormTmp.Hide();//(複數)(frmBatchView)初始化完後隱藏
            }
            if (xForm != null)//主Form存在
            {
                xForm.TopLevel = false;
                xForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                xForm.Visible = true;
                xForm.Dock = DockStyle.Fill;
                this.Controls.Add(xForm);
            }
        }
        private bool NeedMoveWindow(Form xForm)
        {
            //Get the top-most window in the desktop
            if(xForm.Visible)
            {
                IntPtr window = GetTopWindow(GetDesktopWindow());//取得指定最上層視窗控制碼
                do
                {
                    if (IsWindowVisible(window))//有無隱藏
                    {
                        RECT rct = new RECT();
                        GetWindowRect(window, ref rct);
                        Rectangle rcWnd = new Rectangle(rct.Left, rct.Top, (rct.Right - rct.Left), (rct.Bottom - rct.Top));
                        Rectangle rcForm = new Rectangle(xForm.Location.X, xForm.Location.Y, xForm.Size.Width, xForm.Size.Height);
                        if (rcForm.IntersectsWith(rcWnd) /*&& !m_lsHandle.Any(x=> x.xHandle == window)*/) //find overlap window 有touch到
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
                    window = GetWindow(window, GetWindowCode.GW_HWNDNEXT);
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
            int nWinHandle = FindWindow(null, "AOI Master"); //找AOI Master
            //RECT rc = new RECT();
            long nWndStyle = GetWindowLong((IntPtr)nWinHandle, (int)GetWindowLongCode.GWL_STYLE);//Form訊息
            if ((nWndStyle & WS_MINIMIZE) == 0) //AOI Master 有附錄值且 WS_MINIMIZE(視窗最小化)
            {
                if (m_Aoi_ShowHide)//AOI Master要顯示
                {
                    if(this.Visible == false)
                        this.Show();
                }
                nIndex = 0;//count
                foreach (var xView in m_lsBatchView)
                {
                    if (nIndex != 0)//主畫面的不處理
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
            //-----------
            List<IntPtr> lsWindows = new List<IntPtr>();
#if DEBUG
            const int nChars = 256;
            System.Text.StringBuilder Buff = new System.Text.StringBuilder(nChars);
#endif
            //Rectangle rcThis = new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height);

            if (NeedMoveWindow(this))//set top most again 有沒有與最上層視窗衝突 並移動
            {
                Console.WriteLine("top most again");
                this.TopMost = true;
                TopmostEditForm(this);//再設定為最上層
            }
            nIndex = 0;
            foreach(var xView in m_lsBatchView)
            {
                if (nIndex != 0)
                {    //skip first(it contains in main form
                    if (NeedMoveWindow(xView))
                    {
                        //在每個螢幕單元中將視窗調為最上層最大化
                        Console.WriteLine("top most view again");
                        xView.TopMost = true;
                        TopmostEditForm(xView);
                    }
                }
                nIndex++;
            }
        }
       

        private bool bInit = false;
        private void frmMain_Shown(object sender, EventArgs e)
        {
#if DEBUG
            bInit = true;//DEBUG怎都要開啟AOI模式
#endif
            if (IsAOIMode && bInit == false) //AOI模式要預設隱藏
            {
                this.Hide();
                bInit = true;
            }
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_TUC_MSG)//若視窗傳值為台燿訊息通道
            {
                if ((m.WParam.ToInt32() == WM_TUC_SHOW_CMD))//若視窗傳值為台燿訊息指令
                {
                    int nData = m.LParam.ToInt32();//訊息
                    for (int i = 0; i < m_lsBatchView.Count; i++)//有多少監視視窗控制
                    {
                        bool bShow = (nData & (1 << i)) > 0;//有點不懂邏輯
                        if(i == 0)//第一個視窗
                        {
                            m_Aoi_ShowHide = bShow;//這邊是要帶m_Aoi_ShowHide還是frmBatchView.m_Aoi_ShowHide
                            if (bShow)
                            {
                                this.Show();//顯示
                            }
                            else
                                this.Hide();//隱藏
                        }
                        else //後面視窗
                        {
                            m_lsBatchView[i].Set_AOI_ShowHide(bShow);//
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
        //-------------------------------------------------
        /// <summary>
        /// 增加新的視窗或控制項
        /// </summary>
        /// <param name="xAdd"控制項增加</param>
        public void AddHandle(Form xAdd)
        {
            m_lsHandle.Add(new HandleObject(HandleType.EditForm, xAdd));
        }
        /// <summary>
        /// 移除控制項
        /// </summary>
        /// <param name="xRemove"></param>
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
