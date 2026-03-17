using JC.TransUnion.Cibil.Models;

namespace JC.TransUnion.Cibil.Interface
{
    public interface ICibilHttpClient
    {
        Task<CibilApiResponse> PostAsync(
             string url,
             object body,
             Dictionary<string, string> headers,
             bool isProduction, string basePath);

    }
}
