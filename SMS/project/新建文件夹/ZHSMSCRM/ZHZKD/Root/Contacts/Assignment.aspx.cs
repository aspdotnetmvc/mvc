using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SMSModel;
using org.in2bits.MyXls;
using System.Xml;

namespace ZKD.Root.Contacts
{
    public partial class Assignment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            if (!IsPostBack)
            {
                dd_l.Items.Clear();
                DataTable dt = BLL.PhoneAndGroup.GetGroupByAccountCode(user.AccountCode);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dd_l.Items.Add(new ListItem(dt.Rows[i]["TelPhoneGroupName"].ToString(), dt.Rows[i]["GID"].ToString()));
                    }
                }
                dd_l.Items.Remove("0");
                dd_l.Items.Insert(0, new ListItem("--请选择--", "-1"));
                BindTreeview(TreeView1);
                CheckBoxAll.Checked = false;
                CheckBox1.Checked = false;
                load();
            }
        }
        private void load()
        {
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            DataTable dt = new DataTable();
            string value = TreeView1.SelectedValue;
            if (value != "")
            {
                if (value != "-1")
                {
                    dt = BLL.PhoneAndGroup.GetPhoneByAccountCodeAndGroup(user.AccountCode, value);
                }
                else
                {
                    dt = BLL.PhoneAndGroup.GetPhoneByAccountCodes(user.AccountCode);
                }
            }
            else
            {
                dt = BLL.PhoneAndGroup.GetPhoneByAccountCodes(user.AccountCode);
            }
            if (dt.Rows.Count == 0)
            {
                lbl_message.Visible = true;
            }
            else
            {
                lbl_message.Visible = false;
            }
            this.ViewState["TXL"] = dt;
            GridView1.DataSource = dt;
            GridView1.DataBind();
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
            GridView1.DataSource = (DataTable)this.ViewState["TXL"];
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
            GridView1.DataSource = (DataTable)this.ViewState["TXL"];
            GridView1.DataBind();

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

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((LinkButton)e.Row.Cells[18].Controls[0]).Attributes.Add("onclick", "return confirm('确认删除吗？');");
                string aa = e.Row.Cells[16].Text;
                if (aa == "" || aa == string.Empty || aa == "&nbsp;" || aa == "0")
                {
                    e.Row.Cells[15].Text = "未分组";
                    e.Row.Cells[16].Text = "未分组";
                    e.Row.Cells[17].Text = "未分组";
                }
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string c = this.GridView1.DataKeys[e.RowIndex].Value.ToString();
            bool r = BLL.PhoneAndGroup.PhoneDelByID(c);
            if (r)
            {
                DataTable dt = (DataTable)this.ViewState["TXL"];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["PID"].ToString() == c)
                    {
                        dt.Rows.Remove(dr);
                        break;
                    }
                }
                this.ViewState["TXL"] = dt;
                GridView1.DataSource = dt;
                GridView1.DataBind();
                Message.Success(this, "删除号码成功", "null");
            }
            else
            {
                Message.Error(this, "删除号码失败", "null");
            }
        }


        protected void BindTreeview(TreeView treeview)
        {
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];

            DataTable ds = BLL.PhoneAndGroup.GetGroupForTreeByAccountCode(user.AccountCode);
            treeview.Nodes.Clear();
            TreeNode tn = new TreeNode("通讯录组", "-1");
            tn.Expanded = true;
            treeview.Nodes.Add(tn);
            DataRow[] rowG = ds.Select();
            foreach (DataRow dr in rowG)
            {
                TreeNode NewNode = new TreeNode();
                if (dr["TelPhoneGroupName"].ToString() == "0")
                {
                    dr["TelPhoneGroupName"] = "未分组";
                }
                NewNode.Text = dr["TelPhoneGroupName"].ToString();
                NewNode.Value = dr["GID"].ToString();
                NewNode.Expanded = true;
                NewNode.SelectAction = TreeNodeSelectAction.SelectExpand;
                tn.ChildNodes.Add(NewNode);
            }
        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            CheckBoxAll.Checked = false;
            CheckBox1.Checked = false;
            load();
        }
        protected void btn_cancel_Click(object sender, EventArgs e)
        {
            //取消
            CheckBoxAll.Checked = false;
            CheckBox1.Checked = false;
            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            {
                CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                CheckBox.Checked = false;
            }
        }

        protected void btn_group_Click(object sender, EventArgs e)
        {
            //分组
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            DataTable dt = BLL.PhoneAndGroup.GetGroupByTelPhoneGroupNameAndAccountCode("0", user.AccountCode);
            string GID = string.Empty;
            if (dt.Rows.Count > 0)
            {
                GID = dt.Rows[0][0].ToString();
            }
            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            {
                CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                if (CheckBox.Checked == true)
                {
                    string PID = GridView1.DataKeys[i][0].ToString();
                    if (dd_l.SelectedValue == "-1")
                    {

                        BLL.PhoneAndGroup.PhoneConnectGroup(GID, user.AccountCode, PID);
                    }
                    else
                    {
                        BLL.PhoneAndGroup.PhoneConnectGroup(dd_l.SelectedValue, user.AccountCode, PID);
                    }
                }
            }
            CheckBoxAll.Checked = false;
            CheckBox1.Checked = false;
            GridView1.DataSource = (DataTable)this.ViewState["TXL"];
            GridView1.DataBind();
            load();
            Message.Success(this, " 分组成功", "null");

        }

        protected void btn_cancelGroup_Click(object sender, EventArgs e)
        {
            //取消分组
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            DataTable dt = BLL.PhoneAndGroup.GetGroupByTelPhoneGroupNameAndAccountCode("0", user.AccountCode);
            string GID = string.Empty;
            if (dt.Rows.Count > 0)
            {
                GID = dt.Rows[0][0].ToString();
            }
            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            {
                CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                if (CheckBox.Checked == true)
                {
                    string PID = GridView1.DataKeys[i][0].ToString();
                    BLL.PhoneAndGroup.PhoneConnectGroup(GID, user.AccountCode, PID);
                }
            }
            CheckBoxAll.Checked = false;
            CheckBox1.Checked = false;
            GridView1.DataSource = (DataTable)this.ViewState["TXL"];
            GridView1.DataBind();
            load();
            Message.Success(this, " 取消分组", "null");
        }
        protected void btn_sendSMS_Click(object sender, EventArgs e)
        {
            //短信发送
            List<string> num = new List<string>();
            string value = TreeView1.SelectedValue;
            if (value != "")
            {
                DataTable dt = (DataTable)this.ViewState["TXL"];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        num.Add(dt.Rows[i]["TelPhoneNum"].ToString());
                    }
                }
            }
            else
            {
                for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
                {
                    CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                    if (CheckBox.Checked == true)
                    {
                        string TelPhoneNum = GridView1.DataKeys[i][1].ToString();
                        num.Add(TelPhoneNum);
                    }
                }
            }
            if(num.Count==0)
            {
                Message.Alert(this, "请选择电话号码");
                return;
            }
            if(num.Count>1000)
            {
                Message.Alert(this, "单次提交最多不超过1000个号码");
                return;
            }
            Session["nums"] = num;
            Response.Redirect("Send.aspx");
        }

        protected void btn_import_Click(object sender, EventArgs e)
        {
            //导出通讯录
            DataTable dt = (DataTable)this.ViewState["TXL"];
            if (dt == null || dt.Rows.Count == 0)
            {
                Message.Alert(this, "通讯录无内容", "null");
                return;
            }
            DataTable copydt = dt.Copy();
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                if (CheckBox.Checked == true)
                {
                    string pid = GridView1.DataKeys[i][0].ToString();
                    copydt.Rows.Remove(copydt.Select("PID='" + pid + "'")[0]);
                }
            }
            if (dt.Rows.Count != copydt.Rows.Count)
            {
                IEnumerable<DataRow> query = dt.AsEnumerable().Except(copydt.AsEnumerable(), DataRowComparer.Default);
                dt = query.CopyToDataTable();
            }
            XlsDocument xls = new XlsDocument();
            xls.FileName = ((Model.EnterpriseUser)Session["Login"]).Name + (TreeView1.SelectedValue == "" ? "" : TreeView1.SelectedNode.Text) + "通讯录.xls";

            //添加文件属性 
            xls.SummaryInformation.Author = "person"; //作者 
            xls.SummaryInformation.Subject = "通讯录";
            xls.DocumentSummaryInformation.Company = "person";
            string sheetname = "通讯录";

            Worksheet sheet = xls.Workbook.Worksheets.Add(sheetname);
            Cells cells = sheet.Cells;

            XF xf;
            xf = xls.NewXF();//为xls生成一个XF实例（XF是cell格式对象）

            xf.HorizontalAlignment = HorizontalAlignments.Centered;//设定文字居中
            xf.VerticalAlignment = VerticalAlignments.Centered;
            xf.Font.FontName = "宋体";//设定字体
            xf.Font.Bold = true;

            //设定列宽
            ColumnInfo colInfoTime = new ColumnInfo(xls, sheet);
            colInfoTime.ColumnIndexStart = 0;
            colInfoTime.ColumnIndexEnd = 10;
            colInfoTime.Width = 20 * 300;
            sheet.AddColumnInfo(colInfoTime);

            string[] name = new string[] { "联系人", "生日", "性别", "电话号码", "联系人公司", "联系人Email", "联系人QQ", "联系人职位", "联系人微信", "联系人公司网站" };
            string[] text = new string[] { "UserName", "UserBrithday", "UserSex", "TelPhoneNum", "CompanyName", "CompanyEmail", "QQ", "ComPostion", "WebChat", "CompanyWeb" };
            addItem(name, text, dt, xf, cells);
            xls.Send();

        }

        private void addItem(string[] name, string[] value, DataTable dt, XF xf, Cells cells)
        {
            int pos = 3;
            int step = 0;
            for (int index = 1; index <= name.Length; index++)
            {
                cells.Merge(1, 2, index, index);
            }
            for (int i = 1; i <= name.Length; i++)
            {
                cells.Add(1, i, name[i - 1].ToString(), xf);
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                xf.Font.Bold = false;
                Merge(ref  pos, ref  step, i, dt, cells, value[0]);
                for (int col = 1; col <= value.Length; col++)
                {
                    string colvalue = value[col - 1].ToString();
                    cells.Add(i + 3, col, dt.Rows[i][colvalue].ToString(), xf);
                }
            }
        }

        private void Merge(ref int pos, ref int step, int i, DataTable dt, Cells cells, string colname)
        {
            //合并单元格
            string name = dt.Rows[i][colname].ToString();
            string nextname = "";
            if (i < dt.Rows.Count - 1)
            {
                nextname = dt.Rows[i + 1][colname].ToString();
                if (name == nextname)
                {
                    step++;
                }
                else
                {
                    cells.Merge(pos, pos + step, 1, 1);
                    pos = pos + step + 1;
                    step = 0;
                }
            }
            else
            {
                cells.Merge(pos, pos + step, 1, 1);
            }
        }
    }
}