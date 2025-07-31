using Kentico.Xperience.Mjml.StarterKit.Rcl.Sections;
using TrainingGuides.Web.Features.Activities.Widgets.PageLike;
using TrainingGuides.Web.Features.Articles.EmailWidgets;
using TrainingGuides.Web.Features.Articles.Widgets.ArticleList;
using TrainingGuides.Web.Features.DataProtection.Widgets.CookiePreferences;
using TrainingGuides.Web.Features.Gallery.Widgets.GalleryWidget;
using TrainingGuides.Web.Features.Html.Widgets.HtmlCode;
using TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;
using TrainingGuides.Web.Features.LandingPages.Widgets.HeroBanner;
using TrainingGuides.Web.Features.LandingPages.Widgets.SimpleCallToAction;
using TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;
using TrainingGuides.Web.Features.Membership.Widgets.Registration;
using TrainingGuides.Web.Features.Membership.Widgets.ResetPassword;
using TrainingGuides.Web.Features.Membership.Widgets.SignIn;
using TrainingGuides.Web.Features.Products.Widgets.Product;
using TrainingGuides.Web.Features.Products.Widgets.ProductComparator;
using TrainingGuides.Web.Features.Shared.EmailBuilder.Sections;
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
        public const string GALLERY = GalleryWidgetViewComponent.IDENTIFIER;
        public const string SIGN_IN = SignInWidgetViewComponent.IDENTIFIER;
        public const string LINK_OR_SIGN_OUT = LinkOrSignOutWidgetViewComponent.IDENTIFIER;
        public const string REGISTRATION = RegistrationWidgetViewComponent.IDENTIFIER;
        public const string RESET_PASSWORD = ResetPasswordWidgetViewComponent.IDENTIFIER;
    }

    public static class EmailBuilderSections
    {
        public const string GENERAL = GeneralEmailSection.IDENTIFIER;

        // Kentico.Xperience.Mjml.StarterKit.Rcl sections
        public const string FULL_WIDTH = FullWidthEmailSection.IDENTIFIER;
        public const string TWO_COLUMN = TwoColumnEmailSection.IDENTIFIER;
    }

    public static class EmailBuilderWidgets
    {
        public const string ARTICLE = ArticleEmailWidget.IDENTIFIER;

        // Kentico.Xperience.Mjml.StarterKit.Rcl widgets
        public const string BUTTON = Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets.ButtonWidget.IDENTIFIER;
        public const string IMAGE = Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets.ImageWidget.IDENTIFIER;
        public const string TEXT = Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets.TextWidget.IDENTIFIER;
        public const string DIVIDER = Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets.DividerWidget.IDENTIFIER;
        public const string PRODUCT = Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets.ProductWidget.IDENTIFIER;
    }
}
