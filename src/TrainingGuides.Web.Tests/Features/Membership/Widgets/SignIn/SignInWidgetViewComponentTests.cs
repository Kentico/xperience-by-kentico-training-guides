using Microsoft.AspNetCore.Http;
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

    private const string BASE_URL = "http://localhost:5000";
    private const string ACTION_URL = "http://localhost:5000/authenticate";
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
        httpRequestServiceMock.Setup(x => x.GetAbsoluteUrlForPath(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<QueryString?>())).Returns(ACTION_URL);

        membershipServiceMock = new Mock<IMembershipService>();
        membershipServiceMock.Setup(x => x.IsMemberAuthenticated()).ReturnsAsync(true);

        viewComponent = new SignInWidgetViewComponent(httpRequestServiceMock.Object, membershipServiceMock.Object);

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
    public async Task BuildWidgetViewModel_SetsActionUrl_FromHttpRequestService()
    {
        var viewModel = await viewComponent.BuildWidgetViewModel(referenceProperties);
        Assert.Equal(ACTION_URL, viewModel.ActionUrl);
    }

    [Fact]
    public async Task BuildWidgetViewModel_WhenUserSetsRedirectPage_SetsDefaultRedirectPageGuid_ToWebPageGuid()
    {
        var testGuid = Guid.NewGuid();
        referenceProperties.DefaultRedirectPage = [new() { WebPageGuid = testGuid }];

        var viewModel = await viewComponent.BuildWidgetViewModel(referenceProperties);
        Assert.Equal(testGuid, viewModel.DefaultRedirectPageGuid);
    }

    [Fact]
    public async Task BuildWidgetViewModel_WhenUserDoesNOTSetRedirectPage_SetsDefaultRedirectPageGuid_ToEmptyGuid()
    {
        var viewModel = await viewComponent.BuildWidgetViewModel(referenceProperties);
        Assert.Equal(Guid.Empty, viewModel.DefaultRedirectPageGuid);
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