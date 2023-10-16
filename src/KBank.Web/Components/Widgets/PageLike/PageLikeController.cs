using CMS.Activities;
using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Linq;
using KBank.Web.Helpers.Cookies;

namespace KBank.Web.Components.Widgets.PageLike;


public class PageLikeController : Controller
{
    private const string NO_TRACKING_MESSAGE = "<span>You have not consented to tracking, so we cannot save this page like.</span>";
    private const string BAD_PAGE_DATA_MESSAGE = "<span>Error in page like data. Please try again later.</span>";
    private const string THANK_YOU_MESSAGE = "<span>Thank you!</span>";

    private readonly IPageRetriever _pageRetriever;
    private readonly ICustomActivityLogger _customActivityLogger;


    public PageLikeController(IPageRetriever pageRetriever, ICustomActivityLogger customActivityLogger)
    {
        _pageRetriever = pageRetriever;
        _customActivityLogger = customActivityLogger;
    }

    [Route("pagelike")]
    [HttpPost]
    public async Task<IActionResult> PageLike(PageLikeRequestModel requestModel)
    {
        if (!CookieConsentHelper.CurrentContactIsVisitorOrHigher())
            return Content(NO_TRACKING_MESSAGE);

        if (!Guid.TryParse(requestModel.PageGuid, out Guid pageGuid))
            return Content(BAD_PAGE_DATA_MESSAGE);

        var nodes = await _pageRetriever.RetrieveAsync<TreeNode>(documentQuery => documentQuery
                    .WhereEquals("DocumentGuid", pageGuid));

        if (nodes.Count() == 0)
            return Content(BAD_PAGE_DATA_MESSAGE);

        var node = nodes.FirstOrDefault();
        

        string likedPageName = node.DocumentName;
        string likedPagePath = node.NodeAliasPath;

        var pageLikeActicityData = new CustomActivityData()
        {
            ActivityTitle = $"Page Like - {likedPageName}",
            ActivityValue = likedPagePath,
        };

        _customActivityLogger.Log(PageLikeWidgetViewComponent.ACTIVITY_IDENTIFIER, pageLikeActicityData);
        return Content(THANK_YOU_MESSAGE);
    }
}