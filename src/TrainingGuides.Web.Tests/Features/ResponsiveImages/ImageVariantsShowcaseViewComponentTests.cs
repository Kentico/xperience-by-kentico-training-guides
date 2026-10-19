using CMS.ContentEngine;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using TrainingGuides.Web.Features.ResponsiveImages;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ResponsiveImages;

public class ImageVariantsShowcaseViewComponentTests
{
    private const int TEASER_ASSET_DEPTH = 2;
    private const string ARTICLE_TITLE = "Title";
    private const string ALT_TEXT = "A lizard";

    private readonly Mock<IContentItemRetrieverService> retrieverServiceMock;
    private readonly ImageVariantsShowcaseViewComponent viewComponent;

    public ImageVariantsShowcaseViewComponentTests()
    {
        retrieverServiceMock = new Mock<IContentItemRetrieverService>();
        viewComponent =
            new ImageVariantsShowcaseViewComponent(retrieverServiceMock.Object);
    }

    private void SetupCurrentPage(ArticlePage? articlePage) =>
        retrieverServiceMock
            .Setup(x => x.RetrieveCurrentPage<ArticlePage>(
                TEASER_ASSET_DEPTH,
                It.IsAny<bool>(),
                It.IsAny<string?>()))
            .ReturnsAsync(articlePage);

    private static ArticlePage BuildArticlePage(IEnumerable<Asset> teaser) =>
        new()
        {
            ArticlePageArticleContent = [new GeneralArticle()
            {
                ArticleSchemaTitle = ARTICLE_TITLE,
                ArticleSchemaTeaser = teaser
            }]
        };

    private async Task<ImageVariantsShowcaseViewModel> InvokeAndGetModel()
    {
        var result = Assert.IsType<ViewViewComponentResult>(
            await viewComponent.InvokeAsync());
        return Assert.IsType<ImageVariantsShowcaseViewModel>(
            result.ViewData?.Model);
    }

    [Fact]
    public async Task Invoke_MapsTeaserAssetAndAltText()
    {
        // Arrange
        var assetFile = new ContentItemAsset();
        SetupCurrentPage(BuildArticlePage(
            [new Asset { AssetFile = assetFile, AssetAltText = ALT_TEXT }]));

        // Act
        var model = await InvokeAndGetModel();

        // Assert
        Assert.NotNull(model.Image);
        Assert.Same(assetFile, model.Image.Asset);
        Assert.Equal(ALT_TEXT, model.Image.AltText);
    }

    [Fact]
    public async Task Invoke_MapsArticleTitle()
    {
        // Arrange
        SetupCurrentPage(BuildArticlePage([]));

        // Act
        var model = await InvokeAndGetModel();

        // Assert
        Assert.Equal(ARTICLE_TITLE, model.Title);
    }

    [Fact]
    public async Task Invoke_ReturnsNullImage_WhenPageHasNoArticle()
    {
        // Arrange
        SetupCurrentPage(new ArticlePage { ArticlePageArticleContent = [] });

        // Act
        var model = await InvokeAndGetModel();

        // Assert
        Assert.Null(model.Image);
    }

    [Fact]
    public async Task Invoke_ReturnsNullImage_WhenArticleHasNoTeaser()
    {
        // Arrange
        SetupCurrentPage(BuildArticlePage([]));

        // Act
        var model = await InvokeAndGetModel();

        // Assert
        Assert.Null(model.Image);
    }

    [Fact]
    public async Task Invoke_ReturnsNullImage_WhenPageIsNull()
    {
        // Arrange
        SetupCurrentPage(null);

        // Act
        var model = await InvokeAndGetModel();

        // Assert
        Assert.Null(model.Image);
    }
}
