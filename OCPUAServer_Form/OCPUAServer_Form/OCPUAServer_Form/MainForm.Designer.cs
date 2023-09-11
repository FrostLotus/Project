
namespace OCPUAServer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Btn_Stop = new System.Windows.Forms.Button();
            this.Btn_Run = new System.Windows.Forms.Button();
            this.Lab_Status = new System.Windows.Forms.Label();
            this.Btn_Close = new System.Windows.Forms.Button();
            this.Dgv_DataItem = new System.Windows.Forms.DataGridView();
            this.Btn_UpdateFile = new System.Windows.Forms.Button();
            this.Btn_Delete = new System.Windows.Forms.Button();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Cbb_DataType = new System.Windows.Forms.ComboBox();
            this.Txt_Value = new System.Windows.Forms.TextBox();
            this.Txt_Inital = new System.Windows.Forms.TextBox();
            this.Txt_Length = new System.Windows.Forms.TextBox();
            this.Txt_NodeId = new System.Windows.Forms.TextBox();
            this.Txt_ItemName = new System.Windows.Forms.TextBox();
            this.Txt_Variable = new System.Windows.Forms.TextBox();
            this.serverControl1 = new OCPUAServer.ServerControl();
            this.Trv_NodeViewer = new System.Windows.Forms.TreeView();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_DataItem)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Btn_Stop);
            this.groupBox1.Controls.Add(this.Btn_Run);
            this.groupBox1.Controls.Add(this.Lab_Status);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 59);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server狀態";
            // 
            // Btn_Stop
            // 
            this.Btn_Stop.Location = new System.Drawing.Point(130, 22);
            this.Btn_Stop.Name = "Btn_Stop";
            this.Btn_Stop.Size = new System.Drawing.Size(99, 30);
            this.Btn_Stop.TabIndex = 3;
            this.Btn_Stop.Text = "STOP";
            this.Btn_Stop.UseVisualStyleBackColor = true;
            this.Btn_Stop.Click += new System.EventHandler(this.Btn_Stop_Click);
            // 
            // Btn_Run
            // 
            this.Btn_Run.Location = new System.Drawing.Point(15, 21);
            this.Btn_Run.Name = "Btn_Run";
            this.Btn_Run.Size = new System.Drawing.Size(99, 30);
            this.Btn_Run.TabIndex = 3;
            this.Btn_Run.Text = "RUN";
            this.Btn_Run.UseVisualStyleBackColor = true;
            this.Btn_Run.Click += new System.EventHandler(this.Btn_Run_Click);
            // 
            // Lab_Status
            // 
            this.Lab_Status.AutoSize = true;
            this.Lab_Status.Location = new System.Drawing.Point(248, 30);
            this.Lab_Status.Name = "Lab_Status";
            this.Lab_Status.Size = new System.Drawing.Size(73, 12);
            this.Lab_Status.TabIndex = 2;
            this.Lab_Status.Text = "SERVER停止";
            // 
            // Btn_Close
            // 
            this.Btn_Close.Location = new System.Drawing.Point(1095, 584);
            this.Btn_Close.Name = "Btn_Close";
            this.Btn_Close.Size = new System.Drawing.Size(99, 30);
            this.Btn_Close.TabIndex = 23;
            this.Btn_Close.Text = "Close Window";
            this.Btn_Close.UseVisualStyleBackColor = true;
            this.Btn_Close.Click += new System.EventHandler(this.Btn_Close_Click);
            // 
            // Dgv_DataItem
            // 
            this.Dgv_DataItem.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dgv_DataItem.Location = new System.Drawing.Point(678, 240);
            this.Dgv_DataItem.Name = "Dgv_DataItem";
            this.Dgv_DataItem.RowTemplate.Height = 24;
            this.Dgv_DataItem.Size = new System.Drawing.Size(402, 317);
            this.Dgv_DataItem.TabIndex = 24;
            this.Dgv_DataItem.VirtualMode = true;
            // 
            // Btn_UpdateFile
            // 
            this.Btn_UpdateFile.Location = new System.Drawing.Point(1084, 87);
            this.Btn_UpdateFile.Name = "Btn_UpdateFile";
            this.Btn_UpdateFile.Size = new System.Drawing.Size(99, 30);
            this.Btn_UpdateFile.TabIndex = 22;
            this.Btn_UpdateFile.Text = "Update_File";
            this.Btn_UpdateFile.UseVisualStyleBackColor = true;
            this.Btn_UpdateFile.Click += new System.EventHandler(this.Btn_UpdateFile_Click);
            // 
            // Btn_Delete
            // 
            this.Btn_Delete.Location = new System.Drawing.Point(1084, 51);
            this.Btn_Delete.Name = "Btn_Delete";
            this.Btn_Delete.Size = new System.Drawing.Size(99, 30);
            this.Btn_Delete.TabIndex = 21;
            this.Btn_Delete.Text = "Delete";
            this.Btn_Delete.UseVisualStyleBackColor = true;
            this.Btn_Delete.Click += new System.EventHandler(this.Btn_Delete_Click);
            // 
            // Btn_Add
            // 
            this.Btn_Add.Location = new System.Drawing.Point(1084, 15);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(99, 30);
            this.Btn_Add.TabIndex = 20;
            this.Btn_Add.Text = "Add";
            this.Btn_Add.UseVisualStyleBackColor = true;
            this.Btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(676, 194);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 12);
            this.label7.TabIndex = 18;
            this.label7.Text = "Value";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(676, 166);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 12);
            this.label6.TabIndex = 17;
            this.label6.Text = "Initial";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(676, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 12);
            this.label5.TabIndex = 19;
            this.label5.Text = "Length";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(676, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(676, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "Node Id";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(676, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "Item Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(676, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "Var Num";
            // 
            // Cbb_DataType
            // 
            this.Cbb_DataType.FormattingEnabled = true;
            this.Cbb_DataType.Items.AddRange(new object[] {
            "String",
            "Word",
            "Bool",
            "Real"});
            this.Cbb_DataType.Location = new System.Drawing.Point(743, 109);
            this.Cbb_DataType.Name = "Cbb_DataType";
            this.Cbb_DataType.Size = new System.Drawing.Size(321, 20);
            this.Cbb_DataType.TabIndex = 12;
            // 
            // Txt_Value
            // 
            this.Txt_Value.Location = new System.Drawing.Point(743, 191);
            this.Txt_Value.Name = "Txt_Value";
            this.Txt_Value.Size = new System.Drawing.Size(321, 22);
            this.Txt_Value.TabIndex = 10;
            // 
            // Txt_Inital
            // 
            this.Txt_Inital.Location = new System.Drawing.Point(743, 163);
            this.Txt_Inital.Name = "Txt_Inital";
            this.Txt_Inital.Size = new System.Drawing.Size(321, 22);
            this.Txt_Inital.TabIndex = 9;
            // 
            // Txt_Length
            // 
            this.Txt_Length.Location = new System.Drawing.Point(743, 135);
            this.Txt_Length.Name = "Txt_Length";
            this.Txt_Length.Size = new System.Drawing.Size(321, 22);
            this.Txt_Length.TabIndex = 8;
            // 
            // Txt_NodeId
            // 
            this.Txt_NodeId.Location = new System.Drawing.Point(743, 81);
            this.Txt_NodeId.Name = "Txt_NodeId";
            this.Txt_NodeId.Size = new System.Drawing.Size(321, 22);
            this.Txt_NodeId.TabIndex = 7;
            // 
            // Txt_ItemName
            // 
            this.Txt_ItemName.Location = new System.Drawing.Point(743, 49);
            this.Txt_ItemName.Name = "Txt_ItemName";
            this.Txt_ItemName.Size = new System.Drawing.Size(321, 22);
            this.Txt_ItemName.TabIndex = 11;
            // 
            // Txt_Variable
            // 
            this.Txt_Variable.Location = new System.Drawing.Point(743, 21);
            this.Txt_Variable.Name = "Txt_Variable";
            this.Txt_Variable.Size = new System.Drawing.Size(321, 22);
            this.Txt_Variable.TabIndex = 6;
            // 
            // serverControl1
            // 
            this.serverControl1.Location = new System.Drawing.Point(12, 81);
            this.serverControl1.Name = "serverControl1";
            this.serverControl1.Size = new System.Drawing.Size(488, 322);
            this.serverControl1.TabIndex = 27;
            // 
            // Trv_NodeViewer
            // 
            this.Trv_NodeViewer.Location = new System.Drawing.Point(27, 409);
            this.Trv_NodeViewer.Name = "Trv_NodeViewer";
            this.Trv_NodeViewer.Size = new System.Drawing.Size(452, 205);
            this.Trv_NodeViewer.TabIndex = 28;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1206, 626);
            this.Controls.Add(this.Trv_NodeViewer);
            this.Controls.Add(this.serverControl1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Btn_Close);
            this.Controls.Add(this.Dgv_DataItem);
            this.Controls.Add(this.Btn_UpdateFile);
            this.Controls.Add(this.Btn_Delete);
            this.Controls.Add(this.Btn_Add);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cbb_DataType);
            this.Controls.Add(this.Txt_Value);
            this.Controls.Add(this.Txt_Inital);
            this.Controls.Add(this.Txt_Length);
            this.Controls.Add(this.Txt_NodeId);
            this.Controls.Add(this.Txt_ItemName);
            this.Controls.Add(this.Txt_Variable);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_DataItem)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Btn_Stop;
        private System.Windows.Forms.Button Btn_Run;
        private System.Windows.Forms.Label Lab_Status;
        private System.Windows.Forms.Button Btn_Close;
        private System.Windows.Forms.DataGridView Dgv_DataItem;
        private System.Windows.Forms.Button Btn_UpdateFile;
        private System.Windows.Forms.Button Btn_Delete;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox Cbb_DataType;
        private System.Windows.Forms.TextBox Txt_Value;
        private System.Windows.Forms.TextBox Txt_Inital;
        private System.Windows.Forms.TextBox Txt_Length;
        private System.Windows.Forms.TextBox Txt_NodeId;
        private System.Windows.Forms.TextBox Txt_ItemName;
        private System.Windows.Forms.TextBox Txt_Variable;
        private ServerControl serverControl1;
        private System.Windows.Forms.TreeView Trv_NodeViewer;
    }
}