using TrainingGuides.Web.Features.Articles;
using TrainingGuides.Web.Features.Downloads;
using TrainingGuides.Web.Features.LandingPages;
using TrainingGuides.Web.Features.Shared.Sections.FormColumn;
using TrainingGuides.Web.Features.Shared.Sections.SingleColumn;

namespace TrainingGuides.Web.Components;

public static class ComponentIdentifiers
{
    public static class Sections
    {
        public const string SINGLE_COLUMN = SingleColumnSectionViewComponent.IDENTIFIER;
        public const string FORM_COLUMN = FormColumnSectionViewComponent.IDENTIFIER;
        public const string FORM_COLUMN_CONSENT = FormColumnSectionViewComponent.IDENTIFIER;
    }

    public static class PageTemplates
    {
        public const string ARTICLE = ArticlePagePageTemplate.IDENTIFIER;
        public const string DOWNLOADS = DownloadsPagePageTemplate.IDENTIFIER;
        public const string EMPTY_PAGE_TEMPLATE = EmptyPagePageTemplate.IDENTIFIER;
        public const string LANDING_PAGE_TEMPLATE = LandingPagePageTemplate.IDENTIFIER;
    }
}
