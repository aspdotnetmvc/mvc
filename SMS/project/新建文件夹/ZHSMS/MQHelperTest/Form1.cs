using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MQHelperTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string host = System.Configuration.ConfigurationManager.AppSettings["MQHost"];
            string vhost = System.Configuration.ConfigurationManager.AppSettings["MQVHost"];
            string username = System.Configuration.ConfigurationManager.AppSettings["MQName"];
            string password = System.Configuration.ConfigurationManager.AppSettings["MQPassword"];
            SendMQHelper = new MQHelper.RabbitMQHelper(host, vhost, username, password);
            RecieveMQHelper = new MQHelper.RabbitMQHelper(host, vhost, username, password);
            RecieveMQHelper.OnSubsribeMessageRecieve += OnSubsribeMessageRecieve;
            SendMQHelper.BindQueue("exchange",10, "test");
        }

        private bool OnSubsribeMessageRecieve(string message)
        {
            if (textBox2.InvokeRequired)
            {
                Action actionDelegate = () =>
                {
                    textBox2.AppendText(message + "\r\n");
                };
                textBox2.Invoke(actionDelegate);
            }

            return true;
        }

        private MQHelper.RabbitMQHelper SendMQHelper;
        private MQHelper.RabbitMQHelper RecieveMQHelper;

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("发布的消息不能为空！");
                return;
            }

            var r = SendMQHelper.PublishMessage(textBox1.Text,int.Parse(textBox1.Text.Split(',')[0]));
            if (r)
            {
                textBox1.Text = "";
            }
            else
            {
                MessageBox.Show("发布消息失败！");
            }
            //Random rand = new Random();

            //Task task = new Task(() =>
            //{
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        Thread t = new Thread(new ThreadStart(() =>
            //        {
            //            while (true)
            //            {
            //                SendMQHelper.PublishMessage(rand.Next(100000000).ToString());
            //                Thread.Sleep(1000);
            //            }
            //        }));
            //        t.IsBackground = true;
            //        t.Start();
            //    }
            //});
            //task.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RecieveMQHelper.SubsribeMessage("test",10);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
