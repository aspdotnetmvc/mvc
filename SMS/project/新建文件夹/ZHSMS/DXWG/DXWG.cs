using BXM.HttpServer;
using BXM.Utils;
using GatewayInterface;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Xml;

namespace DXWG
{
    [Serializable]
    public class ExSendEventArgs
    {
        SendEventArgs _sendEventArgs;
        DateTime _time;
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
    public class DXWG : ISMSGateway, IDisposable
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
        System.Timers.Timer tmrReport, tmrMO;
        Dictionary<string, ExSendEventArgs> sends;
        bool reportProcess = false;

        public DXWG()
        {
            XmlFileSerialize xfs = new XmlFileSerialize("DXWG.Config");
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
            tmrReport = new System.Timers.Timer(10000);
            tmrReport.Elapsed += tmrReport_Elapsed;
            tmrMO = new System.Timers.Timer(10000);
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
                LogClient.LogHelper.LogInfo("DXWG", "Save ReportCache Error ->", ex.Message);
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
            try
            {
                if (sends.Count > 0)
                {
                    string msgid = "";
                    bool ok = true;
                    DateTime datetime = DateTime.Now;
                    string post =  string.Format("action=query&userid={0}&account={1}&password={2}", userId, account, password);
                    string t = HTTPRequest.PostWebRequest(statusUrl,post, System.Text.Encoding.UTF8);

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
                                statetext = "发送失败。";
                            }

                            ReportEventArgs re = new ReportEventArgs(msgid, ok, statecode, statetext, datetime);
                            ReportEvent(this, re);
                            string tr = JsonSerialize.Instance.Serialize<ReportEventArgs>(re);
                            Console.WriteLine("返回状态报告：" + tr);
                            LogClient.LogHelper.LogInfo("DXWG", "GetReport Process ->", tr);
                            sends.Remove(re.Serial);
                        }
                    }
                }



                List<string> rkeys = new List<string>();
                foreach (string key in sends.Keys)
                {
                    if ((DateTime.Now - sends[key].Time).TotalDays >= 2)
                    {
                        ReportEventArgs re = new ReportEventArgs(sends[key].SendEventArgs.Serial, true, 2100, "状态报告超时,默认成功.", DateTime.Now);
                        ReportEvent(this, re);
                        string ttr = JsonSerialize.Instance.Serialize<ReportEventArgs>(re);
                        Console.WriteLine("超时返回状态报告：" + JsonSerialize.Instance.Serialize<ReportEventArgs>(re));
                        LogClient.LogHelper.LogInfo("DXWG", "GetReport Timeout ->", ttr);
                        rkeys.Add(key);
                    }
                }
                foreach (string key in rkeys)
                {
                    sends.Remove(key);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("DXWG", "GetReport Error ->", ex.Message);
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
                string msg ="";
                string mobile = "";
                DateTime datetime = DateTime.Now;
                string post = string.Format("action=query&userid={0}&account={1}&password={2}", userId, account, password);
                string t = HTTPRequest.PostWebRequest(moUrl,post, System.Text.Encoding.UTF8);

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
                        LogClient.LogHelper.LogInfo("DXWG", "MO ->", tr);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("DXWG", "MO Error ->", ex.Message);
            }
        }
        public bool Connect()
        {
            try
            {
                string post = string.Format("action=overage&userid={0}&account={1}&password={2}", userId, account, password);
                string t = HTTPRequest.PostWebRequest(balanceUrl,post, System.Text.Encoding.UTF8);
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
                            ok = (node.InnerText == "Success") ? true : false;
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
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("DXWG", "Connect Error ->", ex.Message);
            }
            return false;
        }

        public void SendSMS(SMSModel.SMS sms)
        {
            try
            {
                string post = "";
                for (int i = 0; i < sms.Number.Count; i++)
                {
                    int phone, mobile;
                    if(sms.Number[i].Length>=11 && sms.Number[i].Substring(0,1) == "1")
                    {
                        mobile = 1;
                        phone = 0;
                    }
                    else
                    {
                        mobile = 0;
                        phone = 1;
                    }
                    post = "action=send&userid="+ userId+"&account="+ account+"&password="+ password+"&mobile="+ sms.Number[i]+"&content="+ sms.Content + sms.Signature+"&sendTime=&taskName=&checkcontent=1&countnumber=1&mobilenumber="+ mobile+"&telephonenumber="+ phone;
                    string t = HTTPRequest.PostWebRequest(sendUrl,post, System.Text.Encoding.UTF8);

                    //<? xml version = "1.0" encoding = "utf-8" ?>
                    //< returnsms >
                    //< returnstatus > status </ returnstatus > ----------返回状态值：成功返回Success 失败返回：Faild
                    //<message> message</ message > ----------返回信息：见下表
                    //<remainpoint> remainpoint</ remainpoint > ----------返回余额
                    //< taskID > taskID </ taskID > -----------返回本次任务的序列ID
                    //< successCounts > successCounts </ successCounts > --成功短信数：当成功后返回提交成功短信数
                    //</ returnsms >

                    bool ok = true;
                    string rmsg = "";
                    string msgid="";
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(t);
                    XmlElement root = null;
                    root = doc.DocumentElement;
                    XmlNodeList listNodes = root.ChildNodes;
                    foreach (XmlNode node in listNodes)
                    {
                        switch(node.Name)
                        {
                            case "returnstatus":
                                ok = (node.InnerText == "Success") ?true:false;
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
                        r = 99;//提交失败 错误码应该在100以内吧   by lmw
                    }
                    SendEventArgs se = new SendEventArgs(sms, msgid, ok, (ushort)(2000 + r), rmsg, 1, 1);
                    string result = JsonSerialize.Instance.Serialize<SendEventArgs>(se);
                    Console.WriteLine("短信发送结果：" + result);
                    ExSendEventArgs ese = new ExSendEventArgs(se);
                    if (ok)
                    {
                        sends.Add(se.Serial,ese);
                        LogClient.LogHelper.LogInfo("DXWG", "SendSMS OK ->", result);
                    }
                    else
                    {
                        LogClient.LogHelper.LogInfo("DXWG", "SendSMS Fail ->", result);
                    }
                    if (SendEvent != null)
                    {
                        SendEvent(this, se);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("DXWG", "SendSMS Error ->", ex.Message);
            }
            Thread.Sleep(5);
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
                    LogClient.LogHelper.LogInfo("DXWG", "Save ReportCache Error ->", ex.Message);
                }
            }
        }

        public void Close()
        {
            this.Dispose();
        }
    }
}
