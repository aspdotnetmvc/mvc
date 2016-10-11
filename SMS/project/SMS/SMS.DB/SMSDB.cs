using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMS.Model;
using System.Data;
using DBTools;

namespace SMS.DB
{
    public partial class SMSDAL
    {
        #region 查询短信
        /// <summary>
        /// 查询短信审核记录 ，预设查询条件为人工审核，且已审核的
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        public static QueryResult<SMSMessage> GetSMSByAudit(QueryParams qp)
        {
            string sql = @"select ID,AccountID,SPNumber,Content,Signature,NumberCount,SendTime,SplitNumber,FeeTotalCount,AuditResult,AuditFailureCase,AuditTime,AuditType,AuditAccountLoginName,Channel,SMSType,Status,Source from SMS where AuditType=0 and AuditResult >0";
            //添加各种查询条件
            ParamList pl = DBHelper.Instance.GetParamList(qp);

            if (pl.isnotnull("AuditResult"))
            {
                sql += " and AuditResult=@AuditResult";
            }
            if (pl.isnotnull("StartTime"))
            {
                sql += " and SendTime>@StartTime";
            }
            if (pl.isnotnull("EndTime"))
            {
                sql += " and SendTime<@EndTime";
            }
            if (pl.isnotnull("AccountID"))
            {
                sql += " and AccountID=@AccountID";
            }
            var rs = DBHelper.Instance.GetResultSet<SMSMessage>(sql, "SendTime desc", pl);
            return DBHelper.Instance.ToQueryResult(rs);
        }



        /// <summary>
        /// 短信质检，查询一天的所有短信
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        public static QueryResult<SMSMessage> GetSMSList(QueryParams qp)
        {
            //不查审核失败的短信
            string sql = @"select ID,AccountID,SPNumber,Content,Signature,NumberCount,FailureCount,FeeBack,SendTime,SplitNumber,FeeTotalCount,AuditResult,AuditFailureCase,AuditTime,AuditType,AuditAccountLoginName,Channel,SMSType,Status,Source from SMS where AuditResult<>2";

            var pl = DBHelper.Instance.GetParamList(qp);

            if (pl.isnotnull("StartTime"))
            {
                sql += " and SendTime> @StartTime";
            }
            if (pl.isnotnull("EndTime"))
            {
                sql += " and SendTime < @EndTime";
            }

            if (pl.isnotnull("keywords"))
            {
                sql += " and instr(Content,@keywords)>0";
            }
            if (pl.isnotnull("Signature"))
            {
                sql += " and instr(Signature,@Signature)>0";
            }
            if (pl.isnotnull("Channel"))
            {
                sql += " and instr(Channel,@Channel)>0";
            }
            if (pl.isnotnull("AccountID"))
            {
                sql += " and AccountID=@AccountID";
            }
            if (pl.isnotnull("SMSType"))
            {
                sql += " and SMSType= @SMSType";
            }
            var r = DBHelper.Instance.GetResultSet<SMSMessage>(sql, "SendTime desc", pl);
            return DBHelper.Instance.ToQueryResult(r);
        }

        /// <summary>
        /// 根据ID 获取短信
        /// </summary>
        /// <param name="smsid"></param>
        /// <returns></returns>
        public static SMSMessage GetSMSMessageById(string smsid)
        {
            string sql = @"select ID,AccountID,SPNumber,Content,Signature,NumberCount,SendTime,SplitNumber,FeeTotalCount,AuditResult,AuditTime,AuditType,AuditAccountLoginName,Channel,SMSType,Status,Source from SMS where ID=@ID";
            return DBHelper.Instance.Query<SMSMessage>(sql, new { ID = smsid }).FirstOrDefault();
        }
        #endregion

