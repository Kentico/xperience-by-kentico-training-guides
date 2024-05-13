using CMS.Activities;
using CMS.ContactManagement;
using TrainingGuides.Web.Features.Activities.Widgets.PageLike;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.Shared.Services;
using CMS.DataEngine;

[assembly: RegisterWidget(
    identifier: PageLikeWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(PageLikeWidgetViewComponent),
    name: "Page like button",
    Description = "Displays a page like button.",
    IconClass = "icon-check-circle")]

namespace TrainingGuides.Web.Features.Activities.Widgets.PageLike;

public class PageLikeWidgetViewComponent : ViewComponent
{
    private readonly IInfoProvider<ActivityInfo> activityInfoProvider;
    private readonly IContentItemRetrieverService contentItemRetrieverService;
    private readonly IHttpRequestService httpRequestService;

    public const string IDENTIFIER = "TrainingGuides.PageLikeWidget";
    public const string ACTIVITY_IDENTIFIER = "pagelike";

    public PageLikeWidgetViewComponent(IInfoProvider<ActivityInfo> activityInfoProvider,
        IContentItemRetrieverService contentItemRetrieverService,
        IHttpRequestService httpRequestService)
    {
        this.activityInfoProvider = activityInfoProvider;
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.httpRequestService = httpRequestService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ComponentViewModel properties)
    {
        var currentContact = ContactManagementContext.GetCurrentContact(false);

        var webPage = await contentItemRetrieverService.RetrieveWebPageById(
            properties.Page.WebPageItemID);

        var likesOfThisPage = currentContact != null
            ? await activityInfoProvider.Get()
                .WhereEquals("ActivityContactID", currentContact.ContactID)
                .And().WhereEquals("ActivityType", ACTIVITY_IDENTIFIER)
                .And().WhereEquals("ActivityValue", webPage?.SystemFields.WebPageItemGUID.ToString())
                .GetEnumerableTypedResultAsync()
            : [];

        bool showLikeButton = likesOfThisPage.Count() == 0;

        var model = new PageLikeWidgetViewModel()
        {
            ShowLikeButton = showLikeButton,
            WebPageItemID = properties.Page.WebPageItemID,
            BaseUrl = httpRequestService.GetBaseUrl()
        };

        return View("~/Features/Activities/Widgets/PageLike/PageLikeWidget.cshtml", model);
    }
}
