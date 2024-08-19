using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(
        identifier: CallToActionWidgetViewComponent.IDENTIFIER,
        viewComponentType: typeof(CallToActionWidgetViewComponent),
        name: CallToActionWidgetViewComponent.NAME,
        propertiesType: typeof(CallToActionWidgetProperties),
        Description = "Displays a call to action button. As seen in KBank demo site.",
        IconClass = "icon-bubbles")]

namespace TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;

public class CallToActionWidgetViewComponent : ViewComponent
{
    private readonly IContentItemRetrieverService<Asset> contentItemRetrieverService;

    public const string IDENTIFIER = "TrainingGuides.CallToActionWidget";

    public const string NAME = "Call to action (CTA)";

    public CallToActionWidgetViewComponent(
        IContentItemRetrieverService<Asset> contentItemRetrieverService)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(CallToActionWidgetProperties properties)
    {
        string targetUrl = string.Empty;

        if (properties.Type.Equals("page", StringComparison.InvariantCultureIgnoreCase))
        {
            targetUrl = properties.TargetPage;
        }

        if (properties.Type.Equals("absolute", StringComparison.InvariantCultureIgnoreCase))
        {
            targetUrl = properties.AbsoluteUrl;
        }

        if (properties.Type.Equals("content", StringComparison.InvariantCultureIgnoreCase))
        {
            var selectedId = properties?.ContentItem?.Select(i => i.Identifier).ToList().FirstOrDefault();
            var selectedItem = selectedId != null
                ? await contentItemRetrieverService.RetrieveContentItemByGuid(
                    selectedId.Value,
                    Asset.CONTENT_TYPE_NAME)
                : null;

            if (selectedItem != null)
            {
                targetUrl = selectedItem.AssetFile?.Url ?? string.Empty;
            }
        }

        var model = new CallToActionWidgetViewModel()
        {
            Identifier = properties.Identifier,
            Text = properties.Text,
            Url = targetUrl,
            OpenInNewTab = properties.OpenInNewTab,
            IsDownload = properties.Type.Equals("content", StringComparison.InvariantCultureIgnoreCase) || properties.IsDownload,
        };

        return View("~/Features/LandingPages/Widgets/CallToAction/CallToACtionWidget.cshtml", model);
    }
}
