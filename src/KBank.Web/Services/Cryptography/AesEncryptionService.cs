using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;

namespace KBank.Web.Services.Cryptography
{
    public class AesEncryptionService : IStringEncryptionService
    {
        private readonly IConfiguration _configuration;
        private readonly string _key;
        private readonly string _iv;

        public AesEncryptionService(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = _configuration["AesEncryptionKey"];
            _iv = _configuration["AesEncryptionIv"];
        }


        /// <summary>
        /// Encryptes the provided string using Aes
        /// </summary>
        /// <param name="plainText">The string to be encrypted</param>
        /// <returns>An encrypted string</returns>
        /// <remarks>Relies on AesEncryptionKey and AesEncryptionIV in appsettings.json</remarks>
        public string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(_key);
                aesAlg.IV = Convert.FromBase64String(_iv);

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
        }


        /// <summary>
        /// Decrypts the provided string using Aes
        /// </summary>
        /// <param name="cipherText">The string to be decrypted</param>
        /// <returns>A decrypted string</returns>
        /// <remarks>Relies on AesEncryptionKey and AesEncryptionIV in appsettings.json</remarks>
        public string DecryptString(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return cipherText;
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(_key);
                aesAlg.IV = Convert.FromBase64String(_iv);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
