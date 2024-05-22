using CMS.DataEngine;

namespace TrainingGuides.ProjectSettings
{
    /// <summary>
    /// Class providing <see cref="SeoSettingsInfo"/> management.
    /// </summary>
    [ProviderInterface(typeof(ISeoSettingsInfoProvider))]
    public partial class SeoSettingsInfoProvider : AbstractInfoProvider<SeoSettingsInfo, SeoSettingsInfoProvider>, ISeoSettingsInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeoSettingsInfoProvider"/> class.
        /// </summary>
        public SeoSettingsInfoProvider()
            : base(SeoSettingsInfo.TYPEINFO)
        {
        }
    }
}