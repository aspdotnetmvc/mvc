using SMS.Model;
using System.Collections.Generic;

namespace GatewayInterface
{
    public interface HttpGatewayInterface
    {
        void init();
        HttpGatewayConfig Config
        {
            get;
            set;
        }
        BalanceResult GetBalance();
        SendResult SendSMS(PlainSMS sms);
        List<StatusResult> GetStatusReport(string SerialNumber = null);
        List<MOSMS> GetMO();
    }
}
