using System;
using System.Windows.Forms;
using MongoDB;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

namespace MongDBBigDataTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            stop = false;
            Task t = new Task(delegate() { DoAdd(); });
            t.Start();
        }
        public Random rand = new Random();
        private bool stop = false;
        private void DoAdd()
        {
            var datetime = dateTimePicker1.Value;

            var maxdatetime = datetime.AddDays(int.Parse(txtnum.Text));
            int i = 0;
            while (datetime < maxdatetime && !stop)
            {
                TestModel data = new TestModel()
                {
                    field1 = "field" + rand.Next(1000),
                    field2 = "field" + rand.Next(1000),
                    field3 = "field" + rand.Next(1000),
                    field4 = "field" + rand.Next(1000),
                    field5 = "field" + rand.Next(1000),
                    field6 = "field" + rand.Next(1000),
                    field7 = "field" + rand.Next(1000),
                    field8 = "field" + rand.Next(1000),
                    field9 = "field" + rand.Next(1000),
                    field10 = "field" + rand.Next(1000),
                    field11 = "field" + rand.Next(1000),
                    JoinTime = datetime
                };
                datetime = datetime.AddSeconds(1);
                i++;
                SaveData(data);
                Action actionDelegate = () =>
                {
                    txtInfo.AppendText("正在插入 第 " + i + " 条。。。\r\n");
                };
                txtInfo.Invoke(actionDelegate);
            }
        }
        public static void SaveData(TestModel data)
        {
            var c = MongoHelper.GetCollection<TestModel>("TestBigData");
            c.InsertOne(data);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            stop = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch watch;

            watch = new Stopwatch();
            watch.Start();
            var c = MongoHelper.GetCollection<TestModel>("TestBigData");
            var list = c.Find(s => s.field1 == "field" + txtnum.Text && s.field10.StartsWith("field")&&s.field2.StartsWith("field1")).Limit(100).ToList();
            watch.Stop();
            var t = watch.Elapsed.TotalSeconds;

            Action actionDelegate = () =>
            {
                txtInfo.AppendText("查询记录" + list.Count + "条，用时" + t + "s\r\n");
            };
            txtInfo.Invoke(actionDelegate);
        }

        private void button3_Click(object sender, System.EventArgs e)
        {

            System.Diagnostics.Stopwatch watch;

            watch = new Stopwatch();
            watch.Start();
            var c = MongoHelper.GetCollection<TestModel>("TestBigData");
            var count = c.Count(new BsonDocument());
            watch.Stop();
            var t = watch.Elapsed.TotalSeconds;

            Action actionDelegate = () =>
            {
                txtInfo.AppendText("查询总记录数" + count + "条，用时" + t + "s\r\n");
            };
            txtInfo.Invoke(actionDelegate);
        }
    }
}
