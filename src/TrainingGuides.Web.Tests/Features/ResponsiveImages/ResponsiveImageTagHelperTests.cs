using System.Collections.ObjectModel;
using System.Text.Json;
using CMS.ContentEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using TrainingGuides.Web.Features.ResponsiveImages;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ResponsiveImages;

[Collection(ServiceContainerCollection.NAME)]
public class ResponsiveImageTagHelperTests
{
    private const string ORIGINAL_URL = "~/getContentAsset/item/field/photo.jpg";

    /// <summary>
    /// URL helper factory that returns content paths unchanged.
    /// </summary>
    private readonly IUrlHelperFactory unchangedUrlHelperFactory =
        CreateUrlHelperFactory(url => url);

    /// <summary>
    /// URL helper factory that resolves <c>~/</c> against the <c>/base</c> path base.
    /// </summary>
    private readonly IUrlHelperFactory pathBaseUrlHelperFactory =
        CreateUrlHelperFactory(url => url.Replace("~/", "/base/"));

    [Fact]
    public void Process_SuppressesOutput_WhenAssetIsNull()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = null
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Null(output.TagName);
    }

    [Fact]
    public void Process_SuppressesOutput_WhenAssetIsNotAnImage()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = CreateAsset(null, null, extension: ".pdf")
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Null(output.TagName);
    }

    [Fact]
    public void Process_RendersOriginal_WhenAssetHasNoVariants()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = CreateAsset(1433, 955)
        };

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
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = asset
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal(ORIGINAL_URL, GetAttribute(output, "src"));
        Assert.Null(GetAttribute(output, "srcset"));
    }

    [Fact]
    public void Process_RendersSeriesNarrowestFirst()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = CreateAsset(3000, 2000, SeriesVariants()),
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
    public void Process_DropsUpscaledWidthsAndClosesWithOriginal()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = CreateAsset(1433, 955, SeriesVariants())
        };

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
    public void Process_ClosesWithSeriesVariant_WhenOriginalMatchesItsWidth()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = CreateAsset(1920, 1280, SeriesVariants())
        };

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
    public void Process_ClosesWithOriginalOnly_WhenEverySeriesVariantIsWider()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = CreateAsset(400, 267, SeriesVariants()),
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
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = CreateAsset(3000, 2000, [.. SeriesVariants(), (ImageVariants.CARD_800X600, 800, 600)]),
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
    public void Process_FallsBackToSeries_WhenVariantIsUnknown()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = CreateAsset(3000, 2000, [.. SeriesVariants(), (ImageVariants.CARD_800X600, 800, 600)]),
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
    public void Process_FallsBackToSeries_WhenVariantCaseDiffers()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = CreateAsset(3000, 2000, [.. SeriesVariants(), (ImageVariants.CARD_800X600, 800, 600)]),
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
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = CreateAsset(null, null, SeriesVariants())
        };

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
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
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
        var tagHelper = new ResponsiveImageTagHelper(unchangedUrlHelperFactory)
        {
            Asset = CreateAsset(null, null)
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal(string.Empty, GetAttribute(output, "alt"));
        Assert.Equal("lazy", GetAttribute(output, "loading"));
    }

    [Fact]
    public void Process_ResolvesSrcAndEverySrcsetUrl_WhenRenderingSeries()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper(pathBaseUrlHelperFactory)
        {
            Asset = CreateAsset(1433, 955, SeriesVariants())
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal(
            "/base/getContentAsset/item/field/ScaleW480/photo_ScaleW480.jpg 480w, "
            + "/base/getContentAsset/item/field/ScaleW828/photo_ScaleW828.jpg 828w, "
            + "/base/getContentAsset/item/field/ScaleW1280/photo_ScaleW1280.jpg 1280w, "
            + "/base/getContentAsset/item/field/photo.jpg 1433w",
            GetAttribute(output, "srcset"));
        Assert.Equal(
            "/base/getContentAsset/item/field/photo.jpg", GetAttribute(output, "src"));
    }

    [Fact]
    public void Process_ResolvesSrc_WhenRenderingFixedVariant()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper(pathBaseUrlHelperFactory)
        {
            Asset = CreateAsset(3000, 2000, [(ImageVariants.CARD_800X600, 800, 600)]),
            Variant = ImageVariants.CARD_800X600
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal(
            "/base/getContentAsset/item/field/Card800x600/photo_Card800x600.jpg",
            GetAttribute(output, "src"));
    }

    [Fact]
    public void Process_ResolvesSrc_WhenRenderingOriginal()
    {
        // Arrange
        var tagHelper = new ResponsiveImageTagHelper(pathBaseUrlHelperFactory)
        {
            Asset = CreateAsset(1433, 955)
        };

        // Act
        var output = Process(tagHelper);

        // Assert
        Assert.Equal(
            "/base/getContentAsset/item/field/photo.jpg", GetAttribute(output, "src"));
    }

    /// <summary>
    /// Creates a URL helper factory that resolves content paths with <paramref name="resolve"/>.
    /// </summary>
    private static IUrlHelperFactory CreateUrlHelperFactory(Func<string, string> resolve)
    {
        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(helper => helper.Content(It.IsAny<string>()))
            .Returns((string? path) => resolve(path!));

        var factory = new Mock<IUrlHelperFactory>();
        factory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
            .Returns(urlHelper.Object);

        return factory.Object;
    }

    /// <summary>
    /// Returns the Scaled width-series variants stored at a 3:2 aspect ratio.
    /// </summary>
    private static (string Name, int? W, int? H)[] SeriesVariants() =>
    [
        (ImageVariants.SCALE_W480, 480, 320),
        (ImageVariants.SCALE_W828, 828, 552),
        (ImageVariants.SCALE_W1280, 1280, 853),
        (ImageVariants.SCALE_W1600, 1600, 1067),
        (ImageVariants.SCALE_W1920, 1920, 1280),
        (ImageVariants.SCALE_W2560, 2560, 1707)
    ];

    /// <summary>
    /// Creates an asset with the specified original dimensions, variant URLs, and variant metadata.
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
    /// Deserializes <see cref="ContentItemAssetMetadata"/> from JSON because
    /// <see cref="ContentItemAssetMetadata.Variants"/> has no public setter.
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

        tagHelper.ViewContext = new ViewContext();
        tagHelper.Process(context, output);

        return output;
    }

    private static string? GetAttribute(TagHelperOutput output, string name) =>
        output.Attributes.TryGetAttribute(name, out var attribute) ? attribute.Value?.ToString() : null;
}
