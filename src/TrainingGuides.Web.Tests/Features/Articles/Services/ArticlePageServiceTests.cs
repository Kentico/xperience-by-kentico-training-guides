using Xunit;
using Moq;
using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Articles;
using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Tests.Features.Articles.Services;

public class ArticlePageServiceTests
{
    private readonly Mock<ArticlePageService> articlePageServiceMock;

    private const string ARTICLE_TITLE = "Title";
    private const string ARTICLE_SUMMARY = "Summary";
    private const string ARTICLE_TEXT = "Text";
    private const string ARTICLE_URL = "test";

    private readonly ArticlePageViewModel referenceArticleViewModel;

    public ArticlePageServiceTests()
    {
        articlePageServiceMock = new Mock<ArticlePageService> { CallBase = true };

        // Mock the GetArticlePageRelativeUrl method to return our test URL
        articlePageServiceMock.Setup(x => x.GetArticlePageRelativeUrl(It.IsAny<ArticlePage>()))
            .Returns(ARTICLE_URL);

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
    public void GetArticlePageViewModel_IfArticlePageNull_ReturnsEmptyModel()
    {
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(null);
        Assert.Equivalent(new ArticlePageViewModel(), articlePageViewModel);
    }

    [Fact]
    public void GetArticlePageViewModel_ReturnsModel_WithArticleTitleSet()
    {
        var articlePage = BuildSampleArticlePage();
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Title, articlePageViewModel.Title);
    }

    [Fact]
    public void GetArticlePageViewModel_ReturnsModel_WithArticleSummarySet()
    {
        var articlePage = BuildSampleArticlePage();
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.SummaryHtml.Value, articlePageViewModel.SummaryHtml.Value);
    }

    [Fact]
    public void GetArticlePageViewModel_ReturnsModel_WithArticleTextSet()
    {
        var articlePage = BuildSampleArticlePage();
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.TextHtml.Value, articlePageViewModel.TextHtml.Value);
    }

    [Fact]
    public void GetArticlePageViewModel_ReturnsModel_WithArticleUrlSet()
    {
        var articlePage = BuildSampleArticlePage();
        var articlePageViewModel = articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);
        Assert.Equal(referenceArticleViewModel.Url, articlePageViewModel.Url);
    }

    [Fact]
    public void GetArticlePageRelativeUrl_IsCalledCorrectly()
    {
        var articlePage = BuildSampleArticlePage();
        articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);

        // Verify that GetArticlePageRelativeUrl was called with the correct article page
        articlePageServiceMock.Verify(x => x.GetArticlePageRelativeUrl(articlePage), Times.Once);
    }

    [Fact]
    public void GetArticlePageRelativeUrl_IsNotCalledWhenArticlePageIsNull()
    {
        articlePageServiceMock.Object.GetArticlePageViewModel(null);

        // Verify that GetArticlePageRelativeUrl was not called when article page is null
        articlePageServiceMock.Verify(x => x.GetArticlePageRelativeUrl(It.IsAny<ArticlePage>()), Times.Never);
    }
}
