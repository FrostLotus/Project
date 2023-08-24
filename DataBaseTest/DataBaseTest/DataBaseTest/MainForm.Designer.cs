
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
            this.Btn_Connect = new System.Windows.Forms.Button();
            this.Btn_Read = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Dgv_Customers = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.Dgv_Orderdetail = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.Dgv_Orders = new System.Windows.Forms.DataGridView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.Dgv_Product = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.Tv_DataBaseList = new System.Windows.Forms.TreeView();
            this.Dgv_DataTable = new System.Windows.Forms.DataGridView();
            this.Lab_DataTable = new System.Windows.Forms.Label();
            this.Btn_FrashDataTable = new System.Windows.Forms.Button();
            this.Lab_Rsult = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_Customers)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_Orderdetail)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_Orders)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_Product)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_DataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // Btn_Connect
            // 
            this.Btn_Connect.Location = new System.Drawing.Point(12, 12);
            this.Btn_Connect.Name = "Btn_Connect";
            this.Btn_Connect.Size = new System.Drawing.Size(90, 45);
            this.Btn_Connect.TabIndex = 0;
            this.Btn_Connect.Text = "資料庫開啟測試";
            this.Btn_Connect.UseVisualStyleBackColor = true;
            this.Btn_Connect.Click += new System.EventHandler(this.Btn_Connect_Click);
            // 
            // Btn_Read
            // 
            this.Btn_Read.Location = new System.Drawing.Point(12, 75);
            this.Btn_Read.Name = "Btn_Read";
            this.Btn_Read.Size = new System.Drawing.Size(90, 44);
            this.Btn_Read.TabIndex = 2;
            this.Btn_Read.Text = "讀取單列測試";
            this.Btn_Read.UseVisualStyleBackColor = true;
            this.Btn_Read.Click += new System.EventHandler(this.Btn_Read_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(1027, 41);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(639, 426);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.Dgv_Customers);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(631, 400);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "顧客";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // Dgv_Customers
            // 
            this.Dgv_Customers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dgv_Customers.Location = new System.Drawing.Point(3, 3);
            this.Dgv_Customers.Name = "Dgv_Customers";
            this.Dgv_Customers.RowTemplate.Height = 24;
            this.Dgv_Customers.Size = new System.Drawing.Size(625, 394);
            this.Dgv_Customers.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.Dgv_Orderdetail);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(631, 400);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "交易細項";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // Dgv_Orderdetail
            // 
            this.Dgv_Orderdetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dgv_Orderdetail.Location = new System.Drawing.Point(3, 3);
            this.Dgv_Orderdetail.Name = "Dgv_Orderdetail";
            this.Dgv_Orderdetail.RowTemplate.Height = 24;
            this.Dgv_Orderdetail.Size = new System.Drawing.Size(625, 394);
            this.Dgv_Orderdetail.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.Dgv_Orders);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(631, 400);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "交易列表";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // Dgv_Orders
            // 
            this.Dgv_Orders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dgv_Orders.Location = new System.Drawing.Point(3, 3);
            this.Dgv_Orders.Name = "Dgv_Orders";
            this.Dgv_Orders.RowTemplate.Height = 24;
            this.Dgv_Orders.Size = new System.Drawing.Size(625, 394);
            this.Dgv_Orders.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.Dgv_Product);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(631, 400);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "產品";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // Dgv_Product
            // 
            this.Dgv_Product.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dgv_Product.Location = new System.Drawing.Point(3, 3);
            this.Dgv_Product.Name = "Dgv_Product";
            this.Dgv_Product.RowTemplate.Height = 24;
            this.Dgv_Product.Size = new System.Drawing.Size(625, 394);
            this.Dgv_Product.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 188);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "能存取之資料庫";
            // 
            // Tv_DataBaseList
            // 
            this.Tv_DataBaseList.Location = new System.Drawing.Point(12, 203);
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
            this.Btn_FrashDataTable.Size = new System.Drawing.Size(105, 38);
            this.Btn_FrashDataTable.TabIndex = 9;
            this.Btn_FrashDataTable.Text = "更新修改值";
            this.Btn_FrashDataTable.UseVisualStyleBackColor = true;
            this.Btn_FrashDataTable.Click += new System.EventHandler(this.Btn_FrashDataTable_Click);
            // 
            // Lab_Rsult
            // 
            this.Lab_Rsult.AutoSize = true;
            this.Lab_Rsult.Location = new System.Drawing.Point(362, 450);
            this.Lab_Rsult.Name = "Lab_Rsult";
            this.Lab_Rsult.Size = new System.Drawing.Size(56, 12);
            this.Lab_Rsult.TabIndex = 8;
            this.Lab_Rsult.Text = "共影響:列";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1202, 536);
            this.Controls.Add(this.Btn_FrashDataTable);
            this.Controls.Add(this.Lab_Rsult);
            this.Controls.Add(this.Lab_DataTable);
            this.Controls.Add(this.Dgv_DataTable);
            this.Controls.Add(this.Tv_DataBaseList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.Btn_Read);
            this.Controls.Add(this.Btn_Connect);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_Customers)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_Orderdetail)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_Orders)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_Product)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_DataTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_Connect;
        private System.Windows.Forms.Button Btn_Read;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView Dgv_Customers;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView Dgv_Orderdetail;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView Dgv_Orders;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView Dgv_Product;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TreeView Tv_DataBaseList;
        private System.Windows.Forms.DataGridView Dgv_DataTable;
        private System.Windows.Forms.Label Lab_DataTable;
        private System.Windows.Forms.Button Btn_FrashDataTable;
        private System.Windows.Forms.Label Lab_Rsult;
    }
}

