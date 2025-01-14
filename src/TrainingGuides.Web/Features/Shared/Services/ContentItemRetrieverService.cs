using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc.Routing;
using TrainingGuides.Web.Features.Shared.OptionProviders.OrderBy;

namespace TrainingGuides.Web.Features.Shared.Services;

public class ContentItemRetrieverService<T> : IContentItemRetrieverService<T>
{
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IWebsiteChannelContext webSiteChannelContext;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;

    public ContentItemRetrieverService(
        IContentQueryExecutor contentQueryExecutor,
        IWebsiteChannelContext webSiteChannelContext,
        IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.contentQueryExecutor = contentQueryExecutor;
        this.webSiteChannelContext = webSiteChannelContext;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }


    /// <inheritdoc/>
    public async Task<T?> RetrieveWebPageById(
        int webPageItemId,
        string contentTypeName,
        int depth = 1)
    {
        var pages = await RetrieveWebPageContentItems(
                contentTypeName,
                config => config
                    .Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemID), webPageItemId))
                    .WithLinkedItems(depth));
        return pages.FirstOrDefault();
    }

    ///  <inheritdoc/>
    public async Task<T?> RetrieveWebPageByGuid(
        Guid? webPageItemGuid,
        string contentTypeName,
        int depth = 1)
    {
        var pages = await RetrieveWebPageContentItems(
                contentTypeName,
                config => config
                    .Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemGUID), webPageItemGuid))
                    .WithLinkedItems(depth));
        return pages.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> RetrieveWebPageContentItems(
        string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter)
    {
        var builder = new ContentItemQueryBuilder()
                            .ForContentType(
                                contentTypeName,
                                config => queryFilter(config)
                                .ForWebsite(webSiteChannelContext.WebsiteChannelName)
                            )
                            .InLanguage(preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentQueryExecutor.GetMappedWebPageResult<T>(builder, queryExecutorOptions);

        return pages;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> RetrieveWebPageChildrenByPath(
        string parentPageContentTypeName,
        string parentPagePath,
        int depth = 1) => await RetrieveWebPageChildrenByPath(parentPageContentTypeName, parentPagePath, null, depth);

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> RetrieveWebPageChildrenByPathAndReference(
        string parentPageContentTypeName,
        string parentPagePath,
        string referenceFieldName,
        IEnumerable<int> referenceIds,
        int depth = 1
    ) => await RetrieveWebPageChildrenByPath(
            parentPageContentTypeName,
            parentPagePath,
            config => config.Linking(referenceFieldName, referenceIds),
            depth);

    /// <inheritdoc/>
    public async Task<T?> RetrieveContentItemByGuid(
        Guid contentItemGuid,
        string contentTypeName,
        int depth = 1)
    {
        var items = await RetrieveReusableContentItems(
                contentTypeName ?? string.Empty,
                config => config
                    .Where(where => where.WhereEquals(nameof(ContentItemFields.ContentItemGUID), contentItemGuid))
                    .WithLinkedItems(depth));
        return items.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> RetrieveReusableContentItems(
        string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter)
    {
        var builder = new ContentItemQueryBuilder()
                            .ForContentType(
                                contentTypeName,
                                config => queryFilter(config)
                            )
                            .InLanguage(preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        var items = await contentQueryExecutor.GetMappedResult<T>(builder, queryExecutorOptions);

        return items;
    }

    private async Task<IEnumerable<T>> RetrieveContentItems(
        Action<ContentTypesQueryParameters> contentTypesQueryParameters,
        Action<ContentQueryParameters> contentQueryParameters)
    {
        var builder = new ContentItemQueryBuilder();

        builder
            .ForContentTypes(contentTypesQueryParameters)
            .Parameters(contentQueryParameters)
            .InLanguage(preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        return await contentQueryExecutor.GetMappedResult<T>(builder, queryExecutorOptions);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> RetrieveReusableContentItemsFromSmartFolder(
        string contentTypeName,
        Guid smartFolderGuid,
        OrderByOption orderBy,
        int topN = 20,
        int depth = 1)
    {
        const string LAST_PUBLISHED_COLUMN_NAME = "ContentItemCommonDataLastPublishedWhen";


        Action<ContentTypesQueryParameters> contentTypesQueryParameters = parameters => parameters
            .InSmartFolder(smartFolderGuid)
            .OfContentType(contentTypeName)
            .WithLinkedItems(depth)
            .WithContentTypeFields();

        Action<ContentQueryParameters> contentQueryParameters = parameters
            => parameters
                .OrderBy(new OrderByColumn(
                    LAST_PUBLISHED_COLUMN_NAME,
                    orderBy.Equals(OrderByOption.NewestFirst) ? OrderDirection.Descending : OrderDirection.Ascending))
                .TopN(topN);

        return await RetrieveContentItems(contentTypesQueryParameters, contentQueryParameters);
    }

    private async Task<IEnumerable<T>> RetrieveWebPageChildrenByPath(
        string parentPageContentTypeName,
        string parentPagePath,
        Action<ContentTypeQueryParameters>? customContentTypeQueryParameters,
        int depth = 1)
    {
        Action<ContentTypeQueryParameters> contentQueryParameters = customContentTypeQueryParameters != null
            ? config => customContentTypeQueryParameters(config
                .ForWebsite(webSiteChannelContext.WebsiteChannelName, [PathMatch.Children(parentPagePath)])
                .WithLinkedItems(depth)
            )
            : config => config
                .ForWebsite(webSiteChannelContext.WebsiteChannelName, [PathMatch.Children(parentPagePath)])
                .WithLinkedItems(depth);

        var builder = new ContentItemQueryBuilder()
                            .ForContentType(
                                parentPageContentTypeName,
                                contentQueryParameters
                                )
                            .InLanguage(preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentQueryExecutor.GetMappedWebPageResult<T>(builder, queryExecutorOptions);

        return pages;
    }
}


public class ContentItemRetrieverService : IContentItemRetrieverService
{
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IWebsiteChannelContext websiteChannelContext;

    public ContentItemRetrieverService(
        IContentQueryExecutor contentQueryExecutor,
        IWebsiteChannelContext websiteChannelContext)
    {
        this.contentQueryExecutor = contentQueryExecutor;
        this.websiteChannelContext = websiteChannelContext;
    }

    private async Task<IEnumerable<IContentItemFieldsSource>> RetrieveContentItems(Action<ContentQueryParameters> contentQueryParameters,
        Action<ContentTypesQueryParameters> contentTypesQueryParameters)
    {
        var builder = new ContentItemQueryBuilder();

        builder.ForContentTypes(contentTypesQueryParameters)
            .Parameters(contentQueryParameters);

        return await contentQueryExecutor.GetMappedResult<IContentItemFieldsSource>(builder);
    }

    /// <summary>
    /// Retrieves content items based on the provided schema name and tag guids.
    /// </summary>
    /// <param name="schemaName">The name of the reusable field schema</param>
    /// <param name="taxonomyColumnName">The name of the column that holds the taxonomy value</param>
    /// <param name="tagGuids">Guids of tags to filter the output by</param>
    /// <returns>Enumerable list of content items</returns>
    public async Task<IEnumerable<IContentItemFieldsSource>> RetrieveContentItemsBySchemaAndTags(string schemaName, string taxonomyColumnName, IEnumerable<Guid> tagGuids)
    {
        Action<ContentQueryParameters> contentQueryParameters = parameters
            => parameters.Where(where => where.WhereContainsTags(taxonomyColumnName, tagGuids));

        Action<ContentTypesQueryParameters> contentTypesQueryParameters = parameters
            => parameters.OfReusableSchema(schemaName);

        return await RetrieveContentItems(contentQueryParameters, contentTypesQueryParameters);
    }

    private async Task<IEnumerable<IWebPageFieldsSource>> RetrieveWebPages(Action<ContentQueryParameters> parameters)
    {
        var builder = new ContentItemQueryBuilder();

        builder.ForContentTypes(query =>
            {
                query.ForWebsite(websiteChannelContext.WebsiteChannelName);
            })
            .Parameters(parameters);

        return await contentQueryExecutor.GetMappedResult<IWebPageFieldsSource>(builder);
    }

    /// <summary>
    /// Retrieves the IWebPageFieldsSource of a web page item by Id.
    /// </summary>
    /// <param name="webPageItemId">the Id of the web page item</param>
    /// <returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    public async Task<IWebPageFieldsSource?> RetrieveWebPageById(
        int webPageItemId)
    {
        var pages = await RetrieveWebPages(parameters =>
            {
                parameters.Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemID), webPageItemId));
            });

        return pages.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the IWebPageFieldsSource of a web page item by Guid.
    /// </summary>
    /// <param name="webPageItemGuid">the Guid of the web page item</param>
    /// <returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    public async Task<IWebPageFieldsSource?> RetrieveWebPageByGuid(
        Guid webPageItemGuid)
    {
        var pages = await RetrieveWebPages(parameters =>
            {
                parameters.Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemGUID), webPageItemGuid));
            });

        return pages.FirstOrDefault();
    }
}