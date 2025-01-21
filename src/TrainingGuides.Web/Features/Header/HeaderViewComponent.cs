using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.Features.Header;

public class HeaderViewComponent : ViewComponent
{

    public HeaderViewComponent()
    { }

    public IViewComponentResult Invoke()
    {
        var model = new HeaderViewModel()
        {
            Heading = "Training guides"
        };
        return View("~/Features/Header/Header.cshtml", model);
    }
}