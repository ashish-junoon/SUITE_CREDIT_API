using CIC.Model.Criff.Response;
using System.Globalization;
using System.Text;

namespace JC.Criff.Highmark
{
    public class CrifHtmlBuilder
    {
        public static string BuildHtml(CrifResponseReturn? crif, Dictionary<string, string> dictVariation)
        {
            if (crif?.Data?.B2CReport == null) return "<html><body><h1>No CRIF Data Available</h1></body></html>";

            var sb = new StringBuilder();

            // Get data from CRIF response
            var header = crif.Data.B2CReport.Header;
            var applicant = crif.Data.B2CReport.RequestData?.Applicant;
            var standardData = crif.Data.B2CReport.ReportData?.StandardData;
            var demogs = standardData?.Demogs;
            var summary = crif.Data.B2CReport.ReportData?.AccountsSummary;
            var tradelines = standardData?.Tradelines;
            var scores = standardData?.Score;
            var inquiries = standardData?.InquiryHistory;

            // Helper function to format dates
            string formatDate(string? dateStr)
            {
                if (string.IsNullOrEmpty(dateStr) || dateStr == "NA" || dateStr == "na" || dateStr == "-") return "-";
                if (DateTime.TryParseExact(dateStr, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    return date.ToString("dd-MM-yyyy");
                if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    return date.ToString("dd-MM-yyyy");
                if (DateTime.TryParse(dateStr, out date))
                    return date.ToString("dd-MM-yyyy");
                return dateStr;
            }

            // Helper to format currency
            string formatCurrency(string? amount)
            {
                if (string.IsNullOrEmpty(amount) || amount == "0" || amount == "0.00" || amount == "0,00") return "0";

                // Remove commas and try parsing
                string cleanAmount = amount?.Replace(",", "") ?? "0";
                if (decimal.TryParse(cleanAmount, out decimal value))
                    return value.ToString("#,##0");
                return amount ?? "0";
            }

            // Helper to get variations
            List<Variation> GetVariations(string variationType)
            {
                var variations = demogs?.Variations?.FirstOrDefault(v => v.Type == variationType);
                return variations?.Variation ?? new List<Variation>();
            }

            // Helper to get the primary address
            string GetPrimaryAddress()
            {
                if (dictVariation["ADDRESS-VARIATIONS"].Any())
                {
                    return dictVariation["ADDRESS-VARIATIONS"];
                }
                var addresses = applicant?.Addresses;
                if (addresses?.Any() == true)
                {
                    var primary = addresses.FirstOrDefault();
                    return $"{primary?.AddressText}, {primary?.City}, {primary?.State} - {primary?.Pin}";
                }
                return "-";
            }

            // Helper to get the primary phone
            string GetPrimaryPhone()
            {
                if(dictVariation["PHONE-VARIATIONS"].Any())
                {
                    return dictVariation["PHONE-VARIATIONS"];
                }
                var phones = applicant?.Phones;
                return phones?.FirstOrDefault()?.Value ?? "-";
            }

            // Helper to get the primary email
            string GetPrimaryEmail()
            {
                if (dictVariation["EMAIL-VARIATIONS"].Any())
                {
                    return dictVariation["EMAIL-VARIATIONS"];
                }
                var emails = applicant?.Emails;
                return emails?.FirstOrDefault()?.Email ?? "-";
            }

            // Helper to get PAN
            string GetPan()
            {
                if (dictVariation["PAN-VARIATIONS"].Any())
                {
                    return dictVariation["PAN-VARIATIONS"];
                }

                var ids = applicant?.Ids;
                var pan = ids?.FirstOrDefault(i => i.Type == "ID07");
                return pan?.Value ?? "-";
            }

            // Get applicant name
            string GetApplicantName()
            {
                string name = string.Empty;
                if (dictVariation["NAME-VARIATIONS"].Any())
                {
                    name = dictVariation["NAME-VARIATIONS"].Trim();
                }
                return string.IsNullOrEmpty(name) ? $"{applicant?.FirstName} {applicant?.LastName}".Trim() : name;
            }

            // Get DOB
            string GetDob()
            {
                //return formatDate(applicant?.Dob?.Date);
                return formatDate(dictVariation["DOB-VARIATIONS"]);
            }

            // Build the HTML using your template structure
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\" />");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />");
            sb.AppendLine("    <title>CRIF Report</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        /* ================= BASE ================= */");
            sb.AppendLine("        * {");
            sb.AppendLine("          box-sizing: border-box;");
            sb.AppendLine("        }");
            sb.AppendLine("        body {");
            sb.AppendLine("          margin: 0;");
            sb.AppendLine("          padding: 0;");
            sb.AppendLine("          font-family: Arial, Helvetica, sans-serif;");
            sb.AppendLine("        }");
            sb.AppendLine("        img {");
            sb.AppendLine("          max-width: 100%;");
            sb.AppendLine("          height: auto;");
            sb.AppendLine("        }");
            sb.AppendLine("        table {");
            sb.AppendLine("          width: 100%;");
            sb.AppendLine("          border-collapse: collapse;");
            sb.AppendLine("        }");
            sb.AppendLine("        /* ================= PRINT ================= */");
            sb.AppendLine("        @media print {");
            sb.AppendLine("          body {");
            sb.AppendLine("            -webkit-print-color-adjust: exact !important;");
            sb.AppendLine("            print-color-adjust: exact !important;");
            sb.AppendLine("            margin: 0;");
            sb.AppendLine("            padding: 0;");
            sb.AppendLine("          }");
            sb.AppendLine("          /* Page size */");
            sb.AppendLine("          @page {");
            sb.AppendLine("            size: A4;");
            sb.AppendLine("            margin: 0;");
            sb.AppendLine("          }");
            sb.AppendLine("        }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body style=\"font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif\">");
            sb.AppendLine("    <div style=\"width: 1000px; margin: auto; border: 1px solid rgba(128, 128, 128, 0.281);\">");

            // HEADER SECTION - CONSUMER BASE REPORT
            sb.AppendLine("        <div style=\"padding: 6px; display: grid; grid-template-columns: 1fr 2fr 1fr; align-items: center; gap: 30px; border-bottom: 1px solid #d1d5db;\">");
            sb.AppendLine("            <div style=\"width: 150px\">");
            sb.AppendLine("                <img src=\"https://www.junooncapital.com/img/crif.png\" style=\"width: 100%\" />");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div style=\"padding-left: 12px; width: fit-content;\">");
            sb.AppendLine("                <h1 style=\"font-size: 18px; font-weight: 700; text-transform: uppercase; color: #083a5dc4; margin: 0; text-align: right;\">");
            sb.AppendLine("                    Consumer Base<sup style=\"font-size: 8px;\">TM</sup>");
            sb.AppendLine("                    Report");
            sb.AppendLine("                </h1>");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0; width: fit-content; color: #000; font-size: 12px; text-transform: uppercase; float: right;\">");
            sb.AppendLine($"                    For {GetApplicantName()}");
            sb.AppendLine("                </p>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div style=\"padding-top: 16px; padding-bottom: 16px; display: flex; flex-direction: column; gap: 6px; min-width: 260px; font-size: 12px;\">");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">CHM Ref #:</span> {header?.ReportId ?? "-"}</p>");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">Application ID:</span>{header?.BatchId ?? "-"}</p>");
            //sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">Prepared For:</span> JUNOON CAPITAL SERVICES PRIVATE LIMITED</p>");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">Date of Request:</span> {formatDate(header?.DateOfRequest)}</p>");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">Date of Issue:</span> {formatDate(header?.DateOfIssue)}</p>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");

