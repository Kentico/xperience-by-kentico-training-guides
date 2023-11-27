using TrainingGuides.Web.Components.Sections;

namespace TrainingGuides.Web.Components;

public static class ComponentIdentifiers
{
    public static class Sections
    {
        public const string SINGLE_COLUMN = SingleColumnSectionViewComponent.IDENTIFIER;
        public const string FORM_COLUMN = FormColumnSectionViewComponent.IDENTIFIER;
        public const string FORM_COLUMN_CONSENT = FormColumnSectionViewComponent.IDENTIFIER;
    }
    
    // Sections
    
    //public const string FORM_COLUMN_SECTION = "TrainingGuides.FormColumnSection";
    //public const string FORM_COLUMN_SECTION_CONSENT = "TrainingGuides.FormColumnSectionConsent";

    //Page templates
    public const string ARTICLE_PAGE_TEMPLATE = "TrainingGuides.ArticlePage";
    public const string DOWNLOADS_PAGE_TEMPLATE = "TrainingGuides.DownloadsPage";
    public const string LANDING_PAGE_TEMPLATE = "TrainingGuides.LandingPage";
    public const string EMPTY_PAGE_TEMPLATE = "TrainingGuides.EmptyPage";

}
