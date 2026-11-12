using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using TrainingGuides.Web.Features.Shared.Models;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ContentPromotion.Widgets.ContentPromotion;

public class ContentPromotionWidgetViewModelTests
{
    private const string TITLE = "Autumn savings on pet cover";
    private const string LINK_URL = "/pet-insurance";
    private const string DESCRIPTION_MARKUP = "<p>Cover your cat this autumn for less.</p>";
    private const string CALL_TO_ACTION = "Get a quote";

    [Fact]
    public void ManualModeWithNothingAuthored_IsMisconfiguredForNothingAuthored()
    {
        var viewModel = new ContentPromotionWidgetViewModel();

        Assert.True(viewModel.IsMisconfigured);
        Assert.Equal(MisconfigurationReason.NothingAuthored, viewModel.MisconfigurationReason);
    }

    [Fact]
    public void SelectionCouldNotBeLoaded_IsMisconfiguredForItemCouldNotBeLoaded()
    {
        var viewModel = new ContentPromotionWidgetViewModel
        {
            SelectionFailed = true
        };

        Assert.True(viewModel.IsMisconfigured);
        Assert.Equal(MisconfigurationReason.ItemCouldNotBeLoaded, viewModel.MisconfigurationReason);
    }

    [Fact]
    public void ItemResolvedButNoDestination_IsNotMisconfiguredAndReportsNoDestination()
    {
        var viewModel = new ContentPromotionWidgetViewModel
        {
            DisplayValues = new ContentPromotionDisplayValues { Title = TITLE },
            Link = null
        };

        Assert.False(viewModel.IsMisconfigured);
        Assert.Equal(MisconfigurationReason.NoDestination, viewModel.MisconfigurationReason);
    }

    // The description is the one display value that is markup rather than a string, so the
    // "is there anything to show" test has to look inside it.
    [Fact]
    public void OnlyADescriptionWasResolved_IsNotMisconfiguredForNothingAuthored()
    {
        var viewModel = new ContentPromotionWidgetViewModel
        {
            DisplayValues = new ContentPromotionDisplayValues
            {
                DescriptionHtml = new HtmlString(DESCRIPTION_MARKUP),
                CallToActionText = CALL_TO_ACTION
            },
            Link = new LinkViewModel { LinkUrl = LINK_URL }
        };

        Assert.False(viewModel.IsMisconfigured);
        Assert.Equal(MisconfigurationReason.None, viewModel.MisconfigurationReason);
    }

    [Fact]
    public void FullyConfigured_IsNotMisconfiguredAndReportsNoReason()
    {
        var viewModel = new ContentPromotionWidgetViewModel
        {
            DisplayValues = new ContentPromotionDisplayValues { Title = TITLE, CallToActionText = CALL_TO_ACTION },
            Link = new LinkViewModel { LinkUrl = LINK_URL }
        };

        Assert.False(viewModel.IsMisconfigured);
        Assert.Equal(MisconfigurationReason.None, viewModel.MisconfigurationReason);
    }

    // The card has somewhere to go but no anchor to get there - the view renders the anchor only
    // when there is call to action text. The public still sees the card, so it is a warning.
    [Fact]
    public void DestinationButNoCallToActionText_IsNotMisconfiguredAndReportsCallToActionMissing()
    {
        var viewModel = new ContentPromotionWidgetViewModel
        {
            DisplayValues = new ContentPromotionDisplayValues { Title = TITLE },
            Link = new LinkViewModel { LinkUrl = LINK_URL }
        };

        Assert.False(viewModel.IsMisconfigured);
        Assert.Equal(MisconfigurationReason.CallToActionMissing, viewModel.MisconfigurationReason);
    }

    // Hiding the call to action is a deliberate editor choice, so the notice names that choice
    // rather than asking for text the editor has chosen not to show.
    [Fact]
    public void DestinationButCallToActionHidden_IsNotMisconfiguredAndReportsCallToActionHidden()
    {
        var viewModel = new ContentPromotionWidgetViewModel
        {
            DisplayValues = new ContentPromotionDisplayValues { Title = TITLE },
            Link = new LinkViewModel { LinkUrl = LINK_URL },
            CallToActionHidden = true
        };

        Assert.False(viewModel.IsMisconfigured);
        Assert.Equal(MisconfigurationReason.CallToActionHidden, viewModel.MisconfigurationReason);
    }
}
