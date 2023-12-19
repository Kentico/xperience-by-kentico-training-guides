using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.DataProtection.Widgets.CookiePreferences;

public class CookiePreferencesWidgetViewModel
{
    public string EssentialHeader { get; set; }
    public string EssentialDescription { get; set; }
    public string PreferenceHeader { get; set; }
    public HtmlString PreferenceDescription { get; set; }
    public string AnalyticalHeader { get; set; }
    public HtmlString AnalyticalDescription { get; set; }
    public string MarketingHeader { get; set; }
    public HtmlString MarketingDescription { get; set; }
    public string ButtonText { get; set; }
    public int CookieLevelSelected { get; set; }
    public string ConsentMapping { get; set; }
    public string BaseUrl { get; set; }
}
