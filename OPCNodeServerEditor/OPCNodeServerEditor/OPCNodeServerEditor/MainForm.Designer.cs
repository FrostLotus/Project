
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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Cbb_DataType = new System.Windows.Forms.ComboBox();
            this.Txt_Value = new System.Windows.Forms.TextBox();
            this.Txt_Length = new System.Windows.Forms.TextBox();
            this.Txt_NodeId = new System.Windows.Forms.TextBox();
            this.Txt_ItemName = new System.Windows.Forms.TextBox();
            this.Txt_Index = new System.Windows.Forms.TextBox();
            this.Btn_UpdateValue = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Btn_Stop = new System.Windows.Forms.Button();
            this.Btn_Run = new System.Windows.Forms.Button();
            this.Lab_Status = new System.Windows.Forms.Label();
            this.Gbx_NodeList = new System.Windows.Forms.GroupBox();
            this.Txt_Initial = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Cbb_EndpointsUrl = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Lsv_Sessions = new System.Windows.Forms.ListView();
            this.SessionIdCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SessionNameCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UserNameCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LastContactTimeCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Lsv_Subscriptions = new System.Windows.Forms.ListView();
            this.SubscriptionIdCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PublishingIntervalCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ItemCountCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SequenceNumberCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.StatusBAR = new System.Windows.Forms.StatusStrip();
            this.Lab_Time = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_ServerTimeNow = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_sessions = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_sessionsCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_subscriptions = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_subscriptionsCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_items = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_itemsCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.Lsv_VariableList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.Gbx_NodeList.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.StatusBAR.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // Btn_Close
            // 
            this.Btn_Close.Location = new System.Drawing.Point(29, 21);
            this.Btn_Close.Name = "Btn_Close";
            this.Btn_Close.Size = new System.Drawing.Size(99, 30);
            this.Btn_Close.TabIndex = 42;
            this.Btn_Close.Text = "Close Window";
            this.Btn_Close.UseVisualStyleBackColor = true;
            // 
            // Btn_UpdateFile
            // 
            this.Btn_UpdateFile.Location = new System.Drawing.Point(441, 92);
            this.Btn_UpdateFile.Name = "Btn_UpdateFile";
            this.Btn_UpdateFile.Size = new System.Drawing.Size(99, 30);
            this.Btn_UpdateFile.TabIndex = 41;
            this.Btn_UpdateFile.Text = "Update_File";
            this.Btn_UpdateFile.UseVisualStyleBackColor = true;
            this.Btn_UpdateFile.Click += new System.EventHandler(this.Btn_UpdateFile_Click);
            // 
            // Btn_Delete
            // 
            this.Btn_Delete.Location = new System.Drawing.Point(440, 57);
            this.Btn_Delete.Name = "Btn_Delete";
            this.Btn_Delete.Size = new System.Drawing.Size(99, 30);
            this.Btn_Delete.TabIndex = 40;
            this.Btn_Delete.Text = "Delete";
            this.Btn_Delete.UseVisualStyleBackColor = true;
            this.Btn_Delete.Click += new System.EventHandler(this.Btn_Delete_Click);
            // 
            // Btn_Add
            // 
            this.Btn_Add.Location = new System.Drawing.Point(441, 25);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(99, 30);
            this.Btn_Add.TabIndex = 39;
            this.Btn_Add.Text = "Add";
            this.Btn_Add.UseVisualStyleBackColor = true;
            this.Btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("新細明體", 10F);
            this.label7.Location = new System.Drawing.Point(10, 167);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 14);
            this.label7.TabIndex = 37;
            this.label7.Text = "Value";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("新細明體", 10F);
            this.label5.Location = new System.Drawing.Point(234, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 14);
            this.label5.TabIndex = 38;
            this.label5.Text = "Length";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("新細明體", 10F);
            this.label4.Location = new System.Drawing.Point(10, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 14);
            this.label4.TabIndex = 35;
            this.label4.Text = "Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("新細明體", 10F);
            this.label3.Location = new System.Drawing.Point(234, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 14);
            this.label3.TabIndex = 34;
            this.label3.Text = "Node Id";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 10F);
            this.label2.Location = new System.Drawing.Point(9, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 14);
            this.label2.TabIndex = 33;
            this.label2.Text = "Item Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 10F);
            this.label1.Location = new System.Drawing.Point(10, 34);
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
            this.Cbb_DataType.Location = new System.Drawing.Point(82, 98);
            this.Cbb_DataType.Name = "Cbb_DataType";
            this.Cbb_DataType.Size = new System.Drawing.Size(145, 20);
            this.Cbb_DataType.TabIndex = 31;
            // 
            // Txt_Value
            // 
            this.Txt_Value.Location = new System.Drawing.Point(82, 164);
            this.Txt_Value.Name = "Txt_Value";
            this.Txt_Value.Size = new System.Drawing.Size(353, 22);
            this.Txt_Value.TabIndex = 29;
            // 
            // Txt_Length
            // 
            this.Txt_Length.Location = new System.Drawing.Point(290, 98);
            this.Txt_Length.Name = "Txt_Length";
            this.Txt_Length.Size = new System.Drawing.Size(145, 22);
            this.Txt_Length.TabIndex = 27;
            // 
            // Txt_NodeId
            // 
            this.Txt_NodeId.Location = new System.Drawing.Point(290, 63);
            this.Txt_NodeId.Name = "Txt_NodeId";
            this.Txt_NodeId.Size = new System.Drawing.Size(144, 22);
            this.Txt_NodeId.TabIndex = 26;
            // 
            // Txt_ItemName
            // 
            this.Txt_ItemName.Location = new System.Drawing.Point(82, 63);
            this.Txt_ItemName.Name = "Txt_ItemName";
            this.Txt_ItemName.Size = new System.Drawing.Size(145, 22);
            this.Txt_ItemName.TabIndex = 30;
            // 
            // Txt_Index
            // 
            this.Txt_Index.Location = new System.Drawing.Point(82, 31);
            this.Txt_Index.Name = "Txt_Index";
            this.Txt_Index.Size = new System.Drawing.Size(145, 22);
            this.Txt_Index.TabIndex = 25;
            // 
            // Btn_UpdateValue
            // 
            this.Btn_UpdateValue.Location = new System.Drawing.Point(441, 126);
            this.Btn_UpdateValue.Name = "Btn_UpdateValue";
            this.Btn_UpdateValue.Size = new System.Drawing.Size(99, 30);
            this.Btn_UpdateValue.TabIndex = 41;
            this.Btn_UpdateValue.Text = "Update_Value";
            this.Btn_UpdateValue.UseVisualStyleBackColor = true;
            this.Btn_UpdateValue.Click += new System.EventHandler(this.Btn_UpdateValue_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Btn_Stop);
            this.groupBox1.Controls.Add(this.Btn_Run);
            this.groupBox1.Controls.Add(this.Lab_Status);
            this.groupBox1.Location = new System.Drawing.Point(12, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 59);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server狀態";
            // 
            // Btn_Stop
            // 
            this.Btn_Stop.Location = new System.Drawing.Point(128, 21);
            this.Btn_Stop.Name = "Btn_Stop";
            this.Btn_Stop.Size = new System.Drawing.Size(99, 30);
            this.Btn_Stop.TabIndex = 3;
            this.Btn_Stop.Text = "STOP";
            this.Btn_Stop.UseVisualStyleBackColor = true;
            this.Btn_Stop.Click += new System.EventHandler(this.Btn_Stop_Click);
            // 
            // Btn_Run
            // 
            this.Btn_Run.Location = new System.Drawing.Point(13, 21);
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
            this.Lab_Status.Location = new System.Drawing.Point(266, 30);
            this.Lab_Status.Name = "Lab_Status";
            this.Lab_Status.Size = new System.Drawing.Size(73, 12);
            this.Lab_Status.TabIndex = 2;
            this.Lab_Status.Text = "SERVER停止";
            // 
            // Gbx_NodeList
            // 
            this.Gbx_NodeList.Controls.Add(this.Txt_Length);
            this.Gbx_NodeList.Controls.Add(this.Txt_Index);
            this.Gbx_NodeList.Controls.Add(this.Txt_ItemName);
            this.Gbx_NodeList.Controls.Add(this.Txt_NodeId);
            this.Gbx_NodeList.Controls.Add(this.Btn_UpdateValue);
            this.Gbx_NodeList.Controls.Add(this.Btn_UpdateFile);
            this.Gbx_NodeList.Controls.Add(this.Txt_Initial);
            this.Gbx_NodeList.Controls.Add(this.Txt_Value);
            this.Gbx_NodeList.Controls.Add(this.Btn_Delete);
            this.Gbx_NodeList.Controls.Add(this.Cbb_DataType);
            this.Gbx_NodeList.Controls.Add(this.Btn_Add);
            this.Gbx_NodeList.Controls.Add(this.label1);
            this.Gbx_NodeList.Controls.Add(this.label6);
            this.Gbx_NodeList.Controls.Add(this.label7);
            this.Gbx_NodeList.Controls.Add(this.label2);
            this.Gbx_NodeList.Controls.Add(this.label3);
            this.Gbx_NodeList.Controls.Add(this.label5);
            this.Gbx_NodeList.Controls.Add(this.label4);
            this.Gbx_NodeList.Location = new System.Drawing.Point(569, 227);
            this.Gbx_NodeList.Name = "Gbx_NodeList";
            this.Gbx_NodeList.Size = new System.Drawing.Size(551, 209);
            this.Gbx_NodeList.TabIndex = 45;
            this.Gbx_NodeList.TabStop = false;
            this.Gbx_NodeList.Text = "NodeList";
            // 
            // Txt_Initial
            // 
            this.Txt_Initial.Location = new System.Drawing.Point(82, 132);
            this.Txt_Initial.Name = "Txt_Initial";
            this.Txt_Initial.Size = new System.Drawing.Size(353, 22);
            this.Txt_Initial.TabIndex = 29;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("新細明體", 10F);
            this.label6.Location = new System.Drawing.Point(10, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 14);
            this.label6.TabIndex = 37;
            this.label6.Text = "Initial";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Btn_Close);
            this.groupBox2.Location = new System.Drawing.Point(407, 69);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(156, 61);
            this.groupBox2.TabIndex = 47;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Form Close";
            // 
            // Cbb_EndpointsUrl
            // 
            this.Cbb_EndpointsUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Cbb_EndpointsUrl.FormattingEnabled = true;
            this.Cbb_EndpointsUrl.Location = new System.Drawing.Point(18, 33);
            this.Cbb_EndpointsUrl.Name = "Cbb_EndpointsUrl";
            this.Cbb_EndpointsUrl.Size = new System.Drawing.Size(375, 20);
            this.Cbb_EndpointsUrl.TabIndex = 48;
            this.Cbb_EndpointsUrl.Text = "opc.tcp://127.0.0.1:62547/DataAccessServer";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Lsv_Sessions);
            this.groupBox3.Location = new System.Drawing.Point(12, 136);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(523, 129);
            this.groupBox3.TabIndex = 49;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "外部連線狀態";
            // 
            // Lsv_Sessions
            // 
            this.Lsv_Sessions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SessionIdCH,
            this.SessionNameCH,
            this.UserNameCH,
            this.LastContactTimeCH});
            this.Lsv_Sessions.FullRowSelect = true;
            this.Lsv_Sessions.HideSelection = false;
            this.Lsv_Sessions.Location = new System.Drawing.Point(26, 21);
            this.Lsv_Sessions.Name = "Lsv_Sessions";
            this.Lsv_Sessions.Size = new System.Drawing.Size(475, 94);
            this.Lsv_Sessions.TabIndex = 1;
            this.Lsv_Sessions.UseCompatibleStateImageBehavior = false;
            this.Lsv_Sessions.View = System.Windows.Forms.View.Details;
            // 
            // SessionIdCH
            // 
            this.SessionIdCH.Text = "連結溝通Id";
            this.SessionIdCH.Width = 128;
            // 
            // SessionNameCH
            // 
            this.SessionNameCH.Text = "名稱";
            this.SessionNameCH.Width = 111;
            // 
            // UserNameCH
            // 
            this.UserNameCH.Text = "使用者";
            this.UserNameCH.Width = 96;
            // 
            // LastContactTimeCH
            // 
            this.LastContactTimeCH.Text = "最後一次連結時間";
            this.LastContactTimeCH.Width = 126;
            // 
            // Lsv_Subscriptions
            // 
            this.Lsv_Subscriptions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SubscriptionIdCH,
            this.PublishingIntervalCH,
            this.ItemCountCH,
            this.SequenceNumberCH});
            this.Lsv_Subscriptions.FullRowSelect = true;
            this.Lsv_Subscriptions.HideSelection = false;
            this.Lsv_Subscriptions.Location = new System.Drawing.Point(26, 21);
            this.Lsv_Subscriptions.Name = "Lsv_Subscriptions";
            this.Lsv_Subscriptions.Size = new System.Drawing.Size(475, 130);
            this.Lsv_Subscriptions.TabIndex = 2;
            this.Lsv_Subscriptions.UseCompatibleStateImageBehavior = false;
            this.Lsv_Subscriptions.View = System.Windows.Forms.View.Details;
            // 
            // SubscriptionIdCH
            // 
            this.SubscriptionIdCH.Text = "訂閱Id";
            this.SubscriptionIdCH.Width = 107;
            // 
            // PublishingIntervalCH
            // 
            this.PublishingIntervalCH.Text = "發行間隔";
            this.PublishingIntervalCH.Width = 102;
            // 
            // ItemCountCH
            // 
            this.ItemCountCH.Text = "項目記數";
            this.ItemCountCH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ItemCountCH.Width = 121;
            // 
            // SequenceNumberCH
            // 
            this.SequenceNumberCH.Text = "S/N";
            this.SequenceNumberCH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.SequenceNumberCH.Width = 112;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.Lsv_Subscriptions);
            this.groupBox4.Location = new System.Drawing.Point(12, 271);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(523, 165);
            this.groupBox4.TabIndex = 49;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "訂閱狀態";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "Server位址";
            // 
            // StatusBAR
            // 
            this.StatusBAR.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Lab_Time,
            this.Lab_ServerTimeNow,
            this.Lab_sessions,
            this.Lab_sessionsCount,
            this.Lab_subscriptions,
            this.Lab_subscriptionsCount,
            this.Lab_items,
            this.Lab_itemsCount});
            this.StatusBAR.Location = new System.Drawing.Point(0, 453);
            this.StatusBAR.Name = "StatusBAR";
            this.StatusBAR.Size = new System.Drawing.Size(1155, 22);
            this.StatusBAR.TabIndex = 50;
            this.StatusBAR.Text = "statusStrip1";
            // 
            // Lab_Time
            // 
            this.Lab_Time.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.Lab_Time.Name = "Lab_Time";
            this.Lab_Time.Size = new System.Drawing.Size(66, 17);
            this.Lab_Time.Text = "目前時間:";
            // 
            // Lab_ServerTimeNow
            // 
            this.Lab_ServerTimeNow.Name = "Lab_ServerTimeNow";
            this.Lab_ServerTimeNow.Size = new System.Drawing.Size(55, 17);
            this.Lab_ServerTimeNow.Text = "00:00:00";
            // 
            // Lab_sessions
            // 
            this.Lab_sessions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.Lab_sessions.Name = "Lab_sessions";
            this.Lab_sessions.Size = new System.Drawing.Size(52, 17);
            this.Lab_sessions.Text = "連結數:";
            // 
            // Lab_sessionsCount
            // 
            this.Lab_sessionsCount.Name = "Lab_sessionsCount";
            this.Lab_sessionsCount.Size = new System.Drawing.Size(14, 17);
            this.Lab_sessionsCount.Text = "0";
            // 
            // Lab_subscriptions
            // 
            this.Lab_subscriptions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.Lab_subscriptions.Name = "Lab_subscriptions";
            this.Lab_subscriptions.Size = new System.Drawing.Size(52, 17);
            this.Lab_subscriptions.Text = "訂閱數:";
            // 
            // Lab_subscriptionsCount
            // 
            this.Lab_subscriptionsCount.Name = "Lab_subscriptionsCount";
            this.Lab_subscriptionsCount.Size = new System.Drawing.Size(14, 17);
            this.Lab_subscriptionsCount.Text = "0";
            // 
            // Lab_items
            // 
            this.Lab_items.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.Lab_items.Name = "Lab_items";
            this.Lab_items.Size = new System.Drawing.Size(52, 17);
            this.Lab_items.Text = "項目數:";
            // 
            // Lab_itemsCount
            // 
            this.Lab_itemsCount.Name = "Lab_itemsCount";
            this.Lab_itemsCount.Size = new System.Drawing.Size(14, 17);
            this.Lab_itemsCount.Text = "0";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.Lsv_VariableList);
            this.groupBox5.Location = new System.Drawing.Point(569, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(551, 209);
            this.groupBox5.TabIndex = 49;
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1155, 475);
            this.Controls.Add(this.StatusBAR);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.Cbb_EndpointsUrl);
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
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.StatusBAR.ResumeLayout(false);
            this.StatusBAR.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_Close;
        private System.Windows.Forms.Button Btn_UpdateFile;
        private System.Windows.Forms.Button Btn_Delete;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox Cbb_DataType;
        private System.Windows.Forms.TextBox Txt_Value;
        private System.Windows.Forms.TextBox Txt_Length;
        private System.Windows.Forms.TextBox Txt_NodeId;
        private System.Windows.Forms.TextBox Txt_ItemName;
        private System.Windows.Forms.TextBox Txt_Index;
        private System.Windows.Forms.Button Btn_UpdateValue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Btn_Stop;
        private System.Windows.Forms.Button Btn_Run;
        private System.Windows.Forms.Label Lab_Status;
        private System.Windows.Forms.GroupBox Gbx_NodeList;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox Cbb_EndpointsUrl;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView Lsv_Sessions;
        private System.Windows.Forms.ColumnHeader SessionIdCH;
        private System.Windows.Forms.ColumnHeader SessionNameCH;
        private System.Windows.Forms.ColumnHeader UserNameCH;
        private System.Windows.Forms.ColumnHeader LastContactTimeCH;
        private System.Windows.Forms.ListView Lsv_Subscriptions;
        private System.Windows.Forms.ColumnHeader SubscriptionIdCH;
        private System.Windows.Forms.ColumnHeader PublishingIntervalCH;
        private System.Windows.Forms.ColumnHeader ItemCountCH;
        private System.Windows.Forms.ColumnHeader SequenceNumberCH;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.StatusStrip StatusBAR;
        private System.Windows.Forms.ToolStripStatusLabel Lab_Time;
        private System.Windows.Forms.ToolStripStatusLabel Lab_ServerTimeNow;
        private System.Windows.Forms.ToolStripStatusLabel Lab_sessions;
        private System.Windows.Forms.ToolStripStatusLabel Lab_sessionsCount;
        private System.Windows.Forms.ToolStripStatusLabel Lab_subscriptions;
        private System.Windows.Forms.ToolStripStatusLabel Lab_subscriptionsCount;
        private System.Windows.Forms.ToolStripStatusLabel Lab_items;
        private System.Windows.Forms.ToolStripStatusLabel Lab_itemsCount;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListView Lsv_VariableList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.TextBox Txt_Initial;
        private System.Windows.Forms.Label label6;
    }
}

