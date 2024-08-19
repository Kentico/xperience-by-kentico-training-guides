using TrainingGuides.Web.Features.LandingPages.Widgets.HeroBanner;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.Services;
using TrainingGuides.Web.Features.Shared.OptionProviders;

[assembly:
    RegisterWidget(
    identifier: HeroBannerWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(HeroBannerWidgetViewComponent),
    name: "Hero banner",
    propertiesType: typeof(HeroBannerWidgetProperties),
    Description = "Displays text, image, and benefits.",
    IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.LandingPages.Widgets.HeroBanner;

public class HeroBannerWidgetViewComponent : ViewComponent
{
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IContentItemRetrieverService<ProductPage> productRetrieverService;
    private readonly IContentItemRetrieverService<Hero> heroRetrieverService;
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;


    public const string IDENTIFIER = "TrainingGuides.HeroBanner";

    public HeroBannerWidgetViewComponent(IWebPageDataContextRetriever webPageDataContextRetriever,
        IContentItemRetrieverService<ProductPage> productRetrieverService,
        IContentItemRetrieverService<Hero> heroRetrieverService,
        IWebPageUrlRetriever webPageUrlRetriever,
        IPreferredLanguageRetriever preferredLanguageRetriever
        )
    {
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.productRetrieverService = productRetrieverService;
        this.heroRetrieverService = heroRetrieverService;
        this.webPageUrlRetriever = webPageUrlRetriever;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(HeroBannerWidgetProperties properties,
        CancellationToken cancellationToken)
    {
        var banner = new HeroBannerWidgetViewModel();

        if (string.Equals(properties.Mode, "currentProductPage"))
        {
            var context = webPageDataContextRetriever.Retrieve();

            var productPage = await productRetrieverService
                .RetrieveWebPageById(context.WebPage.WebPageItemID,
                    ProductPage.CONTENT_TYPE_NAME,
                    3);

            if (productPage != null)
            {
                banner = GetProductPageBanner(productPage);

                if (banner != null)
                {
                    banner.CTALink = properties.ProductPageAnchor;
                    banner.CTAText = properties.CTA;
                    banner.LinkTitle = properties.CTA;
                }
            }
        }
        else if (string.Equals(properties.Mode, "productPage"))
        {
            if (properties.ProductPage.FirstOrDefault() != null)
            {
                var productPageGuid = properties.ProductPage?.Select(i => i.WebPageGuid).FirstOrDefault();
                var productPage = productPageGuid.HasValue
                    ? await productRetrieverService
                        .RetrieveWebPageByGuid((Guid)productPageGuid,
                            ProductPage.CONTENT_TYPE_NAME,
                            3)
                    : null;

                banner = GetProductPageBanner(productPage);
                if (banner != null)
                {
                    string relativeUrl = productPage?.SystemFields.WebPageUrlPath is not null
                        ? $"~/{productPage.SystemFields.WebPageUrlPath}"
                        : string.Empty;
                    banner.CTALink = relativeUrl + (string.IsNullOrWhiteSpace(properties.SelectedProductPageAnchor)
                                        ? string.Empty
                                        : $"#{properties.SelectedProductPageAnchor}");
                    banner.CTAText = properties.CTA;
                }
            }
        }
        else
        {
            if (properties.Hero != null && properties.Hero.Any())
            {
                var heroGuid = properties?.Hero?.Select(i => i.Identifier).ToList().FirstOrDefault();

                var hero = heroGuid.HasValue
                    ? await heroRetrieverService
                        .RetrieveContentItemByGuid((Guid)heroGuid,
                            Hero.CONTENT_TYPE_NAME,
                            3)
                    : null;

                banner = await GetModel(hero, properties, cancellationToken);

                if (banner?.Link != null)
                {
                    banner.CTALink = !string.IsNullOrEmpty(banner.Link.LinkUrl) ? banner!.Link.LinkUrl : banner.Link.LinkToExternal ?? string.Empty;
                    banner.CTAText = !string.IsNullOrEmpty(properties?.CTA) ? properties.CTA : banner.Link.CallToAction;
                    banner.LinkTitle = banner.Link.LinkTitleText;
                }
            }
        }

        if (banner != null)
        {

            banner.DisplayCTA = !string.IsNullOrEmpty(banner.CTALink) && !string.IsNullOrEmpty(banner.CTAText) && properties!.DisplayCTA;
            banner.OpenInNewTab = properties!.OpenInNewTab;

            if (properties.ChangeDesign)
            {
                banner.ShowBenefits = properties.ShowBenefits;
                banner.FullWidth = (properties.Width ?? "circle").Equals("full", StringComparison.InvariantCultureIgnoreCase);
                banner.TextColor = properties.TextColor;
                banner.ThemeClass = new DropdownEnumOptionProvider<TextColorOption>().Parse(properties.TextColor, TextColorOption.Dark) switch
                {
                    TextColorOption.Light => "light",
                    TextColorOption.Dark => "",
                    _ => ""
                };
                banner.ShowImage = properties.ShowImage;
            }

            if (!string.IsNullOrEmpty(banner.Media?.FilePath))
            {
                string backgroundImageStyle = $"background-image: url('{Url.Content(banner.Media.FilePath)}');";
                string backgroundNoRepeatStyle = "background-repeat: no-repeat;background-size: cover;background-position: center";

                string backgroundStyle = banner.FullWidth
                        ? backgroundImageStyle
                        : $"{backgroundImageStyle};{backgroundNoRepeatStyle}";

                banner.StyleAttribute = new HtmlString($"style=\"{backgroundStyle}\"");
            }
        }

        return View("~/Features/LandingPages/Widgets/HeroBanner/HeroBannerWidget.cshtml", banner);
    }

    private HeroBannerWidgetViewModel? GetProductPageBanner(ProductPage? productPage) =>
        productPage == null ? null : GetHeroBannerViewModel(productPage);

    private static HeroBannerWidgetViewModel? GetHeroBannerViewModel(ProductPage productPage)
    {
        var product = productPage.ProductPageProduct.FirstOrDefault();

        if (product != null)
        {
            var benefits = product.ProductBenefits.ToList();
            var media = product.ProductMedia.FirstOrDefault();

            return new HeroBannerWidgetViewModel()
            {
                Header = product.ProductName,
                Subheader = new HtmlString(product.ProductShortDescription),
                Benefits = benefits.Select(BenefitViewModel.GetViewModel).ToList(),
                Media = media != null
                    ? AssetViewModel.GetViewModel(media)
                    : new AssetViewModel()
            };
        }

        return null;
    }

    private async Task<HeroBannerWidgetViewModel?> GetModel(Hero? hero, HeroBannerWidgetProperties? properties, CancellationToken _)
    {
        if (hero == null || properties == null)
        {
            return null;
        }

        var guid = hero.HeroTarget?.FirstOrDefault()?.WebPageGuid ?? new Guid();

        var url = await webPageUrlRetriever.Retrieve(guid, preferredLanguageRetriever.Get());

        var media = hero.HeroMedia.FirstOrDefault();

        var model = new HeroBannerWidgetViewModel()
        {
            Header = hero.HeroHeader,
            Subheader = new HtmlString(hero.HeroSubheader),
            Benefits = hero.HeroBenefits.Select(BenefitViewModel.GetViewModel).ToList(),
            Link = new LinkViewModel()
            {
                LinkUrl = url.RelativePath,
                CallToAction = hero.HeroCallToAction
            },
            Media = media != null
                ? AssetViewModel.GetViewModel(media)
                : new AssetViewModel()
        };

        return model;
    }
}
