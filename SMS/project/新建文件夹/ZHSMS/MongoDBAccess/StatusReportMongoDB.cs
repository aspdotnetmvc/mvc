using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MongoDBAccess
{
    public class StatusReportCache : StatusReport
    {
        public ObjectId id { get; set; }

        public StatusReport Clone()
        {
            StatusReport srm = new StatusReport();
            srm.Describe = base.Describe;
            srm.Gateway = base.Gateway;
            srm.Message = base.Message;
            srm.ResponseTime = base.ResponseTime;
            srm.Serial = base.Serial;
            srm.SplitNumber = base.SplitNumber;
            srm.SplitTotal = base.SplitTotal;
            srm.Succeed = base.Succeed;
            return srm;
        }
    }

    public class StatusReportMongoDB
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
            MongoCollection collection = db.GetCollection("StatusReportCache");
            return collection;
        }

        public static void AddStatusReport(StatusReport report)
        {
            StatusReportCache srm = new StatusReportCache();
            srm.Describe = report.Describe;
            srm.Gateway = report.Gateway;
            srm.Message = report.Message;
            srm.ResponseTime = report.ResponseTime;
            srm.Serial = report.Serial;
            srm.SplitNumber = report.SplitNumber;
            srm.SplitTotal = report.SplitTotal;
            srm.Succeed = report.Succeed;
            MongoCollection collection = GetCollection();
            collection.Insert<StatusReportCache>(srm);
        }

        //public static void AddStatusReport(List<StatusReport> reports)
        //{
        //    MongoCollection collection = GetCollection();
        //    collection.InsertBatch(typeof(BsonDocument), reports);
        //}

        //public void Update(StatusReport report)
        //{
        //    MongoCollection collection = GetCollection();
        //    var query = new QueryDocument("Serial", report.Serial);
        //    StatusReport sr = collection.FindOneAs<StatusReport>(query);
        //    if (sr != null)
        //    {
        //        sr = report;
        //        collection.Save(sr);
        //    }
        //}

        public static List<StatusReport> GetStatusReports()
        {
            List<StatusReport> reports = new List<StatusReport>();
            MongoCollection collection = GetCollection();
            
            MongoCursor<StatusReportCache> cursor = collection.FindAllAs<StatusReportCache>();
            foreach (StatusReportCache report in cursor)
            {
                reports.Add((StatusReport)report.Clone());
            }
            return reports;
        }

        public static StatusReport GetStatusReport(string serial)
        {
            MongoCollection collection = GetCollection();
            var query = new QueryDocument("Serial", serial);
            StatusReport report = collection.FindOneAs<StatusReportCache>(query).Clone();
            return report;
        }

        public static void Del(string serial)
        {
            MongoCollection collection = GetCollection();
            var remove = new QueryDocument { { "Serial", serial } };
            collection.Remove(remove);
        }
    }
}
