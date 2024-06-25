namespace TrainingGuides.ProjectSettings;

public partial class WebChannelSnippetInfo
{
    static WebChannelSnippetInfo()
    {
        //TYPEINFO.ContinuousIntegrationSettings.Enabled = true;
        TYPEINFO.ParentObjectType = WebChannelSettingsInfo.OBJECT_TYPE;
        TYPEINFO.ParentIDColumn = nameof(WebChannelSnippetWebChannelSettingsID);
    }
}