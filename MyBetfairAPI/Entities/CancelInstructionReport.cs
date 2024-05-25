using Newtonsoft.Json;
using System;

namespace MyBetfairAPI.Entities
{
    public class CancelInstructionReporta
    {
        [JsonProperty(PropertyName = "status")]
        public InstructionReportStatusa Status { get; set; }

        [JsonProperty(PropertyName = "errorCode")]
        public InstructionReportErrorCodea? ErrorCode { get; set; }

        [JsonProperty(PropertyName = "instruction")]
        public CancelInstructiona Instruction { get; set; }

        [JsonProperty(PropertyName = "sizeCancelled")]
        public double SizeCancelled { get; set; }

        [JsonProperty(PropertyName = "cancelledDate")]
        public DateTime CancelledDate { get; set; }
    }

}