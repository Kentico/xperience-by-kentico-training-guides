using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TrainingGuides.Web.Features.Shared;

[assembly: RegisterPageTemplate(
    identifier: GeneralPageTemplate.IDENTIFIER,
    name: "General page template",
    propertiesType: typeof(GeneralPageTemplateProperties),
    customViewName: "~/Features/Shared/Templates/GeneralPageTemplate.cshtml",
    IconClass = "icon-square")]

namespace TrainingGuides.Web.Features.Shared;
public static class GeneralPageTemplate
{
    public const string IDENTIFIER = "TrainingGuides.GeneralPageTemplate";
}
