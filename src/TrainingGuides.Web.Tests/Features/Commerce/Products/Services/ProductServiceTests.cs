using CMS.Commerce;
using CMS.ContentEngine;
using CMS.DataEngine;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TrainingGuides.ProductStock;
using TrainingGuides.Web.Commerce.Products.Services;
using TrainingGuides.Web.Features.Commerce.PriceCalculation.Models;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Commerce.Products.Services;

public class ProductServiceTests
{
    private readonly ProductService productService;

    public ProductServiceTests()
    {
        // Create minimal mocks - we only need these to instantiate the service
        var contentItemRetrieverServiceMock = new Mock<IContentItemRetrieverService>();
        var loggerMock = new Mock<ILogger<ProductService>>();
        var productAvailableStockInfoProviderMock = new Mock<IInfoProvider<ProductAvailableStockInfo>>();
        var tagInfoProviderMock = new Mock<IInfoProvider<TagInfo>>();
        var taxonomyRetrieverMock = new Mock<ITaxonomyRetriever>();
        var preferredLanguageRetrieverMock = new Mock<IPreferredLanguageRetriever>();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var priceCalculationServiceMock = new Mock<IPriceCalculationService<PriceCalculationRequest, TrainingGuidesPriceCalculationResult>>();

        productService = new ProductService(
            contentItemRetrieverServiceMock.Object,
            loggerMock.Object,
            productAvailableStockInfoProviderMock.Object,
            tagInfoProviderMock.Object,
            taxonomyRetrieverMock.Object,
            preferredLanguageRetrieverMock.Object,
                httpContextAccessorMock.Object,
            priceCalculationServiceMock.Object);
    }

