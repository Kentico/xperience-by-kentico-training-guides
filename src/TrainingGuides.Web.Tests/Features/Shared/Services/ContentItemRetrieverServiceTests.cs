using CMS.ContentEngine;
using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Moq;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Shared.Services;

public class ContentItemRetrieverServiceTests
{
    private readonly ContentItemRetrieverService contentItemRetrieverService;
    private readonly Mock<IContentRetriever> contentRetrieverMock;

    public ContentItemRetrieverServiceTests()
    {
        contentRetrieverMock = new Mock<IContentRetriever>();
        var webSiteChannelContextMock = new Mock<IWebsiteChannelContext>();
        var preferredLanguageRetrieverMock = new Mock<IPreferredLanguageRetriever>();
        var contentQueryExecutorMock = new Mock<IContentQueryExecutor>();

        contentItemRetrieverService = new ContentItemRetrieverService(
            contentRetrieverMock.Object,
            webSiteChannelContextMock.Object,
            preferredLanguageRetrieverMock.Object,
            contentQueryExecutorMock.Object);
    }

    /// <summary>
    /// Sets up the mocked <see cref="IContentRetriever"/> to capture the <see cref="RetrieveContentOfReusableSchemasParameters"/>
    /// passed to it, so tests can assert on the values the service builds.
    /// </summary>
    private Func<RetrieveContentOfReusableSchemasParameters?> SetupParameterCapture()
    {
        RetrieveContentOfReusableSchemasParameters? captured = null;

        contentRetrieverMock
            .Setup(retriever => retriever.RetrieveContentOfReusableSchemas<IContentItemFieldsSource>(
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<RetrieveContentOfReusableSchemasParameters>(),
                It.IsAny<Action<RetrieveContentOfReusableSchemasQueryParameters>>(),
                It.IsAny<RetrievalCacheSettings>(),
                It.IsAny<Func<IContentQueryDataContainer, IContentItemFieldsSource, Task<IContentItemFieldsSource>>>(),
                It.IsAny<CancellationToken>()))
            .Callback<
                IEnumerable<string>,
                RetrieveContentOfReusableSchemasParameters,
                Action<RetrieveContentOfReusableSchemasQueryParameters>,
                RetrievalCacheSettings,
                Func<IContentQueryDataContainer, IContentItemFieldsSource, Task<IContentItemFieldsSource>>,
                CancellationToken>(
                (schemaNames, parameters, additionalQueryConfiguration, cacheSettings, configureModel, cancellationToken)
                    => captured = parameters)
            .ReturnsAsync(Enumerable.Empty<IContentItemFieldsSource>());

        return () => captured;
    }

    [Fact]
    public async Task RetrieveContentItemsBySchemas_IncludeContentTypeFieldsNotSpecified_DefaultsToTrue()
    {
        var getCapturedParameters = SetupParameterCapture();

        await contentItemRetrieverService.RetrieveContentItemsBySchemas<IContentItemFieldsSource>(
            ["SomeSchema"],
            query => { });

        var capturedParameters = getCapturedParameters();
        Assert.NotNull(capturedParameters);
        Assert.True(capturedParameters!.IncludeContentTypeFields);
    }

    [Fact]
    public async Task RetrieveContentItemsBySchemas_IncludeContentTypeFieldsSetFalse_PropagatesFalse()
    {
        var getCapturedParameters = SetupParameterCapture();

        await contentItemRetrieverService.RetrieveContentItemsBySchemas<IContentItemFieldsSource>(
            ["SomeSchema"],
            query => { },
            includeContentTypeFields: false);

        var capturedParameters = getCapturedParameters();
        Assert.NotNull(capturedParameters);
        Assert.False(capturedParameters!.IncludeContentTypeFields);
    }

    [Fact]
    public async Task RetrieveContentItemsBySchemaAndTags_IncludeContentTypeFieldsNotSpecified_DefaultsToTrue()
    {
        var getCapturedParameters = SetupParameterCapture();

        await contentItemRetrieverService.RetrieveContentItemsBySchemaAndTags(
            "SomeSchema",
            "TaxonomyColumn",
            [Guid.NewGuid()]);

        var capturedParameters = getCapturedParameters();
        Assert.NotNull(capturedParameters);
        Assert.True(capturedParameters!.IncludeContentTypeFields);
    }
}
