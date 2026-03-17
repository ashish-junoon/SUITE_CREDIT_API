using Newtonsoft.Json.Linq;

namespace CIC.Model.TransUnionCibil
{
    public class TransuniunReturnResponse
    {
        public int Status { get; set; }
        public ResultResponse? Data { get; set; }
        public DateTime timestamp { get; set; }
        public string? transaction_id { get; set; }
        public bool success { get; set; } = false;
        public string? message { get; set; }
    }
    public class ResultResponse
    {
        public object? response { get; set; }
        public string? cibilURL { get; set; }
    }
    //public class ResultResponse
    //{
    //    public JToken? response { get; set; }   // ✅ object ki jagah JToken
    //    public string? cibilURL { get; set; }
    //}

}
