using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Components.Sections;

[assembly: RegisterSection(
    identifier: SingleColumnSectionViewComponent.IDENTIFIER,
    viewComponentType: typeof(SingleColumnSectionViewComponent),
    name: "1 column",
    propertiesType: typeof(SingleColumnSectionProperties),
    Description = "Single-column section with one full-width zone.",
    IconClass = "icon-square")]

namespace TrainingGuides.Web.Components.Sections;

public class SingleColumnSectionViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.SingleColumnSection";

    public IViewComponentResult Invoke() => View("~/Components/Sections/SingleColumnSection/SingleColumnSection.cshtml");
}
