using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.HTML.Services;
using TrainingGuides.Web.Features.HTML.Widgets.HtmlCode;

[assembly: RegisterWidget(
    identifier: HtmlCodeWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(HtmlCodeWidgetViewComponent),
    name: "HTML code",
    propertiesType: typeof(HtmlCodeWidgetProperties),
    Description = "Displays the heading of the page.",
    IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.HTML.Widgets.HtmlCode;

public class HtmlCodeWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.HtmlCodeWidget";

    private readonly IHeadTagStoreService headTagStoreService;

    public HtmlCodeWidgetViewComponent(IHeadTagStoreService headTagStoreService)
    {
        this.headTagStoreService = headTagStoreService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(HtmlCodeWidgetProperties properties)
    {
        var model = new HtmlCodeWidgetViewModel();

        if (properties.InsertToHead)
        {
            await headTagStoreService.StoreCodeAsync(CodeLocation.Head, properties.Code);
            model.Code = null;
        }
        else
        {
            model.Code = new(properties.Code);
        }

        return View("~/Features/HTML/Widgets/HtmlCode/HtmlCodeWidget.cshtml", model);
    }
}
