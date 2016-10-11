using BXM.Utils;
using GatewayInterface;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace TPYXWS
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
        public DateTime Time {
            get
            {
                return _time;
            }
        }
    }

    public class TPYX:ISMSGateway, IDisposable
    {
        public event EventHandler<SMSEventArgs> SMSEvent;

        public event EventHandler<DeliverEventArgs> DeliverEvent;

        public event EventHandler<SendEventArgs> SendEvent;

        public event EventHandler<ReportEventArgs> ReportEvent;


        Dictionary<string, ExSendEventArgs> sends;
        TPWS.MsgSendSoapClient client;
        Timer tmrReport;
        Timer tmrMO;

        bool reportProcess = false;
        string account;
        string password;
        string scrid;
        public TPYX()
        {
            XmlFileSerialize xfs = new XmlFileSerialize("TPYX.Config");
            Config _config = xfs.DeSerialize<Config>();
            account = _config.Account;
            password = _config.Password;
            scrid = _config.SrcID;

            tmrReport = new Timer(10000);
            tmrMO = new Timer(10000);
            tmrMO.Elapsed += tmrMO_Elapsed;
            tmrReport.Elapsed += tmr_Elapsed;
            client = new TPWS.MsgSendSoapClient();

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
                LogClient.LogHelper.LogInfo("TPYX", "Save ReportCache Error ->", ex.Message);
            }
        }

        void tmrMO_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                string t = client.GetMo(account, password);
                //t = "15854881986|,|A 回复内容|,|2015-1-18 3:02:01|;|";
                //t = "15854881986|,|A 回复内容|,|2015-1-18 3:02:01|,|106575010199|;|";
                t = t.Replace("|,|", ((char)3).ToString());
                t = t.Replace("|;|", ((char)2).ToString());

                string[] rs = t.Split((char)2);
                foreach (string r in rs)
                {
                    if (r != "")
                    {
                        string[] result = r.Split((char)3);

                        if (DeliverEvent != null)
                        {
                            DateTime d;
                            if (!DateTime.TryParse(result[2], out d))
                            {
                                d = DateTime.Now;
                            }

                            DeliverEventArgs re = new DeliverEventArgs(Guid.NewGuid().ToString(), d, result[1], result[0], SrcID, "");
                            DeliverEvent(this, re);
                            string tr = JsonSerialize.Instance.Serialize<DeliverEventArgs>(re);
                            LogClient.LogHelper.LogInfo("TPYX", "MO ->", tr);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("TPYX", "MO Error ->", ex.Message);
            }
        }

        void tmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (reportProcess)
            {
                return;
            }
            reportProcess = true;
            try
            {
                Console.WriteLine("报告扫描时，已短信发送数：" + sends.Count);
                if (sends.Count > 0)
                {
                    string t = client.GetReport2(account, password);
                    LogClient.LogHelper.LogInfo("TPYX", "GetReport RAW ->", t);
                    Console.WriteLine("获取的报告：" + t);
                    if (t == "")
                    {
                        reportProcess = false;
                        return;
                    }
                    string[] rs = t.Split('|');
                    foreach (string r in rs)
                    {
                        if (r != "")
                        {
                            string[] result = r.Split(',');
                            if (ReportEvent != null)
                            {
                                bool ok = result[3] == "DELIVRD" ? true : false;
                                ushort statecode;
                                string statetext = "发送成功。";
                                statecode = 2100;
                                if (!ok)
                                {
                                    statetext = "发送失败。";
                                }

                                DateTime d;
                                if (!DateTime.TryParse(result[2], out d))
                                {
                                    d = DateTime.Now;
                                }
                                ReportEventArgs re = new ReportEventArgs(result[0], ok, statecode, statetext, d);
                                ReportEvent(this, re);
                                string tr = JsonSerialize.Instance.Serialize<ReportEventArgs>(re);
                                Console.WriteLine("返回状态报告：" + tr);
                                LogClient.LogHelper.LogInfo("TPYX", "GetReport Process ->", tr);
                                sends.Remove(re.Serial);
                            }
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
                        LogClient.LogHelper.LogInfo("TPYX", "GetReport Timeout ->", ttr);
                        rkeys.Add(key);
                    }
                }
                foreach (string key in rkeys)
                {
                    sends.Remove(key);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("TPYX", "GetReport Error ->", ex.Message);
            }
            reportProcess = false;
        }

        public bool Connect()
        {
            try
            {
                string t = client.GetBalance(account, password);
                int r;
                if (int.TryParse(t, out r))
                {
                    tmrReport.Start();
                    tmrMO.Start();
                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo("TPYX", "Connect Error ->", ex.Message);
            }
            return false;
        }

        public void SendSMS(SMSModel.SMS sms)
        {
            try
            {
                for (int i = 0; i < sms.Number.Count; i++)
                {
                    string t = client.SendMsg(account, password, sms.Number[i], sms.Content + sms.Signature, "");

                    long r = long.Parse(t);
                    bool ok = r > 0 ? true : false;
                    string rmsg = "发送成功.";
                    switch (r)
                    {
                        case -1:
                            rmsg = "提交接口错误。";
                            break;
                        case -3:
                            rmsg = "用户名或密码错误。";
                            break;
                        case -4:
                            rmsg = "短信内容和备案的模板不一样。";
                            break;
                        case -5:
                            rmsg = "签名不正确。";
                            break;
                        case -7:
                            rmsg = "余额不足。";
                            break;
                        case -8:
                            rmsg = "通道错误。";
                            break;
                        case -9:
                            rmsg = "无效号码。";
                            break;
                        case -10:
                            rmsg = "签名内容不符合长度。";
                            break;
                        case -11:
                            rmsg = "用户有效期过期。";
                            break;
                        case -12:
                            rmsg = "黑名单。";
                            break;
                        case -13:
                            rmsg = "语音验证码的 Amount 参数必须是整形字符串。";
                            break;
                        case -14:
                            rmsg = "语音验证码的内容只能为数字和字母。";
                            break;
                        case -15:
                            rmsg = "语音验证码的内容最长为 6 位。";
                            break;
                    }
                    if (r < 0)
                    {
                        //发送失败时，网关自己生成serial
                        t = System.Guid.NewGuid().ToString();
                    }
                    SendEventArgs se = new SendEventArgs(sms, t, ok, (ushort)(2000 - r), rmsg, 1, 1);
                    string result = JsonSerialize.Instance.Serialize<SendEventArgs>(se);
                    Console.WriteLine("短信发送结果：" + result);
                    ExSendEventArgs ese = new ExSendEventArgs(se);
                    
                    if (ok)
                    {
                        sends.Add(se.Serial,ese);
                        LogClient.LogHelper.LogInfo("TPYX", "SendSMS OK ->", result);
                    }
                    else
                    {
                        LogClient.LogHelper.LogInfo("TPYX", "SendSMS Fail ->", result);
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
                LogClient.LogHelper.LogInfo("TPYX", "SendSMS Error ->", ex.Message);
            }
        }

        public void SendSubmit(SMSModel.SMS sms)
        {
            
        }

        public string SrcID
        {
            get { return scrid; }
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
                    LogClient.LogHelper.LogInfo("TPYX", "Save ReportCache Error ->", ex.Message);
                }
            }
        }

        public void Close()
        {
            this.Dispose();
        }
    }
}
