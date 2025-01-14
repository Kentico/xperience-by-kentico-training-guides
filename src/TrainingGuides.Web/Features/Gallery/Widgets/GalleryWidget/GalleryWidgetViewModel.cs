using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Gallery.Widgets.GalleryWidget;

public class GalleryWidgetViewModel : WidgetViewModel
{
    public string Title { get; set; } = string.Empty;
    public List<AssetViewModel> Images { get; set; } = [];
    public override bool IsMisconfigured => Images == null || Images.Count == 0;
}