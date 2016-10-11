using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SMSModel;

namespace ZHSMSPlatform.Root.Notice
{
    public partial class Notice : System.Web.UI.Page
    {
        private const string del = "Notice.Manage.del";
        private const string detail = "Notice.Manage.detail";
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
        void load()
        {
            DateTime beg = DateTime.Now;
            DateTime end = DateTime.Now.AddDays(-3);
            QueryByInputText(beg, end);
        }
        private void QueryByInputText(DateTime beg, DateTime end)
        {
            DataTable dt = CreateTable();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            //string ro = string.Empty;
            //List<string> role = new List<string>();
            //role = account.Roles;
            //foreach (string a in role)
            //{
            //    ro = a;
            //}
            SMSModel.RPCResult<List<Model.Annunciate>> r = ZHSMSProxy.GetZHSMSPlatService().GetAunnuciateByMangge(beg, end);
            if (r.Success)
            {
                List<Model.Annunciate> an = r.Value;
                foreach (Model.Annunciate a in an)
                {
                    List<string> num = new List<string>();
                    DataRow dr = dt.NewRow();
                    dr["AnnunciateAccount"] = a.AnnunciateAccount;

                    if (a.AnnunciateContent.Length > 30)
                    {
                        dr["AnnunciateContent"] = a.AnnunciateContent.Substring(0, 30);
                    }
                    else
                    {
                        dr["AnnunciateContent"] = a.AnnunciateContent;
                    }
                    dr["AnnunciateID"] = a.AnnunciateID;
                    dr["AnnunciateTitle"] = a.AnnunciateTitle;
                    dr["CreateTime"] = a.CreateTime;
                    dr["PlatType"] = GetSysPlatType(a.PlatType);
                    dr["AnnunciateType"] = GetAnnunciateType(a.Type);
                    num = a.Users;
                    if (a.Users.Count > 3)
                    {
                        dr["UserLists"] = num[0] + "，" + num[1] + "，" + num[2] + " 等" + num.Count + "个用户";
                    }
                    else
                    {
                        foreach (string st in num)
                        {
                            dr["UserLists"] += st + ",";
                        }
                    }

                    dt.Rows.Add(dr);
                }
                //if (condtion != "")
                //{
                //    DataView dv = new DataView(dt);
                //    dv.RowFilter = "UserLists like '%" + condtion + "%' or AnnunciateAccount like '%" + condtion + "%'";
                //    dt = dv.ToTable();
                //}
                this.ViewState["AgentEnterprises"] = dt;
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }
        string GetAnnunciateType(Model.AnnunciateType a)
        {
            string str = "";
            switch (a)
            {
                case Model.AnnunciateType.All:
                    str = "全体可见";
                    break;
                case Model.AnnunciateType.Agent:
                    str = "代理商可见";
                    break;
                case Model.AnnunciateType.NoAgent:
                    str = "终端用户可见";
                    break;
                case Model.AnnunciateType.Person:
                    str = "部分用户可见";
                    break;
                case Model.AnnunciateType.Role:
                    str = "某角色可见";
                    break;
            }
            return str;
        }
        string GetSysPlatType(Model.SysPlatType a)
        {
            string str = "";
            switch (a)
            {
                case Model.SysPlatType.SysPlat:
                    str = "管理平台";
                    break;
                case Model.SysPlatType.ZKDPlat:
                    str = "直客端平台";
                    break;

            }
            return str;
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((LinkButton)e.Row.Cells[10].Controls[0]).Attributes.Add("onclick", "return confirm('确认删除吗？');");
                Model.SysAccount account = (Model.SysAccount)Session["Login"];
                if (account.UserCode != "admin")
                {
                    List<string> permissions = (List<string>)this.ViewState["Permissions"];
                    if (!permissions.Contains(detail))
                    {
                        e.Row.Cells[9].Text = "无权限";
                    }
                    if (!permissions.Contains(del))
                    {
                        e.Row.Cells[10].Text = "无权限";
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
            table.Columns.Add("PlatType", Type.GetType("System.String"));
            table.Columns.Add("AnnunciateID", Type.GetType("System.String"));
            table.Columns.Add("AnnunciateAccount", Type.GetType("System.String"));
            table.Columns.Add("AnnunciateTitle", Type.GetType("System.String"));
            table.Columns.Add("AnnunciateContent", Type.GetType("System.String"));
            table.Columns.Add("CreateTime", Type.GetType("System.String"));
            table.Columns.Add("AnnunciateType", Type.GetType("System.String"));
            table.Columns.Add("UserLists", Type.GetType("System.String"));

            return table;
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {
            DateTime beg = Convert.ToDateTime(txt_S.Text);
            DateTime end = Convert.ToDateTime(txt_E.Text);
            QueryByInputText(beg, end);
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                bool o = BLL.Permission.IsUsePermission(account.UserCode, del);
                if (!o)
                {
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
            string ID = this.GridView1.DataKeys[e.RowIndex].Value.ToString();
            RPCResult rr = ZHSMSProxy.GetZHSMSPlatService().DelAnnunciate(ID);

            if (rr.Success)
            {
                Message.Success(this, rr.Message, "null");
                load();
            }
            else
            {
                Message.Alert(this, rr.Message, "null");
                return;
            }
        }
    }
}