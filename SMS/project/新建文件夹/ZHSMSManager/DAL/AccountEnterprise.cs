using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL
{
    public class AccountEnterprise
    {
        public static bool Exists(string EnterpriseCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from AccountEnterprise");
            strSql.Append(" where EnterpriseCode=@EnterpriseCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@EnterpriseCode", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = EnterpriseCode;
            return DBUtility.MySqlHelper.Exists(strSql.ToString(), parameters);
        }

        public static DataTable GetAccountEnterpriseByUserCode(string UserCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from AccountEnterprise where UserCode='" + UserCode + "' ");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            return ds.Tables[0];
        }

        public static List<string> GetEnterpriseByUserCode(string UserCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from AccountEnterprise where UserCode='" + UserCode + "' ");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            List<string> list = new List<string>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(row["EnterpriseCode"].ToString());
                }
            }
            else
            {
                strSql = new StringBuilder();
                strSql.Append("SELECT * FROM plat_EnterpriseManage ");
                strSql.Append("WHERE ChannelManager=@ChannelManager OR CSManager=@ChannelManager");
                MySqlParameter[] parameters = {
					new MySqlParameter("@ChannelManager", MySqlDbType.VarChar,16)};
                parameters[0].Value = UserCode;
                ds = DBUtility.MySqlHelper.Query(strSql.ToString(),parameters);
                list = new List<string>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(row["EnterpriseCode"].ToString());
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 获取用户分管的企业
        /// 用户是企业的渠道经理或者客服
        /// </summary>
        /// <param name="UserCode"></param>
        /// <returns></returns>
        public static List<SMS.Model.EnterpriseUser> GetEnterpriseUserByUserCode(string UserCode)
        {
            //首先从accountEnterprise表中检查userCode 是否全部分配了
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from AccountEnterprise where UserCode='" + UserCode + "' ");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            List<SMS.Model.EnterpriseUser> list = new List<SMS.Model.EnterpriseUser>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    //-1 表示全部分管
                    if("-1".Equals(row["EnterpriseCode"].ToString())){
                        return DAL.EnterpriseUser.GetEnterprises().Where(en=>en.IsAgent==true).ToList();
                    }
                }
            }
            else
            {
                strSql = new StringBuilder();
                strSql.Append("SELECT * FROM plat_EnterpriseManage ");
                strSql.Append("WHERE ChannelManager=@ChannelManager OR CSManager=@ChannelManager");
                MySqlParameter[] parameters = {
					new MySqlParameter("@ChannelManager", MySqlDbType.VarChar,16)};
                parameters[0].Value = UserCode;
                ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
              
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var dt=ds.Tables[0].AsEnumerable();
                    list = DAL.EnterpriseUser.GetEnterprises().Where(en => en.IsAgent == true).Where(en => dt.Where(dr => dr["EnterpriseCode"].ToString().Equals(en.AccountCode)).Count() > 0).ToList();
                }
            }
            return list;
        }
        public static DataTable GetAccountEnterprise()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select en.*,et.* from plat_EnterpriseUser  as en left join  AccountEnterprise as et on en.AccountCode = et.EnterpriseCode");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            return ds.Tables[0];
        }

        public static DataTable GetAccountEnterpriseByAccoutID(string accountID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select en.*,et.* from plat_EnterpriseUser  as en left join plat_EnterpriseInformation as et on en.AccountCode = et.AccountCode where en.AccountID='" + accountID + "'");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            return ds.Tables[0];
        }

        //public static DataTable GetAccountEnterprise(string UserCode)
        //{
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("select en.*,et.* from plat_EnterpriseUser  as en left join  AccountEnterprise as et on en.AccountCode = et.EnterpriseCode and  (et.UserCode ='" + UserCode + "'   or   et.UserCode='')");
        //    DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
        //    return ds.Tables[0];
        //}

        public static DataTable GetAccountEnterprise(string userCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select p.* ,a.*  from (select * from plat_EnterpriseInformation where  AccountCode not in (select EnterpriseCode from AccountEnterprise where UserCode!=@UserCode)) p left join AccountEnterprise a on p.AccountCode = a.EnterpriseCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = userCode;
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            return ds.Tables[0];
        }

        public static bool Add(SMS.Model.AccountEnterprise model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into AccountEnterprise(");
            strSql.Append("UserCode,EnterpriseCode)");
            strSql.Append(" values (");
            strSql.Append("@UserCode,@EnterpriseCode)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@EnterpriseCode", MySqlDbType.VarChar,128) };
            parameters[0].Value = model.UserCode;
            parameters[1].Value = model.EnterpriseCode;
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

        public static bool Del(string EnterpriseCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from AccountEnterprise ");
            strSql.Append(" where EnterpriseCode=@EnterpriseCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@EnterpriseCode", MySqlDbType.VarChar,64)			};
            parameters[0].Value = EnterpriseCode;
            DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            return true;
        }

        public static bool DelByUserCodeAndEnterpriseCode(string UserCode, string EnterpriseCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from AccountEnterprise ");
            strSql.Append(" where EnterpriseCode=@EnterpriseCode and UserCode=@UserCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@EnterpriseCode", MySqlDbType.VarChar,64),
                    new MySqlParameter("@UserCode", MySqlDbType.VarChar,64)		};
            parameters[0].Value = EnterpriseCode;
            parameters[1].Value = UserCode;
            DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            return true;
        }

        public static bool DelByUserCode(string UserCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from AccountEnterprise ");
            strSql.Append(" where UserCode=@UserCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,64)			};
            parameters[0].Value = UserCode;
            DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            return true;
        }

    }

    // 所有代理商指派的对接渠道、客户由此类处理
    public class EnterpriseManage
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static bool Add(SMS.Model.EnterpriseManage model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from plat_EnterpriseManage");
            strSql.Append(" where EnterpriseCode=@EnterpriseCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@EnterpriseCode", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = model.EnterpriseCode;
            if (DBUtility.MySqlHelper.Exists(strSql.ToString(), parameters))
            {
                return Update(model);
            }
            else
            {
                strSql = new StringBuilder();
                strSql.Append("insert into plat_EnterpriseManage(");
                strSql.Append("EnterpriseCode,ChannelManager,CSManager,Reserve)");
                strSql.Append(" values (");
                strSql.Append("@EnterpriseCode,@ChannelManager,@CSManager,@Reserve)");
                MySqlParameter[] parametersInsert = {
					new MySqlParameter("@EnterpriseCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@ChannelManager", MySqlDbType.VarChar,16),
					new MySqlParameter("@CSManager", MySqlDbType.VarChar,16),
					new MySqlParameter("@Reserve", MySqlDbType.VarChar,128)};
                parametersInsert[0].Value = model.EnterpriseCode;
                parametersInsert[1].Value = model.ChannelManager;
                parametersInsert[2].Value = model.CSManager;
                parametersInsert[3].Value = model.Reserve;

                int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parametersInsert);
                if (rows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool Update(SMS.Model.EnterpriseManage model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_EnterpriseManage set ");
            strSql.Append("EnterpriseCode=@EnterpriseCode,");
            strSql.Append("ChannelManager=@ChannelManager,");
            strSql.Append("CSManager=@CSManager,");
            strSql.Append("Reserve=@Reserve");
            strSql.Append(" where EnterpriseCode=@EnterpriseCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@EnterpriseCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@ChannelManager", MySqlDbType.VarChar,16),
					new MySqlParameter("@CSManager", MySqlDbType.VarChar,16),
					new MySqlParameter("@Reserve", MySqlDbType.VarChar,128)};
            parameters[0].Value = model.EnterpriseCode;
            parameters[1].Value = model.ChannelManager;
            parameters[2].Value = model.CSManager;
            parameters[3].Value = model.Reserve;

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
        public static bool Delete(Int32 rsid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_EnterpriseManage ");
            strSql.Append(" where ID=@ID");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.Int32)
			};
            parameters[0].Value = rsid;

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
        /// 根据企业代码获取该企业与渠道、客户对应关系表
        /// </summary>
        public static List<SMS.Model.EnterpriseManage> GetEnManageByEnCode(string enCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM plat_EnterpriseManage ");
            strSql.Append("WHERE EnterpriseCode=@EnterpriseCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@EnterpriseCode", MySqlDbType.VarChar,64)};
            parameters[0].Value = enCode;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            List<SMS.Model.EnterpriseManage> list = new List<SMS.Model.EnterpriseManage>();
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
        /// 根据渠道经理账号获取该渠道管理的企业列表
        /// </summary>
        public static List<SMS.Model.EnterpriseManage> GetEnterpriseByCM(string channelManager)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM plat_EnterpriseManage ");
            strSql.Append("WHERE ChannelManager=@ChannelManager");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ChannelManager", MySqlDbType.VarChar,16)};
            parameters[0].Value = channelManager;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            List<SMS.Model.EnterpriseManage> list = new List<SMS.Model.EnterpriseManage>();
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
        /// 根据用户账号获取该渠道管理的企业列表
        /// </summary>
        public static List<SMS.Model.EnterpriseManage> GetEnterpriseByUserCode(string userCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM plat_EnterpriseManage ");
            strSql.Append("WHERE ChannelManager=@ChannelManager OR CSManager=@ChannelManager");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ChannelManager", MySqlDbType.VarChar,16)};
            parameters[0].Value = userCode;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            List<SMS.Model.EnterpriseManage> list = new List<SMS.Model.EnterpriseManage>();
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
        /// 根据客服账号获取该客服管理的企业列表
        /// </summary>
        public static List<SMS.Model.EnterpriseManage> GetEnterpriseByCS(string csManager)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM plat_EnterpriseManage ");
            strSql.Append("WHERE CSManager='@CSManager'");
            MySqlParameter[] parameters = {
					new MySqlParameter("@CSManager", MySqlDbType.VarChar,16)};
            parameters[0].Value = csManager;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            List<SMS.Model.EnterpriseManage> list = new List<SMS.Model.EnterpriseManage>();
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
        /// 获取所有企业与渠道、客户对应关系表
        /// </summary>
        public static List<SMS.Model.EnterpriseManage> GetEnManageInfo()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM plat_EnterpriseManage ORDER BY EnterpriseCode");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            List<SMS.Model.EnterpriseManage> list = new List<SMS.Model.EnterpriseManage>();
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
        static SMS.Model.EnterpriseManage DataRowToModel(DataRow row)
        {
            SMS.Model.EnterpriseManage model = new SMS.Model.EnterpriseManage();
            if (row != null)
            {
                if (row["ID"] != null)
                {
                    model.ID = Int32.Parse(row["ID"].ToString());
                }
                if (row["EnterpriseCode"] != null)
                {
                    model.EnterpriseCode = row["EnterpriseCode"].ToString();
                }
                if (row["ChannelManager"] != null && row["ChannelManager"].ToString() != "")
                {
                    model.ChannelManager = row["ChannelManager"].ToString();
                }
                if (row["CSManager"] != null && row["CSManager"].ToString() != "")
                {
                    model.CSManager = row["CSManager"].ToString();
                }
                if (row["Reserve"] != null)
                {
                    model.Reserve = row["Reserve"].ToString();
                }
            }
            return model;
        }
    }
}
