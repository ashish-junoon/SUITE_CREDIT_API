namespace CIC.Model.Criff.Response
{
    public class FusionResponseReturn
    {
        public DateTime timestamp { get; set; }
        public string transaction_id { get; set; }
        public string status { get; set; }
        public Data data { get; set; }
        public string? message { get; set; }
    }

    public class Data
    {
        public string client_id { get; set; }
        public Cirreportdata CIRReportData { get; set; }
        public string? html_url { get; set; }
    }

    public class Cirreportdata
    {
        public Idandcontactinfo IDAndContactInfo { get; set; }
    }

    public class Idandcontactinfo
    {
        public Personalinfo PersonalInfo { get; set; }
        public Identityinfo Identityinfo { get; set; }
       
    }

    public class Identityinfo
    {
        public List<idVarition> PANId { get; set; }
        public List<idVarition> Addressinfo { get; set; }
        public List<idVarition> Phoneinfo { get; set; }
        public List<idVarition> Emailaddressinfo { get; set; }
        public List<idVarition> DobInfo { get; set; }
        public List<idVarition> Voterid { get; set; }
        public List<idVarition> NationalIdCard { get; set; }
        public List<idVarition> OtherId { get; set; }
    }
    public class Personalinfo
    {
        public Name Name { get; set; }
        public Aliasname AliasName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public Age Age { get; set; }
        public Placeofbirthinfo PlaceOfBirthInfo { get; set; }
        public string IncomeRange { get; set; }
        public string Occupation { get; set; }
    }

    public class Name
    {
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Aliasname
    {
    }

    public class Age
    {
        public int age { get; set; }
    }

    public class Placeofbirthinfo
    {
    }
    public class idVarition
    {
        public string seq { get; set; }
        public string ReportedDate { get; set; }
        public string IdNumber { get; set; }
    }
}
