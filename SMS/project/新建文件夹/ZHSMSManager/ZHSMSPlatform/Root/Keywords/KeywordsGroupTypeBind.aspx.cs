using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Keywords
{
    public partial class KeywordsGroupTypeBind : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //BLL.Login.IsLogin();
            //Model.SysAccount account = (Model.SysAccount)Session["Login"];
            //if (account.UserCode != "admin")
            //{
            //    string permissionValue = "Keywords.Manage.GroupTypeBind";
            //    bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
            //    if (!ok)
            //    {
            //        Message.Alert(this, "无权限", "null");
            //        btnSubmit.Visible = false;
            //        return;
            //    }
            //}
            //if (!IsPostBack)
            //{
            //    load();
            //}
        }
        private void load()
        {
            //SMSModel.RPCResult<Dictionary<string, string>> rt = ZHSMSProxy.GetZHSMSPlatService().GetKeyGroups();
            //if (rt.Success)
            //{
            //    foreach (var v in rt.Value)
            //    {
            //        ListItem li = new ListItem();
            //        li.Value = v.Key;
            //        li.Text = v.Key;
            //        dd_groups.Items.Add(li);
            //    }
            //}
            //dd_groups.Items.Insert(0, new ListItem("--请选择--", "-1"));
            //SMSModel.RPCResult<Dictionary<string, string>> r = ZHSMSProxy.GetZHSMSPlatService().GetKeywordsTypes();
            //if (r.Success)
            //{
            //    foreach (var v in r.Value)
            //    {
            //        ListItem li = new ListItem();
            //        li.Value = v.Key;
            //        li.Text = v.Key;
            //        cb_types.Items.Add(li);
            //    }
            //}
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //if (dd_groups.SelectedIndex == 0)
            //{
            //    Message.Alert(this, "请选择敏感词组", "null");
            //    return;
            //}
            //List<string> list = new List<string>();
            //for (int i = 0; i < cb_types.Items.Count; i++)
            //{
            //    if (cb_types.Items[i].Selected == true)
            //    {
            //        list.Add(cb_types.Items[i].Value);
            //    }
            //}
            //if (list.Count == 0)
            //{
            //    Message.Alert(this, "请选择敏感词类型", "null");
            //    return;
            //}
            //SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().AddKeywordsGroupTypeBind(dd_groups.SelectedValue, list);
            //if (r.Success)
            //{
            //    Message.Success(this, "添加成功", "null");
            //}
            //else
            //{
            //    Message.Alert(this, r.Message, "null");
            //}
        }

        protected void dd_groups_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (dd_groups.SelectedIndex == 0) return;
            //string keyGroup = dd_groups.SelectedValue;
            //SMSModel.RPCResult<List<string>> rt = ZHSMSProxy.GetZHSMSPlatService().GetKeywordsTypesByGroup(keyGroup);
            //if (rt.Success)
            //{
            //    foreach (ListItem li in cb_types.Items)
            //    {
            //        if (rt.Value.Contains(li.Value)) li.Selected = true;
            //    }
            //}
        }
    }
}