            // INQUIRY INPUT INFORMATION
            sb.AppendLine("        <div style=\"padding: 0px 12px; padding-top: 0px; padding-bottom: 0\">");
            sb.AppendLine("            <h2 style=\"margin-bottom: 0px; min-height: 15px; color: #ffffff; font-weight: 500; padding-left: 12px;  font-size: 15px; background-color: #08395d; opacity: 0.9; padding: 4px 8px;\">");
            sb.AppendLine("                Inquiry Input Information");
            sb.AppendLine("            </h2>");
            sb.AppendLine("            <div style=\"display: grid; grid-template-columns: repeat(3, 1fr); padding-left: 12px; padding-right: 12px; font-weight: 500;\">");
            sb.AppendLine($"                <p style=\"display: grid; grid-template-columns: 120px 1fr; font-size: 12px;\"><span style=\"color: #083a5dc4\">Name:</span><span>{GetApplicantName()}</span></p>");
            sb.AppendLine($"                <p style=\"display: grid; grid-template-columns: 120px 1fr; font-size: 12px;\"><span style=\"color: #083a5dc4\">DOB:</span><span>{GetDob()}</span></p>");
            sb.AppendLine($"                <p style=\"display: grid; grid-template-columns: 120px 1fr; font-size: 12px;\"><span style=\"color: #083a5dc4\">Phone Number:</span><span>{GetPrimaryPhone()}</span></p>");
            sb.AppendLine($"                <p style=\"display: grid; grid-template-columns: 120px 1fr; font-size: 12px;\"><span style=\"color: #083a5dc4\">Id(s):</span><span>{GetPan()} [PAN]</span></p>");
            sb.AppendLine($"                <p style=\"display: grid; grid-template-columns: 120px 1fr; font-size: 12px;\"><span style=\"color: #083a5dc4\">Email Id(s):</span><span>{GetPrimaryEmail()}</span></p>");
            sb.AppendLine("            </div>");
            sb.AppendLine($"            <p style=\"margin-top: 8px; font-size: 12px; font-weight: 500; padding-left: 12px;\"><span style=\"color: #083a5dc4\">Entity Id</span></p>");
            sb.AppendLine($"            <p style=\"margin-top: 8px; font-size: 12px; font-weight: 500; padding-left: 12px;\"><span style=\"color: #083a5dc4\">Current Address:</span> {GetPrimaryAddress()}</p>");
            sb.AppendLine("            <p style=\"margin-top: 8px; font-size: 12px; font-weight: 500; padding-left: 12px;\"><span style=\"color: #083a5dc4\">Other Address:</span></p>");
            sb.AppendLine("        </div>");

            // CRIF HM SCORE(S)
            sb.AppendLine("        <div style=\"padding: 0px 12px; padding-top: 20px; padding-bottom: 0\">");
            sb.AppendLine("            <h2 style=\"margin-bottom: 18px; min-height: 15px; color: #ffffff; font-weight: 600; padding-left: 12px; font-size: 15px; background-color: #08395d; opacity: 0.9; padding: 6px 8px;\">");
            sb.AppendLine("                CRIF HM Score(s):");
            sb.AppendLine("            </h2>");
            sb.AppendLine("            <table style=\"width: 100%; border-collapse: collapse\">");
            sb.AppendLine("                <thead>");
            sb.AppendLine("                    <tr style=\"color: #083a5dc4; opacity: 0.8; text-transform: uppercase; text-align: left; font-size: 14px;\">");
            sb.AppendLine("                        <th style=\"font-weight: 600; padding: 6px\">Score Name</th>");
            sb.AppendLine("                        <th style=\"font-weight: 600; padding: 6px\">Score</th>");
            sb.AppendLine("                        <th style=\"font-weight: 600; padding: 6px\">Scoring Factors</th>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </thead>");
            sb.AppendLine("                <tbody style=\"background-color: #8765ff54; opacity: 0.6;\">");
            sb.AppendLine("                    <tr>");
            sb.AppendLine($"                        <td style=\"padding: 6px; font-weight: 700; border: 1px solid white; font-size: 13px; text-transform: uppercase;\">{scores?.FirstOrDefault()?.Name ?? "PERFORM CONSUMER 2.0"}</td>");
            sb.AppendLine("                        <td style=\"padding: 6px; border: 1px solid white;\">");
            sb.AppendLine($"                            <span style=\"font-size: 22px; font-weight: 700\"> {scores?.FirstOrDefault()?.Value ?? "0"} </span>");
            sb.AppendLine("                            <span style=\"font-size: 12px; font-weight: 700; color: #000\">&nbsp;Score Range : 300-900</span>");
            sb.AppendLine("                        </td>");
            sb.AppendLine("                        <td style=\"padding: 6px; font-weight: 700; font-size: 12px; border: 1px solid white; color: #000;\">");
            sb.AppendLine("                            <ul style=\"padding: 0; margin: 0; list-style: none\">");

            var scoreFactors = scores?.FirstOrDefault()?.Factors;
            if (scoreFactors?.Any() == true)
            {
                var scoreFactorMap = new Dictionary<string, string>
                {
                    { "SF01", "No missed payments, healthy balance ratio" },
                    { "SF02", "Normal proportion of outstanding balance to disbursed amount" },
                    { "SF03", "Low credit utilization across accounts" },
                    { "SF04", "Long and satisfactory credit history" },
                    { "SF05", "Limited number of active credit accounts" },
                    { "SF06", "No recent adverse credit events" },
                    { "SF07", "Timely repayment behavior on recent loans" },
                    { "SF08", "Balanced mix of secured and unsecured credit" },
                    { "SF09", "Low number of recent credit enquiries" },
                    { "SF10", "Stable outstanding balance over time" },
                    { "SF11", "No settlement, write-off, or default history" }
                };

                foreach (var factor in scoreFactors)
                {
                    string factorText = scoreFactorMap.ContainsKey(factor.Type) ? scoreFactorMap[factor.Type] : factor.Description;
                    sb.AppendLine($"                                <li style=\"position: relative; padding-left: 14px; margin-bottom: 6px;\"><span style=\"position: absolute; left: 0; top: 6px; width: 5px; height: 5px; background-color: #15803d;\"></span>{factorText}</li>");
                }
            }
            else
            {
                sb.AppendLine("                                <li style=\"position: relative; padding-left: 14px; margin-bottom: 6px;\"><span style=\"position: absolute; left: 0; top: 6px; width: 5px; height: 5px; background-color: #15803d;\"></span>No missed payments, healthy balance ratio</li>");
                sb.AppendLine("                                <li style=\"position: relative; padding-left: 14px;\"><span style=\"position: absolute; left: 0; top: 6px; width: 5px; height: 5px; background-color: #15803d;\"></span>Normal proportion of outstanding balance to disbursed amount</li>");
            }

            sb.AppendLine("                            </ul>");
            sb.AppendLine("                        </td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </tbody>");
            sb.AppendLine("            </table>");
            sb.AppendLine("            <div style=\"display: flex; justify-content: flex-end; gap: 12px; padding-right: 24px; margin-top: 8px;\">");
            sb.AppendLine("                <span style=\"font-size: 12px\">Tip :</span>");
            sb.AppendLine("                <span style=\"position: relative; padding-left: 12px; font-size: 12px\"><span style=\"position: absolute; left: 0; top: 6px; width: 5px; height: 5px; background-color: #22c55e;\"></span>Positive impact on credit score</span>");
            sb.AppendLine("                <span style=\"position: relative; padding-left: 12px; font-size: 12px\"><span style=\"position: absolute; left: 0; top: 5px; width: 5px; height: 5px; background-color: #ef4444;\"></span>Negative impact on credit score</span>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");

