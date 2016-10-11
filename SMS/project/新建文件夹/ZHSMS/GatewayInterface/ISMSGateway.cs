using SMS.Model;
using System;

namespace GatewayInterface
{
    public interface ISMSGateway
    {
        event EventHandler<SMSEventArgs> SMSEvent;
        event EventHandler<DeliverEventArgs> DeliverEvent;
        event EventHandler<ReportEventArgs> SendEvent;
        event EventHandler<ReportEventArgs> ReportEvent;
        bool Connect();
        void SendSMS(PlainSMS sms);
        void Close();
    }
}
