using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using SMSModel;

namespace ZKD.Root.Agent
{
    public partial class EnterpriseDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                ProvinceLoad();
                load();
            }
        }
        void load()
        {
            string accountCode = Request.QueryString["AccountID"];
            RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZKD().GetEnterprise(accountCode);
            if (r.Success)
            {
                Model.EnterpriseUser account = r.Value;
                if (account != null)
                {
                    lbl_account.Text = account.AccountCode;
                    lbl_accountAudit.Text = GetAccountAuditType(account.Audit);
                    lbl_accountEnable.Text = account.Enabled == true ? "启用" : "禁用";
                    lbl_accountLevel.Text = ((ushort)account.Priority).ToString();
                    lbl_address.Text = account.Address;
                    dd_province.Items.FindByValue(account.Province).Selected = true;
                    CityLoad(account.Province);
                    dd_city.Items.FindByValue(account.City).Selected = true;
                    //lbl_area.Text = ProvinceLoad(account.Province) + " " + CityLoad(account.City);
                    lbl_contact.Text = account.Contact;
                    lbl_name.Text = account.Name;
                    lbl_phone.Text = account.Phone;
                    lbl_spNumber.Text = account.SPNumber;
                    SMSModel.RPCResult<SMSModel.Channel> rt = ZHSMSProxy.GetZKD().GetSMSChannel(account.Channel);
                    if (rt.Success)
                    {
                        lbl_smsChannel.Text = rt.Value.ChannelName == "" ? "所有通道" : rt.Value.ChannelName;
                    }
                    else
                    {
                        lbl_smsChannel.Text = "无";
                    }
                    lbl_smsFilter.Text = GetFilteType((SMSModel.FilterType)account.Filter);
                    lbl_smsLevel.Text = account.Level.ToString();
                    lbl_smsReport.Text = GetReportStatus((SMSModel.StatusReportType)account.StatusReport);

                    dd_city.Enabled = false;
                    dd_province.Enabled = false;
                }
            }
        }

        private void ProvinceLoad()
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Utils.GetMapPath("../../xmlconfig/ChinaArea.xml"));
            xmldoc.InnerXml = Regex.Replace(xmldoc.InnerXml, @"<!--(?s).*?-->", "", RegexOptions.IgnoreCase);
            XmlNodeList xnl = xmldoc.SelectNodes("district/province");
            foreach (XmlNode xn in xnl)
            {
                ListItem li = new ListItem();
                XmlElement xe = (XmlElement)xn;
                li.Value = xe.GetAttribute("code");
                li.Text = xe.GetAttribute("name");
                dd_province.Items.Add(li);
            }
            dd_province.Items.Insert(0, new ListItem("--请选择--", "-1"));
        }

        private void CityLoad(string provinceCode)
        {
            dd_city.Items.Clear();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Utils.GetMapPath("../../xmlconfig/ChinaArea.xml"));
            xmldoc.InnerXml = Regex.Replace(xmldoc.InnerXml, @"<!--(?s).*?-->", "", RegexOptions.IgnoreCase);
            XmlNodeList xnl = xmldoc.SelectNodes("district/province");

            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;
                if (provinceCode == xe.GetAttribute("code"))
                {
                    XmlNodeList cityNodes = xe.ChildNodes;
                    foreach (XmlNode city in cityNodes)
                    {
                        XmlElement xt = (XmlElement)city;
                        ListItem li = new ListItem();
                        li.Value = xt.GetAttribute("code");
                        li.Text = xt.InnerText;
                        dd_city.Items.Add(li);
                    }
                    break;
                }
            }
            dd_city.Items.Insert(0, new ListItem("--请选择--", "-1"));
        }


        string GetAccountAuditType(SMSModel.AccountAuditType type)
        {
            string str = "";
            switch (type)
            {
                case SMSModel.AccountAuditType.Auto:
                    str = "自动审核";
                    break;
                case SMSModel.AccountAuditType.Manual:
                    str = "人工审核";
                    break;
            }
            return str;
        }
        string GetSMSAuditType(SMSModel.AuditType type)
        {
            string str = "";
            switch (type)
            {
                case SMSModel.AuditType.Auto:
                    str = "自动审核";
                    break;
                case SMSModel.AuditType.Manual:
                    str = "人工审核";
                    break;
            }
            return str;
        }
      
        string GetFilteType(SMSModel.FilterType type)
        {
            string str = "";
            switch (type)
            {
                case SMSModel.FilterType.Failure:
                    str = "关键字过滤不通过";
                    break;
                case SMSModel.FilterType.Replace:
                    str = "关键字过滤";
                    break;
                case SMSModel.FilterType.NoOperation:
                    str = "关键字不过滤";
                    break;
            }
            return str;
        }
        string GetReportStatus(SMSModel.StatusReportType type)
        {
            string str = "";
            switch (type)
            {
                case SMSModel.StatusReportType.Disable:
                    str = "不发送";
                    break;
                case SMSModel.StatusReportType.Enabled:
                    str = "发送";
                    break;
                case SMSModel.StatusReportType.Push:
                    str = "推送";
                    break;
            }
            return str;
        }
    }
}