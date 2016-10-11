using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL
{
    public class Permission
    {
        public static List<SMS.Model.Permission> GetPermissionMenus()
        {
            return new List<SMS.Model.Permission>();
        }

        public static List<SMS.Model.Permission> GetRolePermissions(string role)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select p.* from plat_RolePermission as rp left join plat_Permission as p on rp.PermissionCode=p.PermissionCode and rp.RoleID=@RoleID");
            strSql.Append(" where RoleID=@RoleID");
            MySqlParameter[] parameters = {
					new MySqlParameter("@RoleID", MySqlDbType.VarChar,32)
			};
            parameters[0].Value = role;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            List<SMS.Model.Permission> list = new List<SMS.Model.Permission>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string menuCode = row["MenuCode"].ToString();
                    SMS.Model.PermissionAction action = DataRowToModel(row);
                    if (list.FirstOrDefault<SMS.Model.Permission>(c => c.MenuID == menuCode) != null)
                    {
                        SMS.Model.Permission permission = list.FirstOrDefault<SMS.Model.Permission>(c => c.MenuID == menuCode);
                        permission.Actions.Add(action);
                    }
                    else
                    {
                        SMS.Model.Permission permission = new SMS.Model.Permission();
                        permission.MenuID = menuCode;
                        permission.Actions = new List<SMS.Model.PermissionAction>();
                        permission.Actions.Add(action);
                        list.Add(permission);
                    }
                }
            }
            return list;
        }

        public static bool IsUsePermission(string account, string permissionCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_AccountRole as ar , plat_RolePermission as rp where ar.RoleID = rp.RoleID and ar.UserCode=@UserCode and rp.PermissionCode=@PermissionCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,64),
                    new MySqlParameter("@PermissionCode", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = account;
            parameters[1].Value = permissionCode;
            bool ok = DBUtility.MySqlHelper.Exists(strSql.ToString(), parameters);
            if (ok)
            {
                return true;
            }
            return false;
        }

        public static List<string> GetPermissionByAccount(string account)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select rp.PermissionCode from plat_AccountRole as ar,plat_RolePermission as rp where ar.RoleID = rp.RoleID and ar.UserCode=@UserCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = account;
            List<string> list = new List<string>();
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count>0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(row["PermissionCode"].ToString());
                }
            }
            return list;
        }

        public static List<SMS.Model.PermissionAction> GetPermissionByMenucode(string menuCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_Permission ");
            strSql.Append(" where MenuCode=@MenuCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@MenuCode", MySqlDbType.VarChar,32)
			};
            parameters[0].Value = menuCode;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            List<SMS.Model.PermissionAction> list = new List<SMS.Model.PermissionAction>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(DataRowToModel(row));
                }
            }
            return list;
        }

        public static bool DelRolePermisson(string roleID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_RolePermission ");
            strSql.Append(" where RoleID=@RoleID");
            MySqlParameter[] parameters = {
					new MySqlParameter("@RoleID", MySqlDbType.VarChar,32)
			};
            parameters[0].Value = roleID;

            DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            return true;
        }

        public static bool AddRolePermisson(string roleID, List<string> permissionCodes)
        {
            foreach (var v in permissionCodes)
            {
                Add(roleID, v);
            }
            return true;
        }

        static bool Add(string roleID, string permissionCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_RolePermission(");
            strSql.Append("RoleID,PermissionCode)");
            strSql.Append(" values (");
            strSql.Append("@RoleID,@PermissionCode)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@RoleID", MySqlDbType.VarChar,32),
					new MySqlParameter("@PermissionCode", MySqlDbType.VarChar,64)};
            parameters[0].Value = roleID;
            parameters[1].Value = permissionCode;

            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static SMS.Model.PermissionAction DataRowToModel(DataRow row)
        {
            SMS.Model.PermissionAction model = new SMS.Model.PermissionAction();
            if (row != null)
            {
                if (row["PermissionCode"] != null)
                {
                    model.PermissionCode = row["PermissionCode"].ToString();
                }
                if (row["PermissionTitle"] != null)
                {
                    model.PermissionTitle = row["PermissionTitle"].ToString();
                }
                if (row["OperateType"] != null && row["OperateType"].ToString() != "")
                {
                    model.OperateType = (SMS.Model.PermissionOperateType)((ushort)row["OperateType"]);
                }
                if (row["OperateContent"] != null)
                {
                    model.Content = row["OperateContent"].ToString();
                }
            }
            return model;
        }
    }
}
