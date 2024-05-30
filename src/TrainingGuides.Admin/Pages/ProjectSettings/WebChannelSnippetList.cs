using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;
using CMS.DataEngine;
using CMS.Membership;

[assembly: UIPage(
    parentType: typeof(WebChannelSettingsEditSection),
    slug: "snippets",
    uiPageType: typeof(WebChannelSnippetList),
    name: "Channel snippets",
    templateName: TemplateNames.LISTING,
    order: 10)]

namespace TrainingGuides.Admin.ProjectSettings;
public class WebChannelSnippetList : ListingPage
{
    protected override string ObjectType => WebChannelSnippetInfo.OBJECT_TYPE;

    [PageParameter(typeof(IntPageModelBinder))]
    public int WebChannelSettingsId { get; set; }

    public WebChannelSnippetList() : base()
    {
    }

    public override Task ConfigurePage()
    {
        PageConfiguration.ColumnConfigurations
                    .AddColumn(nameof(WebChannelSnippetInfo.WebChannelSnippetDisplayName), "Snippet")
                    .AddColumn(nameof(WebChannelSnippetInfo.WebChannelSnippetType), "Type");

        PageConfiguration.HeaderActions.AddLink<WebChannelSnippetCreate>(LocalizationService.GetString("New snippet"), parameters: WebChannelSettingsId.ToString());

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