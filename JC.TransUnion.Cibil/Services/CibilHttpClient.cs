using JC.TransUnion.Cibil.Interface;
using JC.TransUnion.Cibil.Models;
using LoggerLibrary;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;

namespace JC.TransUnion.Cibil.Services
{
    public class CibilHttpClient: ICibilHttpClient
    {
        private readonly ILoggerManager _logger;

        private readonly ICibilTokenService _tokenService;

        public CibilHttpClient(ICibilTokenService tokenService, ILoggerManager logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<CibilApiResponse> PostAsync(
            string url,
            object body,
            Dictionary<string, string> headers,
            bool IsProduction, string basePath)
        {

            string PfxPath = IsProduction ? CibilConfig.ProdVariables.MEMBER_PFX_CERT_PATH : CibilConfig.UATVariables.MEMBER_PFX_CERT_PATH;
            string PfxPassphrase = IsProduction ? CibilConfig.ProdVariables.MEMBER_PFX_PASSWORD : CibilConfig.UATVariables.MEMBER_PFX_PASSWORD;
            string MemberRefId = IsProduction ? CibilConfig.ProdVariables.MEMBER_REF_ID : CibilConfig.UATVariables.MEMBER_REF_ID;
            string ApiKey = IsProduction ? CibilConfig.ProdVariables.API_KEY : CibilConfig.UATVariables.API_KEY;
            string BasicToken = IsProduction ? CibilConfig.ProdVariables.CLIENT_SECRET : CibilConfig.UATVariables.CLIENT_SECRET;
            string ClientSecret = IsProduction ? CibilConfig.ProdVariables.CLIENT_SECRET : CibilConfig.UATVariables.CLIENT_SECRET;

            _logger.LogInfo($"X509Certificate2 basepath {basePath + PfxPath}");

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(
                new X509Certificate2(basePath + PfxPath, PfxPassphrase));

            using var client = new HttpClient(handler);

           // var token = await _tokenService.GetTokenAsync(basePath);

            client.DefaultRequestHeaders.Add("apikey", ApiKey);
            client.DefaultRequestHeaders.Add("client-secret", ClientSecret);
            //client.DefaultRequestHeaders.Authorization =
            //    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            foreach (var h in headers)
                client.DefaultRequestHeaders.Add(h.Key, h.Value);

            var resp = await client.PostAsJsonAsync(url, body);

            return new CibilApiResponse
            {
                Status = (int)resp.StatusCode,
                Data = await resp.Content.ReadFromJsonAsync<object>()
            };
        }
    }
}
