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
        public GetCustomerAssetsModel? response { get; set; }
        public string? cibilURL { get; set; }
        public string? score { get; set; }
        public string? custName { get; set; }
        public string? contactNo { get; set; }
        public string? emailAddress { get; set; }
    }
    

}
