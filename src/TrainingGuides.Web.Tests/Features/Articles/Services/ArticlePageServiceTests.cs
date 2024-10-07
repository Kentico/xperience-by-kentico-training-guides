using Xunit;
using CMS.Websites;
using Moq;
using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Articles;
using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Tests.Features.Articles.Services;

public class ArticlePageServiceTests
{
    private readonly Mock<ArticlePageService> articlePageServiceMock;
    private readonly Mock<IWebPageUrlRetriever> webPageUrlRetrieverMock;

    private const string ARTICLE_TITLE = "Title";
    private const string ARTICLE_SUMMARY = "Summary";
    private const string ARTICLE_TEXT = "Text";
    private const string ARTICLE_URL = "test";

    private readonly ArticlePageViewModel referenceArticleViewModel;

    public ArticlePageServiceTests()
    {
        webPageUrlRetrieverMock = new Mock<IWebPageUrlRetriever>();
        webPageUrlRetrieverMock.Setup(x => x.Retrieve(It.IsAny<ArticlePage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebPageUrl(ARTICLE_URL));

        articlePageServiceMock = new Mock<ArticlePageService>(webPageUrlRetrieverMock.Object);

        referenceArticleViewModel = new()
        {
            Title = ARTICLE_TITLE,
            Summary = new HtmlString(ARTICLE_SUMMARY),
            Text = new HtmlString(ARTICLE_TEXT),
            Url = ARTICLE_URL
        };
    }

    private ArticlePage BuildSampleArticlePage() => new()
    {
        ArticlePageArticleContent = [new GeneralArticle()
        {
            ArticleSchemaTitle = ARTICLE_TITLE,
            ArticleSchemaSummary = ARTICLE_SUMMARY,
            ArticleSchemaText = ARTICLE_TEXT,
            ArticleSchemaTeaser = []
        }],
        ArticlePagePublishDate = DateTime.Now
    };

    [Fact]
    public async Task GetArticlePageViewModel_ReturnsModel_WithArticleTitleSet()
    {
        var articlePage = BuildSampleArticlePage();
        var articlePageViewModel = await articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Title, articlePageViewModel.Title);
    }

    [Fact]
    public async Task GetArticlePageViewModel_ReturnsModel_WithArticleSummarySet()
    {
        var articlePage = BuildSampleArticlePage();
        var articlePageViewModel = await articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Summary.Value, articlePageViewModel.Summary.Value);
    }

    [Fact]
    public async Task GetArticlePageViewModel_ReturnsModel_WithArticleTextSet()
    {
        var articlePage = BuildSampleArticlePage();
        var articlePageViewModel = await articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Text.Value, articlePageViewModel.Text.Value);
    }

    [Fact]
    public async Task GetArticlePageViewModel_ReturnsModel_WithArticleUrlSet()
    {
        var articlePage = BuildSampleArticlePage();
        var articlePageViewModel = await articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Url, articlePageViewModel.Url);
    }
}