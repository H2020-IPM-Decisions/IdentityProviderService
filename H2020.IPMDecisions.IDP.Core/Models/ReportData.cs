using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace H2020.IPMDecisions.IDP.Core.Models
{
    public class ReportData
    {
        public ReportData()
        {
            Farm = new ReportDataFarm();
        }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("farm")]
        public ReportDataFarm Farm { get; set; }
    }

    public class ReportDataFarm
    {
        public ReportDataFarm()
        {
            DssModels = new List<ReportDataDssModel>();
        }
        [JsonPropertyName("country")]
        public string Country { get; set; }
        [JsonPropertyName("dssModels")]
        public List<ReportDataDssModel> DssModels { get; set; }
    }

    public class ReportDataDssModel
    {
        [JsonPropertyName("modelName")]
        public string ModelName { get; set; }
        [JsonPropertyName("modelId")]
        public string ModelId { get; set; }
    }
}