namespace JC.TransUnion.Cibil.Models
{
    public class CibilApiResponse
    {
        public int Status { get; set; }
        public BaseResponse? Data { get; set; }
        public DateTime timestamp => DateTime.UtcNow;
        public string? transaction_id { get; set; }
        public bool success { get; set; } = false;
        public string? message { get; set; }


    }

    public class BaseResponse
    {
        public object? response { get; set; }
        public string? cibilURL { get; set; }
    }

    //public class ResultResponse
    //{
    //    public string cibilURL { get; set; }
    //    public string RawResponse { get; set; }   // original response for audit
    //    public string Score { get; set; }
    //    public string Name { get; set; }
    //    public string DOB { get; set; }
    //    public string PAN { get; set; }
    //    // Add more fields here as per API response
    //}
}
