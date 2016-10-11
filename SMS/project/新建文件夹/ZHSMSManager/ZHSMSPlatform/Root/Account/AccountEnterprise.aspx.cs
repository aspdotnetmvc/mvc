using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Account
{
    public partial class AccountEnterprise : System.Web.UI.Page
    {
        private const string reps = "System.Ae.ae";
        private const string del = "System.Ae.Del";
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
            List<Model.EnterpriseUser> list = BLL.AccountEnterprise.GetEnterpriseUserByUserCode(Request.QueryString["accountID"]);

            DataTable dt = CreateTable();

            foreach (Model.EnterpriseUser a in list)
            {

                DataRow dr = dt.NewRow();
                dr["AccountID"] = a.AccountID;
                dr["AccountCode"] = a.AccountCode;
                dr["Name"] = a.Name;
                dr["Contact"] = a.Contact;
                dr["Telephone"] = a.Phone;
                dr["Address"] = a.Address;
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
                else
                {
                    dr["ChannelManager"] = "未指定";
                    dr["CSManager"] = "未指定";
                }
                dt.Rows.Add(dr);
            }
            GridView1.DataSource = dt;
            GridView1.DataBind();
            Session["dt"] = dt;
        }


        private DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("AccountID", Type.GetType("System.String"));
            table.Columns.Add("AccountCode", Type.GetType("System.String"));
            table.Columns.Add("Name", Type.GetType("System.String"));
            table.Columns.Add("Contact", Type.GetType("System.String"));
            table.Columns.Add("Telephone", Type.GetType("System.String"));
            table.Columns.Add("Address", Type.GetType("System.String"));
            table.Columns.Add("ChannelManager", Type.GetType("System.String"));
            table.Columns.Add("CSManager", Type.GetType("System.String"));
          
            return table;
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    string UserCode = e.Row.Cells[5].Text;
            //    if (UserCode == "" || UserCode == string.Empty || UserCode == "&nbsp;")
            //    {
            //        e.Row.Cells[6].Text = "未分配";
            //    }
            //}
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
        //protected void bt_n_Click(object sender, EventArgs e)
        //{
        //    CheckBoxAll.Checked = false;
        //    CheckBox1.Checked = false;
        //    for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
        //    {
        //        CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
        //        CheckBox.Checked = false;
        //    }
        //}

        //protected void bt1_Click(object sender, EventArgs e)
        //{
        //    Model.SysAccount account = (Model.SysAccount)Session["Login"];
        //    if (account.UserCode != "admin")
        //    {
        //        bool ok = BLL.Permission.IsUsePermission(account.UserCode, reps);
        //        if (!ok)
        //        {
        //            bt1.Visible = false;
        //            Message.Alert(this, "无权限", "null");
        //            return;
        //        }
        //    }
        //    for (int i = 0; i <GridView1.Rows.Count; i++)
        //    {
        //        CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
        //        if (CheckBox.Checked == true)
        //        {
        //            string se = GridView1.DataKeys[i].Value.ToString();
        //            Model.AccountEnterprise ac = new Model.AccountEnterprise();
        //            ac.UserCode = Request.QueryString["accountID"];
        //            ac.EnterpriseCode = se;
        //            if (BLL.AccountEnterprise.Exists(se))
        //            {
        //                BLL.AccountEnterprise.Del(se);
        //            }
        //            BLL.AccountEnterprise.Add(ac);
        //            BLL.AccountEnterprise.DelByUserCodeAndEnterpriseCode(ac.UserCode,"-1");
        //        }
        //    }
        //    CheckBoxAll.Checked = false;
        //    CheckBox1.Checked = false;
        //    load();
        //}
        //protected void bt2_Click(object sender, EventArgs e)
        //{
        //    Model.SysAccount account = (Model.SysAccount)Session["Login"];
        //    if (account.UserCode != "admin")
        //    {
        //        bool ok = BLL.Permission.IsUsePermission(account.UserCode, del);
        //        if (!ok)
        //        {
        //            bt1.Visible = false;
        //            Message.Alert(this, "无权限", "null");
        //            return;
        //        }
        //    }
        //    for (int i = 0; i < GridView1.Rows.Count; i++)
        //    {
        //        CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
        //        if (CheckBox.Checked == true)
        //        {
        //            string se = GridView1.DataKeys[i].Value.ToString();
        //            BLL.AccountEnterprise.Del(se);
        //            BLL.AccountEnterprise.DelByUserCodeAndEnterpriseCode(se, "-1");
        //        }
        //    }
        //    CheckBoxAll.Checked = false;
        //    CheckBox1.Checked = false;
        //    load();
        //}

        //protected void CheckBoxAll_CheckedChanged(object sender, EventArgs e)
        //{
        //    for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
        //    {
        //        CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
        //        if (CheckBoxAll.Checked == true)
        //        {
        //            CheckBox.Checked = true;
        //        }
        //        else
        //        {
        //            CheckBox.Checked = false;
        //        }
        //    }
        //    CheckBox1.Checked = false;
        //}

        //protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        //{
        //    for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
        //    {
        //        CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
        //        if (CheckBox.Checked == false)
        //        {
        //            CheckBox.Checked = true;
        //        }
        //        else
        //        {
        //            CheckBox.Checked = false;
        //        }
        //    }
        //    CheckBoxAll.Checked = false;
        //}

        //protected void btnAll_Click(object sender, EventArgs e)
        //{

        //}
    }
}