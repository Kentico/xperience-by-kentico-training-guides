using Xunit;
using CMS.Websites;
using Moq;
using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Articles;
using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Tests;

public class ArticlePageServiceTests
{
    private readonly Mock<ArticlePageService> articlePageServiceMock;
    private readonly Mock<IWebPageUrlRetriever> webPageUrlRetrieverMock;

    private const string TEST_URL = "test";

    public ArticlePageServiceTests()
    {
        webPageUrlRetrieverMock = new Mock<IWebPageUrlRetriever>();
        webPageUrlRetrieverMock.Setup(x => x.Retrieve(It.IsAny<ArticlePage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebPageUrl(TEST_URL));

        articlePageServiceMock = new Mock<ArticlePageService>(webPageUrlRetrieverMock.Object);
    }

    [Fact]
    public async Task GetArticlePageViewModel_ShouldReturnArticleWithUrl()
    {
        ArticlePageViewModel referenceViewModel = new()
        {
            Title = string.Empty,
            Summary = HtmlString.Empty,
            Text = HtmlString.Empty,
            Url = TEST_URL
        };

        var articlePage = new ArticlePage()
        {
            ArticlePageArticleContent = [new GeneralArticle()
            {
                ArticleSchemaTitle = "Title",
                ArticleSchemaSummary = "Summary",
                ArticleSchemaText = "Text",
                ArticleSchemaTeaser = []
            }],
            ArticlePagePublishDate = DateTime.Now
        };
        var viewModelWithUrl = await articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);

        Assert.Equal(referenceViewModel.Url, viewModelWithUrl.Url);
    }
}