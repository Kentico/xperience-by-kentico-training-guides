namespace TrainingGuides.ProjectSettings;

public partial class SeoSettingsInfo
{
    static SeoSettingsInfo()
    {
        TYPEINFO.ParentObjectType = WebChannelSettingsInfo.OBJECT_TYPE;
        TYPEINFO.ParentIDColumn = nameof(SeoSettingsWebChannelSettingID);
    }
}