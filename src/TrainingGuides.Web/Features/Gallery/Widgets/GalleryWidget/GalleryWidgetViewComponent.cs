using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Kentico.PageBuilder.Web.Mvc;
using TrainingGuides.Web.Features.Shared.OptionProviders.OrderBy;
using TrainingGuides.Web.Features.Shared.Services;
using TrainingGuides.Web.Features.Gallery.Widgets.GalleryWidget;

[assembly:
    RegisterWidget(GalleryWidgetViewComponent.IDENTIFIER, typeof(GalleryWidgetViewComponent), "Gallery widget",
        typeof(GalleryWidgetProperties), Description = "Displays gallery of images from a smart folder", IconClass = "icon-pictures")]

namespace TrainingGuides.Web.Features.Gallery.Widgets.GalleryWidget;

public class GalleryWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.GalleryWidget";

    private readonly IContentItemRetrieverService<GalleryImage> galleryImageRetriever;

    public GalleryWidgetViewComponent(IContentItemRetrieverService<GalleryImage> galleryImageRetriever)
    {
        this.galleryImageRetriever = galleryImageRetriever;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(GalleryWidgetProperties properties)
    {
        var model = new GalleryWidgetViewModel();
        var smartFolderGuid = properties.SmartFolder?.Identifier ?? Guid.Empty;
        var orderBy = properties.OrderBy.Equals(OrderByOption.OldestFirst.ToString())
            ? OrderByOption.OldestFirst
            : OrderByOption.NewestFirst;

        if (!smartFolderGuid.Equals(Guid.Empty))
        {
            var galleryImages = await RetrieveGalleryImages(smartFolderGuid, orderBy);

            model.Images = galleryImages.ToList();
        }

        return View("~/Features/Gallery/Widgets/GalleryWidget/GalleryWidget.cshtml", model);
    }

    private async Task<IEnumerable<GalleryImage>> RetrieveGalleryImages(Guid smartFolderGuid, OrderByOption orderBy) =>
        await galleryImageRetriever.RetrieveReusableContentItemsFromSmartFolder(
            GalleryImage.CONTENT_TYPE_NAME,
            smartFolderGuid,
            orderBy);
}
