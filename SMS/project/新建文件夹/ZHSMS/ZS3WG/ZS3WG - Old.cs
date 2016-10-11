using BXM.Utils;
using GatewayInterface;
using MessageTools;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Xml;

namespace ZS3WG
{
    [Serializable]
    public class ExSendEventArgs
    {
        SendEventArgs _sendEventArgs;
        DateTime _time;
        public ExSendEventArgs()
        {
        }
        public ExSendEventArgs(SendEventArgs sendEventArgs)
        {
            _sendEventArgs = sendEventArgs;
            _time = DateTime.Now;
        }

        public SendEventArgs SendEventArgs
        {
            get
            {
                return _sendEventArgs;
            }

        }
        public DateTime Time
        {
            get
            {
                return _time;
            }
        }
    }
    public class ZS3WG : ISMSGateway, IDisposable
    {
        public event EventHandler<SMSEventArgs> SMSEvent;
        public event EventHandler<DeliverEventArgs> DeliverEvent;
        public event EventHandler<SendEventArgs> SendEvent;
        public event EventHandler<ReportEventArgs> ReportEvent;

        string userId = "";
        string account = "";
        string password = "";
        string serverUrl = "";
        string balanceUrl = "";
        string sendUrl = "";
        string scrId = "";
        string statusUrl = "";
        string moUrl = "";
        Timer tmrReport, tmrMO;
        Dictionary<string, ExSendEventArgs> sends;
        bool reportProcess = false;
        object locker = new object();
        CacheManager<List<SMS>> _cm;
       


