using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.Products.Models;
using TrainingGuides.Web.Features.Products.Widgets.ProductComparator;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(
        identifier: ProductComparatorWidgetViewComponent.IDENTIFIER,
        viewComponentType: typeof(ProductComparatorWidgetViewComponent),
        name: "Product comparator",
        propertiesType: typeof(ProductComparatorWidgetProperties),
        Description = "Displays a comparison between products.",
        IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Products.Widgets.ProductComparator;

public class ProductComparatorWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ProductComparatorWidget";

    private readonly IContentItemRetrieverService<ProductPage> productRetrieverService;
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    private readonly IHttpRequestService httpRequestService;

    public ProductComparatorWidgetViewComponent(IContentItemRetrieverService<ProductPage> productRetrieverService,
        IWebPageUrlRetriever webPageUrlRetriever,
        IHttpRequestService httpRequestService)
    {
        this.productRetrieverService = productRetrieverService;
        this.webPageUrlRetriever = webPageUrlRetriever;
        this.httpRequestService = httpRequestService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ProductComparatorWidgetProperties properties,
        CancellationToken cancellationToken)
    {
        var guids = properties.Products?.Select(i => i.WebPageGuid).ToList() ?? [];

        var model = new ProductComparatorWidgetViewModel()
        {
            Products = [],
            GroupedFeatures = [],
            ComparatorHeading = properties.ComparatorHeading,
            HeadingType = properties.HeadingType,
            HeadingMargin = properties.HeadingMargin,
            ShowShortDescription = properties.ShowShortDescription,
            CheckboxIconUrl = $"{httpRequestService.GetBaseUrl()}/assets/img/icons.svg#check"
        };

        foreach (var guid in guids)
        {
            var product = await GetProduct(guid, properties, cancellationToken);

            if (product != null)
            {
                model.Products.Add(product);

                model.GroupedFeatures.AddRange(product.Features.Where(i => i.ShowInComparator)
                    .Select(feature => new KeyValuePair<string, HtmlString>(feature.Key, feature.Label)));
            }
        }

        model.GroupedFeatures = model.GroupedFeatures.DistinctBy(item => item.Key).ToList();

        return View("~/Features/Products/Widgets/ProductComparator/ProductComparatorWidget.cshtml", model);
    }

    private async Task<ProductPageViewModel?> GetProduct(Guid guid, ProductComparatorWidgetProperties properties, CancellationToken cancellationToken)
    {
        var productPage = await productRetrieverService.RetrieveWebPageByGuid(
                            guid,
                            ProductPage.CONTENT_TYPE_NAME,
                            4);

        if (productPage == null)
        {
            return new ProductPageViewModel
            {
                Name = new("Error"),
                Features =
                [
                    new ProductFeatureViewModel
                    {
                        Key = "error",
                        Name = "Error",
                        Label = new("Error"),
                        Price = 0,
                        Value = new("Unable to load product.<br/>Please double-check your page selection."),
                        FeatureIncluded = false,
                        ValueType = ProductFeatureValueType.Text,
                        ShowInComparator = true,
                    }
                ]
            };
        }

        var product = productPage.ProductPageProduct.FirstOrDefault();

        if (product == null)
            return null;

        var linkComponent = new LinkViewModel()
        {
            LinkUrl = webPageUrlRetriever.Retrieve(productPage, cancellationToken).Result.RelativePath,
            CallToAction = properties.CallToAction ?? string.Empty
        };

        var model = new ProductPageViewModel
        {
            Name = new(product.ProductName),
            ShortDescription = new(product.ProductShortDescription),
            Features = product.ProductFeatures.Select(ProductFeatureViewModel.GetViewModel).ToList(),
            Link = linkComponent,
            Price = product.ProductPrice
        };

        if (properties.ShowPrice)
        {
            model.Features.Add(
                new ProductFeatureViewModel
                {
                    Key = "price-from-product-content-item",
                    Name = "Price",
                    Label = new("Price"),
                    Price = model.Price,
                    Value = new(string.Empty),
                    FeatureIncluded = false,
                    ValueType = ProductFeatureValueType.Number,
                    ShowInComparator = true,
                });
        }

        return model;
    }
}
