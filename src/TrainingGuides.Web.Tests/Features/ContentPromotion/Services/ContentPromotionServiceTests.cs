using CMS.ContentEngine;
using CMS.Websites;
using Kentico.Content.Web.Mvc.Routing;
using Moq;
using TrainingGuides.Web.Commerce.Products.Models;
using TrainingGuides.Web.Commerce.Products.Services;
using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.ContentPromotion.Services;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ContentPromotion.Services;

public class ContentPromotionServiceTests
{
    private const string WIDGET_TITLE = "Autumn savings on pet cover";
    private const string ITEM_TITLE = "Pet insurance";
    private const string WIDGET_DESCRIPTION = "Cover your cat this autumn for less.";
    private const string ITEM_DESCRIPTION = "Comprehensive cover for cats and dogs.";
    private const string WIDGET_CALL_TO_ACTION = "Get a quote";
    private const string ITEM_CALL_TO_ACTION = "Read more";
    private const string WIDGET_IMAGE_PATH = "/assets/promo-autumn.jpg";
    private const string WIDGET_IMAGE_ALT = "Cat wearing an autumn scarf";
    private const string ITEM_IMAGE_PATH = "/assets/pet-insurance.jpg";
    private const string TARGET_RELATIVE_URL = "/contact-us";
    private const string EXTERNAL_URL = "https://example.com/offer";
    private const string PAGE_RELATIVE_URL = "/pet-insurance";
    private const string ITEM_IMAGE_ALT = "Dog and cat sitting together";

    private const string RICH_TEXT_DESCRIPTION = "<p>Cover for <strong>cats</strong> and dogs.</p>";
    private const string LINK_TEXT = "the full policy";
    private const string LINKED_DESCRIPTION =
        "<p>Read <a href=\"/policies\" title=\"Policies\">the full policy</a> for <strong>details</strong>.</p>";

    private const string PRODUCT_IMAGE_PATH = "/assets/cat-food.jpg";
    private const string PRODUCT_IMAGE_ALT = "A bag of cat food";
    private const string VARIANT_IMAGE_PATH = "/assets/cat-food-large.jpg";
    private const string VARIANT_IMAGE_ALT = "The large bag of cat food";

    private const string CATEGORY_NAME = "Insurance";
    private const string OTHER_CATEGORY_NAME = "Pets";
    private const string LANGUAGE = "en";
    private const string FIRST_BENEFIT = "Vet visits covered from day one.";
    private const string SECOND_BENEFIT = "No excess on routine check-ups.";
    private const decimal VARIANT_PRICE = 24.99m;
    private const decimal DISCOUNTED_PRICE = 19.99m;

    private static readonly Guid targetGuid = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid selectedGuid = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid categoryGuid = new("33333333-3333-3333-3333-333333333333");
    private static readonly Guid otherCategoryGuid = new("44444444-4444-4444-4444-444444444444");

    private readonly Mock<IContentItemRetrieverService> contentItemRetrieverServiceMock = new();
    private readonly Mock<IWebPageUrlRetriever> webPageUrlRetrieverMock = new();
    private readonly Mock<ITaxonomyRetriever> taxonomyRetrieverMock = new();
    private readonly Mock<IPreferredLanguageRetriever> preferredLanguageRetrieverMock = new();
    private readonly Mock<IProductService> productServiceMock = new();
    private readonly ContentPromotionService contentPromotionService;

    public ContentPromotionServiceTests()
    {
        preferredLanguageRetrieverMock
            .Setup(x => x.Get())
            .Returns(LANGUAGE);

        taxonomyRetrieverMock
            .Setup(x => x.RetrieveTags(It.IsAny<IEnumerable<Guid>>(), It.IsAny<string>()))
            .ReturnsAsync([]);

        productServiceMock
            .Setup(x => x.GetListingStockForProduct(It.IsAny<IProductSchema>()))
            .ReturnsAsync(ProductStockEnum.Unknown);

        productServiceMock
            .Setup(x => x.GetCatalogPrice(It.IsAny<IProductSchema>()))
            .ReturnsAsync(VARIANT_PRICE);

        contentPromotionService = new ContentPromotionService(
            contentItemRetrieverServiceMock.Object,
            webPageUrlRetrieverMock.Object,
            taxonomyRetrieverMock.Object,
            preferredLanguageRetrieverMock.Object,
            productServiceMock.Object);
    }

    private static ProductImage ProductImageWith(string url, string altText) => new()
    {
        ProductImageAsset = new ContentItemAsset { Url = url },
        ProductImageAltText = altText
    };

    private static Tag TagWith(Guid identifier, string title) => new()
    {
        Identifier = identifier,
        Title = title
    };

