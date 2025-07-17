using TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Membership.Widgets.LinkOrSignOut;

public class LinkOrSignOutWidgetViewModelTests
{
    private readonly LinkOrSignOutWidgetViewModel viewModel;

    public LinkOrSignOutWidgetViewModelTests()
    {
        viewModel = new();
    }

    [Fact]
    public void WhenModelInitialized_Text_IsEmpty() => Assert.Equal(string.Empty, viewModel.Text);

    [Fact]
    public void WhenModelInitialized_ButtonText_IsEmpty() => Assert.Equal(string.Empty, viewModel.ButtonText);

    [Fact]
    public void WhenModelInitialized_Url_IsEmpty() => Assert.Equal(string.Empty, viewModel.Url);

    [Fact]
    public void IsMisconfigured_WhenButtonTextAndUrlAreSet_ReturnsFalse()
    {
        var viewModelAllFieldsSet = new LinkOrSignOutWidgetViewModel
        {
            Text = "Text",
            ButtonText = "Button",
            Url = "https://www.example.com"
        };

        Assert.False(viewModelAllFieldsSet.IsMisconfigured);
    }

    [Fact]
    public void IsMisconfigured_WhenButtonTextIsMissing_ReturnsTrue()
    {
        var viewModelAllFieldsSet = new LinkOrSignOutWidgetViewModel
        {
            Text = "Text",
            Url = "https://www.example.com"
        };

        Assert.True(viewModelAllFieldsSet.IsMisconfigured);
    }

    [Fact]
    public void IsMisconfigured_WhenButtonUrlIsMissing_ReturnsTrue()
    {
        var viewModelAllFieldsSet = new LinkOrSignOutWidgetViewModel
        {
            Text = "Text",
            ButtonText = "Button",
        };

        Assert.True(viewModelAllFieldsSet.IsMisconfigured);
    }
}
