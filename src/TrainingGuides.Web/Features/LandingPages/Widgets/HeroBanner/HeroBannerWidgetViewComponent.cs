using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.Websites.Internal;
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
    private readonly IContentItemRetrieverService contentItemRetrieverService;
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    

    public const string IDENTIFIER = "TrainingGuides.HeroBannerWidget";

    public HeroBannerWidgetViewComponent(IWebPageDataContextRetriever webPageDataContextRetriever,
        IWebPageQueryResultMapper webPageQueryResultMapper,
        IContentQueryResultMapper contentQueryResultMapper,
        IContentItemRetrieverService<ProductPage> productRetrieverService,
        IContentItemRetrieverService<Hero> heroRetrieverService,
        IContentItemRetrieverService contentItemRetrieverService,
        IWebPageUrlRetriever webPageUrlRetriever,
        IPreferredLanguageRetriever preferredLanguageRetriever
        )
    {
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.webPageQueryResultMapper = webPageQueryResultMapper;
        this.contentQueryResultMapper = contentQueryResultMapper;
        this.productRetrieverService = productRetrieverService;
        this.heroRetrieverService = heroRetrieverService;
        this.contentItemRetrieverService = contentItemRetrieverService;
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
                    1);

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
                        webPageQueryResultMapper.Map<ProductPage>,
                        1)
                    : null;

                banner = GetProductPageBanner(productPage);
                if (banner != null)
                {
                    string relativeUrl = productPage?.SystemFields.WebPageUrlPath is not null
                        ? $"~/{productPage.SystemFields.WebPageUrlPath}"
                        : string.Empty;
                    banner.CTALink = relativeUrl +
                                    (string.IsNullOrWhiteSpace(properties.SelectedProductPageAnchor)
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
                        contentQueryResultMapper.Map<Hero>,
                        1)
                    : null;

                banner = await GetModel(hero, properties, cancellationToken);

                if (banner?.Link != null)
                {
                    banner.CTALink = !string.IsNullOrEmpty(banner.Link!.Page) ? banner.Link.Page : banner.Link.LinkToExternal;
                    banner.CTAText = !string.IsNullOrEmpty(properties?.CTA) ? properties.CTA : banner.Link.CTA;
                    banner.LinkTitle = banner.Link.LinkTitleText;
                }
            }
        }

        if (banner != null)
        {
            if (properties!.CustomAbsoluteUrl && !string.IsNullOrWhiteSpace(properties.AbsoluteUrl))
            {
                banner.CTALink = properties.AbsoluteUrl;
            }

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
                banner.StyleAttribute = new HtmlString(banner.FullWidth
                    ? $"style=\"background-image: url('{Url.Content(banner.Media.FilePath)}');\""
                    : $"style=\"background-image: url('{Url.Content(banner.Media.FilePath)}'); background-repeat: no-repeat;background-size: cover;background-position: center\"");
            }
        }

        return View("~/Features/LandingPages/Widgets/HeroBanner/_HeroBannerWidget.cshtml", banner);
    }

    private HeroBannerViewModel? GetProductPageBanner(ProductPage? productPage) => productPage == null ? null : GetHeroBannerViewModel(productPage);

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
                Subheader = product.ProductShortDescription,
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
            
            var media = hero.HeroMedia.FirstOrDefault();

            var model = new HeroBannerViewModel()
            {
                Header = hero.HeroHeader,
                Subheader = hero.HeroSubheader,
                Benefits = hero.HeroBenefits.Select(BenefitViewModel.GetViewModel).ToList(),
                Link = new LinkViewModel()
                {
                    Page = url.RelativePath,
                    CTA = hero.HeroCallToAction
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
