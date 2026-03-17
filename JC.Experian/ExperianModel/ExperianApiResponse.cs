namespace JC.Experian.ExperianModel
{
    public class ExperianApiResponse
    {
        public int StatusCode { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

        public string MessageCode { get; set; }

        public object Data { get; set; }

        public string Error { get; set; }
    }
}
