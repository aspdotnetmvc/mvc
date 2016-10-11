using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace ZKD.Root.Contacts
{
    public partial class GroupAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            if (txt_Group.Text == "")
            {
                Message.Alert(this, "请输入通讯录组名称", "null");
                return;
            }
            DataTable dt = BLL.PhoneAndGroup.GetGroupByTelPhoneGroupNameAndAccountCode(txt_Group.Text.Trim().ToString(), user.AccountCode);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    Message.Alert(this, "通讯录组名称已存在，请重新输入", "null");
                    return;
                }
            }
            if (txt_Group.Text == "")
            {
                Message.Alert(this, "请输入通讯录组名称", "null");
                return;
            }
            if (txt_Group.Text == "0")
            {
                Message.Alert(this, "请修改通讯录组名称", "null");
                return;
            }

            bool r = BLL.PhoneAndGroup.GroupAdd(user.AccountCode, txt_Group.Text, txt_Mark.Text);
            if (r)
            {
                Message.Success(this, "添加成功", "null");
            }
            else
            {
                Message.Error(this, "添加失败", "null");
            }
        }
    }
}