using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using Kentico.Xperience.Admin.Base.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TrainingGuides.ProductStock;

namespace TrainingGuides.Admin.ProductStock;

public class ProductMetadataRetriever(IContentItemManagerFactory contentItemManagerFactory,
    IHttpContextAccessor httpContextAccessor,
    IContentLanguageRetriever contentLanguageRetriever) : IProductMetadataRetriever
{
    /// <inheritdoc/>
    public async Task<ContentItemLanguageMetadata> GetProductMetadata(ProductAvailableStockInfo productStockInfo)
    {
        // Uses the default language to ensure consistent metadata retrieval
        var defaultContentLanguage = await contentLanguageRetriever.GetDefaultContentLanguage();

        // Gets the current authenticated user for content manager context
        var currentUser = await httpContextAccessor.HttpContext?.RequestServices?.GetRequiredService<IAuthenticatedUserAccessor>().Get();

        // Creates content manager with proper user context for security
        var contentItemManager = contentItemManagerFactory.Create(currentUser?.UserID ?? 0);

        // Retrieves the product metadata using the content item ID stored in the stock record
        // This gets the product name and other language-specific metadata
        var productMetadata = await contentItemManager.GetContentItemLanguageMetadata(
            productStockInfo.ProductAvailableStockContentItemID,
            defaultContentLanguage.ContentLanguageName);

        return productMetadata;
    }
}

public interface IProductMetadataRetriever
{
    /// <summary>
    /// Retrieves the product metadata for a given product stock record.
    /// </summary>
    /// <param name="productStockInfo">The product stock information containing the product ID.</param>
    /// <returns>The product metadata as a ContentItemLanguageMetadata object.</returns>
    Task<ContentItemLanguageMetadata> GetProductMetadata(ProductAvailableStockInfo productStockInfo);
}