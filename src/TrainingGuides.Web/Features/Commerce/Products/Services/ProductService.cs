using CMS.ContentEngine;
using CMS.DataEngine;
using Microsoft.AspNetCore.Html;
using TrainingGuides.ProductStock;
using TrainingGuides.Web.Commerce.Products.Models;
using TrainingGuides.Web.Features.Shared.Logging;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Commerce.Products.Services;

public class ProductService(IContentItemRetrieverService contentItemRetrieverService,
    ILogger<ProductService> logger,
    IInfoProvider<ProductAvailableStockInfo> productAvailableStockInfoProvider,
    IInfoProvider<TagInfo> tagInfoProvider) : IProductService
{
    private const int LowStockThreshold = 20;

    /// <inheritdoc/>
    public IProductSchema? GetFirstVariant(IProductParentSchema product) =>
        product.ProductParentSchemaVariants.FirstOrDefault() as IProductSchema;

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
                return await GetGenericParentProductViewModel(parentProduct, productVariant);
            }
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
            ProductVariantDescription = new HtmlString(string.Empty),
            ProductOtherDetails = new HtmlString(string.Empty),
            ProductStockStatus = GetFriendlyEnumString(await GetProductStockStatus(null))
        };

        return model;
    }

    private async Task<ProductViewModel> GetGenericParentProductViewModel(IProductParentSchema parentProduct, IProductSchema? productVariant)
    {
        var parentProductProduct = parentProduct as IProductSchema;

        var model = new ProductViewModel
        {
            ProductName = parentProductProduct?.ProductSchemaName ?? string.Empty,
            ProductSkuCode = (productVariant as IProductSkuSchema)?.ProductSkuSchemaSkuCode ?? string.Empty,
            ProductPrice = ((productVariant as IProductPriceSchema)?.ProductPriceSchemaPrice
                ?? parentProduct.ProductParentSchemaVariants
                    .Cast<IProductPriceSchema>()
                    .Select(v => v?.ProductPriceSchemaPrice)
                    .FirstOrDefault())
                ?? (parentProduct as IProductPriceSchema)?.ProductPriceSchemaPrice
                ?? 0m,
            ProductImages = GetImageViewModels(productVariant?.ProductSchemaImages)
                .UnionBy(GetImageViewModels(parentProductProduct?.ProductSchemaImages), img => img.ProductImageUrl),
            ProductSelectedVariantCodeName = (productVariant as IProductVariantSchema)?.ProductVariantSchemaCodeName ?? string.Empty,
            ProductVariants = parentProduct.ProductParentSchemaVariants
                .Cast<IProductSchema>()
                .Select(variant => new VariantViewModel
                {
                    VariantName = variant.ProductSchemaName,
                    VariantCodeName = (variant as IProductVariantSchema)?.ProductVariantSchemaCodeName ?? string.Empty,
                    VariantImage = GetImageViewModels(variant.ProductSchemaImages).FirstOrDefault()
                        ?? new ProductImageViewModel()
                }),
            ProductParentDescription = new HtmlString(parentProductProduct?.ProductSchemaDescription ?? string.Empty),
            ProductVariantDescription = new HtmlString(productVariant?.ProductSchemaDescription ?? string.Empty),
            ProductOtherDetails = new HtmlString(string.Empty),
            ProductStockStatus = GetFriendlyEnumString(await GetProductStockStatus(productVariant as IProductSkuSchema))
        };

        return model;
    }

    private async Task<ProductViewModel> GetCatFoodViewModel(CatFood catFoodProduct, CatFoodVariant? catFoodVariant)
    {
        var model = await GetGenericParentProductViewModel(catFoodProduct, catFoodVariant);

        model.ProductOtherDetails = new HtmlString
                (("Ingredients:<br/>" + string.Join("<br/>", catFoodVariant?.CatFoodVariantFormulation
                    .Select(formulation => formulation.PetFoodFormulationIngredients) ?? []))
                ?? string.Empty);

        return model;
    }

    private async Task<ProductViewModel> GetDogCollarViewModel(DogCollar dogCollarProduct, DogCollarVariant? dogCollarVariant)
    {
        var model = await GetGenericParentProductViewModel(dogCollarProduct, dogCollarVariant);

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

        if (stockValue <= 0)
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
            depth: 4,
            includeSecuredItems: true);

    /// <inheritdoc/>
    public async Task<ProductPage?> GetProductPageByGuid(Guid contentItemGuid)
    {
        if (contentItemGuid == Guid.Empty)
        {
            return null;
        }

        return await contentItemRetrieverService.RetrieveWebPageByContentItemGuid<ProductPage>(
            contentItemGuid,
            depth: 4,
            includeSecuredItems: true);
    }
}