    [Fact]
    public void ResolveDisplayValues_TitleOverriddenAndItemHasTitle_UsesOverride()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            Title = WIDGET_TITLE
        };
        var item = new PromotedItemSource
        {
            Title = ITEM_TITLE
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(WIDGET_TITLE, result.Title);
    }

    [Fact]
    public void ResolveDisplayValues_TitleNotOverriddenAndItemHasTitle_InheritsItemTitle()
    {
        var properties = new ContentPromotionWidgetProperties();
        var item = new PromotedItemSource
        {
            Title = ITEM_TITLE
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(ITEM_TITLE, result.Title);
    }

    [Fact]
    public void ResolveDisplayValues_TitleNotOverriddenAndItemHasNoTitle_ReturnsEmptyTitle()
    {
        var properties = new ContentPromotionWidgetProperties();
        var item = new PromotedItemSource();

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(string.Empty, result.Title);
    }

    // Hide elements is not offered in manual mode, but a value chosen before switching to manual is
    // still stored. Applying it would hide what the editor typed, for a reason they cannot see.
    [Fact]
    public void ResolveDisplayValues_ManualModeWithStoredHiddenElements_ShowsEveryTypedValue()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.MANUAL,
            Title = WIDGET_TITLE,
            Description = WIDGET_DESCRIPTION,
            CallToActionText = WIDGET_CALL_TO_ACTION,
            HideElements =
            [
                ContentPromotionElement.TITLE,
                ContentPromotionElement.DESCRIPTION,
                ContentPromotionElement.IMAGE,
                ContentPromotionElement.CALL_TO_ACTION
            ]
        };
        var overrideImage = new AssetViewModel { FilePath = WIDGET_IMAGE_PATH, AltText = WIDGET_IMAGE_ALT };

        var result = contentPromotionService.ResolveDisplayValues(properties, null, overrideImage);

        Assert.Equal(WIDGET_TITLE, result.Title);
        Assert.Equal(WIDGET_DESCRIPTION, result.DescriptionHtml.Value);
        Assert.Equal(WIDGET_CALL_TO_ACTION, result.CallToActionText);
        Assert.Equal(WIDGET_IMAGE_PATH, result.Image?.FilePath);
    }

    [Fact]
    public async Task ResolveOverrideImage_ManualModeWithStoredHiddenImage_StillResolvesTheImage()
    {
        var imageGuid = Guid.NewGuid();
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveContentItemByGuid<Asset>(imageGuid, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(new Asset
            {
                AssetAltText = WIDGET_IMAGE_ALT,
                AssetFile = new ContentItemAsset
                {
                    Url = WIDGET_IMAGE_PATH,
                    Metadata = new ContentItemAssetMetadata { Name = "promo-autumn.jpg" }
                }
            });
        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.MANUAL,
            Image = [new ContentItemReference { Identifier = imageGuid }],
            HideElements = [ContentPromotionElement.IMAGE]
        };

        var result = await contentPromotionService.ResolveOverrideImage(properties);

        Assert.Equal(WIDGET_IMAGE_PATH, result?.FilePath);
    }

    [Fact]
    public void ResolveDisplayValues_TitleHiddenAndOverridden_ReturnsEmptyTitle()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            Title = WIDGET_TITLE,
            HideElements = [ContentPromotionElement.TITLE]
        };
        var item = new PromotedItemSource
        {
            Title = ITEM_TITLE
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(string.Empty, result.Title);
    }

    [Fact]
    public void ResolveDisplayValues_TitleHiddenAndOnlyItemHasTitle_ReturnsEmptyTitle()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            HideElements = [ContentPromotionElement.TITLE]
        };
        var item = new PromotedItemSource
        {
            Title = ITEM_TITLE
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(string.Empty, result.Title);
    }

    [Fact]
    public void ResolveDisplayValues_NoItemAndTitleOverridden_UsesOverride()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            Title = WIDGET_TITLE
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, null);

        Assert.Equal(WIDGET_TITLE, result.Title);
    }

    [Fact]
    public void ResolveDisplayValues_NoItemAndNoTitleOverride_ReturnsEmptyTitle()
    {
        var properties = new ContentPromotionWidgetProperties();

        var result = contentPromotionService.ResolveDisplayValues(properties, null);

        Assert.Equal(string.Empty, result.Title);
    }

    [Fact]
    public void ResolveDisplayValues_DescriptionOverriddenAndItemHasDescription_UsesOverride()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            Description = WIDGET_DESCRIPTION
        };
        var item = new PromotedItemSource
        {
            Description = ITEM_DESCRIPTION
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(WIDGET_DESCRIPTION, result.DescriptionHtml.Value);
    }

    [Fact]
    public void ResolveDisplayValues_CallToActionOverriddenAndItemHasCallToAction_UsesOverride()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            CallToActionText = WIDGET_CALL_TO_ACTION
        };
        var item = new PromotedItemSource
        {
            CallToActionText = ITEM_CALL_TO_ACTION
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(WIDGET_CALL_TO_ACTION, result.CallToActionText);
    }

    [Fact]
    public void ResolveDisplayValues_DescriptionNotOverridden_InheritsItemDescription()
    {
        var properties = new ContentPromotionWidgetProperties();
        var item = new PromotedItemSource
        {
            Description = ITEM_DESCRIPTION
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(ITEM_DESCRIPTION, result.DescriptionHtml.Value);
    }

    [Fact]
    public void ResolveDisplayValues_InheritedDescriptionCarriesMarkup_PassesItThroughUnescaped()
    {
        var properties = new ContentPromotionWidgetProperties();
        var item = new PromotedItemSource
        {
            Description = RICH_TEXT_DESCRIPTION
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(RICH_TEXT_DESCRIPTION, result.DescriptionHtml.Value);
    }

    [Fact]
    public void ResolveDisplayValues_OverrideCarriesMarkup_EncodesItSoItStaysLiteral()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            Description = RICH_TEXT_DESCRIPTION
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, null);

        Assert.DoesNotContain("<p>", result.DescriptionHtml.Value);
        Assert.Contains("&lt;p&gt;", result.DescriptionHtml.Value);
    }

    [Fact]
    public void ResolveDisplayValues_InheritedDescriptionContainsAnchors_KeepsTheirTextAndDropsTheLinks()
    {
        var properties = new ContentPromotionWidgetProperties();
        var item = new PromotedItemSource
        {
            Description = LINKED_DESCRIPTION
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.DoesNotContain("<a ", result.DescriptionHtml.Value);
        Assert.DoesNotContain("</a>", result.DescriptionHtml.Value);
        Assert.Contains(LINK_TEXT, result.DescriptionHtml.Value);
        Assert.Contains("<strong>", result.DescriptionHtml.Value);
    }

    // Only anchors go. Elements whose names merely start with "a" stay, and a legal ">" inside
    // a quoted attribute does not end the tag early and spill the rest onto the page.
    [Theory]
    [InlineData("<abbr title=\"x\">ABC</abbr>", "<abbr title=\"x\">ABC</abbr>")]
    [InlineData("<article>Text</article>", "<article>Text</article>")]
    [InlineData("<a title=\"a>b\">Policy</a>", "Policy")]
    [InlineData("<A HREF=\"/x\" target=\"_blank\">Policy</A>", "Policy")]
    public void ResolveDisplayValues_InheritedDescription_StripsOnlyAnchors(string stored, string expected)
    {
        var properties = new ContentPromotionWidgetProperties();
        var item = new PromotedItemSource { Description = stored };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(expected, result.DescriptionHtml.Value);
    }

    [Fact]
    public void ResolveDisplayValues_DescriptionHiddenAndOverridden_ReturnsEmptyDescription()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            Description = WIDGET_DESCRIPTION,
            HideElements = [ContentPromotionElement.DESCRIPTION]
        };
        var item = new PromotedItemSource
        {
            Description = ITEM_DESCRIPTION
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(string.Empty, result.DescriptionHtml.Value);
    }

    [Fact]
    public void ResolveDisplayValues_CallToActionNotOverridden_InheritsItemCallToAction()
    {
        var properties = new ContentPromotionWidgetProperties();
        var item = new PromotedItemSource
        {
            CallToActionText = ITEM_CALL_TO_ACTION
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(ITEM_CALL_TO_ACTION, result.CallToActionText);
    }

    [Fact]
    public void ResolveDisplayValues_CallToActionHiddenAndOverridden_ReturnsEmptyCallToAction()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            CallToActionText = WIDGET_CALL_TO_ACTION,
            HideElements = [ContentPromotionElement.CALL_TO_ACTION]
        };
        var item = new PromotedItemSource
        {
            CallToActionText = ITEM_CALL_TO_ACTION
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(string.Empty, result.CallToActionText);
    }

    [Fact]
    public void ResolveDisplayValues_OneElementHidden_LeavesOtherElementsUntouched()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            HideElements = [ContentPromotionElement.DESCRIPTION]
        };
        var item = new PromotedItemSource
        {
            Title = ITEM_TITLE,
            Description = ITEM_DESCRIPTION,
            CallToActionText = ITEM_CALL_TO_ACTION
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(ITEM_TITLE, result.Title);
        Assert.Equal(string.Empty, result.DescriptionHtml.Value);
        Assert.Equal(ITEM_CALL_TO_ACTION, result.CallToActionText);
    }

    [Fact]
    public void ResolveDisplayValues_ImageOverridden_UsesOverrideImageWithItsOwnAltText()
    {
        var properties = new ContentPromotionWidgetProperties();
        var item = new PromotedItemSource
        {
            Image = new AssetViewModel { FilePath = ITEM_IMAGE_PATH, AltText = ITEM_IMAGE_ALT }
        };
        var overrideImage = new AssetViewModel { FilePath = WIDGET_IMAGE_PATH, AltText = WIDGET_IMAGE_ALT };

        var result = contentPromotionService.ResolveDisplayValues(properties, item, overrideImage);

        Assert.Equal(WIDGET_IMAGE_PATH, result.Image?.FilePath);
        Assert.Equal(WIDGET_IMAGE_ALT, result.Image?.AltText);
    }

    [Fact]
    public void ResolveDisplayValues_ImageNotOverridden_InheritsItemImageWithItsAltText()
    {
        var properties = new ContentPromotionWidgetProperties();
        var item = new PromotedItemSource
        {
            Image = new AssetViewModel { FilePath = ITEM_IMAGE_PATH, AltText = ITEM_IMAGE_ALT }
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, item);

        Assert.Equal(ITEM_IMAGE_PATH, result.Image?.FilePath);
        Assert.Equal(ITEM_IMAGE_ALT, result.Image?.AltText);
    }

    [Fact]
    public void ResolveDisplayValues_ImageHiddenAndOverridden_ReturnsNoImage()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            HideElements = [ContentPromotionElement.IMAGE]
        };
        var item = new PromotedItemSource
        {
            Image = new AssetViewModel { FilePath = ITEM_IMAGE_PATH, AltText = ITEM_IMAGE_ALT }
        };
        var overrideImage = new AssetViewModel { FilePath = WIDGET_IMAGE_PATH, AltText = WIDGET_IMAGE_ALT };

        var result = contentPromotionService.ResolveDisplayValues(properties, item, overrideImage);

        Assert.Null(result.Image);
    }

    [Fact]
    public void ResolveDisplayValues_NoItemAndNoOverrideImage_ReturnsNoImage()
    {
        var properties = new ContentPromotionWidgetProperties();

        var result = contentPromotionService.ResolveDisplayValues(properties, null);

        Assert.Null(result.Image);
    }

    [Fact]
    public async Task ResolvePromotedItem_PageSourceWithArticlePage_ReturnsArticleFamilyFromLinkedArticle()
    {
        var articlePage = new ArticlePage
        {
            ArticlePageArticleContent =
            [
                new GeneralArticle
                {
                    ArticleSchemaTitle = ITEM_TITLE,
                    ArticleSchemaSummary = ITEM_DESCRIPTION
                }
            ]
        };
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveWebPageByContentItemGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(articlePage);

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE,
            SelectedPage = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Equal(ContentFamily.Article, result.Family);
        Assert.Equal(ITEM_TITLE, result.Item?.Title);
        Assert.Equal(ITEM_DESCRIPTION, result.Item?.Description);
    }

    [Fact]
    public async Task ResolvePromotedItem_ContentItemSourceWithGeneralArticle_ReturnsArticleFamily()
    {
        var article = new GeneralArticle
        {
            ArticleSchemaTitle = ITEM_TITLE,
            ArticleSchemaSummary = ITEM_DESCRIPTION
        };
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveContentItemByGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(article);

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.CONTENT_ITEM,
            SelectedContentItem = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Equal(ContentFamily.Article, result.Family);
        Assert.Equal(ITEM_TITLE, result.Item?.Title);
        Assert.Equal(ITEM_DESCRIPTION, result.Item?.Description);
    }

    [Fact]
    public async Task ResolvePromotedItem_PageSourceWithProductPage_ReturnsProductFamilyFromLinkedProduct()
    {
        var productPage = new ProductPage
        {
            ProductPageProducts =
            [
                new CatFood
                {
                    ProductSchemaName = ITEM_TITLE,
                    ProductSchemaDescription = ITEM_DESCRIPTION
                }
            ]
        };
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveWebPageByContentItemGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(productPage);

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE,
            SelectedPage = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Equal(ContentFamily.Product, result.Family);
        Assert.Equal(ITEM_TITLE, result.Item?.Title);
        Assert.Equal(ITEM_DESCRIPTION, result.Item?.Description);
    }

    [Fact]
    public async Task ResolvePromotedItem_ContentItemSourceWithProductThatHasImages_InheritsTheFirstImage()
    {
        var variant = new CatFoodVariant
        {
            ProductSchemaName = ITEM_TITLE,
            ProductSchemaImages =
            [
                ProductImageWith(VARIANT_IMAGE_PATH, VARIANT_IMAGE_ALT),
                ProductImageWith(PRODUCT_IMAGE_PATH, PRODUCT_IMAGE_ALT)
            ]
        };
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveContentItemByGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(variant);

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.CONTENT_ITEM,
            SelectedContentItem = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Equal(VARIANT_IMAGE_PATH, result.Item?.Image?.FilePath);
        Assert.Equal(VARIANT_IMAGE_ALT, result.Item?.Image?.AltText);
    }

    [Fact]
    public async Task ResolvePromotedItem_PageSourceWithProductPage_InheritsTheProductImage()
    {
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveWebPageByContentItemGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(new ProductPage
            {
                ProductPageProducts =
                [
                    new CatFood
                    {
                        ProductSchemaName = ITEM_TITLE,
                        ProductSchemaImages = [ProductImageWith(PRODUCT_IMAGE_PATH, PRODUCT_IMAGE_ALT)]
                    }
                ]
            });

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE,
            SelectedPage = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Equal(PRODUCT_IMAGE_PATH, result.Item?.Image?.FilePath);
        Assert.Equal(PRODUCT_IMAGE_ALT, result.Item?.Image?.AltText);
    }

    [Fact]
    public async Task ResolvePromotedItem_ParentProductHasNoImagesButAVariantDoes_InheritsTheVariantImage()
    {
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveWebPageByContentItemGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(new ProductPage
            {
                ProductPageProducts =
                [
                    new CatFood
                    {
                        ProductSchemaName = ITEM_TITLE,
                        ProductSchemaImages = [],
                        ProductParentSchemaVariants =
                        [
                            new CatFoodVariant
                            {
                                ProductSchemaImages = [ProductImageWith(VARIANT_IMAGE_PATH, VARIANT_IMAGE_ALT)]
                            }
                        ]
                    }
                ]
            });

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE,
            SelectedPage = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Equal(VARIANT_IMAGE_PATH, result.Item?.Image?.FilePath);
        Assert.Equal(VARIANT_IMAGE_ALT, result.Item?.Image?.AltText);
    }

    // The product path feeds the same field every other family does, so it must still lose to an
    // override and disappear when the image element is hidden - T2's rule, not a product rule.
    [Fact]
    public async Task ResolveDisplayValues_ProductImageInheritedAndOverridden_UsesTheOverride()
    {
        var promotedItem = await PromotedProductWithImage();
        var overrideImage = new AssetViewModel { FilePath = WIDGET_IMAGE_PATH, AltText = WIDGET_IMAGE_ALT };

        var result = contentPromotionService.ResolveDisplayValues(
            new ContentPromotionWidgetProperties(), promotedItem.Item, overrideImage);

        Assert.Equal(WIDGET_IMAGE_PATH, result.Image?.FilePath);
    }

    [Fact]
    public async Task ResolveDisplayValues_ProductImageInheritedAndImageHidden_ReturnsNoImage()
    {
        var promotedItem = await PromotedProductWithImage();
        var properties = new ContentPromotionWidgetProperties
        {
            HideElements = [ContentPromotionElement.IMAGE]
        };

        var result = contentPromotionService.ResolveDisplayValues(properties, promotedItem.Item);

        Assert.Null(result.Image);
    }

    /// <summary>
    /// Resolves a real product selection rather than hand-building a source, so these two assert
    /// the override and hide rules against the image the product path actually produces.
    /// </summary>
    private async Task<PromotedItemResult> PromotedProductWithImage()
    {
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveContentItemByGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(new CatFoodVariant
            {
                ProductSchemaName = ITEM_TITLE,
                ProductSchemaImages = [ProductImageWith(PRODUCT_IMAGE_PATH, PRODUCT_IMAGE_ALT)]
            });

        return await contentPromotionService.ResolvePromotedItem(new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.CONTENT_ITEM,
            SelectedContentItem = [new ContentItemReference { Identifier = selectedGuid }]
        });
    }

    [Fact]
    public async Task ResolvePromotedItem_FirstProductImageHasNoFile_FallsThroughToTheNextUsableOne()
    {
        var variant = new CatFoodVariant
        {
            ProductSchemaName = ITEM_TITLE,
            ProductSchemaImages =
            [
                ProductImageWith(string.Empty, PRODUCT_IMAGE_ALT),
                ProductImageWith(VARIANT_IMAGE_PATH, VARIANT_IMAGE_ALT)
            ]
        };
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveContentItemByGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(variant);

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.CONTENT_ITEM,
            SelectedContentItem = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Equal(VARIANT_IMAGE_PATH, result.Item?.Image?.FilePath);
    }

    [Fact]
    public async Task ResolvePromotedItem_ProductHasNoImagesAnywhere_InheritsNoImage()
    {
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveWebPageByContentItemGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(new ProductPage
            {
                ProductPageProducts =
                [
                    new CatFood
                    {
                        ProductSchemaName = ITEM_TITLE,
                        ProductSchemaImages = [],
                        ProductParentSchemaVariants = [new CatFoodVariant { ProductSchemaImages = [] }]
                    }
                ]
            });

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE,
            SelectedPage = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Null(result.Item?.Image);
    }

    [Fact]
    public async Task ResolvePromotedItem_ContentItemSourceWithProductVariant_ReturnsProductFamily()
    {
        var variant = new CatFoodVariant
        {
            ProductSchemaName = ITEM_TITLE,
            ProductSchemaDescription = ITEM_DESCRIPTION
        };
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveContentItemByGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(variant);

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.CONTENT_ITEM,
            SelectedContentItem = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Equal(ContentFamily.Product, result.Family);
        Assert.Equal(ITEM_TITLE, result.Item?.Title);
    }

    [Fact]
    public async Task ResolvePromotedItem_PageSourceWithServicePage_ReturnsServiceFamilyFromLinkedService()
    {
        var servicePage = new ServicePage
        {
            ServicePageService =
            [
                new Service
                {
                    ServiceName = ITEM_TITLE,
                    ServiceShortDescription = ITEM_DESCRIPTION
                }
            ]
        };
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveWebPageByContentItemGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(servicePage);

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE,
            SelectedPage = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Equal(ContentFamily.Service, result.Family);
        Assert.Equal(ITEM_TITLE, result.Item?.Title);
        Assert.Equal(ITEM_DESCRIPTION, result.Item?.Description);
    }

    [Fact]
    public async Task ResolvePromotedItem_ManualSource_DoesNotRetrieveAnything()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.MANUAL
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Equal(ContentFamily.None, result.Family);
        Assert.Null(result.Item);
        contentItemRetrieverServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ResolvePromotedItem_PageSourceWithEmptySelector_ReturnsNothingSelected()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Equal(ContentFamily.None, result.Family);
        Assert.Null(result.Item);
        Assert.False(result.SelectionFailed);
    }

    [Fact]
    public async Task ResolvePromotedItem_PageSourceWithUnresolvableSelection_ReportsSelectionFailed()
    {
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveWebPageByContentItemGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync((IWebPageFieldsSource?)null);

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE,
            SelectedPage = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.Equal(ContentFamily.None, result.Family);
        Assert.Null(result.Item);
        Assert.True(result.SelectionFailed);
    }

    [Fact]
    public async Task ResolveLink_PageSource_LinksToTheSelectedPageItself()
    {
        var articlePage = new ArticlePage();
        webPageUrlRetrieverMock
            .Setup(x => x.Retrieve(articlePage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebPageUrl(PAGE_RELATIVE_URL, PAGE_RELATIVE_URL));

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE
        };

        var result = await contentPromotionService.ResolveLink(properties, articlePage);

        Assert.Equal(PAGE_RELATIVE_URL, result?.LinkUrl);
    }

    [Fact]
    public async Task ResolveLink_ContentItemSourceWithLinkTargetPage_LinksToTargetPage()
    {
        var targetPage = new ArticlePage();
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveWebPageByContentItemGuid(
                targetGuid, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(targetPage);
        webPageUrlRetrieverMock
            .Setup(x => x.Retrieve(targetPage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebPageUrl(TARGET_RELATIVE_URL, TARGET_RELATIVE_URL));

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.CONTENT_ITEM,
            LinkTargetPage = [new ContentItemReference { Identifier = targetGuid }]
        };

        var result = await contentPromotionService.ResolveLink(properties, null);

        Assert.Equal(TARGET_RELATIVE_URL, result?.LinkUrl);
    }

    [Fact]
    public async Task ResolveLink_ContentItemSourceWithTypedUrlOnly_LinksToTypedUrl()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.CONTENT_ITEM,
            LinkUrl = EXTERNAL_URL
        };

        var result = await contentPromotionService.ResolveLink(properties, null);

        Assert.Equal(EXTERNAL_URL, result?.LinkUrl);
    }

    [Fact]
    public async Task ResolveLink_BothTargetPageAndTypedUrlStored_TargetPageWins()
    {
        var targetPage = new ArticlePage();
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveWebPageByContentItemGuid(
                targetGuid, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(targetPage);
        webPageUrlRetrieverMock
            .Setup(x => x.Retrieve(targetPage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebPageUrl(TARGET_RELATIVE_URL, TARGET_RELATIVE_URL));

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.CONTENT_ITEM,
            LinkTargetPage = [new ContentItemReference { Identifier = targetGuid }],
            LinkUrl = EXTERNAL_URL
        };

        var result = await contentPromotionService.ResolveLink(properties, null);

        Assert.Equal(TARGET_RELATIVE_URL, result?.LinkUrl);
    }

    [Fact]
    public async Task ResolveLink_ContentItemSourceWithNoDestination_ReturnsNoLink()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.CONTENT_ITEM
        };

        var result = await contentPromotionService.ResolveLink(properties, null);

        Assert.Null(result);
    }

    [Fact]
    public async Task ResolveLink_ManualSourceWithNoDestination_ReturnsNoLink()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.MANUAL
        };

        var result = await contentPromotionService.ResolveLink(properties, null);

        Assert.Null(result);
    }

    [Fact]
    public async Task ResolveLink_UrlRetrieverReturnsEmptyUrl_ReturnsNoLinkRatherThanEmptyHref()
    {
        var articlePage = new ArticlePage();
        webPageUrlRetrieverMock
            .Setup(x => x.Retrieve(articlePage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebPageUrl(string.Empty, string.Empty));

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE
        };

        var result = await contentPromotionService.ResolveLink(properties, articlePage);

        Assert.Null(result);
    }

    [Fact]
    public async Task ResolveLink_OpenInNewTabSet_IsCarriedToTheLink()
    {
        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.MANUAL,
            LinkUrl = EXTERNAL_URL,
            OpenInNewTab = true
        };

        var result = await contentPromotionService.ResolveLink(properties, null);

        Assert.True(result?.OpenInNewTab);
    }

    [Fact]
    public async Task ResolvePromotedItem_PageLoadsButItsContentItemIsMissing_ReportsSelectionFailed()
    {
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveWebPageByContentItemGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(new ArticlePage { ArticlePageArticleContent = [] });

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE,
            SelectedPage = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.True(result.SelectionFailed);
        Assert.Null(result.Item);
    }

    [Fact]
    public async Task ResolvePromotedItem_SelectionIsAnUnsupportedType_ReportsSelectionFailed()
    {
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveWebPageByContentItemGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(new DownloadsPage());

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE,
            SelectedPage = [new ContentItemReference { Identifier = selectedGuid }]
        };

        var result = await contentPromotionService.ResolvePromotedItem(properties);

        Assert.True(result.SelectionFailed);
        Assert.Equal(ContentFamily.None, result.Family);
    }

    [Fact]
    public async Task ResolveExtras_ArticleHasCategories_ReturnsEveryCategoryName()
    {
        var article = new GeneralArticle
        {
            ArticleSchemaCategory =
            [
                new TagReference { Identifier = categoryGuid },
                new TagReference { Identifier = otherCategoryGuid }
            ]
        };

        taxonomyRetrieverMock
            .Setup(x => x.RetrieveTags(It.IsAny<IEnumerable<Guid>>(), LANGUAGE))
            .ReturnsAsync([
                TagWith(categoryGuid, CATEGORY_NAME),
                TagWith(otherCategoryGuid, OTHER_CATEGORY_NAME)
            ]);

        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Article,
            PromotedContent = article
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Equal([CATEGORY_NAME, OTHER_CATEGORY_NAME], result.Categories);
    }

    [Fact]
    public async Task ResolveExtras_ArticleHasNoCategories_ReturnsEmptyExtras()
    {
        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Article,
            PromotedContent = new GeneralArticle { ArticleSchemaCategory = [] }
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.False(result.HasContent);
    }

    [Fact]
    public async Task ResolveExtras_ExtrasSwitchedOff_ReturnsEmptyExtrasEvenThoughTheArticleHasCategories()
    {
        var article = new GeneralArticle
        {
            ArticleSchemaCategory = [new TagReference { Identifier = categoryGuid }]
        };

        taxonomyRetrieverMock
            .Setup(x => x.RetrieveTags(It.IsAny<IEnumerable<Guid>>(), LANGUAGE))
            .ReturnsAsync([TagWith(categoryGuid, CATEGORY_NAME)]);

        var properties = new ContentPromotionWidgetProperties { ShowExtras = false };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Article,
            PromotedContent = article
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.False(result.HasContent);
    }

    [Fact]
    public async Task ResolveExtras_ServiceHasBenefits_ReturnsEveryBenefitDescription()
    {
        var service = new Service
        {
            ServiceBenefits =
            [
                new Benefit { BenefitDescription = FIRST_BENEFIT },
                new Benefit { BenefitDescription = SECOND_BENEFIT }
            ]
        };

        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Service,
            PromotedContent = service
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Equal([FIRST_BENEFIT, SECOND_BENEFIT], result.Benefits);
    }

    [Fact]
    public async Task ResolveExtras_ServiceHasNoBenefits_ReturnsEmptyExtras()
    {
        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Service,
            PromotedContent = new Service { ServiceBenefits = [] }
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.False(result.HasContent);
    }

    [Fact]
    public async Task ResolveExtras_SelectedItemIsAVariant_ReturnsItsPrice()
    {
        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Product,
            PromotedContent = new CatFoodVariant { ProductPriceSchemaPrice = VARIANT_PRICE }
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Equal(VARIANT_PRICE, result.Price);
    }

    [Fact]
    public async Task ResolveExtras_VariantHasACatalogDiscount_ReturnsTheDiscountedPrice()
    {
        productServiceMock
            .Setup(x => x.GetCatalogPrice(It.IsAny<IProductSchema>()))
            .ReturnsAsync(DISCOUNTED_PRICE);

        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Product,
            PromotedContent = new CatFoodVariant { ProductPriceSchemaPrice = VARIANT_PRICE }
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Equal(DISCOUNTED_PRICE, result.Price);
    }

    // A parent product carries no price or stock of its own, but the product listing shows both
    // for exactly these items by falling back to their variants. A promotion card that showed
    // nothing would contradict the listing on the same page, so it falls back the same way.
    [Fact]
    public async Task ResolveExtras_SelectedItemIsAParentProduct_ReturnsTheCatalogPriceItsVariantsImply()
    {
        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Product,
            PromotedContent = new CatFood()
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Equal(VARIANT_PRICE, result.Price);
    }

    [Fact]
    public async Task ResolveExtras_SelectedItemIsAParentProduct_ReturnsTheListingStockStatus()
    {
        productServiceMock
            .Setup(x => x.GetListingStockForProduct(It.IsAny<IProductSchema>()))
            .ReturnsAsync(ProductStockEnum.LowStock);

        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Product,
            PromotedContent = new CatFood()
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Equal(ProductStockEnum.LowStock, result.StockStatus);
    }

    [Fact]
    public async Task ResolveExtras_ProductHasNoCatalogPrice_OmitsThePriceRatherThanShowingZero()
    {
        productServiceMock
            .Setup(x => x.GetCatalogPrice(It.IsAny<IProductSchema>()))
            .ReturnsAsync(0m);

        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Product,
            PromotedContent = new CatFood()
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Null(result.Price);
    }

    [Fact]
    public async Task ResolveExtras_SelectionIsAProductPage_ReturnsThePriceAndStockOfWhatItPromotes()
    {
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveWebPageByContentItemGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(new ProductPage { ProductPageProducts = [new CatFood()] });

        productServiceMock
            .Setup(x => x.GetListingStockForProduct(It.IsAny<IProductSchema>()))
            .ReturnsAsync(ProductStockEnum.InStock);

        var properties = new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.PAGE,
            SelectedPage = [new ContentItemReference { Identifier = selectedGuid }],
            ShowExtras = true
        };

        var promotedItem = await contentPromotionService.ResolvePromotedItem(properties);

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Equal(VARIANT_PRICE, result.Price);
        Assert.Equal(ProductStockEnum.InStock, result.StockStatus);
    }

    [Fact]
    public async Task ResolveExtras_ParentProductHasNoStockRecord_OmitsTheStockButKeepsThePrice()
    {
        productServiceMock
            .Setup(x => x.GetListingStockForProduct(It.IsAny<IProductSchema>()))
            .ReturnsAsync(ProductStockEnum.Unknown);

        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Product,
            PromotedContent = new CatFood()
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Null(result.StockStatus);
        Assert.Equal(VARIANT_PRICE, result.Price);
    }

    // The one case where the behaviour T6c originally specified survives: with no variants to
    // fall back to there is genuinely nothing to say, so the extras block disappears.
    [Fact]
    public async Task ResolveExtras_ParentProductHasNoVariantsAtAll_ReturnsEmptyExtras()
    {
        productServiceMock
            .Setup(x => x.GetCatalogPrice(It.IsAny<IProductSchema>()))
            .ReturnsAsync(0m);

        productServiceMock
            .Setup(x => x.GetListingStockForProduct(It.IsAny<IProductSchema>()))
            .ReturnsAsync(ProductStockEnum.Unknown);

        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Product,
            PromotedContent = new CatFood { ProductParentSchemaVariants = [] }
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Null(result.Price);
        Assert.Null(result.StockStatus);
        Assert.False(result.HasContent);
    }

    [Fact]
    public async Task ResolveExtras_VariantIsInStock_ReturnsTheInStockState()
    {
        productServiceMock
            .Setup(x => x.GetListingStockForProduct(It.IsAny<IProductSchema>()))
            .ReturnsAsync(ProductStockEnum.InStock);

        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Product,
            PromotedContent = new CatFoodVariant { ProductPriceSchemaPrice = VARIANT_PRICE }
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Equal(ProductStockEnum.InStock, result.StockStatus);
    }

    [Fact]
    public async Task ResolveExtras_VariantHasZeroStock_ReturnsTheOutOfStockStateRatherThanOmittingIt()
    {
        productServiceMock
            .Setup(x => x.GetListingStockForProduct(It.IsAny<IProductSchema>()))
            .ReturnsAsync(ProductStockEnum.OutOfStock);

        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Product,
            PromotedContent = new CatFoodVariant { ProductPriceSchemaPrice = VARIANT_PRICE }
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Equal(ProductStockEnum.OutOfStock, result.StockStatus);
    }

    [Fact]
    public async Task ResolveExtras_VariantHasNoStockRecord_OmitsTheStockState()
    {
        productServiceMock
            .Setup(x => x.GetListingStockForProduct(It.IsAny<IProductSchema>()))
            .ReturnsAsync(ProductStockEnum.Unknown);

        var properties = new ContentPromotionWidgetProperties { ShowExtras = true };
        var promotedItem = new PromotedItemResult
        {
            Family = ContentFamily.Product,
            PromotedContent = new CatFoodVariant { ProductPriceSchemaPrice = VARIANT_PRICE }
        };

        var result = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.Null(result.StockStatus);
        Assert.Equal(VARIANT_PRICE, result.Price);
    }
}
