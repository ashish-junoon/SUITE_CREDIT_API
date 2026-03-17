using System.Text;

namespace JC.Criff.Highmark
{
    public static class HttpClientPost
    {
        public static async Task<dynamic> PostAsync(
            string url,
            string body,
            Dictionary<string, string> headers,
            string contentType)
        {
            HttpClient _http = new HttpClient();
            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Content = new StringContent(body, Encoding.UTF8, contentType);

            foreach (var h in headers)
                req.Headers.TryAddWithoutValidation(h.Key, h.Value);

            var resp = await _http.SendAsync(req);
            var text = await resp.Content.ReadAsStringAsync();

            return text; //System.Text.Json.JsonSerializer.Deserialize<dynamic>(text);
        }
    }
}

