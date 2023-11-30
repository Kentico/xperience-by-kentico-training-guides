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

    public PageLikeController(ICustomActivityLogger customActivityLogger,
        IContentItemRetrieverService contentItemRetrieverService,
        ICookieConsentService cookieConsentService)
    {
        this.customActivityLogger = customActivityLogger;
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.cookieConsentService = cookieConsentService;
    }

    /*
     * You can combine these two attributes into one
     * by specifying the route path pattern in the [HttpPost] attribute
     */
    [Route("pagelike")]
    [HttpPost]
    public async Task<IActionResult> PageLike(PageLikeRequestModel requestModel)
    {
        if (!cookieConsentService.CurrentContactIsVisitorOrHigher())
            return Content(NO_TRACKING_MESSAGE);

        if (!int.TryParse(requestModel.WebPageItemID, out int webPageItemID))
            return Content(BAD_PAGE_DATA_MESSAGE);

        if (string.IsNullOrEmpty(requestModel.ContentTypeName))
            return Content(BAD_PAGE_DATA_MESSAGE);

        //var webPage = (await WebPageItemInfo.Provider.Get().WhereEquals(nameof(WebPageItemInfo.WebPageItemID), webPageId).GetEnumerableTypedResultAsync()).FirstOrDefault();
        var webPage = await contentItemRetrieverService.RetrieveWebPageById(
            webPageItemID,
            requestModel.ContentTypeName);

        if (webPage is null)
            return Content(BAD_PAGE_DATA_MESSAGE);

        string likedPageName = webPage.SystemFields.WebPageItemName;
        string likedPageGuid = webPage.SystemFields.WebPageItemGUID.ToString();

        var pageLikeActicityData = new CustomActivityData()
        {
            ActivityTitle = $"Page Like - {likedPageName}",
            ActivityValue = likedPageGuid,
        };

        customActivityLogger.Log(PageLikeWidgetViewComponent.ACTIVITY_IDENTIFIER, pageLikeActicityData);
        return Content(THANK_YOU_MESSAGE);
    }

}
