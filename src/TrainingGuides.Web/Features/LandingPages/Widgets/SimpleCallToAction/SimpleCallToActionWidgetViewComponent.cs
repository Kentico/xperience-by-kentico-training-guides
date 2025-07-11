using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;
using TrainingGuides.Web.Features.LandingPages.Widgets.SimpleCallToAction;
using TrainingGuides.Web.Features.Shared.Services;

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

    private readonly IContentItemRetrieverService generalContentItemRetrieverService;

    public SimpleCallToActionWidgetViewComponent(IContentItemRetrieverService generalContentItemRetrieverService)
    {
        this.generalContentItemRetrieverService = generalContentItemRetrieverService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(SimpleCallToActionWidgetProperties properties)
    {

        string targetUrl = properties.TargetContent switch
        {
            nameof(TargetContentOption.Page) => await GetWebPageUrl(properties.TargetContentPage?.FirstOrDefault()) ?? string.Empty,
            nameof(TargetContentOption.AbsoluteUrl) => properties.TargetContentAbsoluteUrl,
            _ => string.Empty
        };

        var model = new SimpleCallToActionWidgetViewModel()
        {
            Text = properties.Text,
            Url = targetUrl,
            OpenInNewTab = properties?.OpenInNewTab ?? false,
        };

        return View("~/Features/LandingPages/Widgets/SimpleCallToAction/SimpleCallToACtionWidget.cshtml", model);
    }

    private async Task<string?> GetWebPageUrl(ContentItemReference? webPage)
    {
        if (webPage is not null)
        {
            var page = await generalContentItemRetrieverService.RetrieveWebPageByContentItemGuid(webPage.Identifier);
            return page?.GetUrl()?.RelativePath ?? string.Empty;
        }
        return string.Empty;
    }
}