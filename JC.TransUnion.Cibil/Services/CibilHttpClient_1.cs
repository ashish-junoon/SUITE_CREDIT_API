using Org.BouncyCastle.Asn1;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace JC.TransUnion.Cibil.Services
{
    public static class CibilHttpClient_1
    {
        public static async Task<(int Status, dynamic Data)> PostAsync(
            string url, object body, bool IsProduction)
        {
           string PfxPath = IsProduction ? CibilConfig.ProdVariables.MEMBER_PFX_CERT_PATH : CibilConfig.UATVariables.MEMBER_PFX_CERT_PATH;
            string PfxPassphrase = IsProduction ? CibilConfig.ProdVariables.MEMBER_PFX_PASSWORD : CibilConfig.UATVariables.MEMBER_PFX_PASSWORD;
            string MemberRefId = IsProduction ? CibilConfig.ProdVariables.MEMBER_REF_ID : CibilConfig.UATVariables.MEMBER_REF_ID;
            string ApiKey = IsProduction ? CibilConfig.ProdVariables.API_KEY : CibilConfig.UATVariables.API_KEY;
            string BasicToken = IsProduction ? CibilConfig.ProdVariables.CLIENT_SECRET : CibilConfig.UATVariables.CLIENT_SECRET;
           
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(
                new X509Certificate2(PfxPath, PfxPassphrase));
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            using var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("member-ref-id", MemberRefId);
            client.DefaultRequestHeaders.Add("x-api-key", ApiKey);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", BasicToken);

            var json = JsonSerializer.Serialize(body);
            var resp = await client.PostAsync(url,
                new StringContent(json, Encoding.UTF8, "application/json"));

            var content = await resp.Content.ReadAsStringAsync();
            return ((int)resp.StatusCode, JsonSerializer.Deserialize<dynamic>(content));
        }
    }
}
