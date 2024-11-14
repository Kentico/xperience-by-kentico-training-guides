using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
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

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = BuildViewModel(await GetSignInPage());
        return View("~/Features/Header/Header.cshtml", model);
    }

    private async Task<IEnumerable<WebPageRelatedItem>> GetSignInPage()
    {
        const string EXPECTED_SIGN_IN_PATH = "/Sign_in";
        var page = await contentItemRetrieverService.RetrieveWebPageByPath(EXPECTED_SIGN_IN_PATH);

        return page != null
            ? [new WebPageRelatedItem() { WebPageGuid = page.SystemFields.WebPageItemGUID }]
            : Enumerable.Empty<WebPageRelatedItem>();
    }
    public HeaderViewModel BuildViewModel(IEnumerable<WebPageRelatedItem> signInPage)
    {
        var model = new HeaderViewModel()
        {
            Heading = stringLocalizer["Training guides"],
            LinkOrSignOutWidgetProperties = new()
            {
                UnauthenticatedButtonText = stringLocalizer["Sign in"],
                UnauthenticatedTargetContentPage = signInPage,
                AuthenticatedButtonText = stringLocalizer["Sign out"]
            }
        };

        return model;
    }
}
