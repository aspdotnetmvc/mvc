using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL
{
    public class Annunciatecs
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static bool Add(SMS.Model.Annunciate model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_annunciate(");
            strSql.Append("AnnunciateID,AnnunciateAccount,AnnunciateTitle,AnnunciateContent,CreateTime,AnnunciateType,UserLists,PlatType)");
            strSql.Append(" values (");
            strSql.Append("@AnnunciateID,@AnnunciateAccount,@AnnunciateTitle,@AnnunciateContent,@CreateTime,@AnnunciateType,@UserLists,@PlatType)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AnnunciateID", MySqlDbType.VarChar,64),
					new MySqlParameter("@AnnunciateAccount", MySqlDbType.VarChar,64),
					new MySqlParameter("@AnnunciateTitle", MySqlDbType.VarChar,256),
					new MySqlParameter("@AnnunciateContent", MySqlDbType.Text),
					new MySqlParameter("@CreateTime", MySqlDbType.DateTime),
					new MySqlParameter("@AnnunciateType", MySqlDbType.Int32,11),
					new MySqlParameter("@UserLists", MySqlDbType.LongText),
					new MySqlParameter("@PlatType", MySqlDbType.Int32,11)};
            parameters[0].Value = model.AnnunciateID;
            parameters[1].Value = model.AnnunciateAccount;
            parameters[2].Value = model.AnnunciateTitle;
            parameters[3].Value = model.AnnunciateContent;
            parameters[4].Value = model.CreateTime;
            parameters[5].Value = (int)model.Type;
            if (model.Users != null && model.Users.Count > 0)
            {
                string s = "";
                foreach(var v in model.Users){
                    s += v + ",";
                }
                parameters[6].Value = s.Remove(s.Length - 1, 1);
            }
            else
            {
                parameters[6].Value = "";
            }
            parameters[7].Value = (int)model.PlatType;

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
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool Update(SMS.Model.Annunciate model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_annunciate set ");
            strSql.Append("AnnunciateAccount=@AnnunciateAccount,");
            strSql.Append("AnnunciateTitle=@AnnunciateTitle,");
            strSql.Append("AnnunciateContent=@AnnunciateContent,");
            strSql.Append("CreateTime=@CreateTime,");
            strSql.Append("AnnunciateType=@AnnunciateType,");
            strSql.Append("UserLists=@UserLists");
            strSql.Append(" where AnnunciateID=@AnnunciateID");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AnnunciateID", MySqlDbType.VarChar,64),
					new MySqlParameter("@AnnunciateAccount", MySqlDbType.VarChar,64),
					new MySqlParameter("@AnnunciateTitle", MySqlDbType.VarChar,256),
					new MySqlParameter("@AnnunciateContent", MySqlDbType.Text),
					new MySqlParameter("@CreateTime", MySqlDbType.DateTime),
					new MySqlParameter("@AnnunciateType", MySqlDbType.Int32,11),
					new MySqlParameter("@UserLists", MySqlDbType.LongText)};
            parameters[0].Value = model.AnnunciateID;
            parameters[1].Value = model.AnnunciateAccount;
            parameters[2].Value = model.AnnunciateTitle;
            parameters[3].Value = model.AnnunciateContent;
            parameters[4].Value = model.CreateTime;
            parameters[5].Value = model.Type;
            if (model.Users != null && model.Users.Count > 0)
            {
                string s = "";
                foreach (var v in model.Users)
                {
                    s += v + ",";
                }
                parameters[6].Value = s.Remove(s.Length - 1, 1);
            }
            else
            {
                parameters[6].Value = "";
            }

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

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static bool Del(string annunciateID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_annunciate ");
            strSql.Append(" where AnnunciateID=@AnnunciateID");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AnnunciateID", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = annunciateID;
            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            return true;
        }

        public static SMS.Model.Annunciate GetAnnunciate(string annunciateID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_annunciate ");
            strSql.Append(" where AnnunciateID=@AnnunciateID");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AnnunciateID", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = annunciateID;

            SMS.Model.Annunciate model = new SMS.Model.Annunciate();
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            if (ds.Tables.Count == 0) return null;
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }

        /// <summary>
        /// 根据帐号获取发布的公告
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static List<SMS.Model.Annunciate> GetAnnunciateByAccount(string account)
        {
            List<SMS.Model.Annunciate> list = new List<SMS.Model.Annunciate>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_annunciate ");
            strSql.Append(" where AnnunciateAccount=@AnnunciateAccount order by CreateTime desc");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AnnunciateAccount", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = account;

            SMS.Model.Annunciate model = new SMS.Model.Annunciate();
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(),parameters);
            if (ds.Tables.Count == 0) return list;
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
        /// 获取公告内容
        /// </summary>
        public static List<SMS.Model.Annunciate> GetAnnunciates(DateTime beginTime,DateTime endTime)
        {
            List<SMS.Model.Annunciate> list = new List<SMS.Model.Annunciate>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_annunciate ");
            strSql.Append(" where CreateTime>=@BeginTime and CreateTime<=@EndTime order by CreateTime desc");
            MySqlParameter[] parameters = {
					new MySqlParameter("@BeginTime", MySqlDbType.DateTime),
                    new MySqlParameter("@EndTime", MySqlDbType.DateTime)
			};
            parameters[0].Value = beginTime;
            parameters[1].Value = endTime;

            SMS.Model.Annunciate model = new SMS.Model.Annunciate();
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(),parameters);
            if (ds.Tables.Count == 0) return list;
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
        static SMS.Model.Annunciate DataRowToModel(DataRow row)
        {
            SMS.Model.Annunciate model = new SMS.Model.Annunciate();
            if (row != null)
            {
                if (row["AnnunciateID"] != null)
                {
                    model.AnnunciateID = row["AnnunciateID"].ToString();
                }
                if (row["AnnunciateAccount"] != null)
                {
                    model.AnnunciateAccount = row["AnnunciateAccount"].ToString();
                }
                if (row["AnnunciateTitle"] != null)
                {
                    model.AnnunciateTitle = row["AnnunciateTitle"].ToString();
                }
                if (row["AnnunciateContent"] != null)
                {
                    model.AnnunciateContent = row["AnnunciateContent"].ToString();
                }
                if (row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                {
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                }
                if (row["AnnunciateType"] != null && row["AnnunciateType"].ToString() != "")
                {
                    model.Type = (SMS.Model.AnnunciateType)int.Parse(row["AnnunciateType"].ToString());
                }
                if (row["PlatType"] != null && row["PlatType"].ToString() != "")
                {
                    model.PlatType = (SMS.Model.SysPlatType)int.Parse(row["PlatType"].ToString());
                }
                model.Users = new List<string>();
                if (row["UserLists"] != null && row["UserLists"].ToString()!="")
                {
                    foreach (var v in row["UserLists"].ToString().Split(','))
                    {
                        model.Users.Add(v);
                    }
                }
            }
            return model;
        }
    }
}
