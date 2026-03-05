// MEMBERSHIP IN COMMERCE:
// This branch excludes membership implementation, which is essential for full commerce functionality.
// Search for "IMembershipService" and "includeSecuredItems" to see where it plugs in.
// See the "finished" branch for complete implementation.

using CMS.Commerce;
using CMS.ContentEngine;
using CMS.DataEngine;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Html;
using TrainingGuides.ProductStock;
using TrainingGuides.Web.Commerce.Products.Models;
using TrainingGuides.Web.Features.Commerce.PriceCalculation.Models;
using TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductListing;
// using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Shared.Logging;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Commerce.Products.Services;

public class ProductService(IContentItemRetrieverService contentItemRetrieverService,
    ILogger<ProductService> logger,
    IInfoProvider<ProductAvailableStockInfo> productAvailableStockInfoProvider,
    IInfoProvider<TagInfo> tagInfoProvider,
    IPreferredLanguageRetriever preferredLanguageRetriever,
    // IMembershipService membershipService,
    IPriceCalculationService<PriceCalculationRequest, TrainingGuidesPriceCalculationResult> priceCalculationService) : IProductService
{
    private const decimal LowStockThreshold = 20m;

    /// <inheritdoc/>
    public IProductSchema? GetFirstVariant(IProductParentSchema product) =>
        product.ProductParentSchemaVariants
            .OrderBy(variant => variant is IProductPriceSchema pricedVariant ? pricedVariant.ProductPriceSchemaPrice : 0m)
            .FirstOrDefault() as IProductSchema;

    /// <inheritdoc/>
    public IProductSchema? GetVariantByCodeName(IProductParentSchema product, string variantCodeName)
    {
        var variant = product.ProductParentSchemaVariants
            .FirstOrDefault(variant => variant.ProductVariantSchemaCodeName == variantCodeName);

        return variant as IProductSchema;
    }

    /// <inheritdoc/>
    public async Task<IProductSchema?> GetVariantParent(IProductSchema variant)
    {
        if (variant is not IProductVariantSchema || variant is not IContentItemFieldsSource variantItem)
        {
            return null;
        }

        var parents = await contentItemRetrieverService.RetrieveParentItemsOfSchema<IProductSchema>(
                IProductSchema.REUSABLE_FIELD_SCHEMA_NAME,
                nameof(IProductParentSchema.ProductParentSchemaVariants),
                [variantItem.SystemFields.ContentItemID],
                true,
                depth: 4);

        if (parents.Count() > 1)
        {
            LogVariantMultipleParentsError(variant, parents.First());
        }

        return parents.FirstOrDefault();
    }

    private void LogVariantMultipleParentsError(IProductSchema variant, IProductSchema parent)
    {
        var contentItem = parent as IContentItemFieldsSource;
        logger.LogError(EventIds.ProductVariantHasMultipleParents,
            "Variant with ID {VariantContentItemID} of type {VariantType} has multiple parent products of type {ParentType}.",
            contentItem?.SystemFields.ContentItemID,
            variant.GetType().Name,
            parent.GetType().Name);
    }

    /// <inheritdoc/>
    public async Task<ProductViewModel> GetViewModel(IProductSchema? product, IProductSchema? selectedVariant = null, bool accessDenied = false)
    {
        if (accessDenied)
        {
            return await GetAccessDeniedViewModel(product);
        }

        if (product is IProductParentSchema parentProduct)
        {

            var productVariant = selectedVariant is IContentItemFieldsSource ciVariant
                && selectedVariant is IProductSchema pVariant
                && parentProduct.ProductParentSchemaVariants
                    .Cast<IContentItemFieldsSource>()
                    .Select(variant => variant.SystemFields.ContentItemID)
                    .Contains(ciVariant.SystemFields.ContentItemID)
                ? pVariant
                : GetFirstVariant(parentProduct);

            if (product is CatFood catFoodProduct)
            {
                var catFoodVariant = productVariant as CatFoodVariant;

                return await GetCatFoodViewModel(catFoodProduct, catFoodVariant);
            }
            else if (product is DogCollar dogCollarProduct)
            {
                var dogCollarVariant = productVariant as DogCollarVariant;

                return await GetDogCollarViewModel(dogCollarProduct, dogCollarVariant);
            }
            else
            {
                return await GetGenericProductViewModel(product, productVariant);
            }
        }
        else if (product is not null)
        {
            return await GetGenericProductViewModel(product, null);
        }
        else
        {
            return new ProductViewModel();
        }
    }

    private async Task<ProductViewModel> GetAccessDeniedViewModel(IProductSchema? product)
    {
        var model = new ProductViewModel
        {
            ProductName = product is not null
                ? $"{product.ProductSchemaName} (requires authentication)"
                : "Product requires authentication",
            ProductSkuCode = string.Empty,
            ProductPrice = 0m,
            ProductImages = [],
            ProductSelectedVariantCodeName = string.Empty,
            ProductVariants = [],
            ProductParentDescription = new HtmlString("Please sign in to view this product."),
            ProductStockStatus = GetFriendlyEnumString(await GetProductStockStatus(null))
        };

        return model;
    }

    private async Task<ProductViewModel> GetGenericProductViewModel(IProductSchema product, IProductSchema? productVariant)
    {
        var parentProduct = product as IProductParentSchema;

        var variantStockStatus = await GetProductStockStatus(productVariant as IProductSkuSchema);
        var stockStatus = variantStockStatus != ProductStockEnum.Unknown
            ? variantStockStatus
            : await GetProductStockStatus(product as IProductSkuSchema);

        var model = new ProductViewModel
        {
            ProductName = product.ProductSchemaName ?? string.Empty,
            ProductSkuCode = (productVariant as IProductSkuSchema)?.ProductSkuSchemaSkuCode
                ?? (product as IProductSkuSchema)?.ProductSkuSchemaSkuCode
                ?? string.Empty,
            ProductRegularPrice = ((productVariant as IProductPriceSchema)?.ProductPriceSchemaPrice
                ?? parentProduct?.ProductParentSchemaVariants
                    .Cast<IProductPriceSchema>()
                    .Select(v => v?.ProductPriceSchemaPrice)
                    .FirstOrDefault())
                ?? (product as IProductPriceSchema)?.ProductPriceSchemaPrice
                ?? 0m,
            ProductPrice = await GetCatalogPrice(productVariant ?? product),
            ProductImages = GetImageViewModels(productVariant?.ProductSchemaImages)
                .UnionBy(GetImageViewModels(product.ProductSchemaImages), img => img.ProductImageUrl),
            ProductSelectedVariantCodeName = (productVariant as IProductVariantSchema)?.ProductVariantSchemaCodeName ?? string.Empty,
            ProductVariants = parentProduct?.ProductParentSchemaVariants
                .Cast<IProductSchema>()
                .Select(variant => new VariantViewModel
                {
                    VariantName = variant.ProductSchemaName,
                    VariantCodeName = (variant as IProductVariantSchema)?.ProductVariantSchemaCodeName ?? string.Empty,
                    VariantImage = GetImageViewModels(variant.ProductSchemaImages).FirstOrDefault()
                        ?? new ProductImageViewModel()
                }) ?? [],
            ProductParentDescription = new HtmlString(product.ProductSchemaDescription ?? string.Empty),
            ProductVariantDescription = new HtmlString(productVariant?.ProductSchemaDescription ?? string.Empty),
            ProductStockStatus = GetFriendlyEnumString(stockStatus)
        };

        return model;
    }


    private async Task<ProductViewModel> GetProductViewModelForListing(IProductSchema product, IProductSchema? productVariant, bool accessDenied)
    {
        if (accessDenied)
        {
            return await GetAccessDeniedViewModel(product);
        }

        var parentProduct = product as IProductParentSchema;

        var stockStatus = await GetListingStockForProduct(product);

        var model = new ProductViewModel
        {
            ProductName = product.ProductSchemaName ?? string.Empty,
            ProductSkuCode = string.Empty,
            ProductRegularPrice = ((productVariant as IProductPriceSchema)?.ProductPriceSchemaPrice
                ?? parentProduct?.ProductParentSchemaVariants
                    .Cast<IProductPriceSchema>()
                    .Select(v => v?.ProductPriceSchemaPrice)
                    .FirstOrDefault())
                ?? (product as IProductPriceSchema)?.ProductPriceSchemaPrice
                ?? 0m,
            ProductPrice = await GetCatalogPrice(productVariant ?? product),
            ProductImages = GetImageViewModels([product.ProductSchemaImages.FirstOrDefault() ?? new()]),
            ProductSelectedVariantCodeName = string.Empty,
            ProductVariants = [],
            ProductParentDescription = new HtmlString(string.Empty),
            ProductVariantDescription = new HtmlString(string.Empty),
            ProductOtherDetails = new HtmlString(string.Empty),
            ProductStockStatus = GetFriendlyEnumString(stockStatus)
        };

        return model;
    }

    private async Task<ProductViewModel> GetCatFoodViewModel(CatFood catFoodProduct, CatFoodVariant? catFoodVariant)
    {
        var model = await GetGenericProductViewModel(catFoodProduct, catFoodVariant);

        model.ProductOtherDetails = new HtmlString
                (("Ingredients:<br/>" + string.Join("<br/>", catFoodVariant?.CatFoodVariantFormulation
                    .Select(formulation => formulation.PetFoodFormulationIngredients) ?? []))
                ?? string.Empty);

        return model;
    }

    private async Task<ProductViewModel> GetDogCollarViewModel(DogCollar dogCollarProduct, DogCollarVariant? dogCollarVariant)
    {
        var model = await GetGenericProductViewModel(dogCollarProduct, dogCollarVariant);

        var tagIdentifiers = dogCollarProduct
            .MaterialSchemaMaterial
            .Union((dogCollarVariant?.ColorPattern ?? [])
            .Union(dogCollarVariant?.SizeSchemaSize ?? []))
            .Select(tag => tag.Identifier)
            ?? [];

        var tags = await tagInfoProvider.Get()
            .WhereIn(nameof(TagInfo.TagGUID), tagIdentifiers)
            .Columns(nameof(TagInfo.TagGUID), nameof(TagInfo.TagTitle))
            .GetEnumerableTypedResultAsync();

        string materials = GetAggregatedTags(tags, dogCollarProduct.MaterialSchemaMaterial);
        string colors = GetAggregatedTags(tags, dogCollarVariant?.ColorPattern ?? []);
        string sizes = GetAggregatedTags(tags, dogCollarVariant?.SizeSchemaSize ?? []);

        model.ProductOtherDetails = new HtmlString
                ($"<strong>Material:</strong> {materials} <br/>" +
                 $"<strong>Color:</strong> {colors}<br/>" +
                 $"<strong>Size (circumference):</strong> {sizes}");

        return model;
    }

    private string GetAggregatedTags(IEnumerable<TagInfo> tags, IEnumerable<TagReference> tagIdentifiers) => string.Join(", ",
            tags.Where(tag => tagIdentifiers
                    .Select(tag => tag.Identifier)
                    .Contains(tag.TagGUID))
                .Select(tag => tag.TagTitle));

    private async Task<ProductStockEnum> GetProductStockStatus(IProductSkuSchema? skuProduct)
    {
        if (skuProduct is null)
        {
            return ProductStockEnum.Unknown;
        }

        var contentItem = skuProduct as IContentItemFieldsSource;

        var stockInfos = await productAvailableStockInfoProvider.Get()
            .WhereEquals(nameof(ProductAvailableStockInfo.ProductAvailableStockContentItemID),
                contentItem?.SystemFields.ContentItemID)
            .GetEnumerableTypedResultAsync();

        if (stockInfos.Count() > 1)
        {
            logger.LogWarning(EventIds.ProductStockMultipleRecords,
                "Product SKU with ID {ProductSkuContentItemID} has multiple stock records.",
                contentItem?.SystemFields.ContentItemID);
        }
        else if (stockInfos.Count() == 0)
        {
            return ProductStockEnum.Unknown;
        }

        decimal stockValue = stockInfos.First().ProductAvailableStockValue;

        return GetEnumForStockNumber((int)stockValue);

    }

    private ProductStockEnum GetEnumForStockNumber(decimal stockValue)
    {
        if (stockValue <= 0m)
        {
            return ProductStockEnum.OutOfStock;
        }
        if (stockValue <= LowStockThreshold)
        {
            return ProductStockEnum.LowStock;
        }

        return ProductStockEnum.InStock;
    }

    private string GetFriendlyEnumString(ProductStockEnum stockEnum) =>
        stockEnum switch
        {
            ProductStockEnum.OutOfStock => "Out of Stock - ✕",
            ProductStockEnum.InStock => "In Stock - ✓",
            ProductStockEnum.LowStock => "Low Stock - ⚠",
            ProductStockEnum.PreOrder => "Pre-Order - ⏲",
            ProductStockEnum.Unknown or _ => "Unknown - ?"
        };

    private List<ProductImageViewModel> GetImageViewModels(IEnumerable<ProductImage>? images)
    {
        if (images is null)
        {
            return [];
        }

        return images.Select(image => new ProductImageViewModel
        {
            ProductImageUrl = image.ProductImageAsset.Url,
            ProductImageAltText = image.ProductImageAltText
        }).ToList();
    }

    /// <inheritdoc/>
    public bool ProductHasVariants(IProductSchema product)
    {
        if (product is IProductParentSchema parentProduct)
        {
            return parentProduct.ProductParentSchemaVariants.Any();
        }

        return false;
    }

    /// <inheritdoc/>
    public bool ProductIsVariant(IProductSchema product) => product is IProductVariantSchema;

    /// <inheritdoc/>
    public async Task<ProductPage?> GetCurrentProductPage() =>
        await contentItemRetrieverService.RetrieveCurrentPage<ProductPage>(
            depth: 4);
    //      includeSecuredItems: true);

    /// <inheritdoc/>
    public async Task<ProductPage?> GetProductPageByGuid(Guid contentItemGuid)
    {
        if (contentItemGuid == Guid.Empty)
        {
            return null;
        }

        return await contentItemRetrieverService.RetrieveWebPageByContentItemGuid<ProductPage>(
            contentItemGuid,
            depth: 4);
        //  includeSecuredItems: true);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ProductPage>> RetrieveProductPages(
        string parentPagePath,
        string securedItemsDisplayMode)
    {
        if (string.IsNullOrEmpty(parentPagePath))
        {
            return [];
        }

        bool includeSecuredItems = securedItemsDisplayMode.Equals(SecuredOption.IncludeEverything.ToString())
                || securedItemsDisplayMode.Equals(SecuredOption.PromptForLogin.ToString());

        return await contentItemRetrieverService.RetrieveWebPageChildrenByPath<ProductPage>(
            path: parentPagePath,
            depth: 3);
        //  includeSecuredItems: includeSecuredItems);
    }

    /// <inheritdoc/>
    public async Task<List<ProductListingItemViewModel>> GetProductListingItemViewModels(
        IEnumerable<ProductPage> productPages,
        string securedItemsDisplayMode)
    {
        var models = new List<ProductListingItemViewModel>();

        // This is a placeholder, as the main starter branch does not implement membership yet.
        // Follow along with the training guides to implement membership or see the finished branch to see this in action.
        bool isAuthenticated = false; //await membershipService.IsMemberAuthenticated();

        foreach (var productPage in productPages)
        {
            if (productPage?.ProductPageProducts == null || !productPage.ProductPageProducts.Any())
            {
                continue;
            }

            var product = productPage.ProductPageProducts.First();

            bool accessDenied = securedItemsDisplayMode.Equals(SecuredOption.PromptForLogin.ToString())
                && !isAuthenticated
                && (IsProductSecured(product) || productPage.SystemFields.ContentItemIsSecured);

            var variant = ProductHasVariants(product)
                ? GetFirstVariant((product as IProductParentSchema)!)
                : null;

            var productViewModel = await GetProductViewModelForListing(product, variant, accessDenied: accessDenied);
            string productPageUrl = productPage.GetUrl()?.RelativePath ?? string.Empty;

            models.Add(new ProductListingItemViewModel
            {
                Product = productViewModel,
                ProductPageUrl = productPageUrl,
                AccessDenied = accessDenied
            });
        }

        return models;
    }

    /// <summary>
    /// Gets the listing stock status for a product, considering the stocks of all variants if applicable.
    /// </summary>
    /// <param name="product">Product to check stock for</param>
    /// <returns>The stock status of the product, or the highest stock status from among its variants</returns>
    private async Task<ProductStockEnum> GetListingStockForProduct(IProductSchema product)
    {
        if (product is IProductSkuSchema skuProduct)
        {
            var productStatus = await GetProductStockStatus(skuProduct);

            if (productStatus != ProductStockEnum.Unknown)
            {
                return productStatus;
            }
        }

        if (ProductHasVariants(product))
        {
            var parentProduct = product as IProductParentSchema;

            var variantSkus = parentProduct?.ProductParentSchemaVariants
                .Cast<IContentItemFieldsSource>()
                .Select(variant => variant.SystemFields.ContentItemID) ?? [];

            var variantStockInfos = await productAvailableStockInfoProvider.Get()
                .WhereIn(nameof(ProductAvailableStockInfo.ProductAvailableStockContentItemID), variantSkus)
                .Columns(nameof(ProductAvailableStockInfo.ProductAvailableStockValue))
                .GetEnumerableTypedResultAsync();

            var variantStockStatuses = variantStockInfos
                .Select(stockInfo => GetEnumForStockNumber(stockInfo.ProductAvailableStockValue))
                .Distinct();

            return variantStockStatuses.Any()
                ? variantStockStatuses.Max()
                : ProductStockEnum.Unknown;
        }

        return ProductStockEnum.Unknown;
    }

    /// <summary>
    /// Determines whether the specified product is secured.
    /// </summary>
    /// <param name="product">The product to check.</param>
    /// <returns><c>true</c> if the product is secured; otherwise, <c>false</c>.</returns>
    private bool IsProductSecured(IProductSchema product)
    {
        if (product is IContentItemFieldsSource contentItem)
        {
            return contentItem.SystemFields.ContentItemIsSecured;
        }
        return false;
    }

    private async Task<decimal> GetCatalogPrice(IProductSchema product)
    {
        if (product is IProductPriceSchema pricedProduct)
        {
            return await GetCatalogPrice((product as IContentItemFieldsSource)?.SystemFields.ContentItemID ?? 0)
                ?? pricedProduct.ProductPriceSchemaPrice;
        }
        else if (product is IProductParentSchema parentProduct)
        {
            var firstVariant = GetFirstVariant(parentProduct);
            if (firstVariant is IProductPriceSchema pricedVariant)
            {
                return await GetCatalogPrice((firstVariant as IContentItemFieldsSource)?.SystemFields.ContentItemID ?? 0)
                    ?? pricedVariant.ProductPriceSchemaPrice;
            }
        }
        else if (product is IProductVariantSchema)
        {
            var parent = await GetVariantParent(product);
            if (parent is IProductPriceSchema pricedParent)
            {
                return await GetCatalogPrice((parent as IContentItemFieldsSource)?.SystemFields.ContentItemID ?? 0)
                    ?? pricedParent.ProductPriceSchemaPrice;
            }
        }
        return 0m;
    }

    public async Task<decimal?> GetCatalogPrice(int productId,
        CancellationToken cancellationToken = default)
    {
        // Creates a minimal calculation request for catalog display
        var calculationRequest = new PriceCalculationRequest
        {
            Items = [new PriceCalculationRequestItem
            {
                ProductIdentifier = new TrainingGuidesPriceIdentifier { Identifier = productId },
                Quantity = 1,
            }],
            LanguageName = preferredLanguageRetriever.Get(),
            // Use Catalog mode for product listing/detail pages
            Mode = PriceCalculationMode.Catalog
        };

        // Calculates the prices based on the request
        var calculationResult =
            await priceCalculationService.Calculate(calculationRequest, cancellationToken);

        // Access pricing for the product
        var item = calculationResult?.Items.FirstOrDefault();

        // Price after catalog promotion (if any was applied)
        return item?.LineSubtotalAfterLineDiscount;
    }
}