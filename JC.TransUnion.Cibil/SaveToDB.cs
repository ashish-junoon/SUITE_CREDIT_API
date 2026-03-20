using CIC.DataUtility.Repository;
using CIC.Model.TransUnionCibil;
using LoggerLibrary;

namespace JC.TransUnion.Cibil
{
    public static class SaveToDB
    {
        public static void PushToDatabase(FulfillOfferRQ request,TransuniunReturnResponse result, string requiredHeader , string requiredcompanyid , string connection , ILoggerManager logger)
        {
            //string assetsStatus = JsonSerializer.Serialize(result);
            //JObject obj = null;
            try
            {
                //obj = JObject.Parse(assetsStatus);
                string status = result.message;
                string score = result.Data.score;
                string name = result.Data.custName;
                string pan = request.FulfillOfferRequest.CustomerInfo.IdentificationNumber.Id;
                var phones = result.Data.contactNo;

                
                string transaction_id = result.transaction_id;
                string company_id = requiredHeader;
                string vendor_code = requiredcompanyid;
               
                string email = result.Data.emailAddress;
               
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
                        transaction_id ?? string.Empty,
                        vendor_code ?? string.Empty,
                        company_id ?? string.Empty,
                        email ?? string.Empty,
                        phones ?? string.Empty,
                        connection ?? string.Empty,
                        logger
                    );
                    logger.LogInfo("PushToDatabase - Data saved successfully in database.");
                }
                catch (Exception ex)
                {
                    logger.LogError("PushToDatabase Trying Push to DB: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Invalid JSON: " + ex.Message);
            }
        }
    }
}
