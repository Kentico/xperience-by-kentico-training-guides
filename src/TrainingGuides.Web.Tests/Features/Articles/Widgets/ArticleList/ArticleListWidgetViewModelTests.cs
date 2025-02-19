using TrainingGuides.Web.Features.Articles;
using TrainingGuides.Web.Features.Articles.Widgets.ArticleList;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetViewModelTests
{
    private readonly ArticleListWidgetViewModel emptyWidgetViewModel;

    public ArticleListWidgetViewModelTests()
    {
        emptyWidgetViewModel = new();
    }

    [Fact]
    public void WhenInitialized_ArticlesIsEmptyList() => Assert.Empty(emptyWidgetViewModel.Articles);

    [Fact]
    public void WhenInitialized_CtaTextIsEmpty() => Assert.Equal(string.Empty, emptyWidgetViewModel.CtaText);

    [Fact]
    public void IsMisconfigured_When_ArticlesEmpty_ReturnsTrue() => Assert.True(emptyWidgetViewModel.IsMisconfigured);

    [Fact]
    public void IsMisconfigured_When_ArticlesContainItems_ReturnsFalse()
    {
        var widgetViewModel = new ArticleListWidgetViewModel();
        widgetViewModel.Articles.Add(new ArticlePageViewModel());

        Assert.False(widgetViewModel.IsMisconfigured);
    }
}