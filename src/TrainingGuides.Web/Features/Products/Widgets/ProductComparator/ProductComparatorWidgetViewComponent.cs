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
    private readonly IHttpRequestService httpRequestService;

    public ProductComparatorWidgetViewComponent(
        IContentItemRetrieverService<ProductPage> productRetrieverService,
        IHttpRequestService httpRequestService)
    {
        this.productRetrieverService = productRetrieverService;
        this.httpRequestService = httpRequestService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ProductComparatorWidgetProperties properties)
    {
        var guids = properties.Products?.Select(i => i.Identifier).ToList() ?? [];

        var model = new ProductComparatorWidgetViewModel()
        {
            Products = [],
            GroupedFeaturesHtmlDictionary = [],
            ComparatorHeading = properties.ComparatorHeading,
            HeadingType = properties.HeadingType,
            HeadingMargin = properties.HeadingMargin,
            ShowShortDescription = properties.ShowShortDescription,
            CheckboxIconUrl = $"{httpRequestService.GetBaseUrl()}/assets/img/icons.svg#check"
        };

        foreach (var guid in guids)
        {
            var product = await GetProduct(guid, properties);

            if (product != null)
            {
                model.Products.Add(product);

                model.GroupedFeaturesHtmlDictionary.AddRange(product.Features.Where(i => i.ShowInComparator)
                    .Select(feature => new KeyValuePair<string, HtmlString>(feature.Key, feature.LabelHtml)));
            }
        }

        model.GroupedFeaturesHtmlDictionary = model.GroupedFeaturesHtmlDictionary.DistinctBy(item => item.Key).ToList();

        return View("~/Features/Products/Widgets/ProductComparator/ProductComparatorWidget.cshtml", model);
    }

    private async Task<ProductPageViewModel?> GetProduct(Guid guid, ProductComparatorWidgetProperties properties)
    {
        var productPage = await productRetrieverService.RetrieveWebPageByContentItemGuid(
                            guid,
                            ProductPage.CONTENT_TYPE_NAME,
                            4);

        if (productPage == null)
        {
            return new ProductPageViewModel
            {
                NameHtml = new("Error"),
                Features =
                [
                    new ProductFeatureViewModel
                    {
                        Key = "error",
                        Name = "Error",
                        LabelHtml = new("Error"),
                        Price = 0,
                        ValueHtml = new("Unable to load product.<br/>Please double-check your page selection."),
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
            LinkUrl = productPage.GetUrl().RelativePath,
            CallToAction = properties.CallToAction ?? string.Empty
        };

        var model = new ProductPageViewModel
        {
            NameHtml = new(product.ProductName),
            ShortDescriptionHtml = new(product.ProductShortDescription),
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
                    LabelHtml = new("Price"),
                    Price = model.Price,
                    ValueHtml = new(string.Empty),
                    FeatureIncluded = false,
                    ValueType = ProductFeatureValueType.Number,
                    ShowInComparator = true,
                });
        }

        return model;
    }
}
