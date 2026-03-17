namespace JC.TransUnion.Cibil.Models
{
    public class Responses
    {

        public class FulFillResposeRoot
        {
            public Fulfillofferresponse? FulfillOfferResponse { get; set; }
        }

        public class Fulfillofferresponse
        {
            public string ResponseStatus { get; set; }
            public string? ResponseKey { get; set; }
            public Fulfilloffersuccess? FulfillOfferSuccess { get; set; }
            public Fulfilloffererror? FulfillOfferError { get; set; }
        }

        public class Fulfilloffersuccess
        {
            public string? Status { get; set; }
        }
        public class Fulfilloffererror
        {
            public Failure? Failure { get; set; }
        }

        public class Failure
        {
            public string? FailureEnum { get; set; }
            public string? Message { get; set; }
            public string? ClientUserKey { get; set; }
        }
        public class AuthResponseRoot
        {
            public Getauthenticationquestionsresponse? GetAuthenticationQuestionsResponse { get; set; }
        }

        public class Getauthenticationquestionsresponse
        {
            public string ResponseStatus { get; set; }
            public string ResponseKey { get; set; }
            public Getauthenticationquestionssuccess GetAuthenticationQuestionsSuccess { get; set; }
        }

        public class Getauthenticationquestionssuccess
        {
            public string ChallengeConfigGUID { get; set; }
            public string IVStatus { get; set; }
        }

        public class WebTokenRS
        {
            public Getproductwebtokenresponse GetProductWebTokenResponse { get; set; }
        }

        public class Getproductwebtokenresponse
        {
            public string ResponseKey { get; set; }
            public string ResponseStatus { get; set; }
            public Getproductwebtokensuccess GetProductWebTokenSuccess { get; set; }
        }

        public class Getproductwebtokensuccess
        {
            public string PartnerCustomerId { get; set; }
            public string WebToken { get; set; }
        }




    }
}
