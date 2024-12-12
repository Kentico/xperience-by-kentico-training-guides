using CMS;
using CMS.Core;

[assembly: RegisterImplementation(typeof(IWebPageUrlRetriever), typeof(TrainingGuidesWebPageUrlRetriever))]
public class TrainingGuidesWebPageUrlRetriever : IWebPageUrlRetriever
{
    private WebPageUrl EmptyUrl => new(string.Empty, string.Empty);
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    private readonly IEventLogService eventLogService;

    public TrainingGuidesWebPageUrlRetriever(IWebPageUrlRetriever webPageUrlRetriever,
        IEventLogService eventLogService)
    {
        this.webPageUrlRetriever = webPageUrlRetriever;
        this.eventLogService = eventLogService;
    }

    /// <summary>
    /// Retrieves URL for a web page represented by a model that implements <see cref="IWebPageFieldsSource"/> interface.
    /// </summary>
    /// <param name="webPageFieldsSource">Web page model that implements <see cref="IWebPageFieldsSource"/> with data retrieved from the database.</param>
    /// <param name="cancellationToken">Cancellation instruction.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="webPageFieldsSource"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <see cref="IWebPageFieldsSource.SystemFields"/> in <paramref name="webPageFieldsSource"/> is null.</exception>
    /// <returns>
    /// Empty URL when <paramref name="webPageFieldsSource"/> model does not contain web page identifier or content language identifier specified in <see cref="IWebPageFieldsSource.SystemFields"/> system data.
    /// Empty URL when web page identified by <see cref="WebPageFields.WebPageItemID"/> in <see cref="IWebPageFieldsSource.SystemFields"/> does not exist.
    /// </returns>
    /// <remarks>
    /// <para>
    /// To construct the resulting URL use <see cref="ContentTypeQueryParametersExtensions.ForWebsite(ContentEngine.ContentTypeQueryParameters, string, PathMatch, bool)"/> or <see cref="ContentTypeQueryParametersExtensions.ForWebsite(ContentEngine.ContentTypeQueryParameters, string, PathMatch[], bool)"/>
    /// with 'includeUrlPath' flag set to <c>true</c>.
    /// </para>
    /// In case of limiting the columns in the query use <see cref="ContentTypeQueryParametersUrlExtensions.UrlPathColumns(ContentEngine.ContentTypeQueryParameters)"/>
    /// to add all required columns to construct the URL.
    /// </remarks>
    public async Task<WebPageUrl> Retrieve(IWebPageFieldsSource webPageFieldsSource, CancellationToken cancellationToken = default)
    {
        try
        {
            return await webPageUrlRetriever.Retrieve(webPageFieldsSource, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            string message = $"{ex.Message}\r\nweb page guid: {webPageFieldsSource?.SystemFields.WebPageItemGUID},\r\nweb page id: {webPageFieldsSource?.SystemFields.WebPageItemID},\r\ntree path: {webPageFieldsSource?.SystemFields.WebPageItemTreePath}";
            eventLogService.LogError(ex.Source, "Retrieve URL", message);
            return EmptyUrl;
        }
    }

    /// <summary>
    /// Retrieves URL for a web page represented by a model that implements <see cref="IWebPageFieldsSource"/> interface and for the <paramref name="languageName"/>.
    /// </summary>
    /// <param name="webPageFieldsSource">Web page model that implements <see cref="IWebPageFieldsSource"/> with data retrieved from the database.</param>
    /// <param name="languageName">Language code.</param>
    /// <param name="cancellationToken">Cancellation instruction.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="webPageFieldsSource"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <see cref="IWebPageFieldsSource.SystemFields"/> in <paramref name="webPageFieldsSource"/> is null.</exception>
    /// <returns>
    /// Empty URL <paramref name="webPageFieldsSource"/> model does not contain web page identifier specified in <see cref="IWebPageFieldsSource.SystemFields"/> system data.
    /// Empty URL when content language identified by <paramref name="languageName"/> does not exist.
    /// Empty URL when web page identified by <see cref="WebPageFields.WebPageItemID"/> in <see cref="IWebPageFieldsSource.SystemFields"/> does not exist.
    /// </returns>
    /// <remarks>
    /// In case the <paramref name="languageName"/> doesn't match the language of the model, the URL is retrieved from the database for the <paramref name="languageName"/>.
    /// <para>
    /// To construct the resulting URL use <see cref="ContentTypeQueryParametersExtensions.ForWebsite(ContentEngine.ContentTypeQueryParameters, string, PathMatch, bool)"/> or <see cref="ContentTypeQueryParametersExtensions.ForWebsite(ContentEngine.ContentTypeQueryParameters, string, PathMatch[], bool)"/>
    /// with 'includeUrlPath' flag set to <c>true</c>.
    /// </para>
    /// In case of limiting the columns in the query use <see cref="ContentTypeQueryParametersUrlExtensions.UrlPathColumns(ContentEngine.ContentTypeQueryParameters)"/>
    /// to add all required columns to construct the URL.
    /// </remarks>
    public async Task<WebPageUrl> Retrieve(IWebPageFieldsSource webPageFieldsSource, string languageName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await webPageUrlRetriever.Retrieve(webPageFieldsSource, languageName, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            string message = $"{ex.Message}\r\nweb page guid: {webPageFieldsSource?.SystemFields.WebPageItemGUID},\r\nweb page id: {webPageFieldsSource?.SystemFields.WebPageItemID},\r\ntree path: {webPageFieldsSource?.SystemFields.WebPageItemTreePath}\r\nlanguage: {languageName}";
            eventLogService.LogError(ex.Source, "Retrieve URL", message);
            return EmptyUrl;
        }
    }

    /// <summary>
    /// Retrieves URL for a web page represented by <paramref name="webPageUrlPath"/>, <paramref name="webPageTreePath"/>, <paramref name="websiteChannelId"/> and <paramref name="languageName"/>.
    /// </summary>
    /// <param name="webPageUrlPath">Web page URL path.</param>
    /// <param name="webPageTreePath">Web page tree path.</param>
    /// <param name="websiteChannelId">Website channel identifier.</param>
    /// <param name="languageName">Language code.</param>
    /// <param name="cancellationToken">Cancellation instruction.</param>
    /// <returns>
    /// Empty URL when <paramref name="webPageTreePath"/> is null or empty.
    /// Empty URL when website channel identified by <paramref name="websiteChannelId"/> does not exist.
    /// Empty URL when content language identified by <paramref name="languageName"/> does not exist.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="languageName"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// To construct the resulting URL use <see cref="ContentTypeQueryParametersExtensions.ForWebsite(ContentEngine.ContentTypeQueryParameters, string, PathMatch, bool)"/> or <see cref="ContentTypeQueryParametersExtensions.ForWebsite(ContentEngine.ContentTypeQueryParameters, string, PathMatch[], bool)"/>
    /// with 'includeUrlPath' flag set to <c>true</c>.
    /// </para>
    /// Use <see cref="ContentTypeQueryParametersUrlExtensions.UrlPathColumns(ContentEngine.ContentTypeQueryParameters)"/> to add all required columns to the query to construct the URL.
    /// </remarks>
    public async Task<WebPageUrl> Retrieve(string webPageUrlPath, string webPageTreePath, int websiteChannelId, string languageName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await webPageUrlRetriever.Retrieve(webPageUrlPath, webPageTreePath, websiteChannelId, languageName, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            string message = $"{ex.Message}\r\ntree path: {webPageTreePath},\r\nweb page URL path: {webPageUrlPath},\r\nwebsite channel id: {websiteChannelId}\r\nlanguage: {languageName}";
            eventLogService.LogError(ex.Source, "Retrieve URL", message);
            return EmptyUrl;
        }
    }

    /// <summary>
    /// Retrieves the URL for a web page language variant identified by <paramref name="webPageItemId"/> and <paramref name="languageName"/>.
    /// </summary>
    /// <param name="webPageItemId">Web page item identifier.</param>
    /// <param name="languageName">Language code.</param>
    /// <param name="forPreview">Indicates whether the latest version of the URL should be retrieved. The default value is <c>false</c>.</param>
    /// <param name="cancellationToken">Cancellation instruction.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="languageName"/> is null.</exception>
    /// <returns>
    /// Empty URL when web page identified by <paramref name="webPageItemId"/> or when content language identified by <paramref name="languageName"/> does not exists.
    /// </returns>
    /// <remarks>The method caches the retrieved URL unless retrieving the latest version of the URL for preview.</remarks>
    public async Task<WebPageUrl> Retrieve(int webPageItemId, string languageName, bool forPreview = false, CancellationToken cancellationToken = default)
    {
        try
        {
            return await webPageUrlRetriever.Retrieve(webPageItemId, languageName, forPreview, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            string message = $"{ex.Message}\r\nweb page id: {webPageItemId},\r\nlanguage: {languageName}\r\nfor preview: {forPreview}";
            eventLogService.LogError(ex.Source, "Retrieve URL", message);
            return EmptyUrl;
        }
    }

    /// <summary>
    /// Retrieves the URL for a web page language variant identified by <paramref name="webPageItemGuid"/> and <paramref name="languageName"/>.
    /// </summary>
    /// <param name="webPageItemGuid">Web page item identifier.</param>
    /// <param name="languageName">Language code.</param>
    /// <param name="forPreview">Indicates whether the latest version of the URL should be retrieved. The default value is <c>false</c>.</param>
    /// <param name="cancellationToken">Cancellation instruction.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="languageName"/> is null.</exception>
    /// <returns>
    /// Empty URL when web page identified by <paramref name="webPageItemGuid"/> or when content language identified by <paramref name="languageName"/> does not exists.
    /// </returns>
    /// <remarks>The method caches the retrieved URL unless retrieving the latest version of the URL for preview.</remarks>
    public async Task<WebPageUrl> Retrieve(Guid webPageItemGuid, string languageName, bool forPreview = false, CancellationToken cancellationToken = default)
    {
        try
        {
            return await webPageUrlRetriever.Retrieve(webPageItemGuid, languageName, forPreview, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            string message = $"{ex.Message}\r\nweb page guid: {webPageItemGuid},\r\nlanguage: {languageName}\r\nfor preview: {forPreview}";
            eventLogService.LogError(ex.Source, "Retrieve URL", message);
            return EmptyUrl;
        }
    }

    /// <summary>
    /// Retrieves the URLs for a web page language variants identified by <paramref name="webPageItemGuids"/>, <paramref name="websiteChannelName"/> and <paramref name="languageName"/>.
    /// </summary>
    /// <param name="webPageItemGuids">Web page item identifiers.</param>
    /// <param name="websiteChannelName">Website channel name.</param>
    /// <param name="languageName">Language code.</param>
    /// <param name="forPreview">Indicates whether the latest version of the URL should be retrieved. The default value is <c>false</c>.</param>
    /// <param name="cancellationToken">Cancellation instruction.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="webPageItemGuids"/>, <paramref name="languageName"/> or <paramref name="websiteChannelName"/> is null.</exception>
    /// <returns>
    /// Empty URL when website channel identified by <paramref name="websiteChannelName"/> or when content language identified by <paramref name="languageName"/> does not exists.
    /// </returns>
    /// <remarks>The method doesn't cache the retrieved URLs.</remarks>
    public async Task<IDictionary<Guid, WebPageUrl>> Retrieve(IReadOnlyCollection<Guid> webPageItemGuids, string websiteChannelName, string languageName, bool forPreview = false, CancellationToken cancellationToken = default)
    {
        try
        {
            return await webPageUrlRetriever.Retrieve(webPageItemGuids, websiteChannelName, languageName, forPreview, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            string message = $"{ex.Message}\r\nweb page guids: {string.Join(", ", webPageItemGuids)},\r\nwebsite channel name: {websiteChannelName},\r\nlanguage: {languageName}\r\nfor preview: {forPreview}";
            eventLogService.LogError(ex.Source, "Retrieve URLs", message);
            return webPageItemGuids.ToDictionary(guid => guid, _ => EmptyUrl);
        }
    }

    /// <summary>
    /// Retrieves the URL for a web page language variant identified by <paramref name="webPageTreePath"/>, <paramref name="websiteChannelName"/> and <paramref name="languageName"/>.
    /// </summary>
    /// <param name="webPageTreePath">Web page tree path.</param>
    /// <param name="websiteChannelName">Website channel name.</param>
    /// <param name="languageName">Language code.</param>
    /// <param name="forPreview">Indicates whether the latest version of the URL should be retrieved. The default value is <c>false</c>.</param>
    /// <param name="cancellationToken">Cancellation instruction.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="languageName"/>, <paramref name="webPageTreePath"/> or <paramref name="websiteChannelName"/> is null.</exception>
    /// <returns>
    /// Empty URL when web page identified by <paramref name="webPageTreePath"/> or when website channel identified by <paramref name="websiteChannelName"/> or 
    /// when content language identified by <paramref name="languageName"/> does not exists.
    /// </returns>
    /// <remarks>The method caches the retrieved URL unless retrieving the latest version of the URL for preview.</remarks>
    public async Task<WebPageUrl> Retrieve(string webPageTreePath, string websiteChannelName, string languageName, bool forPreview = false, CancellationToken cancellationToken = default)
    {
        try
        {
            return await webPageUrlRetriever.Retrieve(webPageTreePath, websiteChannelName, languageName, forPreview, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            string message = $"{ex.Message}\r\ntree path: {webPageTreePath},\r\nwebsite channel name: {websiteChannelName},\r\nlanguage: {languageName}\r\nfor preview: {forPreview}";
            eventLogService.LogError(ex.Source, "Retrieve URL", message);
            return EmptyUrl;
        }
    }
}