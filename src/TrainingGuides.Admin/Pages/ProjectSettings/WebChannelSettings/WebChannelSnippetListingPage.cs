using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;
using CMS.DataEngine;
using CMS.Membership;
using Microsoft.Extensions.Localization;

[assembly: UIPage(
    parentType: typeof(WebChannelSettingsEditSection),
    slug: "snippets",
    uiPageType: typeof(WebChannelSnippetListingPage),
    name: "Channel snippets",
    templateName: TemplateNames.LISTING,
    order: 10)]

namespace TrainingGuides.Admin.ProjectSettings;
public class WebChannelSnippetListingPage : ListingPage
{
    private readonly IStringLocalizer<SharedResources> localizer;
    protected override string ObjectType => WebChannelSnippetInfo.OBJECT_TYPE;

    [PageParameter(typeof(IntPageModelBinder))]
    public int WebChannelSettingsId { get; set; }

    public WebChannelSnippetListingPage(IStringLocalizer<SharedResources> localizer) : base()
    {
        this.localizer = localizer;
    }

    public override Task ConfigurePage()
    {
        PageConfiguration.ColumnConfigurations
                    .AddColumn(nameof(WebChannelSnippetInfo.WebChannelSnippetDisplayName), localizer["Snippet"])
                    .AddColumn(nameof(WebChannelSnippetInfo.WebChannelSnippetType), localizer["Type"]);

        PageConfiguration.HeaderActions.AddLink<WebChannelSnippetCreatePage>(localizer["New snippet"], parameters: WebChannelSettingsId.ToString());

        PageConfiguration.AddEditRowAction<WebChannelSnippetEditSection>(parameters: WebChannelSettingsId.ToString());

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