using CMS.Activities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using TrainingGuides.Web.Features.DataProtection.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ContentPromotion.Widgets.ContentPromotion;

public class ContentPromotionActivityControllerTests
{
    private const string TRACKING_VALUE = "autumn-pet-cover";

    private readonly Mock<ICustomActivityLogger> customActivityLoggerMock = new();
    private readonly Mock<ICookieConsentService> cookieConsentServiceMock = new();
    private readonly ContentPromotionActivityController controller;

    public ContentPromotionActivityControllerTests()
    {
        cookieConsentServiceMock
            .Setup(x => x.CurrentContactCanBeTracked())
            .Returns(true);

        controller = new ContentPromotionActivityController(
            customActivityLoggerMock.Object,
            cookieConsentServiceMock.Object);
    }

    [Fact]
    public void TrackableContact_LogsTheActivityOnceWithTheTrackingValue()
    {
        var result = controller.LogClick(new ContentPromotionClickRequestModel { TrackingValue = TRACKING_VALUE });

        customActivityLoggerMock.Verify(
            x => x.Log(
                ContentPromotionWidgetViewComponent.ACTIVITY_IDENTIFIER,
                It.Is<CustomActivityData>(data => data.ActivityValue == TRACKING_VALUE)),
            Times.Once);

        Assert.IsType<OkResult>(result);
    }

    // ActivityTitle and ActivityValue are both nvarchar(250). The value is capped at that length, so
    // the title - which adds a prefix to it - must not be allowed to outgrow its column.
    [Fact]
    public void LongestAcceptedTrackingValue_LogsATitleThatFitsItsColumn()
    {
        string longestValue = new('x', 250);

        controller.LogClick(new ContentPromotionClickRequestModel { TrackingValue = longestValue });

        customActivityLoggerMock.Verify(
            x => x.Log(
                ContentPromotionWidgetViewComponent.ACTIVITY_IDENTIFIER,
                It.Is<CustomActivityData>(data => data.ActivityValue == longestValue && data.ActivityTitle.Length <= 250)),
            Times.Once);
    }

    /// <summary>
    /// A visitor who withheld consent is a perfectly normal visitor. Answering with an error
    /// would put a console error on their screen for behaving exactly as intended.
    /// </summary>
    [Fact]
    public void ContactThatCannotBeTracked_LogsNothingAndStillSucceeds()
    {
        cookieConsentServiceMock
            .Setup(x => x.CurrentContactCanBeTracked())
            .Returns(false);

        var result = controller.LogClick(new ContentPromotionClickRequestModel { TrackingValue = TRACKING_VALUE });

        customActivityLoggerMock.Verify(
            x => x.Log(It.IsAny<string>(), It.IsAny<CustomActivityData>()),
            Times.Never);

        Assert.IsType<OkResult>(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyTrackingValue_LogsNothing(string trackingValue)
    {
        var result = controller.LogClick(new ContentPromotionClickRequestModel { TrackingValue = trackingValue });

        customActivityLoggerMock.Verify(
            x => x.Log(It.IsAny<string>(), It.IsAny<CustomActivityData>()),
            Times.Never);

        Assert.IsType<OkResult>(result);
    }
}
