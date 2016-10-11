using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Keywords
{
    public partial class KeywordImport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "Keywords.Manage.KeywordImport";
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
            try
            {
                if (dd_groups.SelectedIndex == 0)
                {
                    Message.Alert(this, "请选择一个敏感词组", "null");
                    return;
                }
                string path = this.FileUpload1.PostedFile.FileName;
                if (path == "" || path == null)
                {
                    Message.Alert(this, "请选择一个文件，文件格式为txt", "null");
                    return;
                }
                if (System.IO.Path.GetExtension(FileUpload1.FileName).ToLower() != ".txt")
                {
                    Message.Alert(this, "文件格式为txt", "null");
                    return;
                }
                path = Server.MapPath("~/Temp/") + FileUpload1.FileName;
                FileUpload1.PostedFile.SaveAs(path);
                DataTable dt = CreateTable();
                using (StreamReader sr = new StreamReader(path))
                {
                    string str;
                    while ((str = sr.ReadLine()) != null)
                    {

                        if (str != "")
                        {
                            DataRow dr = dt.NewRow();
                            string[] arr = str.Split(',');
                            if (arr.Length >= 2)
                            {
                                if (arr[0] != "")
                                {
                                    dr["keyword"] = arr[0];
                                    dr["other"] = arr[1];
                                    dt.Rows.Add(dr);
                                }
                            }
                            else if (arr.Length == 1)
                            {
                                if (arr[0] != "")
                                {
                                    dr["keyword"] = arr[0];
                                    dr["other"] = "";
                                    dt.Rows.Add(dr);
                                }
                            }
                        }
                    }
                }
                File.Delete(path);
                if (dt.Rows.Count == 0)
                {
                    Message.Alert(this, "文件是空的", "null");
                    return;
                }

                List<SMSModel.Keywords> list = new List<SMSModel.Keywords>();
                foreach (DataRow row in dt.Rows)
                {
                    SMSModel.Keywords keyword = new SMSModel.Keywords();
                    keyword.Enable = rb_status.SelectedValue == "1" ? true : false;
                    keyword.KeyGroup = dd_groups.SelectedValue;
                    keyword.KeywordsType = rb_types.SelectedValue == "-1" ? "" : rb_types.SelectedValue;
                    keyword.ReplaceKeywords = row["other"].ToString();
                    keyword.Words = row["keyword"].ToString();
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
            catch
            {
                Message.Alert(this, "操作失败", "null");
            }
        }

        private DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("keyword", Type.GetType("System.String"));
            table.Columns.Add("other", Type.GetType("System.String"));
            return table;
        }
    }
}