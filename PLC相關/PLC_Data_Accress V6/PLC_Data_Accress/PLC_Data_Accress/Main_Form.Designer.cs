namespace PLC_Data_Access
{
    partial class Main_Form
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
            this.dgv_ReadDataGrid = new System.Windows.Forms.DataGridView();
            this.Read_SN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Read_Label = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Read_Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Read_DataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Read_Data = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Read_IsUse = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Read_DeviceValueGet = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.檔案ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_DataGridLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_DataGridSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_PLCSet = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_WriteDataGrid = new System.Windows.Forms.DataGridView();
            this.Write_SN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Write_Label = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Write_Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Write_DataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Write_Data = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Write_IsUse = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Write_DeviceValueGet = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Write_DeviceValueSet = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_ModelChange = new System.Windows.Forms.Button();
            this.p_ModelChange = new System.Windows.Forms.Panel();
            this.btn_DataUpload = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_ReadTime = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.p_MxOpenStatus = new System.Windows.Forms.Panel();
            this.btn_MxOpen = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ReadDataGrid)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_WriteDataGrid)).BeginInit();
            this.p_ModelChange.SuspendLayout();
            this.p_MxOpenStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv_ReadDataGrid
            // 
            this.dgv_ReadDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ReadDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Read_SN,
            this.Read_Label,
            this.Read_Address,
            this.Read_DataType,
            this.Read_Data,
            this.Read_IsUse,
            this.Read_DeviceValueGet});
            this.dgv_ReadDataGrid.Location = new System.Drawing.Point(6, 21);
            this.dgv_ReadDataGrid.Name = "dgv_ReadDataGrid";
            this.dgv_ReadDataGrid.RowHeadersVisible = false;
            this.dgv_ReadDataGrid.RowTemplate.Height = 24;
            this.dgv_ReadDataGrid.Size = new System.Drawing.Size(682, 420);
            this.dgv_ReadDataGrid.TabIndex = 0;
            this.dgv_ReadDataGrid.VirtualMode = true;
            this.dgv_ReadDataGrid.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.Dgv_ReadDataGrid_CellValueNeeded);
            this.dgv_ReadDataGrid.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.Dgv_ReadDataGrid_CellValuePushed);
            // 
            // Read_SN
            // 
            this.Read_SN.HeaderText = "序号";
            this.Read_SN.Name = "Read_SN";
            this.Read_SN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Read_SN.Width = 60;
            // 
            // Read_Label
            // 
            this.Read_Label.HeaderText = "名称";
            this.Read_Label.Name = "Read_Label";
            this.Read_Label.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Read_Label.Width = 160;
            // 
            // Read_Address
            // 
            this.Read_Address.HeaderText = "地址";
            this.Read_Address.Name = "Read_Address";
            this.Read_Address.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Read_DataType
            // 
            this.Read_DataType.HeaderText = "数据类型";
            this.Read_DataType.Name = "Read_DataType";
            this.Read_DataType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Read_DataType.Width = 60;
            // 
            // Read_Data
            // 
            this.Read_Data.HeaderText = "变量";
            this.Read_Data.Name = "Read_Data";
            this.Read_Data.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Read_Data.Width = 80;
            // 
            // Read_IsUse
            // 
            this.Read_IsUse.HeaderText = "是否使用";
            this.Read_IsUse.Name = "Read_IsUse";
            this.Read_IsUse.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Read_IsUse.Width = 60;
            // 
            // Read_DeviceValueGet
            // 
            this.Read_DeviceValueGet.HeaderText = "軟元件當前值";
            this.Read_DeviceValueGet.Name = "Read_DeviceValueGet";
            this.Read_DeviceValueGet.Width = 150;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.檔案ToolStripMenuItem,
            this.mi_PLCSet});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1503, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 檔案ToolStripMenuItem
            // 
            this.檔案ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_DataGridLoad,
            this.mi_DataGridSave});
            this.檔案ToolStripMenuItem.Name = "檔案ToolStripMenuItem";
            this.檔案ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.檔案ToolStripMenuItem.Text = "檔案";
            // 
            // mi_DataGridLoad
            // 
            this.mi_DataGridLoad.Name = "mi_DataGridLoad";
            this.mi_DataGridLoad.Size = new System.Drawing.Size(122, 22);
            this.mi_DataGridLoad.Text = "格式讀取";
            this.mi_DataGridLoad.Click += new System.EventHandler(this.Mi_DataGridLoad_Click);
            // 
            // mi_DataGridSave
            // 
            this.mi_DataGridSave.Name = "mi_DataGridSave";
            this.mi_DataGridSave.Size = new System.Drawing.Size(122, 22);
            this.mi_DataGridSave.Text = "格式儲存";
            this.mi_DataGridSave.Click += new System.EventHandler(this.Mi_DataGridSave_Click);
            // 
            // mi_PLCSet
            // 
            this.mi_PLCSet.Name = "mi_PLCSet";
            this.mi_PLCSet.Size = new System.Drawing.Size(64, 20);
            this.mi_PLCSet.Text = "PLC設定";
            this.mi_PLCSet.Click += new System.EventHandler(this.Mi_PLCSet_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgv_ReadDataGrid);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(694, 458);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "下載";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgv_WriteDataGrid);
            this.groupBox2.Location = new System.Drawing.Point(712, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(783, 458);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "上傳";
            // 
            // dgv_WriteDataGrid
            // 
            this.dgv_WriteDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_WriteDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Write_SN,
            this.Write_Label,
            this.Write_Address,
            this.Write_DataType,
            this.Write_Data,
            this.Write_IsUse,
            this.Write_DeviceValueGet,
            this.Write_DeviceValueSet});
            this.dgv_WriteDataGrid.Location = new System.Drawing.Point(6, 21);
            this.dgv_WriteDataGrid.Name = "dgv_WriteDataGrid";
            this.dgv_WriteDataGrid.RowHeadersVisible = false;
            this.dgv_WriteDataGrid.RowTemplate.Height = 24;
            this.dgv_WriteDataGrid.Size = new System.Drawing.Size(771, 420);
            this.dgv_WriteDataGrid.TabIndex = 0;
            this.dgv_WriteDataGrid.VirtualMode = true;
            this.dgv_WriteDataGrid.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.Dgv_WriteDataGrid_CellValueNeeded);
            this.dgv_WriteDataGrid.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.Dgv_WriteDataGrid_CellValuePushed);
            // 
            // Write_SN
            // 
            this.Write_SN.HeaderText = "序号";
            this.Write_SN.Name = "Write_SN";
            this.Write_SN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Write_SN.Width = 40;
            // 
            // Write_Label
            // 
            this.Write_Label.HeaderText = "名称";
            this.Write_Label.Name = "Write_Label";
            this.Write_Label.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Write_Label.Width = 130;
            // 
            // Write_Address
            // 
            this.Write_Address.HeaderText = "地址";
            this.Write_Address.Name = "Write_Address";
            this.Write_Address.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Write_DataType
            // 
            this.Write_DataType.HeaderText = "数据类型";
            this.Write_DataType.Name = "Write_DataType";
            this.Write_DataType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Write_DataType.Width = 60;
            // 
            // Write_Data
            // 
            this.Write_Data.HeaderText = "变量";
            this.Write_Data.Name = "Write_Data";
            this.Write_Data.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Write_Data.Width = 80;
            // 
            // Write_IsUse
            // 
            this.Write_IsUse.HeaderText = "是否使用";
            this.Write_IsUse.Name = "Write_IsUse";
            this.Write_IsUse.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Write_IsUse.Width = 60;
            // 
            // Write_DeviceValueGet
            // 
            this.Write_DeviceValueGet.HeaderText = "軟元件當前值";
            this.Write_DeviceValueGet.Name = "Write_DeviceValueGet";
            this.Write_DeviceValueGet.Width = 150;
            // 
            // Write_DeviceValueSet
            // 
            this.Write_DeviceValueSet.HeaderText = "修改值(可監視中易值)";
            this.Write_DeviceValueSet.Name = "Write_DeviceValueSet";
            this.Write_DeviceValueSet.Width = 145;
            // 
            // btn_ModelChange
            // 
            this.btn_ModelChange.Location = new System.Drawing.Point(20, 6);
            this.btn_ModelChange.Name = "btn_ModelChange";
            this.btn_ModelChange.Size = new System.Drawing.Size(131, 52);
            this.btn_ModelChange.TabIndex = 4;
            this.btn_ModelChange.Text = "格式修改模式中";
            this.btn_ModelChange.UseVisualStyleBackColor = true;
            this.btn_ModelChange.Click += new System.EventHandler(this.Btn_ModelChange_Click);
            // 
            // p_ModelChange
            // 
            this.p_ModelChange.BackColor = System.Drawing.Color.LightBlue;
            this.p_ModelChange.Controls.Add(this.btn_ModelChange);
            this.p_ModelChange.Location = new System.Drawing.Point(12, 512);
            this.p_ModelChange.Name = "p_ModelChange";
            this.p_ModelChange.Size = new System.Drawing.Size(170, 62);
            this.p_ModelChange.TabIndex = 5;
            // 
            // btn_DataUpload
            // 
            this.btn_DataUpload.Location = new System.Drawing.Point(1326, 493);
            this.btn_DataUpload.Name = "btn_DataUpload";
            this.btn_DataUpload.Size = new System.Drawing.Size(163, 45);
            this.btn_DataUpload.TabIndex = 74;
            this.btn_DataUpload.Text = "上傳資料上傳";
            this.btn_DataUpload.UseVisualStyleBackColor = true;
            this.btn_DataUpload.Click += new System.EventHandler(this.Btn_DataUpdate_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(20, 493);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 16);
            this.label1.TabIndex = 50;
            this.label1.Text = "模式切換(修改模式/運行)";
            // 
            // txt_ReadTime
            // 
            this.txt_ReadTime.Location = new System.Drawing.Point(203, 512);
            this.txt_ReadTime.Name = "txt_ReadTime";
            this.txt_ReadTime.Size = new System.Drawing.Size(123, 22);
            this.txt_ReadTime.TabIndex = 1;
            this.txt_ReadTime.Text = "2000";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(201, 493);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 16);
            this.label2.TabIndex = 50;
            this.label2.Text = "讀取回傳時間設定(ms)";
            // 
            // p_MxOpenStatus
            // 
            this.p_MxOpenStatus.BackColor = System.Drawing.Color.Lime;
            this.p_MxOpenStatus.Controls.Add(this.btn_MxOpen);
            this.p_MxOpenStatus.Location = new System.Drawing.Point(630, 500);
            this.p_MxOpenStatus.Name = "p_MxOpenStatus";
            this.p_MxOpenStatus.Size = new System.Drawing.Size(147, 38);
            this.p_MxOpenStatus.TabIndex = 5;
            // 
            // btn_MxOpen
            // 
            this.btn_MxOpen.Location = new System.Drawing.Point(8, 3);
            this.btn_MxOpen.Name = "btn_MxOpen";
            this.btn_MxOpen.Size = new System.Drawing.Size(128, 32);
            this.btn_MxOpen.TabIndex = 4;
            this.btn_MxOpen.Text = "PLC連結中";
            this.btn_MxOpen.UseVisualStyleBackColor = true;
            this.btn_MxOpen.Click += new System.EventHandler(this.btn_MxOpen_Click);
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1503, 581);
            this.Controls.Add(this.txt_ReadTime);
            this.Controls.Add(this.btn_DataUpload);
            this.Controls.Add(this.p_MxOpenStatus);
            this.Controls.Add(this.p_ModelChange);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main_Form";
            this.Text = "DeviceDataView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_Form_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ReadDataGrid)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_WriteDataGrid)).EndInit();
            this.p_ModelChange.ResumeLayout(false);
            this.p_MxOpenStatus.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_ReadDataGrid;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 檔案ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mi_PLCSet;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgv_WriteDataGrid;
        private System.Windows.Forms.ToolStripMenuItem mi_DataGridSave;
        private System.Windows.Forms.ToolStripMenuItem mi_DataGridLoad;
        private System.Windows.Forms.Button btn_ModelChange;
        private System.Windows.Forms.Panel p_ModelChange;
        private System.Windows.Forms.Button btn_DataUpload;
        internal System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_ReadTime;
        internal System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel p_MxOpenStatus;
        private System.Windows.Forms.Button btn_MxOpen;
        private System.Windows.Forms.DataGridViewTextBoxColumn Read_SN;
        private System.Windows.Forms.DataGridViewTextBoxColumn Read_Label;
        private System.Windows.Forms.DataGridViewTextBoxColumn Read_Address;
        private System.Windows.Forms.DataGridViewTextBoxColumn Read_DataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Read_Data;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Read_IsUse;
        private System.Windows.Forms.DataGridViewTextBoxColumn Read_DeviceValueGet;
        private System.Windows.Forms.DataGridViewTextBoxColumn Write_SN;
        private System.Windows.Forms.DataGridViewTextBoxColumn Write_Label;
        private System.Windows.Forms.DataGridViewTextBoxColumn Write_Address;
        private System.Windows.Forms.DataGridViewTextBoxColumn Write_DataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Write_Data;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Write_IsUse;
        private System.Windows.Forms.DataGridViewTextBoxColumn Write_DeviceValueGet;
        private System.Windows.Forms.DataGridViewTextBoxColumn Write_DeviceValueSet;
    }
}

