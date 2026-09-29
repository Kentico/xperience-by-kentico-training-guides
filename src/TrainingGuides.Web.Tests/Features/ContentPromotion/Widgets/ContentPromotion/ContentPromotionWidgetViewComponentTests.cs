using CMS.Websites;
using Moq;
using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.ContentPromotion.Services;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ContentPromotion.Widgets.ContentPromotion;

/// <summary>
/// Covers the one styling rule with real branching - whether the chosen color scheme reaches the
/// card. Every other styling decision is verified by eye, per the widget spec section 2.
/// </summary>
public class ContentPromotionWidgetViewComponentTests
{
    private const string TITLE = "Autumn savings on pet cover";
    private const string COLOR_SCHEME_CLASS = "tg-bg-light-1";
    private const string TEXT_COLOR_CLASS = "tg-txt-dark";
    private const string CORNER_STYLE_CLASS = "tg-corner-shrp";

    private readonly Mock<IContentPromotionService> contentPromotionServiceMock = new();
    private readonly Mock<IComponentStyleEnumService> componentStyleEnumServiceMock = new();
    private readonly ContentPromotionWidgetViewComponent viewComponent;

    public ContentPromotionWidgetViewComponentTests()
    {
        contentPromotionServiceMock
            .Setup(x => x.ResolvePromotedItem(It.IsAny<ContentPromotionWidgetProperties>()))
            .ReturnsAsync(new PromotedItemResult());

        contentPromotionServiceMock
            .Setup(x => x.ResolveOverrideImage(It.IsAny<ContentPromotionWidgetProperties>()))
            .ReturnsAsync((AssetViewModel?)null);

        contentPromotionServiceMock
            .Setup(x => x.ResolveDisplayValues(
                It.IsAny<ContentPromotionWidgetProperties>(),
                It.IsAny<PromotedItemSource?>(),
                It.IsAny<AssetViewModel?>()))
            .Returns(new ContentPromotionDisplayValues { Title = TITLE });

        contentPromotionServiceMock
            .Setup(x => x.ResolveLink(
                It.IsAny<ContentPromotionWidgetProperties>(),
                It.IsAny<IWebPageFieldsSource?>()))
            .ReturnsAsync((LinkViewModel?)null);

        contentPromotionServiceMock
            .Setup(x => x.ResolveExtras(
                It.IsAny<ContentPromotionWidgetProperties>(),
                It.IsAny<PromotedItemResult>()))
            .ReturnsAsync(new PromotionExtrasViewModel());

        componentStyleEnumServiceMock
            .Setup(x => x.GetColorScheme(It.IsAny<string>()))
            .Returns(ColorSchemeOption.Light1);

        componentStyleEnumServiceMock
            .Setup(x => x.GetColorSchemeClasses(ColorSchemeOption.Light1))
            .Returns([COLOR_SCHEME_CLASS, TEXT_COLOR_CLASS]);

        componentStyleEnumServiceMock
            .Setup(x => x.GetLinkStyle(It.IsAny<string>()))
            .Returns(ColorSchemeOption.Dark2);

        componentStyleEnumServiceMock
            .Setup(x => x.GetColorSchemeClasses(ColorSchemeOption.Dark2))
            .Returns(["tg-bg-secondary", "tg-txt-light"]);

        componentStyleEnumServiceMock
            .Setup(x => x.GetCornerStyle(It.IsAny<string>()))
            .Returns(CornerStyleOption.Sharp);

        componentStyleEnumServiceMock
            .Setup(x => x.GetCornerStyleClasses(CornerStyleOption.Sharp))
            .Returns([CORNER_STYLE_CLASS]);

        viewComponent = new ContentPromotionWidgetViewComponent(
            contentPromotionServiceMock.Object,
            componentStyleEnumServiceMock.Object);
    }

    private static ContentPromotionWidgetProperties PropertiesWithDesign(CardDesignOption cardDesign) => new()
    {
        ShowAdvanced = true,
        CardDesign = cardDesign.ToString(),
        ColorScheme = nameof(ColorSchemeOption.Light1)
    };

    [Fact]
    public async Task StandardDesign_PutsTheColorSchemeClassOnTheCard()
    {
        var viewModel = await viewComponent.BuildWidgetViewModel(PropertiesWithDesign(CardDesignOption.Standard));

        Assert.Contains(COLOR_SCHEME_CLASS, viewModel.CardCssClasses.Split(' '));
        Assert.Contains(TEXT_COLOR_CLASS, viewModel.CardCssClasses.Split(' '));
    }

    [Theory]
    [InlineData(CardDesignOption.ImageOverlay)]
    [InlineData(CardDesignOption.Gradient)]
    public async Task DesignThatPaintsItsOwnBackground_LeavesTheColorSchemeClassOff(CardDesignOption cardDesign)
    {
        var viewModel = await viewComponent.BuildWidgetViewModel(PropertiesWithDesign(cardDesign));

        Assert.DoesNotContain(COLOR_SCHEME_CLASS, viewModel.CardCssClasses.Split(' '));
        Assert.DoesNotContain(TEXT_COLOR_CLASS, viewModel.CardCssClasses.Split(' '));
    }
}
