using CIC.Model.Criff.Response;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Xml;
using System.Xml.Linq;

namespace CIC_Services.ResultParser.CiffFusion
{
    public static class ResultParser
    {

        public static FusionResponseReturn ParseResponse(FusionParsedResponse response)
        {
            string trnID = Guid.NewGuid().ToString();
            XElement? node = response?.FusionNode;

            var chkStatus = node?.Descendants("REQ-SERVICE").FirstOrDefault().Element("STATUS").Value;
            if(chkStatus != "Y")
            {
                return new FusionResponseReturn
                {
                    data = null,
                    status = "failure",
                    timestamp = DateTime.Now,
                    transaction_id = trnID,
                    message = node?.Descendants("REQ-SERVICE").FirstOrDefault().Element("DESCRIPTION").Value
                };
            }

            var addresses = GetVariations(node, "ADDRESS-VARIATIONS");
            var names = GetVariations(node, "NAME-VARIATIONS");
            var pan = GetVariations(node, "PAN-VARIATIONS");
            var dob = GetVariations(node, "DATE-OF-BIRTH-VARIATIONS");
            var voterID = GetVariations(node, "VOTER-ID-VARIATIONS");
            var phone = GetVariations(node, "PHONE-NUMBER-VARIATIONS");
            var rationCard = GetVariations(node, "RATION-CARD-VARIATIONS");
            var emails = GetVariations(node, "EMAIL-VARIATIONS");
            var passport = GetVariations(node, "PASSPORT-VARIATIONS");
            var uid = GetVariations(node, "UID-VARIATIONS");

            var request = node?.Descendants("REQUEST").FirstOrDefault();

            int yearDiff = 0;

            var dateStr = dob.FirstOrDefault()?.Element("VALUE")?.Value;

            if (DateTime.TryParse(dateStr, out DateTime reportedDate))
            {
                yearDiff = DateTime.Now.Year - reportedDate.Year;
            }

            return new FusionResponseReturn
            {
                data = new Data
                {
                    CIRReportData = new Cirreportdata
                    {
                        IDAndContactInfo = new Idandcontactinfo
                        {
                            Identityinfo = new Identityinfo
                            {
                                Addressinfo = ParseVariations(addresses),
                                PANId = ParseVariations(pan),
                                DobInfo = ParseVariations(dob),
                                Emailaddressinfo = ParseVariations(emails),
                                NationalIdCard = ParseVariations(passport),
                                OtherId = ParseVariations(uid),
                                Phoneinfo = ParseVariations(phone),
                                Voterid = ParseVariations(voterID),
                            },
                            PersonalInfo = new Personalinfo
                            {
                                Name = new Name
                                {
                                    FullName = request.Element("NAME")?.Value,
                                },
                                DateOfBirth = dob.FirstOrDefault().Element("REPORTED-DATE")?.Value,
                                Age = new Age
                                {
                                    age = yearDiff
                                }
                            }
                        }
                    },
                    client_id = trnID
                },
                status = response?.success == true ? "success" : "failure",
                timestamp = DateTime.Now,
                transaction_id = trnID,
                message = "success"
            };
        }



        private static List<XElement> GetVariations(XElement? node, string elementName)
        {
            return node?
                .Descendants(elementName)
                .Descendants("VARIATION")
                .ToList()
                ?? new List<XElement>();
        }

        private static List<idVarition> ParseVariations(IEnumerable<XElement>? variationElements)
        {
            return variationElements?
              .Select((x, index) => new idVarition
              {
                    IdNumber = x.Element("VALUE")?.Value,
                    ReportedDate = x.Element("REPORTED-DATE")?.Value,
                    seq = (index + 1).ToString()
              })
                .ToList() ?? new List<idVarition>();
        }

        #region MyRegion
        //public static FusionResponseReturn ParseResponse(FusionParsedResponse response)
        //{
        //    var addresses = response.FusionNode?.Descendants("ADDRESS-VARIATIONS").Descendants("VARIATION").ToList();

        //    var names = response.FusionNode?.Descendants("NAME-VARIATIONS").Descendants("VARIATION").ToList();

        //    var pan = response.FusionNode?.Descendants("PAN-VARIATIONS").Descendants("VARIATION").ToList();

        //    var dob = response.FusionNode?.Descendants("DATE-OF-BIRTH-VARIATIONS").Descendants("VARIATION").ToList();

        //    var voterID = response.FusionNode?.Descendants("VOTER-ID-VARIATIONS").Descendants("VARIATION").ToList();

        //    var phone = response.FusionNode?.Descendants("PHONE-NUMBER-VARIATIONS").Descendants("VARIATION").ToList();

        //    var rationCard = response.FusionNode?.Descendants("RATION-CARD-VARIATIONS").Descendants("VARIATION").ToList();
        //    var emails = response.FusionNode?.Descendants("EMAIL-VARIATIONS").Descendants("VARIATION").ToList();
        //    var passport = response.FusionNode?.Descendants("PASSPORT-VARIATIONS").Descendants("VARIATION").ToList();
        //    var uid = response.FusionNode?.Descendants("UID-VARIATIONS").Descendants("VARIATION").ToList();

        //    return new FusionResponseReturn
        //    {
        //       data = new Data
        //       {
        //           CIRReportData = new Cirreportdata
        //           {
        //               IDAndContactInfo = new Idandcontactinfo
        //               {
        //                   Identityinfo = new Identityinfo
        //                   {
        //                       Addressinfo =ParseVariations(addresses),
        //                       PANId = ParseVariations(pan),
        //                       DobInfo = ParseVariations(dob),
        //                       Emailaddressinfo = ParseVariations(emails),
        //                       NationalIdCard = ParseVariations(passport),
        //                       OtherId = ParseVariations(uid),
        //                       Phoneinfo = ParseVariations(phone),
        //                       Voterid= ParseVariations(voterID),
        //                   },
        //               }
        //           },
        //       },
        //       status = response.success ? "success" : "failure",
        //       timestamp = DateTime.Now,
        //       transaction_id = Guid.NewGuid().ToString()
        //    };
        //}

        //private static List<idVarition> ParseVariations(IEnumerable<XElement>? variationElements)
        //{
        //    return variationElements?
        //        .Select(x => new idVarition
        //        {
        //            IdNumber = x.Element("VALUE")?.Value,
        //            ReportedDate = x.Element("REPORTED-DATE")?.Value,
        //            seq = x.Value
        //        })
        //        .ToList() ?? new List<idVarition>();
        //}
        #endregion


    }
}
