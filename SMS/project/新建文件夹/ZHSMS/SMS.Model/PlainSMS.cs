using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    /// <summary>
    /// 短信（发送单元）
    /// </summary>
    [Serializable]
    public class PlainSMS : SMSMessage
    {
        public string Numbers { get; set; }
        public OperatorType OperatorType { get; set; }

        public static PlainSMS CreatePlainSMS(SMSMessage message, SMSNumber number)
        {
            PlainSMS sms = new PlainSMS()
            {
                ID = message.ID,
                AccountID = message.AccountID,
                Channel = message.Channel,
                Content = message.Content,
                Numbers = number.Numbers,
                SplitNumber = message.SplitNumber,
                FeeTotalCount = message.SplitNumber * number.NumberCount,
                NumberCount = number.NumberCount,
                OperatorType = number.Operator,
                SendTime = message.SendTime,
                Signature = "【" + message.Signature + "】",
                SMSLevel = message.SMSLevel,
                SMSType = message.SMSType,
                SPNumber = message.SPNumber
            };
            return sms;
        }

        public static PlainSMS CreatePlainSMS(SMSMessage message, List<string> numbers, OperatorType Op)
        {
            PlainSMS sms = new PlainSMS()
            {
                ID = message.ID,
                AccountID = message.AccountID,
                Channel = message.Channel,
                Content = message.Content,
                Numbers = string.Join(",", numbers),
                SplitNumber = message.SplitNumber,
                FeeTotalCount = message.SplitNumber * numbers.Count,
                NumberCount = numbers.Count,
                OperatorType = Op,
                SendTime = message.SendTime,
                Signature = "【" + message.Signature + "】",
                SMSLevel = message.SMSLevel,
                SMSType = message.SMSType,
                SPNumber = message.SPNumber
            };
            return sms;
        }
        public static PlainSMS GetPlainSMSWithOneNumber(PlainSMS sms, string number)
        {
            PlainSMS csms = new PlainSMS()
            {
                ID = sms.ID,
                AccountID = sms.AccountID,
                Channel = sms.Channel,
                Content = sms.Content,
                Numbers = number,
                SplitNumber = sms.SplitNumber,
                FeeTotalCount = sms.SplitNumber,
                NumberCount = 1,
                OperatorType = sms.OperatorType,
                SendTime = sms.SendTime,
                Signature = "【" + sms.Signature + "】",
                SMSLevel = sms.SMSLevel,
                SMSType = sms.SMSType,
                SPNumber = sms.SPNumber
            };
            return csms;
        }

    }
}
