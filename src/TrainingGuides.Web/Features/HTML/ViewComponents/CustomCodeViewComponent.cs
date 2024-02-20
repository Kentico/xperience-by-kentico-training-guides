using TrainingGuides.Web.Features.Html.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.Features.Html.ViewComponents;

public class CustomCodeViewComponent : ViewComponent
{
    private readonly IHeadTagStoreService headTagStoreService;

    public CustomCodeViewComponent(IHeadTagStoreService headTagStoreService)
    {
        this.headTagStoreService = headTagStoreService;
    }

    public async Task<IViewComponentResult> InvokeAsync(CodeLocation location)
    {
        var code = (await headTagStoreService.GetCodeAsync(location))
            .Select(customCode => new HtmlString(customCode))
            .ToList();

        return View("~/Features/Html/ViewComponents/CustomCode.cshtml", model: code);
    }
}
