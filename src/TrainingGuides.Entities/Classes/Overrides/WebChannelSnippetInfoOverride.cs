namespace TrainingGuides.ProjectSettings;

public partial class WebChannelSnippetInfo
{
    static WebChannelSnippetInfo()
    {
        TYPEINFO.ParentObjectType = WebChannelSettingsInfo.OBJECT_TYPE;
        TYPEINFO.ParentIDColumn = nameof(WebChannelSnippetWebChannelSettingsID);
    }
}