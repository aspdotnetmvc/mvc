using SMSModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace StatusReportHost
{
    public class PretreatmentProxy
    {
        public static ISMS GetPretreatmentService()
        {
            ISMS sms = (ISMS)Activator.GetObject(typeof(ISMS), ConfigurationManager.AppSettings["Pretreatment"]);
            return sms;
        }
    }
}
