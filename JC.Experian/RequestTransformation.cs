using CIC.Model.Experian.Request;
using JC.Experian.ExperianModel;

namespace JC.Experian
{
    public static class RequestTransformation
    {
        public static ExperianApiRequest TransformRequest(ExperianRequest payload)
        {
            // Example transformation: Convert to uppercase
            return new ExperianApiRequest
            {
                Name = payload.Name.ToUpper(),
                Pan = payload.Pan.ToUpper(),
                Mobile = payload.Mobile,
                Consent = true // Set consent to true by default
            };
        }
    }
}
