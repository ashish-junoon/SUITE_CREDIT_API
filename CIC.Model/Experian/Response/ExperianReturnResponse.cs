using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CIC.Model.Experian.Response
{
    public class ExperianReturnResponse
    {
        public DateTime timestamp { get; set; }
        public string? transaction_id { get; set; }
        public int? statusCode { get; set; }
        public bool success { get; set; } = false;
        public string? message { get; set; }
        public Inprofileresponse? INProfileResponse { get; set; }

    }

    public class Inprofileresponse
    {
        public Usermessage? UserMessage { get; set; }
        public CreditProfileHeader? CreditProfileHeader { get; set; }
        public Current_Application? Current_Application { get; set; }
        public CAIS_Account? CAIS_Account { get; set; }
        public Match_Result? Match_result { get; set; }
        public Totalcaps_Summary? TotalCAPS_Summary { get; set; }
        public CAPS? CAPS { get; set; }
        public Noncreditcaps? NonCreditCAPS { get; set; }
        public SCORE? SCORE { get; set; }
    }

    public class Usermessage
    {
        public string? UserMessageText { get; set; }
    }
    public class CreditProfileHeader
    {
        public string? Enquiry_Username { get; set; }
        public string? ReportDate { get; set; }
        public string? ReportTime { get; set; }
        public string? Version { get; set; }
        public string? ReportNumber { get; set; }
        public string? Subscriber { get; set; }
        public string? Subscriber_Name { get; set; }
        public string? CustomerReferenceID { get; set; }
    }

    public class Current_Application
    {
        public Current_Application_Details? Current_Application_Details { get; set; }
    }

    public class Current_Application_Details
    {
        public string? Enquiry_Reason { get; set; }
        public string? Finance_Purpose { get; set; }
        public string? Amount_Financed { get; set; }
        public string? Duration_Of_Agreement { get; set; }
        public Current_Applicant_Details? Current_Applicant_Details { get; set; }
        public Current_Other_Details? Current_Other_Details { get; set; }
        public Current_Applicant_Address_Details? Current_Applicant_Address_Details { get; set; }
        public string? Current_Applicant_Additional_AddressDetails { get; set; } // Changed from object to string
    }

    public class Current_Applicant_Details
    {
        public string? Last_Name { get; set; }
        public string? First_Name { get; set; }
        public string? Middle_Name1 { get; set; }
        public string? Middle_Name2 { get; set; }
        public string? Middle_Name3 { get; set; }
        public string? Gender_Code { get; set; }
        public string? IncomeTaxPan { get; set; }
        public string? PAN_Issue_Date { get; set; }
        public string? PAN_Expiration_Date { get; set; }
        public string? Passport_Number { get; set; }
        public string? Passport_Issue_Date { get; set; }
        public string? Passport_Expiration_Date { get; set; }
        public string? Voter_s_Identity_Card { get; set; }
        public string? Voter_ID_Issue_Date { get; set; }
        public string? Voter_ID_Expiration_Date { get; set; }
        public string? Driver_License_Number { get; set; }
        public string? Driver_License_Issue_Date { get; set; }
        public string? Driver_License_Expiration_Date { get; set; }
        public string? Ration_Card_Number { get; set; }
        public string? Ration_Card_Issue_Date { get; set; }
        public string? Ration_Card_Expiration_Date { get; set; }
        public string? Universal_ID_Number { get; set; }
        public string? Universal_ID_Issue_Date { get; set; }
        public string? Universal_ID_Expiration_Date { get; set; }
        public string? Date_Of_Birth_Applicant { get; set; }
        public string? Telephone_Number_Applicant_1st { get; set; }
        public string? Telephone_Extension { get; set; }
        public string? Telephone_Type { get; set; }
        public string? MobilePhoneNumber { get; set; }
        public string? EMailId { get; set; }
    }

    public class Current_Other_Details
    {
        public string? Income { get; set; }
        public string? Marital_Status { get; set; }
        public string? Employment_Status { get; set; }
        public string? Time_with_Employer { get; set; }
        public string? Number_of_Major_Credit_Card_Held { get; set; }
    }

    public class Current_Applicant_Address_Details
    {
        public string? FlatNoPlotNoHouseNo { get; set; }
        public string? BldgNoSocietyName { get; set; }
        public string? RoadNoNameAreaLocality { get; set; }
        public string? City { get; set; }
        public string? Landmark { get; set; }
        public string? State { get; set; }
        public string? PINCode { get; set; }
        public string? Country_Code { get; set; }
    }

    public class CAIS_Account
    {
        public CAIS_Summary? CAIS_Summary { get; set; }
        public List<CAIS_Account_DETAILS>? CAIS_Account_DETAILS { get; set; } // Changed to List
    }

    public class CAIS_Summary
    {
        public Credit_Account? Credit_Account { get; set; }
        public Total_Outstanding_Balance? Total_Outstanding_Balance { get; set; }
    }

    public class Credit_Account
    {
        public string? CreditAccountTotal { get; set; }
        public string? CreditAccountActive { get; set; }
        public string? CreditAccountDefault { get; set; }
        public string? CreditAccountClosed { get; set; }
        public string? CADSuitFiledCurrentBalance { get; set; }
    }

    public class Total_Outstanding_Balance
    {
        public string? Outstanding_Balance_Secured { get; set; }
        public string? Outstanding_Balance_Secured_Percentage { get; set; }
        public string? Outstanding_Balance_UnSecured { get; set; }
        public string? Outstanding_Balance_UnSecured_Percentage { get; set; }
        public string? Outstanding_Balance_All { get; set; }
    }

    public class CAIS_Account_DETAILS
    {
        public string? Identification_Number { get; set; }
        public string? Subscriber_Name { get; set; }
        public string? Account_Number { get; set; }
        public string? Portfolio_Type { get; set; }
        public string? Account_Type { get; set; }
        public string? Open_Date { get; set; }
        public string? Credit_Limit_Amount { get; set; }
        public string? Highest_Credit_or_Original_Loan_Amount { get; set; }
        public string? Terms_Duration { get; set; }
        public string? Terms_Frequency { get; set; }
        public string? Scheduled_Monthly_Payment_Amount { get; set; }
        public string? Account_Status { get; set; }
        public string? Payment_Rating { get; set; }
        public string? Payment_History_Profile { get; set; }
        public string? Special_Comment { get; set; }
        public string? Current_Balance { get; set; }
        public string? Amount_Past_Due { get; set; }
        public string? Original_Charge_Off_Amount { get; set; }
        public string? Date_Reported { get; set; }
        public string? Date_of_First_Delinquency { get; set; }
        public string? Date_Closed { get; set; }
        public string? Date_of_Last_Payment { get; set; }
        public string? SuitFiledWillfulDefaultWrittenOffStatus { get; set; }
        public string? SuitFiled_WillfulDefault { get; set; }
        public string? Written_off_Settled_Status { get; set; }
        public string? Value_of_Credits_Last_Month { get; set; }
        public string? Occupation_Code { get; set; }
        public string? Settlement_Amount { get; set; }
        public string? Value_of_Collateral { get; set; }
        public string? Type_of_Collateral { get; set; }
        public string? Written_Off_Amt_Total { get; set; }
        public string? Written_Off_Amt_Principal { get; set; }
        public string? Rate_of_Interest { get; set; }
        public string? Repayment_Tenure { get; set; }
        public string? Promotional_Rate_Flag { get; set; }
        public string? Income { get; set; }
        public string? Income_Indicator { get; set; }
        public string? Income_Frequency_Indicator { get; set; }
        public string? DefaultStatusDate { get; set; }
        public string? LitigationStatusDate { get; set; }
        public string? WriteOffStatusDate { get; set; }
        public string? DateOfAddition { get; set; }
        public string? CurrencyCode { get; set; }
        public string? Subscriber_comments { get; set; }
        public string? Consumer_comments { get; set; }
        public string? AccountHoldertypeCode { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<CAIS_Account_History>))]
        public List<CAIS_Account_History>? CAIS_Account_History { get; set; } // Changed to List

        [JsonConverter(typeof(SingleOrArrayConverter<CAIS_Holder_Details>))]
        public List<CAIS_Holder_Details>? CAIS_Holder_Details { get; set; } // Changed to List

        [JsonConverter(typeof(SingleOrArrayConverter<CAIS_Holder_Address_Details>))]
        public List<CAIS_Holder_Address_Details>? CAIS_Holder_Address_Details { get; set; } // Changed to List

        [JsonConverter(typeof(SingleOrArrayConverter<CAIS_Holder_Phone_Details>))]
        public List<CAIS_Holder_Phone_Details>? CAIS_Holder_Phone_Details { get; set; } // Changed to List

        [JsonConverter(typeof(SingleOrArrayConverter<CAIS_Holder_ID_Details>))]
        public List<CAIS_Holder_ID_Details>? CAIS_Holder_ID_Details { get; set; } // Changed to List

        [JsonConverter(typeof(SingleOrArrayConverter<Account_Review_Data>))]
        public List<Account_Review_Data>? Account_Review_Data { get; set; } // Changed to List
    }

    // Add these missing classes
    public class CAIS_Account_History
    {
        public string? Year { get; set; }
        public string? Month { get; set; }
        public string? Days_Past_Due { get; set; }
        public string? Asset_Classification { get; set; }
    }

    public class Account_Review_Data
    {
        public string? Year { get; set; }
        public string? Month { get; set; }
        public string? Account_Status { get; set; }
        public string? Actual_Payment_Amount { get; set; }
        public string? Current_Balance { get; set; }
        public string? Credit_Limit_Amount { get; set; }
        public string? Amount_Past_Due { get; set; }
        public string? Cash_Limit { get; set; }
        public string? EMI_Amount { get; set; }
    }

    public class CAIS_Holder_Details
    {
        public string? Surname_Non_Normalized { get; set; }
        public string? First_Name_Non_Normalized { get; set; }
        public string? Middle_Name_1_Non_Normalized { get; set; }
        public string? Middle_Name_2_Non_Normalized { get; set; }
        public string? Middle_Name_3_Non_Normalized { get; set; }
        public string? Alias { get; set; }
        public string? Gender_Code { get; set; }
        public string? Income_TAX_PAN { get; set; }
        public string? Passport_Number { get; set; }
        public string? Voter_ID_Number { get; set; }
        public string? Date_of_birth { get; set; }
    }

    public class CAIS_Holder_Address_Details
    {
        public string? First_Line_Of_Address_non_normalized { get; set; }
        public string? Second_Line_Of_Address_non_normalized { get; set; }
        public string? Third_Line_Of_Address_non_normalized { get; set; }
        public string? City_non_normalized { get; set; }
        public string? Fifth_Line_Of_Address_non_normalized { get; set; }
        public string? State_non_normalized { get; set; }
        public string? ZIP_Postal_Code_non_normalized { get; set; }
        public string? CountryCode_non_normalized { get; set; }
        public string? Address_indicator_non_normalized { get; set; }
        public string? Residence_code_non_normalized { get; set; }
    }

    public class CAIS_Holder_Phone_Details
    {
        public string? Telephone_Number { get; set; }
        public string? Telephone_Type { get; set; }
        public string? Telephone_Extension { get; set; }
        public string? Mobile_Telephone_Number { get; set; }
        public string? FaxNumber { get; set; }
        public string? EMailId { get; set; }
    }

    public class CAIS_Holder_ID_Details
    {
        public string? Income_TAX_PAN { get; set; }
        public string? PAN_Issue_Date { get; set; }
        public string? PAN_Expiration_Date { get; set; }
        public string? Passport_Number { get; set; }
        public string? Passport_Issue_Date { get; set; }
        public string? Passport_Expiration_Date { get; set; }
        public string? Voter_ID_Number { get; set; }
        public string? Voter_ID_Issue_Date { get; set; }
        public string? Voter_ID_Expiration_Date { get; set; }
        public string? Driver_License_Number { get; set; }
        public string? Driver_License_Issue_Date { get; set; }
        public string? Driver_License_Expiration_Date { get; set; }
        public string? Ration_Card_Number { get; set; }
        public string? Ration_Card_Issue_Date { get; set; }
        public string? Ration_Card_Expiration_Date { get; set; }
        public string? Universal_ID_Number { get; set; }
        public string? Universal_ID_Issue_Date { get; set; }
        public string? Universal_ID_Expiration_Date { get; set; }
        public string? EMailId { get; set; }
    }

    public class Match_Result
    {
        public string? Exact_match { get; set; }
    }

    public class Totalcaps_Summary
    {
        public string? TotalCAPSLast7Days { get; set; }
        public string? TotalCAPSLast30Days { get; set; }
        public string? TotalCAPSLast90Days { get; set; }
        public string? TotalCAPSLast180Days { get; set; }
    }

    public class CAPS
    {
        public CAPS_Summary? CAPS_Summary { get; set; }
        public List<CAPS_Application_Details>? CAPS_Application_Details { get; set; } // Changed to List
    }

    public class CAPS_Summary
    {
        public string? CAPSLast7Days { get; set; }
        public string? CAPSLast30Days { get; set; }
        public string? CAPSLast90Days { get; set; }
        public string? CAPSLast180Days { get; set; }
    }

    public class CAPS_Application_Details
    {
        public string? Subscriber_code { get; set; }
        public string? Subscriber_Name { get; set; }
        public string? Date_of_Request { get; set; }
        public string? ReportTime { get; set; }
        public string? ReportNumber { get; set; }
        public string? Enquiry_Reason { get; set; }
        public string? Finance_Purpose { get; set; }
        public string? Amount_Financed { get; set; }
        public string? Duration_Of_Agreement { get; set; }
    }

    public class Noncreditcaps
    {
        public Noncreditcaps_Summary? NonCreditCAPS_Summary { get; set; }
        public List<CAPS_Application_Details>? CAPS_Application_Details { get; set; } // Changed to List<object> since it's empty array
    }

    public class Noncreditcaps_Summary
    {
        public string? NonCreditCAPSLast7Days { get; set; }
        public string? NonCreditCAPSLast30Days { get; set; }
        public string? NonCreditCAPSLast90Days { get; set; }
        public string? NonCreditCAPSLast180Days { get; set; }
    }

    // You can remove CAPS_Application_Details1 class since it's not needed

    public class SCORE
    {
        public string? BureauScore { get; set; }
        public string? BureauScoreConfidLevel { get; set; }
    }
    public class SingleOrArrayConverter<T> : JsonConverter where T : class
    {
        public override bool CanConvert(Type objectType)
        {
            // Check if the type is List<T> or IList<T>
            return objectType.IsGenericType &&
                   (objectType.GetGenericTypeDefinition() == typeof(List<>) ||
                    objectType.GetGenericTypeDefinition() == typeof(IList<>)) &&
                   objectType.GetGenericArguments()[0] == typeof(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>(serializer);
            }

            if (token.Type == JTokenType.Object)
            {
                var singleItem = token.ToObject<T>(serializer);
                return singleItem == null ? new List<T>() : new List<T> { singleItem };
            }

            if (token.Type == JTokenType.Null)
            {
                return new List<T>();
            }

            throw new JsonSerializationException($"Unexpected token type: {token.Type}");
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}