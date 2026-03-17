using System.Xml.Linq;

namespace CIC.Model.Criff.Response
{
    public class FusionParsedResponse
    {
        public XElement FusionNode { get; set; }
        public string HtmlContent { get; set; }
        public string RawXml { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public bool success { get; set; }
    }
}