        public ZS3WG()
        {
            XmlFileSerialize xfs = new XmlFileSerialize("ZS3WG.Config");
            Config _config = xfs.DeSerialize<Config>();

            account = _config.Account;
            password = _config.Password;
            serverUrl = _config.ServerUrl;
            scrId = _config.SrcID;
            userId = _config.UserId;
            balanceUrl = serverUrl + "/sms";
            sendUrl = serverUrl + "/sms";
            statusUrl = serverUrl + "/sms";
            moUrl = serverUrl + "/sms";
            tmrReport = new Timer(10000);
            tmrReport.Elapsed += tmrReport_Elapsed;
            tmrMO = new Timer(10000);
            tmrMO.Elapsed += tmrMO_Elapsed;
            sends = new Dictionary<string, ExSendEventArgs>();
            try
            {
                BinarySerialize<List<ExSendEventArgs>> bs = new BinarySerialize<List<ExSendEventArgs>>();
                List<ExSendEventArgs> cache = bs.DeSerialize("ReportCache");
                foreach (ExSendEventArgs c in cache)
                {
                    sends.Add(c.SendEventArgs.Serial, c);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("ZS3WG", "Save ReportCache Error ->", ex.Message);
            }

            _cm = CacheManager<List<SMS>>.Instance;
            _cm.ExpireRemoveCache = true;
            _cm.ExpireMillisecond = 2000;
            _cm.CheckMillisecond = 1000;
            _cm.ExpireCache += _cm_ExpireCache;
        }

        private void _cm_ExpireCache(List<List<SMS>> caches)
        {

        }

        void tmrReport_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (reportProcess)
            {
                return;
            }
            reportProcess = true;
            Console.WriteLine("缓存条数 " + _cm.Count);
            Console.WriteLine("查询状态报告触发, sends.count = " + sends.Count);
            try
            {
                if (sends.Count > 0)
                {
                    string msgid = "";
                    string mobile = "";
                    bool ok = true;
                    DateTime datetime = DateTime.Now;
                    string post = string.Format("action=report&userid={0}&account={1}&password={2}", userId, account, password);
                   
                    string t = HTTPRequest.PostWebRequest(statusUrl, post, System.Text.Encoding.UTF8);
                   
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(t);
                    XmlElement root = null;
                    root = doc.DocumentElement;
                    XmlNodeList listNodes = root.ChildNodes;
                    foreach (XmlNode node in listNodes)
                    {
                        foreach (XmlNode cNode in node.ChildNodes)
                        {
                            switch (cNode.Name)
                            {
                                case "mid":
                                    break;
                                case "mobile":
                                    mobile = cNode.InnerText;
                                    break;
                                case "status":
                                    if (cNode.InnerText == "10")
                                    {
                                        ok = true;
                                    }
                                    else
                                    {
                                        ok = false;
                                    }
                                    break;
                                case "taskid":
                                    msgid = cNode.InnerText;
                                    break;
                                case "receivetime":

                                    if (!DateTime.TryParse(cNode.InnerText, out datetime))
                                    {
                                        datetime = DateTime.Now;
                                    }
                                    break;
                            }
                        }
                        if (ReportEvent != null)
                        {
                            ushort statecode;
                            string statetext = "发送成功。";
                            statecode = 2100;
                            if (!ok)
                            {
                                statecode = 2101;
                                statetext = "发送失败。";
                            }
                            lock (locker)
                            {
                                var se = sends.FirstOrDefault(s => s.Value.SendEventArgs.Serial.StartsWith(msgid) && s.Value.SendEventArgs.Message.Number[0] == mobile);
                                if (!se.Equals(default(KeyValuePair<string, ExSendEventArgs>)))
                                {
                                    ReportEventArgs re = new ReportEventArgs(se.Value.SendEventArgs.Serial, ok, statecode, statetext, datetime);
                                    ReportEvent(this, re);
                                    string tr = JsonSerialize.Instance.Serialize<ReportEventArgs>(re);

                                    //  LogClient.LogHelper.LogInfo("ZSXWG", "GetReport Process ->", tr);
                                    sends.Remove(re.Serial);
                                }
                            }
                        }
                    }
                }

                lock (locker)
                {
                    List<string> rkeys = new List<string>();
                    foreach (string key in sends.Keys)
                    {
                        if ((DateTime.Now - sends[key].Time).TotalDays >= 2)
                        {
                            ReportEventArgs re = new ReportEventArgs(sends[key].SendEventArgs.Serial, true, 2100, "状态报告超时,默认成功.", DateTime.Now);
                            ReportEvent(this, re);
                            string ttr = JsonSerialize.Instance.Serialize<ReportEventArgs>(re);
                            Console.WriteLine("超时返回状态报告：" + JsonSerialize.Instance.Serialize<ReportEventArgs>(re));
                            LogClient.LogHelper.LogInfo("ZS3WG", "GetReport Timeout ->", ttr);
                            rkeys.Add(key);
                        }
                    }
                    foreach (string key in rkeys)
                    {
                        sends.Remove(key);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("ZS3WG", "GetReport Error ->", ex.Message);
            }
            reportProcess = false;
        }


        void tmrMO_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                string msgid = "";
                string msg = "";
                string mobile = "";
                string extno = "";
                DateTime datetime = DateTime.Now;
                string post = string.Format("action=mo&userid={0}&account={1}&password={2}", userId, account, password);
                string t = HTTPRequest.PostWebRequest(moUrl, post, System.Text.Encoding.UTF8);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(t);
                XmlElement root = null;
                root = doc.DocumentElement;
                XmlNodeList listNodes = root.ChildNodes;
                foreach (XmlNode node in listNodes)
                {
                    foreach (XmlNode cNode in node.ChildNodes)
                    {
                        switch (cNode.Name)
                        {
                            case "mid":
                                break;
                            case "mobile":
                                mobile = cNode.InnerText;
                                break;
                            case "extno":
                                extno = cNode.InnerText;
                                break;
                            case "content":
                                msg = cNode.InnerText;
                                break;
                            case "taskid":
                                msgid = cNode.InnerText;
                                break;
                            case "updatetime":

                                if (!DateTime.TryParse(cNode.InnerText, out datetime))
                                {
                                    datetime = DateTime.Now;
                                }
                                break;
                        }
                    }
                    if (DeliverEvent != null)
                    {

                        DeliverEventArgs re = new DeliverEventArgs(Guid.NewGuid().ToString(), datetime, msg, mobile, extno, "");
                        DeliverEvent(this, re);
                        string tr = JsonSerialize.Instance.Serialize<DeliverEventArgs>(re);
                        LogClient.LogHelper.LogInfo("ZS3WG", "MO ->", tr);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("ZS3WG", "MO Error ->", ex.Message);
            }
        }
        public bool Connect()
        {
            try
            {
                string post = string.Format("action=overage&userid={0}&account={1}&password={2}", userId, account, password);
                string t = HTTPRequest.PostWebRequest(balanceUrl, post, Encoding.UTF8);

                bool ok = false;
                string overage = "", msg = "";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(t);
                XmlElement root = null;
                root = doc.DocumentElement;
                XmlNodeList listNodes = root.ChildNodes;
                foreach (XmlNode node in listNodes)
                {
                    switch (node.Name)
                    {
                        case "returnstatus":
                            ok = (node.InnerText == "Sucess") ? true : false;
                            break;
                        case "overage":
                            overage = node.InnerText;
                            break;
                        case "message":
                            msg = node.InnerText;
                            break;
                    }
                }
                Console.WriteLine("连接消息 : " + msg);
                Console.WriteLine("余额 : " + overage);
                if (ok)
                {
                    tmrMO.Start();
                    tmrReport.Start();

                    //SMS sms = new SMS();
                    //sms.Content = "测试短信 123 ";
                    //sms.Number = new List<string> { "15854881986" };
                    //sms.Signature = "";
                    //List<List<SMS>> caches = new List<List<SMS>>();
                    //List<SMS> list = new List<SMS>();
                    //list.Add(sms);
                    //caches.Add(list);
                    //_cm_ExpireCache(caches);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("ZS3WG", "Connect Error ->", ex.Message);
            }
            return false;
        }
        public void SendSMS(SMSModel.SMS sms)
        {

            try
            {
                string post = "";
                post = "action=send&userid=" + userId + "&account=" + account + "&password=" + password + "&mobile=" + string.Join(",", sms.Number) + "&content=" + sms.Content + sms.Signature + "&sendTime=&extno=" + scrId + sms.SPNumber;
                //   Console.WriteLine("发送字符串 " + post);
                string t = HTTPRequest.PostWebRequest(sendUrl, post, Encoding.UTF8);
                //<? xml version = "1.0" encoding = "utf-8" ?>
                //< returnsms >
                //< returnstatus > status </ returnstatus > ----------返回状态值：成功返回Success 失败返回：Faild
                //<message> message</ message > ----------返回信息：见下表
                //<remainpoint> remainpoint</ remainpoint > ----------返回余额
                //< taskID > taskID </ taskID > -----------返回本次任务的序列ID
                //< successCounts > successCounts </ successCounts > --成功短信数：当成功后返回提交成功短信数
                //</ returnsms >
                // Console.WriteLine("发送返回消息 " + t);
                bool ok = true;
                string rmsg = "";
                string msgid = "";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(t);
                XmlElement root = null;
                root = doc.DocumentElement;
                XmlNodeList listNodes = root.ChildNodes;
                foreach (XmlNode node in listNodes)
                {
                    switch (node.Name)
                    {
                        case "returnstatus":
                            ok = (node.InnerText == "Success") ? true : false;
                            break;
                        case "message":
                            if (node.InnerText == "OK")
                            {
                                rmsg = "短信提交成功.";
                            }
                            else
                            {
                                rmsg = node.InnerText;
                            }
                            break;
                        case "remainpoint":
                            break;
                        case "taskID":
                            msgid = node.InnerText;
                            break;
                        case "successCounts":
                            break;
                    }
                }
                int r = 1;
                if (!ok)
                {
                    r = 99;
                }
                int i = 0;
                MessageHelper.Instance.WirteTest("短信提交一次  " + string.Join(",", sms.Number) + "         " + rmsg);
                foreach (string num in sms.Number)
                {
                    SMS s = new SMS();

                    s.Account = sms.Account;
                    s.Audit = sms.Audit;
                    s.Channel = sms.Channel;
                    s.Content = sms.Content;
                    s.Filter = sms.Filter;
                    s.Level = sms.Level;
                    s.Number = new List<string> { num };
                    s.SendTime = sms.SendTime;
                    s.SerialNumber = sms.SerialNumber;
                    s.StatusReport = sms.StatusReport;
                    s.Signature = sms.Signature;
                    s.SPNumber = sms.SPNumber;
                    s.WapURL = sms.WapURL;
                    i++;
                    SendEventArgs se = new SendEventArgs(s, msgid + i.ToString().PadLeft(5, '0'), ok, (ushort)(2000 + r), rmsg, 1, 1);
                    ExSendEventArgs ese = new ExSendEventArgs(se);
                    if (SendEvent != null)
                    {
                        SendEvent(this, se);
                    }
                    if (ok)
                    {
                        lock (locker)
                        {
                            sends.Add(se.Serial, ese);
                        }
                    }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("ZS3WG", "SendSMS Error ->", ex.Message);
            }
        }

        public void SendSubmit(SMSModel.SMS sms)
        {

        }

        public string SrcID
        {
            get { return scrId; }
        }


        bool _dispose = true;
        public void Dispose()
        {
            if (_dispose)
            {
                _dispose = false;
                _cm.TriggerExpireCache();
                tmrReport.Stop();
                tmrMO.Stop();

                BinarySerialize<List<ExSendEventArgs>> bs = new BinarySerialize<List<ExSendEventArgs>>();
                try
                {
                    bs.Serialize(sends.Values.ToList(), "ReportCache");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogClient.LogHelper.LogInfo("ZS3WG", "Save ReportCache Error ->", ex.Message);
                }
            }
        }

        public void Close()
        {
            this.Dispose();
        }
    }
}
