namespace TrainingGuides.Web.Features.DataProtection.Services;

public interface IStringEncryptionService
{
    string EncryptString(string plainText);

    string DecryptString(string cipherText);
}