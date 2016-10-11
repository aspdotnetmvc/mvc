using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHCRM.Root.Account
{
    public partial class AccountEnter : System.Web.UI.Page
    {
        private const string ae = "System.Ae.ae";
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                this.ViewState["Permissions"] = BLL.Permission.GetPermissionByAccount(account.UserCode);
            }
            if (!IsPostBack)
            {
                load();
            }
        }
        private void load()
        {
            List<Model.SysAccount> accounts = BLL.SysAccount.GetAccounts();
            DataTable dt = CreateTable();
            if (accounts.Count > 0)
            {
                accounts = accounts.OrderByDescending(c => c.AddTime).ToList();
                foreach (Model.SysAccount account in accounts)
                {
                    if (account.UserCode == "admin") continue;
                    DataRow dr = dt.NewRow();
                    dr["usercode"] = account.UserCode;
                    dr["username"] = account.UserName;
                    dr["addtime"] = account.AddTime;
                    dr["status"] = account.Status == true ? "正常" : "禁用";
                    dt.Rows.Add(dr);
                }
            }
            //dt.DefaultView.Sort = "addtime desc";
            GridView1.DataSource = dt;
            GridView1.DataBind();
            Session["dt"] = dt;

        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Model.SysAccount account = (Model.SysAccount)Session["Login"];
                if (account.UserCode != "admin")
                {
                    List<string> permissions = (List<string>)this.ViewState["Permissions"];
                    if (!permissions.Contains(ae))
                    {
                        e.Row.Cells[5].Text = "无权限";
                    }
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
            GridViewRow pagerRow = GridView1.BottomPagerRow;
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            GridView1.PageIndex = pageList.SelectedIndex;
            this.CurrentPage = pageList.SelectedIndex;
            load();
        }
        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            try
            {
                GridViewRow pagerRow = GridView1.BottomPagerRow;
                LinkButton linkBtnFirst = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnFirst");
                LinkButton linkBtnPrev = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnPrev");
                LinkButton linkBtnNext = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnNext");
                LinkButton linkBtnLast = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnLast");
                if (GridView1.PageIndex == 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                }
                else if (GridView1.PageIndex == GridView1.PageCount - 1)
                {
                    linkBtnLast.Enabled = false;
                    linkBtnNext.Enabled = false;
                }
                else if (GridView1.PageCount <= 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                    linkBtnNext.Enabled = false;
                    linkBtnLast.Enabled = false;
                }
                DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
                Label pageLabel = (Label)pagerRow.Cells[0].FindControl("CurrentPageLabel");
                if (pageList != null)
                {
                    for (int i = 0; i < GridView1.PageCount; i++)
                    {
                        int pageNumber = i + 1;
                        ListItem item = new ListItem(pageNumber.ToString() + "/" + GridView1.PageCount.ToString(), pageNumber.ToString());
                        if (i == GridView1.PageIndex)
                        {
                            item.Selected = true;
                        }
                        pageList.Items.Add(item);
                    }
                }
                if (pageLabel != null)
                {
                    int currentPage = GridView1.PageIndex + 1;
                    pageLabel.Text = "当前页： " + currentPage.ToString() +
                      " / " + GridView1.PageCount.ToString();
                }
            }
            catch
            {
            }
        }
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            load();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            load();
        }

        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("usercode", Type.GetType("System.String"));
            table.Columns.Add("username", Type.GetType("System.String"));
            table.Columns.Add("status", Type.GetType("System.String"));
            table.Columns.Add("addTime", Type.GetType("System.String"));
            return table;
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string usercode = e.CommandArgument.ToString();
            if (e.CommandName == "start")
            {
                BLL.AccountEnterprise.DelByUserCode(usercode);
                Model.AccountEnterprise ac = new Model.AccountEnterprise();
                ac.UserCode = usercode;
                ac.EnterpriseCode = "-1";
                if (BLL.AccountEnterprise.Add(ac))
                {
                    Message.Alert(this, "分配成功", "null");
                }
                else
                {
                    Message.Alert(this, "分配失败", "null");
                }
            }
            if (e.CommandName == "stop")
            {
                if (BLL.AccountEnterprise.DelByUserCodeAndEnterpriseCode(usercode, "-1"))
                {
                    Message.Alert(this, "全部取消成功", "null");
                }
                else
                {
                    Message.Alert(this, "全部取消失败", "null");
                }
            }
        }
    }
}