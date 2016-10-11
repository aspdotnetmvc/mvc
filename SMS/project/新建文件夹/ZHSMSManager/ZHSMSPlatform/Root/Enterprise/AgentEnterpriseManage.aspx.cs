using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Enterprise
{
    public partial class AgentEnterpriseManage : System.Web.UI.Page
    {
        private const string edit = "AgentEnterprise.Manage.Edit";
        private const string passReset = "AgentEnterprise.Manage.PassReset";
        private const string reCharge = "AgentEnterprise.Manage.ReCharge";
        private const string reBlance = "AgentEnterprise.Manage.ReBlance";
        private const string detail = "AgentEnterprise.Manage.Detail";
        private const string del = "AgentEnterprise.Manage.Del";
        private const string chargeRecord = "AgentEnterprise.Manage.ChargeRecord";
        private const string enterpriseSMSStatistics = "AgentEnterprise.Manage.enterpriseSMSStatistics";
        private const string op = "AgentEnterprise.Manage.Operate";
        private const string cancelAgent = "AgentEnterprise.Manage.cancelAgent";
        private const string lowerAgent = "AgentEnterprise.Manage.LowerAgent";
        private const string secretKey = "AgentEnterprise.Manage.SecretKeyReset";
        private const string enterprisemanage = "AgentEnterprise.Manage.EnterpriseManage";

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
                //List<Model.SysAccount> accounts = BLL.SysAccount.GetAccounts();
                //if (accounts.Count > 0)
                //{
                //    accounts = accounts.OrderByDescending(c => c.AddTime).ToList();
                //    foreach (Model.SysAccount acc in accounts)
                //    {
                //        ddlCM.Items.Add(new ListItem(acc.UserName, acc.UserCode));
                //    }
                //}

                QueryByInputText();
            }
        }

        private void QueryByInputText()
        {
           
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            string condtion = txt_name.Text.Trim();
            string ddlcm = txt_ddlCM.Text.Trim();
            SMSModel.RPCResult<List<Model.EnterpriseUser>> r = ZHSMSProxy.GetZHSMSPlatService().GetAgentEnterprises();
            if (r.Success)
            {
                List<Model.EnterpriseUser> enterprises = r.Value;// BLL.EnterpriseUser.GetAgentEnterprises();
                if (account.UserCode == "admin")
                {
                    
                }
                else
                {
                    List<string> list = BLL.AccountEnterprise.GetEnterpriseByUserCode(account.UserCode);
                    if (list.Count == 1 && list[0] == "-1")//为-1时为全部Enterprise
                    {
                        
                        
                    }
                    else
                    {
                        //只取可以管理的Enterprise
                        enterprises = enterprises.Where(e => list.Where(s => e.AccountCode.Equals(s)).Count() > 0).ToList();
                    }
                }
                DataTable dt = EnterprisesToTable(enterprises);
                var ndt = dt.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(ddlcm) && !string.IsNullOrWhiteSpace(condtion))
                {
                    ndt = ndt.Where(dr => dr["code"].ToString().IndexOf(condtion) > -1 || dr["name"].ToString().IndexOf(condtion) > -1 || dr["ChannelManager"].ToString().IndexOf(ddlcm) > -1 || dr["CSManager"].ToString().IndexOf(ddlcm) > -1);
                }
                else if (!string.IsNullOrWhiteSpace(condtion))
                {
                    ndt = ndt.Where(dr => dr["code"].ToString().IndexOf(condtion) > -1 || dr["name"].ToString().IndexOf(condtion) > -1);
                }
                else if (!string.IsNullOrWhiteSpace(ddlcm))
                {
                    ndt = ndt.Where(dr => dr["ChannelManager"].ToString().IndexOf(ddlcm) > -1 || dr["CSManager"].ToString().IndexOf(ddlcm) > -1);
                }
                dt = ndt.AsDataView().ToTable();
                //if (condtion != "")
                //{
                //    DataView dv = new DataView(dt);
                //    dv.RowFilter = "code like '%" + condtion + "%' or name like '%" + condtion + "%'";
                //    dt = dv.ToTable();
                //}
                this.ViewState["AgentEnterprises"] = dt;
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }
        /// <summary>
        /// 把list 转为datatable
        /// 作为gridview的数据源
        /// </summary>
        /// <param name="enterprises"></param>
        /// <returns></returns>
        private DataTable EnterprisesToTable(List<Model.EnterpriseUser> enterprises)
        {
            DataTable dt = CreateTable();

            foreach (Model.EnterpriseUser a in enterprises)
            {
                DataRow dr = dt.NewRow();
                dr["accountID"] = a.AccountID;
                dr["code"] = a.AccountCode;
                dr["name"] = a.Name;
                dr["contact"] = a.Contact;
                dr["phone"] = a.Phone;
                dr["address"] = a.Address;

                // 根据数据限制，一个企业只会取到一个维护的渠道和客户
                List<Model.EnterpriseManage> emList = BLL.EnterpriseManage.GetEnManageByEnCode(a.AccountCode);
                if (emList.Count > 0)
                {
                    foreach (Model.EnterpriseManage em in emList)
                    {
                        dr["ChannelManager"] = em.ChannelManager;
                        dr["CSManager"] = em.CSManager;
                    }
                }
                if (string.IsNullOrWhiteSpace(Convert.ToString(dr["ChannelManager"])))
                {
                    dr["ChannelManager"] = "设置";
                }

                if (string.IsNullOrWhiteSpace(Convert.ToString(dr["CSManager"])))
                {
                    dr["CSManager"] = "设置";
                }

                dt.Rows.Add(dr);
            }
            return dt;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((Button)e.Row.Cells[19].FindControl("btn")).Attributes.Add("onclick", "return confirm('确认进行此操作吗？');");
                ((LinkButton)e.Row.Cells[21].Controls[0]).Attributes.Add("onclick", "return confirm('确认删除吗？');");
                Model.SysAccount account = (Model.SysAccount)Session["Login"];
                if (account.UserCode != "admin")
                {
                    List<string> permissions = (List<string>)this.ViewState["Permissions"];
                    if (!permissions.Contains(enterprisemanage))
                    {
                        e.Row.Cells[6].Enabled = false;
                        e.Row.Cells[7].Enabled = false;
                        //((HyperLink)e.Row.Cells[6].FindControl("hlCM")).Enabled = false;
                        //((HyperLink)e.Row.Cells[7].FindControl("hlCS")).Enabled = false;
                    }
                    if (!permissions.Contains(edit))
                    {
                        e.Row.Cells[9].Text = "无权限";
                    }
                    if (!permissions.Contains(passReset))
                    {
                        e.Row.Cells[11].Text = "无权限";
                    }
                    if (!permissions.Contains(secretKey))
                    {
                        e.Row.Cells[12].Text = "无权限";
                    }
                    if (!permissions.Contains(reCharge))
                    {
                        e.Row.Cells[13].Text = "无权限";
                    }
                    if (!permissions.Contains(reBlance))
                    {
                        e.Row.Cells[14].Text = "无权限";
                    }
                    if (!permissions.Contains(chargeRecord))
                    {
                        e.Row.Cells[15].Text = "无权限";
                    }
                    if (!permissions.Contains(detail))
                    {
                        e.Row.Cells[16].Text = "无权限";
                    }
                    if (!permissions.Contains(op))
                    {
                        e.Row.Cells[17].Text = "无权限";
                    }
                    if (!permissions.Contains(enterpriseSMSStatistics))
                    {
                        e.Row.Cells[18].Text = "无权限";
                    }
                    if (!permissions.Contains(cancelAgent))
                    {
                        e.Row.Cells[19].Text = "无权限";
                    }
                    if (!permissions.Contains(lowerAgent))
                    {
                        e.Row.Cells[20].Text = "无权限";
                    }
                    if (!permissions.Contains(del))
                    {
                        e.Row.Cells[21].Text = "无权限";
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
            table.Columns.Add("ChannelManager", Type.GetType("System.String"));
            table.Columns.Add("CSManager", Type.GetType("System.String"));
            return table;
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string account = e.CommandArgument.ToString();
            if (e.CommandName == "cancelAgent")
            {
                SMSModel.RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZHSMSPlatService().GetEnterprise(account);// BLL.EnterpriseUser.GetEnerprise(accountID);
                if (r.Success)
                {
                    Model.EnterpriseUser user = r.Value;
                    user.IsAgent = false;
                    user.ParentAccountCode = "-1";
                    ZHSMSProxy.GetZHSMSPlatService().UpdateAccountSetting(user);// BLL.EnterpriseUser.UpdateAccountSetting(user);
                    QueryByInputText();
                }
                else
                {
                    Message.Alert(this, r.Message, "null");
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
                DataTable dt = (DataTable)this.ViewState["AgentEnterprises"];
                dt.Rows.RemoveAt(e.RowIndex);
                this.ViewState["AgentEnterprises"] = dt;
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            else
            {
                Message.Alert(this, "删除失败", "null");
            }
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            QueryByInputText();
        }
    }
}