using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace ZHSMSDEMO
{
    public partial class fMain : Form
    {
        public fMain()
        {
            InitializeComponent();
        }

        private void btnWS_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["wsurl"] + "SMWebService.asmx";
            string[] args = new string[6];
            args[0] = txtAccount.Text;
            args[1] = txtPassword.Text;
            args[2] = txtDestinations.Text;
            args[3] = txtContent.Text;
            args[4] = txtWapPushUrl.Text;
            args[5] = txtSendTime.Text;
            string sr = (string)WSHelper.InvokeWebService(url, "SendSMS", args);
            if (sr == "")
            {
                MessageBox.Show("请求接口失败!");
                return;
            }

            SendSMSResult result = JsonConvert.DeserializeObject<SendSMSResult>(sr);
            MessageBox.Show(result.Result);
        }

        private void btnHS_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["httpurl"] + "SendSMS.aspx";

            IDictionary<string, string> p = new Dictionary<string, string>();
            p.Add("User", txtAccount.Text);
            p.Add("Pass", txtPassword.Text);
            p.Add("Destinations", txtDestinations.Text);
            p.Add("Content", txtContent.Text);
            p.Add("WapPushUrl", txtWapPushUrl.Text);
            p.Add("SendTime", txtSendTime.Text);
            string sr = HTTPRequest.Post(url, p, System.Text.Encoding.UTF8);
            if (sr == "")
            {
                MessageBox.Show("请求接口失败!");
                return;
            }

            SendSMSResult result = JsonConvert.DeserializeObject<SendSMSResult>(sr);
            MessageBox.Show(result.Result);
        }

        private void btnWGS_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["wsurl"] + "SMWebService.asmx";
            string[] args = new string[2];
            args[0] = txtAccount.Text;
            args[1] = txtPassword.Text;

            string sr = (string)WSHelper.InvokeWebService(url, "GetSMS", args);
            if (sr == "")
            {
                MessageBox.Show("请求接口失败!");
                return;
            }

            GetSMSResult result = JsonConvert.DeserializeObject<GetSMSResult>(sr);
            dgv.DataSource = result.Msgs;
            MessageBox.Show(result.Result);
        }

        private void btnWGR_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["wsurl"] + "SMWebService.asmx";
            string[] args = new string[3];
            args[0] = txtAccount.Text;
            args[1] = txtPassword.Text;
            args[2] = "";
            string sr = (string)WSHelper.InvokeWebService(url, "GetReport", args);
            if (sr == "")
            {
                MessageBox.Show("请求接口失败!");
                return;
            }

            GetReportResult result = JsonConvert.DeserializeObject<GetReportResult>(sr);
            dgv.DataSource = result.Reports;
            MessageBox.Show(result.Result);
        }

        private void btnWGB_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["wsurl"] + "SMWebService.asmx";
            string[] args = new string[2];
            args[0] = txtAccount.Text;
            args[1] = txtPassword.Text;

            string sr = (string)WSHelper.InvokeWebService(url, "GetBalance", args);
            if (sr == "")
            {
                MessageBox.Show("请求接口失败!");
                return;
            }

            GetBalanceResult result = JsonConvert.DeserializeObject<GetBalanceResult>(sr);
            List<GetBalanceResult> data = new List<GetBalanceResult>();
            data.Add(result);
            dgv.DataSource = data;
            MessageBox.Show(result.Result);
        }

        private void btnHGS_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["httpurl"] + "GetSMS.aspx";
            string args = "";
            args += "User=" + txtAccount.Text;
            args += "&Pass=" + txtPassword.Text;
            string sr = (string)HTTPRequest.PostWebRequest(url, args, System.Text.Encoding.UTF8);
            if (sr == "")
            {
                MessageBox.Show("请求接口失败!");
                return;
            }

            GetSMSResult result = JsonConvert.DeserializeObject<GetSMSResult>(sr);
            dgv.DataSource = result.Msgs;
            MessageBox.Show(result.Result);
        }

        private void btnHGR_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["httpurl"] + "GetReport.aspx";
            string args = "";
            args += "User=" + txtAccount.Text;
            args += "&Pass=" + txtPassword.Text;
            args += "&MsgID=";
            string sr = (string)HTTPRequest.PostWebRequest(url, args, System.Text.Encoding.UTF8);
            if (sr == "")
            {
                MessageBox.Show("请求接口失败!");
                return;
            }

            GetReportResult result = JsonConvert.DeserializeObject<GetReportResult>(sr);
            dgv.DataSource = result.Reports;
            MessageBox.Show(result.Result);
        }

        private void btnHGB_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["httpurl"] + "GetBalance.aspx";
            string args = "";
            args += "User=" + txtAccount.Text;
            args += "&Pass=" + txtPassword.Text;
            string sr = (string)HTTPRequest.PostWebRequest(url, args, System.Text.Encoding.UTF8);
            if (sr == "")
            {
                MessageBox.Show("请求接口失败!");
                return;
            }

            GetBalanceResult result = JsonConvert.DeserializeObject<GetBalanceResult>(sr);
            List<GetBalanceResult> data = new List<GetBalanceResult>();
            data.Add(result);
            dgv.DataSource = data;
            MessageBox.Show(result.Result);
        }

        private void btnHSEX_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["httpurl"] + "SendSMSEx.aspx";
            string args = "";
            args += "User=" + txtAccount.Text;
            args += "&Pass=" + txtPassword.Text;
            args += "&SubCode=" + txtSubCode.Text;
            args += "&Destinations=" + txtDestinations.Text;
            args += "&Content=" + txtContent.Text;
            args += "&WapPushUrl=" + txtWapPushUrl.Text;
            args += "&SendTime=" + txtSendTime.Text;

            string sr = (string)HTTPRequest.PostWebRequest(url, args, System.Text.Encoding.UTF8);
            if (sr == "")
            {
                MessageBox.Show("请求接口失败!");
                return;
            }

            SendSMSResult result = JsonConvert.DeserializeObject<SendSMSResult>(sr);
            MessageBox.Show(result.Result);
        }

        private void btnWSEX_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["wsurl"] + "SMWebService.asmx";
            string[] args = new string[7];
            args[0] = txtAccount.Text;
            args[1] = txtPassword.Text;
            args[2] = txtSubCode.Text;
            args[3] = txtDestinations.Text;
            args[4] = txtContent.Text;
            args[5] = txtWapPushUrl.Text;
            args[6] = txtSendTime.Text;
            string sr = (string)WSHelper.InvokeWebService(url, "SendSMSEx", args);
            if (sr == "")
            {
                MessageBox.Show("请求接口失败!");
                return;
            }

            SendSMSResult result = JsonConvert.DeserializeObject<SendSMSResult>(sr);
            MessageBox.Show(result.Result);
        }
    }
}
