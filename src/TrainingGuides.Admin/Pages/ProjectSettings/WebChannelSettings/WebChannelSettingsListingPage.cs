using CMS.ContentEngine;
using CMS.DataEngine;
using Kentico.Xperience.Admin.Base;
using Microsoft.Extensions.Localization;
using TrainingGuides.Admin.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings.WebChannelSettings;
using TrainingGuides.ProjectSettings;

[assembly: UIPage(
    parentType: typeof(ProjectSettingsApplication),
    slug: "channel-settings",
    uiPageType: typeof(WebChannelSettingsListingPage),
    name: "Channel settings",
    templateName: TemplateNames.LISTING,
    order: 0)]

namespace TrainingGuides.Admin.ProjectSettings.WebChannelSettings;
public class WebChannelSettingsListingPage : ListingPage
{
    private readonly IInfoProvider<ChannelInfo> channelInfoProvider;
    private readonly IInfoProvider<WebChannelSettingsInfo> webChannelSettingsInfoProvider;
    private readonly IInfoProvider<SeoSettingsInfo> seoSettingsInfoProvider;
    private readonly IStringLocalizer<SharedResources> stringLocalizer;

    protected override string ObjectType => WebChannelSettingsInfo.OBJECT_TYPE;

    public WebChannelSettingsListingPage(
        IInfoProvider<ChannelInfo> channelInfoProvider,
        IInfoProvider<WebChannelSettingsInfo> webChannelSettingsInfoProvider,
        IInfoProvider<SeoSettingsInfo> seoSettingsInfoProvider,
        IStringLocalizer<SharedResources> stringLocalizer) : base()
    {
        this.channelInfoProvider = channelInfoProvider;
        this.webChannelSettingsInfoProvider = webChannelSettingsInfoProvider;
        this.seoSettingsInfoProvider = seoSettingsInfoProvider;
        this.stringLocalizer = stringLocalizer;

        EnsureSettingsListData();
    }

    public override async Task ConfigurePage()
    {
        PageConfiguration.ColumnConfigurations
                     .AddColumn(nameof(
                        WebChannelSettingsInfo.WebChannelSettingsChannelDisplayName), stringLocalizer["Channel"]);

        PageConfiguration.AddEditRowAction<WebChannelSettingsEditSection>();

        await base.ConfigurePage();
    }

    private void EnsureSettingsListData()
    {
        var channels = channelInfoProvider.Get();
        var webChannelSettings = webChannelSettingsInfoProvider.Get().ToList();
        var seoSettings = seoSettingsInfoProvider.Get().ToList();

        foreach (var channel in channels)
        {
            var currentChannelSettings = webChannelSettings.Where(setting => setting.WebChannelSettingsChannelID.Equals(channel.ChannelID)).ToList();

            EnsureChannelSetting(currentChannelSettings, channel);

            var currentChannelSetting = currentChannelSettings.FirstOrDefault();

            EnsureChannelSettingDisplayName(channel, currentChannelSetting);

            EnsureSeoSettings(currentChannelSetting, seoSettings);
        }
    }

    private void EnsureChannelSetting(List<WebChannelSettingsInfo> currentChannelSettings, ChannelInfo channel)
    {
        if (currentChannelSettings.Count() == 0)
        {
            var newSetting = new WebChannelSettingsInfo
            {
                WebChannelSettingsChannelID = channel.ChannelID,
                WebChannelSettingsChannelDisplayName = channel.ChannelDisplayName,
            };
            webChannelSettingsInfoProvider.Set(newSetting);
            currentChannelSettings.Add(newSetting);
        }
    }

    private void EnsureChannelSettingDisplayName(ChannelInfo channel, WebChannelSettingsInfo? currentChannelSetting)
    {
        if (currentChannelSetting != null && !channel.ChannelDisplayName.Equals(currentChannelSetting.WebChannelSettingsChannelDisplayName))
        {
            currentChannelSetting.WebChannelSettingsChannelDisplayName = channel.ChannelDisplayName;
            webChannelSettingsInfoProvider.Set(currentChannelSetting);
        }
    }

    private void EnsureSeoSettings(WebChannelSettingsInfo? currentChannelSetting, IEnumerable<SeoSettingsInfo> seoSettings)
    {
        int? webChannelSettingsId = currentChannelSetting?.WebChannelSettingsID;

        var currentSeoSettings = seoSettings
            .Where(setting => setting.SeoSettingsWebChannelSettingID == webChannelSettingsId).ToList();

        if (currentSeoSettings.Count() == 0 && webChannelSettingsId != null)
        {
            var newSeoSetting = new SeoSettingsInfo
            {
                SeoSettingsWebChannelSettingID = (int)webChannelSettingsId,
                SeoSettingsRobots = string.Empty,
            };

            seoSettingsInfoProvider.Set(newSeoSetting);
        }
    }
}