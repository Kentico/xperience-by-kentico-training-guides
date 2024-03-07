using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.IdentityModel.Tokens;
using CMS.Helpers;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using TrainingGuides.Web.Features.Products.Models;
using TrainingGuides.Web.Features.Products.Services;
using TrainingGuides.Web.Features.Products.Widgets.Product;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWidget(
    identifier: ProductWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(ProductWidgetViewComponent),
    name: "Product",
    propertiesType: typeof(ProductWidgetProperties),
    Description = "Displays selected product.",
    IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Products.Widgets.Product;

public class ProductWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ProductWidget";
    private const string BS_DROP_SHADOW_CLASS = "shadow";
    private const string BS_MARGIN_CLASS = "m-3";
    private const string BS_PADDING_CLASS = "p-3";

    private readonly IContentItemRetrieverService<ProductPage> productRetrieverService;
    private readonly IWebPageQueryResultMapper webPageQueryResultMapper;
    private readonly IComponentStyleEnumService componentStyleEnumService;
    private readonly IProductPageService productPageService;
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;

    public ProductWidgetViewComponent(
        IContentItemRetrieverService<ProductPage> productRetrieverService,
        IWebPageQueryResultMapper webPageQueryResultMapper,
        IComponentStyleEnumService componentStyleEnumService,
        IProductPageService productPageService,
        IWebPageDataContextRetriever webPageDataContextRetriever)
    {
        this.productRetrieverService = productRetrieverService;
        this.webPageQueryResultMapper = webPageQueryResultMapper;
        this.componentStyleEnumService = componentStyleEnumService;
        this.productPageService = productPageService;
        this.webPageDataContextRetriever = webPageDataContextRetriever;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ProductWidgetProperties properties)
    {
        var model = await GetProductWidgetViewModel(properties);
        return View("~/Features/Products/Widgets/Product/ProductWidget.cshtml", model);
    }

    private async Task<ProductWidgetViewModel> GetProductWidgetViewModel(ProductWidgetProperties properties)
    {
        if (properties == null)
            return new ProductWidgetViewModel();

        var productPageViewModel = await GetProductPageViewModel(properties);

        return new ProductWidgetViewModel()
        {
            Product = productPageViewModel,
            ShowProductFeatures = properties.ShowProductFeatures,
            ProductImage = properties.ShowProductImage ? productPageViewModel?.Media.FirstOrDefault() : null,
            ShowAdvanced = properties.ShowAdvanced,
            ColorScheme = properties.ColorScheme,
            CornerStyle = IsFullSizeImageLayout(properties.ImagePosition)
                ? nameof(CornerStyleOption.Sharp)
                : properties.CornerStyle,
            ParentElementCssClasses = GetParentElementCssClasses(properties).Join(" "),
            MainContentElementCssClasses = GetMainContentElementCssClasses(properties).Join(" "),
            ImageElementCssClasses = GetImageElementCssClasses(properties).Join(" "),
            CallToActionCssClasses = componentStyleEnumService
                .GetColorSchemeClasses(componentStyleEnumService.GetLinkStyle(properties.CallToActionStyle ?? string.Empty))
                .Join(" ")
        };
    }

    private async Task<ProductPage?> GetProductPage(ProductWidgetProperties properties)
    {
        ProductPage? productPage;

        if (properties.Mode.Equals(ProductWidgetMode.CURRENT_PAGE))
        {
            productPage = await productRetrieverService
                .RetrieveWebPageById(webPageDataContextRetriever.Retrieve().WebPage.WebPageItemID,
                    ProductPage.CONTENT_TYPE_NAME,
                    webPageQueryResultMapper.Map<ProductPage>,
                    3);
        }
        else
        {
            var guid = properties.SelectedProductPage?.Select(webPage => webPage.WebPageGuid).FirstOrDefault();

            productPage = guid.HasValue
                ? await productRetrieverService.RetrieveWebPageByGuid(
                    guid,
                    ProductPage.CONTENT_TYPE_NAME,
                    webPageQueryResultMapper.Map<ProductPage>,
                    4)
                : null;
        }

        return productPage;
    }
    private async Task<ProductPageViewModel?> GetProductPageViewModel(ProductWidgetProperties properties)
    {
        var productPage = await GetProductPage(properties);

        if (!properties.PageAnchor.IsNullOrEmpty())
            properties.PageAnchor = properties.PageAnchor!.StartsWith('#') ? properties.PageAnchor : $"#{properties.PageAnchor}";

        return productPage != null
            ? await productPageService.GetProductPageViewModel(
                productPage: productPage,
                getMedia: properties.ShowProductImage,
                getFeatures: properties.ShowProductFeatures,
                getBenefits: properties.ShowProductBenefits,
                callToAction: properties.CallToAction,
                callToActionLink: properties.PageAnchor,
                openInNewTab: properties.OpenInNewTab,
                getPrice: properties.ShowProductFeatures)
            : null;
    }

    private List<string> GetParentElementCssClasses(ProductWidgetProperties properties)
    {
        const string PARENT_ELEMENT_BASE_CLASS = "tg-product";
        const string LAYOUT_ASCENDING_CLASS = "tg-layout-ascending";
        const string LAYOUT_DESCENDING_CLASS = "tg-layout-descending";

        List<string> cssClasses = [PARENT_ELEMENT_BASE_CLASS];

        if (properties.ShowProductImage)
        {
            string imagePositionCssClass = properties.ImagePosition switch
            {
                nameof(ImagePositionOption.Ascending) => LAYOUT_ASCENDING_CLASS,
                nameof(ImagePositionOption.Descending) => LAYOUT_DESCENDING_CLASS,
                nameof(ImagePositionOption.FullWidth) => string.Empty,
                _ => string.Empty
            };
            cssClasses.Add(imagePositionCssClass);
        }

        if (IsFullSizeImageLayout(properties.ImagePosition))
        {
            cssClasses.Add(BS_MARGIN_CLASS);

            cssClasses.AddRange(
                componentStyleEnumService.GetCornerStyleClasses(
                    componentStyleEnumService.GetCornerStyle(properties.CornerStyle!)));

            if (properties.DropShadow)
                cssClasses.Add(BS_DROP_SHADOW_CLASS);

        }
        return cssClasses;
    }

    private List<string> GetMainContentElementCssClasses(ProductWidgetProperties properties)
    {
        const string MAIN_CONTENT_BASE_CSS_CLASS = "tg-product_main";
        const string ALIGN_LEFT_CLASS = "align-left";
        const string ALIGN_CENTER_CLASS = "align-center";
        const string ALIGN_RIGHT_CLASS = "align-right";

        List<string> cssClasses = [MAIN_CONTENT_BASE_CSS_CLASS, BS_PADDING_CLASS];

        cssClasses.AddRange(GetChildElementCssClasses(properties));

        string textAlignmentClass = properties.TextAlignment switch
        {
            nameof(ContentAlignmentOption.Left) => ALIGN_LEFT_CLASS,
            nameof(ContentAlignmentOption.Center) => ALIGN_CENTER_CLASS,
            nameof(ContentAlignmentOption.Right) => ALIGN_RIGHT_CLASS,
            _ => ALIGN_LEFT_CLASS
        };

        cssClasses.Add(textAlignmentClass);
        return cssClasses;
    }

    private List<string> GetImageElementCssClasses(ProductWidgetProperties properties)
    {
        const string IMAGE_BASE_CSS_CLASS = "tg-product_img";

        List<string> cssClasses = [IMAGE_BASE_CSS_CLASS];

        cssClasses.AddRange(GetChildElementCssClasses(properties));
        return cssClasses;
    }

    private List<string> GetChildElementCssClasses(ProductWidgetProperties properties)
    {
        List<string> cssClasses = [];

        if (!IsFullSizeImageLayout(properties.ImagePosition))
        {
            if (properties.DropShadow)
                cssClasses.Add(BS_DROP_SHADOW_CLASS);

            cssClasses.Add(BS_MARGIN_CLASS);
        }
        return cssClasses;
    }

    private bool IsFullSizeImageLayout(string? imagePosition) =>
        imagePosition == null || imagePosition.Equals(nameof(ImagePositionOption.FullWidth));
}