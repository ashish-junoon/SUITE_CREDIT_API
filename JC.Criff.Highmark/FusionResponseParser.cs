using CIC.Model.Criff.Response;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace JC.Criff.Highmark
{
    public static class FusionResponseParser
    {
        public static FusionParsedResponse Parse(string response)
        {
            FusionParsedResponse result = new FusionParsedResponse();

            if (string.IsNullOrWhiteSpace(response))
                return result;

            // Extract XML
            var xmlMatch = Regex.Match(
                response,
                @"<FUSION-REPORT-FILE[\s\S]*?</FUSION-REPORT-FILE>",
                RegexOptions.IgnoreCase);

            if (xmlMatch.Success)
            {
                result.RawXml = xmlMatch.Value;

                try
                {
                    result.FusionNode = XElement.Parse(xmlMatch.Value);
                }
                catch
                {
                    // if XML malformed keep raw
                    result.FusionNode = null;
                }
            }

            // Extract HTML
            var htmlMatch = Regex.Match(
                response,
                @"<!DOCTYPE html[\s\S]*?</html>",
                RegexOptions.IgnoreCase);

            if (htmlMatch.Success)
            {
                result.HtmlContent = htmlMatch.Value;
            }

            return result;
        }
    }
}
