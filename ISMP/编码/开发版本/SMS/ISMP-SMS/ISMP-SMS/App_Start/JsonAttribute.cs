using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISMP_SMS
{
    public class JsonAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 兼容IE下无法接受JSON问题
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            string contentType = filterContext.HttpContext.Response.ContentType;
            if (contentType == "application/json")
                filterContext.HttpContext.Response.ContentType = "text/html";


            base.OnResultExecuted(filterContext);
        }
    }
}
