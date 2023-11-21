namespace TrainingGuides.Web.Components.Widgets.CookiePreferences
{
    public class CookiePreferencesWidgetViewModel
    {
        public string EssentialHeader { get; set; }
        public string EssentialDescription { get; set; }
        public string PreferenceHeader { get; set; }
        public string PreferenceDescription { get; set; }
        public string AnalyticalHeader { get; set; }
        public string AnalyticalDescription { get; set; }
        public string MarketingHeader { get; set; }
        public string MarketingDescription { get; set; }
        public string ButtonText { get; set; }
        public int CookieLevelSelected { get; set; }
        public string ConsentMapping { get; set; }
    }
}