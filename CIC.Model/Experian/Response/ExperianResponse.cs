namespace CIC.Model.Experian.Response
{
    public class ExperianResponse
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Transaction_id { get; set; } = Guid.NewGuid().ToString();
        public int StatusCode { get; set; }

        public bool Success { get; set; }

        public string? Message { get; set; }

        public string? MessageCode { get; set; }

        public object? Data { get; set; }

        public string? Error { get; set; }
    }



}
