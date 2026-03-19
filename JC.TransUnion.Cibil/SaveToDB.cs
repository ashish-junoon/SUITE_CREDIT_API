using CIC.DataUtility.Repository;
using CIC.Model.TransUnionCibil;
using LoggerLibrary;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace JC.TransUnion.Cibil
{
    public static class SaveToDB
    {
        public static void PushToDatabase(TransuniunReturnResponse result, string guid , string requiredHeader , string requiredcompanyid , string connection , ILoggerManager logger)
        {
            string assetsStatus = JsonSerializer.Serialize(result);
            JObject obj = null;
            try
            {
                obj = JObject.Parse(assetsStatus);
                string status = obj["GetCustomerAssetsResponse"]?["ResponseStatus"]?.ToString();
                string score = obj["GetCustomerAssetsResponse"]?["GetCustomerAssetsSuccess"]?["Asset"]?["TrueLinkCreditReport"]?["Borrower"]?["CreditScore"]?["riskScore"]?.ToString();
                string name = obj["GetCustomerAssetsResponse"]?["GetCustomerAssetsSuccess"]?["Asset"]?["TrueLinkCreditReport"]?["Borrower"]?["BorrowerName"]?["Name"]?["Forename"]?.ToString();
                var identifiers = obj.SelectToken("GetCustomerAssetsResponse.GetCustomerAssetsSuccess.Asset.TrueLinkCreditReport.Borrower.IdentifierPartition.Identifier") as JArray;
                string pan = identifiers?.FirstOrDefault(x => x["ID"]?["IdentifierName"]?.ToString() == "TaxId")?["ID"]?["Id"]?.ToString();
                var phones = obj.SelectToken("GetCustomerAssetsResponse.GetCustomerAssetsSuccess.Asset.TrueLinkCreditReport.Borrower.BorrowerTelephone") as JArray;
                List<string> numbers = phones?.Select(p => p["PhoneNumber"]?["Number"]?.ToString()).Where(n => !string.IsNullOrEmpty(n)).ToList();
                var tradeline = obj.SelectToken("GetCustomerAssetsResponse.GetCustomerAssetsSuccess.Asset.TrueLinkCreditReport.TradeLinePartition.Tradeline");
                string lender = tradeline?["creditorName"]?.ToString();
                string balance = tradeline?["currentBalance"]?.ToString();
                string overdue = tradeline?["GrantedTrade"]?["amountPastDue"]?.ToString();
                string transaction_id = guid;
                string company_id = requiredHeader;
                string vendor_code = requiredcompanyid;
                string dob = obj["GetCustomerAssetsResponse"]?["GetCustomerAssetsSuccess"]?["Asset"]?["TrueLinkCreditReport"]?["Borrower"]?["Birth"]?["date"]?.ToString();
                string email = obj["GetCustomerAssetsResponse"]?["GetCustomerAssetsSuccess"]?["Asset"]?["TrueLinkCreditReport"]?["Borrower"]?["EmailAddress"]?["Email"].FirstOrDefault()?.ToString();
                var address = obj.SelectToken("GetCustomerAssetsResponse.GetCustomerAssetsSuccess.Asset.TrueLinkCreditReport.Borrower.BorrowerAddress.CreditAddress");
                string street = address?["StreetAddress"]?.ToString();
                string city = address?["City"]?.ToString();
                string postalCode = address?["PostalCode"]?.ToString();
                string stateCode = address?["Region"]?.ToString();
                string _Address = street + city + postalCode + stateCode;

                logger.LogInfo("PushToDatabase - Data has been sent and saved successfully in the database.");
                try
                {
                   logger.LogInfo("PushToDatabase - Sending data to database...");
                   ExperianRepository.SaveCibilReport
                   (
                       status ?? string.Empty,
                        score ?? string.Empty,
                        name ?? string.Empty,
                        pan ?? string.Empty,
                        numbers ?? new List<string>(),
                        transaction_id ?? string.Empty,
                        vendor_code ?? string.Empty,
                        company_id ?? string.Empty,
                        _Address ?? string.Empty,
                        dob ?? string.Empty,
                        email ?? string.Empty,
                        connection ?? string.Empty,
                        logger
                    );
                    logger.LogInfo("PushToDatabase - Data saved successfully in database.");
                }
                catch (Exception ex)
                {
                    
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Invalid JSON: " + ex.Message);
            }
        }
    }
}
