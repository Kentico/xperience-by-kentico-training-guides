using CMS.ContentEngine;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Commerce.Products.Services;
using TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductListing;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(
        identifier: ProductListingWidgetViewComponent.IDENTIFIER,
        viewComponentType: typeof(ProductListingWidgetViewComponent),
        name: "Product listing widget",
        propertiesType: typeof(ProductListingWidgetProperties),
        Description = "Displays a filterable list of products from a selected content tree section.",
        IconClass = "icon-list")]

namespace TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductListing;

public class ProductListingWidgetViewComponent(
        IContentItemRetrieverService contentItemRetrieverService,
        IProductService productService,
        IMembershipService membershipService,
        IWebPageDataContextRetriever webPageDataContextRetriever,
        ITaxonomyRetriever taxonomyRetriever,
        IPreferredLanguageRetriever preferredLanguageRetriever,
        IHttpRequestService httpRequestService) : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ProductListingWidget";

    private const string MATERIAL_TAXONOMY = "ProductMaterial";
    private const string COLOR_PATTERN_TAXONOMY = "ProductColor_Pattern";

    public async Task<ViewViewComponentResult> InvokeAsync(ProductListingWidgetProperties properties)
    {
        string appliedMaterialsFilter = httpRequestService.GetQueryStringValue(MATERIAL_TAXONOMY).ToLower();
        string appliedColorsFilter = httpRequestService.GetQueryStringValue(COLOR_PATTERN_TAXONOMY).ToLower();

        var model = new ProductListingWidgetViewModel
        {
            CtaText = properties.CtaText,
            SignInText = properties.SignInText,
            IsAuthenticated = await membershipService.IsMemberAuthenticated(),
            AvailableFilters = await GetProductListingFilters()
        };

        string parentPagePath = await GetParentPagePath(properties.ParentPageSelection);

        if (!string.IsNullOrEmpty(parentPagePath))
        {
            var productPages = await RetrieveProductPagesByPath(
                parentPagePath,
                properties.SecuredItemsDisplayMode,
                appliedMaterialsFilter,
                appliedColorsFilter);

            model.Products = await GetProductViewModels(
                productPages,
                properties.SecuredItemsDisplayMode);
        }

        return View("~/Features/Commerce/Products/Widgets/ProductListing/ProductListingWidget.cshtml", model);
    }

    private async Task<string> GetParentPagePath(IEnumerable<ContentItemReference> parentPageSelection)
    {
        // If a specific page is selected, use it
        if (parentPageSelection.Any())
        {
            var selectedPageGuid = parentPageSelection.First().Identifier;
            var selectedPage = await contentItemRetrieverService.RetrieveWebPageByContentItemGuid(selectedPageGuid);
            return selectedPage?.SystemFields.WebPageItemTreePath ?? string.Empty;
        }

        // Otherwise, use the current page from the web page context
        var currentPageContext = webPageDataContextRetriever.Retrieve();
        if (currentPageContext.WebPage != null)
        {
            var currentPage = await contentItemRetrieverService.RetrieveWebPageById(
                currentPageContext.WebPage.WebPageItemID);
            return currentPage?.SystemFields.WebPageItemTreePath ?? string.Empty;
        }

        return string.Empty;
    }


    private async Task<IEnumerable<ProductPage>> RetrieveProductPagesByPath(
        string parentPagePath,
        string securedItemsDisplayMode,
        string appliedMaterialsFilter,
        string appliedColorsFilter)
    {
        if (string.IsNullOrEmpty(parentPagePath))
        {
            return [];
        }

        bool includeSecuredItems = securedItemsDisplayMode.Equals(SecuredOption.IncludeEverything.ToString())
                || securedItemsDisplayMode.Equals(SecuredOption.PromptForLogin.ToString());

        var materialFilters = ParseFilterValues(appliedMaterialsFilter);
        var colorFilters = ParseFilterValues(appliedColorsFilter);

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

        // TODO figure out why this doesn't work with multiple valid filter options selected
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
    private IEnumerable<string>? ParseFilterValues(string filterValues)
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

    private async Task<List<ProductListingItemViewModel>> GetProductViewModels(
        IEnumerable<ProductPage> productPages,
        string securedItemsDisplayMode)
    {
        var models = new List<ProductListingItemViewModel>();
        bool isAuthenticated = await membershipService.IsMemberAuthenticated();

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

            var productViewModel = await productService.GetViewModel(product, accessDenied: accessDenied);
            string productPageUrl = productPage.GetUrl()?.RelativePath ?? string.Empty;

            models.Add(new ProductListingItemViewModel
            {
                Product = productViewModel,
                ProductPageUrl = productPageUrl
            });
        }

        return models;
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

    private async Task<IEnumerable<ProductListingFilterViewModel>> GetProductListingFilters()
    {
        var filters = new List<ProductListingFilterViewModel?>
            {
                await GetProductListingFilter(MATERIAL_TAXONOMY),
                await GetProductListingFilter(COLOR_PATTERN_TAXONOMY)
            };

        return filters.Where(filter => filter is not null).Cast<ProductListingFilterViewModel>();
    }
}
