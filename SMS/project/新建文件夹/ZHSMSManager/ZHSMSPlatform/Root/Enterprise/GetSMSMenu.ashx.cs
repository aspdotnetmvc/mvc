using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Xml;

namespace ZHSMSPlatform.Root.Enterprise
{
    /// <summary>
    /// GetSMSMenu 的摘要说明
    /// </summary>
    public class GetSMSMenu : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(Utils.GetMapPath("../../xmlconfig/Smsmenu.xml"));
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

                string strjson = "{\"SysModules\":[$SysModules],\"result\":\"ok\"}";
                System.Text.StringBuilder sysModules = new System.Text.StringBuilder();
                string sysModule = "{\"SysModuleTitle\":\"$SysModuleTitle\",\"SysModuleCss\":\"$SysModuleCss\",\"Menus\":[$Menus]},";

                string menu = "{\"MenuTitle\":\"$MenuTitle\",\"MenuUrl\":\"$MenuUrl\"},";

                string result = "";
                foreach (var s in sysModuleNames)
                {
                    System.Text.StringBuilder menus = new System.Text.StringBuilder();
                    foreach (DataRow dr in dtMenu.Rows)
                    {
                        if (dr["parentName"].ToString() == s)
                        {
                            menus.Append(menu.Replace("$MenuTitle", dr["name"].ToString()).Replace("$MenuUrl", dr["url"].ToString()));
                        }
                    }
                    string temp = menus.ToString();
                    if (!string.IsNullOrEmpty(temp))
                    {
                        temp = temp.Substring(0, temp.Length - 1);
                    }
                    sysModules.Append(sysModule.Replace("$SysModuleTitle", s).Replace("$SysModuleCss", "123")).Replace("$Menus", temp);
                }
                string strTemp = sysModules.ToString();
                if (!string.IsNullOrEmpty(strTemp))
                {
                    strTemp = strTemp.Substring(0, strTemp.Length - 1);
                }
                result = strjson.Replace("$SysModules", strTemp);
                context.Response.Write(result);
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
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
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}