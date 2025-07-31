using CMS.Websites;
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
    private readonly Mock<IStringLocalizer<SharedResources>> stringLocalizerMock;
    private readonly Mock<IHttpRequestService> httpRequestServiceMock;
    private readonly Mock<ArticlePageService> articlePageServiceMock;

    private const string ARTICLE_TITLE = "Title";
    private const string ARTICLE_SUMMARY = "Summary";
    private const string ARTICLE_TEXT = "Text";
    private const string ARTICLE_URL = "test";

    private readonly ArticlePageViewModel referenceArticleViewModel;

    public ArticlePageServiceTests()
    {
        stringLocalizerMock = new Mock<IStringLocalizer<SharedResources>>();
        httpRequestServiceMock = new Mock<IHttpRequestService>();

        articlePageServiceMock = new Mock<ArticlePageService>(
            stringLocalizerMock.Object,
            httpRequestServiceMock.Object);

        // Mock the GetArticlePageRelativeUrl method to avoid IoC container issues
        articlePageServiceMock.Setup(x => x.GetArticlePageRelativeUrl(It.IsAny<ArticlePage>()))
            .Returns(ARTICLE_URL);

        // Call the real implementation for GetArticlePageViewModel
        articlePageServiceMock.CallBase = true;

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
    public void GetArticlePageViewModel_IfArticlePageNull_ReturnsEmptyModel()
    {
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(null);
        Assert.Equivalent(new ArticlePageViewModel(), articlePageViewModel);
    }

    // old Article type, not using Reusable Field Schema
    [Fact]
    public void GetArticlePageViewModel_ForOldArticle_ReturnsModel_WithArticleTitleSet()
    {
        var articlePage = BuildSampleArticlePageWithOldArticle();
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Title, articlePageViewModel.Title);
    }

    [Fact]
    public void GetArticlePageViewModel_ForOldArticle_ReturnsModel_WithArticleSummarySet()
    {
        var articlePage = BuildSampleArticlePageWithOldArticle();
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.SummaryHtml.Value, articlePageViewModel.SummaryHtml.Value);
    }

    [Fact]
    public void GetArticlePageViewModel_ForOldArticle_ReturnsModel_WithArticleTextSet()
    {
        var articlePage = BuildSampleArticlePageWithOldArticle();
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.TextHtml.Value, articlePageViewModel.TextHtml.Value);
    }

    [Fact]
    public void GetArticlePageViewModel_ForOldArticle_ReturnsModel_WithArticleUrlSet()
    {
        var articlePage = BuildSampleArticlePageWithOldArticle();
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Url, articlePageViewModel.Url);
    }

    // new General article type implementing the reusable field schema

    [Fact]
    public void GetArticlePageViewModel_ForNewRFSArticle_ReturnsModel_WithArticleTitleSet()
    {
        var articlePage = BuildSampleArticlePageWithNewRFSArticle();
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Title, articlePageViewModel.Title);
    }

    [Fact]
    public void GetArticlePageViewModel_ForNewRFSArticle_ReturnsModel_WithArticleSummarySet()
    {
        var articlePage = BuildSampleArticlePageWithNewRFSArticle();
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.SummaryHtml.Value, articlePageViewModel.SummaryHtml.Value);
    }

    [Fact]
    public void GetArticlePageViewModel_ForNewRFSArticle_ReturnsModel_WithArticleTextSet()
    {
        var articlePage = BuildSampleArticlePageWithNewRFSArticle();
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.TextHtml.Value, articlePageViewModel.TextHtml.Value);
    }

    [Fact]
    public void GetArticlePageViewModel_ForNewRFSArticle_ReturnsModel_WithArticleUrlSet()
    {
        var articlePage = BuildSampleArticlePageWithNewRFSArticle();
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Url, articlePageViewModel.Url);
    }
}