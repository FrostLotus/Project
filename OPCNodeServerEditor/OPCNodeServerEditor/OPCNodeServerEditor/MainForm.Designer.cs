
namespace OPCNodeServerEditor
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
            this.Btn_Close = new System.Windows.Forms.Button();
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
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Btn_Stop = new System.Windows.Forms.Button();
            this.Btn_Run = new System.Windows.Forms.Button();
            this.Lab_Status = new System.Windows.Forms.Label();
            this.Gbx_NodeList = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.Gbx_NodeList.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Btn_Close
            // 
            this.Btn_Close.Location = new System.Drawing.Point(7, 25);
            this.Btn_Close.Name = "Btn_Close";
            this.Btn_Close.Size = new System.Drawing.Size(99, 30);
            this.Btn_Close.TabIndex = 42;
            this.Btn_Close.Text = "Close Window";
            this.Btn_Close.UseVisualStyleBackColor = true;
            // 
            // Btn_UpdateFile
            // 
            this.Btn_UpdateFile.Location = new System.Drawing.Point(443, 474);
            this.Btn_UpdateFile.Name = "Btn_UpdateFile";
            this.Btn_UpdateFile.Size = new System.Drawing.Size(99, 30);
            this.Btn_UpdateFile.TabIndex = 41;
            this.Btn_UpdateFile.Text = "Update_File";
            this.Btn_UpdateFile.UseVisualStyleBackColor = true;
            // 
            // Btn_Delete
            // 
            this.Btn_Delete.Location = new System.Drawing.Point(443, 438);
            this.Btn_Delete.Name = "Btn_Delete";
            this.Btn_Delete.Size = new System.Drawing.Size(99, 30);
            this.Btn_Delete.TabIndex = 40;
            this.Btn_Delete.Text = "Delete";
            this.Btn_Delete.UseVisualStyleBackColor = true;
            // 
            // Btn_Add
            // 
            this.Btn_Add.Location = new System.Drawing.Point(443, 402);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(99, 30);
            this.Btn_Add.TabIndex = 39;
            this.Btn_Add.Text = "Add";
            this.Btn_Add.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("新細明體", 10F);
            this.label7.Location = new System.Drawing.Point(14, 519);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 14);
            this.label7.TabIndex = 37;
            this.label7.Text = "Value";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("新細明體", 10F);
            this.label6.Location = new System.Drawing.Point(13, 490);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 14);
            this.label6.TabIndex = 36;
            this.label6.Text = "Initial";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("新細明體", 10F);
            this.label5.Location = new System.Drawing.Point(236, 460);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 14);
            this.label5.TabIndex = 38;
            this.label5.Text = "Length";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("新細明體", 10F);
            this.label4.Location = new System.Drawing.Point(12, 460);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 14);
            this.label4.TabIndex = 35;
            this.label4.Text = "Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("新細明體", 10F);
            this.label3.Location = new System.Drawing.Point(236, 432);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 14);
            this.label3.TabIndex = 34;
            this.label3.Text = "Node Id";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 10F);
            this.label2.Location = new System.Drawing.Point(12, 432);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 14);
            this.label2.TabIndex = 33;
            this.label2.Text = "Item Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 10F);
            this.label1.Location = new System.Drawing.Point(12, 404);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 14);
            this.label1.TabIndex = 32;
            this.label1.Text = "Index";
            // 
            // Cbb_DataType
            // 
            this.Cbb_DataType.FormattingEnabled = true;
            this.Cbb_DataType.Items.AddRange(new object[] {
            "String",
            "Word",
            "Bool",
            "Real"});
            this.Cbb_DataType.Location = new System.Drawing.Point(84, 457);
            this.Cbb_DataType.Name = "Cbb_DataType";
            this.Cbb_DataType.Size = new System.Drawing.Size(145, 20);
            this.Cbb_DataType.TabIndex = 31;
            // 
            // Txt_Value
            // 
            this.Txt_Value.Location = new System.Drawing.Point(84, 516);
            this.Txt_Value.Name = "Txt_Value";
            this.Txt_Value.Size = new System.Drawing.Size(353, 22);
            this.Txt_Value.TabIndex = 29;
            // 
            // Txt_Inital
            // 
            this.Txt_Inital.Location = new System.Drawing.Point(84, 487);
            this.Txt_Inital.Name = "Txt_Inital";
            this.Txt_Inital.Size = new System.Drawing.Size(353, 22);
            this.Txt_Inital.TabIndex = 28;
            // 
            // Txt_Length
            // 
            this.Txt_Length.Location = new System.Drawing.Point(292, 457);
            this.Txt_Length.Name = "Txt_Length";
            this.Txt_Length.Size = new System.Drawing.Size(145, 22);
            this.Txt_Length.TabIndex = 27;
            // 
            // Txt_NodeId
            // 
            this.Txt_NodeId.Location = new System.Drawing.Point(293, 427);
            this.Txt_NodeId.Name = "Txt_NodeId";
            this.Txt_NodeId.Size = new System.Drawing.Size(144, 22);
            this.Txt_NodeId.TabIndex = 26;
            // 
            // Txt_ItemName
            // 
            this.Txt_ItemName.Location = new System.Drawing.Point(84, 429);
            this.Txt_ItemName.Name = "Txt_ItemName";
            this.Txt_ItemName.Size = new System.Drawing.Size(145, 22);
            this.Txt_ItemName.TabIndex = 30;
            // 
            // Txt_Variable
            // 
            this.Txt_Variable.Location = new System.Drawing.Point(84, 401);
            this.Txt_Variable.Name = "Txt_Variable";
            this.Txt_Variable.Size = new System.Drawing.Size(145, 22);
            this.Txt_Variable.TabIndex = 25;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(443, 510);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(99, 30);
            this.button1.TabIndex = 41;
            this.button1.Text = "Update_Value";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Btn_Stop);
            this.groupBox1.Controls.Add(this.Btn_Run);
            this.groupBox1.Controls.Add(this.Lab_Status);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 59);
            this.groupBox1.TabIndex = 44;
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
            // 
            // Btn_Run
            // 
            this.Btn_Run.Location = new System.Drawing.Point(15, 21);
            this.Btn_Run.Name = "Btn_Run";
            this.Btn_Run.Size = new System.Drawing.Size(99, 30);
            this.Btn_Run.TabIndex = 3;
            this.Btn_Run.Text = "RUN";
            this.Btn_Run.UseVisualStyleBackColor = true;
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
            // Gbx_NodeList
            // 
            this.Gbx_NodeList.Controls.Add(this.Txt_Length);
            this.Gbx_NodeList.Controls.Add(this.treeView1);
            this.Gbx_NodeList.Controls.Add(this.Txt_Variable);
            this.Gbx_NodeList.Controls.Add(this.Txt_ItemName);
            this.Gbx_NodeList.Controls.Add(this.Txt_NodeId);
            this.Gbx_NodeList.Controls.Add(this.button1);
            this.Gbx_NodeList.Controls.Add(this.Txt_Inital);
            this.Gbx_NodeList.Controls.Add(this.Btn_UpdateFile);
            this.Gbx_NodeList.Controls.Add(this.Txt_Value);
            this.Gbx_NodeList.Controls.Add(this.Btn_Delete);
            this.Gbx_NodeList.Controls.Add(this.Cbb_DataType);
            this.Gbx_NodeList.Controls.Add(this.Btn_Add);
            this.Gbx_NodeList.Controls.Add(this.label1);
            this.Gbx_NodeList.Controls.Add(this.label7);
            this.Gbx_NodeList.Controls.Add(this.label2);
            this.Gbx_NodeList.Controls.Add(this.label6);
            this.Gbx_NodeList.Controls.Add(this.label3);
            this.Gbx_NodeList.Controls.Add(this.label5);
            this.Gbx_NodeList.Controls.Add(this.label4);
            this.Gbx_NodeList.Location = new System.Drawing.Point(12, 77);
            this.Gbx_NodeList.Name = "Gbx_NodeList";
            this.Gbx_NodeList.Size = new System.Drawing.Size(551, 552);
            this.Gbx_NodeList.TabIndex = 45;
            this.Gbx_NodeList.TabStop = false;
            this.Gbx_NodeList.Text = "NodeList";
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(6, 21);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(536, 364);
            this.treeView1.TabIndex = 46;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Btn_Close);
            this.groupBox2.Location = new System.Drawing.Point(1096, 627);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(112, 61);
            this.groupBox2.TabIndex = 47;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Form Close";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1248, 700);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.Gbx_NodeList);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.Gbx_NodeList.ResumeLayout(false);
            this.Gbx_NodeList.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Btn_Close;
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Btn_Stop;
        private System.Windows.Forms.Button Btn_Run;
        private System.Windows.Forms.Label Lab_Status;
        private System.Windows.Forms.GroupBox Gbx_NodeList;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

