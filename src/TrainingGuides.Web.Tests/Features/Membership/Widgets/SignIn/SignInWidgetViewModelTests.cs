using Xunit;
using TrainingGuides.Web.Features.Membership.Widgets.SignIn;

namespace TrainingGuides.Web.Tests.Features.Membership.Widgets.SignIn;

public class SignInWidgetViewModelTests
{
    private readonly SignInWidgetViewModel viewModel;

    public SignInWidgetViewModelTests()
    {
        viewModel = new();
    }

    [Fact]
    public void WhenModelInitialized_BaseUrl_IsEmpty() => Assert.Equal(string.Empty, viewModel.BaseUrl);

    [Fact]
    public void WhenModelInitialized_RedirectUrl_IsEmpty() => Assert.Equal(string.Empty, viewModel.RedirectUrl);

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
}