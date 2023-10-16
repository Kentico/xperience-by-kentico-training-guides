using CMS.Activities;
using CMS.ContactManagement;
using KBank.Web.Components.Widgets.PageLike;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly:
    RegisterWidget(PageLikeWidgetViewComponent.IDENTIFIER, typeof(PageLikeWidgetViewComponent), "Page like button", Description = "Displays a page like button.",
        IconClass = "xp-plus-square")]
namespace KBank.Web.Components.Widgets.PageLike;

public class PageLikeWidgetViewComponent : ViewComponent
{
    private readonly IActivityInfoProvider _activityInfoProvider;

    public const string IDENTIFIER = "KBank.PageLike";
    public const string ACTIVITY_IDENTIFIER = "pagelike";

    public PageLikeWidgetViewComponent(IActivityInfoProvider activityInfoProvider)
    {
        _activityInfoProvider = activityInfoProvider;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ComponentViewModel properties)
    {
        var currentContact = ContactManagementContext.GetCurrentContact(false);

        IEnumerable<ActivityInfo> likesOfThisPage;
        if (currentContact != null)
        {
            likesOfThisPage = await _activityInfoProvider.Get()
                .WhereEquals("ActivityContactID", currentContact.ContactID)
                .And().WhereEquals("ActivityType", ACTIVITY_IDENTIFIER)
                .And().WhereEquals("ActivityValue", properties.Page.NodeAliasPath)
                .GetEnumerableTypedResultAsync();
        }
        else
        {
            likesOfThisPage = new List<ActivityInfo>();
        }

        bool showLikeButton = (likesOfThisPage.Count() == 0);

        var model = new PageLikeWidgetViewModel()
        {
            ShowLikeButton = showLikeButton,
            PageGuid = properties.Page.DocumentGUID,
        };

        return View("~/Components/Widgets/PageLike/_PageLikeWidget.cshtml", model);
    }
}