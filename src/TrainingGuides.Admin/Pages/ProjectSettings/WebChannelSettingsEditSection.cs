using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;

[assembly: UIPage(
    parentType: typeof(WebChannelSettingsList),
    slug: PageParameterConstants.PARAMETERIZED_SLUG,
    uiPageType: typeof(WebChannelSettingsEditSection),
    name: "Edit",
    templateName: TemplateNames.SECTION_LAYOUT,
    order: 0)]

namespace TrainingGuides.Admin.ProjectSettings;

public class WebChannelSettingsEditSection : EditSectionPage<WebChannelSettingsInfo>
{
}