using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TrainingGuides;
using TrainingGuides.Web.Features.Downloads;

[assembly: RegisterPageTemplate(
    identifier: DownloadsPagePageTemplate.IDENTIFIER,
    name: "Downloads page content type template",
    customViewName: "~/Features/Downloads/DownloadsPagePageTemplate.cshtml",
    ContentTypeNames = [DownloadsPage.CONTENT_TYPE_NAME],
    IconClass = "xp-arrow-down-line")]

namespace TrainingGuides.Web.Features.Downloads;
public static class DownloadsPagePageTemplate
{
    public const string IDENTIFIER = "TrainingGuides.DownloadsPagePageTemplate";
}