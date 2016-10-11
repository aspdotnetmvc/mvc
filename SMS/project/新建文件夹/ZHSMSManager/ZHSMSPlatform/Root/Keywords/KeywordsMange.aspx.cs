using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Keywords
{
    public partial class KeywordsMange : System.Web.UI.Page
    {
        private const string enabled = "Keywords.Manage.Enable";
        private const string del = "Keywords.Manage.Del";
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            this.ViewState["enabled"] = true;
            this.ViewState["del"] = true;
            if (account.UserCode != "admin")
            {
                List<string> permissions = BLL.Permission.GetPermissionByAccount(account.UserCode);
                if (!permissions.Contains(enabled))
                {
                    this.ViewState["enabled"] = false;
                }
                if (!permissions.Contains(del))
                {
                    this.ViewState["del"] = false;
                }
            }
            if (!IsPostBack)
            {
                load();
                pageBottom(0, 0);
            }
        }
        private void load()
        {
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
                foreach (var v in rt.Value)
                {
                    ListItem li = new ListItem();
                    li.Value = v.Key;
                    li.Text = v.Key;
                    dd_types.Items.Add(li);
                }
            }
            dd_groups.Items.Insert(0, new ListItem("--请选择--", "-1"));
            dd_types.Items.Insert(0, new ListItem("--请选择--", "-1"));
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView1.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                ((LinkButton)e.Row.Cells[7].Controls[0]).Attributes.Add("onclick", "return confirm('确认删除吗？');");
                string status = DataBinder.Eval(e.Row.DataItem, "Enable").ToString();
                switch (status)
                {
                    case "启用":
                        (e.Row.Cells[7].FindControl("btn_start") as Button).Enabled = false;
                        break;
                    case "禁用":
                        (e.Row.Cells[7].FindControl("btn_stop") as Button).Enabled = false;
                        break;
                }
                if ((bool)this.ViewState["enabled"] == false)
                {
                    e.Row.Cells[6].Text = "无权限";
                }
                if ((bool)this.ViewState["del"] == false)
                {
                    e.Row.Cells[7].Text = "无权限";
                }
            }
        }

        public int CurrentPage
        {
            get
            {
                if (ViewState["CurrentPage"] == null)
                    return 0;
                else
                    return (int)ViewState["CurrentPage"];
            }
            set
            {
                ViewState["CurrentPage"] = value;
            }
        }
        protected void PageDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CurrentPage = PageDropDownList.SelectedIndex;
            search(this.CurrentPage);
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if ((bool)this.ViewState["del"] == false)
            {
                Message.Alert(this, "无权限", "null");
                return;
            }
            string group = this.GridView1.DataKeys[e.RowIndex][0].ToString();
            string keywords = this.GridView1.DataKeys[e.RowIndex][1].ToString();

            SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().DelKeywords(group, new List<string> { keywords });
            if (r.Success)
            {
                search(this.CurrentPage);
                Message.Success(this, "删除成功", "null");
            }
            else
            {
                Message.Error(this, r.Message, "null");
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string[] arr = e.CommandArgument.ToString().Split(',');//敏感词，组
            if (e.CommandName == "start")
            {
                ZHSMSProxy.GetZHSMSPlatService().KeywordsEnabled(arr[1], arr[0], true);
                search(this.CurrentPage);
            }
            else if (e.CommandName == "stop")
            {
                ZHSMSProxy.GetZHSMSPlatService().KeywordsEnabled(arr[1], arr[0], false);
                search(this.CurrentPage);
            }
        }

        protected void btnFindByGroup_Click(object sender, EventArgs e)
        {
            //根据组
            this.ViewState["condition"] = "group";
            search(0);
        }

        protected void btnFindType_Click(object sender, EventArgs e)
        {
            //根据类型
            this.ViewState["condition"] = "type";
            search(0);
        }

        protected void btnFindByKeyword_Click(object sender, EventArgs e)
        {
            //根据词
            this.ViewState["condition"] = "words";
            search(0);
        }

        private void search(int pageIndex)
        {
            DataTable dt = CreateTable();
            string ss = this.ViewState["condition"].ToString();
            SMSModel.RPCResult<List<SMSModel.Keywords>> r = new SMSModel.RPCResult<List<SMSModel.Keywords>>(false, null, "");
            if (ss.Equals("group"))
            {
                if (dd_groups.SelectedIndex == 0)
                {
                    Message.Alert(this, "请选择一个词组", "null");
                    return;
                }
                r = ZHSMSProxy.GetZHSMSPlatService().GetKeywords(dd_groups.SelectedValue);

            }
            if (ss.Equals("type"))
            {
                if (dd_types.SelectedIndex == 0)
                {
                    Message.Alert(this, "请选择一个类型", "null");
                    return;
                }
                r = ZHSMSProxy.GetZHSMSPlatService().GetKeywordsByType(dd_types.SelectedValue);
            }
            if (ss.Equals("words"))
            {
                r = ZHSMSProxy.GetZHSMSPlatService().GetKeywordsByKeyword(txt_keywords.Text.Trim());
            }
            if (r.Success)
            {
                List<SMSModel.Keywords> list = r.Value;
                pageBottom(list.Count, pageIndex);
                int startIndex = pageIndex * GridView1.PageSize;
                if (startIndex != 0)
                {
                    list = list.Skip(startIndex).ToList();
                }
                list = list.Take(GridView1.PageSize).ToList();
                foreach (var s in list)
                {
                    DataRow dr = dt.NewRow();
                    dr["Group"] = s.KeyGroup;
                    dr["Keywords"] = s.Words;
                    dr["KeywordType"] = s.KeywordsType;
                    dr["Enable"] = s.Enable == true ? "启用" : "禁用";
                    dr["Replace"] = s.ReplaceKeywords;
                    dt.Rows.Add(dr);
                }

            }
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("Group", Type.GetType("System.String"));
            table.Columns.Add("Keywords", Type.GetType("System.String"));
            table.Columns.Add("KeywordType", Type.GetType("System.String"));
            table.Columns.Add("Enable", Type.GetType("System.String"));
            table.Columns.Add("Replace", Type.GetType("System.String"));
            return table;
        }

        protected void linkBtnFirst_Click(object sender, EventArgs e)
        {
            //首页
            this.CurrentPage = 0;
            search(this.CurrentPage);
        }

        protected void linkBtnPrev_Click(object sender, EventArgs e)
        {
            //上一页
            this.CurrentPage--;
            search(this.CurrentPage);
        }

        protected void linkBtnNext_Click(object sender, EventArgs e)
        {
            //下一页
            this.CurrentPage++;
            search(this.CurrentPage);
        }

        protected void linkBtnLast_Click(object sender, EventArgs e)
        {
            //尾页
            this.CurrentPage = PageDropDownList.Items.Count - 1;
            search(this.CurrentPage);
        }

        private void pageBottom(int count, int pageIndex)
        {
            div_page.Visible = true;
            if (count == 0 && pageIndex == 0) div_page.Visible = false;
            try
            {
                linkBtnFirst.Enabled = true;
                linkBtnPrev.Enabled = true;
                linkBtnNext.Enabled = true;
                linkBtnLast.Enabled = true;
                int pageCount = count % GridView1.PageSize == 0 ? count / GridView1.PageSize : count / GridView1.PageSize + 1;
                lbRecordCount.Text = count.ToString();
                if (this.CurrentPage == 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                }
                if (this.CurrentPage >= pageCount - 1)
                {
                    linkBtnLast.Enabled = false;
                    linkBtnNext.Enabled = false;
                }
                if (pageCount <= 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                    linkBtnNext.Enabled = false;
                    linkBtnLast.Enabled = false;
                }
                PageDropDownList.Items.Clear();

                for (int i = 0; i < pageCount; i++)
                {
                    int pageNumber = i + 1;
                    ListItem item = new ListItem(pageNumber + "/" + pageCount, pageNumber.ToString());
                    if (i == this.CurrentPage)
                    {
                        item.Selected = true;
                    }
                    PageDropDownList.Items.Add(item);
                }

                int currentPage = this.CurrentPage + 1;
                CurrentPageLabel.Text = currentPage.ToString() +
                  " / " + pageCount;

            }
            catch
            {
            }
        }
    }
}