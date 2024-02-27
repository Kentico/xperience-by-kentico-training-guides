using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Shared.Sections.General;

[assembly: RegisterSection(
    identifier: GeneralSectionViewComponent.IDENTIFIER,
    viewComponentType: typeof(GeneralSectionViewComponent),
    name: "General section",
    propertiesType: typeof(GeneralSectionProperties),
    Description = "Highly customizable general section.",
    IconClass = "icon-square")]

namespace TrainingGuides.Web.Features.Shared.Sections.General;

public class GeneralSectionViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.GeneralSection";

    public IViewComponentResult Invoke(ComponentViewModel<GeneralSectionProperties> sectionProperties)
    {
        var properties = sectionProperties.Properties;
        var model = new GeneralSectionViewModel()
        {
            SectionAnchor = properties.SectionAnchor,
            RoundedCornersClass = properties.RondedCorners ? "tg-corner-v-rnd" : string.Empty
        };

        return View("~/Features/Shared/Sections/General/GeneralSection.cshtml", model);
    }
}
