using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Kentico.PageBuilder.Web.Mvc;
using TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;
using TrainingGuides.Web.Features.LandingPages.Widgets.SimpleCallToAction;

[assembly:
    RegisterWidget(
        identifier: SimpleCallToActionWidgetViewComponent.IDENTIFIER,
        viewComponentType: typeof(SimpleCallToActionWidgetViewComponent),
        name: "Simple call to action",
        propertiesType: typeof(SimpleCallToActionWidgetProperties),
        Description = $"Displays a call to action button. Simpler configuration options than {CallToActionWidgetViewComponent.NAME} widget.",
        IconClass = "icon-bubble")]

namespace TrainingGuides.Web.Features.LandingPages.Widgets.SimpleCallToAction;

public class SimpleCallToActionWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.SimpleCallToActionWidget";

    public ViewViewComponentResult Invoke(SimpleCallToActionWidgetProperties properties)
    {
        string? targetUrl = properties.TargetContent switch
        {
            nameof(TargetContentOption.Page) => properties.TargetContentPage,
            nameof(TargetContentOption.AbsoluteUrl) => properties.TargetContentAbsoluteUrl,
            _ => string.Empty
        };

        var model = new SimpleCallToActionWidgetViewModel()
        {
            Text = properties?.Text,
            Url = targetUrl,
            OpenInNewTab = properties?.OpenInNewTab ?? false,
        };

        return View("~/Features/LandingPages/Widgets/SimpleCallToAction/SimpleCallToACtionWidget.cshtml", model);
    }
}