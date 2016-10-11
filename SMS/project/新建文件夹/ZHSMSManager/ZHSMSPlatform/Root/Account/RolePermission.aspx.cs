using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace ZHSMSPlatform.Root.Account
{
    public partial class RolePermission : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "System.Role.PermissionEdite";
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
            if (!IsPostBack)
            {
                LoadPermission();
            }
        }
        private void LoadPermission()
        {
            string roleID = Request.QueryString["RoleID"];
            Model.Role role = BLL.Role.GetRole(roleID);
            if (role != null)
            {
                hf_roleID.Value = role.RoleID;
                lbl_roleName.Text = role.RoleName;
            }
            this.DataListSysModule.DataSource = GetSysModules();
            this.DataBind();
            RolePermissionLoad();
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

        private void RolePermissionLoad()
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
            List<Model.Permission> list = BLL.Permission.GetRolePermissions(hf_roleID.Value);
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string roleID = hf_roleID.Value;
            List<string> permissionCodes = new List<string>();

            foreach (DataListItem item in DataListSysModule.Items)
            {
                System.Web.UI.WebControls.DataList dlChild = (DataList)item.FindControl("DataListChild");
                foreach (DataListItem dl in dlChild.Items)
                {
                    System.Web.UI.WebControls.CheckBoxList cbk = (CheckBoxList)dl.FindControl("Application_ID");
                    for (int i = 0; i < cbk.Items.Count; i++)
                    {
                        if (cbk.Items[i].Selected == true)
                        {
                            permissionCodes.Add(cbk.Items[i].Value);
                        }
                    }
                }
            }

            if (permissionCodes.Count > 0)
            {
                bool ok = BLL.Permission.AddRolePermission(roleID, permissionCodes);
                if (ok)
                {
                    Message.Success(this, "添加成功！", "null");
                }
                else
                {
                    Message.Alert(this, "添加失败！", "null");
                }
            }
            else
            {
                Message.Alert(this, "请选择菜单名称", "null");
                return;
            }
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