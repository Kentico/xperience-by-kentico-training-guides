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
            SummaryHtml = new HtmlString(ARTICLE_SUMMARY),
            TextHtml = new HtmlString(ARTICLE_TEXT),
            Url = ARTICLE_URL
        };
    }

    private ArticlePage BuildSampleArticlePage() => new()
    {
        ArticlePageContent = [new Article()
            {
                ArticleTitle = ARTICLE_TITLE,
                ArticleSummary = ARTICLE_SUMMARY,
                ArticleText = ARTICLE_TEXT,
                ArticleTeaser = []
            }],
        ArticlePagePublishDate = DateTime.Now
    };

    [Fact]
    public async Task GetArticlePageViewModel_IfArticlePageNull_ReturnsEmptyModel()
    {
        var articlePageViewModel = await articlePageServiceMock.Object.GetArticlePageViewModel(null);
        Assert.Equivalent(new ArticlePageViewModel(), articlePageViewModel);
    }

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
        Assert.Equal(referenceArticleViewModel.SummaryHtml.Value, articlePageViewModel.SummaryHtml.Value);
    }

    [Fact]
    public async Task GetArticlePageViewModel_ReturnsModel_WithArticleTextSet()
    {
        var articlePage = BuildSampleArticlePage();
        var articlePageViewModel = await articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.TextHtml.Value, articlePageViewModel.TextHtml.Value);
    }

    [Fact]
    public async Task GetArticlePageViewModel_ReturnsModel_WithArticleUrlSet()
    {
        var articlePage = BuildSampleArticlePage();
        var articlePageViewModel = await articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Url, articlePageViewModel.Url);
    }
}