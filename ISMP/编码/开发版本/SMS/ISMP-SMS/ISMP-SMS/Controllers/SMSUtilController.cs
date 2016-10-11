using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISMP_SMS.Controllers
{
    public class SMSUtilController : BaseController
    {
        public ActionResult GetSplitNumber()
        {
            string _content = Request["Content"];
            string _signature = Request["Signature"];
            _signature = '【' + _signature.TrimStart('【').TrimEnd('】') + '】';
            int p = 134;
            int c = SMS.Util.SMSSplit.GetSplitNumber(_content, _signature, out p);
            int len = (_signature + _content).Length;

            string result = "{\"SMSCount\":" + c + ",\"ContentLength\":" + len + "}";
            return Content(result);
        }

    }
}
