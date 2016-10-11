using GatewayInterface;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HttpGateways
{
    public class ZSHY : HttpGatewayInterface
    {
        public void init()
        {
            
        }
        public GatewayInterface.HttpGatewayConfig Config
        {
            get;
            set;
        }
        public GatewayInterface.BalanceResult GetBalance()
        {
            Dictionary<string, string> par = new Dictionary<string, string>();
            par.Add("action", "overage");
            par.Add("userid", Config.UserId);
            par.Add("account", Config.Account);
            par.Add("password", Config.Password);
            string res = HttpUtil.Post(Config.BalanceUrl, par, Encoding.UTF8);
            var xdoc = XDocument.Parse(res);

            var balance = new BalanceResult()
            {
                Success = xdoc.Root.Element("returnstatus").Value.Equals("Sucess"),
                Message = xdoc.Root.Element("message").Value,
                Balance = int.Parse(xdoc.Root.Element("overage").Value)
            };
            return balance;
        }

        public GatewayInterface.SendResult SendSMS(PlainSMS sms)
        {
            Dictionary<string, string> par = new Dictionary<string, string>();
            par.Add("action", "send");
            par.Add("userid", Config.UserId);
            par.Add("account", Config.Account);
            par.Add("password", Config.Password);
            par.Add("mobile", sms.Numbers);
            var content = sms.Content + sms.Signature;
            if (Config.SignaturePos == 1)
            {
                content = sms.Signature + sms.Content;
            }
            par.Add("content", content);
            par.Add("sendTime", "");
            if (Config.ExtendNo == 1)
            {
                par.Add("extno", sms.SPNumber);
            }
            else
            {
                par.Add("extno", "");
            }

            string res = HttpUtil.Post(Config.SendUrl, par, Encoding.UTF8);
            var xdoc = XDocument.Parse(res);

            var sendresult = new SendResult()
            {
                Success = xdoc.Root.Element("returnstatus").Value.Equals("Success"),
                Message = xdoc.Root.Element("message").Value,
                SerialNumber = xdoc.Root.Element("taskID").Value,
                StatusCode = 2000
            };
            if (!sendresult.Success)
            {
                sendresult.StatusCode = 2099;
            }
            if (string.IsNullOrWhiteSpace(sendresult.SerialNumber))
            {
                sendresult.SerialNumber = System.Guid.NewGuid().ToString();
            }
            return sendresult;

        }

        public List<GatewayInterface.StatusResult> GetStatusReport(string SerialNumber = null)
        {
            Dictionary<string, string> par = new Dictionary<string, string>();
            par.Add("action", "query");
            par.Add("userid", Config.UserId);
            par.Add("account", Config.Account);
            par.Add("password", Config.Password);
            var res = HttpUtil.Post(Config.StatusReportUrl, par, Encoding.UTF8);
            var xdoc = XDocument.Parse(res);

            var sr = from statusbox in xdoc.Root.Elements()
                     select new StatusResult()
                     {
                         Number = statusbox.Element("mobile").Value,
                         SerialNumber = statusbox.Element("taskid").Value,
                         Message = statusbox.Element("errorcode").Value,
                         StatusCode = statusbox.Element("status").Value.Equals("10") ? 2100 : 2101,
                         Success = statusbox.Element("status").Value.Equals("10")
                     };
            return sr.ToList();
        }

        public List<SMS.Model.MOSMS> GetMO()
        {
            Dictionary<string, string> par = new Dictionary<string, string>();
            par.Add("action", "query");
            par.Add("userid", Config.UserId);
            par.Add("account", Config.Account);
            par.Add("password", Config.Password);
            string res = HttpUtil.Post(Config.MOUrl, par, Encoding.UTF8);
            var xdoc = XDocument.Parse(res);
            var mo = from callbox in xdoc.Root.Elements()
                     select new MOSMS()
                     {
                         Gateway = Config.GatewayName,
                         SerialNumber = System.Guid.NewGuid().ToString(),
                         Message = callbox.Element("content").Value,
                         UserNumber = callbox.Element("mobile").Value,
                         ReceiveTime = DateTime.Now
                     };
            return mo.ToList();
        }


    }
}
