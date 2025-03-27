﻿using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides;
using TrainingGuides.Web.Features.Products.Models;
using TrainingGuides.Web.Features.Products.Services;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWebPageRoute(
    contentTypeName: ProductPage.CONTENT_TYPE_NAME,
    controllerType: typeof(TrainingGuides.Web.Features.Products.ProductPageController))]

namespace TrainingGuides.Web.Features.Products;

public class ProductPageController : Controller
{
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IContentItemRetrieverService<ProductPage> contentItemRetriever;
    private readonly IProductPageService productPageService;

    public ProductPageController(
        IWebPageDataContextRetriever webPageDataContextRetriever,
        IContentItemRetrieverService<ProductPage> contentItemRetriever,
        IProductPageService productPageService)
    {
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.contentItemRetriever = contentItemRetriever;
        this.productPageService = productPageService;
    }
    public async Task<IActionResult> Index()
    {
        var context = webPageDataContextRetriever.Retrieve();

        var productPage = await contentItemRetriever.RetrieveWebPageById
            (context.WebPage.WebPageItemID,
            ProductPage.CONTENT_TYPE_NAME,
            3);

        var model = await productPageService.GetProductPageViewModel(productPage);
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

        return new TemplateResult(model);
    }
}
