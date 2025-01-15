using CMS.Membership;
using Kentico.Xperience.Admin.Base;
using Microsoft.Extensions.Localization;
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
    private readonly IStringLocalizer<SharedResources> stringLocalizer;
    protected override string ObjectType => GlobalSettingsKeyInfo.OBJECT_TYPE;

    public GlobalSettingsListingPage(IStringLocalizer<SharedResources> stringLocalizer) : base()
    {
        this.stringLocalizer = stringLocalizer;
    }

    public override async Task ConfigurePage()
    {
        PageConfiguration.ColumnConfigurations
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyDisplayName), stringLocalizer["Name"])
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyValue), stringLocalizer["Value"])
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyNote), stringLocalizer["Note"])
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyName), stringLocalizer["Codename"]);

        PageConfiguration.HeaderActions.AddLink<GlobalSettingsCreatePage>(stringLocalizer["New setting"]);

        PageConfiguration.AddEditRowAction<GlobalSettingsEditSection>();

        PageConfiguration.TableActions
                .AddDeleteAction(nameof(Delete));

        await base.ConfigurePage();

    }

    [PageCommand(Permission = SystemPermissions.DELETE)]
    public override Task<ICommandResponse<RowActionResult>> Delete(int id) => base.Delete(id);
}