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
    IInfoProvider<ProductAvailableStockInfo> productAvailableStockInfoProvider) : IProductService
{
    private const int LowStockThreshold = 20;
    private void LogVariantMultipleParentsError(IProductVariantSchema variant, IProductSchema parent)
    {
        var contentItem = parent as IContentItemFieldsSource;
        logger.LogError(EventIds.ProductVariantHasMultipleParents,
            "Variant with ID {VariantContentItemID} of type {VariantType} has multiple parent products of type {ParentType}.",
            contentItem?.SystemFields.ContentItemID,
            variant.GetType().Name,
            parent.GetType().Name);
    }

    /// <inheritdoc/>
    public IProductSchema? GetFirstVariant(IProductSchema product)
    {
        if (product is IProductVariantSchema)
        {
            return null;
        }

        if (product is CatFood catFoodProduct)
        {
            return catFoodProduct.CatFoodVariants
                .OrderBy(cfVariant => cfVariant.ProductPriceSchemaPrice)
                .FirstOrDefault();
        }

        return null;
    }

    /// <inheritdoc/>
    public IProductSchema? GetVariantByCodeName(IProductSchema product, string variantCodeName)
    {
        if (product is CatFood catFoodProduct)
        {
            return catFoodProduct
                .CatFoodVariants
                .FirstOrDefault(variant => variant.VariantSchemaCodeName == variantCodeName);
        }
        return null;
    }

    /// <inheritdoc/>
    public async Task<IProductSchema?> GetVariantParent(IProductSchema variant)
    {
        if (variant is not IProductVariantSchema)
        {
            return null;
        }
        if (variant is CatFoodVariant catFoodVariant)
        {
            var parents = await contentItemRetrieverService.RetrieveParentItems<CatFood>(
                nameof(CatFood.CatFoodVariants),
                [catFoodVariant.SystemFields.ContentItemID],
                true,
                depth: 4);

            if (parents.Count() > 1)
            {
                LogVariantMultipleParentsError(catFoodVariant, parents.First());
            }

            return parents.FirstOrDefault();
        }
        return null;

    }

    /// <inheritdoc/>
    public async Task<ProductViewModel> GetViewModel(IProductSchema? product, IProductSchema? selectedVariant = null)
    {
        if (product is CatFood catFoodProduct)
        {
            var catFoodVariant = selectedVariant is CatFoodVariant cfVariant
                && catFoodProduct.CatFoodVariants
                    .Select(variant => variant.SystemFields.ContentItemID)
                    .Contains(cfVariant.SystemFields.ContentItemID)
                ? cfVariant
                : GetFirstVariant(catFoodProduct) as CatFoodVariant;

            return await GetCatFoodViewModel(catFoodProduct, catFoodVariant);
        }
        return new ProductViewModel();
    }

    private async Task<ProductViewModel> GetCatFoodViewModel(CatFood catFoodProduct, CatFoodVariant? catFoodVariant)
    {
        var model = new ProductViewModel
        {
            ProductName = catFoodProduct.ProductSchemaName,
            ProductDescription = catFoodProduct.ProductSchemaDescription,
            ProductSkuCode = catFoodVariant?.ProductSkuSchemaSkuCode ?? string.Empty,
            ProductPrice = catFoodVariant?.ProductPriceSchemaPrice
                ?? catFoodProduct.CatFoodVariants.Select(v => v.ProductPriceSchemaPrice).FirstOrDefault(),
            ProductImageUrls = GetImageViewModels(catFoodVariant?.ProductSchemaImages)
                .Union(GetImageViewModels(catFoodProduct.ProductSchemaImages)),
            ProductSelectedVariantCodeName = catFoodVariant?.VariantSchemaCodeName ?? string.Empty,
            ProductVariants = catFoodProduct.CatFoodVariants
                .Select(variant => new VariantViewModel
                {
                    VariantName = variant.ProductSchemaName,
                    VariantCodeName = variant.VariantSchemaCodeName,
                    VariantImage = GetImageViewModels(variant.ProductSchemaImages).FirstOrDefault()
                        ?? new ProductImageViewModel()
                }),
            ProductParentDescription = new HtmlString(catFoodProduct.ProductSchemaDescription ?? string.Empty),
            ProductVariantDescription = new HtmlString(catFoodVariant?.ProductSchemaDescription ?? string.Empty),
            ProductOtherDetails = new HtmlString
                (string.Join("<br/>", catFoodVariant?.CatFoodVariantFormulation
                    .Select(formulation => formulation.PetFoodFormulationIngredients) ?? [])
                ?? string.Empty),
            ProductStockStatus = await GetProductStockStatus(catFoodVariant)
        };

        return model;
    }

    private async Task<ProductStockEnum> GetProductStockStatus(IProductSkuSchema? skuProduct)
    {
        if (skuProduct == null)
        {
            return ProductStockEnum.OutOfStock;
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
            return ProductStockEnum.OutOfStock;
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
        if (product is CatFood catFoodProduct)
        {
            return catFoodProduct.CatFoodVariants.Any();
        }

        return false;
    }

    /// <inheritdoc/>
    public bool ProductIsVariant(IProductSchema product) => product is IProductVariantSchema;
}