            // PERSONAL INFORMATION - VARIATIONS
            sb.AppendLine("        <div style=\"padding: 0px 24px; padding-top: 20px\">");
            sb.AppendLine("            <h2 style=\"min-height: 15px; color: #ffffff; font-weight: 600; padding-left: 12px;  font-size: 15px; background-color: #08395d; opacity: 0.9; padding: 6px 8px;\">");
            sb.AppendLine("                Personal Information – Variations");
            sb.AppendLine("            </h2>");
            sb.AppendLine("            <span style=\"color: #9ca3af; font-size: 9px; display: block; margin-bottom: 6px; text-align: right; position: relative; top: -10px;\">");
            sb.AppendLine("                Tip: These are applicant's personal information variations as contributed by various financial institutions.");
            sb.AppendLine("            </span>");
            sb.AppendLine("            <div style=\"width: 100%; font-size: 14px\">");
            sb.AppendLine("                <div style=\"display: grid; grid-template-columns: 2fr 1fr; gap: 24px; padding: 4px 16px; font-weight: 700; font-size: 13px; color: #656565;\">");

            // LEFT COLUMN
            sb.AppendLine("                    <div style=\"display: flex; flex-direction: column; gap: 8px\">");

            // Name Variations
            var nameVariations = GetVariations("NAME-VARIATIONS");
            sb.AppendLine("                        <div>");
            sb.AppendLine("                            <div style=\"display: flex; justify-content: space-between; color: #0f3c64; border-bottom: 1px solid #93c5fd; padding-bottom: 10px;\">");
            sb.AppendLine("                                <span>Name Variations</span>");
            sb.AppendLine("                                <span>Reported On</span>");
            sb.AppendLine("                            </div>");

