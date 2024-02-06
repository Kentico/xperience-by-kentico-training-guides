using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(
        identifier: CallToActionWidgetViewComponent.IDENTIFIER,
        viewComponentType: typeof(CallToActionWidgetViewComponent),
        name: "Call to action (CTA)",
        propertiesType: typeof(CallToActionWidgetProperties),
        Description = "Displays a call to action button.",
        IconClass = "icon-bubbles")]

namespace TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;

public class CallToActionWidgetViewComponent : ViewComponent
{
    private readonly IContentItemRetrieverService<Asset> contentItemRetrieverService;
    private readonly IContentQueryResultMapper contentQueryResultMapper;

    public const string IDENTIFIER = "TrainingGuides.CallToActionWidget";

    public CallToActionWidgetViewComponent(
        IContentItemRetrieverService<Asset> contentItemRetrieverService,
        IContentQueryResultMapper contentQueryResultMapper)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.contentQueryResultMapper = contentQueryResultMapper;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(CallToActionWidgetProperties properties)
    {
        string? targetUrl = string.Empty;

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
                    Asset.CONTENT_TYPE_NAME,
                    container => contentQueryResultMapper.Map<Asset>(container))
                : null;

            if (selectedItem != null)
            {
                targetUrl = selectedItem.AssetFile?.Url;
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