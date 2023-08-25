using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
namespace TUCBatchEditorCSharp
{
    
    public partial class frmAddnEdit : Form
    {
        public delegate void dgFinishEdit(FormType eType, DB.IEditable xOld, DB.IEditable xNew);
        public event dgFinishEdit OnFinishEdit = null;
        public delegate void dgCancelEdit();
        public event dgCancelEdit OnCancelEdit = null;
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
         );
        const int ctHeaderHeight = 50;
        private bool mouseDown;
        private Point lastLocation;
        private Font m_TextFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        private enum BTN_TYPE
        {
            OK,
            CANCEL,
            BTN_MAX
        }
        UI.AoiButton[] m_xBtn = new UI.AoiButton[(int)BTN_TYPE.BTN_MAX];
        /// <summary>
        /// key: Control Name, value: list of data
        /// </summary>
        List<KeyValuePair<string, List<string>>> m_lsComboData;
        public enum FormType
        {
            Add,
            Edit
        }
        private FormType m_eType { get; set; }
        private DB.IEditable m_xData { get; set; }
        public frmAddnEdit()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 顯示表單前須設定表單類型及預設顯示資料
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="xData"></param>
        /// <param name="lsComboData">key: Control Name, value: list of data</param>
        public void SetEditParam(FormType eType, DB.IEditable xData, List<KeyValuePair<string, List<string>>> lsComboData)
        {
            m_xData = xData;
            m_eType = eType;
            m_lsComboData = lsComboData;
        }
        private void InitControl()
        {
            var xAttributeList = m_xData.GetType().GetProperties().Select(x => x.GetCustomAttributes(typeof(DB.FieldInfoAttribute), false).Select(y => new KeyValuePair<string, DB.FieldInfoAttribute>(x.Name, (DB.FieldInfoAttribute)y))
                    .FirstOrDefault())
                .Where(x => x.Value != null && x.Value.Editable)
                .ToList();

            int nWidth = 450; //fix width

            int nYPos = ctHeaderHeight + 20;
            foreach(var xAttr in xAttributeList)
            {
                //add label
                Label xLabel = new Label();
                xLabel.Text = xAttr.Value.Name;
                xLabel.Name = xAttr.Key;
                xLabel.Size = new Size(80, 30);
                xLabel.Location = new Point(20, nYPos);
                xLabel.Font = m_TextFont;
                xLabel.ForeColor = Color.FromArgb(93, 93, 93);
                xLabel.TextAlign = ContentAlignment.MiddleRight;
                this.Controls.Add(xLabel);

                //add control
                if(xAttr.Value.ControlType == typeof(TextBox))
                {
                    TextBox xTextBox = new TextBox();
                    xTextBox.Size = new Size(300, 35);
                    xTextBox.Name = xAttr.Key;
                    xTextBox.Font = m_TextFont;
                    xTextBox.Location = new Point(120, nYPos);
                    xTextBox.TextAlign = HorizontalAlignment.Center;
                    this.Controls.Add(xTextBox);
                }
                else if(xAttr.Value.ControlType == typeof(ComboBox))
                {
                    ComboBox xCB = new ComboBox();
                    xCB.Size = new Size(300, 35);
                    xCB.Font = m_TextFont;
                    xCB.Location = new Point(120, nYPos);
                    xCB.Name = xAttr.Key;
                    xCB.DropDownStyle = ComboBoxStyle.DropDownList;
                    m_lsComboData.Where(x => x.Key == xAttr.Key).Select(x =>
                    {
                        x.Value.ForEach(y => xCB.Items.Add(y));
                        return x;
                    }
                        ).ToList();
                    //if(m_eType == FormType.Add && xCB.Items.Count > 0) //select top 1 
                    //{
                    //    xCB.SelectedIndex = 1;
                    //}
                    //else if(m_eType == FormType.Edit)
                    //{
                        
                    //    foreach(var xCBItem in xCB.Items)
                    //    {
                            
                    //    }
                    //}
                    this.Controls.Add(xCB);
                }
                nYPos += 45;
            }
            //add ok/cancel
            for(int i=0; i < (int)BTN_TYPE.BTN_MAX; i++)
            {
                Bitmap xBitmap = null;
                string strName = "";
                Point xPoint = new Point(0, nYPos);
                switch((BTN_TYPE)i)
                {
                    case BTN_TYPE.OK:
                        xBitmap = Properties.Resources.ok_btn;
                        strName = "OK";
                        xPoint.X = 20;
                        break;
                    case BTN_TYPE.CANCEL:
                        xBitmap = Properties.Resources.cancel_btn;
                        strName = "CANCEL";
                        xPoint.X = 232;
                        break;
                }
                m_xBtn[i] = new UI.AoiButton(xBitmap, Color.White);
                m_xBtn[i].Name = strName;
                m_xBtn[i].Location = xPoint;
                m_xBtn[i].Click += btn_Click;
                this.Controls.Add(m_xBtn[i]);
            }
            nYPos += m_xBtn[(int)BTN_TYPE.OK].Height + 20;

            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, nWidth, nYPos, 20, 20)); //change to round rect
            this.Size = new Size(nWidth, nYPos);
            this.CenterToScreen();
        }
        private void frmAddnEdit_Load(object sender, EventArgs e)
        {
            InitControl();
            FillDefault();
        }
        private void FillDefault()
        {
            foreach (Control xCtrl in this.Controls)
            {
                Type xCtrlType = xCtrl.GetType();
                if (xCtrlType == typeof(Label)) continue;

                foreach (var prop in m_xData.GetType().GetProperties())
                {
                    if (prop.Name == xCtrl.Name)
                    {
                        object xValue = prop.GetValue(m_xData, null);
                        if (xCtrlType == typeof(TextBox))
                        {
                            xCtrl.Text = xValue == null ? "" : xValue.ToString();
                        }
                        else if (xCtrlType == typeof(ComboBox))
                        {
                            if (xValue != null)
                            {
                                foreach (var xItem in ((ComboBox)xCtrl).Items)//item must exist
                                {
                                    if (xItem.ToString() == prop.GetValue(m_xData, null).ToString())
                                    {
                                        ((ComboBox)xCtrl).SelectedItem = xItem;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            string strText = "";
            switch(m_eType)
            {
                default:
                case FormType.Add:
                    strText = "Add Item";
                    break;
                case FormType.Edit:
                    strText = "Edit Item";
                    break;
            }
            base.OnPaint(e);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(73,73,73)), new Rectangle(0, 0, ClientRectangle.Width, ctHeaderHeight));
            e.Graphics.DrawString(strText, m_TextFont,
                new SolidBrush(Color.White), 10, 15);
        }

        private void frmAddnEdit_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void frmAddnEdit_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Y >= 0 && e.Y <= ctHeaderHeight)
            {
                mouseDown = true;
                lastLocation = e.Location;
            }
        }

        private void frmAddnEdit_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
        private void btn_Click(object sender, EventArgs e)
        {
            if(sender == m_xBtn[(int)BTN_TYPE.OK])
            {
                string strErr = "";
                DB.IEditable xNew = Activator.CreateInstance(m_xData.GetType()) as DB.IEditable;
                foreach(Control xCtrl in this.Controls)
                {
                    if (xCtrl.GetType() == typeof(Label))
                        continue;

                    foreach(var prop in m_xData.GetType().GetProperties())
                    {
                        if (prop.Name == xCtrl.Name)
                        {
                            if (xCtrl.Text.Length == 0)
                            {
                                var attr = prop.GetCustomAttributes(typeof(DB.FieldInfoAttribute), false).Select(x => (DB.FieldInfoAttribute)x).FirstOrDefault();
                                if(attr != null)
                                {
                                    strErr = string.Format("{0} 空白", attr.Name);
                                }
                                else
                                {
                                    strErr = string.Format("{0} 空白!", xCtrl.Name);
                                }
                            }
                            else
                            {
                                prop.SetValue(xNew, xCtrl.Text, null);
                            }
                            break;
                        }
                    }
                }
                if (strErr.Length == 0)
                {
                    OnFinishEdit(m_eType, m_xData, xNew);
                    this.Close();
                }
                else
                {
                    MessageBox.Show(strErr);
                }
            }
            else if(sender == m_xBtn[(int)BTN_TYPE.CANCEL])
            {
                OnCancelEdit();
                this.Close();
            }
        }
    }
}
