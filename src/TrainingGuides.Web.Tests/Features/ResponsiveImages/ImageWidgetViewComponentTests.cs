using CMS.ContentEngine;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using TrainingGuides.Web.Features.ResponsiveImages.ImageWidget;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ResponsiveImages;

public class ImageWidgetViewComponentTests
{
    private const string ALT_TEXT = "A lizard";
    private const string VARIANT = "Card800x600";

    private readonly Guid assetGuid = Guid.NewGuid();
    private readonly Mock<IContentItemRetrieverService> contentItemRetrieverServiceMock;
    private readonly ImageWidgetViewComponent viewComponent;

    public ImageWidgetViewComponentTests()
    {
        contentItemRetrieverServiceMock = new Mock<IContentItemRetrieverService>();
        viewComponent = new ImageWidgetViewComponent(contentItemRetrieverServiceMock.Object);
    }

    private void SetupAsset(Asset? asset) =>
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveContentItemByGuid<Asset>(assetGuid,
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(asset);

    private ImageWidgetProperties BuildProperties(string? imageVariant = null) => new()
    {
        Image = [new ContentItemReference { Identifier = assetGuid }],
        ImageVariant = imageVariant
    };

    private async Task<ImageWidgetViewModel> InvokeAndGetModel(
        ImageWidgetProperties properties)
    {
        var result = Assert.IsType<ViewViewComponentResult>(
            await viewComponent.InvokeAsync(properties));
        return Assert.IsType<ImageWidgetViewModel>(result.ViewData?.Model);
    }

    [Fact]
    public async Task Invoke_MapsAssetFileAndAltText_WhenAssetIsPicked()
    {
        // Arrange
        var assetFile = new ContentItemAsset();
        SetupAsset(new Asset { AssetFile = assetFile, AssetAltText = ALT_TEXT });

        // Act
        var model = await InvokeAndGetModel(BuildProperties());

        // Assert
        Assert.False(model.IsMisconfigured);
        Assert.NotNull(model.Image);
        Assert.Same(assetFile, model.Image.Asset);
        Assert.Equal(ALT_TEXT, model.Image.AltText);
    }

    [Fact]
    public async Task Invoke_PassesPickedVariant()
    {
        // Arrange
        SetupAsset(new Asset { AssetFile = new(), AssetAltText = ALT_TEXT });

        // Act
        var model = await InvokeAndGetModel(BuildProperties(VARIANT));

        // Assert
        Assert.Equal(VARIANT, model.ImageVariant);
    }

    [Fact]
    public async Task Invoke_PassesNullVariant_WhenAutomaticIsPicked()
    {
        // Arrange
        SetupAsset(new Asset { AssetFile = new(), AssetAltText = ALT_TEXT });

        // Act
        var model = await InvokeAndGetModel(BuildProperties());

        // Assert
        Assert.Null(model.ImageVariant);
    }

    [Fact]
    public async Task Invoke_ReturnsMisconfiguredModel_WhenNoAssetIsPicked()
    {
        // Arrange
        var properties = new ImageWidgetProperties { ImageVariant = VARIANT };

        // Act
        var model = await InvokeAndGetModel(properties);

        // Assert
        Assert.True(model.IsMisconfigured);
        Assert.Null(model.Image);
        contentItemRetrieverServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Invoke_ReturnsMisconfiguredModel_WhenAssetItemIsNotFound()
    {
        // Arrange
        SetupAsset(null);

        // Act
        var model = await InvokeAndGetModel(BuildProperties());

        // Assert
        Assert.True(model.IsMisconfigured);
        Assert.Null(model.Image);
    }

    [Fact]
    public async Task Invoke_ReturnsMisconfiguredModel_WhenAssetFileIsMissing()
    {
        // Arrange
        SetupAsset(new Asset { AssetFile = null, AssetAltText = ALT_TEXT });

        // Act
        var model = await InvokeAndGetModel(BuildProperties());

        // Assert
        Assert.True(model.IsMisconfigured);
        Assert.Null(model.Image);
    }
}
