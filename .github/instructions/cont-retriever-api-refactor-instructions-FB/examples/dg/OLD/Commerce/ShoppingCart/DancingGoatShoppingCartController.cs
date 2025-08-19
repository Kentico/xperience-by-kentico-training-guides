﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.Commerce;
using CMS.ContentEngine;

using DancingGoat;
using DancingGoat.Commerce;
using DancingGoat.Helpers;
using DancingGoat.Models;
using DancingGoat.Services;

using Kentico.Commerce.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;

using Microsoft.AspNetCore.Mvc;

#pragma warning disable KXE0002 // Commerce feature is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[assembly: RegisterWebPageRoute(ShoppingCart.CONTENT_TYPE_NAME, typeof(DancingGoatShoppingCartController), WebsiteChannelNames = new[] { DancingGoatConstants.WEBSITE_CHANNEL_NAME })]

namespace DancingGoat.Commerce;

/// <summary>
/// Controller for managing the shopping cart.
/// </summary>
public sealed class DancingGoatShoppingCartController : Controller
{
    private readonly ICurrentShoppingCartService currentShoppingCartService;
    private readonly IPreferredLanguageRetriever currentLanguageRetriever;
    private readonly IProductVariantsExtractor productVariantsExtractor;
    private readonly ProductRepository productRepository;
    private readonly ProductPageRepository productPageRepository;
    private readonly IWebPageUrlProvider webPageUrlProvider;


    public DancingGoatShoppingCartController(ICurrentShoppingCartService currentShoppingCartService,
        IPreferredLanguageRetriever currentLanguageRetriever, IProductVariantsExtractor productVariantsExtractor,
        ProductRepository productRepository, ProductPageRepository productPageRepository, IWebPageUrlProvider webPageUrlProvider)
    {
        this.currentShoppingCartService = currentShoppingCartService;
        this.currentLanguageRetriever = currentLanguageRetriever;
        this.productVariantsExtractor = productVariantsExtractor;
        this.productRepository = productRepository;
        this.productPageRepository = productPageRepository;
        this.webPageUrlProvider = webPageUrlProvider;
    }


    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        // Web page identification data
        var languageName = currentLanguageRetriever.Get();

        var shoppingCart = await currentShoppingCartService.Get(cancellationToken);
        if (shoppingCart == null)
        {
            return View(new ShoppingCartViewModel(new List<ShoppingCartItemViewModel>(), 0));
        }

        var shoppingCartData = shoppingCart.GetShoppingCartDataModel();

        var products = await productRepository.GetProducts(shoppingCartData.Items.Select(item => item.ContentItemId).ToList(), languageName, cancellationToken);
        var productPageUrls = await productPageRepository.GetProductPageUrls(products.Cast<IContentItemFieldsSource>(), languageName, cancellationToken);

        var totalPrice = CalculationService.CalculateTotalPrice(shoppingCartData, products);

        return View(new ShoppingCartViewModel(
            shoppingCartData.Items.Select(item =>
            {
                var product = products.FirstOrDefault(product => (product as IContentItemFieldsSource)?.SystemFields.ContentItemID == item.ContentItemId);
                var variantValues = product == null ? null : productVariantsExtractor.ExtractVariantsValue(product);
                productPageUrls.TryGetValue(item.ContentItemId, out var pageUrl);

                return product == null
                    ? null
                    : new ShoppingCartItemViewModel(
                        item.ContentItemId,
                        FormatProductName(product.ProductFieldName, variantValues, item.VariantId),
                        product.ProductFieldImage.FirstOrDefault()?.ImageFile.Url,
                        pageUrl,
                        item.Quantity,
                        product.ProductFieldPrice,
                        item.Quantity * product.ProductFieldPrice,
                        item.VariantId);
            })
            .Where(x => x != null)
            .ToList(),
            totalPrice));
    }


    [HttpPost]
    [Route("/ShoppingCart/HandleAddRemove")]
    public async Task<IActionResult> HandleAddRemove(int contentItemId, int quantity, int? variantId, string action, string languageName)
    {
        if (string.Equals(action, "Remove", StringComparison.OrdinalIgnoreCase))
        {
            quantity *= -1;
        }
        else if (action == "RemoveAll")
        {
            quantity = 0;
        }

        var shoppingCart = await GetCurrentShoppingCart();

        UpdateQuantity(shoppingCart, contentItemId, quantity, variantId, setAbsoluteValue: new[] { "RemoveAll", "Update" }.Contains(action));

        shoppingCart.Update();

        return Redirect(await webPageUrlProvider.ShoppingCartPageUrl(languageName));
    }


    [HttpPost]
    [Route("/ShoppingCart/Add")]
    public async Task<IActionResult> Add(int contentItemId, int quantity, int? variantId, string languageName)
    {
        var shoppingCart = await GetCurrentShoppingCart();

        UpdateQuantity(shoppingCart, contentItemId, quantity, variantId);

        shoppingCart.Update();

        return Redirect(await webPageUrlProvider.ShoppingCartPageUrl(languageName));
    }


    private static string FormatProductName(string productName, IDictionary<int, string> variants, int? variantId)
    {
        return variants != null && variantId != null && variants.TryGetValue(variantId.Value, out string variantValue)
            ? $"{productName} - {variantValue}"
            : productName;
    }


    /// <summary>
    /// Updates the quantity of the product in the shopping cart.
    /// </summary>
    private static void UpdateQuantity(ShoppingCartInfo shoppingCart, int contentItemId, int quantity, int? variantId, bool setAbsoluteValue = false)
    {
        var shoppingCartData = shoppingCart.GetShoppingCartDataModel();

        var productItem = shoppingCartData.Items.FirstOrDefault(x => x.ContentItemId == contentItemId && x.VariantId == variantId);
        if (productItem != null)
        {
            productItem.Quantity = setAbsoluteValue ? quantity : Math.Max(0, productItem.Quantity + quantity);
            if (productItem.Quantity == 0)
            {
                shoppingCartData.Items.Remove(productItem);
            }
        }
        else if (quantity > 0)
        {
            shoppingCartData.Items.Add(new ShoppingCartDataItem { ContentItemId = contentItemId, Quantity = quantity, VariantId = variantId });
        }

        shoppingCart.StoreShoppingCartDataModel(shoppingCartData);
    }


    /// <summary>
    /// Gets the current shopping cart or creates a new one if it does not exist.
    /// </summary>
    private async Task<ShoppingCartInfo> GetCurrentShoppingCart()
    {
        var shoppingCart = await currentShoppingCartService.Get();

        shoppingCart ??= await currentShoppingCartService.Create(null);

        return shoppingCart;
    }
}
#pragma warning restore KXE0002 // Commerce feature is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
