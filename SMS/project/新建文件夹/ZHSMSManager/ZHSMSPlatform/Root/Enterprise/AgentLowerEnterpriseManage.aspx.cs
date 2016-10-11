using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Enterprise
{
    public partial class AgentLowerEnterpriseManage : System.Web.UI.Page
    {
        private const string edit = "AgentLowerEnterprise.Manage.Edit";
        private const string passReset = "AgentLowerEnterprise.Manage.PassReset";
        private const string reCharge = "AgentLowerEnterprise.Manage.ReCharge";
        private const string reBlance = "AgentLowerEnterprise.Manage.ReBlance";
        private const string detail = "AgentLowerEnterprise.Manage.Detail";
        private const string del = "AgentLowerEnterprise.Manage.Del";
        private const string chargeRecord = "AgentLowerEnterprise.Manage.ChargeRecord";
        private const string enterpriseSMSStatistics = "AgentLowerEnterprise.Manage.enterpriseSMSStatistics";
        private const string op = "AgentLowerEnterprise.Manage.Operate";
        private const string upgrade = "AgentLowerEnterprise.Manage.UpGrade";
        private const string secretKey = "AgentLowerEnterprise.Manage.SecretKeyReset";
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
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            DataTable dt = CreateTable();
            SMSModel.RPCResult<List<Model.EnterpriseUser>> r = ZHSMSProxy.GetZHSMSPlatService().GetLowerEnterprises(Request.QueryString["accountID"]);
            if (r.Success)
            {
                if (account.UserCode == "admin")
                {
                    foreach (Model.EnterpriseUser a in r.Value)
                    {
                        DataRow dr = dt.NewRow();
                        dr["accountID"] = a.AccountID;
                        dr["code"] = a.AccountCode;
                        dr["name"] = a.Name;
                        dr["contact"] = a.Contact;
                        dr["phone"] = a.Phone;
                        dr["address"] = a.Address;
                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    List<string> list = BLL.AccountEnterprise.GetEnterpriseByUserCode(account.UserCode);
                    if (list.Count == 1 && list[0] == "-1")
                    {
                        foreach (Model.EnterpriseUser a in r.Value)
                        {
                            DataRow dr = dt.NewRow();
                            dr["accountID"] = a.AccountID;
                            dr["code"] = a.AccountCode;
                            dr["name"] = a.Name;
                            dr["contact"] = a.Contact;
                            dr["phone"] = a.Phone;
                            dr["address"] = a.Address;
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        foreach (Model.EnterpriseUser a in r.Value)
                        {
                            if (list.Contains(a.AccountCode))
                            {
                                DataRow dr = dt.NewRow();
                                dr["accountID"] = a.AccountID;
                                dr["code"] = a.AccountCode;
                                dr["name"] = a.Name;
                                dr["contact"] = a.Contact;
                                dr["phone"] = a.Phone;
                                dr["address"] = a.Address;
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }
            }
            this.ViewState["AgentLowerEnterprises"] = dt;
            GridView1.DataSource = dt;
            GridView1.DataBind();

        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((LinkButton)e.Row.Cells[18].Controls[0]).Attributes.Add("onclick", "return confirm('确认删除吗？');");
                Model.SysAccount account = (Model.SysAccount)Session["Login"];
                if (account.UserCode != "admin")
                {
                    List<string> permissions = (List<string>)this.ViewState["Permissions"];
                    if (!permissions.Contains(edit))
                    {
                        e.Row.Cells[7].Text = "无权限";
                    }
                    if (!permissions.Contains(passReset))
                    {
                        e.Row.Cells[9].Text = "无权限";
                    }
                    if (!permissions.Contains(secretKey))
                    {
                        e.Row.Cells[10].Text = "无权限";
                    }
                    if (!permissions.Contains(reCharge))
                    {
                        e.Row.Cells[11].Text = "无权限";
                    }
                    if (!permissions.Contains(reBlance))
                    {
                        e.Row.Cells[12].Text = "无权限";
                    }
                    if (!permissions.Contains(chargeRecord))
                    {
                        e.Row.Cells[13].Text = "无权限";
                    }
                    if (!permissions.Contains(detail))
                    {
                        e.Row.Cells[14].Text = "无权限";
                    }
                    if (!permissions.Contains(op))
                    {
                        e.Row.Cells[15].Text = "无权限";
                    }
                    if (!permissions.Contains(enterpriseSMSStatistics))
                    {
                        e.Row.Cells[16].Text = "无权限";
                    }
                    if (!permissions.Contains(upgrade))
                    {
                        e.Row.Cells[17].Text = "无权限";
                    }
                    if (!permissions.Contains(del))
                    {
                        e.Row.Cells[18].Text = "无权限";
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
            DataTable dt = (DataTable)this.ViewState["AgentLowerEnterprises"];
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
            DataTable dt = (DataTable)this.ViewState["AgentLowerEnterprises"];
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
            return table;
        }
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string account = e.CommandArgument.ToString();
            if (e.CommandName == "upgrade")
            {
                SMSModel.RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZHSMSPlatService().GetEnterprise(account);
                if (r.Success)
                {
                    Model.EnterpriseUser user = r.Value;// BLL.EnterpriseUser.GetEnerprise(accountID);
                    user.IsAgent = true;
                    user.ParentAccountCode = "-1";
                    ZHSMSProxy.GetZHSMSPlatService().UpdateAccountSetting(user);// BLL.EnterpriseUser.UpdateAccountSetting(user);
                    load();
                }
            }
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
            string accountCode = this.GridView1.DataKeys[e.RowIndex][0].ToString();
            SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().DelEnterprise(accountCode);
            if (r.Success)
            {
                BLL.AccountEnterprise.Del(accountCode);
                Message.Success(this, "操作成功", "null");
                DataTable dt = (DataTable)this.ViewState["AgentLowerEnterprises"];
                dt.Rows.RemoveAt(e.RowIndex);
                this.ViewState["AgentLowerEnterprises"] = dt;
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
        }
    }
}