using TrainingGuides.Web.Features.Activities.Widgets.PageLike;
using TrainingGuides.Web.Features.Articles.Widgets.ArticleList;
using TrainingGuides.Web.Features.DataProtection.Widgets.CookiePreferences;
using TrainingGuides.Web.Features.Html.Widgets.HtmlCode;
using TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;
using TrainingGuides.Web.Features.LandingPages.Widgets.HeroBanner;
using TrainingGuides.Web.Features.LandingPages.Widgets.SimpleCallToAction;
using TrainingGuides.Web.Features.Membership.Widgets.Authentication;
using TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;
using TrainingGuides.Web.Features.Products.Widgets.Product;
using TrainingGuides.Web.Features.Products.Widgets.ProductComparator;
using TrainingGuides.Web.Features.Shared.Sections.FormColumn;
using TrainingGuides.Web.Features.Shared.Sections.General;
using TrainingGuides.Web.Features.Shared.Sections.SingleColumn;
using TrainingGuides.Web.Features.Videos.Widgets.VideoEmbed;

namespace TrainingGuides.Web;

public static class ComponentIdentifiers
{
    public static class Sections
    {
        public const string SINGLE_COLUMN = SingleColumnSectionViewComponent.IDENTIFIER;
        public const string FORM_COLUMN = FormColumnSectionViewComponent.IDENTIFIER;
        public const string FORM_COLUMN_CONSENT = FormColumnSectionViewComponent.IDENTIFIER;
        public const string GENERAL = GeneralSectionViewComponent.IDENTIFIER;
    }

    public static class Widgets
    {
        public const string PAGE_LIKE = PageLikeWidgetViewComponent.IDENTIFIER;
        public const string COOKIE_PREFERENCES = CookiePreferencesWidgetViewComponent.IDENTIFIER;
        public const string CALL_TO_ACTION = CallToActionWidgetViewComponent.IDENTIFIER;
        public const string HERO_BANNER = HeroBannerWidgetViewComponent.IDENTIFIER;
        public const string PRODUCT_COMPARATOR = ProductComparatorWidgetViewComponent.IDENTIFIER;
        public const string HTML_CODE = HtmlCodeWidgetViewComponent.IDENTIFIER;
        public const string ARTICLE_LIST = ArticleListWidgetViewComponent.IDENTIFIER;
        public const string SIMPLE_CALL_TO_ACTION = SimpleCallToActionWidgetViewComponent.IDENTIFIER;
        public const string PRODUCT = ProductWidgetViewComponent.IDENTIFIER;
        public const string VIDEO_EMBED = VideoEmbedWidgetViewComponent.IDENTIFIER;
        public const string SING_IN = SignInWidgetViewComponent.IDENTIFIER;
        public const string LINK_OR_SIGN_OUT = LinkOrSignOutWidgetViewComponent.IDENTIFIER;

    }
}
