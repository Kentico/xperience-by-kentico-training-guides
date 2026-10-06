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
/// Open to anonymous visitors on purpose: it exists to measure public visitors, accepts one opaque
/// string, writes no content and returns nothing about the site. It enforces the consent gate and
/// caps the value's length.
/// </para>
/// <para>
/// Like <c>/pagelike</c>, it has no antiforgery token - <c>navigator.sendBeacon</c> cannot send one.
/// A beacon form post is a CORS simple request, so another site can make a consenting visitor's
/// browser log a click they did not make. The impact is a spurious activity on that visitor's
/// contact; weigh that before copying this pattern for anything with consequences.
/// </para>
/// </remarks>
public class ContentPromotionActivityController(
    ICustomActivityLogger customActivityLogger,
    ICookieConsentService cookieConsentService) : Controller
{
    /// <summary>
    /// The length of the activity title and value columns (nvarchar(250)), so nothing logged here is
    /// truncated by the database.
    /// </summary>
    private const int ACTIVITY_COLUMN_LENGTH = 250;

    private const string ACTIVITY_TITLE_PREFIX = "Content promotion click - ";

    [HttpPost("/contentpromotionclick")]
    public IActionResult LogClick([FromForm] ContentPromotionClickRequestModel requestModel)
    {
        // Every branch below answers with success. A visitor who withheld consent, and a widget
        // with no tracking value configured, are both ordinary states - answering with an error
        // would put a console error on a perfectly normal visit.
        if (!cookieConsentService.CurrentContactCanBeTracked())
        {
            return Ok();
        }

        string trackingValue = requestModel.TrackingValue;

        if (string.IsNullOrWhiteSpace(trackingValue) || trackingValue.Length > ACTIVITY_COLUMN_LENGTH)
        {
            return Ok();
        }

        // The value is refused rather than shortened, because it is what reports group by. The title
        // is only a label, so it is the one that gives way.
        string title = ACTIVITY_TITLE_PREFIX + trackingValue;

        var activityData = new CustomActivityData()
        {
            ActivityTitle = title.Length > ACTIVITY_COLUMN_LENGTH ? title[..ACTIVITY_COLUMN_LENGTH] : title,
            ActivityValue = trackingValue
        };

        customActivityLogger.Log(ContentPromotionWidgetViewComponent.ACTIVITY_IDENTIFIER, activityData);

        return Ok();
    }
}
