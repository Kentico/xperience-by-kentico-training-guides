using CMS.ContentEngine;
using CMS.Websites;
using Kentico.Content.Web.Mvc.Routing;
using Moq;
using TrainingGuides.Web.Commerce.Products.Models;
using TrainingGuides.Web.Commerce.Products.Services;
using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.ContentPromotion.Services;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ContentPromotion.Services;

/// <summary>
/// The promoted item's content family is decided twice: once when the card's values are mapped and
/// again when the extras are read, because <see cref="PromotedItemResult.PromotedContent"/> is an
/// <c>object</c>. A family missing from the second switch still builds and renders, just with no
/// extras. These tests run every content type the widget offers through both decisions, so a type
/// added to <see cref="ContentPromotionContentTypes"/> without an extras case fails here.
/// </summary>
public class ContentPromotionFamilyCoverageTests
{
    private const string CATEGORY_NAME = "Insurance";
    private const string BENEFIT = "Vet visits covered from day one.";
    private const decimal PRICE = 24.99m;

    private static readonly Guid selectedGuid = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid categoryGuid = new("33333333-3333-3333-3333-333333333333");

    /// <summary>
    /// One selection per content type, populated so that its family has extras to show. Product
    /// extras come from the product service mock, so any product will do.
    /// </summary>
    private static readonly Dictionary<string, Func<object>> examples = new()
    {
        [ArticlePage.CONTENT_TYPE_NAME] = () => new ArticlePage { ArticlePageArticleContent = [CategorizedArticle()] },
        [ProductPage.CONTENT_TYPE_NAME] = () => new ProductPage { ProductPageProducts = [new CatFood()] },
        [ServicePage.CONTENT_TYPE_NAME] = () => new ServicePage { ServicePageService = [ServiceWithBenefit()] },
        [GeneralArticle.CONTENT_TYPE_NAME] = CategorizedArticle,
        [Interview.CONTENT_TYPE_NAME] = () => new Interview { ArticleSchemaCategory = [new TagReference { Identifier = categoryGuid }] },
        [Service.CONTENT_TYPE_NAME] = ServiceWithBenefit,
        [CatFood.CONTENT_TYPE_NAME] = () => new CatFood(),
        [DogCollar.CONTENT_TYPE_NAME] = () => new DogCollar(),
        [CatFoodVariant.CONTENT_TYPE_NAME] = () => new CatFoodVariant(),
        [DogCollarVariant.CONTENT_TYPE_NAME] = () => new DogCollarVariant()
    };

    public static TheoryData<string> OfferedContentTypes => new(AllOfferedContentTypes());

    private readonly Mock<IContentItemRetrieverService> contentItemRetrieverServiceMock = new();
    private readonly Mock<ITaxonomyRetriever> taxonomyRetrieverMock = new();
    private readonly Mock<IPreferredLanguageRetriever> preferredLanguageRetrieverMock = new();
    private readonly Mock<IProductService> productServiceMock = new();
    private readonly ContentPromotionService contentPromotionService;

    public ContentPromotionFamilyCoverageTests()
    {
        preferredLanguageRetrieverMock
            .Setup(x => x.Get())
            .Returns("en");

        taxonomyRetrieverMock
            .Setup(x => x.RetrieveTags(It.IsAny<IEnumerable<Guid>>(), It.IsAny<string>()))
            .ReturnsAsync([new Tag { Identifier = categoryGuid, Title = CATEGORY_NAME }]);

        productServiceMock
            .Setup(x => x.GetCatalogPrice(It.IsAny<IProductSchema>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PRICE);

        productServiceMock
            .Setup(x => x.GetListingStockForProduct(It.IsAny<IProductSchema>()))
            .ReturnsAsync(ProductStockEnum.InStock);

        contentPromotionService = new ContentPromotionService(
            contentItemRetrieverServiceMock.Object,
            Mock.Of<IWebPageUrlRetriever>(),
            taxonomyRetrieverMock.Object,
            preferredLanguageRetrieverMock.Object,
            productServiceMock.Object);
    }

    [Fact]
    public void Examples_CoverExactlyTheContentTypesTheWidgetOffers() =>
        Assert.Equal(
            AllOfferedContentTypes().Order(),
            examples.Keys.Order());

    [Theory]
    [MemberData(nameof(OfferedContentTypes))]
    public async Task ResolveExtras_EveryOfferedContentType_ReturnsExtrasForItsFamily(string contentTypeName)
    {
        var properties = SelectionOf(contentTypeName);

        var promotedItem = await contentPromotionService.ResolvePromotedItem(properties);
        var extras = await contentPromotionService.ResolveExtras(properties, promotedItem);

        Assert.False(promotedItem.SelectionFailed, $"{contentTypeName} is not mapped to a content family.");
        Assert.True(extras.HasContent, $"{contentTypeName} is mapped to a content family that has no extras.");
    }

    private static IEnumerable<string> AllOfferedContentTypes() =>
        ContentPromotionContentTypes.PAGES.Concat(ContentPromotionContentTypes.CONTENT_ITEMS);

    /// <summary>
    /// Selects the content type's example from the selector that offers it, so it reaches the
    /// service the same way an editor's selection does.
    /// </summary>
    private ContentPromotionWidgetProperties SelectionOf(string contentTypeName)
    {
        object example = examples[contentTypeName]();
        var selection = new ContentItemReference { Identifier = selectedGuid };

        if (ContentPromotionContentTypes.PAGES.Contains(contentTypeName))
        {
            contentItemRetrieverServiceMock
                .Setup(x => x.RetrieveWebPageByContentItemGuid(
                    selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
                .ReturnsAsync((IWebPageFieldsSource)example);

            return new ContentPromotionWidgetProperties
            {
                ContentSource = ContentPromotionSource.PAGE,
                SelectedPage = [selection],
                ShowExtras = true
            };
        }

        contentItemRetrieverServiceMock
            .Setup(x => x.RetrieveContentItemByGuid(
                selectedGuid, It.IsAny<IEnumerable<string>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync((IContentItemFieldsSource)example);

        return new ContentPromotionWidgetProperties
        {
            ContentSource = ContentPromotionSource.CONTENT_ITEM,
            SelectedContentItem = [selection],
            ShowExtras = true
        };
    }

    private static GeneralArticle CategorizedArticle() => new()
    {
        ArticleSchemaCategory = [new TagReference { Identifier = categoryGuid }]
    };

    private static Service ServiceWithBenefit() => new()
    {
        ServiceBenefits = [new Benefit { BenefitDescription = BENEFIT }]
    };
}
