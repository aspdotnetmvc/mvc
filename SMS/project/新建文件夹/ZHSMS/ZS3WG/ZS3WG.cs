using BXM.Utils;
using GatewayInterface;
using MessageTools;
using SMS.Model;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Xml;

namespace ZS3WG
{
    public class ZS3WG : ISMSGateway, IDisposable
    {
        public event EventHandler<SMSEventArgs> SMSEvent;
        public event EventHandler<DeliverEventArgs> DeliverEvent;
        public event EventHandler<ReportEventArgs> SendEvent;
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
        Dictionary<string, ReportEventArgs> sends;
        bool reportProcess = false;
        object locker = new object();
       
        public ZS3WG()
        {
            XmlSerialize xfs = new XmlSerialize("ZS3WG.Config");
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
            sends = new Dictionary<string, ReportEventArgs>();
            try
            {
                BinarySerialize<Dictionary<string, ReportEventArgs>> bs = new BinarySerialize<Dictionary<string, ReportEventArgs>>();
                sends  = bs.DeSerialize("ReportCache");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("ZS3WG", "Save ReportCache Error ->", ex.Message);
            }
        }
 

        void tmrReport_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (reportProcess)
            {
                return;
            }
            reportProcess = true;
         //   Console.WriteLine("缓存条数 " + _cm.Count);
           // Console.WriteLine("查询状态报告触发, sends.count = " + sends.Count);
            try
            {
                if (sends.Count > 0)
                {
                    string msgid = "";
                    string mobile = "";
                    bool ok = true;
                    DateTime datetime = DateTime.Now;
                    string post = string.Format("action=query&userid={0}&account={1}&password={2}", userId, account, password);
                   
                    string t = HTTPRequest.PostWebRequest(statusUrl, post, System.Text.Encoding.UTF8);
                   
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(t);
                    XmlElement root = null;
                    root = doc.DocumentElement;
                    XmlNodeList listNodes = root.ChildNodes;



                    ReportEventArgs re = new ReportEventArgs();
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
                            var se = sends.FirstOrDefault(s => s.Key == msgid);
                            if (!se.Equals(default(KeyValuePair<string, ReportEventArgs>)))
                            {

                                var sr = se.Value.StatusReports.Where(r => r.Number == mobile);
                                re.StatusReports.AddRange(sr);
                                re.StatusReports.ForEach(s => { s.ResponseTime = DateTime.Now; s.StatusCode = statecode; s.Description = statetext; });

                                foreach (var r in sr)
                                {
                                    se.Value.StatusReports.Remove(r);
                                }
                                if (se.Value.StatusReports.Count == 0)
                                {
                                    sends.Remove(se.Key);
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
                        if ((DateTime.Now - sends[key].SubmitTime).TotalDays >= 2)
                        {
                            ReportEventArgs sre = new ReportEventArgs();

                            sre.StatusReports.AddRange(sends[key].StatusReports);
                            sends[key].StatusReports.ForEach(s => { s.StatusCode = 2100; s.Description = "超时返回状态报告"; s.ResponseTime = DateTime.Now; });

                            ReportEvent(this, sre);
                            string ttr = JsonSerialize.Instance.Serialize<ReportEventArgs>(sre);
                            MessageHelper.Instance.WirteInfo("超时返回状态报告：" + ttr);
                            LogClient.LogHelper.LogInfo("ZSXWG", "GetReport Timeout ->", ttr);
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
                            case "extno":
                                //extno = cNode.InnerText;
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
                MessageTools.MessageHelper.Instance.WirteInfo("提交内容："+post);
                MessageTools.MessageHelper.Instance.WirteInfo("返回结果："+t);
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
        public void SendSMS(PlainSMS sms)
        {

            try
            {
                string post = "";
                post = "action=send&userid=" + userId + "&account=" + account + "&password=" + password + "&mobile=" +sms.Numbers + "&content=" + sms.Content + sms.Signature + "&sendTime=&extno=" + sms.SPNumber;
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
                ReportEventArgs re = new ReportEventArgs();
                int i=0;
                re.StatusReports.AddRange(sms.Numbers.Split(',').Select(n => new StatusReport()
                {
                    SMSID = sms.ID,
                    SerialNumber = msgid+(++i).ToString().PadLeft(5,'0'),
                    SendTime = sms.SendTime.Value,
                    StatusCode = 2000 + r,
                    Succeed = ok,
                    Channel = sms.Channel,
                    Description = rmsg,
                    Number = n,
                    ResponseTime = null
                }));
                SendEvent(this, re);
                if (ok)
                {
                    lock (locker)
                    {
                        sends.Add(msgid, re);
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageHelper.Instance.WirteError(ex.ToString());
                LogClient.LogHelper.LogInfo("ZS3WG", "SendSMS Error ->", ex.Message);
            }
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
                tmrReport.Stop();
                tmrMO.Stop();

                BinarySerialize<Dictionary<string, ReportEventArgs>> bs = new BinarySerialize<Dictionary<string, ReportEventArgs>>();
                try
                {
                    bs.Serialize(sends, "ReportCache");
                }
                catch (Exception ex)
                {
                    MessageHelper.Instance.WirteError(ex.ToString());
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
