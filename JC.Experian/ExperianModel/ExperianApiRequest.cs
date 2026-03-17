namespace JC.Experian.ExperianModel
{
    public class ExperianApiRequest
    {
        public string Name { get; set; }
        public string Pan { get; set; }
        public string Mobile { get; set; }
        public bool Consent { get; set; } = true;
    }
}
