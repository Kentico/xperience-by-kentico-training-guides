using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Helpers;

namespace TrainingGuides.Admin.ProductStock;

/// <summary>
/// Service for retrieving the default content language.
/// This ensures consistent data retrieval across multilingual content scenarios.
/// </summary>
public sealed class DefaultContentLanguageRetriever(
        IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider,
        IProgressiveCache progressiveCache,
        ICacheDependencyBuilderFactory cacheDependencyBuilderFactory) : IDefaultContentLanguageRetriever
{
    /// <summary>
    /// Cache duration in minutes (24 hours).
    /// </summary>
    private const int ONE_DAY = 24 * 60;

    /// <summary>
    /// Retrieves the default content language with caching for performance.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The default ContentLanguageInfo.</returns>
    public async Task<ContentLanguageInfo> Get(CancellationToken cancellationToken = default) =>
        // Uses progressive cache to avoid repeated database queries
        (await progressiveCache.LoadAsync(async (cacheSettings, token) =>
        {
            // Sets up cache dependency to invalidate when languages change
            var cacheDependencyBuilder = cacheDependencyBuilderFactory.Create();
            cacheSettings.CacheDependency = cacheDependencyBuilder
                .ForInfoObjects<ContentLanguageInfo>()
                    .All()
                    .Builder()
                .Build();

            // Queries for the default language (marked as default in the system)
            var result = await contentLanguageInfoProvider.Get()
               .WhereTrue(nameof(ContentLanguageInfo.ContentLanguageIsDefault))
               .TopN(1)
               .GetEnumerableTypedResultAsync(cancellationToken: cancellationToken);

            return result.FirstOrDefault();
        },
        // Caches for one day with automatic invalidation when content languages change
        new CacheSettings(ONE_DAY, true, nameof(DefaultContentLanguageRetriever), nameof(Get)), cancellationToken))!;
}