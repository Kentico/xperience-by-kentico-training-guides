using Kentico.Content.Web.Mvc.Routing;
using Moq;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.Registration;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Membership.Widgets.Registration;
public class RegistrationWidgetViewComponentTests
{
    private readonly RegistrationWidgetViewComponent viewComponent;
    private readonly Mock<IHttpRequestService> httpRequestServiceMock;
    private readonly Mock<IMembershipService> membershipServiceMock;
    private readonly Mock<IPreferredLanguageRetriever> preferredLanguageRetrieverMock;
    private readonly RegistrationWidgetProperties referenceProperties;

    private const string FORM_TITLE = "Register";
    private const string SUBMIT_BUTTON_TEXT = "Submit";
    private const string USERNAME_LABEL = "Username";
    private const string EMAIL_ADDRESS_LABEL = "Email";
    private const string PASSWORD_LABEL = "Password";
    private const string CONFIRM_PASSWORD_LABEL = "Confirm Password";
    private const string GIVEN_NAME_LABEL = "Given Name";
    private const string FAMILY_NAME_LABEL = "Family Name";
    private const string FAMILY_NAME_FIRST_LABEL = "Family Name First";
    private const string FAVORITE_COFFEE_LABEL = "Favorite Coffee";
    private const string BASE_URL = "http://localhost:5000";

    public RegistrationWidgetViewComponentTests()
    {
        httpRequestServiceMock = new Mock<IHttpRequestService>();
        httpRequestServiceMock.Setup(x => x.GetBaseUrlWithLanguage()).Returns(BASE_URL);
        httpRequestServiceMock.Setup(x => x.GetBaseUrl()).Returns(BASE_URL);

        preferredLanguageRetrieverMock = new Mock<IPreferredLanguageRetriever>();
        preferredLanguageRetrieverMock.Setup(x => x.Get()).Returns("en");

        membershipServiceMock = new Mock<IMembershipService>();
        membershipServiceMock.Setup(x => x.IsMemberAuthenticated()).ReturnsAsync(true);

        viewComponent = new RegistrationWidgetViewComponent(httpRequestServiceMock.Object, membershipServiceMock.Object, preferredLanguageRetrieverMock.Object);

        referenceProperties = new RegistrationWidgetProperties()
        {
            FormTitle = FORM_TITLE,
            SubmitButtonText = SUBMIT_BUTTON_TEXT,
            UserNameLabel = USERNAME_LABEL,
            EmailAddressLabel = EMAIL_ADDRESS_LABEL,
            PasswordLabel = PASSWORD_LABEL,
            ConfirmPasswordLabel = CONFIRM_PASSWORD_LABEL,
            ShowName = true,
            ShowExtraFields = true,
            GivenNameLabel = GIVEN_NAME_LABEL,
            FamilyNameLabel = FAMILY_NAME_LABEL,
            FamilyNameFirstLabel = FAMILY_NAME_FIRST_LABEL,
            FavoriteCoffeeLabel = FAVORITE_COFFEE_LABEL
        };
    }

    [Fact]
    public async Task BuildWidgetViewModel_ReturnsWidgetViewModel_WithBaseUrlSet()
    {
        var viewModel = await viewComponent.BuildWidgetViewModel(referenceProperties);
        Assert.NotEmpty(viewModel.BaseUrl);
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
        Assert.Equal(referenceProperties.UserNameLabel, viewModel.UserNameLabel);
        Assert.Equal(referenceProperties.EmailAddressLabel, viewModel.EmailAddressLabel);
        Assert.Equal(referenceProperties.PasswordLabel, viewModel.PasswordLabel);
        Assert.Equal(referenceProperties.ConfirmPasswordLabel, viewModel.ConfirmPasswordLabel);
        Assert.Equal(referenceProperties.GivenNameLabel, viewModel.GivenNameLabel);
        Assert.Equal(referenceProperties.FamilyNameLabel, viewModel.FamilyNameLabel);
        Assert.Equal(referenceProperties.FamilyNameFirstLabel, viewModel.FamilyNameFirstLabel);
        Assert.Equal(referenceProperties.FavoriteCoffeeLabel, viewModel.FavoriteCoffeeLabel);
    }
}
