using Xunit;
using CMS.Websites;
using Moq;
using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Articles;
using Microsoft.AspNetCore.Html;
using Kentico.Content.Web.Mvc.Routing;

namespace TrainingGuides.Web.Tests.Features.Articles.Services;

public class ArticlePageServiceTests
{
    private readonly Mock<ArticlePageService> articlePageServiceMock;
    private readonly Mock<IWebPageUrlRetriever> webPageUrlRetrieverMock;
    private readonly Mock<IPreferredLanguageRetriever> preferredLanguageRetrieverMock;

    private const string ARTICLE_TITLE = "Title";
    private const string ARTICLE_SUMMARY = "Summary";
    private const string ARTICLE_TEXT = "Text";
    private const string ARTICLE_URL = "test";
    private const string ARTICLE_ABSOLUTE_URL = "https://test/test";
    private const string LANGUAGE_EN = "en-US";

    private readonly ArticlePageViewModel referenceArticleViewModel;

    public ArticlePageServiceTests()
    {
        webPageUrlRetrieverMock = new Mock<IWebPageUrlRetriever>();
        webPageUrlRetrieverMock.Setup(x => x.Retrieve(It.IsAny<ArticlePage>(), LANGUAGE_EN, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebPageUrl(ARTICLE_URL, ARTICLE_ABSOLUTE_URL));

        preferredLanguageRetrieverMock = new Mock<IPreferredLanguageRetriever>();
        preferredLanguageRetrieverMock.Setup(x => x.Get()).Returns(LANGUAGE_EN);

        articlePageServiceMock = new Mock<ArticlePageService>(
            webPageUrlRetrieverMock.Object,
            preferredLanguageRetrieverMock.Object);

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
