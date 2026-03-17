namespace JC.TransUnion.Cibil.Models
{
    public class Requests
    {
        public class PingRequestRoot { public PingRequest PingRequest { get; set; } }

        public class PingRequest
        {
            public string SiteName { get; set; }
            public string AccountName { get; set; }
            public string AccountCode { get; set; }
            public string ClientKey { get; set; }
            public string RequestKey { get; set; }
        }


        public class PingResponseRoot
        {
            public Pingresponse PingResponse { get; set; }
        }

        public class Pingresponse
        {
            public string ResponseStatus { get; set; }
            public string ResponseKey { get; set; }
        }


        public class FulfillOfferRQRoot
        {
            public Fulfillofferrequest FulfillOfferRequest { get; set; }
        }

        public class Fulfillofferrequest : PingRequest
        {
            public Customerinfo CustomerInfo { get; set; }
            public string ProductConfigurationId { get; set; }
            public string PartnerCustomerId { get; set; }
            public string LegalCopyStatus { get; set; }
            public string IsIdentityVerified { get; set; }
        }

        public class Customerinfo
        {
            public Name Name { get; set; }
            public Identificationnumber IdentificationNumber { get; set; }
            public Address Address { get; set; }
            public string DateOfBirth { get; set; }
            public Phonenumber PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Gender { get; set; }
        }

        public class Name
        {
            public string Forename { get; set; }
            public string Surname { get; set; }
        }

        public class Identificationnumber
        {
            public string IdentifierName { get; set; }
            public string Id { get; set; }
        }

        public class Address
        {
            public string PostalCode { get; set; }
            public string City { get; set; }
            public string Region { get; set; }
            public string StreetAddress { get; set; }
            public string AddressType { get; set; }
            public string AddressCategoryType { get; set; }
        }

        public class Phonenumber
        {
            public string Number { get; set; }
        }





        public class AuthRequestRoot
        {
            public Getauthenticationquestionsrequest GetAuthenticationQuestionsRequest { get; set; }
        }

        public class Getauthenticationquestionsrequest : PingRequest
        {
            public string PartnerCustomerId { get; set; }
            public string LocaleType { get; set; } = "enIN";
        }



        public class GetCustomerAssetsRequestRoot
        {
            public Getcustomerassetsrequest GetCustomerAssetsRequest { get; set; }
        }

        public class Getcustomerassetsrequest
        {
            public string SiteName { get; set; }
            public string AccountName { get; set; }
            public string AccountCode { get; set; }
            public string ClientKey { get; set; }
            public string RequestKey { get; set; }
            public string PartnerCustomerId { get; set; }
            public string LegalCopyStatus { get; set; }
        }


        public class ProductWebTokenRequestRoot
        {
            public Getproductwebtokenrequest GetProductWebTokenRequest { get; set; }
        }


        public class Getproductwebtokenrequest : PingRequest
        {
            public string PartnerCustomerId { get; set; }
        }

    }
}
