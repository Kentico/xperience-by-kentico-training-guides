namespace KBank.Web.Models;

public class TrackingConsentViewModel
{
    public bool CookieAccepted { get; set; }

    public bool IsAgreed { get; set; }

    public string CookieHeader { get; set; }

    public string CookieMessage { get; set; }

    public string AcceptMessage { get; set; }

    public string RedirectUrl { get; set; }

    public string ConfigureMessage { get; set; }

    public string ConsentMapping { get; set; }

}