using MQAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace StatusReportService
{
    public class BillingSend
    {
        private volatile static BillingSend mng = null;
        private static object lockHelper = new object();

        FanoutQueue fqBilling;

        private BillingSend()
        {
            fqBilling = new FanoutQueue(ConfigurationManager.AppSettings["MQVHost"], ConfigurationManager.AppSettings["MQUrl"], ConfigurationManager.AppSettings["MQName"], ConfigurationManager.AppSettings["MQPassword"], "Billing", new string[] { "Billing" });

        }

        public static BillingSend Instance
        {
            get
            {
                if (mng == null)
                {
                    lock (lockHelper)
                    {
                        if (mng == null)
                        {
                            mng = new BillingSend();
                            return mng;
                        }
                    }
                }
                return mng;
            }
        }

        public void Send(string message)
        {
            fqBilling.Send(message);
        }
    }
}

