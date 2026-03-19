using JC.TransUnion.Cibil.Models;
using LoggerLibrary;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace JC.TransUnion.Cibil
{
    public class HttpTransUnionCall
    {
        private readonly FileService _fileService;
        private readonly HttpClient _httpClient;
        private readonly ConfigModel config;
        private readonly string RootPath = string.Empty;
        private readonly ILoggerManager _logger;
        public HttpTransUnionCall(ConfigModel _config, FileService fileService, ILoggerManager logger)
        {
            _logger = logger;
            _fileService = fileService;
            config = _config;
            RootPath = _fileService.RootPath();
            var handler = new HttpClientHandler();
            string PFX_CERT_PATH = RootPath + config.MEMBER_PFX_CERT_PATH;
            var cert = new X509Certificate2(
                PFX_CERT_PATH,
                config.MEMBER_PFX_PASSWORD,
                X509KeyStorageFlags.MachineKeySet);

            handler.ClientCertificates.Add(cert);

            // UAT only
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
        }

        public async Task<object> CallCibil(string endpoint, object payload)
        {
           // _logger.LogInfo($"Initiating CallCibil for endpoint: {endpoint} with payload: {JsonSerializer.Serialize(payload)} PUBLIC_CERT_PATH: {RootPath + config.PUBLIC_CERT_PATH}");
            var encryptedPayload = Crypto.HybridEncryptor.Encrypt_V1(payload, RootPath + config.PUBLIC_CERT_PATH);
           // _logger.LogInfo($"Encrypted Payload: {JsonSerializer.Serialize(encryptedPayload)}"); // Debug log
           // _logger.LogInfo($"HYBRID_BASE_URL: {config.HYBRID_BASE_URL + endpoint}"); // Debug log
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                config.HYBRID_BASE_URL + endpoint);

            request.Content = new StringContent(
                JsonSerializer.Serialize(encryptedPayload),
                Encoding.UTF8,
                "application/json");

            //_logger.LogInfo($"member-ref-id: {config.MEMBER_REF_ID} client-secret: {config.CLIENT_SECRET} apikey: {config.API_KEY}"); // Debug log

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Headers.Add("member-ref-id", config.MEMBER_REF_ID);
            request.Headers.Add("client-secret", config.CLIENT_SECRET);
            request.Headers.Add("apikey", config.API_KEY);
            try
            {
                var response = await _httpClient.SendAsync(request);

                var content = await response.Content.ReadAsStringAsync();
               // _logger.LogInfo($"Response Status: {response.StatusCode}, Response Content: {content}"); // Debug log
                if (!response.IsSuccessStatusCode)
                {
                    return new
                    {
                        Status = response.StatusCode,
                        Raw = content
                    };
                }
                var encryptedResponse = JsonSerializer.Deserialize<JsonElement>(content);

                return Crypto.HybridDecryptor.Decrypt_V1(
                    encryptedResponse.GetProperty("EncryptedKey").GetString(),
                    encryptedResponse.GetProperty("EncryptedData").GetString(), RootPath + config.PRIVATE_KEY_PATH, config.MEMBER_PFX_PASSWORD);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CallCibil: {ex.Message} ex: {ex}");
                var errorDetails = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return errorDetails;
            }
        }
    }
}
