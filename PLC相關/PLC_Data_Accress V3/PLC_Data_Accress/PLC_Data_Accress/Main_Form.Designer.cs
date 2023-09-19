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
            this.SN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Label = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Data = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsUse = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DeviceValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.檔案ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_DataGridLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_DataGridSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_PLCSet = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_WriteDataGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SetDataValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_ModelChange = new System.Windows.Forms.Button();
            this.p_ModelChange = new System.Windows.Forms.Panel();
            this.btn_DataUpload = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_ReadTime = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.p_MxOpenStatus = new System.Windows.Forms.Panel();
            this.btn_MxOpen = new System.Windows.Forms.Button();
            this.Lb_Status = new System.Windows.Forms.Label();
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
            this.SN,
            this.Label,
            this.Address,
            this.DataType,
            this.Data,
            this.IsUse,
            this.DeviceValue});
            this.dgv_ReadDataGrid.Location = new System.Drawing.Point(6, 21);
            this.dgv_ReadDataGrid.Name = "dgv_ReadDataGrid";
            this.dgv_ReadDataGrid.RowHeadersVisible = false;
            this.dgv_ReadDataGrid.RowTemplate.Height = 24;
            this.dgv_ReadDataGrid.Size = new System.Drawing.Size(682, 420);
            this.dgv_ReadDataGrid.TabIndex = 0;
            // 
            // SN
            // 
            this.SN.HeaderText = "序号";
            this.SN.Name = "SN";
            this.SN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SN.Width = 60;
            // 
            // Label
            // 
            this.Label.HeaderText = "名称";
            this.Label.Name = "Label";
            this.Label.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Label.Width = 160;
            // 
            // Address
            // 
            this.Address.HeaderText = "地址";
            this.Address.Name = "Address";
            this.Address.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // DataType
            // 
            this.DataType.HeaderText = "数据类型";
            this.DataType.Name = "DataType";
            this.DataType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.DataType.Width = 60;
            // 
            // Data
            // 
            this.Data.HeaderText = "变量";
            this.Data.Name = "Data";
            this.Data.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Data.Width = 80;
            // 
            // IsUse
            // 
            this.IsUse.HeaderText = "是否使用";
            this.IsUse.Name = "IsUse";
            this.IsUse.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.IsUse.Width = 60;
            // 
            // DeviceValue
            // 
            this.DeviceValue.HeaderText = "軟元件當前值";
            this.DeviceValue.Name = "DeviceValue";
            this.DeviceValue.Width = 150;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.檔案ToolStripMenuItem,
            this.mi_PLCSet});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1748, 24);
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
            this.mi_DataGridLoad.Click += new System.EventHandler(this.mi_DataGridLoad_Click);
            // 
            // mi_DataGridSave
            // 
            this.mi_DataGridSave.Name = "mi_DataGridSave";
            this.mi_DataGridSave.Size = new System.Drawing.Size(122, 22);
            this.mi_DataGridSave.Text = "格式儲存";
            this.mi_DataGridSave.Click += new System.EventHandler(this.mi_DataGridSave_Click);
            // 
            // mi_PLCSet
            // 
            this.mi_PLCSet.Name = "mi_PLCSet";
            this.mi_PLCSet.Size = new System.Drawing.Size(64, 20);
            this.mi_PLCSet.Text = "PLC設定";
            this.mi_PLCSet.Click += new System.EventHandler(this.mi_PLCSet_Click);
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
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewCheckBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.SetDataValue});
            this.dgv_WriteDataGrid.Location = new System.Drawing.Point(6, 21);
            this.dgv_WriteDataGrid.Name = "dgv_WriteDataGrid";
            this.dgv_WriteDataGrid.RowHeadersVisible = false;
            this.dgv_WriteDataGrid.RowTemplate.Height = 24;
            this.dgv_WriteDataGrid.Size = new System.Drawing.Size(771, 420);
            this.dgv_WriteDataGrid.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "序号";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 40;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "名称";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn3.Width = 130;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "地址";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "数据类型";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn5.Width = 60;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "变量";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn6.Width = 80;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.HeaderText = "是否使用";
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            this.dataGridViewCheckBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewCheckBoxColumn1.Width = 60;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "軟元件當前值";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 150;
            // 
            // SetDataValue
            // 
            this.SetDataValue.HeaderText = "修改值(可監視中易值)";
            this.SetDataValue.Name = "SetDataValue";
            this.SetDataValue.Width = 145;
            // 
            // btn_ModelChange
            // 
            this.btn_ModelChange.Location = new System.Drawing.Point(20, 6);
            this.btn_ModelChange.Name = "btn_ModelChange";
            this.btn_ModelChange.Size = new System.Drawing.Size(131, 52);
            this.btn_ModelChange.TabIndex = 4;
            this.btn_ModelChange.Text = "格式修改模式中";
            this.btn_ModelChange.UseVisualStyleBackColor = true;
            this.btn_ModelChange.Click += new System.EventHandler(this.btn_ModelChange_Click);
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
            this.btn_DataUpload.Click += new System.EventHandler(this.btn_DataUpdate_Click);
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
            this.txt_ReadTime.Location = new System.Drawing.Point(32, 605);
            this.txt_ReadTime.Name = "txt_ReadTime";
            this.txt_ReadTime.Size = new System.Drawing.Size(123, 22);
            this.txt_ReadTime.TabIndex = 1;
            this.txt_ReadTime.Text = "2000";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(30, 586);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 16);
            this.label2.TabIndex = 50;
            this.label2.Text = "讀取回傳時間設定(ms)";
            // 
            // p_MxOpenStatus
            // 
            this.p_MxOpenStatus.BackColor = System.Drawing.Color.Lime;
            this.p_MxOpenStatus.Controls.Add(this.btn_MxOpen);
            this.p_MxOpenStatus.Location = new System.Drawing.Point(1342, 596);
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
            // 
            // Lb_Status
            // 
            this.Lb_Status.AutoSize = true;
            this.Lb_Status.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Lb_Status.Location = new System.Drawing.Point(682, 506);
            this.Lb_Status.Name = "Lb_Status";
            this.Lb_Status.Size = new System.Drawing.Size(51, 16);
            this.Lb_Status.TabIndex = 75;
            this.Lb_Status.Text = "Status";
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1748, 826);
            this.Controls.Add(this.Lb_Status);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn SetDataValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn SN;
        private System.Windows.Forms.DataGridViewTextBoxColumn Label;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Data;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsUse;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceValue;
        private System.Windows.Forms.Label Lb_Status;
    }
}

