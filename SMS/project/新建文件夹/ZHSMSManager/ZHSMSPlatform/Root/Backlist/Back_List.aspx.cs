using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml.Linq;
using SMSModel;

namespace ZHSMSPlatform.Root.Backlist
{
    public partial class Back_List : System.Web.UI.Page
    {
        private const string del = "Backlist.Back.Del";
        private const string loads = "Backlist.Back.Load";
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
            string qryNum = txt_Num.Text.Trim();
            DataTable dt = CreateTable();
            RPCResult<List<string>> r = ZHSMSProxy.GetZHSMSPlatService().GetBlacklist();
            if (r.Success)
            {
                lbl_message.Visible = false;
                List<string> ss = r.Value;
                if (!string.IsNullOrWhiteSpace(qryNum))
                {
                    ss = ss.Where(n => n.IndexOf(qryNum) > -1).ToList();
                }
                if (ss.Count > 0)
                {
                    foreach (string s in ss)
                    {
                        DataRow dr = dt.NewRow();
                        dr["A"] = s;
                        dt.Rows.Add(dr);
                    }
                }
            }
            GridView1.DataSource = dt;
            GridView1.DataBind();
            this.ViewState["BlackDt"] = dt;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView1.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                ((LinkButton)e.Row.Cells[1].Controls[0]).Attributes.Add("onclick", "return confirm('确认删除吗？');");
                Model.SysAccount account = (Model.SysAccount)Session["Login"];
                if (account.UserCode != "admin")
                {
                    List<string> permissions = (List<string>)this.ViewState["Permissions"];
                    if (!permissions.Contains(del))
                    {
                        e.Row.Cells[1].Text = "无权限";
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
            DataTable dt = (DataTable)this.ViewState["BlackDt"];
            GridView1.DataSource = dt;
            GridView1.DataBind();
            //load();
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
            DataTable dt = (DataTable)this.ViewState["BlackDt"];
            GridView1.DataSource = dt;
            GridView1.DataBind();

            //load();
        }

        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("A", Type.GetType("System.String"));
            return table;
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, del);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
            string phone = this.GridView1.DataKeys[e.RowIndex].Value.ToString();
            RPCResult r = ZHSMSProxy.GetZHSMSPlatService().DelBlacklist(new List<string> { phone });
            if (r.Success)
            {
                Message.Success(this, "黑名单删除成功", "null");
                DataTable dt = (DataTable)this.ViewState["BlackDt"];
                int index = e.RowIndex;
                dt.Rows.RemoveAt(index);
                this.ViewState["BlackDt"] = dt;
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            load();
        }
    }
}