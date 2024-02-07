using TrainingGuides.Web.Features.HTML.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.Features.HTML.ViewComponents;

public class CustomCodeViewComponent : ViewComponent
{
    private readonly IHeadTagStoreService headTagStoreService;

    public CustomCodeViewComponent(IHeadTagStoreService headTagStoreService)
    {
        this.headTagStoreService = headTagStoreService;
    }

    public async Task<IViewComponentResult> InvokeAsync(CodeLocation location)
    {
        var code = (await headTagStoreService.GetCodeAsync(location)).Select(x => new HtmlString(x)).ToList();

        return View("~/Features/HTML/ViewComponents/CustomCode.cshtml", model: code);
    }
}
