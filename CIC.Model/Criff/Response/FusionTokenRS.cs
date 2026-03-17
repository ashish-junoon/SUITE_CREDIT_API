namespace CIC.Model.Criff.Response
{
    using System.Xml.Serialization;

    [XmlRoot("REPORT-FILE")]
    public class ReportFile
    {
        [XmlElement("INQUIRY-STATUS")]
        public InquiryStatus InquiryStatus { get; set; }
    }

    public class InquiryStatus
    {
        [XmlElement("INQUIRY")]
        public Inquiry Inquiry { get; set; }
    }

    public class Inquiry
    {
        [XmlElement("INQUIRY-UNIQUE-REF-NO")]
        public string InquiryUniqueRefNo { get; set; }

        [XmlElement("CLIENT-CUSTOMER-ID")]
        public string ClientCustomerId { get; set; }

        [XmlElement("REQUEST-DT-TM")]
        public string RequestDateTime { get; set; }

        [XmlElement("REPORT-ID")]
        public string ReportId { get; set; }

        [XmlElement("RESPONSE-DT-TM")]
        public string ResponseDateTime { get; set; }

        [XmlElement("REPONSE-TYPE")]
        public string ResponseType { get; set; }
    }

}
