using CIC.Model.TransUnionCibil;

namespace CIC_Services.Interfaces
{
    public interface ITransunionCibilService
    {
        Task<TransuniunReturnResponse> GetCusomerCibilAsync(FulfillOfferRQ request , string requiredHeader , string requiredcompanyid);

    }
}
