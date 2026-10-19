using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.ResponsiveImages.ImageWidget;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWidget(
    identifier: ImageWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(ImageWidgetViewComponent),
    name: "Image",
    propertiesType: typeof(ImageWidgetProperties),
    Description = "Displays an image as a responsive image or in a picked image variant.",
    IconClass = "icon-picture")]

namespace TrainingGuides.Web.Features.ResponsiveImages.ImageWidget;

/// <summary>
/// Shows the picked image asset, responsive or in the picked image variant.
/// </summary>
/// <param name="contentItemRetrieverService">
/// Retrieves the picked Asset content item.
/// </param>
public class ImageWidgetViewComponent(
    IContentItemRetrieverService contentItemRetrieverService) : ViewComponent
{
    /// <summary>
    /// Identifier of the widget.
    /// </summary>
    public const string IDENTIFIER = "TrainingGuides.ImageWidget";

    /// <summary>
    /// Retrieves the picked Asset content item and maps its file, its alternative
    /// text and the picked image variant.
    /// </summary>
    /// <param name="properties">The widget properties.</param>
    /// <returns>
    /// The widget view; its model is misconfigured when there is no image to show.
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(ImageWidgetProperties properties)
    {
        var assetGuid = properties.Image.Select(item => item.Identifier).FirstOrDefault();
        var asset = assetGuid == Guid.Empty
            ? null
            : await contentItemRetrieverService.RetrieveContentItemByGuid<Asset>(assetGuid);

        var model = new ImageWidgetViewModel
        {
            Image = ImageViewModel.GetViewModel(asset),
            ImageVariant = properties.ImageVariant
        };

        return View("~/Features/ResponsiveImages/ImageWidget/ImageWidget.cshtml", model);
    }
}
