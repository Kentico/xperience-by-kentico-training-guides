using System.Security.Cryptography;

namespace TrainingGuides.Web.Features.DataProtection.Services;

public class AesEncryptionService : IStringEncryptionService
{
    private readonly string key;
    private readonly string iv;

    public AesEncryptionService(IConfiguration configuration)
    {
        key = configuration["AesEncryptionKey"] ?? string.Empty;
        iv = configuration["AesEncryptionIv"] ?? string.Empty;
    }

    /// <summary>
    /// Encrypts the provided string using Aes
    /// </summary>
    /// <param name="plainText">The string to be encrypted</param>
    /// <returns>An encrypted string</returns>
    /// <remarks>Relies on AesEncryptionKey and AesEncryptionIV in appsettings.json</remarks>
    /// <exception cref="ArgumentException">Thrown when AesEncryptionKey or AesEncryptionIV are not set in appsettings.json</exception>
    public string EncryptString(string plainText)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(iv))
            throw new ArgumentException("AesEncryptionKey and AesEncryptionIV must be set in appsettings.json");

        if (string.IsNullOrEmpty(plainText))
            return plainText;

        byte[] encrypted;

        // Create an Aes object
        // with the specified key and IV.
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Convert.FromBase64String(key);
            aesAlg.IV = Convert.FromBase64String(iv);

            // Create an encryptor to perform the stream transform.
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                //Write all data to the stream.
                swEncrypt.Write(plainText);
            }
            encrypted = msEncrypt.ToArray();
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
            return cipherText;

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = string.Empty;

        // Create an Aes object
        // with the specified key and IV.
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Convert.FromBase64String(key);
            aesAlg.IV = Convert.FromBase64String(iv);

            // Create a decryptor to perform the stream transform.
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            // Read the decrypted bytes from the decrypting stream
            // and place them in a string.
            plaintext = srDecrypt.ReadToEnd();
        }

        return plaintext;
    }
}
