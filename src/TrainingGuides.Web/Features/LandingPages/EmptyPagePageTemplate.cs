using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TrainingGuides.Web.Features.LandingPages;

[assembly: RegisterPageTemplate(
    identifier: EmptyPagePageTemplate.IDENTIFIER,
    name: "Empty page content type template",
    customViewName: "~/Features/LandingPages/EmptyPagePageTemplate.cshtml",
    IconClass = "xp-binder")]

namespace TrainingGuides.Web.Features.LandingPages;
public static class EmptyPagePageTemplate
{
    public const string IDENTIFIER = "TrainingGuides.EmptyPage";
}