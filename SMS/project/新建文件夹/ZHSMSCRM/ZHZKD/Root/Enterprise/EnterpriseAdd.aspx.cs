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

namespace ZKD.Root.Enterprise
{
    public partial class EnterpriseAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                load();
                ProvinceLoad();
                dd_city.Items.Clear();
                dd_city.Items.Insert(0, new ListItem("--请选择--", "-1"));
            }
        }
        private void load()
        {
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            RPCResult<Model.EnterpriseUser> rr = ZHSMSProxy.GetZKD().GetEnterprise(user.AccountCode);
            if (rr.Success)
            {
                Model.EnterpriseUser a = rr.Value;
                if (a != null)
                {
                    txt_spNumber.Text = a.SPNumber;

                }
            }
            else
            {
                Message.Alert(this, rr.Message, "null");
                return;

            }
            SMSModel.RPCResult<List<SMSModel.Channel>> r = ZHSMSProxy.GetZKD().GetChannels();
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
            else
            {
                Message.Alert(this, r.Message, "null");
                return;

            }
            if (rb_SMSChannel.Items.Count == 1)
            {
                rb_SMSChannel.Items[0].Selected = true;
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
            if (txt_account.Text.Trim() == "-1")
            {
                Message.Alert(this, "已存在该企业帐号", "null");
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
            if (txt_Wei.Text.Trim() == "")
            {
                Message.Alert(this, "请填写企业接入号码尾号", "null");
                return;
            }
            Model.EnterpriseUser user = new Model.EnterpriseUser();
            user.AccountCode = txt_account.Text.Trim();
            user.Audit = SMSModel.AccountAuditType.Manual;
            if (!IsPassword(txt_pass.Text.Trim()))
            {
                Message.Alert(this, "密码必须是以字母开头，长度在6~18之间，只能包含字母、数字和下划线", "null");
                return;
            }
            user.Password = txt_pass.Text.ToString();
            user.Priority = SMSModel.AccountPriorityType.Level0;
            user.RegisterDate = DateTime.Now;
            user.SPNumber = txt_spNumber.Text.Trim() + txt_Wei.Text.Trim();
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
            user.Signature = txt_smsSigure.Text.Trim();
            user.Channel = rb_SMSChannel.SelectedValue;
            user.Filter = (ushort)SMSModel.FilterType.Replace;
            user.Level = (ushort)SMSModel.LevelType.Level2;
            user.StatusReport = (ushort)(SMSModel.StatusReportType)(int.Parse(rb_SMSReportType.SelectedValue));
            SMSModel.RPCResult<Guid> r = ZHSMSProxy.GetZKD().AddLowerAccount(((Model.EnterpriseUser)Session["Login"]).AccountCode, user);
            if (r.Success)
            {
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
    }
}