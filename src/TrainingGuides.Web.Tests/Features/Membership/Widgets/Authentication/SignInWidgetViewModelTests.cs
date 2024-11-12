using Xunit;
using TrainingGuides.Web.Features.Membership.Widgets.Authentication;

namespace TrainingGuides.Web.Tests.Features.Membership.Widgets.Authentication;

public class SignInWidgetViewModelTests
{
    private readonly SignInWidgetViewModel viewModel;

    public SignInWidgetViewModelTests()
    {
        viewModel = new();
    }

    [Fact]
    public void WhenInitialized_BaseUrl_IsEmpty() => Assert.Equal(string.Empty, viewModel.BaseUrl);

    [Fact]
    public void WhenInitialized_DisplayForm_IsTrue() => Assert.True(viewModel.DisplayForm);

    [Fact]
    public void WhenInitialized_FormTitle_IsEmpty() => Assert.Equal(string.Empty, viewModel.FormTitle);

    [Fact]
    public void WhenInitialized_SubmitButtonText_IsEmpty() => Assert.Equal(string.Empty, viewModel.SubmitButtonText);

    [Fact]
    public void WhenInitialized_UserNameOrEmailLabel_IsEmpty() => Assert.Equal(string.Empty, viewModel.UserNameOrEmailLabel);

    [Fact]
    public void WhenInitialized_PasswordLabel_IsEmpty() => Assert.Equal(string.Empty, viewModel.PasswordLabel);

    [Fact]
    public void WhenInitialized_StaySignedInLabel_IsEmpty() => Assert.Equal(string.Empty, viewModel.StaySignedInLabel);

    [Fact]
    public void WhenInitialized_UserNameOrEmail_IsEmpty() => Assert.Equal(string.Empty, viewModel.UserNameOrEmail);

    [Fact]
    public void WhenInitialized_Password_IsEmpty() => Assert.Equal(string.Empty, viewModel.Password);

    [Fact]
    public void WhenInitialized_StaySignedIn_IsFalse() => Assert.False(viewModel.StaySignedIn);
}