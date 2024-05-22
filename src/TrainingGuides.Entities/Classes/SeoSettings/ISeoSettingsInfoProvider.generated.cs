using CMS.DataEngine;

namespace TrainingGuides.ProjectSettings
{
    /// <summary>
    /// Declares members for <see cref="SeoSettingsInfo"/> management.
    /// </summary>
    public partial interface ISeoSettingsInfoProvider : IInfoProvider<SeoSettingsInfo>, IInfoByIdProvider<SeoSettingsInfo>
    {
    }
}