using Xunit;
using TrainingGuides.Web.Features.Gallery.Widgets.GalleryWidget;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Tests.Features.Gallery.Widgets.GalleryWidget;

public class GalleryWidgetViewModelTests
{
    private readonly GalleryWidgetViewModel emptyWidgetViewModel;

    public GalleryWidgetViewModelTests()
    {
        emptyWidgetViewModel = new();
    }

    [Fact]
    public void WhenInitialized_TitleIsEmpty() => Assert.Equal(string.Empty, emptyWidgetViewModel.Title);

    [Fact]
    public void WhenInitialized_ImagesIsEmptyList() => Assert.Empty(emptyWidgetViewModel.Images);

    [Fact]
    public void IsMisconfigured_When_ImagesEmpty_ReturnsTrue() => Assert.True(emptyWidgetViewModel.IsMisconfigured);

    [Fact]
    public void IsMisconfigured_When_ImagesContainItems_ReturnsFalse()
    {
        var widgetViewModel = new GalleryWidgetViewModel();
        widgetViewModel.Images.Add(new AssetViewModel());

        Assert.False(widgetViewModel.IsMisconfigured);
    }
}