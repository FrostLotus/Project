
namespace OCPUAServer
{
    partial class ServerControl
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

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Lab_Endpoints = new System.Windows.Forms.Label();
            this.Cbb_EndpointsUrl = new System.Windows.Forms.ComboBox();
            this.SessionsGB = new System.Windows.Forms.GroupBox();
            this.Lsv_Sessions = new System.Windows.Forms.ListView();
            this.SessionIdCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SessionNameCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UserNameCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LastContactTimeCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SubscriptionsGB = new System.Windows.Forms.GroupBox();
            this.Lsv_Subscriptions = new System.Windows.Forms.ListView();
            this.SubscriptionIdCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PublishingIntervalCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ItemCountCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SequenceNumberCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.StatusBAR = new System.Windows.Forms.StatusStrip();
            this.Lab_ServerStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_ServerState = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_Time = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_ServerTimeNow = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_sessions = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_sessionsCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_subscriptions = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_subscriptionsCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_items = new System.Windows.Forms.ToolStripStatusLabel();
            this.Lab_itemsCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.SessionsGB.SuspendLayout();
            this.SubscriptionsGB.SuspendLayout();
            this.StatusBAR.SuspendLayout();
            this.SuspendLayout();
            // 
            // Lab_Endpoints
            // 
            this.Lab_Endpoints.AutoSize = true;
            this.Lab_Endpoints.Location = new System.Drawing.Point(4, 9);
            this.Lab_Endpoints.Name = "Lab_Endpoints";
            this.Lab_Endpoints.Size = new System.Drawing.Size(92, 12);
            this.Lab_Endpoints.TabIndex = 2;
            this.Lab_Endpoints.Text = "Server 終端 URLs";
            this.Lab_Endpoints.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Cbb_EndpointsUrl
            // 
            this.Cbb_EndpointsUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Cbb_EndpointsUrl.FormattingEnabled = true;
            this.Cbb_EndpointsUrl.Location = new System.Drawing.Point(102, 6);
            this.Cbb_EndpointsUrl.Name = "Cbb_EndpointsUrl";
            this.Cbb_EndpointsUrl.Size = new System.Drawing.Size(375, 20);
            this.Cbb_EndpointsUrl.TabIndex = 3;
            // 
            // SessionsGB
            // 
            this.SessionsGB.Controls.Add(this.Lsv_Sessions);
            this.SessionsGB.Location = new System.Drawing.Point(3, 32);
            this.SessionsGB.Name = "SessionsGB";
            this.SessionsGB.Size = new System.Drawing.Size(474, 115);
            this.SessionsGB.TabIndex = 4;
            this.SessionsGB.TabStop = false;
            this.SessionsGB.Text = "連結溝通";
            // 
            // Lsv_Sessions
            // 
            this.Lsv_Sessions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SessionIdCH,
            this.SessionNameCH,
            this.UserNameCH,
            this.LastContactTimeCH});
            this.Lsv_Sessions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Lsv_Sessions.FullRowSelect = true;
            this.Lsv_Sessions.HideSelection = false;
            this.Lsv_Sessions.Location = new System.Drawing.Point(3, 18);
            this.Lsv_Sessions.Name = "Lsv_Sessions";
            this.Lsv_Sessions.Size = new System.Drawing.Size(468, 94);
            this.Lsv_Sessions.TabIndex = 0;
            this.Lsv_Sessions.UseCompatibleStateImageBehavior = false;
            this.Lsv_Sessions.View = System.Windows.Forms.View.Details;
            // 
            // SessionIdCH
            // 
            this.SessionIdCH.Text = "連結溝通Id";
            this.SessionIdCH.Width = 125;
            // 
            // SessionNameCH
            // 
            this.SessionNameCH.Text = "名稱";
            this.SessionNameCH.Width = 110;
            // 
            // UserNameCH
            // 
            this.UserNameCH.Text = "使用者";
            this.UserNameCH.Width = 100;
            // 
            // LastContactTimeCH
            // 
            this.LastContactTimeCH.Text = "最後一次連結時間";
            this.LastContactTimeCH.Width = 126;
            // 
            // SubscriptionsGB
            // 
            this.SubscriptionsGB.Controls.Add(this.Lsv_Subscriptions);
            this.SubscriptionsGB.Location = new System.Drawing.Point(6, 153);
            this.SubscriptionsGB.Name = "SubscriptionsGB";
            this.SubscriptionsGB.Size = new System.Drawing.Size(468, 139);
            this.SubscriptionsGB.TabIndex = 5;
            this.SubscriptionsGB.TabStop = false;
            this.SubscriptionsGB.Text = "訂閱";
            // 
            // Lsv_Subscriptions
            // 
            this.Lsv_Subscriptions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SubscriptionIdCH,
            this.PublishingIntervalCH,
            this.ItemCountCH,
            this.SequenceNumberCH});
            this.Lsv_Subscriptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Lsv_Subscriptions.FullRowSelect = true;
            this.Lsv_Subscriptions.HideSelection = false;
            this.Lsv_Subscriptions.Location = new System.Drawing.Point(3, 18);
            this.Lsv_Subscriptions.Name = "Lsv_Subscriptions";
            this.Lsv_Subscriptions.Size = new System.Drawing.Size(462, 118);
            this.Lsv_Subscriptions.TabIndex = 0;
            this.Lsv_Subscriptions.UseCompatibleStateImageBehavior = false;
            this.Lsv_Subscriptions.View = System.Windows.Forms.View.Details;
            // 
            // SubscriptionIdCH
            // 
            this.SubscriptionIdCH.Text = "訂閱Id";
            this.SubscriptionIdCH.Width = 90;
            // 
            // PublishingIntervalCH
            // 
            this.PublishingIntervalCH.Text = "發行間隔";
            this.PublishingIntervalCH.Width = 101;
            // 
            // ItemCountCH
            // 
            this.ItemCountCH.Text = "項目記數";
            this.ItemCountCH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ItemCountCH.Width = 126;
            // 
            // SequenceNumberCH
            // 
            this.SequenceNumberCH.Text = "S/N";
            this.SequenceNumberCH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // StatusBAR
            // 
            this.StatusBAR.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Lab_ServerStatus,
            this.Lab_ServerState,
            this.Lab_Time,
            this.Lab_ServerTimeNow,
            this.Lab_sessions,
            this.Lab_sessionsCount,
            this.Lab_subscriptions,
            this.Lab_subscriptionsCount,
            this.Lab_items,
            this.Lab_itemsCount});
            this.StatusBAR.Location = new System.Drawing.Point(0, 300);
            this.StatusBAR.Name = "StatusBAR";
            this.StatusBAR.Size = new System.Drawing.Size(494, 22);
            this.StatusBAR.TabIndex = 6;
            this.StatusBAR.Text = "statusStrip1";
            // 
            // Lab_ServerStatus
            // 
            this.Lab_ServerStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.Lab_ServerStatus.Name = "Lab_ServerStatus";
            this.Lab_ServerStatus.Size = new System.Drawing.Size(38, 17);
            this.Lab_ServerStatus.Text = "狀態:";
            // 
            // Lab_ServerState
            // 
            this.Lab_ServerState.Name = "Lab_ServerState";
            this.Lab_ServerState.Size = new System.Drawing.Size(54, 17);
            this.Lab_ServerState.Text = "Running";
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
            // UpdateTimer
            // 
            this.UpdateTimer.Interval = 1000;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // ServerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StatusBAR);
            this.Controls.Add(this.SubscriptionsGB);
            this.Controls.Add(this.SessionsGB);
            this.Controls.Add(this.Lab_Endpoints);
            this.Controls.Add(this.Cbb_EndpointsUrl);
            this.Name = "ServerControl";
            this.Size = new System.Drawing.Size(494, 322);
            this.SessionsGB.ResumeLayout(false);
            this.SubscriptionsGB.ResumeLayout(false);
            this.StatusBAR.ResumeLayout(false);
            this.StatusBAR.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lab_Endpoints;
        private System.Windows.Forms.ComboBox Cbb_EndpointsUrl;
        private System.Windows.Forms.GroupBox SessionsGB;
        private System.Windows.Forms.ListView Lsv_Sessions;
        private System.Windows.Forms.ColumnHeader UserNameCH;
        private System.Windows.Forms.ColumnHeader LastContactTimeCH;
        private System.Windows.Forms.GroupBox SubscriptionsGB;
        private System.Windows.Forms.ListView Lsv_Subscriptions;
        private System.Windows.Forms.ColumnHeader SubscriptionIdCH;
        private System.Windows.Forms.ColumnHeader PublishingIntervalCH;
        private System.Windows.Forms.ColumnHeader ItemCountCH;
        private System.Windows.Forms.ColumnHeader SequenceNumberCH;
        private System.Windows.Forms.StatusStrip StatusBAR;
        private System.Windows.Forms.ToolStripStatusLabel Lab_ServerStatus;
        private System.Windows.Forms.ToolStripStatusLabel Lab_ServerState;
        private System.Windows.Forms.ToolStripStatusLabel Lab_Time;
        private System.Windows.Forms.ToolStripStatusLabel Lab_ServerTimeNow;
        private System.Windows.Forms.ToolStripStatusLabel Lab_sessions;
        private System.Windows.Forms.ToolStripStatusLabel Lab_sessionsCount;
        private System.Windows.Forms.ToolStripStatusLabel Lab_subscriptions;
        private System.Windows.Forms.ToolStripStatusLabel Lab_subscriptionsCount;
        private System.Windows.Forms.ToolStripStatusLabel Lab_items;
        private System.Windows.Forms.ToolStripStatusLabel Lab_itemsCount;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.ColumnHeader SessionIdCH;
        private System.Windows.Forms.ColumnHeader SessionNameCH;
    }
}
