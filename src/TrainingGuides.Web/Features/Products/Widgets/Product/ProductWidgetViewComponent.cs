using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using CMS.Helpers;
using Kentico.PageBuilder.Web.Mvc;
using TrainingGuides.Web.Features.Products.Models;
using TrainingGuides.Web.Features.Products.Services;
using TrainingGuides.Web.Features.Products.Widgets.Product;
using TrainingGuides.Web.Features.Shared.Services;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;

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

    public ProductWidgetViewComponent(
        IContentItemRetrieverService<ProductPage> productRetrieverService,
        IWebPageQueryResultMapper webPageQueryResultMapper,
        IComponentStyleEnumService componentStyleEnumService,
        IProductPageService productPageService)
    {
        this.productRetrieverService = productRetrieverService;
        this.webPageQueryResultMapper = webPageQueryResultMapper;
        this.componentStyleEnumService = componentStyleEnumService;
        this.productPageService = productPageService;
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

        var guid = properties.SelectedProductPage?.Select(webPage => webPage.WebPageGuid).FirstOrDefault();
        var product = guid.HasValue
            ? await GetProduct(guid.Value, properties)
            : null;

        return new ProductWidgetViewModel()
        {
            Product = product!,
            ShowProductFeatures = properties.ShowProductFeatures,
            ProductImage = properties.ShowProductImage ? product?.Media.FirstOrDefault() : null,
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

    private async Task<ProductPageViewModel?> GetProduct(Guid guid, ProductWidgetProperties properties)
    {
        var productPage = await productRetrieverService.RetrieveWebPageByGuid(
                            guid,
                            ProductPage.CONTENT_TYPE_NAME,
                            webPageQueryResultMapper.Map<ProductPage>,
                            4);

        return productPage != null
            ? await productPageService.GetProductPageViewModel(
                productPage: productPage,
                getMedia: properties.ShowProductImage,
                getFeatures: properties.ShowProductFeatures,
                getBenefits: properties.ShowProductBenefits,
                callToAction: properties.CallToAction,
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