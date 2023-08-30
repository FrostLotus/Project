using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace TUCBatchEditorCSharp.UI
{
    public class AoiButton : Button
    {
        public Bitmap m_xBitmap { get; set; }
        private PushButtonState m_eState = PushButtonState.Normal;
        
        public AoiButton(): base()
        {

        }
        public AoiButton(Bitmap xBitmap, Color xColor)
            : base()
        {
            this.m_xBitmap = xBitmap;
            this.BackColor = xColor;
            this.Size = new Size(xBitmap.Width / 4, xBitmap.Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ButtonRenderer.DrawParentBackground(e.Graphics, ClientRectangle, this);
            int nUnit = m_xBitmap.Width / 4;
            int nIdx = 0;
            if (!this.Enabled)
            {
                nIdx = 2;
            }
            else
            {
                switch (m_eState)
                {
                    case PushButtonState.Pressed:
                        nIdx = 1;
                        break;
                    case PushButtonState.Disabled:
                        nIdx = 2;
                        break;
                    case PushButtonState.Hot:
                        nIdx = 3;
                        break;
                    case PushButtonState.Normal:
                    default:
                        nIdx = 0;
                        break;
                }
            }
            e.Graphics.FillRectangle(new SolidBrush(this.BackColor), ClientRectangle);
            e.Graphics.DrawImage(m_xBitmap, -nUnit * nIdx, 0, new Rectangle(0, 0, m_xBitmap.Width, m_xBitmap.Height), GraphicsUnit.Pixel);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            m_eState = PushButtonState.Pressed;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            m_eState = PushButtonState.Hot;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            m_eState = PushButtonState.Normal;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            OnMouseEnter(e);
        }
    }
}
