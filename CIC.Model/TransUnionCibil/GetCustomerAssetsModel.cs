using System.Text.Json;
using System.Text.Json.Serialization;

namespace CIC.Model.TransUnionCibil
{
    public class GetCustomerAssetsModel
    {
        public Getcustomerassetsresponse? GetCustomerAssetsResponse { get; set; }
    }

    public class Getcustomerassetsresponse
    {
        public string? ResponseStatus { get; set; }
        public string? ResponseKey { get; set; }
        public Getcustomerassetssuccess? GetCustomerAssetsSuccess { get; set; }
    }

    public class Getcustomerassetssuccess
    {
        public Asset? Asset { get; set; }
    }

    public class Asset
    {
        public string? Status { get; set; }
        public bool SafetyCheckFailure { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public Truelinkcreditreport? TrueLinkCreditReport { get; set; }
        public string? AssetId { get; set; }
        public string? Type { get; set; }
    }

    public class Truelinkcreditreport
    {
        public string? ReferenceKey { get; set; }
        public string? currentversion { get; set; }
        public Borrower? Borrower { get; set; }
        public bool SafetyCheckPassed { get; set; }
        public string? Frozen { get; set; }
        public Message? Message { get; set; }
        public Sources? Sources { get; set; }
        public bool DeceasedIndicator { get; set; }
        public bool FraudIndicator { get; set; }
        public List<Tradelinepartition>? TradeLinePartition { get; set; }
        public List<Inquirypartition>? InquiryPartition { get; set; }
    }

    public class Borrower
    {
        public Birth? Birth { get; set; }
        public string? borrowerKey { get; set; }
        [JsonConverter(typeof(SingleOrArrayConverter<Borroweraddress>))]
        public List<Borroweraddress>? BorrowerAddress { get; set; }
        public Creditscore? CreditScore { get; set; }
        public Employer? Employer { get; set; }
        public string? Gender { get; set; }
        public Creditstatement? CreditStatement { get; set; }
        [JsonConverter(typeof(SingleOrArrayConverter<Emailaddress>))]
        public List<Emailaddress>? EmailAddress { get; set; }
        public Identifierpartition? IdentifierPartition { get; set; }
        public Borrowername? BorrowerName { get; set; }
        [JsonConverter(typeof(SingleOrArrayConverter<Borrowertelephone>))]
        public List<Borrowertelephone>? BorrowerTelephone { get; set; }
    }

    public class Birth
    {
        public string? date { get; set; }
        public string? partitionSet { get; set; }
        public Birthdate? BirthDate { get; set; }
        public Source? Source { get; set; }
        public string? age { get; set; }
    }

    public class Birthdate
    {
        public string? month { get; set; }
        public string? year { get; set; }
        public string? day { get; set; }
    }

    public class Source
    {
        public string? Reference { get; set; }
        public string? InquiryDate { get; set; }
        public string? Locale { get; set; }
        public string? BorrowerKey { get; set; }
        public Bureau Bureau { get; set; }
    }

    public class Bureau
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Creditscore
    {
        public Creditscoremodel? CreditScoreModel { get; set; }
        [JsonConverter(typeof(SingleOrArrayConverter<Creditscorefactor>))]
        public List<Creditscorefactor>? CreditScoreFactor { get; set; }
        public Noscorereason? NoScoreReason { get; set; }
        public string? riskScore { get; set; }
        public string? populationRank { get; set; }
        public Source1? Source { get; set; }
        public string? scoreName { get; set; }
    }

    public class Creditscoremodel
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Noscorereason
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Source1
    {
        public string? Reference { get; set; }
        public string? InquiryDate { get; set; }
        public string? Locale { get; set; }
        public string? BorrowerKey { get; set; }
        public Bureau? Bureau { get; set; }
    }

    

    public class Creditscorefactor
    {
        public string? bureauCode { get; set; }
        public Factor? Factor { get; set; }
        public List<string>? FactorText { get; set; }
    }

    public class Factor
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Employer
    {
        public string? serialNumber { get; set; }
        public string? IncomeFreqIndicator { get; set; }
        public string? NetGrossIndicator { get; set; }
        public string? dateReported { get; set; }
        public string? name { get; set; }
        public Occupationcode? OccupationCode { get; set; }
        public string? partitionSet { get; set; }
        public Creditaddress? CreditAddress { get; set; }
        public Source2? Source { get; set; }
        public string? account { get; set; }
    }

