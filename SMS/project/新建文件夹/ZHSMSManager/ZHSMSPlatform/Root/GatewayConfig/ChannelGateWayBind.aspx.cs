using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SMSModel;

namespace ZHSMSPlatform.Root.GatewayConfig
{
    public partial class ChannelGateWayBind : System.Web.UI.Page
    {
        private const string bind = "Channel.Manage.RelateGateway";

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
                dd_l.Items.Clear();
                RPCResult<List<SMSModel.Channel>> r = ZHSMSProxy.GetZHSMSPlatService().GetChannels();
                if (r.Success)
                {
                    if (r.Value.Count > 0)
                    {
                        foreach (SMSModel.Channel c in r.Value)
                        {
                            dd_l.Items.Add(new ListItem(c.ChannelName, c.ChannelID));
                        }
                    }
                }
                dd_l.Items.Insert(0, new ListItem("--请选择--", "-1"));
                load();
            }
        }
        private void load()
        {
            SMSModel.RPCResult<List<SMSModel.GatewayConfiguration>> r = ZHSMSProxy.GetZHSMSPlatService().GetGatewayConfigs();
            DataTable dt = CreateTable();

            if (r.Success)
            {
                foreach (var config in r.Value)
                {
                    DataRow dr = dt.NewRow();
                    dr["gateway"] = config.Gateway;
                    dr["operators"] = config.Operators;
                    dr["handlingAbility"] = config.HandlingAbility;
                    dt.Rows.Add(dr);
                }
            }
            GridView1.DataSource = dt;
            GridView1.DataBind();
            Session["dt"] = dt;

        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Model.SysAccount account = (Model.SysAccount)Session["Login"];
                //if (account.UserCode != "admin")
                //{
                //    List<string> permissions = (List<string>)this.ViewState["Permissions"];
                   
                //    if (!permissions.Contains(bind))
                //    {
                //        e.Row.Cells[7].Text = "无权限";
                //    }
                //}
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
            table.Columns.Add("gateway", Type.GetType("System.String"));
            table.Columns.Add("operators", Type.GetType("System.String"));
            table.Columns.Add("handlingAbility", Type.GetType("System.String"));
            table.Columns.Add("othersend", Type.GetType("System.String"));
            return table;
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //Model.SysAccount account = (Model.SysAccount)Session["Login"];
            //if (account.UserCode != "admin")
            //{
            //    bool o = BLL.Permission.IsUsePermission(account.UserCode, nobind);
            //    if (!o)
            //    {
            //        Message.Alert(this, "无权限", "null");
            //        return;
            //    }
            //}
            //string gateway = this.GridView1.DataKeys[e.RowIndex].Value.ToString();
            //SMSModel.RPCResult ok = ZHSMSProxy.GetZHSMSPlatService().DelGatewayByGateway(gateway);
            //if (ok.Success)
            //{
            //    Message.Success(this, "操作成功", "null");
            //    load();
            //}
            //else
            //{
            //    Message.Alert(this, ok.Message, "null");
            //}

        }

        protected void CheckBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            {
                CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                if (CheckBoxAll.Checked == true)
                {
                    CheckBox.Checked = true;
                }
                else
                {
                    CheckBox.Checked = false;
                }
            }
            CheckBox1.Checked = false;
        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            {
                CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                if (CheckBox.Checked == false)
                {
                    CheckBox.Checked = true;
                }
                else
                {
                    CheckBox.Checked = false;
                }
            }
            CheckBoxAll.Checked = false;

        }

        protected void bt1_Click(object sender, EventArgs e)
        {
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, bind);
                if (!ok)
                {
                    bt1.Visible = false;
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
            List<string> a = new List<string>();
            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            {
                CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                if (CheckBox.Checked == true)
                {
                    string c = GridView1.DataKeys[i].Value.ToString();
                    a.Add(c);
                }
                RPCResult r = ZHSMSProxy.GetZHSMSPlatService().AddChannelGatewayBind(dd_l.SelectedValue, a);
            }
            CheckBoxAll.Checked = false;
            CheckBox1.Checked = false;
            load();
            Message.Success(this, "通道网关绑定成功", "null");
        }

        protected void bt_n_Click(object sender, EventArgs e)
        {
            CheckBoxAll.Checked = false;
            CheckBox1.Checked = false;
            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            {
                CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                CheckBox.Checked = false;
            }
        }
    }
}