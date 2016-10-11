using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SMSPlatform;

namespace HttpSMSService
{
    public class InterfaceProxy
    {
        public static ISMSPlatformService GetSendService()
        {
            ISMSPlatformService sms = (ISMSPlatformService)Activator.GetObject(typeof(ISMSPlatformService), ConfigurationManager.AppSettings["SMSPlatformService"]);
            return sms;
        }
    }
}