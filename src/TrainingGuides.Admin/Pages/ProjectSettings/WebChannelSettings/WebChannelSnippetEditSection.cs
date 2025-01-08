using Kentico.Xperience.Admin.Base;
using TrainingGuides.Admin.ProjectSettings.WebChannelSettings;
using TrainingGuides.ProjectSettings;

[assembly: UIPage(
    parentType: typeof(WebChannelSnippetListingPage),
    slug: PageParameterConstants.PARAMETERIZED_SLUG,
    uiPageType: typeof(WebChannelSnippetEditSection),
    name: "Edit snippets",
    templateName: TemplateNames.SECTION_LAYOUT,
    order: 0)]

namespace TrainingGuides.Admin.ProjectSettings.WebChannelSettings;

public class WebChannelSnippetEditSection : EditSectionPage<WebChannelSnippetInfo>
{
}