using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Keywords
{
    public partial class KeywordAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "Keywords.Manage.KeywordAdd";
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
                load();
            }

        }
        private void load()
        {
            dd_groups.Items.Clear();
            rb_types.Items.Clear();
            SMSModel.RPCResult<Dictionary<string, string>> r = ZHSMSProxy.GetZHSMSPlatService().GetKeyGroups();
            if (r.Success)
            {
                foreach (var v in r.Value)
                {
                    ListItem li = new ListItem();
                    li.Value = v.Key;
                    li.Text = v.Key;
                    dd_groups.Items.Add(li);
                }
            }
            SMSModel.RPCResult<Dictionary<string, string>> rt = ZHSMSProxy.GetZHSMSPlatService().GetKeywordsTypes();
            if (rt.Success)
            {
                int i = 0;
                foreach (var v in rt.Value)
                {
                    ListItem li = new ListItem();
                    li.Value = v.Key;
                    li.Text = v.Key;
                    rb_types.Items.Add(li);
                    if (i == 0) li.Selected = true;
                    i++;
                }
            }
            dd_groups.Items.Insert(0, new ListItem("--请选择--", "-1"));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //依据词组
            if (dd_groups.SelectedIndex == 0)
            {
                Message.Alert(this, "请选择一个词组", "null");
                return;
            }
            List<SMSModel.Keywords> list = new List<SMSModel.Keywords>();
            string keywords = txt_keywords.Text.Trim();
            foreach (string s in keywords.Split(','))
            {
                SMSModel.Keywords keyword = new SMSModel.Keywords();
                keyword.Enable = rb_status.SelectedValue == "1" ? true : false;
                keyword.KeyGroup = dd_groups.SelectedValue;
                keyword.KeywordsType = rb_types.SelectedValue == "-1" ? "" : rb_types.SelectedValue;
                keyword.ReplaceKeywords = txt_other.Text.Trim();
                keyword.Words = s;
                if (!string.IsNullOrEmpty(keyword.Words))
                {
                    list.Add(keyword);
                }
            }
            if (list.Count == 0)
            {
                Message.Alert(this, "请添加敏感词", "null");
                return;
            }
            SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().AddKeywords(dd_groups.SelectedValue, list);
            if (r.Success)
            {
                load();
                Message.Success(this, "添加成功", "null");
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
        }
    }
}