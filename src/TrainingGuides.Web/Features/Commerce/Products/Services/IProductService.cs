using TrainingGuides.Web.Commerce.Products.Models;

namespace TrainingGuides.Web.Commerce.Products.Services;

public interface IProductService
{
    /// <summary>
    /// Determines whether the specified product has variants.
    /// </summary>
    /// <param name="product">The product to check.</param>
    /// <returns><c>true</c> if the product has variants; otherwise, <c>false</c>.</returns>
    bool ProductHasVariants(IProductSchema product);

    /// <summary>
    /// Determines whether the specified product is a variant.
    /// </summary>
    /// <param name="product">The product to check.</param>
    /// <returns><c>true</c> if the product is a variant; otherwise, <c>false</c>.</returns>
    bool ProductIsVariant(IProductSchema product);

    /// <summary>
    /// Retrieves the parent product of the specified variant.
    /// </summary>
    /// <param name="variant">The variant whose parent product to retrieve.</param>
    /// <returns>The parent product if the specified product is a variant; otherwise, <c>null</c>.</returns>
    Task<IProductSchema?> GetVariantParent(IProductSchema variant);

    /// <summary>
    /// Retrieves the first variant of the specified product, ordered by price.
    /// </summary>
    /// <param name="product">The product whose first variant to retrieve.</param>
    /// <returns>The first variant if the product has variants; otherwise, <c>null</c>.</returns>
    IProductSchema? GetFirstVariant(IProductParentSchema product);

    /// <summary>
    /// Retrieves a variant of the specified product by its code name.
    /// </summary>
    /// <param name="product">The product whose variant to retrieve.</param>
    /// <param name="variantSlug">The code name of the variant to retrieve.</param>
    /// <returns>The variant with the specified code name if found; otherwise, <c>null</c>.</returns>
    IProductSchema? GetVariantByCodeName(IProductParentSchema product, string variantSlug);

    /// <summary>
    /// Creates a view model for the specified product with an optional selected variant.
    /// </summary>
    /// <param name="product">The product for which to create the view model.</param>
    /// <param name="selectedVariant">The optional selected variant. If not provided and the product has variants, the first variant will be used.</param>
    /// <param name="accessDenied">Indicates whether access to the product is denied. Assumed false if no value is provided.</param>
    /// <returns>A <see cref="ProductViewModel"/> populated with product and variant information.</returns>
    Task<ProductViewModel> GetViewModel(IProductSchema? product, IProductSchema? selectedVariant = null, bool accessDenied = false);

    /// <summary>
    /// Retrieves the current product page with all related product data.
    /// </summary>
    /// <returns>The current product page if found; otherwise, <c>null</c>.</returns>
    Task<ProductPage?> GetCurrentProductPage();

    /// <summary>
    /// Retrieves a product page by its content item GUID.
    /// </summary>
    /// <param name="contentItemGuid">The GUID of the product page content item.</param>
    /// <returns>The product page if found; otherwise, <c>null</c>.</returns>
    Task<ProductPage?> GetProductPageByGuid(Guid contentItemGuid);
}