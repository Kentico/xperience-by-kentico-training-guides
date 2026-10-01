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
                DescriptionHtml = new HtmlString(DESCRIPTION_MARKUP)
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
            DisplayValues = new ContentPromotionDisplayValues { Title = TITLE },
            Link = new LinkViewModel { LinkUrl = LINK_URL }
        };

        Assert.False(viewModel.IsMisconfigured);
        Assert.Equal(MisconfigurationReason.None, viewModel.MisconfigurationReason);
    }

    [Fact]
    public void MisconfigurationReason_SelectionUnsupported_ReportsTheUnsupportedPageType()
    {
        var model = new ContentPromotionWidgetViewModel
        {
            SelectionUnsupported = true
        };

        Assert.Equal(MisconfigurationReason.UnsupportedPageType, model.MisconfigurationReason);
        Assert.True(model.IsMisconfigured);
    }

    [Fact]
    public void MisconfigurationReason_SelectionUnsupported_WinsOverNothingAuthored()
    {
        // A container page resolves no values at all, so both states are true at once. The editor
        // needs to be told the page choice is wrong, not that they typed nothing in.
        var model = new ContentPromotionWidgetViewModel
        {
            SelectionUnsupported = true,
            SelectionFailed = false
        };

        Assert.Equal(MisconfigurationReason.UnsupportedPageType, model.MisconfigurationReason);
    }
}
