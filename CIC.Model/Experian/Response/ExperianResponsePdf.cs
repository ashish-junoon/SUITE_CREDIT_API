using System.Text.Json.Serialization;

namespace CIC.Model.Experian.Response
{
    public class ExperianResponsePdf
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("transaction_id")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; } = false;
        [JsonPropertyName("StatusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("data")]
        public CreditScoreData? Data { get; set; }
        [JsonPropertyName("message")]
        public string? message { get; set; }
    }
    public class CreditScoreData
    {
        [JsonPropertyName("pan")]
        public string? Pan { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("mobile")]
        public string? Mobile { get; set; }

        [JsonPropertyName("match_result")]
        public string? match_result { get; set; }

        [JsonPropertyName("credit_score")]
        public int CreditScore { get; set; }

        [JsonPropertyName("credit_report_link")]
        public string? CreditReportLink { get; set; }

        [JsonPropertyName("address")]
        public string? address { get; set; }

        [JsonPropertyName("dob")]
        public string? dob { get; set; }

        [JsonPropertyName("Email")]
        public string? Email { get; set; }
    }
}
