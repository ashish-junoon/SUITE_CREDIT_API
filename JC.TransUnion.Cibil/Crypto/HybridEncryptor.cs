using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace JC.TransUnion.Cibil.Crypto
{
    public static class HybridEncryptor
    {
        public static object Encrypt_V1(object payload, string serverPublicCertPath)
        {
            string json = JsonSerializer.Serialize(payload);

            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] encryptedData;

            using (var encryptor = aes.CreateEncryptor())
            {
                var plainBytes = Encoding.UTF8.GetBytes(json);
                encryptedData = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }

            byte[] ivPlusCipher = aes.IV.Concat(encryptedData).ToArray();

            var cert = new X509Certificate2(serverPublicCertPath);

            using var rsa = cert.GetRSAPublicKey();

            var encryptedKey = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);

            return new
            {
                EncryptedKey = Convert.ToBase64String(encryptedKey),
                EncryptedData = Convert.ToBase64String(ivPlusCipher)
            };
        }
    }

}

