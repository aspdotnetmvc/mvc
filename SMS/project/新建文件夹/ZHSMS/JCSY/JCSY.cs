using BXM.Utils;
using GatewayInterface;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Xml;

namespace JCSY
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

    public class JCSY : ISMSGateway, IDisposable
    {
        public event EventHandler<SMSEventArgs> SMSEvent;
        public event EventHandler<DeliverEventArgs> DeliverEvent;
        public event EventHandler<SendEventArgs> SendEvent;
        public event EventHandler<ReportEventArgs> ReportEvent;

        string balanceUrl = "";
        string sendUrl = "";
        string statusUrl = "";
        string moUrl = "";
        Timer tmrReport, tmrMO;
        Dictionary<string, ExSendEventArgs> sends;
        bool reportProcess = false;
        bool sendProcess = false;
        Config _config;
        CacheManager<List<SMS>> _cm;

        string[] split = new string[] { "|;|" };
        string[] splitContent = new string[] { "|^|" };
        static object locker = new object();



        public JCSY()
        {
            XmlFileSerialize xfs = new XmlFileSerialize("JCSY.Config");
            _config = xfs.DeSerialize<Config>();

            balanceUrl = _config.ServerUrl + "/smshttp";
            sendUrl = _config.ServerUrl + "/smshttp";
            statusUrl = _config.ServerUrl + "/smshttp";
            moUrl = _config.ServerUrl + "/smshttp";
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
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("JCSY", "Save ReportCache Error ->", ex.Message);
            }

            _cm = CacheManager<List<SMS>>.Instance;
            _cm.ExpireRemoveCache = true;
            _cm.ExpireMillisecond = 2000;
            _cm.CheckMillisecond = 1000;
            _cm.ExpireCache += _cm_ExpireCache;
        }

        private void _cm_ExpireCache(List<List<SMS>> caches)
        {
            if (sendProcess)
            {
                return;
            }
            sendProcess = true;
            Console.WriteLine("缓存过期条数 " + caches.Count);

            try
            {
                string post = "";
                for (int i = 0; i < caches.Count; i++)
                {
                    List<string> ss = new List<string>();
                    foreach (SMS s in caches[i])
                    {
                        ss.AddRange(s.Number);
                    }
                    Console.WriteLine("第 " + i + " 条包含短信数量 " + caches[i].Count);
                    string send = "";
                    foreach (string s in ss)
                    {
                        send += s + ",";
                    }
                    send = send.Substring(0, send.Length - 1);
                    string content = caches[i][0].Content + caches[i][0].Signature;
                    post = "act=sendmsg&unitid=" + _config.UserId + "&username=" + _config.Account + "&passwd=" + _config.Password + "&phone=" + send + "&msg=" + content + "&port=&sendtime=" + DateTime.Now.ToString();
                    string t = HTTPRequest.PostWebRequest(sendUrl, post, Encoding.UTF8);

                    Console.WriteLine("发送字符串 " + post);
                    //状态,批号,说明
                    bool ok = true;
                    string rmsg = "短信提交成功";

                    string[] rt = t.Split(',');
                    int rok = 0;
                    int.TryParse(rt[0], out rok);
                    if (rok < 0)
                    {
                        rmsg = GetErrorMsg(rok);
                        Console.WriteLine(rmsg);
                        LogClient.LogHelper.LogInfo("JCSY", "Connect Error ->", rmsg);
                        ok = false;
                    }
                    int r = 1;//原来默认0 ,应为1  by lmw  1 代表已发送
                    if (!ok)
                    {
                        // r = 101;
                        r = 99;//提交失败 错误码应该在100以内吧   by lmw
                    }

                    foreach (SMS s in caches[i])
                    {
                        SendEventArgs se = new SendEventArgs(s, rt[1] + s.Number[0], ok, (ushort)(2000 + r), rmsg, 1, 1);
                        //string result = JsonSerialize.Instance.Serialize<SendEventArgs>(se);
                        //Console.WriteLine("短信发送结果：" + result);
                        ExSendEventArgs ese = new ExSendEventArgs(se);
                        if (ok)
                        {
                            sends.Add(se.SerialNumber, ese);
                            //LogClient.LogHelper.LogInfo("JCSY", "SendSMS OK ->", result);
                        }
                        else
                        {
                            //LogClient.LogHelper.LogInfo("JCSY", "SendSMS Fail ->", result);
                        }
                        if (SendEvent != null)
                        {
                            SendEvent(this, se);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("JCSY", "SendSMS Error ->", ex.Message);
            }
            sendProcess = false;
        }

        void tmrReport_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (reportProcess)
            {
                return;
            }
            reportProcess = true;
            //格式: 批号 |^| 号码 |^| 状态  两条记录这间用 |;| 分隔
            try
            {
                if (sends.Count > 0)
                {
                    string msgid = "";
                    bool ok = true;
                    DateTime datetime = DateTime.Now;
                    string post = string.Format("act=getstatue&unitid={0}&username={1}&passwd={2}&rowid=0", _config.UserId, _config.Account, _config.Password);
                    string t = HTTPRequest.PostWebRequest(statusUrl, post, Encoding.UTF8);
                    Console.WriteLine("查询原始状态报告：" + t);
                    if (t == "0")
                    {
                        reportProcess = false;
                        return;
                    }
                    string[] repotrs = t.Split(split, StringSplitOptions.RemoveEmptyEntries);
                    if (repotrs.Length == 0)
                    {
                        reportProcess = false;
                        return;
                    }
                    foreach (string report in repotrs)
                    {
                        string[] rc = report.Split(splitContent, StringSplitOptions.RemoveEmptyEntries);
                        msgid = rc[0];
                        datetime = DateTime.Now;
                        if (rc[2] == "DELIVRD")
                        {
                            ok = true;
                        }
                        else
                        {
                            ok = false;
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
                                var se = sends.FirstOrDefault(s => s.Value.SendEventArgs.SerialNumber.StartsWith(msgid) && s.Value.SendEventArgs.Message.Number[0] == rc[1]);
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
                                    MessageTools.MessageHelper.Instance.WirteInfo("无法匹配状态报告：msgid:" + msgid + " mobile:" + rc[1]);
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
                            LogClient.LogHelper.LogInfo("JCSY", "GetReport Timeout ->", ttr);
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
                LogClient.LogHelper.LogInfo("JCSY", "GetReport Error ->", ex.Message);
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
                DateTime datetime = DateTime.Now;
                string post = string.Format("act=smsrecord&unitid={0}&username={1}&passwd={2}&rowid=0", _config.UserId, _config.Account, _config.Password);
                string t = HTTPRequest.PostWebRequest(moUrl, post, Encoding.UTF8);
                if (t == "0") return;
                //格式: 序号 |^| 上行号码 |^| 内容 |^| 时间 |^| sp端口号 两条记录之间用 |;| 分隔
                //32221 | 15800000000 |^| 这是回复测试 |^| 2000 - 07 - 16 15:10:27 |^| 1065788 |;| 32222 |^| 15800000000 |^| 这是回复测试2 |^| 2000 - 07 - 16 15:10:27 |^| 1065788
                string[] mos = t.Split(split, StringSplitOptions.RemoveEmptyEntries);
                if (mos.Length == 0) return;
                foreach (string mo in mos)
                {
                    string[] moc = t.Split(splitContent, StringSplitOptions.RemoveEmptyEntries);
                    mobile = moc[1];
                    msg = moc[2];
                    msgid = moc[0];
                    if (!DateTime.TryParse(moc[3], out datetime))
                    {
                        datetime = DateTime.Now;
                    }
                    if (DeliverEvent != null)
                    {
                        DeliverEventArgs re = new DeliverEventArgs(Guid.NewGuid().ToString(), datetime, msg, mobile, SrcID, "");
                        DeliverEvent(this, re);
                        string tr = JsonSerialize.Instance.Serialize<DeliverEventArgs>(re);
                        LogClient.LogHelper.LogInfo("JCSY", "MO ->", tr);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("JCSY", "MO Error ->", ex.Message);
            }
        }
        public bool Connect()
        {
            try
            {
                string post = string.Format("act=getbalance&unitid={0}&username={1}&passwd={2}", _config.UserId, _config.Account, _config.Password);
                string t = HTTPRequest.PostWebRequest(balanceUrl, post, Encoding.UTF8);
                int ok = 0;

                if (int.TryParse(t, out ok) && ok >= 0)
                {
                    tmrMO.Start();
                    tmrReport.Start();
                    MessageTools.MessageHelper.Instance.WirteInfo("状态报告和上行短信服务已启动");
                    return true;
                }
                else
                {
                    MessageTools.MessageHelper.Instance.WirteInfo("连接失败，状态报告和上行短信服务未启动");
                    string errmsg = GetErrorMsg(ok);
                    Console.WriteLine(errmsg);
                    LogClient.LogHelper.LogInfo("JCSY", "Connect Error ->", errmsg);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("JCSY", "Connect Error ->", ex.Message);
            }
            return false;
        }
        public void SendSMS(PlainSMS sms)
        {
            try
            {
                string post = "";


                string send = string.Join(",", sms.Number);

                string content = sms.Signature + sms.Content;
                post = "act=sendmsg&unitid=" + _config.UserId + "&username=" + _config.Account + "&passwd=" + _config.Password + "&phone=" + send + "&msg=" + content + "&port=&sendtime=" + DateTime.Now.ToString();
                string t = HTTPRequest.PostWebRequest(sendUrl, post, Encoding.GetEncoding("GBK"));

                Console.WriteLine("发送字符串 " + post);
                //状态,批号,说明
                bool ok = true;
                string rmsg = "短信提交成功";

                string[] rt = t.Split(',');
                int rok = 0;
                int r = 1;//原来默认0 ,应为1  by lmw  1 代表已发送
                if (int.TryParse(rt[0], out rok) && rok == 1)
                {

                }
                else
                {
                    rmsg = GetErrorMsg(rok);
                    Console.WriteLine(rmsg);
                    LogClient.LogHelper.LogInfo("JCSY", "Connect Error ->", rmsg);
                    ok = false;
                    r = 99;
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
                    SendEventArgs se = new SendEventArgs(s, rt[1] + i.ToString().PadLeft(5, '0'), ok, (ushort)(2000 + r), rmsg, 1, 1);

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
                LogClient.LogHelper.LogInfo("JCSY", "SendSMS Error ->", ex.Message);
            }
        }

        public void SendSubmit(PlainSMS sms)
        {

        }

        string GetErrorMsg(int errcode)
        {
            string msg = "";
            switch (errcode)
            {
                case 1:
                    msg = "成功";
                    break;
                case -1:
                    msg = "账号错误";
                    break;
                case -2:
                    msg = "企业ID错误";
                    break;
                case -3:
                    msg = "密码错误";
                    break;
                case -4:
                    msg = "账号已停用";
                    break;
                case -5:
                    msg = "内容或号码为空";
                    break;
                case -6:
                    msg = "余额不足";
                    break;
                case -7:
                    msg = "内容包含非法关键字";
                    break;
                case -9:
                    msg = "获取批号失败";
                    break;
                case -10:
                    msg = "发送短信失败";
                    break;
                case -11:
                    msg = "时间格式错误";
                    break;
                case -100:
                    msg = "非法参数";
                    break;
                case -200:
                    msg = "系统错误";
                    break;
            }
            return msg;
        }

        public string SrcID
        {
            get { return _config.SrcID; }
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

                //List<ExSendEventArgs> cache = sends.Values.ToList();
                //ExSendEventArgs t = new ExSendEventArgs(new SendEventArgs(new SMS(new List<string> { "15888888888" },"123", LevelType.Level10,DateTime.Now, StatusReportType.Enabled), 1, 1));
                //t.SendEventArgs.Serial = "123321";
                //cache.Add(t);
                BinarySerialize<List<ExSendEventArgs>> bs = new BinarySerialize<List<ExSendEventArgs>>();
                try
                {
                    bs.Serialize(sends.Values.ToList(), "ReportCache");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogClient.LogHelper.LogInfo("JCSY", "Save ReportCache Error ->", ex.Message);
                }
            }
        }

        public void Close()
        {
            this.Dispose();
        }
    }
}
