using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.DataProtection.Services;
using TrainingGuides.Web.Features.Shared.Sections.FormColumn;

[assembly: RegisterSection(
    identifier: FormColumnSectionConsentViewComponent.IDENTIFIER,
    viewComponentType: typeof(FormColumnSectionConsentViewComponent),
    name: "Form column: Consent-based",
    propertiesType: typeof(FormColumnSectionProperties),
    Description = "Form column section that hides its contents if the visitor has not consented to tracking.",
    IconClass = "icon-square")]

namespace TrainingGuides.Web.Features.Shared.Sections.FormColumn;

public class FormColumnSectionConsentViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.FormColumnSectionConsent";

    private readonly ICookieConsentService cookieConsentService;

    public FormColumnSectionConsentViewComponent(ICookieConsentService cookieConsentService)
    {
        this.cookieConsentService = cookieConsentService;
    }

    public IViewComponentResult Invoke(ComponentViewModel<FormColumnSectionProperties> sectionProperties)
    {
        //If the CMSCookieLevel is set to All (1000) or higher, and Data Protection is set up, it means the visitor has the appropriate consent level for tracking.
        bool showContents = cookieConsentService.CurrentContactCanBeTracked();

        var model = new FormColumnSectionViewModel()
        {
            SectionAnchor = sectionProperties.Properties.SectionAnchor,
            ShowContents = showContents
        };

        return View("~/Features/Shared/Sections/FormColumn/FormColumnSection.cshtml", model);
    }
}
