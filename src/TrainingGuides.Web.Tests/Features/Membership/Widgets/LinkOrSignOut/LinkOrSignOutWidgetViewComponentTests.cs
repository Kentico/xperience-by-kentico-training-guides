using CMS.ContentEngine;
using CMS.Websites;
using Kentico.Content.Web.Mvc.Routing;
using Moq;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Membership.Widgets.LinkOrSignOut;

public class LinkOrSignOutWidgetViewComponentTests
{
    private readonly Mock<IContentItemRetrieverService> contentItemRetrieverServiceMock;
    private readonly Mock<IPreferredLanguageRetriever> preferredLanguageRetrieverMock;
    private readonly Mock<IMembershipService> membershipServiceMock;
    private readonly Mock<IHttpRequestService> httpRequestServiceMock;

    private const string BASE_URL = "http://localhost:5000";
    private const string UNAUTHENTICATED_TEXT = "Already have an account?";
    private const string UNAUTHENTICATED_BUTTON_TEXT = "Sign In";
    private const string AUTHENTICATED_TEXT = "You're already signed in.";
    private const string AUTHENTICATED_BUTTON_TEXT = "Sign Out";
    private const string LANGUAGE = "en";
    private const string CURRENT_PAGE_URL = "/current-page";
    private static readonly Guid unauthenticatedGuid = new("00000000-0000-0000-0000-000000000000");

    private readonly LinkOrSignOutWidgetProperties widgetProperties;

    public LinkOrSignOutWidgetViewComponentTests()
    {
        contentItemRetrieverServiceMock = new Mock<IContentItemRetrieverService>();
        contentItemRetrieverServiceMock.Setup(x => x.RetrieveWebPageByContentItemGuid(It.IsAny<Guid>()))
            .ReturnsAsync((IWebPageFieldsSource?)null);

        preferredLanguageRetrieverMock = new Mock<IPreferredLanguageRetriever>();
        preferredLanguageRetrieverMock.Setup(x => x.Get()).Returns(LANGUAGE);

        membershipServiceMock = new Mock<IMembershipService>();
        membershipServiceMock.Setup(x => x.IsMemberAuthenticated()).ReturnsAsync(true);

        httpRequestServiceMock = new Mock<IHttpRequestService>();
        httpRequestServiceMock.Setup(x => x.GetBaseUrl()).Returns(BASE_URL);
        httpRequestServiceMock.Setup(x => x.GetCurrentPageUrlForLanguage(It.IsAny<string>())).ReturnsAsync(CURRENT_PAGE_URL);

        var relatedItem = new ContentItemReference()
        {
            Identifier = unauthenticatedGuid,
        };

        widgetProperties = new LinkOrSignOutWidgetProperties()
        {
            UnauthenticatedText = UNAUTHENTICATED_TEXT,
            UnauthenticatedButtonText = UNAUTHENTICATED_BUTTON_TEXT,
            UnauthenticatedTargetContentPage = new[] { relatedItem },
            AuthenticatedText = AUTHENTICATED_TEXT,
            AuthenticatedButtonText = AUTHENTICATED_BUTTON_TEXT
        };
    }

    [Fact]
    public async Task BuildWidgetViewModel_ReturnsUnauthenticatedViewModel_WhenUserIsNotAuthenticated()
    {
        membershipServiceMock.Setup(x => x.IsMemberAuthenticated()).ReturnsAsync(false);

        var viewComponent = new LinkOrSignOutWidgetViewComponent(
            preferredLanguageRetrieverMock.Object,
            membershipServiceMock.Object,
            httpRequestServiceMock.Object,
            contentItemRetrieverServiceMock.Object);

        var viewModel = await viewComponent.BuildWidgetViewModel(widgetProperties);

        Assert.False(viewModel.IsAuthenticated);
        Assert.Equal(UNAUTHENTICATED_TEXT, viewModel.Text);
        Assert.Equal(UNAUTHENTICATED_BUTTON_TEXT, viewModel.ButtonText);
        Assert.Equal(string.Empty, viewModel.Url); // When page is not found, URL should be empty
    }

    [Fact]
    public async Task BuildWidgetViewModel_ReturnsAuthenticatedViewModel_WhenUserIsAuthenticated()
    {
        membershipServiceMock.Setup(x => x.IsMemberAuthenticated()).ReturnsAsync(true);

        var viewComponent = new LinkOrSignOutWidgetViewComponent(
            preferredLanguageRetrieverMock.Object,
            membershipServiceMock.Object,
            httpRequestServiceMock.Object,
            contentItemRetrieverServiceMock.Object);

        var viewModel = await viewComponent.BuildWidgetViewModel(widgetProperties);

        Assert.True(viewModel.IsAuthenticated);
        Assert.Equal(AUTHENTICATED_TEXT, viewModel.Text);
        Assert.Equal(AUTHENTICATED_BUTTON_TEXT, viewModel.ButtonText);
        Assert.Equal(CURRENT_PAGE_URL, viewModel.Url);
    }

    [Fact]
    public async Task BuildWidgetViewModel_FallsBackToBaseUrl_WhenAuthenticatedAndCurrentPageUrlNotFound()
    {
        membershipServiceMock.Setup(x => x.IsMemberAuthenticated()).ReturnsAsync(true);

        httpRequestServiceMock.Setup(x => x.GetCurrentPageUrlForLanguage(It.IsAny<string>())).ReturnsAsync(string.Empty);

        var viewComponent = new LinkOrSignOutWidgetViewComponent(
            preferredLanguageRetrieverMock.Object,
            membershipServiceMock.Object,
            httpRequestServiceMock.Object,
            contentItemRetrieverServiceMock.Object);

        var viewModel = await viewComponent.BuildWidgetViewModel(widgetProperties);

        Assert.Equal(BASE_URL, viewModel.Url);
    }
}