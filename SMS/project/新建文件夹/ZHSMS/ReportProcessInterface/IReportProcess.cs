using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportProcessInterface
{
    public interface IReportProcess
    {
        void ReportSendProcess(StatusReport report);
        void ReportResponseProcess(StatusReport report);
    }
}
