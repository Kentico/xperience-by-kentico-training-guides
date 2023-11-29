using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TrainingGuides;
using TrainingGuides.Web.Features.LandingPages;

[assembly: RegisterPageTemplate(
    identifier: LandingPagePageTemplate.IDENTIFIER,
    name: "Landing page content type template",
    propertiesType: typeof(LandingPageTemplateProperties),
    customViewName: "~/Features/LandingPages/LandingPagePageTemplate.cshtml",
    ContentTypeNames = [LandingPage.CONTENT_TYPE_NAME],
    IconClass = "xp-market")]

namespace TrainingGuides.Web.Features.LandingPages;
public static class LandingPagePageTemplate
{
    public const string IDENTIFIER = "TrainingGuides.LandingPagePageTemplate";
}
