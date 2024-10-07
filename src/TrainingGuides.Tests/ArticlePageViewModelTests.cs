using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Articles;
using TrainingGuides.Web.Features.Shared.Models;
using Xunit;
using Xunit.Abstractions;

namespace TrainingGuides.Web.Tests;

public class ArticlePageViewModelTests
{
    private readonly ArticlePageViewModel viewModel = new();
    private readonly ITestOutputHelper output;

    public ArticlePageViewModelTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void ViewModel_Initialized_TitleIsEmpty()
    {
        ArticlePageViewModel referenceViewModel = new()
        {
            Title = string.Empty,
            Summary = HtmlString.Empty,
            Text = HtmlString.Empty,
            TeaserImage = new AssetViewModel(),
            Url = string.Empty
        };
        Assert.Equal(referenceViewModel.Title, viewModel.Title);
        output.WriteLine("Title initialized.");
    }

    [Fact]
    public void ViewModel_Initialized_TeaserIsNull()
    {
        Assert.Null(viewModel.TeaserImage);
        output.WriteLine("Teaser is null.");
    }
}