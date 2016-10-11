using GatewayInterface;
using MessageTools;
using Newtonsoft.Json;
using SMS.Model;
using SMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace HttpGateways
{
    /// <summary>
    /// http 网关,状态报告和上行短信从第三方push过来
    /// GatewayRecieve 接收到后发送到MQ，网关从MQ接收
    /// </summary>
    public class HttpPushGateway : ISMSGateway, IDisposable
    {
        public event EventHandler<SMSEventArgs> SMSEvent;
        public event EventHandler<DeliverEventArgs> DeliverEvent;
        public event EventHandler<ReportEventArgs> SendEvent;
        public event EventHandler<ReportEventArgs> ReportEvent;

        public HttpGatewayInterface Gateway;
        StatusReportMQHelper SRMQ = new StatusReportMQHelper();
        

        Dictionary<string, ReportEventArgs> sends;
        bool reportProcess = false;
        object locker = new object();
        TrafficController tc = new TrafficController();

        public HttpPushGateway(string GatewayName)
        {

            HttpGatewayConfig Config = SMS.Util.XmlSerialize.DeSerialize<HttpGatewayConfig>(GatewayName+".config");

            this.Gateway = (HttpGatewayInterface)Assembly.GetExecutingAssembly().CreateInstance("HttpGateways." + Config.GatewayClass, true);
            this.Gateway.Config = Config;
            if (Gateway.Config.StatusReportInterval > 0)
            {
                //启动状态报告MQ监听
                SRMQ.Start(this);
            }

            if (Gateway.Config.MOInterval > 0)
            {
                 //启动上行短信MQ监听
            }

            sends = new Dictionary<string, ReportEventArgs>();
            try
            {
                BinarySerialize<Dictionary<string, ReportEventArgs>> bs = new BinarySerialize<Dictionary<string, ReportEventArgs>>();
                sends = bs.DeSerialize("ReportCache");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo(Gateway.Config.GatewayName, "Save ReportCache Error ->", ex.Message);
            }
            if (Config.EnableTrafficControl > 0)
            {
                tc.IntervalMicroSeconds = Config.TrafficMonitorIntervalSeconds * 1000;
                tc.MaxCount = Config.HandlingAbility * Config.TrafficMonitorIntervalSeconds;
                tc.Start();
            }
        }


       public void StatusReportRecieved(List<StatusResult> statuslist){
            if (reportProcess)
            {
                return;
            }
            reportProcess = true;
            try
            {
                if (sends.Count > 0)
                {

                    MessageHelper.Instance.WirteTest("状态报告返回结果：" + JsonConvert.SerializeObject(statuslist));
                    ReportEventArgs re = new ReportEventArgs();
                    lock (locker)
                    {
                        foreach (var sr in statuslist)
                        {
                            var se = sends.FirstOrDefault(s => s.Key == sr.SerialNumber);
                            if (!se.Equals(default(KeyValuePair<string, ReportEventArgs>)))
                            {
                                var r = se.Value;
                                var osr = r.StatusReports.Where(s => s.Number == sr.Number).ToList();
                                foreach (var s in osr)
                                {
                                    s.ResponseTime = DateTime.Now;
                                    s.StatusCode = sr.StatusCode;
                                    s.Description =   sr.Message;
                                    s.Succeed = sr.Success;
                                };
                                re.StatusReports.AddRange(osr);

                                foreach (var o in osr)
                                {
                                    r.StatusReports.Remove(o);
                                }
                            }
                        }
                        ReportEvent(this, re);


                        List<string> rkeys = new List<string>();
                        foreach (string key in sends.Keys)
                        {
                            if (sends[key].StatusReports.Count == 0)
                            {
                                rkeys.Add(key);
                                continue;
                            }
                            if ((DateTime.Now - sends[key].SubmitTime).TotalDays >= 2)
                            {
                                ReportEventArgs sre = new ReportEventArgs();

                                sre.StatusReports.AddRange(sends[key].StatusReports);
                                sends[key].StatusReports.ForEach(s => { s.StatusCode = 2100; s.Description = "超时返回状态报告"; s.ResponseTime = DateTime.Now; });

                                ReportEvent(this, sre);
                                string ttr = JsonConvert.SerializeObject(sre);
                                MessageHelper.Instance.WirteInfo("超时返回状态报告：" + ttr);
                                LogClient.LogHelper.LogInfo(Gateway.Config.GatewayName, "GetReport Timeout ->", ttr);
                                rkeys.Add(key);
                            }
                        }
                        foreach (string key in rkeys)
                        {
                            sends.Remove(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo(Gateway.Config.GatewayName, "GetReport Error ->", ex.Message);
            }
            reportProcess = false;
        }
       public void MORecieved(List<MOSMS> list)
        {
            try
            {
                if (list == null || list.Count == 0)
                {
                    return;
                }
                if (DeliverEvent != null)
                {
                    foreach (var mo in list)
                    {
                        DeliverEventArgs de = new DeliverEventArgs(mo);
                        DeliverEvent(this, de);
                        string tr = JsonConvert.SerializeObject(de);
                        LogClient.LogHelper.LogInfo(Gateway.Config.GatewayName, "MO ->", tr);
                    }
                }
                MessageHelper.Instance.WirteTest("上行短信：" + JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {
                MessageHelper.Instance.WirteError("获取上行短信发生错误", ex);
                LogClient.LogHelper.LogInfo(Gateway.Config.GatewayName, "MO Error ->", ex.Message);
            }
        }
        public bool Connect()
        {
            try
            {
                BalanceResult br = Gateway.GetBalance();
                if (br.Success)
                {
                    Console.WriteLine("连接消息 : " + br.Message);
                    Console.WriteLine("余额 : " + br.Balance);
                }
                else
                {
                    Console.WriteLine("获取余额失败：" + br.Message);
                }
                return br.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogClient.LogHelper.LogInfo(Gateway.Config.GatewayName, "Connect Error ->", ex.Message);
            }
            return false;
        }
        public void SendSMS(PlainSMS sms)
        {
            try
            {
                MessageHelper.Instance.WirteTest("提交短信：" + JsonConvert.SerializeObject(sms));
                SendResult sr = Gateway.SendSMS(sms);
                if (string.IsNullOrWhiteSpace(sr.SerialNumber))
                {
                    sr.SerialNumber = System.Guid.NewGuid().ToString();
                }
                MessageHelper.Instance.WirteTest("提交短信返回结果：" + JsonConvert.SerializeObject(sr));
                ReportEventArgs re = new ReportEventArgs();
                int i = 0;
                re.StatusReports.AddRange(sms.Numbers.Split(',').Select(n => new StatusReport()
                {
                    SMSID = sms.ID,
                    SerialNumber = sr.SerialNumber + (++i).ToString().PadLeft(5, '0'),
                    SendTime = sms.SendTime.Value,
                    StatusCode = sr.StatusCode,
                    Succeed = sr.Success,
                    Channel = sms.Channel,
                    Description = sr.Message,
                    Number = n,
                    ResponseTime = null,
                    Gateway = Gateway.Config.GatewayName,
                    AccountID = sms.AccountID,
                    StatusReportType = (StatusReportType)sms.StatusReportType
                }));
                SendEvent(this, re);
                if (sr.Success)
                {
                    lock (locker)
                    {
                        sends.Add(sr.SerialNumber, re);
                    }
                }
                if (this.Gateway.Config.EnableTrafficControl > 0)
                {
                    tc.AddCountAndCheckTraffic(sms.NumberCount);
                }
            }
            catch (Exception ex)
            {
                MessageHelper.Instance.WirteError(ex.ToString());
                LogClient.LogHelper.LogInfo(Gateway.Config.GatewayName, "SendSMS Error ->", ex.Message);
            }
        }

        bool _dispose = true;
        public void Dispose()
        {
            if (_dispose)
            {
                _dispose = false;
             
                BinarySerialize<Dictionary<string, ReportEventArgs>> bs = new BinarySerialize<Dictionary<string, ReportEventArgs>>();
                try
                {
                    bs.Serialize(sends, "ReportCache");
                }
                catch (Exception ex)
                {
                    MessageHelper.Instance.WirteError(ex.ToString());
                    LogClient.LogHelper.LogInfo(Gateway.Config.GatewayName, "Save ReportCache Error ->", ex.Message);
                }
            }
        }
        public void Close()
        {
            this.Dispose();
        }

        internal void RecieveStatusReport(List<StatusReport> sr)
        {
            throw new NotImplementedException();
        }
    }
}
