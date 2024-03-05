using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Kentico.PageBuilder.Web.Mvc;
using TrainingGuides.Web.Features.Products.Models;
using TrainingGuides.Web.Features.Products.Widgets.Product;
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

    private const string ALIGN_LEFT_CLASS = "align-left";
    private const string ALIGN_CENTER_CLASS = "align-center";
    private const string ALIGN_RIGHT_CLASS = "align-right";
    private readonly IContentItemRetrieverService<ProductPage> productRetrieverService;
    private readonly IWebPageQueryResultMapper webPageQueryResultMapper;
    private readonly IComponentStyleEnumService componentStyleEnumService;

    public ProductWidgetViewComponent(
        IContentItemRetrieverService<ProductPage> productRetrieverService,
        IWebPageQueryResultMapper webPageQueryResultMapper,
        IComponentStyleEnumService componentStyleEnumService)
    {
        this.productRetrieverService = productRetrieverService;
        this.webPageQueryResultMapper = webPageQueryResultMapper;
        this.componentStyleEnumService = componentStyleEnumService;
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

        var callToActionStyleClasses = componentStyleEnumService.GetColorSchemeClasses(
            componentStyleEnumService.GetColorScheme(properties.CallToActionStyle ?? string.Empty));

        return new ProductWidgetViewModel()
        {
            Product = product!,
            ShowProductFeatures = properties.ShowProductFeatures,
            ProductImage = properties.ShowProductImage ? product?.Media.FirstOrDefault() : null,
            CallToAction = properties.CallToAction,
            OpenInNewTab = properties.OpenInNewTab,
            ShowAdvanced = properties.ShowAdvanced,
            ColorScheme = properties.ColorScheme,
            CornerStyle = properties.CornerStyle,
            ImagePosition = properties.ImagePosition,
            ContentAlignmentClass = properties.TextAlignment switch
            {
                nameof(ContentAlignmentOption.Left) => ALIGN_LEFT_CLASS,
                nameof(ContentAlignmentOption.Center) => ALIGN_CENTER_CLASS,
                nameof(ContentAlignmentOption.Right) => ALIGN_RIGHT_CLASS,
                _ => ALIGN_LEFT_CLASS
            },
            CallToActionStyleClasses = string.Join(" ", callToActionStyleClasses)
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
            ? ProductPageViewModel.GetViewModel(
                productPage: productPage,
                getMedia: properties.ShowProductImage,
                getFeatures: properties.ShowProductFeatures,
                getBenefits: properties.ShowProductBenefits,
                getCallToAction: false,
                getPrice: properties.ShowProductFeatures)
            : null;
    }
}