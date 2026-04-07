using CIC.DataUtility.Repository;
using CIC.Helper;
using CIC.Model.Criff.Request;
using CIC.Model.Criff.Response;
using CIC_Services.Interfaces;
using JC.Criff.Highmark;
using LoggerLibrary;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Globalization;
using System.Reflection;

namespace CIC_Services.Services
{
    public class CrifService : ICrifService
    {
        private readonly ICirffServiceApp _cirffServiceApp;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILoggerManager _logger;
        private readonly IOptions<CIC.DataUtility.AppSettingModel> _appsetting;
        private readonly MethodBase method;
        private readonly IWebHostEnvironment _env;
        public CrifService(IWebHostEnvironment env,ICirffServiceApp cirffServiceApp, IHttpContextAccessor contextAccessor, ILoggerManager logger, IOptions<CIC.DataUtility.AppSettingModel> options)
        {
            _cirffServiceApp = cirffServiceApp;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _appsetting = options;
            method = MethodBase.GetCurrentMethod();
            _env = env;
        }

        public async Task<FusionResponseReturn> CriffPrefil(CrifPrefillRQ request, bool CRIF_FUSION_PROD, string company_id)
        {
            var result = await _cirffServiceApp.CriffPrefil(request, CRIF_FUSION_PROD, company_id);
            var fusionresponse = CIC_Services.ResultParser.CiffFusion.ResultParser.ParseResponse(result);
            if (fusionresponse != null)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        ExperianRepository.SaveFusionReport(request,
                            fusionresponse,
                            company_id,
                            _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "",
                            _logger
                        );
                    });
                    _logger.LogInfo($"FusionResponse saved successfully for company_id: {company_id}, transaction_id: {fusionresponse?.transaction_id}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while saving FusionResponse for company_id: {company_id}, transaction_id: {fusionresponse?.transaction_id}, Exception: {ex.Message}");
                }
            }
            else
            {
                _logger.LogInfo($"company_id: {company_id}. CrifPrefillRQ: {JsonConvert.SerializeObject(request)} \n fusionresponse: {JsonConvert.SerializeObject(fusionresponse)}");
                _logger.LogError($"FusionResponse is null or empty for company_id: {company_id}. Nothing saved to database.");
            }

            string filePath =  SaveHtml(result, fusionresponse?.transaction_id);
            fusionresponse.data.html_url = filePath;
            return fusionresponse;
        }


        public string SaveHtml(FusionParsedResponse response, string trn_id)
        {
            var request = _contextAccessor?.HttpContext.Request;
            try
            {
                string htmlContent = response?.HtmlContent;
                string folderPath = Path.Combine(_env.ContentRootPath, "FusionReport");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, $"{trn_id}.html");

                File.WriteAllText(filePath, htmlContent);


                return $"{request.Scheme}://{request.Host}/FusionReport/{trn_id}.html";
            }
            catch (Exception ex)
            {
               _logger.LogError($"Class Name : {method.DeclaringType?.Name} Method Name: SaveHtml, Error Details: {ex.Message}");
                return null;
            }
           
        }

        public async Task<CrifResponseReturn> AuthQuestionnaireCriff(AuthRQ request, bool CRIF_HIGHMARK_SERVICES_PROD)
        {
            CrifResponse crifResponse = new CrifResponse();
            CrifResponseReturn responseReturn = new CrifResponseReturn();
            try
            {
                var result = await _cirffServiceApp.AuthQuestionnaireCriff(request, CRIF_HIGHMARK_SERVICES_PROD);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                if (result is ResponseStages)
                {
                    _logger.LogError($" Class Name : {method.DeclaringType?.Name} Method Name: {method.Name}, Status code return {((ResponseStages)result)?.status}, Response : {JsonConvert.SerializeObject(result)}");
                    responseReturn = new CrifResponseReturn
                    {
                        Timestamp = Convert.ToString(DateTime.UtcNow),
                        TransactionId = ((ResponseStages)result)?.reportId,
                        StatusCode = 200,
                        Status = true,
                        message = "success",
                        Data = new CrifResponse
                        {
                            orderid = ((ResponseStages)result)?.orderId,
                            redirectURL = ((ResponseStages)result)?.redirectURL,
                            reportId = ((ResponseStages)result)?.reportId,
                            status = ((ResponseStages)result)?.status,
                            statusDesc = ((ResponseStages)result)?.statusDesc,
                            question = ((ResponseStages)result)?.question,
                            optionsList = ((ResponseStages)result)?.optionsList
                        }
                    };
                    // return responseReturn;
                }
                else
                {
                    crifResponse = JsonConvert.DeserializeObject<CrifResponse>(result.ToString() ?? "", settings);
                    if (crifResponse.B2CReport.Header.Status == "SUCCESS")
                    {
                        responseReturn = new CrifResponseReturn
                        {
                            Timestamp = crifResponse.B2CReport.Header.DateOfRequest,
                            TransactionId = crifResponse.B2CReport.Header.ReportId,
                            Status = true,
                            StatusCode = 200,
                            message = "success",
                            Data = new CrifResponse
                            {
                                B2CReport = crifResponse.B2CReport
                            }
                        };
                    }
                    // return responseReturn;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Class Name : {method.DeclaringType?.Name} Method Name: AuthQuestionnaireCriff, Error Details: {ex.Message}");
            }

            return responseReturn;
        }

        public async Task<CrifResponsePdf> GetCreditReportPdfAsync(CrifResponseReturn crifResponse, SoftPullRQ request, string company_id)
        {
            CrifResponsePdf crifResponsePdf = new CrifResponsePdf();
            string pdfurl = string.Empty;
            try
            {
                var dict = GetLatestVariationValues(JsonConvert.SerializeObject(crifResponse));

                if (crifResponse.Data?.B2CReport?.Header?.Status?.ToUpper() == "SUCCESS")
                {
                    string html = CrifHtmlBuilder.BuildHtml(crifResponse, dict);
                    pdfurl = await HtmlToPdfService.GeneratePdfAndGetUrlAsync(html, "Crif_Report", _logger);
                    string domain = $"{_contextAccessor.HttpContext?.Request.Scheme}://{_contextAccessor.HttpContext?.Request.Host}";
                    pdfurl = $"{domain}/{pdfurl}";
                    _logger.LogInfo("Crif PDF URL: " + pdfurl);
                    var currentApplicant = crifResponse.Data?.B2CReport?.RequestData?.Applicant;
                    var score = crifResponse.Data?.B2CReport?.ReportData?.StandardData?.Score;
                    //var dob = crifResponse.Data?.B2CReport?.RequestData?.Applicant?.Dob?.Date;
                    // var address = crifResponse.Data?.B2CReport?.RequestData?.Applicant?.Addresses?[0].AddressText;
                    crifResponsePdf = new CrifResponsePdf
                    {
                        Timestamp = crifResponse.Data?.B2CReport.Header.DateOfRequest,
                        TransactionId = crifResponse.Data?.B2CReport.Header.ReportId,
                        StatusCode = 200,
                        Status = true,
                        Data = new CIC.Model.Criff.Response.CreditScoreData
                        {
                            uid_number = request?.uid_number,
                            Name = dict["NAME-VARIATIONS"], //request?.first_name + " " + request?.last_name,
                            Mobile = request?.mobile_number,
                            CreditScore = Convert.ToInt32(score?[0].Value),
                            CreditReportLink = pdfurl,
                            address = dict["ADDRESS-VARIATIONS"],
                            dob = dict["DOB-VARIATIONS"],
                        }
                    };
                   // _logger.LogInfo("Crif Response PDF Data: " + System.Text.Json.JsonSerializer.Serialize(crifResponsePdf));
                    //await Task.Run(() => ExperianRepository.SaveCrifReport(crifResponsePdf, company_id, _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger));

                }
            }
            catch (Exception exc)
            {
                _logger.LogError($"{exc.Message}");
                crifResponsePdf = new CrifResponsePdf
                {
                    Timestamp = Convert.ToString(DateTime.UtcNow),
                    TransactionId = crifResponse.Data?.B2CReport.Header.ReportId,
                    StatusCode = 500,
                    Status = false,
                    Data = null,
                    message = "fail"
                };
            }
            return crifResponsePdf;
        }

        public async Task<CrifResponsePdf> GetCreditReportPdfAsync(CrifResponseReturn crifResponse, SoftPullRQV1 request, string company_id)
        {
            CrifResponsePdf crifResponsePdf = new CrifResponsePdf();
            string pdfurl = string.Empty;
            try
            {
                if (crifResponse.Data?.B2CReport?.Header?.Status?.ToUpper() == "SUCCESS")
                {
                    var dict = GetLatestVariationValues(JsonConvert.SerializeObject(crifResponse));
                    string html = CrifHtmlBuilder.BuildHtml(crifResponse, dict);
                    pdfurl = await HtmlToPdfService.GeneratePdfAndGetUrlAsync(html, "Crif_Report", _logger);
                    string domain = $"{_contextAccessor.HttpContext?.Request.Scheme}://{_contextAccessor.HttpContext?.Request.Host}";
                    pdfurl = $"{domain}/{pdfurl}";
                    _logger.LogInfo("Crif PDF URL: " + pdfurl);
                    var currentApplicant = crifResponse.Data?.B2CReport?.RequestData?.Applicant;
                    var score = crifResponse.Data?.B2CReport?.ReportData?.StandardData?.Score;
                    var dob = crifResponse.Data?.B2CReport?.RequestData?.Applicant?.Dob?.Date;
                    var address = crifResponse.Data?.B2CReport?.RequestData?.Applicant?.Addresses?[0].AddressText;
                    crifResponsePdf = new CrifResponsePdf
                    {
                        Timestamp = crifResponse.Data?.B2CReport.Header.DateOfRequest,
                        TransactionId = crifResponse.Data?.B2CReport.Header.ReportId,
                        StatusCode = 200,
                        Status = true,
                        Data = new CIC.Model.Criff.Response.CreditScoreData
                        {
                            uid_number = request?.aadhaar_number,
                            Name = dict["NAME-VARIATIONS"], //request?.first_name + " " + request?.last_name,
                            Mobile = request?.mobile_number,
                            CreditScore = Convert.ToInt32(score?[0].Value),
                            CreditReportLink = pdfurl,
                            address = dict["ADDRESS-VARIATIONS"],
                            dob = dict["DOB-VARIATIONS"],
                        }
                    };
                    _logger.LogInfo("Crif Response PDF Data: " + System.Text.Json.JsonSerializer.Serialize(crifResponsePdf));
                    //await Task.Run(() => ExperianRepository.SaveCrifReport(crifResponsePdf, company_id, _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger));

                }
            }
            catch (Exception exc)
            {
                _logger.LogError($"{exc.Message}");
                crifResponsePdf = new CrifResponsePdf
                {
                    Timestamp = Convert.ToString(DateTime.UtcNow),
                    TransactionId = crifResponse.Data?.B2CReport?.Header?.ReportId,
                    StatusCode = 500,
                    Status = false,
                    Data = null,
                    message = "fail"
                };
            }
            return crifResponsePdf;
        }

        public async Task<CrifResponseReturn> GetCreditReportAsync(SoftPullRQ request, bool CRIF_HIGHMARK_SERVICES_PROD)
        {
            CrifResponse crifResponse = new();
            CrifResponseReturn responseReturn = new();
            try
            {
                var result = await _cirffServiceApp.GetCreditReportAsync(request, CRIF_HIGHMARK_SERVICES_PROD);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                _logger.LogInfo($"GetCreditReportAsync Class Name : {method.DeclaringType?.Name} Method Name: {method.Name}, Raw Response: {JsonConvert.SerializeObject(result)}");

                try
                {
                    if (result is not ResponseStages)
                    {
                        crifResponse = JsonConvert.DeserializeObject<CrifResponse>(result.ToString() ?? "", settings);
                        if (crifResponse?.B2CReport?.Header?.Status == "SUCCESS")
                        {
                            responseReturn = new CrifResponseReturn
                            {
                                Timestamp = crifResponse.B2CReport.Header.DateOfRequest,
                                TransactionId = crifResponse.B2CReport.Header.ReportId,
                                Status = true,
                                StatusCode = 200,
                                message = "success",
                                Data = new CrifResponse
                                {
                                    B2CReport = crifResponse.B2CReport
                                }
                            };
                        }
                    }
                    else
                    {
                        //_logger.LogError($" Class Name : {method.DeclaringType?.Name} Method Name: {method.Name}, Status code return {((ResponseStages)result)?.status}, Response : {JsonConvert.SerializeObject(result)}");
                        responseReturn = new CrifResponseReturn
                        {
                            Timestamp = Convert.ToString(DateTime.UtcNow),
                            TransactionId = ((ResponseStages)result).reportId,
                            StatusCode = 401,
                            Status = false,
                            message = "success",
                            Data = new CrifResponse
                            {
                                orderid = ((ResponseStages)result).orderId,
                                redirectURL = ((ResponseStages)result)?.redirectURL,
                                reportId = ((ResponseStages)result).reportId,
                                status = ((ResponseStages)result).status,
                                statusDesc = ((ResponseStages)result).statusDesc,
                                question= ((ResponseStages)result)?.question,
                                optionsList = ((ResponseStages)result)?.optionsList
                            }
                        };
                    }
                }
                catch (JsonReaderException jsonEx)
                {
                    _logger.LogError($"Class Name : {method.DeclaringType?.Name} Method Name: {method.Name}, JSON Parsing Error: {jsonEx.Message}, Response: {result}");
                    responseReturn = new CrifResponseReturn
                    {
                        Timestamp = Convert.ToString(DateTime.UtcNow),
                        TransactionId = ((ResponseStages)result).reportId,
                        StatusCode = 401,
                        Status = true,
                        message = "fail",
                        Data = new CrifResponse
                        {
                            orderid = ((ResponseStages)result).orderId,
                            redirectURL = ((ResponseStages)result).redirectURL,
                            reportId = ((ResponseStages)result).reportId,
                            status = ((ResponseStages)result).status,
                            statusDesc = ((ResponseStages)result).statusDesc
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Class Name : {method.DeclaringType?.Name} Method Name: GetCreditReportAsync, Error Details: {ex.Message}");
                responseReturn = new CrifResponseReturn
                {
                    Timestamp = Convert.ToString(DateTime.UtcNow),
                    TransactionId = null,
                    StatusCode = 500,
                    Status = false,
                    message = "An error occurred while processing the credit report request."
                };
            }
            return responseReturn;
        }

        public async Task<CrifResponseReturn> GetCreditReportV1Async(SoftPullRQV1 request, bool CRIF_HIGHMARK_SERVICES_PROD)
        {
            CrifResponse crifResponse = new();
            CrifResponseReturn responseReturn = new();
            try
            {
                var result = await _cirffServiceApp.GetCreditReportV1Async(request, CRIF_HIGHMARK_SERVICES_PROD);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                try
                {
                    if (result is not ResponseStages)
                    {
                        crifResponse = JsonConvert.DeserializeObject<CrifResponse>(result.ToString() ?? "", settings);
                        if (crifResponse.B2CReport.Header.Status == "SUCCESS")
                        {
                            responseReturn = new CrifResponseReturn
                            {
                                Timestamp = crifResponse.B2CReport.Header.DateOfRequest,
                                TransactionId = crifResponse.B2CReport.Header.ReportId,
                                Status = true,
                                StatusCode = 200,
                                message = "success",
                                Data = new CrifResponse
                                {
                                    B2CReport = crifResponse.B2CReport
                                }
                            };
                        }
                    }
                    else
                    {
                        _logger.LogError($" Class Name : {method.DeclaringType?.Name} Method Name: {method.Name}, Status code return {((ResponseStages)result)?.status}, Response : {JsonConvert.SerializeObject(result)}");
                        responseReturn = new CrifResponseReturn
                        {
                            Timestamp = Convert.ToString(DateTime.UtcNow),
                            TransactionId = ((ResponseStages)result).reportId,
                            StatusCode = 401,
                            Status = false,
                            message = "success",
                            Data = new CrifResponse
                            {
                                orderid = ((ResponseStages)result).orderId,
                                redirectURL = ((ResponseStages)result)?.redirectURL,
                                reportId = ((ResponseStages)result).reportId,
                                status = ((ResponseStages)result).status,
                                statusDesc = ((ResponseStages)result).statusDesc
                            }
                        };
                    }
                }
                catch (JsonReaderException jsonEx)
                {
                    _logger.LogError($"Class Name : {method.DeclaringType?.Name} Method Name: {method.Name}, JSON Parsing Error: {jsonEx.Message}, Response: {result}");
                    responseReturn = new CrifResponseReturn
                    {
                        Timestamp = Convert.ToString(DateTime.UtcNow),
                        TransactionId = ((ResponseStages)result).reportId,
                        StatusCode = 401,
                        Status = true,
                        message = "fail",
                        Data = new CrifResponse
                        {
                            orderid = ((ResponseStages)result).orderId,
                            redirectURL = ((ResponseStages)result).redirectURL,
                            reportId = ((ResponseStages)result).reportId,
                            status = ((ResponseStages)result).status,
                            statusDesc = ((ResponseStages)result).statusDesc
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Class Name : {method.DeclaringType?.Name} Method Name: GetCreditReportAsync, Error Details: {ex.Message}");
                responseReturn = new CrifResponseReturn
                {
                    Timestamp = Convert.ToString(DateTime.UtcNow),
                    TransactionId = null,
                    StatusCode = 500,
                    Status = false,
                    message = "An error occurred while processing the credit report request."
                };
            }
            return responseReturn;
        }

        public async Task<CrifResponsePdf> AuthQuestionnaireCrifPdf(CrifResponseReturn crifResponse, AuthRQ request, string company_id)
        {
            CrifResponsePdf crifResponsePdf = new CrifResponsePdf();
            string pdfurl = string.Empty;
            try
            {
                if (crifResponse.Data?.B2CReport?.Header?.Status?.ToUpper() == "SUCCESS")
                {
                    var dict = GetLatestVariationValues(JsonConvert.SerializeObject(crifResponse));
                    string html = CrifHtmlBuilder.BuildHtml(crifResponse, dict);
                    pdfurl = await HtmlToPdfService.GeneratePdfAndGetUrlAsync(html, "Crif_Report", _logger);
                    string domain = $"{_contextAccessor.HttpContext?.Request.Scheme}://{_contextAccessor.HttpContext?.Request.Host}";
                    pdfurl = $"{domain}/{pdfurl}";
                    _logger.LogInfo("Crif PDF URL: " + pdfurl);
                    var currentApplicant = crifResponse.Data?.B2CReport?.RequestData?.Applicant;
                    var score = crifResponse.Data?.B2CReport?.ReportData?.StandardData?.Score;
                    var dob = crifResponse.Data?.B2CReport?.RequestData?.Applicant?.Dob?.Date;
                    var address = crifResponse.Data?.B2CReport?.RequestData?.Applicant?.Addresses?[0].AddressText;
                    crifResponsePdf = new CrifResponsePdf
                    {
                        Timestamp = crifResponse.Data?.B2CReport.Header.DateOfRequest,
                        TransactionId = crifResponse.Data?.B2CReport.Header.ReportId,
                        StatusCode = 200,
                        Status = true,
                        message = "success",
                        Data = new CIC.Model.Criff.Response.CreditScoreData
                        {
                            //Pan = request?.pan_number,
                            //Name = request?.first_name + " " + request?.last_name,
                            //Mobile = request?.mobile_number,
                            CreditScore = Convert.ToInt32(score?[0].Value),
                            CreditReportLink = pdfurl,
                            address = address,
                            dob = dob,
                        }
                    };
                    _logger.LogInfo("Crif Response PDF Data: " + System.Text.Json.JsonSerializer.Serialize(crifResponsePdf));
                    //await Task.Run(() => ExperianRepository.SaveCrifReport(crifResponsePdf, company_id, _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger));

                }
            }
            catch (Exception exc)
            {
                _logger.LogError($"{exc.Message}");
                crifResponsePdf = new CrifResponsePdf
                {
                    Timestamp = Convert.ToString(DateTime.UtcNow),
                    TransactionId = crifResponse.Data?.B2CReport.Header.ReportId,
                    StatusCode = 500,
                    Status = false,
                    Data = null,
                    message = "fail"
                };
            }
            return crifResponsePdf;
        }

        public static Dictionary<string, string> GetLatestVariationValues(string json)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(json))
                return result;

            JObject obj;
            try
            {
                obj = JObject.Parse(json);
            }
            catch
            {
                return result; // invalid JSON
            }

            var variations = obj.SelectToken("DATA.B2C-REPORT.REPORT-DATA.STANDARD-DATA.DEMOGS.VARIATIONS")
                             as JArray;

            if (variations == null || !variations.Any())
                return result;

            foreach (var variationGroup in variations)
            {
                var type = variationGroup?["TYPE"]?.ToString();

                if (string.IsNullOrWhiteSpace(type))
                    continue;

                var variationArray = variationGroup["VARIATION"] as JArray;

                if (variationArray == null || !variationArray.Any())
                    continue;

                DateTime latestDate = DateTime.MinValue;
                string latestValue = null;

                foreach (var item in variationArray)
                {
                    var reportedDateStr = item?["REPORTED-DT"]?.ToString();
                    var value = item?["VALUE"]?.ToString();

                    if (string.IsNullOrWhiteSpace(reportedDateStr) || string.IsNullOrWhiteSpace(value))
                        continue;

                    if (DateTime.TryParseExact(
                            reportedDateStr,
                            "dd-MM-yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out DateTime parsedDate))
                    {
                        if (parsedDate > latestDate)
                        {
                            latestDate = parsedDate;
                            latestValue = value;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(latestValue))
                {
                    result[type] = latestValue;
                }
            }

            return result;
        }


    }
}
