using iText.StyledXmlParser.Jsoup.Nodes;
using LoggerLibrary;
using System.Data.SqlTypes;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace CIC.Helper
{
    public static class XmlHelper
    {
        public static T DeserializeXml<T>(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return default;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var reader = new StringReader(xml))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
            catch
            {
                return default;
            }
        }

        public static string ExtractAndParseExperianXml_New(string soapXml, ILoggerManager logger)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(soapXml))
                    return soapXml;

                // Parse SOAP safely
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Ignore,
                    CheckCharacters = false
                };

                XDocument soapDoc;
                using (var reader = XmlReader.Create(new StringReader(soapXml), settings))
                {
                    soapDoc = XDocument.Load(reader);
                }

                // Find processReturn
                var processReturn = soapDoc.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "processReturn");

                if (processReturn == null)
                    return soapXml;

                // Decode inner XML
                var decodedInnerXml = WebUtility.HtmlDecode(processReturn.Value?.Trim());

                if (string.IsNullOrWhiteSpace(decodedInnerXml))
                    return soapXml;

                // Remove embedded XML declarations
                decodedInnerXml = Regex.Replace(decodedInnerXml,
                    @"<\?xml.*?\?>",
                    "",
                    RegexOptions.IgnoreCase);

                // Fix invalid XML entities
                decodedInnerXml = Regex.Replace(decodedInnerXml,
                    @"&(?!amp;|lt;|gt;|quot;|apos;)",
                    "&amp;");

                // Parse inner XML safely
                logger.LogInfo($"XmlHelper ExtractAndParseExperianXml Error parsing SOAP XML decodedInnerXml: {decodedInnerXml}");
                XDocument innerDoc;
                using (var reader = XmlReader.Create(new StringReader(decodedInnerXml), settings))
                {
                    innerDoc = XDocument.Load(reader);
                }

                // Mask Subscriber_Name
                var subscriberNodes = innerDoc.Descendants()
                    .Where(x => x.Name.LocalName == "Subscriber_Name");

                foreach (var node in subscriberNodes)
                {
                    if (!string.IsNullOrWhiteSpace(node.Value) &&
                        node.Value.Contains("JUNOON CAPITAL SERVICES PRIVATE LIMITED",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        node.Value = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
                    }
                }

                // Encode inner XML before putting back
                processReturn.Value = WebUtility.HtmlEncode(innerDoc.ToString(SaveOptions.DisableFormatting));

                return soapDoc.ToString(SaveOptions.DisableFormatting);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error parsing SOAP XML: {ex}");
                return soapXml;
            }
        }
        public static string ExtractAndParseExperianXml(string soapXml, ILoggerManager logger)
        {
            //logger.LogInfo($"XmlHelper ExtractAndParseExperianXml Error parsing SOAP XML soapXml: {soapXml}");
            try
            {
                var doc = XDocument.Parse(soapXml);

                // SAFE: use FirstOrDefault
                var processReturn = doc.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "processReturn");

               // logger.LogInfo($"XmlHelper ExtractAndParseExperianXml Error parsing SOAP XML processReturn: {processReturn}");

                if (processReturn == null)
                    return soapXml;
                var innerXml = processReturn.Value.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&apos;", "'").Trim();
                // Decode + parse inner XML
                var decodedInnerXml = WebUtility.HtmlDecode(innerXml);
                //logger.LogInfo($"XmlHelper ExtractAndParseExperianXml Error parsing SOAP XML decodedInnerXml: {decodedInnerXml}");
                var innerDoc = ConvertStringToXDocument(decodedInnerXml);  //XDocument.Parse(decodedInnerXml);
               // logger.LogInfo($"XmlHelper ExtractAndParseExperianXml Error parsing SOAP XML innerDoc: {innerDoc.ToString()}");
                // Replace Subscriber_Name
                var subscriberNodes = innerDoc.Descendants()
                    .Where(x => x.Name.LocalName == "Subscriber_Name");

                foreach (var node in subscriberNodes)
                {
                    if (node.Value.Contains("JUNOON CAPITAL SERVICES PRIVATE LIMITED",
                                            StringComparison.OrdinalIgnoreCase))
                    {
                        node.Value = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
                    }
                }

                // ⭐ IMPORTANT: put inner XML back into SOAP
                processReturn.Value = innerDoc.ToString();

                //logger.LogInfo("Subscriber_Name masked successfully."+ processReturn.ToString());

                // ⭐ Return full SOAP
                return WebUtility.HtmlDecode(processReturn.ToString());
            }
            catch (Exception ex)
            {
                logger.LogError($"Error parsing SOAP XML: {ex} InnerException : {ex.Message}");
                return soapXml;
            }

        }

        public static XDocument ConvertStringToXDocument(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return null;

            // Fix invalid &
            xml = Regex.Replace(xml, @"&(?!amp;|lt;|gt;|quot;|apos;)", "&amp;");

            return XDocument.Parse(xml);
        }
    }
}
