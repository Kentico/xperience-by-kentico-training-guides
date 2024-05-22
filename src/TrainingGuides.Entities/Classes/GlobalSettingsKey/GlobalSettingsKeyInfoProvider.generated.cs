using CMS.DataEngine;

namespace TrainingGuides.ProjectSettings
{
    /// <summary>
    /// Class providing <see cref="GlobalSettingsKeyInfo"/> management.
    /// </summary>
    [ProviderInterface(typeof(IGlobalSettingsKeyInfoProvider))]
    public partial class GlobalSettingsKeyInfoProvider : AbstractInfoProvider<GlobalSettingsKeyInfo, GlobalSettingsKeyInfoProvider>, IGlobalSettingsKeyInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalSettingsKeyInfoProvider"/> class.
        /// </summary>
        public GlobalSettingsKeyInfoProvider()
            : base(GlobalSettingsKeyInfo.TYPEINFO)
        {
        }
    }
}