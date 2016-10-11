using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL
{
    public class Role
    {
        public static bool Exists(string role)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from plat_Role");
            strSql.Append(" where RoleID=@RoleID");
            MySqlParameter[] parameters = {
					new MySqlParameter("@RoleID", MySqlDbType.VarChar,32)
			};
            parameters[0].Value = role;

            return DBUtility.MySqlHelper.Exists(strSql.ToString(), parameters);
        }

        public static bool Add(SMS.Model.Role role)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_Role(");
            strSql.Append("RoleID,RoleName,AddTime,Remark)");
            strSql.Append(" values (");
            strSql.Append("@RoleID,@RoleName,@AddTime,@Remark)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@RoleID", MySqlDbType.VarChar,32),
					new MySqlParameter("@RoleName", MySqlDbType.VarChar,32),
					new MySqlParameter("@AddTime", MySqlDbType.DateTime),
					new MySqlParameter("@Remark", MySqlDbType.VarChar,128)};
            parameters[0].Value = role.RoleID;
            parameters[1].Value = role.RoleName;
            parameters[2].Value = DateTime.Now;
            parameters[3].Value = role.Remark;

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

        public static bool Update(SMS.Model.Role role)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_Role set ");
            strSql.Append("RoleName=@RoleName,");
            strSql.Append("AddTime=@AddTime,");
            strSql.Append("Remark=@Remark");
            strSql.Append(" where RoleID=@RoleID");
            MySqlParameter[] parameters = {
					new MySqlParameter("@RoleID", MySqlDbType.VarChar,32),
					new MySqlParameter("@RoleName", MySqlDbType.VarChar,32),
					new MySqlParameter("@AddTime", MySqlDbType.DateTime),
					new MySqlParameter("@Remark", MySqlDbType.VarChar,128)};
            parameters[0].Value = role.RoleID;
            parameters[1].Value = role.RoleName;
            parameters[2].Value = role.AddTime;
            parameters[3].Value = role.Remark;

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

        public static bool Del(string role)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_Role ");
            strSql.Append(" where RoleID=@RoleID");
            MySqlParameter[] parameters = {
					new MySqlParameter("@RoleID", MySqlDbType.VarChar,32)
			};
            parameters[0].Value = role;

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

        public static SMS.Model.Role GetRole(string role)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_Role ");
            strSql.Append(" where RoleID=@RoleID");
            MySqlParameter[] parameters = {
					new MySqlParameter("@RoleID", MySqlDbType.VarChar,32)
			};
            parameters[0].Value = role;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public static List<SMS.Model.Role> GetRoles()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_Role ");

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            List<SMS.Model.Role> list = new List<SMS.Model.Role>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(DataRowToModel(row));
                }
            }
            return list;
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        static SMS.Model.Role DataRowToModel(DataRow row)
        {
            SMS.Model.Role model = new SMS.Model.Role();
            if (row != null)
            {
                if (row["RoleID"] != null)
                {
                    model.RoleID = row["RoleID"].ToString();
                }
                if (row["RoleName"] != null)
                {
                    model.RoleName = row["RoleName"].ToString();
                }
                if (row["AddTime"] != null && row["AddTime"].ToString() != "")
                {
                    model.AddTime = DateTime.Parse(row["AddTime"].ToString());
                }
                if (row["Remark"] != null)
                {
                    model.Remark = row["Remark"].ToString();
                }
            }
            return model;
        }
    }
}
