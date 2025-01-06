using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Gallery.Widgets.GalleryWidget;

public class GalleryWidgetViewModel : WidgetViewModel
{
    public List<GalleryImage> Images { get; set; } = [];

    public override bool IsMisconfigured => Images == null || Images.Count == 0;
}