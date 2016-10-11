using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using SMSModel;
using System.IO;
using System.Data;


namespace ZKD.Root.Contacts
{
    public partial class PhoneSingle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
                dd_l.Items.Clear();
                DataTable dt = BLL.PhoneAndGroup.GetGroupByAccountCode(user.AccountCode);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dd_l.Items.Add(new ListItem(dt.Rows[i]["TelPhoneGroupName"].ToString(), dt.Rows[i]["GID"].ToString()));
                    }
                }
                dd_l.Items.Remove("0");
                dd_l.Items.Insert(0, new ListItem("--请选择--", "-1"));
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            DataTable dataTabe = (DataTable)Session["dt"];
            bool r = false;
            if (!Page.IsValid)
            {
                return;
            }
            if (txt_phone.Text != "")
            {
                if (!IsNumeric(txt_phone.Text))
                {
                    Message.Alert(this, "请输入正确的手机号码", "null");
                    return;
                }
            }
            if (txt_Email.Text != "")
            {
                if (!IsEmail(txt_Email.Text))
                {
                    Message.Alert(this, "请输入正确的Email", "null");
                    return;
                }
            }
            string GroupID = string.Empty;
            if (dd_l.SelectedValue == "-1")
            {
                DataTable dt = BLL.PhoneAndGroup.GetGroupByTelPhoneGroupNameAndAccountCode("0",user.AccountCode);
                if (dt.Rows.Count > 0)
                {
                    GroupID = dt.Rows[0][0].ToString();
                }
            }
            else
            {
                GroupID = dd_l.SelectedValue;
            }
            r = BLL.PhoneAndGroup.PhoneUpload(user.AccountCode, txt_name.Text, txt_brithday.Text.ToString(), r_sex.SelectedItem.Text, txt_Company.Text, txt_phone.Text, txt_Email.Text, txt_QQ.Text, txt_Postion.Text, txt_webchat.Text, txt_Comweb.Text, GroupID);
            if (r)
            {

                Message.Success(this, "号码添加成功", "null");
            }
            else
            {
                Message.Error(this, "号码添加失败", "null");
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

        private bool IsEmail(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("^(([0-9a-zA-Z]+)|([0-9a-zA-Z]+[_.0-9a-zA-Z-]*))@([a-zA-Z0-9-]+[.])+([a-zA-Z]{2}|net|NET|com|COM|gov|GOV|mil|MIL|org|ORG|edu|EDU|int|INT|name|NAME)$");
            if (reg.IsMatch(str))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
      
    }
}