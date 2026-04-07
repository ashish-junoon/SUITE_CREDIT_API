using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CIC.Model.Criff.Response
{
    #region Converter

    public class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(List<T>);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
                return token.ToObject<List<T>>(serializer);

            if (token.Type == JTokenType.Object || token.Type == JTokenType.String)
                return new List<T> { token.ToObject<T>(serializer) };

            return new List<T>();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => serializer.Serialize(writer, value);
    }

    #endregion

    public class CrifResponse
    {
        [JsonProperty("B2C-REPORT")]
        public B2CReport? B2CReport { get; set; }
        [JsonProperty("orderid")]
        public string? orderid { get; set; }
        [JsonProperty("redirectURL")]
        public string? redirectURL { get; set; }
        [JsonProperty("reportId")]
        public string? reportId { get; set; }
        [JsonProperty("status")]
        public string? status { get; set; }
        [JsonProperty("statusDesc")]
        public string? statusDesc { get; set; }
        public string? question { get; set; }
        public List<string>? optionsList { get; set; }
    }

    public class B2CReport
    {
        [JsonProperty("HEADER-SEGMENT")] public HeaderSegment? Header { get; set; }
        [JsonProperty("REQUEST-DATA")] public RequestData? RequestData { get; set; }
        [JsonProperty("REPORT-DATA")] public ReportData? ReportData { get; set; }
    }

    #region HEADER

    public class HeaderSegment
    {
        [JsonProperty("DATE-OF-REQUEST")] public string? DateOfRequest { get; set; }
        //[JsonProperty("PREPARED-FOR")] public string? PreparedFor { get; set; }
        //[JsonProperty("PREPARED-FOR-ID")] public string? PreparedForId { get; set; }
        [JsonProperty("DATE-OF-ISSUE")] public string? DateOfIssue { get; set; }
        [JsonProperty("REPORT-ID")] public string? ReportId { get; set; }
        [JsonProperty("BATCH-ID")] public string? BatchId { get; set; }
        [JsonProperty("STATUS")] public string? Status { get; set; }
        //[JsonProperty("PRODUCT-TYPE")] public string? ProductType { get; set; }
        //[JsonProperty("PRODUCT-VER")] public string? ProductVer { get; set; }
    }

    #endregion

    #region REQUEST DATA

    public class RequestData
    {
        [JsonProperty("APPLICANT-SEGMENT")] public ApplicantSegment? Applicant { get; set; }
        [JsonProperty("APPLICATION-SEGMENT")] public ApplicationSegment? Application { get; set; }
    }

    public class ApplicantSegment
    {
        [JsonProperty("FIRST-NAME")] public string? FirstName { get; set; }
        [JsonProperty("MIDDLE-NAME")] public string? MiddleName { get; set; }
        [JsonProperty("LAST-NAME")] public string? LastName { get; set; }
        [JsonProperty("GENDER")] public string? Gender { get; set; }
        [JsonProperty("APPLICANT-ID")] public string? ApplicantId { get; set; }
        [JsonProperty("DOB")] public Dob? Dob { get; set; }

        [JsonProperty("IDS"), JsonConverter(typeof(SingleOrArrayConverter<IdInfo>))]
        public List<IdInfo>? Ids { get; set; }

        [JsonProperty("ADDRESSES"), JsonConverter(typeof(SingleOrArrayConverter<Address>))]
        public List<Address>? Addresses { get; set; }

        [JsonProperty("PHONES"), JsonConverter(typeof(SingleOrArrayConverter<Phone>))]
        public List<Phone>? Phones { get; set; }

        [JsonProperty("EMAILS"), JsonConverter(typeof(SingleOrArrayConverter<EmailInfo>))]
        public List<EmailInfo>? Emails { get; set; }

        [JsonProperty("ACCOUNT-NUMBER")] public string? AccountNumber { get; set; }
    }

    public class ApplicationSegment
    {
        [JsonProperty("INQUIRY-UNIQUE-REF-NO")] public string? InquiryUniqueRefNo { get; set; }
        [JsonProperty("CREDIT-RPT-ID")] public string? CreditRptId { get; set; }
        [JsonProperty("CREDIT-RPT-TRN-DT-TM")] public string? CreditRptTrnDtTm { get; set; }
        [JsonProperty("CREDIT-INQ-PURPS-TYPE")] public string? CreditInqPurpsType { get; set; }
        [JsonProperty("CREDIT-INQUIRY-STAGE")] public string? CreditInquiryStage { get; set; }
        [JsonProperty("CLIENT-CONTRIBUTOR-ID")] public string? ClientContributorId { get; set; }
        [JsonProperty("BRANCH-ID")] public string? BranchId { get; set; }
        [JsonProperty("APPLICATION-ID")] public string? ApplicationId { get; set; }
        [JsonProperty("ACNT-OPEN-DT")] public string? AcntOpenDt { get; set; }
        [JsonProperty("LOAN-AMT")] public string? LoanAmt { get; set; }
        [JsonProperty("LTV")] public string? Ltv { get; set; }
        [JsonProperty("TERM")] public string? Term { get; set; }
        [JsonProperty("LOAN-TYPE")] public string? LoanType { get; set; }
    }

    public class Dob
    {
        [JsonProperty("DOB-DT")] public string? Date { get; set; }
        [JsonProperty("AGE")] public string? Age { get; set; }
        [JsonProperty("AGE-AS-ON")] public string? AgeAsOn { get; set; }
    }

    public class IdInfo
    {
        [JsonProperty("TYPE")] public string? Type { get; set; }
        [JsonProperty("VALUE")] public string? Value { get; set; }
    }

    public class Address
    {
        [JsonProperty("TYPE")] public string? Type { get; set; }
        [JsonProperty("ADDRESSTEXT")] public string? AddressText { get; set; }
        [JsonProperty("CITY")] public string? City { get; set; }
        [JsonProperty("LOCALITY")] public string? Locality { get; set; }
        [JsonProperty("STATE")] public string? State { get; set; }
        [JsonProperty("PIN")] public string? Pin { get; set; }
        [JsonProperty("COUNTRY")] public string? Country { get; set; }
    }

    public class Phone
    {
        [JsonProperty("TYPE")] public string? Type { get; set; }
        [JsonProperty("VALUE")] public string? Value { get; set; }
    }

    public class EmailInfo
    {
        [JsonProperty("EMAIL")] public string? Email { get; set; }
    }

    #endregion

    #region REPORT DATA

    public class ReportData
    {
        [JsonProperty("STANDARD-DATA")] public StandardData? StandardData { get; set; }
        [JsonProperty("REQUESTED-SERVICES")] public object? RequestedServices { get; set; }
        [JsonProperty("ACCOUNTS-SUMMARY")] public AccountsSummary? AccountsSummary { get; set; }
        [JsonProperty("TRENDS")] public Trends? Trends { get; set; }

        [JsonProperty("ALERTS"), JsonConverter(typeof(SingleOrArrayConverter<Alert>))]
        public List<Alert>? Alerts { get; set; }
    }

    #endregion

    #region STANDARD DATA

    public class StandardData
    {
        [JsonProperty("DEMOGS")] public Demogs? Demogs { get; set; }

        [JsonProperty("EMPLOYMENT-DETAILS"), JsonConverter(typeof(SingleOrArrayConverter<EmploymentDetail>))]
        public List<EmploymentDetail>? EmploymentDetails { get; set; }

        [JsonProperty("TRADELINES"), JsonConverter(typeof(SingleOrArrayConverter<Tradeline>))]
        public List<Tradeline>? Tradelines { get; set; }

        [JsonProperty("INQUIRY-HISTORY"), JsonConverter(typeof(SingleOrArrayConverter<InquiryHistory>))]
        public List<InquiryHistory>? InquiryHistory { get; set; }

        [JsonProperty("SCORE"), JsonConverter(typeof(SingleOrArrayConverter<Score>))]
        public List<Score>? Score { get; set; }
    }

    #endregion

    #region DEMOGRAPHICS

    public class Demogs
    {
        [JsonProperty("VARIATIONS"), JsonConverter(typeof(SingleOrArrayConverter<VariationType>))]
        public List<VariationType>? Variations { get; set; }
    }

    public class VariationType
    {
        [JsonProperty("TYPE")] public string? Type { get; set; }

        [JsonProperty("VARIATION"), JsonConverter(typeof(SingleOrArrayConverter<Variation>))]
        public List<Variation>? Variation { get; set; }
    }

    public class Variation
    {
        [JsonProperty("VALUE")] public string? Value { get; set; }
        [JsonProperty("REPORTED-DT")] public string? ReportedDate { get; set; }
        [JsonProperty("FIRST-REPORTED-DT")] public string? FirstReportedDate { get; set; }
        [JsonProperty("LOAN-TYPE-ASSOC")] public string? LoanTypeAssoc { get; set; }
        [JsonProperty("SOURCE-INDICATOR")] public string? SourceIndicator { get; set; }
    }

    #endregion

    #region EMPLOYMENT DETAILS

    public class EmploymentDetail
    {
        // Add properties based on your JSON structure
        // The provided JSON shows empty array for EMPLOYMENT-DETAILS
    }

    #endregion

    #region TRADELINES

    public class Tradeline
    {
        [JsonProperty("ACCT-NUMBER")] public string? AccountNumber { get; set; }
        [JsonProperty("CREDIT-GRANTOR")] public string? CreditGrantor { get; set; }
        [JsonProperty("CREDIT-GRANTOR-GROUP")] public string? CreditGrantorGroup { get; set; }
        [JsonProperty("CREDIT-GRANTOR-TYPE")] public string? CreditGrantorType { get; set; }
        [JsonProperty("ACCT-TYPE")] public string? AccountType { get; set; }
        [JsonProperty("REPORTED-DT")] public string? ReportedDate { get; set; }
        [JsonProperty("OWNERSHIP-TYPE")] public string? OwnershipType { get; set; }
        [JsonProperty("ACCOUNT-STATUS")] public string? AccountStatus { get; set; }
        [JsonProperty("CLOSED-DT")] public string? ClosedDate { get; set; }
        [JsonProperty("DISBURSED-AMT")] public string? DisbursedAmount { get; set; }
        [JsonProperty("DISBURSED-DT")] public string? DisbursedDate { get; set; }
        [JsonProperty("INSTALLMENT-AMT")] public string? InstallmentAmount { get; set; }
        [JsonProperty("CREDIT-LIMIT")] public string? CreditLimit { get; set; }
        [JsonProperty("CASH-LIMIT")] public string? CashLimit { get; set; }
        [JsonProperty("CURRENT-BAL")] public string? CurrentBalance { get; set; }
        [JsonProperty("INSTALLMENT-FREQUENCY")] public string? InstallmentFrequency { get; set; }
        [JsonProperty("ORIGINAL-TERM")] public int? OriginalTerm { get; set; }
        [JsonProperty("TERM-TO-MATURITY")] public int? TermToMaturity { get; set; }
        [JsonProperty("REPAYMENT-TENURE")] public string? RepaymentTenure { get; set; }
        [JsonProperty("INTEREST-RATE")] public string? InterestRate { get; set; }
        [JsonProperty("ACTUAL-PAYMENT")] public string? ActualPayment { get; set; }
        [JsonProperty("LAST-PAYMENT-DT")] public string? LastPaymentDate { get; set; }
        [JsonProperty("OVERDUE-AMT")] public string? OverdueAmount { get; set; }
        [JsonProperty("WRITE-OFF-AMT")] public string? WriteOffAmount { get; set; }
        [JsonProperty("PRINCIPAL-WRITE-OFF-AMT")] public string? PrincipalWriteOffAmount { get; set; }
        [JsonProperty("SETTLEMENT-AMT")] public string? SettlementAmount { get; set; }
        [JsonProperty("OBLIGATION")] public string? Obligation { get; set; }
        [JsonProperty("SECURITY-STATUS")] public string? SecurityStatus { get; set; }
        [JsonProperty("ACCT-IN-DISPUTE")] public string? AccountInDispute { get; set; }
        [JsonProperty("SUIT-FILED-WILFUL-DEFAULT-STATUS")] public string? SuitFiledWilfulDefaultStatus { get; set; }
        [JsonProperty("WRITTEN-OFF-SETTLED-STATUS")] public string? WrittenOffSettledStatus { get; set; }
        [JsonProperty("WRITE-OFF-DT")] public string? WriteOffDate { get; set; }
        [JsonProperty("SUIT-FILED-DT")] public string? SuitFiledDate { get; set; }
        [JsonProperty("LAST-PAID-AMOUNT")] public string? LastPaidAmount { get; set; }
        [JsonProperty("OCCUPATION")] public string? Occupation { get; set; }
        [JsonProperty("INCOME-FREQUENCY")] public string? IncomeFrequency { get; set; }
        [JsonProperty("INCOME-AMOUNT")] public string? IncomeAmount { get; set; }
        [JsonProperty("ACCOUNT-REMARKS")] public string? AccountRemarks { get; set; }

        [JsonProperty("HISTORY"), JsonConverter(typeof(SingleOrArrayConverter<History>))]
        public List<History>? History { get; set; }

        [JsonProperty("SECURITY-DETAILS"), JsonConverter(typeof(SingleOrArrayConverter<SecurityDetail>))]
        public List<SecurityDetail>? SecurityDetails { get; set; }

        [JsonProperty("LINKED-ACCOUNTS"), JsonConverter(typeof(SingleOrArrayConverter<LinkedAccount>))]
        public List<LinkedAccount>? LinkedAccounts { get; set; }
    }

    public class History
    {
        [JsonProperty("NAME")] public string? Name { get; set; }
        [JsonProperty("DATES")] public string? Dates { get; set; }
        [JsonProperty("VALUES")] public string? Values { get; set; }
    }

    public class SecurityDetail
    {
        [JsonProperty("SECURITY-TYPE")] public string? SecurityType { get; set; }
        [JsonProperty("OWNER-NAME")] public string? OwnerName { get; set; }
        [JsonProperty("SECURITY-VALUATION")] public string? SecurityValuation { get; set; }
        [JsonProperty("DATE-OF-VALUATION")] public string? DateOfValuation { get; set; }
        [JsonProperty("SECURITY-CHARGE")] public string? SecurityCharge { get; set; }
        [JsonProperty("PROPERTY-ADDRESS")] public string? PropertyAddress { get; set; }
        [JsonProperty("AUTOMOBILE-TYPE")] public string? AutomobileType { get; set; }
        [JsonProperty("YEAR-OF-MANUFACTURING")] public string? YearOfManufacturing { get; set; }
        [JsonProperty("REGISTRATION-NUMBER")] public string? RegistrationNumber { get; set; }
        [JsonProperty("ENGINE-NUMBER")] public string? EngineNumber { get; set; }
        [JsonProperty("CHASSIE-NUMBER")] public string? ChassisNumber { get; set; }
    }

    public class LinkedAccount
    {
        // Add properties based on your JSON structure
        // The provided JSON shows empty arrays for LINKED-ACCOUNTS
    }

    #endregion

    #region INQUIRY HISTORY

    public class InquiryHistory
    {
        // Add properties based on your JSON structure
        // The provided JSON shows empty array for INQUIRY-HISTORY
    }

    #endregion

    #region SCORE

    public class Score
    {
        [JsonProperty("NAME")] public string? Name { get; set; }
        [JsonProperty("VERSION")] public string? Version { get; set; }
        [JsonProperty("VALUE")] public string? Value { get; set; }
        [JsonProperty("DESCRIPTION")] public string? Description { get; set; }

        [JsonProperty("FACTORS"), JsonConverter(typeof(SingleOrArrayConverter<ScoreFactor>))]
        public List<ScoreFactor>? Factors { get; set; }
    }

    public class ScoreFactor
    {
        [JsonProperty("TYPE")] public string? Type { get; set; }
        [JsonProperty("DESC")] public string? Description { get; set; }
    }

    #endregion

    #region ACCOUNTS SUMMARY

    public class AccountsSummary
    {
        [JsonProperty("PRIMARY-ACCOUNTS-SUMMARY")] public PrimaryAccountsSummary? PrimaryAccountsSummary { get; set; }
        [JsonProperty("SECONDARY-ACCOUNTS-SUMMARY")] public SecondaryAccountsSummary? SecondaryAccountsSummary { get; set; }
        [JsonProperty("MFI-GROUP-ACCOUNTS-SUMMARY")] public MfiGroupAccountsSummary? MfiGroupAccountsSummary { get; set; }

        [JsonProperty("ADDITIONAL-SUMMARY"), JsonConverter(typeof(SingleOrArrayConverter<AdditionalSummary>))]
        public List<AdditionalSummary>? AdditionalSummary { get; set; }

        [JsonProperty("PERFORM-ATTRIBUTES"), JsonConverter(typeof(SingleOrArrayConverter<object>))]
        public List<object>? PerformAttributes { get; set; }
    }

    public class PrimaryAccountsSummary
    {
        [JsonProperty("NUMBER-OF-ACCOUNTS")] public string? NumberOfAccounts { get; set; }
        [JsonProperty("ACTIVE-ACCOUNTS")] public string? ActiveAccounts { get; set; }
        [JsonProperty("OVERDUE-ACCOUNTS")] public string? OverdueAccounts { get; set; }
        [JsonProperty("SECURED-ACCOUNTS")] public string? SecuredAccounts { get; set; }
        [JsonProperty("UNSECURED-ACCOUNTS")] public string? UnsecuredAccounts { get; set; }
        [JsonProperty("UNTAGGED-ACCOUNTS")] public string? UntaggedAccounts { get; set; }
        [JsonProperty("TOTAL-CURRENT-BALANCE")] public string? TotalCurrentBalance { get; set; }
        [JsonProperty("CURRENT-BALANCE-SECURED")] public string? CurrentBalanceSecured { get; set; }
        [JsonProperty("CURRENT-BALANCE-UNSECURED")] public string? CurrentBalanceUnsecured { get; set; }
        [JsonProperty("TOTAL-SANCTIONED-AMT")] public string? TotalSanctionedAmount { get; set; }
        [JsonProperty("TOTAL-DISBURSED-AMT")] public string? TotalDisbursedAmount { get; set; }
        [JsonProperty("TOTAL-AMT-OVERDUE")] public string? TotalAmountOverdue { get; set; }
    }

    public class SecondaryAccountsSummary
    {
        [JsonProperty("NUMBER-OF-ACCOUNTS")] public string? NumberOfAccounts { get; set; }
        [JsonProperty("ACTIVE-ACCOUNTS")] public string? ActiveAccounts { get; set; }
        [JsonProperty("OVERDUE-ACCOUNTS")] public string? OverdueAccounts { get; set; }
        [JsonProperty("SECURED-ACCOUNTS")] public string? SecuredAccounts { get; set; }
        [JsonProperty("UNSECURED-ACCOUNTS")] public string? UnsecuredAccounts { get; set; }
        [JsonProperty("UNTAGGED-ACCOUNTS")] public string? UntaggedAccounts { get; set; }
        [JsonProperty("TOTAL-CURRENT-BALANCE")] public string? TotalCurrentBalance { get; set; }
        [JsonProperty("TOTAL-SANCTIONED-AMT")] public string? TotalSanctionedAmount { get; set; }
        [JsonProperty("TOTAL-DISBURSED-AMT")] public string? TotalDisbursedAmount { get; set; }
        [JsonProperty("TOTAL-AMT-OVERDUE")] public string? TotalAmountOverdue { get; set; }
    }

    public class MfiGroupAccountsSummary
    {
        [JsonProperty("NUMBER-OF-ACCOUNTS")] public string? NumberOfAccounts { get; set; }
        [JsonProperty("ACTIVE-ACCOUNTS")] public string? ActiveAccounts { get; set; }
        [JsonProperty("OVERDUE-ACCOUNTS")] public string? OverdueAccounts { get; set; }
        [JsonProperty("CLOSED-ACCOUNTS")] public string? ClosedAccounts { get; set; }
        [JsonProperty("NO-OF-OTHER-MFIS")] public string? NumberOfOtherMfis { get; set; }
        [JsonProperty("NO-OF-OWN-MFIS")] public string? NumberOfOwnMfis { get; set; }
        [JsonProperty("TOTAL-OWN-CURRENT-BALANCE")] public string? TotalOwnCurrentBalance { get; set; }
        [JsonProperty("TOTAL-OWN-INSTALLMENT-AMT")] public string? TotalOwnInstallmentAmount { get; set; }
        [JsonProperty("TOTAL-OWN-DISBURSED-AMT")] public string? TotalOwnDisbursedAmount { get; set; }
        [JsonProperty("TOTAL-OWN-OVERDUE-AMT")] public string? TotalOwnOverdueAmount { get; set; }
        [JsonProperty("TOTAL-OTHER-CURRENT-BALANCE")] public string? TotalOtherCurrentBalance { get; set; }
        [JsonProperty("TOTAL-OTHER-INSTALLMENT-AMT")] public string? TotalOtherInstallmentAmount { get; set; }
        [JsonProperty("TOTAL-OTHER-DISBURSED-AMT")] public string? TotalOtherDisbursedAmount { get; set; }
        [JsonProperty("TOTAL-OTHER-OVERDUE-AMT")] public string? TotalOtherOverdueAmount { get; set; }
        [JsonProperty("MAX-WORST-DELINQUENCY")] public string? MaxWorstDelinquency { get; set; }
    }

    public class AdditionalSummary
    {
        [JsonProperty("ATTR-NAME")] public string? AttrName { get; set; }
        [JsonProperty("ATTR-VALUE")] public string? AttrValue { get; set; }
    }

    #endregion

    #region TRENDS

    public class Trends
    {
        [JsonProperty("NAME")] public string? Name { get; set; }
        [JsonProperty("DATES")] public string? Dates { get; set; }
        [JsonProperty("VALUES")] public string? Values { get; set; }
        [JsonProperty("RESERVED1")] public string? Reserved1 { get; set; }
        [JsonProperty("RESERVED2")] public string? Reserved2 { get; set; }
        [JsonProperty("RESERVED3")] public string? Reserved3 { get; set; }
        [JsonProperty("DESCRIPTION")] public string? Description { get; set; }
    }

    #endregion

    #region ALERTS

    public class Alert
    {
        // Add properties based on your JSON structure
        // The provided JSON shows empty array for ALERTS
    }

    #endregion
}