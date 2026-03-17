namespace JC.Experian.Interfaces
{
    public interface IExperianSoapClient
    {
        Task<string> FetchCreditReportAsync(CIC.Model.Experian.Request.ExperianRequest request);
    }
}
