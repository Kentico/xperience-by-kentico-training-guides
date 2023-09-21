using CMS.Activities;
using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Kentico.PageBuilder.Web.Mvc;
using KBank.Web.Helpers.Cookies;

namespace KBank.Web.Components.Widgets.PageLike;


public class PageLikeController : Controller
{
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
            return Content("<span>You have not consented to tracking, so we cannot save this page like.</span>");

        var nodes = await _pageRetriever.RetrieveAsync<TreeNode>(documentQuery => documentQuery
                    .WhereEquals("DocumentGuid", requestModel.PageGuid));

        var node = nodes.FirstOrDefault();

        string likedPageName = node.DocumentName;
        string likedPagePath = node.NodeAliasPath;

        var pageLikeActicityData = new CustomActivityData()
        {
            ActivityTitle = $"Page Like - {likedPageName}",
            ActivityValue = likedPagePath,
        };

        _customActivityLogger.Log(PageLikeWidgetViewComponent.ACTIVITY_IDENTIFIER, pageLikeActicityData);
        return Content("<span>Thank you!</span>");
    }
}

