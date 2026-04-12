using CMS.Commerce;
using CMS.ContentEngine;
using CMS.DataEngine;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Html;
using TrainingGuides.ProductStock;
using TrainingGuides.Web.Commerce.Products.Models;
using TrainingGuides.Web.Features.Commerce.PriceCalculation.Models;
using TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductListing;
using TrainingGuides.Web.Features.Shared.Logging;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Commerce.Products.Services;

public class ProductService(IContentItemRetrieverService contentItemRetrieverService,
    ILogger<ProductService> logger,
    IInfoProvider<ProductAvailableStockInfo> productAvailableStockInfoProvider,
    IInfoProvider<TagInfo> tagInfoProvider,
    ITaxonomyRetriever taxonomyRetriever,
    IPreferredLanguageRetriever preferredLanguageRetriever,
    IHttpContextAccessor httpContextAccessor,
    IPriceCalculationService<PriceCalculationRequest, TrainingGuidesPriceCalculationResult> priceCalculationService) : IProductService
{
    private const decimal LowStockThreshold = 20m;
    private const string MATERIAL_TAXONOMY = "ProductMaterial";
    private const string COLOR_PATTERN_TAXONOMY = "ProductColor_Pattern";

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

    /// <summary>
    /// Extracts all specified taxonomy tags for a specific schema from a product and its variants.
    /// Checks both the product itself and any variants that implement the schema.
    /// </summary>
    /// <typeparam name="TSchema">The schema interface to check for (e.g., IColorPatternSchema, IMaterialSchema)</typeparam>
    /// <param name="product">The product to extract tags from</param>
    /// <param name="tagSelector">Function to extract tags from a product object implementing TSchema</param>
    /// <returns>All distinct tags found on the product and its variants</returns>
    internal IEnumerable<TagReference> GetAllTagsForSchema<TSchema>(
        IProductSchema product,
        Func<TSchema, IEnumerable<TagReference>> tagSelector) where TSchema : class
    {
        var tags = new List<TagReference>();

        // Check if product itself implements the schema
        if (product is TSchema schemaProduct)
            tags.AddRange(tagSelector(schemaProduct));

        // Check variants if product has them
        if (product is IProductParentSchema parent)
        {
            foreach (var variant in parent.ProductParentSchemaVariants.OfType<TSchema>())
                tags.AddRange(tagSelector(variant));
        }

        return tags.DistinctBy(t => t.Identifier);
    }

    /// <summary>
    /// Extracts all color tags from the product and its variants, if applicable.
    /// </summary>
    /// <param name="product">The product to extract color tags from</param>
    /// <returns>All color tags found on the product and its variants</returns>
    public IEnumerable<TagReference> GetAllColors(IProductSchema product) =>
        GetAllTagsForSchema<IColorPatternSchema>(product, p => p.ColorPattern);

    /// <summary>
    /// Extracts all material tags from the product and its variants, if applicable.
    /// </summary>
    /// <param name="product">The product to extract material tags from</param>
    /// <returns>All material tags found on the product and its variants</returns>
    public IEnumerable<TagReference> GetAllMaterials(IProductSchema product) =>
        GetAllTagsForSchema<IMaterialSchema>(product, p => p.MaterialSchemaMaterial);

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

    /// <inheritdoc/>
    /// <remarks>
    /// When <paramref name="useAndLogic"/> is true, applies LINQ post-filtering for AND logic.
    /// Uses flexible taxonomy extraction that works regardless of where attributes are stored
    /// (parent only, variant only, or both).
    /// 
    /// Warning: AND post-filtering breaks traditional pagination (pages may have uneven/zero items).
    /// This example retrieves all items for simplicity, but lazy loading/infinite scroll is
    /// recommended for production scenarios with large catalogs.
    /// </remarks>
    public async Task<IEnumerable<ProductPage>> RetrieveProductPages(
        string parentPagePath,
        string securedItemsDisplayMode,
        string appliedMaterialsFilter = "",
        string appliedColorsFilter = "",
        bool useAndLogic = false)
    {
        var materialFilters = ParseFilterValues(appliedMaterialsFilter);
        var colorFilters = ParseFilterValues(appliedColorsFilter);

        // Get all products matching ANY filter using the core method
        var productPages = await RetrieveFilteredProductPages(
            parentPagePath, securedItemsDisplayMode, materialFilters, colorFilters);

        // If AND logic is requested and both filters are specified, apply LINQ post-filtering
        if (useAndLogic && materialFilters is not null && colorFilters is not null)
        {
            var materialTaxonomy = await GetTaxonomyData(MATERIAL_TAXONOMY);
            var colorTaxonomy = await GetTaxonomyData(COLOR_PATTERN_TAXONOMY);

            var materialTagGuids = materialTaxonomy?.Tags
                .Where(tag => materialFilters.Contains(tag.Name.ToLower()))
                .Select(tag => tag.Identifier) ?? [];

            var colorTagGuids = colorTaxonomy?.Tags
                .Where(tag => colorFilters.Contains(tag.Name.ToLower()))
                .Select(tag => tag.Identifier) ?? [];

            // Filter to keep only products that have ALL required attributes (flexible logic)
            return productPages.Where(page =>
            {
                var product = page.ProductPageProducts.FirstOrDefault();
                if (product is null)
                {
                    return false;
                }

                // Use flexible extraction methods that handle any taxonomy distribution
                bool productMatchesMaterial = GetAllMaterials(product)
                    .Any(tag => materialTagGuids.Contains(tag.Identifier));

                bool productMatchesColor = GetAllColors(product)
                    .Any(tag => colorTagGuids.Contains(tag.Identifier));

                return productMatchesMaterial && productMatchesColor;
            });
        }

        // OR logic (default): return database results directly
        return productPages;
    }

    /// <summary>
    /// Core method that retrieves product pages with pre-parsed filter values.
    /// </summary>
    private async Task<IEnumerable<ProductPage>> RetrieveFilteredProductPages(
        string parentPagePath,
        string securedItemsDisplayMode,
        IEnumerable<string>? materialFilters,
        IEnumerable<string>? colorFilters)
    {
        if (string.IsNullOrEmpty(parentPagePath))
        {
            return [];
        }

        bool includeSecuredItems = securedItemsDisplayMode.Equals(SecuredOption.IncludeEverything.ToString())
                || securedItemsDisplayMode.Equals(SecuredOption.PromptForLogin.ToString());

        if (materialFilters is not null || colorFilters is not null)
        {
            var filteredIds = await RetrieveFilteredProductIDs(includeSecuredItems, materialFilters, colorFilters);

            var parentIds = await RetrieveFilteredProductParentIDs(includeSecuredItems, filteredIds);

            var allProductIds = parentIds.Union(filteredIds);

            if (!allProductIds.Any())
            {
                return [];
            }

            return await contentItemRetrieverService.RetrieveWebPageChildrenByPath<ProductPage>(
                path: parentPagePath,
                depth: 3,
                includeSecuredItems: includeSecuredItems,
                additionalQueryConfiguration: query => query
                    .Linking(nameof(ProductPage.ProductPageProducts), allProductIds));
        }

        return await contentItemRetrieverService.RetrieveWebPageChildrenByPath<ProductPage>(
            path: parentPagePath,
            depth: 3,
            includeSecuredItems: includeSecuredItems);
    }

    private async Task<IEnumerable<int>> RetrieveFilteredProductIDs(bool includeSecuredItems,
        IEnumerable<string>? materialFilters,
        IEnumerable<string>? colorFilters)
    {
        var materialTaxonomy = await GetTaxonomyData(MATERIAL_TAXONOMY);
        var colorTaxonomy = await GetTaxonomyData(COLOR_PATTERN_TAXONOMY);

        Func<RetrieveContentOfReusableSchemasQueryParameters, RetrieveContentOfReusableSchemasQueryParameters> filterFunc = query =>
        {
            // Set up filter functions that do nothing for each taxonomy
            Func<RetrieveContentOfReusableSchemasQueryParameters, RetrieveContentOfReusableSchemasQueryParameters> materialFilterFunc = q => q;
            Func<RetrieveContentOfReusableSchemasQueryParameters, RetrieveContentOfReusableSchemasQueryParameters> colorFilterFunc = q => q;

            if (materialFilters is not null)
            {
                materialFilterFunc = GetFuncForFilter(materialFilters, materialTaxonomy, nameof(IMaterialSchema.MaterialSchemaMaterial));
            }

            if (colorFilters is not null)
            {
                colorFilterFunc = GetFuncForFilter(colorFilters, colorTaxonomy, nameof(IColorPatternSchema.ColorPattern));
            }

            Func<RetrieveContentOfReusableSchemasQueryParameters, RetrieveContentOfReusableSchemasQueryParameters> columnsFilterFunc = q =>
                q.Columns(nameof(ContentItemFields.ContentItemID));

            return columnsFilterFunc(colorFilterFunc(materialFilterFunc(query)));
        };

        var items = await contentItemRetrieverService.RetrieveContentItemsBySchemas<IProductSchema>(
            schemaNames: [IProductSchema.REUSABLE_FIELD_SCHEMA_NAME, IMaterialSchema.REUSABLE_FIELD_SCHEMA_NAME, IColorPatternSchema.REUSABLE_FIELD_SCHEMA_NAME],
            additionalQueryConfiguration: query => filterFunc(query),
            depth: 0,
            includeSecuredItems: includeSecuredItems
        );

        return items.Select(item => (item as IContentItemFieldsSource)?.SystemFields.ContentItemID)
            .Where(id => id is not null)
            .Cast<int>();
    }

    /// <summary>
    /// Retrieves parent IDs of products matching the specified filtered IDs.
    /// </summary>
    /// <param name="includeSecuredItems">Indicates whether to include secured items.</param>
    /// <param name="filteredIds">The filtered product IDs.</param>
    private async Task<IEnumerable<int>> RetrieveFilteredProductParentIDs(bool includeSecuredItems, IEnumerable<int> filteredIds)
    {
        if (!filteredIds.Any())
        {
            return [];
        }

        var parents = await contentItemRetrieverService.RetrieveContentItemsBySchemas<IContentItemFieldsSource>(
                schemaNames: [IProductParentSchema.REUSABLE_FIELD_SCHEMA_NAME],
                additionalQueryConfiguration: query => query
                    .LinkingSchemaField(nameof(IProductParentSchema.ProductParentSchemaVariants), filteredIds)
                    .Columns(nameof(ContentItemFields.ContentItemID)),
                depth: 3,
                includeSecuredItems: includeSecuredItems);

        return parents.Select(parent => parent.SystemFields.ContentItemID);
    }

    /// <summary>
    /// Generates a filter function for retrieving content items based on the specified filter values and taxonomy.
    /// </summary>
    /// <param name="filterValues"></param>
    /// <param name="taxonomy"></param>
    /// <param name="taxonomyColumnName"></param>
    /// <returns></returns>
    private Func<RetrieveContentOfReusableSchemasQueryParameters, RetrieveContentOfReusableSchemasQueryParameters> GetFuncForFilter(IEnumerable<string> filterValues, TaxonomyData? taxonomy, string taxonomyColumnName)
    {
        var tags = taxonomy?.Tags
                    .Where(tag => filterValues.Contains(tag.Name.ToLower()))
                    .Select(tag => tag.Identifier) ?? [];

        return query => query.Where(where => where.WhereContainsTags(taxonomyColumnName, tags).Or());
    }

    /// <summary>
    /// Parses filter values from a comma-separated string.
    /// </summary>
    /// <param name="filterValues">A comma-separated string of filter values.</param>
    /// <returns>A collection of parsed filter values, or <c>null</c> if no valid filters are found.</returns>
    internal IEnumerable<string>? ParseFilterValues(string filterValues)
    {
        if (string.IsNullOrWhiteSpace(filterValues) || filterValues.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var values = filterValues.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(value => value.Trim().ToLower())
            .Where(value => !value.Equals("all", StringComparison.OrdinalIgnoreCase));

        return values.Any() ? values : null;
    }

    /// <inheritdoc/>
    public async Task<List<ProductListingItemViewModel>> GetProductListingItemViewModels(
        IEnumerable<ProductPage> productPages,
        string securedItemsDisplayMode)
    {
        var models = new List<ProductListingItemViewModel>();
        var user = httpContextAccessor.HttpContext?.User;

        foreach (var productPage in productPages)
        {
            if (productPage?.ProductPageProducts == null || !productPage.ProductPageProducts.Any())
            {
                continue;
            }

            var product = productPage.ProductPageProducts.First();

            bool accessDenied = securedItemsDisplayMode.Equals(SecuredOption.PromptForLogin.ToString())
                && (!productPage.HasAccess(user)
                    || !((product as IContentItemFieldsSource)?.HasAccess(user) ?? true));

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
    /// Retrieves taxonomy data for a given taxonomy name.
    /// </summary>
    /// <param name="taxonomyName">The name of the taxonomy to retrieve.</param>
    /// <returns>The <see cref="TaxonomyData"/> if found; otherwise, <c>null</c>.</returns>
    /// <remarks>
    /// Use extra caching in real-world scenarios.
    /// </remarks>
    private async Task<TaxonomyData?> GetTaxonomyData(string taxonomyName)
    {
        var taxonomy = await taxonomyRetriever.RetrieveTaxonomy(taxonomyName, preferredLanguageRetriever.Get());

        if (taxonomy is null || taxonomy.Tags is null || !taxonomy.Tags.Any())
        {
            return null;
        }

        return taxonomy;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ProductListingFilterViewModel>> GetProductListingFilters()
    {
        var filters = new List<ProductListingFilterViewModel?>
            {
                await GetProductListingFilter("ProductMaterial"),
                await GetProductListingFilter("ProductColor_Pattern")
            };

        return filters.Where(filter => filter is not null).Cast<ProductListingFilterViewModel>();
    }

    /// <summary>
    /// Retrieves data for a product listing filter based on the specified taxonomy name.
    /// </summary>
    /// <param name="taxonomyName">The name of the taxonomy to retrieve.</param>
    /// <returns>The <see cref="ProductListingFilterViewModel"/> if found; otherwise, <c>null</c>.</returns>
    private async Task<ProductListingFilterViewModel?> GetProductListingFilter(string taxonomyName)
    {
        var taxonomy = await GetTaxonomyData(taxonomyName);

        if (taxonomy is null)
        {
            return null;
        }

        return new ProductListingFilterViewModel
        {
            ProductListingFilterName = taxonomy.Taxonomy.Name,
            ProductListingFilterDisplayName = taxonomy.Taxonomy.Title,
            ProductListingFilterOptions = taxonomy.Tags.Select(tag => new ProductListingFilterOptionViewModel
            {
                FilterOptionDisplayName = tag.Title,
                FilterOptionValue = tag.Name
            }).ToList() ?? []
        };
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
