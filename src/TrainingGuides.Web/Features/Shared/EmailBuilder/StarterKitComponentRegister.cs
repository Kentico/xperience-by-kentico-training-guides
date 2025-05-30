using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Templates;

[assembly: RegisterEmailTemplate(
    identifier: EmailBuilderStarterKitTemplate.IDENTIFIER,
    name: "MJML Starter Kit template",
    componentType: typeof(EmailBuilderStarterKitTemplate),
    // Enter the code names of all email content types where you wish to use Email Builder
    ContentTypeNames = ["TrainingGuides.BasicEmail"])]