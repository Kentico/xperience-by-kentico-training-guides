using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.Html.Services;
using TrainingGuides.Web.Features.Html.Widgets.HtmlCode;

[assembly: RegisterWidget(
    identifier: HtmlCodeWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(HtmlCodeWidgetViewComponent),
    name: "Html code",
    propertiesType: typeof(HtmlCodeWidgetProperties),
    Description = "Displays the heading of the page.",
    IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Html.Widgets.HtmlCode;

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
            model.CodeHtml = HtmlString.Empty;
        }
        else
        {
            model.CodeHtml = new(properties.Code);
        }

        return View("~/Features/Html/Widgets/HtmlCode/HtmlCodeWidget.cshtml", model);
    }
}
