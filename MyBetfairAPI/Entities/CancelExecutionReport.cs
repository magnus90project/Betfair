using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBetfairAPI.Entities
{
    public class CancelExecutionReporta
    {
        public string CustomerRef { get; set; }
        public ExecutionReportStatusa Status { get; set; }
        public ExecutionReportErrorCodea ErrorCode { get; set; }
        public string MarketId { get; set; }
        public List<CancelInstructionReporta> InstructionReports { get; set; }
    }

}
