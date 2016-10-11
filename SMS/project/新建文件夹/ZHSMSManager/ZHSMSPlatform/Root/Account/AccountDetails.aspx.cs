using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace ZHCRM.Root.Account
{
    public partial class AccountDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "System.Account.Detail";
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
            if (!IsPostBack)
            {
                load();
            }
        }
        void load()
        {
            string usercode = Request.QueryString["accountID"];
            Model.SysAccount account = BLL.SysAccount.GetAccount(usercode);
            if (account != null)
            {
                lbl_accountID.Text = account.UserCode;
                lbl_accountName.Text = account.UserCode;
                lbl_status.Text = account.Status == true ? "启用" : "禁用";
            }
            List<Model.Role> list = BLL.Role.GetRoles();
            foreach (string s in account.Roles)
            {
                lbl_roles.Text += list.FirstOrDefault<Model.Role>(c => c.RoleID == s).RoleName + "  ";
            }
            List<Model.Permission> pers = BLL.Permission.GetAccountPermissions(account.Roles);
            LoadPermission(pers);
        }

        private void LoadPermission(List<Model.Permission> pers)
        {
            this.DataListSysModule.DataSource = GetSysModules();
            this.DataBind();
            RolePermissionLoad(pers);
        }

        private DataTable GetSysModules()
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Utils.GetMapPath("../../xmlconfig/Config.xml"));
            xmldoc.InnerXml = Regex.Replace(xmldoc.InnerXml, @"<!--(?s).*?-->", "", RegexOptions.IgnoreCase);
            XmlNodeList xnl = xmldoc.SelectNodes("Roots/Root");

            DataTable dt = CreateSysModuleTable();
            DataTable child = CreateMenuTable();
            foreach (XmlNode xn in xnl)
            {
                DataRow dr = dt.NewRow();
                XmlElement xe = (XmlElement)xn;
                dr["Code"] = xe.GetAttribute("id");
                dr["Title"] = xe.GetAttribute("name");
                dt.Rows.Add(dr);

                foreach (XmlNode cxn in xe.ChildNodes)
                {
                    DataRow row = child.NewRow();
                    XmlElement cxe = (XmlElement)cxn;
                    row["ParentCode"] = xe.GetAttribute("id");
                    row["ChildCode"] = cxe.GetAttribute("id");
                    row["ChildTitle"] = cxn.InnerText;
                    child.Rows.Add(row);
                }

            }
            this.ViewState["childTable"] = child;
            return dt;
        }

        private void RolePermissionLoad(List<Model.Permission> list)
        {
            foreach (DataListItem item in DataListSysModule.Items)
            {
                System.Web.UI.WebControls.DataList dlChild = (DataList)item.FindControl("DataListChild");
                foreach (DataListItem dl in dlChild.Items)
                {
                    System.Web.UI.WebControls.CheckBoxList cbk = (CheckBoxList)dl.FindControl("Application_ID");
                    foreach (ListItem li in cbk.Items)
                    {
                        li.Selected = false;
                    }
                }

            }

            DataTable dt = new DataTable();
            foreach (var v in list)
            {
                foreach (var vv in v.Actions)
                {
                    foreach (DataListItem item in DataListSysModule.Items)
                    {
                        System.Web.UI.WebControls.DataList dlChild = (DataList)item.FindControl("DataListChild");
                        foreach (DataListItem dl in dlChild.Items)
                        {
                            System.Web.UI.WebControls.CheckBoxList cbk = (CheckBoxList)dl.FindControl("Application_ID");
                            for (int j = 0; j < cbk.Items.Count; j++)
                            {
                                if (cbk.Items[j].Value == vv.PermissionCode)
                                {
                                    cbk.Items[j].Selected = true;
                                }
                            }
                        }
                    }
                }

            }
        }

        public void DataListSysModule_ItemDataBound(Object sender, DataListItemEventArgs e)
        {
            Label code = (Label)e.Item.FindControl("lbl_SysModule");
            if (code != null)
            {
                System.Web.UI.WebControls.CheckBoxList cbk = (System.Web.UI.WebControls.CheckBoxList)e.Item.FindControl("Application_ID");
                System.Web.UI.WebControls.DataList dl = (System.Web.UI.WebControls.DataList)e.Item.FindControl("DataListChild");
                dl.DataSource = GetSysModuleMenu(code.Text.Trim());
                dl.DataBind();
            }
        }
        public void DataListChild_ItemDataBound(Object sender, DataListItemEventArgs e)
        {
            Label code = (Label)e.Item.FindControl("lbl_menuCode");
            if (code != null)
            {
                System.Web.UI.WebControls.CheckBoxList cbk = (System.Web.UI.WebControls.CheckBoxList)e.Item.FindControl("Application_ID");
                DataTable dt1 = GetMenuPermission(code.Text.Trim());
                for (int j = 0; j < dt1.Rows.Count; j++)
                {
                    ListItem li = new ListItem();
                    li.Text = dt1.Rows[j]["PermissionTitle"].ToString();
                    li.Value = dt1.Rows[j]["PermissionCode"].ToString();
                    cbk.Items.Add(li);
                }
            }
        }
        DataTable GetSysModuleMenu(string SysModuleCode)
        {
            DataTable dt = CreateMenuTable();
            foreach (DataRow row in ((DataTable)this.ViewState["childTable"]).Rows)
            {
                if (row["ParentCode"].ToString() == SysModuleCode)
                {
                    DataRow dr = dt.NewRow();
                    dr["ParentCode"] = row["ParentCode"];
                    dr["ChildCode"] = row["ChildCode"];
                    dr["ChildTitle"] = row["ChildTitle"];
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        DataTable GetMenuPermission(string ChildCode)
        {
            DataTable dt = CreatePermissionTable();
            List<Model.PermissionAction> list = BLL.Permission.GetPermissionByMenucode(ChildCode);
            foreach (var v in list)
            {
                DataRow dr = dt.NewRow();
                dr["PermissionCode"] = v.PermissionCode;
                dr["PermissionTitle"] = v.PermissionTitle;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        DataTable CreateSysModuleTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("Code", Type.GetType("System.String"));
            table.Columns.Add("Title", Type.GetType("System.String"));
            return table;
        }
        DataTable CreateMenuTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("ParentCode", Type.GetType("System.String"));
            table.Columns.Add("ChildCode", Type.GetType("System.String"));
            table.Columns.Add("ChildTitle", Type.GetType("System.String"));
            return table;
        }
        DataTable CreatePermissionTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("PermissionCode", Type.GetType("System.String"));
            table.Columns.Add("PermissionTitle", Type.GetType("System.String"));
            return table;
        }
    }
}