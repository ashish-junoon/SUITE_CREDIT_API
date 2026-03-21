using CIC.Helper;
using CIC.Model.Experian.Request;
using JC.Experian.ExperianModel;
using JC.Experian.Interfaces;
using LoggerLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Text;

namespace JC.Experian
{
    public class ExperianSoapClient : IExperianSoapClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private static bool EXPERIAN_SERVICES_PROD = false;
        private readonly ILoggerManager _logger;
        public ExperianSoapClient(HttpClient httpClient, ILoggerManager logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
            EXPERIAN_SERVICES_PROD = Convert.ToBoolean(_config["CIC_SERVICES:EXPERIAN_SERVICES_PROD"]);
        }

        public async Task<string> FetchCreditReportAsync(ExperianRequest payload)
        {
            string strResponse = string.Empty;
            string EXPERIAN_SOAP_URL = string.Empty;
            if (EXPERIAN_SERVICES_PROD)
            {
                EXPERIAN_SOAP_URL = ExperianConfig.ProdVariables.SOAP_URL;
            }
            else
            {
                EXPERIAN_SOAP_URL = ExperianConfig.UATVariables.SOAP_URL;
            }
            var (firstName, lastName) = NameHelper.SplitName(payload.Name);

            var experianPayload = new ExperianApiRequest
            {
                Pan = payload.Pan,
                Mobile = payload.Mobile,
                Consent = true
            };


            try
            {
                var xmlBody = SoapTemplateBuilder.Build(experianPayload, firstName, lastName, EXPERIAN_SERVICES_PROD);
               // _logger.LogInfo("ExperianSoapClient Experian SOAP Request XML: " + xmlBody);
                var request = new HttpRequestMessage(HttpMethod.Post, EXPERIAN_SOAP_URL);
                request.Content = new StringContent(xmlBody, Encoding.UTF8, "text/xml");
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                strResponse = await response.Content.ReadAsStringAsync();
                _logger.LogInfo("ExperianSoapClient Experian SOAP Response: \n" + strResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("Experian SOAP Response Exception: " + ex.Message);
            }

            return strResponse;
        }
    }
}
