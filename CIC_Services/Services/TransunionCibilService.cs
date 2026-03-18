using CIC.Model.Experian.Response;
using CIC.Model.TransUnionCibil;
using CIC_Services.Interfaces;
using JC.TransUnion.Cibil.Interface;
using JC.TransUnion.Cibil.Models;
using LoggerLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CIC_Services.Services
{
    public class TransunionCibilService : ITransunionCibilService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICibilService _cibilService;
        private readonly ILoggerManager _logger;
        public TransunionCibilService(ICibilService cibilService, ILoggerManager logger)
        {
            _cibilService = cibilService;
            _logger = logger;
        }

        public async Task<TransuniunReturnResponse> GetCusomerCibilAsync(FulfillOfferRQ request,  string requiredHeader, string requiredcompanyid)
        {
            CibilApiResponse cibilApi = await _cibilService.GetCusomerCibil(request , requiredHeader , requiredcompanyid);
            return ResultParser.TransCibil.ResultParser.ParseResponse(cibilApi, _logger);
        }
    }
}
