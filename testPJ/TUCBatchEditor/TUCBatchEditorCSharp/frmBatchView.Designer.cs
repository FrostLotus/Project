using System.Drawing;
using System.Windows.Forms;
namespace TUCBatchEditorCSharp
{
    partial class frmBatchView
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(204, 204, 204);
            this.dgvUsed = new System.Windows.Forms.DataGridView();
            this.dgvUsing = new System.Windows.Forms.DataGridView();
            this.dgvUnUsed = new System.Windows.Forms.DataGridView();
            this.lblUsed = new System.Windows.Forms.Label();
            this.lblUsing = new System.Windows.Forms.Label();
            this.lblUnUsed = new System.Windows.Forms.Label();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.lbl_DBStatus = new System.Windows.Forms.Label();
            this.btnAdd = new UI.AoiButton(Properties.Resources.btn_pg5_add, this.BackColor);
            this.btnEdit = new UI.AoiButton(Properties.Resources.btn_pg3_edit, this.BackColor);
            this.btnRemove = new UI.AoiButton(Properties.Resources.btn_pg3_del, this.BackColor);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnUsed)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvUsed
            // 
            this.dgvUsed.MultiSelect = false;
            this.dgvUsed.AllowUserToResizeRows = false;
            this.dgvUsed.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            this.dgvUsed.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dgvUsed.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dgvUsed.AllowUserToAddRows = false;
            this.dgvUsed.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvUsed.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsed.Location = new System.Drawing.Point(25, 55);
            this.dgvUsed.Name = "dgvUsed";
            this.dgvUsed.RowHeadersVisible = false;
            this.dgvUsed.RowTemplate.Height = 24;
            this.dgvUsed.RowTemplate.ReadOnly = true;
            this.dgvUsed.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsed.Size = new System.Drawing.Size(710, 140);
            this.dgvUsed.TabIndex = 0;
            this.dgvUsed.VirtualMode = true;
            this.dgvUsed.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgv_CellValueNeeded);
            this.dgvUsed.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.dgv_RowStateChanged);
            // 
            // dgvUsing
            // 
            this.dgvUsing.MultiSelect = false;
            this.dgvUsing.AllowUserToResizeRows = false;
            this.dgvUsing.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            this.dgvUsing.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dgvUsing.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dgvUsing.AllowUserToAddRows = false;
            this.dgvUsing.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvUsing.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsing.Location = new System.Drawing.Point(25, 235);
            this.dgvUsing.Name = "dgvUsing";
            this.dgvUsing.RowHeadersVisible = false;
            this.dgvUsing.RowTemplate.Height = 24;
            this.dgvUsing.RowTemplate.ReadOnly = true;
            this.dgvUsing.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsing.Size = new System.Drawing.Size(710, 60);
            this.dgvUsing.TabIndex = 1;
            this.dgvUsing.VirtualMode = true;
            this.dgvUsing.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgv_CellValueNeeded);
            this.dgvUsing.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.dgv_RowStateChanged);
            // 
            // dgvUnUsed
            // 
            this.dgvUnUsed.MultiSelect = false;
            this.dgvUnUsed.AllowUserToResizeRows = false;
            this.dgvUnUsed.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            this.dgvUnUsed.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dgvUnUsed.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dgvUnUsed.AllowUserToAddRows = false;
            this.dgvUnUsed.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvUnUsed.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUnUsed.Location = new System.Drawing.Point(25, 335);
            this.dgvUnUsed.Name = "dgvUnUsed";
            this.dgvUnUsed.RowHeadersVisible = false;
            this.dgvUnUsed.RowTemplate.Height = 24;
            this.dgvUnUsed.RowTemplate.ReadOnly = true;
            this.dgvUnUsed.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUnUsed.Size = new System.Drawing.Size(710, 300);
            this.dgvUnUsed.TabIndex = 2;
            this.dgvUnUsed.VirtualMode = true;
            this.dgvUnUsed.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgv_CellValueNeeded);
            this.dgvUnUsed.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.dgv_RowStateChanged);
            // 
            // lblUsed
            // 
            this.lblUsed.AutoSize = true;
            this.lblUsed.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblUsed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.lblUsed.Location = new System.Drawing.Point(25, 30);
            this.lblUsed.Name = "lblUsed";
            this.lblUsed.Size = new System.Drawing.Size(51, 19);
            this.lblUsed.TabIndex = 3;
            this.lblUsed.Text = "已完成";
            // 
            // lblUsing
            // 
            this.lblUsing.AutoSize = true;
            this.lblUsing.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblUsing.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.lblUsing.Location = new System.Drawing.Point(25, 210);
            this.lblUsing.Name = "lblUsing";
            this.lblUsing.Size = new System.Drawing.Size(51, 19);
            this.lblUsing.TabIndex = 4;
            this.lblUsing.Text = "執行中";
            // 
            // lblUnUsed
            // 
            this.lblUnUsed.AutoSize = true;
            this.lblUnUsed.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblUnUsed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.lblUnUsed.Location = new System.Drawing.Point(25, 310);
            this.lblUnUsed.Name = "lblUnUsed";
            this.lblUnUsed.Size = new System.Drawing.Size(51, 19);
            this.lblUnUsed.TabIndex = 5;
            this.lblUnUsed.Text = "未完成";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(740, 55);
            this.btnAdd.m_xBitmap = global::TUCBatchEditorCSharp.Properties.Resources.btn_pg5_add;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(42, 42);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(740, 105);
            this.btnEdit.m_xBitmap = global::TUCBatchEditorCSharp.Properties.Resources.btn_pg3_edit;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(42, 42);
            this.btnEdit.TabIndex = 7;
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(740, 155);
            this.btnRemove.m_xBitmap = global::TUCBatchEditorCSharp.Properties.Resources.btn_pg3_del;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(42, 42);
            this.btnRemove.TabIndex = 8;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btn_Click);
            // 
            // lblDBStatus
            // 
            this.lbl_DBStatus.AutoSize = true;
            this.lbl_DBStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_DBStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.lbl_DBStatus.Location = new System.Drawing.Point(60, 660);
            this.lbl_DBStatus.Name = "lblDBStatus";
            this.lbl_DBStatus.Text = "資料庫連線異常";
            this.lbl_DBStatus.Size = new System.Drawing.Size(0, 19);
            this.lbl_DBStatus.TabIndex = 10;
            this.lbl_DBStatus.Visible = true;
            // 
            // lblStatus
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_Status.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.lbl_Status.Location = new System.Drawing.Point(25, 640);
            this.lbl_Status.Name = "lblStatus";
            this.lbl_Status.Size = new System.Drawing.Size(0, 19);
            this.lbl_Status.TabIndex = 9;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 869);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.lbl_DBStatus);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblUnUsed);
            this.Controls.Add(this.lblUsing);
            this.Controls.Add(this.lblUsed);
            this.Controls.Add(this.dgvUnUsed);
            this.Controls.Add(this.dgvUsing);
            this.Controls.Add(this.dgvUsed);
            this.Name = "frmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmBatchView_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmBatchView_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnUsed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvUsed;
        private System.Windows.Forms.DataGridView dgvUsing;
        private System.Windows.Forms.DataGridView dgvUnUsed;
        private System.Windows.Forms.Label lblUsed;
        private System.Windows.Forms.Label lblUsing;
        private System.Windows.Forms.Label lblUnUsed;
        private UI.AoiButton btnAdd;
        private UI.AoiButton btnEdit;
        private UI.AoiButton btnRemove;
        private Label lbl_Status;
        private Label lbl_DBStatus;
    }
}