        #region  SMSDTO
        /// <summary>
        /// 插入短信，同时插入到ReviewMT 表中,同时扣费
        /// </summary>
        /// <param name="sms"></param>
        public static void AddSMS(SMS.DTO.SMSDTO sms)
        {
            IDbTransaction tran = DBHelper.Instance.GetDBAdapter().BeginTransaction();

            try
            {
                foreach (var sn in sms.SMSNumbers)
                {
                    sn.SendTime = sms.Message.SendTime;
                }
                AddSMS(sms.Message, tran);
                AddSMSNumber(sms.SMSNumbers, tran);

                //扣费
                AccountDeductBalance(sms.Message.AccountID, sms.Message.FeeTotalCount, tran);
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
            finally
            {
                tran.Connection.Close();
            }
        }
        /// <summary>
        /// 查询待审核短信
        /// </summary>
        /// <param name="qp"></param>
        public static QueryResult<SMSMessage> GetSMSForAudit(QueryParams qp)
        {
            string sql = @"select ID,AccountID,SPNumber,Content,Signature,NumberCount,SendTime,SplitNumber,FeeTotalCount,AuditResult,AuditTime,AuditType,AuditAccountLoginName,Channel,SMSType,Status,Source from SMS where AuditType=0 and AuditResult = 0";
            //添加各种查询条件
            ParamList pl = DBHelper.Instance.GetParamList(qp);
            if (pl.isnotnull("SMSType"))
            {
                sql += " and SMSType=@SMSType";
            }

            var rs = DBHelper.Instance.GetResultSet<SMSMessage>(sql, "SendTime desc", pl);
            return DBHelper.Instance.ToQueryResult(rs);
        }
        /// <summary>
        /// 审核成功
        /// </summary>
        /// <param name="AuditAccountLoginName"></param>
        /// <param name="SMSIDList"></param>
        public static void AuditSMSSuccess(string AuditAccountLoginName, List<string> SMSIDList, string SendChannel)
        {
            IDbTransaction tran = DBHelper.Instance.GetDBAdapter().BeginTransaction();

            try
            {
                string sql = "update SMS set AuditAccountLoginName=@AuditAccountLoginName,AuditResult=1,AuditTime=Now(),Channel=@Channel,Status='待发送' where ID =@ID and AuditResult = 0";

                foreach (var smsid in SMSIDList)
                {
                    int i = DBHelper.Instance.Execute(sql, new { AuditAccountLoginName = AuditAccountLoginName, Channel = SendChannel, ID = smsid }, tran);

                    if (i == 0)
                    {
                        throw new OperateException("短信已被其他审核人审核！");
                    }
                    var list = DBHelper.Instance.Query<string>("select ID from SMSNumber where SMSID=@SMSID", new { SMSID = smsid }).ToList();
                    AuditReviewMTSuccess(list, SendChannel, tran);

                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
            finally
            {
                tran.Connection.Close();
            }
        }

        /// <summary>
        /// 审核失败,并退费
        /// </summary>
        /// <param name="AuditAccountLoginName"></param>
        /// <param name="SMSIDList"></param>
        /// <param name="FailureCase"></param>
        public static void AuditSMSFailure(string AuditAccountLoginName, List<string> SMSIDList, string FailureCase)
        {
            IDbTransaction tran = DBHelper.Instance.GetDBAdapter().BeginTransaction();

            try
            {
                string sql = "update SMS set AuditAccountLoginName=@AuditAccountLoginName,AuditResult=@AuditResult,AuditTime=Now(),AuditFailureCase=@AuditFailureCase,FailureCount=NumberCount,FeeBack=FeeTotalCount,FeeBackReason='审核失败',Status='审核失败' where ID=@ID and AuditResult=0";

                foreach (var smsid in SMSIDList)
                {
                    int i = DBHelper.Instance.Execute(sql, new { AuditAccountLoginName = AuditAccountLoginName, ID = smsid, AuditResult = 2, AuditFailureCase = FailureCase }, tran);
                    if (i == 0)
                    {
                        throw new OperateException("短信已被其他审核人审核！");
                    }
                    var list = DBHelper.Instance.Query<string>("select ID from SMSNumber where SMSID=@SMSID", new { SMSID = smsid }, tran).ToList();
                    var FeeToReturn = DBHelper.Instance.Query("select AccountID,FeeTotalCount from SMS where ID=@ID ", new { ID = smsid }, tran).FirstOrDefault();
                    if (FeeToReturn != null)
                    {
                        //退费
                        AccountDeductBalance(FeeToReturn.AccountID, 0 - FeeToReturn.FeeTotalCount, tran);

                    }
                    AuditReviewMTFailure(list, tran);

                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
            finally
            {
                tran.Connection.Close();
            }
        }
        /// <summary>
        /// 发送失败短信退费
        /// </summary>
        /// <param name="smsid"></param>
        /// <param name="number"></param>
        public static void ReturnBalanceBySMS(string smsid, string number)
        {
            IDbTransaction tran = DBHelper.Instance.GetDBAdapter().BeginTransaction();

            try
            {
                string sql = "select SMS.ID,SMS.AccountID,SMS.SplitNumber from SMSNumber left join SMS on SMSNumber.SMSID=SMS.ID where SMSNumber.ID=@ID ";
                var FeeToReturn = DBHelper.Instance.Query(sql, new { ID = smsid }, tran).FirstOrDefault();
                if (FeeToReturn == null)
                {
                    throw new OperateException("没有找到原始发送记录。");
                }
                DBHelper.Instance.Execute("update SMS set FeeBack=ifnull(Feeback,0)+SplitNumber,FailureCount=ifnull(FailureCount,0)+1 where ID=@ID", new { ID = FeeToReturn.ID }, tran);
                AccountDeductBalance(FeeToReturn.AccountID, 0 - FeeToReturn.SplitNumber, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
            finally
            {
                tran.Connection.Close();
            }
        }
        /// <summary>
        /// 根据ID 获取短信
        /// </summary>
        /// <param name="smsid"></param>
        /// <returns></returns>
        public static SMS.DTO.SMSDTO GetSMSById(string smsid)
        {

            SMS.DTO.SMSDTO sms = new DTO.SMSDTO();
            string sql = @"select ID,AccountID,SPNumber,Content,Signature,NumberCount,SendTime,SplitNumber,FeeTotalCount,AuditResult,AuditTime,AuditType,AuditAccountLoginName,Channel,SMSType,Status,Source from SMS where ID=@ID";

            sms.Message = DBHelper.Instance.Query<SMS.Model.SMSMessage>(sql, new { ID = smsid }).FirstOrDefault();

            sql = @"select ID,SMSID,Numbers,NumberCount,Operator from SMSNumber where SMSID=@SMSID";
            sms.SMSNumbers = DBHelper.Instance.Query<SMS.Model.SMSNumber>(sql, new { SMSID = smsid });
            return sms;
        }

        #endregion

        #region  扣费，退费
        /// <summary>
        /// FeeCount 为正，扣费，为负，退费
        /// </summary>
        /// <param name="AccountID"></param>
        /// <param name="FeeCount"></param>
        public static void AccountDeductBalance(string AccountID, int FeeCount, IDbTransaction tran = null)
        {
            string sql = "update Account set  SMSNumber=SMSNumber-@FeeCount where AccountID=@AccountID";
            DBHelper.Instance.Execute(sql, new
            {
                AccountID = AccountID,
                FeeCount = FeeCount
            }, tran);
        }


        #endregion

        #region ReviewMT 操作
        /// <summary>
        /// 插入记录到ReviewMT 中
        /// </summary>
        /// <param name="sms"></param>
        /// <param name="Numbers"></param>
        /// <param name="tran"></param>
        private static void AddReviewMT(SMSMessage sms, SMSNumber Numbers, IDbTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ReviewMT(");
            strSql.Append("SMSID,AccountID,Phones,SMSContent,StatusReport,SMSLevel,SendTime,Audit,ContentFilter,PhoneCount,SendGateway,PriorityDate,AuditResult,Signature,SPNumber,@WapURL,SMSTimer,Source)");
            strSql.Append(" values (");
            strSql.Append("@SMSID,@AccountID,@Phones,@SMSContent,@StatusReport,@SMSLevel,@SendTime,@Audit,@ContentFilter,@PhoneCount,@SendGateway,@PriorityDate,@AuditResult,@Signature,@SPNumber,@WapURL,@SMSTimer,@Source)");

            DBHelper.Instance.Execute(strSql.ToString(),
                new
                {
                    SMSID = Numbers.ID,
                    AccountID = sms.AccountID,
                    Phones = Numbers.Numbers,
                    SMSContent = sms.Content,
                    StatusReport = (ushort)sms.StatusReportType,
                    SMSLevel = (ushort)sms.SMSLevel,
                    SendTime = sms.SendTime,
                    Audit = (ushort)(sms.AuditType == AuditType.Template ? AuditType.Manual : sms.AuditType),
                    ContentFilter = 0,
                    PhoneCount = Numbers.NumberCount,
                    SendGateway = sms.Channel,
                    PriorityDate = sms.SendTime,
                    AuditResult = sms.AuditType == AuditType.Manual ? 0 : 1,
                    Signature = sms.Signature,
                    SPNumber = sms.SPNumber,
                    WapURL = sms.WapURL,
                    SMSTimer = sms.SMSTimer,
                    Source = sms.Source
                }, tran);
        }
        /// <summary>
        /// 短信审核成功
        /// </summary>
        /// <param name="SerialList"></param>
        /// <param name="tran"></param>
        private static void AuditReviewMTSuccess(List<string> SerialList, string SendChannel, IDbTransaction tran = null)
        {
            string sql = "update ReviewMT set AuditResult=1,SendGateway = @Channel where SMSID = @SerialNumber";

            DBHelper.Instance.Execute(sql, (from serial in SerialList select new { SerialNumber = serial, Channel = SendChannel }), tran);
        }
        /// <summary>
        /// 短信审核失败
        /// </summary>
        /// <param name="SerialList"></param>
        /// <param name="tran"></param>
        private static void AuditReviewMTFailure(List<string> SerialList, IDbTransaction tran = null)
        {
            string sql = "delete from ReviewMT where SMSID = @SerialNumber";

            DBHelper.Instance.Execute(sql, (from serial in SerialList select new { SerialNumber = serial }), tran);
        }
        #endregion



        #region SMS 短信表 增删改查
        /// <summary>
        /// 增加一条短信表
        /// </summary>
        public static void AddSMS(SMSMessage sms, IDbTransaction tran = null)
        {
            string sql = @"insert into SMS(ID,AccountID,SPNumber,Content,Signature,NumberCount,SendTime,SplitNumber,FeeTotalCount,
                            AuditResult,AuditTime,AuditType,AuditAccountLoginName,Channel,SMSType,Status,
                            Source,SMSTimer,SMSLevel,StatusReportType,FilterType)
                            values(@ID,@AccountID,@SPNumber,@Content,@Signature,@NumberCount,@SendTime,@SplitNumber,@FeeTotalCount,
                            @AuditResult,@AuditTime,@AuditType,@AuditAccountLoginName,@Channel,@SMSType,@Status,
                            @Source,@SMSTimer,@SMSLevel,@StatusReportType,@FilterType)";
            DBHelper.EnsureID(sms);

            DBHelper.Instance.Execute(sql, sms, tran);
        }
        /// <summary>
        /// 更新SMSStatus
        /// </summary>
        /// <param name="ID"></param>
        public static void UpdateSMSStatus(string SMSID, string Status, IDbTransaction tran = null)
        {
            string sql = "update SMS set Status=@Status where ID= @ID";
            DBHelper.Instance.Execute(sql, new { ID = SMSID, Status = Status }, tran);
        }

        #endregion

        #region SMSNumber 短信号码 增删改查
        /// <summary>
        /// 增加一条短信号码
        /// </summary>
        public static void AddSMSNumber(SMSNumber smsnumber, IDbTransaction tran = null)
        {
            string sql = @"insert into SMSNumber(ID,SMSID,Numbers,NumberCount,Operator,SendTime)
                       values(@ID,@SMSID,@Numbers,@NumberCount,@Operator,@SendTime)";
            DBHelper.EnsureID(smsnumber);

            DBHelper.Instance.Execute(sql, smsnumber, tran);
        }
        /// <summary>
        /// 批量添加SMSNumber
        /// </summary>
        /// <param name="numberlist"></param>
        /// <param name="tran"></param>
        public static void AddSMSNumber(List<SMSNumber> numberlist, IDbTransaction tran = null)
        {
            string sql = @"insert into SMSNumber(ID,SMSID,Numbers,NumberCount,Operator,SendTime)
                       values(@ID,@SMSID,@Numbers,@NumberCount,@Operator,@SendTime)";
            DBHelper.Instance.Execute(sql, numberlist, tran);
        }
        /// <summary>
        /// 查询一条短信的号码
        /// </summary>
        /// <param name="smsid"></param>
        /// <returns></returns>
        public static string GetNumbersBySMSID(string smsid)
        {
            string sql = "select Numbers from SMSNumber where SMSID=@SMSID";
            var list = DBHelper.Instance.Query<string>(sql, new { SMSID = smsid });
            return string.Join(",", list);
        }


        /// <summary>
        /// 列表查询
        /// </summary>
        /// <param name="pl"></param>
        /// <returns></returns>
        public static QueryResult<SMSNumber> GetSMSNumberList(QueryParams qp)
        {
            string sql = @"select  ID,SMSID,Numbers,NumberCount,Operator from SMSNumber where 1=1";
            //添加各种查询条件
            ParamList pl = DBHelper.Instance.GetParamList(qp);
            if (pl.isnotnull("SMSID"))
            {
                sql += " and SMSID=@SMSID";
            }

            var rs = DBHelper.Instance.GetResultSet<SMSNumber>(sql, "SendTime", pl);

            return DBHelper.Instance.ToQueryResult(rs);
        }
        #endregion


        #region 定时发送
        public static void AddSMSTimer(string smsid, DateTime SendTime)
        {
            string sql = "insert into SMSTimer(SMSID,SendTime) values (@SMSID,@SendTime)";
            DBHelper.Instance.Execute(sql, new { SMSID = smsid, SendTime = SendTime });
        }
        public static List<string> GetTimerSMS()
        {
            string sql = "select SMSID from SMSTimer where SendTime<=@SendTime";
            return DBHelper.Instance.Query<string>(sql, new { SendTime = DateTime.Now.AddSeconds(30) });
        }
        public static void DeleteSMSTimer(string smsid)
        {
            string sql = "delete from SMSTimer where SMSID=@SMSID";
            DBHelper.Instance.Execute(sql, new { SMSID = smsid });
        }
        #endregion
    }
}
