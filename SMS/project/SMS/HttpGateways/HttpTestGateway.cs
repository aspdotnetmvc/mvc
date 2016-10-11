using GatewayInterface;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HttpGateways
{
    public class HttpTestGateway : HttpGatewayInterface
    {
        public void init()
        {

        }
        Dictionary<string, PlainSMS> smslist = new Dictionary<string, PlainSMS>();
        List<MOSMS> molist = new List<MOSMS>();
        Random ran = new Random();
        public GatewayInterface.HttpGatewayConfig Config
        {
            get;
            set;
        }
        public GatewayInterface.BalanceResult GetBalance()
        {
            var balance = new BalanceResult()
            {
                Success = true,
                Message = "连接成功",
                Balance = 9999
            };
            return balance;
        }

        public SendResult SendSMS(PlainSMS sms)
        {
            var r = ran.Next(100);
            if (r < 10)
            {
                //十分之一提交失败
                return new SendResult()
                {
                    Message = "提交失败",
                    SerialNumber = System.Guid.NewGuid().ToString(),
                    StatusCode = 2099,
                    Success = false
                };
            }


            var sr = new SendResult()
            {
                Message = "提交成功",
                Success = true,
                SerialNumber = System.Guid.NewGuid().ToString(),
                StatusCode = 2000
            };
            smslist.Add(sr.SerialNumber, sms);
            lock (this)
            {
                foreach (var num in sms.Numbers.Split(','))
                {
                    r = ran.Next(100);
                    if (r > 50)
                    {
                        var mo = new MOSMS()
                                          {
                                              Gateway = Config.GatewayName,
                                              SerialNumber = System.Guid.NewGuid().ToString(),
                                              Message = "回复：" + sms.Content,
                                              UserNumber = num,
                                              ReceiveTime = DateTime.Now
                                          };
                        molist.Add(mo);
                    }
                }

            }
            return sr;
        }

        public List<StatusResult> GetStatusReport(string SerialNumber = null)
        {
            if (smslist.Count > 0)
            {
                var key = smslist.Keys.First();
                var sms = smslist[key];
                smslist.Remove(key);
                var sr = new List<StatusResult>();
                foreach (var s in sms.Numbers.Split(','))
                {
                    if (ran.Next(100) > 50)
                    {
                        sr.Add(new StatusResult()
                                                                {
                                                                    Number = s,
                                                                    SerialNumber = key,
                                                                    Message = "发送成功",
                                                                    StatusCode = 2100,
                                                                    Success = true
                                                                });
                    }
                    else
                    {
                        sr.Add(new StatusResult()
                                                                {
                                                                    Number = s,
                                                                    SerialNumber = key,
                                                                    Message = "发送失败",
                                                                    StatusCode = 2101,
                                                                    Success = false
                                                                });
                    }

                }

                return sr.ToList();
            }
            else
            {
                return new List<StatusResult>();
            }
        }

        public List<SMS.Model.MOSMS> GetMO()
        {
            lock (this)
            {
                if (molist.Count > 0)
                {
                    var list = molist.Take(100).ToList();
                    foreach (var mo in list)
                    {
                        molist.Remove(mo);
                    }
                    return list;
                }
            }
            return null;

        }

    }
}
