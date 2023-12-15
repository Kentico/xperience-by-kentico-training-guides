using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Shared.Resources;

namespace TrainingGuides.Web.Features.Header;

public class HeaderViewComponent : ViewComponent
{

    private readonly IStringLocalizer<SharedResources> localizer;
    public HeaderViewComponent(IStringLocalizer<SharedResources> localizer)
    {
        this.localizer = localizer;
    }

    public IViewComponentResult Invoke()
    {
        var model = new HeaderViewModel()
        {
            Heading = localizer["Heading"]
        };
        return View("~/Features/Header/Header.cshtml", model);
    }
}
