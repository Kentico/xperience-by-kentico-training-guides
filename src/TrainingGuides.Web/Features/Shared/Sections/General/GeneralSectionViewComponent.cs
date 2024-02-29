using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.Sections.General;
using TrainingGuides.Web.Features.Shared.Services;

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

        if (!Enum.TryParse(properties.ColumnLayout, out ColumnLayoutOption columnLayout))
        {
            columnLayout = ColumnLayoutOption.OneColumn;
        }

        var model = new GeneralSectionViewModel()
        {
            SectionAnchor = properties.SectionAnchor,
            ColumnLayout = columnLayout,
            ColorScheme = properties.ColorScheme,
            CornerStyle = properties.CornerStyle,
        };

        return View("~/Features/Shared/Sections/General/GeneralSection.cshtml", model);
    }
}
