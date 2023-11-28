using CMS.Activities;
using CMS.ContactManagement;
using TrainingGuides.Web.Features.Activities.Widgets.PageLike;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(PageLikeWidgetViewComponent.IDENTIFIER, typeof(PageLikeWidgetViewComponent), "Page like button", Description = "Displays a page like button.",
        IconClass = "icon-check-circle")]

namespace TrainingGuides.Web.Features.Activities.Widgets.PageLike;

public class PageLikeWidgetViewComponent : ViewComponent
{
    private readonly IActivityInfoProvider activityInfoProvider;
    private readonly IContentItemRetrieverService contentItemRetrieverService;

    public const string IDENTIFIER = "TrainingGuides.PageLike";
    public const string ACTIVITY_IDENTIFIER = "pagelike";

    public PageLikeWidgetViewComponent(IActivityInfoProvider activityInfoProvider,
        IContentItemRetrieverService contentItemRetrieverService)
    {
        this.activityInfoProvider = activityInfoProvider;
        this.contentItemRetrieverService = contentItemRetrieverService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ComponentViewModel properties)
    {
        var currentContact = ContactManagementContext.GetCurrentContact(false);

        //var webPage = WebPageItemInfo.Provider.Get().WhereEquals(nameof(WebPageItemInfo.WebPageItemID), properties.Page.WebPageItemID).FirstOrDefault();
        var webPage = await contentItemRetrieverService.RetrieveWebPageById(properties.Page.WebPageItemID, properties.Page.ContentTypeName);

        var likesOfThisPage = currentContact != null
            ? await activityInfoProvider.Get()
                .WhereEquals("ActivityContactID", currentContact.ContactID)
                .And().WhereEquals("ActivityType", ACTIVITY_IDENTIFIER)
                .And().WhereEquals("ActivityValue", webPage.SystemFields.WebPageItemGUID.ToString())
                .GetEnumerableTypedResultAsync()
            : new List<ActivityInfo>();

        bool showLikeButton = likesOfThisPage.Count() == 0;

        var model = new PageLikeWidgetViewModel()
        {
            ShowLikeButton = showLikeButton,
            WebPageItemID = properties.Page.WebPageItemID,
            ContentTypeName = properties.Page.ContentTypeName
        };

        return View("~/Features/Activities/Widgets/PageLike/PageLikeWidget.cshtml", model);
    }
}
