using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMSModel;
using System.Data;
namespace BLL
{
    public class SMSdo
    {
        public static bool SMSAdd(SMS sms)
        {
            return DAL.SMSdo.SMSAdd(sms);
        }
        public static DataTable GetSMSByAccount(string accountID)
        {
            return DAL.SMSdo.GetSMSByAccount(accountID);
        }
        public static DataTable GetSMSByAccountAndSendTime(string accountID, DateTime start, DateTime end)
        {
            return DAL.SMSdo.GetSMSByAccountAndSendTime(accountID, start, end);
        }
    }
}
