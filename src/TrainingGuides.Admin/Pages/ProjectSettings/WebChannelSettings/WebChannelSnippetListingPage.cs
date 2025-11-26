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
    protected override string ObjectType => WebChannelSnippetInfo.OBJECT_TYPE;

    [PageParameter(typeof(IntPageModelBinder))]
    public int WebChannelSettingsId { get; set; }

    public override Task ConfigurePage()
    {
        PageConfiguration.ColumnConfigurations
            .AddColumn(nameof(WebChannelSnippetInfo.WebChannelSnippetDisplayName), "Snippet")
            .AddColumn(nameof(WebChannelSnippetInfo.WebChannelSnippetType), "Type");

        PageConfiguration.HeaderActions.AddLink<WebChannelSnippetCreatePage>(
            "New snippet",
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