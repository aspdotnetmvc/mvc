using MongoDB.Bson;
using MongoDB.Driver;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MongoDBAccess
{
    public class ReportStatisticsCache : ReportStatistics
    {
        public ObjectId id { get; set; }

        public ReportStatistics Clone()
        {
            ReportStatistics rs = new ReportStatistics();
            rs.Account = base.Account;
            rs.BeginSendTime = base.BeginSendTime;
            rs.FailureCount = base.FailureCount;
            rs.LastResponseTime = base.LastResponseTime;
            rs.Numbers = base.Numbers;
            rs.SendCount = base.SendCount;
            rs.SerialNumber = base.SerialNumber;
            rs.SplitNumber = base.SplitNumber;
            return rs;
        }
    }

    public class ReportStatisticsMongoDB
    {
        //数据库连接字符串
        static readonly string strconn = ConfigurationManager.AppSettings["MongoConn"];// "mongodb://127.0.0.1:27017";
        //数据库名称
        static readonly string dbName = ConfigurationManager.AppSettings["MongoDB"];// "cnblogs";

        static MongoCollection GetCollection()
        {
            MongoClient client = new MongoClient(strconn);
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(dbName);
            MongoCollection collection = db.GetCollection("ReportStatisticsCache");
            return collection;
        }

        public static void AddReportStatistics(ReportStatistics report)
        {
            ReportStatisticsCache srm = new ReportStatisticsCache();
            srm.Account = report.Account;
            srm.BeginSendTime = report.BeginSendTime;
            srm.FailureCount = report.FailureCount;
            srm.LastResponseTime = report.LastResponseTime;
            srm.Numbers = report.Numbers;
            srm.SendCount = report.SendCount;
            srm.SerialNumber = report.SerialNumber;
            srm.SplitNumber = report.SplitNumber;
            MongoCollection collection = GetCollection();
            collection.Insert<ReportStatisticsCache>(srm);
        }

        //public static void AddStatusReport(List<StatusReport> reports)
        //{
        //    MongoCollection collection = GetCollection();
        //    collection.InsertBatch(typeof(BsonDocument), reports);
        //}

        public static void Update(ReportStatistics report)
        {
            MongoCollection collection = GetCollection();
            var query = new QueryDocument("SerialNumber", report.SerialNumber);
            ReportStatisticsCache sr = collection.FindOneAs<ReportStatisticsCache>(query);
            if (sr != null)
            {
                sr.Account = report.Account;
                sr.BeginSendTime = report.BeginSendTime;
                sr.FailureCount = report.FailureCount;
                sr.LastResponseTime = report.LastResponseTime;
                sr.Numbers = report.Numbers;
                sr.SendCount = report.SendCount;
                sr.SerialNumber = report.SerialNumber;
                sr.SplitNumber = report.SplitNumber;
                collection.Save(sr);
            }
            else
            {
                sr = new ReportStatisticsCache();
                sr.Account = report.Account;
                sr.BeginSendTime = report.BeginSendTime;
                sr.FailureCount = report.FailureCount;
                sr.LastResponseTime = report.LastResponseTime;
                sr.Numbers = report.Numbers;
                sr.SendCount = report.SendCount;
                sr.SerialNumber = report.SerialNumber;
                sr.SplitNumber = report.SplitNumber;
                collection.Insert<ReportStatisticsCache>(sr);
            }
        }

        public static List<ReportStatistics> GetStatusReports()
        {
            List<ReportStatistics> reports = new List<ReportStatistics>();
            MongoCollection collection = GetCollection();

            MongoCursor<ReportStatisticsCache> cursor = collection.FindAllAs<ReportStatisticsCache>();
            foreach (ReportStatisticsCache report in cursor)
            {
                reports.Add((ReportStatistics)report.Clone());
            }
            return reports;
        }

        public static ReportStatistics GetStatusReport(string serial)
        {
            MongoCollection collection = GetCollection();
            var query = new QueryDocument("SerialNumber", serial);
            ReportStatistics report = collection.FindOneAs<ReportStatisticsCache>(query).Clone();
            return report;
        }

        public static void Del(string serial)
        {
            MongoCollection collection = GetCollection();
            var remove = new QueryDocument { { "SerialNumber",Guid.Parse(serial) } };
            collection.Remove(remove);
        }
    }
}
