using GatewayInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GatewayRecieve.Controllers
{
    public class JCHYController : Controller
    {
        //
        // GET: /JCHY/
        /// <summary>
        /// 状态报告
        /// </summary>
        /// <param name="reciver"></param>
        /// <param name="pswd"></param>
        /// <param name="msgid"></param>
        /// <param name="reportTime"></param>
        /// <param name="mobile"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult StatusReport(string reciver, string pswd, string msgid, string reportTime,string mobile,string status)
        {
            var sr=  new StatusResult()
            {
                SerialNumber=msgid,
               Number= mobile
            };


            return Content("OK");
        }

        public ActionResult SMSMO(string reciver,string pswd,string moTime,string mobile,string destcode,string msg)
        {
           // receiver=admin&pswd=12345&moTime=1208212205&mobile=13800210021&destcode=1065751600001&msg=hello
            return Content("OK");
        }

    }
}
