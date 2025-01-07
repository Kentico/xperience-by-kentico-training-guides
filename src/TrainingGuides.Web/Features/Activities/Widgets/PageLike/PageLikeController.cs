using CMS.Activities;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.DataProtection.Services;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Activities.Widgets.PageLike;

public class PageLikeController : Controller
{
    private const string NO_TRACKING_MESSAGE = "<span>You have not consented to tracking, so we cannot save this page like.</span>";
    private const string BAD_PAGE_DATA_MESSAGE = "<span>Error in page like data. Please try again later.</span>";
    private const string THANK_YOU_MESSAGE = "<span>Thank you!</span>";

    private readonly ICustomActivityLogger customActivityLogger;
    private readonly IContentItemRetrieverService contentItemRetrieverService;
    private readonly ICookieConsentService cookieConsentService;

    public PageLikeController(
        ICustomActivityLogger customActivityLogger,
        IContentItemRetrieverService contentItemRetrieverService,
        ICookieConsentService cookieConsentService)
    {
        this.customActivityLogger = customActivityLogger;
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.cookieConsentService = cookieConsentService;
    }

    [HttpPost("/pagelike")]
    public async Task<IActionResult> PageLike(PageLikeRequestModel requestModel)
    {
        if (!cookieConsentService.CurrentContactCanBeTracked())
            return Content(NO_TRACKING_MESSAGE);

        if (!int.TryParse(requestModel.WebPageItemID, out int webPageItemID))
            return Content(BAD_PAGE_DATA_MESSAGE);

        var webPage = await contentItemRetrieverService.RetrieveWebPageById(
            webPageItemID);

        if (webPage is null)
            return Content(BAD_PAGE_DATA_MESSAGE);

        string likedPageName = webPage.SystemFields.WebPageItemName;
        string likedPageTreePath = webPage.SystemFields.WebPageItemTreePath;
        string likedPageGuid = webPage.SystemFields.WebPageItemGUID.ToString();

        var pageLikeActicityData = new CustomActivityData()
        {
            ActivityTitle = $"Page like - {likedPageTreePath} ({likedPageName})",
            ActivityValue = likedPageGuid,
        };

        customActivityLogger.Log(PageLikeWidgetViewComponent.ACTIVITY_IDENTIFIER, pageLikeActicityData);
        return Content(THANK_YOU_MESSAGE);
    }
}
