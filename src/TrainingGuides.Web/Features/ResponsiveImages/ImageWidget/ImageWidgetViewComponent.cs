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
/// Retrieves the selected image asset and passes it to the widget view with the chosen variant.
/// </summary>
/// <param name="contentItemRetrieverService">
/// Service used to retrieve the selected Asset content item.
/// </param>
public class ImageWidgetViewComponent(
    IContentItemRetrieverService contentItemRetrieverService) : ViewComponent
{
    /// <summary>
    /// Unique identifier for the "Image" widget.
    /// </summary>
    public const string IDENTIFIER = "TrainingGuides.ImageWidget";

    /// <summary>
    /// Loads the selected Asset content item and builds the widget view model.
    /// </summary>
    /// <param name="properties">The configured widget properties.</param>
    /// <returns>
    /// The widget view; the model is marked misconfigured when no valid image is selected.
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
