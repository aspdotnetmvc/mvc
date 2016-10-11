using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace ZHSMSPlatform.Root.Enterprise
{
    public partial class EnterpriseAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "Enterprise.Account.Add";
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    btnSubmit.Visible = false;
                    return;
                }
            }
            if (!IsPostBack)
            {
                ProvinceLoad();
                dd_city.Items.Clear();
                dd_city.Items.Insert(0, new ListItem("--请选择--", "-1"));
                SMSModel.RPCResult<List<SMSModel.Channel>> r = ZHSMSProxy.GetZHSMSPlatService().GetChannels();
                if (r.Success)
                {
                    int i = 0;
                    foreach (var v in r.Value)
                    {

                        ListItem li = new ListItem();
                        li.Value = v.ChannelID;
                        li.Text = v.ChannelName;
                        rb_SMSChannel.Items.Add(li);
                        if (i == 0) li.Selected = true;
                        i++;
                    }
                }
                if (rb_SMSChannel.Items.Count == 1)
                {
                    rb_SMSChannel.Items[0].Selected = true;
                }
            }
        }

        private void ProvinceLoad()
        {
            dd_province.Items.Clear();
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
            if (txt_account.Text.Trim() == "-1")
            {
                Message.Alert(this, "此企业帐号已存在", "null");
                return;
            }
            if (dd_city.SelectedIndex == 0 || dd_province.SelectedIndex == 0)
            {
                Message.Alert(this, "请选择企业所在的地区", "null");
                return;
            }
            if (txt_spNumber.Text.Trim() == "")
            {
                Message.Alert(this, "请填写企业接入号码", "null");
                return;
            }
            Model.EnterpriseUser user = new Model.EnterpriseUser();
            user.AccountCode = txt_account.Text.Trim();
            user.Audit = (SMSModel.AccountAuditType)(int.Parse(rb_accountAudit.SelectedValue));
            user.Enabled = rb_accountEnable.SelectedValue == "1" ? true : false;
            if (!IsPassword(txt_pass.Text.Trim()))
            {
                Message.Alert(this, "密码必须是以字母开头，长度在6~18之间，只能包含字母、数字和下划线", "null");
                return;
            }
            user.Password = txt_pass.Text.Trim();
            user.Priority = (SMSModel.AccountPriorityType)(int.Parse(rb_accountLevel.SelectedValue));
            user.RegisterDate = DateTime.Now;
            if (!System.Text.RegularExpressions.Regex.IsMatch(txt_spNumber.Text.Trim(), @"^[0-9]+$"))
            {
                Message.Alert(this, "企业接入号码是应为数字", "null");
                return;
            }
            user.SPNumber = txt_spNumber.Text.Trim();
            user.Name = txt_name.Text.Trim();
            if (!IsNumeric(txt_phone.Text.Trim()))
            {
                Message.Alert(this, "手机号码输入不正确", "null");
                return;
            }
            user.Phone = txt_phone.Text.Trim();
            user.Address = txt_address.Text;
            user.Contact = txt_contact.Text;
            user.City = dd_city.SelectedItem.Value;
            user.Province = dd_province.SelectedItem.Value;
            user.IsAgent = rb_IsAgent.SelectedValue == "1" ? true : false;
            user.ParentAccountCode = "-1";
            user.IsOpen = rb_isOpen.SelectedValue == "1" ? true : false;
            //if (txt_smsSigure.Text.Trim() == "")
            //{
            //    Message.Alert(this, "短信签名不能为空", "null");
            //    return;
            //}
            //else
            {
                user.Signature = txt_smsSigure.Text.Trim();
            }
            user.Channel = rb_SMSChannel.SelectedValue == "-1-" ? "" : rb_SMSChannel.SelectedValue;
            user.Filter = ushort.Parse(rb_SMSFilter.SelectedValue);
            user.Level = ushort.Parse(rb_SMSLevel.SelectedValue);
            user.StatusReport = ushort.Parse(rb_SMSReportType.SelectedValue);

            SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().AddEnterprise(user);
            if (r.Success)
            {
                if (user.IsAgent)
                {
                    //添加代理商账号时，自动指定创建人账号为渠道经理
                    Model.EnterpriseManage em = new Model.EnterpriseManage();
                    em.EnterpriseCode = user.AccountCode;
                    Model.SysAccount account = (Model.SysAccount)Session["Login"];
                    em.ChannelManager = account.UserCode;
                    em.CSManager = "";
                    em.Reserve = "";
                    BLL.EnterpriseManage.Add(em);
                }
                Message.Success(this, "操作成功", "null");
            }
            else
            {
                Message.Alert(this, r.Message, "null");
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

        private bool IsPassword(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z]\w{5,17}$");
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