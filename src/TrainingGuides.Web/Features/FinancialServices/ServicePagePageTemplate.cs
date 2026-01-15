using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TrainingGuides;
using TrainingGuides.Web.Features.FinancialServices;

[assembly: RegisterPageTemplate(
    identifier: ServicePagePageTemplate.IDENTIFIER,
    name: "Service page template",
    propertiesType: typeof(ServicePagePageTemplateProperties),
    customViewName: "~/Features/FinancialServices/ServicePagePageTemplate.cshtml",
    ContentTypeNames = [ServicePage.CONTENT_TYPE_NAME],
    IconClass = "xp-box")]

namespace TrainingGuides.Web.Features.FinancialServices;

public static class ServicePagePageTemplate
{
    public const string IDENTIFIER = "TrainingGuides.ServicePageTemplate";
}
