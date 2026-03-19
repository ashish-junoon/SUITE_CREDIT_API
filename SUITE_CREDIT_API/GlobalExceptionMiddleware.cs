using CIC.DataUtility;
using LoggerLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CIC_Services
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<AppSettingModel> _options;
        private readonly IOptions<ExternalPartner> _appexter;
        private readonly ILoggerManager _logger;
        public GlobalExceptionMiddleware(RequestDelegate next, IOptions<ExternalPartner> _app, IOptions<AppSettingModel> options, ILoggerManager _log)
        {
            _next = next;
            _options = options;
            _appexter = _app ?? throw new ArgumentNullException();
            _logger = _log;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                var methodName = string.Empty;
                var endpoint = context.GetEndpoint();
                var allowAnyIP = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>();
                if (endpoint != null)
                {
                    var routeData = context.GetRouteData();
                    var controllerName = routeData.Values["controller"]?.ToString();
                    methodName = routeData.Values["action"]?.ToString();
                    if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(methodName))
                    {
                        _logger.LogInfo($"Controller: {controllerName}, Action: {methodName}");
                    }
                    else
                    {
                        _logger.LogError($"Controller and action name not found: {controllerName}, Action: {methodName}");
                    }
                }

                if (allowAnyIP != null)
                {
                    await _next(context);
                    return;
                }
                string? vendor = null;
                string? token = null;
                if (context!.Request!.Headers != null)
                {
                    token = context.Request.Headers["token"];
                    vendor = context.Request.Headers["companyid"];
                }
                if (context.Request.Headers != null)
                {
                    ExternalPartner? partners = ValiateUserRepository.GetPartnersDetails(token, vendor, methodName, _options.Value.ConnectionStrings?.dbconnection ?? string.Empty,null);
                    ////CommonRequestRepository.SaveVendorServiceRequest(vendor, methodName, body , _options.Value.ConnectionStrings?.dbconnection ?? string.Empty, _logger);
                    if (partners?.message == "401 Unauthorized: IP not allowed." && partners.status == false)
                    {
                        //_logger.LogInfo($"Invalid token: {token} and vendor not allowed : {vendor}");
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("401 Unauthorized: IP not allowed.");
                        return;
                    }
                    //else if (partners?.status == false || (!string.IsNullOrEmpty(partners?.message)))
                    //{
                    //    //_logger.LogInfo($"Partner validation failed for token: {token}, vendor: {vendor}, message: {partners?.message}");
                    //    var responseObj = new
                    //    {
                    //        status = partners?.status,
                    //        message = partners?.message
                    //    };
                    //    string jsonResponse = JsonConvert.SerializeObject(responseObj);
                    //    await context.Response.WriteAsync(jsonResponse);
                    //    return;
                    //}
                }
                await _next(context);
            }
            catch (ApiException ex)
            {
                //_logger.LogInfo($"{ex.Message}");
                await context.Response.WriteAsJsonAsync(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                //_logger.LogInfo($"Internal server error : {ex.Message}");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { success = false, message = "Internal server error" });
            }
        }
    }
}
