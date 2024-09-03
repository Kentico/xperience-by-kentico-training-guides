using Xunit;
using CMS.Websites;
using Moq;
using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Articles;
using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Tests;

public class ArticlePageServiceTests
{
    private readonly Mock<ArticlePageService> articlePageServiceMock;
    private readonly Mock<IWebPageUrlRetriever> webPageUrlRetrieverMock;

    public ArticlePageServiceTests()
    {
        webPageUrlRetrieverMock = new Mock<IWebPageUrlRetriever>();
        articlePageServiceMock = new Mock<ArticlePageService>(webPageUrlRetrieverMock.Object);

        webPageUrlRetrieverMock.Setup(x => x.Retrieve(It.IsAny<ArticlePage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebPageUrl("test"));
    }

    [Fact]
    public async Task GetArticlePageViewModel_ShouldReturnArticleWithUrl()
    {
        ArticlePageViewModel referenceViewModel = new()
        {
            Title = string.Empty,
            Summary = HtmlString.Empty,
            Text = HtmlString.Empty,
            Url = "test"
        };

        var articlePage = new ArticlePage() { ArticlePageContent = [new Article() { ArticleTeaser = Enumerable.Empty<Asset>() }], ArticlePagePublishDate = DateTime.Now };
        var viewModelWithUrl = await articlePageServiceMock.Object.GetArticlePageViewModel(articlePage);

        Assert.Equal(referenceViewModel.Url, viewModelWithUrl.Url);
    }
}