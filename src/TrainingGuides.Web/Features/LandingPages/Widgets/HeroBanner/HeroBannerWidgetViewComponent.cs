using CMS.ContentEngine;
//remove KBank leftover
using Kbank.Web.Components.Widgets.HeroBannerWidget;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Features.LandingPages.Widgets.HeroBannerWidget;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(
    identifier: HeroBannerWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(HeroBannerWidgetViewComponent),
    name: "Hero banner",
    propertiesType: typeof(HeroBannerWidgetProperties),
    Description = "Displays text, image, and benefits.",
    IconClass = "icon-ribbon")]

namespace TrainingGuides.Features.LandingPages.Widgets.HeroBannerWidget;

public class HeroBannerWidgetViewComponent : ViewComponent
{
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IWebPageQueryResultMapper webPageQueryResultMapper;
    private readonly IContentQueryResultMapper contentQueryResultMapper;
    private readonly IContentItemRetrieverService<ProductPage> productRetrieverService;
    private readonly IContentItemRetrieverService<Hero> heroRetrieverService;
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;


    public const string IDENTIFIER = "TrainingGuides.HeroBannerWidget";

    public HeroBannerWidgetViewComponent(IWebPageDataContextRetriever webPageDataContextRetriever,
        IWebPageQueryResultMapper webPageQueryResultMapper,
        IContentQueryResultMapper contentQueryResultMapper,
        IContentItemRetrieverService<ProductPage> productRetrieverService,
        IContentItemRetrieverService<Hero> heroRetrieverService,
        IWebPageUrlRetriever webPageUrlRetriever,
        IPreferredLanguageRetriever preferredLanguageRetriever
        )
    {
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.webPageQueryResultMapper = webPageQueryResultMapper;
        this.contentQueryResultMapper = contentQueryResultMapper;
        this.productRetrieverService = productRetrieverService;
        this.heroRetrieverService = heroRetrieverService;
        this.webPageUrlRetriever = webPageUrlRetriever;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(HeroBannerWidgetProperties properties,
        CancellationToken cancellationToken)
    {
        var banner = new HeroBannerViewModel();

        if (string.Equals(properties.Mode, "currentProductPage"))
        {
            var context = webPageDataContextRetriever.Retrieve();

            var productPage = await productRetrieverService
                .RetrieveWebPageById(context.WebPage.WebPageItemID,
                    ProductPage.CONTENT_TYPE_NAME,
                    webPageQueryResultMapper.Map<ProductPage>,
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
                        //indentating the method like this makes it more readable in my opinion (it belongs with the first part of the condition)
                        .RetrieveWebPageByGuid((Guid)productPageGuid,
                            ProductPage.CONTENT_TYPE_NAME,
                            webPageQueryResultMapper.Map<ProductPage>,
                            3)
                    : null;

                banner = GetProductPageBanner(productPage);
                if (banner != null)
                {
                    string relativeUrl = productPage?.SystemFields.WebPageUrlPath is not null
                        ? $"~/{productPage.SystemFields.WebPageUrlPath}"
                        : string.Empty;
                    //just a proposal - i think this reads better
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
                //same indentation comment as above
                    ? await heroRetrieverService
                        .RetrieveContentItemByGuid((Guid)heroGuid,
                            Hero.CONTENT_TYPE_NAME,
                            contentQueryResultMapper.Map<Hero>,
                            3)
                    : null;

                banner = await GetModel(hero, properties, cancellationToken);

                if (banner?.Link != null)
                {
                    banner.CTALink = !string.IsNullOrEmpty(banner.Link.Page) ? banner.Link.Page : banner.Link.LinkToExternal;
                    banner.CTAText = !string.IsNullOrEmpty(properties?.CTA) ? properties.CTA : banner.Link.CTA;
                    banner.LinkTitle = banner.Link.LinkTitleText;
                }
            }
        }

        if (banner != null)
        {

            banner.DisplayCTA = !string.IsNullOrEmpty(banner.CTALink) && !string.IsNullOrEmpty(banner.CTAText) && properties.DisplayCTA;
            banner.OpenInNewTab = properties.OpenInNewTab;

            if (properties.ChangeDesign)
            {
                banner.ShowBenefits = properties.ShowBenefits;
                banner.FullWidth = (properties.Width ?? "circle").Equals("full", StringComparison.InvariantCultureIgnoreCase);
                banner.TextColor = properties.TextColor;
                banner.ShowImage = properties.ShowImage;
            }

            if (!string.IsNullOrEmpty(banner.Media?.FilePath))
            {
                // My suggestion is to refactor this to avoid repetition and accidental typo-related issues.
                // Yes, it's a bit longer, my hope is that putting the styles into variables and naming them meaningfully will help with readability.
                // banner.StyleAttribute = new HtmlString(banner.FullWidth
                //     ? $"style=\"background-image: url('{Url.Content(banner.Media.FilePath)}');\""
                //     : $"style=\"background-image: url('{Url.Content(banner.Media.FilePath)}'); background-repeat: no-repeat;background-size: cover;background-position: center\"");

                string backgroundImageStyle = $"background-image: url('{Url.Content(banner.Media.FilePath)}');";
                string backgroundNoRepeatStyle = "background-repeat: no-repeat;background-size: cover;background-position: center";

                string backgroundStyle = banner.FullWidth
                        ? backgroundImageStyle
                        : $"{backgroundImageStyle};{backgroundNoRepeatStyle}";

                banner.StyleAttribute = new HtmlString($"style=\"{backgroundStyle}\"");
            }
        }

        return View("~/Features/LandingPages/Widgets/HeroBanner/_HeroBannerWidget.cshtml", banner);
    }

    //suggestion: I think it reads better to have the arrow function body on a new line
    private HeroBannerViewModel? GetProductPageBanner(ProductPage? productPage) =>
        productPage == null ? null : GetHeroBannerViewModel(productPage);

    private static HeroBannerViewModel? GetHeroBannerViewModel(ProductPage productPage)
    {
        var product = productPage.ProductPageProduct.FirstOrDefault();

        if (product != null)
        {
            var benefits = product.ProductBenefits.ToList();
            var media = product.ProductMedia.FirstOrDefault();

            return new HeroBannerViewModel()
            {
                Header = product.ProductName,
                Subheader = new HtmlString(product.ProductShortDescription),
                Benefits = benefits.Select(BenefitViewModel.GetViewModel).ToList(),
                Media = media != null
                    ? AssetViewModel.GetViewModel(media)
                    : null
            };
        }

        return null;
    }

    private async Task<HeroBannerViewModel?> GetModel(Hero? hero, HeroBannerWidgetProperties? properties, CancellationToken _)
    {
        if (hero != null && properties != null)
        {
            var guid = hero?.HeroTarget?.FirstOrDefault()?.WebPageGuid ?? new Guid();

            //are these comments leftovers to eb removed? If they are left on purpose, it would be good to have a comment with an explanation of some kind. 

            //var dataSet = WebPageItemInfo.Provider.Get().WhereEquals(nameof(WebPageFields.WebPageItemGUID);
            //.Source(sourceItem => sourceItem
            //    .Join<ContentItemInfo>(nameof(WebPageItemInfo.WebPageItemContentItemID), nameof(ContentItemInfo.ContentItemID))
            //    .Join<DataClassInfo>(nameof(ContentItemInfo.ContentItemContentTypeID), nameof(DataClassInfo.ClassID)))
            //.Columns(nameof(DataClassInfo.ClassName))
            //.Result;

            //var contentTypeName = dataSet.Tables?[0]?.Rows?[0]?[nameof(DataClassInfo.ClassName)] ?? string.Empty

            var url = await webPageUrlRetriever.Retrieve(guid, preferredLanguageRetriever.Get());

            //var webPage = await contentItemRetrieverService.RetrieveWebPageByGuid(guid,
            //"");//TODO figure out how to get the codename, it's not available in the selector >:/


            //hero will never be null here (the if condition above), use the ! (null forgiving operator) to get rid of warnings
            var media = hero!.HeroMedia.FirstOrDefault();

            var model = new HeroBannerViewModel()
            {
                Header = hero!.HeroHeader,
                Subheader = new HtmlString(hero?.HeroSubheader),
                Benefits = hero!.HeroBenefits.Select(BenefitViewModel.GetViewModel).ToList(),
                Link = new LinkViewModel()
                {
                    Page = url.RelativePath,
                    CTA = hero!.HeroCallToAction
                },
                Media = media != null
                    ? AssetViewModel.GetViewModel(media)
                    : null
            };

            return model;
        }

        return null;
    }
}
