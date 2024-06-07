using CMS.Membership;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;
using TrainingGuides.Admin.ProjectSettings;

[assembly: UIApplication(
    identifier: ProjectSettingsApplication.IDENTIFIER,
    type: typeof(ProjectSettingsApplication),
    slug: "project-settings",
    name: "Project settings",
    category: BaseApplicationCategories.CONFIGURATION,
    icon: Icons.BoxCogwheel,
    templateName: TemplateNames.SECTION_LAYOUT)]

namespace TrainingGuides.Admin.ProjectSettings;

[UIPermission(SystemPermissions.VIEW)]
[UIPermission(SystemPermissions.CREATE)]
[UIPermission(SystemPermissions.DELETE)]
[UIPermission(SystemPermissions.UPDATE)]
public class ProjectSettingsApplication : ApplicationPage
{
    public const string IDENTIFIER = "TrainingGuides.ProjectSettingsApplication";
}