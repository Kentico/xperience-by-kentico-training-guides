using Xunit;
using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Articles;

namespace TrainingGuides.Web.Tests.Features.Articles;

public class ArticlePageViewModelTests
{
    private readonly ArticlePageViewModel articlePageViewModel;

    public ArticlePageViewModelTests()
    {
        articlePageViewModel = new();
    }

    [Fact]
    public void WhenInitialized_TitleIsEmpty() => Assert.Equal(string.Empty, articlePageViewModel.Title);

    [Fact]
    public void WhenInitialized_SummaryIsEmpty() => Assert.Equal(HtmlString.Empty, articlePageViewModel.Summary);

    [Fact]
    public void WhenInitialized_TextIsEmpty() => Assert.Equal(HtmlString.Empty, articlePageViewModel.Text);

    [Fact]
    public void WhenInitialized_RelatedNewsIsEmpty() => Assert.Empty(articlePageViewModel.RelatedNews);

    [Fact]
    public void WhenInitialized_UrlIsEmpty() => Assert.Equal(string.Empty, articlePageViewModel.Url);
}