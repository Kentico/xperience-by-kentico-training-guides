using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(
        identifier: LinkOrSignOutWidgetViewComponent.IDENTIFIER,
        viewComponentType: typeof(LinkOrSignOutWidgetViewComponent),
        name: "Link or Sign Out",
        propertiesType: typeof(LinkOrSignOutWidgetProperties),
        Description = $"Displays a line of text and a link button that will log out a member if they are authenticated and link to a specified page (such as Sign-in or Registration) if not.",
        IconClass = "icon-arrow-leave-square")]

namespace TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;

public class LinkOrSignOutWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.LinkOrSignOutWidget";

    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    private readonly IMembershipService membershipService;
    private readonly IHttpRequestService httpRequestService;

    public LinkOrSignOutWidgetViewComponent(
        IWebPageUrlRetriever webPageUrlRetriever,
        IPreferredLanguageRetriever preferredLanguageRetriever,
        IMembershipService membershipService,
        IHttpRequestService httpRequestService)
    {
        this.webPageUrlRetriever = webPageUrlRetriever;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
        this.membershipService = membershipService;
        this.httpRequestService = httpRequestService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(LinkOrSignOutWidgetProperties properties)
    {
        var model = await BuildWidgetViewModel(properties);

        return View("~/Features/Membership/Widgets/LinkOrSignOut/LinkOrSignOutWidget.cshtml", model);
    }

    public async Task<LinkOrSignOutWidgetViewModel> BuildWidgetViewModel(LinkOrSignOutWidgetProperties properties)
    {
        bool isAuthenticated = await membershipService.IsMemberAuthenticated();

        string preferredLanguageCode = preferredLanguageRetriever.Get();

        LinkOrSignOutWidgetViewModel model;

        if (isAuthenticated)
        {
            string baseUrl = httpRequestService.GetBaseUrl();
            string currentPageUrl = await httpRequestService.GetCurrentPageUrlForLanguage(preferredLanguageCode);

            model = new LinkOrSignOutWidgetViewModel()
            {
                Text = properties.AuthenticatedText,
                ButtonText = properties.AuthenticatedButtonText,
                Url = string.IsNullOrWhiteSpace(currentPageUrl) ? baseUrl : currentPageUrl,
                IsAuthenticated = isAuthenticated,
            };
        }
        else
        {
            string linkTargetUrl = await GetWebPageUrl(properties.UnauthenticatedTargetContentPage?.FirstOrDefault(), preferredLanguageCode)
                ?? string.Empty;

            model = new LinkOrSignOutWidgetViewModel()
            {
                Text = properties.UnauthenticatedText,
                ButtonText = properties.UnauthenticatedButtonText,
                Url = linkTargetUrl,
                IsAuthenticated = isAuthenticated,
            };
        }

        return model;
    }

    private async Task<string?> GetWebPageUrl(WebPageRelatedItem? webPage, string preferredLanguageCode) =>
        webPage != null
        ? (await webPageUrlRetriever.Retrieve(webPage.WebPageGuid, preferredLanguageCode))
            .RelativePath
        : string.Empty;
}