using System.Collections.ObjectModel;
using System.Text.Json;
using CMS.ContentEngine;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TrainingGuides.Web.Features.ResponsiveImages;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ResponsiveImages;

[Collection(ServiceContainerCollection.NAME)]
public class ResponsiveImageTagHelperTests
{
    private const string ORIGINAL_URL = "~/getContentAsset/item/field/photo.jpg";

    [Fact]
    public void Process_SuppressesOutput_WhenAssetIsNull()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper { Asset = null };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Null(output.TagName);
    }

    [Fact]
    public void Process_SuppressesOutput_WhenAssetIsNotAnImage()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper { Asset = CreateAsset(null, null, extension: ".pdf") };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Null(output.TagName);
    }

    [Fact]
    public void Process_RendersOriginal_WhenAssetHasNoVariants()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper { Asset = CreateAsset(1433, 955) };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal("img", output.TagName);
        Assert.Equal(ORIGINAL_URL, GetAttribute(output, "src"));
        Assert.Null(GetAttribute(output, "srcset"));
        Assert.Null(GetAttribute(output, "sizes"));
        Assert.Equal("1433", GetAttribute(output, "width"));
        Assert.Equal("955", GetAttribute(output, "height"));
    }

    [Fact]
    public void Process_RendersOriginal_WhenAssetHasNoVariantDictionaries()
    {
        // Arrange
        var asset = new ContentItemAsset { Url = ORIGINAL_URL, Metadata = CreateMetadata(1433, 955) };
        var tagHelper = new ResponsiveImageTagHelper { Asset = asset };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal(ORIGINAL_URL, GetAttribute(output, "src"));
        Assert.Null(GetAttribute(output, "srcset"));
    }

    [Fact]
    public void Process_RendersLadderNarrowestFirst()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper
        {
            Asset = CreateAsset(3000, 2000, LadderVariants()),
            Sizes = "(min-width: 768px) 50vw, 100vw"
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal(
            $"{VariantUrl(ImageVariants.SCALE_W480)} 480w, "
            + $"{VariantUrl(ImageVariants.SCALE_W828)} 828w, "
            + $"{VariantUrl(ImageVariants.SCALE_W1280)} 1280w, "
            + $"{VariantUrl(ImageVariants.SCALE_W1600)} 1600w, "
            + $"{VariantUrl(ImageVariants.SCALE_W1920)} 1920w, "
            + $"{VariantUrl(ImageVariants.SCALE_W2560)} 2560w",
            GetAttribute(output, "srcset"));
        Assert.Equal(VariantUrl(ImageVariants.SCALE_W2560), GetAttribute(output, "src"));
        Assert.Equal("(min-width: 768px) 50vw, 100vw", GetAttribute(output, "sizes"));
        Assert.Equal("3000", GetAttribute(output, "width"));
        Assert.Equal("2000", GetAttribute(output, "height"));
    }

    [Fact]
    public void Process_DropsUpscaledRungsAndClosesWithOriginal()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper { Asset = CreateAsset(1433, 955, LadderVariants()) };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal(
            $"{VariantUrl(ImageVariants.SCALE_W480)} 480w, "
            + $"{VariantUrl(ImageVariants.SCALE_W828)} 828w, "
            + $"{VariantUrl(ImageVariants.SCALE_W1280)} 1280w, "
            + $"{ORIGINAL_URL} 1433w",
            GetAttribute(output, "srcset"));
        Assert.Equal(ORIGINAL_URL, GetAttribute(output, "src"));
    }

    [Fact]
    public void Process_ClosesWithRung_WhenOriginalMatchesRungWidth()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper { Asset = CreateAsset(1920, 1280, LadderVariants()) };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal(
            $"{VariantUrl(ImageVariants.SCALE_W480)} 480w, "
            + $"{VariantUrl(ImageVariants.SCALE_W828)} 828w, "
            + $"{VariantUrl(ImageVariants.SCALE_W1280)} 1280w, "
            + $"{VariantUrl(ImageVariants.SCALE_W1600)} 1600w, "
            + $"{VariantUrl(ImageVariants.SCALE_W1920)} 1920w",
            GetAttribute(output, "srcset"));
        Assert.Equal(VariantUrl(ImageVariants.SCALE_W1920), GetAttribute(output, "src"));
    }

    [Fact]
    public void Process_ClosesWithOriginalOnly_WhenEveryRungIsWider()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper
        {
            Asset = CreateAsset(400, 267, LadderVariants()),
            Sizes = "50vw"
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal($"{ORIGINAL_URL} 400w", GetAttribute(output, "srcset"));
        Assert.Equal(ORIGINAL_URL, GetAttribute(output, "src"));
        Assert.Equal("50vw", GetAttribute(output, "sizes"));
        Assert.Equal("400", GetAttribute(output, "width"));
        Assert.Equal("267", GetAttribute(output, "height"));
    }

    [Fact]
    public void Process_RendersFixedVariant_WhenAssetHasIt()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper
        {
            Asset = CreateAsset(3000, 2000, [.. LadderVariants(), (ImageVariants.CARD_800X600, 800, 600)]),
            Variant = ImageVariants.CARD_800X600
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal(VariantUrl(ImageVariants.CARD_800X600), GetAttribute(output, "src"));
        Assert.Equal("800", GetAttribute(output, "width"));
        Assert.Equal("600", GetAttribute(output, "height"));
        Assert.Null(GetAttribute(output, "srcset"));
        Assert.Null(GetAttribute(output, "sizes"));
    }

    [Fact]
    public void Process_FallsBackToLadder_WhenVariantIsUnknown()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper
        {
            Asset = CreateAsset(3000, 2000, [.. LadderVariants(), (ImageVariants.CARD_800X600, 800, 600)]),
            Variant = "RetiredHero"
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.StartsWith($"{VariantUrl(ImageVariants.SCALE_W480)} 480w, ", GetAttribute(output, "srcset"));
        Assert.Equal(VariantUrl(ImageVariants.SCALE_W2560), GetAttribute(output, "src"));
        Assert.Equal("3000", GetAttribute(output, "width"));
        Assert.Equal("2000", GetAttribute(output, "height"));
    }

    // VariantUrls is case-sensitive, so a wrong-case code name finds no URL even though Metadata.Variants ignores case
    [Fact]
    public void Process_FallsBackToLadder_WhenVariantCaseDiffers()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper
        {
            Asset = CreateAsset(3000, 2000, [.. LadderVariants(), (ImageVariants.CARD_800X600, 800, 600)]),
            Variant = "card800x600"
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.StartsWith($"{VariantUrl(ImageVariants.SCALE_W480)} 480w, ", GetAttribute(output, "srcset"));
        Assert.Equal(VariantUrl(ImageVariants.SCALE_W2560), GetAttribute(output, "src"));
        Assert.Equal("3000", GetAttribute(output, "width"));
        Assert.Equal("2000", GetAttribute(output, "height"));
    }

    [Fact]
    public void Process_OmitsDimensions_WhenMetadataHasNoSize()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper { Asset = CreateAsset(null, null, LadderVariants()) };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Null(GetAttribute(output, "width"));
        Assert.Null(GetAttribute(output, "height"));
        Assert.Equal(
            $"{VariantUrl(ImageVariants.SCALE_W480)} 480w, "
            + $"{VariantUrl(ImageVariants.SCALE_W828)} 828w, "
            + $"{VariantUrl(ImageVariants.SCALE_W1280)} 1280w, "
            + $"{VariantUrl(ImageVariants.SCALE_W1600)} 1600w, "
            + $"{VariantUrl(ImageVariants.SCALE_W1920)} 1920w, "
            + $"{VariantUrl(ImageVariants.SCALE_W2560)} 2560w",
            GetAttribute(output, "srcset"));
    }

    [Fact]
    public void Process_OmitsDimensions_WhenFixedVariantHasNoSize()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper
        {
            Asset = CreateAsset(3000, 2000, [(ImageVariants.CARD_800X600, null, null)]),
            Variant = ImageVariants.CARD_800X600
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal(VariantUrl(ImageVariants.CARD_800X600), GetAttribute(output, "src"));
        Assert.Null(GetAttribute(output, "width"));
        Assert.Null(GetAttribute(output, "height"));
    }

    [Fact]
    public void Process_AlwaysRendersAltAndLoading()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper { Asset = CreateAsset(null, null) };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal(string.Empty, GetAttribute(output, "alt"));
        Assert.Equal("lazy", GetAttribute(output, "loading"));
    }

    /// <summary>
    /// Creates the scale variants of the width ladder, each stored at its nominal width in a 3:2 aspect ratio.
    /// </summary>
    private static (string Name, int? W, int? H)[] LadderVariants() =>
    [
        (ImageVariants.SCALE_W480, 480, 320),
        (ImageVariants.SCALE_W828, 828, 552),
        (ImageVariants.SCALE_W1280, 1280, 853),
        (ImageVariants.SCALE_W1600, 1600, 1067),
        (ImageVariants.SCALE_W1920, 1920, 1280),
        (ImageVariants.SCALE_W2560, 2560, 1707)
    ];

    /// <summary>
    /// Creates an asset whose original has the given size, with a URL and stored metadata for each variant.
    /// </summary>
    private static ContentItemAsset CreateAsset(int? width, int? height, (string Name, int? W, int? H)[]? variants = null, string extension = ".jpg")
    {
        variants ??= [];

        return new ContentItemAsset
        {
            Url = ORIGINAL_URL,
            Metadata = CreateMetadata(width, height, variants, extension),
            VariantUrls = new ReadOnlyDictionary<string, string>(
                variants.ToDictionary(variant => variant.Name, variant => VariantUrl(variant.Name)))
        };
    }

    /// <summary>
    /// Deserializes asset metadata from JSON shaped like the value stored in a content item, as the product does,
    /// because <see cref="ContentItemAssetMetadata.Variants"/> has no public setter.
    /// The <c>Variants</c> property is left out of the JSON when <paramref name="variants"/> is <c>null</c>.
    /// </summary>
    private static ContentItemAssetMetadata CreateMetadata(int? width, int? height, (string Name, int? W, int? H)[]? variants = null, string extension = ".jpg")
    {
        var stored = new Dictionary<string, object?>
        {
            ["Name"] = "photo.jpg",
            ["Extension"] = extension,
            ["Width"] = width,
            ["Height"] = height
        };

        if (variants is not null)
        {
            stored["Variants"] = variants.ToDictionary(
                variant => variant.Name,
                variant => new { Name = $"photo_{variant.Name}.jpg", Extension = ".jpg", Width = variant.W, Height = variant.H });
        }

        return JsonSerializer.Deserialize<ContentItemAssetMetadata>(JsonSerializer.Serialize(stored))!;
    }

    private static string VariantUrl(string name) => $"~/getContentAsset/item/field/{name}/photo_{name}.jpg";

    private static TagHelperOutput Process(ResponsiveImageTagHelper tagHelper)
    {
        var context = new TagHelperContext([], new Dictionary<object, object>(), "unique-id");
        var output = new TagHelperOutput(
            "tg-responsive-image",
            [],
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        tagHelper.Process(context, output);

        return output;
    }

    private static string? GetAttribute(TagHelperOutput output, string name) =>
        output.Attributes.TryGetAttribute(name, out var attribute) ? attribute.Value?.ToString() : null;
}
