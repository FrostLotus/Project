﻿
namespace OPCNodeClientEditor
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
            this.Txt_ServerURL = new System.Windows.Forms.TextBox();
            this.Btn_Connect = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.Lab_ConnectStatus = new System.Windows.Forms.Label();
            this.Gbx_NodeList = new System.Windows.Forms.GroupBox();
            this.Txt_Index = new System.Windows.Forms.TextBox();
            this.Txt_ItemName = new System.Windows.Forms.TextBox();
            this.Txt_NodeId = new System.Windows.Forms.TextBox();
            this.Btn_UpdateValue = new System.Windows.Forms.Button();
            this.Txt_Value = new System.Windows.Forms.TextBox();
            this.Cbb_DataType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.Lsv_VariableList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_ServerTimeNow = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_ConnectNow = new System.Windows.Forms.ToolStripStatusLabel();
            this.Txt_ReconnectTime = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Txt_ClientName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Txt_SubscriptionPublish = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Txt_MonitoredItemSampling = new System.Windows.Forms.TextBox();
            this.Txt_SessionKeepAlive = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Txt_SessionTimeout = new System.Windows.Forms.TextBox();
            this.Gbx_NodeList.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Txt_ServerURL
            // 
            this.Txt_ServerURL.Location = new System.Drawing.Point(18, 43);
            this.Txt_ServerURL.Name = "Txt_ServerURL";
            this.Txt_ServerURL.Size = new System.Drawing.Size(539, 22);
            this.Txt_ServerURL.TabIndex = 69;
            this.Txt_ServerURL.Text = "opc.tcp://127.0.0.1:9048/NodeServer";
            // 
            // Btn_Connect
            // 
            this.Btn_Connect.BackColor = System.Drawing.Color.Coral;
            this.Btn_Connect.Location = new System.Drawing.Point(487, 11);
            this.Btn_Connect.Name = "Btn_Connect";
            this.Btn_Connect.Size = new System.Drawing.Size(75, 26);
            this.Btn_Connect.TabIndex = 68;
            this.Btn_Connect.Text = "連線";
            this.Btn_Connect.UseVisualStyleBackColor = false;
            this.Btn_Connect.Click += new System.EventHandler(this.Btn_Connect_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 12);
            this.label13.TabIndex = 66;
            this.label13.Text = "Server位址";
            // 
            // Lab_ConnectStatus
            // 
            this.Lab_ConnectStatus.AutoSize = true;
            this.Lab_ConnectStatus.Font = new System.Drawing.Font("新細明體", 10F);
            this.Lab_ConnectStatus.Location = new System.Drawing.Point(379, 18);
            this.Lab_ConnectStatus.Name = "Lab_ConnectStatus";
            this.Lab_ConnectStatus.Size = new System.Drawing.Size(35, 14);
            this.Lab_ConnectStatus.TabIndex = 65;
            this.Lab_ConnectStatus.Text = "離線";
            // 
            // Gbx_NodeList
            // 
            this.Gbx_NodeList.Controls.Add(this.Txt_Index);
            this.Gbx_NodeList.Controls.Add(this.Txt_ItemName);
            this.Gbx_NodeList.Controls.Add(this.Txt_NodeId);
            this.Gbx_NodeList.Controls.Add(this.Btn_UpdateValue);
            this.Gbx_NodeList.Controls.Add(this.Txt_Value);
            this.Gbx_NodeList.Controls.Add(this.Cbb_DataType);
            this.Gbx_NodeList.Controls.Add(this.label6);
            this.Gbx_NodeList.Controls.Add(this.label8);
            this.Gbx_NodeList.Controls.Add(this.label9);
            this.Gbx_NodeList.Controls.Add(this.label10);
            this.Gbx_NodeList.Controls.Add(this.label12);
            this.Gbx_NodeList.Location = new System.Drawing.Point(12, 348);
            this.Gbx_NodeList.Name = "Gbx_NodeList";
            this.Gbx_NodeList.Size = new System.Drawing.Size(551, 140);
            this.Gbx_NodeList.TabIndex = 64;
            this.Gbx_NodeList.TabStop = false;
            this.Gbx_NodeList.Text = "NodeList";
            // 
            // Txt_Index
            // 
            this.Txt_Index.Location = new System.Drawing.Point(83, 31);
            this.Txt_Index.Name = "Txt_Index";
            this.Txt_Index.ReadOnly = true;
            this.Txt_Index.Size = new System.Drawing.Size(145, 22);
            this.Txt_Index.TabIndex = 25;
            // 
            // Txt_ItemName
            // 
            this.Txt_ItemName.Location = new System.Drawing.Point(83, 68);
            this.Txt_ItemName.Name = "Txt_ItemName";
            this.Txt_ItemName.ReadOnly = true;
            this.Txt_ItemName.Size = new System.Drawing.Size(145, 22);
            this.Txt_ItemName.TabIndex = 30;
            // 
            // Txt_NodeId
            // 
            this.Txt_NodeId.Location = new System.Drawing.Point(288, 68);
            this.Txt_NodeId.Name = "Txt_NodeId";
            this.Txt_NodeId.ReadOnly = true;
            this.Txt_NodeId.Size = new System.Drawing.Size(144, 22);
            this.Txt_NodeId.TabIndex = 26;
            // 
            // Btn_UpdateValue
            // 
            this.Btn_UpdateValue.Location = new System.Drawing.Point(446, 62);
            this.Btn_UpdateValue.Name = "Btn_UpdateValue";
            this.Btn_UpdateValue.Size = new System.Drawing.Size(99, 30);
            this.Btn_UpdateValue.TabIndex = 41;
            this.Btn_UpdateValue.Text = "Update_Value";
            this.Btn_UpdateValue.UseVisualStyleBackColor = true;
            this.Btn_UpdateValue.Click += new System.EventHandler(this.Btn_UpdateValue_Click);
            // 
            // Txt_Value
            // 
            this.Txt_Value.Location = new System.Drawing.Point(83, 104);
            this.Txt_Value.Name = "Txt_Value";
            this.Txt_Value.Size = new System.Drawing.Size(350, 22);
            this.Txt_Value.TabIndex = 29;
            // 
            // Cbb_DataType
            // 
            this.Cbb_DataType.Enabled = false;
            this.Cbb_DataType.FormattingEnabled = true;
            this.Cbb_DataType.Items.AddRange(new object[] {
            "String",
            "Word",
            "Bool",
            "Real"});
            this.Cbb_DataType.Location = new System.Drawing.Point(288, 31);
            this.Cbb_DataType.Name = "Cbb_DataType";
            this.Cbb_DataType.Size = new System.Drawing.Size(145, 20);
            this.Cbb_DataType.TabIndex = 31;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("新細明體", 10F);
            this.label6.Location = new System.Drawing.Point(6, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 14);
            this.label6.TabIndex = 32;
            this.label6.Text = "Index";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("新細明體", 10F);
            this.label8.Location = new System.Drawing.Point(6, 107);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 14);
            this.label8.TabIndex = 37;
            this.label8.Text = "Value";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("新細明體", 10F);
            this.label9.Location = new System.Drawing.Point(6, 71);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 14);
            this.label9.TabIndex = 33;
            this.label9.Text = "Item Name";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("新細明體", 10F);
            this.label10.Location = new System.Drawing.Point(234, 71);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 14);
            this.label10.TabIndex = 34;
            this.label10.Text = "Node Id";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("新細明體", 10F);
            this.label12.Location = new System.Drawing.Point(234, 34);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 14);
            this.label12.TabIndex = 35;
            this.label12.Text = "Type";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.Lsv_VariableList);
            this.groupBox5.Location = new System.Drawing.Point(12, 71);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(551, 271);
            this.groupBox5.TabIndex = 63;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "變數列表";
            // 
            // Lsv_VariableList
            // 
            this.Lsv_VariableList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.Lsv_VariableList.FullRowSelect = true;
            this.Lsv_VariableList.HideSelection = false;
            this.Lsv_VariableList.Location = new System.Drawing.Point(6, 21);
            this.Lsv_VariableList.MultiSelect = false;
            this.Lsv_VariableList.Name = "Lsv_VariableList";
            this.Lsv_VariableList.Size = new System.Drawing.Size(539, 244);
            this.Lsv_VariableList.TabIndex = 2;
            this.Lsv_VariableList.UseCompatibleStateImageBehavior = false;
            this.Lsv_VariableList.View = System.Windows.Forms.View.Details;
            this.Lsv_VariableList.SelectedIndexChanged += new System.EventHandler(this.Lsv_VariableList_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "目錄";
            this.columnHeader1.Width = 91;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "名稱";
            this.columnHeader2.Width = 112;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "節點名稱";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 119;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "值";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 140;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.Lab_ServerTimeNow,
            this.toolStripStatusLabel3,
            this.Lab_ConnectNow});
            this.statusStrip1.Location = new System.Drawing.Point(0, 503);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(758, 22);
            this.statusStrip1.TabIndex = 70;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(66, 17);
            this.toolStripStatusLabel1.Text = "目前時間:";
            // 
            // Lab_ServerTimeNow
            // 
            this.Lab_ServerTimeNow.Name = "Lab_ServerTimeNow";
            this.Lab_ServerTimeNow.Size = new System.Drawing.Size(55, 17);
            this.Lab_ServerTimeNow.Text = "00:00:00";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(38, 17);
            this.toolStripStatusLabel3.Text = "狀態:";
            // 
            // Lab_ConnectNow
            // 
            this.Lab_ConnectNow.Name = "Lab_ConnectNow";
            this.Lab_ConnectNow.Size = new System.Drawing.Size(43, 17);
            this.Lab_ConnectNow.Text = "未連線";
            // 
            // Txt_ReconnectTime
            // 
            this.Txt_ReconnectTime.Enabled = false;
            this.Txt_ReconnectTime.Location = new System.Drawing.Point(9, 39);
            this.Txt_ReconnectTime.Name = "Txt_ReconnectTime";
            this.Txt_ReconnectTime.Size = new System.Drawing.Size(98, 22);
            this.Txt_ReconnectTime.TabIndex = 29;
            this.Txt_ReconnectTime.Text = "3";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 10F);
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 14);
            this.label1.TabIndex = 32;
            this.label1.Text = "斷線重連時間(暫時唯讀)/秒";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 10F);
            this.label2.Location = new System.Drawing.Point(6, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 14);
            this.label2.TabIndex = 32;
            this.label2.Text = "Client端名稱";
            // 
            // Txt_ClientName
            // 
            this.Txt_ClientName.Enabled = false;
            this.Txt_ClientName.Location = new System.Drawing.Point(9, 81);
            this.Txt_ClientName.Name = "Txt_ClientName";
            this.Txt_ClientName.Size = new System.Drawing.Size(98, 22);
            this.Txt_ClientName.TabIndex = 29;
            this.Txt_ClientName.Text = "Test_Client";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.Txt_SubscriptionPublish);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.Txt_MonitoredItemSampling);
            this.groupBox1.Controls.Add(this.Txt_SessionKeepAlive);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.Txt_ReconnectTime);
            this.groupBox1.Controls.Add(this.Txt_ClientName);
            this.groupBox1.Location = new System.Drawing.Point(568, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(177, 298);
            this.groupBox1.TabIndex = 71;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "設定";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("新細明體", 10F);
            this.label7.Location = new System.Drawing.Point(6, 148);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 14);
            this.label7.TabIndex = 32;
            this.label7.Text = "SubscriptionPublish";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("新細明體", 10F);
            this.label5.Location = new System.Drawing.Point(6, 191);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 14);
            this.label5.TabIndex = 32;
            this.label5.Text = "MonitoredItemSampling";
            // 
            // Txt_SubscriptionPublish
            // 
            this.Txt_SubscriptionPublish.Location = new System.Drawing.Point(10, 165);
            this.Txt_SubscriptionPublish.Name = "Txt_SubscriptionPublish";
            this.Txt_SubscriptionPublish.Size = new System.Drawing.Size(97, 22);
            this.Txt_SubscriptionPublish.TabIndex = 29;
            this.Txt_SubscriptionPublish.Text = "100";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("新細明體", 10F);
            this.label4.Location = new System.Drawing.Point(6, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 14);
            this.label4.TabIndex = 32;
            this.label4.Text = "KeepAlive_Time";
            // 
            // Txt_MonitoredItemSampling
            // 
            this.Txt_MonitoredItemSampling.Location = new System.Drawing.Point(10, 208);
            this.Txt_MonitoredItemSampling.Name = "Txt_MonitoredItemSampling";
            this.Txt_MonitoredItemSampling.Size = new System.Drawing.Size(97, 22);
            this.Txt_MonitoredItemSampling.TabIndex = 29;
            this.Txt_MonitoredItemSampling.Text = "100";
            // 
            // Txt_SessionKeepAlive
            // 
            this.Txt_SessionKeepAlive.Location = new System.Drawing.Point(9, 123);
            this.Txt_SessionKeepAlive.Name = "Txt_SessionKeepAlive";
            this.Txt_SessionKeepAlive.Size = new System.Drawing.Size(97, 22);
            this.Txt_SessionKeepAlive.TabIndex = 29;
            this.Txt_SessionKeepAlive.Text = "3000";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.Txt_SessionTimeout);
            this.groupBox2.Location = new System.Drawing.Point(568, 326);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(177, 143);
            this.groupBox2.TabIndex = 71;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "取得";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("新細明體", 10F);
            this.label3.Location = new System.Drawing.Point(7, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 14);
            this.label3.TabIndex = 32;
            this.label3.Text = "SessionTimeout";
            // 
            // Txt_SessionTimeout
            // 
            this.Txt_SessionTimeout.Enabled = false;
            this.Txt_SessionTimeout.Location = new System.Drawing.Point(10, 40);
            this.Txt_SessionTimeout.Name = "Txt_SessionTimeout";
            this.Txt_SessionTimeout.Size = new System.Drawing.Size(97, 22);
            this.Txt_SessionTimeout.TabIndex = 29;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 525);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.Txt_ServerURL);
            this.Controls.Add(this.Btn_Connect);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.Lab_ConnectStatus);
            this.Controls.Add(this.Gbx_NodeList);
            this.Controls.Add(this.groupBox5);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Gbx_NodeList.ResumeLayout(false);
            this.Gbx_NodeList.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Txt_ServerURL;
        private System.Windows.Forms.Button Btn_Connect;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label Lab_ConnectStatus;
        private System.Windows.Forms.GroupBox Gbx_NodeList;
        private System.Windows.Forms.TextBox Txt_Index;
        private System.Windows.Forms.TextBox Txt_ItemName;
        private System.Windows.Forms.TextBox Txt_NodeId;
        private System.Windows.Forms.Button Btn_UpdateValue;
        private System.Windows.Forms.TextBox Txt_Value;
        private System.Windows.Forms.ComboBox Cbb_DataType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListView Lsv_VariableList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel Lab_ServerTimeNow;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel Lab_ConnectNow;
        private System.Windows.Forms.TextBox Txt_ReconnectTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Txt_ClientName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Txt_SessionKeepAlive;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Txt_SessionTimeout;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox Txt_MonitoredItemSampling;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox Txt_SubscriptionPublish;
    }
}

