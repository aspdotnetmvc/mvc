using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatewayInterface
{
    [Serializable]
    public class ReportEventArgs : EventArgs
    {
        public List<StatusReport> StatusReports { get; set; }
        public ReportEventArgs()
        {
            StatusReports= new List<StatusReport>();
            SubmitTime = DateTime.Now;
        }
        public DateTime SubmitTime
        {
            get;
            set;
        }
    }
}
