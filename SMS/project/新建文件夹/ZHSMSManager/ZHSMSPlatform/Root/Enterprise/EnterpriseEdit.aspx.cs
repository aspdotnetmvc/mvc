using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace ZHSMSPlatform.Root.Enterprise
{
    public partial class EnterpriseEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = Request.QueryString["type"] + "Enterprise.Manage.Edit";
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
            if (!IsPostBack)
            {
                ProvinceLoad();
                SMSModel.RPCResult<List<SMSModel.Channel>> r = ZHSMSProxy.GetZHSMSPlatService().GetChannels();
                if (r.Success)
                {
                    foreach (var v in r.Value)
                    {
                        ListItem li = new ListItem();
                        li.Value = v.ChannelID;
                        li.Text = v.ChannelName;
                        rb_SMSChannel.Items.Add(li);
                    }
                }
                load();
            }
        }
        void load()
        {
            string code = Request.QueryString["AccountID"];
            SMSModel.RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZHSMSPlatService().GetEnterprise(code);
            if (r.Success)
            {
                Model.EnterpriseUser user = r.Value;// BLL.EnterpriseUser.GetEnerprise(code);
                lbl_account.Text = user.AccountCode;
                txt_spNumber.Text = user.SPNumber;
                rb_accountAudit.Items.FindByValue(((ushort)user.Audit).ToString()).Selected = true;
                //rb_accountEnable.Items.FindByValue(user.Enabled == true ? "1" : "0").Selected = true;
                rb_accountLevel.Items.FindByValue(((ushort)user.Priority).ToString()).Selected = true;

                txt_contact.Text = user.Contact;
                txt_name.Text = user.Name;
                txt_phone.Text = user.Phone;
                txt_address.Text = user.Address;
                dd_province.Items.FindByValue(user.Province).Selected = true;
                CityLoad(user.Province);
                dd_city.Items.FindByValue(user.City).Selected = true;
                user.Channel = user.Channel == "" ? "-1-" : user.Channel;
                if (rb_SMSChannel.Items.FindByValue(user.Channel) != null)
                {
                    rb_SMSChannel.Items.FindByValue((user.Channel).ToString()).Selected = true;
                }
                else
                {
                    rb_SMSChannel.Items.FindByValue(("-1-").ToString()).Selected = true;
                }
                rb_isOpen.Items.FindByValue(user.IsOpen ? "1" : "0").Selected = true;

                rb_SMSFilter.Items.FindByValue((user.Filter).ToString()).Selected = true;
                rb_SMSLevel.Items.FindByValue((user.Level).ToString()).Selected = true;
                rb_SMSReportType.Items.FindByValue((user.StatusReport).ToString()).Selected = true;

                if (user.Signature != null)
                {
                    txt_smsSigure.Text = user.Signature;
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
        protected void dd_province_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dd_province.SelectedIndex == 0)
            {
                dd_city.Items.Clear();
                dd_city.Items.Insert(0, new ListItem("--请选择--", "-1"));
            }
            else
            {
                CityLoad(dd_province.SelectedValue);
            }
        }

        protected void btn_infoSave_Click(object sender, EventArgs e)
        {
            //资料
            if (dd_city.SelectedIndex == 0 || dd_province.SelectedIndex == 0)
            {
                Message.Alert(this, "请选择地区信息", "null");
                return;
            }
            if (txt_name.Text.Trim() == "")
            {
                Message.Alert(this, "请填写企业名字", "null");
                return;
            }
            if (txt_contact.Text.Trim() == "")
            {
                Message.Alert(this, "请填写企业联系人", "null");
                return;
            }
            if (txt_phone.Text.Trim() == "")
            {
                Message.Alert(this, "请填写手机号码", "null");
                return;
            }
            if (!IsNumeric(txt_phone.Text.Trim()))
            {
                Message.Alert(this, "手机号码输入不正确", "null");
                return;
            }
            string code = Request.QueryString["AccountID"];
            SMSModel.RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZHSMSPlatService().GetEnterprise(code);
            if (r.Success)
            {
                Model.EnterpriseUser user = r.Value;
                user.Name = txt_name.Text.Trim();
                user.Phone = txt_phone.Text.Trim();
                user.Address = txt_address.Text;
                user.Contact = txt_contact.Text;
                user.City = dd_city.SelectedItem.Value;
                user.Province = dd_province.SelectedItem.Value;
                SMSModel.RPCResult ok = ZHSMSProxy.GetZHSMSPlatService().UpdateAccontInfo(user);// BLL.EnterpriseUser.UpdateAccontInfo(user);
                if (ok.Success)
                {
                    Message.Success(this, "操作成功", "null");
                }
                else
                {
                    Message.Alert(this, ok.Message, "null");
                }
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
        }

        //protected void btn_BasicSave_Click(object sender, EventArgs e)
        //{
        //    //企业设置
        //    if (txt_spNumber.Text.Trim() == "")
        //    {
        //        Message.Alert(this, "请填写企业接入号码", "null");
        //        return;
        //    }
        //    string code = Request.QueryString["AccountID"];
        //    SMSModel.RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZHSMSPlatService().GetEnterprise(code);
        //    if (r.Success)
        //    {
        //        Model.EnterpriseUser user = r.Value;
        //        user.Audit = (SMSModel.AccountAuditType)(int.Parse(rb_accountAudit.SelectedValue));
        //        //user.Enabled = rb_accountEnable.SelectedValue == "1" ? true : false;
        //        user.Priority = (SMSModel.AccountPriorityType)(int.Parse(rb_accountLevel.SelectedValue));
        //        if (!IsNum(txt_spNumber.Text.Trim()))
        //        {
        //            Message.Alert(this, "企业接入号码是非零正整数", "null");
        //            return;
        //        }
        //        user.SPNumber = txt_spNumber.Text.Trim();
        //        user.IsOpen = rb_isOpen.SelectedValue == "1" ? true : false;
        //        SMSModel.RPCResult ok = ZHSMSProxy.GetZHSMSPlatService().UpdateAccountSetting(user);// BLL.EnterpriseUser.UpdateAccountSetting(user);
        //        if (ok.Success)
        //        {
        //            Message.Success(this, "操作成功", "null");
        //        }
        //        else
        //        {
        //            Message.Alert(this, ok.Message, "null");
        //        }
        //    }
        //    else
        //    {
        //        Message.Alert(this, r.Message, "null");
        //    }
        //}

        protected void btn_SMSSave_Click(object sender, EventArgs e)
        {
            //短信设置
            //if (txt_smsSigure.Text.Trim() == "")
            //{
            //    Message.Alert(this, "请填写企业签名", "null");
            //    return;
            //}
            if (txt_spNumber.Text.Trim() == "")
            {
                Message.Alert(this, "请填写企业接入号码", "null");
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txt_spNumber.Text.Trim(), @"^[0-9]+$"))
            {
                Message.Alert(this, "企业接入号码是应为数字", "null");
                return;
            }
            string code = Request.QueryString["AccountID"];
            SMSModel.RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZHSMSPlatService().GetEnterprise(code);
            if (r.Success)
            {
                Model.EnterpriseUser user = r.Value;
                if (user != null)
                {
                    SMSModel.SMS sms = new SMSModel.SMS();
                    sms.Account = user.AccountCode;
                    sms.Channel = rb_SMSChannel.SelectedValue == "-1-" ? "" : rb_SMSChannel.SelectedValue;
                    sms.Filter = (SMSModel.FilterType)(int.Parse(rb_SMSFilter.SelectedValue));
                    sms.Level = (SMSModel.LevelType)(int.Parse(rb_SMSLevel.SelectedValue));
                    sms.StatusReport = (SMSModel.StatusReportType)(int.Parse(rb_SMSReportType.SelectedValue));
                    sms.Signature = txt_smsSigure.Text.Trim();
                    SMSModel.RPCResult rs = ZHSMSProxy.GetZHSMSPlatService().UpdateEnterpriseSMS(sms);
                }
            }
            SMSModel.RPCResult<Model.EnterpriseUser> rr = ZHSMSProxy.GetZHSMSPlatService().GetEnterprise(code);
            if (rr.Success)
            {
                Model.EnterpriseUser user = rr.Value;
                user.Audit = (SMSModel.AccountAuditType)(int.Parse(rb_accountAudit.SelectedValue));
                //user.Enabled = rb_accountEnable.SelectedValue == "1" ? true : false;
                user.Priority = (SMSModel.AccountPriorityType)(int.Parse(rb_accountLevel.SelectedValue));
                //if (!IsNum(txt_spNumber.Text.Trim()))
                //{
                //    Message.Alert(this, "企业接入号码是非零正整数", "null");
                //    return;
                //}
                user.SPNumber = txt_spNumber.Text.Trim();
                user.IsOpen = rb_isOpen.SelectedValue == "1" ? true : false;
                SMSModel.RPCResult ok = ZHSMSProxy.GetZHSMSPlatService().UpdateAccountSetting(user);// BLL.EnterpriseUser.UpdateAccountSetting(user);
                if (ok.Success)
                {
                    Message.Success(this, "操作成功", "null");
                }
                else
                {
                    Message.Alert(this, ok.Message, "null");
                }
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }

        }
        private bool IsNum(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\+?[1-9][0-9]*$");
            if (reg1.IsMatch(str))
            {
                //数字
                return true;
            }
            else
            {
                //非数字
                return false;
            }
        }
        private bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^0?(13[0-9]|15[012356789]|18[0-9]|14[57])[0-9]{8}$");
            if (reg1.IsMatch(str))
            {
                //数字
                return true;
            }
            else
            {
                //非数字
                return false;
            }
        }
    }
}