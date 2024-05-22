using CMS.DataEngine;

namespace TrainingGuides.ProjectSettings
{
    /// <summary>
    /// Class providing <see cref="WebChannelSnippetInfo"/> management.
    /// </summary>
    [ProviderInterface(typeof(IWebChannelSnippetInfoProvider))]
    public partial class WebChannelSnippetInfoProvider : AbstractInfoProvider<WebChannelSnippetInfo, WebChannelSnippetInfoProvider>, IWebChannelSnippetInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebChannelSnippetInfoProvider"/> class.
        /// </summary>
        public WebChannelSnippetInfoProvider()
            : base(WebChannelSnippetInfo.TYPEINFO)
        {
        }
    }
}