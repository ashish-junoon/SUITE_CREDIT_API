using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace JC.TransUnion.Cibil.Crypto
{
    public static class HybridDecryptor
    {
        public static object Decrypt_V1(string encryptedKeyBase64, string encryptedDataBase64, string privateKeyPath, string MEMBER_PFX_PASSWORD)
        {
            byte[] encryptedKey = Convert.FromBase64String(encryptedKeyBase64);

            string privateKeyPem = System.IO.File.ReadAllText(privateKeyPath);

            using RSA rsa = RSA.Create();
            //rsa.ImportFromPem(privateKeyPem.ToCharArray());
            rsa.ImportFromEncryptedPem(privateKeyPem, MEMBER_PFX_PASSWORD);

            byte[] aesKey = rsa.Decrypt(encryptedKey, RSAEncryptionPadding.Pkcs1);

            byte[] encryptedData = Convert.FromBase64String(encryptedDataBase64);

            byte[] iv = encryptedData.Take(16).ToArray();
            byte[] cipherText = encryptedData.Skip(16).ToArray();

            using var aes = Aes.Create();
            aes.Key = aesKey;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

            return JsonSerializer.Deserialize<object>(
                Encoding.UTF8.GetString(decrypted));
        }
    }
}
