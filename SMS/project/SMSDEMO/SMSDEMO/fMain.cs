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

        private void btnHS_Click(object sender, EventArgs e)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["httpurl"] + "/SMS/SendSMS";

                IDictionary<string, string> p = new Dictionary<string, string>();
                p.Add("User", txtAccount.Text);
                p.Add("Pass", txtPassword.Text);
                p.Add("Mobiles", txtDestinations.Text);
                p.Add("Content", txtContent.Text);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnHGS_Click(object sender, EventArgs e)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["httpurl"] + "/SMS/GetMO";
                IDictionary<string, string> par = new Dictionary<string, string>();
                par.Add("User", txtAccount.Text);
                par.Add("Pass", txtPassword.Text);
                string sr = HTTPRequest.Post(url, par, System.Text.Encoding.UTF8);
                if (sr == "")
                {
                    MessageBox.Show("请求接口失败!");
                    return;
                }

                GetMOResult result = JsonConvert.DeserializeObject<GetMOResult>(sr);
                dgv.DataSource = result.Msgs;
                MessageBox.Show(result.Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnHGR_Click(object sender, EventArgs e)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["httpurl"] + "/SMS/GetReport";
                IDictionary<string, string> par = new Dictionary<string, string>();
                par.Add("User", txtAccount.Text);
                par.Add("Pass", txtPassword.Text);
                string sr = (string)HTTPRequest.Post(url, par, System.Text.Encoding.UTF8);
                if (sr == "")
                {
                    MessageBox.Show("请求接口失败!");
                    return;
                }

                GetReportResult result = JsonConvert.DeserializeObject<GetReportResult>(sr);
                dgv.DataSource = result.Reports;
                MessageBox.Show(result.Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnHGB_Click(object sender, EventArgs e)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["httpurl"] + "/SMS/GetBalance";
                IDictionary<string, string> par = new Dictionary<string, string>();
                par.Add("User", txtAccount.Text);
                par.Add("Pass", txtPassword.Text);
                string sr = (string)HTTPRequest.Post(url, par, System.Text.Encoding.UTF8);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
