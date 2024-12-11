using Microsoft.Extensions.Localization;
using CMS.Websites;
using Moq;
using TrainingGuides.Web.Features.Header;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Shared.Header;

public class HeaderViewComponentTests
{
    private readonly HeaderViewComponent viewComponent;
    private readonly Mock<IStringLocalizer<SharedResources>> stringLocalizerMock;
    private readonly Mock<IContentItemRetrieverService> contentItemRetrieverServiceMock;

    private const string TRAINING_GUIDES = "Training guides";
    private const string SIGN_IN = "Sign in";
    private const string SIGN_OUT = "Sign out";

    public HeaderViewComponentTests()
    {
        stringLocalizerMock = new Mock<IStringLocalizer<SharedResources>>();
        stringLocalizerMock.Setup(x => x[TRAINING_GUIDES]).Returns(new LocalizedString(TRAINING_GUIDES, TRAINING_GUIDES));
        stringLocalizerMock.Setup(x => x[SIGN_IN]).Returns(new LocalizedString(SIGN_IN, SIGN_IN));
        stringLocalizerMock.Setup(x => x[SIGN_OUT]).Returns(new LocalizedString(SIGN_OUT, SIGN_OUT));

        contentItemRetrieverServiceMock = new Mock<IContentItemRetrieverService>();

        viewComponent = new HeaderViewComponent(stringLocalizerMock.Object, contentItemRetrieverServiceMock.Object);
    }

    [Fact]
    public void BuildViewModel_SetsUpWidgetProperties_ToShowSignInSignOutButton()
    {
        var testGuid = Guid.NewGuid();

        var viewModel = viewComponent.BuildViewModel([new WebPageRelatedItem() { WebPageGuid = testGuid }]);
        Assert.Equal(SIGN_IN, viewModel.LinkOrSignOutWidgetProperties.UnauthenticatedButtonText);
        Assert.Equal(SIGN_OUT, viewModel.LinkOrSignOutWidgetProperties.AuthenticatedButtonText);
        Assert.Single(viewModel.LinkOrSignOutWidgetProperties.UnauthenticatedTargetContentPage);
        Assert.Equal(testGuid, viewModel.LinkOrSignOutWidgetProperties.UnauthenticatedTargetContentPage.First().WebPageGuid);
    }

    [Fact]
    public void BuildViewModel_IfShowNavigationNotSpecified_SetsUpShowNavigationInViewModel_ToTrue()
    {
        var testGuid = Guid.NewGuid();

        var viewModel = viewComponent.BuildViewModel([new WebPageRelatedItem() { WebPageGuid = testGuid }]);
        Assert.True(viewModel.ShowNavigation);
    }

    [Fact]
    public void BuildViewModel_IfShowNavigationIsSpecified_SetsUpShowNavigationInViewModel_ToThisValue()
    {
        var testGuid = Guid.NewGuid();

        var viewModelWithNavigation = viewComponent.BuildViewModel([new WebPageRelatedItem() { WebPageGuid = testGuid }], true);
        var viewModelWithoutNavigation = viewComponent.BuildViewModel([new WebPageRelatedItem() { WebPageGuid = testGuid }], false);
        Assert.True(viewModelWithNavigation.ShowNavigation);
        Assert.False(viewModelWithoutNavigation.ShowNavigation);
    }
}