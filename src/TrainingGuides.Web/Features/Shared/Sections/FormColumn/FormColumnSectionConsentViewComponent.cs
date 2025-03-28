using CMS.Base.Internal;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.DataProtection.Services;
using TrainingGuides.Web.Features.Shared.Sections.FormColumn;
using TrainingGuides.Web.Features.Shared.Services;

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
    private readonly IStringLocalizer<SharedResources> stringLocalizer;
    private readonly IHttpRequestService httpRequestService;
    private readonly IHttpContextAccessor httpContextAccessor;


    public FormColumnSectionConsentViewComponent(ICookieConsentService cookieConsentService,
        IStringLocalizer<SharedResources> stringLocalizer,
        IHttpRequestService httpRequestService,
        IHttpContextAccessor httpContextAccessor)
    {
        this.cookieConsentService = cookieConsentService;
        this.stringLocalizer = stringLocalizer;
        this.httpRequestService = httpRequestService;
        this.httpContextAccessor = httpContextAccessor;
    }

    public IViewComponentResult Invoke(ComponentViewModel<FormColumnSectionProperties> sectionProperties)
    {
        var httpContext = httpContextAccessor.HttpContext;

        bool showContents = cookieConsentService.CurrentContactCanBeTracked() // Display if the visitor has consented to tracking.
            || httpContext.Kentico().PageBuilder().GetMode() != PageBuilderMode.Off // Display if the page is in Page Builder mode.
            || httpContext.Kentico().Preview().Enabled; // Display if the page is in Preview mode.

        var cookiePolicyUrlBuilder = new UriBuilder(httpRequestService.GetBaseUrlWithLanguage());
        cookiePolicyUrlBuilder.Path = httpRequestService.CombineUrlPaths(cookiePolicyUrlBuilder.Path, "cookie-policy");

        var noConsentHtml = new HtmlString(
            "<div>" +
            stringLocalizer["The content of this section includes tracking functionality. To view it, please consent to Marketing cookies."] +
            $"</div><div>"
            + $"<a href=\"{cookiePolicyUrlBuilder}\">{stringLocalizer["Configure cookies"]}</a>"
            + "</div>");

        var model = new FormColumnSectionViewModel()
        {
            SectionAnchor = sectionProperties.Properties.SectionAnchor,
            ShowContents = showContents,
            NoConsentHtml = noConsentHtml
        };

        return View("~/Features/Shared/Sections/FormColumn/FormColumnSection.cshtml", model);
    }
}
