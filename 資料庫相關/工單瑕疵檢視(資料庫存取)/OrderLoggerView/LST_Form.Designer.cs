
namespace OrderLoggerView
{
    partial class LST_Form
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
            this.label1 = new System.Windows.Forms.Label();
            this.Grb_Search = new System.Windows.Forms.GroupBox();
            this.Btn_Clean = new System.Windows.Forms.Button();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Txt_Search = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Grb_List = new System.Windows.Forms.GroupBox();
            this.Dgv_Order = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Btn_Login = new System.Windows.Forms.Button();
            this.Lab_LoginInfo = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Txt_Password = new System.Windows.Forms.TextBox();
            this.Txt_User = new System.Windows.Forms.TextBox();
            this.Txt_Port = new System.Windows.Forms.TextBox();
            this.Txt_Host = new System.Windows.Forms.TextBox();
            this.Grb_Search.SuspendLayout();
            this.panel1.SuspendLayout();
            this.Grb_List.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_Order)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 61);
            this.label1.TabIndex = 0;
            this.label1.Text = "檢測作業";
            // 
            // Grb_Search
            // 
            this.Grb_Search.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Grb_Search.Controls.Add(this.Btn_Clean);
            this.Grb_Search.Controls.Add(this.Btn_Search);
            this.Grb_Search.Controls.Add(this.panel1);
            this.Grb_Search.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Grb_Search.Location = new System.Drawing.Point(677, 12);
            this.Grb_Search.Name = "Grb_Search";
            this.Grb_Search.Size = new System.Drawing.Size(640, 96);
            this.Grb_Search.TabIndex = 1;
            this.Grb_Search.TabStop = false;
            this.Grb_Search.Text = "搜索";
            this.Grb_Search.Visible = false;
            // 
            // Btn_Clean
            // 
            this.Btn_Clean.Location = new System.Drawing.Point(522, 29);
            this.Btn_Clean.Name = "Btn_Clean";
            this.Btn_Clean.Size = new System.Drawing.Size(75, 44);
            this.Btn_Clean.TabIndex = 1;
            this.Btn_Clean.Text = "清除";
            this.Btn_Clean.UseVisualStyleBackColor = true;
            this.Btn_Clean.Click += new System.EventHandler(this.Btn_Clean_Click);
            // 
            // Btn_Search
            // 
            this.Btn_Search.Location = new System.Drawing.Point(414, 29);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(75, 44);
            this.Btn_Search.TabIndex = 1;
            this.Btn_Search.Text = "搜索";
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Txt_Search);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(38, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(341, 49);
            this.panel1.TabIndex = 0;
            // 
            // Txt_Search
            // 
            this.Txt_Search.Location = new System.Drawing.Point(95, 10);
            this.Txt_Search.Name = "Txt_Search";
            this.Txt_Search.Size = new System.Drawing.Size(230, 29);
            this.Txt_Search.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 21);
            this.label2.TabIndex = 0;
            this.label2.Text = "工單編號";
            // 
            // Grb_List
            // 
            this.Grb_List.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Grb_List.Controls.Add(this.Dgv_Order);
            this.Grb_List.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Grb_List.Location = new System.Drawing.Point(331, 114);
            this.Grb_List.Name = "Grb_List";
            this.Grb_List.Size = new System.Drawing.Size(986, 708);
            this.Grb_List.TabIndex = 1;
            this.Grb_List.TabStop = false;
            this.Grb_List.Text = "工單清單";
            this.Grb_List.Visible = false;
            // 
            // Dgv_Order
            // 
            this.Dgv_Order.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.Dgv_Order.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.Dgv_Order.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dgv_Order.Location = new System.Drawing.Point(20, 28);
            this.Dgv_Order.Name = "Dgv_Order";
            this.Dgv_Order.ReadOnly = true;
            this.Dgv_Order.RowTemplate.Height = 24;
            this.Dgv_Order.Size = new System.Drawing.Size(960, 672);
            this.Dgv_Order.TabIndex = 1;
            this.Dgv_Order.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Dgv_Order_CellClick);
            this.Dgv_Order.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Dgv_Order_CellDoubleClick);
            this.Dgv_Order.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox3.Controls.Add(this.Btn_Login);
            this.groupBox3.Controls.Add(this.Lab_LoginInfo);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.Txt_Password);
            this.groupBox3.Controls.Add(this.Txt_User);
            this.groupBox3.Controls.Add(this.Txt_Port);
            this.groupBox3.Controls.Add(this.Txt_Host);
            this.groupBox3.Location = new System.Drawing.Point(23, 114);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(302, 370);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "groupBox3";
            // 
            // Btn_Login
            // 
            this.Btn_Login.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Login.Location = new System.Drawing.Point(106, 201);
            this.Btn_Login.Name = "Btn_Login";
            this.Btn_Login.Size = new System.Drawing.Size(102, 47);
            this.Btn_Login.TabIndex = 2;
            this.Btn_Login.Text = "登入";
            this.Btn_Login.UseVisualStyleBackColor = true;
            this.Btn_Login.Click += new System.EventHandler(this.Btn_Login_Click);
            // 
            // Lab_LoginInfo
            // 
            this.Lab_LoginInfo.AutoSize = true;
            this.Lab_LoginInfo.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Lab_LoginInfo.Location = new System.Drawing.Point(6, 266);
            this.Lab_LoginInfo.Name = "Lab_LoginInfo";
            this.Lab_LoginInfo.Size = new System.Drawing.Size(74, 21);
            this.Lab_LoginInfo.TabIndex = 1;
            this.Lab_LoginInfo.Text = "登入訊息";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(16, 158);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 24);
            this.label6.TabIndex = 1;
            this.label6.Text = "Password";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(16, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 24);
            this.label5.TabIndex = 1;
            this.label5.Text = "User";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(16, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 24);
            this.label4.TabIndex = 1;
            this.label4.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(16, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 24);
            this.label3.TabIndex = 1;
            this.label3.Text = "Host";
            // 
            // Txt_Password
            // 
            this.Txt_Password.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Txt_Password.Location = new System.Drawing.Point(117, 161);
            this.Txt_Password.Name = "Txt_Password";
            this.Txt_Password.Size = new System.Drawing.Size(165, 27);
            this.Txt_Password.TabIndex = 0;
            this.Txt_Password.Text = "AOIuser80689917";
            // 
            // Txt_User
            // 
            this.Txt_User.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Txt_User.Location = new System.Drawing.Point(117, 116);
            this.Txt_User.Name = "Txt_User";
            this.Txt_User.Size = new System.Drawing.Size(165, 27);
            this.Txt_User.TabIndex = 0;
            this.Txt_User.Text = "postgres";
            // 
            // Txt_Port
            // 
            this.Txt_Port.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Txt_Port.Location = new System.Drawing.Point(117, 71);
            this.Txt_Port.Name = "Txt_Port";
            this.Txt_Port.Size = new System.Drawing.Size(165, 27);
            this.Txt_Port.TabIndex = 0;
            this.Txt_Port.Text = "54321";
            // 
            // Txt_Host
            // 
            this.Txt_Host.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Txt_Host.Location = new System.Drawing.Point(117, 28);
            this.Txt_Host.Name = "Txt_Host";
            this.Txt_Host.Size = new System.Drawing.Size(165, 27);
            this.Txt_Host.TabIndex = 0;
            this.Txt_Host.Text = "192.168.2.105";
            // 
            // LST_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(1329, 826);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.Grb_List);
            this.Controls.Add(this.Grb_Search);
            this.Controls.Add(this.label1);
            this.Name = "LST_Form";
            this.Text = "檢測表單列表";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LST_Form_FormClosing);
            this.Load += new System.EventHandler(this.LST_Form_Load);
            this.Grb_Search.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.Grb_List.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_Order)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox Grb_Search;
        private System.Windows.Forms.GroupBox Grb_List;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox Txt_Search;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView Dgv_Order;
        private System.Windows.Forms.Button Btn_Clean;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button Btn_Login;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Txt_Password;
        private System.Windows.Forms.TextBox Txt_User;
        private System.Windows.Forms.TextBox Txt_Port;
        private System.Windows.Forms.TextBox Txt_Host;
        private System.Windows.Forms.Label Lab_LoginInfo;
    }
}