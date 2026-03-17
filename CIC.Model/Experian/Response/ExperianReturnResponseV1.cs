using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CIC.Model.Experian.ResponseNew
{
    public class ExperianReturnResponseV1
    {
        public DateTime timestamp { get; set; }
        public string? transaction_id { get; set; }
        public int? statusCode { get; set; }
        public bool success { get; set; } = false;
        public string? message { get; set; }
        public INProfileResponse? INProfileResponse { get; set; }
    }


    [XmlRoot("INProfileResponse")]
    public class INProfileResponse
    {
        [XmlElement("Header")]
        public Header? Header { get; set; }

        [XmlElement("UserMessage")]
        public UserMessage? UserMessage { get; set; }

        [XmlElement("CreditProfileHeader")]
        public CreditProfileHeader? CreditProfileHeader { get; set; }

        [XmlElement("Current_Application")]
        public CurrentApplication? Current_Application { get; set; }

        [XmlElement("CAIS_Account")]
        public CAISAccount? CAIS_Account { get; set; }

        [XmlElement("Match_result")]
        public MatchResult? Match_result { get; set; }

        [XmlElement("TotalCAPS_Summary")]
        public TotalCAPSSummary? TotalCAPS_Summary { get; set; }

        [XmlElement("CAPS")]
        public CAPS? CAPS { get; set; }

        [XmlElement("NonCreditCAPS")]
        public NonCreditCAPS? NonCreditCAPS { get; set; }

        [XmlElement("SCORE")]
        public SCORE? SCORE { get; set; }
    }
    public class SCORE
    {
        [XmlElement("BureauScore")]
        public int BureauScore { get; set; }

        [XmlElement("BureauScoreConfidLevel")]
        public string? BureauScoreConfidLevel { get; set; }
    }
    public class NonCreditCAPS
    {
        [XmlElement("NonCreditCAPS_Summary")]
        public NonCreditCAPSSummary? NonCreditCAPS_Summary { get; set; }

        [XmlElement("CAPS_Application_Details")]
        [JsonConverter(typeof(SingleOrArrayConverter<CAPSApplicationDetails>))]
        public List<CAPSApplicationDetails>? CAPS_Application_Details { get; set; }
    }
    public class NonCreditCAPSSummary
    {
        [XmlElement("NonCreditCAPSLast7Days")]
        public int NonCreditCAPSLast7Days { get; set; }

        [XmlElement("NonCreditCAPSLast30Days")]
        public int NonCreditCAPSLast30Days { get; set; }

        [XmlElement("NonCreditCAPSLast90Days")]
        public int NonCreditCAPSLast90Days { get; set; }

        [XmlElement("NonCreditCAPSLast180Days")]
        public int NonCreditCAPSLast180Days { get; set; }
    }
    public class CAPS
    {
        [XmlElement("CAPS_Summary")]
        public CAPSSummary? CAPS_Summary { get; set; }

        [XmlElement("CAPS_Application_Details")]
        [JsonConverter(typeof(SingleOrArrayConverter<CAPSApplicationDetails>))]
        public List<CAPSApplicationDetails>? CAPS_Application_Details { get; set; }
    }
    public class CAPSApplicationDetails
    {
        [XmlElement("Subscriber_code")]
        public string? Subscriber_code { get; set; }

        [XmlElement("Subscriber_Name")]
        public string? Subscriber_Name { get; set; }

        [XmlElement("Date_of_Request")]
        public string? Date_of_Request { get; set; }

        [XmlElement("ReportTime")]
        public string? ReportTime { get; set; }

        [XmlElement("ReportNumber")]
        public string? ReportNumber { get; set; }

        [XmlElement("Enquiry_Reason")]
        public string? Enquiry_Reason { get; set; }

        [XmlElement("Finance_Purpose")]
        public string? Finance_Purpose { get; set; }

        [XmlElement("Amount_Financed")]
        public string? Amount_Financed { get; set; }

        [XmlElement("Duration_Of_Agreement")]
        public string? Duration_Of_Agreement { get; set; }
    }
    public class CAPSSummary
    {
        [XmlElement("CAPSLast7Days")]
        public int CAPSLast7Days { get; set; }

        [XmlElement("CAPSLast30Days")]
        public int CAPSLast30Days { get; set; }

        [XmlElement("CAPSLast90Days")]
        public int CAPSLast90Days { get; set; }

        [XmlElement("CAPSLast180Days")]
        public int CAPSLast180Days { get; set; }
    }
    public class TotalCAPSSummary
    {
        [XmlElement("TotalCAPSLast7Days")]
        public int TotalCAPSLast7Days { get; set; }

        [XmlElement("TotalCAPSLast30Days")]
        public int TotalCAPSLast30Days { get; set; }

        [XmlElement("TotalCAPSLast90Days")]
        public int TotalCAPSLast90Days { get; set; }

        [XmlElement("TotalCAPSLast180Days")]
        public int TotalCAPSLast180Days { get; set; }
    }
    public class MatchResult
    {
        [XmlElement("Exact_match")]
        public string Exact_match { get; set; }
    }
    public class CAISAccount
    {
        [XmlElement("CAIS_Summary")]
        public CAISSummary? CAIS_Summary { get; set; }

        [XmlElement("CAIS_Account_DETAILS")]
        [JsonConverter(typeof(SingleOrArrayConverter<CAISAccountDETAILS>))]
        public List<CAISAccountDETAILS>? CAIS_Account_DETAILS { get; set; }
    }
    public class CAISAccountDETAILS
    {
        [XmlElement("Identification_Number")]
        public string? Identification_Number { get; set; }

        [XmlElement("Subscriber_Name")]
        public string? Subscriber_Name { get; set; }

        [XmlElement("Account_Number")]
        public string? Account_Number { get; set; }

        [XmlElement("Portfolio_Type")]
        public string? Portfolio_Type { get; set; }

        [XmlElement("Account_Type")]
        public string? Account_Type { get; set; }

        [XmlElement("Open_Date")]
        public string?  Open_Date { get; set; }

        [XmlElement("Credit_Limit_Amount")]
        public string? Credit_Limit_Amount { get; set; }

        [XmlElement("Highest_Credit_or_Original_Loan_Amount")]
        public string? Highest_Credit_or_Original_Loan_Amount { get; set; }

        [XmlElement("Terms_Duration")]
        public string? Terms_Duration { get; set; }

        [XmlElement("Terms_Frequency")]
        public string? Terms_Frequency { get; set; }

        [XmlElement("Scheduled_Monthly_Payment_Amount")]
        public string? Scheduled_Monthly_Payment_Amount { get; set; }

        [XmlElement("Account_Status")]
        public string? Account_Status { get; set; }

        [XmlElement("Payment_Rating")]
        public string? Payment_Rating { get; set; }

        [XmlElement("Payment_History_Profile")]
        public string? Payment_History_Profile { get; set; }

        [XmlElement("Special_Comment")]
        public string? Special_Comment { get; set; }

        [XmlElement("Current_Balance")]
        public string? Current_Balance { get; set; }

        [XmlElement("Amount_Past_Due")]
        public string? Amount_Past_Due { get; set; }

        [XmlElement("Original_Charge_Off_Amount")]
        public string? Original_Charge_Off_Amount { get; set; }

        [XmlElement("Date_Reported")]
        public string? Date_Reported { get; set; }

        [XmlElement("Date_of_First_Delinquency")]
        public string? Date_of_First_Delinquency { get; set; }

        [XmlElement("Date_Closed")]
        public string? Date_Closed { get; set; }

        [XmlElement("Date_of_Last_Payment")]
        public string? Date_of_Last_Payment { get; set; }

        [XmlElement("SuitFiledWillfulDefaultWrittenOffStatus")]
        public string? SuitFiledWillfulDefaultWrittenOffStatus { get; set; }

        [XmlElement("SuitFiled_WillfulDefault")]
        public string? SuitFiled_WillfulDefault { get; set; }

        [XmlElement("Written_off_Settled_Status")]
        public string? Written_off_Settled_Status { get; set; }

        [XmlElement("Value_of_Credits_Last_Month")]
        public string? Value_of_Credits_Last_Month { get; set; }

        [XmlElement("Occupation_Code")]
        public string? Occupation_Code { get; set; }

        [XmlElement("Settlement_Amount")]
        public string? Settlement_Amount { get; set; }

        [XmlElement("Value_of_Collateral")]
        public string? Value_of_Collateral { get; set; }

        [XmlElement("Type_of_Collateral")]
        public string? Type_of_Collateral { get; set; }

        [XmlElement("Written_Off_Amt_Total")]
        public string? Written_Off_Amt_Total { get; set; }

        [XmlElement("Written_Off_Amt_Principal")]
        public string? Written_Off_Amt_Principal { get; set; }

        [XmlElement("Rate_of_Interest")]
        public string? Rate_of_Interest { get; set; }

        [XmlElement("Repayment_Tenure")]
        public string? Repayment_Tenure { get; set; }

        [XmlElement("Promotional_Rate_Flag")]
        public string? Promotional_Rate_Flag { get; set; }

        [XmlElement("Income")]
        public string? Income { get; set; }

        [XmlElement("Income_Indicator")]
        public string? Income_Indicator { get; set; }

        [XmlElement("Income_Frequency_Indicator")]
        public string? Income_Frequency_Indicator { get; set; }

        [XmlElement("DefaultStatusDate")]
        public string? DefaultStatusDate { get; set; }

        [XmlElement("LitigationStatusDate")]
        public string? LitigationStatusDate { get; set; }

        [XmlElement("WriteOffStatusDate")]
        public string? WriteOffStatusDate { get; set; }

        [XmlElement("DateOfAddition")]
        public string? DateOfAddition { get; set; }

        [XmlElement("CurrencyCode")]
        public string? CurrencyCode { get; set; }

        [XmlElement("Subscriber_comments")]
        public string? Subscriber_comments { get; set; }

        [XmlElement("Consumer_comments")]
        public string? Consumer_comments { get; set; }

        [XmlElement("AccountHoldertypeCode")]
        public string? AccountHoldertypeCode { get; set; }

        [XmlElement("CAIS_Account_History")]
        [JsonConverter(typeof(SingleOrArrayConverter<CAISAccountHistory>))]
        public List<CAISAccountHistory>? CAIS_Account_History { get; set; }

        [XmlElement("CAIS_Holder_Details")]
        [JsonConverter(typeof(SingleOrArrayConverter<CAISHolderDetails>))]
        public List<CAISHolderDetails>? CAIS_Holder_Details { get; set; }

        [XmlElement("CAIS_Holder_Address_Details")]
        [JsonConverter(typeof(SingleOrArrayConverter<CAISHolderAddressDetails>))]
        public List<CAISHolderAddressDetails>? CAIS_Holder_Address_Details { get; set; }

        [XmlElement("CAIS_Holder_Phone_Details")]
        [JsonConverter(typeof(SingleOrArrayConverter<CAISHolderPhoneDetails>))]
        public List<CAISHolderPhoneDetails>? CAIS_Holder_Phone_Details { get; set; }

        [XmlElement("CAIS_Holder_ID_Details")]
        [JsonConverter(typeof(SingleOrArrayConverter<CAISHolderIDDetails>))]
        public List<CAISHolderIDDetails>? CAIS_Holder_ID_Details { get; set; }

        [XmlElement("Account_Review_Data")]
        [JsonConverter(typeof(SingleOrArrayConverter<AccountReviewData>))]
        public List<AccountReviewData>? Account_Review_Data { get; set; }
    }
    public class AccountReviewData
    {
        [XmlElement("Year")]
        public int? Year { get; set; }

        [XmlElement("Month")]
        public int? Month { get; set; }

        [XmlElement("Account_Status")]
        public int? Account_Status { get; set; }

        [XmlElement("Actual_Payment_Amount")]
        public string? Actual_Payment_Amount { get; set; }

        [XmlElement("Current_Balance")]
        public int? Current_Balance { get; set; }

        [XmlElement("Credit_Limit_Amount")]
        public string? Credit_Limit_Amount { get; set; }

        [XmlElement("Amount_Past_Due")]
        public string? Amount_Past_Due { get; set; }

        [XmlElement("Cash_Limit")]
        public string? Cash_Limit { get; set; }

        [XmlElement("EMI_Amount")]
        public string? EMI_Amount { get; set; }
    }
    public class CAISHolderIDDetails
    {
        [XmlElement("Income_TAX_PAN")]
        public string? Income_TAX_PAN { get; set; }

        [XmlElement("PAN_Issue_Date")]
        public string? PAN_Issue_Date { get; set; }

        [XmlElement("PAN_Expiration_Date")]
        public string? PAN_Expiration_Date { get; set; }

        [XmlElement("Passport_Number")]
        public string? Passport_Number { get; set; }

        [XmlElement("Passport_Issue_Date")]
        public string? Passport_Issue_Date { get; set; }

        [XmlElement("Passport_Expiration_Date")]
        public string? Passport_Expiration_Date { get; set; }

        [XmlElement("Voter_ID_Number")]
        public string? Voter_ID_Number { get; set; }

        [XmlElement("Voter_ID_Issue_Date")]
        public string? Voter_ID_Issue_Date { get; set; }

        [XmlElement("Voter_ID_Expiration_Date")]
        public string? Voter_ID_Expiration_Date { get; set; }

        [XmlElement("Driver_License_Number")]
        public string? Driver_License_Number { get; set; }

        [XmlElement("Driver_License_Issue_Date")]
        public string? Driver_License_Issue_Date { get; set; }

        [XmlElement("Driver_License_Expiration_Date")]
        public string? Driver_License_Expiration_Date { get; set; }

        [XmlElement("Ration_Card_Number")]
        public string? Ration_Card_Number { get; set; }

        [XmlElement("Ration_Card_Issue_Date")]
        public string? Ration_Card_Issue_Date { get; set; }

        [XmlElement("Ration_Card_Expiration_Date")]
        public string? Ration_Card_Expiration_Date { get; set; }

        [XmlElement("Universal_ID_Number")]
        public string? Universal_ID_Number { get; set; }

        [XmlElement("Universal_ID_Issue_Date")]
        public string? Universal_ID_Issue_Date { get; set; }

        [XmlElement("Universal_ID_Expiration_Date")]
        public string? Universal_ID_Expiration_Date { get; set; }

        [XmlElement("EMailId")]
        public string? EMailId { get; set; }
    }
    public class CAISHolderPhoneDetails
    {
        [XmlElement("Telephone_Number")]
        public string? Telephone_Number { get; set; }

        [XmlElement("Telephone_Type")]
        public int? Telephone_Type { get; set; }

        [XmlElement("Telephone_Extension")]
        public string? Telephone_Extension { get; set; }

        [XmlElement("Mobile_Telephone_Number")]
        public string? Mobile_Telephone_Number { get; set; }

        [XmlElement("FaxNumber")]
        public string? FaxNumber { get; set; }

        [XmlElement("EMailId")]
        public string? EMailId { get; set; }
    }
    public class CAISHolderAddressDetails
    {
        [XmlElement("First_Line_Of_Address_non_normalized")]
        public string? First_Line_Of_Address_non_normalized { get; set; }

        [XmlElement("Second_Line_Of_Address_non_normalized")]
        public string? Second_Line_Of_Address_non_normalized { get; set; }

        [XmlElement("Third_Line_Of_Address_non_normalized")]
        public string? Third_Line_Of_Address_non_normalized { get; set; }

        [XmlElement("City_non_normalized")]
        public string? City_non_normalized { get; set; }

        [XmlElement("Fifth_Line_Of_Address_non_normalized")]
        public string? Fifth_Line_Of_Address_non_normalized { get; set; }

        [XmlElement("State_non_normalized")]
        public string? State_non_normalized { get; set; }

        [XmlElement("ZIP_Postal_Code_non_normalized")]
        public string? ZIP_Postal_Code_non_normalized { get; set; }

        [XmlElement("CountryCode_non_normalized")]
        public string? CountryCode_non_normalized { get; set; }

        [XmlElement("Address_indicator_non_normalized")]
        public string? Address_indicator_non_normalized { get; set; }

        [XmlElement("Residence_code_non_normalized")]
        public string? Residence_code_non_normalized { get; set; }
    }
    public class CAISHolderDetails
    {
        [XmlElement("Surname_Non_Normalized")]
        public string? Surname_Non_Normalized { get; set; }

        [XmlElement("First_Name_Non_Normalized")]
        public string? First_Name_Non_Normalized { get; set; }

        [XmlElement("Middle_Name_1_Non_Normalized")]
        public string? Middle_Name_1_Non_Normalized { get; set; }

        [XmlElement("Middle_Name_2_Non_Normalized")]
        public string? Middle_Name_2_Non_Normalized { get; set; }

        [XmlElement("Middle_Name_3_Non_Normalized")]
        public string? Middle_Name_3_Non_Normalized { get; set; }

        [XmlElement("Alias")]
        public string? Alias { get; set; }

        [XmlElement("Gender_Code")]
        public string? Gender_Code { get; set; }

        [XmlElement("Income_TAX_PAN")]
        public string? Income_TAX_PAN { get; set; }

        [XmlElement("Passport_Number")]
        public string? Passport_Number { get; set; }

        [XmlElement("Voter_ID_Number")]
        public string? Voter_ID_Number { get; set; }

        [XmlElement("Date_of_birth")]
        public string? Date_of_birth { get; set; }
    }
    public class CAISAccountHistory
    {
        [XmlElement("Year")]
        public int? Year { get; set; }

        [XmlElement("Month")]
        public int? Month { get; set; }

        [XmlElement("Days_Past_Due")]
        public int? Days_Past_Due { get; set; }

        [XmlElement("Asset_Classification")]
        public string? Asset_Classification { get; set; }
    }
    public class CAISSummary
    {
        [XmlElement("Credit_Account")]
        public CreditAccount? Credit_Account { get; set; }

        [XmlElement("Total_Outstanding_Balance")]
        public TotalOutstandingBalance? Total_Outstanding_Balance { get; set; }
    }
    public class TotalOutstandingBalance
    {
        [XmlElement("Outstanding_Balance_Secured")]
        public int Outstanding_Balance_Secured { get; set; }

        [XmlElement("Outstanding_Balance_Secured_Percentage")]
        public int Outstanding_Balance_Secured_Percentage { get; set; }

        [XmlElement("Outstanding_Balance_UnSecured")]
        public int Outstanding_Balance_UnSecured { get; set; }

        [XmlElement("Outstanding_Balance_UnSecured_Percentage")]
        public int Outstanding_Balance_UnSecured_Percentage { get; set; }

        [XmlElement("Outstanding_Balance_All")]
        public int Outstanding_Balance_All { get; set; }
    }
    public class CreditAccount
    {
        [XmlElement("CreditAccountTotal")]
        public int CreditAccountTotal { get; set; }

        [XmlElement("CreditAccountActive")]
        public int CreditAccountActive { get; set; }

        [XmlElement("CreditAccountDefault")]
        public int CreditAccountDefault { get; set; }

        [XmlElement("CreditAccountClosed")]
        public int CreditAccountClosed { get; set; }

        [XmlElement("CADSuitFiledCurrentBalance")]
        public int CADSuitFiledCurrentBalance { get; set; }
    }
    public class CurrentApplication
    {
        [XmlElement("Current_Application_Details")]
        public CurrentApplicationDetails? Current_Application_Details { get; set; }
    }
    public class CurrentApplicationDetails
    {
        [XmlElement("Enquiry_Reason")]
        public int Enquiry_Reason { get; set; }

        [XmlElement("Finance_Purpose")]
        public int Finance_Purpose { get; set; }

        [XmlElement("Amount_Financed")]
        public int Amount_Financed { get; set; }

        [XmlElement("Duration_Of_Agreement")]
        public int Duration_Of_Agreement { get; set; }

        [XmlElement("Current_Applicant_Details")]
        public CurrentApplicantDetails? Current_Applicant_Details { get; set; }

        [XmlElement("Current_Other_Details")]
        public CurrentOtherDetails? Current_Other_Details { get; set; }

        [XmlElement("Current_Applicant_Address_Details")]
        public CurrentApplicantAddressDetails? Current_Applicant_Address_Details { get; set; }

        [XmlElement("Current_Applicant_Additional_AddressDetails")]
        public string? Current_Applicant_Additional_AddressDetails { get; set; }
    }
    public class CurrentApplicantAddressDetails
    {
        [XmlElement("FlatNoPlotNoHouseNo")]
        public string? FlatNoPlotNoHouseNo { get; set; }

        [XmlElement("BldgNoSocietyName")]
        public string? BldgNoSocietyName { get; set; }

        [XmlElement("RoadNoNameAreaLocality")]
        public string? RoadNoNameAreaLocality { get; set; }

        [XmlElement("City")]
        public string? City { get; set; }

        [XmlElement("Landmark")]
        public string? Landmark { get; set; }

        [XmlElement("State")]
        public int State { get; set; }

        [XmlElement("PINCode")]
        public int PINCode { get; set; }

        [XmlElement("Country_Code")]
        public string? Country_Code { get; set; }
    }
    public class CurrentOtherDetails
    {
        [XmlElement("Income")]
        public int Income { get; set; }

        [XmlElement("Marital_Status")]
        public string? Marital_Status { get; set; }

        [XmlElement("Employment_Status")]
        public string? Employment_Status { get; set; }

        [XmlElement("Time_with_Employer")]
        public string? Time_with_Employer { get; set; }

        [XmlElement("Number_of_Major_Credit_Card_Held")]
        public string? Number_of_Major_Credit_Card_Held { get; set; }
    }
    public class CurrentApplicantDetails
    {
        [XmlElement("Last_Name")]
        public string? Last_Name { get; set; }

        [XmlElement("First_Name")]
        public string? First_Name { get; set; }

        [XmlElement("Middle_Name1")]
        public string? Middle_Name1 { get; set; }

        [XmlElement("Middle_Name2")]
        public string? Middle_Name2 { get; set; }

        [XmlElement("Middle_Name3")]
        public string? Middle_Name3 { get; set; }

        [XmlElement("Gender_Code")]
        public int Gender_Code { get; set; }

        [XmlElement("IncomeTaxPan")]
        public string? IncomeTaxPan { get; set; }

        [XmlElement("PAN_Issue_Date")]
        public string? PAN_Issue_Date { get; set; }

        [XmlElement("PAN_Expiration_Date")]
        public string? PAN_Expiration_Date { get; set; }

        [XmlElement("Passport_Number")]
        public string? Passport_Number { get; set; }

        [XmlElement("Passport_Issue_Date")]
        public string? Passport_Issue_Date { get; set; }

        [XmlElement("Passport_Expiration_Date")]
        public string? Passport_Expiration_Date { get; set; }

        [XmlElement("Voter_s_Identity_Card")]
        public string? Voter_s_Identity_Card { get; set; }

        [XmlElement("Voter_ID_Issue_Date")]
        public string? Voter_ID_Issue_Date { get; set; }

        [XmlElement("Voter_ID_Expiration_Date")]
        public string? Voter_ID_Expiration_Date { get; set; }

        [XmlElement("Driver_License_Number")]
        public string? Driver_License_Number { get; set; }

        [XmlElement("Driver_License_Issue_Date")]
        public string? Driver_License_Issue_Date { get; set; }

        [XmlElement("Driver_License_Expiration_Date")]
        public string? Driver_License_Expiration_Date { get; set; }

        [XmlElement("Ration_Card_Number")]
        public string? Ration_Card_Number { get; set; }

        [XmlElement("Ration_Card_Issue_Date")]
        public string? Ration_Card_Issue_Date { get; set; }

        [XmlElement("Ration_Card_Expiration_Date")]
        public string? Ration_Card_Expiration_Date { get; set; }

        [XmlElement("Universal_ID_Number")]
        public string? Universal_ID_Number { get; set; }

        [XmlElement("Universal_ID_Issue_Date")]
        public string? Universal_ID_Issue_Date { get; set; }

        [XmlElement("Universal_ID_Expiration_Date")]
        public string? Universal_ID_Expiration_Date { get; set; }

        [XmlElement("Date_Of_Birth_Applicant")]
        public int Date_Of_Birth_Applicant { get; set; }

        [XmlElement("Telephone_Number_Applicant_1st")]
        public string? Telephone_Number_Applicant_1st { get; set; }

        [XmlElement("Telephone_Extension")]
        public string? Telephone_Extension { get; set; }

        [XmlElement("Telephone_Type")]
        public string? Telephone_Type { get; set; }

        [XmlElement("MobilePhoneNumber")]
        public string? MobilePhoneNumber { get; set; }

        [XmlElement("EMailId")]
        public string? EMailId { get; set; }
    }
    public class CreditProfileHeader
    {
        //[XmlElement("Enquiry_Username")]
        //public string? Enquiry_Username { get; set; }

        [XmlElement("ReportDate")]
        public string? ReportDate { get; set; }

        [XmlElement("ReportTime")]
        public string? ReportTime { get; set; }

        [XmlElement("Version")]
        public string? Version { get; set; }

        [XmlElement("ReportNumber")]
        public string? ReportNumber { get; set; }

        [XmlElement("Subscriber")]
        public string? Subscriber { get; set; }

        [XmlElement("Subscriber_Name")]
        public string? Subscriber_Name { get; set; }

        [XmlElement("CustomerReferenceID")]
        public string? CustomerReferenceID { get; set; }
    }
    public class UserMessage
    {
        [XmlElement("UserMessageText")]
        public string? UserMessageText { get; set; }
    }
    public class Header
    {
        [XmlElement("SystemCode")]
        public int SystemCode { get; set; }

        [XmlElement("MessageText")]
        public string? MessageText { get; set; }

        [XmlElement("ReportDate")]
        public string? ReportDate { get; set; }

        [XmlElement("ReportTime")]
        public string? ReportTime { get; set; }
    }

    public class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<T>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            if (token.Type == JTokenType.Array)
                return token.ToObject<List<T>>();

            return new List<T> { token.ToObject<T>() };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}


