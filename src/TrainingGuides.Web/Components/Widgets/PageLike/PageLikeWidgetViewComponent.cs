using CMS.Activities;
using CMS.ContactManagement;
using CMS.Websites.Internal;
using TrainingGuides.Web.Components.Widgets.PageLike;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly:
    RegisterWidget(PageLikeWidgetViewComponent.IDENTIFIER, typeof(PageLikeWidgetViewComponent), "Page like button", Description = "Displays a page like button.",
        IconClass = "icon-check-circle")]
namespace TrainingGuides.Web.Components.Widgets.PageLike;

public class PageLikeWidgetViewComponent : ViewComponent
{
    private readonly IActivityInfoProvider _activityInfoProvider;

    public const string IDENTIFIER = "TrainingGuides.PageLike";
    public const string ACTIVITY_IDENTIFIER = "pagelike";

    public PageLikeWidgetViewComponent(IActivityInfoProvider activityInfoProvider)
    {
        _activityInfoProvider = activityInfoProvider;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ComponentViewModel properties)
    {
        var currentContact = ContactManagementContext.GetCurrentContact(false);

        var webPage = WebPageItemInfo.Provider.Get().WhereEquals(nameof(WebPageItemInfo.WebPageItemID), properties.Page.WebPageItemID).FirstOrDefault();

        IEnumerable<ActivityInfo> likesOfThisPage;
        if (currentContact != null)
        {
            likesOfThisPage = await _activityInfoProvider.Get()
                .WhereEquals("ActivityContactID", currentContact.ContactID)
                .And().WhereEquals("ActivityType", ACTIVITY_IDENTIFIER)
                .And().WhereEquals("ActivityValue", webPage.WebPageItemTreePath)
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
            WebPageId = properties.Page.WebPageItemID,
        };

        return View("~/Components/Widgets/PageLike/_PageLikeWidget.cshtml", model);
    }
}