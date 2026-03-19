using CIC.Model.Experian.Response;
using CIC.Model.TransUnionCibil;
using CIC_Services.Interfaces;
using iText.Kernel.XMP.Options;
using JC.TransUnion.Cibil;
using JC.TransUnion.Cibil.Interface;
using JC.TransUnion.Cibil.Models;
using LoggerLibrary;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace CIC_Services.Services
{
    public class TransunionCibilService : ITransunionCibilService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICibilService _cibilService;
        private readonly ILoggerManager _logger;
        private readonly IOptions<CIC.DataUtility.AppSettingModel> _appsetting;
        public TransunionCibilService(ICibilService cibilService, IOptions<CIC.DataUtility.AppSettingModel> options, ILoggerManager logger)
        {
            _cibilService = cibilService;
            _logger = logger;
            _appsetting = options;
        }

        public async Task<TransuniunReturnResponse> GetCusomerCibilAsync(FulfillOfferRQ request,  string requiredHeader, string requiredcompanyid)
        {
            CibilApiResponse cibilApi = await _cibilService.GetCusomerCibil(request , requiredHeader , requiredcompanyid);
            TransuniunReturnResponse transuniunReturn =  ResultParser.TransCibil.ResultParser.ParseResponse(cibilApi, _logger);
            Task.Run(() => SaveToDB.PushToDatabase(transuniunReturn, requiredHeader, requiredcompanyid, _appsetting?.Value?.ConnectionStrings?.dbconnection ?? "", _logger));
            return transuniunReturn;
        }
    }
}
