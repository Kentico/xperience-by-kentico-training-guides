using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Shared.Sections.FormColumn;

[assembly: RegisterSection(
    identifier: FormColumnSectionViewComponent.IDENTIFIER,
    viewComponentType: typeof(FormColumnSectionViewComponent),
    name: "Form column",
    propertiesType: typeof(FormColumnSectionProperties),
    Description = "Form column section.",
    IconClass = "icon-square")]

namespace TrainingGuides.Web.Features.Shared.Sections.FormColumn;

public class FormColumnSectionViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.FormColumnSection";

    public IViewComponentResult Invoke(ComponentViewModel<FormColumnSectionProperties> sectionProperties)
    {
        var model = new FormColumnSectionViewModel()
        {
            SectionAnchor = sectionProperties.Properties.SectionAnchor,
            ShowContents = true
        };

        return View("~/Features/Shared/Sections/FormColumn/FormColumnSection.cshtml", model);
    }
}