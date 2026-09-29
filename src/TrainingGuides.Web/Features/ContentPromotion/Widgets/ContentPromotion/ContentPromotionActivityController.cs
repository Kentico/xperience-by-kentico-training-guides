using CMS.Activities;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.DataProtection.Services;

namespace TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;

/// <summary>
/// Logs the "Content promotion click" custom activity, following the PageLike pattern rather than
/// the client-side <c>kxt</c> script.
/// </summary>
/// <remarks>
/// <para>
/// This endpoint is deliberately left open to anonymous visitors. The Kentico guidance on securing
/// custom endpoints is about endpoints that read or write confidential data, perform privileged
/// actions, or need to identify the user; restricting this one to signed-in administration users
/// would mean it never fires for the public visitors it exists to measure. It accepts one opaque
/// string, writes no content, and returns nothing about the site.
/// </para>
/// <para>
/// What it does enforce is the consent gate and a cap on the value's length, so a caller cannot use
/// it to write unbounded data into the activity log.
/// </para>
/// </remarks>
public class ContentPromotionActivityController : Controller
{
    /// <summary>
    /// Matches the column the activity value is stored in, so an over-long value is refused here
    /// rather than truncated by the database.
    /// </summary>
    private const int MAX_TRACKING_VALUE_LENGTH = 250;

    private readonly ICustomActivityLogger customActivityLogger;
    private readonly ICookieConsentService cookieConsentService;

    public ContentPromotionActivityController(
        ICustomActivityLogger customActivityLogger,
        ICookieConsentService cookieConsentService)
    {
        this.customActivityLogger = customActivityLogger;
        this.cookieConsentService = cookieConsentService;
    }

    [HttpPost("/contentpromotionclick")]
    public IActionResult LogClick(ContentPromotionClickRequestModel requestModel)
    {
        // Every branch below answers with success. A visitor who withheld consent, and a widget
        // with no tracking value configured, are both ordinary states - answering with an error
        // would put a console error on a perfectly normal visit.
        if (!cookieConsentService.CurrentContactCanBeTracked())
        {
            return Ok();
        }

        string trackingValue = requestModel?.TrackingValue ?? string.Empty;

        if (string.IsNullOrWhiteSpace(trackingValue) || trackingValue.Length > MAX_TRACKING_VALUE_LENGTH)
        {
            return Ok();
        }

        var activityData = new CustomActivityData()
        {
            ActivityTitle = $"Content promotion click - {trackingValue}",
            ActivityValue = trackingValue
        };

        customActivityLogger.Log(ContentPromotionWidgetViewComponent.ACTIVITY_IDENTIFIER, activityData);

        return Ok();
    }
}
