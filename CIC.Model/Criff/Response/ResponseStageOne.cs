namespace CIC.Model.Criff.Response
{
    public class ResponseStages
    {
        public string? redirectURL { get; set; }
        public string? reportId { get; set; }
        public string? orderId { get; set; }
        public string? status { get; set; }
        public string? statusDesc { get; set; }
        public string? buttonbehaviour { get; set; }
        public string? question { get; set; }
        public List<string>? optionsList { get; set; }
    }
}
