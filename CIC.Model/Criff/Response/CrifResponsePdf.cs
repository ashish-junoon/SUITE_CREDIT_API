using Newtonsoft.Json;

namespace CIC.Model.Criff.Response
{
    public class CrifResponsePdf
    {
        [JsonProperty("TIMESTAMP")]
        public string? Timestamp { get; set; }

        [JsonProperty("TRANSACTION_ID")]
        public string? TransactionId { get; set; }

        [JsonProperty("STATUS-CODE")]
        public int? StatusCode { get; set; }

        [JsonProperty("STATUS")]
        public bool Status { get; set; } = false;

        [JsonProperty("DATA")]
        public CreditScoreData? Data { get; set; }
        [JsonProperty("MESSAGE")]
        public string? message { get; set; }
    }
    public class CreditScoreData
    {
        [JsonProperty("UID_NUMBER")]
        public string? uid_number { get; set; }

        [JsonProperty("NAME")]
        public string? Name { get; set; }

        [JsonProperty("MOBILE")]
        public string? Mobile { get; set; }

        [JsonProperty("CREDIT_SCORE")]
        public int CreditScore { get; set; }

        [JsonProperty("CREDIT_REPORT_LINK")]
        public string? CreditReportLink { get; set; }

        [JsonProperty("ADDRESS")]
        public string? address { get; set; }

        [JsonProperty("DOB")]
        public string? dob { get; set; }

        [JsonProperty("EMAIL")]
        public string? email { get; set; }

        //[JsonProperty("transaction_id")]
        //public string? transaction_id { get; set; }
    }
}
