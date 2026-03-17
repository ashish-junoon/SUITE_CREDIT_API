//using CIC.Helper;
using CIC.Experian;
using CIC.Model.Experian.ResponseNew;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace JC.Experian
{
    public class ExperianHtmlBuilder
    {
        public static string BuildHtml(CIC.Model.Experian.ResponseNew.ExperianReturnResponseV1 response)
        {
            Console.WriteLine(JsonConvert.SerializeObject(response));
            var sb = new StringBuilder();

            // Get data from response
            var currentApplicant = response?.INProfileResponse?.Current_Application?.Current_Application_Details?.Current_Applicant_Details;
            var caisSummary = response?.INProfileResponse?.CAIS_Account?.CAIS_Summary;
            var caisDetails = response?.INProfileResponse?.CAIS_Account?.CAIS_Account_DETAILS;
            var capsSummary = response?.INProfileResponse?.CAPS?.CAPS_Summary;
            var capsDetails = response?.INProfileResponse?.CAPS?.CAPS_Application_Details;
            var nonCapsSummary = response?.INProfileResponse?.NonCreditCAPS?.NonCreditCAPS_Summary;
            var nonCapsDetails = response?.INProfileResponse?.NonCreditCAPS?.CAPS_Application_Details;
            var score = response?.INProfileResponse?.SCORE;
            var matchResult = response?.INProfileResponse?.Match_result;
            var totalCapsSummary = response?.INProfileResponse?.TotalCAPS_Summary;

            // Helper function to get full name
            string GetFullName(CurrentApplicantDetails? details)
            {
                if (details == null) return "—";

                var nameParts = new List<string>();
                if (!string.IsNullOrEmpty(details.First_Name)) nameParts.Add(details.First_Name);
                if (!string.IsNullOrEmpty(details.Middle_Name1)) nameParts.Add(details.Middle_Name1);
                if (!string.IsNullOrEmpty(details.Middle_Name2)) nameParts.Add(details.Middle_Name2);
                if (!string.IsNullOrEmpty(details.Middle_Name3)) nameParts.Add(details.Middle_Name3);
                if (!string.IsNullOrEmpty(details.Last_Name)) nameParts.Add(details.Last_Name);

                return nameParts.Any() ? string.Join(" ", nameParts) : "—";
            }

            // Helper function to get CAIS holder full name from first item in list
            string GetCAISHolderFullName(List<CAISHolderDetails>? detailsList)
            {
                if (detailsList == null || !detailsList.Any()) return "—";

                var details = detailsList.FirstOrDefault();
                if (details == null) return "—";

                var nameParts = new List<string>();
                if (!string.IsNullOrEmpty(details.First_Name_Non_Normalized)) nameParts.Add(details.First_Name_Non_Normalized);
                if (!string.IsNullOrEmpty(details.Middle_Name_1_Non_Normalized)) nameParts.Add(details.Middle_Name_1_Non_Normalized);
                if (!string.IsNullOrEmpty(details.Middle_Name_2_Non_Normalized)) nameParts.Add(details.Middle_Name_2_Non_Normalized);
                if (!string.IsNullOrEmpty(details.Middle_Name_3_Non_Normalized)) nameParts.Add(details.Middle_Name_3_Non_Normalized);
                if (!string.IsNullOrEmpty(details.Surname_Non_Normalized)) nameParts.Add(details.Surname_Non_Normalized);

                return nameParts.Any() ? string.Join(" ", nameParts) : "—";
            }

            // Helper function to get CAIS address from first item in list
            string GetCAISAddress(List<CAISHolderAddressDetails>? addressList)
            {
                if (addressList == null || !addressList.Any()) return "—";

                var details = addressList.FirstOrDefault();
                if (details == null) return "—";

                var addressParts = new List<string>();
                if (!string.IsNullOrEmpty(details.First_Line_Of_Address_non_normalized)) addressParts.Add(details.First_Line_Of_Address_non_normalized);
                if (!string.IsNullOrEmpty(details.Second_Line_Of_Address_non_normalized)) addressParts.Add(details.Second_Line_Of_Address_non_normalized);
                if (!string.IsNullOrEmpty(details.Third_Line_Of_Address_non_normalized)) addressParts.Add(details.Third_Line_Of_Address_non_normalized);
                if (!string.IsNullOrEmpty(details.City_non_normalized)) addressParts.Add(details.City_non_normalized);
                if (!string.IsNullOrEmpty(details.Fifth_Line_Of_Address_non_normalized)) addressParts.Add(details.Fifth_Line_Of_Address_non_normalized);
                if (!string.IsNullOrEmpty(details.State_non_normalized)) addressParts.Add(details.State_non_normalized);
                if (!string.IsNullOrEmpty(details.ZIP_Postal_Code_non_normalized)) addressParts.Add(details.ZIP_Postal_Code_non_normalized);

                return addressParts.Any() ? string.Join(", ", addressParts.Where(p => !string.IsNullOrEmpty(p))) : "—";
            }

            // Helper function to get CAIS phone details from first item in list
            string GetCAISPhoneDetailsValue(List<CAISHolderPhoneDetails>? phoneList, Func<CAISHolderPhoneDetails, string?> selector)
            {
                if (phoneList == null || !phoneList.Any()) return "—";

                var details = phoneList.FirstOrDefault();
                if (details == null) return "—";

                var value = selector(details);
                return string.IsNullOrEmpty(value) ? "—" : value;
            }

            // Helper function to get CAIS ID details from first item in list
            string GetCAISIDDetailsValue(List<CAISHolderIDDetails>? idList, Func<CAISHolderIDDetails, string?> selector)
            {
                if (idList == null || !idList.Any()) return "—";

                var details = idList.FirstOrDefault();
                if (details == null) return "—";

                var value = selector(details);
                return string.IsNullOrEmpty(value) ? "—" : value;
            }

            // Helper function to get CAIS holder details from first item in list
            string GetCAISHolderDetailsValue(List<CAISHolderDetails>? detailsList, Func<CAISHolderDetails, string?> selector)
            {
                if (detailsList == null || !detailsList.Any()) return "—";

                var details = detailsList.FirstOrDefault();
                if (details == null) return "—";

                var value = selector(details);
                return string.IsNullOrEmpty(value) ? "—" : value;
            }

            // Helper function to get display value
            string GetDisplayValue(string? value) => string.IsNullOrEmpty(value) ? "—" : value;

            // Helper function to format date
            string FormatDate(string? dateString)
            {
                if (string.IsNullOrEmpty(dateString)) return "—";

                if (DateTime.TryParse(dateString, out DateTime date))
                {
                    return date.ToString("yyyy-MM-dd");
                }
                if (dateString.Length == 8 && dateString.All(char.IsDigit))
                {
                    try
                    {
                        int year = int.Parse(dateString.Substring(0, 4));
                        int month = int.Parse(dateString.Substring(4, 2));
                        int day = int.Parse(dateString.Substring(6, 2));

                        date = new DateTime(year, month, day);
                        return date.ToString("yyyy-MM-dd");
                    }
                    catch
                    {
                        return dateString;
                    }
                }
                return dateString;
            }
            int bureauScore = 0;
            int.TryParse(score?.BureauScore.ToString(), out bureauScore);

            bureauScore = Math.Max(300, Math.Min(900, bureauScore));

            const double minAngle = -80;   // left limit
            const double maxAngle = 80;    // right limit

            double angleDeg =
                minAngle +
                ((bureauScore - 300) / 600.0) * (maxAngle - minAngle);
            double angleRad = angleDeg * Math.PI / 180;

            double markerRotate = angleDeg;

            string rotateCss = markerRotate.ToString("0.##", CultureInfo.InvariantCulture);

            double radius = 78;  // spacing fix
            double centerX = 100;
            double centerY = 85;

            double x = centerX + radius * Math.Cos(angleRad);
            double y = centerY + radius * Math.Sin(angleRad);

            string markerLeft = x.ToString("0.##", CultureInfo.InvariantCulture);
            string markerTop = y.ToString("0.##", CultureInfo.InvariantCulture);


            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine();
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"UTF-8\">");
            sb.AppendLine("  <title>Experian Report</title>");

            #region style
            sb.AppendLine("  <style>");
            // ... [CSS remains exactly the same as before] ...
            sb.AppendLine("    body {");
            sb.AppendLine("      font-family: 'Arial', sans-serif;");
            sb.AppendLine("      margin: 22px;");
            sb.AppendLine("      padding: 25px 35px;");
            sb.AppendLine("      font-size: 11px;");
            sb.AppendLine("      color: #000;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .header {");
            sb.AppendLine("      display: flex;");
            sb.AppendLine("      justify-content: space-between;");
            sb.AppendLine("      align-items: flex-start;");
            sb.AppendLine("      margin-bottom: 20px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .header img {");
            sb.AppendLine("      height: 35px;");
            sb.AppendLine("      margin-top: 10px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .header-right {");
            sb.AppendLine("      text-align: right;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .header-right h2 {");
            sb.AppendLine("      color: #0071CE;");
            sb.AppendLine("      margin: 0 0 6px;");
            sb.AppendLine("      font-size: 18px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .header-right p {");
            sb.AppendLine("      margin: 0;");
            sb.AppendLine("      margin-bottom: 2px;");
            sb.AppendLine("      font-size: 11px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .section-title {");
            sb.AppendLine("      background: #f0f0f0;");
            sb.AppendLine("      padding: 6px 10px;");
            sb.AppendLine("      font-weight: bold;");
            sb.AppendLine("      font-size: 12px;");
            sb.AppendLine("      border: 1px solid #ccc;");
            sb.AppendLine("      margin: 20px 0 10px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .checkmark {");
            sb.AppendLine("      color: green;");
            sb.AppendLine("      margin-right: 5px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .info-table {");
            sb.AppendLine("      width: 100%;");
            sb.AppendLine("      border-collapse: collapse;");
            sb.AppendLine("      font-size: 11.2px;");
            sb.AppendLine("      margin-bottom: 10px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .info-table td {");
            sb.AppendLine("      padding: 6px 8px;");
            sb.AppendLine("      vertical-align: top;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .info-table .label {");
            sb.AppendLine("      color: #0071CE;");
            sb.AppendLine("      font-weight: normal;");
            sb.AppendLine("      width: 130px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .info-table .value {");
            sb.AppendLine("      font-size: 10.3px;");
            sb.AppendLine("      color: #000;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .score-section {");
            sb.AppendLine("      margin-top: 15px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .score-note {");
            sb.AppendLine("      color: orange;");
            sb.AppendLine("      font-size: 11px;");
            sb.AppendLine("      margin-bottom: 10px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .score-box {");
            sb.AppendLine("      display: flex;");
            sb.AppendLine("      align-items: flex-start;");
            sb.AppendLine("      justify-content: flex-start;");
            sb.AppendLine("      gap: 25px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .score-dial-box {");
            sb.AppendLine("      position: relative;");
            sb.AppendLine("      width: 200px;");
            sb.AppendLine("      height: 130px;");
            sb.AppendLine("      display: flex;");
            sb.AppendLine("      align-items: center;");
            sb.AppendLine("      justify-content: center;");
            sb.AppendLine("      margin: 10px 0px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .score-dial {");
            sb.AppendLine("      width: 150px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .score-dial-box img{");
            sb.AppendLine("      width: 100%;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .score-inside {");
            sb.AppendLine("      position: absolute;");
            sb.AppendLine("      font-size: 22px;");
            sb.AppendLine("      font-weight: bold;");
            sb.AppendLine("      bottom: 22px;");
            sb.AppendLine("      left: 50%;");
            sb.AppendLine("      transform: translateX(-50%);");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .score-marker {");
            sb.AppendLine("      position: absolute;");
            sb.AppendLine("      width: 0;");
            sb.AppendLine("      height: 0;");
            sb.AppendLine("      border-top: 6px solid transparent;");
            sb.AppendLine("      border-bottom: 6px solid transparent;");
            sb.AppendLine("      border-left: 10px solid #000;"); // ▶ triangle
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .score-factors-box {");
            sb.AppendLine("      border: 1px solid #ddd;");
            sb.AppendLine("      padding: 10px 15px;");
            sb.AppendLine("      font-size: 10.5px;");
            sb.AppendLine("      background: #fff;");
            sb.AppendLine("      width: 420px;");
            sb.AppendLine("      box-shadow: 1px 1px 3px rgba(0, 0, 0, 0.03);");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .score-factors-box p {");
            sb.AppendLine("      margin: 4px 0;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .score-factors-box strong {");
            sb.AppendLine("      font-weight: bold;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .report-summary {");
            sb.AppendLine("      display: flex;");
            sb.AppendLine("      margin-top: 8px;");
            sb.AppendLine("      font-size: 10.8px;");
            sb.AppendLine("      /* REMOVE the border-top */");
            sb.AppendLine("      /* border-top: 1px solid #ccc; */");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .summary-col {");
            sb.AppendLine("      flex: 1;");
            sb.AppendLine("      padding: 8px 12px;");
            sb.AppendLine("      border-right: 1px dashed #ccc;");
            sb.AppendLine("      display: flex;");
            sb.AppendLine("      flex-direction: column;");
            sb.AppendLine("      justify-content: space-between;");
            sb.AppendLine("      min-height: 125px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .summary-col:last-child {");
            sb.AppendLine("      border-right: none;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .summary-title {");
            sb.AppendLine("      font-weight: bold;");
            sb.AppendLine("      color: #0b60ba;");
            sb.AppendLine("      /* slightly softer than #0071CE */");
            sb.AppendLine("      margin-bottom: 5px;");
            sb.AppendLine("      font-size: 11px;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .summary-line {");
            sb.AppendLine("      margin-bottom: 4px;");
            sb.AppendLine("      line-height: 1.3;");
            sb.AppendLine("      font-size: 10px;");
            sb.AppendLine("      display: flex;");
            sb.AppendLine("      justify-content: space-between;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .summary-line .label {");
            sb.AppendLine("      color: #0b60ba;");
            sb.AppendLine("      white-space: nowrap;");
            sb.AppendLine("      /* 🔥 Prevent label text from wrapping */");
            sb.AppendLine("      overflow: hidden;");
            sb.AppendLine("      text-overflow: ellipsis;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .summary-line .value {");
            sb.AppendLine("      font-weight: normal;");
            sb.AppendLine("      color: #000;");
            sb.AppendLine("      min-width: 20px;");
            sb.AppendLine("      text-align: right;");
            sb.AppendLine("    }");
            sb.AppendLine("  </style>");
            #endregion

            sb.AppendLine("</head>");
            sb.AppendLine();
            sb.AppendLine("<body>");
            sb.AppendLine();
            sb.AppendLine("  <div class=\"header\">");
            sb.AppendLine("    <img src='https://www.junooncapital.com/img/experian.png' />");
            sb.AppendLine("    <div class=\"header-right\">");
            sb.AppendLine("      <h2>Experian Credit Report</h2>");
            sb.AppendLine($"      <p><strong>Report Date:</strong> {DateTime.Now:yyyyMMdd}</p>");
            sb.AppendLine($"      <p><strong>Report Time:</strong> {DateTime.Now:HHmmss}</p>");
            sb.AppendLine($"      <p><strong>Report Number:</strong> {GetDisplayValue(response.transaction_id)}</p>");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine();
            sb.AppendLine("  <div class=\"section-title\"><span class=\"checkmark\"></span> CURRENT APPLICATION INFORMATION</div>");
            sb.AppendLine("  <table class=\"info-table\">");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td class=\"label\">Name </td>");
            sb.AppendLine($"      <td class=\"value\">{GetFullName(currentApplicant)}</td>");
            sb.AppendLine("      <td class=\"label\">Date of Birth </td>");
            sb.AppendLine($"      <td class=\"value\">{GetDisplayValue(FormatDate(currentApplicant?.Date_Of_Birth_Applicant.ToString()))}</td>");
            sb.AppendLine("      <td class=\"label\">Ration Card </td>");
            sb.AppendLine($"      <td class=\"value\">{GetDisplayValue(currentApplicant?.Ration_Card_Number)}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td class=\"label\">Mobile Phone </td>");
            sb.AppendLine($"      <td class=\"value\">{GetDisplayValue(currentApplicant?.Telephone_Number_Applicant_1st)}</td>");
            sb.AppendLine("      <td class=\"label\">PAN </td>");
            sb.AppendLine($"      <td class=\"value\">{GetDisplayValue(currentApplicant?.IncomeTaxPan)}</td>");
            sb.AppendLine("      <td class=\"label\">Driving License </td>");
            sb.AppendLine($"      <td class=\"value\">{GetDisplayValue(currentApplicant?.Driver_License_Number)}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td class=\"label\">Passport Number </td>");
            sb.AppendLine($"      <td class=\"value\">{GetDisplayValue(currentApplicant?.Passport_Number)}</td>");
            sb.AppendLine("      <td class=\"label\">Voter ID </td>");
            sb.AppendLine($"      <td class=\"value\">{GetDisplayValue(currentApplicant?.Voter_s_Identity_Card)}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td class=\"label\">Email </td>");
            sb.AppendLine($"      <td class=\"value\">{GetDisplayValue(currentApplicant?.EMailId)}</td>");
            sb.AppendLine("      <td class=\"label\">Aadhaar Number </td>");
            sb.AppendLine($"      <td class=\"value\">{GetDisplayValue(currentApplicant?.Universal_ID_Number)}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("  </table>");
            sb.AppendLine();
            sb.AppendLine("  <div class=\"section-title\"><span class=\"checkmark\"></span> EXPERIAN CREDIT SCORE</div>");
            sb.AppendLine("  <div class=\"score-section\">");
            sb.AppendLine("    <p class=\"score-note\">Your Experian Credit Report is summarized in the form of Experian Credit Score which ranges");
            sb.AppendLine("      from 300 - 900.</p>");
            sb.AppendLine();
            sb.AppendLine("    <div class=\"score-box\">");
            sb.AppendLine("      <div class=\"score-dial-box\">");
            sb.AppendLine("        <img src=\"https://www.junooncapital.com/img/score_dial.png\" class=\"score-dial\" />");

            // ▶ exact arc marker
            //sb.AppendLine(
            //  $"<div class=\"score-marker\" " +
            //  $"style=\"left:{markerLeft}px; " +
            //  $"transform: rotate({rotateCss}deg);\"></div>"
            //);

            sb.AppendLine($"        <div class=\"score-inside\">{GetDisplayValue(score?.BureauScore.ToString())}</div>");
            sb.AppendLine("      </div>");

            sb.AppendLine();
            sb.AppendLine("      <div class=\"score-factors-box\">");
            sb.AppendLine("        <p><strong>Score Factors</strong></p>");
            sb.AppendLine("        <p>1. <strong>Recency:</strong> Recent Credit Account Defaults</p>");
            sb.AppendLine("        <p>2. <strong>Leverage:</strong> Credit Accounts with on-time re-payment history</p>");
            sb.AppendLine("        <p>3. <strong>Coverage:</strong> Non-delinquent and delinquent Credit Accounts</p>");
            sb.AppendLine("        <p>4. <strong>Delinquency Status:</strong> Defaults on Credit Accounts (current & recent periodic intervals)</p>");
            sb.AppendLine("        <p>5. <strong>Credit Applications:</strong> Applications over last 30 days</p>");
            sb.AppendLine("      </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine();
            sb.AppendLine("  <div class=\"section-title\"><span class=\"checkmark\"></span> REPORT SUMMARY</div>");
            sb.AppendLine("  <div class=\"report-summary\">");
            sb.AppendLine("    <div class=\"summary-col\">");
            sb.AppendLine("      <div class=\"summary-title\">Credit Account Summary</div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Total number of Accounts </span><span class=\"value\">{GetDisplayValue(caisSummary?.Credit_Account?.CreditAccountTotal.ToString())}</span></div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Active Accounts </span><span class=\"value\">{GetDisplayValue(caisSummary?.Credit_Account?.CreditAccountActive.ToString())}</span></div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Closed Accounts </span><span class=\"value\">{GetDisplayValue(caisSummary?.Credit_Account?.CreditAccountClosed.ToString())}</span></div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">SF/WD/WO/SET/RES </span><span class=\"value\">{GetDisplayValue(caisSummary?.Credit_Account?.CADSuitFiledCurrentBalance.ToString())}</span></div>");
            sb.AppendLine("    </div>");
            sb.AppendLine();
            sb.AppendLine("    <div class=\"summary-col\">");
            sb.AppendLine("      <div class=\"summary-title\">Current Balance Amount Summary</div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Total Current Bal. amt </span><span class=\"value\">{GetDisplayValue(caisSummary?.Total_Outstanding_Balance?.Outstanding_Balance_All.ToString())}</span></div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">SF/WD/WO/Settled amt </span><span class=\"value\">{GetDisplayValue(caisSummary?.Credit_Account?.CADSuitFiledCurrentBalance.ToString())}</span></div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Secured Accounts amt </span><span class=\"value\">{GetDisplayValue(caisSummary?.Total_Outstanding_Balance?.Outstanding_Balance_Secured.ToString())}</span></div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Unsecured Accounts amt </span><span class=\"value\">{GetDisplayValue(caisSummary?.Total_Outstanding_Balance?.Outstanding_Balance_UnSecured.ToString())}</span></div>");
            sb.AppendLine("    </div>");
            sb.AppendLine();
            sb.AppendLine("    <div class=\"summary-col\">");
            sb.AppendLine("      <div class=\"summary-title\">Credit Enquiry Summary</div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Last 7 days credit enquiries </span><span class=\"value\">{GetDisplayValue(capsSummary?.CAPSLast7Days.ToString())}</span></div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Last 30 days credit enquiries </span><span class=\"value\">{GetDisplayValue(capsSummary?.CAPSLast30Days.ToString())}</span></div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Last 90 days credit enquiries </span><span class=\"value\">{GetDisplayValue(capsSummary?.CAPSLast90Days.ToString())}</span></div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Last 180 days credit enquiries </span><span class=\"value\">{GetDisplayValue(capsSummary?.CAPSLast180Days.ToString())}</span></div>");
            sb.AppendLine("    </div>");
            sb.AppendLine();
            sb.AppendLine("    <div class=\"summary-col\">");
            sb.AppendLine("      <div class=\"summary-title\">Non-Credit Enquiry Summary</div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Last 7 days non-credit enquiries </span><span class=\"value\">{GetDisplayValue(nonCapsSummary?.NonCreditCAPSLast7Days.ToString())}</span></div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Last 30 days non-credit enquiries </span><span class=\"value\">{GetDisplayValue(nonCapsSummary?.NonCreditCAPSLast30Days.ToString())}</span></div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Last 90 days non-credit enquiries </span><span class=\"value\">{GetDisplayValue(nonCapsSummary?.NonCreditCAPSLast90Days.ToString())}</span></div>");
            sb.AppendLine($"      <div class=\"summary-line\"><span class=\"label\">Last 180 days non-credit enquiries </span><span class=\"value\">{GetDisplayValue(nonCapsSummary?.NonCreditCAPSLast180Days.ToString())}</span></div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine();
            sb.AppendLine("  <div class=\"section-title\"><span class=\"checkmark\"></span> SUMMARY</div>");
            sb.AppendLine("  <p style=\"color: orange; font-size: 10.3px; margin: 4px 0 10px;\">");
            sb.AppendLine("    This section displays summary of all your reported credit accounts found in the Experian Credit Bureau database");
            sb.AppendLine("  </p>");
            sb.AppendLine();
            sb.AppendLine("  <table");
            sb.AppendLine("    style=\"width: 100%; border-collapse: collapse; font-size: 10.4px; margin-bottom: 35px; page-break-inside: auto;\">");
            sb.AppendLine("    <thead style=\"background: #f5f5f5; border-top: 1px solid #ccc;\">");
            sb.AppendLine("      <tr style=\"border-bottom: 1px solid #ccc;\">");
            sb.AppendLine("        <th style=\"text-align: left; padding: 4px 5px; color: #0071CE;\">Sr.no</th>");
            sb.AppendLine("        <th style=\"text-align: left; padding: 4px 5px; color: #0071CE; width: 135px;\">Account Type</th>");
            sb.AppendLine("        <th style=\"text-align: left; padding: 4px 5px; color: #0071CE; width: 110px;\">Account No</th>");
            sb.AppendLine("        <th style=\"text-align: left; padding: 3px 5px; color: #0071CE; width: 85px;\">Date Reported</th>");
            sb.AppendLine("        <th style=\"text-align: left; padding: 4px 4px; color: #0071CE; width: 75px;\">Account Status</th>");
            sb.AppendLine("        <th style=\"text-align: left; padding: 4px 4px; color: #0071CE; width: 85px;\">Date Opened</th>");
            sb.AppendLine("        <th style=\"text-align: left; padding: 3px 4px; color: #0071CE; width: 90px;\">Sanction Amt/Highest Credit</th>");
            sb.AppendLine("        <th style=\"text-align: left; padding: 3px 4px; color: #0071CE; width: 70px;\">Current<br>Balance</th>");
            sb.AppendLine("        <th style=\"text-align: left; padding: 3px 4px; color: #0071CE; width: 70px;\">Overdue<br>Amount</th>");
            sb.AppendLine("      </tr>");
            sb.AppendLine("    </thead>");
            sb.AppendLine("    <tbody>");

            // Account Details Rows
            if (caisDetails != null)
            {
                for (int i = 0; i < caisDetails.Count; i++)
                {
                    var acc = caisDetails[i];
                    sb.AppendLine("        <tr style=\"border-bottom: 1px dashed #ccc; page-break-inside: avoid;\">");
                    sb.AppendLine($"          <td style=\"padding: 4px 5px;\">{i + 1}</td>");
                    sb.AppendLine($"          <td style=\"padding: 4px 4px;\">{GetDisplayValue(GetStaticExperianInfo.GetLoanAccountType(acc.Account_Type ?? "").ToUpper())}</td>");
                    sb.AppendLine($"          <td style=\"padding: 4px 5px;\">{GetDisplayValue(acc.Account_Number)}</td>");
                    sb.AppendLine($"          <td style=\"padding: 4px 5px;\">{GetDisplayValue(FormatDate(acc.Date_Reported))}</td>");
                    sb.AppendLine($"          <td style=\"padding: 3px 4px;\">{GetDisplayValue(GetStaticExperianInfo.GetAccountStatus(acc.Account_Status ?? "").ToUpper())}</td>");
                    sb.AppendLine($"          <td style=\"padding: 3px 4px;\">{GetDisplayValue(FormatDate(acc.Open_Date))}</td>");
                    sb.AppendLine($"          <td style=\"padding: 3px 4px;\">{GetDisplayValue(acc.Highest_Credit_or_Original_Loan_Amount)}</td>");
                    sb.AppendLine($"          <td style=\"padding: 3px 5px;\">{GetDisplayValue(acc.Current_Balance)}</td>");
                    sb.AppendLine($"          <td style=\"padding: 3px 4px;\">{GetDisplayValue(acc.Amount_Past_Due)}</td>");
                    sb.AppendLine("        </tr>");
                }
            }

            sb.AppendLine("          <tr>");
            sb.AppendLine("            <td colspan=\"9\" style=\"border-top: 1px solid #ccc;\"></td>");
            sb.AppendLine("          </tr>");
            sb.AppendLine("    </tbody>");
            sb.AppendLine("  </table>");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("  <div class=\"section-title\">");
            sb.AppendLine("    <span class=\"checkmark\"></span> CREDIT ACCOUNT INFORMATION DETAILS");
            sb.AppendLine("  </div>");
            sb.AppendLine("  <p style=\"color: orange; font-size: 10.3px; margin-bottom: 10px;\">");
            sb.AppendLine("    This section has information based on the details provided to Experian by all our member banks, credit / financial");
            sb.AppendLine("    institutions and other credit grantors with whom you have a credit / loan account.");
            sb.AppendLine("  </p>");
            sb.AppendLine();
            // Detailed Account Information
            if (caisDetails != null)
            {
                for (int i = 0; i < caisDetails.Count; i++)
                {
                    var acct = caisDetails[i];
                    sb.AppendLine("    <div style=\"border: 1px solid #ccc; padding: 10px 14px; margin-bottom: 25px;\">");
                    sb.AppendLine();
                    sb.AppendLine("      <!-- HEADER TABLE FOR NAME, ADDRESS, ACCOUNT NUMBER -->");
                    sb.AppendLine("      <table");
                    sb.AppendLine("        style=\"width: 100%; font-size: 11px; border: 1px solid #ccc; margin-bottom: 10px; border-collapse: collapse;\">");
                    sb.AppendLine("        <tr>");
                    sb.AppendLine($"          <td style=\"padding: 8px 10px; color: #0071CE; font-weight: bold; text-align: left;\">{GetCAISHolderFullName(acct.CAIS_Holder_Details)}</td>");
                    sb.AppendLine("          <td style=\"text-align: right; padding: 8px 10px; white-space: nowrap;\">");
                    sb.AppendLine("            <span style=\"color: green;\"></span>");
                    sb.AppendLine($"            <span style=\"font-weight: bold; color: #0071CE;\">Acct :</span> {i + 1}");
                    sb.AppendLine("          </td>");
                    sb.AppendLine("        </tr>");
                    sb.AppendLine("        <tr>");
                    sb.AppendLine("          <td colspan=\"2\" style=\"padding: 4px 10px; font-weight: normal; color: #000;\">");
                    sb.AppendLine($"            <span style=\"color: #0071CE;\">Address</span> - {GetCAISAddress(acct.CAIS_Holder_Address_Details)}");
                    sb.AppendLine("          </td>");
                    sb.AppendLine("        </tr>");
                    sb.AppendLine("      </table>");
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("      <!-- CREDIT ACCOUNT DETAILS TITLE -->");
                    sb.AppendLine("      <div style=\"color: #0071CE; font-weight: bold; font-size: 12px; margin-bottom: 6px;\">Credit Account details</div>");
                    sb.AppendLine();
                    sb.AppendLine("      <!-- DETAILS ROW -->");
                    sb.AppendLine("      <div style=\"display: flex; gap: 8px; font-size: 10.5px;\">");
                    sb.AppendLine("        <!-- Account Terms -->");
                    sb.AppendLine("        <div style=\"flex: 1; border: 1px solid #ccc; padding: 4px;\">");
                    sb.AppendLine("          <div style=\"color: #0071CE; font-weight: bold; font-size: 11px;\">Account Terms</div>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Institution</span> <span style=\"float: right;\">{GetDisplayValue(acct.Subscriber_Name)}</span></p>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Account Number</span> <span style=\"float: right;\">{GetDisplayValue(acct.Account_Number)}</span></p>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Date Opened</span> <span style=\"float: right;\">{GetDisplayValue(FormatDate(acct.Open_Date))}</span></p>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Date Closed</span> <span style=\"float: right;\">{GetDisplayValue(FormatDate(acct.Date_Closed))}</span></p>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Ownership</span> <span style=\"float: right;\">{GetDisplayValue(GetStaticExperianInfo.GetAccountHolderType(acct.AccountHoldertypeCode ?? ""))}</span></p>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Rate of Interest</span> <span style=\"float: right;\">{GetDisplayValue(acct.Rate_of_Interest)}</span></p>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Value of Collateral</span> <span style=\"float: right;\">{GetDisplayValue(acct.Value_of_Collateral)}</span></p>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Type of Collateral</span> <span style=\"float: right;\">{GetDisplayValue(acct.Type_of_Collateral)}</span></p>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">SuitFiled Willful Default WrittenOff Status</span> <span");
                    sb.AppendLine($"              style=\"float: right;\">{GetDisplayValue(acct.SuitFiledWillfulDefaultWrittenOffStatus)}</span></p>");
                    sb.AppendLine("        </div>");
                    sb.AppendLine();
                    sb.AppendLine("        <!-- Account Description -->");
                    sb.AppendLine("        <div style=\"flex: 1; border: 1px solid #ccc; padding: 4px;\">");
                    sb.AppendLine("          <div style=\"color: #0071CE; font-weight: bold; font-size: 11px;\">Account Description</div>");
                    sb.AppendLine();
                    sb.AppendLine("          <p style=\"display: flex; justify-content: space-between;\">");
                    sb.AppendLine("            <span style=\"color: #0071CE;\">Date Reported</span>");
                    sb.AppendLine($"            <span style=\"text-align: right;\">{GetDisplayValue(FormatDate(acct.Date_Reported))}</span>");
                    sb.AppendLine("          </p>");
                    sb.AppendLine();
                    sb.AppendLine("          <p style=\"display: flex; justify-content: space-between; align-items: flex-start;\">");
                    sb.AppendLine("            <span style=\"color: #0071CE;\">Loan Type</span>");
                    sb.AppendLine($"            <span style=\"text-align: right; max-width: 60%; word-break: break-word;\">{GetDisplayValue(GetStaticExperianInfo.GetLoanAccountType(acct.Account_Type ?? "").ToUpper())}</span>");
                    sb.AppendLine("          </p>");
                    sb.AppendLine();
                    sb.AppendLine("          <p style=\"display: flex; justify-content: space-between; align-items: flex-start;\">");
                    sb.AppendLine("            <span style=\"color: #0071CE;\">Account Status</span>");
                    sb.AppendLine($"            <span style=\"text-align: right; max-width: 60%; word-break: break-word;\">{GetDisplayValue(GetStaticExperianInfo.GetAccountStatus(acct.Account_Status ?? "").ToUpper())}</span>");
                    sb.AppendLine("          </p>");
                    sb.AppendLine();
                    sb.AppendLine("          <p style=\"display: flex; justify-content: space-between;\">");
                    sb.AppendLine("            <span style=\"color: #0071CE;\">Current Balance</span>");
                    sb.AppendLine($"            <span style=\"text-align: right;\">{GetDisplayValue(acct.Current_Balance)}</span>");
                    sb.AppendLine("          </p>");
                    sb.AppendLine();
                    sb.AppendLine("          <p style=\"display: flex; justify-content: space-between;\">");
                    sb.AppendLine("            <span style=\"color: #0071CE;\">Amount Overdue</span>");
                    sb.AppendLine($"            <span style=\"text-align: right;\">{GetDisplayValue(acct.Amount_Past_Due)}</span>");
                    sb.AppendLine("          </p>");
                    sb.AppendLine();
                    sb.AppendLine("          <p style=\"display: flex; justify-content: space-between;\">");
                    sb.AppendLine("            <span style=\"color: #0071CE;\">Last Payment Date</span>");
                    sb.AppendLine($"            <span style=\"text-align: right;\">{GetDisplayValue(FormatDate(acct.Date_of_Last_Payment))}</span>");
                    sb.AppendLine("          </p>");
                    sb.AppendLine();
                    sb.AppendLine("          <p style=\"display: flex; justify-content: space-between; align-items: flex-start;\">");
                    sb.AppendLine("            <span style=\"color: #0071CE;\">SuitFiled Willful Default</span>");
                    sb.AppendLine($"            <span style=\"text-align: right; max-width: 60%; word-break: break-word;\">{GetDisplayValue(acct.SuitFiled_WillfulDefault)}</span>");
                    sb.AppendLine("          </p>");
                    sb.AppendLine("        </div>");
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("        <!-- Account Details -->");
                    sb.AppendLine("        <div style=\"flex: 1; border: 1px solid #ccc; padding: 4px;\">");
                    sb.AppendLine("          <div style=\"color: #0071CE; font-weight: bold; font-size: 11px;\">Account Details</div>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Credit Limit Amt</span> <span style=\"float: right;\">{GetDisplayValue(acct.Credit_Limit_Amount)}</span></p>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Repayment Tenure</span> <span style=\"float: right;\">{GetDisplayValue(acct.Repayment_Tenure)}</span></p>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Total Write-off Amt</span> <span style=\"float: right;\">{GetDisplayValue(acct.Written_Off_Amt_Total)}</span></p>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Principal Write-off</span> <span style=\"float: right;\">{GetDisplayValue(acct.Written_Off_Amt_Principal)}</span></p>");
                    sb.AppendLine($"          <p><span style=\"color: #0071CE;\">Settlement Amt</span> <span style=\"float: right;\">{GetDisplayValue(acct.Settlement_Amount)}</span></p>");
                    sb.AppendLine($"          <p style=\"display: flex; justify-content: space-between; word-break: break-word;\">");
                    sb.AppendLine("            <span style=\"color: #0071CE;\">Written off Settled Status</span>");
                    sb.AppendLine($"            <span style=\"text-align: right; max-width: 50%; word-wrap: break-word;\">{GetDisplayValue(acct.Written_off_Settled_Status)}</span>");
                    sb.AppendLine("          </p>");
                    sb.AppendLine("        </div>");
                    sb.AppendLine("      </div>");
                    sb.AppendLine();
                    sb.AppendLine();

                    sb.AppendLine("      <div style=\"margin-top: 10px;\">");
                    sb.AppendLine("        <div style=\"color: #0071CE; font-size: 12px; font-weight: bold;\">Payment History</div>");
                    sb.AppendLine();

                    // Payment History Table
                    string[] PaymentMonths =
                    {
                        "Jan","Feb","Mar","Apr","May","Jun",
                        "Jul","Aug","Sep","Oct","Nov","Dec"
                    };

                    //Helper function to get payment status symbol based on days past due
                    string GetPaymentStatusSymbol(string daysPastDue)
                    {
                        if (string.IsNullOrEmpty(daysPastDue)) return "–";

                        if (int.TryParse(daysPastDue, out int days))
                        {
                            if (days == 0) return "0";
                            if (days > 0 && days <= 29) return "1-29";
                            if (days >= 30 && days <= 59) return "30-59";
                            if (days >= 60 && days <= 89) return "60-89";
                            if (days >= 90) return "90+";
                        }

                        return "?";
                    }

                    // Get payment history from CAIS_Account_History
                    if (acct.CAIS_Account_History != null && acct.CAIS_Account_History.Any())
                    {
                        // Group history by year
                        var historyByYear = acct.CAIS_Account_History?
                            .GroupBy(h => h.Year)
                            .ToDictionary(
                                g => g.Key,
                                g => g.ToDictionary(h => h.Month, h => h.Days_Past_Due)
                            );

                        sb.AppendLine("<table style='width:100%; border-collapse:collapse; font-size:10px;'>");

                        // HEADER
                        sb.AppendLine("<tr>");
                        sb.AppendLine("<th style='border:1px solid #ccc; padding:4px; color:#0071CE;'>Year</th>");
                        foreach (var m in PaymentMonths)
                            sb.AppendLine($"<th style='border:1px solid #ccc; padding:4px; color:#0071CE;'>{m}</th>");
                        sb.AppendLine("</tr>");

                        // ROWS - Display years in descending order
                        foreach (var year in historyByYear.Keys.OrderByDescending(y => y))
                        {
                            sb.AppendLine("<tr>");
                            sb.AppendLine($"<td style='border:1px solid #ccc; padding:4px; font-weight:bold;'>{year}</td>");

                            for (int month = 1; month <= 12; month++)
                            {
                                int? daysPastDueNullable;
                                if (historyByYear[year].TryGetValue(month, out daysPastDueNullable))
                                {
                                    var daysPastDueStr = daysPastDueNullable.HasValue ? daysPastDueNullable.Value.ToString() : "";
                                    if (daysPastDueStr == "0")
                                    {
                                        sb.AppendLine(
                                            "<td style='border:1px solid #ccc; padding:4px; text-align:center;'>" +
                                            "<span style='display:inline-block;width:18px;height:18px;line-height:18px;" +
                                            $"border-radius:50%;background:#2cb34a;color:#fff;font-size:9px;'>{daysPastDueStr}</span>" +
                                            "</td>"
                                        );
                                    }
                                    else if (!string.IsNullOrEmpty(daysPastDueStr))
                                    {
                                        sb.AppendLine(
                                            "<td style='border:1px solid #ccc; padding:4px; text-align:center;'>" +
                                            "<span style='display:inline-block;width:18px;height:18px;line-height:18px;" +
                                            $"border-radius:50%;background:#FF8C00;color:#fff;font-size:9px;'>{daysPastDueStr}</span>" +
                                            "</td>"
                                        );
                                    }
                                    else
                                    {
                                        sb.AppendLine("<td style='border:1px solid #ccc; padding:4px; text-align:center;'>–</td>");
                                    }
                                }
                                else
                                {
                                    // No data for this month
                                    sb.AppendLine("<td style='border:1px solid #ccc; padding:4px; text-align:center;'>–</td>");
                                }
                            }

                            sb.AppendLine("</tr>");
                        }

                        sb.AppendLine("</table>");
                    }
                    else if (!string.IsNullOrEmpty(acct.Payment_History_Profile))
                    {
                        // Fallback to Payment_History_Profile if CAIS_Account_History is not available
                        DateTime reportDate = DateTime.TryParse(acct.Date_Reported, out var d) ? d : DateTime.Now;

                        // Build grid from Payment_History_Profile string
                        var grid = new Dictionary<int, Dictionary<int, char>>();
                        var cursor = new DateTime(reportDate.Year, reportDate.Month, 1);

                        foreach (char ch in acct.Payment_History_Profile.Reverse()) // Process from oldest to newest
                        {
                            int year = cursor.Year;
                            int month = cursor.Month;

                            if (!grid.ContainsKey(year))
                                grid[year] = new Dictionary<int, char>();

                            grid[year][month] = ch;
                            cursor = cursor.AddMonths(-1);
                        }

                        sb.AppendLine("<table style='width:100%; border-collapse:collapse; font-size:10px;'>");

                        // HEADER
                        sb.AppendLine("<tr>");
                        sb.AppendLine("<th style='border:1px solid #ccc; padding:4px; color:#0071CE;'>Year</th>");
                        foreach (var m in PaymentMonths)
                            sb.AppendLine($"<th style='border:1px solid #ccc; padding:4px; color:#0071CE;'>{m}</th>");
                        sb.AppendLine("</tr>");

                        // ROWS
                        foreach (var year in grid.Keys.OrderByDescending(y => y))
                        {
                            sb.AppendLine("<tr>");
                            sb.AppendLine($"<td style='border:1px solid #ccc; padding:4px; font-weight:bold;'>{year}</td>");

                            for (int month = 1; month <= 12; month++)
                            {
                                if (grid[year].TryGetValue(month, out char status))
                                {
                                    if (status == '0')
                                    {
                                        sb.AppendLine(
                                            "<td style='border:1px solid #ccc; padding:4px; text-align:center;'>" +
                                            "<span style='display:inline-block;width:18px;height:18px;line-height:18px;" +
                                            "border-radius:50%;background:#2cb34a;color:#fff;font-size:10px;'>0</span>" +
                                            "</td>"
                                        );
                                    }
                                    else if (status == '1' || status == '2')
                                    {
                                        // Numbers 1 or 2 for late payments
                                        sb.AppendLine(
                                            "<td style='border:1px solid #ccc; padding:4px; text-align:center;'>" +
                                            "<span style='display:inline-block;width:18px;height:18px;line-height:18px;" +
                                            $"border-radius:50%;background:#ff6600;color:#fff;font-size:10px;'>{status}</span>" +
                                            "</td>"
                                        );
                                    }
                                    else
                                    {
                                        sb.AppendLine("<td style='border:1px solid #ccc; padding:4px; text-align:center;'>–</td>");
                                    }
                                }
                                else
                                {
                                    sb.AppendLine("<td style='border:1px solid #ccc; padding:4px; text-align:center;'>–</td>");
                                }
                            }

                            sb.AppendLine("</tr>");
                        }

                        sb.AppendLine("</table>");
                    }
                    else
                    {
                        sb.AppendLine("<p style='font-size:10px;color:#777;'>No payment history available.</p>");
                    }
                    sb.AppendLine("      </div>");
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("      <div style=\"margin-top: 15px; font-size: 10px;\">");
                    sb.AppendLine("        <div style=\"color: #0071CE; font-size: 12px; font-weight: bold; margin-bottom: 5px;\">");
                    sb.AppendLine("          Consumer Personal details on the Credit Account");
                    sb.AppendLine("        </div>");
                    sb.AppendLine();
                    sb.AppendLine("        <div style=\"display: flex; gap: 10px;\">");
                    sb.AppendLine("          <!-- Table 1: Basic & Contact Info -->");
                    sb.AppendLine("          <table style=\"border: 1px solid #ccc; border-collapse: collapse; width: 45%; font-size: 10px;\">");
                    sb.AppendLine($"            <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc; width: 40%;\">Date of Birth</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISHolderDetailsValue(acct.CAIS_Holder_Details, d => FormatDate(d?.Date_of_birth))}</td></tr>");
                    sb.AppendLine($"            <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc; width: 40%;\">Gender</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISHolderDetailsValue(acct.CAIS_Holder_Details, d => GetStaticExperianInfo.GetGenderType(d?.Gender_Code ?? ""))}</td></tr>");
                    sb.AppendLine($"            <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc; width: 40%;\">Occupation</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetDisplayValue(GetStaticExperianInfo.GetEmploymentStatus(acct.Occupation_Code ?? ""))}</td></tr>");
                    sb.AppendLine($"            <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc; width: 40%;\">Email Address</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISPhoneDetailsValue(acct.CAIS_Holder_Phone_Details, p => p?.EMailId)}</td></tr>");
                    sb.AppendLine($"            <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc; width: 40%;\">Phone Number</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISPhoneDetailsValue(acct.CAIS_Holder_Phone_Details, p => p?.Telephone_Number)}</td></tr>");
                    sb.AppendLine($"            <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc; width: 40%;\">Extension</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISPhoneDetailsValue(acct.CAIS_Holder_Phone_Details, p => p?.Telephone_Extension)}</td></tr>");
                    sb.AppendLine($"            <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc; width: 40%;\">Alt Phone Number</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISPhoneDetailsValue(acct.CAIS_Holder_Phone_Details, p => p?.Mobile_Telephone_Number)}</td></tr>");
                    sb.AppendLine($"            <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc; width: 40%;\">Alt Extension</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISPhoneDetailsValue(acct.CAIS_Holder_Phone_Details, p => p?.Telephone_Extension)}</td></tr>");
                    sb.AppendLine("          </table>");
                    sb.AppendLine();
                    sb.AppendLine("          <!-- Table 2: ID Info -->");
                    sb.AppendLine("          <table style=\"border: 1px solid #ccc; border-collapse: collapse; width: 55%; font-size: 10px;\">");
                    sb.AppendLine("            <thead>");
                    sb.AppendLine("              <tr style=\"color: #0071CE;\">");
                    sb.AppendLine("                <th style=\"padding: 5px 6px; text-align: left; border: 1px solid #ccc;\">ID Type</th>");
                    sb.AppendLine("                <th style=\"padding: 5px 6px; text-align: left; border: 1px solid #ccc;\">ID Number</th>");
                    sb.AppendLine("                <th style=\"padding: 5px 6px; text-align: left; border: 1px solid #ccc;\">Date of Issue</th>");
                    sb.AppendLine("                <th style=\"padding: 5px 6px; text-align: left; border: 1px solid #ccc;\">Date of Expiry</th>");
                    sb.AppendLine("              </tr>");
                    sb.AppendLine("            </thead>");
                    sb.AppendLine("            <tbody>");
                    sb.AppendLine($"              <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc;\">PAN</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => id?.Income_TAX_PAN)}</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => FormatDate(id?.PAN_Issue_Date))}</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => FormatDate(id?.PAN_Expiration_Date))}</td></tr>");
                    sb.AppendLine($"              <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc;\">Passport</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => id?.Passport_Number)}</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => FormatDate(id?.Passport_Issue_Date))}</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => FormatDate(id?.Passport_Expiration_Date))}</td></tr>");
                    sb.AppendLine($"              <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc;\">Voter ID</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => id?.Voter_ID_Number)}</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => FormatDate(id?.Voter_ID_Issue_Date))}</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => FormatDate(id?.Voter_ID_Expiration_Date))}</td></tr>");
                    sb.AppendLine($"              <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc;\">Aadhaar/UID</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => id?.Universal_ID_Number)}</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => FormatDate(id?.Universal_ID_Issue_Date))}</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => FormatDate(id?.Universal_ID_Expiration_Date))}</td></tr>");
                    sb.AppendLine($"              <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc;\">Driving License</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => id?.Driver_License_Number)}</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => FormatDate(id?.Driver_License_Issue_Date))}</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => FormatDate(id?.Driver_License_Expiration_Date))}</td></tr>");
                    sb.AppendLine($"              <tr><td style=\"padding: 5px 6px; color: #0071CE; border: 1px solid #ccc;\">Ration Card</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => id?.Ration_Card_Number)}</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => FormatDate(id?.Ration_Card_Issue_Date))}</td><td style=\"padding: 5px 6px; border: 1px solid #ccc;\">{GetCAISIDDetailsValue(acct.CAIS_Holder_ID_Details, id => FormatDate(id?.Ration_Card_Expiration_Date))}</td></tr>");
                    sb.AppendLine("            </tbody>");
                    sb.AppendLine("          </table>");
                    sb.AppendLine("        </div>");
                    sb.AppendLine("      </div>");
                    sb.AppendLine();
                    sb.AppendLine("    </div>");
                }
            }

            sb.AppendLine();
            sb.AppendLine("      <div class=\"section-title\">");
            sb.AppendLine("        <span class=\"checkmark\" style=\"color: green;\"></span> CREDIT ENQUIRIES");
            sb.AppendLine("      </div>");
            sb.AppendLine("      <p style=\"color: orange; font-size: 10.3px; margin-bottom: 10px;\">");
            sb.AppendLine("        This section shows the names of the credit institutions that have processed a credit / loan application for you.");
            sb.AppendLine("      </p>");
            sb.AppendLine();

            // Combine credit and non-credit enquiries
            var allEnquiries = new List<CAPSApplicationDetails>();
            if (capsDetails != null) allEnquiries.AddRange(capsDetails);
            if (nonCapsDetails != null && nonCapsDetails.Any()) // Check if nonCapsDetails has any items
            {
                foreach (var item in nonCapsDetails)
                {
                    allEnquiries.Add(item);
                }
            }

            if (allEnquiries.Count > 0)
            {
                for (int i = 0; i < allEnquiries.Count; i++)
                {
                    if (allEnquiries[i] is CAPSApplicationDetails creditEnq)
                    {
                        sb.AppendLine("        <table");
                        sb.AppendLine("          style=\"width: 100%; font-size: 10px; margin-bottom: 16px; border-collapse: collapse; border: 1px solid #ccc;\">");
                        sb.AppendLine("          <tr>");
                        sb.AppendLine("            <td colspan=\"3\"></td>");
                        sb.AppendLine("            <td style=\"text-align: right; padding: 6px 10px; font-weight: bold;\">");
                        sb.AppendLine("              <span style=\"color: green;\"></span>");
                        sb.AppendLine($"              <span style=\"color: #0071CE;\">Cr Enq:</span> CE{i + 1}");
                        sb.AppendLine("            </td>");
                        sb.AppendLine("          </tr>");
                        sb.AppendLine("          <tr>");
                        sb.AppendLine("            <td style=\"padding: 5px; color: #0071CE;\">Institution </td>");
                        sb.AppendLine($"            <td colspan=\"3\" style=\"font-weight: normal;\">{GetDisplayValue(creditEnq.Subscriber_Name?.Trim())}</td>");
                        sb.AppendLine("          </tr>");
                        sb.AppendLine("          <tr>");
                        sb.AppendLine("            <td style=\"padding: 5px; color: #0071CE;\">ERN</td>");
                        sb.AppendLine($"            <td>{GetDisplayValue(creditEnq.ReportNumber)}</td>");
                        sb.AppendLine("            <td style=\"padding: 5px; color: #0071CE;\">Search Type</td>");
                        sb.AppendLine($"            <td>{GetDisplayValue(creditEnq.Enquiry_Reason)}</td>");
                        sb.AppendLine("          </tr>");
                        sb.AppendLine("          <tr>");
                        sb.AppendLine("            <td style=\"padding: 5px; color: #0071CE;\">Application Date</td>");
                        sb.AppendLine($"            <td>{GetDisplayValue(FormatDate(creditEnq.Date_of_Request))}</td>");
                        sb.AppendLine("            <td style=\"padding: 5px; color: #0071CE;\">Amount Applied For</td>");
                        sb.AppendLine($"            <td>{GetDisplayValue(creditEnq.Amount_Financed)}</td>");
                        sb.AppendLine("          </tr>");
                        sb.AppendLine("          <tr>");
                        sb.AppendLine("            <td style=\"padding: 5px; color: #0071CE;\">Duration of Agreement</td>");
                        sb.AppendLine($"            <td>{GetDisplayValue(creditEnq.Duration_Of_Agreement)}</td>");
                        sb.AppendLine("            <td></td>");
                        sb.AppendLine("            <td></td>");
                        sb.AppendLine("          </tr>");
                        sb.AppendLine("        </table>");
                    }
                    else if (allEnquiries[i] is object nonCreditEnq)
                    {
                        // Handle non-credit enquiries (these are now just objects)
                        sb.AppendLine("        <table");
                        sb.AppendLine("          style=\"width: 100%; font-size: 10px; margin-bottom: 16px; border-collapse: collapse; border: 1px solid #ccc;\">");
                        sb.AppendLine("          <tr>");
                        sb.AppendLine("            <td colspan=\"3\"></td>");
                        sb.AppendLine("            <td style=\"text-align: right; padding: 6px 10px; font-weight: bold;\">");
                        sb.AppendLine("              <span style=\"color: green;\"></span>");
                        sb.AppendLine($"              <span style=\"color: #0071CE;\">Cr Enq:</span> {i + 1}");
                        sb.AppendLine("            </td>");
                        sb.AppendLine("          </tr>");
                        sb.AppendLine("          <tr>");
                        sb.AppendLine("            <td style=\"padding: 5px; color: #0071CE;\">Institution </td>");
                        sb.AppendLine($"            <td colspan=\"3\" style=\"font-weight: normal;\">Non-Credit Enquiry</td>");
                        sb.AppendLine("          </tr>");
                        sb.AppendLine("        </table>");
                    }
                }
            }
            else
            {
                sb.AppendLine("        <p style=\"font-size: 10px; color: #555;\">No credit enquiries found.</p>");
            }

            sb.AppendLine();
            sb.AppendLine("</body>");
            sb.AppendLine();
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }
}