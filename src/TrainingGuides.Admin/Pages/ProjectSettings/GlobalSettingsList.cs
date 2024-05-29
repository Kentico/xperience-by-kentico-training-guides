using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;


[assembly: UIPage(
    parentType: typeof(PojectSettingsApplication),
    slug: "global-settings",
    uiPageType: typeof(GlobalSettingsList),
    name: "Global settings",
    templateName: TemplateNames.LISTING,
    order: 0)]

namespace TrainingGuides.Admin.ProjectSettings;
public class GlobalSettingsList : ListingPage
{
    protected override string ObjectType => GlobalSettingsKeyInfo.OBJECT_TYPE;

    public override async Task ConfigurePage()
    {
        // Adds the specified columns to the grid and sets their caption
        PageConfiguration.ColumnConfigurations
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyDisplayName), "Name")
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyValue), "Value")
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyNote), "Note")
                    .AddColumn(nameof(GlobalSettingsKeyInfo.GlobalSettingsKeyName), "Codename");

        PageConfiguration.HeaderActions.AddLink<GlobalSettingsCreate>(LocalizationService.GetString("New Setting"));

        PageConfiguration.AddEditRowAction<GlobalSettingsEditSection>();

        PageConfiguration.TableActions
                .AddDeleteAction(nameof(Delete));

        await base.ConfigurePage();

    }

    [PageCommand]
    public override Task<ICommandResponse<RowActionResult>> Delete(int id) => base.Delete(id);
}