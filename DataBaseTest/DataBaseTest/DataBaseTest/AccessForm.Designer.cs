
namespace DataBaseTest
{
    partial class AccessForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Tb_UserID = new System.Windows.Forms.TextBox();
            this.Tb_Password = new System.Windows.Forms.TextBox();
            this.Cb_DataSource = new System.Windows.Forms.ComboBox();
            this.Cb_InitialCatalog = new System.Windows.Forms.ComboBox();
            this.Btn_Connect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "DataSource:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "InitialCatalog:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "UserID:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 178);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Password:";
            // 
            // Tb_UserID
            // 
            this.Tb_UserID.Location = new System.Drawing.Point(132, 141);
            this.Tb_UserID.Name = "Tb_UserID";
            this.Tb_UserID.Size = new System.Drawing.Size(100, 22);
            this.Tb_UserID.TabIndex = 1;
            this.Tb_UserID.Text = "AOI";
            // 
            // Tb_Password
            // 
            this.Tb_Password.Location = new System.Drawing.Point(132, 175);
            this.Tb_Password.Name = "Tb_Password";
            this.Tb_Password.PasswordChar = '*';
            this.Tb_Password.Size = new System.Drawing.Size(100, 22);
            this.Tb_Password.TabIndex = 1;
            this.Tb_Password.Text = "aoi0817";
            // 
            // Cb_DataSource
            // 
            this.Cb_DataSource.FormattingEnabled = true;
            this.Cb_DataSource.Items.AddRange(new object[] {
            "AOI-142\\\\SQLEXPRESS"});
            this.Cb_DataSource.Location = new System.Drawing.Point(121, 61);
            this.Cb_DataSource.Name = "Cb_DataSource";
            this.Cb_DataSource.Size = new System.Drawing.Size(121, 20);
            this.Cb_DataSource.TabIndex = 2;
            this.Cb_DataSource.Text = "AOI-142\\SQLEXPRESS";
            // 
            // Cb_InitialCatalog
            // 
            this.Cb_InitialCatalog.FormattingEnabled = true;
            this.Cb_InitialCatalog.Items.AddRange(new object[] {
            "MVC_TestDB"});
            this.Cb_InitialCatalog.Location = new System.Drawing.Point(121, 95);
            this.Cb_InitialCatalog.Name = "Cb_InitialCatalog";
            this.Cb_InitialCatalog.Size = new System.Drawing.Size(121, 20);
            this.Cb_InitialCatalog.TabIndex = 2;
            this.Cb_InitialCatalog.Text = "MVC_TestDB";
            // 
            // Btn_Connect
            // 
            this.Btn_Connect.Location = new System.Drawing.Point(86, 226);
            this.Btn_Connect.Name = "Btn_Connect";
            this.Btn_Connect.Size = new System.Drawing.Size(100, 30);
            this.Btn_Connect.TabIndex = 3;
            this.Btn_Connect.Text = "Connect";
            this.Btn_Connect.UseVisualStyleBackColor = true;
            this.Btn_Connect.Click += new System.EventHandler(this.Btn_Connect_Click);
            // 
            // AccessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 268);
            this.Controls.Add(this.Btn_Connect);
            this.Controls.Add(this.Cb_InitialCatalog);
            this.Controls.Add(this.Cb_DataSource);
            this.Controls.Add(this.Tb_Password);
            this.Controls.Add(this.Tb_UserID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Name = "AccessForm";
            this.Text = "AccessForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Tb_UserID;
        private System.Windows.Forms.TextBox Tb_Password;
        private System.Windows.Forms.ComboBox Cb_DataSource;
        private System.Windows.Forms.ComboBox Cb_InitialCatalog;
        private System.Windows.Forms.Button Btn_Connect;
    }
}