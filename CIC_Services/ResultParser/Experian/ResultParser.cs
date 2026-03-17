using LoggerLibrary;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Linq;

namespace CIC_Services.ResultParser.Experian
{
    public static class ResultParser
    {
        public static object ParsetoXml(CIC.Model.Experian.Response.ExperianResponse input, ILoggerManager _logger)
        {
            
            if (input.Success)
            {
                var doc = XDocument.Parse(Convert.ToString(input?.Data));
                
                try
                {
                    XElement elementToRemove = doc?.Descendants("Header")?.FirstOrDefault();
                    if (elementToRemove != null)
                    {
                        elementToRemove.Remove(); // Removes the element from its parent
                    }
                    //elementToRemove = doc?.Descendants("CreditProfileHeader")?.FirstOrDefault();
                    //if (elementToRemove != null)
                    //{
                    //    elementToRemove.Remove();
                    //}

                    var applicant = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "INProfileResponse");
                    var parent = new XElement("ExperianResponse");
                    parent.Add(new XElement("timestamp", input?.Timestamp));
                    parent.Add(new XElement("transaction_id", input?.Transaction_id));
                    parent.Add(new XElement("statusCode", input?.StatusCode));
                    parent.Add(new XElement("success", input?.Success));
                    parent.Add(new XElement("message", input?.Message),new XElement(applicant));
                    //parent.Add(new XElement("data", new XElement(applicant)));
                    //parent.Add(new XElement(applicant));
                    return parent;
                }
                catch (Exception ex)
                {
                    var applicant = doc.Element("INProfileResponse");
                    var parent = new XElement("ExperianResponse");
                    parent.Add(new XElement("timestamp", input?.Timestamp));
                    parent.Add(new XElement("transaction_id", input?.Transaction_id));
                    parent.Add(new XElement("statusCode", input?.StatusCode));
                    parent.Add(new XElement("success", input?.Success));
                    parent.Add(new XElement("message", input?.Message));
                    parent.Add(new XElement("result", ex.Message));
                    return parent;
                }
            }
            else
            {
                var parent = new XElement("ExperianResponse");
                parent.Add(new XElement("timestamp", input?.Timestamp));
                parent.Add(new XElement("transaction_id", input?.Transaction_id));
                parent.Add(new XElement("statusCode", input?.StatusCode));
                parent.Add(new XElement("success", input?.Success));
                parent.Add(new XElement("message", input?.Message));
                parent.Add(new XElement("messageCode", "ERR_CREDIT_REPORT"));
                parent.Add(new XElement("error", "Error fetching credit report"));
                return parent;
            }
        }

        public static T ConvertXmlToJson<T>(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            string json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None, true);
            //ExperianResponsePdf experianResponse = JsonConvert.DeserializeObject<ExperianResponsePdf>(JsonConvert.SerializeObject(json));
            //await Task.Run(() => ExperianRepository.SaveExperianReport(experianResponse, requiredcompanyid));

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
