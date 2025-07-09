using Kentico.EmailBuilder.Web.Mvc;
using TrainingGuides;
using TrainingGuides.Web.Features.Shared.EmailBuilder.Templates;

[assembly: RegisterEmailTemplate(
    identifier: GeneralEmailTemplate.IDENTIFIER,
    name: "General email template",
    componentType: typeof(GeneralEmailTemplate),
    PropertiesType = typeof(GeneralEmailTemplateProperties),
    IconClass = "xp-l-cols-3",
    ContentTypeNames = [BasicEmail.CONTENT_TYPE_NAME])]