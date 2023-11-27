using CMS.Activities;
using TrainingGuides.Web.Helpers.Cookies;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Services.Content;

namespace TrainingGuides.Web.Components.Widgets.PageLike;


public class PageLikeController : Controller
{
    private const string NO_TRACKING_MESSAGE = "<span>You have not consented to tracking, so we cannot save this page like.</span>";
    private const string BAD_PAGE_DATA_MESSAGE = "<span>Error in page like data. Please try again later.</span>";
    private const string THANK_YOU_MESSAGE = "<span>Thank you!</span>";

    private readonly ICustomActivityLogger customActivityLogger;
    private readonly IContentItemRetrieverService contentItemRetrieverService;

    public PageLikeController(ICustomActivityLogger customActivityLogger,
        IContentItemRetrieverService contentItemRetrieverService)
    {
        this.customActivityLogger = customActivityLogger;
        this.contentItemRetrieverService = contentItemRetrieverService;
    }

    [Route("pagelike")]
    [HttpPost]
    public async Task<IActionResult> PageLike(PageLikeRequestModel requestModel)
    {
        if (!CookieConsentHelper.CurrentContactIsVisitorOrHigher())
            return Content(NO_TRACKING_MESSAGE);

        if (!int.TryParse(requestModel.WebPageItemID, out int webPageItemID))
            return Content(BAD_PAGE_DATA_MESSAGE);

        if (string.IsNullOrEmpty(requestModel.ContentTypeName))
            return Content(BAD_PAGE_DATA_MESSAGE);

        //var webPage = (await WebPageItemInfo.Provider.Get().WhereEquals(nameof(WebPageItemInfo.WebPageItemID), webPageId).GetEnumerableTypedResultAsync()).FirstOrDefault();
        var webPage = await contentItemRetrieverService.RetrieveWebPageById(webPageItemID, requestModel.ContentTypeName);

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
