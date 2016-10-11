using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogQuery
{
    public partial class fMain : Form
    {
        List<LogInterface.LogMessage> lms = new List<LogInterface.LogMessage>();
        bool ff = false;

        public fMain()
        {
            InitializeComponent();
        }

        private void txtEndDate_Click(object sender, EventArgs e)
        {
            txtEndDate.Text = DateTime.Now.ToString();
        }

        private void txtBeginDate_Click(object sender, EventArgs e)
        {
            if (txtBeginDate.Text == "") txtBeginDate.Text = DateTime.Now.ToString();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            sslabCount.Text = "正在查询...";
            bool f = false;
            foreach (var s in cbxRecorder.Items)
            {
                if (s.ToString() == cbxRecorder.Text)
                {
                    f = true;
                }
            }
            if (!f)
            {
                cbxRecorder.Items.Add(cbxRecorder.Text);
            }

            f = false;

            foreach (var s in  cbxEvent.Items)
            {
                if (s.ToString() == cbxEvent.Text)
                {
                    f = true;
                }
            }
            if (!f)
            {
                cbxEvent.Items.Add(cbxEvent.Text);
            }

            try
            {
                lms = LogProxy.Instance.Query(txtBeginDate.Text, txtEndDate.Text, cbxLevel.Text, cbxRecorder.Text, cbxEvent.Text);
            }
            catch
            {
                MessageBox.Show("查询失败.");
                return;
            }
            view.DataSource = lms;
            
            ff = false;
            if (lms.Count == 0)
                sslabCount.Text = "无查询结果.";
            else
                sslabCount.Text = "查询到 " + lms.Count + " 条记录.";
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            txtBeginDate.Text = DateTime.Now.ToShortDateString() + " 00:00:00";
            txtEndDate.Text = DateTime.Now.ToString();

            cbxLevel.Items.Add("Debug");
            cbxLevel.Items.Add("Info");
            cbxLevel.Items.Add("Warn");
            cbxLevel.Items.Add("Error");
            cbxLevel.Items.Add("Fatal");
        }

        private void tsbFilter_Click(object sender, EventArgs e)
        {
            if (txtFilter.Text != "")
            {
                var t = lms.AsParallel<LogInterface.LogMessage>();
                var fs = from f in t
                         where f.Message.Contains(txtFilter.Text)
                         select f;

                view.DataSource = fs.ToList<LogInterface.LogMessage>();
                ff = true;
            }
            else
            {
                if (ff)
                {
                    view.DataSource = lms;
                    ff = false;
                }
                
            }
        }

        private void view_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            fText f = new fText(view.CurrentCell.Value.ToString());
            f.ShowDialog();
        }

        private void tsbKQuery_Click(object sender, EventArgs e)
        {
            txtEndDate.Text = DateTime.Now.ToString();
            sslabCount.Text = "正在查询...";
            Application.DoEvents();
            btnQuery_Click(tsbKQuery, e);
        }
    }
}
