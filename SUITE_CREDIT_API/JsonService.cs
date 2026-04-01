using Newtonsoft.Json;

namespace SUITE_CREDIT_API
{
    public class JsonService
    {
        private readonly IWebHostEnvironment _env;
        public JsonService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<T> ReadJsonAsync<T>(string folderName,string fileName)
        {
            var filePath = Path.Combine(_env.ContentRootPath, folderName, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {folderName}//{fileName}");

            var json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
