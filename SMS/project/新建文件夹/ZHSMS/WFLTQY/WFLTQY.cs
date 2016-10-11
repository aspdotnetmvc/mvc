using BXM.HttpServer;
using BXM.Utils;
using GatewayInterface;
using MessageTools;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Web;
using System.Xml;

namespace WFLTQY
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
    public class WFLTQY : ISMSGateway, IDisposable
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
        Queue<ExSendEventArgs> sends;
      //  Dictionary<string, ExSendEventArgs> sends;
        bool reportProcess = false;

        public WFLTQY()
        {
            XmlFileSerialize xfs = new XmlFileSerialize("WFLTQY.Config");
            Config _config = xfs.DeSerialize<Config>();

            account = _config.Account;
            password = _config.Password;
            serverUrl = _config.ServerUrl;
            scrId = _config.SrcID;
            balanceUrl = serverUrl + "/http/GetBalance";
            sendUrl = serverUrl + "/http/SendSms";
            statusUrl = serverUrl + "/http/GetReport";
            moUrl = serverUrl + "/http/GetSms";
            tmrReport = new System.Timers.Timer(35000);
            tmrReport.Elapsed += tmrReport_Elapsed;
            tmrMO = new System.Timers.Timer(35000);
            tmrMO.Elapsed += tmrMO_Elapsed;

           // sends = new Dictionary<string, ExSendEventArgs>();
            sends = new Queue<ExSendEventArgs>();
            try
            {
                BinarySerialize<List<ExSendEventArgs>> bs = new BinarySerialize<List<ExSendEventArgs>>();
                List<ExSendEventArgs> cache = bs.DeSerialize("ReportCache");
                foreach (ExSendEventArgs c in cache)
                {
                   // sends.Add(c.SendEventArgs.Serial, c);
                    sends.Enqueue(c);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("WFLTQY", "Save ReportCache Error ->", ex.Message);
            }
        }

        void tmrReport_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (reportProcess)
            {
                return;
            }
            reportProcess = true;
            try
            {
                if (sends.Count > 0)
                {
                    var ese = sends.Dequeue();

                    if ((DateTime.Now - ese.Time).TotalDays >= 2)
                    {
                        ReportEventArgs re = new ReportEventArgs(ese.SendEventArgs.SerialNumber, true, 2100, "状态报告超时,默认成功.", DateTime.Now);
                        ReportEvent(this, re);
                        string ttr = JsonSerialize.Instance.Serialize<ReportEventArgs>(re);
                        Console.WriteLine("超时返回状态报告：" + JsonSerialize.Instance.Serialize<ReportEventArgs>(re));
                        LogClient.LogHelper.LogInfo("WFLTQY", "GetReport Timeout ->", ttr);
                        reportProcess = false;
                        tmrReport_Elapsed(null, null);
                        return;
                    }



                    string msgid = "";
                    bool ok = true;
                    DateTime datetime = DateTime.Now;
                    string md5p = MD5Helper.GetMD5(password);
                    string post =  string.Format("Account={0}&Password={1}&SmsID={2}",  account, md5p,ese.SendEventArgs.SerialNumber);
                    string t = HTTPRequest.PostWebRequest(statusUrl,post, System.Text.Encoding.UTF8);
                    MessageHelper.Instance.WirteTest("提交返回原始内容: " + t);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(t);
                    XmlElement root = null;
                    root = doc.DocumentElement;
                    XmlNodeList listNodes = root.ChildNodes;
                    foreach (XmlNode node in listNodes)
                    {
                        if (node.Name == "response")
                        {
                            int c = int.Parse(node.InnerText);
                            if (c == 0)
                            {
                                sends.Enqueue(ese);
                                reportProcess = false;
                                return;
                            }
                            if (c < 0)
                            {
                                sends.Enqueue(ese);
                                Console.WriteLine("获取状态报告：" +  GetSendErrorMsg(c));
                                reportProcess = false;
                                return;
                            }
                        }
                        else
                        {
                            foreach (XmlNode cNode in node.ChildNodes)
                            {
                                switch (cNode.Name)
                                {
                                    case "stat":
                                        if (cNode.InnerText == "0")
                                        {
                                            ok = true;
                                        }
                                        else
                                        {
                                            ok = false;
                                        }
                                        break;
                                    case "smsID":
                                        msgid = cNode.InnerText;
                                        break;
                                    case "phone":
                                         
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
                                LogClient.LogHelper.LogInfo("WFLTQY", "GetReport Process ->", tr);
                            }
                        }


                    }
                }

                //List<string> rkeys = new List<string>();
                //foreach (string key in sends.Keys)
                //{
                //    if ((DateTime.Now - sends[key].Time).TotalDays >= 2)
                //    {
                //        ReportEventArgs re = new ReportEventArgs(sends[key].SendEventArgs.Serial, true, 2100, "状态报告超时,默认成功.", DateTime.Now);
                //        ReportEvent(this, re);
                //        string ttr = JsonSerialize.Instance.Serialize<ReportEventArgs>(re);
                //        Console.WriteLine("超时返回状态报告：" + JsonSerialize.Instance.Serialize<ReportEventArgs>(re));
                //        LogClient.LogHelper.LogInfo("WFLTQY", "GetReport Timeout ->", ttr);
                //        rkeys.Add(key);
                //    }
                //}
                //foreach (string key in rkeys)
                //{
                //    sends.Remove(key);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("WFLTQY", "GetReport Error ->", ex.Message);
            }
            reportProcess = false;
        }


        void tmrMO_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                string msg ="";
                string mobile = "";
                DateTime datetime = DateTime.Now;
                string md5p = MD5Helper.GetMD5(password);
                string post = string.Format("Account={0}&Password={1}", account, md5p);
                string t = HTTPRequest.PostWebRequest(moUrl,post, System.Text.Encoding.UTF8);
                Console.WriteLine("原始MO查询消息: " + t);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(t);
                XmlElement root = null;
                root = doc.DocumentElement;
                XmlNodeList listNodes = root.ChildNodes;
                foreach (XmlNode node in listNodes)
                {
                    if (node.Name == "response")
                    {
                        int c = int.Parse(node.InnerText);
                        if (c == 0) return;
                        if (c < 0)
                        {
                            Console.WriteLine(GetErrorMsg(c));
                            return;
                        }
                    }
                    else
                    {
                        foreach (XmlNode cNode in node.ChildNodes)
                        {
                            switch (cNode.Name)
                            {
                                case "phone":
                                    mobile = cNode.InnerText;
                                    break;
                                case "content":
                                    msg =HttpUtility.UrlDecode(cNode.InnerText);
                                    break;
                                case "sendTime":

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
                            LogClient.LogHelper.LogInfo("WFLTQY", "MO ->", tr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("WFLTQY", "MO Error ->", ex.Message);
            }
        }
        public bool Connect()
        {
            try
            {
                string md5p = MD5Helper.GetMD5(password);
                string post = string.Format("Account={0}&Password={1}", account, md5p);
                string t = HTTPRequest.PostWebRequest(balanceUrl,post, System.Text.Encoding.UTF8);
                bool ok = false;
                string overage = "", msg = "";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(t);
                XmlElement root = null;
                root = doc.DocumentElement;
                int c = int.Parse(root["response"].InnerText);
                if (c < 0)
                {
                    msg = GetErrorMsg(c);
                }
                else
                {
                    XmlNodeList listNodes = root["response"].ChildNodes;
                    foreach (XmlNode node in listNodes)
                    {
                        switch (node.Name)
                        {
                            case "sms":
                                overage = node.InnerText;
                                ok = true;
                                break;
                        }
                    }
                }
                Console.WriteLine("连接消息 : " + msg);
                Console.WriteLine("余额 : " + overage);
                if (ok)
                {
                    tmrMO.Start();
                    tmrReport.Start();

                    //SMS sms = new SMS();
                    //sms.Content = "测试短信1";
                    //sms.Number = new List<string> { "15662528858" };
                    //sms.Signature = "";
                    //SendSMS(sms);

                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("WFLTQY", "Connect Error ->", ex.Message);
            }
            return false;
        }

        public void SendSMS(PlainSMS sms)
        {
            try
            {
                string post = "";
                for (int i = 0; i < sms.Number.Count; i++)
                {
                    string md5p = MD5Helper.GetMD5(password);
                    post = "Account="+ account+"&Password="+ md5p+"&SubCode=&Phone="+ sms.Number[i]+"&Content="+ sms.Content + sms.Signature+"&SendTime=";
                    string t = HTTPRequest.PostWebRequest(sendUrl,post, System.Text.Encoding.UTF8);
                    bool ok = false;
                    string rmsg = "";
                    string msgid="";
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(t);
                    XmlElement root = null;
                    root = doc.DocumentElement;
                    XmlNodeList listNodes = root.ChildNodes;
                    foreach (XmlNode node in listNodes)
                    {
                        if (node.Name == "response")
                        {
                            int c = int.Parse(node.InnerText);
                            if (c <= 0)
                            {
                                msgid = System.Guid.NewGuid().ToString();
                                rmsg = GetSendErrorMsg(c);
                                break;
                            }
                        }
                        else
                        {
                            ok = true;
                            rmsg = "短信提交成功.";
                            foreach (XmlNode n in node.ChildNodes)
                            {
                                switch (n.Name)
                                {
                                    case "smsID":
                                        msgid = n.InnerText;
                                        break;
                                }
                            }
                        }
                    }
                    int r = 0;
                    if(!ok)
                    {
                        r = 101;
                    }
                    SendEventArgs se = new SendEventArgs(sms, msgid, ok, (ushort)(2000 + r), rmsg, 1, 1);
                    string result = JsonSerialize.Instance.Serialize<SendEventArgs>(se);
                    Console.WriteLine("短信发送结果：" + result);
                    ExSendEventArgs ese = new ExSendEventArgs(se);
                    if (ok)
                    {
                        //sends.Add(se.Serial,ese);
                        sends.Enqueue(ese);
                        LogClient.LogHelper.LogInfo("WFLTQY", "SendSMS OK ->", result);
                    }
                    else
                    {
                        LogClient.LogHelper.LogInfo("WFLTQY", "SendSMS Fail ->", result);
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
                LogClient.LogHelper.LogInfo("WFLTQY", "SendSMS Error ->", ex.Message);
            }
        }

        private string GetErrorMsg(int code)
        {
            string msg = "";
            switch (code)
            {
                case -1:
                    msg = "帐号登陆失败";
                    break;
                case -2:
                    msg = "账户余额不足";
                    break;
                case -3:
                    msg = "此帐号被禁用";
                    break;
                case -4:
                    msg = "ip鉴权失败";
                    break;
                case -8:
                    msg = "缺少请求参数";
                    break;
                case -9:
                    msg = "访问速度太快，每次访问间隔不能小于30秒";
                    break;

            }
            return msg;
        }

        private string GetSendErrorMsg(int code)
        {
            string msg = "";
            switch (code)
            {
                case -1:
                    msg = "帐号不存在，请检查用户名或者密码是否正确";
                    break;
                case -2:
                    msg = "账户余额不足";
                    break;
                case -3:
                    msg = "帐号已被禁用";
                    break;
                case -4:
                    msg = "ip鉴权失败";
                    break;
                case -8:
                    msg = "缺少请求参数或参数不正确";
                    break;
                case -9:
                    msg = "内容不合法";
                    break;
                case -10:
                    msg = "账户当日发送短信量已经超过允许的每日最大发送量";
                    break;
            }
            return msg;
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
                tmrReport.Stop();
                tmrMO.Stop();

                BinarySerialize<List<ExSendEventArgs>> bs = new BinarySerialize<List<ExSendEventArgs>>();
                try
                {
                    bs.Serialize(sends.ToList(), "ReportCache");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogClient.LogHelper.LogInfo("WFLTQY", "Save ReportCache Error ->", ex.Message);
                }
            }
        }

        public void Close()
        {
            this.Dispose();
        }
    }
}
