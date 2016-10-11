using Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ZHSMSServiceHost
{
    public partial class fMain : Form
    {
        //是否退出
        bool exit = false;
        DateTime startTime;
        SMSServer acServer;
        Thread acThread;
        public fMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化界面显示
        /// </summary>
        private void UIInit()
        {
            startTime = DateTime.Now;
            tsstStartTime.Text = startTime.ToString();
            HostOutput.Instance.Output += Instance_Output;
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            UIInit();
            InitAcService();
        }

        void Instance_Output(string message)
        {
            this.BeginInvoke(new Action(() =>
            {
                if (txtOutput.Lines.Length > 200)
                {
                    //删除第一行
                    txtOutput.Text = txtOutput.Text.Remove(0, txtOutput.Text.IndexOf('\r') + 1);
                }
                txtOutput.AppendText(message + "\r");
            }));
        }

        private void tsbExit_Click(object sender, EventArgs e)
        {
            exit = true;
            this.Close();
        }

        private void InitAcService()
        {
            int port = 18880;
            try
            {
                port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ServicePort"].ToString());
            }
            catch
            { }
            acServer = new SMSServer();
            ThreadStart acServerStarter = delegate { acServer.Start(port); };
            acThread = new Thread(acServerStarter);
            acThread.Start();
            Output("starting AccountingCenter server on port " + port + " ...");
        }

        private void Output(string message)
        {
            HostOutput.Instance.OutputText(message + "\n");
            Log4Logger.Info(message);
        }

        private void fMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            acServer.Stop();
            acThread.Abort();
            System.Environment.Exit(0);
        }

        private void fMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!exit)
            {
                e.Cancel = true;
                return;
            }
        }
    }
}
