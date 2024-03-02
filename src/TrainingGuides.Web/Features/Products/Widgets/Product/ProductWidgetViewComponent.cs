using TrainingGuides.Web.Features.Products.Widgets.Product;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.Products.Models;
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
        var guid = properties.SelectedProductPage?.Select(webPage => webPage.WebPageGuid).FirstOrDefault();
        var product = guid.HasValue
            ? await GetProduct(guid.Value, properties)
            : null;

        var callToActionStyleClasses = componentStyleEnumService.GetColorSchemeClasses(
            componentStyleEnumService.GetColorScheme(properties?.CallToActionStyle ?? string.Empty));

        var model = new ProductWidgetViewModel()
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
            TextAlignment = properties.TextAlignment,
            CallToActionStyleClasses = string.Join(" ", callToActionStyleClasses)
        };

        return View("~/Features/Products/Widgets/Product/ProductWidget.cshtml", model);
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
                getCallToAction: false,
                getPrice: false)
            : null;
    }
}