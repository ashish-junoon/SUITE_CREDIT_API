using CIC.Model.TransUnionCibil;
using JC.TransUnion.Cibil.Models;

namespace CIC_Services.Interfaces
{
    public interface ITransunionCibilService
    {
        Task<TransuniunReturnResponse> GetCusomerCibilAsync(FulfillOfferRQ request , string requiredHeader , string requiredcompanyid);

    }
}
