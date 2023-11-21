namespace TrainingGuides.Web.Services.Cryptography;

public interface IStringEncryptionService
{
    string EncryptString(string plainText);

    string DecryptString(string cipherText);
}