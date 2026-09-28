using CMS.ContentEngine;
using CMS.Websites;
using Moq;
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

    private static readonly Guid targetGuid = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid selectedGuid = new("11111111-1111-1111-1111-111111111111");

    private readonly Mock<IContentItemRetrieverService> contentItemRetrieverServiceMock = new();
    private readonly Mock<IWebPageUrlRetriever> webPageUrlRetrieverMock = new();
    private readonly ContentPromotionService contentPromotionService;

    public ContentPromotionServiceTests()
    {
        contentPromotionService = new ContentPromotionService(
            contentItemRetrieverServiceMock.Object,
            webPageUrlRetrieverMock.Object);
    }

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

        Assert.Equal(WIDGET_DESCRIPTION, result.Description);
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

        Assert.Equal(ITEM_DESCRIPTION, result.Description);
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

        Assert.Equal(string.Empty, result.Description);
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
        Assert.Equal(string.Empty, result.Description);
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
                selectedGuid, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
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
                selectedGuid, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
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
                selectedGuid, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
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
    public async Task ResolvePromotedItem_ContentItemSourceWithProductVariant_ReturnsProductFamily()
    {
        var variant = new CatFoodVariant
        {
            ProductSchemaName = ITEM_TITLE,
            ProductSchemaDescription = ITEM_DESCRIPTION
        };
        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveContentItemByGuid(
                selectedGuid, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
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
                selectedGuid, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
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
                selectedGuid, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
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
                selectedGuid, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
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
                selectedGuid, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
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
}
