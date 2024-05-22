using CMS.DataEngine;

namespace TrainingGuides.ProjectSettings
{
    /// <summary>
    /// Declares members for <see cref="GlobalSettingsKeyInfo"/> management.
    /// </summary>
    public partial interface IGlobalSettingsKeyInfoProvider : IInfoProvider<GlobalSettingsKeyInfo>, IInfoByIdProvider<GlobalSettingsKeyInfo>, IInfoByNameProvider<GlobalSettingsKeyInfo>
    {
    }
}