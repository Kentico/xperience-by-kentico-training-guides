using Kentico.EmailBuilder.Web.Mvc;
using TrainingGuides.Web.Features.Shared.EmailBuilder.Sections;

using Microsoft.AspNetCore.Components;

[assembly: RegisterEmailSection(
    identifier: GeneralEmailSection.IDENTIFIER,
    name: "General email template section",
    componentType: typeof(GeneralEmailSection),
    Description = "Plain section for the General email template.",
    IconClass = "icon-collapse-scheme")]

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.Sections;

public partial class GeneralEmailSection : ComponentBase
{
    public const string IDENTIFIER = $"TrainingGuides.GeneralEmailSection";
}
