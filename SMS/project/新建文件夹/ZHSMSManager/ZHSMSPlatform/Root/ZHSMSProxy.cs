using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using ZHSMSPlatInterface;

namespace ZHSMSPlatform.Root
{
    public class ZHSMSProxy
    {
        public static ISMService GetZHSMSPlatService()
        {
            ISMService sr = (ISMService)Activator.GetObject(typeof(ISMService), ConfigurationManager.AppSettings["ZHSMSPlatService"]);
            return sr;
        }
    }
}