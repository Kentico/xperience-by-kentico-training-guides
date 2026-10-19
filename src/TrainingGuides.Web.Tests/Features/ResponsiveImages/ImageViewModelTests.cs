using CMS.ContentEngine;
using TrainingGuides.Web.Features.ResponsiveImages;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ResponsiveImages;

public class ImageViewModelTests
{
    [Fact]
    public void GetViewModel_ReturnsNull_WhenAssetIsNull()
    {
        // Act
        var viewModel = ImageViewModel.GetViewModel(null);

        // Assert
        Assert.Null(viewModel);
    }

    [Fact]
    public void GetViewModel_ReturnsNull_WhenAssetFileIsNull()
    {
        // Arrange
        var asset = new Asset { AssetFile = null, AssetAltText = "A lizard" };

        // Act
        var viewModel = ImageViewModel.GetViewModel(asset);

        // Assert
        Assert.Null(viewModel);
    }

    [Fact]
    public void GetViewModel_ReturnsAssetFileAndAltText_WhenAssetHasFile()
    {
        // Arrange
        var assetFile = new ContentItemAsset();
        var asset = new Asset
        {
            AssetFile = assetFile,
            AssetAltText = "A lizard"
        };

        // Act
        var viewModel = ImageViewModel.GetViewModel(asset);

        // Assert
        Assert.NotNull(viewModel);
        Assert.Same(assetFile, viewModel.Asset);
        Assert.Equal("A lizard", viewModel.AltText);
    }

    [Fact]
    public void GetViewModel_UsesEmptyAltText_WhenAltTextIsNull()
    {
        // Arrange
        var asset = new Asset
        {
            AssetFile = new ContentItemAsset(),
            AssetAltText = null
        };

        // Act
        var viewModel = ImageViewModel.GetViewModel(asset);

        // Assert
        Assert.NotNull(viewModel);
        Assert.Equal(string.Empty, viewModel.AltText);
    }
}
