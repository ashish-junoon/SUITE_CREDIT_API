using CIC.DataUtility.Repository;
using CIC.Model.Criff.Response;
using CIC.Model.Experian.Response;
using CIC.Model.TransUnionCibil;
using CIC_Services.Interfaces;
using LoggerLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CIC_Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditScoreController : ControllerBase
    {
        private readonly ICrifService _crifService;
        private readonly IExperianService _experianService;
        private readonly ITransunionCibilService _transunionCibilService;
        private readonly IOptions<CIC.DataUtility.AppSettingModel> _appsetting;
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        private static bool CIBIL_SERVICES_PROD, CRIF_FUSION_PROD, CRIF_HIGHMARK_SERVICES_PROD = false;
        public CreditScoreController(ICrifService crifService, IExperianService experianService, ITransunionCibilService transunionCibilService, IConfiguration config, ILoggerManager logger, IOptions<CIC.DataUtility.AppSettingModel> options)
        {
            _crifService = crifService;
            _transunionCibilService = transunionCibilService;
            _experianService = experianService;
            _config = config;
            _appsetting = options;
            CIBIL_SERVICES_PROD = Convert.ToBoolean(_config["CIC_SERVICES:TRANSUNION_CIBIL_SERVICES_PROD"]);
            CRIF_HIGHMARK_SERVICES_PROD = Convert.ToBoolean(_config["CIC_SERVICES:CRIF_HIGHMARK_SERVICES_PROD"]);
            CRIF_FUSION_PROD = Convert.ToBoolean(_config["CIC_SERVICES:CRIF_FUSION_PROD"]);
            _logger = logger;
        }

        #region Experian Credit report Start from here

        [HttpPost("credit-report-experian")]
        public async Task<IActionResult> GetCreditReport([FromBody] CIC.Model.Experian.Request.ExperianRequest request, [FromHeader(Name = "token")] string requiredHeader, [FromHeader(Name = "companyid")] string requiredcompanyid)
        {
            var accept = Request.Headers["Accept"].ToString();
            var result = await _experianService.GetCreditReportAsync(request, requiredcompanyid);
            //_logger.LogInfo($"Experian Credit Report Response: {JsonConvert.SerializeObject(result)}");
            if (result.Success == false)
            {
                var badrequest = new ExperianResponse
                {
                    Success = false,
                    Message = Convert.ToString(result.Data),
                    Error = "Error fetching credit report",
                    MessageCode = "ERR_CREDIT_REPORT",
                    StatusCode = 400,
                    Timestamp = DateTime.UtcNow,
                    Transaction_id = Guid.NewGuid().ToString()
                };
                if (accept.Contains("application/xml", StringComparison.OrdinalIgnoreCase))
                {
                    var xmlrequest = ResultParser.Experian.ResultParser.ParsetoXml(badrequest, _logger);
                    return Content(xmlrequest.ToString(), "application/xml");
                }
                return BadRequest(badrequest);
            }
            var xml = ResultParser.Experian.ResultParser.ParsetoXml(result, _logger);
            //_logger.LogInfo($"Experian Credit Report Response XML : {xml.ToString()}");
            if (accept.Contains("application/xml", StringComparison.OrdinalIgnoreCase))
            {
                return Content(xml.ToString(), "application/xml");
            }
            try
            {
                var jsonObj = ResultParser.Experian.ResultParser.ConvertXmlToJson<CIC.Model.Experian.ResponseNew.ExperianReturnResponseV1>(xml.ToString());
                return Ok(jsonObj);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while converting XML to JSON: {ex.Message}. Raw Response: {result}");
                var badrequest = new ExperianResponse
                {
                    Success = false,
                    Message = Convert.ToString(result.Data),
                    Error = "Error while converting XMl to JSON and Save To db",
                    MessageCode = "ERR_CREDIT_REPORT",
                    StatusCode = 400,
                    Timestamp = DateTime.UtcNow,
                    Transaction_id = Guid.NewGuid().ToString()
                };

                return BadRequest(badrequest);
            }
        }

        [HttpPost("credit-report-experian-pdf")]
        public async Task<IActionResult> GetCreditReport_pdf([FromBody] CIC.Model.Experian.Request.ExperianRequest request, [FromHeader(Name = "token")] string requiredHeader, [FromHeader(Name = "companyid")] string requiredcompanyid)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogError("Request object is null.");
                    return BadRequest("Request cannot be null.");
                }
                var result = await _experianService.GetCreditReportAsync(request, requiredcompanyid);
                if (result == null)
                {
                    _logger.LogError("Experian service returned null result.");
                    return NotFound("Credit report not found.");
                }
                var xml = ResultParser.Experian.ResultParser.ParsetoXml(result, _logger);
                if (xml == null)
                {
                    _logger.LogError("Failed to parse result to XML.");
                    return StatusCode(500, "Error parsing XML from Experian response.");
                }
                var jsonObj = ResultParser.Experian.ResultParser.ConvertXmlToJson<CIC.Model.Experian.ResponseNew.ExperianReturnResponseV1>(xml.ToString());

                if (jsonObj == null)
                {
                    _logger.LogError("Failed to convert XML to JSON.");
                    return StatusCode(500, "Error converting Experian XML to JSON.");
                }
               // _logger.LogInfo($"Experian Credit Report PDF Response JSON : {jsonObj}");
                var experianResponse = await _experianService.GetCreditReportPdf(jsonObj, requiredcompanyid);
                if (experianResponse == null)
                {
                    _logger.LogError("Failed to generate Experian PDF response.");
                    return StatusCode(500, "Failed to generate credit report PDF.");
                }
                //       else
                //       {
                //           var addr = jsonObj?.INProfileResponse?.Current_Application?.Current_Application_Details?.Current_Applicant_Address_Details;
                //           var merged = string.Join(", ", new[] {
                //                   addr.FlatNoPlotNoHouseNo,
                //                   addr.BldgNoSocietyName,
                //                   addr.RoadNoNameAreaLocality,
                //                   addr.City,
                //                   addr.Landmark,
                //                   addr.State,
                //                   addr.PINCode,
                //                   addr.Country_Code
                //               }.Where(v => !string.IsNullOrWhiteSpace(v)
                //&& v != "DEFAULT"
                //&& v != "NA"
                //&& v != "NULL" && v != ""));

                //           experianResponse.Data.address= jsonObj.INProfileResponse.Current_Application.Current_Application_Details.Current_Applicant_Additional_AddressDetails.
                //       }
                _logger.LogInfo("Experian PDF response generated successfully.");
                return Ok(experianResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in GetCreditReport_pdf: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion Experian Credit report End here

        #region Ciff Credit Report Start from here

        [HttpPost("credit-report-crif")]
        public async Task<IActionResult> GetCreditReportCriff([FromBody] CIC.Model.Criff.Request.SoftPullRQ request, [FromHeader(Name = "token")] string requiredHeader, [FromHeader(Name = "companyid")] string requiredcompanyid)
        {
            CrifResponseReturn? crifResponse = new CrifResponseReturn();

            if (request.uid_number.Length == 10 || request.uid_number.Length == 12)
            {
                try
                {
                    crifResponse = await _crifService.GetCreditReportAsync(request, CRIF_HIGHMARK_SERVICES_PROD);

                    if (crifResponse == null || crifResponse.Data?.B2CReport?.Header == null)
                    {
                        crifResponse = new CrifResponseReturn
                        {
                            Timestamp = crifResponse?.Timestamp,
                            TransactionId = crifResponse.TransactionId,
                            StatusCode = crifResponse?.StatusCode,
                            Status = crifResponse.Status,
                            Data = crifResponse?.Data,
                            message = crifResponse.message ?? "Invalid response from CRIF service"
                        };
                    }
                    _logger.LogInfo("Crif Response PDF Data: " + System.Text.Json.JsonSerializer.Serialize(crifResponse));
                    //await Task.Run(() => ExperianRepository.SaveCrifReport(crifResponsePdf, requiredcompanyid, _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger));
                    await Task.Run(() =>
                    {
                        ExperianRepository.PrepareAndSaveCrifResponseForDb(crifResponse, request, requiredcompanyid, "", _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger);
                    });
                }
                catch (Exception exc)
                {
                    _logger.LogError($"CRIF JSON Parse Failed : {exc.Message}. Raw Response: {crifResponse}");
                }
            }
            else
            {
                crifResponse = new CrifResponseReturn
                {
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    TransactionId = Guid.NewGuid().ToString(),
                    StatusCode = 400,
                    Status = false,
                    Data = null,
                    message = "Invalid UID number. It should be either 10 or 12 characters long."
                };
                return BadRequest(crifResponse);
            }
            return Ok(crifResponse);
        }

        [HttpPost("credit-report-crif-v1")]
        public async Task<IActionResult> GetCreditReportCriffV1([FromBody] CIC.Model.Criff.Request.SoftPullRQV1 request, [FromHeader(Name = "token")] string requiredHeader, [FromHeader(Name = "companyid")] string requiredcompanyid)
        {
            if (ModelState.IsValid == false)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var errorMessage = string.Join("; ", errors);
                var badRequestResponse = new CrifResponseReturn
                {
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    TransactionId = Guid.NewGuid().ToString(),
                    StatusCode = 400,
                    Status = false,
                    Data = null,
                    message = $"Invalid request: {errorMessage}"
                };
                return BadRequest(badRequestResponse);
            }

            CrifResponseReturn? crifResponse = new CrifResponseReturn();

                try
                {
                    crifResponse = await _crifService.GetCreditReportV1Async(request, CRIF_HIGHMARK_SERVICES_PROD);

                    if (crifResponse == null || crifResponse.Data?.B2CReport?.Header == null)
                    {
                        crifResponse = new CrifResponseReturn
                        {
                            Timestamp = crifResponse?.Timestamp,
                            TransactionId = crifResponse?.TransactionId,
                            StatusCode = crifResponse?.StatusCode,
                            Status = crifResponse.Status,
                            Data = crifResponse?.Data,
                            message = crifResponse.message ?? "Invalid response from CRIF service"
                        };
                    }
                    _logger.LogInfo("Crif Response PDF Data: " + System.Text.Json.JsonSerializer.Serialize(crifResponse));
                    //await Task.Run(() => ExperianRepository.SaveCrifReport(crifResponsePdf, requiredcompanyid, _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger));
                    await Task.Run(() =>
                    {
                        ExperianRepository.PrepareAndSaveCrifResponseForDbV1(crifResponse, request, requiredcompanyid, "", _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger);
                    });
                }
                catch (Exception exc)
                {
                    _logger.LogError($"CRIF JSON Parse Failed : {exc.Message}. Raw Response: {crifResponse}");
                }
            
            return Ok(crifResponse);
        }


        [HttpPost("auth-questionnaire-crif")]
        public async Task<IActionResult> AuthQuestionnaireCriff([FromBody] CIC.Model.Criff.Request.AuthRQ request, [FromHeader(Name = "token")] string requiredHeader, [FromHeader(Name = "companyid")] string requiredcompanyid)
        {
            CrifResponseReturn? crifResponse = new CrifResponseReturn();
            try
            {
                crifResponse = await _crifService.AuthQuestionnaireCriff(request, CRIF_HIGHMARK_SERVICES_PROD);
                var status = crifResponse?.Data?.status;
                if (string.IsNullOrEmpty(status) || (status != "S11" && status != "S02"))
                {
                    await Task.Run(() =>
                    {
                        ExperianRepository.AuthQuestionnaireCrifResponseForDb(
                            crifResponse,
                            requiredcompanyid,
                            "",
                            _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "",
                            _logger
                        );
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"CRIF JSON Parse Failed : {ex.Message}. Raw Response: {crifResponse}");
                //if (crifResponse != null)
                //{
                //    ExperianRepository.AuthQuestionnaireCrifResponseForDb(
                //        crifResponse,
                //        requiredcompanyid,
                //        "",
                //        _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "",
                //        _logger
                //    );
                //}
            }
            return Ok(crifResponse);
        }


        [HttpPost("credit-report-crif-pdf")]
        public async Task<IActionResult> GetCreditReportCriffPdf([FromBody] CIC.Model.Criff.Request.SoftPullRQ request, [FromHeader(Name = "token")] string requiredHeader, [FromHeader(Name = "companyid")] string requiredcompanyid)
        {
            CrifResponseReturn? crifResponseReturn = null;
            if (request.uid_number.Length == 10 || request.uid_number.Length == 12)
            {
                crifResponseReturn = await _crifService.GetCreditReportAsync(request, CRIF_HIGHMARK_SERVICES_PROD);

                CrifResponsePdf crifResponsePdf = new CrifResponsePdf();
                if (!string.IsNullOrWhiteSpace(crifResponseReturn?.ToString()))
                {
                    try
                    {
                        crifResponsePdf = await _crifService.GetCreditReportPdfAsync(crifResponseReturn, request, requiredcompanyid);

                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError($"CRIF JSON Parse Failed : {ex.Message}. Raw Response: {crifResponseReturn}");
                    }
                }
                await Task.Run(() =>
                {
                    ExperianRepository.PrepareAndSaveCrifResponseForDb(crifResponseReturn, request, requiredcompanyid, "", _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger);
                });
                return Ok(crifResponsePdf);
            }
            else
            {
                crifResponseReturn = new CrifResponseReturn
                {
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    TransactionId = Guid.NewGuid().ToString(),
                    StatusCode = 400,
                    Status = false,
                    Data = null,
                    message = "Invalid UID number. It should be either 10 or 12 characters long."
                };
                return BadRequest(crifResponseReturn);
            }
        }

        [HttpPost("auth-questionnaire-crif-pdf")]
        public async Task<IActionResult> AuthQuestionnaireCrifPdf([FromBody] CIC.Model.Criff.Request.AuthRQ request, [FromHeader(Name = "token")] string requiredHeader, [FromHeader(Name = "companyid")] string requiredcompanyid)
        {
            CrifResponseReturn? crifResponse = new CrifResponseReturn();
            crifResponse = await _crifService.AuthQuestionnaireCriff(request, CRIF_HIGHMARK_SERVICES_PROD);
            CrifResponsePdf crifResponsePdf = new CrifResponsePdf();
            if (!string.IsNullOrWhiteSpace(crifResponse?.ToString()))
            {
                try
                {
                    crifResponsePdf = await _crifService.AuthQuestionnaireCrifPdf(crifResponse, request, requiredcompanyid);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"CRIF JSON Parse Failed : {ex.Message}. Raw Response: {crifResponse}");
                }
            }
            try
            {
                var status = crifResponse?.Data?.status;
                if (string.IsNullOrEmpty(status) || (status != "S11" && status != "S02"))
                {
                    await Task.Run(() =>
                    {
                        ExperianRepository.AuthQuestionnaireCrifResponseForDb(
                            crifResponse,
                            requiredcompanyid,
                            "",
                            _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "",
                            _logger
                        );
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"CRIF JSON Parse Failed : {ex.Message}. Raw Response: {crifResponse}");
                if (crifResponse != null)
                {
                    ExperianRepository.AuthQuestionnaireCrifResponseForDb(
                        crifResponse,
                        requiredcompanyid,
                        "",
                        _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "",
                        _logger
                    );
                }
            }
            return Ok(crifResponsePdf);
        }

        [HttpPost("credit-report-crif-pdf-v1")]
        public async Task<IActionResult> GetCreditReportCriffPdfV1([FromBody] CIC.Model.Criff.Request.SoftPullRQV1 request, [FromHeader(Name = "token")] string requiredHeader, [FromHeader(Name = "companyid")] string requiredcompanyid)
        {
            if (ModelState.IsValid == false)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var errorMessage = string.Join("; ", errors);
                var badRequestResponse = new CrifResponseReturn
                {
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    TransactionId = Guid.NewGuid().ToString(),
                    StatusCode = 400,
                    Status = false,
                    Data = null,
                    message = $"Invalid request: {errorMessage}"
                };
                return BadRequest(badRequestResponse);
            }
            CrifResponseReturn? crifResponseReturn = null;

            crifResponseReturn = await _crifService.GetCreditReportV1Async(request, CRIF_HIGHMARK_SERVICES_PROD);

            CrifResponsePdf crifResponsePdf = new CrifResponsePdf();
            if (!string.IsNullOrWhiteSpace(crifResponseReturn?.ToString()))
            {
                try
                {
                    crifResponsePdf = await _crifService.GetCreditReportPdfAsync(crifResponseReturn, request, requiredcompanyid);

                }
                catch (JsonException ex)
                {
                    _logger.LogError($"CRIF JSON Parse Failed : {ex.Message}. Raw Response: {crifResponseReturn}");
                }
            }
            await Task.Run(() =>
            {
                ExperianRepository.PrepareAndSaveCrifResponseForDbV1(crifResponseReturn, request, requiredcompanyid, "", _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger);
            });
            return Ok(crifResponsePdf);

        }

        [HttpPost("customer-fusion")]
        public async Task<IActionResult> CriffPrefil([FromBody] CIC.Model.Criff.Request.CrifPrefillRQ request, [FromHeader(Name = "token")] string requiredHeader, [FromHeader(Name = "companyid")] string requiredcompanyid)
        {
            if (ModelState.IsValid == false)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var errorMessage = string.Join("; ", errors);
                var badRequestResponse = new CrifResponseReturn
                {
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    TransactionId = Guid.NewGuid().ToString(),
                    StatusCode = 400,
                    Status = false,
                    Data = null,
                    message = $"Invalid request: {errorMessage}"
                };
                return BadRequest(badRequestResponse);
            }
            //CrifResponseReturn? crifResponseReturn = null;

           var crifResponseReturn = await _crifService.CriffPrefil(request, CRIF_FUSION_PROD, requiredcompanyid);

          
            await Task.Run(() =>
            {
                //ExperianRepository.PrepareAndSaveCrifResponseForDbV1(crifResponseReturn, request, requiredcompanyid, "", _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger);
            });
            return Ok(crifResponseReturn);

        }


        #endregion

        #region Transuniun Cibil


        private IActionResult Forward(TransuniunReturnResponse result) => StatusCode(result.Status, result);


        /// <summary>
        /// Prepares and partitions CRIF response data into a CrifResponsePdf object for database insertion.
        /// </summary>

        [HttpPost("credit-report-cibil")]
        public async Task<IActionResult> Fulfill([FromBody] FulfillOfferRQ body, [FromHeader(Name = "token")] string requiredHeader, [FromHeader(Name = "companyid")] string requiredcompanyid) => Forward(await _transunionCibilService.GetCusomerCibilAsync(body, requiredHeader, requiredcompanyid));
        #endregion
    }
}
