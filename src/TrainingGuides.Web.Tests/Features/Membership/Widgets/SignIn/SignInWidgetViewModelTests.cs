using TrainingGuides.Web.Features.Membership.Widgets.SignIn;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Membership.Widgets.SignIn;

public class SignInWidgetViewModelTests
{
    private readonly SignInWidgetViewModel viewModel;

    public SignInWidgetViewModelTests()
    {
        viewModel = new();
    }

    [Fact]
    public void WhenModelInitialized_ActionUrl_IsEmpty() => Assert.Equal(string.Empty, viewModel.ActionUrl);

    [Fact]
    public void WhenModelInitialized_DefaultRedirectPageGuid_IsEmpty() => Assert.Equal(Guid.Empty, viewModel.DefaultRedirectPageGuid);

    [Fact]
    public void WhenModelInitialized_DisplayForm_IsTrue() => Assert.True(viewModel.DisplayForm);

    [Fact]
    public void WhenModelInitialized_FormTitle_IsEmpty() => Assert.Equal(string.Empty, viewModel.FormTitle);

    [Fact]
    public void WhenModelInitialized_SubmitButtonText_IsEmpty() => Assert.Equal(string.Empty, viewModel.SubmitButtonText);

    [Fact]
    public void WhenModelInitialized_UserNameOrEmailLabel_IsEmpty() => Assert.Equal(string.Empty, viewModel.UserNameOrEmailLabel);

    [Fact]
    public void WhenModelInitialized_PasswordLabel_IsEmpty() => Assert.Equal(string.Empty, viewModel.PasswordLabel);

    [Fact]
    public void WhenModelInitialized_StaySignedInLabel_IsEmpty() => Assert.Equal(string.Empty, viewModel.StaySignedInLabel);

    [Fact]
    public void WhenModelInitialized_UserNameOrEmail_IsEmpty() => Assert.Equal(string.Empty, viewModel.UserNameOrEmail);

    [Fact]
    public void WhenModelInitialized_Password_IsEmpty() => Assert.Equal(string.Empty, viewModel.Password);

    [Fact]
    public void WhenModelInitialized_StaySignedIn_IsFalse() => Assert.False(viewModel.StaySignedIn);

    [Fact]
    public void IsMisconfigured_WhenActionUrlAndAllLabelsAreSet_ReturnsFalse()
    {
        var viewModelAllFieldsSet = new SignInWidgetViewModel
        {
            ActionUrl = "https://www.example.com/authenticate",
            SubmitButtonText = "Submit",
            UserNameOrEmailLabel = "Username",
            PasswordLabel = "Password",
            StaySignedInLabel = "Stay signed in"
        };

        Assert.False(viewModelAllFieldsSet.IsMisconfigured);
    }

    [Fact]
    public void IsMisconfigured_WhenActionUrlOrAnyLabelIsMissing_ReturnsTrue()
    {
        var viewModelActionUrlMissing = new SignInWidgetViewModel
        {
            SubmitButtonText = "Submit",
            UserNameOrEmailLabel = "Username",
            PasswordLabel = "Password",
            StaySignedInLabel = "Stay signed in"
        };
        var viewModelSubmitButtonTextMissing = new SignInWidgetViewModel
        {
            ActionUrl = "https://www.example.com/authenticate",
            UserNameOrEmailLabel = "Username",
            PasswordLabel = "Password",
            StaySignedInLabel = "Stay signed in"
        };
        var viewModelUserNameOrEmailLabelMissing = new SignInWidgetViewModel
        {
            ActionUrl = "https://www.example.com/authenticate",
            SubmitButtonText = "Submit",
            PasswordLabel = "Password",
            StaySignedInLabel = "Stay signed in"
        };
        var viewModelPasswordLabelMissing = new SignInWidgetViewModel
        {
            ActionUrl = "https://www.example.com/authenticate",
            SubmitButtonText = "Submit",
            UserNameOrEmailLabel = "Username",
            StaySignedInLabel = "Stay signed in"
        };
        var viewModelStaySignedInLabelMissing = new SignInWidgetViewModel
        {
            ActionUrl = "https://www.example.com/authenticate",
            SubmitButtonText = "Submit",
            UserNameOrEmailLabel = "Username",
            PasswordLabel = "Password"
        };
        Assert.True(viewModel.IsMisconfigured);
        Assert.True(viewModelActionUrlMissing.IsMisconfigured);
        Assert.True(viewModelSubmitButtonTextMissing.IsMisconfigured);
        Assert.True(viewModelUserNameOrEmailLabelMissing.IsMisconfigured);
        Assert.True(viewModelPasswordLabelMissing.IsMisconfigured);
        Assert.True(viewModelStaySignedInLabelMissing.IsMisconfigured);
    }
}