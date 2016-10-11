using BXM.Utils;
using GatewayInterface;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Xml;
using MessageTools;
using SMS.Model;

namespace OPWO
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
    public class OPWO : ISMSGateway, IDisposable
    {
        public event EventHandler<SMSEventArgs> SMSEvent;
        public event EventHandler<DeliverEventArgs> DeliverEvent;
        public event EventHandler<SendEventArgs> SendEvent;
        public event EventHandler<ReportEventArgs> ReportEvent;
        string AssDir = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        string userId = "";
        string account = "";
        string password = "";
        string serverUrl = "";
        string balanceUrl = "";
        string sendUrl = "";
        string scrId = "";
        string statusUrl = "";
        string moUrl = "";
        string SignaturePos = "0"; //0 前，1 后
        Timer tmrReport, tmrMO;
        Dictionary<string, ExSendEventArgs> sends;
        private static object sendslocker = new object();
        bool reportProcess = false;




        public OPWO()
        {
            Console.WriteLine("DIR:" + AssDir);
            XmlSerialize xfs = new XmlSerialize(System.IO.Path.Combine(AssDir, "OPWO.Config"));
            Config _config = xfs.DeSerialize<Config>();

            account = _config.Account;
            password = _config.Password;
            serverUrl = _config.ServerUrl;
            scrId = _config.SrcID;
            userId = _config.UserId;
            SignaturePos = _config.SignaturePos;
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
                List<ExSendEventArgs> cache = bs.DeSerialize(System.IO.Path.Combine(AssDir, "ReportCache"));
                foreach (ExSendEventArgs c in cache)
                {
                    sends.Add(c.SendEventArgs.SerialNumber, c);
                }
            }
            catch (Exception ex)
            {
                MessageHelper.Instance.WirteError(ex.ToString());
                LogClient.LogHelper.LogInfo("OPWO", "Save ReportCache Error ->", ex.Message);
            }

        }

        void tmrReport_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (reportProcess)
            {
                //    MessageHelper.Instance.WirteInfo("查询状态报告触发, 但上次处理仍在进行，退出，sends.count = " + sends.Count);
                return;
            }
            try
            {
                reportProcess = true;
             //   MessageHelper.Instance.WirteInfo("查询状态报告触发, sends.count = " + sends.Count);

                //            < returnsms >
                //< statusbox >
                //< mobile > 18666620923 </ mobile > -------------对应的手机号码
                //< taskid > 1212 </ taskid > -------------同一批任务ID
                //< status > 10 </ status > ---------状态报告----10：发送成功，20：发送失败
                //       < receivetime > 2011 - 12 - 02 22:12:11 </ receivetime > -------------接收时间
                //              < errorcode > DELIVRD </ errorcode > -上级网关返回值，不同网关返回值不同，仅作为参考
                //                     < extno > 01 </ extno > --子号，即自定义扩展号
                //                            </ statusbox >
                //                            < statusbox >
                //                            < mobile > 18666620923 </ mobile >
                //                            < taskid > 1212 </ taskid >
                //                            < status > 20 </ status >
                //                            < receivetime > 2011 - 12 - 02 22:12:11 </ receivetime >
                //                                   < errorcode > 2 </ errorcode >
                //                                   < extno ></ extno >
                //                                   </ statusbox >
                //                                   </ returnsms >


                if (sends.Count > 0)
                {
                    string msgid = "";
                    string mobile = "";
                    bool ok = true;
                    DateTime datetime = DateTime.Now;
                    string post = string.Format("action=report&userid={0}&account={1}&password={2}", userId, account, password);
                    //  MessageHelper.Instance.WirteInfo("请求状态报告内容：" + post);
                    //  MessageHelper.Instance.WirteInfo("请求状态报告地址：" + statusUrl);
                    string t = HTTPRequest.PostWebRequest(statusUrl, post, System.Text.Encoding.UTF8);
                   // MessageHelper.Instance.WirteInfo("原始状态报告：" + t);
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
                            lock (sendslocker)
                            {
                                var se = sends.FirstOrDefault(s => s.Value.SendEventArgs.SerialNumber.StartsWith(msgid) && s.Value.SendEventArgs.Numbers[0] == mobile);
                                if (!se.Equals(default(KeyValuePair<string, ExSendEventArgs>)))
                                {
                                    ReportEventArgs re = new ReportEventArgs(se.Value.SendEventArgs.SerialNumber, ok, statecode, statetext, datetime);
                                    ReportEvent(this, re);
                                    string tr = JsonSerialize.Instance.Serialize<ReportEventArgs>(re);
                                    MessageHelper.Instance.WirteInfo("返回状态报告：" + tr);
                                    //  LogClient.LogHelper.LogInfo("OPWO", "GetReport Process ->", tr);
                                    sends.Remove(re.SerialNumber);
                                }
                            }
                        }
                    }
                }

                lock (sendslocker)
                {
                    List<string> rkeys = new List<string>();
                    foreach (string key in sends.Keys)
                    {
                        if ((DateTime.Now - sends[key].Time).TotalDays >= 2)
                        {
                            ReportEventArgs re = new ReportEventArgs(sends[key].SendEventArgs.SerialNumber, true, 2100, "状态报告超时,默认成功.", DateTime.Now);
                            ReportEvent(this, re);
                            string ttr = JsonSerialize.Instance.Serialize<ReportEventArgs>(re);
                            MessageHelper.Instance.WirteInfo("超时返回状态报告：" + JsonSerialize.Instance.Serialize<ReportEventArgs>(re));
                            LogClient.LogHelper.LogInfo("OPWO", "GetReport Timeout ->", ttr);
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
                MessageHelper.Instance.WirteError("OPWO,GetReport Error", ex);
                LogClient.LogHelper.LogInfo("OPWO", "GetReport Error ->", ex.Message);
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
                //MessageHelper.Instance.WirteInfo(t);
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

                            case "mobile":
                                mobile = cNode.InnerText;
                                break;
                            case "content":
                                msg = cNode.InnerText;
                                break;
                            case "taskid":
                                msgid = cNode.InnerText;
                                break;
                            case "extno":
                                extno = cNode.InnerText;
                                break;
                            case "receivetime":

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
                        LogClient.LogHelper.LogInfo("OPWO", "MO ->", tr);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageHelper.Instance.WirteError(ex.ToString());
                LogClient.LogHelper.LogInfo("OPWO", "MO Error ->", ex.Message);
            }
        }
        public bool Connect()
        {
            try
            {
                string post = string.Format("action=overage&userid={0}&account={1}&password={2}", userId, account, password);
                string t = HTTPRequest.PostWebRequest(balanceUrl, post, Encoding.UTF8);
                // MessageHelper.Instance.WirteInfo("连接原始返回消息：" + t);
                //             <? xml version = "1.0" encoding = "utf-8" ?>
                //< returnsms >
                //< returnstatus > status </ returnstatus > --------------返回状态值：成功返回Success 失败返回：Faild
                //       <message> message</ message > --------------返回信息提示：见下表
                //           <payinfo> payinfo</ payinfo > --------------返回支付方式  后付费，预付费
                //               <overage> overage</ overage > -------------返回已发送条数
                //                < sendTotal > sendTotal </ sendTotal > -------返回总点数  当支付方式为预付费是返回总充值点数
                //                       </ returnsms >

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
                    MessageHelper.Instance.WirteInfo("状态报告和上行短信查询服务启动！");
                    return true;
                }
                else
                {
                    MessageHelper.Instance.WirteInfo("连接失败");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                LogClient.LogHelper.LogInfo("OPWO", "Connect Error ->", ex.Message);
            }
            return false;
        }
        public void SendSMS(PlainSMS sms)
        {

            try
            {

                //  List<SMS> caches = new List<SMS>();

                string post = "";

            
                string content = "";
                if (SignaturePos == "0")
                {
                    //签名在前
                    content = sms.Signature + sms.Content;
                }
                else
                {
                    //签名在后
                    content = sms.Content + sms.Signature;
                }
                post = "action=send&userid=" + userId + "&account=" + account + "&password=" + password + "&mobile=" + sms.Numbers + "&content=" + content + "&sendTime=&extno=" + SrcID + sms.SPNumber;
                // MessageHelper.Instance.WirteTest("发送字符串: " + post);
                string t = HTTPRequest.PostWebRequest(sendUrl, post, Encoding.UTF8);
                //string t = "";
                MessageHelper.Instance.WirteTest("提交返回原始内容: " + t);
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
                    r = 99;//提交失败 错误码应该在100以内吧   by lmw 
                    if (string.IsNullOrWhiteSpace(msgid) || msgid == "0")
                    {
                        msgid = System.Guid.NewGuid().ToString();
                    }
                }



                int i = 0;
                foreach (string num in sms.Numbers.Split(','))
                {
                    PlainSMS s = PlainSMS.GetPlainSMSWithOneNumber(sms, num);
                    i++;
                    SendEventArgs se = new SendEventArgs(s, msgid + i.ToString().PadLeft(5, '0'), ok, (ushort)(2000 + r), rmsg, 1, 1);
                    if (SendEvent != null)
                    {
                        SendEvent(this, se);
                    }
                    //string result = JsonSerialize.Instance.Serialize<SendEventArgs>(se);
                    //IService.ServiceProxy.Instance.WriteTest("短信发送结果：" + result);
                    ExSendEventArgs ese = new ExSendEventArgs(se);
                    if (ok)
                    {
                        lock (sendslocker)
                        {
                            sends.Add(se.SerialNumber, ese);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageHelper.Instance.WirteError(ex.ToString());
                LogClient.LogHelper.LogInfo("OPWO", "SendSMS Error ->", ex.Message);
            }
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
                // _cm.TriggerExpireCache();
                tmrReport.Stop();
                tmrMO.Stop();

                BinarySerialize<List<ExSendEventArgs>> bs = new BinarySerialize<List<ExSendEventArgs>>();
                try
                {
                    bs.Serialize(sends.Values.ToList(), System.IO.Path.Combine(AssDir, "ReportCache"));
                }
                catch (Exception ex)
                {
                    MessageHelper.Instance.WirteError(ex.ToString());
                    LogClient.LogHelper.LogInfo("OPWO", "Save ReportCache Error ->", ex.Message);
                }
            }
        }

        public void Close()
        {
            this.Dispose();
        }
    }
}
