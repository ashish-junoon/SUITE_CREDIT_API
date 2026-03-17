using CIC.Model.Criff.Request;
using CIC.Model.Criff.Response;
using CIC.Model.Experian.Response;
using LoggerLibrary;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlTypes;
using System.Security.AccessControl;
using System.Transactions;
using System.Xml.Linq;

namespace CIC.DataUtility.Repository
{
    public class ExperianRepository
    {
        public async static void PrepareAndSaveExperianResponseForDb(string xmlString,string company_id,string pdf_url,string connection,ILoggerManager logger)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xmlString))
                {
                    logger.LogError("Experian response string is empty. Cannot process.");
                    return;
                }
                XDocument doc = XDocument.Parse(xmlString);

                var applicantNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "Current_Applicant_Details");
                var firstName = applicantNode?.Descendants().FirstOrDefault(x => x.Name.LocalName == "First_Name")?.Value;
                var pan = applicantNode?.Descendants().FirstOrDefault(x => x.Name.LocalName == "IncomeTaxPan")?.Value;
                var addressNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "Current_Applicant_Address_Details");
                //var applicant = doc.Descendants("Current_Applicant_Details").FirstOrDefault();
                var score = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "SCORE");
               var creditScore = Convert.ToInt32(score?.Descendants().FirstOrDefault(x => x.Name.LocalName == "BureauScore")?.Value);
                //string dob = applicant?.Elements().FirstOrDefault(x => x.Name.LocalName == "Date_Of_Birth_Applicant")?.Value ?? string.Empty;
                //var addressNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "Current_Applicant_Address_Details");
                string address = "";
                if (addressNode != null)
                {
                    address = string.Join(" ",
                        addressNode.Elements()
                                   .Where(e => e.Name.LocalName == "FlatNoPlotNoHouseNo" ||
                                               e.Name.LocalName == "BldgNoSocietyName" ||
                                               e.Name.LocalName == "RoadNoNameAreaLocality" ||
                                               e.Name.LocalName == "City" ||
                                               e.Name.LocalName == "State" ||
                                               e.Name.LocalName == "PINCode" ||
                                               e.Name.LocalName == "Country_Code")
                                   .Select(e => e.Value)
                    ).Trim();
                }
                
                var header = doc.Descendants("Header").FirstOrDefault();
                string reportDate = header?.Element("ReportDate")?.Value ?? DateTime.Now.ToString("yyyyMMdd");
                string reportTime = header?.Element("ReportTime")?.Value ?? DateTime.Now.ToString("HHmmss");
                string exactMatch = doc.Descendants("match_result").FirstOrDefault()?.Element("Exact_match")?.Value ?? string.Empty;
                
                DateTime timestamp;
                if (!DateTime.TryParseExact(reportDate + reportTime,"yyyyMMddHHmmss",null,System.Globalization.DateTimeStyles.None,out timestamp))
                {
                    timestamp = DateTime.Now;
                }
                var creditData = new CIC.Model.Experian.Response.CreditScoreData
                {
                    Pan = pan,// applicant?.Element("IncomeTaxPan")?.Value ?? string.Empty,
                    Name = firstName,// string.Join(" ", applicant?.Element("First_Name")?.Value ?? string.Empty, applicant?.Element("Last_Name")?.Value ?? string.Empty).Trim(),
                    Mobile = !string.IsNullOrEmpty(applicantNode?.Element("MobilePhoneNumber")?.Value)
                             ? applicantNode.Element("MobilePhoneNumber")?.Value
                             : applicantNode?.Element("Telephone_Number_Applicant_1st")?.Value ?? string.Empty,
                    CreditScore = creditScore,
                    CreditReportLink = pdf_url ?? string.Empty,
                    address = address ?? string.Empty.Trim(),
                    dob = applicantNode?.Element("Date_Of_Birth_Applicant")?.Value ?? string.Empty,
                    Email = applicantNode?.Element("EMailId")?.Value ?? string.Empty ,
                    match_result = exactMatch ?? string.Empty
                };
                var experianResponsePdf = new ExperianResponsePdf
                {
                    Timestamp = timestamp,
                    TransactionId = Guid.NewGuid().ToString(),
                    Status = true,
                    Data = creditData,
                    message = doc?.Descendants()?.FirstOrDefault(x => x.Name?.LocalName == "UserMessageText").Value,
                    StatusCode = Convert.ToInt32(doc?.Descendants()?.FirstOrDefault(x => x.Name?.LocalName == "SystemCode").Value)
                };
                await Task.Run(() =>
                {
                    if (experianResponsePdf != null)
                    {
                        ExperianRepository.SaveExperianReport(
                            experianResponsePdf,
                            company_id,
                            connection ?? string.Empty,
                            logger
                        );
                    }
                    else
                    {
                        logger.LogError("ExperianResponsePdf is null. Cannot save to DB.");
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError("Error occurred while saving Experian response: " + ex.Message);
            }
        }

        public static async Task SaveExperianReport(ExperianResponsePdf response, string companyid, string connection, ILoggerManager logger)
        {
            SqlParameter[] param = new SqlParameter[13];
            param[0] = new SqlParameter("company_id", SqlDbType.VarChar, 10) { Value = companyid };
            param[1] = new SqlParameter("pan_number", SqlDbType.VarChar, 15) { Value = response?.Data?.Pan };
            param[2] = new SqlParameter("credit_score", SqlDbType.VarChar, 5) { Value = response?.Data?.CreditScore };
            param[3] = new SqlParameter("message", SqlDbType.VarChar, 200) { Value = response?.message};
            param[4] = new SqlParameter("provider", SqlDbType.VarChar, 30) { Value = "EXPERIAN" };
            param[5] = new SqlParameter("status", SqlDbType.VarChar, 30) { Value = response?.Status };
            param[6] = new SqlParameter("customer_name", SqlDbType.VarChar, 50) { Value = response?.Data?.Name };
            param[7] = new SqlParameter("mobile_number", SqlDbType.VarChar, 15) { Value = response?.Data?.Mobile };
            param[8] = new SqlParameter("match_result", SqlDbType.VarChar, 5) { Value = response?.Data?.match_result };
            param[9] = new SqlParameter("transaction_id", SqlDbType.VarChar, 200) { Value = response?.TransactionId };
            param[10] = new SqlParameter("dob", SqlDbType.VarChar, 20) { Value = response?.Data?.dob };
            param[11] = new SqlParameter("address", SqlDbType.VarChar, 500) { Value = response?.Data?.address };
            param[12] = new SqlParameter("email", SqlDbType.VarChar, 200) { Value = response?.Data?.Email };
            try
            {
                using (SqlConnection con = GetDBConnection.getConnection(connection))
                {
                    await con.OpenAsync();
                    SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "USP_Credit_Report_New", param);
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in SaveExperianReport");
                throw;
            }
        }

        public static void PrepareAndSaveCrifResponseForDbV1(CrifResponseReturn crifResponse, SoftPullRQV1 request, string company_id, string pdf_url, string connection, ILoggerManager logger)
        {
            CrifResponsePdf crifResponsePdf = new CrifResponsePdf();
            if (crifResponse?.Data?.B2CReport?.Header == null)
            {
                crifResponsePdf = new CrifResponsePdf
                {
                    Timestamp = Convert.ToString(DateTime.Now),
                    TransactionId = string.Empty,
                    StatusCode = 500,
                    Status = false,
                    Data = new CIC.Model.Criff.Response.CreditScoreData
                    {
                        uid_number = request?.pan_number ?? string.Empty,
                        Name = $"{request?.first_name ?? string.Empty} {request?.last_name ?? string.Empty}".Trim(),
                        Mobile = request?.mobile_number ?? string.Empty,
                        CreditScore = 0,
                        CreditReportLink = pdf_url ?? string.Empty,
                        address = string.Empty,
                        dob = null,
                        email = string.Empty
                    }
                };
            }
            else
            {
                var currentApplicant = crifResponse.Data.B2CReport.RequestData?.Applicant;
                var score = crifResponse.Data.B2CReport.ReportData?.StandardData?.Score;
                var dob = currentApplicant?.Dob?.Date;
                var address = currentApplicant?.Addresses?.FirstOrDefault()?.AddressText;
                var _email = currentApplicant?.Emails?.FirstOrDefault()?.Email;
                crifResponsePdf = new CrifResponsePdf
                {
                    Timestamp = crifResponse.Data.B2CReport.Header.DateOfRequest,
                    TransactionId = crifResponse.Data.B2CReport.Header.ReportId ?? string.Empty,
                    StatusCode = 200,
                    Status = true,
                    Data = new CIC.Model.Criff.Response.CreditScoreData
                    {
                        uid_number = $"{request?.pan_number},{request?.aadhaar_number}" ?? string.Empty,
                        Name = $"{request?.first_name ?? string.Empty} {request?.last_name ?? string.Empty}".Trim(),
                        Mobile = request?.mobile_number ?? string.Empty,
                        CreditScore = score != null && score.Any() && int.TryParse(score[0].Value, out var sc) ? sc : 0,
                        CreditReportLink = pdf_url ?? string.Empty,
                        address = address ?? string.Empty,
                        dob = dob,
                        email = _email ?? string.Empty
                    }
                };
            }
            ExperianRepository.SaveCrifReport(
                crifResponsePdf,
                company_id,
                connection ?? string.Empty,
                logger
            );
        }
        public static void PrepareAndSaveCrifResponseForDb(CrifResponseReturn crifResponse,SoftPullRQ request,string company_id,string pdf_url,string connection,ILoggerManager logger)
        {
            CrifResponsePdf crifResponsePdf = new CrifResponsePdf();
            if (crifResponse?.Data?.B2CReport?.Header == null)
            {
                crifResponsePdf = new CrifResponsePdf
                {
                    Timestamp =Convert.ToString(DateTime.Now),
                    TransactionId = string.Empty,
                    StatusCode = 500,
                    Status = false,
                    Data = new CIC.Model.Criff.Response.CreditScoreData
                    {
                        uid_number = request?.uid_number ?? string.Empty,
                        Name = $"{request?.first_name ?? string.Empty} {request?.last_name ?? string.Empty}".Trim(),
                        Mobile = request?.mobile_number ?? string.Empty,
                        CreditScore = 0,
                        CreditReportLink = pdf_url ?? string.Empty,
                        address = string.Empty,
                        dob = null,
                        email = string.Empty 
                    }
                };
            }
            else
            {
                var currentApplicant = crifResponse.Data.B2CReport.RequestData?.Applicant;
                var score = crifResponse.Data.B2CReport.ReportData?.StandardData?.Score;
                var dob = currentApplicant?.Dob?.Date;
                var address = currentApplicant?.Addresses?.FirstOrDefault()?.AddressText;
                var _email = currentApplicant?.Emails?.FirstOrDefault()?.Email;
                crifResponsePdf = new CrifResponsePdf
                {
                    Timestamp = crifResponse.Data.B2CReport.Header.DateOfRequest,
                    TransactionId = crifResponse.Data.B2CReport.Header.ReportId ?? string.Empty,
                    StatusCode = 200,
                    Status = true,
                    Data = new CIC.Model.Criff.Response.CreditScoreData
                    {
                        uid_number = request?.uid_number ?? string.Empty,
                        Name = $"{request?.first_name ?? string.Empty} {request?.last_name ?? string.Empty}".Trim(),
                        Mobile = request?.mobile_number ?? string.Empty,
                        CreditScore = score != null && score.Any() && int.TryParse(score[0].Value, out var sc) ? sc : 0,
                        CreditReportLink = pdf_url ?? string.Empty,
                        address = address ?? string.Empty,
                        dob = dob,
                        email = _email ?? string.Empty
                    }
                };
            }
            ExperianRepository.SaveCrifReport(
                crifResponsePdf,
                company_id,
                connection ?? string.Empty,
                logger
            );
        }

        public static void AuthQuestionnaireCrifResponseForDb(CrifResponseReturn crifResponse, string company_id, string pdf_url, string connection, ILoggerManager logger)
        {
            try
            {
                if (crifResponse == null || crifResponse.Data?.B2CReport == null)
                {
                    logger.LogError("CRIF response or B2CReport is null. Cannot process the response.");
                    return;
                }
                var currentApplicant = crifResponse.Data.B2CReport.RequestData?.Applicant;
                var score = crifResponse?.Data?.B2CReport?.ReportData?.StandardData?.Score?.FirstOrDefault()?.Value;
                var dob = currentApplicant?.Dob?.Date;
                var address = currentApplicant?.Addresses?.FirstOrDefault()?.AddressText;
                var _email = currentApplicant?.Emails?.FirstOrDefault()?.Email;
                var mobile = currentApplicant?.Phones?.FirstOrDefault()?.Value ?? string.Empty;
                string pan = currentApplicant?.Ids?.FirstOrDefault(x => x.Type == "PAN")?.Value ?? string.Empty;
                string name = string.Join(" ", currentApplicant?.FirstName ?? string.Empty, currentApplicant?.MiddleName ?? string.Empty,currentApplicant?.LastName ?? string.Empty).Trim();
                
                var crifResponsePdf = new CrifResponsePdf
                {
                    Timestamp = crifResponse?.Data?.B2CReport?.Header?.DateOfRequest,
                    TransactionId = crifResponse?.Data?.B2CReport?.Header?.ReportId ?? string.Empty,
                    StatusCode = 200,
                    Status = true,
                    Data = new CIC.Model.Criff.Response.CreditScoreData
                    {
                        uid_number = pan,
                        Name = name,
                        Mobile = mobile,
                        CreditScore = Convert.ToInt32(score),
                        CreditReportLink = pdf_url ?? string.Empty,
                        address = address ?? string.Empty,
                        dob = dob,
                        email = _email ?? string.Empty
                    }
                };
                ExperianRepository.SaveCrifReport(
                    crifResponsePdf,
                    company_id,
                    connection ?? string.Empty,
                    logger
                );
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, "Error occurred while processing CRIF response for DB.");
            }
        }

        public static async void SaveCrifReport(CrifResponsePdf response, string companyid, string dbconnection, ILoggerManager logger)
        {
            SqlParameter[] param = new SqlParameter[12];

            param[0] = new SqlParameter("company_id", SqlDbType.VarChar, 10);
            param[0].Value = companyid;

            param[1] = new SqlParameter("uid_number", SqlDbType.VarChar, 25);
            param[1].Value = response?.Data?.uid_number;

            param[2] = new SqlParameter("credit_score", SqlDbType.VarChar, 5);
            param[2].Value = response?.Data?.CreditScore;

            param[3] = new SqlParameter("message", SqlDbType.VarChar, 200);
            param[3].Value = response.Status ? "Normal Response" : "Error In Report";

            param[4] = new SqlParameter("provider", SqlDbType.VarChar, 30);
            param[4].Value = "CRIF";

            param[5] = new SqlParameter("status", SqlDbType.VarChar, 30);
            param[5].Value = response?.Status;

            param[6] = new SqlParameter("customer_name", SqlDbType.VarChar, 50);
            param[6].Value = response?.Data?.Name;

            param[7] = new SqlParameter("mobile_number", SqlDbType.VarChar, 12);
            param[7].Value = response?.Data?.Mobile;

            param[8] = new SqlParameter("address", SqlDbType.VarChar, 500);
            param[8].Value = response?.Data?.address;

            param[9] = new SqlParameter("dob", SqlDbType.VarChar, 20);
            param[9].Value = response?.Data?.dob;

            param[10] = new SqlParameter("email", SqlDbType.VarChar, 200);
            param[10].Value = response?.Data?.email;

            param[11] = new SqlParameter("transaction_id", SqlDbType.VarChar, 100);
            param[11].Value = response?.TransactionId;

            using (SqlConnection con = GetDBConnection.getConnection(dbconnection))
            {
                DataSet Objds = new DataSet();
                try
                {
                    Objds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "USP_Crif_Credit_Report", param);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error in SaveExperianReport: {ex.Message}");
                }
            }
        }

        public static async Task SaveCibilReport(string status,string score,string name,string pan,List<string> numbers,string transaction_id,string company_id,string vendor_code,string _Address,string dob,string email,string connection,ILoggerManager logger)
        {
                SqlParameter[] param = new SqlParameter[12];
                param[0] = new SqlParameter("company_id", SqlDbType.VarChar, 10) { Value = company_id ?? string.Empty };
                param[1] = new SqlParameter("pan_number", SqlDbType.VarChar, 15) { Value = pan ?? string.Empty };
                param[2] = new SqlParameter("credit_score", SqlDbType.VarChar, 5) { Value = score ?? string.Empty };
                param[3] = new SqlParameter("message", SqlDbType.VarChar, 200) { Value = status ?? string.Empty };
                param[4] = new SqlParameter("provider", SqlDbType.VarChar, 30) { Value = "CIBIL" };
                param[5] = new SqlParameter("status", SqlDbType.VarChar, 30) { Value = status ?? string.Empty };
                param[6] = new SqlParameter("customer_name", SqlDbType.VarChar, 50) { Value = name ?? string.Empty };
                param[7] = new SqlParameter("mobile_number", SqlDbType.VarChar, 15) { Value = numbers?.FirstOrDefault() ?? string.Empty };
                param[8] = new SqlParameter("transaction_id", SqlDbType.VarChar, 50) { Value = transaction_id ?? string.Empty };
                param[9] = new SqlParameter("_Address", SqlDbType.VarChar, 100) { Value = _Address ?? string.Empty };
                param[10] = new SqlParameter("dob", SqlDbType.VarChar, 20) { Value = dob ?? string.Empty };
                param[11] = new SqlParameter("email", SqlDbType.VarChar, 100) { Value = email ?? string.Empty };

                try
                {
                    logger.LogInfo("SaveCibilReport - Preparing to push data into database...");

                    using (SqlConnection con = GetDBConnection.getConnection(connection))
                    {
                        await con.OpenAsync();
                        await Task.Run(() =>
                        {
                            SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "USP_Cibil_Report", param);
                        });
                    }

                    logger.LogInfo("SaveCibilReport - Data saved successfully in database.");
                }
                catch (Exception ex)
                {
                    //logger.LogError(ex, "SaveCibilReport - Error occurred while saving data.");
                    //throw;
                }
            }

   
    }
}
