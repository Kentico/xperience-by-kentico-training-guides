using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Shared.Resources;

namespace TrainingGuides.Web.Features.Header;

public class HeaderViewComponent : ViewComponent
{

    private readonly IStringLocalizer<SharedResources> stringLocalizer;
    public HeaderViewComponent(IStringLocalizer<SharedResources> stringLocalizer)
    {
        this.stringLocalizer = stringLocalizer;
    }

    public IViewComponentResult Invoke()
    {
        var model = new HeaderViewModel()
        {
            Heading = stringLocalizer["Header.Heading"]
        };
        return View("~/Features/Header/Header.cshtml", model);
    }
}