    public class Occupationcode
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Creditaddress
    {
        public string? AddressType { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Region { get; set; }
    }

    public class Source2
    {
        public string? Reference { get; set; }
        public string? InquiryDate { get; set; }
        public string? Locale { get; set; }
        public string? BorrowerKey { get; set; }
        public Bureau Bureau { get; set; }
    }

   

    public class Creditstatement
    {
        public Statementtype? StatementType { get; set; }
        public string? statement { get; set; }
        public Source3? Source { get; set; }
        public string? dateUpdated { get; set; }
    }

    public class Statementtype
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Source3
    {
        public string? Reference { get; set; }
        public string? InquiryDate { get; set; }
        public string? Locale { get; set; }
        public string? BorrowerKey { get; set; }
        public Bureau? Bureau { get; set; }
    }

    

    public class Identifierpartition
    {
        public List<Identifier> Identifier { get; set; }
    }

    public class Identifier
    {
        public ID? ID { get; set; }
        public Source4? Source { get; set; }
        public string? enrichMode { get; set; }
    }

    public class ID
    {
        public string? Id { get; set; }
        public string? IdentifierName { get; set; }
        public string? SerialNumber { get; set; }
    }

    public class Source4
    {
        public string? Reference { get; set; }
        public string? InquiryDate { get; set; }
        public string? Locale { get; set; }
        public string? BorrowerKey { get; set; }
        public Bureau? Bureau { get; set; }
    }

   
    public class Borrowername
    {
        public BorrowName? Name { get; set; }
        public string? partitionSet { get; set; }
        public Nametype? NameType { get; set; }
        public Source5? Source { get; set; }
    }

    public class BorrowName
    {
        public string? Forename { get; set; }
    }

    public class Nametype
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Source5
    {
        public string? Reference { get; set; }
        public string? InquiryDate { get; set; }
        public string? Locale { get; set; }
        public string? BorrowerKey { get; set; }
        public Bureau? Bureau { get; set; }
    }

   
    public class Borroweraddress
    {
        public Dwelling? Dwelling { get; set; }
        public Ownership? Ownership { get; set; }
        public string? addressOrder { get; set; }
        public string? dateReported { get; set; }
        public string? partitionSet { get; set; }
        public Creditaddress1? CreditAddress { get; set; }
        public Origin? Origin { get; set; }
        public string? enrichMode { get; set; }
        public Source6? Source { get; set; }
    }

    public class Dwelling
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Ownership
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Creditaddress1
    {
        public string? SerialNumber { get; set; }
        public string? AddressType { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Region { get; set; }
    }

    public class Origin
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Source6
    {
        public string? Reference { get; set; }
        public string? InquiryDate { get; set; }
        public string? Locale { get; set; }
        public string? BorrowerKey { get; set; }
        public Bureau? Bureau { get; set; }
    }

   

    public class Emailaddress
    {
        public string? serialNumber { get; set; }
        public string? Email { get; set; }
    }

    public class Borrowertelephone
    {
        public string? partitionSet { get; set; }
        public BorrowPhonenumber? PhoneNumber { get; set; }
        public string? enrichMode { get; set; }
        public Source7? Source { get; set; }
        public Phonetype? PhoneType { get; set; }
    }

    public class BorrowPhonenumber
    {
        public string? SerialNumber { get; set; }
        public string? Number { get; set; }
    }

    public class Source7
    {
        public string? Reference { get; set; }
        public string? InquiryDate { get; set; }
        public string? Locale { get; set; }
        public string? BorrowerKey { get; set; }
        public Bureau? Bureau { get; set; }
    }

    

    public class Phonetype
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Message
    {
        public Code? Code { get; set; }
        public string? text { get; set; }
        public Type? Type { get; set; }
    }

    public class Code
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Type
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Sources
    {
        public Source8? Source { get; set; }
    }

    public class Source8
    {
        public DateTime? InquiryDate { get; set; }
        public Bureau? Bureau { get; set; }
        public string? OriginalData { get; set; }
    }

   

    public class Tradelinepartition
    {
        public string? accountTypeAbbreviation { get; set; }
        public string? accountTypeDescription { get; set; }
        public string? accountTypeSymbol { get; set; }
        public Tradeline? Tradeline { get; set; }
    }

    public class Tradeline
    {
        public Disputeflag? DisputeFlag { get; set; }
        public string? creditorName { get; set; }
        public string? dateClosed { get; set; }
        public string? branch { get; set; }
        public string? highBalance { get; set; }
        public string? dateOpened { get; set; }
        public string? dateReported { get; set; }
        public string? accountsSoldTo { get; set; }
        public string? bureau { get; set; }
        public string? writtenOffAmtTotal { get; set; }
        public Paystatus PayStatus { get; set; }
        public string? currentBalance { get; set; }
        public string? subscriberCode { get; set; }
        public Accountdesignator? AccountDesignator { get; set; }
        public string? accountNumber { get; set; }
        public Accountcondition? AccountCondition { get; set; }
        public Source9? Source { get; set; }
        public Industrycode? IndustryCode { get; set; }
        public string? thirdPartyName { get; set; }
        public Grantedtrade? GrantedTrade { get; set; }
        public Verificationindicator? VerificationIndicator { get; set; }
        public string? noOfParticipants { get; set; }
        public Openclosed? OpenClosed { get; set; }
        public string? position { get; set; }
        public string? settlementAmount { get; set; }
        public string? writtenOffPrincipal { get; set; }
        public string? dateAccountStatus { get; set; }
    }

    public class Disputeflag
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Paystatus
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Accountdesignator
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Accountcondition
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Source9
    {
        public string? Reference { get; set; }
        public string? InquiryDate { get; set; }
        public string? Locale { get; set; }
        public string? BorrowerKey { get; set; }
        public Bureau? Bureau { get; set; }
    }

   

    public class Industrycode
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Grantedtrade
    {
        public string? interestRate { get; set; }
        public Paymentfrequency? PaymentFrequency { get; set; }
        public string? serialNumber { get; set; }
        public Paystatushistory? PayStatusHistory { get; set; }
        public string? EMIAmount { get; set; }
        public Worstpaystatus? WorstPayStatus { get; set; }
        public string? CashLimit { get; set; }
        public Accounttype? AccountType { get; set; }
        public Credittype? CreditType { get; set; }
        public string? actualPaymentAmount { get; set; }
        public string? termMonths { get; set; }
        public string? CreditLimit { get; set; }
        public Collateraltype? CollateralType { get; set; }
        public string? amountPastDue { get; set; }
        public string? collateral { get; set; }
        public Termtype? TermType { get; set; }
        public string? dateLastPayment { get; set; }
    }

    public class Paymentfrequency
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Paystatushistory
    {
        public string? endDate { get; set; }
        public string? startDate { get; set; }
        [JsonConverter(typeof(SingleOrArrayConverter<Monthlypaystatu>))]
        public object? MonthlyPayStatus { get; set; }
        public string? status { get; set; }
    }

    public class Monthlypaystatu
    {
        public string? date { get; set; }
        public string? status { get; set; }
    }

    public class Worstpaystatus
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Accounttype
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Credittype
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Collateraltype
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Termtype
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Verificationindicator
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Openclosed
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Inquirypartition
    {
        public Inquiry? Inquiry { get; set; }
    }

    public class Inquiry
    {
        public string? amount { get; set; }
        public Industrycode1? IndustryCode { get; set; }
        public string? inquiryType { get; set; }
        public string? subscriberName { get; set; }
        public string? enqControlNum { get; set; }
        public string? description { get; set; }
        public string? subscriberNumber { get; set; }
        public string? bureau { get; set; }
        public Source10? Source { get; set; }
        public string? inquiryDate { get; set; }
    }

    public class Industrycode1
    {
        public string? symbol { get; set; }
        public string? description { get; set; }
        public string? rank { get; set; }
        public string? abbreviation { get; set; }
    }

    public class Source10
    {
        public string? Reference { get; set; }
        public string? InquiryDate { get; set; }
        public string? Locale { get; set; }
        public string? BorrowerKey { get; set; }
        public Bureau? Bureau { get; set; }
    }


    
}

public class SingleOrArrayConverter<T> : JsonConverter<List<T>>
{
    public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var list = new List<T>();

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            list = JsonSerializer.Deserialize<List<T>>(ref reader, options);
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            var item = JsonSerializer.Deserialize<T>(ref reader, options);
            list.Add(item);
        }

        return list;
    }

    public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
