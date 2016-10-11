using BXM.HttpServer;
using BXM.Utils;
using GatewayInterface;
using SMS.Model;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace JCWG
{

    public class JCWG : ISMSGateway, IDisposable
    {
        public event EventHandler<SMSEventArgs> SMSEvent;
        public event EventHandler<DeliverEventArgs> DeliverEvent;
        public event EventHandler<SendEventArgs> SendEvent;
        public event EventHandler<ReportEventArgs> ReportEvent;
        SMSHttpServer httpServer;

        string account = "";
        string password = "";
        string serverUrl = "";
        string queryBalanceUrl = "";
        string sendUrl = "";
        string scrId = "";
        public JCWG()
        {
            XmlFileSerialize xfs = new XmlFileSerialize("JCWG.Config");
            Config _config = xfs.DeSerialize<Config>();

            string httpport = _config.HttpPort;
            account = _config.Account;
            password = _config.Password;
            serverUrl = _config.ServerUrl;
            scrId = _config.SrcID;

            queryBalanceUrl = serverUrl + "/msg/QueryBalance";
            sendUrl = serverUrl + "/msg/HttpSendSM";
            int port;
            if (!int.TryParse(httpport, out port))
            {
                throw new Exception("Initialization HttpPort error.");
            }
            httpServer = new SMSHttpServer(port);
            httpServer.DeliverEvent += HttpServer_DeliverEvent;
            httpServer.ReportEvent += HttpServer_ReportEvent;
            httpServer.Start();
        }

        private void HttpServer_ReportEvent(object sender, ReportEventArgs e)
        {
            if (ReportEvent != null)
            {
                ReportEvent(this, e);
            }
        }

        private void HttpServer_DeliverEvent(object sender, DeliverEventArgs e)
        {
            if (DeliverEvent != null)
            {
                DeliverEvent(this, e);
            }
        }

        public bool Connect()
        {
            try
            {
                string post = "account=" + account + "&pswd=" + password;
                string t = HTTPRequest.PostWebRequest(queryBalanceUrl, post, System.Text.Encoding.UTF8);
                if (t != "")
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("JCWG", "Connect Error ->", ex.Message);
            }
            return false;
        }

        public void SendSMS(PlainSMS sms)
        {
            try
            {
                string post = "";
                string content = "";
                foreach (var num in sms.Numbers.Split(','))
                {
                    content = System.Web.HttpUtility.UrlEncode(sms.Content + sms.Signature, System.Text.Encoding.UTF8);
                    post = "account=" + account + "&pswd=" + password + "&mobile=" + num + "&msg=" + content + "&needstatus=true&product=";
                    Console.WriteLine("post-> " + post);
                    string t = HTTPRequest.PostWebRequest(sendUrl, post, System.Text.Encoding.UTF8);
                    Console.WriteLine("request-> " + t);
                    string[] ts = t.Split((char)0xa);
                    string msgid = "";
                    if (ts.Length > 1)
                    {
                        msgid = ts[1];
                    }
                    ts = ts[0].Split(',');

                    int r = int.Parse(ts[1]);
                    bool ok = (r == 0) ? true : false;
                    string rmsg = "发送成功.";
                    switch (r)
                    {
                        case 101:
                            rmsg = "无此用户。";
                            break;
                        case 102:
                            rmsg = "密码错。";
                            break;
                        case 103:
                            rmsg = "提交过快（提交速度超过流速限制）";
                            break;
                        case 104:
                            rmsg = "系统忙（因平台侧原因，暂时无法处理提交的短信）";
                            break;
                        case 105:
                            rmsg = "敏感短信（短信内容包含敏感词）";
                            break;
                        case 106:
                            rmsg = "消息长度错（> 536或 <= 0）";
                            break;
                        case 107:
                            rmsg = "包含错误的手机号码";
                            break;
                        case 108:
                            rmsg = "手机号码个数错（群发 > 50000或 <= 0; 单发 > 200或 <= 0）";
                            break;
                        case 109:
                            rmsg = "无发送额度（该用户可用短信数已使用完）";
                            break;
                        case 110:
                            rmsg = "不在发送时间内";
                            break;
                        case 111:
                            rmsg = "超出该账户当月发送额度限制";
                            break;
                        case 112:
                            rmsg = "无此产品，用户没有订购该产品";
                            break;
                        case 113:
                            rmsg = "extno格式错（非数字或者长度不对）";
                            break;
                        case 115:
                            rmsg = "自动审核驳回";
                            break;
                        case 116:
                            rmsg = "签名不合法，未带签名（用户必须带签名的前提下）";
                            break;
                        case 117:
                            rmsg = "IP地址认证错,请求调用的IP地址不是系统登记的IP地址";
                            break;
                        case 118:
                            rmsg = "用户没有相应的发送权限";
                            break;
                        case 119:
                            rmsg = "用户已过期";
                            break;
                    }
                    if (r > 100)
                    {
                        r = r - 100;  //状态报告程序会根据r的值的范围进行不同的处理 
                    }
                    if (!ok)
                    {
                        msgid = System.Guid.NewGuid().ToString();
                    }
                    SendEventArgs se = new SendEventArgs(sms, msgid, ok, (ushort)(2000 + r), rmsg, 1, 1);
                    string result = JsonSerialize.Instance.Serialize<SendEventArgs>(se);
                    Console.WriteLine("短信发送结果：" + result);

                    if (ok)
                    {
                        LogClient.LogHelper.LogInfo("JCWG", "SendSMS OK ->", result);
                    }
                    else
                    {
                        LogClient.LogHelper.LogInfo("JCWG", "SendSMS Fail ->", result);
                    }
                    if (SendEvent != null)
                    {
                        SendEvent(this, se);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("JCWG", "SendSMS Error ->", ex.Message);
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
                httpServer.Stop();
            }
        }
        public void Close()
        {
            this.Dispose();
        }
    }
}
