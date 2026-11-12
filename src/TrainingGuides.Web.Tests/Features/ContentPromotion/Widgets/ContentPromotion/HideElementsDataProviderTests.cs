using CMS.Core;
using Moq;
using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ContentPromotion.Widgets.ContentPromotion;

public class HideElementsDataProviderTests
{
    private const string KEY_PREFIX = "TrainingGuides.ContentPromotionWidget.HideElements.Options.";

    private static readonly Dictionary<string, string> english = new()
    {
        [KEY_PREFIX + "Title"] = "Title",
        [KEY_PREFIX + "Description"] = "Description",
        [KEY_PREFIX + "Image"] = "Image",
        [KEY_PREFIX + "CallToAction"] = "Call to action"
    };

    private static readonly Dictionary<string, string> french = new()
    {
        [KEY_PREFIX + "Title"] = "Titre",
        [KEY_PREFIX + "Description"] = "Description",
        [KEY_PREFIX + "Image"] = "Image",
        [KEY_PREFIX + "CallToAction"] = "Appel à l'action"
    };

    private readonly HideElementsDataProvider provider = CreateProvider(english);

    private static HideElementsDataProvider CreateProvider(Dictionary<string, string> resourceStrings)
    {
        var localizationService = new Mock<ILocalizationService>();
        localizationService
            .Setup(service => service.GetString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .Returns((string key, string _, bool _) => resourceStrings.GetValueOrDefault(key, key));

        return new HideElementsDataProvider(localizationService.Object);
    }

    [Fact]
    public async Task GetItemsAsync_NoSearchTerm_OffersEveryCardElement()
    {
        var result = await provider.GetItemsAsync(string.Empty, 0, CancellationToken.None);

        Assert.Equal(
            [
                ContentPromotionElement.TITLE,
                ContentPromotionElement.DESCRIPTION,
                ContentPromotionElement.IMAGE,
                ContentPromotionElement.CALL_TO_ACTION
            ],
            result.Items.Select(item => item.Value));
        Assert.False(result.NextPageAvailable);
    }

    // The editor types what they see, so the search matches the displayed text, not the stored value.
    [Fact]
    public async Task GetItemsAsync_SearchTerm_OffersOnlyElementsWhoseTextContainsIt()
    {
        var result = await provider.GetItemsAsync("ACTION", 0, CancellationToken.None);

        Assert.Equal([ContentPromotionElement.CALL_TO_ACTION], result.Items.Select(item => item.Value));
    }

    // An editor using the admin UI in another language searches in that language.
    [Fact]
    public async Task GetItemsAsync_SearchTerm_MatchesTheLocalizedText()
    {
        var result = await CreateProvider(french).GetItemsAsync("appel", 0, CancellationToken.None);

        Assert.Equal([ContentPromotionElement.CALL_TO_ACTION], result.Items.Select(item => item.Value));
    }

    [Fact]
    public async Task GetSelectedItemsAsync_StoredElements_ComeBackValidWithTheirText()
    {
        var result = (await provider.GetSelectedItemsAsync(
            [ContentPromotionElement.IMAGE, ContentPromotionElement.TITLE], CancellationToken.None)).ToList();

        Assert.Equal([ContentPromotionElement.IMAGE, ContentPromotionElement.TITLE], result.Select(item => item.Value));
        Assert.Equal(["Image", "Title"], result.Select(item => item.Text));
        Assert.All(result, item => Assert.True(item.IsValid));
    }

    [Fact]
    public async Task GetSelectedItemsAsync_StoredElements_ComeBackWithLocalizedText()
    {
        var result = (await CreateProvider(french).GetSelectedItemsAsync(
            [ContentPromotionElement.TITLE, ContentPromotionElement.CALL_TO_ACTION], CancellationToken.None)).ToList();

        Assert.Equal(["Titre", "Appel à l'action"], result.Select(item => item.Text));
    }

    // A value no element answers to - renamed, or typed into stored data by hand - is shown to the
    // editor as invalid so they can remove it, rather than silently dropped.
    [Fact]
    public async Task GetSelectedItemsAsync_UnknownStoredValue_ComesBackInvalid()
    {
        var result = (await provider.GetSelectedItemsAsync(["subtitle"], CancellationToken.None)).Single();

        Assert.False(result.IsValid);
        Assert.Equal("subtitle", result.Text);
    }
}
