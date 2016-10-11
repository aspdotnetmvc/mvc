using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SMSPlatInterface;

namespace HttpSMService
{
    public class InterfaceProxy
    {
        public static ISMService GetSendService()
        {
            ISMService sms = (ISMService)Activator.GetObject(typeof(ISMService), ConfigurationManager.AppSettings["ISMService"]);
            return sms;
        }
    }
}