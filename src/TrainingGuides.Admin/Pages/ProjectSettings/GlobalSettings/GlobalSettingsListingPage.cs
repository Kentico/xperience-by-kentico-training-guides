using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;
using CMS.Membership;
using Microsoft.Extensions.Localization;


[assembly: UIPage(
    parentType: typeof(ProjectSettingsApplication),
    slug: "global-settings",
    uiPageType: typeof(GlobalSettingsListingPage),
    name: "Global settings",
    templateName: TemplateNames.LISTING,
    order: 0)]

namespace TrainingGuides.Admin.ProjectSettings;
public class GlobalSettingsListingPage : ListingPage
{
    private readonly IStringLocalizer<SharedResources> localizer;
    protected override string ObjectType => GlobalSettingsKeyInfo.OBJECT_TYPE;

    public GlobalSettingsListingPage(IStringLocalizer<SharedResources> localizer) : base()
    {
        this.localizer = localizer;
    }

    public override async Task ConfigurePage()
    {
        PageConfiguration.ColumnConfigurations
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyDisplayName), localizer["Name"])
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyValue), localizer["Value"])
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyNote), localizer["Note"])
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyName), localizer["Codename"]);

        PageConfiguration.HeaderActions.AddLink<GlobalSettingsCreatePage>(localizer["New setting"]);

        PageConfiguration.AddEditRowAction<GlobalSettingsEditSection>();

        PageConfiguration.TableActions
                .AddDeleteAction(nameof(Delete));

        await base.ConfigurePage();

    }

    [PageCommand(Permission = SystemPermissions.DELETE)]
    public override Task<ICommandResponse<RowActionResult>> Delete(int id) => base.Delete(id);
}