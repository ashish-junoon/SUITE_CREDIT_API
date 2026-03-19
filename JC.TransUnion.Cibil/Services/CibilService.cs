using CIC.Model.TransUnionCibil;
using JC.TransUnion.Cibil.Interface;
using JC.TransUnion.Cibil.Models;
using LoggerLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
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
        private readonly IOptions<CIC.DataUtility.AppSettingModel> _appsetting;
        private readonly IConfiguration _config;
        private readonly FileService _fileService;
        private static bool CIBIL_SERVICES_PROD = false;
        private readonly ConfigModel configModel;
        private readonly HttpTransUnionCall httpTransUnionCall;

        public CibilService(FileService fileService, IConfiguration config, ILoggerManager logger , IOptions<CIC.DataUtility.AppSettingModel> options)
        {
            _fileService = fileService;
            _logger = logger;
            _config = config;
            CIBIL_SERVICES_PROD = Convert.ToBoolean(_config["CIC_SERVICES:TRANSUNION_CIBIL_SERVICES_PROD"]);
            configModel = CibilConfig.GetCibilModel(CIBIL_SERVICES_PROD);
            httpTransUnionCall = new HttpTransUnionCall(configModel, _fileService, logger);
            _appsetting = options;
        }



        private async Task<CibilApiResponse> CallHybrid(FulfillOfferRQ payload , string requiredHeader, string requiredcompanyid)
        {
            string guid = Guid.NewGuid().ToString();
            string Unique = $"Junoon@{Random.Shared.Next(10000000, 99999999)}";
            PingRequestRoot rqPayload = RequestGenerator.ReturnPingRequest(configModel);
            rqPayload.PingRequest.ClientKey = Unique;
            rqPayload.PingRequest.RequestKey = Unique;
            _logger.LogInfo($"ping_rqPayload_request_{guid}-{JsonSerializer.Serialize(rqPayload, new JsonSerializerOptions
            {
                WriteIndented = true
            })}");
            var result = await httpTransUnionCall.CallCibil("/ping", rqPayload);

            _logger.LogInfo($"ping_rqPayload_response_{guid}-{JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = true
            })}");

            PingResponseRoot pingResponse = JsonSerializer.Deserialize<PingResponseRoot>(JsonSerializer.Serialize(result));
            if (pingResponse?.PingResponse?.ResponseStatus != "Success")
            {
                return new CibilApiResponse
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Data = new BaseResponse { response = pingResponse },
                    message = "Ping Failed, cannot proceed with further calls",
                    success = false,
                    transaction_id = guid
                };
            }


            //var fullfilPayload = RequestGenerator.ReturnFulfillOfferRequest(configModel, payload, Unique);
            //result = await httpTransUnionCall.CallCibil("/fulfilloffer", fullfilPayload);
            //_logger.LogInfo($"fulfilloffer_{guid}-{JsonSerializer.Serialize(result, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //})}");

            //FulFillResposeRoot fulfillOfferRSRoot = JsonSerializer.Deserialize<FulFillResposeRoot>(JsonSerializer.Serialize(result));

            //_logger.LogInfo($"fulfilloffer_fulfillOfferRSRoot_{guid}-{JsonSerializer.Serialize(fulfillOfferRSRoot, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //})}");

            FulFillResposeRoot fulfillOfferRSRoot = await FulFillOffer(configModel, payload, Unique, guid);
            if (fulfillOfferRSRoot?.FulfillOfferResponse?.ResponseStatus == "Failure")
            {
                Unique = fulfillOfferRSRoot?.FulfillOfferResponse?.FulfillOfferError?.Failure?.ClientUserKey;
                //_logger.LogInfo($" clientUserKey : {Unique} fulfilloffer_fulfillOfferRSRoot_Failure_{guid}-{JsonSerializer.Serialize(fulfillOfferRSRoot, new JsonSerializerOptions
                //{
                //    WriteIndented = true
                //})}");
                fulfillOfferRSRoot = new FulFillResposeRoot();
                fulfillOfferRSRoot = await FulFillOffer(configModel, payload, Unique, guid);
            }

            //_logger.LogInfo($"fulfilloffer_fulfillOfferRSRoot_Final_{guid}-{JsonSerializer.Serialize(fulfillOfferRSRoot, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //})}");

            if (fulfillOfferRSRoot?.FulfillOfferResponse?.ResponseStatus != "Success")
            {
                return new CibilApiResponse
                {
                    Status = (int)HttpStatusCode.OK,
                    Data = new BaseResponse
                    {
                        response = fulfillOfferRSRoot
                    },
                    success = false,
                    message = fulfillOfferRSRoot?.FulfillOfferResponse?.FulfillOfferError?.Failure?.Message,
                    transaction_id = guid
                };
            }


            AuthRequestRoot authRequest = RequestGenerator.ReturnAuthRequest(configModel, Unique);

            result = await httpTransUnionCall.CallCibil("/GetAuthenticationQuestions", authRequest);
            //_logger.LogInfo($"GetAuthenticationQuestions_{guid}-{JsonSerializer.Serialize(result, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //})}");

            AuthResponseRoot authResponseRoot = JsonSerializer.Deserialize<AuthResponseRoot>(JsonSerializer.Serialize(result));

            if (authResponseRoot?.GetAuthenticationQuestionsResponse?.ResponseStatus != "Success")
            {
                return new CibilApiResponse
                {
                    Status = (int)HttpStatusCode.OK,
                    Data = new BaseResponse
                    {
                        response = authResponseRoot
                    },
                    message = "Failure",
                    success = false,
                    transaction_id = guid
                };
            }

            GetCustomerAssetsRequestRoot assetsRequestRoot = RequestGenerator.ReturnGetCustomerAssetsRequest(configModel, Unique);

            result = await httpTransUnionCall.CallCibil("/GetCustomerAssets", assetsRequestRoot);
            //_logger.LogInfo($"GetCustomerAssets_{guid}-{JsonSerializer.Serialize(result, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //})}");

            _logger.LogInfo("PushToDatabase - Data prepared and sent for saving into database.");
            
            //Task.Run(() => SaveToDB.PushToDatabase(result, guid , requiredHeader , requiredcompanyid , _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger));
            
            ProductWebTokenRequestRoot productWeb = RequestGenerator.ReturnGetProductWebTokenRequest(configModel, Unique);

            var TokenResult = await httpTransUnionCall.CallCibil("/GetProductWebToken", productWeb);

            WebTokenRS responce = JsonSerializer.Deserialize<WebTokenRS>(JsonSerializer.Serialize(TokenResult));
            string htmlUrl = $"{configModel.WEBTOKEN_BASE_URL}?enterprise={configModel.SITE_NAME}&pcc={Unique}&webtoken={responce.GetProductWebTokenResponse.GetProductWebTokenSuccess.WebToken}";

            CibilApiResponse apiResponse = new CibilApiResponse
            {
                Status = (int)HttpStatusCode.OK,
                Data = new BaseResponse
                {
                    response = result,
                    cibilURL = htmlUrl
                },
                message = "success",
                success = true,
                transaction_id = guid
            };

            _logger.LogInfo($"FinalResponse_{guid}-{JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
            {
                WriteIndented = true
            })}");

            return apiResponse;
        }




        public async Task<CibilApiResponse> GetCusomerCibil(FulfillOfferRQ req , string requiredHeader, string requiredcompanyid)
        {
            return await CallHybrid(req , requiredHeader , requiredcompanyid);
        }


        private async Task<FulFillResposeRoot> FulFillOffer(ConfigModel configModel, FulfillOfferRQ payload, string Unique, string guid)
        {
            var fullfilPayload = RequestGenerator.ReturnFulfillOfferRequest(configModel, payload, Unique);
           var result = await httpTransUnionCall.CallCibil("/fulfilloffer", fullfilPayload);
            _logger.LogInfo($"fulfilloffer_{guid}-{JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = true
            })}");

            FulFillResposeRoot fulfillOfferRSRoot = JsonSerializer.Deserialize<FulFillResposeRoot>(JsonSerializer.Serialize(result));

            _logger.LogInfo($"fulfilloffer_fulfillOfferRSRoot_{guid}-{JsonSerializer.Serialize(fulfillOfferRSRoot, new JsonSerializerOptions
            {
                WriteIndented = true
            })}");

            return fulfillOfferRSRoot;
        }

    }

}
