using JC.TransUnion.Cibil.Interface;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace JC.TransUnion.Cibil.Services
{
    public class CibilTokenService : ICibilTokenService
    {
        private readonly IConfiguration _config;
        private static bool CIBIL_SERVICES_PROD = false;
        private static string _token;
        private static DateTime _expiry;
        private static string PfxPath;
        private static string PfxPassphrase;
        private static string ApiKey;
        private static string ClientSecret;
        private static string TokenUrl;
        public CibilTokenService(IConfiguration config)
        {
            _config = config;
            CIBIL_SERVICES_PROD = Convert.ToBoolean(_config["CIC_SERVICES:TRANSUNION_CIBIL_SERVICES_PROD"]);

        }
        public async Task<string> GetTokenAsync(string BasePath)
        {
            PfxPath = CIBIL_SERVICES_PROD ? CibilConfig.ProdVariables.MEMBER_PFX_CERT_PATH : CibilConfig.UATVariables.MEMBER_PFX_CERT_PATH;
            PfxPassphrase = CIBIL_SERVICES_PROD ? CibilConfig.ProdVariables.MEMBER_PFX_PASSWORD : CibilConfig.UATVariables.MEMBER_PFX_PASSWORD;
            ApiKey = CIBIL_SERVICES_PROD ? CibilConfig.ProdVariables.API_KEY : CibilConfig.UATVariables.API_KEY;
            ClientSecret = CIBIL_SERVICES_PROD ? CibilConfig.ProdVariables.CLIENT_SECRET : CibilConfig.UATVariables.CLIENT_SECRET;
            TokenUrl = CIBIL_SERVICES_PROD ? CibilConfig.ProdVariables.HYBRID_TOKEN_URL : CibilConfig.UATVariables.HYBRID_TOKEN_URL;

            if (_token != null && DateTime.UtcNow < _expiry)
                return _token;

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(
                new X509Certificate2(BasePath+PfxPath, PfxPassphrase));

            using var client = new HttpClient(handler);

            var auth = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{ApiKey}:{ClientSecret}"));

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", auth);

            var resp = await client.PostAsync(
                $"{TokenUrl}?grant_type=client_credentials",
                new StringContent(""));

            var json = await resp.Content.ReadFromJsonAsync<JsonElement>();

            _token = json.GetProperty("access_token").GetString();
            _expiry = DateTime.UtcNow.AddSeconds(
                json.GetProperty("expires_in").GetInt32() - 60);

            return _token;
        }
    }
}
