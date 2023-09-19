
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
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.Lsv_VariableList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label8 = new System.Windows.Forms.Label();
            this.Cbb_EndpointsUrl = new System.Windows.Forms.ComboBox();
            this.Gbx_NodeList = new System.Windows.Forms.GroupBox();
            this.Txt_Length = new System.Windows.Forms.TextBox();
            this.Txt_Index = new System.Windows.Forms.TextBox();
            this.Txt_ItemName = new System.Windows.Forms.TextBox();
            this.Txt_NodeId = new System.Windows.Forms.TextBox();
            this.Btn_UpdateValue = new System.Windows.Forms.Button();
            this.Txt_Initial = new System.Windows.Forms.TextBox();
            this.Txt_Value = new System.Windows.Forms.TextBox();
            this.Cbb_DataType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Lab_ConnectStatus = new System.Windows.Forms.Label();
            this.Btn_Connect = new System.Windows.Forms.Button();
            this.groupBox5.SuspendLayout();
            this.Gbx_NodeList.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.Lsv_VariableList);
            this.groupBox5.Location = new System.Drawing.Point(12, 58);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(551, 220);
            this.groupBox5.TabIndex = 53;
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
            this.Lsv_VariableList.Location = new System.Drawing.Point(13, 21);
            this.Lsv_VariableList.MultiSelect = false;
            this.Lsv_VariableList.Name = "Lsv_VariableList";
            this.Lsv_VariableList.Size = new System.Drawing.Size(526, 182);
            this.Lsv_VariableList.TabIndex = 2;
            this.Lsv_VariableList.UseCompatibleStateImageBehavior = false;
            this.Lsv_VariableList.View = System.Windows.Forms.View.Details;
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
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 8);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 50;
            this.label8.Text = "Server位址";
            // 
            // Cbb_EndpointsUrl
            // 
            this.Cbb_EndpointsUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Cbb_EndpointsUrl.FormattingEnabled = true;
            this.Cbb_EndpointsUrl.Location = new System.Drawing.Point(12, 32);
            this.Cbb_EndpointsUrl.Name = "Cbb_EndpointsUrl";
            this.Cbb_EndpointsUrl.Size = new System.Drawing.Size(538, 20);
            this.Cbb_EndpointsUrl.TabIndex = 52;
            this.Cbb_EndpointsUrl.Text = "opc.tcp://127.0.0.1:62547/DataAccessServer";
            // 
            // Gbx_NodeList
            // 
            this.Gbx_NodeList.Controls.Add(this.Txt_Length);
            this.Gbx_NodeList.Controls.Add(this.Txt_Index);
            this.Gbx_NodeList.Controls.Add(this.Txt_ItemName);
            this.Gbx_NodeList.Controls.Add(this.Txt_NodeId);
            this.Gbx_NodeList.Controls.Add(this.Btn_UpdateValue);
            this.Gbx_NodeList.Controls.Add(this.Txt_Initial);
            this.Gbx_NodeList.Controls.Add(this.Txt_Value);
            this.Gbx_NodeList.Controls.Add(this.Cbb_DataType);
            this.Gbx_NodeList.Controls.Add(this.label1);
            this.Gbx_NodeList.Controls.Add(this.label6);
            this.Gbx_NodeList.Controls.Add(this.label7);
            this.Gbx_NodeList.Controls.Add(this.label2);
            this.Gbx_NodeList.Controls.Add(this.label3);
            this.Gbx_NodeList.Controls.Add(this.label5);
            this.Gbx_NodeList.Controls.Add(this.label4);
            this.Gbx_NodeList.Location = new System.Drawing.Point(12, 284);
            this.Gbx_NodeList.Name = "Gbx_NodeList";
            this.Gbx_NodeList.Size = new System.Drawing.Size(551, 183);
            this.Gbx_NodeList.TabIndex = 51;
            this.Gbx_NodeList.TabStop = false;
            this.Gbx_NodeList.Text = "NodeList";
            // 
            // Txt_Length
            // 
            this.Txt_Length.Location = new System.Drawing.Point(288, 110);
            this.Txt_Length.Name = "Txt_Length";
            this.Txt_Length.Size = new System.Drawing.Size(145, 22);
            this.Txt_Length.TabIndex = 27;
            // 
            // Txt_Index
            // 
            this.Txt_Index.Location = new System.Drawing.Point(83, 31);
            this.Txt_Index.Name = "Txt_Index";
            this.Txt_Index.Size = new System.Drawing.Size(145, 22);
            this.Txt_Index.TabIndex = 25;
            // 
            // Txt_ItemName
            // 
            this.Txt_ItemName.Location = new System.Drawing.Point(83, 71);
            this.Txt_ItemName.Name = "Txt_ItemName";
            this.Txt_ItemName.Size = new System.Drawing.Size(145, 22);
            this.Txt_ItemName.TabIndex = 30;
            // 
            // Txt_NodeId
            // 
            this.Txt_NodeId.Location = new System.Drawing.Point(289, 71);
            this.Txt_NodeId.Name = "Txt_NodeId";
            this.Txt_NodeId.Size = new System.Drawing.Size(144, 22);
            this.Txt_NodeId.TabIndex = 26;
            // 
            // Btn_UpdateValue
            // 
            this.Btn_UpdateValue.Location = new System.Drawing.Point(446, 25);
            this.Btn_UpdateValue.Name = "Btn_UpdateValue";
            this.Btn_UpdateValue.Size = new System.Drawing.Size(99, 30);
            this.Btn_UpdateValue.TabIndex = 41;
            this.Btn_UpdateValue.Text = "Update_Value";
            this.Btn_UpdateValue.UseVisualStyleBackColor = true;
            // 
            // Txt_Initial
            // 
            this.Txt_Initial.Location = new System.Drawing.Point(82, 110);
            this.Txt_Initial.Name = "Txt_Initial";
            this.Txt_Initial.Size = new System.Drawing.Size(145, 22);
            this.Txt_Initial.TabIndex = 29;
            // 
            // Txt_Value
            // 
            this.Txt_Value.Location = new System.Drawing.Point(81, 147);
            this.Txt_Value.Name = "Txt_Value";
            this.Txt_Value.Size = new System.Drawing.Size(353, 22);
            this.Txt_Value.TabIndex = 29;
            // 
            // Cbb_DataType
            // 
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 10F);
            this.label1.Location = new System.Drawing.Point(6, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 14);
            this.label1.TabIndex = 32;
            this.label1.Text = "Index";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("新細明體", 10F);
            this.label6.Location = new System.Drawing.Point(6, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 14);
            this.label6.TabIndex = 37;
            this.label6.Text = "Initial";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("新細明體", 10F);
            this.label7.Location = new System.Drawing.Point(6, 150);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 14);
            this.label7.TabIndex = 37;
            this.label7.Text = "Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 10F);
            this.label2.Location = new System.Drawing.Point(6, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 14);
            this.label2.TabIndex = 33;
            this.label2.Text = "Item Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("新細明體", 10F);
            this.label3.Location = new System.Drawing.Point(234, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 14);
            this.label3.TabIndex = 34;
            this.label3.Text = "Node Id";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("新細明體", 10F);
            this.label5.Location = new System.Drawing.Point(234, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 14);
            this.label5.TabIndex = 38;
            this.label5.Text = "Length";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("新細明體", 10F);
            this.label4.Location = new System.Drawing.Point(234, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 14);
            this.label4.TabIndex = 35;
            this.label4.Text = "Type";
            // 
            // Lab_ConnectStatus
            // 
            this.Lab_ConnectStatus.AutoSize = true;
            this.Lab_ConnectStatus.Font = new System.Drawing.Font("新細明體", 10F);
            this.Lab_ConnectStatus.Location = new System.Drawing.Point(383, 8);
            this.Lab_ConnectStatus.Name = "Lab_ConnectStatus";
            this.Lab_ConnectStatus.Size = new System.Drawing.Size(63, 14);
            this.Lab_ConnectStatus.TabIndex = 35;
            this.Lab_ConnectStatus.Text = "連線狀態";
            // 
            // Btn_Connect
            // 
            this.Btn_Connect.Location = new System.Drawing.Point(477, 3);
            this.Btn_Connect.Name = "Btn_Connect";
            this.Btn_Connect.Size = new System.Drawing.Size(75, 23);
            this.Btn_Connect.TabIndex = 54;
            this.Btn_Connect.Text = "連線";
            this.Btn_Connect.UseVisualStyleBackColor = true;
            this.Btn_Connect.Click += new System.EventHandler(this.Btn_Connect_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 477);
            this.Controls.Add(this.Btn_Connect);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.Cbb_EndpointsUrl);
            this.Controls.Add(this.Gbx_NodeList);
            this.Controls.Add(this.Lab_ConnectStatus);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox5.ResumeLayout(false);
            this.Gbx_NodeList.ResumeLayout(false);
            this.Gbx_NodeList.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListView Lsv_VariableList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox Cbb_EndpointsUrl;
        private System.Windows.Forms.GroupBox Gbx_NodeList;
        private System.Windows.Forms.TextBox Txt_Length;
        private System.Windows.Forms.TextBox Txt_Index;
        private System.Windows.Forms.TextBox Txt_ItemName;
        private System.Windows.Forms.TextBox Txt_NodeId;
        private System.Windows.Forms.Button Btn_UpdateValue;
        private System.Windows.Forms.TextBox Txt_Initial;
        private System.Windows.Forms.TextBox Txt_Value;
        private System.Windows.Forms.ComboBox Cbb_DataType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label Lab_ConnectStatus;
        private System.Windows.Forms.Button Btn_Connect;
    }
}

