using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatusReportService
{
    public class TableService
    {
        private volatile static TableService mng = null;
        private static object lockHelper = new object();
        List<string> statusReportTables = new List<string>();
        List<string> statisticsReportTables = new List<string>();
        System.Timers.Timer timer = null;
        bool operatorFlag = true;
        public TableService()
        {
            timer = new System.Timers.Timer(1000 * 60 * 60 * 2);//2小时
            LoadStatisticsReportTables();
            LoadStatusReportTables();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CreateTable();
        }

        public static TableService Instance
        {
            get
            {
                if (mng == null)
                {
                    lock (lockHelper)
                    {
                        if (mng == null)
                        {
                            mng = new TableService();
                            return mng;
                        }
                    }
                }
                return mng;
            }
        }

        public void TriggerTable()
        {
            CreateTable();
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }
        void CreateTable()
        {
            try
            {
                if (operatorFlag)
                {
                    string tableName = "SMSStatusReport_" + DateTime.Now.AddDays(1).ToString("yyyyMMdd");
                    if (!statusReportTables.Contains(tableName.ToLower()))
                    {
                        StatusReportDB.CloneTable(tableName);
                        statusReportTables.Add(tableName.ToLower());
                    }
                    tableName = "ReportStatistics_" + DateTime.Now.AddDays(1).ToString("yyyyMMdd");
                    if (!statisticsReportTables.Contains(tableName.ToLower()))
                    {
                        ReportStatisticsDB.CloneTable(tableName);
                        statisticsReportTables.Add(tableName.ToLower());
                    }
                }
            }
            catch
            {
                operatorFlag = true;
            }
        }

        /// <summary>
        /// 统计报告表
        /// </summary>
        void LoadStatisticsReportTables()
        {
            List<string> tables = ReportStatisticsDB.GetTables();
            if (tables.Count == 0)
            {
                string name = "ReportStatistics_" + DateTime.Now.ToString("yyyyMMdd");
                ReportStatisticsDB.CloneTable(name);
                statisticsReportTables.Add(name.ToLower());
                return;
            }
            int min = 99991231;
            foreach (var table in tables)
            {
                statisticsReportTables.Add(table.ToLower());
                if (min > int.Parse(table.Remove(0, 17)))
                {
                    min = int.Parse(table.Remove(0, 17));
                }
            }
            string date = min.ToString();
            date = date.Insert(4, "-");
            date = date.Insert(7, "-");
            DateTime beginTime = DateTime.Parse(date).AddDays(1);
            while (DateTime.Compare(DateTime.Now, beginTime) > 0)
            {
                string name = "ReportStatistics_" + beginTime.ToString("yyyyMMdd");
                if (!statisticsReportTables.Contains(name.ToLower()))
                {
                    ReportStatisticsDB.CloneTable(name);
                    statisticsReportTables.Add(name.ToLower());
                }
                beginTime = beginTime.AddDays(1);
            }
        }
        /// <summary>
        /// 状态报告表
        /// </summary>
        void LoadStatusReportTables()
        {
            List<string> tables = StatusReportDB.GetTables();
            if (tables.Count == 0)
            {
                string name = "SMSStatusReport_" + DateTime.Now.ToString("yyyyMMdd");
                StatusReportDB.CloneTable(name);
                statusReportTables.Add(name.ToLower());
                return;
            }
            int min = 99991231;
            foreach (var table in tables)
            {
                statusReportTables.Add(table.ToLower());
                if (min > int.Parse(table.Remove(0, 16)))
                {
                    min = int.Parse(table.Remove(0, 16));
                }
            }
            string date = min.ToString();
            date = date.Insert(4, "-");
            date = date.Insert(7, "-");
            DateTime beginTime = DateTime.Parse(date).AddDays(1);
            while (DateTime.Compare(DateTime.Now, beginTime) > 0)
            {
                string name = "SMSStatusReport_" + beginTime.ToString("yyyyMMdd");
                if (!statusReportTables.Contains(name.ToLower()))
                {
                    StatusReportDB.CloneTable(name);
                    statusReportTables.Add(name.ToLower());
                }
                beginTime = beginTime.AddDays(1);
            }
        }

        public List<string> GetStatisticsTables()
        {
            return statisticsReportTables;
        }
    }
}