using CIC.Model.Experian.Request;
using CIC.Model.Experian.Response;

namespace CIC_Services.Interfaces
{
    public interface IExperianService
    {
        Task<CIC.Model.Experian.Response.ExperianResponse> GetCreditReportAsync(ExperianRequest request, string requiredcompanyid);
        Task<CIC.Model.Experian.Response.ExperianResponsePdf> GetCreditReportPdf(CIC.Model.Experian.ResponseNew.ExperianReturnResponseV1 response, string company_id);

    }
}
