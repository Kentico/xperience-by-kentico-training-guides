using Microsoft.AspNetCore.Html;
namespace TrainingGuides.Web.Features.DataProtection.ViewComponents.TrackingConsent;

public class TrackingConsentViewModel
{
    public bool CookieAccepted { get; set; }

    public bool IsAgreed { get; set; }

    public HtmlString CookieHeader { get; set; }

    public HtmlString CookieMessage { get; set; }

    public string AcceptMessage { get; set; }

    public string RedirectUrl { get; set; }

    public string ConfigureMessage { get; set; }

    public string ConsentMapping { get; set; }

}
