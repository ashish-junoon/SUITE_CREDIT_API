using CIC.Model.TransUnionCibil;
using JC.TransUnion.Cibil.Interface;
using JC.TransUnion.Cibil.Models;
using LoggerLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Text.Json;
using static JC.TransUnion.Cibil.Models.Requests;
using static JC.TransUnion.Cibil.Models.Responses;

namespace JC.TransUnion.Cibil.Services
{
    public class CibilService : ICibilService
    {
        private readonly ILoggerManager _logger;
        private readonly IConfiguration _config;
        private readonly FileService _fileService;
        private readonly ConfigModel _configModel;
        private readonly HttpTransUnionCall _httpClient;
        private readonly string _connectionString;

        public CibilService(
            FileService fileService,
            IConfiguration config,
            ILoggerManager logger,
            IOptions<CIC.DataUtility.AppSettingModel> options)
        {
            _fileService = fileService;
            _logger = logger;
            _config = config;

            bool isProd = Convert.ToBoolean(_config["CIC_SERVICES:TRANSUNION_CIBIL_SERVICES_PROD"]);
            _configModel = CibilConfig.GetCibilModel(isProd);

            _httpClient = new HttpTransUnionCall(_configModel, _fileService, _logger);
            _connectionString = options?.Value?.ConnectionStrings?.dbconnection ?? "";
        }

        #region PUBLIC METHODS

        public async Task<CibilApiResponse> GetCusomerCibil(
            FulfillOfferRQ request,
            string requiredHeader,
            string requiredCompanyId)
        {
            return await ExecuteFlow(request, requiredHeader, requiredCompanyId);
        }

        public async Task<bool> CallPingAsync(CancellationToken token = default)
        {
            try
            {
                var unique = GenerateUnique();
                var response = await PingAsync(unique);

                bool success = response?.PingResponse?.ResponseStatus == "Success";

                if (success)
                    _logger.LogInfo("Ping Success");
                else
                    _logger.LogError("Ping Failed");

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ping Exception: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region CORE FLOW

        private async Task<CibilApiResponse> ExecuteFlow(
            FulfillOfferRQ payload,
            string requiredHeader,
            string requiredCompanyId)
        {
            string transactionId = Guid.NewGuid().ToString();
            string unique = GenerateUnique();

            // Step 1: Ping
            var ping = await PingAsync(unique);
            if (ping?.PingResponse?.ResponseStatus != "Success")
                return FailResponse("Ping Failed", transactionId, ping, HttpStatusCode.Unauthorized);
            _logger.LogInfo($"Ping Success_{unique}");
            // Step 2: Fulfill Offer
            var fulfill = await RetryAsync(() => FulfillOfferAsync(payload, unique));
            //_logger.LogInfo($"FulfillOffer_{unique}-{JsonSerializer.Serialize(fulfill, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //})}");
            if (fulfill?.FulfillOfferResponse?.ResponseStatus == "Failure")
            {
                unique = fulfill?.FulfillOfferResponse?.FulfillOfferError?.Failure?.ClientUserKey;
                //fulfillOfferRSRoot = new FulFillResposeRoot();
                _logger.LogInfo($"FulfillOffer Retry_{unique}");
                fulfill = await RetryAsync(() => FulfillOfferAsync(payload, unique));
                _logger.LogInfo($"FulfillOffer retyr 1_{unique}-{JsonSerializer.Serialize(fulfill, new JsonSerializerOptions
                {
                    WriteIndented = true
                })}");
            }
           

            if (fulfill?.FulfillOfferResponse?.ResponseStatus != "Success")
                return FailResponse(
                    fulfill?.FulfillOfferResponse?.FulfillOfferError?.Failure?.Message,
                    transactionId,
                    fulfill);

            // Step 3: Auth Questions
            var auth = await CallApi<AuthResponseRoot>("/GetAuthenticationQuestions",
                RequestGenerator.ReturnAuthRequest(_configModel, unique));
            //_logger.LogInfo($"GetAuthenticationQuestions_{unique}-{JsonSerializer.Serialize(auth, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //})}");
            if (auth?.GetAuthenticationQuestionsResponse?.ResponseStatus != "Success")
                return FailResponse("Auth Failed", transactionId, auth);

            // Step 4: Customer Assets
            var assets = await CallApi<object>("/GetCustomerAssets",
                RequestGenerator.ReturnGetCustomerAssetsRequest(_configModel, unique));

            //_logger.LogInfo($"GetCustomerAssets_{unique}-{JsonSerializer.Serialize(assets, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //})}");

            // Fire & forget DB save (safe)
            _ = Task.Run(() =>
                SaveToDB.PushToDatabase(
                    assets,
                    transactionId,
                    requiredHeader,
                    requiredCompanyId,
                    _connectionString,
                    _logger));

            // Step 5: Web Token
            var token = await CallApi<WebTokenRS>("/GetProductWebToken",
                RequestGenerator.ReturnGetProductWebTokenRequest(_configModel, unique));

            string htmlUrl = BuildHtmlUrl(unique, token);

            GetCustomerAssetsModel custAssets = new();
            try
            {
                custAssets = Newtonsoft.Json.JsonConvert.DeserializeObject<GetCustomerAssetsModel>(assets.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Deserialization Error: {ex.Message}");
            }
            return new CibilApiResponse
            {
                Status = (int)HttpStatusCode.OK,
                success = true,
                message = "Success",
                transaction_id = transactionId,
                Data = new BaseResponse
                {
                    response = custAssets,
                    cibilURL = htmlUrl
                }
            };
        }

        #endregion

        #region API HELPERS

        private async Task<T?> CallApi<T>(string endpoint, object request)
        {
            var result = await _httpClient.CallCibil(endpoint, request);

            return JsonSerializer.Deserialize<T>(
                JsonSerializer.Serialize(result));
        }

        private async Task<PingResponseRoot?> PingAsync(string unique)
        {
            var req = RequestGenerator.ReturnPingRequest(_configModel);
            req.PingRequest.ClientKey = unique;
            req.PingRequest.RequestKey = unique;

            return await CallApi<PingResponseRoot>("/ping", req);
        }

        private async Task<FulFillResposeRoot?> FulfillOfferAsync(
            FulfillOfferRQ payload,
            string unique)
        {
            var req = RequestGenerator.ReturnFulfillOfferRequest(_configModel, payload, unique);
            return await CallApi<FulFillResposeRoot>("/fulfilloffer", req);
        }

        #endregion

        #region UTILITIES

        private async Task<T?> RetryAsync<T>(Func<Task<T?>> action, int retries = 2)
        {
            int delay = 1000;

            for (int i = 0; i <= retries; i++)
            {
                var result = await action();

                if (result != null)
                    return result;

                await Task.Delay(delay);
                delay *= 2;
            }

            return default;
        }

        private static string GenerateUnique()
            => $"Junoon@{Random.Shared.Next(10000000, 99999999)}";

        private string BuildHtmlUrl(string unique, WebTokenRS token)
        {
            var webToken = token?.GetProductWebTokenResponse?
                .GetProductWebTokenSuccess?.WebToken;

            return $"{_configModel.WEBTOKEN_BASE_URL}?enterprise={_configModel.SITE_NAME}&pcc={unique}&webtoken={webToken}";
        }

        private CibilApiResponse FailResponse(
            string message,
            string transactionId,
            object response,
            HttpStatusCode status = HttpStatusCode.OK)
        {
            return new CibilApiResponse
            {
                Status = (int)status,
                success = false,
                message = message,
                transaction_id = transactionId,
                Data = new BaseResponse { response = new GetCustomerAssetsModel() }
            };
        }

        #endregion
    }
}