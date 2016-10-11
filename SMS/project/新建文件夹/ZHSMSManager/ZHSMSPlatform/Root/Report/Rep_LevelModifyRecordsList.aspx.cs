using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using SMSModel;
using System.Data;


namespace ZHSMSPlatform.Root.Report
{
    public partial class Rep_LevelModifyRecordsList : System.Web.UI.Page
    {
        private const string detail = "Report.LevelModifyRecords.Detail";
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
                txt_Start.Text = DateTime.Now.AddDays(-1).ToString();
                txt_End.Text = DateTime.Now.ToString();
            }
        }
        private void load()
        {
            DateTime starttime = Convert.ToDateTime(txt_Start.Text);
            DateTime endtime = Convert.ToDateTime(txt_End.Text);
            if (DateTime.Compare(starttime, endtime) >= 0)
            {
                Message.Alert(this, "开始时间应小于结束时间", "null");
                return;
            }
            DataTable dt = CreateTable();
            RPCResult<List<LevelModifyRecord>> r = ZHSMSProxy.GetZHSMSPlatService().GetLevelModifyRecords(starttime, endtime);
            if (r.Success)
            {
                lbl_message.Visible = false;
                Label1.Text = "优先级调整记录短信有" + r.Value.Count + "条";
                List<LevelModifyRecord> lm = r.Value;
                if (lm.Count > 0)
                {
                    foreach (LevelModifyRecord s in lm)
                    {
                        DataRow dr = dt.NewRow();
                        dr["AccountID"] = s.AccountID;
                        dr["Content"] = s.Content;
                        dr["ModifyContent"] = s.ModifyContent;
                        dr["ModifyTime"] = s.ModifyTime;
                        dr["SendTime"] = s.SendTime;
                        dr["SerialNumber"] = s.SerialNumber;
                        dt.Rows.Add(dr);
                    }
                }
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
            dt.DefaultView.Sort = "SerialNumber desc,ModifyTime desc";
            this.ViewState["LevelModifyRecords"] = dt;
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }


        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView1.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                Model.SysAccount account = (Model.SysAccount)Session["Login"];
                if (account.UserCode != "admin")
                {
                    List<string> permissions = (List<string>)this.ViewState["Permissions"];
                    if (!permissions.Contains(detail))
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
            DataTable dt = (DataTable)this.ViewState["LevelModifyRecords"];
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
            DataTable dt = (DataTable)this.ViewState["LevelModifyRecords"];
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
            table.Columns.Add("AccountID", Type.GetType("System.String"));
            table.Columns.Add("SerialNumber", Type.GetType("System.String"));
            table.Columns.Add("Content", Type.GetType("System.String"));
            table.Columns.Add("ModifyContent", Type.GetType("System.String"));
            table.Columns.Add("ModifyTime", Type.GetType("System.String"));
            table.Columns.Add("SendTime", Type.GetType("System.String"));
            return table;
        }

        protected void btn_n_Click(object sender, EventArgs e)
        {
            load();
        }

    }
}