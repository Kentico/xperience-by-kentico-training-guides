using TrainingGuides.Web.Features.Activities.Widgets.PageLike;
using TrainingGuides.Web.Features.DataProtection.Widgets.CookiePreferences;
using TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;
using TrainingGuides.Web.Features.Shared.Sections.FormColumn;
using TrainingGuides.Web.Features.Shared.Sections.SingleColumn;

namespace TrainingGuides.Web;

public static class ComponentIdentifiers
{
    public static class Sections
    {
        public const string SINGLE_COLUMN = SingleColumnSectionViewComponent.IDENTIFIER;
        public const string FORM_COLUMN = FormColumnSectionViewComponent.IDENTIFIER;
        public const string FORM_COLUMN_CONSENT = FormColumnSectionViewComponent.IDENTIFIER;
    }

    public static class Widgets
    {
        public const string PAGE_LIKE = PageLikeWidgetViewComponent.IDENTIFIER;
        public const string COOKIE_PREFERENCES = CookiePreferencesWidgetViewComponent.IDENTIFIER;
        public const string CALL_TO_ACTION = CallToActionWidgetComponent.IDENTIFIER;
    }
}
