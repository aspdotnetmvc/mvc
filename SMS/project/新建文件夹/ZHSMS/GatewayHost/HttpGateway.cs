using BXM.Utils;
using GatewayInterface;
using MessageTools;
using Newtonsoft.Json;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Timers;

namespace GatewayHost
{
    public class HttpGateway : ISMSGateway, IDisposable
    {
        public event EventHandler<SMSEventArgs> SMSEvent;
        public event EventHandler<DeliverEventArgs> DeliverEvent;
        public event EventHandler<ReportEventArgs> SendEvent;
        public event EventHandler<ReportEventArgs> ReportEvent;

        HttpGatewayInterface Gateway;
        Timer tmrReport, tmrMO;
        Dictionary<string, ReportEventArgs> sends;
        bool reportProcess = false;
        object locker = new object();

        public HttpGateway()
        {
            
            HttpGatewayConfig Config = BXM.Utils.XmlSerialize.DeSerialize<HttpGatewayConfig>("HttpGatewayConfig");

            this.Gateway = (HttpGatewayInterface)Assembly.GetExecutingAssembly().CreateInstance("HttpGateways." + Config.GatewayClass, true);

            if (Gateway.Config.StatusReportInterval > 0)
            {
                tmrReport = new Timer(Gateway.Config.StatusReportInterval);
                tmrReport.Elapsed += tmrReport_Elapsed;
            }

            if (Gateway.Config.MOInterval > 0)
            {
                tmrMO = new Timer(Gateway.Config.MOInterval);
                tmrMO.Elapsed += tmrMO_Elapsed;
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
        }


        public 
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
                    var list = Gateway.GetStatusReport();

                    ReportEventArgs re = new ReportEventArgs();
                    lock (locker)
                    {
                        foreach (var sr in list)
                        {
                            var se = sends.FirstOrDefault(s => s.Key == sr.SerialNumber);
                            if (!se.Equals(default(KeyValuePair<string, ReportEventArgs>)))
                            {

                                var osr = se.Value.StatusReports.Where(r => r.Number == sr.Number);
                                osr.ToList().ForEach(s => { s.ResponseTime = DateTime.Now; s.StatusCode = sr.StatusCode; s.Description = sr.Message; });
                                re.StatusReports.AddRange(osr);
                                

                                foreach (var r in osr)
                                {
                                    se.Value.StatusReports.Remove(r);
                                }
                                if (se.Value.StatusReports.Count == 0)
                                {
                                    sends.Remove(se.Key);
                                }
                            }
                        }
                        ReportEvent(this, re);


                        List<string> rkeys = new List<string>();
                        foreach (string key in sends.Keys)
                        {
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
        void tmrMO_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var list = Gateway.GetMO();
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                    if (Gateway.Config.MOInterval > 0)
                    {
                        tmrMO.Start();

                    }
                    if (Gateway.Config.StatusReportInterval > 0)
                    {
                        tmrReport.Start();

                    }
                    tmrReport.Start();

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
                SendResult sr = Gateway.SendSMS(sms);
                ReportEventArgs re = new ReportEventArgs();
                int i = 0;
                re.StatusReports.AddRange(sms.Numbers.Split(',').Select(n => new StatusReport()
                {
                    SMSID = sms.ID,
                    SerialNumber = sr.SerialNumber + (++i).ToString().PadLeft(5, '0'),
                    SendTime = sms.SendTime.Value,
                    StatusCode = 2000 + sr.StatusCode,
                    Succeed = sr.Success,
                    Channel = sms.Channel,
                    Description = sr.Message,
                    Number = n,
                    ResponseTime = null
                }));
                SendEvent(this, re);
                if (sr.Success)
                {
                    lock (locker)
                    {
                        sends.Add(sr.SerialNumber, re);
                    }
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
                    LogClient.LogHelper.LogInfo(Gateway.Config.GatewayName, "Save ReportCache Error ->", ex.Message);
                }
            }
        }
        public void Close()
        {
            this.Dispose();
        }
    }
}
