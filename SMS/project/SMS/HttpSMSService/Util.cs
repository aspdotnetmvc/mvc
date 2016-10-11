using ISMPModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace HttpSMSService
{
    public class Util
    {
        public static string SMSHost = ConfigurationManager.AppSettings["SMSHost"];
        public static string ISMPHost = ConfigurationManager.AppSettings["ISMPHost"];
        public static string CommercialSuffix = ConfigurationManager.AppSettings["CommercialSuffix"];
        public static string SMSProductId = ConfigurationManager.AppSettings["SMSProductId"];
        public static string SMSProductName = ConfigurationManager.AppSettings["SMSProductName"];


        /// <summary>
        /// 给审核人发送一条消息
        /// </summary>
        /// <param name="audit"></param>
        public static void SendToDoToSMSAuditor(SMS.Model.SMSMessage sms)
        {
            try
            {
                string urlSMSEdit = SMSHost + "/Platform/SendSMSAudit";
                SystemToDoList std = new SystemToDoList();
                std.CreateTime = DateTime.Now;
                std.Id = System.Guid.NewGuid().ToString();
                std.IsDealed = false;
                std.PageId = "SMSSendSMSAudit" + Util.CommercialSuffix;
                std.PageTitle = Util.SMSProductName + "审核";
                std.Title = "您的" + Util.SMSProductName + "审核中有新的【" + Util.SMSProductName + "审核】申请，请及时审核！";
                std.Url = "/Home/Transfer?url=" + urlSMSEdit + "&urlParam=/Common/GetBaseParam";
                std.ProjectId = Util.SMSProductId;
                std.TableName = "";
                std.RowId = sms.ID;
                std.ToDoType = "SMSSendSMSAudit" + Util.CommercialSuffix;

                string Param = JsonSerialize.Serialize<SystemToDoList>(std);

                string url = Util.ISMPHost + "/CallBack/SendToDoToOneGroupByPermission";
                IDictionary<string, string> pdic = new Dictionary<string, string>();
                pdic.Add("Param", Param);
                pdic.Add("Identifier", std.ToDoType);
                Post(url, pdic);

            }
            catch (Exception ex)
            {

            }

        }
        /// <summary>
        /// Post web 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="requestEncoding"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string Post(string url, IDictionary<string, string> parameters, Encoding requestEncoding = null, CookieCollection cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                requestEncoding = Encoding.UTF8;
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";


            request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143  Safari/537.36";

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                var postData = string.Join("&", parameters.Select(
                p => string.Format("{0}={1}", p.Key, System.Web.HttpUtility.UrlEncode(p.Value))).ToArray());
                byte[] data = requestEncoding.GetBytes(postData);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            var response = request.GetResponse();

            StreamReader sr = new StreamReader(response.GetResponseStream(), requestEncoding);
            string ret = sr.ReadToEnd();
            sr.Close();
            response.Close();
            return ret;
        }
    }
}