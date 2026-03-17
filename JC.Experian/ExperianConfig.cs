namespace JC.Experian
{
    public static class ExperianConfig
    {
        public static class UATVariables
        {
            public const string SOAP_URL = "https://connectuat.experian.in:8443/ngwsconnect/ngws";
            public const string SOAP_USERNAME = "cpu2junooncapital_uat01";
            public const string SOAP_PASSWORD = "Junoon@2025";
        }
        public static class ProdVariables
        {
            public const string SOAP_URL = "https://connect.experian.in:8443/ngwsconnect/ngws";
            public const string SOAP_USERNAME = "cpu2junooncap_prod03";
            public const string SOAP_PASSWORD = "ZbY4r8iS9j3A";
        }
    }
}
