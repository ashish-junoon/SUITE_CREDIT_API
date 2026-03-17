using JC.TransUnion.Cibil.Crypto;
using JC.TransUnion.Cibil.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JC.TransUnion.Cibil.Services
{
    public class CibilHybridService_1: ICibilService
    {
        private readonly IConfiguration _config;
        private static bool CIBIL_SERVICES_PROD = false;
        private static string BASE_URL;
        private static string ServerPubCertPath;
        private static string PrivateKeyPath;
        public CibilHybridService_1(IConfiguration config)
        {
            _config = config;
            CIBIL_SERVICES_PROD = Convert.ToBoolean(_config["CIC_SERVICES:TRANSUNION_CIBIL_SERVICES_PROD"]);
            BASE_URL = CIBIL_SERVICES_PROD ? CibilConfig.ProdVariables.HYBRID_BASE_URL : CibilConfig.UATVariables.HYBRID_BASE_URL;
            ServerPubCertPath = CIBIL_SERVICES_PROD ? CibilConfig.ProdVariables.PUBLIC_CERT_PATH : CibilConfig.UATVariables.PUBLIC_CERT_PATH;
            PrivateKeyPath = CIBIL_SERVICES_PROD ? CibilConfig.ProdVariables.PRIVATE_KEY_PATH : CibilConfig.UATVariables.PRIVATE_KEY_PATH;
        }
        public async Task<(int, object)> CallAsync(string path, object payload, bool IsProduction)
        {
            var encrypted = HybridEncryptor.Encrypt(payload, ServerPubCertPath);

            var upstream = await CibilHttpClient.PostAsync(BASE_URL + path,
                new { encrypted.EncryptedKey, encrypted.EncryptedData }, IsProduction);

            if (upstream.Data?.EncryptedKey == null)
                return upstream;

            var decrypted = HybridDecryptor.Decrypt<object>(
                upstream.Data.EncryptedKey.ToString(),
                upstream.Data.EncryptedData.ToString(),
                PrivateKeyPath);

            return (upstream.Status, decrypted);
        }
    }
}
