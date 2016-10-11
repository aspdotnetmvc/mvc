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
    public partial class Rep_AuditRecordsList : System.Web.UI.Page
    {
        private const string loads = "Report.AuditRecords.Load";
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
                txt_Start.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                txt_End.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        private void load( bool? bSuccess)
        {
          

           
            DateTime starttime = Convert.ToDateTime(txt_Start.Text);
            DateTime endtime = Convert.ToDateTime(txt_End.Text);
            if (DateTime.Compare(starttime, endtime) >= 0)
            {
                Message.Alert(this, "开始时间应小于结束时间", "null");
                return;
            }

            DataTable dt = CreateTable();
            
            if (bSuccess==null || bSuccess.Value)
            {
                RPCResult<List<AuditRecord>> r = ZHSMSProxy.GetZHSMSPlatService().GetAuditRecords(starttime, endtime);
                if (r.Success)
                {
                    List<AuditRecord> au = r.Value;
                    if (bSuccess != null)
                    {
                        //加一层过滤，只要审核通过的
                        au=au.Where(c => c.Result == true).ToList();
                    }
                    au = au.OrderByDescending(c => c.SendTime).ToList();
                    lbl_message.Visible = false;
                    Label1.Text = "审核记录短信有" + au.Count + "条";
                    
                    if (au.Count > 0)
                    {
                        foreach (AuditRecord s in au)
                        {
                            DataRow dr = dt.NewRow();
                            dr["AccountID"] = s.AccountID;
                            dr["SerialNumber"] = s.SerialNumber;
                            dr["Content"] = s.Content;
                            dr["AuditTime"] = s.AuditTime;
                            dr["SendTime"] = s.SendTime;
                            dr["Result"] = s.Result == true ? "成功" : "失败";
                            dr["FailureCase"] = "无";

                            dt.Rows.Add(dr);
                        }
                    }
                }
                else
                {
                    Message.Alert(this, r.Message, "null");
                }
            }
            else
            {
                RPCResult<List<FailureSMS>> r = ZHSMSProxy.GetZHSMSPlatService().GetSMSByAuditFailure(starttime, endtime);
                if (r.Success)
                {
                    lbl_message.Visible = false;
                    Label1.Text = "审核记录短信有" + r.Value.Count + "条";
                    List<FailureSMS> au = r.Value.OrderByDescending(f => f.SendTime).ToList() ;
                    if (au.Count > 0)
                    {
                        foreach (FailureSMS s in au)
                        {
                            DataRow dr = dt.NewRow();
                            dr["AccountID"] = s.AuditUser;
                            dr["SerialNumber"] = s.SerialNumber;
                            dr["Content"] = s.Content;
                            dr["AuditTime"] = s.AuditTime;
                            dr["SendTime"] = s.SendTime;
                            dr["Result"] = "失败";
                            dr["FailureCase"] = s.FailureCase;

                            dt.Rows.Add(dr);
                        }
                    }
                }
                else
                {
                    Message.Alert(this, r.Message, "null");
                }
            }
            this.ViewState["AuditRecords"] = dt;
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView1.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
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
            DataTable dt = (DataTable)this.ViewState["AuditRecords"];
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
            DataTable dt = (DataTable)this.ViewState["AuditRecords"];
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
            table.Columns.Add("SerialNumber", Type.GetType("System.String"));
            table.Columns.Add("AccountID", Type.GetType("System.String"));
            table.Columns.Add("Content", Type.GetType("System.String"));
            table.Columns.Add("AuditTime", Type.GetType("System.String"));
            table.Columns.Add("SendTime", Type.GetType("System.String"));
            table.Columns.Add("Result", Type.GetType("System.String"));
            table.Columns.Add("FailureCase", Type.GetType("System.String"));
            return table;
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, loads);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }

            if (rbtnSuccess.Checked)
            {
                // 显示审核成功的短信
                load(true);
            }
            else
            {
                // 显示审核失败的短信
                if (rbtnFailure.Checked)
                {
                    load(false);
                }
                else
                {
                    load(null);
                }
            }
        }
    }
}