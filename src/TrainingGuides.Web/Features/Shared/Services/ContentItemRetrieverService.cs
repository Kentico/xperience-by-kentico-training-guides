using CMS.ContentEngine;
using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;

namespace TrainingGuides.Web.Features.Shared.Services;

public class ContentItemRetrieverService : IContentItemRetrieverService
{
    private readonly IContentRetriever contentRetriever;
    private readonly IWebsiteChannelContext webSiteChannelContext;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;

    public ContentItemRetrieverService(
        IContentRetriever contentRetriever,
        IWebsiteChannelContext webSiteChannelContext,
        IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.contentRetriever = contentRetriever;
        this.webSiteChannelContext = webSiteChannelContext;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }

    /// <inheritdoc />
    public async Task<T?> RetrieveWebPageById<T>(
        int webPageItemId,
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrievePagesParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentRetriever.RetrievePages<T>(
            parameters,
            query =>
                query.Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemID), webPageItemId)),
            RetrievalCacheSettings.CacheDisabled);

        return pages.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<T?> RetrieveCurrentPage<T>(
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrieveCurrentPageParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        return await contentRetriever.RetrieveCurrentPage<T>(parameters);
    }

    /// <inheritdoc />
    public async Task<T?> RetrieveWebPageByGuid<T>(
        Guid webPageItemGuid,
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrieveContentParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentRetriever.RetrieveContentByGuids<T>(
            [webPageItemGuid],
            parameters);

        return pages.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<T?> RetrieveWebPageByContentItemGuid<T>(
        Guid contentItemGuid,
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrieveContentParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentRetriever.RetrieveContentByGuids<T>(
            [contentItemGuid],
            parameters);

        return pages.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> RetrieveWebPageContentItems<T>(
        Action<RetrievePagesQueryParameters>? additionalQueryConfiguration = null,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrievePagesParameters
        {
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        if (additionalQueryConfiguration != null)
        {
            return await contentRetriever.RetrievePages<T>(
                parameters,
                additionalQueryConfiguration,
                RetrievalCacheSettings.CacheDisabled);
        }
        else
        {
            return await contentRetriever.RetrievePages<T>(parameters);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> RetrieveWebPageChildrenByPath<T>(
        string path,
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrievePagesParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview,
            PathMatch = PathMatch.Children(path)
        };

        return await contentRetriever.RetrievePages<T>(parameters);
    }

    /// <inheritdoc />
    public async Task<T?> RetrieveContentItemByGuid<T>(
        Guid contentItemGuid,
        int depth = 1,
        string? languageName = null)
        where T : IContentItemFieldsSource, new()
    {
        var parameters = new RetrieveContentParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        var items = await contentRetriever.RetrieveContentByGuids<T>(
            [contentItemGuid],
            parameters);

        return items.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> RetrieveReusableContentItems<T>(
        Action<RetrieveContentQueryParameters>? additionalQueryConfiguration = null,
        string? languageName = null)
        where T : IContentItemFieldsSource, new()
    {
        var parameters = new RetrieveContentParameters
        {
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        return additionalQueryConfiguration != null
            ? await contentRetriever.RetrieveContent<T>(
                    parameters,
                    additionalQueryConfiguration,
                    RetrievalCacheSettings.CacheDisabled)
            : await contentRetriever.RetrieveContent<T>(parameters);
    }
}