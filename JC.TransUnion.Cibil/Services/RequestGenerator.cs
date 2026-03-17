using CIC.Model.TransUnionCibil;
using JC.TransUnion.Cibil.Models;

namespace JC.TransUnion.Cibil.Services
{
    public static class RequestGenerator
    {
        public static Models.Requests.ProductWebTokenRequestRoot ReturnGetProductWebTokenRequest(ConfigModel configModel, string Unique)
        {
            return new Models.Requests.ProductWebTokenRequestRoot
            {
                GetProductWebTokenRequest = new Requests.Getproductwebtokenrequest
                {
                    SiteName = configModel.SITE_NAME,
                    AccountName = configModel.ACCOUNT_NAME,
                    AccountCode = configModel.ACCOUNT_CODE,
                    ClientKey = Unique,
                    RequestKey = Unique,
                    PartnerCustomerId = Unique,
                }
            };
        }
        public static Models.Requests.GetCustomerAssetsRequestRoot ReturnGetCustomerAssetsRequest(ConfigModel configModel, string Unique)
        {
            return new Models.Requests.GetCustomerAssetsRequestRoot
            {
                GetCustomerAssetsRequest = new Requests.Getcustomerassetsrequest
                {
                    SiteName = configModel.SITE_NAME,
                    AccountName = configModel.ACCOUNT_NAME,
                    AccountCode = configModel.ACCOUNT_CODE,
                    ClientKey = Unique,
                    RequestKey = Unique,
                    PartnerCustomerId = Unique,
                    LegalCopyStatus = "Accept"
                }
            };
        }
        public static Models.Requests.AuthRequestRoot ReturnAuthRequest(ConfigModel configModel, string Unique)
        {
            return new Models.Requests.AuthRequestRoot
            {
                GetAuthenticationQuestionsRequest = new Requests.Getauthenticationquestionsrequest
                {
                    AccountCode = configModel.ACCOUNT_CODE,
                    AccountName = configModel.ACCOUNT_NAME,
                    SiteName = configModel.SITE_NAME,
                    ClientKey = Unique,
                    RequestKey = Unique,
                    PartnerCustomerId = Unique,
                    LocaleType = "enIN"
                }
            };
        }

        public static Models.Requests.PingRequestRoot ReturnPingRequest(ConfigModel configModel)
        {
            return new Models.Requests.PingRequestRoot
            {
                PingRequest = new Requests.PingRequest
                {
                    SiteName = configModel.SITE_NAME,
                    AccountName = configModel.ACCOUNT_NAME,
                    AccountCode = configModel.ACCOUNT_CODE
                }
            };
        }

        public static Models.Requests.FulfillOfferRQRoot ReturnFulfillOfferRequest(ConfigModel configModel, FulfillOfferRQ req, string Unique)
        {
            return new Models.Requests.FulfillOfferRQRoot
            {
                FulfillOfferRequest = new Requests.Fulfillofferrequest
                {
                    CustomerInfo = new Requests.Customerinfo
                    {
                        Name = new Requests.Name
                        {
                            Forename = req.FulfillOfferRequest.CustomerInfo.Name.Forename,
                            Surname = req.FulfillOfferRequest.CustomerInfo.Name.Surname
                        },
                        IdentificationNumber = new Requests.Identificationnumber
                        {
                            IdentifierName = req.FulfillOfferRequest.CustomerInfo.IdentificationNumber.IdentifierName,
                            Id = req.FulfillOfferRequest.CustomerInfo.IdentificationNumber.Id
                        },
                        Address = new Requests.Address
                        {
                            PostalCode = req.FulfillOfferRequest.CustomerInfo.Address.PostalCode,
                            City = req.FulfillOfferRequest.CustomerInfo.Address.City,
                            Region = req.FulfillOfferRequest.CustomerInfo.Address.Region,
                            StreetAddress = req.FulfillOfferRequest.CustomerInfo.Address.StreetAddress,
                            AddressType = req.FulfillOfferRequest.CustomerInfo.Address.AddressType,
                            AddressCategoryType = req.FulfillOfferRequest.CustomerInfo.Address.AddressCategoryType
                        },
                        DateOfBirth = req.FulfillOfferRequest.CustomerInfo.DateOfBirth,
                        PhoneNumber = new Requests.Phonenumber
                        {
                            Number = req.FulfillOfferRequest.CustomerInfo.PhoneNumber.Number
                        },
                        Email = req.FulfillOfferRequest.CustomerInfo.Email,
                        Gender = req.FulfillOfferRequest.CustomerInfo.Gender
                    },
                    SiteName = configModel.SITE_NAME,
                    AccountName = configModel.ACCOUNT_NAME,
                    AccountCode = configModel.ACCOUNT_CODE,
                    ClientKey = Unique,
                    RequestKey = Unique,
                    ProductConfigurationId = configModel.PRODUCT_CONFIG_ID,
                    PartnerCustomerId = Unique,
                    LegalCopyStatus = "Accept",
                    IsIdentityVerified = "Y",
                }
            };
        }


    }
}
