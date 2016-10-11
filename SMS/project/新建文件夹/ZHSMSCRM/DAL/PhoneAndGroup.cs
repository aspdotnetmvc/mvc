using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL
{
    public class PhoneAndGroup
    {
        //--------通讯录

        public static bool PhoneUpload(string AccountCode, string UserName, string UserBrithday, string UserSex, string CompanyName, string TelPhoneNum, string CompanyEmail, string QQ, string ComPostion, string WebChat, string CompanyWeb, string GroupID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_Phone(");
            strSql.Append("AccountCode,UserName,UserBrithday,UserSex,CompanyName,TelPhoneNum,CompanyEmail,QQ,ComPostion,WebChat,CompanyWeb,AddTime,GroupID)");
            strSql.Append(" values (");
            strSql.Append("@AccountCode,@UserName,@UserBrithday,@UserSex,@CompanyName,@TelPhoneNum,@CompanyEmail,@QQ,@ComPostion,@WebChat,@CompanyWeb,@AddTime,@GroupID)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64),
                    new MySqlParameter("@UserName", MySqlDbType.VarChar,64),
                    new MySqlParameter("@UserBrithday", MySqlDbType.VarChar,64),
                    new MySqlParameter("@UserSex", MySqlDbType.VarChar,5),
                    new MySqlParameter("@CompanyName", MySqlDbType.VarChar,50),
                    new MySqlParameter("@TelPhoneNum", MySqlDbType.VarChar,16),
                    new MySqlParameter("@CompanyEmail", MySqlDbType.VarChar,50),
                    new MySqlParameter("@QQ", MySqlDbType.VarChar,16),
                    new MySqlParameter("@ComPostion", MySqlDbType.VarChar,50),
                    new MySqlParameter("@WebChat", MySqlDbType.VarChar,16),
                    new MySqlParameter("@CompanyWeb", MySqlDbType.VarChar,50),
					new MySqlParameter("@AddTime", MySqlDbType.DateTime),
                    new MySqlParameter("@GroupID", MySqlDbType.VarChar,50)};
            parameters[0].Value = AccountCode;
            parameters[1].Value = UserName;
            parameters[2].Value = UserBrithday;
            parameters[3].Value = UserSex;
            parameters[4].Value = CompanyName;
            parameters[5].Value = TelPhoneNum;
            parameters[6].Value = CompanyEmail;
            parameters[7].Value = QQ;
            parameters[8].Value = ComPostion;
            parameters[9].Value = WebChat;
            parameters[10].Value = CompanyWeb;
            parameters[11].Value = DateTime.Now;
            parameters[12].Value = GroupID;
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
        public static bool PhoneUpload(string AccountCode, string GroupID, DataTable dt)
        {
            MySqlTransaction tran = DBUtility.MySqlHelper.CreateTransaction();
            try
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bool rr = PhoneUpload(tran,
                        AccountCode, dt.Rows[i][0].ToString(),
                        dt.Rows[i][1].ToString(),
                        dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(),
                        dt.Rows[i][4].ToString(), dt.Rows[i][5].ToString(),
                        dt.Rows[i][6].ToString(), dt.Rows[i][7].ToString(),
                        dt.Rows[i][8].ToString(), dt.Rows[i][9].ToString(), GroupID);
                    if (!rr)
                    {
                        tran.Rollback();
                        return false;
                    }
                }
                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                tran.Connection.Close();
            }
        }

        public static bool PhoneUpload(MySqlTransaction tran,string AccountCode, string UserName, string UserBrithday, string UserSex, string CompanyName, string TelPhoneNum, string CompanyEmail, string QQ, string ComPostion, string WebChat, string CompanyWeb, string GroupID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_Phone(");
            strSql.Append("AccountCode,UserName,UserBrithday,UserSex,CompanyName,TelPhoneNum,CompanyEmail,QQ,ComPostion,WebChat,CompanyWeb,AddTime,GroupID)");
            strSql.Append(" values (");
            strSql.Append("@AccountCode,@UserName,@UserBrithday,@UserSex,@CompanyName,@TelPhoneNum,@CompanyEmail,@QQ,@ComPostion,@WebChat,@CompanyWeb,@AddTime,@GroupID)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64),
                    new MySqlParameter("@UserName", MySqlDbType.VarChar,64),
                    new MySqlParameter("@UserBrithday", MySqlDbType.VarChar,64),
                    new MySqlParameter("@UserSex", MySqlDbType.VarChar,5),
                    new MySqlParameter("@CompanyName", MySqlDbType.VarChar,50),
                    new MySqlParameter("@TelPhoneNum", MySqlDbType.VarChar,16),
                    new MySqlParameter("@CompanyEmail", MySqlDbType.VarChar,50),
                    new MySqlParameter("@QQ", MySqlDbType.VarChar,16),
                    new MySqlParameter("@ComPostion", MySqlDbType.VarChar,50),
                    new MySqlParameter("@WebChat", MySqlDbType.VarChar,16),
                    new MySqlParameter("@CompanyWeb", MySqlDbType.VarChar,50),
					new MySqlParameter("@AddTime", MySqlDbType.DateTime),
                    new MySqlParameter("@GroupID", MySqlDbType.VarChar,50)};
            parameters[0].Value = AccountCode;
            parameters[1].Value = UserName;
            parameters[2].Value = UserBrithday;
            parameters[3].Value = UserSex;
            parameters[4].Value = CompanyName;
            parameters[5].Value = TelPhoneNum;
            parameters[6].Value = CompanyEmail;
            parameters[7].Value = QQ;
            parameters[8].Value = ComPostion;
            parameters[9].Value = WebChat;
            parameters[10].Value = CompanyWeb;
            parameters[11].Value = DateTime.Now;
            parameters[12].Value = GroupID;
            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(tran, strSql.ToString(), parameters);
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
        /// 删除电话号码
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static bool PhoneDelByID(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_Phone ");
            strSql.Append(" where PID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,16)			};
            parameters[0].Value = ID;
            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            return true;
        }
        public static DataTable GetPhoneByAccountCode(string AccountCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from  plat_Phone where AccountCode='" + AccountCode + "'");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            return ds.Tables[0];
        }
        public static DataTable GetPhoneByAccountCodes(string AccountCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select a.*,b.GID,b.TelPhoneGroupName,b.ReMark from plat_Phone a inner join plat_Group b on a.GroupID=b.GID AND a.AccountCode='" + AccountCode + "' group by a.PID");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            return ds.Tables[0];
        }
        //--------分组
        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static bool GroupAdd(string AccountCode, string TelPhoneGroupName, string ReMark)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_Group(");
            strSql.Append("AccountCode,TelPhoneGroupName,ReMark)");
            strSql.Append(" values (");
            strSql.Append("@AccountCode,@TelPhoneGroupName,@ReMark)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@TelPhoneGroupName", MySqlDbType.VarChar,64),
					new MySqlParameter("@ReMark", MySqlDbType.VarChar,64)};
            parameters[0].Value = AccountCode;
            parameters[1].Value = TelPhoneGroupName;
            parameters[2].Value = ReMark;
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
        public static bool GroupDelByID(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_Group ");
            strSql.Append(" where GID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,16)			};
            parameters[0].Value = ID;
            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            return true;
        }
        public static DataTable GetGroupByTelPhoneGroupNameAndAccountCode(string TelPhoneGroupName,string AccountCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_Group where TelPhoneGroupName='" + TelPhoneGroupName + "' and AccountCode='"+AccountCode+"'");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            return ds.Tables[0];
        }
      
        public static DataTable GetGroupByAccountCode(string AccountCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_Group where AccountCode='" + AccountCode + "' and TelPhoneGroupName!='0'");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            return ds.Tables[0];
        }
        public static DataTable GetGroupForTreeByAccountCode(string AccountCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_Group where AccountCode='" + AccountCode + "' ");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            return ds.Tables[0];
        }

        public static DataTable GetPhoneByAccountCodeAndGroup(string AccountCode, string GroupID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select a.*,b.GID,b.TelPhoneGroupName,b.ReMark from plat_Phone a inner join plat_Group b on a.GroupID=b.GID AND a.AccountCode='" + AccountCode + "'and a.GroupID='" + GroupID + "'  group by a.PID");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            return ds.Tables[0];
        }
      
        ///关联
        public static bool PhoneConnectGroup(string GroupID, string AccountCode, string PID)
        {
            string strSql = "Update plat_Phone Set GroupID='" + GroupID + "' Where AccountCode='" + AccountCode + "' And PID='" + PID + "'";
            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString());
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
}
