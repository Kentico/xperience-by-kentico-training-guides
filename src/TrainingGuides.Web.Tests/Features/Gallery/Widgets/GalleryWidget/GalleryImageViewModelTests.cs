using Xunit;
using TrainingGuides.Web.Features.Gallery.Widgets.GalleryWidget;

namespace TrainingGuides.Web.Tests.Features.Gallery.Widgets.GalleryWidget;

public class GalleryImageViewModelTests
{
    private readonly GalleryImageViewModel emptyWidgetViewModel;

    public GalleryImageViewModelTests()
    {
        emptyWidgetViewModel = new();
    }

    [Fact]
    public void WhenInitialized_DescriptionIsEmpty() => Assert.Equal(string.Empty, emptyWidgetViewModel.Description);

    [Fact]
    public void WhenInitialized_ImageIsNull() => Assert.Null(emptyWidgetViewModel.Image);
}