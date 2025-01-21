using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Shared.Sections.SingleColumn;

[assembly: RegisterSection(
    identifier: SingleColumnSectionViewComponent.IDENTIFIER,
    viewComponentType: typeof(SingleColumnSectionViewComponent),
    name: "1 column",
    propertiesType: typeof(SingleColumnSectionProperties),
    Description = "Single-column section with one full-width zone.",
    IconClass = "icon-square")]

namespace TrainingGuides.Web.Features.Shared.Sections.SingleColumn;

public class SingleColumnSectionViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.SingleColumnSection";

    public IViewComponentResult Invoke() => View("~/Features/Shared/Sections/SingleColumn/SingleColumnSection.cshtml");
}