using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ContentPromotion.Widgets.ContentPromotion;

public class HideElementsDataProviderTests
{
    private readonly HideElementsDataProvider provider = new();

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

    [Fact]
    public async Task GetSelectedItemsAsync_StoredElements_ComeBackValidWithTheirText()
    {
        var result = (await provider.GetSelectedItemsAsync(
            [ContentPromotionElement.IMAGE, ContentPromotionElement.TITLE], CancellationToken.None)).ToList();

        Assert.Equal([ContentPromotionElement.IMAGE, ContentPromotionElement.TITLE], result.Select(item => item.Value));
        Assert.Equal(["Image", "Title"], result.Select(item => item.Text));
        Assert.All(result, item => Assert.True(item.IsValid));
    }

    // A value no element answers to - renamed, or typed into stored data by hand - is shown to the
    // editor as invalid so they can remove it, rather than silently dropped.
    [Fact]
    public async Task GetSelectedItemsAsync_UnknownStoredValue_ComesBackInvalid()
    {
        var result = (await provider.GetSelectedItemsAsync(["subtitle"], CancellationToken.None)).Single();

        Assert.False(result.IsValid);
    }
}
