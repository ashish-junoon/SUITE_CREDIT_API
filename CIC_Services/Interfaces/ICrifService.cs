using CIC.Model.Criff.Request;
using CIC.Model.Criff.Response;

namespace CIC_Services.Interfaces
{
    public interface ICrifService
    {
        Task<CrifResponseReturn> GetCreditReportAsync(SoftPullRQ request, bool CRIF_HIGHMARK_SERVICES_PROD);
        Task<CrifResponseReturn> GetCreditReportV1Async(SoftPullRQV1 request, bool CRIF_HIGHMARK_SERVICES_PROD);
        Task<CrifResponseReturn> AuthQuestionnaireCriff(AuthRQ request, bool CRIF_HIGHMARK_SERVICES_PROD);
        Task<CrifResponsePdf> AuthQuestionnaireCrifPdf(CrifResponseReturn? crifResponse, AuthRQ request, string company_id);
        Task<CrifResponsePdf> GetCreditReportPdfAsync(CrifResponseReturn? crifResponse, SoftPullRQ request,string company_id);
        Task<CrifResponsePdf> GetCreditReportPdfAsync(CrifResponseReturn? crifResponse, SoftPullRQV1 request, string company_id);
        Task<FusionResponseReturn> CriffPrefil(CrifPrefillRQ request, bool CRIF_FUSION_PROD, string company_id);
    }
}
