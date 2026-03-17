using Microsoft.AspNetCore.Http;

namespace CIC.Helper
{
    public class UrlService
    {
        private readonly IHttpContextAccessor _context;

        public UrlService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetCurrentUrl()
        {
            var req = _context.HttpContext.Request;
            return $"{req.Scheme}://{req.Host}{req.Path}{req.QueryString}";
        }
    }

}
