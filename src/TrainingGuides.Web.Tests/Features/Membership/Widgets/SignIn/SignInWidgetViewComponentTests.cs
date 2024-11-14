using Kentico.Content.Web.Mvc.Routing;
using Moq;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.SignIn;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Membership.Widgets.SignIn;

public class SignInWidgetViewComponentTests
{
    private readonly SignInWidgetViewComponent viewComponent;
    private readonly Mock<IHttpRequestService> httpRequestServiceMock;
    private readonly Mock<IMembershipService> membershipServiceMock;
    private readonly Mock<IPreferredLanguageRetriever> preferredLanguageRetrieverMock;

    private const string BASE_URL = "http://localhost:5000";
    private const string PAGE_URL = "/page";
    private const string ROOT_URL = "/";
    private const string SIGN_IN = "Sign In";
    private const string SUBMIT = "Submit";
    private const string USERNAME = "Username";
    private const string PASSWORD = "Password";
    private const string STAY_SIGNED_IN = "Stay Signed In";

    private readonly SignInWidgetProperties referenceProperties;

    public SignInWidgetViewComponentTests()
    {
        httpRequestServiceMock = new Mock<IHttpRequestService>();
        httpRequestServiceMock.Setup(x => x.GetBaseUrl()).Returns(BASE_URL);

        membershipServiceMock = new Mock<IMembershipService>();
        membershipServiceMock.Setup(x => x.IsMemberAuthenticated()).ReturnsAsync(true);

        preferredLanguageRetrieverMock = new Mock<IPreferredLanguageRetriever>();
        preferredLanguageRetrieverMock.Setup(x => x.Get()).Returns("en");

        viewComponent = new SignInWidgetViewComponent(httpRequestServiceMock.Object, membershipServiceMock.Object, preferredLanguageRetrieverMock.Object);

        referenceProperties = new SignInWidgetProperties()
        {
            FormTitle = SIGN_IN,
            SubmitButtonText = SUBMIT,
            UserNameLabel = USERNAME,
            PasswordLabel = PASSWORD,
            StaySignedInLabel = STAY_SIGNED_IN
        };
    }

    [Fact]
    public async Task BuildWidgetViewModel_ReturnsWidgetViewModel_WithBaseUrlSet()
    {
        var viewModel = await viewComponent.BuildWidgetViewModel(referenceProperties);
        Assert.NotEmpty(viewModel.BaseUrl);
    }

    [Fact]
    public async Task BuildWidgetViewModel_WhenUserSetsRedirectPage_SetsRedirectUrl_ToPageUrl()
    {
        httpRequestServiceMock.Setup(x => x.GetPageRelativeUrl(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync($"~{PAGE_URL}");
        referenceProperties.RedirectPage = [new() { WebPageGuid = Guid.NewGuid() }];

        var viewModel = await viewComponent.BuildWidgetViewModel(referenceProperties);
        Assert.Equal(PAGE_URL, viewModel.RedirectUrl);
    }

    [Fact]
    public async Task BuildWidgetViewModel_WhenUserDoesNOTSetRedirectPage_SetsRedirectUrl_ToRoot()
    {
        var viewModel = await viewComponent.BuildWidgetViewModel(referenceProperties);
        Assert.Equal(ROOT_URL, viewModel.RedirectUrl);
    }

    [Fact]
    public async Task BuildWidgetViewModel_WhenUserIsAuthenticated_SetsDisplayForm_ToFalse()
    {
        membershipServiceMock.Setup(x => x.IsMemberAuthenticated()).ReturnsAsync(true);

        var viewModel = await viewComponent.BuildWidgetViewModel(referenceProperties);
        Assert.False(viewModel.DisplayForm);
    }

    [Fact]
    public async Task BuildWidgetViewModel_WhenUserIsNOTAuthenticated_SetsDisplayForm_ToTrue()
    {
        membershipServiceMock.Setup(x => x.IsMemberAuthenticated()).ReturnsAsync(false);

        var viewModel = await viewComponent.BuildWidgetViewModel(referenceProperties);
        Assert.True(viewModel.DisplayForm);
    }

    [Fact]
    public async Task BuildWidgetViewModel_SetsFormLabels_BasedOnWidgetProperties()
    {
        var viewModel = await viewComponent.BuildWidgetViewModel(referenceProperties);
        Assert.Equal(referenceProperties.FormTitle, viewModel.FormTitle);
        Assert.Equal(referenceProperties.SubmitButtonText, viewModel.SubmitButtonText);
        Assert.Equal(referenceProperties.UserNameLabel, viewModel.UserNameOrEmailLabel);
        Assert.Equal(referenceProperties.PasswordLabel, viewModel.PasswordLabel);
        Assert.Equal(referenceProperties.StaySignedInLabel, viewModel.StaySignedInLabel);
    }

}