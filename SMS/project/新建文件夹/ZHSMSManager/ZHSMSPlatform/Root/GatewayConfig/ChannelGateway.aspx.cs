using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.GatewayConfig
{
    public partial class ChannelGateway : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue  = "Channel.Manage.RelateGateway";
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
        private void load()
        {
            string channelID = Request.QueryString["channel"];
            DataTable dt = this.CreateGatewayTable();
            SMSModel.RPCResult<List<SMSModel.GatewayConfiguration>> rt = ZHSMSProxy.GetZHSMSPlatService().GetGatewayConfigs();
            if (rt.Success)
            {
                foreach (var v in rt.Value)
                {
                    DataRow dr = dt.NewRow();
                    dr["Gateway"] = v.Gateway;
                    if (v.Operators.Count == 1)
                    {
                        dr["Operators"] = v.Operators[0];
                    }
                    else if(v.Operators.Count==2)
                    {
                        string str = "";
                        foreach (var o in v.Operators)
                        {
                            str += o + "_";
                        }
                        dr["Operators"] = str == "" ? "" : str.Substring(0, str.Length - 1);
                    }
                    else if (v.Operators.Count == 3)
                    {
                        dr["Operators"] = "three";
                    }
                    dt.Rows.Add(dr);
                }
                this.ViewState["gateway"] = dt;
            }
            else
            {
                Message.Alert(this, rt.Message, "null");
                btnSubmit.Visible = false;
                return;
            }
            
            SMSModel.RPCResult<SMSModel.Channel> r = ZHSMSProxy.GetZHSMSPlatService().GetSMSChannel(channelID);
            if (r.Success)
            {
                lbl_channelID.Text = r.Value.ChannelID;
                lbl_channelName.Text = r.Value.ChannelName;
                DataListGateway.DataSource = GetOperators();
                DataListGateway.DataBind();
                ChannelGatewayBindLoad();
            }
            else
            {
                btnSubmit.Visible = false;
                Message.Alert(this, r.Message, "null");
            }
        }

        private void ChannelGatewayBindLoad()
        {
            foreach (DataListItem item in DataListGateway.Items)
            {
                System.Web.UI.WebControls.CheckBoxList cbk = (CheckBoxList)item.FindControl("Application_ID");
                foreach (ListItem li in cbk.Items)
                {
                    li.Selected = false;
                }

            }
            SMSModel.RPCResult<List<string>> r = ZHSMSProxy.GetZHSMSPlatService().GetGatewaysByChannel(lbl_channelID.Text);
            if (r.Success)
            {
                foreach (string str in r.Value)
                {
                    foreach (DataListItem item in DataListGateway.Items)
                    {
                        System.Web.UI.WebControls.CheckBoxList cbk = (CheckBoxList)item.FindControl("Application_ID");
                        for (int i = 0; i < cbk.Items.Count; i++)
                        {
                            if (cbk.Items[i].Value == str)
                            {
                                cbk.Items[i].Selected = true;
                            }
                        }
                    }
                }
            }
            else
            {
                btnSubmit.Visible = false;
                Message.Alert(this, r.Message, "null");
            }
        }

        public void DataListGateway_ItemDataBound(Object sender, DataListItemEventArgs e)
        {
            Label code = (Label)e.Item.FindControl("lbl_operators");
            if (code != null)
            {
                System.Web.UI.WebControls.CheckBoxList cbk = (System.Web.UI.WebControls.CheckBoxList)e.Item.FindControl("Application_ID");
                System.Web.UI.WebControls.Label lbl_item = (System.Web.UI.WebControls.Label)e.Item.FindControl("lbl_message");
                DataTable dt =(DataTable)this.ViewState["gateway"];
                int count = 0;
                foreach (DataRow row in dt.Rows )
                {
                    if (code.Text.Split('_').Length == 2)
                    {
                        string[] arr = code.Text.Split('_');
                        string s1 = arr[0] + "_" + arr[1];
                        string s2 = arr[1] + "_" + arr[0];
                        if (s1 == row["Operators"].ToString() || s2 == row["Operators"].ToString())
                        {
                            ListItem li = new ListItem();
                            li.Text = row["Gateway"].ToString();
                            li.Value = row["Gateway"].ToString();
                            cbk.Items.Add(li);
                            count++;
                        }
                    }
                    else
                    {
                        if (code.Text == row["Operators"].ToString())
                        {
                            ListItem li = new ListItem();
                            li.Text = row["Gateway"].ToString();
                            li.Value = row["Gateway"].ToString();
                            cbk.Items.Add(li);
                            count++;
                        }
                    }
                }
                if (count == 0)
                {
                    lbl_item.Visible = true;
                    lbl_item.Text = "无可用的网关";
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> gateways = new List<string>();

            foreach (DataListItem item in DataListGateway.Items)
            {
                System.Web.UI.WebControls.CheckBoxList cbk = (CheckBoxList)item.FindControl("Application_ID");
                for (int i = 0; i < cbk.Items.Count; i++)
                {
                    if (cbk.Items[i].Selected == true)
                    {
                        gateways.Add(cbk.Items[i].Value);
                    }
                }
            }

            if (gateways.Count > 0)
            {
                SMSModel.RPCResult ok = ZHSMSProxy.GetZHSMSPlatService().AddChannelGatewayBind(lbl_channelID.Text, gateways);// BLL.Permission.AddRolePermission(roleID, gateways);
                if (ok.Success)
                {
                    Message.Success(this, "添加成功！", "null");
                }
                else
                {
                    Message.Alert(this, ok.Message, "null");
                }
            }
            else
            {
                Message.Alert(this, "请选择通道要绑定的网关", "null");
                return;
            }
        }
        private DataTable GetOperators()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.Int32"));
            dt.Columns[0].AutoIncrement = true;
            dt.Columns[0].AutoIncrementSeed = 1;
            dt.Columns[0].AutoIncrementStep = 1;
            dt.Columns.Add("Code", Type.GetType("System.String"));
            dt.Columns.Add("Name", Type.GetType("System.String"));
            DataRow dr = dt.NewRow();
            dr["Code"] = "unicom";
            dr["Name"] = "联通";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Code"] = "mobile";
            dr["Name"] = "移动";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Code"] = "telecom";
            dr["Name"] = "电信";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Code"] = "unicom_mobile";
            dr["Name"] = "联通、移动";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Code"] = "unicom_telecom";
            dr["Name"] = "联通、电信";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Code"] = "mobile_telecom";
            dr["Name"] = "移动、电信";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Code"] = "three";
            dr["Name"] = "联通、移动、电信";
            dt.Rows.Add(dr);
            return dt;
        }
        private DataTable CreateGatewayTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.Int32"));
            dt.Columns[0].AutoIncrement = true;
            dt.Columns[0].AutoIncrementSeed = 1;
            dt.Columns[0].AutoIncrementStep = 1;
            dt.Columns.Add("Gateway", Type.GetType("System.String"));
            dt.Columns.Add("Operators", Type.GetType("System.String"));
            return dt;
        }
    }
}