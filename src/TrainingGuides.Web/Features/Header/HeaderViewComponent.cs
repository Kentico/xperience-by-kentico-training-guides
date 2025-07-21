using CMS.ContentEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Shared.Helpers;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Header;


public class HeaderViewComponent : ViewComponent
{

    private readonly IStringLocalizer<SharedResources> stringLocalizer;
    private readonly IContentItemRetrieverService contentItemRetrieverService;

    public HeaderViewComponent(
        IStringLocalizer<SharedResources> stringLocalizer,
        IContentItemRetrieverService contentItemRetrieverService)
    {
        this.stringLocalizer = stringLocalizer;
        this.contentItemRetrieverService = contentItemRetrieverService;
    }

    public async Task<IViewComponentResult> InvokeAsync(bool showNavigation = true)
    {
        var model = BuildViewModel(await GetSignInPageForWidget(), showNavigation);
        return View("~/Features/Header/Header.cshtml", model);
    }

    private async Task<IEnumerable<ContentItemReference>> GetSignInPageForWidget()
    {
        var page = await contentItemRetrieverService.RetrieveWebPageByPath(ApplicationConstants.EXPECTED_SIGN_IN_PATH);

        return page != null
            ? [new ContentItemReference() { Identifier = page.SystemFields.ContentItemGUID }]
            : [];
    }
    public HeaderViewModel BuildViewModel(IEnumerable<ContentItemReference> signInPage, bool showNavigation = true)
    {
        var model = new HeaderViewModel()
        {
            Heading = stringLocalizer["Training guides"],
            LinkOrSignOutWidgetProperties = new()
            {
                UnauthenticatedButtonText = stringLocalizer["Sign in"],
                UnauthenticatedTargetContentPage = signInPage,
                AuthenticatedButtonText = stringLocalizer["Sign out"]
            },
            ShowNavigation = showNavigation
        };

        return model;
    }
}
