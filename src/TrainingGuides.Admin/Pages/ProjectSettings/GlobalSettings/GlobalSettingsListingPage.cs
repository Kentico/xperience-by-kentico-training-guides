using CMS.Core;
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
    private readonly ILocalizationService localizationService;
    protected override string ObjectType => GlobalSettingsKeyInfo.OBJECT_TYPE;

    public GlobalSettingsListingPage(ILocalizationService localizationService) : base()
    {
        this.localizationService = localizationService;
    }

    public override async Task ConfigurePage()
    {
        PageConfiguration.ColumnConfigurations
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyDisplayName), localizationService.GetString("TrainingGuides.Page.GlobalSettingsListing.Name"))
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyValue), localizationService.GetString("TrainingGuides.Page.GlobalSettingsListing.Value"))
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyNote), localizationService.GetString("TrainingGuides.Page.GlobalSettingsListing.Note"))
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyName), localizationService.GetString("TrainingGuides.Page.GlobalSettingsListing.Codename"));

        PageConfiguration.HeaderActions.AddLink<GlobalSettingsCreatePage>(localizationService.GetString("TrainingGuides.Page.GlobalSettingsListing.NewSetting"));

        PageConfiguration.AddEditRowAction<GlobalSettingsEditSection>();

        PageConfiguration.TableActions
                .AddDeleteAction(nameof(Delete));

        await base.ConfigurePage();

    }

    [PageCommand(Permission = SystemPermissions.DELETE)]
    public override Task<ICommandResponse<RowActionResult>> Delete(int id) => base.Delete(id);
}