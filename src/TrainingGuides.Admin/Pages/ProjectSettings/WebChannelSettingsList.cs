using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;
using CMS.DataEngine;
using CMS.ContentEngine;


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
            var currentChannelSettings = webChannelSettings.Where(setting => setting.WebChannelSettingsChannel.Contains(channel.ChannelName)).ToList();

            if (currentChannelSettings.Count() == 0)
            {
                var newSetting = new WebChannelSettingsInfo
                {
                    WebChannelSettingsChannel = [channel.ChannelName],
                    WebChannelSettingsChannelDisplayName = channel.ChannelDisplayName,
                };
                webChannelSettingsInfoProvider.Set(newSetting);
                currentChannelSettings.Add(newSetting);
            }
            int? webChannelSettingsId = currentChannelSettings.FirstOrDefault()?.WebChannelSettingsID;

            var currentSeoSettings = seoSettings
                .Where(setting => setting.SeoSettingsWebChannelSettingId == webChannelSettingsId).ToList();

            if (currentSeoSettings.Count() == 0 && webChannelSettingsId != null)
            {
                var newSeoSetting = new SeoSettingsInfo
                {
                    SeoSettingsWebChannelSettingId = (int)webChannelSettingsId,
                    SeoSettingsRobots = string.Empty,
                };

                seoSettingsInfoProvider.Set(newSeoSetting);
            }
        }
    }
}