using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.Components.ViewComponents.Header;

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
        return View("~/Components/ViewComponents/Header/_Header.cshtml", model);
    }
}
