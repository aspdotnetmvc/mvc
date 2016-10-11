using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace MessageTools
{
    /// <summary>
    /// 读取配置文件，给特定的人发送告警邮件
    /// </summary>
    public class MailHelper
    {
        /// <summary>
        /// 短信服务器
        /// </summary>
        public string Server { get { return ConfigurationManager.AppSettings["MailServer"]; } }
        /// <summary>
        /// 端口
        /// </summary>
        public string port { get { return ConfigurationManager.AppSettings["MailPort"]; } }
        /// <summary>
        /// 发件邮箱
        /// </summary>
        public string FromAddress { get { return ConfigurationManager.AppSettings["MailFromAddress"]; } }
        /// <summary>
        /// 发件人 
        /// </summary>
        public string Name { get { return ConfigurationManager.AppSettings["MailDisplayName"]; } }
        /// <summary>
        /// 发件人用户名
        /// </summary>
        public string UserName { get { return ConfigurationManager.AppSettings["MailUserName"]; } }
        /// <summary>
        /// 发件人密码
        /// </summary>
        public string Password { get { return ConfigurationManager.AppSettings["MailPassword"]; } }
        /// <summary>
        /// 收件人地址
        /// </summary>
        public string Address { get { return ConfigurationManager.AppSettings["MailAddress"]; } }
        /// <summary>
        /// 
        /// </summary>
        public bool EnableSSL { get { return bool.Parse(ConfigurationManager.AppSettings["MailSSL"]); } }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title">邮件主题</param>
        /// <param name="content">内容</param>
        public void SendMail(string title, string content) //发送失败时抛出异常
        {
            try
            {
                //配置邮件连接
                SmtpClient client = new SmtpClient();

                client = new SmtpClient();
                client.Host = this.Server;
                client.Port = Convert.ToInt32(this.port);

                //发送的邮箱名和密码 
                if (client.Host != "127.0.0.1")
                {
                    System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential(this.UserName, this.Password);
                    client.Credentials = basicAuthenticationInfo;
                }
                else
                {
                    client.UseDefaultCredentials = false;
                }
                client.EnableSsl = this.EnableSSL;

                //配置邮件内容
                MailMessage message = new MailMessage();

                message.To.Clear();
                message.To.Add(Address);

                message.From = new MailAddress(this.FromAddress, this.Name, System.Text.Encoding.UTF8);

                message.Subject = title;//邮件标题 
                message.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码 
                message.Body = content;//邮件内容 
                message.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码 

                new System.Threading.Tasks.Task(() => { client.Send(message); }).Start();
            }
            catch (Exception ex)
            {
                //发送邮件失败
                throw;
            }
        }


    }
}
