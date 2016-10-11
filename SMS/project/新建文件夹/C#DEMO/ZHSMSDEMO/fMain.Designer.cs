namespace ZHSMSDEMO
{
    partial class fMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dgv = new System.Windows.Forms.DataGridView();
            this.labAccount = new System.Windows.Forms.Label();
            this.txtAccount = new System.Windows.Forms.TextBox();
            this.labPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.labDestinations = new System.Windows.Forms.Label();
            this.txtDestinations = new System.Windows.Forms.TextBox();
            this.labContent = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.labWapPushUrl = new System.Windows.Forms.Label();
            this.txtWapPushUrl = new System.Windows.Forms.TextBox();
            this.labSendTime = new System.Windows.Forms.Label();
            this.txtSendTime = new System.Windows.Forms.TextBox();
            this.labSendTimeRemark = new System.Windows.Forms.Label();
            this.btnWS = new System.Windows.Forms.Button();
            this.btnHS = new System.Windows.Forms.Button();
            this.btnWGS = new System.Windows.Forms.Button();
            this.btnHGS = new System.Windows.Forms.Button();
            this.btnWGR = new System.Windows.Forms.Button();
            this.btnHGR = new System.Windows.Forms.Button();
            this.btnWGB = new System.Windows.Forms.Button();
            this.btnHGB = new System.Windows.Forms.Button();
            this.labSubCode = new System.Windows.Forms.Label();
            this.txtSubCode = new System.Windows.Forms.TextBox();
            this.labSubCodeRemark = new System.Windows.Forms.Label();
            this.btnWSEX = new System.Windows.Forms.Button();
            this.btnHSEX = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(12, 275);
            this.dgv.Name = "dgv";
            this.dgv.RowTemplate.Height = 23;
            this.dgv.Size = new System.Drawing.Size(774, 222);
            this.dgv.TabIndex = 1;
            // 
            // labAccount
            // 
            this.labAccount.AutoSize = true;
            this.labAccount.Location = new System.Drawing.Point(10, 16);
            this.labAccount.Name = "labAccount";
            this.labAccount.Size = new System.Drawing.Size(65, 12);
            this.labAccount.TabIndex = 2;
            this.labAccount.Text = "用户账号：";
            // 
            // txtAccount
            // 
            this.txtAccount.Location = new System.Drawing.Point(93, 13);
            this.txtAccount.Name = "txtAccount";
            this.txtAccount.Size = new System.Drawing.Size(150, 21);
            this.txtAccount.TabIndex = 0;
            // 
            // labPassword
            // 
            this.labPassword.AutoSize = true;
            this.labPassword.Location = new System.Drawing.Point(261, 16);
            this.labPassword.Name = "labPassword";
            this.labPassword.Size = new System.Drawing.Size(65, 12);
            this.labPassword.TabIndex = 2;
            this.labPassword.Text = "用户密码：";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(323, 13);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(150, 21);
            this.txtPassword.TabIndex = 1;
            // 
            // labDestinations
            // 
            this.labDestinations.AutoSize = true;
            this.labDestinations.Location = new System.Drawing.Point(10, 69);
            this.labDestinations.Name = "labDestinations";
            this.labDestinations.Size = new System.Drawing.Size(65, 12);
            this.labDestinations.TabIndex = 2;
            this.labDestinations.Text = "接收号码：";
            // 
            // txtDestinations
            // 
            this.txtDestinations.Location = new System.Drawing.Point(93, 66);
            this.txtDestinations.Multiline = true;
            this.txtDestinations.Name = "txtDestinations";
            this.txtDestinations.Size = new System.Drawing.Size(497, 61);
            this.txtDestinations.TabIndex = 3;
            // 
            // labContent
            // 
            this.labContent.AutoSize = true;
            this.labContent.Location = new System.Drawing.Point(10, 136);
            this.labContent.Name = "labContent";
            this.labContent.Size = new System.Drawing.Size(65, 12);
            this.labContent.TabIndex = 2;
            this.labContent.Text = "短信内容：";
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(93, 133);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(497, 61);
            this.txtContent.TabIndex = 4;
            // 
            // labWapPushUrl
            // 
            this.labWapPushUrl.AutoSize = true;
            this.labWapPushUrl.Location = new System.Drawing.Point(10, 203);
            this.labWapPushUrl.Name = "labWapPushUrl";
            this.labWapPushUrl.Size = new System.Drawing.Size(77, 12);
            this.labWapPushUrl.TabIndex = 2;
            this.labWapPushUrl.Text = "WapPushUrl：";
            // 
            // txtWapPushUrl
            // 
            this.txtWapPushUrl.Location = new System.Drawing.Point(93, 200);
            this.txtWapPushUrl.Name = "txtWapPushUrl";
            this.txtWapPushUrl.Size = new System.Drawing.Size(497, 21);
            this.txtWapPushUrl.TabIndex = 5;
            // 
            // labSendTime
            // 
            this.labSendTime.AutoSize = true;
            this.labSendTime.Location = new System.Drawing.Point(10, 230);
            this.labSendTime.Name = "labSendTime";
            this.labSendTime.Size = new System.Drawing.Size(65, 12);
            this.labSendTime.TabIndex = 2;
            this.labSendTime.Text = "发送时间：";
            // 
            // txtSendTime
            // 
            this.txtSendTime.Location = new System.Drawing.Point(93, 227);
            this.txtSendTime.Name = "txtSendTime";
            this.txtSendTime.Size = new System.Drawing.Size(150, 21);
            this.txtSendTime.TabIndex = 6;
            // 
            // labSendTimeRemark
            // 
            this.labSendTimeRemark.AutoSize = true;
            this.labSendTimeRemark.Location = new System.Drawing.Point(249, 230);
            this.labSendTimeRemark.Name = "labSendTimeRemark";
            this.labSendTimeRemark.Size = new System.Drawing.Size(365, 12);
            this.labSendTimeRemark.TabIndex = 2;
            this.labSendTimeRemark.Text = "格式：年月日时分，201404150832，无需定时发送此参数设置为空。";
            // 
            // btnWS
            // 
            this.btnWS.Location = new System.Drawing.Point(641, 11);
            this.btnWS.Name = "btnWS";
            this.btnWS.Size = new System.Drawing.Size(145, 23);
            this.btnWS.TabIndex = 7;
            this.btnWS.Text = "webservice send";
            this.btnWS.UseVisualStyleBackColor = true;
            this.btnWS.Click += new System.EventHandler(this.btnWS_Click);
            // 
            // btnHS
            // 
            this.btnHS.Location = new System.Drawing.Point(641, 36);
            this.btnHS.Name = "btnHS";
            this.btnHS.Size = new System.Drawing.Size(145, 23);
            this.btnHS.TabIndex = 8;
            this.btnHS.Text = "http send";
            this.btnHS.UseVisualStyleBackColor = true;
            this.btnHS.Click += new System.EventHandler(this.btnHS_Click);
            // 
            // btnWGS
            // 
            this.btnWGS.Location = new System.Drawing.Point(641, 61);
            this.btnWGS.Name = "btnWGS";
            this.btnWGS.Size = new System.Drawing.Size(145, 23);
            this.btnWGS.TabIndex = 9;
            this.btnWGS.Text = "webservice get sms";
            this.btnWGS.UseVisualStyleBackColor = true;
            this.btnWGS.Click += new System.EventHandler(this.btnWGS_Click);
            // 
            // btnHGS
            // 
            this.btnHGS.Location = new System.Drawing.Point(641, 87);
            this.btnHGS.Name = "btnHGS";
            this.btnHGS.Size = new System.Drawing.Size(145, 23);
            this.btnHGS.TabIndex = 10;
            this.btnHGS.Text = "http get sms";
            this.btnHGS.UseVisualStyleBackColor = true;
            this.btnHGS.Click += new System.EventHandler(this.btnHGS_Click);
            // 
            // btnWGR
            // 
            this.btnWGR.Location = new System.Drawing.Point(641, 112);
            this.btnWGR.Name = "btnWGR";
            this.btnWGR.Size = new System.Drawing.Size(145, 23);
            this.btnWGR.TabIndex = 11;
            this.btnWGR.Text = "webservice get report";
            this.btnWGR.UseVisualStyleBackColor = true;
            this.btnWGR.Click += new System.EventHandler(this.btnWGR_Click);
            // 
            // btnHGR
            // 
            this.btnHGR.Location = new System.Drawing.Point(641, 138);
            this.btnHGR.Name = "btnHGR";
            this.btnHGR.Size = new System.Drawing.Size(145, 23);
            this.btnHGR.TabIndex = 12;
            this.btnHGR.Text = "http get report";
            this.btnHGR.UseVisualStyleBackColor = true;
            this.btnHGR.Click += new System.EventHandler(this.btnHGR_Click);
            // 
            // btnWGB
            // 
            this.btnWGB.Location = new System.Drawing.Point(641, 164);
            this.btnWGB.Name = "btnWGB";
            this.btnWGB.Size = new System.Drawing.Size(145, 23);
            this.btnWGB.TabIndex = 13;
            this.btnWGB.Text = "webservice get Balance";
            this.btnWGB.UseVisualStyleBackColor = true;
            this.btnWGB.Click += new System.EventHandler(this.btnWGB_Click);
            // 
            // btnHGB
            // 
            this.btnHGB.Location = new System.Drawing.Point(641, 190);
            this.btnHGB.Name = "btnHGB";
            this.btnHGB.Size = new System.Drawing.Size(145, 23);
            this.btnHGB.TabIndex = 14;
            this.btnHGB.Text = "http get Balance";
            this.btnHGB.UseVisualStyleBackColor = true;
            this.btnHGB.Click += new System.EventHandler(this.btnHGB_Click);
            // 
            // labSubCode
            // 
            this.labSubCode.AutoSize = true;
            this.labSubCode.Location = new System.Drawing.Point(10, 42);
            this.labSubCode.Name = "labSubCode";
            this.labSubCode.Size = new System.Drawing.Size(65, 12);
            this.labSubCode.TabIndex = 2;
            this.labSubCode.Text = "扩展子号：";
            // 
            // txtSubCode
            // 
            this.txtSubCode.Location = new System.Drawing.Point(93, 39);
            this.txtSubCode.Name = "txtSubCode";
            this.txtSubCode.Size = new System.Drawing.Size(150, 21);
            this.txtSubCode.TabIndex = 2;
            // 
            // labSubCodeRemark
            // 
            this.labSubCodeRemark.AutoSize = true;
            this.labSubCodeRemark.Location = new System.Drawing.Point(261, 43);
            this.labSubCodeRemark.Name = "labSubCodeRemark";
            this.labSubCodeRemark.Size = new System.Drawing.Size(317, 12);
            this.labSubCodeRemark.TabIndex = 2;
            this.labSubCodeRemark.Text = "接收短信的号码，最多不超过100个，中间用英文“,”隔开";
            // 
            // btnWSEX
            // 
            this.btnWSEX.Location = new System.Drawing.Point(641, 216);
            this.btnWSEX.Name = "btnWSEX";
            this.btnWSEX.Size = new System.Drawing.Size(145, 23);
            this.btnWSEX.TabIndex = 7;
            this.btnWSEX.Text = "webservice send EX";
            this.btnWSEX.UseVisualStyleBackColor = true;
            this.btnWSEX.Click += new System.EventHandler(this.btnWSEX_Click);
            // 
            // btnHSEX
            // 
            this.btnHSEX.Location = new System.Drawing.Point(641, 241);
            this.btnHSEX.Name = "btnHSEX";
            this.btnHSEX.Size = new System.Drawing.Size(145, 23);
            this.btnHSEX.TabIndex = 8;
            this.btnHSEX.Text = "http send EX";
            this.btnHSEX.UseVisualStyleBackColor = true;
            this.btnHSEX.Click += new System.EventHandler(this.btnHSEX_Click);
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 509);
            this.Controls.Add(this.btnHGB);
            this.Controls.Add(this.btnHGR);
            this.Controls.Add(this.btnWGB);
            this.Controls.Add(this.btnWGR);
            this.Controls.Add(this.btnHGS);
            this.Controls.Add(this.btnWGS);
            this.Controls.Add(this.btnHSEX);
            this.Controls.Add(this.btnWSEX);
            this.Controls.Add(this.btnHS);
            this.Controls.Add(this.btnWS);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.labPassword);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.txtDestinations);
            this.Controls.Add(this.labContent);
            this.Controls.Add(this.txtWapPushUrl);
            this.Controls.Add(this.txtSendTime);
            this.Controls.Add(this.txtSubCode);
            this.Controls.Add(this.txtAccount);
            this.Controls.Add(this.labWapPushUrl);
            this.Controls.Add(this.labSendTimeRemark);
            this.Controls.Add(this.labSendTime);
            this.Controls.Add(this.labSubCodeRemark);
            this.Controls.Add(this.labSubCode);
            this.Controls.Add(this.labDestinations);
            this.Controls.Add(this.labAccount);
            this.Controls.Add(this.dgv);
            this.Name = "fMain";
            this.Text = "SMSInterfaceDemo";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Label labAccount;
        private System.Windows.Forms.TextBox txtAccount;
        private System.Windows.Forms.Label labPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label labDestinations;
        private System.Windows.Forms.TextBox txtDestinations;
        private System.Windows.Forms.Label labContent;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label labWapPushUrl;
        private System.Windows.Forms.TextBox txtWapPushUrl;
        private System.Windows.Forms.Label labSendTime;
        private System.Windows.Forms.TextBox txtSendTime;
        private System.Windows.Forms.Label labSendTimeRemark;
        private System.Windows.Forms.Button btnWS;
        private System.Windows.Forms.Button btnHS;
        private System.Windows.Forms.Button btnWGS;
        private System.Windows.Forms.Button btnHGS;
        private System.Windows.Forms.Button btnWGR;
        private System.Windows.Forms.Button btnHGR;
        private System.Windows.Forms.Button btnWGB;
        private System.Windows.Forms.Button btnHGB;
        private System.Windows.Forms.Label labSubCode;
        private System.Windows.Forms.TextBox txtSubCode;
        private System.Windows.Forms.Label labSubCodeRemark;
        private System.Windows.Forms.Button btnWSEX;
        private System.Windows.Forms.Button btnHSEX;
    }
}

