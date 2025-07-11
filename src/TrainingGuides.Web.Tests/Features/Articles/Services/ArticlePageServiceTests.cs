using CMS.Websites;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Localization;
using Moq;
using TrainingGuides.Web.Features.Articles;
using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Articles.Services;

public class ArticlePageServiceTests
{
    private readonly Mock<IWebPageUrlRetriever> webPageUrlRetrieverMock;
    private readonly Mock<IStringLocalizer<SharedResources>> stringLocalizerMock;
    private readonly Mock<IPreferredLanguageRetriever> preferredLanguageRetrieverMock;
    private readonly Mock<IHttpRequestService> httpRequestServiceMock;
    private readonly ArticlePageService articlePageService;

    private const string ARTICLE_TITLE = "Title";
    private const string ARTICLE_SUMMARY = "Summary";
    private const string ARTICLE_TEXT = "Text";
    private const string ARTICLE_URL = "test";

    private readonly ArticlePageViewModel referenceArticleViewModel;

    public ArticlePageServiceTests()
    {
        webPageUrlRetrieverMock = new Mock<IWebPageUrlRetriever>();
        webPageUrlRetrieverMock.Setup(x => x.Retrieve(It.IsAny<ArticlePage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebPageUrl(relativePath: ARTICLE_URL, absoluteUrl: ARTICLE_URL));

        stringLocalizerMock = new Mock<IStringLocalizer<SharedResources>>();

        preferredLanguageRetrieverMock = new Mock<IPreferredLanguageRetriever>();
        preferredLanguageRetrieverMock.Setup(x => x.Get()).Returns("en");

        httpRequestServiceMock = new Mock<IHttpRequestService>();

        articlePageService = new ArticlePageService(
            webPageUrlRetrieverMock.Object,
            stringLocalizerMock.Object,
            preferredLanguageRetrieverMock.Object,
            httpRequestServiceMock.Object);

        referenceArticleViewModel = new()
        {
            Title = ARTICLE_TITLE,
            SummaryHtml = new HtmlString(ARTICLE_SUMMARY),
            TextHtml = new HtmlString(ARTICLE_TEXT),
            Url = ARTICLE_URL
        };
    }

    private ArticlePage BuildSampleArticlePageWithOldArticle() => new()
    {
        ArticlePageContent = [new Article()
        {
            ArticleTitle = ARTICLE_TITLE,
            ArticleSummary = ARTICLE_SUMMARY,
            ArticleText = ARTICLE_TEXT,
            ArticleTeaser = []
        }],
        ArticlePageArticleContent = [],
        ArticlePagePublishDate = DateTime.Now,
        SystemFields = new WebPageFields { ContentItemIsSecured = false }
    };

    private ArticlePage BuildSampleArticlePageWithNewRFSArticle() => new()
    {
        ArticlePageContent = [],
        ArticlePageArticleContent = [new GeneralArticle()
        {
            ArticleSchemaTitle = ARTICLE_TITLE,
            ArticleSchemaSummary = ARTICLE_SUMMARY,
            ArticleSchemaText = ARTICLE_TEXT,
            ArticleSchemaTeaser = []
        }],
        ArticlePagePublishDate = DateTime.Now,
        SystemFields = new WebPageFields { ContentItemIsSecured = false }
    };

    [Fact]
    public async Task GetArticlePageViewModel_IfArticlePageNull_ReturnsEmptyModel()
    {
        var articlePageViewModel = await articlePageService.GetArticlePageViewModel(null);
        Assert.Equivalent(new ArticlePageViewModel(), articlePageViewModel);
    }

    // old Article type, not using Reusable Field Schema
    [Fact]
    public async Task GetArticlePageViewModel_ForOldArticle_ReturnsModel_WithArticleTitleSet()
    {
        var articlePage = BuildSampleArticlePageWithOldArticle();
        var articlePageViewModel = await articlePageService.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Title, articlePageViewModel.Title);
    }

    [Fact]
    public async Task GetArticlePageViewModel_ForOldArticle_ReturnsModel_WithArticleSummarySet()
    {
        var articlePage = BuildSampleArticlePageWithOldArticle();
        var articlePageViewModel = await articlePageService.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.SummaryHtml.Value, articlePageViewModel.SummaryHtml.Value);
    }

    [Fact]
    public async Task GetArticlePageViewModel_ForOldArticle_ReturnsModel_WithArticleTextSet()
    {
        var articlePage = BuildSampleArticlePageWithOldArticle();
        var articlePageViewModel = await articlePageService.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.TextHtml.Value, articlePageViewModel.TextHtml.Value);
    }

    [Fact]
    public async Task GetArticlePageViewModel_ForOldArticle_ReturnsModel_WithArticleUrlSet()
    {
        var articlePage = BuildSampleArticlePageWithOldArticle();
        var articlePageViewModel = await articlePageService.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Url, articlePageViewModel.Url);
    }

    // new General article type implementing the reusable field schema

    [Fact]
    public async Task GetArticlePageViewModel_ForNewRFSArticle_ReturnsModel_WithArticleTitleSet()
    {
        var articlePage = BuildSampleArticlePageWithNewRFSArticle();
        var articlePageViewModel = await articlePageService.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Title, articlePageViewModel.Title);
    }

    [Fact]
    public async Task GetArticlePageViewModel_ForNewRFSArticle_ReturnsModel_WithArticleSummarySet()
    {
        var articlePage = BuildSampleArticlePageWithNewRFSArticle();
        var articlePageViewModel = await articlePageService.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.SummaryHtml.Value, articlePageViewModel.SummaryHtml.Value);
    }

    [Fact]
    public async Task GetArticlePageViewModel_ForNewRFSArticle_ReturnsModel_WithArticleTextSet()
    {
        var articlePage = BuildSampleArticlePageWithNewRFSArticle();
        var articlePageViewModel = await articlePageService.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.TextHtml.Value, articlePageViewModel.TextHtml.Value);
    }

    [Fact]
    public async Task GetArticlePageViewModel_ForNewRFSArticle_ReturnsModel_WithArticleUrlSet()
    {
        var articlePage = BuildSampleArticlePageWithNewRFSArticle();
        var articlePageViewModel = await articlePageService.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Url, articlePageViewModel.Url);
    }
}