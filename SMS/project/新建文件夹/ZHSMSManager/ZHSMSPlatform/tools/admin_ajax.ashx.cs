using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.SessionState;
using Common;
using System.Xml;
using System.Text.RegularExpressions;

namespace DTcms.Web.tools
{
    /// <summary>
    /// 管理后台AJAX处理页
    /// </summary>
    public class admin_ajax : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            //取得处事类型
            string action = DTRequest.GetQueryString("action");

            switch (action)
            {
                case "sys_channel_load": //加载管理菜单
                    sys_channel_load(context);
                    break;
                case "plugins_nav_load": //加载管理菜单
                    plugins_nav_load(context);
                    break;
                case "sys_channel_validate": //验证频道名称是否重复
                    sys_channel_validate(context);
                    break;
                case "sys_model_nav_del": //删除系统模型菜单
                    sys_model_nav_del(context);
                    break;
            }
        }

        private DataTable CreateMenuTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.Int32"));
            dt.Columns[0].AutoIncrement = true;
            dt.Columns[0].AutoIncrementSeed = 1;
            dt.Columns[0].AutoIncrementStep = 1;
            dt.Columns.Add("parentName", Type.GetType("System.String"));
            dt.Columns.Add("name", Type.GetType("System.String"));
            dt.Columns.Add("value", Type.GetType("System.String"));
            dt.Columns.Add("imgurl", Type.GetType("System.String"));
            dt.Columns.Add("url", Type.GetType("System.String"));
            dt.Columns.Add("target", Type.GetType("System.String"));
            dt.Columns.Add("visible", Type.GetType("System.String"));
            return dt;
        }

        #region
        private void sys_channel_load(HttpContext context)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Utils.GetMapPath("../xmlconfig/Config.xml"));
            xmldoc.InnerXml = Regex.Replace(xmldoc.InnerXml, @"<!--(?s).*?-->", "", RegexOptions.IgnoreCase);
            XmlNodeList xnl = xmldoc.SelectNodes("Roots/Root");

            DataTable sysMenuTable = CreateMenuTable();
            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;
                foreach (XmlNode cxn in xe.ChildNodes)
                {
                    DataRow row = sysMenuTable.NewRow();
                    XmlElement cxe = (XmlElement)cxn;
                    row["parentName"] = xe.GetAttribute("name");
                    row["name"] = cxn.InnerText;
                    row["value"] = cxe.GetAttribute("id");
                    row["imgurl"] = cxe.GetAttribute("imgurl");
                    row["url"] = cxe.GetAttribute("url");
                    row["target"] = cxe.GetAttribute("target");
                    row["visible"] = cxe.GetAttribute("visible");
                    sysMenuTable.Rows.Add(row);
                }
            }
            Model.SysAccount account = (Model.SysAccount)System.Web.HttpContext.Current.Session["Login"];
            DataTable dtMenu = CreateMenuTable();
            if (account.UserCode != "admin")
            {
                List<Model.Permission> list = BLL.Permission.GetAccountPermissions(account.Roles);

                foreach (var v in list)
                {
                    foreach (DataRow row in sysMenuTable.Rows)
                    {
                        if (row["value"].ToString() == v.MenuID)
                        {
                            DataRow dr = dtMenu.NewRow();
                            dr["parentName"] = row["parentName"];
                            dr["name"] = row["name"];
                            dr["value"] = row["value"];
                            dr["imgurl"] = row["imgurl"];
                            dr["url"] = row["url"];
                            dr["target"] = row["target"];
                            dr["visible"] = row["visible"];
                            dtMenu.Rows.Add(dr);
                        }
                    }
                }
            }
            else
            {
                dtMenu = sysMenuTable.Copy();
            }
            List<string> sysModuleNames = new List<string>();
            foreach (DataRow row in dtMenu.Rows)
            {
                if (!sysModuleNames.Contains(row["parentName"].ToString()))
                {
                    sysModuleNames.Add(row["parentName"].ToString());
                }
            }

            StringBuilder strTxt = new StringBuilder();
            strTxt.Append("[");
            int i = 1;
            foreach (var s in sysModuleNames)
            {
                if (s == "短信管理") continue;
                strTxt.Append("{");
                strTxt.Append("\"text\":\"" + s + "\",");
                strTxt.Append("\"isexpand\":\"false\",");
                strTxt.Append("\"children\":[");

                int j = 1;
                foreach (DataRow dr in dtMenu.Rows)
                {
                    if (dr["parentName"].ToString() == s)
                    {
                        if (dr["name"].ToString() == "代理商终端用户") continue;
                        strTxt.Append("{");
                        strTxt.Append("\"text\":\"" + dr["name"].ToString() + "\",");
                        strTxt.Append("\"url\":\"" + dr["url"].ToString() + "?channel_id=\"");
                        strTxt.Append("}");
                        //DataView dv = new DataView(dtMenu);
                        //dv.RowFilter = "parentName='" + s + "'";
                        //int a=  dv.ToTable().Rows.Count;
                        //object o = dtMenu.Compute("Count(parentName)", "parentName='" + s + "'");
                        
                        //if (j < int.Parse(dtMenu.Compute("Count(parentName)", "parentName='" + s + "'").ToString()))
                        if (j < int.Parse(dtMenu.Compute("Count(parentName)", "parentName='" + s + "' and name <>'代理商终端用户'").ToString()))
                        {
                            strTxt.Append(",");
                        }
                        j++;
                    }
                }
                strTxt.Append("]");
                strTxt.Append("}");
                strTxt.Append(",");
                i++;
            }

            string newTxt = Utils.DelLastChar(strTxt.ToString(), ",") + "]";
            context.Response.Write(newTxt);


            //string name = "";
            //string value = "";
            //string imgurl = "";
            //string url = "";
            //string target = "";
            //string visible = "";
            //StringBuilder strTxt = new StringBuilder();
            //strTxt.Append("[");
            //int i = 1;
            //foreach (XmlNode xn in xnl)
            //{
            //    XmlElement xe = (XmlElement)xn;
            //    name = xe.GetAttribute("name");
            //    value = xe.GetAttribute("id");
            //    imgurl = xe.GetAttribute("imgurl");
            //    url = xe.GetAttribute("url");
            //    target = xe.GetAttribute("target");
            //    visible = xe.GetAttribute("visible");

            //    strTxt.Append("{");
            //    strTxt.Append("\"text\":\"" + name + "\",");
            //    strTxt.Append("\"isexpand\":\"false\",");
            //    strTxt.Append("\"children\":[");
            //    if (visible == "1")
            //    {
            //        int j = 1;
            //        foreach (XmlNode cxn in xe.ChildNodes)
            //        {
            //            XmlElement cxe = (XmlElement)cxn;
            //            name = cxe.InnerText;
            //            value = cxe.GetAttribute("value");
            //            imgurl = cxe.GetAttribute("imgurl");
            //            url = cxe.GetAttribute("url");
            //            target = cxe.GetAttribute("target");
            //            visible = cxe.GetAttribute("visible");
            //            strTxt.Append("{");
            //            strTxt.Append("\"text\":\"" + name + "\",");
            //            strTxt.Append("\"url\":\"" + url + "?channel_id=" + value + "\""); //此处要优化，加上nav.nav_url网站目录标签替换
            //            strTxt.Append("}");
            //            if (j < xe.ChildNodes.Count)
            //            {
            //                strTxt.Append(",");
            //            }
            //            j++;
            //        }
            //    }
            //    strTxt.Append("]");
            //    strTxt.Append("}");
            //    strTxt.Append(",");
            //    i++;
            //}

            //string newTxt = Utils.DelLastChar(strTxt.ToString(), ",") + "]";
            //context.Response.Write(newTxt);
        }

        #endregion

        #region 加载插件管理菜单================================
        private void plugins_nav_load(HttpContext context)
        {
            //BLL.plugin bll = new BLL.plugin();
            //DirectoryInfo dirInfo = new DirectoryInfo(Utils.GetMapPath("../plugins/"));
            //foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            //{
            //    Model.plugin aboutInfo = bll.GetInfo(dir.FullName + @"\");
            //    if (aboutInfo.isload == 1 && File.Exists(dir.FullName + @"\admin\index.aspx"))
            //    {
            //        context.Response.Write("<li><a class=\"l-link\" href=\"javascript:f_addTab('plugin_" + dir.Name
            //            + "','" + aboutInfo.name + "','../../plugins/" + dir.Name + "/admin/index.aspx')\">" + aboutInfo.name + "</a></li>\n");
            //    }
            //}
            return;
        }
        #endregion

        #region 验证频道名称是否重复============================
        private void sys_channel_validate(HttpContext context)
        {
            //string channelname = DTRequest.GetFormString("channelname");
            //string oldname = DTRequest.GetFormString("oldname");
            //if (string.IsNullOrEmpty(channelname))
            //{
            //    context.Response.Write("false");
            //    return;
            //}
            ////检查是否与站点根目录下的目录同名
            //Model.siteconfig siteConfig = new BLL.siteconfig().loadConfig(Utils.GetXmlMapPath("Configpath"));
            //DirectoryInfo dirInfo = new DirectoryInfo(Utils.GetMapPath(siteConfig.webpath));
            //foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            //{
            //    if (channelname.ToLower() == dir.Name)
            //    {
            //        context.Response.Write("false");
            //        return;
            //    }
            //}
            ////检查是否修改操作
            //if (channelname == oldname)
            //{
            //    context.Response.Write("true");
            //    return;
            //}
            ////检查Key是否与已存在
            //BLL.sys_channel bll = new BLL.sys_channel();
            //if (bll.Exists(channelname))
            //{
            //    context.Response.Write("false");
            //    return;
            //}
            //context.Response.Write("true");
            //return;
            return;
        }
        #endregion

        #region 删除系统模型菜单================================
        private void sys_model_nav_del(HttpContext context)
        {
            //string _nav_id = context.Request.Form["nav_id"];
            //if (string.IsNullOrEmpty(_nav_id))
            //{
            //    context.Response.Write("{msg:0, msgbox:\"对不起，无法获得所要删除的菜单项！\"}");
            //    return;
            //}
            //int nav_id;
            //if (!int.TryParse(_nav_id, out nav_id))
            //{
            //    context.Response.Write("{msg:0, msgbox:\"对不起，数据在转换过程中发生错误！\"}");
            //    return;
            //}
            //BLL.sys_model_nav bll = new BLL.sys_model_nav();
            //if (!bll.Exists(nav_id))
            //{
            //    context.Response.Write("{msg:0, msgbox:\"对不起，您所删除的菜单项不存在！\"}");
            //    return;
            //}
            //bll.Delete(nav_id);
            //context.Response.Write("{msg:1, msgbox:\"删除菜单项成功啦！\"}");
            //return;
        }
        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}