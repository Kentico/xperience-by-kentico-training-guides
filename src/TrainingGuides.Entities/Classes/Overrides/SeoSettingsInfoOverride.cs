namespace TrainingGuides.ProjectSettings;

public partial class SeoSettingsInfo
{
    static SeoSettingsInfo()
    {
        TYPEINFO.ContinuousIntegrationSettings.Enabled = true;
        TYPEINFO.ParentObjectType = WebChannelSettingsInfo.OBJECT_TYPE;
        TYPEINFO.ParentIDColumn = nameof(SeoSettingsWebChannelSettingID);
    }
}