namespace LogQuery
{
    partial class fMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fMain));
            this.tools = new System.Windows.Forms.ToolStrip();
            this.labDate = new System.Windows.Forms.ToolStripLabel();
            this.txtBeginDate = new System.Windows.Forms.ToolStripTextBox();
            this.labDateLine = new System.Windows.Forms.ToolStripLabel();
            this.txtEndDate = new System.Windows.Forms.ToolStripTextBox();
            this.labLevel = new System.Windows.Forms.ToolStripLabel();
            this.cbxLevel = new System.Windows.Forms.ToolStripComboBox();
            this.labRecorder = new System.Windows.Forms.ToolStripLabel();
            this.cbxRecorder = new System.Windows.Forms.ToolStripComboBox();
            this.labEnent = new System.Windows.Forms.ToolStripLabel();
            this.cbxEvent = new System.Windows.Forms.ToolStripComboBox();
            this.tss1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnQuery = new System.Windows.Forms.ToolStripButton();
            this.txtFilter = new System.Windows.Forms.ToolStripTextBox();
            this.tsbFilter = new System.Windows.Forms.ToolStripButton();
            this.status = new System.Windows.Forms.StatusStrip();
            this.sslabCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.view = new System.Windows.Forms.DataGridView();
            this.tsbKQuery = new System.Windows.Forms.ToolStripButton();
            this.tools.SuspendLayout();
            this.status.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.view)).BeginInit();
            this.SuspendLayout();
            // 
            // tools
            // 
            this.tools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labDate,
            this.txtBeginDate,
            this.labDateLine,
            this.txtEndDate,
            this.labLevel,
            this.cbxLevel,
            this.labRecorder,
            this.cbxRecorder,
            this.labEnent,
            this.cbxEvent,
            this.tss1,
            this.tsbKQuery,
            this.btnQuery,
            this.txtFilter,
            this.tsbFilter});
            this.tools.Location = new System.Drawing.Point(0, 0);
            this.tools.Name = "tools";
            this.tools.Size = new System.Drawing.Size(1174, 25);
            this.tools.TabIndex = 0;
            this.tools.Text = "toolStrip1";
            // 
            // labDate
            // 
            this.labDate.Name = "labDate";
            this.labDate.Size = new System.Drawing.Size(38, 22);
            this.labDate.Text = "Date:";
            // 
            // txtBeginDate
            // 
            this.txtBeginDate.Name = "txtBeginDate";
            this.txtBeginDate.Size = new System.Drawing.Size(130, 25);
            this.txtBeginDate.Click += new System.EventHandler(this.txtBeginDate_Click);
            // 
            // labDateLine
            // 
            this.labDateLine.Name = "labDateLine";
            this.labDateLine.Size = new System.Drawing.Size(13, 22);
            this.labDateLine.Text = "-";
            // 
            // txtEndDate
            // 
            this.txtEndDate.Name = "txtEndDate";
            this.txtEndDate.Size = new System.Drawing.Size(130, 25);
            this.txtEndDate.Click += new System.EventHandler(this.txtEndDate_Click);
            // 
            // labLevel
            // 
            this.labLevel.Name = "labLevel";
            this.labLevel.Size = new System.Drawing.Size(40, 22);
            this.labLevel.Text = "Level:";
            // 
            // cbxLevel
            // 
            this.cbxLevel.Name = "cbxLevel";
            this.cbxLevel.Size = new System.Drawing.Size(121, 25);
            // 
            // labRecorder
            // 
            this.labRecorder.Name = "labRecorder";
            this.labRecorder.Size = new System.Drawing.Size(65, 22);
            this.labRecorder.Text = "Recorder:";
            // 
            // cbxRecorder
            // 
            this.cbxRecorder.Name = "cbxRecorder";
            this.cbxRecorder.Size = new System.Drawing.Size(121, 25);
            // 
            // labEnent
            // 
            this.labEnent.Name = "labEnent";
            this.labEnent.Size = new System.Drawing.Size(42, 22);
            this.labEnent.Text = "Event:";
            // 
            // cbxEvent
            // 
            this.cbxEvent.Name = "cbxEvent";
            this.cbxEvent.Size = new System.Drawing.Size(121, 25);
            // 
            // tss1
            // 
            this.tss1.Name = "tss1";
            this.tss1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnQuery
            // 
            this.btnQuery.Image = ((System.Drawing.Image)(resources.GetObject("btnQuery.Image")));
            this.btnQuery.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(52, 22);
            this.btnQuery.Text = "查询";
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // txtFilter
            // 
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(100, 25);
            // 
            // tsbFilter
            // 
            this.tsbFilter.Image = ((System.Drawing.Image)(resources.GetObject("tsbFilter.Image")));
            this.tsbFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFilter.Name = "tsbFilter";
            this.tsbFilter.Size = new System.Drawing.Size(88, 22);
            this.tsbFilter.Text = "关键字过滤";
            this.tsbFilter.Click += new System.EventHandler(this.tsbFilter_Click);
            // 
            // status
            // 
            this.status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sslabCount});
            this.status.Location = new System.Drawing.Point(0, 543);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(1174, 22);
            this.status.TabIndex = 1;
            this.status.Text = "statusStrip1";
            // 
            // sslabCount
            // 
            this.sslabCount.ForeColor = System.Drawing.Color.Blue;
            this.sslabCount.Name = "sslabCount";
            this.sslabCount.Size = new System.Drawing.Size(0, 17);
            // 
            // view
            // 
            this.view.AllowUserToAddRows = false;
            this.view.AllowUserToDeleteRows = false;
            this.view.AllowUserToOrderColumns = true;
            this.view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.view.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view.Location = new System.Drawing.Point(0, 25);
            this.view.Name = "view";
            this.view.ReadOnly = true;
            this.view.RowTemplate.Height = 23;
            this.view.Size = new System.Drawing.Size(1174, 518);
            this.view.TabIndex = 2;
            this.view.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.view_CellDoubleClick);
            // 
            // tsbKQuery
            // 
            this.tsbKQuery.Image = ((System.Drawing.Image)(resources.GetObject("tsbKQuery.Image")));
            this.tsbKQuery.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbKQuery.Name = "tsbKQuery";
            this.tsbKQuery.Size = new System.Drawing.Size(52, 22);
            this.tsbKQuery.Text = "快查";
            this.tsbKQuery.Click += new System.EventHandler(this.tsbKQuery_Click);
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1174, 565);
            this.Controls.Add(this.view);
            this.Controls.Add(this.status);
            this.Controls.Add(this.tools);
            this.Name = "fMain";
            this.Text = "日志查询";
            this.Load += new System.EventHandler(this.fMain_Load);
            this.tools.ResumeLayout(false);
            this.tools.PerformLayout();
            this.status.ResumeLayout(false);
            this.status.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.view)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tools;
        private System.Windows.Forms.StatusStrip status;
        private System.Windows.Forms.DataGridView view;
        private System.Windows.Forms.ToolStripLabel labRecorder;
        private System.Windows.Forms.ToolStripComboBox cbxRecorder;
        private System.Windows.Forms.ToolStripLabel labLevel;
        private System.Windows.Forms.ToolStripComboBox cbxLevel;
        private System.Windows.Forms.ToolStripLabel labDate;
        private System.Windows.Forms.ToolStripTextBox txtEndDate;
        private System.Windows.Forms.ToolStripLabel labDateLine;
        private System.Windows.Forms.ToolStripTextBox txtBeginDate;
        private System.Windows.Forms.ToolStripLabel labEnent;
        private System.Windows.Forms.ToolStripComboBox cbxEvent;
        private System.Windows.Forms.ToolStripSeparator tss1;
        private System.Windows.Forms.ToolStripButton btnQuery;
        private System.Windows.Forms.ToolStripTextBox txtFilter;
        private System.Windows.Forms.ToolStripButton tsbFilter;
        private System.Windows.Forms.ToolStripStatusLabel sslabCount;
        private System.Windows.Forms.ToolStripButton tsbKQuery;
    }
}