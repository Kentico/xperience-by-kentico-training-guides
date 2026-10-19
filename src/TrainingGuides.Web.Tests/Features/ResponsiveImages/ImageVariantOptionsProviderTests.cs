using CMS.ContentEngine;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.ResponsiveImages;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ResponsiveImages;

public class ImageVariantOptionsProviderTests
{
    private static ImageVariantOptionsProvider.VariantDefinition SmartCrop(string codeName,
        string displayName, int width, int height) =>
        new(codeName, displayName, width, height, MediaTransformationType.SmartCrop);

    private static ImageVariantOptionsProvider.VariantDefinition Scaled(string codeName,
        string displayName, int width, int height) =>
        new(codeName, displayName, width, height, MediaTransformationType.Scale);

    private static List<DropDownOptionItem> Build(
        params ImageVariantOptionsProvider.VariantDefinition[] definitions) =>
        ImageVariantOptionsProvider.BuildOptionItems(definitions).ToList();

    [Fact]
    public void BuildOptionItems_ListsAutomaticOptionFirstWithEmptyValue()
    {
        // Arrange
        var definition = Scaled("ScaleW480", "Responsive XS", 480, 0);

        // Act
        var options = Build(definition);

        // Assert
        Assert.Equal(2, options.Count);
        Assert.Equal(string.Empty, options[0].Value);
        Assert.Equal(ImageVariantOptionsProvider.AUTOMATIC_OPTION_TEXT, options[0].Text);
    }

    [Fact]
    public void BuildOptionItems_ListsOnlyAutomaticOption_WhenThereAreNoDefinitions()
    {
        // Act
        var options = Build();

        // Assert
        var option = Assert.Single(options);
        Assert.Equal(string.Empty, option.Value);
    }

    [Fact]
    public void BuildOptionItems_OrdersSmartCropsBeforeScaled_ByWidthThenHeight()
    {
        // Arrange
        var definitions = new[]
        {
            Scaled("ScaleW1280", "Responsive M", 1280, 0),
            SmartCrop("HeroWide1920x1080", "Hero, wide screens", 1920, 1080),
            Scaled("ScaleW480", "Responsive XS", 480, 0),
            SmartCrop("Card800x600", "Card 4:3", 800, 600),
            SmartCrop("Card800x450", "Card 16:9", 800, 450),
            SmartCrop("Thumbnail256x256", "Thumbnail", 256, 256)
        };

        // Act
        var options = Build(definitions);

        // Assert
        Assert.Equal(
            [
                "", "Thumbnail256x256", "Card800x450", "Card800x600", "HeroWide1920x1080",
                "ScaleW480", "ScaleW1280"
            ],
            options.Select(option => option.Value));
    }

    [Fact]
    public void BuildOptionItems_CaptionsBothSides_WhenDefinitionIsSmartCrop()
    {
        // Arrange
        var definition = SmartCrop("Card800x600", "Card 4:3", 800, 600);

        // Act
        var options = Build(definition);

        // Assert
        Assert.Equal("Card 4:3 (800 × 600 px · Smart crop)", options[1].Text);
    }

    [Fact]
    public void BuildOptionItems_CaptionsWidthOnly_WhenScaledHeightIsAutomatic()
    {
        // Arrange
        var definition = Scaled("ScaleW480", "Responsive XS", 480, 0);

        // Act
        var options = Build(definition);

        // Assert
        Assert.Equal("Responsive XS (480 px · Scaled)", options[1].Text);
    }

    [Fact]
    public void BuildOptionItems_CaptionsHeightOnly_WhenScaledWidthIsAutomatic()
    {
        // Arrange
        var definition = Scaled("ScaleH600", "Banner height", 0, 600);

        // Act
        var options = Build(definition);

        // Assert
        Assert.Equal("Banner height (600 px · Scaled)", options[1].Text);
    }

    [Fact]
    public void BuildOptionItems_CaptionsBothSides_WhenScaledDefinitionHasBoth()
    {
        // Arrange
        var definition = Scaled("Box1200x800", "Bounding box", 1200, 800);

        // Act
        var options = Build(definition);

        // Assert
        Assert.Equal("Bounding box (1200 × 800 px · Scaled)", options[1].Text);
    }
}
