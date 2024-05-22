using CMS.DataEngine;

namespace TrainingGuides.ProjectSettings
{
    /// <summary>
    /// Class providing <see cref="WebChannelSettingsInfo"/> management.
    /// </summary>
    [ProviderInterface(typeof(IWebChannelSettingsInfoProvider))]
    public partial class WebChannelSettingsInfoProvider : AbstractInfoProvider<WebChannelSettingsInfo, WebChannelSettingsInfoProvider>, IWebChannelSettingsInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebChannelSettingsInfoProvider"/> class.
        /// </summary>
        public WebChannelSettingsInfoProvider()
            : base(WebChannelSettingsInfo.TYPEINFO)
        {
        }
    }
}