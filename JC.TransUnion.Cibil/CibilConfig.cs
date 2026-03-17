using JC.TransUnion.Cibil.Models;

namespace JC.TransUnion.Cibil
{
    public static class CibilConfig
    {
        public static ConfigModel GetCibilModel(bool IsProd)
        {
            if (IsProd)
            {
                return new ConfigModel
                {
                    SITE_NAME = ProdVariables.SITE_NAME,
                    ACCOUNT_NAME = ProdVariables.ACCOUNT_NAME,
                    ACCOUNT_CODE = ProdVariables.ACCOUNT_CODE,
                    PRODUCT_CONFIG_ID = ProdVariables.PRODUCT_CONFIG_ID,
                    MEMBER_REF_ID = ProdVariables.MEMBER_REF_ID,
                    HYBRID_BASE_URL = ProdVariables.HYBRID_BASE_URL,
                    HYBRID_TOKEN_URL = ProdVariables.HYBRID_TOKEN_URL,
                    API_KEY = ProdVariables.API_KEY,
                    CLIENT_SECRET = ProdVariables.CLIENT_SECRET,
                    MEMBER_PFX_CERT_PATH = ProdVariables.MEMBER_PFX_CERT_PATH,
                    MEMBER_PFX_PASSWORD = ProdVariables.MEMBER_PFX_PASSWORD,
                    PUBLIC_CERT_PATH = ProdVariables.PUBLIC_CERT_PATH,
                    PRIVATE_KEY_PATH = ProdVariables.PRIVATE_KEY_PATH,
                    //PRIVATE_KEY_PASSPHRASE = ProdVariables.PRIVATE_KEY_PASSPHRASE,
                    WEBTOKEN_BASE_URL = ProdVariables.WEBTOKEN_BASE_URL,
                };
            }
            else
            {
                return new ConfigModel
                {
                    SITE_NAME = UATVariables.SITE_NAME,
                    ACCOUNT_NAME = UATVariables.ACCOUNT_NAME,
                    ACCOUNT_CODE = UATVariables.ACCOUNT_CODE,
                    PRODUCT_CONFIG_ID = UATVariables.PRODUCT_CONFIG_ID,
                    MEMBER_REF_ID = UATVariables.MEMBER_REF_ID,
                    HYBRID_BASE_URL = UATVariables.HYBRID_BASE_URL,
                    HYBRID_TOKEN_URL = UATVariables.HYBRID_TOKEN_URL,
                    API_KEY = UATVariables.API_KEY,
                    CLIENT_SECRET = UATVariables.CLIENT_SECRET,
                    MEMBER_PFX_CERT_PATH = UATVariables.MEMBER_PFX_CERT_PATH,
                    MEMBER_PFX_PASSWORD = UATVariables.MEMBER_PFX_PASSWORD,
                    PUBLIC_CERT_PATH = UATVariables.PUBLIC_CERT_PATH,
                    PRIVATE_KEY_PATH = UATVariables.PRIVATE_KEY_PATH,
                    TLS_CACERT = UATVariables.TLS_CACERT,
                    //PRIVATE_KEY_PASSPHRASE = UATVariables.PRIVATE_KEY_PASSPHRASE,
                    WEBTOKEN_BASE_URL = UATVariables.WEBTOKEN_BASE_URL
                };

            }
        }
        private static class UATVariables
        {
            public const string SITE_NAME = "JunoonCapita";
            public const string ACCOUNT_NAME = "GCVD_JunoonCapita";
            public const string ACCOUNT_CODE = "R0NWRF9KdW5vb25DYXBp";

            public const string PRODUCT_CONFIG_ID = "JUNOON01";
            public const string MEMBER_REF_ID = "NB3135";
            public const string HYBRID_BASE_URL = "https://apiuat.cibilhawk.com/consumer/dtc/v1/hybrid";
            public const string HYBRID_TOKEN_URL = "https://apiuat.cibilhawk.com/auth/1.0/token";
            public const string WEBTOKEN_BASE_URL = "https://atlasls-in-live.sd.demo.truelink.com/CreditView/webtokenasset.page";
            public const string API_KEY = "l71fa4dfbecbb04345aecd1361b2c89fcc";
            public const string CLIENT_SECRET = "6fe82e8bb998425a8c1465de03829d07";

            //--------------Certificate Paths and Passwords----------------//

            public const string MEMBER_PFX_CERT_PATH = "\\CertificatesUAT\\junooncapital.com.p12";
            public const string MEMBER_PFX_PASSWORD = "finb2025";
            public const string PUBLIC_CERT_PATH = "\\CertificatesUAT\\apiuat.cibilhawk.com.cer";
            public const string PRIVATE_KEY_PATH = "\\CertificatesUAT\\junooncapital.com.key";
            public const string TLS_CACERT = "\\CertificatesUAT\\L7UATChainCertificate.crt";
           // public const string PRIVATE_KEY_PASSPHRASE = "WO0000008508585";


        }

        private static class ProdVariables
        {
            public const string SITE_NAME = "JunoonCapita";
            public const string ACCOUNT_NAME = "GCVD_JunoonCapita";
            public const string ACCOUNT_CODE = "SnVub29uQDI0MTAyMDI0";

            public const string PRODUCT_CONFIG_ID = "JUNOON01";
            public const string MEMBER_REF_ID = "NB3135";
            public const string HYBRID_BASE_URL = "https://api.transunioncibil.com/consumer/dtc/v1/hybrid";
            public const string HYBRID_TOKEN_URL = "https://api.transunioncibil.com/auth/1.0/token";
            public const string WEBTOKEN_BASE_URL = "https://myscore.cibil.com/CreditView/webtokenasset.page";
            public const string API_KEY = "l785cc627e380947c3b4a235fcbe89826b";
            public const string CLIENT_SECRET = "05fe38643c204bbba9c7ae3c9b901f4b";

            //--------------Certificate Paths and Passwords----------------//

            public const string MEMBER_PFX_CERT_PATH = "\\CertificatesLive\\junoonFinB.p12";
            public const string MEMBER_PFX_PASSWORD = "Junoon@2024";
            public const string PUBLIC_CERT_PATH = "\\CertificatesLive\\api.transunioncibil.com_RSA.cer";
            public const string PRIVATE_KEY_PATH = "\\CertificatesLive\\junoonFinB.key";
            public const string TLS_CACERT = "\\CertificatesLive\\L7-ProductionChain.crt";
            //public const string PRIVATE_KEY_PASSPHRASE = "WO0000008578695";

        }



    }
}
