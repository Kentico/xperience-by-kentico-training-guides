using CMS.ContentEngine;

namespace TrainingGuides.ProjectSettings;

public partial class WebChannelSettingsInfo
{
    static WebChannelSettingsInfo()
    {
        TYPEINFO.ParentObjectType = ChannelInfo.OBJECT_TYPE;
        TYPEINFO.ParentIDColumn = nameof(WebChannelSettingsChannelID);
    }
}