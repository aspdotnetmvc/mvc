using BXM.Utils;
using GatewayInterface;
using SMS.Model;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Xml;

namespace DWWG
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
    public class DWWG : ISMSGateway, IDisposable
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

        CacheManager<List<PlainSMS>> _cm;
        static object locker = new object();

        public DWWG()
        {
            XmlFileSerialize xfs = new XmlFileSerialize("DWWG.Config");
            Config _config = xfs.DeSerialize<Config>();

            account = _config.Account;
            password = _config.Password;
            serverUrl = _config.ServerUrl;
            scrId = _config.SrcID;
            userId = _config.UserId;
            balanceUrl = serverUrl + "/sms.aspx";
            sendUrl = serverUrl + "/sms.aspx";
            statusUrl = serverUrl + "/statusApi.aspx";
            moUrl = serverUrl + "/callApi.aspx";
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
                    sends.Add(c.SendEventArgs.SerialNumber, c);
                }
            }
            catch (Exception ex)
            {
                MessageTools.MessageHelper.Instance.WirteError("加载已发送队列发送错误", ex);
                LogClient.LogHelper.LogInfo("DWWG", "Save ReportCache Error ->", ex.Message);
            }


        }

       

        void tmrReport_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (reportProcess)
            {
                return;
            }
            reportProcess = true;

            //<? xml version = "1.0" encoding = "utf-8" ?>
            //< returnsms >
            //< status >
            //< mid > 121212121212 </ mid > --------------系统里面对应的唯一的mid
            //< mobile > 15023239810 </ mobile > -------------对应的手机号码
            //< status > 10 </ status > -------------状态报告----10表示发送成功，20表示发送失败
            //     < taskid > 1212 </ taskid > --------------某次提交任务的ID
            //     < createtime > 2011 - 12 - 02 22:12:11 </ createtime > -------------发送时间
            //           </ status >
            //           < status >
            //           < mid > 121212121212 </ mid > --------------系统里面对应的唯一的mid
            //           < mobile > 15023239810 </ mobile > -------------对应的手机号码
            //           < status > 10 </ status > -------------状态报告----10表示发送成功，20表示发送失败
            //                 < taskid > 1212 </ taskid > --------------某次提交任务的ID
            //                 < createtime > 2011 - 12 - 02 22:12:11 </ createtime > -------------发送时间
            //                       </ status >
            //                       </ returnsms >
   
            Console.WriteLine("查询状态报告触发, sends.count = " + sends.Count);
            try
            {
                if (sends.Count > 0)
                {
                    string msgid = "";
                    string mobile = "";
                    bool ok = true;
                    DateTime datetime = DateTime.Now;
                    string post = string.Format("action=query&userid={0}&account={1}&password={2}", userId, account, password);
                    //  Console.WriteLine("请求状态报告内容：" + post);
                    Console.WriteLine("请求状态报告地址：" + statusUrl);
                    string t = HTTPRequest.PostWebRequest(statusUrl, post, System.Text.Encoding.UTF8);
                    //  Console.WriteLine("原始状态报告：" + t);
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
                                case "createtime":

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
                                var se = sends.FirstOrDefault(s => s.Value.SendEventArgs.SerialNumber.StartsWith(msgid) && s.Value.SendEventArgs.Message.Numbers == mobile);
                                if (!se.Equals(default(KeyValuePair<string, ExSendEventArgs>)))
                                {
                                    ReportEventArgs re = new ReportEventArgs(se.Value.SendEventArgs.SerialNumber, ok, statecode, statetext, datetime);
                                    ReportEvent(this, re);
                                    string tr = JsonSerialize.Instance.Serialize<ReportEventArgs>(re);
                                    MessageTools.MessageHelper.Instance.WirteInfo("返回状态报告：" + tr);
                                    sends.Remove(re.Serial);
                                }
                                else
                                {
                                    MessageTools.MessageHelper.Instance.WirteInfo("无法匹配状态报告：msgid:" + msgid + " mobile:" + mobile);
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
                            ReportEventArgs re = new ReportEventArgs(sends[key].SendEventArgs.SerialNumber, true, 2100, "状态报告超时,默认成功.", DateTime.Now);
                            ReportEvent(this, re);
                            string ttr = JsonSerialize.Instance.Serialize<ReportEventArgs>(re);
                            Console.WriteLine("超时返回状态报告：" + JsonSerialize.Instance.Serialize<ReportEventArgs>(re));
                            LogClient.LogHelper.LogInfo("DWWG", "GetReport Timeout ->", ttr);
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
                // Console.WriteLine(ex.Message);
                MessageTools.MessageHelper.Instance.WirteError("获取状态报告发生错误：", ex);
                LogClient.LogHelper.LogInfo("DWWG", "GetReport Error ->", ex.Message);
            }
            reportProcess = false;
        }


        void tmrMO_Elapsed(object sender, ElapsedEventArgs e)
        {
            //         <? xml version = "1.0" encoding = "utf-8" ?>
            //< returnsms >
            //< call >
            //< mid > 121212121212 </ mid > --------------系统里面对应的唯一的mid
            //< mobile > 15023239810 </ mobile > -------------手机号码
            //< content > 好 </ content > -------------上行内容
            //< taskid > 1212 </ taskid > --------------某次提交任务的ID
            //< updatetime > 2011 - 12 - 02 22:12:11 </ updatetime > -------------更新时间
            //       </ call >
            //       < call >
            //       < mid > 121212121212 </ mid > --------------系统里面对应的唯一的mid
            //       < mobile > 15023239810 </ mobile > -------------手机号码
            //       < content > 好 </ content > -------------上行内容
            //       < taskid > 1212 </ taskid > --------------某次提交任务的ID
            //       < updatetime > 2011 - 12 - 02 22:12:11 </ updatetime > -------------更新时间
            //              </ call >
            //              </ returnsms >

            try
            {
                string msgid = "";
                string msg = "";
                string mobile = "";
                DateTime datetime = DateTime.Now;
                string post = string.Format("action=query&userid={0}&account={1}&password={2}", userId, account, password);
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

                        DeliverEventArgs re = new DeliverEventArgs(Guid.NewGuid().ToString(), datetime, msg, mobile, SrcID, "");
                        DeliverEvent(this, re);
                        string tr = JsonSerialize.Instance.Serialize<DeliverEventArgs>(re);
                        LogClient.LogHelper.LogInfo("DWWG", "MO ->", tr);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("DWWG", "MO Error ->", ex.Message);
            }
        }
        public bool Connect()
        {
            try
            {
                string post = string.Format("action=overage&userid={0}&account={1}&password={2}", userId, account, password);
                MessageTools.MessageHelper.Instance.WirteTest(balanceUrl);
                string t = HTTPRequest.PostWebRequest(balanceUrl, post, Encoding.UTF8);
                MessageTools.MessageHelper.Instance.WirteTest(t);
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
                            ok = (node.InnerText == "Sucess") ? true : false;  //虽然文档没有指明，但是这里确实是 "Sucess" 而不是Success 
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
                    MessageTools.MessageHelper.Instance.WirteInfo("状态报告和上行短信服务已启动");
                    //SMS sms = new SMS();
                    //sms.Content = "【鑫源金店】情人节大让利：高品质~钻石珠宝、翡翠黄金,数千新品，免费礼+送玫瑰，惊喜翻倍！黄金铂金以旧换新工费0元！2月14-15日退订回T";
                    //sms.Number = new List<string> { "15854881986" };
                    //sms.Signature = "";
                    //List<List<SMS>> caches = new List<List<SMS>>();
                    //List<SMS> list = new List<SMS>();
                    //list.Add(sms);
                    //caches.Add(list);
                    //_cm_ExpireCache(caches);

                    return true;
                }
                else
                {

                    MessageTools.MessageHelper.Instance.WirteInfo("连接失败，状态报告和上行短信服务未启动");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("DWWG", "Connect Error ->", ex.Message);
            }
            return false;
        }
        public void SendSMS(PlainSMS sms)
        {
            try
            {
                string post = "";


                string send = string.Join(",", sms.Number);

                int mobile = 1, phone = 0;
                post = "action=send&userid=" + userId + "&account=" + account + "&password=" + password + "&mobile=" + send + "&content=" + sms.Signature + sms.Content + "&sendTime=&taskName=&checkcontent=1&countnumber=1&mobilenumber=" + mobile + "&telephonenumber=" + phone;
                // Console.WriteLine("发送字符串 " + post);
                string t = HTTPRequest.PostWebRequest(sendUrl, post, Encoding.UTF8);
                //<? xml version = "1.0" encoding = "utf-8" ?>
                //< returnsms >
                //< returnstatus > status </ returnstatus > ----------返回状态值：成功返回Success 失败返回：Faild
                //<message> message</ message > ----------返回信息：见下表
                //<remainpoint> remainpoint</ remainpoint > ----------返回余额
                //< taskID > taskID </ taskID > -----------返回本次任务的序列ID
                //< successCounts > successCounts </ successCounts > --成功短信数：当成功后返回提交成功短信数
                //</ returnsms >
                Console.WriteLine("发送回复字符串 " + t);
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
                            if (node.InnerText == "ok")
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
                int r = 1;//原来默认0 ,应为1  by lmw  1 代表已发送
                if (!ok)
                {
                    // r = 101;
                    r = 99;//提交失败 错误码应该在100以内   by lmw
                }
                int i = 0;
                foreach (string number in sms.Number)
                {
                    SMS s = new SMS();

                    s.Account = sms.Account;
                    s.Audit = sms.Audit;
                    s.Channel = sms.Channel;
                    s.Content = sms.Content;
                    s.Filter = sms.Filter;
                    s.Level = sms.Level;
                    s.Number = new List<string> { number };
                    s.SendTime = sms.SendTime;
                    s.SerialNumber = sms.SerialNumber;
                    s.StatusReport = sms.StatusReport;
                    s.Signature = sms.Signature;
                    s.SPNumber = sms.SPNumber;
                    s.WapURL = sms.WapURL;
                    i++;

                    SendEventArgs se = new SendEventArgs(s, msgid + i.ToString().PadLeft(5, '0'), ok, (ushort)(2000 + r), rmsg, 1, 1); 
                    
                    if (SendEvent != null)
                    {
                        SendEvent(this, se);
                    }

                    //string result = JsonSerialize.Instance.Serialize<SendEventArgs>(se);
                    //Console.WriteLine("短信发送结果：" + result);
                    ExSendEventArgs ese = new ExSendEventArgs(se);
                     
                    if (ok)
                    {
                        lock (locker)
                        {
                            sends.Add(se.SerialNumber, ese);
                        }
                    }
                }
                 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("DWWG", "SendSMS Error ->", ex.Message);
            }





            //if (_cm.ContainsKey(sms.Content))
            //{
            //    var list = _cm.Get(sms.Content);
            //    list.Add(sms);
            //    if (list.Count > 100)
            //    {
            //        List<List<SMS>> caches = new List<List<SMS>>();
            //        caches.Add(list);
            //        _cm_ExpireCache(caches);
            //        _cm.Del(sms.Content);
            //    }
            //}
            //else
            //{
            //    _cm.Add(sms.Content, new List<SMS>() { sms });
            //}
        }

        public void SendSubmit(PlainSMS sms)
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
                    LogClient.LogHelper.LogInfo("DWWG", "Save ReportCache Error ->", ex.Message);
                }
            }
        }

        public void Close()
        {
            this.Dispose();
        }
    }
}
