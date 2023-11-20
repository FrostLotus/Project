
namespace SocketConnect
{
    partial class Form1
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
            this.Btn_UnP_Connect = new System.Windows.Forms.Button();
            this.Btn_UnP_Disconnect = new System.Windows.Forms.Button();
            this.Txt_UnP_ReadData = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Btn_UnP_Write = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Txt_UnP_WriteData = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Btn_FuP_Write = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Btn_FuP_Connect = new System.Windows.Forms.Button();
            this.Btn_FuP_Disconnect = new System.Windows.Forms.Button();
            this.Txt_FuP_WriteData = new System.Windows.Forms.TextBox();
            this.Txt_FuP_ReadData = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Btn_Act_Write = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Btn_Act_Connect = new System.Windows.Forms.Button();
            this.Btn_Act_Disconnect = new System.Windows.Forms.Button();
            this.Txt_Act_WriteData = new System.Windows.Forms.TextBox();
            this.Txt_Act_ReadData = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.Btn_Act_Thread_Disconnect = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.Btn_Act_Thread_Write = new System.Windows.Forms.Button();
            this.Btn_Act_Thread_Connect = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.Txt_Act_Thread_ReadData = new System.Windows.Forms.TextBox();
            this.Txt_Act_Thread_WriteData = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // Btn_UnP_Connect
            // 
            this.Btn_UnP_Connect.Location = new System.Drawing.Point(6, 21);
            this.Btn_UnP_Connect.Name = "Btn_UnP_Connect";
            this.Btn_UnP_Connect.Size = new System.Drawing.Size(91, 52);
            this.Btn_UnP_Connect.TabIndex = 0;
            this.Btn_UnP_Connect.Text = "連線";
            this.Btn_UnP_Connect.UseVisualStyleBackColor = true;
            this.Btn_UnP_Connect.Click += new System.EventHandler(this.Btn_UnP_Connect_Click);
            // 
            // Btn_UnP_Disconnect
            // 
            this.Btn_UnP_Disconnect.Location = new System.Drawing.Point(247, 21);
            this.Btn_UnP_Disconnect.Name = "Btn_UnP_Disconnect";
            this.Btn_UnP_Disconnect.Size = new System.Drawing.Size(91, 52);
            this.Btn_UnP_Disconnect.TabIndex = 0;
            this.Btn_UnP_Disconnect.Text = "離線";
            this.Btn_UnP_Disconnect.UseVisualStyleBackColor = true;
            this.Btn_UnP_Disconnect.Click += new System.EventHandler(this.Btn_UnP_Disconnect_Click);
            // 
            // Txt_UnP_ReadData
            // 
            this.Txt_UnP_ReadData.AcceptsReturn = true;
            this.Txt_UnP_ReadData.Location = new System.Drawing.Point(47, 82);
            this.Txt_UnP_ReadData.Multiline = true;
            this.Txt_UnP_ReadData.Name = "Txt_UnP_ReadData";
            this.Txt_UnP_ReadData.ReadOnly = true;
            this.Txt_UnP_ReadData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Txt_UnP_ReadData.Size = new System.Drawing.Size(239, 127);
            this.Txt_UnP_ReadData.TabIndex = 51;
            this.Txt_UnP_ReadData.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.groupBox1.Controls.Add(this.Btn_UnP_Write);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.Btn_UnP_Connect);
            this.groupBox1.Controls.Add(this.Btn_UnP_Disconnect);
            this.groupBox1.Controls.Add(this.Txt_UnP_WriteData);
            this.groupBox1.Controls.Add(this.Txt_UnP_ReadData);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(344, 428);
            this.groupBox1.TabIndex = 52;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "UnPassive";
            // 
            // Btn_UnP_Write
            // 
            this.Btn_UnP_Write.Location = new System.Drawing.Point(128, 363);
            this.Btn_UnP_Write.Name = "Btn_UnP_Write";
            this.Btn_UnP_Write.Size = new System.Drawing.Size(91, 52);
            this.Btn_UnP_Write.TabIndex = 53;
            this.Btn_UnP_Write.Text = "輸出";
            this.Btn_UnP_Write.UseVisualStyleBackColor = true;
            this.Btn_UnP_Write.Click += new System.EventHandler(this.Btn_UnP_Write_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(107, 215);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 12);
            this.label2.TabIndex = 52;
            this.label2.Text = "請輸入 資料使PLC接收";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(112, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 12);
            this.label1.TabIndex = 52;
            this.label1.Text = "請觸發 M300接收資料";
            // 
            // Txt_UnP_WriteData
            // 
            this.Txt_UnP_WriteData.AcceptsReturn = true;
            this.Txt_UnP_WriteData.Location = new System.Drawing.Point(47, 230);
            this.Txt_UnP_WriteData.Multiline = true;
            this.Txt_UnP_WriteData.Name = "Txt_UnP_WriteData";
            this.Txt_UnP_WriteData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Txt_UnP_WriteData.Size = new System.Drawing.Size(239, 127);
            this.Txt_UnP_WriteData.TabIndex = 51;
            this.Txt_UnP_WriteData.TabStop = false;
            this.Txt_UnP_WriteData.Text = "1234567890";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Btn_FuP_Write);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.Btn_FuP_Connect);
            this.groupBox2.Controls.Add(this.Btn_FuP_Disconnect);
            this.groupBox2.Controls.Add(this.Txt_FuP_WriteData);
            this.groupBox2.Controls.Add(this.Txt_FuP_ReadData);
            this.groupBox2.Location = new System.Drawing.Point(362, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(344, 428);
            this.groupBox2.TabIndex = 52;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "FullPassive";
            // 
            // Btn_FuP_Write
            // 
            this.Btn_FuP_Write.Location = new System.Drawing.Point(117, 363);
            this.Btn_FuP_Write.Name = "Btn_FuP_Write";
            this.Btn_FuP_Write.Size = new System.Drawing.Size(91, 52);
            this.Btn_FuP_Write.TabIndex = 53;
            this.Btn_FuP_Write.Text = "輸出";
            this.Btn_FuP_Write.UseVisualStyleBackColor = true;
            this.Btn_FuP_Write.Click += new System.EventHandler(this.Btn_FuP_Write_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(110, 215);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 12);
            this.label3.TabIndex = 52;
            this.label3.Text = "請輸入 資料使PLC接收";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(115, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 12);
            this.label4.TabIndex = 52;
            this.label4.Text = "請觸發 M320接收資料";
            // 
            // Btn_FuP_Connect
            // 
            this.Btn_FuP_Connect.Location = new System.Drawing.Point(6, 21);
            this.Btn_FuP_Connect.Name = "Btn_FuP_Connect";
            this.Btn_FuP_Connect.Size = new System.Drawing.Size(91, 52);
            this.Btn_FuP_Connect.TabIndex = 0;
            this.Btn_FuP_Connect.Text = "連線";
            this.Btn_FuP_Connect.UseVisualStyleBackColor = true;
            this.Btn_FuP_Connect.Click += new System.EventHandler(this.Btn_FuP_Connect_Click);
            // 
            // Btn_FuP_Disconnect
            // 
            this.Btn_FuP_Disconnect.Location = new System.Drawing.Point(247, 21);
            this.Btn_FuP_Disconnect.Name = "Btn_FuP_Disconnect";
            this.Btn_FuP_Disconnect.Size = new System.Drawing.Size(91, 52);
            this.Btn_FuP_Disconnect.TabIndex = 0;
            this.Btn_FuP_Disconnect.Text = "離線";
            this.Btn_FuP_Disconnect.UseVisualStyleBackColor = true;
            this.Btn_FuP_Disconnect.Click += new System.EventHandler(this.Btn_FuP_Disconnect_Click);
            // 
            // Txt_FuP_WriteData
            // 
            this.Txt_FuP_WriteData.AcceptsReturn = true;
            this.Txt_FuP_WriteData.Location = new System.Drawing.Point(47, 230);
            this.Txt_FuP_WriteData.Multiline = true;
            this.Txt_FuP_WriteData.Name = "Txt_FuP_WriteData";
            this.Txt_FuP_WriteData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Txt_FuP_WriteData.Size = new System.Drawing.Size(239, 127);
            this.Txt_FuP_WriteData.TabIndex = 51;
            this.Txt_FuP_WriteData.TabStop = false;
            this.Txt_FuP_WriteData.Text = "1234567890";
            // 
            // Txt_FuP_ReadData
            // 
            this.Txt_FuP_ReadData.AcceptsReturn = true;
            this.Txt_FuP_ReadData.Location = new System.Drawing.Point(47, 79);
            this.Txt_FuP_ReadData.Multiline = true;
            this.Txt_FuP_ReadData.Name = "Txt_FuP_ReadData";
            this.Txt_FuP_ReadData.ReadOnly = true;
            this.Txt_FuP_ReadData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Txt_FuP_ReadData.Size = new System.Drawing.Size(239, 127);
            this.Txt_FuP_ReadData.TabIndex = 51;
            this.Txt_FuP_ReadData.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox3.Controls.Add(this.Btn_Act_Write);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.Btn_Act_Connect);
            this.groupBox3.Controls.Add(this.Btn_Act_Disconnect);
            this.groupBox3.Controls.Add(this.Txt_Act_WriteData);
            this.groupBox3.Controls.Add(this.Txt_Act_ReadData);
            this.groupBox3.Location = new System.Drawing.Point(712, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(344, 428);
            this.groupBox3.TabIndex = 52;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Active";
            // 
            // Btn_Act_Write
            // 
            this.Btn_Act_Write.Location = new System.Drawing.Point(121, 363);
            this.Btn_Act_Write.Name = "Btn_Act_Write";
            this.Btn_Act_Write.Size = new System.Drawing.Size(91, 52);
            this.Btn_Act_Write.TabIndex = 53;
            this.Btn_Act_Write.Text = "輸出";
            this.Btn_Act_Write.UseVisualStyleBackColor = true;
            this.Btn_Act_Write.Click += new System.EventHandler(this.Btn_Act_Write_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(103, 215);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 12);
            this.label5.TabIndex = 52;
            this.label5.Text = "請輸入 資料使PLC接收";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(103, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 12);
            this.label6.TabIndex = 52;
            this.label6.Text = "請觸發 M340接收資料";
            // 
            // Btn_Act_Connect
            // 
            this.Btn_Act_Connect.Location = new System.Drawing.Point(6, 21);
            this.Btn_Act_Connect.Name = "Btn_Act_Connect";
            this.Btn_Act_Connect.Size = new System.Drawing.Size(91, 52);
            this.Btn_Act_Connect.TabIndex = 0;
            this.Btn_Act_Connect.Text = "監聽";
            this.Btn_Act_Connect.UseVisualStyleBackColor = true;
            this.Btn_Act_Connect.Click += new System.EventHandler(this.Btn_Act_Connect_Click);
            // 
            // Btn_Act_Disconnect
            // 
            this.Btn_Act_Disconnect.Location = new System.Drawing.Point(247, 21);
            this.Btn_Act_Disconnect.Name = "Btn_Act_Disconnect";
            this.Btn_Act_Disconnect.Size = new System.Drawing.Size(91, 52);
            this.Btn_Act_Disconnect.TabIndex = 0;
            this.Btn_Act_Disconnect.Text = "離線";
            this.Btn_Act_Disconnect.UseVisualStyleBackColor = true;
            this.Btn_Act_Disconnect.Click += new System.EventHandler(this.Btn_Act_Disconnect_Click);
            // 
            // Txt_Act_WriteData
            // 
            this.Txt_Act_WriteData.AcceptsReturn = true;
            this.Txt_Act_WriteData.Location = new System.Drawing.Point(47, 230);
            this.Txt_Act_WriteData.Multiline = true;
            this.Txt_Act_WriteData.Name = "Txt_Act_WriteData";
            this.Txt_Act_WriteData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Txt_Act_WriteData.Size = new System.Drawing.Size(239, 127);
            this.Txt_Act_WriteData.TabIndex = 51;
            this.Txt_Act_WriteData.TabStop = false;
            this.Txt_Act_WriteData.Text = "1234567890";
            // 
            // Txt_Act_ReadData
            // 
            this.Txt_Act_ReadData.AcceptsReturn = true;
            this.Txt_Act_ReadData.Location = new System.Drawing.Point(47, 79);
            this.Txt_Act_ReadData.Multiline = true;
            this.Txt_Act_ReadData.Name = "Txt_Act_ReadData";
            this.Txt_Act_ReadData.ReadOnly = true;
            this.Txt_Act_ReadData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Txt_Act_ReadData.Size = new System.Drawing.Size(239, 127);
            this.Txt_Act_ReadData.TabIndex = 51;
            this.Txt_Act_ReadData.TabStop = false;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.Btn_Act_Thread_Disconnect);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.Btn_Act_Thread_Write);
            this.groupBox6.Controls.Add(this.Btn_Act_Thread_Connect);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Controls.Add(this.Txt_Act_Thread_ReadData);
            this.groupBox6.Controls.Add(this.Txt_Act_Thread_WriteData);
            this.groupBox6.Location = new System.Drawing.Point(1062, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(344, 428);
            this.groupBox6.TabIndex = 53;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "groupBox4";
            // 
            // Btn_Act_Thread_Disconnect
            // 
            this.Btn_Act_Thread_Disconnect.Location = new System.Drawing.Point(242, 21);
            this.Btn_Act_Thread_Disconnect.Name = "Btn_Act_Thread_Disconnect";
            this.Btn_Act_Thread_Disconnect.Size = new System.Drawing.Size(96, 37);
            this.Btn_Act_Thread_Disconnect.TabIndex = 0;
            this.Btn_Act_Thread_Disconnect.Text = "離線";
            this.Btn_Act_Thread_Disconnect.UseVisualStyleBackColor = true;
            this.Btn_Act_Thread_Disconnect.Click += new System.EventHandler(this.Btn_Act_Thread_Disconnect_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(98, 216);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(125, 12);
            this.label12.TabIndex = 52;
            this.label12.Text = "請輸入 資料使PLC接收";
            // 
            // Btn_Act_Thread_Write
            // 
            this.Btn_Act_Thread_Write.Location = new System.Drawing.Point(121, 364);
            this.Btn_Act_Thread_Write.Name = "Btn_Act_Thread_Write";
            this.Btn_Act_Thread_Write.Size = new System.Drawing.Size(91, 37);
            this.Btn_Act_Thread_Write.TabIndex = 0;
            this.Btn_Act_Thread_Write.Text = "輸出";
            this.Btn_Act_Thread_Write.UseVisualStyleBackColor = true;
            this.Btn_Act_Thread_Write.Click += new System.EventHandler(this.Btn_Act_Thread_Write_Click);
            // 
            // Btn_Act_Thread_Connect
            // 
            this.Btn_Act_Thread_Connect.Location = new System.Drawing.Point(6, 21);
            this.Btn_Act_Thread_Connect.Name = "Btn_Act_Thread_Connect";
            this.Btn_Act_Thread_Connect.Size = new System.Drawing.Size(96, 37);
            this.Btn_Act_Thread_Connect.TabIndex = 0;
            this.Btn_Act_Thread_Connect.Text = "連線";
            this.Btn_Act_Thread_Connect.UseVisualStyleBackColor = true;
            this.Btn_Act_Thread_Connect.Click += new System.EventHandler(this.Btn_Act_Thread_Connect_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(103, 56);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(120, 12);
            this.label9.TabIndex = 52;
            this.label9.Text = "請觸發 M340接收資料";
            // 
            // Txt_Act_Thread_ReadData
            // 
            this.Txt_Act_Thread_ReadData.AcceptsReturn = true;
            this.Txt_Act_Thread_ReadData.Location = new System.Drawing.Point(47, 86);
            this.Txt_Act_Thread_ReadData.Multiline = true;
            this.Txt_Act_Thread_ReadData.Name = "Txt_Act_Thread_ReadData";
            this.Txt_Act_Thread_ReadData.ReadOnly = true;
            this.Txt_Act_Thread_ReadData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Txt_Act_Thread_ReadData.Size = new System.Drawing.Size(239, 127);
            this.Txt_Act_Thread_ReadData.TabIndex = 51;
            this.Txt_Act_Thread_ReadData.TabStop = false;
            // 
            // Txt_Act_Thread_WriteData
            // 
            this.Txt_Act_Thread_WriteData.AcceptsReturn = true;
            this.Txt_Act_Thread_WriteData.Location = new System.Drawing.Point(47, 231);
            this.Txt_Act_Thread_WriteData.Multiline = true;
            this.Txt_Act_Thread_WriteData.Name = "Txt_Act_Thread_WriteData";
            this.Txt_Act_Thread_WriteData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Txt_Act_Thread_WriteData.Size = new System.Drawing.Size(239, 127);
            this.Txt_Act_Thread_WriteData.TabIndex = 51;
            this.Txt_Act_Thread_WriteData.TabStop = false;
            this.Txt_Act_Thread_WriteData.Text = "1234567890";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1416, 455);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Btn_UnP_Connect;
        private System.Windows.Forms.Button Btn_UnP_Disconnect;
        internal System.Windows.Forms.TextBox Txt_UnP_ReadData;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Btn_UnP_Write;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox Txt_UnP_WriteData;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Btn_FuP_Write;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Btn_FuP_Connect;
        private System.Windows.Forms.Button Btn_FuP_Disconnect;
        internal System.Windows.Forms.TextBox Txt_FuP_WriteData;
        internal System.Windows.Forms.TextBox Txt_FuP_ReadData;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button Btn_Act_Write;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button Btn_Act_Connect;
        private System.Windows.Forms.Button Btn_Act_Disconnect;
        internal System.Windows.Forms.TextBox Txt_Act_WriteData;
        internal System.Windows.Forms.TextBox Txt_Act_ReadData;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button Btn_Act_Thread_Disconnect;
        private System.Windows.Forms.Button Btn_Act_Thread_Connect;
        private System.Windows.Forms.Label label9;
        internal System.Windows.Forms.TextBox Txt_Act_Thread_ReadData;
        private System.Windows.Forms.Label label12;
        internal System.Windows.Forms.TextBox Txt_Act_Thread_WriteData;
        private System.Windows.Forms.Button Btn_Act_Thread_Write;
    }
}

