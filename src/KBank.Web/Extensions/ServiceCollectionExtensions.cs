using KBank.Web.Services.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace KBank.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddKBankServices(this IServiceCollection services)
    {
        services.AddSingleton<IStringEncryptionService,AesEncryptionService>();
    }
}