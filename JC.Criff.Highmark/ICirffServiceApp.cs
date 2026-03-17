using CIC.Model.Criff.Request;
using CIC.Model.Criff.Response;

namespace JC.Criff.Highmark
{
    public interface ICirffServiceApp
    {
        Task<dynamic> GetCreditReportAsync(SoftPullRQ requestBody, bool CRIF_HIGHMARK_SERVICES_PROD);
        Task<dynamic> AuthQuestionnaireCriff(AuthRQ requestBody, bool CRIF_HIGHMARK_SERVICES_PROD);
        Task<dynamic> GetCreditReportV1Async(SoftPullRQV1 requestBody, bool CRIF_HIGHMARK_SERVICES_PROD);
        Task<FusionParsedResponse> CriffPrefil(CrifPrefillRQ requestBody, bool CRIF_FUSION_PROD, string company_id);
    }
}
