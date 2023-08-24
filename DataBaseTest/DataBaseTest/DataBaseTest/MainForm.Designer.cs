
namespace DataBaseTest
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
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
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.Tv_DataBaseList = new System.Windows.Forms.TreeView();
            this.Dgv_DataTable = new System.Windows.Forms.DataGridView();
            this.Lab_DataTable = new System.Windows.Forms.Label();
            this.Btn_FrashDataTable = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_DataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "能存取之資料庫";
            // 
            // Tv_DataBaseList
            // 
            this.Tv_DataBaseList.Location = new System.Drawing.Point(14, 41);
            this.Tv_DataBaseList.Name = "Tv_DataBaseList";
            this.Tv_DataBaseList.Size = new System.Drawing.Size(224, 245);
            this.Tv_DataBaseList.TabIndex = 6;
            this.Tv_DataBaseList.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.Tv_DataBaseList_NodeMouseDoubleClick);
            // 
            // Dgv_DataTable
            // 
            this.Dgv_DataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dgv_DataTable.Location = new System.Drawing.Point(251, 26);
            this.Dgv_DataTable.Name = "Dgv_DataTable";
            this.Dgv_DataTable.RowTemplate.Height = 24;
            this.Dgv_DataTable.Size = new System.Drawing.Size(659, 405);
            this.Dgv_DataTable.TabIndex = 7;
            this.Dgv_DataTable.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.Dgv_DataTable_UserDeletedRow);
            this.Dgv_DataTable.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.Dgv_DataTable_UserDeletingRow);
            // 
            // Lab_DataTable
            // 
            this.Lab_DataTable.AutoSize = true;
            this.Lab_DataTable.Location = new System.Drawing.Point(249, 11);
            this.Lab_DataTable.Name = "Lab_DataTable";
            this.Lab_DataTable.Size = new System.Drawing.Size(89, 12);
            this.Lab_DataTable.TabIndex = 8;
            this.Lab_DataTable.Text = "請先選擇資料表";
            // 
            // Btn_FrashDataTable
            // 
            this.Btn_FrashDataTable.Location = new System.Drawing.Point(251, 437);
            this.Btn_FrashDataTable.Name = "Btn_FrashDataTable";
            this.Btn_FrashDataTable.Size = new System.Drawing.Size(138, 38);
            this.Btn_FrashDataTable.TabIndex = 9;
            this.Btn_FrashDataTable.Text = "更新與新增修改值";
            this.Btn_FrashDataTable.UseVisualStyleBackColor = true;
            this.Btn_FrashDataTable.Click += new System.EventHandler(this.Btn_FrashDataTable_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1202, 536);
            this.Controls.Add(this.Btn_FrashDataTable);
            this.Controls.Add(this.Lab_DataTable);
            this.Controls.Add(this.Dgv_DataTable);
            this.Controls.Add(this.Tv_DataBaseList);
            this.Controls.Add(this.label2);
            this.Name = "MainForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_DataTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TreeView Tv_DataBaseList;
        private System.Windows.Forms.DataGridView Dgv_DataTable;
        private System.Windows.Forms.Label Lab_DataTable;
        private System.Windows.Forms.Button Btn_FrashDataTable;
    }
}

