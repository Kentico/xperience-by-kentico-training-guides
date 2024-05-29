using CMS.DataEngine;
using CMS.Websites.Routing;
using Microsoft.Extensions.Options;
using TrainingGuides.ProjectSettings;

namespace TrainingGuides.Web.Features.SEO;
public class RobotsOptionsSetup : IConfigureOptions<RobotsOptions>
{
    private readonly IInfoProvider<SeoSettingsInfo> seoSettingsInfoProvider;
    private readonly IWebsiteChannelContext websiteChannelContext;
    private readonly IInfoProvider<WebChannelSettingsInfo> webChannelSettingsInfoProvider;

    public RobotsOptionsSetup(IInfoProvider<SeoSettingsInfo> seoSettingsInfoProvider, IWebsiteChannelContext websiteChannelContext, IInfoProvider<WebChannelSettingsInfo> webChannelSettingsInfoProvider)
    {
        this.seoSettingsInfoProvider = seoSettingsInfoProvider;
        this.websiteChannelContext = websiteChannelContext;
        this.webChannelSettingsInfoProvider = webChannelSettingsInfoProvider;
    }

    public void Configure(RobotsOptions options)
    {
        string context = websiteChannelContext.WebsiteChannelName;

        var channelSettings = webChannelSettingsInfoProvider.Get()
            .WhereContains(nameof(WebChannelSettingsInfo.WebChannelSettingsChannel), context)
            .FirstOrDefault();

        var seoSettings = seoSettingsInfoProvider.Get()
            .WhereEquals(nameof(SeoSettingsInfo.SeoSettingsWebChannelSettingId), channelSettings?.WebChannelSettingsID ?? 0)
            .FirstOrDefault();

        options.RobotsText = seoSettings?.SeoSettingsRobots ?? string.Empty;
    }
}