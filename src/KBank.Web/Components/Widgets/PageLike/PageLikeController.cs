using CMS.Activities;
using CMS.Websites.Internal;
using KBank.Web.Helpers.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace KBank.Web.Components.Widgets.PageLike;


public class PageLikeController : Controller
{
    private const string NO_TRACKING_MESSAGE = "<span>You have not consented to tracking, so we cannot save this page like.</span>";
    private const string BAD_PAGE_DATA_MESSAGE = "<span>Error in page like data. Please try again later.</span>";
    private const string THANK_YOU_MESSAGE = "<span>Thank you!</span>";

    private readonly ICustomActivityLogger _customActivityLogger;

    public PageLikeController(ICustomActivityLogger customActivityLogger)
    {
        _customActivityLogger = customActivityLogger;
    }

    [Route("pagelike")]
    [HttpPost]
    public async Task<IActionResult> PageLike(PageLikeRequestModel requestModel)
    {
        if (!CookieConsentHelper.CurrentContactIsVisitorOrHigher())
            return Content(NO_TRACKING_MESSAGE);

        if (!int.TryParse(requestModel.WebPageId, out int webPageId))
            return Content(BAD_PAGE_DATA_MESSAGE);

        var webPage = (await WebPageItemInfo.Provider.Get().WhereEquals(nameof(WebPageItemInfo.WebPageItemID), webPageId).GetEnumerableTypedResultAsync()).FirstOrDefault();

        if (webPage is null)
            return Content(BAD_PAGE_DATA_MESSAGE);

        string likedPageName = webPage.WebPageItemName;
        string likedPagePath = webPage.WebPageItemTreePath;

        var pageLikeActicityData = new CustomActivityData()
        {
            ActivityTitle = $"Page Like - {likedPageName}",
            ActivityValue = likedPagePath,
        };

        _customActivityLogger.Log(PageLikeWidgetViewComponent.ACTIVITY_IDENTIFIER, pageLikeActicityData);
        return Content(THANK_YOU_MESSAGE);
    }

}