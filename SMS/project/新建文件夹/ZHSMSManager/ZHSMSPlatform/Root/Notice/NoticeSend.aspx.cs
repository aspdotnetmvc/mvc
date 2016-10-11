using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Notice
{
    public partial class NoticeSend : System.Web.UI.Page
    {
        private const string add = "Notice.Send.Add";
        protected void Page_Load(object sender, EventArgs e)
        {
            TreeView1.Attributes.Add("onclick", "postBackByObject()");
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                this.ViewState["Permissions"] = BLL.Permission.GetPermissionByAccount(account.UserCode);
            }
            if (!IsPostBack)
            {
                loadtata();
            }
        }
        public void treeband()
        {
            DataTable dt = (DataTable)Session["dt"];
            TreeView1.Nodes.Clear();
            if (dt.Rows.Count > 0)
            {
                TreeNode nd = new TreeNode();
                nd.Text = "所有人";
                nd.Value = "-1";
                TreeView1.Nodes.Add(nd);
                TreeNode nd1 = new TreeNode();
                nd1.Text = "代理商";
                nd1.Value = "1";
                addChildNodes(dt, nd1, "1");
                TreeView1.Nodes.Add(nd1);
                TreeNode nd2 = new TreeNode();
                nd2.Text = "终端客户";
                nd2.Value = "0";
                addChildNodes(dt, nd2, "0");
                TreeView1.Nodes.Add(nd2);
            }
        }
        protected void addChildNodes(DataTable dtbl, TreeNode parentNode, string isagent)
        {
            if (dtbl != null && dtbl.Rows.Count > 0)
            {
                for (int i = 0; i < dtbl.Rows.Count; i++)
                {
                    TreeNode tn = new TreeNode();
                    if (dtbl.Rows[i]["isagent"].ToString() == isagent)
                    {
                        tn.Text = dtbl.Rows[i]["name"].ToString();
                        tn.Value = dtbl.Rows[i]["code"].ToString();
                        parentNode.ChildNodes.Add(tn);
                    }

                }
            }
        }

        void loadtata()
        {
            DataTable dt = CreateTable();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            string condtion = txt_search.Value;
            SMSModel.RPCResult<List<Model.EnterpriseUser>> r = ZHSMSProxy.GetZHSMSPlatService().GetAgentEnterprises();
            if (r.Success)
            {
                List<Model.EnterpriseUser> enterprises = r.Value;
                if (account.UserCode == "admin")
                {
                    foreach (Model.EnterpriseUser a in enterprises)
                    {
                        DataRow dr = dt.NewRow();
                        dr["accountID"] = a.AccountID;
                        dr["code"] = a.AccountCode;
                        dr["name"] = a.Name;
                        dr["contact"] = a.Contact;
                        dr["phone"] = a.Phone;
                        dr["address"] = a.Address;
                        dr["isagent"] = a.IsAgent == true ? 1 : 0;
                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    List<string> list = BLL.AccountEnterprise.GetEnterpriseByUserCode(account.UserCode);
                    if (list.Count == 1 && list[0] == "-1")
                    {
                        foreach (Model.EnterpriseUser a in enterprises)
                        {
                            DataRow dr = dt.NewRow();
                            dr["accountID"] = a.AccountID;
                            dr["code"] = a.AccountCode;
                            dr["name"] = a.Name;
                            dr["contact"] = a.Contact;
                            dr["phone"] = a.Phone;
                            dr["address"] = a.Address;
                            dr["isagent"] = a.IsAgent == true ? 1 : 0;
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        foreach (Model.EnterpriseUser a in enterprises)
                        {
                            if (list.Contains(a.AccountCode))
                            {
                                DataRow dr = dt.NewRow();
                                dr["accountID"] = a.AccountID;
                                dr["code"] = a.AccountCode;
                                dr["name"] = a.Name;
                                dr["contact"] = a.Contact;
                                dr["phone"] = a.Phone;
                                dr["address"] = a.Address;
                                dr["isagent"] = a.IsAgent == true ? 1 : 0;
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }
                SMSModel.RPCResult<List<Model.EnterpriseUser>> rr = ZHSMSProxy.GetZHSMSPlatService().GetLowerEnterprise();
                if (rr.Success)
                {
                    if (account.UserCode == "admin")
                    {
                        foreach (Model.EnterpriseUser a in rr.Value)
                        {
                            DataRow dr = dt.NewRow();
                            dr["accountID"] = a.AccountID;
                            dr["code"] = a.AccountCode;
                            dr["name"] = a.Name;
                            dr["contact"] = a.Contact;
                            dr["phone"] = a.Phone;
                            dr["address"] = a.Address;
                            dr["isagent"] = a.IsAgent == true ? 1 : 0;
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        List<string> list = BLL.AccountEnterprise.GetEnterpriseByUserCode(account.UserCode);
                        if (list.Count == 1 && list[0] == "-1")
                        {
                            foreach (Model.EnterpriseUser a in rr.Value)
                            {
                                DataRow dr = dt.NewRow();
                                dr["accountID"] = a.AccountID;
                                dr["code"] = a.AccountCode;
                                dr["name"] = a.Name;
                                dr["contact"] = a.Contact;
                                dr["phone"] = a.Phone;
                                dr["address"] = a.Address;
                                dr["isagent"] = a.IsAgent == true ? 1 : 0;
                                dt.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            foreach (Model.EnterpriseUser a in rr.Value)
                            {
                                if (list.Contains(a.AccountCode))
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["accountID"] = a.AccountID;
                                    dr["code"] = a.AccountCode;
                                    dr["name"] = a.Name;
                                    dr["contact"] = a.Contact;
                                    dr["phone"] = a.Phone;
                                    dr["address"] = a.Address;
                                    dr["isagent"] = a.IsAgent == true ? 1 : 0;
                                    dt.Rows.Add(dr);
                                }
                            }
                        }
                    }
                }
                if (condtion != "")
                {
                    DataView dv = new DataView(dt);
                    dv.RowFilter = "code like '%" + condtion + "%' or name like '%" + condtion + "%'";
                    dt = dv.ToTable();
                }
                Session["dt"] = dt;
                treeband();
            }
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
            table.Columns.Add("isagent", Type.GetType("System.String"));
            return table;
        }

        protected void btn_search_Click(object sender, EventArgs e)
        {
            loadtata();
        }

        protected void TreeView1_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {

            SetChildChecked(e.Node);
            if (e.Node.Parent != null)
            {
                SetParentChecked(e.Node);
            }
        }

        private void SetChildChecked(TreeNode parentNode)
        {
            foreach (TreeNode node in parentNode.ChildNodes)
            {
                node.Checked = parentNode.Checked;
                if (node.ChildNodes.Count > 0)
                {
                    SetChildChecked(node);
                }
            }
        }
        private void SetParentChecked(TreeNode childNode)
        {
            TreeNode parentNode = childNode.Parent;
            if (!parentNode.Checked && childNode.Checked)
            {
                int ichecks = 0;
                foreach (TreeNode node in parentNode.ChildNodes)
                {
                    if (node.Checked)
                    {
                        ichecks++;
                    }
                }
                if (ichecks == parentNode.ChildNodes.Count)
                {
                    parentNode.Checked = true;
                    if (parentNode.Parent != null)
                    {
                        SetParentChecked(parentNode);
                    }
                }
            }
            else if (parentNode.Checked && !childNode.Checked)
            {
                parentNode.Checked = false;
            }
        }

        protected void btn_Sendnotice_Click(object sender, EventArgs e)
        {

            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, add);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
            if (txt_title.Value == "")
            {
                Message.Alert(this, "请输入公告标题", "null");
                return;
            }
            if (txt_content.Value == "")
            {
                Message.Alert(this, "请输入公告内容", "null");
                return;
            }
            Model.Annunciate an = new Model.Annunciate();
            an.AnnunciateContent = txt_content.Value;
            an.AnnunciateTitle = txt_title.Value;
            an.AnnunciateAccount = account.UserCode;
            an.CreateTime = DateTime.Now;
            an.PlatType = 0;
            an.Type = Model.AnnunciateType.Person;
            List<string> num = new List<string>();
            for (int i = 0; i < TreeView1.CheckedNodes.Count; i++)
            {
                if (TreeView1.CheckedNodes[i].Checked)
                {
                    string GetValue = TreeView1.CheckedNodes[i].Value;
                    num.Add(GetValue);
                }
            }
            an.Users = num;
            if (num.Contains("-1"))
            {
                an.Type = Model.AnnunciateType.All;
                num.Remove("-1");
                if (num.Contains("1"))
                {
                    num.Remove("1");
                }
                if (num.Contains("0"))
                {
                    num.Remove("0");
                }
            }
            else
            {
                if (num.Contains("0") && num.Contains("1"))
                {
                    an.Type = Model.AnnunciateType.All;
                    num.Remove("0");
                    num.Remove("1");
                }
                else
                {
                    if (num.Contains("1"))
                    {
                        an.Type = Model.AnnunciateType.Person;
                        num.Remove("1");

                    }
                    if (num.Contains("0"))
                    {
                        an.Type = Model.AnnunciateType.Person;
                        num.Remove("0");

                    }
                }
            }
            SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().AddAunnuciateByManage(an);
            if (r.Success)
            {
                Message.Success(this, "发送公告成功", "null");
            }
            else
            {
                Message.Alert(this, r.Message, "null");
                return;
            }

        }
    }
}