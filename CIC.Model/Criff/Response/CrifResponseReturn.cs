using Newtonsoft.Json;

namespace CIC.Model.Criff.Response
{
    public class CrifResponseReturn
    {
        [JsonProperty("TIMESTAMP")]
        public string? Timestamp { get; set; }

        [JsonProperty("TRANSACTION_ID")]
        public string? TransactionId { get; set; }

        [JsonProperty("STATUS-CODE")]
        public int? StatusCode { get; set; }

        [JsonProperty("STATUS")]
        public bool Status { get; set; } = false;
        [JsonProperty("MESSAGE")]
        public string? message { get; set; } 

        [JsonProperty("DATA")]
        public CrifResponse? Data { get; set; }
    }
}
