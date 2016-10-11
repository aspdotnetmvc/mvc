namespace ZHSMSServiceHost
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
            this.ssHost = new System.Windows.Forms.StatusStrip();
            this.tsslStartTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsstStartTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsstRunTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsHost = new System.Windows.Forms.ToolStrip();
            this.tsbExit = new System.Windows.Forms.ToolStripButton();
            this.tcMonitoring = new System.Windows.Forms.TabControl();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.tpParameter = new System.Windows.Forms.TabPage();
            this.ssHost.SuspendLayout();
            this.tsHost.SuspendLayout();
            this.tcMonitoring.SuspendLayout();
            this.tpOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // ssHost
            // 
            this.ssHost.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStartTime,
            this.tsstStartTime,
            this.tsstRunTime});
            this.ssHost.Location = new System.Drawing.Point(0, 317);
            this.ssHost.Name = "ssHost";
            this.ssHost.Size = new System.Drawing.Size(630, 22);
            this.ssHost.TabIndex = 9;
            this.ssHost.Text = "status";
            // 
            // tsslStartTime
            // 
            this.tsslStartTime.Name = "tsslStartTime";
            this.tsslStartTime.Size = new System.Drawing.Size(65, 17);
            this.tsslStartTime.Text = "start time:";
            // 
            // tsstStartTime
            // 
            this.tsstStartTime.Name = "tsstStartTime";
            this.tsstStartTime.Size = new System.Drawing.Size(0, 17);
            // 
            // tsstRunTime
            // 
            this.tsstRunTime.Name = "tsstRunTime";
            this.tsstRunTime.Size = new System.Drawing.Size(0, 17);
            // 
            // tsHost
            // 
            this.tsHost.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.tsHost.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbExit});
            this.tsHost.Location = new System.Drawing.Point(0, 0);
            this.tsHost.Name = "tsHost";
            this.tsHost.Size = new System.Drawing.Size(630, 25);
            this.tsHost.TabIndex = 10;
            this.tsHost.Text = "toolStrip1";
            // 
            // tsbExit
            // 
            this.tsbExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbExit.Name = "tsbExit";
            this.tsbExit.Size = new System.Drawing.Size(31, 22);
            this.tsbExit.Text = "Exit";
            this.tsbExit.Click += new System.EventHandler(this.tsbExit_Click);
            // 
            // tcMonitoring
            // 
            this.tcMonitoring.Controls.Add(this.tpOutput);
            this.tcMonitoring.Controls.Add(this.tpParameter);
            this.tcMonitoring.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMonitoring.Location = new System.Drawing.Point(0, 25);
            this.tcMonitoring.Name = "tcMonitoring";
            this.tcMonitoring.SelectedIndex = 0;
            this.tcMonitoring.Size = new System.Drawing.Size(630, 292);
            this.tcMonitoring.TabIndex = 11;
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.txtOutput);
            this.tpOutput.Location = new System.Drawing.Point(4, 22);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tpOutput.Size = new System.Drawing.Size(622, 266);
            this.tpOutput.TabIndex = 0;
            this.tpOutput.Text = "Output";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // txtOutput
            // 
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Location = new System.Drawing.Point(3, 3);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(616, 260);
            this.txtOutput.TabIndex = 0;
            // 
            // tpParameter
            // 
            this.tpParameter.Location = new System.Drawing.Point(4, 22);
            this.tpParameter.Name = "tpParameter";
            this.tpParameter.Padding = new System.Windows.Forms.Padding(3);
            this.tpParameter.Size = new System.Drawing.Size(622, 266);
            this.tpParameter.TabIndex = 1;
            this.tpParameter.Text = "Parameter";
            this.tpParameter.UseVisualStyleBackColor = true;
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 339);
            this.Controls.Add(this.tcMonitoring);
            this.Controls.Add(this.tsHost);
            this.Controls.Add(this.ssHost);
            this.Name = "fMain";
            this.Text = "SMSServer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.fMain_FormClosed);
            this.Load += new System.EventHandler(this.fMain_Load);
            this.ssHost.ResumeLayout(false);
            this.ssHost.PerformLayout();
            this.tsHost.ResumeLayout(false);
            this.tsHost.PerformLayout();
            this.tcMonitoring.ResumeLayout(false);
            this.tpOutput.ResumeLayout(false);
            this.tpOutput.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip ssHost;
        private System.Windows.Forms.ToolStripStatusLabel tsslStartTime;
        private System.Windows.Forms.ToolStripStatusLabel tsstStartTime;
        private System.Windows.Forms.ToolStripStatusLabel tsstRunTime;
        private System.Windows.Forms.ToolStrip tsHost;
        private System.Windows.Forms.ToolStripButton tsbExit;
        private System.Windows.Forms.TabControl tcMonitoring;
        private System.Windows.Forms.TabPage tpOutput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.TabPage tpParameter;
    }
}