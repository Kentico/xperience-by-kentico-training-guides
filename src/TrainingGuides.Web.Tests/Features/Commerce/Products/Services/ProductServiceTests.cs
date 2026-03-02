using CMS.Commerce;
using CMS.ContentEngine;
using CMS.DataEngine;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using TrainingGuides.ProductStock;
using TrainingGuides.Web.Commerce.Products.Services;
using TrainingGuides.Web.Features.Commerce.PriceCalculation.Models;
using TrainingGuides.Web.Features.Membership.Services;
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
        var membershipServiceMock = new Mock<IMembershipService>();
        var priceCalculationServiceMock = new Mock<IPriceCalculationService<PriceCalculationRequest, TrainingGuidesPriceCalculationResult>>();

        productService = new ProductService(
            contentItemRetrieverServiceMock.Object,
            loggerMock.Object,
            productAvailableStockInfoProviderMock.Object,
            tagInfoProviderMock.Object,
            taxonomyRetrieverMock.Object,
            preferredLanguageRetrieverMock.Object,
            membershipServiceMock.Object,
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
}
