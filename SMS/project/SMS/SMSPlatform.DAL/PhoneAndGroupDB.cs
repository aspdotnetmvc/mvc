using MySql.Data.MySqlClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMSPlatform.DAL
{
    public class PhoneAndGroupDB
    {
        //--------通讯录

        public static bool PhoneUpload(string AccountCode, string UserName, string UserBrithday, string UserSex, string CompanyName, string TelPhoneNum, string CompanyEmail, string QQ, string ComPostion, string WebChat, string CompanyWeb, string GroupID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_Phone(");
            strSql.Append("AccountCode,UserName,UserBrithday,UserSex,CompanyName,TelPhoneNum,CompanyEmail,QQ,ComPostion,WebChat,CompanyWeb,AddTime,GroupID)");
            strSql.Append(" values (");
            strSql.Append("@AccountCode,@UserName,@UserBrithday,@UserSex,@CompanyName,@TelPhoneNum,@CompanyEmail,@QQ,@ComPostion,@WebChat,@CompanyWeb,Now(),@GroupID)");

            DBHelper.Instance.Execute(strSql.ToString(),
                new {
                    AccountCode=AccountCode,
                    UserName=UserName,
                    UserBrithday=UserBrithday,
                    UserSex = UserSex,
                    CompanyName = CompanyName,
                    TelPhoneNum = TelPhoneNum,
                    CompanyEmail = CompanyEmail,
                    QQ = QQ,
                    ComPostion = ComPostion,
                    WebChat = WebChat,
                    CompanyWeb = CompanyWeb,
                    GroupID = GroupID
            });
            return true;
        }
        public static bool PhoneUpload(string AccountCode, string GroupID, DataTable dt)
        {
            var tran = DBHelper.Instance.GetDBAdapter().BeginTransaction();
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
                tran.Rollback();
                return false;
            }
            finally
            {
                tran.Connection.Close();
            }
        }

        public static bool PhoneUpload(IDbTransaction tran,string AccountCode, string UserName, string UserBrithday, string UserSex, string CompanyName, string TelPhoneNum, string CompanyEmail, string QQ, string ComPostion, string WebChat, string CompanyWeb, string GroupID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_Phone(");
            strSql.Append("AccountCode,UserName,UserBrithday,UserSex,CompanyName,TelPhoneNum,CompanyEmail,QQ,ComPostion,WebChat,CompanyWeb,AddTime,GroupID)");
            strSql.Append(" values (");
            strSql.Append("@AccountCode,@UserName,@UserBrithday,@UserSex,@CompanyName,@TelPhoneNum,@CompanyEmail,@QQ,@ComPostion,@WebChat,@CompanyWeb,Now(),@GroupID)");
            DBHelper.Instance.Execute(strSql.ToString(),
              new
              {
                  AccountCode = AccountCode,
                  UserName = UserName,
                  UserBrithday = UserBrithday,
                  UserSex = UserSex,
                  CompanyName = CompanyName,
                  TelPhoneNum = TelPhoneNum,
                  CompanyEmail = CompanyEmail,
                  QQ = QQ,
                  ComPostion = ComPostion,
                  WebChat = WebChat,
                  CompanyWeb = CompanyWeb,
                  GroupID = GroupID
              },tran);
            return true;
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
            DBHelper.Instance.Execute(strSql.ToString(), new { ID=ID});
            return true;
        }
        public static  QueryResult GetPhoneByAccountCode(string AccountCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from  plat_Phone where AccountCode='" + AccountCode + "'");
            var p=new DBTools.ParamList(){ispage=false};
            p.add("AccountCode",AccountCode);
             var r= DBHelper.Instance.GetResultSet(strSql.ToString(),"AccountCode",p);
             return DBHelper.Instance.ToQueryResult(r);
        }
        public static DataTable GetPhoneByAccountCodes(string AccountCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select a.*,b.GID,b.TelPhoneGroupName,b.ReMark from plat_Phone a inner join plat_Group b on a.GroupID=b.GID AND a.AccountCode='" + AccountCode + "' group by a.PID");
            return DBHelper.Instance.GetDataTable(strSql.ToString());
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

            DBHelper.Instance.Execute(strSql.ToString(), new { AccountCode = AccountCode, TelPhoneGroupName = TelPhoneGroupName, ReMark = ReMark });

            return true;
        }
        public static bool GroupDelByID(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_Group ");
            strSql.Append(" where GID=@ID ");
            DBHelper.Instance.Execute(strSql.ToString(), new { ID = ID });
            return true;
        }
        public static DataTable GetGroupByTelPhoneGroupNameAndAccountCode(string TelPhoneGroupName,string AccountCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_Group where TelPhoneGroupName='" + TelPhoneGroupName + "' and AccountCode='"+AccountCode+"'");
            return DBHelper.Instance.GetDataTable(strSql.ToString());
        }
      
        public static DataTable GetGroupByAccountCode(string AccountCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_Group where AccountCode='" + AccountCode + "' and TelPhoneGroupName!='0'");
            return DBHelper.Instance.GetDataTable(strSql.ToString());
        }
        public static DataTable GetGroupForTreeByAccountCode(string AccountCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_Group where AccountCode='" + AccountCode + "' ");
            return DBHelper.Instance.GetDataTable(strSql.ToString());
        }

        public static DataTable GetPhoneByAccountCodeAndGroup(string AccountCode, string GroupID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select a.*,b.GID,b.TelPhoneGroupName,b.ReMark from plat_Phone a inner join plat_Group b on a.GroupID=b.GID AND a.AccountCode='" + AccountCode + "'and a.GroupID='" + GroupID + "'  group by a.PID");
            return DBHelper.Instance.GetDataTable(strSql.ToString());
        }
      
        ///关联
        public static bool PhoneConnectGroup(string GroupID, string AccountCode, string PID)
        {
            string strSql = "Update plat_Phone Set GroupID='" + GroupID + "' Where AccountCode='" + AccountCode + "' And PID='" + PID + "'";
            DBHelper.Instance.Execute(strSql.ToString());
            return true;
        }

    }
}
