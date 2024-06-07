using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;

[assembly: UIPage(
    parentType: typeof(GlobalSettingsList),
    slug: PageParameterConstants.PARAMETERIZED_SLUG,
    uiPageType: typeof(GlobalSettingsEditSection),
    name: "Edit",
    templateName: TemplateNames.SECTION_LAYOUT,
    order: 10)]

namespace TrainingGuides.Admin.ProjectSettings;

public class GlobalSettingsEditSection : EditSectionPage<GlobalSettingsKeyInfo>
{
}