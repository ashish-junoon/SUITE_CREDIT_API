namespace JC.Criff.Highmark
{
    public static class CrifConfig
    {
        public static class UATVariables
        {
            public const string CRIF_APPID = "EARLYUATCRED25062025";
            public const string CRIF_MBRID = "NBF0003185";
            public const string CRIF_USER_ID = "user_junoon";
            public const string CRIF_PRODUCT_CODE = "BBC_CONSUMER_SCORE#85#2.0";
            public const string CRIF_PASSWORD = "521917608ECAA1CD046BF262947CEBFA20BEB67F";
            public const string CRIF_ENDPOINT = "https://test.crifhighmark.com/Inquiry/do.getSecureService/DTC/";
            public const string CRIF_CUSTOMER_NAME = "JUNOON CAPITAL SERVICES PVT LTD";
        }
        public static class ProdVariables
        {
            public const string CRIF_APPID = "JUNOBBCDETAILS11115469181";
            public const string CRIF_MBRID = "NBF0004824";
            public const string CRIF_USER_ID = "prodbbc@junooncapital";
            public const string CRIF_PRODUCT_CODE = "BBC_CONSUMER_SCORE#85#2.0";
            public const string CRIF_PASSWORD = "B7464FA22684EF3865FFF26E1EBD7650F1655449";
            public const string CRIF_ENDPOINT_URL = "https://cir.crifhighmark.com/Inquiry/do.getSecureService/DTC/";
            public const string CRIF_CUSTOMER_NAME = "JUNOON CAPITAL SERVICES PRIVATE LIMITED";
        }
    }

    public static class CrifFusionConfig
    {
        public static class UATVariables
        {
            public const string CRIF_APPID = "EARLYUATCRED25062025";
            public const string CRIF_MBRID = "NBF0003185";
            public const string CRIF_USER_ID = "chm_bbc_uat@junoon";
            public const string CRIF_PRODUCT_CODE = "BBC_CONSUMER_SCORE#85#2.0";
            public const string CRIF_PASSWORD = "6B2D69877DB14A958173C11733F2B4C20DB3FDA9";
            public const string CRIF_FUSION_ENDPOINT_URL = "https://test.crifhighmark.com/Inquiry/doGet.service/FusionService";
            public const string CRIF_CUSTOMER_NAME = "JUNOON CAPITAL SERVICES PVT LTD";
        }
        public static class ProdVariables
        {
            public const string CRIF_APPID = "JUNOBBCDETAILS11115469181";
            public const string CRIF_MBRID = "NBF0004824";
            public const string CRIF_USER_ID = "prodbbc@junooncapital";
            public const string CRIF_PRODUCT_CODE = "BBC_CONSUMER_SCORE#85#2.0";
            public const string CRIF_PASSWORD = "B7464FA22684EF3865FFF26E1EBD7650F1655449";
            public const string CRIF_FUSION_ENDPOINT_URL = "";
            public const string CRIF_CUSTOMER_NAME = "JUNOON CAPITAL SERVICES PRIVATE LIMITED";
        }
    }
}
