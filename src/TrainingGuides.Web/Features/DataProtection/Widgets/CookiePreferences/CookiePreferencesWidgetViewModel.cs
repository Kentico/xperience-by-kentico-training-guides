using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.DataProtection.Widgets.CookiePreferences;

public class CookiePreferencesWidgetViewModel
{
    public string EssentialHeader { get; set; } = string.Empty;
    public string EssentialDescription { get; set; } = string.Empty;
    public string PreferenceHeader { get; set; } = string.Empty;
    public HtmlString PreferenceDescriptionHtml { get; set; } = HtmlString.Empty;
    public string AnalyticalHeader { get; set; } = string.Empty;
    public HtmlString AnalyticalDescriptionHtml { get; set; } = HtmlString.Empty;
    public string MarketingHeader { get; set; } = string.Empty;
    public HtmlString MarketingDescriptionHtml { get; set; } = HtmlString.Empty;
    public string ButtonText { get; set; } = string.Empty;
    public int CookieLevelSelected { get; set; }
    public string ConsentMapping { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
}
