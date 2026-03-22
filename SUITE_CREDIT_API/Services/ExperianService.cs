using CIC.DataUtility.Repository;
using CIC.Helper;
using CIC.Model.Experian.Request;
using CIC.Model.Experian.Response;
using CIC_Services.Interfaces;
using JC.Experian;
using JC.Experian.Interfaces;
using LoggerLibrary;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Xml;

namespace CIC_Services.Services
{
    public class ExperianService : IExperianService
    {
        private readonly IExperianSoapClient _soapClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOptions<CIC.DataUtility.AppSettingModel> _appsetting;
        private readonly ILoggerManager _logger;
        private readonly ExperianRepository _experianRepository;

        public ExperianService(IExperianSoapClient soapClient, IHttpContextAccessor httpContextAccessor, IOptions<CIC.DataUtility.AppSettingModel> options, ILoggerManager logger)
        {
            _soapClient = soapClient;
            _contextAccessor = httpContextAccessor;
            _appsetting = options;
            _logger = logger;
            
        }
        public async Task<CIC.Model.Experian.Response.ExperianResponse> GetCreditReportAsync(ExperianRequest req, string requiredcompanyid)
        {
            try
            {
                CIC.Helper.ValidationHelper.ValidatePan(req.Pan);
            }
            catch (Exception ex)
            {
                return new CIC.Model.Experian.Response.ExperianResponse
                {
                    StatusCode = ((CIC.Helper.ApiException)ex).StatusCode,
                    Success = false,
                    Message = "Experian credit report fetched failed",
                    Data = ex.Message
                };
            }
            var rawXml = await _soapClient.FetchCreditReportAsync(req);
           // _logger.LogInfo("ExperianService GetCreditReportAsync Experian Raw XML Response: " + rawXml);
            var reportJson = CIC.Helper.XmlHelper.ExtractAndParseExperianXml(rawXml, _logger);
           // _logger.LogInfo("ExperianService GetCreditReportAsync Raw reportJson Response: " + reportJson);
            try
            {
                await Task.Run(() =>
                {
                    if (reportJson != null)
                    {
                        ExperianRepository.PrepareAndSaveExperianResponseForDb(
                            reportJson,
                            requiredcompanyid,
                            "",
                            _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "",
                            _logger
                        );
                    }
                    else
                    {
                        _logger.LogError("Experian report data is null. Cannot save to DB.");
                    }
                });
            }
            catch (Exception ex)
            {

                _logger.LogError($"Experian Data Save to DB Error. {ex.Message} ParseExperianXML response : \n{reportJson}");
            }
            
            return new CIC.Model.Experian.Response.ExperianResponse
            {
                StatusCode = 200,
                Success = true,
                Message = "Success",
                Data = reportJson
            };
        }

        public async Task<CIC.Model.Experian.Response.ExperianResponsePdf> GetCreditReportPdf(CIC.Model.Experian.ResponseNew.ExperianReturnResponseV1 response, string company_id)
        {
            ExperianResponsePdf experianResponsePdf = new ExperianResponsePdf();
            string pdfurl = string.Empty;
            try
            {
                if (response.success)
                {
                    string html = ExperianHtmlBuilder.BuildHtml(response);
                    pdfurl = await HtmlToPdfService.GeneratePdfAndGetUrlAsync(html, "Experian_Report", _logger);
                    string domain = $"{_contextAccessor.HttpContext?.Request.Scheme}://{_contextAccessor.HttpContext?.Request.Host}";
                    pdfurl = $"{domain}/{pdfurl}";
                    //_logger.LogInfo("Experian PDF URL: " + pdfurl);
                    var currentApplicant = response.INProfileResponse?.Current_Application?.Current_Application_Details?.Current_Applicant_Details;
                    var score = response.INProfileResponse?.SCORE;
                    experianResponsePdf = new ExperianResponsePdf
                    {
                        Timestamp = response.timestamp,
                        TransactionId = response.transaction_id,
                        Status = response.success,
                        message = response.message,
                        Data = new CreditScoreData
                        {
                            Pan = currentApplicant?.IncomeTaxPan,
                            Name = currentApplicant?.First_Name + " " + currentApplicant?.Last_Name,
                            Mobile = (string.IsNullOrEmpty(currentApplicant?.MobilePhoneNumber) ? currentApplicant?.Telephone_Number_Applicant_1st : currentApplicant.MobilePhoneNumber),
                            CreditScore = Convert.ToInt32(score?.BureauScore),
                            CreditReportLink = pdfurl,
                            
                        }
                    };
                   // _logger.LogInfo("Experian Response PDF Data: " + System.Text.Json.JsonSerializer.Serialize(experianResponsePdf));
                   // await ExperianRepository.SaveExperianReport(experianResponsePdf, company_id, _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError($"{exc.Message}");
                experianResponsePdf = new ExperianResponsePdf
                {
                    Timestamp = response.timestamp,
                    TransactionId = response.transaction_id,
                    Status = false,
                    Data = null,
                    message=exc.Message
                };
            }
            return experianResponsePdf;
        }

    }
}
