using CMS.DataEngine;
using Microsoft.Extensions.Options;
using TrainingGuides.ProjectSettings;

namespace TrainingGuides.Web.Features.EmailNotifications;

public class EmailNotificationOptionsSetup : IConfigureOptions<EmailNotificationOptions>
{
    private const string FROM_ADDRESS_SETTINGS_KEY = "EmailNotificationFromAddress";
    private const string TO_ADDRESSES_SETTINGS_KEY = "EmailNotificationToAddresses";
    private readonly IInfoProvider<GlobalSettingsKeyInfo> globalSettingsKeyInfoProvider;

    public EmailNotificationOptionsSetup(IInfoProvider<GlobalSettingsKeyInfo> globalSettingsKeyInfoProvider)
    {
        this.globalSettingsKeyInfoProvider = globalSettingsKeyInfoProvider;
    }

    public void Configure(EmailNotificationOptions options)
    {
        var emailSettings = globalSettingsKeyInfoProvider.Get()
            .WhereEquals(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyName), FROM_ADDRESS_SETTINGS_KEY)
            .Or()
            .WhereEquals(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyName), TO_ADDRESSES_SETTINGS_KEY)
            .GetEnumerableTypedResult();

        var fromAddress = emailSettings.FirstOrDefault(s => s.GlobalSettingsKeyName == FROM_ADDRESS_SETTINGS_KEY);
        var toAddresses = emailSettings.FirstOrDefault(s => s.GlobalSettingsKeyName == TO_ADDRESSES_SETTINGS_KEY);


        options.SenderAddress = fromAddress?.GlobalSettingsKeyValue ?? string.Empty;
        options.RecipientAddresses = toAddresses?.GlobalSettingsKeyValue ?? string.Empty;
    }
}