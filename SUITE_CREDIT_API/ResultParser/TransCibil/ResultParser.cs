using CIC.Model.TransUnionCibil;
using JC.TransUnion.Cibil.Models;
using LoggerLibrary;

namespace CIC_Services.ResultParser.TransCibil
{
    public static class ResultParser
    {
        public static TransuniunReturnResponse ParseResponse(CibilApiResponse cibilApi, ILoggerManager _logger)
        {
            return new TransuniunReturnResponse
            {
                Data = new CIC.Model.TransUnionCibil.ResultResponse
                {
                    cibilURL = cibilApi.Data?.cibilURL,
                    response = cibilApi.Data?.response,
                    score = cibilApi.Data?.response?.GetCustomerAssetsResponse?.GetCustomerAssetsSuccess?.Asset?.TrueLinkCreditReport?.Borrower?.CreditScore?.riskScore,
                    custName = cibilApi.Data?.response?.GetCustomerAssetsResponse?.GetCustomerAssetsSuccess?.Asset?.TrueLinkCreditReport?.Borrower?.BorrowerName?.Name?.Forename,
                    contactNo = cibilApi.Data?.response?.GetCustomerAssetsResponse?.GetCustomerAssetsSuccess?.Asset?.TrueLinkCreditReport?.Borrower?.BorrowerTelephone?.FirstOrDefault()?.PhoneNumber?.Number,
                    emailAddress = cibilApi.Data?.response?.GetCustomerAssetsResponse?.GetCustomerAssetsSuccess?.Asset?.TrueLinkCreditReport?.Borrower?.EmailAddress?.FirstOrDefault()?.Email
                },
                message = cibilApi.message,
                Status = cibilApi.Status,
                success = cibilApi.success,
                timestamp = cibilApi.timestamp,
                transaction_id = cibilApi.transaction_id
            };
        }

        //public static TransuniunReturnResponse ParseResponse(CibilApiResponse cibilApi)
        //{
        //    return new TransuniunReturnResponse
        //    {
        //        Data = MapResultData(cibilApi),
        //        message = cibilApi?.message ?? "No message",
        //        //Status = cibilApi?.Status ?? "Unknown",
        //        success = cibilApi?.success ?? false,
        //        timestamp = cibilApi?.timestamp ?? DateTime.UtcNow,
        //        transaction_id = cibilApi?.transaction_id ?? Guid.NewGuid().ToString()
        //    };
        //}

        //private static CIC.Model.TransUnionCibil.ResultResponse MapResultData(CibilApiResponse cibilApi)
        //{
        //    var result = new CIC.Model.TransUnionCibil.ResultResponse
        //    {
        //        cibilURL = cibilApi?.Data?.cibilURL ?? string.Empty,
        //        RawResponse = cibilApi?.Data?.response ?? string.Empty
        //    };

        //    if (!string.IsNullOrEmpty(result.RawResponse))
        //    {
        //        try
        //        {
        //            var parsed = JsonConvert.DeserializeObject<dynamic>(result.RawResponse);

        //            result.response. = parsed?.score ?? string.Empty;
        //            result.Name = parsed?.name ?? string.Empty;
        //            result.DOB = parsed?.dob ?? string.Empty;
        //            result.PAN = parsed?.pan ?? string.Empty;
        //        }
        //        catch (Exception ex)
        //        {
        //            // log error, keep RawResponse for debugging
        //        }
        //    }

        //    return result;
        //}

    }
}
