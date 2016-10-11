using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using SMSModel;
using System.Data;
using StatusReportInterface;


namespace ZKD.Root.Report
{
    public partial class Rep_StatisticsList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                ViewState["SortOrder"] = "BeginSendTime";
                ViewState["OrderDire"] = "DESC";
                load();
            }
        }
        private void load()
        {
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            DataTable dt = CreateTable();
            string txtNum = txt_Num.Text.Trim();
            // DataTable dt = BLL.SMSdo.GetSMSByAccountAndSendTime(user.AccountCode, starttime, endtime);
            RPCResult<List<ReportStatistics>> r = ZHSMSProxy.GetZKD().GetDirectStatisticReportByAccount(user.AccountCode);
            if (r.Success)
            {
                lbl_message.Visible = false;
              
                List<ReportStatistics> rr = r.Value;
                if (!string.IsNullOrWhiteSpace(txtNum))
                {
                    rr = rr.Where(s => containNum(s, txtNum)).ToList();
                }
                Label1.Text = "当前有" + rr.Count + "条返回状态报告";
                if (rr.Count > 0)
                {
                    foreach (ReportStatistics s in rr)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Account"] = s.Account;
                        dr["BeginSendTime"] = s.BeginSendTime.ToString("yyyy-MM-dd HH:mm:ss");
                        dr["FailureCount"] = s.FailureCount;
                        dr["LastResponseTime"] = s.LastResponseTime.ToString("yyyy-MM-dd HH:mm:ss");
                        if (s.Telephones != null)
                        {
                            dr["Numbers"] = s.Telephones.Count;
                            List<string> num = s.Telephones;
                            string str = "";
                            foreach (string st in num)
                            {
                                str += st + ",";
                            }
                            dr["PhoneCount"] = str == "" ? "" : str.Substring(0, str.Length - 1);
                        }
                        dr["SendCount"] = s.SendCount;
                        dr["SendTime"] = s.SendTime;
                        dr["SerialNumber"] = s.SerialNumber;
                        dr["SplitNumber"] = s.SplitNumber;
                        dr["Succeed"] = s.Succeed;
                        dr["SMSContent"] = s.SMSContent;
                        dt.Rows.Add(dr);
                        
                    }
                }
            }
            else
            {
                Message.Alert(this, r.Message, "null");
                return;

            }

            DataView view = dt.DefaultView;
            string sort = (string)ViewState["SortOrder"] + " " + (string)ViewState["OrderDire"];
            view.Sort = sort;
            GridView1.DataSource = view.ToTable();
            GridView1.DataBind();
        }
        /// <summary>
        /// 判断是否包含某个号码
        /// </summary>
        /// <param name="s"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private bool containNum(ReportStatistics s, string num)
        {
            if (s.Telephones == null)
                return false;
            else
            {
                return s.Telephones.Count(t => t.IndexOf(num) > -1) > 0 ? true : false;
            }
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView1.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
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



        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {


        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }
        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("Account", Type.GetType("System.String"));
            table.Columns.Add("BeginSendTime", Type.GetType("System.String"));
            table.Columns.Add("FailureCount", Type.GetType("System.String"));
            table.Columns.Add("LastResponseTime", Type.GetType("System.String"));
            table.Columns.Add("Numbers", Type.GetType("System.String"));
            table.Columns.Add("SendCount", Type.GetType("System.String"));
            table.Columns.Add("SendTime", Type.GetType("System.String"));
            table.Columns.Add("SerialNumber", Type.GetType("System.String"));
            table.Columns.Add("SplitNumber", Type.GetType("System.String"));
            table.Columns.Add("Succeed", Type.GetType("System.String"));
            table.Columns.Add("SMSContent", Type.GetType("System.String"));
            table.Columns.Add("PhoneCount", Type.GetType("System.String"));

            return table;
        }

        protected void btn_n_Click(object sender, EventArgs e)
        {
            load();
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sPage = e.SortExpression;
            if (ViewState["SortOrder"].ToString() == sPage)
            {
                if (ViewState["OrderDire"].ToString() == "Desc")
                    ViewState["OrderDire"] = "ASC";
                else
                    ViewState["OrderDire"] = "Desc";
            }
            else
            {
                ViewState["SortOrder"] = e.SortExpression;
            }
            load();
        }
    }
}