using Microsoft.AspNetCore.Html;
namespace TrainingGuides.Web.Features.DataProtection.ViewComponents.TrackingConsent;

public class TrackingConsentViewModel
{
    public bool CookieAccepted { get; set; }
    public bool IsAgreed { get; set; }
    public HtmlString CookieHeaderHtml { get; set; } = HtmlString.Empty;
    public HtmlString CookieMessageHtml { get; set; } = HtmlString.Empty;
    public string AcceptMessage { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
    public string ConfigureMessage { get; set; } = string.Empty;
    public string ConsentMapping { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string BaseUrlWithLanguage { get; set; } = string.Empty;
}
