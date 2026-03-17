using CIC.Model.TransUnionCibil;
using JC.TransUnion.Cibil.Models;

namespace JC.TransUnion.Cibil.Interface
{
    public interface ICibilService
    {
        // Task<CibilApiResponse> Ping(object request);

        Task<CibilApiResponse> GetCusomerCibil(FulfillOfferRQ request  , string requiredHeader, string requiredcompanyid);

        //Task<CibilApiResponse> AuthQuestions(object request);

        //Task<CibilApiResponse> Assets(object request);

        //Task<CibilApiResponse> WebToken(object request);
    }
}