            if (nameVariations.Any())
            {
                foreach (var variation in nameVariations.OrderByDescending(x => formatDate(x.ReportedDate)))
                {
                    sb.AppendLine($"                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
                    sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">{variation.Value ?? "-"}</span>");
                    sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">{formatDate(variation.ReportedDate)}</span>");
                    sb.AppendLine("                            </div>");
                }
            }
            else
            {
                sb.AppendLine("                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
                sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">{GetApplicantName()}</span>");
                sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">{formatDate(header?.DateOfIssue)}</span>");
                sb.AppendLine("                            </div>");
            }
            sb.AppendLine("                        </div>");

            // Address Variations
            var addressVariations = GetVariations("ADDRESS-VARIATIONS");
            sb.AppendLine("                        <div>");
            sb.AppendLine("                            <div style=\"display: flex; justify-content: space-between; color: #0f3c64; border-bottom: 1px solid #93c5fd; padding-bottom: 10px;\">");
            sb.AppendLine("                                <span>Address Variations</span>");
            sb.AppendLine("                                <span>Reported On</span>");
            sb.AppendLine("                            </div>");

            if (addressVariations.Any())
            {
                foreach (var variation in addressVariations.OrderByDescending(x => formatDate(x.ReportedDate)))
                {
                    sb.AppendLine($"                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
                    sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">{variation.Value ?? "-"}</span>");
                    sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">{formatDate(variation.ReportedDate)}</span>");
                    sb.AppendLine("                            </div>");
                }
            }
            else
            {
                sb.AppendLine("                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
                sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">{GetPrimaryAddress()}</span>");
                sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">{formatDate(header?.DateOfIssue)}</span>");
                sb.AppendLine("                            </div>");
            }
            sb.AppendLine("                        </div>");

            // Email Variations
            var emailVariations = GetVariations("EMAIL-VARIATIONS");
            sb.AppendLine("                        <div>");
            sb.AppendLine("                            <div style=\"display: flex; justify-content: space-between; color: #0f3c64; border-bottom: 1px solid #93c5fd; padding-bottom: 10px;\">");
            sb.AppendLine("                                <span>Email ID Variations</span>");
            sb.AppendLine("                                <span>Reported On</span>");
            sb.AppendLine("                            </div>");

            if (emailVariations.Any())
            {
                foreach (var variation in emailVariations.OrderByDescending(x => formatDate(x.ReportedDate)))
                {
                    sb.AppendLine($"                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
                    sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">{variation.Value ?? "-"}</span>");
                    sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">{formatDate(variation.ReportedDate)}</span>");
                    sb.AppendLine("                            </div>");
                }
            }
            else
            {
                sb.AppendLine("                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
                sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">{GetPrimaryEmail()}</span>");
                sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">{formatDate(header?.DateOfIssue)}</span>");
                sb.AppendLine("                            </div>");
            }
            sb.AppendLine("                        </div>");
            sb.AppendLine("                    </div>");

            // RIGHT COLUMN
            sb.AppendLine("                    <div style=\"display: flex; flex-direction: column; gap: 12px\">");

            // DOB Variations
            var dobVariations = GetVariations("DOB-VARIATIONS");
            sb.AppendLine("                        <div>");
            sb.AppendLine("                            <div style=\"display: flex; justify-content: space-between; color: #0f3c64; border-bottom: 1px solid #93c5fd; padding-bottom: 10px;\">");
            sb.AppendLine("                                <span>DOB Variations</span>");
            sb.AppendLine("                                <span>Reported On</span>");
            sb.AppendLine("                            </div>");

            if (dobVariations.Any())
            {
                foreach (var variation in dobVariations.OrderByDescending(x => formatDate(x.ReportedDate)))
                {
                    sb.AppendLine($"                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
                    sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">{formatDate(variation.Value)}</span>");
                    sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">{formatDate(variation.ReportedDate)}</span>");
                    sb.AppendLine("                            </div>");
                }
            }
            else
            {
                sb.AppendLine("                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
                sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">{GetDob()}</span>");
                sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">{formatDate(header?.DateOfIssue)}</span>");
                sb.AppendLine("                            </div>");
            }
            sb.AppendLine("                        </div>");

            // DL Variations (not in new model, keep placeholder)
            sb.AppendLine("                        <div>");
            sb.AppendLine("                            <div style=\"display: flex; justify-content: space-between; color: #0f3c64; border-bottom: 1px solid #93c5fd; padding-bottom: 10px;\">");
            sb.AppendLine("                                <span>DL Variations</span>");
            sb.AppendLine("                                <span>Reported On</span>");
            sb.AppendLine("                            </div>");
            sb.AppendLine("                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
            sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">-</span>");
            sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">-</span>");
            sb.AppendLine("                            </div>");
            sb.AppendLine("                        </div>");

            // Ration Card Variations (not in new model, keep placeholder)
            sb.AppendLine("                        <div>");
            sb.AppendLine("                            <div style=\"display: flex; justify-content: space-between; color: #0f3c64; border-bottom: 1px solid #93c5fd; padding-bottom: 10px;\">");
            sb.AppendLine("                                <span>Ration Card Variations</span>");
            sb.AppendLine("                                <span>Reported On</span>");
            sb.AppendLine("                            </div>");
            sb.AppendLine("                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
            sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">-</span>");
            sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">-</span>");
            sb.AppendLine("                            </div>");
            sb.AppendLine("                        </div>");

            // Phone Variations
            var phoneVariations = GetVariations("PHONE-VARIATIONS");
            sb.AppendLine("                        <div>");
            sb.AppendLine("                            <div style=\"display: flex; justify-content: space-between; color: #0f3c64; border-bottom: 1px solid #93c5fd; padding-bottom: 10px;\">");
            sb.AppendLine("                                <span>Phone Variations</span>");
            sb.AppendLine("                                <span>Reported On</span>");
            sb.AppendLine("                            </div>");

            if (phoneVariations.Any())
            {
                foreach (var variation in phoneVariations.OrderByDescending(x => formatDate(x.ReportedDate)))
                {
                    sb.AppendLine($"                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
                    sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">{variation.Value ?? "-"}</span>");
                    sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">{formatDate(variation.ReportedDate)}</span>");
                    sb.AppendLine("                            </div>");
                }
            }
            else
            {
                sb.AppendLine("                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
                sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">{GetPrimaryPhone()}</span>");
                sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">{formatDate(header?.DateOfIssue)}</span>");
                sb.AppendLine("                            </div>");
            }
            sb.AppendLine("                        </div>");

            // ID Variations (PAN)
            var panVariations = GetVariations("PAN-VARIATIONS");
            sb.AppendLine("                        <div>");
            sb.AppendLine("                            <div style=\"display: flex; justify-content: space-between; color: #0f3c64; border-bottom: 1px solid #93c5fd; padding-bottom: 10px;\">");
            sb.AppendLine("                                <span>ID Variations</span>");
            sb.AppendLine("                                <span>Reported On</span>");
            sb.AppendLine("                            </div>");

            if (panVariations.Any())
            {
                foreach (var variation in panVariations.OrderByDescending(x => formatDate(x.ReportedDate)))
                {
                    sb.AppendLine($"                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
                    sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">{variation.Value ?? "-"} [PAN]</span>");
                    sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">{formatDate(variation.ReportedDate)}</span>");
                    sb.AppendLine("                            </div>");
                }
            }
            else
            {
                sb.AppendLine("                            <div style=\"display: table; width: 100%; background: #f3f4f6; padding: 2px 8px; margin-bottom: 1px;\">");
                sb.AppendLine($"                                <span style=\"display: table-cell; width: 70%; font-size: 12px\">{GetPan()} [PAN]</span>");
                sb.AppendLine($"                                <span style=\"display: table-cell; width: 30%; text-align: right; font-size: 12px; white-space: nowrap;\">{formatDate(header?.DateOfIssue)}</span>");
                sb.AppendLine("                            </div>");
            }
            sb.AppendLine("                        </div>");
            sb.AppendLine("                    </div>");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");

            // ACCOUNT SUMMARY
            sb.AppendLine("        <div style=\"padding: 0px 24px; padding-top: 20px\">");
            sb.AppendLine("            <span style=\"color: #9ca3af; font-size: 9px; display: block; text-align: right; margin-bottom: -10px;\">");
            sb.AppendLine("                Tip: All amounts are in INR.");
            sb.AppendLine("            </span>");
            sb.AppendLine("            <h2 style=\"margin-bottom: 0; min-height: 15px; color: #ffffff; font-weight: 600; padding-left: 12px;  font-size: 15px; background-color: #08395d; opacity: 0.9; padding: 6px 8px;\">");
            sb.AppendLine("                Account Summary");
            sb.AppendLine("            </h2>");
            sb.AppendLine("            <span style=\"color: #9ca3af; font-size: 9px; display: block; margin-bottom: 12px; text-align: right;\">");
            sb.AppendLine("                Tip: These are applicant's personal information variations as contributed by various financial institutions.");
            sb.AppendLine("            </span>");
            sb.AppendLine("            <table style=\"width: 100%; border: 2px solid #bfdbfe; border-bottom: none; border-collapse: collapse;\">");
            sb.AppendLine("                <thead style=\"color: #083a5dc4; text-align: left; font-size: 13px; font-weight: 400;\">");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Type</th>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Number of Account(s)</th>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Active Account(s)</th>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Overdue Account(s)</th>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Current Balance</th>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Amt Disbd/High Credit</th>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </thead>");
            sb.AppendLine("                <tbody style=\"line-height: 1\">");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Primary Match</td>");
            sb.AppendLine($"                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">{summary?.PrimaryAccountsSummary?.NumberOfAccounts ?? "0"}</td>");
            sb.AppendLine($"                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">{summary?.PrimaryAccountsSummary?.ActiveAccounts ?? "0"}</td>");
            sb.AppendLine($"                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">{summary?.PrimaryAccountsSummary?.OverdueAccounts ?? "0"}</td>");
            sb.AppendLine($"                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">{formatCurrency(summary?.PrimaryAccountsSummary?.TotalCurrentBalance ?? "0")}</td>");
            sb.AppendLine($"                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">{formatCurrency(summary?.PrimaryAccountsSummary?.TotalDisbursedAmount ?? "0")}</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr style=\"background-color: #8665ff30;\">");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700;\">Total</td>");
            sb.AppendLine($"                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">{summary?.PrimaryAccountsSummary?.NumberOfAccounts ?? "0"}</td>");
            sb.AppendLine($"                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">{summary?.PrimaryAccountsSummary?.ActiveAccounts ?? "0"}</td>");
            sb.AppendLine($"                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">{summary?.PrimaryAccountsSummary?.OverdueAccounts ?? "0"}</td>");
            sb.AppendLine($"                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">{formatCurrency(summary?.PrimaryAccountsSummary?.TotalCurrentBalance ?? "0")}</td>");
            sb.AppendLine($"                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">{formatCurrency(summary?.PrimaryAccountsSummary?.TotalDisbursedAmount ?? "0")}</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </tbody>");
            sb.AppendLine("            </table>");
            sb.AppendLine("            <div style=\"border: 2px solid #bfdbfe; border-top: none; padding: 0px 6px; display: flex; justify-content: space-between; margin: 0px; font-size: 12px;\">");
            sb.AppendLine($"                <p style=\"margin-top: 8px; color: #083a5dc4; font-weight: 700\">Inquiries in last 24 Months:<span style=\"color: #000\"> 0</span></p>");
            sb.AppendLine($"                <p style=\"margin-top: 8px; color: #083a5dc4; font-weight: 700\">New Account(s) in last 6 Months:<span style=\"color: #000\"> 0</span></p>");
            sb.AppendLine($"                <p style=\"margin-top: 8px; color: #083a5dc4; font-weight: 700\">New Delinquent Account(s) in last 6 Months:<span style=\"color: #000\"> 0</span></p>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");

            // ACCOUNT INFORMATION WITH PAYMENT HISTORY (LOAN SECTION)
            if (tradelines != null && tradelines.Count > 0)
            {
                sb.AppendLine("        <div style=\"padding: 20px 12px\">");
                sb.AppendLine("            <h2 style=\"margin-bottom: 18px; min-height: 15px; color: #ffffff; font-weight: 600; padding-left: 12px;  font-size: 15px; background-color: #08395d; opacity: 0.9; padding: 6px 8px;\">");
                sb.AppendLine("                Account Information");
                sb.AppendLine("            </h2>");

                int accountCounter = 1;
                foreach (var tradeline in tradelines)
                {
                    string statusColor = tradeline.AccountStatus?.ToUpper() == "ACTIVE" ? "#E1F0BE" :
                                        tradeline.AccountStatus?.ToUpper() == "CLOSED" ? "#FEF3C7" : "#FEE2E2";
                    string statusTextColor = tradeline.AccountStatus?.ToUpper() == "ACTIVE" ? "#2f5f00" :
                                           tradeline.AccountStatus?.ToUpper() == "CLOSED" ? "#92400e" : "#991b1b";

                    sb.AppendLine("            <div style=\"margin-bottom: 16px;\">");
                    sb.AppendLine("                <div style=\"background-color: #eff6ff; font-weight: 600; display: flex; align-items: center; gap: 8px;\">");
                    sb.AppendLine($"                    <span style=\"background-color: #083a5dc4; color: #ffffff; font-size: 12px; width: 25px; height: 25px; display: flex; align-items: center; justify-content: center;\">{accountCounter}</span>");
                    sb.AppendLine("                    <div style=\"display: flex; justify-content: space-between; width: 100%; flex-wrap: wrap; gap: 12px; font-size: 12px;\">");
                    sb.AppendLine($"                        <p style=\"margin: 0;\"><span style=\"color: #083a5dc4; font-weight: 700;\">Account Type: </span><span style=\"color: #b91c1c; text-transform: uppercase; font-weight: 700; font-size: 13px;\">{tradeline.AccountType ?? "Personal Loan"}</span></p>");
                    sb.AppendLine($"                        <p style=\"margin: 0;\"><span style=\"color: #083a5dc4; font-weight: 700;\">Credit Grantor:</span> {tradeline.CreditGrantor ?? "-"}</p>");
                    sb.AppendLine($"                        <p style=\"margin: 0;\"><span style=\"color: #083a5dc4; font-weight: 700;\">Account #:</span> {tradeline.AccountNumber ?? "-"}</p>");
                    sb.AppendLine($"                        <p style=\"margin: 0;\"><span style=\"color: #083a5dc4; font-weight: 700;\">Info. as of:</span> {formatDate(tradeline.ReportedDate)}</p>");
                    sb.AppendLine("                </div>");
                    sb.AppendLine("                </div>");
                    sb.AppendLine("                <div style=\"position: relative; padding-left: 18px; margin-top: 8px;\">");
                    sb.AppendLine("                    <div style=\"position: relative; padding-left: 30px; margin-top: 12px;\">");
                    sb.AppendLine($"                        <div style=\"position: absolute; left: -45px; top: 24px; transform: rotate(-90deg); background-color: {statusColor}; padding: 6px 14px; font-weight: 600; font-size: 14px; color: {statusTextColor}; text-align: center; white-space: nowrap;\">{tradeline.AccountStatus?.ToUpper() ?? "CLOSED"}</div>");
                    sb.AppendLine("                    </div>");
                    sb.AppendLine("                    <div style=\"display: grid; grid-template-columns: repeat(3, 1fr); padding: 12px 20px; font-size: 12px; font-weight: 500; gap: 4px;\">");
                    sb.AppendLine($"                        <p style=\"display: grid; grid-template-columns: 140px 1fr; font-weight: 600; margin: 0;\"><span style=\"color: #083a5dc4;\">Ownership:</span> <span style=\"font-weight: 600;\">{tradeline.OwnershipType ?? "Self"}</span></p>");
                    sb.AppendLine($"                        <p style=\"display: grid; grid-template-columns: 140px 1fr; font-weight: 600; margin: 0;\"><span style=\"color: #083a5dc4;\">Disbursed Amt:</span> <span style=\"font-weight: 600;\">{formatCurrency(tradeline.DisbursedAmount)}</span></p>");
                    sb.AppendLine($"                        <p style=\"display: grid; grid-template-columns: 140px 1fr; font-weight: 600; margin: 0;\"><span style=\"color: #083a5dc4;\">Disbursed Date:</span> <span style=\"font-weight: 600;\">{formatDate(tradeline.DisbursedDate)}</span></p>");
                    sb.AppendLine($"                        <p style=\"display: grid; grid-template-columns: 140px 1fr; font-weight: 600; margin: 0;\"><span style=\"color: #083a5dc4;\">Credit Limit:</span> <span style=\"font-weight: 600;\">₹{formatCurrency(tradeline.CreditLimit)}</span></p>");
                    sb.AppendLine($"                        <p style=\"display: grid; grid-template-columns: 140px 1fr; font-weight: 600; margin: 0;\"><span style=\"color: #083a5dc4;\">Last Payment Date:</span> <span style=\"font-weight: 600;\">{formatDate(tradeline.LastPaymentDate)}</span></p>");
                    sb.AppendLine($"                        <p style=\"display: grid; grid-template-columns: 140px 1fr; font-weight: 600; margin: 0;\"><span style=\"color: #083a5dc4;\">Current Balance:</span> <span style=\"font-weight: 600;\">₹{formatCurrency(tradeline.CurrentBalance)}</span></p>");
                    sb.AppendLine($"                        <p style=\"display: grid; grid-template-columns: 140px 1fr; font-weight: 600; margin: 0;\"><span style=\"color: #083a5dc4;\">Closed Date:</span> <span style=\"font-weight: 600;\">{formatDate(tradeline.ClosedDate)}</span></p>");
                    sb.AppendLine($"                        <p style=\"display: grid; grid-template-columns: 140px 1fr; font-weight: 600; margin: 0;\"><span style=\"color: #083a5dc4;\">Last Paid Amt:</span> <span style=\"font-weight: 600;\">₹{formatCurrency(tradeline.LastPaidAmount ?? tradeline.ActualPayment)}</span></p>");
                    sb.AppendLine($"                        <p style=\"display: grid; grid-template-columns: 140px 1fr; font-weight: 600; margin: 0;\"><span style=\"color: #083a5dc4;\">InstlAmt/Freq:</span> <span style=\"font-weight: 600;\">₹{formatCurrency(tradeline.InstallmentAmount)}</span></p>");
                    sb.AppendLine($"                        <p style=\"display: grid; grid-template-columns: 140px 1fr; font-weight: 600; margin: 0;\"><span style=\"color: #083a5dc4;\">Tenure (Month):</span> <span style=\"font-weight: 600;\">{tradeline.RepaymentTenure ?? "-"}</span></p>");
                    sb.AppendLine($"                        <p style=\"display: grid; grid-template-columns: 140px 1fr; font-weight: 600; margin: 0;\"><span style=\"color: #083a5dc4;\">Overdue Amt:</span> <span style=\"font-weight: 600;\">₹{formatCurrency(tradeline.OverdueAmount)}</span></p>");
                    sb.AppendLine("                    </div>");
                    sb.AppendLine("                </div>");

                    // PAYMENT HISTORY TABLE
                    var paymentHistory = tradeline.History?.FirstOrDefault(h => h.Name == "COMBINED-PAYMENT-HISTORY");
                    if (paymentHistory != null && !string.IsNullOrEmpty(paymentHistory.Values))
                    {
                        string[] paymentEntries = paymentHistory.Values.Split("|");
                        string[] paymentDates = paymentHistory.Dates?.Split("|") ?? new string[0];

                        sb.AppendLine("                <div style=\"width: 100%; margin-top: 12px;\">");
                        sb.AppendLine("                    <p style=\"color: #083a5dc4; opacity: 0.8; font-weight: 700; font-size: 16px; margin-bottom: 2px;\">");
                        sb.AppendLine("                        Payment History / Asset Classification:");
                        sb.AppendLine("                    </p>");
                        sb.AppendLine("                    <table style=\"width: 100%; border-collapse: collapse;\">");
                        sb.AppendLine("                        <thead style=\"background-color: #dedbfe; opacity: 0.5;\">");
                        sb.AppendLine("                            <tr>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">Year</th>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">January</th>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">February</th>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">March</th>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">April</th>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">May</th>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">June</th>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">July</th>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">August</th>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">September</th>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">October</th>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">November</th>");
                        sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 12px;\">December</th>");
                        sb.AppendLine("                            </tr>");
                        sb.AppendLine("                        </thead>");
                        sb.AppendLine("                        <tbody>");

                        // Group by year
                        var yearData = new Dictionary<string, Dictionary<string, string>>();

                        for (int i = 0; i < Math.Min(paymentDates.Length, paymentEntries.Length); i++)
                        {
                            if (!string.IsNullOrEmpty(paymentDates[i]) && !string.IsNullOrEmpty(paymentEntries[i]))
                            {
                                var parts = paymentDates[i].Split(':');
                                if (parts.Length >= 2)
                                {
                                    string month = parts[0];
                                    string year = parts[1];
                                    string value = paymentEntries[i];

                                    if (!yearData.ContainsKey(year))
                                        yearData[year] = new Dictionary<string, string>();

                                    yearData[year][month] = value;
                                }
                            }
                        }

                        foreach (var year in yearData.Keys.OrderByDescending(y => y))
                        {
                            sb.AppendLine(" <tr style=\"font-weight: 600; font-size: 12px;\">");
                            sb.AppendLine($"<td style=\"padding: 6px; border: 1px solid #bfdbfe; font-size: 14px;\">{year}</td>");

                            string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                            foreach (var month in months)
                            {
                                string cellValue = yearData[year].ContainsKey(month) ? yearData[year][month] : "-";
                                sb.AppendLine($"<td style=\"padding: 6px; border: 1px solid #bfdbfe;\">{cellValue}</td>");
                            }

                            sb.AppendLine("</tr>");
                        }

                        sb.AppendLine("                        </tbody>");
                        sb.AppendLine("                    </table>");
                        sb.AppendLine("                </div>");
                    }

                    // COLLATERAL/SECURITY DETAILS
                    sb.AppendLine("                <div style=\"width: 100%; margin-top: 12px;\">");
                    sb.AppendLine("                    <p style=\"color: #083a5dc4; opacity: 0.8; font-weight: 700; font-size: 14px; margin-bottom: 2px;\">");
                    sb.AppendLine("                        Collateral/Security Details:");
                    sb.AppendLine("                    </p>");
                    sb.AppendLine("                    <table style=\"width: 100%; border-collapse: collapse; border: 1px solid #d1d5db;\">");
                    sb.AppendLine("                        <thead style=\"background-color: #dedbfe; opacity: 0.5;\">");
                    sb.AppendLine("                            <tr>");
                    sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #d1d5db; font-size: 12px;\">Security Type</th>");
                    sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #d1d5db; font-size: 12px;\">Type of Charge</th>");
                    sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #d1d5db; font-size: 12px;\">Security Value</th>");
                    sb.AppendLine("                                <th style=\"padding: 6px; border: 1px solid #d1d5db; font-size: 12px;\">Date Of Value</th>");
                    sb.AppendLine("                            </tr>");
                    sb.AppendLine("                        </thead>");
                    sb.AppendLine("                        <tbody style=\"text-align: center;\">");

                    var securityDetails = tradeline.SecurityDetails;
                    if (securityDetails?.Any() == true)
                    {
                        foreach (var security in securityDetails)
                        {
                            sb.AppendLine("                            <tr>");
                            sb.AppendLine($"                                <td style=\"padding: 6px; border: 1px solid #d1d5db; font-size: 12px; font-weight: 700;\">{security.SecurityType ?? "-"}</td>");
                            sb.AppendLine($"                                <td style=\"padding: 6px; border: 1px solid #d1d5db; font-size: 12px; font-weight: 700;\">{security.SecurityCharge ?? "-"}</td>");
                            sb.AppendLine($"                                <td style=\"padding: 6px; border: 1px solid #d1d5db; font-size: 12px; font-weight: 700;\">{formatCurrency(security.SecurityValuation)}</td>");
                            sb.AppendLine($"                                <td style=\"padding: 6px; border: 1px solid #d1d5db; font-size: 12px; font-weight: 700;\">{formatDate(security.DateOfValuation)}</td>");
                            sb.AppendLine("                            </tr>");
                        }
                    }
                    else
                    {
                        sb.AppendLine("                            <tr>");
                        sb.AppendLine("                                <td style=\"padding: 6px; border: 1px solid #d1d5db; font-size: 12px; font-weight: 700;\">-</td>");
                        sb.AppendLine("                                <td style=\"padding: 6px; border: 1px solid #d1d5db; font-size: 12px; font-weight: 700;\">-</td>");
                        sb.AppendLine("                                <td style=\"padding: 6px; border: 1px solid #d1d5db; font-size: 12px; font-weight: 700;\">-</td>");
                        sb.AppendLine("                                <td style=\"padding: 6px; border: 1px solid #d1d5db; font-size: 12px; font-weight: 700;\">-</td>");
                        sb.AppendLine("                            </tr>");
                    }

                    sb.AppendLine("                        </tbody>");
                    sb.AppendLine("                    </table>");
                    sb.AppendLine("                </div>");
                    sb.AppendLine("            </div>");

                    accountCounter++;
                }
                sb.AppendLine("        </div>");
            }

            // ADVANCED OVERLAP REPORT HEADER
            sb.AppendLine("        <div style=\"padding: 6px; display: grid; grid-template-columns: 1fr 2fr 1fr; align-items: center; gap: 30px; border-bottom: 1px solid #d1d5db;\">");
            sb.AppendLine("            <div style=\"width: 150px\">");
            sb.AppendLine("                <img src=\"https://www.junooncapital.com/img/crif.png\" style=\"width: 100%\" />");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div style=\"padding-left: 12px; width: fit-content;\">");
            sb.AppendLine("                <h1 style=\"font-size: 18px; font-weight: 700; text-transform: uppercase; color: #083a5dc4; margin: 0;\">");
            sb.AppendLine("                    Advanced Overlap Report");
            sb.AppendLine("                </h1>");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0; width: fit-content; color: #000; font-size: 12px; text-transform: uppercase; float: right;\">");
            sb.AppendLine($"                    For {GetApplicantName().Split(' ')?.FirstOrDefault() ?? "Customer"}");
            sb.AppendLine("                </p>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div style=\"padding-top: 16px; padding-bottom: 16px; display: flex; flex-direction: column; gap: 6px; min-width: 260px; font-size: 12px;\">");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">CHM Ref #:</span> {header?.ReportId ?? "-"}</p>");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">Prepared For:</span> JUNOON CAPITAL SERVICES PRIVATE LIMITED</p>");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">Date of Request:</span> {formatDate(header?.DateOfRequest)}</p>");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">Date of Issue:</span> {formatDate(header?.DateOfIssue)}</p>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");

            // SUMMARY SECTION
            sb.AppendLine("        <div style=\"padding: 0 24px; padding-top: 20px; padding-bottom: 0\">");
            sb.AppendLine("            <span style=\"color: #9ca3af; font-size: 9px; display: block; text-align: right; margin-bottom: -10px;\">");
            sb.AppendLine("                Tip: All amounts are in INR");
            sb.AppendLine("            </span>");
            sb.AppendLine("            <h2 style=\"margin-bottom: 0; min-height: 15px; color: #ffffff; font-weight: 600; padding-left: 12px;  font-size: 15px; background-color: #08395d; opacity: 0.9; padding: 6px;\">");
            sb.AppendLine("                Summary");
            sb.AppendLine("            </h2>");
            sb.AppendLine("            <span style=\"color: #9ca3af; font-size: 9px; display: block; margin-bottom: 12px; text-align: right;\">");
            sb.AppendLine("                Tip: Current Balance, Disbursed Amount & Instalment Amount is considered ONLY for ACTIVE account");
            sb.AppendLine("            </span>");
            sb.AppendLine("            <table style=\"width: 100%; border: 2px solid #bfdbfe; border-collapse: collapse; font-size: 12px;\">");
            sb.AppendLine("                <thead style=\"border: 2px solid #bfdbfe\">");
            sb.AppendLine("                    <tr style=\"color: #083a5dc4; font-weight: 600; text-align: left; line-height: 1.4;\">");
            sb.AppendLine("                        <th rowspan=\"2\" style=\"padding: 4px; vertical-align: middle\">Type</th>");
            sb.AppendLine("                        <th colspan=\"2\" style=\"padding: 4px; text-align: center\">Association</th>");
            sb.AppendLine("                        <th colspan=\"3\" style=\"padding: 4px; text-align: center\">Account Summary</th>");
            sb.AppendLine("                        <th colspan=\"2\" style=\"padding: 4px; text-align: center\">Disbursed Amount</th>");
            sb.AppendLine("                        <th colspan=\"2\" style=\"padding: 4px; text-align: center\">Installment Amount</th>");
            sb.AppendLine("                        <th colspan=\"2\" style=\"padding: 4px; text-align: center\">Current Balance</th>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr style=\"font-weight: 600; text-align: center; color: #083a5dc4;\">");
            sb.AppendLine("                        <th style=\"padding: 4px\">Own</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Others</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Total</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Active</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Overdue</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Own</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Others</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Own</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Others</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Own</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Others</th>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </thead>");
            sb.AppendLine("                <tbody>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"padding: 4px; font-weight: 600\">Primary Match</td>");
            sb.AppendLine($"                        <td style=\"padding: 4px; text-align: center\">{summary?.PrimaryAccountsSummary?.NumberOfAccounts ?? "0"}</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">0</td>");
            sb.AppendLine($"                        <td style=\"padding: 4px; text-align: center\">{summary?.PrimaryAccountsSummary?.NumberOfAccounts ?? "0"}</td>");
            sb.AppendLine($"                        <td style=\"padding: 4px; text-align: center\">{summary?.PrimaryAccountsSummary?.ActiveAccounts ?? "0"}</td>");
            sb.AppendLine($"                        <td style=\"padding: 4px; text-align: center\">{summary?.PrimaryAccountsSummary?.OverdueAccounts ?? "0"}</td>");
            sb.AppendLine($"                        <td style=\"padding: 4px; text-align: center\">{formatCurrency(summary?.PrimaryAccountsSummary?.TotalDisbursedAmount ?? "0")}</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine($"                        <td style=\"padding: 4px; text-align: center\">{formatCurrency(summary?.PrimaryAccountsSummary?.TotalCurrentBalance ?? "0")}</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr style=\"font-weight: 600\">");
            sb.AppendLine("                        <td style=\"padding: 4px\">Secondary Match</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">0</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">0</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">0</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">0</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">0</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </tbody>");
            sb.AppendLine("            </table>");
            sb.AppendLine("        </div>");

            // ACCOUNT DETAILS - PRIMARY SECTION HEADER
            sb.AppendLine("        <div style=\"padding: 0 24px; padding-top: 20px; padding-bottom: 0\">");
            sb.AppendLine("            <h2 style=\"margin-bottom: 0; min-height: 15px; color: #ffffff; font-weight: 600; padding-left: 12px;  font-size: 15px; background-color: #08395d; opacity: 0.9; padding: 6px 8px;\">");
            sb.AppendLine("                Account Details - Primary");
            sb.AppendLine("            </h2>");
            sb.AppendLine("        </div>");

            // GROUP DETAILS HEADER
            sb.AppendLine("        <div style=\"padding: 6px; display: grid; grid-template-columns: 1fr 2fr 1fr; align-items: center; gap: 30px; border-bottom: 1px solid #d1d5db;\">");
            sb.AppendLine("            <div style=\"width: 150px\">");
            sb.AppendLine("                <img src=\"https://www.junooncapital.com/img/crif.png\" style=\"width: 100%\" />");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div style=\"padding-left: 12px;\">");
            sb.AppendLine("                <h1 style=\"font-size: 18px; font-weight: 700; text-transform: uppercase; color: #083a5dc4; margin: 0;\">");
            sb.AppendLine("                    GROUP DETAILS");
            sb.AppendLine("                </h1>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div style=\"padding-top: 16px; padding-bottom: 16px; display: flex; flex-direction: column; gap: 6px; min-width: 260px; font-size: 12px;\">");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">CHM Ref #:</span> {header?.ReportId ?? "-"}</p>");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">Prepared For:</span> JUNOON CAPITAL SERVICES PRIVATE LIMITED</p>");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">Date of Request:</span> {formatDate(header?.DateOfRequest)}</p>");
            sb.AppendLine($"                <p style=\"font-weight: 700; margin: 0\"><span style=\"color: #083a5dc4\">Date of Issue:</span> {formatDate(header?.DateOfIssue)}</p>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");

            // GROUP SUMMARY SECTION
            sb.AppendLine("        <div style=\"padding: 20px 12px\">");
            sb.AppendLine("            <span style=\"color: #9ca3af; font-size: 9px; display: block; text-align: right;\">");
            sb.AppendLine("                Tip: All amounts are in INR");
            sb.AppendLine("            </span>");
            sb.AppendLine("            <h2 style=\"margin-bottom: 0; min-height: 15px; color: #ffffff; font-weight: 600; padding-left: 12px;  font-size: 15px; background-color: #08395d; opacity: 0.9; padding-top: 0; padding: 6px 8px;\">");
            sb.AppendLine("                Summary");
            sb.AppendLine("            </h2>");
            sb.AppendLine("            <span style=\"color: #9ca3af; font-size: 9px; display: block; margin-bottom: 12px; text-align: right;\">");
            sb.AppendLine("                Tip: Current Balance, Disbursed Amount & Instalment Amount is considered ONLY for ACTIVE account");
            sb.AppendLine("            </span>");
            sb.AppendLine("            <table style=\"width: 100%; font-size: 12px; border-collapse: collapse; border: 2px solid #bfdbfe;\">");
            sb.AppendLine("                <thead style=\"border: 2px solid #bfdbfe\">");
            sb.AppendLine("                    <tr style=\"color: #083a5dc4; font-weight: 600; text-align: left\">");
            sb.AppendLine("                        <th rowspan=\"2\" style=\"padding: 4px; vertical-align: middle\">Type</th>");
            sb.AppendLine("                        <th colspan=\"2\" style=\"padding: 4px; text-align: center\">Association</th>");
            sb.AppendLine("                        <th colspan=\"3\" style=\"padding: 4px; text-align: center\">Account Summary</th>");
            sb.AppendLine("                        <th colspan=\"2\" style=\"padding: 4px; text-align: center\">Disbursed Amount</th>");
            sb.AppendLine("                        <th colspan=\"2\" style=\"padding: 4px; text-align: center\">Installment Amount</th>");
            sb.AppendLine("                        <th colspan=\"2\" style=\"padding: 4px; text-align: center\">Current Balance</th>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr style=\"text-align: center; font-weight: 600\">");
            sb.AppendLine("                        <th style=\"padding: 4px\">Own</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Others</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Total</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Active</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Overdue</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Own</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Others</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Own</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Others</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Own</th>");
            sb.AppendLine("                        <th style=\"padding: 4px\">Others</th>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </thead>");
            sb.AppendLine("                <tbody>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"padding: 4px; font-weight: 600\">Primary Match</td>");
            sb.AppendLine($"                        <td style=\"padding: 4px; text-align: center\">{summary?.PrimaryAccountsSummary?.NumberOfAccounts ?? "0"}</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">0</td>");
            sb.AppendLine($"                        <td style=\"padding: 4px; text-align: center\">{summary?.PrimaryAccountsSummary?.NumberOfAccounts ?? "0"}</td>");
            sb.AppendLine($"                        <td style=\"padding: 4px; text-align: center\">{summary?.PrimaryAccountsSummary?.ActiveAccounts ?? "0"}</td>");
            sb.AppendLine($"                        <td style=\"padding: 4px; text-align: center\">{summary?.PrimaryAccountsSummary?.OverdueAccounts ?? "0"}</td>");
            sb.AppendLine($"                        <td style=\"padding: 4px; text-align: center\">{formatCurrency(summary?.PrimaryAccountsSummary?.TotalDisbursedAmount ?? "0")}</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine($"                        <td style=\"padding: 4px; text-align: center\">{formatCurrency(summary?.PrimaryAccountsSummary?.TotalCurrentBalance ?? "0")}</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr style=\"font-weight: 600\">");
            sb.AppendLine("                        <td style=\"padding: 4px\">Secondary Match</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">0</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">0</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">0</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">0</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">0</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                        <td style=\"padding: 4px; text-align: center\">-</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </tbody>");
            sb.AppendLine("            </table>");
            sb.AppendLine("        </div>");

            // ACCOUNT DETAILS - PRIMARY SECTION HEADER (AGAIN)
            sb.AppendLine("        <div style=\"padding: 20px 12px\">");
            sb.AppendLine("            <h2 style=\"margin-bottom: 0; min-height: 15px; color: #ffffff; font-weight: 600; padding-left: 12px;  font-size: 15px; background-color: #08395d; opacity: 0.9; padding-top: 0; padding: 6px 8px;\">");
            sb.AppendLine("                Account Details - Primary");
            sb.AppendLine("            </h2>");
            sb.AppendLine("        </div>");

            // INQUIRIES SECTION (empty in new model)
            sb.AppendLine("        <div style=\"padding: 20px 12px\">");
            sb.AppendLine("            <h2 style=\"margin-bottom: 18px; min-height: 15px; color: #ffffff; font-weight: 600; padding-left: 12px;  font-size: 15px; background-color: #08395d; opacity: 0.9; padding-top: 0; padding: 6px 8px;\">");
            sb.AppendLine("                Inquiries (reported Last 24 Months)");
            sb.AppendLine("            </h2>");
            sb.AppendLine("            <table style=\"width: 100%; border-collapse: collapse\">");
            sb.AppendLine("                <thead style=\"color: #083a5dc4; text-align: left\">");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Credit Grantor</th>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Date of Inquiry</th>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Purpose</th>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Amount</th>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Remark</th>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </thead>");
            sb.AppendLine("                <tbody style=\"line-height: 1\">");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">-</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">-</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">-</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">-</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">-</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </tbody>");
            sb.AppendLine("            </table>");
            sb.AppendLine("            <p style=\"text-align: center; font-size: 15px; font-weight: 700; color: #083a5dc4; opacity: 0.8; margin-top: 20px;\">");
            sb.AppendLine("                - END OF INDIVIDUAL REPORT -");
            sb.AppendLine("            </p>");
            sb.AppendLine("        </div>");

            // APPENDIX SECTION
            sb.AppendLine("        <div style=\"padding: 20px 12px\">");
            sb.AppendLine("            <h2 style=\"margin-bottom: 18px; min-height: 15px; color: #ffffff; font-weight: 600; padding-left: 12px;  font-size: 15px; background-color: #08395d; opacity: 0.9; padding-top: 0; padding: 6px 8px;\">");
            sb.AppendLine("                Appendix");
            sb.AppendLine("            </h2>");
            sb.AppendLine("            <table style=\"width: 100%; border-collapse: collapse\">");
            sb.AppendLine("                <thead style=\"color: #083a5dc4; text-align: left\">");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Section</th>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Code</th>");
            sb.AppendLine("                        <th style=\"padding: 6px; border-bottom: 1px solid #bfdbfe\">Description</th>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </thead>");
            sb.AppendLine("                <tbody style=\"line-height: 1\">");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Account Summary</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Number of Delinquent Accounts</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Indicates number of accounts that the applicant has defaulted on within the last 6 months</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Account Information – Credit Grantor</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">XXXX</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Name of grantor undisclosed as credit grantor is different from inquiring institution</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Account Information – Account #</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">xxxx</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Account Number undisclosed as credit grantor is different from inquiring institution</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Payment History / Asset Classification</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">XXX</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Data not reported by institution</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Payment History / Asset Classification</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">-</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Not applicable</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Payment History / Asset Classification</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">STD</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Account Reported as STANDARD Asset</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Payment History / Asset Classification</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">SUB</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Account Reported as SUB-STANDARD Asset</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Payment History / Asset Classification</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">DBT</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Account Reported as DOUBTFUL Asset</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Payment History / Asset Classification</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">LOS</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Account Reported as LOSS Asset</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Payment History / Asset Classification</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">SMA</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Account Reported as SPECIAL MENTION</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Account Information – Account #</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">CI-Ceased/Membership Terminated</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Credit Institution has Ceased to Operate or Membership Terminated</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">Account Information – Account #</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">License Cancelled Entities</td>");
            sb.AppendLine("                        <td style=\"font-size: 10px; padding: 6px; font-weight: 700\">License of the credit institution cancelled by RBI</td>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </tbody>");
            sb.AppendLine("            </table>");
            sb.AppendLine("        </div>");

            // DISCLAIMER
            sb.AppendLine("        <div style=\"padding: 40px 24px 0 24px; font-size: 10px; line-height: 1.6; color: #9ca3af; font-weight: 500;\">");
            sb.AppendLine("            <p style=\"margin: 0\">");
            sb.AppendLine("                <b>Disclaimer:</b>");
            sb.AppendLine("                This document contains proprietary information to CRIF High Mark and may not be used or disclosed to others, except with the written permission of CRIF High Mark. Any paper copy of this document will be considered uncontrolled. If you are not the intended recipient, you are not authorized to read, print, retain, copy, disseminate, distribute, or use this information or any part thereof.");
            sb.AppendLine("            </p>");
            sb.AppendLine("            <div style=\"margin-top: 8px; display: flex; justify-content: space-between; text-align: center; font-size: 10px; font-weight: 600; color: #9ca3af;\">");
            sb.AppendLine("                <p style=\"margin: 0\">© Copyright 2025. All rights reserved</p>");
            sb.AppendLine("                <p style=\"margin: 0\">CRIF High Mark Credit Information Services Pvt. Ltd</p>");
            sb.AppendLine("                <p style=\"margin: 0\">Confidential</p>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");

            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }
}
