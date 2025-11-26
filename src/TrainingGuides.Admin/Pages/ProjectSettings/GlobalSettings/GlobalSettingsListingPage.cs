using CMS.Membership;
using Kentico.Xperience.Admin.Base;
using TrainingGuides.Admin.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings.GlobalSettings;
using TrainingGuides.ProjectSettings;


[assembly: UIPage(
    parentType: typeof(ProjectSettingsApplication),
    slug: "global-settings",
    uiPageType: typeof(GlobalSettingsListingPage),
    name: "Global settings",
    templateName: TemplateNames.LISTING,
    order: 0)]

namespace TrainingGuides.Admin.ProjectSettings.GlobalSettings;

public class GlobalSettingsListingPage : ListingPage
{
    protected override string ObjectType => GlobalSettingsKeyInfo.OBJECT_TYPE;

    public override async Task ConfigurePage()
    {
        PageConfiguration.ColumnConfigurations
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyDisplayName), "Name")
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyValue), "Value")
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyNote), "Note")
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyName), "Codename");

        PageConfiguration.HeaderActions.AddLink<GlobalSettingsCreatePage>("New setting");

        PageConfiguration.AddEditRowAction<GlobalSettingsEditSection>();

        PageConfiguration.TableActions
                .AddDeleteAction(nameof(Delete));

        await base.ConfigurePage();

    }

    [PageCommand(Permission = SystemPermissions.DELETE)]
    public override Task<ICommandResponse<RowActionResult>> Delete(int id) => base.Delete(id);
}