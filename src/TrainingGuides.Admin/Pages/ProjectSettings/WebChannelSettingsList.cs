using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;
using CMS.DataEngine;
using CMS.ContentEngine;
using System.Threading.Channels;


[assembly: UIPage(
    parentType: typeof(PojectSettingsApplication),
    slug: "channel-settings",
    uiPageType: typeof(WebChannelSettingsList),
    name: "Channel settings",
    templateName: TemplateNames.LISTING,
    order: 0)]

namespace TrainingGuides.Admin.ProjectSettings;
public class WebChannelSettingsList : ListingPage
{
    private readonly IInfoProvider<ChannelInfo> channelInfoProvider;
    private readonly IInfoProvider<WebChannelSettingsInfo> webChannelSettingsInfoProvider;
    private readonly IInfoProvider<SeoSettingsInfo> seoSettingsInfoProvider;

    protected override string ObjectType => WebChannelSettingsInfo.OBJECT_TYPE;

    public WebChannelSettingsList(
        IInfoProvider<ChannelInfo> channelInfoProvider,
        IInfoProvider<WebChannelSettingsInfo> webChannelSettingsInfoProvider,
        IInfoProvider<SeoSettingsInfo> seoSettingsInfoProvider) : base()
    {
        this.channelInfoProvider = channelInfoProvider;
        this.webChannelSettingsInfoProvider = webChannelSettingsInfoProvider;
        this.seoSettingsInfoProvider = seoSettingsInfoProvider;

        GetOrCreateSettingsList();

    }

    public override async Task ConfigurePage()
    {
        PageConfiguration.ColumnConfigurations
                     .AddColumn(nameof(
                        WebChannelSettingsInfo.WebChannelSettingsChannelDisplayName), "Channel");

        PageConfiguration.AddEditRowAction<WebChannelSettingsEditSection>();

        await base.ConfigurePage();

    }

    private void GetOrCreateSettingsList()
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
        if (!channel.ChannelDisplayName.Equals(currentChannelSetting?.WebChannelSettingsChannelDisplayName) && currentChannelSetting != null)
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