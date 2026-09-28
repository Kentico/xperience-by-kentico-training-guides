using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using TrainingGuides.Web.Features.Shared.Models;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ContentPromotion.Widgets.ContentPromotion;

public class ContentPromotionWidgetViewModelTests
{
    private const string TITLE = "Autumn savings on pet cover";
    private const string LINK_URL = "/pet-insurance";

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
}
