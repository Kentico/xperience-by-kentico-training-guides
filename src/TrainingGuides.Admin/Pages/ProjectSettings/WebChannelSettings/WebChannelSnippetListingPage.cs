using CMS.Core;
using CMS.DataEngine;
using CMS.Membership;
using Kentico.Xperience.Admin.Base;
using TrainingGuides.Admin.ProjectSettings.WebChannelSettings;
using TrainingGuides.ProjectSettings;

[assembly: UIPage(
    parentType: typeof(WebChannelSettingsEditSection),
    slug: "snippets",
    uiPageType: typeof(WebChannelSnippetListingPage),
    name: "Channel snippets",
    templateName: TemplateNames.LISTING,
    order: 10)]

namespace TrainingGuides.Admin.ProjectSettings.WebChannelSettings;

public class WebChannelSnippetListingPage : ListingPage
{
    private readonly ILocalizationService localizationService;
    protected override string ObjectType => WebChannelSnippetInfo.OBJECT_TYPE;

    [PageParameter(typeof(IntPageModelBinder))]
    public int WebChannelSettingsId { get; set; }

    public WebChannelSnippetListingPage(ILocalizationService localizationService) : base()
    {
        this.localizationService = localizationService;
    }

    public override Task ConfigurePage()
    {
        PageConfiguration.ColumnConfigurations
            .AddColumn(nameof(WebChannelSnippetInfo.WebChannelSnippetDisplayName), localizationService.GetString("TrainingGuides.Page.WebChannelSnippetListing.Snippet"))
            .AddColumn(nameof(WebChannelSnippetInfo.WebChannelSnippetType), localizationService.GetString("TrainingGuides.Page.WebChannelSnippetListing.Type"));

        PageConfiguration.HeaderActions.AddLink<WebChannelSnippetCreatePage>(
            localizationService.GetString("TrainingGuides.Page.WebChannelSnippetListing.NewSnippet"),
            parameters: new PageParameterValues
                {
                    { typeof(WebChannelSettingsEditSection), WebChannelSettingsId }
                });

        PageConfiguration.AddEditRowAction<WebChannelSnippetEditSection>(
            parameters: new PageParameterValues
                {
                    { typeof(WebChannelSettingsEditSection), WebChannelSettingsId }
                });

        PageConfiguration.TableActions
            .AddDeleteAction(nameof(Delete));

        PageConfiguration.QueryModifiers
            .AddModifier((query, _) =>
            {
                return query.Where(new WhereCondition().WhereEquals(nameof(WebChannelSnippetInfo.WebChannelSnippetWebChannelSettingsID), WebChannelSettingsId));
            });

        return base.ConfigurePage();
    }

    [PageCommand(Permission = SystemPermissions.DELETE)]
    public override Task<ICommandResponse<RowActionResult>> Delete(int id) => base.Delete(id);
}