    #region ParseFilterValues Tests

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("all")]
    [InlineData("ALL")]
    [InlineData("All")]
    public void ParseFilterValues_EmptyOrAll_ReturnsNull(string input)
    {
        var result = productService.ParseFilterValues(input);

        Assert.Null(result);
    }

    [Fact]
    public void ParseFilterValues_OnlyAllValues_ReturnsNull()
    {
        var result = productService.ParseFilterValues("all,ALL,All");

        Assert.Null(result);
    }

    [Fact]
    public void ParseFilterValues_ValidValues_ReturnsLowercaseCollection()
    {
        var result = productService.ParseFilterValues("Polyester,Nylon");

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains("polyester", result);
        Assert.Contains("nylon", result);
    }

    [Fact]
    public void ParseFilterValues_ValuesWithAllIncluded_FiltersOutAll()
    {
        var result = productService.ParseFilterValues("Polyester,all,Nylon");

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains("polyester", result);
        Assert.Contains("nylon", result);
        Assert.DoesNotContain("all", result);
    }

    #endregion

    #region GetAllTagsForSchema Tests

    [Fact]
    public void GetAllTagsForSchema_ProductWithSchema_ReturnsProductTags()
    {
        // Arrange
        var tag1 = new TagReference { Identifier = Guid.NewGuid() };
        var tag2 = new TagReference { Identifier = Guid.NewGuid() };
        var expectedTags = new[] { tag1, tag2 };

        var productMock = new Mock<IProductSchema>();
        productMock.As<IMaterialSchema>()
            .Setup(p => p.MaterialSchemaMaterial)
            .Returns(expectedTags);

        // Act
        var result = productService.GetAllTagsForSchema<IMaterialSchema>(
            productMock.Object, p => p.MaterialSchemaMaterial);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(tag1, result);
        Assert.Contains(tag2, result);
    }

    [Fact]
    public void GetAllTagsForSchema_ProductWithoutSchema_ReturnsEmpty()
    {
        // Arrange
        var productMock = new Mock<IProductSchema>();
        // Product does not implement IMaterialSchema

        // Act
        var result = productService.GetAllTagsForSchema<IMaterialSchema>(
            productMock.Object, p => p.MaterialSchemaMaterial);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAllTagsForSchema_VariantsWithSchema_ReturnsVariantTags()
    {
        // Arrange
        var tag1 = new TagReference { Identifier = Guid.NewGuid() };
        var tag2 = new TagReference { Identifier = Guid.NewGuid() };
        var tag3 = new TagReference { Identifier = Guid.NewGuid() };

        var variant1Mock = new Mock<IProductVariantSchema>();
        variant1Mock.As<IColorPatternSchema>()
            .Setup(v => v.ColorPattern)
            .Returns(new[] { tag1, tag2 });

        var variant2Mock = new Mock<IProductVariantSchema>();
        variant2Mock.As<IColorPatternSchema>()
            .Setup(v => v.ColorPattern)
            .Returns(new[] { tag3 });

        var productMock = new Mock<IProductSchema>();
        productMock.As<IProductParentSchema>()
            .Setup(p => p.ProductParentSchemaVariants)
            .Returns(new[] { variant1Mock.Object, variant2Mock.Object });

        // Act
        var result = productService.GetAllTagsForSchema<IColorPatternSchema>(
            productMock.Object, p => p.ColorPattern);

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Contains(tag1, result);
        Assert.Contains(tag2, result);
        Assert.Contains(tag3, result);
    }

    [Fact]
    public void GetAllTagsForSchema_ProductAndVariantsWithSchema_ReturnsDistinctTags()
    {
        // Arrange
        var tag1 = new TagReference { Identifier = Guid.NewGuid() };
        var tag2 = new TagReference { Identifier = Guid.NewGuid() };
        var tag3 = new TagReference { Identifier = Guid.NewGuid() };

        // Product has some tags
        var productMock = new Mock<IProductSchema>();
        productMock.As<IMaterialSchema>()
            .Setup(p => p.MaterialSchemaMaterial)
            .Returns(new[] { tag1, tag2 });

        // Variant has overlapping and unique tags
        var variantMock = new Mock<IProductVariantSchema>();
        variantMock.As<IMaterialSchema>()
            .Setup(v => v.MaterialSchemaMaterial)
            .Returns(new[] { tag2, tag3 }); // tag2 overlaps with product

        productMock.As<IProductParentSchema>()
            .Setup(p => p.ProductParentSchemaVariants)
            .Returns(new[] { variantMock.Object });

        // Act
        var result = productService.GetAllTagsForSchema<IMaterialSchema>(
            productMock.Object, p => p.MaterialSchemaMaterial);

        // Assert
        Assert.Equal(3, result.Count()); // Should deduplicate tag2
        Assert.Contains(tag1, result);
        Assert.Contains(tag2, result);
        Assert.Contains(tag3, result);
    }

    [Fact]
    public void GetAllTagsForSchema_MixedVariantsOnlyOneWithSchema_ReturnsOnlySchemaVariantTags()
    {
        // Arrange
        var tag1 = new TagReference { Identifier = Guid.NewGuid() };

        var variantWithSchemaMock = new Mock<IProductVariantSchema>();
        variantWithSchemaMock.As<IColorPatternSchema>()
            .Setup(v => v.ColorPattern)
            .Returns(new[] { tag1 });

        var variantWithoutSchemaMock = new Mock<IProductVariantSchema>();
        // This variant doesn't implement IColorPatternSchema

        var productMock = new Mock<IProductSchema>();
        productMock.As<IProductParentSchema>()
            .Setup(p => p.ProductParentSchemaVariants)
            .Returns(new[] { variantWithSchemaMock.Object, variantWithoutSchemaMock.Object });

        // Act
        var result = productService.GetAllTagsForSchema<IColorPatternSchema>(
            productMock.Object, p => p.ColorPattern);

        // Assert
        Assert.Single(result);
        Assert.Contains(tag1, result);
    }

    [Fact]
    public void GetAllTagsForSchema_NoVariants_ReturnsOnlyProductTags()
    {
        // Arrange
        var tag1 = new TagReference { Identifier = Guid.NewGuid() };

        var productMock = new Mock<IProductSchema>();
        productMock.As<IMaterialSchema>()
            .Setup(p => p.MaterialSchemaMaterial)
            .Returns(new[] { tag1 });
        // Product is not a parent (no variants)

        // Act
        var result = productService.GetAllTagsForSchema<IMaterialSchema>(
            productMock.Object, p => p.MaterialSchemaMaterial);

        // Assert
        Assert.Single(result);
        Assert.Contains(tag1, result);
    }

    #endregion
}
