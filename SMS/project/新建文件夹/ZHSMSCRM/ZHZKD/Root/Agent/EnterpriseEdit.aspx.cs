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
    public partial class EnterpriseEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                ProvinceLoad();
                SMSModel.RPCResult<List<SMSModel.Channel>> r = ZHSMSProxy.GetZKD().GetChannels();
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
            RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZKD().GetEnterprise(code);
            if (r.Success)
            {
                Model.EnterpriseUser user = r.Value;
                if (user != null)
                {
                    lbl_account.Text = user.AccountCode;
                    txt_spNumber.Text = user.SPNumber;
                    rb_accountAudit.Items.FindByValue(((ushort)user.Audit).ToString()).Selected = true;
                    rb_accountEnable.Items.FindByValue(user.Enabled == true ? "1" : "0").Selected = true;
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
                    rb_SMSFilter.Items.FindByValue((user.Filter).ToString()).Selected = true;
                    rb_SMSLevel.Items.FindByValue((user.Level).ToString()).Selected = true;
                    rb_SMSReportType.Items.FindByValue((user.StatusReport).ToString()).Selected = true;

                    if (user.Signature != null)
                    {
                        txt_smsSigure.Text = user.Signature;
                    }

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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (dd_city.SelectedIndex == 0 || dd_province.SelectedIndex == 0)
            {
                Message.Alert(this, "请选择地区信息", "null");
                return;
            }
            string code = Request.QueryString["AccountID"];
            RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZKD().GetEnterprise(code);
            if (r.Success)
            {
                Model.EnterpriseUser user = r.Value;
                if (user != null)
                {
                    user.SPNumber = txt_spNumber.Text.Trim();
                    user.Name = txt_name.Text.Trim();
                    user.Phone = txt_phone.Text.Trim();
                    user.Address = txt_address.Text;
                    user.Contact = txt_contact.Text;
                    user.City = dd_city.SelectedItem.Value;
                    user.Province = dd_province.SelectedItem.Value;
                    RPCResult rr = ZHSMSProxy.GetZKD().UpdateEnterprise(user);
                    if (rr.Success)
                    {
                        RPCResult<SMS> rrr = ZHSMSProxy.GetZKD().GetEnterpriseSMS(user.AccountCode);
                        if (rrr.Success)
                        {
                            SMS sms = rrr.Value;
                            if (sms != null)
                            {
                                sms.Account = code;
                                sms.Channel = rb_SMSChannel.SelectedValue;
                                sms.Level = (SMSModel.LevelType)(int.Parse(rb_SMSLevel.SelectedValue));
                                sms.StatusReport = (SMSModel.StatusReportType)(int.Parse(rb_SMSReportType.SelectedValue));
                                sms.Signature = txt_smsSigure.Text.Trim();
                                RPCResult rrrr = ZHSMSProxy.GetZKD().UpdateEnterpriseSMS(sms);
                                if (rrrr.Success)
                                {
                                    Message.Success(this, "修改企业基本资料成功", "null");
                                    return;
                                }
                                else
                                {
                                    Message.Alert(this, rrrr.Message, "null");
                                    return;
                                }
                            }
                            Message.Alert(this, rrr.Message, "null");
                        }
                        else
                        {
                            Message.Alert(this, rr.Message, "null");
                        }
                    }
                    else
                    {
                        Message.Alert(this, r.Message, "null");
                    }
                }
            }
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
                Message.Alert(this, "请填写企业联系电话", "null");
                return;
            }
            string code = Request.QueryString["AccountID"];
            RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZKD().GetEnterprise(code);
            if (r.Success)
            {
                Model.EnterpriseUser user = r.Value;
                if (user != null)
                {
                    user.Name = txt_name.Text.Trim();
                    user.Phone = txt_phone.Text.Trim();
                    user.Address = txt_address.Text;
                    user.Contact = txt_contact.Text;
                    user.City = dd_city.SelectedItem.Value;
                    user.Province = dd_province.SelectedItem.Value;
                    RPCResult rr = ZHSMSProxy.GetZKD().UpdateEnterprise(user);
                    if (rr.Success)
                    {
                        Message.Success(this, "修改企业基本资料成功", "null");
                    }
                    else
                    {
                        Message.Alert(this, rr.Message, "null");
                    }
                }
                else
                {
                    Message.Alert(this, "系统不存在此用户", "null");
                }
            }
        }

        protected void btn_SMSSave_Click(object sender, EventArgs e)
        {
            //短信设置
            if (txt_spNumber.Text.Trim() == "")
            {
                Message.Alert(this, "请填写企业接入号码", "null");
                return;
            }
            if (txt_smsSigure.Text.Trim() == "")
            {
                Message.Alert(this, "请填写企业签名", "null");
                return;
            }
            string code = Request.QueryString["AccountID"];
            RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZKD().GetEnterprise(code);
            if (r.Success)
            {
                Model.EnterpriseUser user = r.Value;
                if (user != null)
                {
                    user.SPNumber = txt_spNumber.Text.Trim();
                    RPCResult rr = ZHSMSProxy.GetZKD().UpdateEnterprise(user);
                    if (rr.Success)
                    {
                        RPCResult<SMS> rrr = ZHSMSProxy.GetZKD().GetEnterpriseSMS(user.AccountCode);
                        if (rrr.Success)
                        {
                            SMS sms = rrr.Value;
                            if (sms != null)
                            {
                                sms.Channel = rb_SMSChannel.SelectedValue == "-1-" ? "" : rb_SMSChannel.SelectedValue;
                                //sms.Filter = (SMSModel.FilterType)(int.Parse(rb_SMSFilter.SelectedValue));
                                sms.Level = (SMSModel.LevelType)(int.Parse(rb_SMSLevel.SelectedValue));
                                sms.StatusReport = (SMSModel.StatusReportType)(int.Parse(rb_SMSReportType.SelectedValue));
                                sms.Signature = txt_smsSigure.Text.Trim();
                                RPCResult rrrr = ZHSMSProxy.GetZKD().UpdateEnterpriseSMS(sms);
                                if (rrrr.Success)
                                {
                                    Message.Success(this, "短信设置成功", "null");
                                    return;
                                }
                                else
                                {
                                    Message.Alert(this, rrrr.Message, "null");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            Message.Alert(this, rrr.Message, "null");
                            return;
                        }
                    }
                    else
                    {
                        Message.Alert(this, rr.Message, "null");
                        return;
                    }
                }
                else
                {
                    Message.Alert(this, r.Message, "null");
                    return;
                }
            }
        }
    }
}