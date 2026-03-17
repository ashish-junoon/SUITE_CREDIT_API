using Microsoft.AspNetCore.Hosting;
namespace JC.TransUnion.Cibil
{
    public class FileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string RootPath()
        {
            return _env.ContentRootPath;
        }
        public string WebRootPath()
        {
            return _env.WebRootPath;
        }
        public string EnvironmentName()
        {
            return _env.EnvironmentName;
        }
    }
}
