using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Gallery.Widgets.GalleryWidget;

public class GalleryImageViewModel
{
    public string Description { get; set; } = string.Empty;
    public AssetViewModel? Image { get; set; } = null;
}