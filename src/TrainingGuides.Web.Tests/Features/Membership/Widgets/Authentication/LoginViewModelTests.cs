using Xunit;
using TrainingGuides.Web.Features.Widgets.Authentication;

namespace TrainingGuides.Web.Tests.Features.Articles;

public class LoginViewModelTests
{
    private readonly SignInViewModel loginViewModel;

    public LoginViewModelTests()
    {
        loginViewModel = new();
    }

    [Fact]
    public void WhenInitialized_UserNameOrEmail_IsEmpty() => Assert.Equal(string.Empty, loginViewModel.UserNameOrEmail);

    [Fact]
    public void WhenInitialized_Password_IsEmpty() => Assert.Equal(string.Empty, loginViewModel.Password);

    [Fact]
    public void WhenInitialized_StaySignedIn_IsFalse() => Assert.False(loginViewModel.StaySignedIn);
}