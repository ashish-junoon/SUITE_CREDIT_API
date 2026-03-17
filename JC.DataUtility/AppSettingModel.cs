namespace CIC.DataUtility
{
    public class AppSettingModel
    {
        public CIC_SERVICES? CIC_SERVICES { get; set; }
        public Logcleanersettings? LogCleanerSettings { get; set; }
        public Logging? Logging { get; set; }
        public string? LogFilePath { get; set; }
        public string? AllowedHosts { get; set; }
        public Connectionstrings? ConnectionStrings { get; set; }
    }

    public class CIC_SERVICES
    {
        public bool EXPERIAN_SERVICES_PROD { get; set; }
        public bool TRANSUNION_CIBIL_SERVICES_PROD { get; set; }
        public bool CRIF_HIGHMARK_SERVICES_PROD { get; set; }
        public bool EQUIFAX_SERVICES_PROD { get; set; }
    }

    public class Logcleanersettings
    {
        public int DaysToKeep { get; set; }
        public string[]? Folders { get; set; }
    }

    public class Logging
    {
        public Loglevel? LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string? Default { get; set; }
        public string? MicrosoftAspNetCore { get; set; }
    }

    public class Connectionstrings
    {
        public string? dbconnection { get; set; }
    }

}
