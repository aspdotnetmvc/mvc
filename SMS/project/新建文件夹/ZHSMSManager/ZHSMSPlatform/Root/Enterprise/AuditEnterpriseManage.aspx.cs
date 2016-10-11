using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Enterprise
{
    public partial class AuditEnterpriseManage : System.Web.UI.Page
    {
        private const string audit = "AuditEnterprise.Manage.Audit";
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
                QueryByInputText();
            }
        }
        private void QueryByInputText()
        {
            DataTable dt = CreateTable();
            string condtion = txt_name.Text;
            SMSModel.RPCResult<List<Model.EnterpriseUser>> r = ZHSMSProxy.GetZHSMSPlatService().GetAuditEnterprises();
            if (r.Success)
            {
                List<Model.EnterpriseUser> enterprises = r.Value;

                foreach (Model.EnterpriseUser a in enterprises)
                {
                    DataRow dr = dt.NewRow();
                    dr["accountID"] = a.AccountID;
                    dr["code"] = a.AccountCode;
                    dr["name"] = a.Name;
                    dr["contact"] = a.Contact;
                    dr["phone"] = a.Phone;
                    dr["address"] = a.Address;
                    dr["registerDate"] = a.RegisterDate;
                    dt.Rows.Add(dr);
                }
                if (condtion != "")
                {
                    DataView dv = new DataView(dt);
                    dv.RowFilter = "code like '%" + condtion + "%' or name like '%" + condtion + "%'";
                    dt = dv.ToTable();
                }
                this.ViewState["AgentEnterprises"] = dt;
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Model.SysAccount account = (Model.SysAccount)Session["Login"];
                if (account.UserCode != "admin")
                {
                    List<string> permissions = (List<string>)this.ViewState["Permissions"];
                    if (!permissions.Contains(audit))
                    {
                        e.Row.Cells[7].Text = "无权限";
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
            DataTable dt = (DataTable)this.ViewState["AgentEnterprises"];
            GridView1.DataSource = dt;
            GridView1.DataBind();
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
            DataTable dt = (DataTable)this.ViewState["AgentEnterprises"];
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
            table.Columns.Add("accountID", Type.GetType("System.String"));
            table.Columns.Add("code", Type.GetType("System.String"));
            table.Columns.Add("name", Type.GetType("System.String"));
            table.Columns.Add("contact", Type.GetType("System.String"));
            table.Columns.Add("phone", Type.GetType("System.String"));
            table.Columns.Add("address", Type.GetType("System.String"));
            table.Columns.Add("registerDate", Type.GetType("System.String"));
            return table;
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {
            QueryByInputText();
        }
    }
}