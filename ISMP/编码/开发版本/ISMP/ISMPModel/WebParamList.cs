using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISMPModel
{
    /// <summary>
    /// 扩展Paramlist  从Request 中自动取值
    /// </summary>
    [Serializable]
    public class WebParamList : ParamList
    {
        public WebParamList()
            : base()
        {

        }
        public WebParamList(HttpRequestBase request)
        {
            addFromRequest(request);
        }
        public void addFromRequest(HttpRequestBase request)
        {
            //添加用户Ip
            this.add("CURRENTIP", getIp(request));
            //添加QueryString

            foreach (string key in request.QueryString.Keys)
            {
                this.add(key, request.QueryString[key]);
            }

            foreach (string key in request.Form.Keys)
            {
                this.add(key, request.Form[key]);
            }
            //setIntField("page","rows", "start");

        }
        private static string getIp(HttpRequestBase request)
        {
            string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(ip))
            {
                ip = request.UserHostAddress;
            }
            if (string.IsNullOrEmpty(ip))
            {
                ip = "0.0.0.0";
            }
            return ip;
        }
    }
}